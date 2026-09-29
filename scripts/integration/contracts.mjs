import { createHash } from "node:crypto";
import { existsSync, mkdirSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { partitionConfiguration } from "../build-sdk.mjs";
import { currentHawaii } from "../build-library.mjs";
import { planPartitions } from "../plan-partitions.mjs";
import { planProjects, checkProjects } from "../projects.mjs";
import { deliveryConfiguration } from "../sdk-delivery.mjs";

const defaultRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "../..");
export const defaultBaseline = "tests/integration/baselines/server-resources-20260913.json";
const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
const object = value => value !== null && typeof value === "object" && !Array.isArray(value);
const compare = (a, b) => a < b ? -1 : a > b ? 1 : 0;
export const canonical = value => JSON.stringify(value, (_, child) => object(child)
  ? Object.fromEntries(Object.entries(child).sort(([a], [b]) => compare(a, b))) : child);
const hash = bytes => createHash("sha256").update(bytes).digest("hex");
const digest = value => hash(canonical(value));
const read = file => JSON.parse(readFileSync(file, "utf8"));
const ordered = values => [...values].sort((a, b) => compare(canonical(a), canonical(b)));
const sha = value => typeof value === "string" && /^[a-f0-9]{64}$/.test(value);

export function checkedHash(file, expected, label = file) {
  requireThat(sha(expected), `${label}: missing SHA-256 evidence`);
  requireThat(existsSync(file), `${label}: missing evidence file`);
  const actual = hash(readFileSync(file));
  requireThat(actual === expected, `${label}: SHA-256 differs from accepted evidence`);
  return actual;
}

function uniqueMap(entries, label) {
  const result = {};
  for (const [key, value] of entries) {
    requireThat(typeof key === "string" && !Object.hasOwn(result, key), `${label}: duplicate identity ${key}`);
    Object.defineProperty(result, key, { value, enumerable: true, writable: true, configurable: true });
  }
  return result;
}

/** Catalog API hashes cover emitted declarations. They do not assert runtime behavior. */
export function catalogContract(target, catalog) {
  requireThat(catalog.schemaVersion === 1 && catalog.owner === target.generator.module
    && Array.isArray(catalog.declarations) && Array.isArray(catalog.inputs), `${target.id}: invalid declaration catalog`);
  const declarations = uniqueMap(catalog.declarations.filter(item => item.owner === catalog.owner).map(item => {
    requireThat(typeof item.fSharpName === "string" && typeof item.role === "string"
      && Number.isInteger(item.arity) && sha(item.api) && Array.isArray(item.constraints),
    `${target.id}: incomplete declaration API evidence`);
    // Source offsets and package versions are deliberately absent from the stable identity.
    return [`${item.role}:${item.fSharpName}:${item.arity}`, { api: item.api, constraints: item.constraints }];
  }), `${target.id} declarations`);
  const inputs = uniqueMap(catalog.inputs.map(item => {
    requireThat(typeof item.package === "string" && typeof item.version === "string" && typeof item.file === "string"
      && sha(item.sha256) && sha(item.manifestSha256), `${target.id}: incomplete TypeScript input evidence`);
    return [`${item.package}@${item.version}/${item.file}`, { sha256: item.sha256, manifestSha256: item.manifestSha256 }];
  }), `${target.id} TypeScript inputs`);
  return { declarations, inputs };
}

const methods = new Set(["get", "put", "post", "delete", "options", "head", "patch", "trace"]);

