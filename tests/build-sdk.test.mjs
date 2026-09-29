import assert from "node:assert/strict";
import { mkdtempSync, mkdirSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import test from "node:test";
import { buildSdk, partitionConfiguration } from "../scripts/build-sdk.mjs";
import { planPartitions } from "../scripts/plan-partitions.mjs";
import { planProjects, reconcileProjects, writeProjects } from "../scripts/projects.mjs";

function fixture(t) {
  const root = mkdtempSync(path.join(os.tmpdir(), "cloudedge-sdk-build-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  mkdirSync(path.join(root, "config"));
  const target = (id, subpath, lib) => ({ id, package: "sdk", version: "1.0.0", profile: "sdk",
    dependencies: [], outputDirectory: `src/${id}`, files: [`Bindings.${id}.fs`],
    generator: { module: `Bindings.${id}`, entry: `${subpath}.d.ts`, lib, types: [] } });
  const config = { schemaVersion: 2, toolchain: { typescript: "7.1.0-dev.20260902.1" },
    xantham: { tool: "xantham" }, profiles: [{ id: "sdk", directory: "profiles/sdk" }],
    projectDefaults: { targetFramework: "net8.0", packages: { "Fable.Core": "5.2.0", "Xantham.Fable.Core": "0.1.0", "Xantham.Fable.Core.TS": "0.1.0" } },
    targets: [target("sdk", "index", ["esnext"]), target("extra", "extra", ["esnext"]), target("browser", "browser", ["esnext", "dom"])] };
  const surface = (id, subpath, entry) => ({ id, package: "sdk", version: "1.0.0", publicSubpath: subpath,
    entry, productionTarget: true, canonicalGenerationSurface: true });
  const catalog = { surfaces: [surface("sdk--root", ".", "index.d.ts"), surface("extra", "./extra", "extra.d.ts"), surface("browser", "./browser", "browser.d.ts")] };
  writeFileSync(path.join(root, "config/targets.json"), JSON.stringify(config));
  writeFileSync(path.join(root, "config/sdk-surfaces.json"), JSON.stringify(catalog));
  const report = () => JSON.parse(readFileSync(path.join(root, "artifacts/sdk-build/latest.json")));
  return { root, config, catalog, report };
}

test("partition materialization preserves explicit group file order and separates browser inference", t => {
  const f = fixture(t);
  f.config.targets[0].files.unshift("groups/Support.fs");
  const output = partitionConfiguration(f.config, planPartitions(f.config, f.catalog));
  assert.equal(output.targets.length, 2);
  const sdk = output.targets.find(target => target.id === "sdk");
  assert.deepEqual(sdk.files, ["groups/Support.fs", "Bindings.sdk.fs"]);
  assert.deepEqual(sdk.generator.publicInputs, { ".": "index.d.ts", "./extra": "extra.d.ts" });
  assert.equal(sdk.generator.entry, undefined);
  assert.equal(f.config.targets.length, 3);
});

test("plan-only mode records selected concrete inputs while retaining the full inventory", async t => {
  const f = fixture(t);
  const before = readFileSync(path.join(f.root, "config/targets.json"), "utf8");
  const report = await buildSdk({ root: f.root, args: ["--plan", "sdk"], generateBindings: () => assert.fail("plan invoked generation") });
  assert.equal(report.status, "planned");
  assert.equal(report.inventoryInputs, 3);
  assert.deepEqual(report.selectedInputs.map(input => input.publicSubpath), [".", "./extra"]);
  assert.deepEqual(report.compiled, []);
  assert.equal(readFileSync(path.join(f.root, "config/targets.json"), "utf8"), before);
});

test("generation receives the exact partition configuration and failures prevent compilation", async t => {
  const f = fixture(t);
  await assert.rejects(buildSdk({ root: f.root, args: ["--resume", "sdk"],
    generateBindings: async ({ configuration, args }) => {
      assert.deepEqual(args, ["--resume", "sdk"]);
      assert.deepEqual(configuration.targets.find(target => target.id === "sdk").generator.publicInputs,
        { ".": "index.d.ts", "./extra": "extra.d.ts" });
      return [{ target: "sdk", status: "failed" }];
    }, compileBindings: () => assert.fail("failed generation reached compilation") }), /did not all generate/);
  assert.equal(f.report().status, "failed");
  assert.equal(f.report().failedStage, "generating");
});

test("all mode includes browser partitions and unknown selections fail before generation", async t => {
  const f = fixture(t);
  const report = await buildSdk({ root: f.root, args: ["--all", "--plan"] });
  assert.equal(report.selectedInputs.length, 3);
  assert.equal(report.requested.length, 2);
  await assert.rejects(buildSdk({ root: f.root, args: ["missing"] }), /Unknown target/);
  await assert.rejects(buildSdk({ root: f.root, args: ["--all", "sdk"] }), /cannot be combined/);
});

test("catalog reconciliation preserves the partition solution destination", t => {
  const f = fixture(t);
  const config = partitionConfiguration(f.config, planPartitions(f.config, f.catalog));
  const plan = planProjects(config, f.root);
  plan.solution = path.join(f.root, "SDK.Partitions.slnx");
  assert.equal(reconcileProjects(plan).solution, plan.solution);
  writeProjects(plan);
  assert.equal(readFileSync(plan.solution, "utf8"), plan.solutionContent);
});
