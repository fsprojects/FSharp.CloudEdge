import assert from 'node:assert/strict';
import { test } from 'node:test';
import { mkdir, mkdtemp, readFile, rm, writeFile } from 'node:fs/promises';
import path from 'node:path';
import { promisify } from 'node:util';
import { execFile, spawn } from 'node:child_process';
import { spawnOwned, stopOwned } from '../../scripts/integration/process-lifetime.mjs';
import { validateManifest, selectCases, summarize, validateLibraryCoverage, executeStep, junitReport, sdkRoot } from '../../scripts/integration/suite.mjs';

const execute = promisify(execFile);
const scratch = path.join(sdkRoot, 'tests/.scratch/integration-suite');
const temporary = async prefix => { await mkdir(scratch, { recursive: true }); return mkdtemp(path.join(scratch, prefix)); };
const row = (id, extra = {}) => ({ id, owner: 'test', contract: id, implementation: 'implemented', profiles: ['local'], steps: [{ root: 'sdk', command: 'node', args: ['-e', 'process.exit(0)'] }], ...extra });
const manifest = cases => ({ schemaVersion: 1, profiles: ['local', 'cloudflare', 'release'], cases });

test('Pi-style teardown remains safe when the owned child exited before stop', async () => {
  const child = spawnOwned(process.execPath, ['-e', 'process.exit(7)'], { stdio: ['ignore', 'pipe', 'pipe'] });
  child.stdout.resume(); child.stderr.resume();
  await new Promise((resolve, reject) => { child.once('close', resolve); child.once('error', reject); });
  assert.equal(child.exitCode, 7);
  await stopOwned(child);
  await stopOwned(child);
});
test('POSIX escalation failures reject owned teardown instead of escaping the timer', async () => {
  const child = spawnOwned(process.execPath, ['-e', 'setInterval(()=>{},1000)'], { stdio: ['ignore', 'pipe', 'pipe'] });
  child.stdout.resume(); child.stderr.resume();
  const descriptor = Object.getOwnPropertyDescriptor(process, 'platform');
  const originalKill = process.kill;
  const failure = Object.assign(new Error('kill denied'), { code: 'EPERM' });
  const signals = [];
  try {
    Object.defineProperty(process, 'platform', { value: 'linux' });
    process.kill = (pid, signal) => { assert.equal(pid, -child.pid); signals.push(signal); if (signal === 'SIGKILL') throw failure; return true; };
    await assert.rejects(stopOwned(child, 1), error => error === failure);
    assert.deepEqual(signals, ['SIGTERM', 'SIGKILL']);
  } finally {
    process.kill = originalKill;
    Object.defineProperty(process, 'platform', descriptor);
    await stopOwned(child);
  }
});

test('gated step wrapper decodes UTF-8 split across stdin chunks', async () => {
  const child = spawn(process.execPath, [path.join(sdkRoot, 'scripts/integration/step-process.mjs')], { stdio: ['pipe', 'pipe', 'pipe'] });
  const payload = Buffer.from(JSON.stringify({ executable: process.execPath,
    args: ['-e', 'process.stdout.write(process.argv[1]);', 'λ-value'] }));
  const split = payload.indexOf(Buffer.from('λ')) + 1;
  let output = '', errors = '';
  child.stdout.setEncoding('utf8'); child.stdout.on('data', chunk => { output += chunk; });
  child.stderr.setEncoding('utf8'); child.stderr.on('data', chunk => { errors += chunk; });
  const complete = new Promise((resolve, reject) => { child.on('error', reject); child.on('close', resolve); });
  child.stdin.write(payload.subarray(0, split));
  const timer = setTimeout(() => child.stdin.end(payload.subarray(split)), 150);
  try { assert.equal(await complete, 0, errors); assert.equal(output, 'λ-value'); }
  finally { clearTimeout(timer); if (child.exitCode === null) child.kill(); }
});

