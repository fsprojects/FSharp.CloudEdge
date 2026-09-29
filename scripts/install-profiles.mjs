import { spawn } from "node:child_process";
import { createWriteStream, existsSync, mkdirSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { inspectInputOverlays } from "./input-overlays.mjs";
import { planPartitions } from "./plan-partitions.mjs";
import { partitionConfiguration } from "./build-sdk.mjs";
import { deliveryConfiguration } from "./sdk-delivery.mjs";
import { generationOrder } from "./catalogs.mjs";

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");

/** Select dependency profiles using the same effective owners as the delivery build. */
export function selectProfiles(config, requested = [], { catalog, delivery, repositoryRoot = root } = {}) {
  const unknownOption = requested.find(arg => arg.startsWith("--") && arg !== "--delivery");
  if (unknownOption) throw new Error(`Unknown install option: ${unknownOption}`);
  if (requested.includes("--delivery")) {
    if (requested.length !== 1) throw new Error("--delivery must be used once and cannot be combined with profile IDs");
    if (!catalog || !delivery) throw new Error("--delivery requires the SDK surface and delivery configurations");
    const plan = planPartitions(config, catalog);
    const effective = deliveryConfiguration(partitionConfiguration(config, plan), plan.partitions, delivery, repositoryRoot);
    const targets = generationOrder(effective.configuration.targets, effective.libraries.map(library => library.id));
    const used = new Set(targets.map(target => target.profile));
    for (const id of used) if (!config.profiles.some(profile => profile.id === id))
      throw new Error(`Unknown dependency profile in delivery: ${id}`);
    return config.profiles.filter(profile => used.has(profile.id));
  }
  for (const id of requested) if (!config.profiles.some(profile => profile.id === id))
    throw new Error(`Unknown dependency profile: ${id}`);
  return config.profiles.filter(profile => requested.length === 0 || requested.includes(profile.id));
}

async function main() {
  const read = file => JSON.parse(readFileSync(path.join(root, file), "utf8"));
  const requested = process.argv.slice(2);
  const config = read("config/targets.json");
  const metadata = requested.includes("--delivery")
    ? { catalog: read("config/sdk-surfaces.json"), delivery: read("config/sdk-delivery.json") } : {};
  const profiles = selectProfiles(config, requested, metadata);
  const directory = path.join(root, "artifacts", "install");
  mkdirSync(directory, { recursive: true });
  const results = [];
  let next = 0;
  async function worker() {
    while (next < profiles.length) {
      const profile = profiles[next++];
      const cwd = path.resolve(root, profile.directory);
      if (cwd !== root && !cwd.startsWith(root + path.sep)) throw new Error(`Profile path escapes the project: ${profile.id}`);
      const command = existsSync(path.join(cwd, "package-lock.json")) ? "ci" : "install";
      const log = createWriteStream(path.join(directory, `${profile.id}.log`));
      const result = await new Promise((resolve) => {
        const child = spawn("npm", [command, "--ignore-scripts", "--no-audit", "--no-fund"], { cwd, stdio: ["ignore", "pipe", "pipe"], env: { ...process.env, PLAYWRIGHT_SKIP_BROWSER_DOWNLOAD: "1", PUPPETEER_SKIP_DOWNLOAD: "true" } });
        child.stdout.pipe(log, { end: false });
        child.stderr.pipe(log, { end: false });
        child.once("error", (error) => resolve({ error: error.message }));
        child.once("close", (code, signal) => resolve({ code, signal }));
      });
      await new Promise((resolve) => log.end(resolve));
      if (result.code === 0) {
        try {
          const inspection = inspectInputOverlays({ root, profileDirectory: cwd });
          result.inputOverlayFingerprint = inspection.fingerprint;
          result.overlayRules = inspection.evidence.map((item) => item.ruleId);
        } catch (error) {
          result.code = 1;
          result.error = error.message;
          console.error(`${profile.id}: ${error.message}`);
        }
      }
      results.push({ profile: profile.id, ...result });
      console.log(`${profile.id}: ${result.code === 0 ? "installed" : "failed; see artifacts/install/" + profile.id + ".log"}`);
    }
  }
  await Promise.all(Array.from({ length: Math.min(3, profiles.length) }, worker));
  writeFileSync(path.join(directory, "results.json"), JSON.stringify(results, null, 2) + "\n");
  if (results.some((result) => result.code !== 0)) process.exitCode = 1;
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  main().catch(error => { console.error(`CloudEdge profile install failed: ${error.message}`); process.exitCode = 1; });
}
