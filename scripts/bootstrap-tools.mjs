import { createHash } from "node:crypto";
import { spawnSync } from "node:child_process";
import { existsSync, mkdirSync, readFileSync, readdirSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { fileHash, installedTool } from "./tool-packages.mjs";
import { packSupport } from "./support-package.mjs";
import { packCompilerSupport } from "./compiler-support-package.mjs";

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const json = (file) => JSON.parse(readFileSync(file, "utf8"));
const writeJson = (file, value) => { mkdirSync(path.dirname(file), { recursive: true }); writeFileSync(file, JSON.stringify(value, null, 2) + "\n"); };
const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
const definitions = {
  xantham: { packageId: "xantham", command: "xantham", source: "../Xantham", project: "src/Xantham.Cli/Xantham.Cli.fsproj", baseVersion: "0.1.0" },
  hawaii: { packageId: "Hawaii.Unofficial", command: "hawaii-unofficial", source: "../Hawaii", project: "src/Hawaii.fsproj", baseVersion: "1.0.0" },
};

function run(dotnet, args, cwd, log) {
  const result = spawnSync(dotnet, args, { cwd, encoding: "utf8", maxBuffer: 32 * 1024 * 1024 });
  if (log) writeFileSync(log, (result.stdout ?? "") + (result.stderr ?? ""));
  requireThat(!result.error && result.status === 0, `dotnet ${args[0]} failed; see ${log ?? result.stderr}`);
  return result.stdout.trim();
}

/** Hash package/build inputs, excluding generated build output and retired source. */
export function sourceFingerprint(directory, sdk) {
  const inputs = {};
  function collect(current, prefix) {
    for (const entry of readdirSync(current, { withFileTypes: true }).sort((a, b) => a.name.localeCompare(b.name, "en"))) {
      if (["bin", "obj", "node_modules", ".archive"].includes(entry.name)) continue;
      const relative = prefix + entry.name;
      requireThat(!entry.isSymbolicLink(), `Source package input is a symlink: ${relative}`);
      if (entry.isDirectory()) collect(path.join(current, entry.name), relative + "/");
      else inputs[relative] = fileHash(path.join(current, entry.name));
    }
  }
  collect(path.join(directory, "src"), "src/");
  for (const file of readdirSync(directory).sort()) {
    if (/^(?:Directory\..*\.(?:props|targets)|global\.json|NuGet\.Config|nuget\.config|README\.md)$/.test(file)) {
      inputs[file] = fileHash(path.join(directory, file));
    }
  }
  const recipeSha256 = fileHash(fileURLToPath(import.meta.url));
  const digest = createHash("sha256").update(JSON.stringify({ sdk, inputs, recipeSha256 })).digest("hex");
  return { digest, sdk, inputs, recipeSha256 };
}

function authenticateArchive(archive, digest) {
  const receipt = `${archive}.provenance.json`;
  requireThat(existsSync(receipt), `Local package has no build receipt: ${archive}`);
  const previous = json(receipt);
  requireThat(previous.sourceDigest === digest && previous.packageSha256 === fileHash(archive),
    `Local package no longer matches its source/build receipt: ${archive}`);
}

function recordArchive(archive, digest) {
  writeJson(`${archive}.provenance.json`, { sourceDigest: digest, packageSha256: fileHash(archive) });
}

export async function bootstrap(args = process.argv.slice(2)) {
  let only;
  const sources = {};
  for (let index = 0; index < args.length; index++) {
    const option = args[index];
    if (option === "--only") only = args[++index];
    else if (option === "--xantham-source") sources.xantham = args[++index];
    else if (option === "--hawaii-source") sources.hawaii = args[++index];
    else throw new Error(`Unknown option ${option}; use --only xantham|hawaii and optional --xantham-source/--hawaii-source paths`);
  }
  requireThat(!only || Object.hasOwn(definitions, only), "--only must name xantham or hawaii");
  const dotnet = process.env.CLOUDEDGE_DOTNET ?? "dotnet";
  const sdk = run(dotnet, ["--version"], root);
  const feed = path.join(root, "artifacts/tool-feed");
  const logs = path.join(root, "artifacts/tools/bootstrap");
  mkdirSync(feed, { recursive: true });
  mkdirSync(logs, { recursive: true });
  const manifestFile = path.join(root, ".config/dotnet-tools.json");
  const lockFile = path.join(root, "config/tool-packages.json");
  const lock = existsSync(lockFile) ? json(lockFile) : { schemaVersion: 1, tools: {}, libraries: {} };
  for (const name of only ? [only] : Object.keys(definitions)) {
    const definition = definitions[name];
    const source = path.resolve(root, sources[name] ?? definition.source);
    requireThat(existsSync(path.join(source, definition.project)), `Missing ${name} source checkout: ${source}`);
    const before = sourceFingerprint(source, sdk);
    const version = `${definition.baseVersion}-local.${before.digest.slice(0, 20)}`;
    const archive = path.join(feed, `${definition.packageId}.${version}.nupkg`);
    if (existsSync(archive)) authenticateArchive(archive, before.digest);
    else {
      const buildArtifacts = path.join(logs, `${name}-build-${before.digest.slice(0, 20)}`);
      run(dotnet, ["pack", definition.project, "--configuration", "Release", "--output", feed,
        "--artifacts-path", buildArtifacts, `-p:PackageVersion=${version}`, "--nologo"], source,
        path.join(logs, `${name}-pack.log`));
      requireThat(sourceFingerprint(source, sdk).digest === before.digest, `${name}: source changed while packing; package was not accepted`);
      requireThat(existsSync(archive), `${name}: pack did not produce the expected package`);
      recordArchive(archive, before.digest);
    }
    requireThat(sourceFingerprint(source, sdk).digest === before.digest, `${name}: source changed while packing; package was not accepted`);
    requireThat(existsSync(archive), `${name}: pack did not produce the expected package`);
    // Upstream renamed Xantham.Cli to xantham; keep exactly one command owner.
    if (name === "xantham" && json(manifestFile).tools["xantham.cli"]) {
      run(dotnet, ["tool", "uninstall", "--local", "Xantham.Cli", "--tool-manifest", manifestFile], root,
        path.join(logs, "xantham-migrate.log"));
    }
    const current = json(manifestFile).tools[definition.packageId.toLowerCase()];
    run(dotnet, ["tool", current ? "update" : "install", "--local", definition.packageId,
      "--tool-manifest", manifestFile, "--version", version, "--source", feed, "--allow-downgrade"], root,
      path.join(logs, `${name}-install.log`));
    const installed = installedTool(root, name, process.env, { ...definition, version });
    requireThat(installed.identity.packageSha256 === fileHash(archive), `${name}: installed cache differs from the local package`);
    run(dotnet, [...installed.args, name === "xantham" ? "schema" : "--version"], root,
      path.join(logs, `${name}-smoke.log`));
    lock.tools[name] = installed.identity;
    const git = spawnSync("git", ["rev-parse", "HEAD"], { cwd: source, encoding: "utf8" });
    writeJson(path.join(logs, `${name}-source.json`), { sourceDirectory: source, gitHead: git.status === 0 ? git.stdout.trim() : null,
      project: definition.project, ...before, version, packageSha256: installed.identity.packageSha256 });
    if (name === "xantham") {
      const supportInputs = Object.fromEntries([
        path.join(source, "src/Xantham.Fable.Core/Library.fs"), path.join(source, "src/Xantham.Fable.Core/Helpers.fs"),
        path.join(source, "src/Xantham.Fable.Core/README.md"),
        path.join(root, "scripts/support-package.mjs"), path.join(root, "scripts/templates/Xantham.Fable.Core.fsproj"),
      ].map((file, index) => [index, fileHash(file)]));
      const supportDigest = createHash("sha256").update(JSON.stringify({ sdk, supportInputs })).digest("hex");
      const supportVersion = `0.1.0-local.${supportDigest.slice(0, 20)}`;
      let supportArchive = path.join(feed, `Xantham.Fable.Core.${supportVersion}.nupkg`);
      if (existsSync(supportArchive)) authenticateArchive(supportArchive, supportDigest);
      else {
        supportArchive = await packSupport({ root, sourceDirectory: path.join(source, "src/Xantham.Fable.Core"), version: supportVersion, feed, dotnet });
        recordArchive(supportArchive, supportDigest);
      }
      requireThat(sourceFingerprint(source, sdk).digest === before.digest, "Xantham support source changed while packing");
      lock.libraries ??= {};
      lock.libraries["Xantham.Fable.Core"] = { version: supportVersion, packageSha256: fileHash(supportArchive) };
      const configFile = path.join(root, "config/targets.json");
      const config = json(configFile);
      config.projectDefaults.packages["Xantham.Fable.Core"] = supportVersion;
      const compilerInputs = Object.fromEntries(Object.entries(before.inputs).filter(([file]) => file.startsWith("src/Xantham.Fable.Core.TS/")));
      const compilerDigest = createHash("sha256").update(JSON.stringify({ sdk, compilerInputs, supportVersion,
        recipe: fileHash(path.join(root, "scripts/compiler-support-package.mjs")) })).digest("hex");
      const compilerVersion = `0.1.0-local.${compilerDigest.slice(0, 20)}`;
      const compilerArchive = path.join(feed, `Xantham.Fable.Core.TS.${compilerVersion}.nupkg`);
      if (existsSync(compilerArchive)) authenticateArchive(compilerArchive, compilerDigest);
      else {
        packCompilerSupport({ root, source, version: compilerVersion, supportVersion, dotnet, feed });
        requireThat(sourceFingerprint(source, sdk).digest === before.digest, "Compiler support source changed while packing");
        recordArchive(compilerArchive, compilerDigest);
      }
      lock.libraries["Xantham.Fable.Core.TS"] = { version: compilerVersion, packageSha256: fileHash(compilerArchive) };
      config.projectDefaults.packages["Xantham.Fable.Core.TS"] = compilerVersion;
      // Standard-library ownership now comes from the same producer as Xantham's compile gate.
      for (const id of Object.keys(config.projectDefaults.packages)) {
        if (id.startsWith("Fable.Browser.")) delete config.projectDefaults.packages[id];
      }
      writeJson(configFile, config);
    }
    writeJson(lockFile, lock);
    console.log(`${definition.command}: installed ${definition.packageId} ${version}`);
  }
  console.log("Local tools are pinned in .config/dotnet-tools.json; package hashes are in config/tool-packages.json.");
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  bootstrap().catch((error) => { console.error(error.message); process.exitCode = 1; });
}
