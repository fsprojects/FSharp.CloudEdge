import assert from "node:assert/strict";
import { createHash } from "node:crypto";
import { existsSync, linkSync, mkdirSync, mkdtempSync, readFileSync, renameSync, rmSync, symlinkSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import test from "node:test";
import { inspectInputOverlays, prepareInputOverlays } from "../scripts/input-overlays.mjs";

const digest = (content) => createHash("sha256").update(content).digest("hex");
const original = 'import { Value } from "@src/types";\nexport declare const value: Value;\n';
const expected = original.replace('"@src/types"', '"../types"');
const target = "export interface Value { id: string; }\n";
const packagePath = "node_modules/@sample/provider";
const sourcePath = "dist/utils/value.d.ts";
const integrity = `sha512-${Buffer.alloc(64, 1).toString("base64")}`;

function fixture(t) {
  const root = mkdtempSync(path.join(os.tmpdir(), "input-overlays-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  const profileDirectory = path.join(root, "profiles/sdk");
  const write = (file, value) => {
    mkdirSync(path.dirname(file), { recursive: true });
    writeFileSync(file, typeof value === "string" ? value : JSON.stringify(value, null, 2) + "\n");
  };
  const lock = { name: "sdk", version: "1.0.0", lockfileVersion: 3, packages: { "": { name: "sdk", version: "1.0.0" }, [packagePath]: { version: "1.0.0", integrity } } };
  const config = { schemaVersion: 1, rules: [{
    id: "published-alias", packageName: "@sample/provider", packageVersion: "1.0.0", tarballIntegrity: integrity,
    replacements: [{ file: sourcePath, beforeSha256: digest(original), afterSha256: digest(expected),
      search: 'import { Value } from "@src/types";', replace: 'import { Value } from "../types";', occurrences: 1,
      corroboratingFiles: [{ file: "dist/types.d.ts", sha256: digest(target) }] }]
  }] };
  const configFile = path.join(root, "config/declaration-overlays.json");
  const lockFile = path.join(profileDirectory, "package-lock.json");
  write(path.join(profileDirectory, "package.json"), { name: "sdk", version: "1.0.0", dependencies: { "@sample/provider": "1.0.0" } });
  write(lockFile, lock);
  write(path.join(profileDirectory, packagePath, "package.json"), { name: "@sample/provider", version: "1.0.0" });
  write(path.join(profileDirectory, packagePath, sourcePath), original);
  write(path.join(profileDirectory, packagePath, "dist/types.d.ts"), target);
  write(configFile, config);
  return { root, profileDirectory, lock, lockFile, config, configFile, write,
    sourceFile: path.join(profileDirectory, packagePath, sourcePath),
    targetFile: path.join(profileDirectory, packagePath, "dist/types.d.ts") };
}

test("inspection validates vanilla bytes without copying; preparation preserves the original and reuses verified evidence", (t) => {
  const f = fixture(t);
  const hardlink = path.join(f.root, "vanilla-hardlink.d.ts");
  linkSync(f.sourceFile, hardlink);
  const inspected = inspectInputOverlays(f);
  assert.equal(inspected.applicable, true);
  assert.equal(inspected.applied, false);
  assert.equal(inspected.profileDirectory, f.profileDirectory);
  assert.equal(existsSync(path.join(f.root, "artifacts/inputs")), false);
  assert.equal(inspected.evidence[0].replacements[0].beforeSha256, digest(original));
  assert.equal(inspected.evidence[0].replacements[0].afterSha256, digest(expected));

  const prepared = prepareInputOverlays(f);
  assert.equal(prepared.applied, true);
  assert.equal(prepared.fingerprint, inspected.fingerprint);
  assert.notEqual(prepared.profileDirectory, f.profileDirectory);
  assert.equal(readFileSync(path.join(prepared.profileDirectory, packagePath, sourcePath), "utf8"), expected);
  assert.equal(readFileSync(f.sourceFile, "utf8"), original);
  assert.equal(readFileSync(hardlink, "utf8"), original);
  assert.deepEqual(readFileSync(path.join(prepared.profileDirectory, "package-lock.json")), readFileSync(f.lockFile));
  const metadata = JSON.parse(readFileSync(path.join(prepared.profileDirectory, "overlay-provenance.json"), "utf8"));
  assert.deepEqual(metadata.evidence, inspected.evidence);
  assert.equal(metadata.sourceProfileDirectory, f.profileDirectory);
  assert.deepEqual(prepareInputOverlays(f), prepared);
});

test("a different installed and locked package version receives no overlay or snapshot", (t) => {
  const f = fixture(t);
  f.lock.packages[packagePath].version = "2.0.0";
  f.write(f.lockFile, f.lock);
  f.write(path.join(f.profileDirectory, packagePath, "package.json"), { name: "@sample/provider", version: "2.0.0" });
  f.write(f.sourceFile, "new publisher declarations\n");
  const result = prepareInputOverlays(f);
  assert.equal(result.applicable, false);
  assert.equal(result.applied, false);
  assert.equal(result.profileDirectory, f.profileDirectory);
  assert.equal(existsSync(path.join(f.root, "artifacts/inputs")), false);
});

test("nested versions are matched at their own lockfile paths", (t) => {
  const f = fixture(t);
  const nestedPath = `node_modules/owner/${packagePath}`;
  mkdirSync(path.dirname(path.join(f.profileDirectory, nestedPath)), { recursive: true });
  renameSync(path.join(f.profileDirectory, packagePath), path.join(f.profileDirectory, nestedPath));
  f.lock.packages[nestedPath] = f.lock.packages[packagePath];
  f.lock.packages[packagePath] = { version: "2.0.0", integrity };
  f.write(f.lockFile, f.lock);
  f.write(path.join(f.profileDirectory, packagePath, "package.json"), { name: "@sample/provider", version: "2.0.0" });
  f.write(f.sourceFile, "new version, unchanged\n");
  const result = prepareInputOverlays(f);
  assert.deepEqual(result.evidence.map((item) => item.packagePath), [nestedPath]);
  assert.equal(readFileSync(path.join(result.profileDirectory, nestedPath, sourcePath), "utf8"), expected);
  assert.equal(readFileSync(path.join(result.profileDirectory, packagePath, sourcePath), "utf8"), "new version, unchanged\n");
  assert.equal(readFileSync(path.join(f.profileDirectory, nestedPath, sourcePath), "utf8"), original);
});

for (const mutation of ["source", "corroboration", "missing corroboration", "lock integrity", "missing lock entry"]) {
  test(`inspection rejects ${mutation} changes without creating a snapshot`, (t) => {
    const f = fixture(t);
    if (mutation === "source") f.write(f.sourceFile, original + "// changed\n");
    if (mutation === "corroboration") f.write(f.targetFile, target + "// changed\n");
    if (mutation === "missing corroboration") rmSync(f.targetFile);
    if (mutation === "lock integrity") f.lock.packages[packagePath].integrity = `sha512-${Buffer.alloc(64, 2).toString("base64")}`;
    if (mutation === "missing lock entry") delete f.lock.packages[packagePath];
    f.write(f.lockFile, f.lock);
    assert.throws(() => inspectInputOverlays(f), /Declaration overlay:/);
    assert.equal(existsSync(path.join(f.root, "artifacts/inputs")), false);
  });
}

test("even a hash-approved source cannot replace a repeated import", (t) => {
  const f = fixture(t);
  f.write(f.sourceFile, original + original);
  f.config.rules[0].replacements[0].beforeSha256 = digest(original + original);
  f.write(f.configFile, f.config);
  assert.throws(() => inspectInputOverlays(f), /expected exactly one matching import/);
});

test("replacement text is literal, including JavaScript replacement metacharacters", (t) => {
  const f = fixture(t);
  const replacement = f.config.rules[0].replacements[0];
  replacement.replace = 'import { Value } from "../$&types";';
  const literalResult = original.replace('"@src/types"', () => '"../$&types"');
  replacement.afterSha256 = digest(literalResult);
  f.write(f.configFile, f.config);
  const prepared = prepareInputOverlays(f);
  assert.equal(readFileSync(path.join(prepared.profileDirectory, packagePath, sourcePath), "utf8"), literalResult);
  assert.equal(readFileSync(f.sourceFile, "utf8"), original);
});

for (const mutation of ["declaration", "corroboration", "metadata", "lockfile"]) {
  test(`cache ${mutation} tampering fails until an explicit rebuild`, (t) => {
    const f = fixture(t);
    const prepared = prepareInputOverlays(f);
    if (mutation === "declaration") f.write(path.join(prepared.profileDirectory, packagePath, sourcePath), expected + "// changed\n");
    if (mutation === "corroboration") f.write(path.join(prepared.profileDirectory, packagePath, "dist/types.d.ts"), target + "// changed\n");
    if (mutation === "metadata") {
      const file = path.join(prepared.profileDirectory, "overlay-provenance.json");
      const metadata = JSON.parse(readFileSync(file, "utf8"));
      metadata.fingerprint = "altered";
      f.write(file, metadata);
    }
    if (mutation === "lockfile") f.write(path.join(prepared.profileDirectory, "package-lock.json"), {});
    assert.throws(() => prepareInputOverlays(f), /cached snapshot is invalid; use rebuildCache:true/);
    assert.equal(readFileSync(f.sourceFile, "utf8"), original);
    const rebuilt = prepareInputOverlays({ ...f, rebuildCache: true });
    assert.equal(rebuilt.profileDirectory, prepared.profileDirectory);
    assert.equal(readFileSync(path.join(rebuilt.profileDirectory, packagePath, sourcePath), "utf8"), expected);
    assert.equal(readFileSync(path.join(rebuilt.profileDirectory, packagePath, "dist/types.d.ts"), "utf8"), target);
    assert.deepEqual(prepareInputOverlays(f), rebuilt);
  });
}

test("matching symlinked package directories are refused before copying or writing", (t) => {
  const f = fixture(t);
  const linkedDirectory = path.join(f.root, "external-package");
  const installedDirectory = path.join(f.profileDirectory, packagePath);
  renameSync(installedDirectory, linkedDirectory);
  symlinkSync(linkedDirectory, installedDirectory, "dir");
  assert.throws(() => prepareInputOverlays(f), /linked package directory cannot be snapshotted/);
  assert.equal(readFileSync(path.join(linkedDirectory, sourcePath), "utf8"), original);
  assert.equal(existsSync(path.join(f.root, "artifacts/inputs")), false);
});

test("rule and lockfile changes independently invalidate the snapshot key", (t) => {
  const f = fixture(t);
  const initial = prepareInputOverlays(f);
  f.config.rules[0].evidence = { note: "reviewed publisher declarations" };
  f.write(f.configFile, f.config);
  const revisedRule = prepareInputOverlays(f);
  assert.notEqual(revisedRule.profileDirectory, initial.profileDirectory);
  f.lock.packages[""].description = "profile metadata changed";
  f.write(f.lockFile, f.lock);
  const revisedLock = prepareInputOverlays(f);
  assert.notEqual(revisedLock.profileDirectory, revisedRule.profileDirectory);
  assert.equal(readFileSync(f.sourceFile, "utf8"), original);
});

function externalProvider(f, packagePath = "node_modules/@sample/shared", version = "2.0.0") {
  const declaration = "export interface Value { id: string; shared: true; }\n";
  f.write(path.join(f.profileDirectory, packagePath, "package.json"), { name: "@sample/shared", version });
  f.write(path.join(f.profileDirectory, packagePath, "index.d.ts"), declaration);
  f.lock.packages[packagePath] = { version, integrity };
  f.write(f.lockFile, f.lock);
  const corroboration = { packageName: "@sample/shared", packageVersion: version, tarballIntegrity: integrity,
    file: "index.d.ts", sha256: digest(declaration) };
  f.config.rules[0].replacements[0].corroboratingFiles = [corroboration];
  f.write(f.configFile, f.config);
  return { declaration, corroboration, packagePath };
}

test("external corroboration verifies the resolved provider and records its exact installed path", (t) => {
  const f = fixture(t);
  const provider = externalProvider(f);
  const inspection = inspectInputOverlays(f);
  assert.equal(inspection.evidence[0].externalCorroborations[0].packagePath, provider.packagePath);
  const prepared = prepareInputOverlays(f);
  assert.equal(readFileSync(path.join(prepared.profileDirectory, provider.packagePath, "index.d.ts"), "utf8"), provider.declaration);
  assert.equal(readFileSync(path.join(prepared.profileDirectory, packagePath, sourcePath), "utf8"), expected);
  assert.deepEqual(prepareInputOverlays(f), prepared);
});

for (const mutation of ["bytes", "version", "integrity", "missing lock", "missing provider"]) {
  test(`external corroboration refuses provider ${mutation} mismatch`, (t) => {
    const f = fixture(t);
    const provider = externalProvider(f);
    const directory = path.join(f.profileDirectory, provider.packagePath);
    if (mutation === "bytes") f.write(path.join(directory, "index.d.ts"), "export interface Value {}\n");
    if (mutation === "version") f.write(path.join(directory, "package.json"), { name: "@sample/shared", version: "3.0.0" });
    if (mutation === "integrity") f.lock.packages[provider.packagePath].integrity = "sha512-different";
    if (mutation === "missing lock") delete f.lock.packages[provider.packagePath];
    if (mutation === "missing provider") rmSync(directory, { recursive: true });
    f.write(f.lockFile, f.lock);
    assert.throws(() => inspectInputOverlays(f), /corroboration.*(SHA-256|identity|integrity|missing)/);
  });
}

test("external corroboration uses the nearest nested provider rather than another matching version", (t) => {
  const f = fixture(t);
  externalProvider(f);
  const nested = externalProvider(f, `${packagePath}/node_modules/@sample/shared`, "3.0.0");
  const inspection = inspectInputOverlays(f);
  assert.equal(inspection.evidence[0].externalCorroborations[0].packagePath, nested.packagePath);
  f.config.rules[0].replacements[0].corroboratingFiles[0].packageVersion = "2.0.0";
  f.write(f.configFile, f.config);
  assert.throws(() => inspectInputOverlays(f), /corroboration.*identity/);
});

test("cached external provider tampering is rejected", (t) => {
  const f = fixture(t);
  const provider = externalProvider(f);
  const prepared = prepareInputOverlays(f);
  f.write(path.join(prepared.profileDirectory, provider.packagePath, "index.d.ts"), "changed provider\n");
  assert.throws(() => prepareInputOverlays(f), /corroboration.*SHA-256/);
});
