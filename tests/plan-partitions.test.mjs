import assert from "node:assert/strict";
import test from "node:test";
import { planPartitions } from "../scripts/plan-partitions.mjs";

const surface = (id, publicSubpath, canonical = true, extra = {}) => ({ id, publicSubpath,
  package: "sdk", version: "1.0.0", entry: publicSubpath === "." ? "index.d.ts" : publicSubpath.slice(2) + ".d.ts",
  productionTarget: true, canonicalGenerationSurface: canonical, ...extra });
const target = (id, generator = {}, extra = {}) => ({ id, package: "sdk", version: "1.0.0",
  profile: "sdk", dependencies: [], generator: { module: `Bindings.${id}`, lib: ["esnext"], types: [], ...generator }, ...extra });
const plan = (targets, surfaces) => planPartitions({ targets }, { surfaces });

test("one program selects canonical and expanded public imports exactly once", () => {
  const surfaces = [surface("sdk--root", "."), surface("sdk--client", "./client"), surface("sdk--card", "./widgets/card", false)];
  const targets = [target("sdk", { entry: "index.d.ts" }), target("sdk--client")];
  const output = plan(targets, surfaces);
  assert.equal(output.inputCount, 3);
  assert.equal(output.partitionCount, 1);
  assert.deepEqual(output.partitions[0].generator.publicInputs, { ".": "index.d.ts", "./client": "client.d.ts", "./widgets/card": "widgets/card.d.ts" });
  assert.equal(output.partitions[0].generator.entry, undefined);
  assert.equal(output.partitions[0].generator.runtime, "sdk");
  assert.deepEqual(plan([...targets].reverse(), [...surfaces].reverse()), output);
});

test("browser, installed profiles, versions and alternative ambient declarations remain independent", () => {
  const surfaces = [surface("root", "."), surface("browser", "./browser"), surface("profile", "./profile"),
    surface("version", "./version", true, { version: "2.0.0" }),
    surface("ambient", "./stable", true, { generationMode: "ambientDeclarations" }),
    surface("experimental", "./experimental", true, { generationMode: "ambientDeclarations" })];
  const targets = [target("root"), target("browser", { lib: ["esnext", "dom"] }), target("profile", {}, { profile: "separate" }),
    target("version", {}, { version: "2.0.0" }), target("ambient"), target("experimental")];
  const output = plan(targets, surfaces);
  assert.equal(output.partitionCount, 6);
  assert.ok(output.partitions.every(partition => partition.inputs.length === 1));
});

test("wildcard inference ambiguity fails instead of assigning browser exports to Workers", () => {
  assert.throws(() => plan([target("root"), target("browser", { lib: ["dom"] })],
    [surface("root", "."), surface("browser", "./browser"), surface("wildcard", "./widgets/card", false)]), /explicit inference partition/);
});

test("rootless input selection keeps the root absent", () => {
  const output = plan([target("client")], [surface("client", "./client")]);
  assert.deepEqual(output.partitions[0].generator.publicInputs, { "./client": "client.d.ts" });
});

test("management SDK and duplicate public inputs fail the partition plan", () => {
  assert.throws(() => plan([{ ...target("rest"), package: "cloudflare" }],
    [surface("rest", ".", true, { package: "cloudflare" })]), /Hawaii/);
  assert.throws(() => plan([target("root"), target("duplicate")],
    [surface("root", "."), surface("duplicate", ".")]), /duplicate public import/);
});