/** Follow local references so changing a shared request/response schema changes its consumers. */
export function operationContracts(schema, coverage, ownership) {
  requireThat(object(schema.paths) && Array.isArray(coverage) && Array.isArray(ownership.clients), "Missing OpenAPI operation evidence");
  const ownerById = uniqueMap(ownership.clients.flatMap(client => client.operationIds.map(id => [id, client.project])), "OpenAPI owner");
  const included = coverage.filter(row => row.disposition === "included");
  requireThat(included.length === Object.keys(ownerById).length, "Selected OpenAPI coverage differs from generated owner inventory");
  const schemaOperations = Object.entries(schema.paths).flatMap(([url, item]) => Object.keys(item)
    .filter(method => methods.has(method)).map(method => `${method.toUpperCase()} ${url}`));
  requireThat(canonical([...schemaOperations].sort()) === canonical(included.map(row => `${row.method.toUpperCase()} ${row.path}`).sort()),
    "Selected OpenAPI schema differs from operation coverage");
  const pointerCache = new Map();
  function resolve(reference) {
    requireThat(typeof reference === "string" && reference.startsWith("#/"), `Unsupported OpenAPI reference: ${reference}`);
    if (!pointerCache.has(reference)) {
      const resolved = reference.slice(2).split("/").map(part => decodeURIComponent(part).replaceAll("~1", "/").replaceAll("~0", "~"))
        .reduce((value, part) => value?.[part], schema);
      requireThat(resolved !== undefined, `Unresolved OpenAPI reference: ${reference}`);
      pointerCache.set(reference, { value: resolved, digest: digest(resolved) });
    }
    return pointerCache.get(reference);
  }
  return uniqueMap(included.map(row => {
    const item = schema.paths[row.path];
    const operation = item?.[row.method];
    requireThat(operation && operation.operationId === row.operationId && ownerById[row.operationId],
      `${row.method} ${row.path}: operation identity or generated owner differs`);
    const refs = new Map();
    function visit(value) {
      if (!value || typeof value !== "object") return;
      if (typeof value.$ref === "string" && !refs.has(value.$ref)) {
        const resolved = resolve(value.$ref);
        refs.set(value.$ref, resolved.digest);
        visit(resolved.value);
      }
      Object.values(value).forEach(visit);
    }
    const security = operation.security ?? schema.security ?? [];
    const securitySchemes = uniqueMap([...new Set(security.flatMap(requirement => Object.keys(requirement)))].map(name => {
      const scheme = schema.components?.securitySchemes?.[name];
      requireThat(scheme !== undefined, `Missing OpenAPI security scheme: ${name}`);
      return [name, scheme];
    }), "security schemes");
    const contract = { operation, path: Object.fromEntries(Object.entries(item).filter(([key]) => !methods.has(key))),
      security, securitySchemes, servers: operation.servers ?? item.servers ?? schema.servers ?? [] };
    visit(contract);
    return [`${row.method.toUpperCase()} ${row.path}`, { operationId: row.operationId, owner: ownerById[row.operationId],
      taxonomy: row.taxonomy, api: digest({ contract, references: Object.fromEntries(refs) }) }];
  }), "OpenAPI operations");
}

