import { createHash } from "node:crypto";
import { spawnSync } from "node:child_process";
import { closeSync, existsSync, mkdirSync, openSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { buildSdk } from "./build-sdk.mjs";
import { planProjects, checkProjects } from "./projects.mjs";
import { resolveTool } from "./tool-packages.mjs";

const defaultRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
const sha256 = file => createHash("sha256").update(readFileSync(file)).digest("hex");
const slash = value => value.split(path.sep).join("/");
const xml = value => value.replaceAll("&", "&amp;").replaceAll('"', "&quot;").replaceAll("<", "&lt;");
const canonical = value => JSON.stringify(value, (_, child) => child && typeof child === "object" && !Array.isArray(child)
  ? Object.fromEntries(Object.entries(child).sort(([a], [b]) => a.localeCompare(b, "en"))) : child);

/** Authenticate the installed Hawaii output against its generator, schema and policy inputs. */
export function currentHawaii(root, toolIdentity) {
  const read = name => JSON.parse(readFileSync(path.join(root, name), "utf8"));
  const pinsFile = path.join(root, "generators/hawaii/pins.json");
  const pins = read("generators/hawaii/pins.json");
  const receipt = read("inventory/hawaii-output-ownership.json");
  requireThat(receipt.schemaVersion === 1 && receipt.validation?.configuration === "Release", "Hawaii ownership receipt is incomplete");
  const run = path.resolve(root, receipt.validation.run);
  const provenance = JSON.parse(readFileSync(path.join(run, "provenance.json"), "utf8"));
  requireThat(provenance.status === "compiled" && provenance.pinsSha256 === sha256(pinsFile), "Hawaii pins or accepted generation changed");
  requireThat(Array.isArray(pins.pipelineInputs) && pins.pipelineInputs.length > 0
    && canonical(provenance.pipelineSources) === canonical(Object.fromEntries(pins.pipelineInputs.map(file =>
      [file, sha256(path.join(root, file))]))), "Hawaii pipeline sources changed");
  const ownership = JSON.parse(readFileSync(path.join(run, "output-ownership.json"), "utf8"));
  requireThat(["schemaVersion", "sharedProject", "clients", "files"].every(key => canonical(ownership[key]) === canonical(receipt[key])),
    "Hawaii installed ownership differs from the accepted run");
  const matchesTool = recorded => Object.entries(toolIdentity).every(([key, value]) => canonical(recorded?.[key]) === canonical(value));
  requireThat(matchesTool(provenance.hawaiiTool) && matchesTool(receipt.validation.tool), "Hawaii generator package changed");
  requireThat(provenance.schemaOriginalSha256 === pins.schema.sha256
    && sha256(path.join(run, "schema-original.json")) === pins.schema.sha256
    && provenance.schemaSelectedSha256 === receipt.validation.schemaSha256
    && sha256(path.join(run, "schema-selected.json")) === provenance.schemaSelectedSha256,
  "Hawaii schema provenance changed");
  const policyHashes = { taxonomy: "taxonomySha256", schemaPolicy: "schemaPolicySha256", operationPolicy: "lifecyclePolicySha256" };
  for (const [pin, field] of Object.entries(policyHashes)) requireThat(
    sha256(path.join(root, "generators/hawaii", pins[pin])) === provenance[field], `Hawaii ${pin} changed`);
  requireThat(canonical(provenance.overlays) === canonical(pins.overlays.map(file => ({ path: file,
    sha256: sha256(path.join(root, "generators/hawaii", file)) }))), "Hawaii input overlays changed");
  requireThat(receipt.validation.selectedOperations === provenance.includedOperations
    && provenance.sourceOperations === pins.schema.operationCount && provenance.unresolvedTaxonomyOperations === 0,
  "Hawaii operation coverage differs from the accepted selection");
  for (const [file, hash] of Object.entries(receipt.files)) requireThat(
    existsSync(path.join(root, file)) && sha256(path.join(root, file)) === hash, `Hawaii owned output changed: ${file}`);
  return { status: "current", run: receipt.validation.run, selectedOperations: receipt.validation.selectedOperations,
    projects: Object.keys(receipt.files).filter(file => file.endsWith(".fsproj")), authenticatedFiles: Object.keys(receipt.files).length };
}

/** Compose runtime, support, management and consumer projects into the library solution. */
export function librarySolution(projectPaths) {
  const folders = new Map();
  for (const project of [...new Set(projectPaths)].sort()) {
    requireThat(typeof project === "string" && !path.isAbsolute(project)
      && !/[\\:\x00-\x1f]/.test(project)
      && project.split("/").every(segment => segment.length > 0 && segment !== "." && segment !== "..")
      && /^(?:src|tests)\//.test(project) && project.endsWith(".fsproj"), `Invalid library project: ${project}`);
    const parts = project.split("/");
    const folder = parts[0] === "src" ? "/" + parts.slice(1, -2).join("/") + "/" : "/tests/";
    if (!folders.has(folder)) folders.set(folder, []);
    folders.get(folder).push(project);
  }
  return ["<Solution>", ...[...folders].sort(([a], [b]) => a.localeCompare(b, "en")).flatMap(([folder, projects]) => [
    `  <Folder Name="${xml(folder)}">`, ...projects.map(project => `    <Project Path="${xml(project)}" />`), "  </Folder>",
  ]), "</Solution>", ""].join("\n");
}

function execute(command, args, root, log, env) {
  const fd = openSync(log, "w");
  const startedAtUtc = new Date().toISOString();
  try {
    const result = spawnSync(command, args, { cwd: root, env, stdio: ["ignore", fd, fd] });
    requireThat(!result.error && result.status === 0, `${command} failed (${result.error?.message ?? result.signal ?? result.status}); see ${log}`);
    return { command, args, log, startedAtUtc, finishedAtUtc: new Date().toISOString(), exitCode: result.status };
  } finally { closeSync(fd); }
}

/** Generate both SDK pipelines and compile the complete selected library hierarchy. */
export async function buildLibrary({ root = defaultRoot, args = process.argv.slice(2), env = process.env } = {}) {
  requireThat(args.every(arg => ["--resume", "--plan"].includes(arg)), "Usage: npm run build -- [--resume] [--plan]");
  const read = name => JSON.parse(readFileSync(path.join(root, name), "utf8"));
  const directory = path.join(root, "artifacts/library-build");
  mkdirSync(directory, { recursive: true });
  const report = { status: "started", startedAtUtc: new Date().toISOString(), runtimeValidation: "separate" };
  const record = () => writeFileSync(path.join(directory, "latest.json"), JSON.stringify(report, null, 2) + "\n");
  try {
    report.status = "sdk"; record();
    report.sdk = await buildSdk({ root, args, env });
    if (args.includes("--plan")) { report.status = "planned"; return report; }
    report.status = "hawaii"; record();
    const stamp = new Date().toISOString().replaceAll(":", "-");
    const run = path.join(root, "artifacts/hawaii/runs", "library-" + stamp);
    const hawaiiArgs = ["generators/hawaii/generate.py", "--run-directory", run];
    const receiptFile = path.join(root, "inventory/hawaii-output-ownership.json");
    if (existsSync(receiptFile)) {
      const previous = read("inventory/hawaii-output-ownership.json");
      const schema = path.resolve(root, previous.validation.run, "schema-original.json");
      if (existsSync(schema) && sha256(schema) === read("generators/hawaii/pins.json").schema.sha256)
        hawaiiArgs.push("--schema", schema);
    }
    if (args.includes("--resume")) {
      try { report.hawaii = currentHawaii(root, resolveTool(root, "hawaii", env).identity); }
      catch (error) { report.hawaiiRefreshReason = error.message; }
    }
    if (!report.hawaii) report.hawaii = { status: "generated",
      ...execute("python3", hawaiiArgs, root, path.join(directory, `hawaii-${stamp}.log`), env) };
    const receipt = read("inventory/hawaii-output-ownership.json");
    for (const [file, hash] of Object.entries(receipt.files)) {
      requireThat(existsSync(path.join(root, file)) && sha256(path.join(root, file)) === hash,
        `Hawaii output differs from its accepted ownership receipt: ${file}`);
    }
    report.hawaii.selectedOperations = receipt.validation.selectedOperations;
    report.hawaii.run = receipt.validation.run;
    const configuration = read("artifacts/sdk-build/targets.json");
    const plan = planProjects(configuration, root);
    plan.solution = path.join(root, "SDK.Partitions.slnx");
    report.authenticatedSdkProjects = checkProjects(plan);
    const sdkProjects = plan.projects.map(project => slash(path.relative(root, project.file)));
    const managementProjects = Object.keys(receipt.files).filter(file => file.endsWith(".fsproj"));
    const supportProjects = ["src/Support/FSharp.CloudEdge.Support.Workers/FSharp.CloudEdge.Support.Workers.fsproj"];
    const consumers = ["tests/ByteBridge/ByteBridge.fsproj", "tests/HawaiiApi/HawaiiApi.fsproj", "tests/SDKComposition/SDKComposition.fsproj"];
    for (const file of supportProjects) requireThat(existsSync(path.join(root, file)), `Required library support is missing: ${file}`);
    for (const file of consumers) requireThat(existsSync(path.join(root, file)), `Required library consumer is missing: ${file}`);
    report.projects = [...sdkProjects, ...supportProjects, ...managementProjects, ...consumers];
    const solution = librarySolution(report.projects);
    writeFileSync(path.join(root, "FSharp.CloudEdge.slnx"), solution);
    writeFileSync(path.join(root, "FSharp.CloudEdge.Bindings.slnx"), librarySolution([...sdkProjects, ...supportProjects, ...managementProjects]));
    report.status = "compiling"; record();
    report.build = execute(env.CLOUDEDGE_DOTNET ?? "dotnet",
      ["build", "FSharp.CloudEdge.slnx", "--configuration", "Release", "--nologo", "--disable-build-servers", "-m:1"],
      root, path.join(directory, `build-${stamp}.log`), env);
    report.crossLibraryConsumers = { status: "compiled", projects: consumers };
    report.handWrittenSupport = { status: "compiled", projects: supportProjects };
    const fidelity = {
      note: "Per-target symbol grades from authenticated generation output; these are not a count of unique declarations across libraries.",
      targets: configuration.targets.map(target => ({ target: target.id, module: target.generator.module,
        counts: read(`artifacts/${target.id}/manifest.json`).counts,
        manifest: `artifacts/${target.id}/manifest.json`, symbols: `artifacts/${target.id}/symbols.jsonl`,
        catalog: `artifacts/${target.id}/declarations.json` })),
    };
    writeFileSync(path.join(directory, "fidelity.json"), JSON.stringify(fidelity, null, 2) + "\n");
    report.fidelity = "artifacts/library-build/fidelity.json";
    report.status = "compiled";
    console.log(`Complete hierarchy compiled: ${sdkProjects.length} generated SDK/support projects, ${supportProjects.length} handwritten support project, ${managementProjects.length} Hawaii projects and ${consumers.length} consumer projects.`);
    return report;
  } catch (error) {
    report.failedStage = report.status;
    report.status = "failed";
    report.error = error.message;
    throw error;
  } finally { report.finishedAtUtc = new Date().toISOString(); record(); }
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url))
  buildLibrary().catch(error => { console.error(`CloudEdge library build failed: ${error.message}`); process.exitCode = 1; });
