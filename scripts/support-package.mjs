import { createHash } from "node:crypto";
import { spawnSync } from "node:child_process";
import { closeSync, copyFileSync, existsSync, mkdirSync, mkdtempSync, openSync, readFileSync, renameSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const template = fileURLToPath(new URL("./templates/Xantham.Fable.Core.fsproj", import.meta.url));
const requireThat = (condition, message) => { if (!condition) throw new Error(message); };
const hash = (file) => createHash("sha256").update(readFileSync(file)).digest("hex");
const sourceFiles = ["Library.fs", "Helpers.fs", "README.md"];

/** Package the peer support source in an isolated consumer-owned staging directory. */
export async function packSupport({ root, sourceDirectory, version, feed, dotnet = "dotnet" }) {
  requireThat(typeof version === "string" && /^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?$/.test(version), "Support package requires an exact version");
  root = path.resolve(root);
  sourceDirectory = path.resolve(sourceDirectory);
  feed = path.resolve(feed);
  for (const file of sourceFiles) {
    requireThat(existsSync(path.join(sourceDirectory, file)), `Support package source is missing: ${path.join(sourceDirectory, file)}`);
  }
  const stagingRoot = path.join(root, "artifacts", "tools", "support-pack");
  mkdirSync(stagingRoot, { recursive: true });
  const stage = mkdtempSync(path.join(stagingRoot, "stage-"));
  for (const file of sourceFiles) copyFileSync(path.join(sourceDirectory, file), path.join(stage, file));
  const project = path.join(stage, "Xantham.Fable.Core.fsproj");
  writeFileSync(project, readFileSync(template, "utf8").replaceAll("{{version}}", version));
  const output = path.join(stage, "packages");
  const log = path.join(stage, "pack.log");
  const fd = openSync(log, "w");
  let result;
  try {
    result = spawnSync(dotnet, ["pack", project, "--configuration", "Release", "--output", output, "--nologo"], {
      cwd: stage, stdio: ["ignore", fd, fd],
    });
  } finally { closeSync(fd); }
  requireThat(!result.error && result.status === 0, `Support package pack failed (${result.error?.message ?? result.signal ?? result.status}); see ${log}`);
  const name = `Xantham.Fable.Core.${version}.nupkg`;
  const packed = path.join(output, name);
  requireThat(existsSync(packed), `Support package pack did not produce ${packed}; see ${log}`);
  mkdirSync(feed, { recursive: true });
  const destination = path.join(feed, name);
  if (existsSync(destination)) {
    requireThat(hash(destination) === hash(packed), `Support package version ${version} already has different bytes in ${feed}; choose a new version`);
  } else {
    const temporary = path.join(feed, `${name}.${process.pid}.tmp`);
    copyFileSync(packed, temporary);
    renameSync(temporary, destination);
  }
  return destination;
}
