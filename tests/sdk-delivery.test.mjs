import assert from "node:assert/strict";
import path from "node:path";
import test from "node:test";
import { deliveryConfiguration } from "../scripts/sdk-delivery.mjs";

function fixture() {
  const inputs = paths => paths.map(publicSubpath => ({ id: "sdk" + publicSubpath,
    publicSubpath, entry: publicSubpath === "." ? "dist/index.d.ts" : `dist/${publicSubpath.slice(2)}.d.ts` }));
  const partitions = [{ id: "sdk", package: "sdk", version: "1.0.0", inputs: inputs([".", "./react"]) },
    { id: "mobile", package: "mobile", version: "1.0.0", inputs: [{ id: "mobile", publicSubpath: ".", entry: "index.d.ts" }] }];
  const target = (id, pkg = id) => ({ id, package: pkg, version: "1.0.0", profile: "sdk", dependencies: [],
    outputDirectory: `src/Runtime/${id}`, files: [`Bindings.${id}.fs`],
    generator: { module: `Bindings.${id}`, declarationCatalog: true, lib: ["esnext"], types: [],
      publicInputs: { ".": "dist/index.d.ts" } } });
  const configuration = { profiles: [{ id: "sdk", directory: "profiles/sdk" }], targets: [target("sdk"), target("mobile")] };
  const delivery = { schemaVersion: 1, name: "server", libraries: [{ partition: "sdk", publicInputs: ["."],
    excludeInputs: { "./react": "Client UI is outside this delivery." } }],
  deferred: [{ partitions: ["mobile"], reason: "Mobile clients remain community work." }],
  supportTargets: [], catalogImports: {} };
  return { configuration, partitions, delivery, target };
}

test("delivery selects exact server inputs and keeps a reasoned disposition for every inventory input", () => {
  const f = fixture();
  const before = JSON.stringify(f.configuration);
  const result = deliveryConfiguration(f.configuration, f.partitions, f.delivery, "/repo");
  assert.deepEqual(result.configuration.targets[0].generator.publicInputs, { ".": "dist/index.d.ts" });
  assert.equal(result.scope.selectedInputs, 1);
  assert.equal(result.scope.deferredInputs, 2);
  assert.equal(result.scope.dispositions.length, 3);
  assert.equal(JSON.stringify(f.configuration), before);
  f.partitions[0].inputs.push({ id: "new-browser", publicSubpath: "./browser", entry: "dist/browser.d.ts" });
  assert.throws(() => deliveryConfiguration(f.configuration, f.partitions, f.delivery, "/repo"), /every input needs/);
});

test("catalog injection produces an ordered support dependency with a relocatable catalog path", () => {
  const f = fixture();
  f.delivery.supportTargets = [f.target("ai-v4", "@ai-sdk/provider")];
  f.delivery.catalogImports = { sdk: ["ai-v4"] };
  const { configuration } = deliveryConfiguration(f.configuration, f.partitions, f.delivery, "/repo");
  assert.deepEqual(configuration.targets.map(target => target.id), ["ai-v4", "sdk"]);
  const consumer = configuration.targets[1];
  assert.deepEqual(consumer.dependencies, ["ai-v4"]);
  assert.equal(path.resolve("/repo/profiles/sdk/node_modules/sdk", consumer.generator.declarationReferences[0]),
    "/repo/artifacts/ai-v4/declarations.json");
  f.delivery.catalogImports["ai-v4"] = ["sdk"];
  assert.throws(() => deliveryConfiguration(f.configuration, f.partitions, f.delivery, "/repo"), /Cyclic target dependencies/);
});

test("server library rename preserves exact source selection and changes its assembly destination", () => {
  const f = fixture();
  Object.assign(f.delivery.libraries[0], { id: "mcp-client", module: "Bindings.McpClient", outputDirectory: "src/Runtime/Agents/McpClient" });
  const result = deliveryConfiguration(f.configuration, f.partitions, f.delivery, "/repo");
  const target = result.configuration.targets[0];
  assert.equal(target.id, "mcp-client");
  assert.deepEqual(target.files, ["Bindings.McpClient.fs"]);
  assert.equal(target.outputDirectory, "src/Runtime/Agents/McpClient");
  assert.equal(result.libraries[0].partition, "sdk");
  assert.equal(result.scope.dispositions[0].library, "mcp-client");
});

test("delivery rejects unknown catalog owners, unexplained omissions and input override attempts", () => {
  const f = fixture();
  f.delivery.catalogImports = { sdk: ["missing"] };
  assert.throws(() => deliveryConfiguration(f.configuration, f.partitions, f.delivery, "/repo"), /catalog producer/);
  f.delivery.catalogImports = {};
  f.delivery.deferred = [];
  assert.throws(() => deliveryConfiguration(f.configuration, f.partitions, f.delivery, "/repo"), /explicit deferred disposition/);
  f.delivery.deferred = [{ partitions: ["mobile"], reason: "Deferred." }];
  f.delivery.targetOverrides = { sdk: { generator: { publicInputs: { "./react": "dist/react.d.ts" } } } };
  assert.throws(() => deliveryConfiguration(f.configuration, f.partitions, f.delivery, "/repo"), /not input selection or ownership/);
});
