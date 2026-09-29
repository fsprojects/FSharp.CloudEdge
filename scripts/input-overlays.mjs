import { createHash } from "node:crypto";
import { constants, copyFileSync, cpSync, existsSync, mkdirSync, mkdtempSync, readdirSync, readFileSync, realpathSync, renameSync, rmSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const defaultRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const sha256 = (value) => createHash("sha256").update(value).digest("hex");
const hashFile = (file) => sha256(readFileSync(file));
const readJson = (file) => JSON.parse(readFileSync(file, "utf8"));
const requireThat = (condition, message) => { if (!condition) throw new Error(`Declaration overlay: ${message}`); };
const sha = (value) => typeof value === "string" && /^[a-f0-9]{64}$/.test(value);

function inside(directory, relative) {
  requireThat(typeof relative === "string" && relative.length > 0 && !path.isAbsolute(relative), "expected a relative file path");
  const file = path.resolve(directory, relative);
  requireThat(file.startsWith(path.resolve(directory) + path.sep), `path escapes its package: ${relative}`);
  return file;
}

function readRules(root) {
  const file = path.join(root, "config/declaration-overlays.json");
  const content = existsSync(file) ? readFileSync(file) : Buffer.from('{"schemaVersion":1,"rules":[]}');
  const config = JSON.parse(content.toString("utf8"));
  requireThat(config.schemaVersion === 1 && Array.isArray(config.rules), "unsupported overlay configuration");
  const ids = new Set();
  const files = new Set();
  for (const rule of config.rules) {
    requireThat(typeof rule.id === "string" && rule.id.length > 0 && !ids.has(rule.id), "rule IDs must be nonempty and unique");
    ids.add(rule.id);
    requireThat(typeof rule.packageName === "string" && /^(?:@[a-z0-9._-]+\/)?[a-z0-9._-]+$/.test(rule.packageName), `${rule.id}: invalid package name`);
    requireThat(typeof rule.packageVersion === "string" && /^\d+\.\d+\.\d+(?:-[a-zA-Z0-9.-]+)?$/.test(rule.packageVersion), `${rule.id}: exact package version required`);
    requireThat(typeof rule.tarballIntegrity === "string" && rule.tarballIntegrity.startsWith("sha512-"), `${rule.id}: pinned tarball integrity required`);
    requireThat(Array.isArray(rule.replacements) && rule.replacements.length > 0, `${rule.id}: replacements required`);
    for (const replacement of rule.replacements) {
      inside(root, replacement.file);
      requireThat(sha(replacement.beforeSha256) && sha(replacement.afterSha256), `${rule.id}: before/after SHA-256 required`);
      requireThat(typeof replacement.search === "string" && replacement.search.length > 0 && typeof replacement.replace === "string" && replacement.replace !== replacement.search, `${rule.id}: distinct exact replacement text required`);
      requireThat(replacement.occurrences === 1, `${rule.id}: exactly one replacement is required`);
      requireThat(Array.isArray(replacement.corroboratingFiles) && replacement.corroboratingFiles.length > 0, `${rule.id}: corroborating files required`);
      for (const corroboration of replacement.corroboratingFiles) {
        inside(root, corroboration.file);
        requireThat(sha(corroboration.sha256), `${rule.id}: corroborating SHA-256 required`);
        if (corroboration.packageName !== undefined) {
          requireThat(typeof corroboration.packageName === "string" && /^(?:@[a-z0-9._-]+\/)?[a-z0-9._-]+$/.test(corroboration.packageName), `${rule.id}: invalid corroborating package name`);
          requireThat(typeof corroboration.packageVersion === "string" && /^\d+\.\d+\.\d+(?:-[a-zA-Z0-9.-]+)?$/.test(corroboration.packageVersion), `${rule.id}: exact corroborating package version required`);
          requireThat(typeof corroboration.tarballIntegrity === "string" && corroboration.tarballIntegrity.startsWith("sha512-"), `${rule.id}: corroborating tarball integrity required`);
        }
      }
      const key = [rule.packageName, rule.packageVersion, replacement.file].join(":");
      requireThat(!files.has(key), `multiple rules target ${key}`);
      files.add(key);
    }
  }
  return { rules: config.rules, configSha256: sha256(content) };
}

function checkedFile(packageDirectory, relative, expected, label) {
  const file = inside(packageDirectory, relative);
  requireThat(existsSync(file), `${label}: missing ${relative}`);
  requireThat(realpathSync(file).startsWith(realpathSync(packageDirectory) + path.sep), `${label}: linked file escapes package: ${relative}`);
  requireThat(hashFile(file) === expected, `${label}: SHA-256 mismatch for ${relative}`);
  return file;
}

function checkedPackageDirectory(profileDirectory, packagePath, label) {
  const directory = inside(profileDirectory, packagePath);
  requireThat(realpathSync(directory) === inside(realpathSync(profileDirectory), packagePath), `${label}: linked package directory cannot be snapshotted: ${packagePath}`);
  return directory;
}

function checkedCorroboration(profileDirectory, packageDirectory, corroboration, lock, label) {
  label += " corroboration";
  if (!corroboration.packageName) {
    checkedFile(packageDirectory, corroboration.file, corroboration.sha256, label);
    return null;
  }
  // Resolve from the importing package, preserving npm's nearest nested version.
  // Never continue past a mismatching provider to find a convenient outer one.
  let provider;
  for (let directory = packageDirectory; directory.startsWith(profileDirectory + path.sep) || directory === profileDirectory; directory = path.dirname(directory)) {
    const candidate = path.join(directory, "node_modules", corroboration.packageName);
    if (existsSync(path.join(candidate, "package.json"))) { provider = candidate; break; }
  }
  requireThat(provider, `${label}: missing provider ${corroboration.packageName}`);
  const packagePath = path.relative(profileDirectory, provider).split(path.sep).join("/");
  checkedPackageDirectory(profileDirectory, packagePath, label);
  const manifest = readJson(path.join(provider, "package.json"));
  const locked = lock.packages?.[packagePath];
  requireThat(manifest.name === corroboration.packageName && manifest.version === corroboration.packageVersion && locked?.version === corroboration.packageVersion,
    `${label}: provider identity differs at ${packagePath}`);
  requireThat(locked.integrity === corroboration.tarballIntegrity, `${label}: provider integrity differs at ${packagePath}`);
  checkedFile(provider, corroboration.file, corroboration.sha256, label);
  return { packagePath, ...corroboration };
}

// Walk only npm's package directories, never the potentially large package contents.
function installedPackages(profileDirectory) {
  const result = new Set();
  function visit(relative) {
    const directory = path.join(profileDirectory, relative);
    if (!existsSync(directory)) return;
    for (const entry of readdirSync(directory, { withFileTypes: true })) {
      if (entry.name.startsWith(".") || (!entry.isDirectory() && !entry.isSymbolicLink())) continue;
      const packagePath = `${relative}/${entry.name}`;
      if (entry.name.startsWith("@")) { if (entry.isDirectory()) visit(packagePath); continue; }
      result.add(packagePath);
      if (entry.isDirectory()) visit(`${packagePath}/node_modules`);
    }
  }
  visit("node_modules");
  return result;
}

function transformed(content, replacement, ruleId) {
  const source = content.toString("utf8");
  requireThat(source.split(replacement.search).length - 1 === 1, `${ruleId}: expected exactly one matching import in ${replacement.file}`);
  const result = Buffer.from(source.replace(replacement.search, () => replacement.replace), "utf8");
  requireThat(sha256(result) === replacement.afterSha256, `${ruleId}: transformed SHA-256 mismatch for ${replacement.file}`);
  return result;
}

/** Verify vanilla inputs and return deterministic evidence without copying node_modules. */
export function inspectInputOverlays({ root = defaultRoot, profileDirectory } = {}) {
  requireThat(typeof profileDirectory === "string", "profileDirectory is required");
  const sourceProfileDirectory = path.resolve(profileDirectory);
  const { rules, configSha256 } = readRules(path.resolve(root));
  const lockFile = path.join(sourceProfileDirectory, "package-lock.json");
  const manifestFile = path.join(sourceProfileDirectory, "package.json");
  const lock = readJson(lockFile);
  const profileLockSha256 = hashFile(lockFile);
  const profileManifestSha256 = hashFile(manifestFile);
  const packagePaths = new Set([...Object.keys(lock.packages ?? {}), ...(rules.length > 0 ? installedPackages(sourceProfileDirectory) : [])]);
  const evidence = [];
  for (const rule of rules) {
    for (const packagePath of [...packagePaths].sort()) {
      if (packagePath !== `node_modules/${rule.packageName}` && !packagePath.endsWith(`/node_modules/${rule.packageName}`)) continue;
      const packageDirectory = inside(sourceProfileDirectory, packagePath);
      const manifest = readJson(path.join(packageDirectory, "package.json"));
      const locked = lock.packages?.[packagePath];
      if (!locked && manifest.version !== rule.packageVersion) continue;
      requireThat(manifest.name === rule.packageName && manifest.version === locked?.version, `${rule.id}: installed package differs from lockfile at ${packagePath}`);
      if (manifest.version !== rule.packageVersion) continue;
      checkedPackageDirectory(sourceProfileDirectory, packagePath, rule.id);
      requireThat(locked.integrity === rule.tarballIntegrity, `${rule.id}: tarball integrity differs from lockfile at ${packagePath}`);
      const externalCorroborations = [];
      for (const replacement of rule.replacements) {
        const file = checkedFile(packageDirectory, replacement.file, replacement.beforeSha256, rule.id);
        transformed(readFileSync(file), replacement, rule.id);
        for (const corroboration of replacement.corroboratingFiles) {
          const external = checkedCorroboration(sourceProfileDirectory, packageDirectory, corroboration, lock, rule.id);
          if (external) externalCorroborations.push(external);
        }
      }
      evidence.push({ ruleId: rule.id, packageName: rule.packageName, packageVersion: rule.packageVersion, packagePath, tarballIntegrity: rule.tarballIntegrity, replacements: rule.replacements,
        ...(externalCorroborations.length ? { externalCorroborations } : {}) });
    }
  }
  const fingerprint = sha256(JSON.stringify({ schemaVersion: 1, sourceProfileDirectory, configSha256, profileLockSha256, profileManifestSha256, evidence }));
  return { profileDirectory: sourceProfileDirectory, sourceProfileDirectory, applicable: evidence.length > 0, applied: false, fingerprint, configSha256, profileLockSha256, profileManifestSha256, evidence };
}

function validateSnapshot(snapshotDirectory, inspection) {
  const label = "cached snapshot is invalid; use rebuildCache:true to rebuild it";
  const metadataFile = path.join(snapshotDirectory, "overlay-provenance.json");
  requireThat(existsSync(metadataFile), `${label}: missing metadata`);
  const metadata = readJson(metadataFile);
  for (const field of ["fingerprint", "sourceProfileDirectory", "configSha256", "profileLockSha256", "profileManifestSha256"]) requireThat(metadata[field] === inspection[field], `${label}: ${field} differs`);
  requireThat(metadata.schemaVersion === 1 && metadata.applied === true && metadata.applicable === true && JSON.stringify(metadata.evidence) === JSON.stringify(inspection.evidence), `${label}: evidence differs`);
  requireThat(hashFile(path.join(snapshotDirectory, "package-lock.json")) === inspection.profileLockSha256, `${label}: profile lockfile differs`);
  requireThat(hashFile(path.join(snapshotDirectory, "package.json")) === inspection.profileManifestSha256, `${label}: profile manifest differs`);
  const lock = readJson(path.join(snapshotDirectory, "package-lock.json"));
  for (const item of inspection.evidence) {
    const packageDirectory = checkedPackageDirectory(snapshotDirectory, item.packagePath, label);
    const manifest = readJson(path.join(packageDirectory, "package.json"));
    requireThat(manifest.name === item.packageName && manifest.version === item.packageVersion && lock.packages[item.packagePath]?.integrity === item.tarballIntegrity, `${label}: package identity differs`);
    const externalCorroborations = [];
    for (const replacement of item.replacements) {
      checkedFile(packageDirectory, replacement.file, replacement.afterSha256, label);
      for (const corroboration of replacement.corroboratingFiles) {
        const external = checkedCorroboration(snapshotDirectory, packageDirectory, corroboration, lock, label);
        if (external) externalCorroborations.push(external);
      }
    }
    requireThat(JSON.stringify(externalCorroborations) === JSON.stringify(item.externalCorroborations ?? []), `${label}: resolved corroborating providers differ`);
  }
}

/** Create or verify an immutable input snapshot. Unaffected profiles retain their original path. */
export function prepareInputOverlays({ root = defaultRoot, profileDirectory, rebuildCache = false } = {}) {
  const inspection = inspectInputOverlays({ root, profileDirectory });
  if (!inspection.applicable) return inspection;
  const cacheRoot = path.join(path.resolve(root), "artifacts/inputs");
  mkdirSync(cacheRoot, { recursive: true });
  const snapshotDirectory = path.join(cacheRoot, `${path.basename(inspection.sourceProfileDirectory)}-${inspection.fingerprint}`);
  if (existsSync(snapshotDirectory) && rebuildCache) rmSync(snapshotDirectory, { recursive: true, force: true });
  if (!existsSync(snapshotDirectory)) {
    const staging = mkdtempSync(path.join(cacheRoot, ".overlay-staging-"));
    try {
      for (const name of ["package.json", "package-lock.json"]) copyFileSync(path.join(inspection.sourceProfileDirectory, name), path.join(staging, name), constants.COPYFILE_FICLONE);
      cpSync(path.join(inspection.sourceProfileDirectory, "node_modules"), path.join(staging, "node_modules"), { recursive: true, mode: constants.COPYFILE_FICLONE, verbatimSymlinks: true });
      for (const item of inspection.evidence) {
        const packageDirectory = checkedPackageDirectory(staging, item.packagePath, item.ruleId);
        for (const replacement of item.replacements) {
          const file = checkedFile(packageDirectory, replacement.file, replacement.beforeSha256, item.ruleId);
          const temporary = `${file}.overlay-${process.pid}.tmp`;
          writeFileSync(temporary, transformed(readFileSync(file), replacement, item.ruleId));
          renameSync(temporary, file);
        }
      }
      writeFileSync(path.join(staging, "overlay-provenance.json"), JSON.stringify({ schemaVersion: 1, ...inspection, applied: true }, null, 2) + "\n");
      validateSnapshot(staging, inspection);
      requireThat(inspectInputOverlays({ root, profileDirectory }).fingerprint === inspection.fingerprint, "vanilla inputs changed while preparing the snapshot");
      try { renameSync(staging, snapshotDirectory); }
      catch (error) {
        if (!existsSync(snapshotDirectory)) throw error;
        validateSnapshot(snapshotDirectory, inspection);
      }
    } finally { rmSync(staging, { recursive: true, force: true }); }
  }
  validateSnapshot(snapshotDirectory, inspection);
  return { ...inspection, profileDirectory: snapshotDirectory, applied: true };
}
