import { createHash } from "node:crypto";
import { existsSync, lstatSync, mkdirSync, mkdtempSync, readFileSync, readdirSync, readlinkSync,
  realpathSync, renameSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { planProjects } from "./projects.mjs";

const defaultRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
const hash = bytes => createHash("sha256").update(bytes).digest("hex");
const read = file => JSON.parse(readFileSync(file, "utf8"));
const slash = value => value.split(path.sep).join("/");
const within = (parent, child) => child === parent || child.startsWith(parent + path.sep);
const json = value => JSON.stringify(value, null, 2) + "\n";
const canonical = value => JSON.stringify(value, (_, child) => child && typeof child === "object" && !Array.isArray(child)
  ? Object.fromEntries(Object.entries(child).sort(([a], [b]) => a.localeCompare(b, "en"))) : child);
const present = file => { try { lstatSync(file); return true; } catch (error) { if (error.code === "ENOENT") return false; throw error; } };

function historicalProjects(root) {
  const records = new Map();
  const folders = [path.join(root, "artifacts/projects"), ...readdirSync(path.join(root, "artifacts"), { withFileTypes: true })
    .filter(entry => entry.isDirectory() && entry.name.startsWith("one-shot-")).map(entry => path.join(root, "artifacts", entry.name))];
  for (const folder of folders) {
    if (!existsSync(folder)) continue;
    for (const name of readdirSync(folder).sort()) {
      if (!name.endsWith(".json") || (path.basename(folder) !== "projects" && !name.startsWith("checked"))) continue;
      const file = path.join(folder, name);
      let document;
      try { document = read(file); } catch { continue; }
      const evidence = { receipt: slash(path.relative(root, file)), receiptSha256: hash(readFileSync(file)) };
      function visit(value) {
        if (Array.isArray(value)) value.forEach(visit);
        else if (value && typeof value === "object") {
          if (typeof value.project === "string" && /^[a-f0-9]{64}$/.test(value.projectSha256 ?? "")) {
            const key = value.project + ":" + value.projectSha256;
            if (!records.has(key)) records.set(key, evidence);
          }
          Object.values(value).forEach(visit);
        }
      }
      visit(document);
    }
  }
  return records;
}

function linkedAncestor(root, directory) {
  let current = root;
  for (const part of path.relative(root, directory).split(path.sep)) {
    current = path.join(current, part);
    if (present(current) && lstatSync(current).isSymbolicLink()) return slash(path.relative(root, current));
  }
  return undefined;
}

function directoryInventory(directory) {
  const files = [];
  function visit(current, prefix = "") {
    for (const name of readdirSync(current).sort()) {
      const file = path.join(current, name), relative = prefix + name, stat = lstatSync(file);
      if (stat.isSymbolicLink()) files.push({ path: relative, kind: "symlink", target: readlinkSync(file) });
      else if (stat.isDirectory()) { files.push({ path: relative, kind: "directory" }); visit(file, relative + "/"); }
      else if (stat.isFile()) files.push({ path: relative, kind: "file", bytes: stat.size, sha256: hash(readFileSync(file)) });
      else files.push({ path: relative, kind: "special" });
    }
  }
  visit(directory);
  return files;
}

const decodeXml = value => value.replace(/&(?:amp|quot|apos|lt|gt|#\d+|#x[0-9a-f]+);/gi, token => {
  const named = { "&amp;": "&", "&quot;": '"', "&apos;": "'", "&lt;": "<", "&gt;": ">" };
  return named[token] ?? String.fromCodePoint(token.startsWith("&#x") ? parseInt(token.slice(3, -1), 16) : Number(token.slice(2, -1)));
});

function liveReferences(root, selectedPlan) {
  const files = new Set(selectedPlan.projects.map(project => project.file));
  function tests(directory) {
    if (!existsSync(directory)) return;
    for (const entry of readdirSync(directory, { withFileTypes: true })) {
      if (entry.isSymbolicLink() || ["bin", "obj", "node_modules"].includes(entry.name)) continue;
      const file = path.join(directory, entry.name);
      if (entry.isDirectory()) tests(file);
      else if (entry.name.endsWith(".fsproj")) files.add(file);
    }
  }
  tests(path.join(root, "tests"));
  for (const name of readdirSync(root)) if (name.endsWith(".slnx")) files.add(path.join(root, name));
  const references = [];
  for (const file of [...files].sort()) {
    if (!existsSync(file)) continue;
    const pattern = file.endsWith(".slnx") ? /<Project\b[^>]*\bPath\s*=\s*["']([^"']+)["']/g
      : /<ProjectReference\b[^>]*\bInclude\s*=\s*["']([^"']+)["']/g;
    for (const match of readFileSync(file, "utf8").matchAll(pattern))
      references.push({ from: slash(path.relative(root, file)), to: path.resolve(path.dirname(file), decodeXml(match[1]).replaceAll("\\", "/")) });
  }
  return references;
}

function inspect(root, project, history) {
  const directory = path.dirname(project.file), reasons = [], evidence = [];
  const linked = linkedAncestor(root, directory);
  if (linked) return { reasons: [`Linked output path: ${linked}`], evidence, files: [] };
  const files = directoryInventory(directory);
  const receiptFile = path.join(root, "artifacts", project.id, "generation.json");
  let receipt;
  if (existsSync(receiptFile)) {
    try { receipt = read(receiptFile); } catch { reasons.push("Unreadable generation receipt"); }
  }
  const target = project.target;
  const identityMatches = receipt?.target === target.id && receipt?.package === target.package
    && receipt?.version === target.version && receipt?.generator?.module === target.generator.module;
  const sources = identityMatches ? receipt.sourceSha256 ?? {} : {};
  const projectName = path.basename(project.file);
  for (const item of files) {
    if (["bin", "obj"].includes(item.path.split("/")[0])) continue;
    if (item.kind === "directory") {
      if (!Object.keys(sources).some(file => file.startsWith(item.path + "/"))) reasons.push(`Unowned directory: ${item.path}`);
    } else if (item.kind !== "file") reasons.push(`Linked or special non-build output: ${item.path}`);
    else if (item.path === projectName) {
      const previous = history.get(slash(path.relative(root, project.file)) + ":" + item.sha256);
      if (item.sha256 === hash(project.content)) evidence.push({ file: item.path, kind: "canonical-project-render", sha256: item.sha256 });
      else if (previous) evidence.push({ file: item.path, kind: "checked-project-receipt", sha256: item.sha256, ...previous });
      else reasons.push(`Edited or unverified project: ${item.path}`);
    } else if (!Object.hasOwn(sources, item.path)) reasons.push(`Unowned file: ${item.path}`);
    else if (sources[item.path] !== item.sha256) reasons.push(`Edited generated source: ${item.path}`);
    else evidence.push({ file: item.path, kind: "generation-receipt", sha256: item.sha256,
      receipt: slash(path.relative(root, receiptFile)), receiptSha256: hash(readFileSync(receiptFile)) });
  }
  return { reasons, evidence, files };
}

/** Plan only: existing output, ownership receipts and retained consumer references decide eligibility. */
export function planSdkArchive(root = defaultRoot) {
  root = realpathSync(root);
  const canonicalFile = path.join(root, "config/targets.json"), selectedFile = path.join(root, "artifacts/sdk-build/targets.json");
  const canonical = planProjects(read(canonicalFile), root), selected = planProjects(read(selectedFile), root);
  const selectedDirectories = selected.projects.map(project => path.dirname(project.file));
  const references = liveReferences(root, selected), history = historicalProjects(root), entries = [];
  for (const project of canonical.projects) {
    const directory = path.dirname(project.file);
    if (selectedDirectories.includes(directory) || !present(directory)) continue;
    const inspected = inspect(root, project, history);
    if (selectedDirectories.some(other => within(directory, other) || within(other, directory))) inspected.reasons.push("Overlaps a selected output directory");
    if (canonical.projects.some(other => other.id !== project.id && within(directory, path.dirname(other.file)))) inspected.reasons.push("Contains another canonical output directory");
    for (const reference of references) if (within(directory, reference.to)) inspected.reasons.push(`Still referenced by ${reference.from}`);
    entries.push({ target: project.id, source: slash(path.relative(root, directory)),
      status: inspected.reasons.length ? "retained" : "eligible", ...inspected });
  }
  return { schemaVersion: 1, status: "planned", root, plannedAtUtc: new Date().toISOString(),
    canonicalConfigurationSha256: hash(readFileSync(canonicalFile)), selectedConfigurationSha256: hash(readFileSync(selectedFile)), entries };
}

/** Archive complete verified directories without deleting canonical configuration or generation evidence. */
export function archiveSdkOutput({ root = defaultRoot, dryRun = false } = {}) {
  const report = planSdkArchive(root);
  root = report.root;
  if (dryRun) return report;
  const buildFile = path.join(root, "artifacts/sdk-build/latest.json");
  const buildBytes = readFileSync(buildFile), build = JSON.parse(buildBytes);
  const canonicalPlan = planProjects(read(path.join(root, "config/targets.json")), root);
  const selectedPlan = planProjects(read(path.join(root, "artifacts/sdk-build/targets.json")), root);
  const projects = new Map(canonicalPlan.projects.map(project => [project.id, project]));
  const history = historicalProjects(root);
  const selectedIds = read(path.join(root, "artifacts/sdk-build/targets.json")).targets.map(target => target.id).sort();
  requireThat(build.status === "compiled" && Array.isArray(build.requested)
    && JSON.stringify([...build.requested].sort()) === JSON.stringify(selectedIds),
    "Archive requires a successful build of the current selected target set");
  const libraryFile = path.join(root, "artifacts/library-build/latest.json");
  requireThat(existsSync(libraryFile), "Archive requires a successful complete library build");
  const libraryBytes = readFileSync(libraryFile), library = JSON.parse(libraryBytes);
  requireThat(library.status === "compiled" && library.build?.exitCode === 0
    && library.crossLibraryConsumers?.status === "compiled", "Archive requires a successful complete library build");
  requireThat(canonical(library.sdk) === canonical(build),
    "Complete library build SDK evidence differs from the current selected build");
  const parent = path.join(root, "artifacts/sdk-output-archive");
  requireThat(!linkedAncestor(root, parent), "Archive location cannot pass through a symbolic link");
  mkdirSync(parent, { recursive: true });
  const archive = mkdtempSync(path.join(parent, new Date().toISOString().replaceAll(":", "-") + "-"));
  report.archive = slash(path.relative(root, archive));
  report.selectedBuildSha256 = hash(buildBytes);
  report.libraryBuildSha256 = hash(libraryBytes);
  report.status = "archiving";
  for (const [name, file] of [["canonical-targets.json", "config/targets.json"], ["selected-targets.json", "artifacts/sdk-build/targets.json"]])
    writeFileSync(path.join(archive, name), readFileSync(path.join(root, file)));
  writeFileSync(path.join(archive, "sdk-build.json"), buildBytes);
  writeFileSync(path.join(archive, "library-build.json"), libraryBytes);
  const record = () => writeFileSync(path.join(archive, "inventory.json"), json(report));
  record();
  try {
    for (const entry of report.entries.filter(entry => entry.status === "eligible")) {
      requireThat(hash(readFileSync(path.join(root, "config/targets.json"))) === report.canonicalConfigurationSha256
        && hash(readFileSync(path.join(root, "artifacts/sdk-build/targets.json"))) === report.selectedConfigurationSha256
        && hash(readFileSync(buildFile)) === report.selectedBuildSha256
        && hash(readFileSync(libraryFile)) === report.libraryBuildSha256,
      "Configuration or selected/full library build changed during archive");
      const source = path.join(root, entry.source), destination = path.join(archive, entry.source);
      const current = present(source) ? inspect(root, projects.get(entry.target), history) : undefined;
      const receiptChanged = entry.evidence.some(item => item.receipt
        && (!existsSync(path.join(root, item.receipt)) || hash(readFileSync(path.join(root, item.receipt))) !== item.receiptSha256));
      const referenced = liveReferences(root, selectedPlan).some(reference => within(source, reference.to));
      if (!current || current.reasons.length || receiptChanged || referenced
        || JSON.stringify(current.files) !== JSON.stringify(entry.files) || JSON.stringify(current.evidence) !== JSON.stringify(entry.evidence)) {
        entry.status = "retained"; entry.reasons.push("Output or ownership evidence changed before archive"); record(); continue;
      }
      requireThat(!existsSync(destination), `Archive destination already exists: ${destination}`);
      mkdirSync(path.dirname(destination), { recursive: true });
      entry.status = "moving";
      entry.destination = slash(path.relative(root, destination));
      record();
      renameSync(source, destination);
      entry.status = "archived";
      record();
    }
    report.status = report.entries.some(entry => entry.status === "retained") ? "completed-with-retained-output" : "completed";
  } catch (error) { report.status = "failed"; report.error = error.message; throw error; }
  finally { report.finishedAtUtc = new Date().toISOString(); record(); }
  return report;
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  try {
    const args = process.argv.slice(2);
    requireThat(args.length === 0 || args.length === 1 && args[0] === "--dry-run", "Usage: node scripts/archive-sdk-output.mjs [--dry-run]");
    const report = archiveSdkOutput({ dryRun: args.includes("--dry-run") });
    console.log(JSON.stringify({ status: report.status, archive: report.archive,
      eligible: report.entries.filter(entry => entry.status === "eligible").length,
      archived: report.entries.filter(entry => entry.status === "archived").length,
      retained: report.entries.filter(entry => entry.status === "retained").map(({ target, reasons }) => ({ target, reasons })) }, null, 2));
  } catch (error) { console.error(`CloudEdge output archive failed: ${error.message}`); process.exitCode = 1; }
}
