import { existsSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { planProjects } from "./projects.mjs";
import { generationOrder, inferenceProfile } from "./catalogs.mjs";

/** Chain catalogs only within an exact installed package and inference partition. */
export function scheduleCatalogs(targets, rootOwners = new Set(), independentInputs = new Set()) {
  const partitions = new Map();
  const result = targets.map((target) => {
    const copy = { ...target, generator: { ...target.generator } };
    delete copy.catalogCandidates;
    const key = JSON.stringify([target.package, target.version, target.profile, inferenceProfile(target.generator), independentInputs.has(target.id) ? target.id : null]);
    if (!partitions.has(key)) partitions.set(key, []);
    partitions.get(key).push(copy);
    return copy;
  });
  for (const entries of partitions.values()) {
    entries.sort((a, b) => Number(rootOwners.has(b.id)) - Number(rootOwners.has(a.id)) || (a.id < b.id ? -1 : a.id > b.id ? 1 : 0));
    if (entries.length < 2) continue;
    entries.forEach((target, index) => {
      target.generator.declarationCatalog = true;
      if (index) target.catalogCandidates = [entries[index - 1].id];
    });
  }
  generationOrder(result);
  return result;
}

export function planSdk(config, catalog, inventory, root) {
  const read = (file) => JSON.parse(readFileSync(path.join(root, file), "utf8"));
  const packages = new Map(inventory.packages.map((pkg) => [`${pkg.name}@${pkg.version}`, pkg]));
  const configuredOwners = new Map();
  for (const target of config.targets) {
    const key = `${target.package}@${target.version}`;
    if (!configuredOwners.has(key)) configuredOwners.set(key, new Set());
    configuredOwners.get(key).add(target.profile);
  }
  const configuredTargets = new Map(config.targets.map((target) => [target.id, target]));
  const profilePackages = new Map();
  for (const profile of config.profiles) {
    const manifest = read(`${profile.directory}/package.json`);
    for (const [name, version] of Object.entries(manifest.devDependencies ?? {})) {
      // Additional ambient packages do not change the SDK that owns the install profile.
      if (name === "@cloudflare/workers-types" && profile.id !== "workers") continue;
      // A directly pinned declaration provider may also be an SDK in another profile.
      // Its existing canonical owner remains authoritative when the plan is regenerated.
      const owner = configuredOwners.get(`${name}@${version}`);
      if (owner && !owner.has(profile.id)) continue;
      const key = `${name}@${version}`;
      if (!profilePackages.has(key)) profilePackages.set(key, new Set());
      profilePackages.get(key).add(profile.id);
    }
  }
  const surfaces = catalog.surfaces.filter((surface) => surface.productionTarget && surface.canonicalGenerationSurface);
  const ordered = [...surfaces].sort((a, b) => {
    const packageOrder = catalog.packages.findIndex((pkg) => pkg.id === a.packageId) - catalog.packages.findIndex((pkg) => pkg.id === b.packageId);
    if (packageOrder) return packageOrder;
    if (a.generationRole === "root-owner") return -1;
    if (b.generationRole === "root-owner") return 1;
    return a.id < b.id ? -1 : a.id > b.id ? 1 : 0;
  });
  const targets = ordered.map((surface) => {
    const id = surface.id.replace(/--root$/, "");
    const previous = configuredTargets.get(id);
    const pkg = packages.get(`${surface.package}@${surface.version}`);
    const candidates = profilePackages.get(`${surface.package}@${surface.version}`) ?? new Set();
    const profile = previous?.package === surface.package && previous?.version === surface.version && candidates.has(previous.profile)
      ? previous.profile : candidates.size === 1 ? [...candidates][0] : undefined;
    if (!profile && candidates.size > 1) throw new Error(`Ambiguous pinned input profiles for ${surface.id}; configure its profile explicitly`);
    if (!pkg || !profile) throw new Error(`No pinned input profile for ${surface.id}`);
    const profileDirectory = config.profiles.find((entry) => entry.id === profile).directory;
    const entry = path.join(root, profileDirectory, "node_modules", surface.package, surface.entry);
    if (!existsSync(entry)) throw new Error(`Catalog entry missing from installed profile: ${surface.id}: ${entry}`);
    const browserEntry = /(?:^|\/)(?:react|ai-react|browser|client|chat-react|xterm)(?:\/|$)/.test(surface.publicSubpath)
      || surface.logicalArea.includes("Browser") && !surface.logicalArea.includes("BrowserAutomation")
      || surface.logicalArea.includes("Mobile");
    const workers = pkg.environments.includes("workerd") && !browserEntry;
    const generator = {
      module: surface.module,
      namespace: "FSharp.CloudEdge.Runtime",
      entry: surface.entry,
      ...(surface.runtime ? { runtime: surface.runtime } : {}),
      lib: previous?.generator.lib ?? (workers ? ["esnext"] : ["esnext", "dom"]),
      types: previous?.generator.types ?? (workers && surface.package !== "@cloudflare/workers-types" ? ["@cloudflare/workers-types"] : []),
      declarationCatalog: true,
      ...Object.fromEntries(["groups", "resolveNoInfer", "declarationCatalog", "declarationReferences"].filter((key) => previous && Object.hasOwn(previous.generator, key)).map((key) => [key, previous.generator[key]])),
    };
    return {
      id,
      package: surface.package,
      version: surface.version,
      profile,
      dependencies: previous?.dependencies ?? [],
      outputDirectory: `src/${surface.logicalArea}/${surface.module}`,
      files: [`${surface.module}.fs`],
      generator,
    };
  });
  const idOf = (surface) => surface.id.replace(/--root$/, "");
  const roots = new Set(ordered.filter((surface) => surface.generationRole === "root-owner").map(idOf));
  // Ambient declaration variants replace each other's globals within a compiler program.
  const independent = new Set(ordered.filter((surface) => surface.generationMode === "ambientDeclarations").map(idOf));
  const planned = { ...config, targets: scheduleCatalogs(targets, roots, independent) };
  planProjects(planned, root);
  return planned;
}

function main() {
  const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
  const read = (file) => JSON.parse(readFileSync(path.join(root, file), "utf8"));
  const catalog = read("config/sdk-surfaces.json");
  const inventory = read(catalog.sourceInventory.startsWith("inventory/") ? catalog.sourceInventory : "inventory/cloudflare-sdk-inventory-20260906.json");
  const config = planSdk(read("config/targets.json"), catalog, inventory, root);
  writeFileSync(path.join(root, "config/targets.json"), JSON.stringify(config, null, 2) + "\n");
  console.log(`Planned ${config.targets.length} canonical SDK inputs in ${new Set(config.targets.map((target) => target.profile)).size} pinned profiles. Catalog candidates connect exact inference partitions; cross-profile ownership remains explicit.`);
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  try { main(); } catch (error) { console.error(`CloudEdge SDK planning failed: ${error.message}`); process.exitCode = 1; }
}
