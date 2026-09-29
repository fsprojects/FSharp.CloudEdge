import { randomUUID } from "node:crypto";
import { closeSync, existsSync, fsyncSync, mkdirSync, openSync, readFileSync, renameSync, rmSync, writeFileSync } from "node:fs";
import { hostname } from "node:os";
import path from "node:path";

const requireThat = (value, message) => { if (!value) throw new Error(message); };
const object = value => value !== null && typeof value === "object" && !Array.isArray(value);
const sha = value => typeof value === "string" && /^[a-f0-9]{64}$/.test(value);
const identifier = value => typeof value === "string" && /^[a-zA-Z0-9][a-zA-Z0-9._:-]{0,127}$/.test(value);
const exactFields = (value, fields) => object(value) && fields.every(key => Object.hasOwn(value, key))
  && Object.keys(value).every(key => fields.includes(key));
const fields = (value, required, optional = []) => object(value) && required.every(key => Object.hasOwn(value, key))
  && Object.keys(value).every(key => required.includes(key) || optional.includes(key));
const now = () => new Date().toISOString();

function validatePlan(plan) {
  requireThat(exactFields(plan, ["runId", "accountFingerprint", "ttlSeconds", "resources"])
    && identifier(plan.runId) && sha(plan.accountFingerprint)
    && Number.isSafeInteger(plan.ttlSeconds) && plan.ttlSeconds > 0 && plan.ttlSeconds <= 86400
    && Array.isArray(plan.resources) && plan.resources.length > 0 && plan.resources.length <= 256,
  "Plan requires a run ID, account fingerprint, 1..86400 second TTL and explicit resources; arbitrary metadata is forbidden");
  for (const resource of plan.resources) requireThat(exactFields(resource, ["key", "kind", "name"])
    && identifier(resource.key) && identifier(resource.kind)
    && typeof resource.name === "string" && /^[a-z][a-z0-9-]{0,62}$/.test(resource.name), "Invalid planned resource identity");
  for (const field of ["key", "name"]) requireThat(new Set(plan.resources.map(resource => resource[field])).size === plan.resources.length,
    `Planned resource ${field}s must be unique`);
}

function validReceipt(receipt, resource, plan) {
  return exactFields(receipt, ["id", "name", "kind", "runId", "accountFingerprint"])
    && typeof receipt.id === "string" && /^[a-zA-Z0-9][a-zA-Z0-9._:/-]{0,255}$/.test(receipt.id)
    && receipt.name === resource.name && receipt.kind === resource.kind
    && receipt.runId === plan.runId && receipt.accountFingerprint === plan.accountFingerprint;
}

/** Only this module's constrained metadata enters the journal. Adapter error text never does. */
function persist(file, journal, create = false) {
  const directory = path.dirname(file);
  mkdirSync(directory, { recursive: true, mode: 0o700 });
  if (create) requireThat(!existsSync(file), "Journal already exists; use resumeCleanup for an existing run");
  const temporary = `${file}.${randomUUID()}.tmp`;
  let fd;
  try {
    fd = openSync(temporary, "wx", 0o600);
    writeFileSync(fd, JSON.stringify(journal, null, 2) + "\n");
    fsyncSync(fd); closeSync(fd); fd = undefined;
    renameSync(temporary, file);
    const directoryFd = openSync(directory, "r");
    try { fsyncSync(directoryFd); } finally { closeSync(directoryFd); }
  } finally {
    if (fd !== undefined) closeSync(fd);
    rmSync(temporary, { force: true });
  }
}

