import assert from "node:assert/strict";
import { mkdtempSync, rmSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import test from "node:test";
import { packSupport } from "../scripts/support-package.mjs";

test("support packing refuses a version range before starting a compiler", async (t) => {
  const root = mkdtempSync(path.join(os.tmpdir(), "cloudedge-support-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  await assert.rejects(packSupport({ root, sourceDirectory: root, feed: path.join(root, "feed"), version: "0.1.*", dotnet: "absent-dotnet" }), /exact version/);
});

test("support packing refuses missing peer source before starting a compiler", async (t) => {
  const root = mkdtempSync(path.join(os.tmpdir(), "cloudedge-support-"));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  await assert.rejects(packSupport({ root, sourceDirectory: root, feed: path.join(root, "feed"), version: "0.1.0-local.test", dotnet: "absent-dotnet" }), /source is missing.*Library.fs/);
});
