import path from "node:path";

const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
const object = (value) => value !== null && typeof value === "object" && !Array.isArray(value);
const canonical = (value) => JSON.stringify(value, (_, child) => object(child)
  ? Object.fromEntries(Object.entries(child).sort(([a], [b]) => a < b ? -1 : a > b ? 1 : 0)) : child);
const uniqueIds = (value, label) => {
  requireThat(Array.isArray(value) && value.every((id) => typeof id === "string" && /^[a-z][a-z0-9-]*$/.test(id)) && new Set(value).size === value.length, `${label} must be unique target IDs`);
  return value;
};

/** Match only the explicit inference options fingerprinted by Xantham's catalog. */
export function inferenceProfile(generator) {
  for (const key of ["lib", "types"]) requireThat(generator[key] === undefined || Array.isArray(generator[key]) && generator[key].every((entry) => typeof entry === "string" && entry.length > 0), `${key} must be an array of nonempty strings`);
  return canonical({ lib: generator.lib === undefined ? null : [...generator.lib].sort(),
    types: generator.types === undefined ? null : [...generator.types].sort(),
    groups: generator.groups ?? {}, resolveNoInfer: generator.resolveNoInfer ?? false });
}

export function catalogCandidates(target) {
  return uniqueIds(target.catalogCandidates === undefined ? [] : target.catalogCandidates, `${target.id}: catalogCandidates`);
}

export function generationDependencies(target) {
  return [...new Set([...uniqueIds(target.dependencies, `${target.id}: dependencies`), ...catalogCandidates(target)])];
}

/** Candidate catalogs are generation inputs; they do not imply F# project references. */
export function generationOrder(targets, requested = []) {
  const byId = new Map(targets.map((target) => [target.id, target]));
  requireThat(byId.size === targets.length, "Target IDs must be unique");
  for (const target of targets) {
    for (const id of generationDependencies(target)) requireThat(byId.has(id), `${target.id}: unknown dependency ${id}`);
    for (const id of catalogCandidates(target)) {
      const producer = byId.get(id);
      requireThat(target.generator.declarationCatalog === true && producer.generator.declarationCatalog === true, `${target.id}: catalog candidates must emit declaration catalogs`);
      requireThat(target.profile === producer.profile && target.package === producer.package && target.version === producer.version,
        `${target.id}: catalog candidate ${id} crosses the package/version/profile ownership boundary`);
      requireThat(inferenceProfile(target.generator) === inferenceProfile(producer.generator), `${target.id}: catalog candidate ${id} uses a different inference profile`);
    }
  }
  const visited = new Set();
  const visiting = [];
  const ordered = [];
  function visit(id) {
    requireThat(byId.has(id), `Unknown target: ${id}`);
    requireThat(!visiting.includes(id), `Cyclic target dependencies: ${[...visiting, id].join(" -> ")}`);
    if (visited.has(id)) return;
    visiting.push(id);
    generationDependencies(byId.get(id)).forEach(visit);
    visiting.pop();
    visited.add(id);
    ordered.push(byId.get(id));
  }
  // Validate the entire graph, including cycles outside a selected closure.
  targets.forEach((target) => visit(target.id));
  if (!requested.length) return ordered;
  visited.clear(); ordered.length = 0;
  requested.forEach(visit);
  return ordered;
}

/** Resolve all references from the original profile, even when generation uses a snapshot. */
export function declarationReferences(target, root, originalPackageDirectory) {
  const configured = target.generator.declarationReferences ?? [];
  requireThat(Array.isArray(configured) && configured.every((file) => typeof file === "string" && file.trim().length > 0) && new Set(configured).size === configured.length,
    `${target.id}: declarationReferences must be unique nonempty paths`);
  const references = configured.map((key) => ({ key, file: path.resolve(originalPackageDirectory, key) }));
  for (const id of catalogCandidates(target)) {
    const file = path.join(root, "artifacts", id, "declarations.json");
    if (!references.some((reference) => reference.file === file)) references.push({ key: path.relative(originalPackageDirectory, file), file });
  }
  return references;
}

/** Read the current owner's actual dependencies, not all inherited candidate declarations. */
export function catalogProjectDependencies(catalog, target, targets) {
  requireThat(object(catalog) && catalog.schemaVersion === 1 && catalog.owner === target.generator.module && Array.isArray(catalog.owners), `${target.id}: declaration catalog owner differs from target`);
  const owners = new Map();
  for (const owner of catalog.owners) {
    requireThat(object(owner) && typeof owner.name === "string" && owner.name.length > 0 && !owners.has(owner.name)
      && Array.isArray(owner.dependencies) && owner.dependencies.every((name) => typeof name === "string" && name.length > 0)
      && new Set(owner.dependencies).size === owner.dependencies.length, `${target.id}: invalid declaration catalog owner graph`);
    owners.set(owner.name, owner.dependencies);
  }
  const visited = new Set();
  function visit(name, ancestors = []) {
    requireThat(!ancestors.includes(name), `${target.id}: declaration catalog owner cycle: ${[...ancestors, name].join(" -> ")}`);
    requireThat(owners.has(name), `${target.id}: declaration catalog is missing owner ${name}`);
    if (visited.has(name)) return;
    owners.get(name).forEach((dependency) => visit(dependency, [...ancestors, name]));
    visited.add(name);
  }
  owners.forEach((_, name) => visit(name));
  requireThat(owners.has(catalog.owner), `${target.id}: declaration catalog is missing its current owner`);
  const modules = new Map(targets.map((entry) => [entry.generator.module, entry.id]));
  requireThat(modules.size === targets.length, "Catalog project ownership requires unique target modules");
  return owners.get(catalog.owner).map((owner) => {
    requireThat(modules.has(owner), `${target.id}: declaration catalog uses unknown project owner ${owner}`);
    return modules.get(owner);
  }).sort();
}
