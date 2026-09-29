import { createHash } from "node:crypto";
import { spawnSync } from "node:child_process";
import { closeSync, copyFileSync, existsSync, mkdirSync, mkdtempSync, openSync, readFileSync, readdirSync, renameSync, rmSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath, pathToFileURL } from "node:url";
import { inspectInputOverlays, prepareInputOverlays } from "./input-overlays.mjs";
import { catalogCandidates, declarationReferences, generationDependencies, generationOrder } from "./catalogs.mjs";
import { resolveTool, sameTool } from "./tool-packages.mjs";

const defaultRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const readJson = (file) => JSON.parse(readFileSync(file, "utf8"));
const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
const hash = (file) => createHash("sha256").update(readFileSync(file)).digest("hex");
const exactVersion = (value) => typeof value === "string" && /^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?$/.test(value);

function fields(value, keys, label, optional = []) {
  requireThat(value && typeof value === "object" && !Array.isArray(value), `${label} must be an object`);
  requireThat(Object.keys(value).every((key) => [...keys, ...optional].includes(key)), `${label} contains an unknown key`);
  requireThat(keys.every((key) => Object.hasOwn(value, key)), `${label} is missing a required key`);
}

function inside(base, relative) {
  requireThat(typeof relative === "string" && relative.length > 0, "Expected a nonempty relative path");
  const resolved = path.resolve(base, relative);
  requireThat(resolved.startsWith(base + path.sep), `Path must stay inside ${base}: ${relative}`);
  return resolved;
}

function run(command, args, options = {}) {
  const result = spawnSync(command, args, { stdio: "inherit", ...options });
  if (result.error) throw result.error;
  requireThat(result.status === 0, `${command} failed (${result.signal ?? result.status}); output was not accepted`);
}

function sourceFiles(directory, prefix = "") {
  return readdirSync(directory, { withFileTypes: true }).flatMap((entry) => {
    const name = prefix + entry.name;
    return entry.isDirectory() ? sourceFiles(path.join(directory, entry.name), name + "/") : name.endsWith(".fs") ? [name] : [];
  }).sort();
}

