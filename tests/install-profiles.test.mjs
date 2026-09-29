import assert from "node:assert/strict";
import test from "node:test";
import { selectProfiles } from "../scripts/install-profiles.mjs";

function fixture() {
  const profiles = ["workers", "agents", "containers", "models", "unused-models", "browser"]
    .map(id => ({ id, directory: `profiles/${id}` }));
  const target = (id, profile = id) => ({ id, package: id, version: "1.0.0", profile,
    dependencies: [], outputDirectory: `src/${id}`, files: [`Bindings.${id}.fs`],
    generator: { module: `Bindings.${id}`, entry: "index.d.ts", lib: ["esnext"], types: [], declarationCatalog: true } });
  const config = { profiles, targets: [target("workers"), target("agents"), target("browser")] };
  const catalog = { surfaces: config.targets.map(item => ({ id: item.id + "--root", package: item.package,
    version: item.version, publicSubpath: ".", entry: "index.d.ts", productionTarget: true,
    canonicalGenerationSurface: true })) };
  const delivery = { schemaVersion: 1, name: "server",
    libraries: ["workers", "agents"].map(partition => ({ partition, publicInputs: ["."] })),
    deferred: [{ partitions: ["browser"], reason: "Browser adapters remain deferred." }],
    supportTargets: [target("provider", "models"), target("unused-provider", "unused-models")],
    catalogImports: { agents: ["provider"] },
    targetOverrides: { workers: { profile: "containers" } } };
  return { config, catalog, delivery, repositoryRoot: "/clean-checkout" };
}

const ids = profiles => profiles.map(profile => profile.id);

test("profile installation retains the broad default and explicit profile selection", () => {
  const f = fixture();
  assert.deepEqual(selectProfiles(f.config), f.config.profiles);
  assert.deepEqual(ids(selectProfiles(f.config, ["containers", "agents", "agents"])), ["agents", "containers"]);
  assert.throws(() => selectProfiles(f.config, ["missing"]), /Unknown dependency profile: missing/);
});

test("delivery profiles include effective overrides and reachable generated support owners only", () => {
  const f = fixture();
  const before = JSON.stringify(f);
  assert.deepEqual(ids(selectProfiles(f.config, ["--delivery"], f)), ["agents", "containers", "models"]);
  assert.equal(JSON.stringify(f), before, "selection leaves pins and delivery configuration unchanged");
});

test("delivery profile selection rejects mixed, repeated and unknown options", () => {
  const f = fixture();
  for (const args of [["--delivery", "agents"], ["agents", "--delivery"], ["--delivery", "--delivery"]])
    assert.throws(() => selectProfiles(f.config, args, f), /must be used once and cannot be combined/);
  assert.throws(() => selectProfiles(f.config, ["--all"], f), /Unknown install option: --all/);
  assert.throws(() => selectProfiles(f.config, ["--delivery", "--unknown"], f), /Unknown install option: --unknown/);
});

test("delivery profile selection fails before installation when metadata or an effective profile is missing", () => {
  const f = fixture();
  assert.throws(() => selectProfiles(f.config, ["--delivery"]), /requires the SDK surface and delivery configurations/);
  f.delivery.targetOverrides.workers.profile = "missing";
  assert.throws(() => selectProfiles(f.config, ["--delivery"], f), /Unknown dependency profile in delivery: missing/);
});
