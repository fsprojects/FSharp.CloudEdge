import assert from "node:assert/strict";
import { createHash } from "node:crypto";
import { existsSync, mkdtempSync, mkdirSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import test from "node:test";
import { buildProjects, checkProjects, planProjects, reconcileProjects, writeProjects } from "../scripts/projects.mjs";
import { catalogCandidates, declarationReferences } from "../scripts/catalogs.mjs";
import { inspectInputOverlays, prepareInputOverlays } from "../scripts/input-overlays.mjs";

const clone = (value) => structuredClone(value);
const hash = (text) => createHash("sha256").update(text).digest("hex");
const target = (id, module, profile = "runtime", dependencies = []) => ({
  id, package: `@example/${id}`, version: "1.2.3", profile, dependencies,
  outputDirectory: `src/${module}`, files: [`${module}.fs`], generator: { module },
});
const configuration = () => ({
  schemaVersion: 2,
  toolchain: { typescript: "7.1.0-dev.20260902.1" },
  xantham: { tool: "xantham" },
  profiles: [{ id: "runtime", directory: "." }, { id: "ai", directory: "profiles/ai" }],
  projectDefaults: { targetFramework: "net8.0", packages: { "Fable.Core": "5.2.0", "Fable.Browser.Dom": "2.20.0", "Xantham.Fable.Core": "0.1.0-alpha.1" } },
  targets: [target("workers", "FSharp.CloudEdge.Workers")],
});

function temporary(t) {
  const root = mkdtempSync(path.join(os.tmpdir(), "cloudedge-projects-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  return root;
}

function generated(plan) {
  writeProjects(plan);
  const generatorTool = { packageId: "Xantham.Cli", command: "xantham", version: "0.1.0-local.fixture",
    packageSha256: hash("package"), payloadSha256: { "xantham.dll": hash("generator") } };
  mkdirSync(path.join(plan.root, "config"), { recursive: true });
  writeFileSync(path.join(plan.root, "config/tool-packages.json"), JSON.stringify({ schemaVersion: 1, tools: { xantham: generatorTool } }));
  for (const profile of plan.config.profiles) {
    const directory = path.resolve(plan.root, profile.directory);
    const targets = plan.config.targets.filter((target) => target.profile === profile.id);
    mkdirSync(directory, { recursive: true });
    writeFileSync(path.join(directory, "package.json"), JSON.stringify({ name: `profile-${profile.id}`, version: "1.0.0", dependencies: Object.fromEntries(targets.map((target) => [target.package, target.version])) }));
    writeFileSync(path.join(directory, "package-lock.json"), JSON.stringify({ lockfileVersion: 3, packages: Object.fromEntries(targets.map((target) => [`node_modules/${target.package}`, { version: target.version }])) }));
    for (const target of targets) {
      const packageDirectory = path.join(directory, "node_modules", target.package);
      mkdirSync(packageDirectory, { recursive: true });
      writeFileSync(path.join(packageDirectory, "package.json"), JSON.stringify({ name: target.package, version: target.version }));
    }
  }
  for (const project of plan.projects) {
    const sources = {};
    project.sources.forEach((file, index) => {
      mkdirSync(path.dirname(file), { recursive: true });
      const text = `module ${project.target.generator.module}\n// ${index}\n`;
      writeFileSync(file, text);
      sources[project.target.files[index]] = hash(text);
    });
    const artifacts = path.join(plan.root, "artifacts", project.id);
    mkdirSync(artifacts, { recursive: true });
    let declarationCatalogSha256;
    if (project.target.generator.declarationCatalog) {
      const catalog = JSON.stringify({ schemaVersion: 1, owner: project.target.generator.module, owners: [{ name: project.target.generator.module, dependencies: [] }], declarations: [] });
      writeFileSync(path.join(artifacts, "declarations.json"), catalog);
      declarationCatalogSha256 = hash(catalog);
    }
    const profile = plan.config.profiles.find((profile) => profile.id === project.profile);
    const profileDirectory = path.resolve(plan.root, profile.directory);
    const inputOverlays = inspectInputOverlays({ root: plan.root, profileDirectory });
    const inputPackageDirectory = path.join(profileDirectory, "node_modules", project.target.package);
    const declarationReferenceSha256 = Object.fromEntries(declarationReferences(project.target, plan.root, inputPackageDirectory).map(({ key: reference, file }) => {
      if (!existsSync(file)) {
        mkdirSync(path.dirname(file), { recursive: true });
        writeFileSync(file, JSON.stringify({ owner: reference, declarations: [] }));
      }
      return [reference, hash(readFileSync(file))];
    }));
    writeFileSync(path.join(artifacts, "generation.json"), JSON.stringify({
      target: project.id, catalogCandidates: catalogCandidates(project.target), profile: project.profile, package: project.target.package, version: project.target.version,
      typescript: plan.config.toolchain.typescript, generator: project.target.generator, sourceSha256: sources,
      generatorTool,
      profileLockSha256: inputOverlays.profileLockSha256, profileManifestSha256: inputOverlays.profileManifestSha256,
      inputOverlays, inputPackageDirectory, declarationCatalogSha256, declarationReferenceSha256,
    }));
  }
}

function changeReceipt(plan, change, id = "workers") {
  const file = path.join(plan.root, "artifacts", id, "generation.json");
  const receipt = JSON.parse(readFileSync(file, "utf8"));
  change(receipt);
  writeFileSync(file, JSON.stringify(receipt));
}

test("projects preserve group-before-entry source order and pinned consumer dependencies", (t) => {
  const config = configuration();
  config.targets[0].files.unshift("groups/TypeScript.Lib.fs");
  const plan = planProjects(config, temporary(t));
  const text = plan.projects[0].content;
  assert.ok(text.indexOf('Include="groups/TypeScript.Lib.fs"') < text.indexOf('Include="FSharp.CloudEdge.Workers.fs"'));
  assert.match(text, /Fable.Core" Version="5.2.0"/);
  assert.match(text, /Fable.Browser.Dom" Version="2.20.0"/);
  assert.match(text, /PackageReference Include="Xantham.Fable.Core" Version="0.1.0-alpha.1"/);
  assert.doesNotMatch(text, /ProjectReference|XanthamFableCoreProject/);
  assert.deepEqual(plan.supportPackage, { id: "Xantham.Fable.Core", version: "0.1.0-alpha.1" });
  assert.deepEqual(plan, planProjects(clone(config), plan.root));
});

test("explicit cross-profile dependencies compile before their consumers", (t) => {
  const config = configuration();
  config.targets.unshift(target("agents", "FSharp.CloudEdge.Agents", "ai", ["workers"]));
  const plan = planProjects(config, temporary(t));
  assert.deepEqual(plan.projects.map((project) => project.id), ["workers", "agents"]);
  assert.match(plan.projects[1].content, /ProjectReference Include="..\/FSharp.CloudEdge.Workers\/FSharp.CloudEdge.Workers.fsproj"/);
  assert.ok(plan.solutionContent.indexOf("Workers.fsproj") < plan.solutionContent.indexOf("Agents.fsproj"));
  assert.equal(plan.projects[1].profile, "ai");
});

test("solution folders follow the organized source parent directories", (t) => {
  const config = configuration();
  config.targets[0].outputDirectory = "src/Core/Workers";
  const agents = target("agents-mcp", "FSharp.CloudEdge.AgentsMcp", "ai", ["workers"]);
  agents.outputDirectory = "src/Runtime/Agents/Mcp";
  config.targets.push(agents);
  const plan = planProjects(config, temporary(t));
  assert.match(plan.solutionContent, /Folder Name="\/Core\/"/);
  assert.match(plan.solutionContent, /Folder Name="\/Runtime\/Agents\/"/);
  assert.match(plan.projects[1].content, /ProjectReference Include="..\/..\/..\/Core\/Workers\/FSharp.CloudEdge.Workers.fsproj"/);
});

for (const [name, change, error] of [
  ["unknown dependency", (c) => { c.targets[0].dependencies = ["missing"]; }, /unknown dependency/i],
  ["self-cycle", (c) => { c.targets[0].dependencies = ["workers"]; }, /workers -> workers/],
  ["indirect cycle", (c) => { c.targets.push(target("agents", "FSharp.CloudEdge.Agents", "ai", ["workers"])); c.targets[0].dependencies = ["agents"]; }, /workers -> agents -> workers/],
  ["unknown profile", (c) => { c.targets[0].profile = "missing"; }, /unknown profile/],
  ["duplicate dependency", (c) => { c.targets[0].dependencies = ["missing", "missing"]; }, /unique target IDs/],
  ["range NuGet pin", (c) => { c.projectDefaults.packages["Fable.Core"] = "[5.2.0,6.0.0)"; }, /exact NuGet pin/],
  ["missing Fable pin", (c) => { delete c.projectDefaults.packages["Fable.Core"]; }, /explicitly pin Fable.Core/],
  ["missing support pin", (c) => { delete c.projectDefaults.packages["Xantham.Fable.Core"]; }, /explicitly pin Xantham.Fable.Core/],
  ["range support pin", (c) => { c.projectDefaults.packages["Xantham.Fable.Core"] = "0.1.*"; }, /exact NuGet pin/],
  ["sibling support source", (c) => { c.projectDefaults.supportProject = "../Xantham/support.fsproj"; }, /unknown field/],
  ["escaping output", (c) => { c.targets[0].outputDirectory = "../outside"; }, /escapes/],
  ["escaping source", (c) => { c.targets[0].files = ["../outside.fs"]; }, /escapes/],
  ["source after entry", (c) => { c.targets[0].files.push("groups/Core.fs"); }, /group files must precede/],
  ["duplicate module", (c) => { c.targets.push(target("other", "FSharp.CloudEdge.Workers")); }, /collides/],
  ["module-prefix collision", (c) => { c.targets.push(target("other", "FSharp.CloudEdge.Workers.Rpc")); }, /collides/],
  ["duplicate source owner", (c) => { const other = target("other", "FSharp.CloudEdge.Other"); other.outputDirectory = c.targets[0].outputDirectory; other.files = [...c.targets[0].files]; c.targets.push(other); }, /source already owned/],
  ["nonboolean catalog flag", (c) => { c.targets[0].generator.declarationCatalog = "true"; }, /declarationCatalog must be a boolean/],
  ["duplicate catalog references", (c) => { c.targets[0].generator.declarationReferences = ["producer.json", "producer.json"]; }, /unique nonempty paths/],
  ["empty catalog reference", (c) => { c.targets[0].generator.declarationReferences = [""]; }, /unique nonempty paths/],
]) {
  test(`planning refuses ${name}`, (t) => {
    const config = configuration();
    change(config);
    assert.throws(() => planProjects(config, temporary(t)), error);
  });
}

test("writing scaffolds projects without claiming generated source exists", (t) => {
  const plan = planProjects(configuration(), temporary(t));
  writeProjects(plan);
  assert.ok(existsSync(plan.solution));
  assert.ok(!existsSync(plan.projects[0].sources[0]));
  assert.throws(() => checkProjects(plan), /generation provenance is missing/);
});

test("checking authenticates source and exact configuration provenance", (t) => {
  const plan = planProjects(configuration(), temporary(t));
  generated(plan);
  const checked = checkProjects(plan);
  assert.equal(checked[0].target, "workers");
  const source = plan.projects[0].sources[0];
  writeFileSync(source, readFileSync(source, "utf8") + "let changed = 1\n");
  assert.throws(() => checkProjects(plan), /source hash differs/);
});

test("checking rejects receipts from a previous generator package without requiring tool installation", (t) => {
  const plan = planProjects(configuration(), temporary(t));
  generated(plan);
  assert.equal(checkProjects(plan).length, 1);
  const file = path.join(plan.root, "config/tool-packages.json");
  const lock = JSON.parse(readFileSync(file, "utf8"));
  lock.tools.xantham.version = "0.1.0-local.updated";
  writeFileSync(file, JSON.stringify(lock));
  assert.throws(() => checkProjects(plan), /generation tool differs from the pinned package/);
});

for (const [file, error] of [["package-lock.json", /profile lockfile hash differs/], ["package.json", /profile manifest hash differs/]]) {
  test(`checking refuses changed installed profile ${file}`, (t) => {
    const plan = planProjects(configuration(), temporary(t));
    generated(plan);
    const profileFile = path.join(plan.root, file);
    const content = JSON.parse(readFileSync(profileFile, "utf8"));
    content.description = "profile changed after generation";
    writeFileSync(profileFile, JSON.stringify(content));
    assert.throws(() => checkProjects(plan), error);
    assert.equal(existsSync(path.join(plan.root, "artifacts/inputs")), false);
  });
}

test("checking refuses changed overlay policy even when the profile lock is unchanged", (t) => {
  const plan = planProjects(configuration(), temporary(t));
  generated(plan);
  mkdirSync(path.join(plan.root, "config"), { recursive: true });
  writeFileSync(path.join(plan.root, "config/declaration-overlays.json"), JSON.stringify({ schemaVersion: 1, rules: [], review: "policy revised" }));
  assert.throws(() => checkProjects(plan), /input overlay fingerprint differs/);
  assert.equal(existsSync(path.join(plan.root, "artifacts/inputs")), false);
});

for (const mutation of ["changed output", "missing output", "missing hash"]) {
  test(`checking refuses declaration catalog ${mutation}`, (t) => {
    const config = configuration();
    config.targets[0].generator.declarationCatalog = true;
    const plan = planProjects(config, temporary(t));
    generated(plan);
    assert.equal(checkProjects(plan).length, 1);
    const file = path.join(plan.root, "artifacts/workers/declarations.json");
    if (mutation === "changed output") writeFileSync(file, JSON.stringify({ owner: "changed", declarations: [] }));
    if (mutation === "missing output") rmSync(file);
    if (mutation === "missing hash") changeReceipt(plan, (receipt) => { delete receipt.declarationCatalogSha256; });
    assert.throws(() => checkProjects(plan), /declaration catalog (?:hash|output) differs/);
  });
}

test("checking refuses a catalog artifact when catalog generation is disabled", (t) => {
  const plan = planProjects(configuration(), temporary(t));
  generated(plan);
  writeFileSync(path.join(plan.root, "artifacts/workers/declarations.json"), "{}\n");
  assert.throws(() => checkProjects(plan), /declaration catalog output differs/);
});

for (const mutation of ["changed reference", "missing reference", "extra inventory", "missing inventory entry"]) {
  test(`checking refuses declaration reference ${mutation}`, (t) => {
    const config = configuration();
    const reference = "../../../catalog-inputs/producer.json";
    config.targets[0].generator.declarationReferences = [reference];
    const plan = planProjects(config, temporary(t));
    generated(plan);
    assert.equal(checkProjects(plan).length, 1);
    const file = path.join(plan.root, "catalog-inputs/producer.json");
    if (mutation === "changed reference") writeFileSync(file, JSON.stringify({ owner: "changed", declarations: [] }));
    if (mutation === "missing reference") rmSync(file);
    if (mutation === "extra inventory") changeReceipt(plan, (receipt) => { receipt.declarationReferenceSha256["unexpected.json"] = "0".repeat(64); });
    if (mutation === "missing inventory entry") changeReceipt(plan, (receipt) => { delete receipt.declarationReferenceSha256[reference]; });
    assert.throws(() => checkProjects(plan), /declaration reference (?:hash differs|is missing|inventory differs)/);
  });
}

test("checks inspect vanilla overlay inputs and resolve references beside the original package without creating snapshots", (t) => {
  const config = configuration();
  const reference = "../../../catalog-inputs/producer.json";
  config.targets[0].generator.declarationReferences = [reference];
  const plan = planProjects(config, temporary(t));
  generated(plan);
  const packageDirectory = path.join(plan.root, "node_modules/@example/workers");
  const before = 'export { Value } from "@src/types";\n';
  const after = 'export { Value } from "./types";\n';
  const corroboration = "export interface Value { id: string; }\n";
  writeFileSync(path.join(packageDirectory, "index.d.ts"), before);
  writeFileSync(path.join(packageDirectory, "types.d.ts"), corroboration);
  const integrity = `sha512-${Buffer.alloc(64, 1).toString("base64")}`;
  const lockFile = path.join(plan.root, "package-lock.json");
  const lock = JSON.parse(readFileSync(lockFile, "utf8"));
  lock.packages["node_modules/@example/workers"].integrity = integrity;
  writeFileSync(lockFile, JSON.stringify(lock));
  mkdirSync(path.join(plan.root, "config"), { recursive: true });
  writeFileSync(path.join(plan.root, "config/declaration-overlays.json"), JSON.stringify({ schemaVersion: 1, rules: [{
    id: "test-alias", packageName: "@example/workers", packageVersion: "1.2.3", tarballIntegrity: integrity,
    replacements: [{ file: "index.d.ts", beforeSha256: hash(before), afterSha256: hash(after),
      search: before.trim(), replace: after.trim(), occurrences: 1,
      corroboratingFiles: [{ file: "types.d.ts", sha256: hash(corroboration) }] }],
  }] }));
  const prepared = prepareInputOverlays({ root: plan.root, profileDirectory: plan.root });
  const inputPackageDirectory = path.join(prepared.profileDirectory, "node_modules/@example/workers");
  changeReceipt(plan, (receipt) => Object.assign(receipt, {
    profileLockSha256: prepared.profileLockSha256, profileManifestSha256: prepared.profileManifestSha256,
    inputOverlays: prepared, inputPackageDirectory,
  }));
  const misleadingReference = path.resolve(inputPackageDirectory, reference);
  mkdirSync(path.dirname(misleadingReference), { recursive: true });
  writeFileSync(misleadingReference, "incorrect snapshot-relative reference\n");
  assert.equal(checkProjects(plan).length, 1);
  rmSync(path.join(plan.root, "artifacts/inputs"), { recursive: true });
  assert.equal(checkProjects(plan).length, 1);
  assert.equal(existsSync(path.join(plan.root, "artifacts/inputs")), false);
  assert.equal(readFileSync(path.join(packageDirectory, "index.d.ts"), "utf8"), before);
  writeFileSync(path.join(packageDirectory, "index.d.ts"), before + "// changed vanilla declaration\n");
  assert.throws(() => checkProjects(plan), /Declaration overlay:.*SHA-256 mismatch/);
});

for (const [name, alter, error] of [
  ["profile", (p) => { p.profile = "ai"; }, /profile differs/],
  ["package version", (p) => { p.version = "9.0.0"; }, /input differs/],
  ["compiler pin", (p) => { p.typescript = "7.0.0"; }, /configuration differs/],
  ["entry configuration", (p) => { p.generator.entry = "different.d.ts"; }, /configuration differs/],
  ["source inventory", (p) => { p.sourceSha256["extra.fs"] = "0".repeat(64); }, /inventory differs/],
  ["missing lock metadata", (p) => { delete p.profileLockSha256; }, /profile lockfile hash differs/],
  ["missing manifest metadata", (p) => { delete p.profileManifestSha256; }, /profile manifest hash differs/],
  ["missing overlay metadata", (p) => { delete p.inputOverlays; }, /input overlay fingerprint differs/],
  ["overlay fingerprint", (p) => { p.inputOverlays.fingerprint = "0".repeat(64); }, /input overlay fingerprint differs/],
  ["input package directory", (p) => { p.inputPackageDirectory = "/another/profile/node_modules/@example/workers"; }, /input package directory differs/],
  ["missing reference inventory", (p) => { delete p.declarationReferenceSha256; }, /declaration reference inventory differs/],
]) {
  test(`checking refuses stale ${name}`, (t) => {
    const plan = planProjects(configuration(), temporary(t));
    generated(plan);
    const file = path.join(plan.root, "artifacts/workers/generation.json");
    const provenance = JSON.parse(readFileSync(file, "utf8"));
    alter(provenance);
    writeFileSync(file, JSON.stringify(provenance));
    assert.throws(() => checkProjects(plan), error);
  });
}

test("a selected check includes dependencies but does not require unrelated generation", (t) => {
  const config = configuration();
  config.targets.push(target("agents", "FSharp.CloudEdge.Agents", "ai", ["workers"]));
  config.profiles.push({ id: "unused", directory: "profiles/unused" });
  config.targets.push(target("unrelated", "FSharp.CloudEdge.Unrelated", "unused"));
  const plan = planProjects(config, temporary(t));
  generated(plan);
  rmSync(path.join(plan.root, "artifacts/unrelated/generation.json"));
  rmSync(path.join(plan.root, "profiles/unused"), { recursive: true });
  assert.deepEqual(checkProjects(plan, ["agents"]).map((item) => item.target), ["workers", "agents"]);
  rmSync(path.join(plan.root, "artifacts/workers/generation.json"));
  assert.throws(() => checkProjects(plan, ["agents"]), /workers: generation provenance is missing/);
});

test("checking refuses drift in generated project or aggregate solution", (t) => {
  const plan = planProjects(configuration(), temporary(t));
  generated(plan);
  writeFileSync(plan.projects[0].file, plan.projects[0].content.replace('Version="5.2.0"', 'Version="5.1.0"'));
  assert.throws(() => checkProjects(plan), /project differs/);
  writeProjects(plan);
  writeFileSync(plan.solution, "<Solution />\n");
  assert.throws(() => checkProjects(plan), /solution differs/);
});

test("a failed compiler invocation remains a failed gate with durable evidence", (t) => {
  const plan = planProjects(configuration(), temporary(t));
  generated(plan);
  // An absent executable exercises the process failure boundary without invoking a compiler.
  assert.throws(() => buildProjects(plan, ["workers"], path.join(plan.root, "absent-dotnet")), /Binding compilation failed/);
  const report = JSON.parse(readFileSync(path.join(plan.root, "artifacts/projects/latest.json"), "utf8"));
  assert.equal(report.succeeded, false);
  assert.equal(report.checked[0].target, "workers");
  assert.equal(report.builds.length, 1);
  assert.match(report.builds[0].error, /ENOENT/);
  assert.ok(existsSync(report.log));
});

function catalogFamily(t, extras = []) {
  const config = configuration();
  config.targets = ["owner", "sibling", "consumer"].map((id, index) => ({ ...target(id, `Example.${id}`), package: "@example/sdk", generator: { module: `Example.${id}`, declarationCatalog: true }, ...(index ? { catalogCandidates: [index === 1 ? "owner" : "sibling"] } : {}) }));
  config.targets.push(...extras);
  const plan = planProjects(config, temporary(t));
  generated(plan);
  const owner = { name: "Example.owner", dependencies: [] };
  const sibling = { name: "Example.sibling", dependencies: [] };
  replaceCatalog(plan, "sibling", [owner, sibling]);
  replaceCatalog(plan, "consumer", [owner, sibling, { name: "Example.consumer", dependencies: ["Example.owner"] }]);
  return plan;
}

function replaceCatalog(plan, id, owners, owner = `Example.${id}`) {
  const file = path.join(plan.root, "artifacts", id, "declarations.json");
  writeFileSync(file, JSON.stringify({ schemaVersion: 1, owner, owners, declarations: [] }));
  const target = plan.config.targets.find((entry) => entry.id === id);
  const profile = plan.config.profiles.find((entry) => entry.id === target.profile);
  changeReceipt(plan, (receipt) => {
    receipt.declarationCatalogSha256 = hash(readFileSync(file));
    receipt.declarationReferenceSha256 = Object.fromEntries(declarationReferences(target, plan.root, path.join(plan.root, profile.directory, "node_modules", target.package)).map(({ key, file }) => [key, hash(readFileSync(file))]));
  }, id);
}

test("fresh scaffolds reconcile actual catalog owners after generation without referencing unused candidates", (t) => {
  const plan = catalogFamily(t);
  const consumer = plan.projects.find((entry) => entry.id === "consumer");
  assert.doesNotMatch(readFileSync(consumer.file, "utf8"), /Example.owner.fsproj/);
  assert.throws(() => checkProjects(plan, ["consumer"]), /project differs.*--write/);
  writeProjects(plan);
  assert.match(readFileSync(consumer.file, "utf8"), /Example.owner.fsproj/);
  assert.doesNotMatch(readFileSync(consumer.file, "utf8"), /Example.sibling.fsproj/);
  rmSync(plan.projects.find((entry) => entry.id === "sibling").sources[0]);
  assert.deepEqual(checkProjects(plan, ["consumer"]).map((entry) => [entry.target, entry.dependencies]), [["owner", []], ["consumer", ["owner"]]]);
  assert.deepEqual(reconcileProjects(plan).projects.find((entry) => entry.id === "consumer").dependencies, ["owner"]);
});

test("unused candidate receipt provenance is still authenticated before inherited owners are trusted", (t) => {
  const plan = catalogFamily(t);
  writeProjects(plan);
  changeReceipt(plan, (receipt) => { receipt.target = "owner"; }, "sibling");
  assert.throws(() => checkProjects(plan, ["consumer"]), /sibling: generation input differs/);
});

test("changed candidate policy requires fresh generation even when catalog bytes are unchanged", (t) => {
  const plan = catalogFamily(t);
  plan.config.targets[2].catalogCandidates = ["owner"];
  assert.throws(() => checkProjects(plan, ["consumer"]), /catalog candidates differ/);
});

test("catalog owner identity must match its authenticated target receipt", (t) => {
  const plan = catalogFamily(t);
  replaceCatalog(plan, "consumer", [{ name: "Example.owner", dependencies: [] }], "Example.owner");
  assert.throws(() => checkProjects(plan, ["consumer"]), /catalog owner differs/);
});

test("inherited owner dependencies must agree with authenticated producer catalogs", (t) => {
  const plan = catalogFamily(t);
  replaceCatalog(plan, "consumer", [
    { name: "Example.owner", dependencies: [] },
    { name: "Example.sibling", dependencies: ["Example.owner"] },
    { name: "Example.consumer", dependencies: ["Example.owner"] },
  ]);
  assert.throws(() => checkProjects(plan, ["consumer"]), /unauthenticated inherited catalog owner: Example.sibling/);
});

for (const [name, owners, error] of [
  ["unknown owner", [{ name: "Foreign", dependencies: [] }, { name: "Example.consumer", dependencies: ["Foreign"] }], /unknown project owner/],
  ["missing owner", [{ name: "Example.consumer", dependencies: ["Example.owner"] }], /missing owner/],
  ["owner cycle", [{ name: "Example.consumer", dependencies: ["Example.consumer"] }], /owner cycle/],
]) test(`catalog reconciliation rejects ${name}`, (t) => {
  const plan = catalogFamily(t);
  replaceCatalog(plan, "consumer", owners);
  assert.throws(() => checkProjects(plan, ["consumer"]), error);
});

test("a selected explicit dependency reconciles its own catalog dependencies transitively", (t) => {
  const plan = catalogFamily(t, [target("wrapper", "Example.wrapper", "runtime", ["consumer"])]);
  writeProjects(plan);
  assert.deepEqual(checkProjects(plan, ["wrapper"]).map((entry) => entry.target), ["owner", "consumer", "wrapper"]);
});
