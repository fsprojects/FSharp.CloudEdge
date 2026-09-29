import assert from "node:assert/strict";
import test from "node:test";
import { mkdirSync, mkdtempSync, rmSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import { generationOrder, declarationReferences } from "../scripts/catalogs.mjs";
import { planSdk, scheduleCatalogs } from "../scripts/plan-sdk.mjs";

const target = (id, generator = {}, profile = "sdk") => ({ id, package: "sdk", version: "1.2.3", profile, dependencies: [], generator: { module: id, lib: ["esnext"], types: [], ...generator } });

test("same-package entries inherit a stable linear catalog chain, root first", () => {
  const input = [target("z-root"), target("c"), target("a"), target("b")];
  const planned = scheduleCatalogs(input, new Set(["z-root"]));
  assert.deepEqual(planned.map((entry) => [entry.id, entry.catalogCandidates ?? []]), [["z-root", []], ["c", ["b"]], ["a", ["z-root"]], ["b", ["a"]]]);
  assert.deepEqual(generationOrder(planned, ["c"]).map((entry) => entry.id), ["z-root", "a", "b", "c"]);
  assert.ok(planned.every((entry) => entry.generator.declarationCatalog && entry.dependencies.length === 0));
  assert.deepEqual(scheduleCatalogs(planned, new Set(["z-root"])), planned);
  assert.ok(input.every((entry) => entry.generator.declarationCatalog === undefined));
});

test("catalog scheduling keeps every inference and installed-profile boundary explicit", () => {
  const input = [target("root"), target("same"), target("dom", { lib: ["esnext", "dom"] }),
    target("node", { types: ["node"] }), target("noinfer", { resolveNoInfer: true }),
    target("groups", { groups: { typescript: "skip" } }), target("profile", {}, "other"),
    { ...target("version"), version: "2.0.0" }, { ...target("package"), package: "other" }];
  const planned = scheduleCatalogs(input, new Set(["root"]));
  assert.deepEqual(planned.filter((entry) => entry.catalogCandidates).map((entry) => entry.id), ["same"]);
  assert.deepEqual(planned.map((entry) => entry.generator.lib), input.map((entry) => entry.generator.lib));
  assert.deepEqual(planned.map((entry) => entry.generator.types), input.map((entry) => entry.generator.types));
});

test("inference option order is normalized but default and explicit ambient lists stay distinct", () => {
  const input = [target("a", { lib: ["esnext", "dom"], types: ["node", "workers"] }), target("b", { lib: ["dom", "esnext"], types: ["workers", "node"] }), target("c", { types: undefined })];
  assert.deepEqual(scheduleCatalogs(input).map((entry) => entry.catalogCandidates ?? []), [[], ["a"], []]);
});

test("alternative ambient declaration inputs have independent catalog owners", () => {
  const input = [target("stable", { declarationCatalog: true }), target("experimental", { declarationCatalog: true })];
  const planned = scheduleCatalogs(input, new Set(["stable"]), new Set(["stable", "experimental"]));
  assert.ok(planned.every((entry) => entry.catalogCandidates === undefined));
  assert.deepEqual(generationOrder(planned, ["experimental"]).map((entry) => entry.id), ["experimental"]);
});

for (const [name, change, error] of [
  ["unknown candidate", (a) => { a.catalogCandidates = ["missing"]; }, /unknown dependency/],
  ["duplicate candidates", (a) => { a.catalogCandidates = ["b", "b"]; }, /unique target IDs/],
  ["disabled catalog", (a) => { a.generator.declarationCatalog = false; }, /must emit/],
  ["cross-profile candidate", (a) => { a.profile = "other"; }, /ownership boundary/],
  ["different inference", (a) => { a.generator.types = ["node"]; }, /different inference profile/],
  ["cycle outside selected closure", (_, b) => { b.catalogCandidates = ["a"]; }, /Cyclic target dependencies/],
]) test(`generation graph rejects ${name}`, () => {
  const a = target("a", { declarationCatalog: true }); a.catalogCandidates = ["b"];
  const b = target("b", { declarationCatalog: true });
  change(a, b);
  assert.throws(() => generationOrder([a, b, target("unrelated")], ["unrelated"]), error);
});

test("derived catalog paths use the original package directory and deduplicate explicit references", () => {
  const entry = target("consumer", { declarationReferences: ["../../../../artifacts/root/declarations.json"] });
  entry.catalogCandidates = ["root"];
  assert.deepEqual(declarationReferences(entry, "/repo", "/repo/profiles/sdk/node_modules/sdk"), [{ key: "../../../../artifacts/root/declarations.json", file: "/repo/artifacts/root/declarations.json" }]);
});

test("SDK planning preserves explicit profiles, ambient choices and dependencies when a package has multiple installations", (t) => {
  const root = mkdtempSync(path.join(os.tmpdir(), "cloudedge-sdk-plan-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  const profiles = ["workers", "browser"].map((id) => ({ id, directory: `profiles/${id}` }));
  for (const profile of profiles) {
    const directory = path.join(root, profile.directory);
    mkdirSync(path.join(directory, "node_modules/sdk"), { recursive: true });
    writeFileSync(path.join(directory, "package.json"), JSON.stringify({ devDependencies: { sdk: "1.2.3" } }));
    writeFileSync(path.join(directory, "node_modules/sdk/index.d.ts"), "export {};\n");
  }
  const targets = [target("one", { module: "Sdk.One", lib: ["esnext"], types: ["workers", "node"], groups: { dependency: "skip" }, resolveNoInfer: true }, "workers"),
    target("two", { module: "Sdk.Two", lib: ["esnext", "dom"], types: ["node"] }, "browser")]
    .map((entry) => ({ ...entry, outputDirectory: `src/Runtime/${entry.generator.module}`, files: [`${entry.generator.module}.fs`] }));
  targets[1].dependencies = ["one"];
  const config = { schemaVersion: 2, toolchain: { typescript: "7.1.0-dev.20260902.1" }, xantham: {}, profiles,
    projectDefaults: { targetFramework: "net8.0", packages: { "Fable.Core": "5.2.0", "Xantham.Fable.Core": "0.1.0-alpha.1" } }, targets };
  const catalog = { packages: [{ id: "sdk" }], surfaces: targets.map((entry, index) => ({ id: entry.id, packageId: "sdk", package: "sdk", version: "1.2.3", module: entry.generator.module,
    entry: "index.d.ts", logicalArea: "Runtime", publicSubpath: index ? "./react" : ".", generationRole: index ? "subentry" : "root-owner", productionTarget: true, canonicalGenerationSurface: true })) };
  const inventory = { packages: [{ name: "sdk", version: "1.2.3", environments: ["workerd", "browser"] }] };
  const planned = planSdk(config, catalog, inventory, root);
  assert.deepEqual(planned.targets.map((entry) => entry.profile), ["workers", "browser"]);
  assert.deepEqual(planned.targets.map((entry) => entry.dependencies), [[], ["one"]]);
  for (const key of ["lib", "types", "groups", "resolveNoInfer"]) assert.deepEqual(planned.targets.map((entry) => entry.generator[key]), targets.map((entry) => entry.generator[key]));
  assert.ok(planned.targets.every((entry) => entry.catalogCandidates === undefined));
  catalog.surfaces.push({ ...catalog.surfaces[1], id: "three", module: "Sdk.Three" });
  assert.throws(() => planSdk(config, catalog, inventory, root), /Ambiguous pinned input profiles/);
});
