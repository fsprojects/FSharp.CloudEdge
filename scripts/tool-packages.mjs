import { createHash } from "node:crypto";
import { spawnSync } from "node:child_process";
import { existsSync, readFileSync, readdirSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { unzipSync } from "fflate";

const readJson = (file) => JSON.parse(readFileSync(file, "utf8"));
const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
export const fileHash = (file, algorithm = "sha256", encoding = "hex") => createHash(algorithm).update(readFileSync(file)).digest(encoding);
const canonical = (value) => JSON.stringify(value, (_, item) => item && typeof item === "object" && !Array.isArray(item)
  ? Object.fromEntries(Object.entries(item).sort(([a], [b]) => a.localeCompare(b, "en"))) : item);

export function packageCache(environment = process.env, root = process.cwd()) {
  if (environment.NUGET_PACKAGES) return path.resolve(root, environment.NUGET_PACKAGES);
  const result = spawnSync(environment.CLOUDEDGE_DOTNET ?? "dotnet", ["nuget", "locals", "global-packages", "--list"],
    { cwd: root, encoding: "utf8", env: { ...environment, DOTNET_CLI_UI_LANGUAGE: "en-US" } });
  requireThat(!result.error && result.status === 0, "Cannot locate the NuGet package cache");
  const match = result.stdout.trim().match(/^global-packages:\s*(.+)$/m);
  requireThat(match, "dotnet did not report its global package cache");
  return path.resolve(match[1]);
}

function files(directory, prefix = "") {
  return readdirSync(directory, { withFileTypes: true }).flatMap((entry) => {
    requireThat(!entry.isSymbolicLink(), `Unexpected symlink in installed tool: ${entry.name}`);
    const relative = prefix + entry.name;
    return entry.isDirectory() ? files(path.join(directory, entry.name), relative + "/") : [relative];
  }).sort();
}

function restoredCommand(root, name, definition, environment) {
  // The SDK keys its local-tool cache by the SDK's own framework, not the
  // framework of the selected tool payload. Read the selected SDK's config
  // rather than inferring that framework from a tool's directory or version.
  const result = spawnSync(environment.CLOUDEDGE_DOTNET ?? "dotnet", ["--info"],
    { cwd: root, encoding: "utf8", env: { ...environment, DOTNET_CLI_UI_LANGUAGE: "en-US" } });
  requireThat(!result.error && result.status === 0, `${name}: cannot identify the SDK used by the tool resolver`);
  const sdk = result.stdout.match(/^\s*Base Path:\s*(.+)$/m)?.[1].trim();
  requireThat(sdk && path.isAbsolute(sdk), `${name}: tool resolver SDK base path is missing`);
  const sdkConfig = path.join(sdk, "dotnet.runtimeconfig.json");
  requireThat(existsSync(sdkConfig), `${name}: tool resolver SDK runtime configuration is missing`);
  const targetFramework = readJson(sdkConfig).runtimeOptions?.tfm;
  requireThat(typeof targetFramework === "string" && /^net\d+\.\d+$/.test(targetFramework), `${name}: unsupported tool resolver SDK framework`);
  const home = environment.DOTNET_CLI_HOME || environment[process.platform === "win32" ? "USERPROFILE" : "HOME"] || os.homedir();
  const cacheFile = path.resolve(root, home, ".dotnet/toolResolverCache/1", definition.packageId.toLowerCase());
  requireThat(existsSync(cacheFile), `${name}: local tool resolver cache is missing; run dotnet tool restore`);
  let rows;
  try { rows = readJson(cacheFile); }
  catch { throw new Error(`${name}: local tool resolver cache is invalid; run dotnet tool restore`); }
  requireThat(Array.isArray(rows), `${name}: local tool resolver cache must be an array`);
  const matching = rows.filter((row) => row && row.Version?.toLowerCase() === definition.version.toLowerCase()
    && row.TargetFramework === targetFramework && row.RuntimeIdentifier?.toLowerCase() === "any" && row.Name === definition.command);
  requireThat(matching.length === 1, `${name}: expected one local tool resolver mapping for ${definition.version}/${targetFramework}; run dotnet tool restore`);
  const row = matching[0];
  requireThat(row.Runner === "dotnet", `${name}: unexpected local tool resolver runner`);
  requireThat(typeof row.PathToExecutable === "string" && path.isAbsolute(row.PathToExecutable), `${name}: invalid local tool resolver executable path`);
  return { cacheFile, targetFramework, executable: path.normalize(row.PathToExecutable), runner: row.Runner, sdkRuntimeConfigSha256: fileHash(sdkConfig) };
}

// Cache only the archive's expected hashes; re-read and hash installed files on
// every call. A reviewed lock must never bless a corrupt extraction on restore.
const archivePayloads = new Map();
function verifyArchivePayload(name, packageBytes, packageSha256, directory, payload, payloadSha256) {
  const prefix = path.relative(directory, payload).split(path.sep).join("/") + "/";
  const key = packageSha256 + ":" + prefix;
  let expected = archivePayloads.get(key);
  if (!expected) {
    const seen = new Set();
    let unpacked;
    try {
      unpacked = unzipSync(packageBytes, { filter: ({ name: entry }) => {
        requireThat(!seen.has(entry), `${name}: duplicate entry in package archive: ${entry}`);
        seen.add(entry);
        requireThat(!entry.includes("\\") && !entry.startsWith("/") && !entry.split("/").some((part) => part === "." || part === ".."),
          `${name}: invalid path in package archive: ${entry}`);
        return entry.startsWith(prefix) && !entry.endsWith("/");
      } });
    } catch (error) { throw new Error(`${name}: cannot verify package ZIP archive: ${error.message}`); }
    expected = Object.fromEntries(Object.entries(unpacked).map(([entry, bytes]) =>
      [entry.slice(prefix.length), createHash("sha256").update(bytes).digest("hex")]));
    requireThat(Object.keys(expected).length > 0, `${name}: package archive has no selected tool payload`);
    archivePayloads.set(key, expected);
  }
  requireThat(canonical(expected) === canonical(payloadSha256), `${name}: installed tool payload differs from its package archive`);
}

export function installedTool(root, name, environment = process.env, expected) {
  const manifestFile = path.join(root, ".config/dotnet-tools.json");
  const manifest = readJson(manifestFile);
  const definition = expected ?? readJson(path.join(root, "config/tool-packages.json")).tools[name];
  requireThat(definition && typeof definition.packageId === "string" && typeof definition.command === "string", `Unknown generator tool: ${name}`);
  requireThat(/^[A-Za-z0-9][A-Za-z0-9_.-]*$/.test(definition.packageId), `${name}: invalid tool package ID`);
  const entry = manifest.tools[definition.packageId.toLowerCase()];
  requireThat(entry && entry.commands?.includes(definition.command), `Install ${definition.packageId} with npm run tools:bootstrap`);
  requireThat(Object.values(manifest.tools).filter((tool) => tool.commands?.includes(definition.command)).length === 1,
    `${name}: ambiguous tool command in the local manifest`);
  requireThat(typeof entry.version === "string" && /^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?$/.test(entry.version), `${name}: tool version must be an exact NuGet version`);
  requireThat(!definition.version || definition.version === entry.version, `${name}: tool manifest differs from the package lock`);
  const resolver = restoredCommand(root, name, { ...definition, version: entry.version }, environment);
  const directory = path.join(packageCache(environment, root), definition.packageId.toLowerCase(), entry.version.toLowerCase());
  const archive = path.join(directory, `${definition.packageId.toLowerCase()}.${entry.version.toLowerCase()}.nupkg`);
  requireThat(existsSync(archive), `${name}: restore the pinned local tool package first`);
  const packageBytes = readFileSync(archive);
  const packageSha256 = createHash("sha256").update(packageBytes).digest("hex");
  if (definition.packageSha256) requireThat(definition.packageSha256 === packageSha256, `${name}: installed package differs from the reviewed package digest`);
  const payloadFiles = files(path.join(directory, "tools"));
  const settings = payloadFiles.filter((file) => file.endsWith("/DotnetToolSettings.xml"));
  const candidates = settings.flatMap((file) => {
    const text = readFileSync(path.join(directory, "tools", file), "utf8");
    const commands = [...text.matchAll(/<Command\s+([^>]+)\/?\s*>/g)];
    return commands.flatMap(([, attributes]) => {
      const values = Object.fromEntries([...attributes.matchAll(/([A-Za-z]+)="([^"]*)"/g)].map(([, key, value]) => [key, value]));
      return values.Name === definition.command && values.Runner === "dotnet" ? [{ file, entryPoint: values.EntryPoint }] : [];
    });
  });
  const resolvedCandidates = candidates.filter((candidate) => candidate.entryPoint
    && path.basename(candidate.entryPoint) === candidate.entryPoint
    && path.join(directory, "tools", path.dirname(candidate.file), candidate.entryPoint) === resolver.executable);
  requireThat(resolvedCandidates.length === 1, `${name}: local tool resolver executable path does not match the installed package payload`);
  const candidate = resolvedCandidates[0];
  const payload = path.dirname(path.join(directory, "tools", candidate.file));
  requireThat(candidate.entryPoint && path.basename(candidate.entryPoint) === candidate.entryPoint, `${name}: invalid tool entry point`);
  const assembly = path.join(payload, candidate.entryPoint);
  requireThat(existsSync(assembly), `${name}: installed entry point is missing`);
  const payloadSha256 = Object.fromEntries(files(payload).map((file) => [file, fileHash(path.join(payload, file))]));
  verifyArchivePayload(name, packageBytes, packageSha256, directory, payload, payloadSha256);
  const identity = { packageId: definition.packageId, command: definition.command, version: entry.version, packageSha256, payloadSha256 };
  if (definition.packageSha256) {
    requireThat(canonical(definition.payloadSha256) === canonical(payloadSha256), `${name}: installed tool payload differs from the reviewed package`);
  }
  return { identity, assembly, resolver, manifestSha256: fileHash(manifestFile), args: ["tool", "run", definition.command, "--"] };
}

export function resolveTool(root, name, environment = process.env) {
  const lockFile = path.join(root, "config/tool-packages.json");
  requireThat(existsSync(lockFile), "Bootstrap generator tools first: npm run tools:bootstrap");
  const lock = readJson(lockFile);
  requireThat(lock.schemaVersion === 1 && lock.tools?.[name]?.packageSha256 && lock.tools[name].payloadSha256,
    `${name}: no reviewed tool package is locked; run npm run tools:bootstrap`);
  const installed = installedTool(root, name, environment, lock.tools[name]);
  return { ...installed, fingerprint: { ...installed.identity, manifestSha256: installed.manifestSha256, resolver: installed.resolver } };
}

export function sameTool(left, right) { return canonical(left.fingerprint) === canonical(right.fingerprint); }

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  try {
    const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
    const name = process.argv[2];
    const tool = resolveTool(root, name);
    console.log(JSON.stringify(tool));
  } catch (error) { console.error(error.message); process.exitCode = 1; }
}

/** Select the single source adapter owner, with legacy per-projection pins accepted when equal. */
export function sourceToolConfiguration(configuration, target) {
  const shared = configuration.xantham?.source;
  const projection = target.projection;
  const local = projection && (projection.project !== undefined || projection.xanthamRevision !== undefined)
    ? { project: projection.project, xanthamRevision: projection.xanthamRevision } : undefined;
  for (const [label, value] of [["xantham.source", shared], ["projection source", local]]) {
    if (value === undefined) continue;
    requireThat(value && typeof value === "object" && !Array.isArray(value)
      && Object.keys(value).every(key => ["project", "xanthamRevision"].includes(key))
      && typeof value.project === "string" && value.project.length > 0
      && typeof value.xanthamRevision === "string" && /^[0-9a-f]{40}$/.test(value.xanthamRevision),
    `${label} must name a project and exact Xantham source revision`);
  }
  requireThat(!shared || !local || canonical(shared) === canonical(local), `${target.id}: projection source differs from shared xantham.source`);
  requireThat(!projection || shared || local, `${target.id}: projection requires a source adapter`);
  return shared ?? local;
}

/** Build and authenticate the source adapter and its complete runtime payload. */
export function resolveSourceTool(root, sourceConfiguration, environment = process.env, build = false) {
  const project = path.resolve(root, sourceConfiguration.project);
  requireThat(project.startsWith(path.resolve(root, "generators") + path.sep) && project.endsWith(".fsproj") && existsSync(project), "Projection project must exist under generators/");
  const xanthamRoot = path.resolve(environment.CLOUDEDGE_XANTHAM_ROOT ?? path.join(root, "../Xantham"));
  requireThat(typeof sourceConfiguration.xanthamRevision === "string" && /^[0-9a-f]{40}$/.test(sourceConfiguration.xanthamRevision), "Projection must pin an exact Xantham source revision");
  const git = args => {
    const result = spawnSync("git", ["-C", xanthamRoot, ...args], { encoding: "utf8" });
    requireThat(!result.error && result.status === 0, "Projection requires the primary Xantham source checkout");
    return result.stdout;
  };
  const inputs = () => {
    const revision = git(["rev-parse", "HEAD"]).trim();
    requireThat(revision === sourceConfiguration.xanthamRevision, `Projection requires Xantham ${sourceConfiguration.xanthamRevision}; current checkout is ${revision}`);
    const scope = ["src/Xantham.Generator", "src/Xantham.Generator.Myriad", "src/Xantham.TypeScript.Wire", "src/Xantham.Fable.Core", "src/Xantham.Fable.Core.TS", "Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props", "global.json", "NuGet.Config", "nuget.config"];
    const diff = git(["diff", "--binary", "HEAD", "--", ...scope]);
    const untracked = git(["ls-files", "--others", "--exclude-standard", "-z", "--", ...scope]).split("\0").filter(Boolean);
    const dirty = diff.length > 0 || untracked.length > 0;
    requireThat(!dirty || environment.CLOUDEDGE_ALLOW_DIRTY_XANTHAM === "1", "Projection requires clean pinned Xantham sources; explicit development override CLOUDEDGE_ALLOW_DIRTY_XANTHAM=1 records the source diff");
    const source = { root: xanthamRoot, revision, dirty, diffSha256: createHash("sha256").update(diff).digest("hex"), untrackedSha256: Object.fromEntries(untracked.map(file => [file, fileHash(path.join(xanthamRoot, file))])) };
    const buildInputs = [project, path.join(path.dirname(project), "Program.fs")];
    for (let directory = path.dirname(project); ; directory = path.dirname(directory)) {
      for (const name of ["Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props", "NuGet.Config", "nuget.config", "global.json"])
        if (existsSync(path.join(directory, name))) buildInputs.push(path.join(directory, name));
      if (path.dirname(directory) === directory) break;
    }
    return { source, configuration: "Release", framework: "net10.0", buildInputSha256: Object.fromEntries([...new Set(buildInputs)].sort().map(file => [path.relative(root, file), fileHash(file)])) };
  };
  const before = inputs();
  if (build) {
    const result = spawnSync(environment.CLOUDEDGE_DOTNET ?? "dotnet", ["build", project, "-c", "Release", `-p:XanthamProjectRoot=${xanthamRoot}`], { cwd: root, env: environment, stdio: "inherit" });
    requireThat(!result.error && result.status === 0, "Projection adapter build failed");
    requireThat(canonical(inputs()) === canonical(before), "Projection source/build inputs changed during compilation; output was not accepted");
  }
  const directory = path.join(path.dirname(project), "bin/Release/net10.0");
  const executable = path.join(directory, path.basename(project, ".fsproj") + ".dll");
  requireThat(existsSync(executable), "Projection adapter is missing; build the configured project");
  const receiptName = "cloudedge-build.json";
  const payloadSha256 = Object.fromEntries(files(directory).filter(file => file !== receiptName).map(file => [file, fileHash(path.join(directory, file))]));
  const association = { schemaVersion: 1, inputs: before, payloadSha256 };
  const receiptFile = path.join(directory, receiptName);
  if (build) writeFileSync(receiptFile, JSON.stringify(association, null, 2) + "\n");
  requireThat(existsSync(receiptFile) && canonical(readJson(receiptFile)) === canonical(association),
    "Projection adapter has no matching source-to-payload build receipt; rebuild the configured project");
  return { args: [executable], fingerprint: { kind: "source-adapter", project: sourceConfiguration.project, ...before, payloadSha256, buildReceiptSha256: fileHash(receiptFile) } };
}

export const resolveProjectionTool = resolveSourceTool;
