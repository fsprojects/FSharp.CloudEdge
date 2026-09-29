import assert from "node:assert/strict";
import { spawnSync } from "node:child_process";
import { existsSync, mkdtempSync, readFileSync, readdirSync, rmSync, statSync, writeFileSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import test from "node:test";
import { resumeCleanup, runLifecycle } from "../../scripts/integration/lifecycle.mjs";

const fingerprint = "a".repeat(64), artifactSha256 = "b".repeat(64), resultSha256 = "c".repeat(64);
function fixture(t, count = 3) {
  const directory = mkdtempSync(path.join(os.tmpdir(), "cloudedge-lifecycle-"));
  t.after(() => rmSync(directory, { recursive: true, force: true }));
  const journalPath = path.join(directory, "journal.json");
  const plan = { runId: "run-0123456789abcdef", accountFingerprint: fingerprint, ttlSeconds: 60,
    resources: Array.from({ length: count }, (_, index) => ({ key: `r${index}`, kind: "worker", name: `ceval-0123456789abcdef-r${index}` })) };
  const remote = new Map(), events = [];
  const receipt = resource => ({ id: `id-${resource.key}`, name: resource.name, kind: resource.kind,
    runId: plan.runId, accountFingerprint: plan.accountFingerprint });
  const adapters = {
    async compile() { events.push("compile"); return { artifactSha256 }; },
    async preflight() { events.push("preflight"); return { accountFingerprint: fingerprint }; },
    async provision({ resource }) {
      events.push(`provision:${resource.key}`);
      const journal = JSON.parse(readFileSync(journalPath, "utf8"));
      assert.equal(journal.resources.find(item => item.intent.key === resource.key).state, "creating", "Intent must precede create");
      const created = receipt(resource); remote.set(resource.name, created); return created;
    },
    async exercise() { events.push("exercise"); return { passed: true }; },
    async collect() { events.push("collect"); return { resultSha256 }; },
    async destroy({ resource, receipt: owned }) {
      events.push(`destroy:${resource.key}`);
      assert.equal(owned.id, remote.get(resource.name)?.id);
      remote.delete(resource.name);
    },
    async verifyAbsent({ resource }) { events.push(`verify:${resource.key}`); return { absent: !remote.has(resource.name) }; },
    async reconcile({ resource }) {
      events.push(`reconcile:${resource.key}`);
      return remote.has(resource.name) ? { state: "owned", receipt: remote.get(resource.name) } : { state: "absent" };
    },
  };
  return { directory, journalPath, plan, adapters, remote, events, receipt,
    run: () => runLifecycle({ plan, journalPath, adapters }),
    journal: () => JSON.parse(readFileSync(journalPath, "utf8")) };
}

test("successful lifecycle journals exact ownership, collects results and verifies reverse teardown", async t => {
  const f = fixture(t); const result = await f.run();
  assert.equal(result.status, "passed"); assert.equal(result.execution.artifactSha256, artifactSha256);
  assert.equal(result.execution.resultSha256, resultSha256);
  assert.deepEqual(f.events, ["compile", "preflight", "provision:r0", "provision:r1", "provision:r2", "exercise", "collect",
    "destroy:r2", "verify:r2", "destroy:r1", "verify:r1", "destroy:r0", "verify:r0"]);
  assert.equal(f.remote.size, 0); assert.equal(statSync(f.journalPath).mode & 0o777, 0o600);
  assert.deepEqual(readdirSync(f.directory), ["journal.json"]);
  assert.ok(f.journal().resources.every(resource => resource.state === "absent" && resource.receipt && resource.absenceVerifiedAtUtc));
  assert.match(result.qualification, /acceptance-depends-on-configured-adapters/);
});

test("compile failure invokes no resource callbacks and remains an execution failure", async t => {
  const f = fixture(t);
  f.adapters.compile = async () => { f.events.push("compile"); throw new Error("compiler failed"); };
  const result = await f.run();
  assert.equal(result.status, "failed"); assert.equal(result.execution.failures[0].stage, "compile");
  assert.equal(result.cleanup.status, "succeeded"); assert.deepEqual(f.events, ["compile"]);
  assert.ok(f.journal().resources.every(resource => resource.state === "planned"));
});

test("preflight account mismatch prevents every provisioning callback", async t => {
  const f = fixture(t);
  f.adapters.preflight = async () => ({ accountFingerprint: "d".repeat(64) });
  const result = await f.run();
  assert.equal(result.execution.failures[0].stage, "preflight");
  assert.deepEqual(f.events, ["compile"]); assert.equal(f.remote.size, 0);
});

test("partial create failure reconciles only the attempted identity then clears earlier resources", async t => {
  const f = fixture(t), create = f.adapters.provision;
  f.adapters.provision = async context => {
    if (context.resource.key === "r1") { f.events.push("provision:r1"); throw new Error("create rejected"); }
    return create(context);
  };
  const result = await f.run();
  assert.equal(result.execution.failures[0].stage, "provision"); assert.equal(result.cleanup.status, "succeeded");
  assert.deepEqual(f.events, ["compile", "preflight", "provision:r0", "provision:r1", "collect",
    "reconcile:r1", "verify:r1", "destroy:r0", "verify:r0"]);
  assert.equal(result.resources[2].state, "planned"); assert.equal(f.remote.size, 0);
});

test("lost create response discovers the exact owned resource and cleans it", async t => {
  const f = fixture(t), create = f.adapters.provision;
  f.adapters.provision = async context => {
    const created = await create(context);
    if (context.resource.key === "r1") throw new Error("response lost after creation");
    return created;
  };
  const result = await f.run();
  assert.equal(result.status, "failed"); assert.equal(result.cleanup.status, "succeeded");
  assert.ok(f.events.indexOf("reconcile:r1") < f.events.indexOf("destroy:r1"));
  assert.equal(f.journal().resources[1].receipt.id, "id-r1"); assert.equal(f.remote.size, 0);
});

test("delete failure and failed absence checks do not stop other reverse cleanup", async t => {
  const f = fixture(t), destroy = f.adapters.destroy;
  f.adapters.destroy = async context => {
    if (context.resource.key === "r1") { f.events.push("destroy:r1"); throw new Error("delete failed"); }
    return destroy(context);
  };
  const result = await f.run();
  assert.equal(result.execution.status, "succeeded"); assert.equal(result.cleanup.status, "failed"); assert.equal(result.status, "failed");
  assert.deepEqual(result.cleanup.attempts[0].failures.map(error => error.stage), ["destroy", "verifyAbsent"]);
  assert.deepEqual(f.events.filter(event => event.startsWith("destroy:")), ["destroy:r2", "destroy:r1", "destroy:r0"]);
  assert.equal(f.remote.size, 1); assert.equal(result.resources[1].state, "deleting");
  f.adapters.destroy = destroy;
  const resumed = await resumeCleanup({ journalPath: f.journalPath, adapters: f.adapters });
  assert.equal(resumed.status, "passed"); assert.equal(resumed.cleanup.attempts.length, 2);
  assert.equal(resumed.cleanup.attempts[0].status, "failed"); assert.equal(resumed.cleanup.attempts[1].status, "succeeded");
  assert.equal(f.remote.size, 0);
});

test("a successful delete acknowledgement cannot substitute for verified absence", async t => {
  const f = fixture(t, 1);
  f.adapters.destroy = async () => {};
  const result = await f.run();
  assert.equal(result.status, "failed"); assert.equal(result.cleanup.attempts[0].failures[0].code, "absence-not-established");
  assert.equal(f.remote.size, 1); assert.equal(result.resources[0].state, "deleted");
});

test("exercise, collection and teardown failures retain separate outcomes without error text", async t => {
  const f = fixture(t, 1), secret = "secret-token-must-never-reach-journal";
  f.adapters.exercise = async () => { throw new Error(secret); };
  f.adapters.collect = async () => { throw new Error(secret); };
  f.adapters.destroy = async () => { throw new Error(secret); };
  const result = await f.run();
  assert.deepEqual(result.execution.failures.map(error => error.stage), ["exercise", "collect"]);
  assert.equal(result.cleanup.status, "failed");
  assert.deepEqual(result.cleanup.attempts[0].failures.map(error => error.stage), ["destroy", "verifyAbsent"]);
  assert.equal(readFileSync(f.journalPath, "utf8").includes(secret), false);
});

test("an explicit failed exercise still collects before clearing resources", async t => {
  const f = fixture(t, 1);
  f.adapters.exercise = async () => ({ passed: false });
  const result = await f.run();
  assert.equal(result.execution.failures[0].stage, "exercise"); assert.equal(result.cleanup.status, "succeeded");
  assert.ok(f.events.indexOf("collect") < f.events.indexOf("destroy:r0")); assert.equal(f.remote.size, 0);
});

test("reconciliation refuses unowned or different-name resources without broad deletion", async t => {
  const f = fixture(t), create = f.adapters.provision;
  f.adapters.provision = async context => {
    if (context.resource.key === "r1") throw new Error("uncertain");
    return create(context);
  };
  f.adapters.reconcile = async ({ resource }) => {
    assert.equal(resource.name, f.plan.resources[1].name);
    return { state: "owned", receipt: { ...f.receipt(resource), name: "ceval-other-run" } };
  };
  const result = await f.run();
  assert.equal(result.cleanup.status, "failed"); assert.equal(result.resources[1].state, "refused");
  assert.equal(result.cleanup.attempts[0].failures[0].code, "ownership-not-established");
  assert.deepEqual(f.events.filter(event => event.startsWith("destroy:")), ["destroy:r0"]);
});

test("receipt metadata and plan metadata reject arbitrary credentials", async t => {
  const f = fixture(t, 1);
  await assert.rejects(runLifecycle({ plan: { ...f.plan, apiToken: "secret" }, journalPath: f.journalPath, adapters: f.adapters }), /arbitrary metadata/);
  assert.equal(existsSync(f.journalPath), false);
  const create = f.adapters.provision;
  f.adapters.provision = async context => ({ ...await create(context), apiToken: "secret-not-persisted" });
  const result = await f.run();
  assert.equal(result.execution.failures[0].stage, "provision"); assert.equal(result.cleanup.status, "succeeded");
  assert.equal(readFileSync(f.journalPath, "utf8").includes("secret-not-persisted"), false);
});

test("resume after real process termination reconciles an uncertain create and never reprovisions", async t => {
  const f = fixture(t, 1), remoteFile = path.join(f.directory, "simulated-resource.json");
  const source = `
    import { writeFileSync } from 'node:fs';
    import { runLifecycle } from ${JSON.stringify(new URL("../../scripts/integration/lifecycle.mjs", import.meta.url).href)};
    const plan = ${JSON.stringify(f.plan)};
    await runLifecycle({ plan, journalPath: ${JSON.stringify(f.journalPath)}, adapters: {
      compile: async () => ({artifactSha256: ${JSON.stringify(artifactSha256)}}),
      preflight: async () => ({accountFingerprint: plan.accountFingerprint}),
      provision: async ({resource}) => {
        writeFileSync(${JSON.stringify(remoteFile)}, JSON.stringify({id:'id-r0', name:resource.name, kind:resource.kind,
          runId:plan.runId, accountFingerprint:plan.accountFingerprint}));
        process.exit(17);
      },
      exercise: async () => {}, collect: async () => {}, destroy: async () => {}, verifyAbsent: async () => {}, reconcile: async () => {}
    }});`;
  const child = spawnSync(process.execPath, ["--input-type=module", "-e", source], { encoding: "utf8", timeout: 10000 });
  assert.equal(child.status, 17, child.stderr); assert.equal(f.journal().resources[0].state, "creating");
  assert.equal(f.journal().resources[0].receipt, null);
  f.remote.set(f.plan.resources[0].name, JSON.parse(readFileSync(remoteFile, "utf8")));
  const result = await resumeCleanup({ journalPath: f.journalPath, adapters: f.adapters });
  assert.equal(result.execution.status, "interrupted"); assert.equal(result.status, "failed");
  assert.equal(result.cleanup.status, "succeeded");
  assert.deepEqual(f.events, ["preflight", "reconcile:r0", "destroy:r0", "verify:r0"]);
  assert.equal(f.remote.size, 0); assert.equal(existsSync(`${f.journalPath}.lock`), false);
});

test("recovery refuses forged ownership and missing absence evidence before calling adapters", async t => {
  const f = fixture(t, 1); await f.run(); f.events.length = 0;
  const journal = f.journal(); journal.resources[0].receipt.accountFingerprint = "d".repeat(64);
  writeFileSync(f.journalPath, JSON.stringify(journal));
  await assert.rejects(resumeCleanup({ journalPath: f.journalPath, adapters: f.adapters }), /unowned resource/);
  assert.deepEqual(f.events, []);
  journal.resources[0].receipt.accountFingerprint = fingerprint; delete journal.resources[0].absenceVerifiedAtUtc;
  writeFileSync(f.journalPath, JSON.stringify(journal));
  await assert.rejects(resumeCleanup({ journalPath: f.journalPath, adapters: f.adapters }), /lacks verification/);
});

test("recovery rejects arbitrary fields instead of rewriting them into its journal", async t => {
  const f = fixture(t, 1); await f.run(); f.events.length = 0;
  const journal = f.journal(); journal.execution.credentials = { token: "secret" };
  writeFileSync(f.journalPath, JSON.stringify(journal));
  await assert.rejects(resumeCleanup({ journalPath: f.journalPath, adapters: f.adapters }), /Incomplete lifecycle/);
  assert.deepEqual(f.events, []);
});

test("an active run refuses concurrent recovery and existing journals cannot be reprovisioned", async t => {
  const f = fixture(t, 1), create = f.adapters.provision;
  f.adapters.provision = async context => {
    await assert.rejects(resumeCleanup({ journalPath: f.journalPath, adapters: f.adapters }), /in use/);
    return create(context);
  };
  await f.run(); const count = f.events.length;
  await assert.rejects(f.run(), /already exists/);
  assert.equal(f.events.length, count);
});

test("absent recovery results still require a separate absence check", async t => {
  const f = fixture(t, 1);
  f.adapters.provision = async () => { throw new Error("uncertain"); };
  f.adapters.verifyAbsent = async () => ({ absent: false });
  const result = await f.run();
  assert.equal(result.cleanup.status, "failed");
  assert.equal(result.cleanup.attempts[0].failures[0].stage, "verifyAbsent");
});

test("cleanup recovery requires current account preflight before every resource action", async t => {
  const f = fixture(t, 1), destroy = f.adapters.destroy;
  f.adapters.destroy = async () => { throw new Error("delete temporarily unavailable"); };
  await f.run(); f.events.length = 0;
  const original = readFileSync(f.journalPath, "utf8");
  const { preflight, ...withoutPreflight } = f.adapters;
  await assert.rejects(resumeCleanup({ journalPath: f.journalPath, adapters: withoutPreflight }), /preflight/);
  assert.deepEqual(f.events, []); assert.equal(f.remote.size, 1);
  f.adapters.preflight = async () => ({ accountFingerprint: "d".repeat(64) });
  await assert.rejects(resumeCleanup({ journalPath: f.journalPath, adapters: f.adapters }), /account mismatch/);
  assert.deepEqual(f.events, []); assert.equal(f.remote.size, 1);
  assert.equal(readFileSync(f.journalPath, "utf8"), original, "Account mismatch must leave the journal resumable");
  f.adapters.preflight = preflight; f.adapters.destroy = destroy;
  const resumed = await resumeCleanup({ journalPath: f.journalPath, adapters: f.adapters });
  assert.equal(resumed.cleanup.status, "succeeded"); assert.equal(f.remote.size, 0);
  assert.deepEqual(f.events, ["preflight", "destroy:r0", "verify:r0"]);
});
