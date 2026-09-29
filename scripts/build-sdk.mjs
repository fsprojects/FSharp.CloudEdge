import { existsSync, mkdirSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { planPartitions } from "./plan-partitions.mjs";
import { generationOrder } from "./catalogs.mjs";
import { generate } from "./generate.mjs";
import { planProjects, writeProjects, buildProjects } from "./projects.mjs";
import { deliveryConfiguration } from "./sdk-delivery.mjs";

const defaultRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
const firstDelivery = ["workers", "agents", "containers", "sandbox"];

/** Materialize partition owners with their configured destinations and exact source lists. */
export function partitionConfiguration(config, plan) {
  const targets = new Map(config.targets.map(target => [target.id, target]));
  return { ...config, targets: plan.partitions.map(partition => {
    const previous = targets.get(partition.id);
    requireThat(previous, `Missing configured partition owner: ${partition.id}`);
    const target = { ...previous, generator: partition.generator };
    delete target.catalogCandidates;
    return target;
  }) };
}

/** Generate, authenticate and compile the selected exact-input partitions. */
export async function buildSdk({ root = defaultRoot, args = process.argv.slice(2), env = process.env,
  generateBindings = generate, compileBindings = buildProjects, toolResolver } = {}) {
  requireThat(args.filter(arg => arg.startsWith("--")).every(arg => ["--all", "--resume", "--plan"].includes(arg)),
    "Usage: node scripts/build-sdk.mjs [--plan] [--resume] [--all | partition ...]");
  const explicit = args.filter(arg => !arg.startsWith("--"));
  requireThat(!args.includes("--all") || explicit.length === 0, "--all cannot be combined with selected partitions");
  const read = file => JSON.parse(readFileSync(path.join(root, file), "utf8"));
  const canonical = read("config/targets.json");
  const plan = planPartitions(canonical, read("config/sdk-surfaces.json"));
  const partitioned = partitionConfiguration(canonical, plan);
  const delivery = !args.includes("--all") && existsSync(path.join(root, "config/sdk-delivery.json"))
    ? deliveryConfiguration(partitioned, plan.partitions, read("config/sdk-delivery.json"), root) : undefined;
  const configuration = delivery?.configuration ?? partitioned;
  const requested = args.includes("--all") ? [] : explicit.length ? [...new Set(explicit)]
    : delivery?.libraries.map(library => library.id) ?? firstDelivery;
  const selected = generationOrder(configuration.targets, requested);
  const selectedIds = new Set(selected.map(target => target.id));
  const selectedPartitions = (delivery?.libraries ?? plan.partitions).filter(partition => selectedIds.has(partition.id));
  const selectedConfiguration = { ...configuration, targets: selected };
  const projectPlan = planProjects(selectedConfiguration, root);
  // This solution records the partition build independently of the canonical-entry plan.
  projectPlan.solution = path.join(root, "SDK.Partitions.slnx");
  const directory = path.join(root, "artifacts/sdk-build");
  mkdirSync(directory, { recursive: true });
  const save = (name, value) => writeFileSync(path.join(directory, name), JSON.stringify(value, null, 2) + "\n");
  save("partitions.json", plan);
  save("targets.json", selectedConfiguration);
  if (delivery) save("scope.json", delivery.scope);
  const report = { status: "planned", inventoryInputs: plan.inputCount, inventoryPartitions: plan.partitionCount,
    delivery: delivery?.scope.name ?? "inventory",
    requested: selected.map(target => target.id), selectedInputs: selectedPartitions.flatMap(partition => partition.inputs),
    selectedLibraries: selectedPartitions.map(partition => partition.id),
    supportTargets: selected.filter(target => !selectedPartitions.some(partition => partition.id === target.id)).map(target => target.id),
    generation: [], compiled: [], crossLibraryConsumers: "pending", runtimeValidation: "pending" };
  const record = () => save("latest.json", report);
  record();
  console.log(`Selected ${report.selectedInputs.length} public inputs in ${selectedPartitions.length} SDK libraries and ${report.supportTargets.length} support libraries; inventory contains ${plan.inputCount} inputs.`);
  if (args.includes("--plan")) return report;
  try {
    report.status = "generating"; record();
    report.generation = await generateBindings({ root, configuration: selectedConfiguration, env, toolResolver,
      args: [...(args.includes("--resume") ? ["--resume"] : []), ...requested] });
    requireThat(Array.isArray(report.generation) && selected.every(target =>
      report.generation.some(result => result.target === target.id && ["generated", "current"].includes(result.status))),
      "Selected partitions did not all generate; compilation was not started");
    report.status = "generated"; record();
    writeProjects(projectPlan);
    report.status = "compiling"; record();
    const built = compileBindings(projectPlan, [], env.CLOUDEDGE_DOTNET ?? "dotnet");
    requireThat(built.succeeded, "Selected partition compilation failed");
    report.compiled = built.checked;
    report.buildLog = built.log;
    report.status = "compiled";
    console.log(`Compiled ${report.compiled.length} selected libraries; cross-library consumers and runtime validation remain separate gates.`);
    return report;
  } catch (error) {
    report.failedStage = report.status;
    report.status = "failed";
    report.error = error.message;
    throw error;
  } finally { record(); }
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  buildSdk().catch(error => { console.error(`CloudEdge SDK build failed: ${error.message}`); process.exitCode = 1; });
}