test('selection includes prerequisites once, but does not substitute local cases for hosted cases', () => {
  const suite = manifest([row('build.test'), row('local.test', { dependsOn: ['build.test'] }), row('cloud.test', { profiles: ['cloudflare'], dependsOn: ['build.test'] })]);
  assert.deepEqual(selectCases(suite, 'cloudflare').map(c => c.id), ['build.test', 'cloud.test']);
  assert.deepEqual(selectCases(suite, 'local', ['local.test', 'cloud.test']).map(c => c.id), ['build.test', 'local.test', 'cloud.test']);
});
test('unknown prerequisites, duplicate case IDs and cycles cannot silently lose required coverage', () => {
  assert.throws(() => validateManifest(manifest([row('a.test', { dependsOn: ['missing.test'] })])), /Unknown dependency/);
  assert.throws(() => validateManifest(manifest([row('a.test'), row('a.test')])), /duplicate/);
  assert.throws(() => validateManifest(manifest([row('a.test', { dependsOn: ['b.test'] }), row('b.test', { dependsOn: ['a.test'] })])), /cycle/);
});
test('empty selection, missing fixture and failed cleanup cannot yield a passing summary', () => {
  assert.equal(summarize([]).passed, false);
  for (const status of ['failed', 'blocked', 'not-implemented']) assert.equal(summarize([{ status: 'passed' }, { status }]).passed, false);
});
test('JUnit reports missing hosted coverage as a failure and escapes fixture names', () => {
  const xml = junitReport({ profile: 'cloudflare', results: [
    { id: 'fixture.test', owner: 'SDK & consumers', status: 'not-implemented', reason: '<missing>', contract: 'remote lifecycle', steps: [] },
  ] });
  assert.match(xml, /failures="1"/); assert.match(xml, /skipped="0"/);
  assert.match(xml, /SDK &amp; consumers/); assert.match(xml, /&lt;missing&gt;/);
  assert.match(xml, /type="not-implemented"/);
});
test('every selected SDK library has a required runtime test owner, additions require an explicit disposition', async () => {
  const suite = JSON.parse(await readFile(path.join(sdkRoot, 'tests/integration/suite.json')));
  const delivery = JSON.parse(await readFile(path.join(sdkRoot, 'config/sdk-delivery.json')));
  assert.equal(validateLibraryCoverage(suite, delivery).selectedLibraries, 30);
  assert.throws(() => validateLibraryCoverage(suite, { libraries: [...delivery.libraries, { partition: 'future-sdk' }] }), /missing=future-sdk/);
  assert.throws(() => validateLibraryCoverage(suite, { libraries: delivery.libraries.slice(1) }), /unknown=/);
});
test('child timeout fails and closes the actual process before reporting', async () => {
  const directory = await temporary('timeout-');
  try {
    const result = await executeStep({ root: 'sdk', command: 'node', args: ['-e', 'setInterval(() => {}, 1000)'] }, { sdk: directory }, path.join(directory, 'step.log'), 80);
    assert.equal(result.status, 'failed'); assert.equal(result.timedOut, true);
    assert.ok(result.terminationSignal || result.exitCode !== 0);
    assert.match(result.logSha256, /^[a-f0-9]{64}$/);
  } finally { await rm(directory, { recursive: true, force: true }); }
});
for (const mode of ['timeout', 'parent-exit', 'abort']) test(`owned descendants close on ${mode}`, async () => {
  const parentExits = mode === 'parent-exit';
  const directory = await temporary('descendant-');
  const pidsFile = path.join(directory, 'pids.json');
  const source = `const {spawn}=require('node:child_process');const fs=require('node:fs');
    const descendant=spawn(process.execPath,['-e','process.send("ready");setInterval(()=>{},1000)'],{stdio:['ignore','inherit','inherit','ipc']});
    fs.writeFileSync(${JSON.stringify(pidsFile)},JSON.stringify([process.pid,descendant.pid]));
    ${parentExits ? 'descendant.once("message",()=>process.exit(0));' : 'setInterval(()=>{},1000);'}`;
  const controller = new AbortController();
  const abortTimer = mode === 'abort' ? setTimeout(() => controller.abort(), 1000) : undefined;
  try {
    const result = await executeStep({ root: 'sdk', command: 'node', args: ['-e', source] }, { sdk: directory }, path.join(directory, 'step.log'), mode === 'abort' ? 10000 : 1000, controller.signal);
    const normalJobClose = parentExits && process.platform === 'win32';
    assert.equal(result.status, normalJobClose ? 'passed' : 'failed');
    assert.equal(result.timedOut, !normalJobClose && mode !== 'abort');
    const pids = JSON.parse(await readFile(pidsFile, 'utf8'));
    assert.equal(pids.length, 2);
    for (const pid of pids) assert.throws(() => process.kill(pid, 0), { code: 'ESRCH' });
  } finally { clearTimeout(abortTimer); await rm(directory, { recursive: true, force: true }); }
});
test('step retains exact argv, inherited environment, output and failure exit code', async () => {
  const directory = await temporary('argv-');
  const log = path.join(directory, 'step.log');
  const argv = ['space value', 'quote"value', 'unicode-λ', '%PATH%', 'trailing\\'];
  try {
    const result = await executeStep({ root: 'sdk', command: 'node', args: ['-e',
      'console.log(JSON.stringify({argv:process.argv.slice(1),root:process.env.CLOUDEDGE_ROOT}));console.error("stderr-proof");process.exit(7);', ...argv] }, { sdk: directory }, log, 10000);
    assert.equal(result.status, 'failed'); assert.equal(result.exitCode, 7);
    const output = await readFile(log, 'utf8');
    assert.match(output, /stderr-proof/);
    const structured = output.split(/\r?\n/).find(line => line.startsWith('{'));
    assert.deepEqual(JSON.parse(structured), { argv, root: directory });
  } finally { await rm(directory, { recursive: true, force: true }); }
});
test('missing sibling repository is reported as blocked', async () => {
  const outcome = await executeStep({ root: 'conclave', command: 'node', args: [] }, {}, '/unused');
  assert.equal(outcome.status, 'blocked');
});
test('CLI retains a failure, blocks dependent work, still executes independent work, and lists missing hosted fixture', async () => {
  const directory = await temporary('cli-');
  try {
    const file = path.join(directory, 'suite.json'), output = path.join(directory, 'run');
    await writeFile(file, JSON.stringify(manifest([
      row('fail.test', { steps: [{ root: 'sdk', command: 'node', args: ['-e', 'process.exit(3)'] }] }),
      row('dependent.test', { dependsOn: ['fail.test'] }), row('independent.test'),
      row('missing.test', { implementation: 'planned', steps: undefined, reason: 'No hosted fixture' }),
    ])));
    await assert.rejects(execute(process.execPath, [path.join(sdkRoot, 'scripts/integration/suite.mjs'), '--profile', 'local', '--manifest', file, '--output', output]), error => error.code === 1);
    const report = JSON.parse(await readFile(path.join(output, 'report.json')));
    assert.deepEqual(Object.keys(report.runnerInputSha256), ['suite.mjs', 'process-lifetime.mjs', 'step-job.ps1', 'step-process.mjs']);
    for (const digest of Object.values(report.runnerInputSha256)) assert.match(digest, /^[a-f0-9]{64}$/);
    assert.equal(report.passed, false);
    assert.deepEqual(report.results.map(row => row.status), ['failed', 'blocked', 'passed', 'not-implemented']);
    assert.equal(report.results[0].steps[0].exitCode, 3);
    assert.equal(report.results[1].steps.length, 0);
    await assert.rejects(execute(process.execPath, [path.join(sdkRoot, 'scripts/integration/suite.mjs'), '--profile', 'local', '--manifest', file, '--output', output]), /Refusing to overwrite/);
  } finally { await rm(directory, { recursive: true, force: true }); }
});
