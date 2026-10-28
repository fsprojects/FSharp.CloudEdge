import { mkdirSync } from "node:fs";
import { fileURLToPath } from "node:url";

export function scratchDirectory() {
  const directory = fileURLToPath(new URL("./.scratch/tooling/", import.meta.url));
  mkdirSync(directory, { recursive: true });
  return directory;
}