/** Read-only. Existing generator authentication runs before inventory is captured. */
export function snapshotContracts({ root = defaultRoot } = {}) {
  root = path.resolve(root);
  const json = file => read(path.join(root, file));
  const configuration = json("config/targets.json");
  const partitions = planPartitions(configuration, json("config/sdk-surfaces.json"));
  const delivery = deliveryConfiguration(partitionConfiguration(configuration, partitions), partitions.partitions,
    json("config/sdk-delivery.json"), root);
  requireThat(canonical(delivery.configuration) === canonical(json("artifacts/sdk-build/targets.json")),
    "Materialized SDK targets differ from current selected delivery; regenerate before contract checking");
  requireThat(canonical(delivery.scope) === canonical(json("artifacts/sdk-build/scope.json")),
    "Materialized public input coverage differs from current selected delivery");
  const plan = planProjects(delivery.configuration, root);
  plan.solution = path.join(root, "SDK.Partitions.slnx");
  const authenticated = checkProjects(plan);
  const tools = json("config/tool-packages.json");
  const hawaiiPins = json("generators/hawaii/pins.json");
  const hawaiiTool = tools.tools?.[hawaiiPins.hawaii.tool];
  requireThat(hawaiiTool && sha(hawaiiTool.packageSha256), "Missing pinned Hawaii tool evidence");
  const hawaii = currentHawaii(root, hawaiiTool);
  const ownership = json("inventory/hawaii-output-ownership.json");
  const coverage = json(path.join(hawaii.run, "operation-coverage.json"));
  const selectedSchema = json(path.join(hawaii.run, "schema-selected.json"));
  const registry = json("generators/hawaii/inventory/operation-selection-coverage.json");
  requireThat(canonical(coverage) === canonical(registry.operations), "OpenAPI selection registry differs from accepted run");
  const operations = operationContracts(selectedSchema, coverage, ownership);
  requireThat(Object.keys(operations).length === hawaii.selectedOperations, "OpenAPI operation count differs from authenticated acceptance");
  const authenticatedDeclarations = new Map();
  const libraries = uniqueMap(delivery.configuration.targets.map(target => {
    const provenance = json(`artifacts/${target.id}/generation.json`);
    const catalog = json(`artifacts/${target.id}/declarations.json`);
    const contract = catalogContract(target, catalog);
    for (const input of catalog.inputs.filter(item => item.package === target.package && item.version === target.version)) {
      requireThat(!path.isAbsolute(input.file) && !input.file.split(/[\\/]/).includes(".."), `${target.id}: invalid declaration input path`);
      const file = path.join(provenance.inputPackageDirectory, input.file);
      if (!authenticatedDeclarations.has(file)) authenticatedDeclarations.set(file, checkedHash(file, input.sha256, `${target.id} ${input.file}`));
      requireThat(authenticatedDeclarations.get(file) === input.sha256, `${target.id}: conflicting declaration input evidence`);
    }
    const publicInputs = uniqueMap(Object.entries(target.generator.publicInputs ?? { ".": target.generator.entry }).map(([subpath, entry]) => {
      const evidence = catalog.inputs.find(item => item.package === target.package && item.version === target.version && item.file === entry);
      requireThat(evidence, `${target.id} ${subpath}: selected public input missing from declaration catalog`);
      const directory = provenance.inputPackageDirectory;
      requireThat(typeof entry === "string" && !path.isAbsolute(entry) && !entry.split(/[\\/]/).includes(".."),
        `${target.id}: invalid public input path`);
      checkedHash(path.join(directory, entry), evidence.sha256, `${target.id} ${subpath} TypeScript declaration`);
      checkedHash(path.join(directory, "package.json"), evidence.manifestSha256, `${target.id} package manifest`);
      return [subpath, { entry, sha256: evidence.sha256 }];
    }), `${target.id} public inputs`);
    return [target.id, { package: target.package, version: target.version, owner: catalog.owner,
      kind: delivery.scope.supportTargets.includes(target.id) ? "generated-support" : "server-sdk",
      publicInputs, ...contract, provenance: { profileLockSha256: provenance.profileLockSha256,
        profileManifestSha256: provenance.profileManifestSha256, overlayFingerprint: provenance.inputOverlays.fingerprint,
        typescript: provenance.typescript, compiler: catalog.compiler, generator: catalog.generator,
        inferenceProfile: catalog.inferenceProfile, sourceSha256: provenance.sourceSha256,
        declarationCatalogSha256: provenance.declarationCatalogSha256 } }];
  }), "SDK targets");
  const supportDirectory = "src/Support/FSharp.CloudEdge.Support.Workers";
  const supportProject = `${supportDirectory}/FSharp.CloudEdge.Support.Workers.fsproj`;
  const project = readFileSync(path.join(root, supportProject), "utf8");
  const supportSources = [...project.matchAll(/<Compile\s+Include="([^"]+)"\s*\/>/g)].map(match => `${supportDirectory}/${match[1]}`);
  requireThat(supportSources.length > 0, "Workers support project has no tracked sources");
  const support = uniqueMap([supportProject, ...supportSources].map(file => [file, hash(readFileSync(path.join(root, file)))]), "Workers support files");
  return JSON.parse(canonical({ schemaVersion: 1, kind: "cloudedge-declaration-contracts", scope: {
    name: delivery.scope.name, serverLibraries: delivery.scope.libraries.length, selectedPublicInputs: delivery.scope.selectedInputs,
    generatedSupportLibraries: delivery.scope.supportTargets.length, managementOperations: Object.keys(operations).length,
    managementProjects: hawaii.projects.length,
    dispositions: uniqueMap(delivery.scope.dispositions.map(item => [item.id, item]), "public input dispositions") },
  evidence: { level: "authenticated-declaration-inventory", runtimeCoverage: "not-established-by-this-snapshot",
    sdkProjects: authenticated.length, managementFiles: hawaii.authenticatedFiles,
    selectedPackageDeclarationFiles: authenticatedDeclarations.size },
  toolchain: { typescript: configuration.toolchain.typescript, tools: tools.tools },
  libraries, support, management: { pins: hawaiiPins, schemaSha256: ownership.validation.schemaSha256,
    files: ownership.files, operations, excluded: ordered(coverage.filter(row => row.disposition !== "included")) } }));
}

function mapDiff(before, after) {
  before ??= {}; after ??= {};
  const added = Object.keys(after).filter(key => !Object.hasOwn(before, key)).sort();
  const removed = Object.keys(before).filter(key => !Object.hasOwn(after, key)).sort();
  const changed = Object.keys(before).filter(key => Object.hasOwn(after, key) && canonical(before[key]) !== canonical(after[key])).sort();
  return { added, removed, changed };
}
const hasChanges = value => Object.values(value).some(list => list.length > 0);

function validateSnapshot(snapshot) {
  requireThat(snapshot?.schemaVersion === 1 && snapshot.kind === "cloudedge-declaration-contracts"
    && object(snapshot.libraries) && object(snapshot.management?.operations)
    && object(snapshot.scope?.dispositions) && object(snapshot.support)
    && snapshot.evidence?.level === "authenticated-declaration-inventory",
  "Invalid or unauthenticated contract snapshot");
}