function acquire(file) {
  mkdirSync(path.dirname(file), { recursive: true, mode: 0o700 });
  const lock = `${file}.lock`;
  try { mkdirSync(lock, { mode: 0o700 }); }
  catch (error) {
    if (error.code !== "EEXIST") throw error;
    requireThat(existsSync(path.join(lock, "owner.json")), "Lifecycle lock has no owner evidence; inspect it before recovery");
    const owner = JSON.parse(readFileSync(path.join(lock, "owner.json"), "utf8"));
    requireThat(Number.isSafeInteger(owner.pid) && owner.pid > 0 && owner.host === hostname(), "Invalid or foreign-host lifecycle lock; inspect it before recovery");
    let alive = true;
    try { process.kill(owner.pid, 0); } catch (error) { if (error.code === "ESRCH") alive = false; }
    requireThat(!alive, "Lifecycle journal is in use by a running process");
    // Exactly one process may retire a stale lock. An interrupted lock recovery
    // leaves this claim for inspection instead of risking concurrent deletion.
    const claim = openSync(path.join(lock, "recovery"), "wx", 0o600);
    closeSync(claim);
    const confirmed = JSON.parse(readFileSync(path.join(lock, "owner.json"), "utf8"));
    requireThat(confirmed.pid === owner.pid && confirmed.host === owner.host, "Lifecycle lock changed during recovery");
    try { process.kill(confirmed.pid, 0); alive = true; } catch (error) { if (error.code !== "ESRCH") alive = true; }
    requireThat(!alive, "Lifecycle lock owner became active during recovery");
    rmSync(lock, { recursive: true });
    mkdirSync(lock, { mode: 0o700 });
  }
  const fd = openSync(path.join(lock, "owner.json"), "wx", 0o600);
  try { writeFileSync(fd, JSON.stringify({ pid: process.pid, host: hostname() })); fsyncSync(fd); } finally { closeSync(fd); }
  return () => rmSync(lock, { recursive: true, force: true });
}

function context(journal, resource) {
  // Callbacks receive detached metadata, never the mutable journal or credential storage.
  return structuredClone({ plan: journal.plan, expiresAtUtc: journal.expiresAtUtc,
    artifactSha256: journal.execution.artifactSha256 ?? null,
    resources: journal.resources.filter(item => item.receipt).map(item => ({ ...item.intent, receipt: item.receipt })),
    ...(resource ? { resource: resource.intent, receipt: resource.receipt } : {}) });
}

function requireAdapters(adapters, names) {
  requireThat(object(adapters) && names.every(name => typeof adapters[name] === "function"),
    `Dedicated lifecycle adapters are required: ${names.join(", ")}`);
}

const failure = (stage, code, resource) => ({ stage, code, ...(resource ? { resourceKey: resource.intent.key } : {}) });
const activeStates = new Set(["creating", "created", "uncertain", "deleting", "deleted", "refused"]);

async function cleanup(journal, file, adapters) {
  const attempt = { startedAtUtc: now(), status: "running", failures: [] };
  requireThat(journal.cleanup.attempts.length < 64, "Cleanup attempt limit reached; inspect the journal");
  journal.cleanup.attempts.push(attempt); journal.cleanup.status = "running";
  persist(file, journal);
  for (const resource of [...journal.resources].reverse()) {
    if (!activeStates.has(resource.state)) continue;
    if (!resource.receipt) {
      let reconciled;
      try { reconciled = await adapters.reconcile(context(journal, resource)); }
      catch {
        attempt.failures.push(failure("reconcile", "adapter-failed", resource));
        persist(file, journal); continue;
      }
      if (exactFields(reconciled, ["state", "receipt"]) && reconciled.state === "owned"
        && validReceipt(reconciled.receipt, resource.intent, journal.plan)) {
        resource.receipt = structuredClone(reconciled.receipt);
        resource.state = "created";
        persist(file, journal);
      } else if (!(exactFields(reconciled, ["state"]) && reconciled.state === "absent")) {
        resource.state = "refused";
        attempt.failures.push(failure("reconcile", "ownership-not-established", resource));
        persist(file, journal); continue;
      }
    }
    if (resource.receipt) {
      if (!validReceipt(resource.receipt, resource.intent, journal.plan)) {
        resource.state = "refused";
        attempt.failures.push(failure("destroy", "ownership-not-established", resource));
        persist(file, journal); continue;
      }
      resource.state = "deleting"; persist(file, journal);
      try {
        await adapters.destroy(context(journal, resource));
        resource.state = "deleted";
      } catch { attempt.failures.push(failure("destroy", "adapter-failed", resource)); }
      persist(file, journal);
    }
    try {
      const result = await adapters.verifyAbsent(context(journal, resource));
      if (exactFields(result, ["absent"]) && result.absent === true) {
        resource.state = "absent"; resource.absenceVerifiedAtUtc = now();
      } else attempt.failures.push(failure("verifyAbsent", "absence-not-established", resource));
    } catch { attempt.failures.push(failure("verifyAbsent", "adapter-failed", resource)); }
    persist(file, journal);
  }
  attempt.finishedAtUtc = now();
  attempt.status = attempt.failures.length === 0 && journal.resources.every(resource => ["planned", "absent"].includes(resource.state))
    ? "succeeded" : "failed";
  journal.cleanup.status = attempt.status;
  journal.finishedAtUtc = now();
  persist(file, journal);
}

