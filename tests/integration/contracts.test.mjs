import assert from "node:assert/strict";
import { createHash } from "node:crypto";
import { spawnSync } from "node:child_process";
import { mkdtempSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import test from "node:test";
import { canonical, catalogContract, checkedHash, diffContracts, operationContracts, runContracts } from "../../scripts/integration/contracts.mjs";

const sha = "a".repeat(64);
const target = { id: "workers", generator: { module: "Workers" } };
function catalog() {
  return { schemaVersion: 1, owner: "Workers", inputs: [{ package: "workers", version: "1.0.0", file: "index.d.ts", sha256: sha, manifestSha256: sha }],
    declarations: [{ owner: "Workers", fSharpName: "Workers.Request", role: "type", arity: 0, api: sha, constraints: [],
      handles: ["workers@1.0.0/index.d.ts#42.12"], identity: sha }] };
}
function schemaFixture() {
  const schema = { openapi: "3.0.3", security: [{ token: [] }], components: {
    securitySchemes: { token: { type: "http", scheme: "bearer" } },
    schemas: { A: { type: "object", properties: { child: { $ref: "#/components/schemas/B" } } },
      B: { type: "object", properties: { value: { type: "string" }, parent: { $ref: "#/components/schemas/A" } } },
      Unused: { type: "string" } } },
  paths: { "/actors/{id}": { parameters: [{ name: "id", in: "path", required: true, schema: { type: "string" } }],
    get: { operationId: "getActor", responses: { "200": { description: "ok", content: { "application/json": { schema: { $ref: "#/components/schemas/A" } } } } } } } } };
  const coverage = [{ method: "get", path: "/actors/{id}", operationId: "getActor", disposition: "included",
    taxonomy: { area: "Management", purpose: "Compute", family: "workers" } }];
  const ownership = { clients: [{ project: "Management.Compute", operationIds: ["getActor"] }] };
  return { schema, coverage, ownership };
}
function operations(fixture) { return operationContracts(fixture.schema, fixture.coverage, fixture.ownership); }
function snapshot() {
  return { schemaVersion: 1, kind: "cloudedge-declaration-contracts", evidence: { level: "authenticated-declaration-inventory" },
    scope: { dispositions: { "workers--root": { library: "workers", publicSubpath: ".", status: "selected" } } },
    libraries: { workers: { package: "workers", version: "1.0.0", publicInputs: { ".": { entry: "index.d.ts", sha256: sha } },
      ...catalogContract(target, catalog()), provenance: { generator: sha } } },
    management: { operations: operations(schemaFixture()), files: { "Client.fs": sha }, pins: {}, excluded: [] }, support: { "Support.fs": sha }, toolchain: {} };
}

test("canonical snapshots ignore object insertion order without reordering overload or parameter arrays", () => {
  assert.equal(canonical({ b: 2, a: { z: 1, c: 2 } }), canonical({ a: { c: 2, z: 1 }, b: 2 }));
  assert.notEqual(canonical({ parameters: ["a", "b"] }), canonical({ parameters: ["b", "a"] }));
});

test("declaration identity survives upstream package versions and source offsets", () => {
  const before = catalog(); const after = catalog();
  after.declarations[0].handles = ["workers@2.0.0/index.d.ts#1234.99"];
  after.declarations[0].identity = "b".repeat(64);
  assert.deepEqual(catalogContract(target, before).declarations, catalogContract(target, after).declarations);
  after.declarations[0].api = "b".repeat(64);
  assert.notDeepEqual(catalogContract(target, before).declarations, catalogContract(target, after).declarations);
});

test("inherited declarations remain owned by their producer instead of inflating consumer inventory", () => {
  const input = catalog();
  input.declarations.push({ ...input.declarations[0], owner: "AI.Provider", fSharpName: "AI.Provider.Model" });
  assert.equal(Object.keys(catalogContract(target, input).declarations).length, 1);
});

test("duplicate declaration identities and missing API evidence fail instead of losing coverage", () => {
  const input = catalog(); input.declarations.push({ ...input.declarations[0] });
  assert.throws(() => catalogContract(target, input), /duplicate identity/);
  input.declarations.pop(); delete input.declarations[0].api;
  assert.throws(() => catalogContract(target, input), /incomplete declaration/);
});

test("authenticated files reject changed, missing and unrecorded input bytes", t => {
  const directory = mkdtempSync(path.join(os.tmpdir(), "cloudedge-contract-hash-"));
  t.after(() => rmSync(directory, { recursive: true, force: true }));
  const file = path.join(directory, "index.d.ts"); writeFileSync(file, "interface Actor {}\n");
  const expected = createHash("sha256").update(readFileSync(file)).digest("hex");
  assert.equal(checkedHash(file, expected), expected);
  writeFileSync(file, "interface Actor { stop(): void }\n");
  assert.throws(() => checkedHash(file, expected), /differs from accepted/);
  assert.throws(() => checkedHash(file, undefined), /missing SHA-256/);
  assert.throws(() => checkedHash(path.join(directory, "missing"), expected), /missing evidence file/);
});

test("referenced schema changes reach operations through cycles, unused schemas do not", () => {
  const fixture = schemaFixture(); const before = operations(fixture);
  fixture.schema.components.schemas.Unused.type = "integer";
  assert.deepEqual(operations(fixture), before);
  fixture.schema.components.schemas.B.properties.value.type = "integer";
  assert.notDeepEqual(operations(fixture), before);
});

test("path parameters and effective security participate in management API drift", () => {
  const fixture = schemaFixture(); const before = operations(fixture);
  fixture.schema.paths["/actors/{id}"].parameters[0].schema.type = "integer";
  assert.notDeepEqual(operations(fixture), before);
  const security = schemaFixture(); const baseline = operations(security);
  security.schema.components.securitySchemes.token.scheme = "basic";
  assert.notDeepEqual(operations(security), baseline);
});

test("unresolved and remote OpenAPI references fail closed", () => {
  const fixture = schemaFixture();
  fixture.schema.components.schemas.B.properties.value = { $ref: "#/components/schemas/Missing" };
  assert.throws(() => operations(fixture), /Unresolved OpenAPI reference/);
  fixture.schema.components.schemas.B.properties.value.$ref = "https://example.invalid/types.json#/Actor";
  assert.throws(() => operations(fixture), /Unsupported OpenAPI reference/);
});

test("missing or duplicate operation owners and stale selection cannot form a baseline", () => {
  const fixture = schemaFixture(); fixture.ownership.clients[0].operationIds = [];
  assert.throws(() => operations(fixture), /differs from generated owner/);
  fixture.ownership.clients[0].operationIds = ["getActor", "getActor"];
  assert.throws(() => operations(fixture), /duplicate identity/);
  fixture.ownership.clients[0].operationIds = ["getActor"];
  fixture.schema.paths["/actors/{id}"].post = { operationId: "newActor" };
  assert.throws(() => operations(fixture), /schema differs from operation coverage/);
});

test("report distinguishes additions, removals, API changes and public input coverage", () => {
  const before = snapshot(), after = snapshot();
  const declarations = after.libraries.workers.declarations;
  declarations["type:Workers.Request:0"].api = "b".repeat(64);
  declarations["type:Workers.Actor:0"] = { api: sha, constraints: [] };
  after.libraries.workers.publicInputs["./actors"] = { entry: "actors.d.ts", sha256: sha };
  after.scope.dispositions["workers--actors"] = { library: "workers", publicSubpath: "./actors", status: "selected" };
  delete after.management.operations["GET /actors/{id}"];
  const report = diffContracts(before, after);
  assert.equal(report.status, "review-required");
  assert.deepEqual(report.libraryChanges.workers.declarations.changed, ["type:Workers.Request:0"]);
  assert.deepEqual(report.libraryChanges.workers.declarations.added, ["type:Workers.Actor:0"]);
  assert.deepEqual(report.libraryChanges.workers.publicInputs.added, ["./actors"]);
  assert.deepEqual(report.publicInputCoverage.added, ["workers--actors"]);
  assert.deepEqual(report.managementOperations.removed, ["GET /actors/{id}"]);
  assert.deepEqual(report.changedPublicInputLibraries, ["workers"]);
  assert.equal(report.behavioralCoverage, "requires-separate-integration-results");
});

test("TypeScript-only and generator-only changes require review even if emitted API hashes match", () => {
  const before = snapshot(), after = snapshot();
  after.libraries.workers.inputs["workers@1.0.0/index.d.ts"].sha256 = "b".repeat(64);
  const report = diffContracts(before, after);
  assert.equal(report.status, "review-required");
  assert.deepEqual(report.libraryChanges.workers.declarations.changed, []);
  assert.deepEqual(report.libraryChanges.workers.typescriptInputs.changed, ["workers@1.0.0/index.d.ts"]);
  const generated = snapshot(); generated.libraries.workers.provenance.generator = "c".repeat(64);
  assert.equal(diffContracts(before, generated).libraryChanges.workers.provenanceChanged, true);
});

test("identical inventory is unchanged and does not imply behavioral acceptance", () => {
  const report = diffContracts(snapshot(), snapshot());
  assert.equal(report.status, "unchanged");
  assert.equal(report.beforeSha256, report.afterSha256);
  assert.equal(report.behavioralCoverage, "requires-separate-integration-results");
  assert.throws(() => diffContracts({}, snapshot()), /Invalid or unauthenticated/);
});

test("CLI refuses implicit baseline creation, overwrites and unsupported options", t => {
  const directory = mkdtempSync(path.join(os.tmpdir(), "cloudedge-contract-cli-"));
  t.after(() => rmSync(directory, { recursive: true, force: true }));
  const first = path.join(directory, "before.json"), second = path.join(directory, "after.json");
  writeFileSync(first, JSON.stringify(snapshot())); writeFileSync(second, JSON.stringify(snapshot()));
  assert.throws(() => runContracts(["snapshot", "--root", directory]), /explicit --output/);
  assert.throws(() => runContracts(["snapshot", "--root", directory, "--output", first]), /already exists/);
  assert.throws(() => runContracts(["diff", first, second, "--output", first]), /cannot overwrite/);
  assert.throws(() => runContracts(["diff", first, second, "--accept"]), /Invalid or repeated option/);
  assert.throws(() => runContracts(["diff", first, second, "--baseline", first]), /applies only to check/);
});

test("CLI returns distinct success, drift and invalid-evidence statuses", t => {
  const directory = mkdtempSync(path.join(os.tmpdir(), "cloudedge-contract-exits-"));
  t.after(() => rmSync(directory, { recursive: true, force: true }));
  const first = path.join(directory, "before.json"), second = path.join(directory, "after.json");
  writeFileSync(first, JSON.stringify(snapshot())); writeFileSync(second, JSON.stringify(snapshot()));
  const invoke = () => spawnSync(process.execPath, [new URL("../../scripts/integration/contracts.mjs", import.meta.url).pathname,
    "diff", first, second], { encoding: "utf8" });
  assert.equal(invoke().status, 0);
  const changed = snapshot(); changed.libraries.workers.version = "2.0.0";
  writeFileSync(second, JSON.stringify(changed));
  const drift = invoke(); assert.equal(drift.status, 2);
  assert.equal(JSON.parse(drift.stdout).status, "review-required");
  writeFileSync(second, "{}");
  assert.equal(invoke().status, 1);
});