/** Any delta requires review, including additions which need behavioral test disposition. */
export function diffContracts(before, after) {
  validateSnapshot(before); validateSnapshot(after);
  const libraries = mapDiff(before.libraries, after.libraries);
  const libraryChanges = uniqueMap(libraries.changed.map(id => {
    const old = before.libraries[id], current = after.libraries[id];
    return [id, { package: { before: `${old.package}@${old.version}`, after: `${current.package}@${current.version}` },
      publicInputs: mapDiff(old.publicInputs, current.publicInputs), declarations: mapDiff(old.declarations, current.declarations),
      typescriptInputs: mapDiff(old.inputs, current.inputs), provenanceChanged: canonical(old.provenance) !== canonical(current.provenance) }];
  }), "library differences");
  const report = { schemaVersion: 1, status: canonical(before) === canonical(after) ? "unchanged" : "review-required",
    beforeSha256: digest(before), afterSha256: digest(after), libraries, libraryChanges,
    publicInputCoverage: mapDiff(before.scope.dispositions, after.scope.dispositions),
    managementOperations: mapDiff(before.management.operations, after.management.operations),
    managementFiles: mapDiff(before.management.files, after.management.files), supportFiles: mapDiff(before.support, after.support),
    toolchainChanged: canonical(before.toolchain) !== canonical(after.toolchain),
    managementSelectionChanged: canonical(before.management.pins) !== canonical(after.management.pins)
      || canonical(before.management.excluded) !== canonical(after.management.excluded),
    behavioralCoverage: "requires-separate-integration-results" };
  report.changedPublicInputLibraries = [...new Set([...libraries.added, ...libraries.removed,
    ...Object.entries(libraryChanges).filter(([, change]) => hasChanges(change.publicInputs)).map(([id]) => id)])].sort();
  return report;
}

export function runContracts(args = process.argv.slice(2)) {
  const [command, ...remaining] = args;
  requireThat(["snapshot", "check", "diff"].includes(command),
    "Usage: contracts.mjs snapshot --output FILE | check [--baseline FILE] [--output FILE] | diff BEFORE AFTER [--output FILE] [--root DIR]");
  const options = {}, positional = [];
  for (let index = 0; index < remaining.length; index++) {
    const value = remaining[index];
    if (!value.startsWith("--")) { positional.push(value); continue; }
    requireThat(["--root", "--output", "--baseline"].includes(value) && !Object.hasOwn(options, value)
      && remaining[index + 1] && !remaining[index + 1].startsWith("--"), `Invalid or repeated option ${value}`);
    options[value] = remaining[++index];
  }
  const root = path.resolve(options["--root"] ?? defaultRoot);
  requireThat(command === "diff" ? positional.length === 2 : positional.length === 0, "Unexpected contract command arguments");
  requireThat(command === "check" || !options["--baseline"], "--baseline applies only to check");
  let result;
  if (command === "snapshot") {
    requireThat(options["--output"], "Snapshot requires an explicit --output; the accepted baseline is never updated implicitly");
    requireThat(!existsSync(path.resolve(root, options["--output"])), "Snapshot output already exists; choose a new candidate path for review");
    result = snapshotContracts({ root });
  } else if (command === "diff") {
    result = diffContracts(...positional.map(file => read(path.resolve(root, file))));
  } else result = diffContracts(read(path.resolve(root, options["--baseline"] ?? defaultBaseline)), snapshotContracts({ root }));
  if (options["--output"]) {
    const output = path.resolve(root, options["--output"]);
    const inputs = command === "check" ? [options["--baseline"] ?? defaultBaseline] : command === "diff" ? positional : [];
    requireThat(!inputs.some(file => path.resolve(root, file) === output), "Report output cannot overwrite an input snapshot");
    mkdirSync(path.dirname(output), { recursive: true });
    writeFileSync(output, JSON.stringify(result, null, 2) + "\n", command === "snapshot" ? { flag: "wx" } : undefined);
  }
  const summary = command === "snapshot" ? { status: "snapshotted", output: options["--output"], scope: {
    serverLibraries: result.scope.serverLibraries, selectedPublicInputs: result.scope.selectedPublicInputs,
    managementOperations: result.scope.managementOperations }, evidence: result.evidence }
    : options["--output"] ? { status: result.status, output: options["--output"], libraries: result.libraries,
      managementOperations: result.managementOperations, publicInputCoverage: result.publicInputCoverage } : result;
  console.log(JSON.stringify(summary, null, 2));
  return command !== "snapshot" && result.status !== "unchanged" ? 2 : 0;
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  try { process.exitCode = runContracts(); }
  catch (error) { console.error(`CloudEdge contract check failed: ${error.message}`); process.exitCode = 1; }
}
