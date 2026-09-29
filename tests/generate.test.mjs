import assert from "node:assert/strict";
import { createHash } from "node:crypto";
import { chmodSync, existsSync, mkdirSync, mkdtempSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import test from "node:test";
import { generate } from "../scripts/generate.mjs";

const compilerVersion = "7.1.0-dev.20260902.1";
const json = (file) => JSON.parse(readFileSync(file, "utf8"));
const hash = (text) => createHash("sha256").update(text).digest("hex");

function fixture(t) {
  const root = mkdtempSync(path.join(os.tmpdir(), "cloudedge-generation-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  const write = (name, value) => {
    const file = path.join(root, name);
    mkdirSync(path.dirname(file), { recursive: true });
    writeFileSync(file, typeof value === "string" ? value : JSON.stringify(value));
    return file;
  };
  write("package.json", { type: "module", devDependencies: { typescript: compilerVersion } });
  write("package-lock.json", { packages: { "": { devDependencies: { typescript: compilerVersion } }, "node_modules/typescript": { version: compilerVersion } } });
  write("node_modules/typescript/package.json", { name: "typescript", version: compilerVersion, type: "module" });
  write("node_modules/typescript/lib/getExePath.js", 'export default () => new URL("../../@typescript/typescript-linux-x64/lib/tsc", import.meta.url).pathname;');
  write("node_modules/@typescript/typescript-linux-x64/package.json", { version: compilerVersion });
  write("node_modules/@typescript/typescript-linux-x64/lib/tsc", "compiler");
  write("profiles/sdk/package.json", { devDependencies: { sdk: "1.2.3" } });
  write("profiles/sdk/package-lock.json", { packages: { "": { devDependencies: { sdk: "1.2.3" } }, "node_modules/sdk": { version: "1.2.3" } } });
  write("profiles/sdk/node_modules/sdk/package.json", { name: "sdk", version: "1.2.3" });
  const toolFile = write("installed-tools/xantham.dll", "generator");
  const toolResolver = () => ({ args: ["tool", "run", "xantham", "--"], fingerprint: {
    packageId: "Xantham.Cli", command: "xantham", version: "0.1.0-local.fixture",
    packageSha256: hash("package"), payloadSha256: { "xantham.dll": hash(readFileSync(toolFile)) },
    manifestSha256: hash("manifest"),
  } });
  const dotnet = write("fake-dotnet.mjs", `#!${process.execPath}
import { appendFileSync, mkdirSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";
const args = process.argv.slice(2);
const config = JSON.parse(readFileSync(args[args.indexOf("--config") + 1], "utf8"));
appendFileSync(process.env.FAKE_CALLS, config.module + "\\n");
if (process.env.FAKE_FAIL === config.module) process.exit(4);
const output = args[args.indexOf("--out") + 1];
mkdirSync(output, { recursive: true });
writeFileSync(path.join(output, config.module + ".fs"), "module " + config.module + "\\n");
if (process.env.FAKE_EXTRA) writeFileSync(path.join(output, "Unexpected.fs"), "module Unexpected");
writeFileSync(path.join(output, "manifest.json"), JSON.stringify({ package: "sdk", module: config.module, counts: { exact: 1 } }));
writeFileSync(path.join(output, "symbols.jsonl"), "{}\\n");
if (config.declarationCatalog) writeFileSync(path.join(output, "declarations.json"), JSON.stringify({ schemaVersion: 1, owner: config.module, owners: [{ name: config.module, dependencies: [] }], declarations: [] }));
if (process.env.FAKE_MUTATE) writeFileSync(process.env.FAKE_TOOL_FILE, "changed generator");
if (process.env.FAKE_MUTATE_REFERENCE) writeFileSync(config.declarationReferences[0], "changed reference");
if (process.env.FAKE_PACKAGE_LOG) writeFileSync(process.env.FAKE_PACKAGE_LOG, args[args.indexOf("generate") + 1]);
if (process.env.FAKE_MUTATE_FILE) writeFileSync(process.env.FAKE_MUTATE_FILE, "{}");
if (process.env.FAKE_MUTATE_SNAPSHOT) writeFileSync(path.join(args[args.indexOf("generate") + 1], "index.d.ts"), "changed snapshot");
`);
  chmodSync(dotnet, 0o755);
  const target = (id, dependencies = []) => ({ id, package: "sdk", version: "1.2.3", profile: "sdk", dependencies,
    outputDirectory: `src/${id}`, files: [`${id}.fs`], generator: { module: id } });
  const config = { schemaVersion: 2, toolchain: { typescript: compilerVersion },
    xantham: { tool: "xantham" },
    profiles: [{ id: "sdk", directory: "profiles/sdk" }], projectDefaults: {},
    targets: [target("owner"), target("adapter", ["owner"]), target("unrelated")] };
  const save = () => write("config/targets.json", config);
  save();
  const env = { ...process.env, CLOUDEDGE_DOTNET: dotnet, FAKE_TOOL_FILE: toolFile, FAKE_CALLS: path.join(root, "calls.txt") };
  const run = (args = ["owner"], extraEnv = {}) => generate({ root, args, env: { ...env, ...extraEnv }, toolResolver });
  const calls = () => existsSync(env.FAKE_CALLS) ? readFileSync(env.FAKE_CALLS, "utf8").trim().split("\n") : [];
  return { root, write, config, save, run, calls };
}

function overlay(f) {
  const before = "import { Value } from '@source/types';\n";
  const after = "import { Value } from './types';\n";
  const corroboration = "export interface Value { text: string }\n";
  const lock = json(path.join(f.root, "profiles/sdk/package-lock.json"));
  lock.packages["node_modules/sdk"].integrity = "sha512-fixture";
  f.write("profiles/sdk/package-lock.json", lock);
  f.write("profiles/sdk/node_modules/sdk/index.d.ts", before);
  f.write("profiles/sdk/node_modules/sdk/types.d.ts", corroboration);
  f.write("config/declaration-overlays.json", { schemaVersion: 1, rules: [{ id: "fixture-import", packageName: "sdk", packageVersion: "1.2.3", tarballIntegrity: "sha512-fixture", replacements: [{ file: "index.d.ts", search: "'@source/types'", replace: "'./types'", occurrences: 1, beforeSha256: hash(before), afterSha256: hash(after), corroboratingFiles: [{ file: "types.d.ts", sha256: hash(corroboration) }] }] }] });
  return { before, after };
}

test("generation schedules only the selected dependency closure, producer first", async (t) => {
  const f = fixture(t);
  assert.deepEqual((await f.run(["adapter"])).map((result) => result.target), ["owner", "adapter"]);
  assert.deepEqual(f.calls(), ["owner", "adapter"]);
});

test("generation rejects a cycle before invoking the tool", async (t) => {
  const f = fixture(t);
  f.config.targets[0].dependencies = ["adapter"];
  f.save();
  await assert.rejects(f.run(), /Cyclic target dependencies/);
  assert.deepEqual(f.calls(), []);
});

test("a profile version mismatch fails before invoking the tool", async (t) => {
  const f = fixture(t);
  f.write("profiles/sdk/node_modules/sdk/package.json", { name: "sdk", version: "2.0.0" });
  await assert.rejects(f.run(), /installed package differs/);
  assert.deepEqual(f.calls(), []);
});

test("resume verifies the exact source inventory and generated bytes", async (t) => {
  const f = fixture(t);
  await f.run();
  assert.equal((await f.run(["--resume", "owner"]))[0].status, "current");
  f.write("src/owner/owner.fs", "changed source");
  assert.equal((await f.run(["--resume", "owner"]))[0].status, "generated");
  const receipt = json(path.join(f.root, "artifacts/owner/generation.json"));
  receipt.sourceSha256["Obsolete.fs"] = "old digest";
  f.write("artifacts/owner/generation.json", receipt);
  assert.equal((await f.run(["--resume", "owner"]))[0].status, "generated");
  assert.deepEqual(f.calls(), ["owner", "owner", "owner"]);
});

test("unexpected output preserves the last accepted source and provenance", async (t) => {
  const f = fixture(t);
  await f.run();
  const receipt = readFileSync(path.join(f.root, "artifacts/owner/generation.json"), "utf8");
  await assert.rejects(f.run(["owner"], { FAKE_EXTRA: "1" }), /emitted F# files differ/);
  assert.equal(readFileSync(path.join(f.root, "artifacts/owner/generation.json"), "utf8"), receipt);
  assert.equal(readFileSync(path.join(f.root, "src/owner/owner.fs"), "utf8"), "module owner\n");
});

test("a changed generator cannot authenticate its own in-flight output", async (t) => {
  const f = fixture(t);
  await assert.rejects(f.run(["owner"], { FAKE_MUTATE: "1" }), /changed during generation/);
  assert.ok(!existsSync(path.join(f.root, "artifacts/owner/generation.json")));
});

test("catalogs survive staging cleanup and resume validates producer and reference hashes", async (t) => {
  const f = fixture(t);
  f.config.targets[0].generator.declarationCatalog = true;
  const adapter = f.config.targets[1];
  adapter.generator.declarationCatalog = true;
  adapter.generator.declarationReferences = [path.relative(path.join(f.root, "profiles/sdk/node_modules/sdk"), path.join(f.root, "artifacts/owner/declarations.json"))];
  f.save();
  await f.run(["adapter"]);
  assert.ok(existsSync(path.join(f.root, "artifacts/owner/declarations.json")));
  const receipt = json(path.join(f.root, "artifacts/adapter/generation.json"));
  assert.equal(Object.keys(receipt.declarationReferenceSha256).length, 1);
  assert.ok(path.isAbsolute(receipt.effectiveGenerator.declarationReferences[0]));
  assert.deepEqual((await f.run(["--resume", "adapter"])).map((result) => result.status), ["current", "current"]);
  f.write("artifacts/owner/declarations.json", "tampered catalog");
  assert.equal((await f.run(["--resume", "owner"]))[0].status, "generated");
  f.config.targets[1].dependencies = [];
  f.save();
  f.write("artifacts/owner/declarations.json", { owner: "owner", declarations: ["new input"] });
  assert.equal((await f.run(["--resume", "adapter"]))[0].status, "generated");
});

test("catalog reference mutation rejects in-flight output", async (t) => {
  const f = fixture(t);
  const reference = f.write("producer.json", { declarations: [] });
  f.config.targets[0].generator.declarationReferences = [reference];
  f.save();
  await assert.rejects(f.run(["owner"], { FAKE_MUTATE_REFERENCE: "1" }), /catalog changed during generation/);
  assert.ok(!existsSync(path.join(f.root, "artifacts/owner/generation.json")));
});

test("continuing after a failure blocks its consumers and generates independent targets", async (t) => {
  const f = fixture(t);
  const results = await f.run(["--continue-on-error"], { FAKE_FAIL: "owner" });
  assert.deepEqual(results.map(({ target, status }) => [target, status]), [["owner", "failed"], ["adapter", "blocked"], ["unrelated", "generated"]]);
  assert.deepEqual(f.calls(), ["owner", "unrelated"]);
});

test("overlays supply separate verified inputs while catalog paths retain their original base", async (t) => {
  const f = fixture(t);
  const { before, after } = overlay(f);
  f.config.targets[0].generator.declarationCatalog = true;
  f.config.targets[1].generator.declarationReferences = ["../../../../artifacts/owner/declarations.json"];
  f.save();
  const inputLog = path.join(f.root, "input.txt");
  await f.run(["--check", "adapter"]);
  assert.ok(!existsSync(path.join(f.root, "artifacts/inputs")));
  await f.run(["adapter"], { FAKE_PACKAGE_LOG: inputLog });
  const receipt = json(path.join(f.root, "artifacts/adapter/generation.json"));
  const actualInput = readFileSync(inputLog, "utf8");
  assert.ok(actualInput.startsWith(path.join(f.root, "artifacts/inputs") + path.sep));
  assert.equal(readFileSync(path.join(actualInput, "index.d.ts"), "utf8"), after);
  assert.equal(readFileSync(path.join(f.root, "profiles/sdk/node_modules/sdk/index.d.ts"), "utf8"), before);
  assert.equal(receipt.inputPackageDirectory, actualInput);
  assert.equal(receipt.inputOverlays.applied, true);
  assert.equal(receipt.inputOverlays.evidence[0].ruleId, "fixture-import");
  assert.equal(receipt.effectiveGenerator.declarationReferences[0], path.join(f.root, "artifacts/owner/declarations.json"));
  assert.deepEqual((await f.run(["--resume", "adapter"])).map((result) => result.status), ["current", "current"]);
  f.write(path.relative(f.root, path.join(actualInput, "index.d.ts")), "tampered cache");
  await assert.rejects(f.run(["--resume", "adapter"]), /cached snapshot is invalid/);
});

test("resume invalidates receipts when profile metadata or overlay policy changes", async (t) => {
  const f = fixture(t);
  await f.run();
  const pkg = json(path.join(f.root, "profiles/sdk/package.json"));
  pkg.description = "Updated dependency profile";
  f.write("profiles/sdk/package.json", pkg);
  assert.equal((await f.run(["--resume", "owner"]))[0].status, "generated");
  f.write("config/declaration-overlays.json", { schemaVersion: 1, rules: [], note: "Reviewed policy" });
  assert.equal((await f.run(["--resume", "owner"]))[0].status, "generated");
});

test("profile metadata mutation rejects in-flight output", async (t) => {
  const f = fixture(t);
  await assert.rejects(f.run(["owner"], { FAKE_MUTATE_FILE: path.join(f.root, "profiles/sdk/package.json") }), /profile or declaration catalog changed/);
  assert.ok(!existsSync(path.join(f.root, "artifacts/owner/generation.json")));
});

test("snapshot mutation rejects in-flight output", async (t) => {
  const f = fixture(t);
  overlay(f);
  await assert.rejects(f.run(["owner"], { FAKE_MUTATE_SNAPSHOT: "1" }), /cached snapshot is invalid/);
  assert.ok(!existsSync(path.join(f.root, "artifacts/owner/generation.json")));
});

function candidateFamily(f) {
  f.config.targets.forEach((target, index) => {
    target.dependencies = [];
    target.generator.declarationCatalog = true;
    if (index) target.catalogCandidates = [f.config.targets[index - 1].id];
  });
  f.save();
}

test("candidate catalogs schedule the selected prefix and record absolute inputs with exact hashes", async (t) => {
  const f = fixture(t);
  candidateFamily(f);
  await f.run(["--check", "adapter"]);
  assert.deepEqual(f.calls(), []);
  assert.deepEqual((await f.run(["adapter"])).map((result) => result.target), ["owner", "adapter"]);
  const receipt = json(path.join(f.root, "artifacts/adapter/generation.json"));
  const reference = path.join(f.root, "artifacts/owner/declarations.json");
  assert.deepEqual(receipt.catalogCandidates, ["owner"]);
  assert.deepEqual(receipt.effectiveGenerator.declarationReferences, [reference]);
  assert.deepEqual(receipt.declarationReferenceSha256, { "../../../../artifacts/owner/declarations.json": hash(readFileSync(reference)) });
});

test("an unused candidate is still a scheduling prerequisite and failure blocks its successors", async (t) => {
  const f = fixture(t);
  candidateFamily(f);
  const results = await f.run(["--continue-on-error", "unrelated"], { FAKE_FAIL: "adapter" });
  assert.deepEqual(results.map((result) => [result.target, result.status]), [["owner", "generated"], ["adapter", "failed"], ["unrelated", "blocked"]]);
  assert.deepEqual(f.calls(), ["owner", "adapter"]);
});

test("resume invalidates changed candidate scheduling policy and missing candidate metadata", async (t) => {
  const f = fixture(t);
  candidateFamily(f);
  await f.run(["unrelated"]);
  f.config.targets[2].catalogCandidates = ["owner"];
  f.save();
  assert.deepEqual((await f.run(["--resume", "unrelated"])).map((result) => [result.target, result.status]), [["owner", "current"], ["unrelated", "generated"]]);
  const receipt = json(path.join(f.root, "artifacts/owner/generation.json"));
  delete receipt.catalogCandidates;
  f.write("artifacts/owner/generation.json", receipt);
  assert.equal((await f.run(["--resume", "owner"]))[0].status, "generated");
});