function result(journal) {
  return { schemaVersion: 1, runId: journal.plan.runId,
    status: journal.execution.status === "succeeded" && journal.cleanup.status === "succeeded" ? "passed" : "failed",
    execution: structuredClone(journal.execution), cleanup: structuredClone(journal.cleanup),
    resources: journal.resources.map(resource => ({ key: resource.intent.key, state: resource.state,
      ...(resource.receipt ? { id: resource.receipt.id } : {}) })),
    qualification: "lifecycle-engine-result; acceptance-depends-on-configured-adapters" };
}

/** No built-in Cloudflare access. Credentials belong in dedicated adapter closures.
 * Resources must be ordered with dependencies first. Cleanup reverses that order.
 */
export async function runLifecycle({ plan, journalPath, adapters }) {
  validatePlan(plan);
  requireThat(typeof journalPath === "string" && journalPath.length > 0, "Journal path is required");
  requireAdapters(adapters, ["compile", "preflight", "provision", "exercise", "collect", "destroy", "verifyAbsent", "reconcile"]);
  const file = path.resolve(journalPath), release = acquire(file);
  const journal = { schemaVersion: 1, kind: "ephemeral-resource-lifecycle", plan: structuredClone(plan),
    createdAtUtc: now(), expiresAtUtc: new Date(Date.now() + plan.ttlSeconds * 1000).toISOString(),
    resources: plan.resources.map(intent => ({ intent: structuredClone(intent), state: "planned", receipt: null })),
    execution: { status: "running", failures: [] }, cleanup: { status: "pending", attempts: [] } };
  let started = false, stage = "compile", current;
  try {
    persist(file, journal, true); started = true;
    try {
      const compiled = await adapters.compile(context(journal));
      requireThat(exactFields(compiled, ["artifactSha256"]) && sha(compiled.artifactSha256), "Invalid compile receipt");
      journal.execution.artifactSha256 = compiled.artifactSha256; persist(file, journal);
      stage = "preflight";
      const checked = await adapters.preflight(context(journal));
      requireThat(exactFields(checked, ["accountFingerprint"]) && checked.accountFingerprint === plan.accountFingerprint, "Preflight account mismatch");
      for (const resource of journal.resources) {
        stage = "provision"; current = resource;
        requireThat(Date.now() < Date.parse(journal.expiresAtUtc), "Run TTL expired");
        // This write precedes the remote mutation, including a create whose response is lost.
        resource.state = "creating"; persist(file, journal);
        const receipt = await adapters.provision(context(journal, resource));
        requireThat(validReceipt(receipt, resource.intent, journal.plan), "Provision receipt does not establish ownership");
        resource.receipt = structuredClone(receipt); resource.state = "created";
        persist(file, journal);
      }
      stage = "exercise"; current = undefined;
      requireThat(Date.now() < Date.parse(journal.expiresAtUtc), "Run TTL expired");
      const exercised = await adapters.exercise(context(journal));
      requireThat(exactFields(exercised, ["passed"]) && exercised.passed === true, "Staged exercise failed");
    } catch {
      if (current?.state === "creating") current.state = "uncertain";
      journal.execution.failures.push(failure(stage, "adapter-or-contract-failed", current));
    } finally {
      if (journal.resources.some(resource => resource.state !== "planned")) {
        try {
          const collected = await adapters.collect(context(journal));
          requireThat(exactFields(collected, ["resultSha256"]) && sha(collected.resultSha256), "Invalid collection receipt");
          journal.execution.resultSha256 = collected.resultSha256;
        } catch { journal.execution.failures.push(failure("collect", "adapter-or-contract-failed")); }
      }
      journal.execution.status = journal.execution.failures.length ? "failed" : "succeeded";
      // Cleanup still runs if recording an execution result fails.
      try { persist(file, journal); } finally { await cleanup(journal, file, adapters); }
    }
    return result(journal);
  } finally {
    release();
    // A failed initial journal write has not invoked any resource mutation adapter.
    if (!started) requireThat(journal.resources.every(resource => resource.state === "planned"), "Unjournaled mutation refused");
  }
}

