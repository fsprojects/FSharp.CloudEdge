import { createHash } from "node:crypto";
import { spawnSync } from "node:child_process";
import { existsSync, mkdirSync, mkdtempSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const args = process.argv.slice(2);
const options = {};
for (let index = 0; index < args.length; index++) {
  const key = args[index];
  if (key === "--upstream-helpers" && !Object.hasOwn(options, key)) options[key] = true;
  else if (["--feed", "--version"].includes(key) && args[index + 1] && !Object.hasOwn(options, key)) options[key] = args[++index];
  else throw new Error("Usage: node scripts/test-support-package.mjs [--feed directory] [--version version] [--upstream-helpers]");
}
const feed = path.resolve(root, options["--feed"] ?? "artifacts/tool-feed");
const config = JSON.parse(readFileSync(path.join(root, "config/targets.json"), "utf8"));
const version = options["--version"] ?? config.projectDefaults.packages["Xantham.Fable.Core"];
if (!/^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?$/.test(version)) throw new Error("Xantham.Fable.Core must have an exact configured version");
const packageFile = path.join(feed, `Xantham.Fable.Core.${version}.nupkg`);
if (!existsSync(packageFile)) throw new Error(`Bootstrap the support package first: ${packageFile}`);
const stagingRoot = path.join(root, "artifacts/tools/support-smoke");
mkdirSync(stagingRoot, { recursive: true });
const stage = mkdtempSync(path.join(stagingRoot, "stage-"));
for (const file of ["SupportPackage.fsproj", "Smoke.fs", "UpstreamHelpers.fs", "check.mjs", "check-upstream.mjs"]) {
  let content = readFileSync(path.join(root, "tests/SupportPackage", file), "utf8");
  if (file.endsWith(".fsproj")) content = content.replaceAll("0.1.0-alpha.1", version);
  writeFileSync(path.join(stage, file), content);
}
writeFileSync(path.join(stage, "package.json"), '{"type":"module","private":true}\n');
const xml = (value) => value.replaceAll("&", "&amp;").replaceAll('"', "&quot;").replaceAll("<", "&lt;").replaceAll(">", "&gt;");
writeFileSync(path.join(stage, "NuGet.Config"), `<configuration><config><add key="globalPackagesFolder" value="${xml(path.join(stage, "packages"))}"/></config><packageSources><clear/><add key="local" value="${xml(feed)}"/><add key="nuget.org" value="https://api.nuget.org/v3/index.json"/></packageSources><packageSourceMapping><packageSource key="local"><package pattern="Xantham.Fable.Core"/></packageSource><packageSource key="nuget.org"><package pattern="*"/></packageSource></packageSourceMapping></configuration>\n`);
const dotnet = process.env.CLOUDEDGE_DOTNET ?? "dotnet";
const env = { ...process.env };
delete env.NUGET_PACKAGES;
const run = (command, parameters) => {
  const result = spawnSync(command, parameters, { cwd: stage, env, stdio: "inherit" });
  if (result.error || result.status !== 0) throw new Error(`Support package smoke failed: ${result.error?.message ?? result.signal ?? result.status}`);
};
run(dotnet, ["build", "SupportPackage.fsproj", "--nologo"]);
const assets = JSON.parse(readFileSync(path.join(stage, "obj/project.assets.json"), "utf8"));
if (Object.values(assets.libraries).some((library) => library.type === "project")) throw new Error("Support package smoke unexpectedly resolved a source project");
const installed = path.join(stage, "packages", "xantham.fable.core", version.toLowerCase());
const hash = (file) => createHash("sha256").update(readFileSync(file)).digest("hex");
if (hash(path.join(installed, `xantham.fable.core.${version.toLowerCase()}.nupkg`)) !== hash(packageFile)) throw new Error("Support package smoke restored different package bytes");
for (const file of ["Library.fs", "Helpers.fs", "Xantham.Fable.Core.fsproj"]) {
  if (!existsSync(path.join(installed, "fable", file))) throw new Error(`Support package is missing its Fable source asset: ${file}`);
}
run(dotnet, ["tool", "run", "fable", "--", "SupportPackage.fsproj", "--outDir", "fable-out", "--noCache"]);
run(process.execPath, [options["--upstream-helpers"] ? "check-upstream.mjs" : "check.mjs"]);
console.log(`Package-only support smoke passed for Xantham.Fable.Core ${version}; evidence: ${stage}`);
