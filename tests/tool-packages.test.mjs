import assert from "node:assert/strict";
import { executableScript } from "./fake-executable.mjs";
import { spawnSync } from "node:child_process";
import { fileURLToPath } from "node:url";
import { createHash } from "node:crypto";
import { mkdirSync, mkdtempSync, readFileSync, rmSync, symlinkSync, writeFileSync } from "node:fs";
import { scratchDirectory } from "./scratch.mjs";
import path from "node:path";
import test from "node:test";
import { zipSync } from "fflate";
import { installedTool, packageCache, resolveTool, resolveProjectionTool, sourceToolConfiguration, sameTool } from "../scripts/tool-packages.mjs";

const hash = (bytes) => createHash("sha256").update(bytes).digest("hex");
const write = (file, bytes) => { mkdirSync(path.dirname(file), { recursive: true }); writeFileSync(file, bytes); };
const json = (file, value) => write(file, JSON.stringify(value));

test("source adapter configuration has one shared pin and rejects conflicting projection ownership", () => {
  const source = { project: "generators/adapter/Adapter.fsproj", xanthamRevision: "0".repeat(40) };
  const configuration = { xantham: { source } };
  assert.equal(sourceToolConfiguration(configuration, { id: "ordinary" }), source);
  assert.equal(sourceToolConfiguration(configuration, { id: "projected", projection: { selections: [{}] } }), source);
  assert.deepEqual(sourceToolConfiguration({ xantham: {} }, { projection: { ...source, selections: [{}] } }), source);
  assert.throws(() => sourceToolConfiguration(configuration, { id: "conflict", projection: { ...source, xanthamRevision: "1".repeat(40) } }), /differs from shared/);
  assert.throws(() => sourceToolConfiguration({ xantham: { source: { ...source, extra: true } } }, {}), /must name a project/);
  assert.throws(() => sourceToolConfiguration({ xantham: {} }, { projection: { selections: [{}] } }), /requires a source adapter/);
  assert.throws(() => sourceToolConfiguration({ xantham: { source: { ...source, xanthamRevision: "develop" } } }, {}), /exact Xantham source revision/);
});

