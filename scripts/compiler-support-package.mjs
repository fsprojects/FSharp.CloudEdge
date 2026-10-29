import { spawnSync } from "node:child_process";
import { copyFileSync, existsSync, mkdirSync, mkdtempSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";

// Package the producer's generated standard library unchanged, including Fable sources.
export function packCompilerSupport({ root, source, version, supportVersion, dotnet, feed }) {
  const directory = path.join(source, "src/Xantham.Fable.Core.TS");
  const projectName = "Xantham.Fable.Core.TS.fsproj";
  let project = readFileSync(path.join(directory, projectName), "utf8");
  const files = [...project.matchAll(/<Compile Include="([^"]+)"\s*\/>/g)].map((match) => match[1]);
  if (!files.length || files.some((file) => path.basename(file) !== file || !file.endsWith(".fs"))) {
    throw new Error("Compiler support project has an unsupported source layout");
  }
  const staging = path.join(root, "artifacts/tools/compiler-support-pack");
  mkdirSync(staging, { recursive: true });
  const stage = mkdtempSync(path.join(staging, "stage-"));
  // The catalogue and its input stamp travel with the sources: Xantham's catalogue targets
  // authenticate them against the staged bindings before packing.
  const catalogue = ["declarations.json", "catalogue-inputs.json"].filter((file) => existsSync(path.join(directory, file)));
  for (const file of [...files, "README.md", ...catalogue]) copyFileSync(path.join(directory, file), path.join(stage, file));
  const catalogueTool = path.join(source, "tools/package-core-ts-catalog.fsx");
  project = project.replaceAll("$(MSBuildThisFileDirectory)../../tools/package-core-ts-catalog.fsx", catalogueTool)
    .replace(/<Version>[^<]+<\/Version>/, `<Version>${version}</Version>`)
    .replace(/<ProjectReference Include="[^\"]+"(?:\s+Condition="[^"]+")?\s*\/>/, `<PackageReference Include="Xantham.Fable.Core" Version="${supportVersion}" />`)
    .replace(/\s*<PackageReference Include="Xantham.Fable.Core" Version="[^"]+" Condition="[^"]+"\s*\/>/, "");
  if (!project.includes('PackagePath="fable/"')) {
    project = project.replace("</Project>", `  <ItemGroup><Content Include="${[...files, projectName].join(";")}" Pack="true" PackagePath="fable/" /></ItemGroup>\n</Project>`);
  }
  writeFileSync(path.join(stage, projectName), project);
  const result = spawnSync(dotnet, ["pack", projectName, "-c", "Release", "-o", feed, "--nologo"], {
    cwd: stage, encoding: "utf8", maxBuffer: 16 * 1024 * 1024,
  });
  writeFileSync(path.join(stage, "pack.log"), (result.stdout ?? "") + (result.stderr ?? ""));
  const archive = path.join(feed, `Xantham.Fable.Core.TS.${version}.nupkg`);
  if (result.error || result.status !== 0 || !existsSync(archive)) throw new Error(`Compiler support pack failed; see ${stage}/pack.log`);
  return archive;
}
