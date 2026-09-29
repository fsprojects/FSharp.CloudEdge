import assert from "node:assert/strict";
import { createHash } from "node:crypto";
import { mkdtempSync, mkdirSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import test from "node:test";
import { currentHawaii } from "../scripts/build-library.mjs";

function fixture(t) {
  const root = mkdtempSync(path.join(os.tmpdir(), "cloudedge-hawaii-receipt-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  const write = (name, text) => { const file = path.join(root, name); mkdirSync(path.dirname(file), { recursive: true }); writeFileSync(file, text); };
  const json = (name, value) => write(name, JSON.stringify(value));
  const hash = name => createHash("sha256").update(readFileSync(path.join(root, name))).digest("hex");
  const run = "artifacts/hawaii/runs/fixture";
  const tool = { packageId: "Hawaii", version: "1.0.0", packageSha256: "package", payloadSha256: { "Hawaii.dll": "assembly" } };
  write(run + "/schema-original.json", "original"); write(run + "/schema-selected.json", "selected");
  for (const name of ["taxonomy.json", "policy.json", "lifecycle.json", "overlay.json"]) write("generators/hawaii/" + name, name);
  write("generators/hawaii/generate.py", "pipeline");
  const pins = { schema: { sha256: hash(run + "/schema-original.json"), operationCount: 4 },
    pipelineInputs: ["generators/hawaii/generate.py"],
    taxonomy: "taxonomy.json", schemaPolicy: "policy.json", operationPolicy: "lifecycle.json", overlays: ["overlay.json"] };
  json("generators/hawaii/pins.json", pins);
  const provenance = { status: "compiled", pinsSha256: hash("generators/hawaii/pins.json"), hawaiiTool: tool,
    pipelineSources: { "generators/hawaii/generate.py": hash("generators/hawaii/generate.py") },
    schemaOriginalSha256: pins.schema.sha256, schemaSelectedSha256: hash(run + "/schema-selected.json"),
    taxonomySha256: hash("generators/hawaii/taxonomy.json"), schemaPolicySha256: hash("generators/hawaii/policy.json"),
    lifecyclePolicySha256: hash("generators/hawaii/lifecycle.json"),
    overlays: [{ path: "overlay.json", sha256: hash("generators/hawaii/overlay.json") }],
    includedOperations: 3, sourceOperations: 4, unresolvedTaxonomyOperations: 0 };
  json(run + "/provenance.json", provenance);
  const project = "src/Core/Api/Api.fsproj"; write(project, "project");
  json(run + "/output-ownership.json", { schemaVersion: 1, files: { [project]: hash(project) } });
  json("inventory/hawaii-output-ownership.json", { schemaVersion: 1, files: { [project]: hash(project) },
    validation: { run, configuration: "Release", tool, schemaSha256: provenance.schemaSelectedSha256, selectedOperations: 3 } });
  return { root, tool, write, json, provenance, run, project };
}

test("Hawaii resume authenticates the pinned pipeline and owned output", t => {
  const f = fixture(t);
  assert.deepEqual(currentHawaii(f.root, f.tool), { status: "current", run: f.run, selectedOperations: 3,
    projects: [f.project], authenticatedFiles: 1 });
  assert.throws(() => currentHawaii(f.root, { ...f.tool, packageSha256: "changed" }), /generator package changed/);
});

test("Hawaii resume rejects changed policy and edited generated files", t => {
  const f = fixture(t);
  f.write("generators/hawaii/policy.json", "changed policy");
  assert.throws(() => currentHawaii(f.root, f.tool), /schemaPolicy changed/);
  f.write("generators/hawaii/policy.json", "policy.json");
  f.write(f.project, "edited project");
  assert.throws(() => currentHawaii(f.root, f.tool), /owned output changed/);
});

test("Hawaii resume rejects failed generation, schema drift and unresolved operation ownership", t => {
  const f = fixture(t);
  f.json(f.run + "/provenance.json", { ...f.provenance, status: "failed" });
  assert.throws(() => currentHawaii(f.root, f.tool), /accepted generation changed/);
  f.json(f.run + "/provenance.json", { ...f.provenance, unresolvedTaxonomyOperations: 1 });
  assert.throws(() => currentHawaii(f.root, f.tool), /operation coverage/);
  f.json(f.run + "/provenance.json", f.provenance);
  f.write(f.run + "/schema-selected.json", "drifted");
  assert.throws(() => currentHawaii(f.root, f.tool), /schema provenance changed/);
});

test("Hawaii resume rejects changed pipeline code and missing accepted output", t => {
  const f = fixture(t);
  f.write("generators/hawaii/generate.py", "changed pipeline");
  assert.throws(() => currentHawaii(f.root, f.tool), /pipeline sources changed/);
  f.write("generators/hawaii/generate.py", "pipeline");
  const receiptFile = "inventory/hawaii-output-ownership.json";
  const receipt = JSON.parse(readFileSync(path.join(f.root, receiptFile), "utf8"));
  f.json(receiptFile, { ...receipt, files: {} });
  assert.throws(() => currentHawaii(f.root, f.tool), /installed ownership differs/);
});
