import assert from "node:assert/strict";
import { mkdirSync, mkdtempSync, rmSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import { createRequire } from "node:module";
import test from "node:test";
import { declarationSyntax, resolveImport } from "../scripts/audit-inputs.mjs";

const require = createRequire(import.meta.url);
const ts = require("../profiles/worker-bundler/node_modules/typescript");

test("declaration imports exclude documentation examples and preserve type imports", () => {
  const parsed = declarationSyntax(ts, "index.d.ts", `
/** import { Fake } from "my-main-module"; */
import type { Value } from "actual";
export { Other } from "./other.js";
export type Imported = import("nested").Value;
import Equals = require("equals");
`);
  assert.deepEqual(parsed.imports.map((item) => [item.kind, item.specifier]), [
    ["import", "actual"], ["export", "./other.js"], ["import-type", "nested"], ["import-equals", "equals"]
  ]);
});

test("ambient declarations are providers while external-module augmentations are not", () => {
  assert.deepEqual(declarationSyntax(ts, "index.d.ts", 'declare module "runtime:api" { export const id: string; }').ambientModules, ["runtime:api"]);
  assert.deepEqual(declarationSyntax(ts, "index.d.ts", 'export {}; declare module "augment" {}').ambientModules, []);
});

test("a deliberately optional import probe keeps its suppression evidence", () => {
  const parsed = declarationSyntax(ts, "index.d.ts", '// @ts-ignore\nexport type Probe = import("optional").Value;');
  assert.equal(parsed.imports[0].suppression, "ts-ignore");
});

test("resolution starts beside the declaration and preserves nested package versions", (t) => {
  const root = mkdtempSync(path.join(os.tmpdir(), "declaration-audit-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  const write = (file, value) => {
    const full = path.join(root, file);
    mkdirSync(path.dirname(full), { recursive: true });
    writeFileSync(full, typeof value === "string" ? value : JSON.stringify(value));
  };
  for (const [prefix, version] of [["node_modules", "1.0.0"], ["node_modules/owner/node_modules", "2.0.0"]]) {
    write(`${prefix}/provider/package.json`, { name: "provider", version, exports: { ".": { bun: { types: "./bun.d.ts" }, types: "./types.d.ts" } } });
    write(`${prefix}/provider/types.d.ts`, "export interface Value {}");
    write(`${prefix}/provider/bun.d.ts`, 'export { BunOnly } from "bun";');
  }
  const result = resolveImport("provider", path.join(root, "node_modules/owner/index.d.ts"));
  assert.equal(result.version, "2.0.0");
  assert.equal(result.files[0], path.join(root, "node_modules/owner/node_modules/provider/types.d.ts"));
  assert.equal(resolveImport("missing", path.join(root, "node_modules/owner/index.d.ts")).status, "missing-package");
  write("node_modules/wild/package.json", { name: "wild", version: "1.0.0", exports: { "./*": { types: "./dist/*.d.ts", import: "./dist/*" } } });
  write("node_modules/wild/dist/subpath.d.ts", "export interface Value {}");
  assert.equal(resolveImport("wild/subpath.js", path.join(root, "index.d.ts")).files[0], path.join(root, "node_modules/wild/dist/subpath.d.ts"));
  write("node_modules/alias/package.json", { name: "alias", version: "1.0.0", imports: { "#types/*": "./types/*.d.ts" } });
  write("node_modules/alias/types/value.d.ts", "export interface Value {}");
  assert.equal(resolveImport("#types/value", path.join(root, "node_modules/alias/index.d.ts")).files[0], path.join(root, "node_modules/alias/types/value.d.ts"));
  assert.equal(resolveImport("./types/value.ts", path.join(root, "node_modules/alias/index.d.ts")).files[0], path.join(root, "node_modules/alias/types/value.d.ts"));
});
