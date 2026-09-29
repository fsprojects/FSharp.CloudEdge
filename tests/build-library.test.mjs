import assert from "node:assert/strict";
import test from "node:test";
import { librarySolution } from "../scripts/build-library.mjs";

test("library solution keeps Core, Runtime, Support, Management, Tenancy and consumers in their hierarchy", () => {
  const projects = [
    "src/Runtime/Agents/Agent/Agent.fsproj",
    "src/Support/AI/V4/Provider/Provider.fsproj",
    "src/Management/AI/Management.AI/Management.AI.fsproj",
    "src/Core/Api/Api.fsproj",
    "src/Tenancy/Tenancy/Tenancy.fsproj",
    "tests/SDKComposition/SDKComposition.fsproj",
    "tests/HawaiiApi/HawaiiApi.fsproj",
  ];
  assert.equal(librarySolution(projects), [
    "<Solution>",
    '  <Folder Name="/Core/">',
    '    <Project Path="src/Core/Api/Api.fsproj" />',
    "  </Folder>",
    '  <Folder Name="/Management/AI/">',
    '    <Project Path="src/Management/AI/Management.AI/Management.AI.fsproj" />',
    "  </Folder>",
    '  <Folder Name="/Runtime/Agents/">',
    '    <Project Path="src/Runtime/Agents/Agent/Agent.fsproj" />',
    "  </Folder>",
    '  <Folder Name="/Support/AI/V4/">',
    '    <Project Path="src/Support/AI/V4/Provider/Provider.fsproj" />',
    "  </Folder>",
    '  <Folder Name="/Tenancy/">',
    '    <Project Path="src/Tenancy/Tenancy/Tenancy.fsproj" />',
    "  </Folder>",
    '  <Folder Name="/tests/">',
    '    <Project Path="tests/HawaiiApi/HawaiiApi.fsproj" />',
    '    <Project Path="tests/SDKComposition/SDKComposition.fsproj" />',
    "  </Folder>",
    "</Solution>",
    "",
  ].join("\n"));
});

test("library solution deduplicates shared projects and is stable without changing its input", () => {
  const shared = "src/Core/Api/Api.fsproj";
  const runtime = "src/Runtime/Workers/Workers/Workers.fsproj";
  const projects = [runtime, shared, runtime, shared];
  const original = [...projects];
  const solution = librarySolution(projects);
  assert.equal(solution, librarySolution([...projects].reverse()));
  assert.equal(solution, librarySolution([shared, runtime]));
  assert.equal(solution.match(/<Project /g).length, 2);
  assert.deepEqual(projects, original);
});

test("library solution escapes XML attributes in both folder names and project paths", () => {
  const solution = librarySolution(['src/Support/AI & Tools/Adapter/Model"<&.fsproj']);
  assert.ok(solution.includes('<Folder Name="/Support/AI &amp; Tools/">'));
  assert.ok(solution.includes('<Project Path="src/Support/AI &amp; Tools/Adapter/Model&quot;&lt;&amp;.fsproj" />'));
});

test("library solution rejects absolute, escaping and non-project inputs", () => {
  for (const invalid of ["/tmp/Bad.fsproj", "../Bad.fsproj", "src/Runtime/../Bad.fsproj",
    "src/Runtime/Bad.csproj", "", null, undefined, 42, {}]) {
    assert.throws(() => librarySolution([invalid]), /Invalid library project/, String(invalid));
  }
});

test("library solution rejects portable path aliases and control characters", () => {
  for (const invalid of ["C:/repo/Bad.fsproj", "C:\\repo\\Bad.fsproj", "src\\..\\Bad.fsproj",
    "./src/Core/Api/Api.fsproj", "src/Core/./Api/Api.fsproj", "src//Core/Api/Api.fsproj",
    "src/Core/Api/Bad\nName.fsproj", "src/Core/Api/Bad\tName.fsproj", "src/Core/Api/Bad\u0000Name.fsproj"]) {
    assert.throws(() => librarySolution([invalid]), /Invalid library project/, JSON.stringify(invalid));
  }
});
