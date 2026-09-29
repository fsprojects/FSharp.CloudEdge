import assert from "node:assert/strict";
import { createHash } from "node:crypto";
import { existsSync, mkdirSync, mkdtempSync, readFileSync, rmSync, symlinkSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import test from "node:test";
import { archiveSdkOutput, planSdkArchive } from "../scripts/archive-sdk-output.mjs";
import { planProjects } from "../scripts/projects.mjs";

const hash = bytes => createHash("sha256").update(bytes).digest("hex");
function fixture(t, names) {
  const root = mkdtempSync(path.join(os.tmpdir(), "cloudedge-sdk-archive-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  const write = (file, content) => {
    const absolute = path.join(root, file);
    mkdirSync(path.dirname(absolute), { recursive: true });
    writeFileSync(absolute, typeof content === "string" ? content : JSON.stringify(content));
  };
  const config = { schemaVersion: 2, toolchain: { typescript: "7.1.0-dev.20260902.1" },
    xantham: { tool: "xantham" }, profiles: [{ id: "sdk", directory: "profiles/sdk" }],
    projectDefaults: { targetFramework: "net8.0", packages: { "Fable.Core": "5.2.0", "Xantham.Fable.Core": "0.1.0" } },
    targets: ["selected", ...names].map(id => ({ id, package: id, version: "1.0.0", profile: "sdk", dependencies: [],
      outputDirectory: `src/Runtime/Services/${id}`, files: [`Bindings.${id}.fs`], generator: { module: `Bindings.${id}` } })) };
  const selected = { ...config, targets: [config.targets[0]] };
  const sdkBuild = { status: "compiled", requested: ["selected"],
    generation: [{ target: "selected", status: "generated" }],
    compiled: [{ target: "selected", generationSha256: hash("accepted generation") }] };
  const libraryBuild = { status: "compiled", sdk: sdkBuild, build: { exitCode: 0 },
    crossLibraryConsumers: { status: "compiled", projects: ["tests/SDKComposition/SDKComposition.fsproj"] } };
  write("config/targets.json", config);
  write("artifacts/sdk-build/targets.json", selected);
  write("artifacts/sdk-build/latest.json", sdkBuild);
  write("artifacts/library-build/latest.json", libraryBuild);
  const projects = new Map(planProjects(config, root).projects.map(project => [project.id, project]));
  for (const project of projects.values()) {
    write(path.relative(root, project.file), project.content);
    const text = `module Bindings.${project.id}\n`;
    write(path.relative(root, project.sources[0]), text);
    write(`artifacts/${project.id}/generation.json`, { target: project.id, package: project.id, version: "1.0.0",
      generator: project.target.generator, sourceSha256: { [project.target.files[0]]: hash(text) } });
  }
  return { root, config, selected, projects, write, sdkBuild, libraryBuild };
}

test("archive moves verified retired output and its build contents while retaining edits, unknown files and links", t => {
  const f = fixture(t, ["clean", "history", "edited", "unowned", "linked", "projectedited"]);
  const directory = id => f.projects.get(id).target.outputDirectory;
  f.write(directory("clean") + "/bin/Debug/result.dll", "generated binary");
  f.write(directory("edited") + "/Bindings.edited.fs", "module UserEdited\n");
  f.write(directory("unowned") + "/notes.txt", "keep this note");
  f.write("outside.txt", "do not move or follow");
  symlinkSync(path.join(f.root, "outside.txt"), path.join(f.root, directory("linked"), "linked.fs"));
  const historical = f.projects.get("history").content + "<!-- Earlier generated reference plan. -->\n";
  f.write(path.relative(f.root, f.projects.get("history").file), historical);
  f.write("artifacts/projects/previous.json", { checked: [{ project: path.relative(f.root, f.projects.get("history").file), projectSha256: hash(historical) }] });
  f.write(path.relative(f.root, f.projects.get("projectedited").file), "<!-- User project -->\n");
  const beforeConfig = readFileSync(path.join(f.root, "config/targets.json"), "utf8");
  const planned = archiveSdkOutput({ root: f.root, dryRun: true });
  assert.equal(planned.entries.filter(entry => entry.status === "eligible").length, 2);
  assert.ok(existsSync(path.join(f.root, directory("clean"))));
  assert.equal(existsSync(path.join(f.root, "artifacts/sdk-output-archive")), false);
  const report = archiveSdkOutput({ root: f.root });
  assert.equal(report.status, "completed-with-retained-output");
  assert.deepEqual(report.entries.filter(entry => entry.status === "archived").map(entry => entry.target), ["clean", "history"]);
  for (const id of ["edited", "unowned", "linked", "projectedited", "selected"]) assert.ok(existsSync(path.join(f.root, directory(id))));
  const archived = report.entries.find(entry => entry.target === "clean");
  assert.equal(existsSync(path.join(f.root, archived.source)), false);
  assert.equal(readFileSync(path.join(f.root, archived.destination, "bin/Debug/result.dll"), "utf8"), "generated binary");
  assert.equal(readFileSync(path.join(f.root, "outside.txt"), "utf8"), "do not move or follow");
  assert.equal(readFileSync(path.join(f.root, "config/targets.json"), "utf8"), beforeConfig);
  assert.ok(existsSync(path.join(f.root, "artifacts/clean/generation.json")));
  const saved = JSON.parse(readFileSync(path.join(f.root, report.archive, "inventory.json"), "utf8"));
  assert.equal(saved.entries.find(entry => entry.target === "history").evidence[1].kind, "checked-project-receipt");
  assert.equal(saved.entries.find(entry => entry.target === "clean").files.find(file => file.path.endsWith("result.dll")).sha256, hash("generated binary"));
  assert.equal(saved.selectedBuildSha256, hash(readFileSync(path.join(f.root, report.archive, "sdk-build.json"))));
  assert.equal(saved.libraryBuildSha256, hash(readFileSync(path.join(f.root, report.archive, "library-build.json"))));
  assert.deepEqual(JSON.parse(readFileSync(path.join(f.root, report.archive, "library-build.json"), "utf8")), f.libraryBuild);
});

test("archive retains retired projects still referenced by a consumer or solution", t => {
  const f = fixture(t, ["consumerref", "solutionref"]);
  const consumerProject = f.projects.get("consumerref").target.outputDirectory + "/Bindings.consumerref.fsproj";
  const solutionProject = f.projects.get("solutionref").target.outputDirectory + "/Bindings.solutionref.fsproj";
  f.write("tests/Consumer/Consumer.fsproj", `<Project><ItemGroup><ProjectReference Include="../../${consumerProject}" /></ItemGroup></Project>`);
  f.write("Custom.slnx", `<Solution><Project Path="${solutionProject}" /></Solution>`);
  const report = planSdkArchive(f.root);
  assert.equal(report.entries.length, 2);
  assert.ok(report.entries.every(entry => entry.status === "retained" && entry.reasons.some(reason => reason.startsWith("Still referenced"))));
});

test("archive requires a successful build for exactly the selected target set", t => {
  const f = fixture(t, ["clean"]);
  f.write("artifacts/sdk-build/latest.json", { status: "planned", requested: ["selected"] });
  assert.throws(() => archiveSdkOutput({ root: f.root }), /successful build/);
  f.write("artifacts/sdk-build/latest.json", { status: "compiled", requested: ["clean"] });
  assert.throws(() => archiveSdkOutput({ root: f.root }), /successful build/);
  assert.ok(existsSync(path.dirname(f.projects.get("clean").file)));
  assert.equal(existsSync(path.join(f.root, "artifacts/sdk-output-archive")), false);
});

test("archive rejects a missing, partial or failed complete library build before moving output", t => {
  const f = fixture(t, ["clean"]);
  rmSync(path.join(f.root, "artifacts/library-build/latest.json"));
  assert.throws(() => archiveSdkOutput({ root: f.root }), /successful complete library build/);
  for (const incomplete of [
    { ...f.libraryBuild, status: "compiling" },
    { ...f.libraryBuild, status: "failed", failedStage: "hawaii" },
    { ...f.libraryBuild, build: { exitCode: 1 } },
    { ...f.libraryBuild, crossLibraryConsumers: { status: "pending" } },
  ]) {
    f.write("artifacts/library-build/latest.json", incomplete);
    assert.throws(() => archiveSdkOutput({ root: f.root }), /successful complete library build/);
    assert.ok(existsSync(path.dirname(f.projects.get("clean").file)));
    assert.equal(existsSync(path.join(f.root, "artifacts/sdk-output-archive")), false);
  }
});

test("archive rejects stale full-library evidence even when the selected target IDs still match", t => {
  const f = fixture(t, ["clean"]);
  for (const staleSdk of [
    { ...f.sdkBuild, requested: ["clean"] },
    { ...f.sdkBuild, compiled: [{ target: "selected", generationSha256: hash("older generation") }] },
    undefined,
  ]) {
    f.write("artifacts/library-build/latest.json", { ...f.libraryBuild, sdk: staleSdk });
    assert.throws(() => archiveSdkOutput({ root: f.root }), /SDK evidence differs/);
    assert.ok(existsSync(path.dirname(f.projects.get("clean").file)));
    assert.equal(existsSync(path.join(f.root, "artifacts/sdk-output-archive")), false);
  }
});

test("archive compares parsed SDK evidence independent of JSON property order", t => {
  const f = fixture(t, ["clean"]);
  const sdk = Object.fromEntries(Object.entries(f.sdkBuild).reverse());
  sdk.compiled = f.sdkBuild.compiled.map(record => Object.fromEntries(Object.entries(record).reverse()));
  f.write("artifacts/library-build/latest.json", { ...f.libraryBuild, sdk });
  assert.equal(archiveSdkOutput({ root: f.root }).status, "completed");
});