function fixture(t) {
  const root = mkdtempSync(path.join(scratchDirectory(), "cloudedge-tool-package-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  const definition = { packageId: "Example.Tool", command: "example", version: "1.2.3-local.test" };
  const directory = path.join(root, "nuget/example.tool", definition.version);
  const prefix = "tools/net8.0/any/";
  const payload = path.join(directory, prefix);
  const assembly = path.join(payload, "example.dll");
  const archive = path.join(directory, `example.tool.${definition.version}.nupkg`);
  const cache = path.join(root, "cli-home/.dotnet/toolResolverCache/1/example.tool");
  const manifest = path.join(root, ".config/dotnet-tools.json");
  const lock = path.join(root, "config/tool-packages.json");
  const data = {
    "DotnetToolSettings.xml": '<DotNetCliTool><Commands><Command Name="example" EntryPoint="example.dll" Runner="dotnet" /></Commands></DotNetCliTool>',
    "example.dll": "fixture assembly",
    "example.deps.json": '{"runtimeTarget":{"name":".NETCoreApp,Version=v8.0"}}',
    "example.runtimeconfig.json": '{"runtimeOptions":{"tfm":"net8.0"}}',
    "fr/Example.resources.dll": "fixture satellite assembly",
  };
  const packageBytes = zipSync(Object.fromEntries(Object.entries(data).map(([name, value]) => [prefix + name, Buffer.from(value)])));
  write(archive, packageBytes);
  for (const [name, bytes] of Object.entries(data)) write(path.join(payload, name), bytes);
  const row = { Version: definition.version, TargetFramework: "net10.0", RuntimeIdentifier: "any", Name: "example", Runner: "dotnet", PathToExecutable: assembly };
  json(cache, [row]);
  json(manifest, { version: 1, isRoot: true, tools: { "example.tool": { version: definition.version, commands: ["example"] } } });
  const identity = { ...definition, packageSha256: hash(packageBytes), payloadSha256: Object.fromEntries(Object.entries(data).map(([name, bytes]) => [name, hash(bytes)])) };
  json(lock, { schemaVersion: 1, tools: { example: identity } });
  const sdk = path.join(root, "sdk/10.0.400");
  json(path.join(sdk, "dotnet.runtimeconfig.json"), { runtimeOptions: { tfm: "net10.0" } });
  const dotnet = path.join(root, "dotnet");
  write(dotnet, `#!${process.execPath}\nconst args = process.argv.slice(2);\nif (args.join(' ') === '--info') console.log(' Base Path: ' + process.env.TEST_SDK);\nelse if (args.join(' ') === 'nuget locals global-packages --list') console.log('global-packages: ' + process.env.TEST_NUGET);\nelse process.exit(9);\n`);
  executableScript(t, dotnet);
  const env = { ...process.env, CLOUDEDGE_DOTNET: dotnet, NUGET_PACKAGES: path.join(root, "nuget"), DOTNET_CLI_HOME: path.join(root, "cli-home"), TEST_SDK: sdk, TEST_NUGET: path.join(root, "nuget") };
  return { root, definition, directory, payload, archive, cache, manifest, lock, assembly, row, data, identity, env, sdk };
}

test("production resolver verifies a pinned package and keeps normal dotnet tool run arguments", (t) => {
  const f = fixture(t);
  const tool = resolveTool(f.root, "example", f.env);
  assert.deepEqual(tool.identity, f.identity);
  assert.equal(tool.assembly, f.assembly);
  assert.deepEqual(tool.args, ["tool", "run", "example", "--"]);
  assert.ok(sameTool(tool, resolveTool(f.root, "example", f.env)));
});

test("initial acceptance verifies extracted payload against the package ZIP", (t) => {
  const f = fixture(t);
  assert.deepEqual(installedTool(f.root, "example", f.env, f.definition).identity, f.identity);
});

function runtimeFixture(t, runtimeIdentifier = "linux-x64", runner = "executable") {
  const f = fixture(t);
  const runtimeId = `Example.Tool.${runtimeIdentifier}`;
  const runtimeDirectory = path.join(f.root, "nuget", runtimeId.toLowerCase(), f.definition.version);
  const runtimePrefix = `tools/net10.0/${runtimeIdentifier}/`;
  const entryPoint = runner === "executable" ? "example" : "example.dll";
  const runtimePayload = {
    "DotnetToolSettings.xml": `<DotNetCliTool Version="2"><Commands><Command Name="example" EntryPoint="${entryPoint}" Runner="${runner}" /></Commands></DotNetCliTool>`,
    [entryPoint]: "fixture executable",
    "example.deps.json": "fixture dependencies",
  };
  const pack = (directory, id, data) => {
    const spec = `<package><metadata><id>${id}</id><version>${f.definition.version}</version></metadata></package>`;
    const archiveData = { [`${id}.nuspec`]: spec, ...data };
    const extracted = { [`${id.toLowerCase()}.nuspec`]: spec, ...data };
    const bytes = zipSync(Object.fromEntries(Object.entries(archiveData).map(([name, value]) => [name, Buffer.from(value)])));
    const archive = path.join(directory, `${id.toLowerCase()}.${f.definition.version}.nupkg`);
    rmSync(directory, { recursive: true, force: true });
    write(archive, bytes);
    for (const [name, value] of Object.entries(extracted)) write(path.join(directory, name), value);
    // NuGet generates these files; they are not archive payloads.
    write(path.join(directory, `${id}.nupkg`), bytes);
    write(`${archive}.sha512`, "cache digest");
    json(path.join(directory, ".nupkg.metadata"), { version: 2 });
    return { archive, packageSha256: hash(bytes), filesSha256: Object.fromEntries(Object.entries(extracted).map(([name, value]) => [name, hash(value)])) };
  };
  const pointer = pack(f.directory, f.definition.packageId, {
    "tools/net10.0/any/DotnetToolSettings.xml": `<DotNetCliTool Version="2"><Commands><Command Name="example" /></Commands><RuntimeIdentifierPackages><RuntimeIdentifierPackage RuntimeIdentifier="${runtimeIdentifier}" Id="${runtimeId}" /></RuntimeIdentifierPackages></DotNetCliTool>`,
    "fable-library-js/TaggedUnion.js": "pointer runtime library",
  });
  const runtime = pack(runtimeDirectory, runtimeId, {
    ...Object.fromEntries(Object.entries(runtimePayload).map(([name, value]) => [runtimePrefix + name, value])),
    "fable-library-js/TaggedUnion.js": "selected runtime library",
  });
  const assembly = path.join(runtimeDirectory, runtimePrefix, entryPoint);
  const row = { ...f.row, Runner: runner, PathToExecutable: assembly };
  json(f.cache, [row]);
  const identity = { ...f.definition, packageSha256: pointer.packageSha256,
    payloadSha256: Object.fromEntries(Object.entries(runtimePayload).map(([name, value]) => [name, hash(value)])),
    packageFilesSha256: pointer.filesSha256,
    runtimePackage: { packageId: runtimeId, version: f.definition.version, runtimeIdentifier, packageSha256: runtime.packageSha256, filesSha256: runtime.filesSha256 } };
  json(f.lock, { schemaVersion: 1, tools: { example: identity } });
  return { ...f, assembly, row, identity, runtimeDirectory, runtimeArchive: runtime.archive };
}

for (const [rid, runner] of [["linux-x64", "executable"], ["any", "dotnet"]]) {
  test(`version 2 resolves and authenticates the selected ${rid} ${runner} package`, t => {
    const f = runtimeFixture(t, rid, runner);
    assert.deepEqual(installedTool(f.root, "example", f.env, f.definition).identity, f.identity);
    const tool = resolveTool(f.root, "example", f.env);
    assert.deepEqual(tool.identity, f.identity);
    assert.equal(tool.assembly, f.assembly);
    assert.equal(tool.resolver.runner, runner);
    assert.deepEqual(tool.args, ["tool", "run", "example", "--"]);
    assert.ok(sameTool(tool, resolveTool(f.root, "example", f.env)));
  });
}

for (const [label, mutate] of [
  ["changed RID executable", f => write(f.assembly, "changed")],
  ["changed RID runtime outside tools", f => write(path.join(f.runtimeDirectory, "fable-library-js/TaggedUnion.js"), "changed")],
  ["changed pointer runtime outside tools", f => write(path.join(f.directory, "fable-library-js/TaggedUnion.js"), "changed")],
  ["additional RID file", f => write(path.join(f.runtimeDirectory, "extra.js"), "extra")],
  ["missing RID file", f => rmSync(path.join(f.runtimeDirectory, "fable-library-js/TaggedUnion.js"))],
  ["changed pointer specification", f => write(path.join(f.directory, "example.tool.nuspec"), "changed")],
]) test(`version 2 initial acceptance refuses ${label}`, t => {
  const f = runtimeFixture(t);
  // Warm the archive hash cache first: acceptance must still reread every extracted byte.
  installedTool(f.root, "example", f.env, f.definition);
  mutate(f);
  assert.throws(() => installedTool(f.root, "example", f.env, f.definition), /extraction.*archive/);
});

test("version 2 accepts original nuspec casing and rejects an altered retained archive", t => {
  const f = runtimeFixture(t);
  const lower = path.join(f.directory, "example.tool.nuspec");
  write(path.join(f.directory, "Example.Tool.nuspec"), readFileSync(lower));
  rmSync(lower);
  assert.deepEqual(resolveTool(f.root, "example", f.env).identity, f.identity);
  write(path.join(f.runtimeDirectory, "Example.Tool.linux-x64.nupkg"), "altered archive copy");
  assert.throws(() => resolveTool(f.root, "example", f.env), /retained package archive.*digest/);
});

test("version 2 binds the executable to the pointer's package and exact version", t => {
  const f = runtimeFixture(t);
  for (const executable of [
    f.assembly.replace("example.tool.linux-x64", "unlisted.tool"),
    f.assembly.replace(f.definition.version, "9.0.0"),
  ]) {
    json(f.cache, [{ ...f.row, PathToExecutable: executable }]);
    assert.throws(() => resolveTool(f.root, "example", f.env), /referenced runtime package and version/);
  }
});

test("version 2 rejects changed RID archives and incomplete or mismatched reviewed identities", t => {
  const f = runtimeFixture(t);
  for (const runtimePackage of [undefined, { ...f.identity.runtimePackage, version: "9.0.0" },
    { ...f.identity.runtimePackage, packageSha256: "0".repeat(64) }]) {
    json(f.lock, { schemaVersion: 1, tools: { example: { ...f.identity, runtimePackage } } });
    assert.throws(() => resolveTool(f.root, "example", f.env), /runtime package.*reviewed identity or digest/);
  }
  json(f.lock, { schemaVersion: 1, tools: { example: f.identity } });
  write(f.runtimeArchive, "altered archive");
  assert.throws(() => resolveTool(f.root, "example", f.env), /runtime package.*reviewed identity or digest/);
});

test("version 2 refuses symlinks outside the selected executable directory", t => {
  const f = runtimeFixture(t);
  symlinkSync(f.assembly, path.join(f.runtimeDirectory, "runtime-alias"));
  assert.throws(() => resolveTool(f.root, "example", f.env), /symlink/);
});

test("resolver refuses an executable from another NuGet cache even when the expected payload is intact", (t) => {
  const f = fixture(t);
  const other = path.join(f.root, "old-cache/example.dll");
  write(other, "different executable");
  json(f.cache, [{ ...f.row, PathToExecutable: other }]);
  assert.throws(() => resolveTool(f.root, "example", f.env), /resolver.*(path|executable|payload)/i);
});

test("resolver matches the active SDK framework, independent of the packaged tool framework", (t) => {
  const f = fixture(t);
  json(f.cache, [{ ...f.row, TargetFramework: "net9.0", PathToExecutable: "/old-sdk/ignored.dll" }, f.row]);
  assert.equal(resolveTool(f.root, "example", f.env).assembly, f.assembly);
});

for (const [label, rows] of [
  ["missing mapping", () => []],
  ["wrong SDK mapping", (row) => [{ ...row, TargetFramework: "net9.0" }]],
  ["wrong version mapping", (row) => [{ ...row, Version: "1.2.4" }]],
  ["wrong command mapping", (row) => [{ ...row, Name: "another" }]],
  ["wrong RID mapping", (row) => [{ ...row, RuntimeIdentifier: "linux-x64" }]],
  ["duplicate mapping", (row) => [row, row]],
  ["wrong runner", (row) => [{ ...row, Runner: "other" }]],
]) test(`resolver refuses ${label}`, (t) => {
  const f = fixture(t);
  json(f.cache, rows(f.row));
  assert.throws(() => resolveTool(f.root, "example", f.env), /resolver/i);
});

test("resolver refuses a missing restore cache", (t) => {
  const f = fixture(t);
  rmSync(f.cache);
  assert.throws(() => resolveTool(f.root, "example", f.env), /resolver|restore/i);
});

for (const [label, mutate] of [
  ["changed assembly", (f) => write(f.assembly, "altered assembly")],
  ["changed dependency metadata", (f) => write(path.join(f.payload, "example.deps.json"), "{}")],
  ["changed satellite", (f) => write(path.join(f.payload, "fr/Example.resources.dll"), "altered resource")],
  ["additional file", (f) => write(path.join(f.payload, "extra.dll"), "extra assembly")],
  ["missing file", (f) => rmSync(path.join(f.payload, "fr/Example.resources.dll"))],
]) test(`initial acceptance refuses ${label} despite an intact archive`, (t) => {
  const f = fixture(t);
  mutate(f);
  assert.throws(() => installedTool(f.root, "example", f.env, f.definition), /payload.*(archive|package)|archive.*payload/i);
});

test("initial acceptance refuses a non-ZIP package", (t) => {
  const f = fixture(t);
  write(f.archive, "invalid archive");
  assert.throws(() => installedTool(f.root, "example", f.env, f.definition), /archive|ZIP/i);
});

test("reviewed locks reject changed package bytes and changed extracted files", (t) => {
  const f = fixture(t);
  write(f.assembly, "different assembly");
  assert.throws(() => resolveTool(f.root, "example", f.env), /payload/);
  write(f.archive, "different archive");
  assert.throws(() => resolveTool(f.root, "example", f.env), /package.*digest/);
});

test("resolver refuses symlinks in an installed payload", (t) => {
  const f = fixture(t);
  symlinkSync(f.assembly, path.join(f.payload, "alias.dll"));
  assert.throws(() => resolveTool(f.root, "example", f.env), /symlink/);
});

test("tool manifest changes cannot override the exact package pin", (t) => {
  const f = fixture(t);
  const manifest = JSON.parse(readFileSync(f.manifest, "utf8"));
  manifest.tools["example.tool"].version = "1.2.4";
  json(f.manifest, manifest);
  assert.throws(() => resolveTool(f.root, "example", f.env), /manifest.*lock/);
});

test("NuGet cache follows explicit relative paths and dotnet configuration", (t) => {
  const f = fixture(t);
  assert.equal(packageCache({ ...f.env, NUGET_PACKAGES: "nuget" }, f.root), path.join(f.root, "nuget"));
  const env = { ...f.env };
  delete env.NUGET_PACKAGES;
  assert.equal(packageCache(env, f.root), path.join(f.root, "nuget"));
});

test("resolver uses the user home when DOTNET_CLI_HOME is omitted", (t) => {
  const f = fixture(t);
  const env = { ...f.env, [process.platform === "win32" ? "USERPROFILE" : "HOME"]: f.env.DOTNET_CLI_HOME };
  delete env.DOTNET_CLI_HOME;
  assert.equal(resolveTool(f.root, "example", env).assembly, f.assembly);
});

test("an SDK resolver change invalidates the generation fingerprint", (t) => {
  const f = fixture(t);
  const first = resolveTool(f.root, "example", f.env);
  json(path.join(f.sdk, "dotnet.runtimeconfig.json"), { runtimeOptions: { tfm: "net9.0" } });
  json(f.cache, [f.row, { ...f.row, TargetFramework: "net9.0" }]);
  assert.equal(sameTool(first, resolveTool(f.root, "example", f.env)), false);
});

test("multiple packages cannot claim the same local tool command", (t) => {
  const f = fixture(t);
  const manifest = JSON.parse(readFileSync(f.manifest, "utf8"));
  manifest.tools["other.tool"] = { version: "1.0.0", commands: ["example"] };
  json(f.manifest, manifest);
  assert.throws(() => resolveTool(f.root, "example", f.env), /ambiguous.*command/);
});

test("initial acceptance refuses package archive traversal paths", (t) => {
  const f = fixture(t);
  const bytes = zipSync({ "tools/net8.0/any/../escape.dll": Buffer.from("escape") });
  write(f.archive, bytes);
  assert.throws(() => installedTool(f.root, "example", f.env, f.definition), /invalid path.*archive/);
});

test("resolver verifies only the payload selected by the active SDK in a multi-framework package", (t) => {
  const f = fixture(t);
  const all = {};
  for (const [name, bytes] of Object.entries(f.data)) {
    all[`tools/net8.0/any/${name}`] = Buffer.from(bytes);
    all[`tools/net9.0/any/${name}`] = Buffer.from(bytes);
    write(path.join(f.directory, "tools/net9.0/any", name), bytes);
  }
  write(f.archive, zipSync(all));
  assert.equal(installedTool(f.root, "example", f.env, f.definition).assembly, f.assembly);
});

test("projection build receipt binds source and ancestor build settings to the emitted payload", t => {
  const scratch = fileURLToPath(new URL("./.scratch/projection-tooling/", import.meta.url));
  mkdirSync(scratch, { recursive: true });
  const root = mkdtempSync(path.join(scratch, "case-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  // The primary checkout supplies stable Git metadata; the compiler process is a test double.
  const checkout = fileURLToPath(new URL("../", import.meta.url));
  const revision = spawnSync("git", ["-C", checkout, "rev-parse", "HEAD"], { encoding: "utf8" }).stdout.trim();
  const projection = { project: "generators/adapter/Adapter.fsproj", xanthamRevision: revision };
  const project = path.join(root, projection.project);
  const source = path.join(path.dirname(project), "Program.fs");
  write(project, "<Project />"); write(source, "module Original");
  const payload = path.join(path.dirname(project), "bin/Release/net10.0/Adapter.dll");
  const compiler = path.join(root, "fake-dotnet.cjs");
  write(compiler, `#!${process.execPath}\nconst fs = require('node:fs');const path=require('node:path');const out=process.env.FIXTURE_PAYLOAD;fs.mkdirSync(path.dirname(out),{recursive:true});fs.writeFileSync(out,'compiled '+fs.readFileSync(process.env.FIXTURE_SOURCE,'utf8'));\n`);
  executableScript(t, compiler);
  const env = { ...process.env, CLOUDEDGE_DOTNET: compiler, CLOUDEDGE_XANTHAM_ROOT: checkout, CLOUDEDGE_ALLOW_DIRTY_XANTHAM: "1", FIXTURE_PAYLOAD: payload, FIXTURE_SOURCE: source };
  const built = resolveProjectionTool(root, projection, env, true);
  assert.ok(sameTool(built, resolveProjectionTool(root, projection, env)));
  const originalPayload = readFileSync(payload);
  write(source, "module Changed");
  assert.deepEqual(readFileSync(payload), originalPayload);
  assert.throws(() => resolveProjectionTool(root, projection, env), /source-to-payload build receipt/);
  resolveProjectionTool(root, projection, env, true);
  write(path.join(root, "Directory.Build.props"), "<Project><PropertyGroup><DefineConstants>CHANGED</DefineConstants></PropertyGroup></Project>");
  assert.throws(() => resolveProjectionTool(root, projection, env), /source-to-payload build receipt/);
  resolveProjectionTool(root, projection, env, true);
  write(payload, "stale or changed assembly");
  assert.throws(() => resolveProjectionTool(root, projection, env), /source-to-payload build receipt/);
  assert.throws(() => resolveProjectionTool(root, { ...projection, xanthamRevision: "1".repeat(40) }, env), /Projection requires Xantham/);
});