export async function generate({ root = defaultRoot, args = process.argv.slice(2), env: environment = process.env, toolResolver = resolveTool, configuration } = {}) {
  requireThat(args.filter((arg) => arg.startsWith("--")).every((arg) => ["--check", "--no-build", "--continue-on-error", "--resume"].includes(arg)), "Usage: node scripts/generate.mjs [--check] [--no-build] [--continue-on-error] [--resume] [target ...]");
  const config = configuration ?? readJson(path.join(root, "config/targets.json"));
  fields(config, ["schemaVersion", "toolchain", "xantham", "profiles", "projectDefaults", "targets"], "targets.json");
  fields(config.toolchain, ["typescript"], "toolchain");
  fields(config.xantham, ["tool"], "xantham");
  requireThat(config.xantham.tool === "xantham", "xantham must select the pinned local tool");
  requireThat(config.schemaVersion === 2 && exactVersion(config.toolchain.typescript), "Unsupported configuration schema or TypeScript pin");
  requireThat(Array.isArray(config.targets) && config.targets.length > 0, "At least one generation target is required");
  requireThat(Array.isArray(config.profiles), "profiles must be an array");
  const profiles = new Map();
  for (const profile of config.profiles) {
    fields(profile, ["id", "directory"], "profile");
    requireThat(typeof profile.id === "string" && /^[a-z][a-z0-9-]*$/.test(profile.id) && !profiles.has(profile.id), "Profile IDs must be unique lowercase names");
    profiles.set(profile.id, profile.directory === "." ? root : inside(root, profile.directory));
  }
  const ids = new Set();
  const destinations = new Set();
  for (const target of config.targets) {
    fields(target, ["id", "package", "version", "profile", "dependencies", "outputDirectory", "files", "generator"], "target", ["catalogCandidates"]);
    requireThat(typeof target.id === "string" && /^[a-z][a-z0-9-]*$/.test(target.id) && !ids.has(target.id), "Target IDs must be unique lowercase names");
    ids.add(target.id);
    requireThat(profiles.has(target.profile), `${target.id}: unknown profile ${target.profile}`);
    requireThat(Array.isArray(target.dependencies) && target.dependencies.every((id) => typeof id === "string"), `${target.id}: dependencies must be target IDs`);
    requireThat(typeof target.package === "string" && /^(?:@[a-z0-9._-]+\/)?[a-z0-9._-]+$/.test(target.package) && exactVersion(target.version), `${target.id}: invalid package or exact version`);
    requireThat(Array.isArray(target.files) && target.files.length > 0 && new Set(target.files).size === target.files.length && target.files.every((file) => typeof file === "string" && file.endsWith(".fs")), `${target.id}: files must list unique generated F# paths`);
    const output = inside(path.join(root, "src"), path.relative(path.join(root, "src"), inside(root, target.outputDirectory)));
    target.files.forEach((file) => {
      const destination = inside(output, file);
      requireThat(!destinations.has(destination), `Targets cannot share a generated destination: ${destination}`);
      destinations.add(destination);
    });
    requireThat(target.generator && typeof target.generator.module === "string" && /^[A-Za-z_][A-Za-z0-9_.]*$/.test(target.generator.module), `${target.id}: generator.module is required`);
  }
  const requested = args.filter((arg) => !arg.startsWith("--"));
  const selected = generationOrder(config.targets, requested);
  const pkg = readJson(path.join(root, "package.json"));
  const lock = readJson(path.join(root, "package-lock.json"));
  const validatePin = (directory, pkg, lock, name, version) => {
    requireThat(exactVersion(version), `${name}: expected an exact version`);
    const declared = { ...pkg.dependencies, ...pkg.devDependencies };
    const locked = { ...lock.packages?.[""]?.dependencies, ...lock.packages?.[""]?.devDependencies };
    requireThat(declared[name] === version && locked[name] === version, `${name}: package.json/package-lock.json must pin ${version} in ${directory}`);
    requireThat(lock.packages?.[`node_modules/${name}`]?.version === version, `${name}: lockfile resolution differs from ${version}`);
    const installed = readJson(path.join(directory, "node_modules", name, "package.json"));
    requireThat(installed.name === name && installed.version === version, `${name}: installed package differs from ${version}; install the profile`);
  };
  validatePin(root, pkg, lock, "typescript", config.toolchain.typescript);
  const profileLocks = new Map();
  const profileInputs = new Map();
  for (const profileId of new Set(selected.map((target) => target.profile))) {
    const directory = profiles.get(profileId);
    const profilePkg = readJson(path.join(directory, "package.json"));
    const profileLock = readJson(path.join(directory, "package-lock.json"));
    for (const [name, version] of Object.entries({ ...profilePkg.dependencies, ...profilePkg.devDependencies })) validatePin(directory, profilePkg, profileLock, name, version);
    for (const target of selected.filter((target) => target.profile === profileId)) validatePin(directory, profilePkg, profileLock, target.package, target.version);
    profileLocks.set(profileId, hash(path.join(directory, "package-lock.json")));
    profileInputs.set(profileId, inspectInputOverlays({ root, profileDirectory: directory }));
  }
  const tsDir = path.join(root, "node_modules/typescript");
  const { default: getExePath } = await import(pathToFileURL(path.join(tsDir, "lib/getExePath.js")).href);
  const tsc = getExePath();
  const platformPackage = readJson(path.resolve(path.dirname(tsc), "../package.json"));
  requireThat(platformPackage.version === config.toolchain.typescript, "TypeScript platform binary version differs from the compiler pin");
  if (args.includes("--check")) {
    console.log(`Checked ${profileLocks.size} dependency profiles and ${selected.length} selected target(s); no generation performed.`);
    return;
  }
  const dotnet = environment.CLOUDEDGE_DOTNET ?? "dotnet";
  const tool = toolResolver(root, config.xantham.tool, environment);
  const typescriptExecutableSha256 = hash(tsc);
  const env = { ...environment, XANTHAM_TSGO_EXE: tsc, XANTHAM_REQUIRE_TSC: "1" };
  const results = [];
  const preparedProfiles = new Map();
  for (const target of selected) {
    const failedDependencies = generationDependencies(target).filter((id) => results.some((result) => result.target === id && ["failed", "blocked"].includes(result.status)));
    if (failedDependencies.length) {
      results.push({ target: target.id, status: "blocked", dependencies: failedDependencies });
      continue;
    }
    const artifacts = path.join(root, "artifacts", target.id);
    mkdirSync(artifacts, { recursive: true });
    let stage;
    try {
      const sourceProfileDirectory = profiles.get(target.profile);
      const expectedInput = profileInputs.get(target.profile);
      if (!preparedProfiles.has(target.profile)) {
        const prepared = prepareInputOverlays({ root, profileDirectory: sourceProfileDirectory });
        requireThat(prepared.fingerprint === expectedInput.fingerprint, `${target.id}: dependency profile changed before generation`);
        preparedProfiles.set(target.profile, prepared);
      }
      const inputOverlays = preparedProfiles.get(target.profile);
      const originalPackageDirectory = path.join(sourceProfileDirectory, "node_modules", target.package);
      const packageDirectory = path.join(inputOverlays.profileDirectory, "node_modules", target.package);
      const references = declarationReferences(target, root, originalPackageDirectory);
      const referenceHashes = () => Object.fromEntries(references.map(({ key, file }) => [key, hash(file)]));
      const declarationReferenceSha256 = referenceHashes();
      const effectiveGenerator = references.length ? { ...target.generator, declarationReferences: references.map(({ file }) => file) } : target.generator;
      const catalogFile = path.join(artifacts, "declarations.json");
      const previousFile = path.join(artifacts, "generation.json");
      if (args.includes("--resume") && existsSync(previousFile)) {
        const previous = readJson(previousFile);
        if (previous.package === target.package && previous.version === target.version && previous.profile === target.profile
            && previous.profileLockSha256 === profileLocks.get(target.profile)
            && previous.profileManifestSha256 === expectedInput.profileManifestSha256
            && previous.inputOverlays?.fingerprint === inputOverlays.fingerprint
            && JSON.stringify(previous.generator) === JSON.stringify(target.generator)
            && JSON.stringify(previous.catalogCandidates) === JSON.stringify(catalogCandidates(target))
            && JSON.stringify(previous.generatorTool) === JSON.stringify(tool.fingerprint)
            && previous.typescriptExecutableSha256 === typescriptExecutableSha256
            && JSON.stringify(previous.declarationReferenceSha256 ?? {}) === JSON.stringify(declarationReferenceSha256)
            && (!target.generator.declarationCatalog || existsSync(catalogFile) && previous.declarationCatalogSha256 === hash(catalogFile))
            && JSON.stringify(Object.keys(previous.sourceSha256 ?? {}).sort()) === JSON.stringify([...target.files].sort())
            && target.files.every((file) => existsSync(inside(inside(root, target.outputDirectory), file)) && previous.sourceSha256?.[file] === hash(inside(inside(root, target.outputDirectory), file)))) {
          results.push({ target: target.id, status: "current" });
          console.log(`${target.id}: current input, generator and source hashes verified`);
          continue;
        }
      }
      stage = mkdtempSync(path.join(artifacts, "staging-"));
      const settings = path.join(stage, "xantham.json");
      writeFileSync(settings, JSON.stringify(effectiveGenerator, null, 2) + "\n");
      const log = openSync(path.join(artifacts, "generate.log"), "w");
      try {
        run(dotnet, [...tool.args, "generate", packageDirectory, "--config", settings, "--out", stage], { cwd: root, env, stdio: ["ignore", log, log] });
      } finally { closeSync(log); }
      requireThat(sameTool(toolResolver(root, config.xantham.tool, environment), tool) && hash(tsc) === typescriptExecutableSha256,
        `${target.id}: generator or compiler changed during generation; output was not accepted`);
      requireThat(prepareInputOverlays({ root, profileDirectory: sourceProfileDirectory }).fingerprint === inputOverlays.fingerprint
        && JSON.stringify(referenceHashes()) === JSON.stringify(declarationReferenceSha256),
        `${target.id}: dependency profile or declaration catalog changed during generation; output was not accepted`);
      requireThat(JSON.stringify(sourceFiles(stage)) === JSON.stringify([...target.files].sort()), `${target.id}: emitted F# files differ from the configured list; inspect ${stage}`);
      const manifest = readJson(path.join(stage, "manifest.json"));
      requireThat(manifest.package === target.package && manifest.module === target.generator.module, `${target.id}: manifest identifies unexpected input or module`);
      requireThat(existsSync(path.join(stage, "symbols.jsonl")), `${target.id}: symbols.jsonl is missing`);
      requireThat(existsSync(path.join(stage, "declarations.json")) === !!target.generator.declarationCatalog,
        `${target.id}: declaration catalog output differs from the requested configuration`);
      target.files.forEach((file) => requireThat(readFileSync(inside(stage, file)).length > 0, `${target.id}: empty generated source ${file}`));
      const declarationCatalogSha256 = target.generator.declarationCatalog ? hash(path.join(stage, "declarations.json")) : undefined;
      const provenance = { target: target.id, catalogCandidates: catalogCandidates(target), package: target.package, version: target.version, profile: target.profile, profileLockSha256: profileLocks.get(target.profile), profileManifestSha256: expectedInput.profileManifestSha256, inputOverlays, inputPackageDirectory: packageDirectory, typescript: config.toolchain.typescript, typescriptExecutable: tsc, typescriptExecutableSha256, generator: target.generator, effectiveGenerator, declarationReferenceSha256, declarationCatalogSha256, generatorTool: tool.fingerprint, generatedAtUtc: new Date().toISOString(), sourceSha256: Object.fromEntries(target.files.map((file) => [file, hash(inside(stage, file))])), counts: manifest.counts };
      for (const file of target.files) {
        const destination = inside(inside(root, target.outputDirectory), file);
        mkdirSync(path.dirname(destination), { recursive: true });
        copyFileSync(inside(stage, file), destination + ".tmp");
        renameSync(destination + ".tmp", destination);
      }
      for (const file of ["manifest.json", "symbols.jsonl", "xantham.json"]) copyFileSync(path.join(stage, file), path.join(artifacts, file));
      if (target.generator.declarationCatalog) copyFileSync(path.join(stage, "declarations.json"), catalogFile);
      else if (existsSync(catalogFile)) rmSync(catalogFile);
      writeFileSync(path.join(artifacts, "generation.json"), JSON.stringify(provenance, null, 2) + "\n");
      rmSync(stage, { recursive: true });
      console.log(`${target.id}: generated ${target.files.length} source file(s); findings are in artifacts/${target.id}/manifest.json. Compilation and runtime checks are separate gates.`);
      results.push({ target: target.id, status: "generated", counts: manifest.counts });
    } catch (error) {
      const message = `${target.id}: ${error.message}. Diagnostic output remains in ${stage ?? artifacts}`;
      results.push({ target: target.id, status: "failed", error: message });
      if (!args.includes("--continue-on-error")) throw new Error(message, { cause: error });
      console.error(message);
    }
  }
  writeFileSync(path.join(root, "artifacts", "generation-run.json"), JSON.stringify({ recordedAtUtc: new Date().toISOString(), results }, null, 2) + "\n");
  return results;
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  generate().then((results) => {
    if (results?.some((result) => ["failed", "blocked"].includes(result.status))) process.exitCode = 1;
  }).catch((error) => { console.error(`CloudEdge generation failed: ${error.message}`); process.exitCode = 1; });
}