function validateJournal(journal) {
  requireThat(fields(journal, ["schemaVersion", "kind", "plan", "createdAtUtc", "expiresAtUtc", "resources", "execution", "cleanup"], ["finishedAtUtc"])
    && journal.schemaVersion === 1 && journal.kind === "ephemeral-resource-lifecycle", "Unsupported lifecycle journal");
  validatePlan(journal.plan);
  const timestamp = value => typeof value === "string" && Number.isFinite(Date.parse(value));
  const validFailure = value => fields(value, ["stage", "code"], ["resourceKey"])
    && ["compile", "preflight", "provision", "exercise", "collect", "reconcile", "destroy", "verifyAbsent", "execution"].includes(value.stage)
    && ["adapter-failed", "ownership-not-established", "absence-not-established", "adapter-or-contract-failed", "process-interrupted"].includes(value.code)
    && (value.resourceKey === undefined || journal.plan.resources.some(resource => resource.key === value.resourceKey));
  requireThat(timestamp(journal.createdAtUtc) && timestamp(journal.expiresAtUtc)
    && (journal.finishedAtUtc === undefined || timestamp(journal.finishedAtUtc)), "Invalid lifecycle timestamps");
  requireThat(Array.isArray(journal.resources) && journal.resources.length === journal.plan.resources.length
    && fields(journal.execution, ["status", "failures"], ["artifactSha256", "resultSha256"])
    && ["running", "succeeded", "failed", "interrupted"].includes(journal.execution.status)
    && Array.isArray(journal.execution.failures) && journal.execution.failures.every(validFailure)
    && ["artifactSha256", "resultSha256"].every(key => journal.execution[key] === undefined || sha(journal.execution[key]))
    && exactFields(journal.cleanup, ["status", "attempts"])
    && ["pending", "running", "succeeded", "failed"].includes(journal.cleanup.status)
    && Array.isArray(journal.cleanup.attempts) && journal.cleanup.attempts.length <= 64
    && journal.cleanup.attempts.every(attempt => fields(attempt, ["startedAtUtc", "status", "failures"], ["finishedAtUtc"])
      && timestamp(attempt.startedAtUtc) && (attempt.finishedAtUtc === undefined || timestamp(attempt.finishedAtUtc))
      && ["running", "succeeded", "failed"].includes(attempt.status)
      && Array.isArray(attempt.failures) && attempt.failures.every(validFailure)), "Incomplete lifecycle journal");
  for (let index = 0; index < journal.resources.length; index++) {
    const resource = journal.resources[index], intent = journal.plan.resources[index];
    requireThat(fields(resource, ["intent", "state", "receipt"], ["absenceVerifiedAtUtc"])
      && exactFields(resource.intent, ["key", "kind", "name"])
      && Object.keys(intent).every(key => resource.intent[key] === intent[key])
      && (activeStates.has(resource.state) || ["planned", "absent"].includes(resource.state)), "Journal resource differs from exact planned identity");
    requireThat(resource.receipt === null || validReceipt(resource.receipt, intent, journal.plan), "Journal contains an unowned resource receipt");
    requireThat((resource.state !== "planned" || resource.receipt === null)
      && (!["created", "deleting", "deleted"].includes(resource.state) || resource.receipt !== null), "Resource state contradicts its ownership receipt");
    requireThat(resource.state !== "absent" || Number.isFinite(Date.parse(resource.absenceVerifiedAtUtc)), "Absent resource lacks verification evidence");
  }
}

/** Cleanup recovery only. Never recompiles, provisions or exercises a resumed run. */
export async function resumeCleanup({ journalPath, adapters }) {
  requireThat(typeof journalPath === "string" && journalPath.length > 0, "Journal path is required");
  requireAdapters(adapters, ["preflight", "destroy", "verifyAbsent", "reconcile"]);
  const file = path.resolve(journalPath), release = acquire(file);
  try {
    const journal = JSON.parse(readFileSync(file, "utf8"));
    validateJournal(journal);
    // Recovery may run in a different process with different credential bindings.
    // The current adapter account must match before reconciliation or deletion.
    let checked;
    try { checked = await adapters.preflight(context(journal)); }
    catch { throw new Error("Cleanup preflight failed; the journal remains available for recovery"); }
    requireThat(exactFields(checked, ["accountFingerprint"]) && checked.accountFingerprint === journal.plan.accountFingerprint,
      "Cleanup preflight account mismatch; no resource actions were started");
    if (journal.execution.status === "running") {
      journal.execution.status = "interrupted";
      journal.execution.failures.push(failure("execution", "process-interrupted"));
    }
    await cleanup(journal, file, adapters);
    return result(journal);
  } finally { release(); }
}
