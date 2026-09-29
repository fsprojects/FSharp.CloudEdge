import path from "node:path";
import { generationOrder } from "./catalogs.mjs";

const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
const object = value => value !== null && typeof value === "object" && !Array.isArray(value);
const slash = value => value.split(path.sep).join("/");

/** Select public SDK inputs and inject explicitly configured dependency catalog owners. */
export function deliveryConfiguration(configuration, partitions, delivery, root) {
  requireThat(delivery.schemaVersion === 1 && typeof delivery.name === "string"
    && Array.isArray(delivery.libraries) && delivery.libraries.length > 0,
  "SDK delivery must name a schema-1 library selection");
  const allowed = ["schemaVersion", "name", "description", "libraries", "deferred", "supportTargets", "catalogImports", "targetOverrides"];
  requireThat(Object.keys(delivery).every(key => allowed.includes(key)), "SDK delivery contains an unknown field");
  const byPartition = new Map(partitions.map(partition => [partition.id, partition]));
  const byTarget = new Map(configuration.targets.map(target => [target.id, target]));
  const profiles = new Map(configuration.profiles.map(profile => [profile.id, profile]));
  const chosen = new Map();
  const libraries = [];
  const dispositions = [];
  const targets = delivery.libraries.map(selection => {
    requireThat(object(selection) && typeof selection.partition === "string"
      && Object.keys(selection).every(key => ["partition", "id", "module", "outputDirectory", "publicInputs", "excludeInputs"].includes(key)),
    "Each delivery library must select a known partition");
    const partition = byPartition.get(selection.partition);
    const original = byTarget.get(selection.partition);
    requireThat(partition && original && !chosen.has(partition.id), `Unknown or repeated delivery partition: ${selection.partition}`);
    chosen.set(partition.id, selection);
    const excluded = selection.excludeInputs ?? {};
    requireThat(Array.isArray(selection.publicInputs) && selection.publicInputs.length > 0
      && new Set(selection.publicInputs).size === selection.publicInputs.length
      && selection.publicInputs.every(subpath => partition.inputs.some(input => input.publicSubpath === subpath)),
    `${partition.id}: publicInputs must explicitly select distinct existing public imports`);
    requireThat(object(excluded), `${partition.id}: excludeInputs must map public imports to reasons`);
    for (const [subpath, reason] of Object.entries(excluded)) {
      requireThat(partition.inputs.some(input => input.publicSubpath === subpath) && !selection.publicInputs.includes(subpath)
        && typeof reason === "string" && reason.trim().length > 0,
      `${partition.id}: exclusion must name an existing input and explain its disposition: ${subpath}`);
    }
    requireThat(partition.inputs.every(input => selection.publicInputs.includes(input.publicSubpath)
      || Object.hasOwn(excluded, input.publicSubpath)), `${partition.id}: every input needs a selected or deferred disposition`);
    const inputs = partition.inputs.filter(input => selection.publicInputs.includes(input.publicSubpath));
    requireThat(inputs.length > 0, `${partition.id}: delivery library cannot have zero inputs`);
    const id = selection.id ?? original.id;
    const generator = { ...original.generator,
      publicInputs: Object.fromEntries(inputs.map(input => [input.publicSubpath, input.entry])) };
    let files = original.files;
    if (selection.module) {
      generator.module = selection.module;
      files = files.map(file => file === original.generator.module + ".fs" ? selection.module + ".fs" : file);
    }
    for (const input of partition.inputs) dispositions.push({ id: input.id, partition: partition.id,
      publicSubpath: input.publicSubpath, status: Object.hasOwn(excluded, input.publicSubpath) ? "deferred" : "selected",
      ...(Object.hasOwn(excluded, input.publicSubpath) ? { reason: excluded[input.publicSubpath] } : { library: id }) });
    libraries.push({ id, partition: partition.id, package: partition.package, version: partition.version, inputs });
    return { ...original, id, generator, files, outputDirectory: selection.outputDirectory ?? original.outputDirectory };
  });
  const deferred = new Map();
  for (const group of delivery.deferred ?? []) {
    requireThat(object(group) && Array.isArray(group.partitions) && typeof group.reason === "string" && group.reason.trim(),
      "Deferred partitions require a reason");
    for (const id of group.partitions) {
      requireThat(byPartition.has(id) && !chosen.has(id) && !deferred.has(id), `Unknown, selected or repeated deferred partition: ${id}`);
      deferred.set(id, group.reason);
    }
  }
  for (const partition of partitions) {
    if (chosen.has(partition.id)) continue;
    requireThat(deferred.has(partition.id), `${partition.id}: delivery scope requires an explicit deferred disposition`);
    dispositions.push(...partition.inputs.map(input => ({ id: input.id, partition: partition.id,
      publicSubpath: input.publicSubpath, status: "deferred", reason: deferred.get(partition.id) })));
  }
  requireThat(Array.isArray(delivery.supportTargets ?? []), "supportTargets must be configured generation targets");
  targets.push(...structuredClone(delivery.supportTargets ?? []));
  const byId = new Map(targets.map(target => [target.id, target]));
  requireThat(byId.size === targets.length, "SDK delivery and support target IDs must be unique");
  for (const [id, override] of Object.entries(delivery.targetOverrides ?? {})) {
    requireThat(byId.has(id) && object(override)
      && Object.keys(override).every(key => ["profile", "files", "generator"].includes(key)),
    `Invalid SDK delivery override: ${id}`);
    const target = byId.get(id);
    if (override.profile !== undefined) target.profile = override.profile;
    if (override.files !== undefined) target.files = override.files;
    if (override.generator !== undefined) {
      requireThat(object(override.generator)
        && Object.keys(override.generator).every(key => ["lib", "types", "groups", "namespace", "resolveNoInfer"].includes(key)),
      `${id}: generator overrides may change inference settings, not input selection or ownership`);
      target.generator = { ...target.generator, ...override.generator };
    }
  }
  for (const [id, producers] of Object.entries(delivery.catalogImports ?? {})) {
    requireThat(byId.has(id) && Array.isArray(producers) && new Set(producers).size === producers.length,
      `Invalid catalog injection target: ${id}`);
    const target = byId.get(id);
    requireThat(profiles.has(target.profile), `${id}: unknown input profile`);
    const packageDirectory = path.join(root, profiles.get(target.profile).directory, "node_modules", target.package);
    const references = producers.map(producer => {
      requireThat(byId.has(producer) && producer !== id && byId.get(producer).generator.declarationCatalog === true,
        `${id}: catalog producer must name a distinct configured owner: ${producer}`);
      return slash(path.relative(packageDirectory, path.join(root, "artifacts", producer, "declarations.json")));
    });
    target.dependencies = [...new Set([...target.dependencies, ...producers])];
    target.generator = { ...target.generator, declarationReferences: references };
  }
  const result = { ...configuration, targets: generationOrder(targets) };
  return { configuration: result, libraries, scope: { schemaVersion: 1, name: delivery.name,
    selectedInputs: dispositions.filter(input => input.status === "selected").length,
    deferredInputs: dispositions.filter(input => input.status === "deferred").length,
    libraries: libraries.map(({ inputs, ...library }) => ({ ...library, inputCount: inputs.length })),
    supportTargets: (delivery.supportTargets ?? []).map(target => target.id), dispositions } };
}
