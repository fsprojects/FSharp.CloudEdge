import { readFileSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { inferenceProfile } from "./catalogs.mjs";

const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
const compare = (a, b) => a < b ? -1 : a > b ? 1 : 0;
const surfaceOrder = (a, b) => a.publicSubpath.split("/").length - b.publicSubpath.split("/").length
  || compare(a.publicSubpath, b.publicSubpath) || compare(a.entry, b.entry);

/** Plan exact public inputs while retaining the installed package and inference boundaries. */
export function planPartitions(config, catalog) {
  const targets = new Map(config.targets.map(target => [target.id, target]));
  const canonical = new Map();
  const selected = catalog.surfaces.filter(surface => surface.productionTarget);
  const packageKey = surface => JSON.stringify([surface.package, surface.version]);
  for (const surface of selected.filter(surface => surface.canonicalGenerationSurface)) {
    const target = targets.get(surface.id.replace(/--root$/, ""));
    requireThat(target && target.package === surface.package && target.version === surface.version,
      `${surface.id}: missing canonical pinned target`);
    const key = packageKey(surface);
    if (!canonical.has(key)) canonical.set(key, []);
    canonical.get(key).push({ surface, target });
  }
  const partitions = new Map();
  const assigned = new Set();
  for (const surface of selected) {
    requireThat(surface.package !== "cloudflare", "Management SDK belongs to Hawaii's OpenAPI pipeline");
    requireThat(!assigned.has(surface.id), `${surface.id}: duplicate inventory input`);
    assigned.add(surface.id);
    const candidates = canonical.get(packageKey(surface)) ?? [];
    const direct = candidates.find(candidate => candidate.surface.id === surface.id);
    const alternatives = new Map(candidates.map(candidate => [JSON.stringify([
      candidate.target.profile, inferenceProfile(candidate.target.generator),
      candidate.surface.generationMode === "ambientDeclarations" ? candidate.surface.id : null,
    ]), candidate]));
    requireThat(direct || alternatives.size === 1,
      `${surface.id}: wildcard input needs an explicit inference partition`);
    const source = direct ?? [...alternatives.values()][0];
    const target = source.target;
    const key = JSON.stringify([surface.package, surface.version, target.profile,
      inferenceProfile(target.generator), surface.generationMode === "ambientDeclarations" ? surface.id : null]);
    if (!partitions.has(key)) partitions.set(key, []);
    partitions.get(key).push({ surface, target });
  }
  const result = [...partitions.values()].map(entries => {
    entries.sort((a, b) => surfaceOrder(a.surface, b.surface));
    const owner = entries.find(entry => entry.surface.canonicalGenerationSurface);
    requireThat(owner, "Partition has no canonical owner");
    const target = owner.target;
    const publicInputs = {};
    for (const { surface } of entries) {
      requireThat(!Object.hasOwn(publicInputs, surface.publicSubpath), `${surface.id}: duplicate public import`);
      const expectedRuntime = surface.package + (surface.publicSubpath === "." ? "" : surface.publicSubpath.slice(1));
      requireThat(!surface.runtime || surface.runtime === expectedRuntime, `${surface.id}: runtime prefix requires an explicit partition`);
      publicInputs[surface.publicSubpath] = surface.entry;
    }
    const generator = { ...target.generator, runtime: target.package, publicInputs, declarationCatalog: true };
    delete generator.entry;
    delete generator.subpaths;
    delete generator.declarationReferences;
    const references = [...new Set(entries.flatMap(entry => entry.target.generator.declarationReferences ?? []))];
    requireThat(!references.length, `${target.id}: explicit declaration references require an ownership migration`);
    requireThat(entries.every(entry => entry.target.dependencies.length === 0), `${target.id}: project dependencies require an ownership migration`);
    return {
      id: target.id, package: target.package, version: target.version, profile: target.profile,
      generator, inputs: entries.map(({ surface }) => ({ id: surface.id, publicSubpath: surface.publicSubpath,
        entry: surface.entry, canonical: !!surface.canonicalGenerationSurface, evidence: surface.evidence })),
    };
  }).sort((a, b) => compare(a.id, b.id));
  requireThat(new Set(result.map(partition => partition.id)).size === result.length, "Partition owner IDs must be unique");
  return { schemaVersion: 1, status: "planned-not-accepted", sourceInventory: catalog.sourceInventory,
    sourceInventorySha256: catalog.sourceInventorySha256, inputCount: assigned.size,
    partitionCount: result.length, partitions: result };
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
  const read = file => JSON.parse(readFileSync(path.join(root, file), "utf8"));
  const plan = planPartitions(read("config/targets.json"), read("config/sdk-surfaces.json"));
  const output = process.argv[2] ?? "artifacts/sdk-partitions-plan.json";
  writeFileSync(path.resolve(root, output), JSON.stringify(plan, null, 2) + "\n");
  console.log(`Planned ${plan.inputCount} explicit inputs in ${plan.partitionCount} compiler programs; generation and acceptance are pending.`);
}
