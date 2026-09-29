import assert from 'node:assert/strict';
import { test } from 'node:test';
import { mkdtemp, readFile, rm, writeFile } from 'node:fs/promises';
import os from 'node:os';
import path from 'node:path';
import { promisify } from 'node:util';
import { execFile } from 'node:child_process';
import { validateManifest, selectCases, summarize, validateLibraryCoverage, executeStep, junitReport, sdkRoot } from '../../scripts/integration/suite.mjs';

const execute = promisify(execFile);
const row = (id, extra = {}) => ({ id, owner: 'test', contract: id, implementation: 'implemented', profiles: ['local'], steps: [{ root: 'sdk', command: 'node', args: ['-e', 'process.exit(0)'] }], ...extra });
const manifest = cases => ({ schemaVersion: 1, profiles: ['local', 'cloudflare', 'release'], cases });

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
  assert.equal(validateLibraryCoverage(suite, delivery).selectedLibraries, 31);
  assert.throws(() => validateLibraryCoverage(suite, { libraries: [...delivery.libraries, { partition: 'future-sdk' }] }), /missing=future-sdk/);
  assert.throws(() => validateLibraryCoverage(suite, { libraries: delivery.libraries.slice(1) }), /unknown=/);
});
test('child timeout fails and closes the actual process before reporting', async () => {
  const directory = await mkdtemp(path.join(os.tmpdir(), 'cloudedge-suite-timeout-'));
  try {
    const result = await executeStep({ root: 'sdk', command: 'node', args: ['-e', 'setInterval(() => {}, 1000)'] }, { sdk: directory }, path.join(directory, 'step.log'), 80);
    assert.equal(result.status, 'failed'); assert.equal(result.timedOut, true); assert.ok(result.terminationSignal);
    assert.match(result.logSha256, /^[a-f0-9]{64}$/);
  } finally { await rm(directory, { recursive: true, force: true }); }
});
test('missing sibling repository is reported as blocked', async () => {
  const outcome = await executeStep({ root: 'conclave', command: 'node', args: [] }, {}, '/unused');
  assert.equal(outcome.status, 'blocked');
});
test('CLI retains a failure, blocks dependent work, still executes independent work, and lists missing hosted fixture', async () => {
  const directory = await mkdtemp(path.join(os.tmpdir(), 'cloudedge-suite-cli-'));
  try {
    const file = path.join(directory, 'suite.json'), output = path.join(directory, 'run');
    await writeFile(file, JSON.stringify(manifest([
      row('fail.test', { steps: [{ root: 'sdk', command: 'node', args: ['-e', 'process.exit(3)'] }] }),
      row('dependent.test', { dependsOn: ['fail.test'] }), row('independent.test'),
      row('missing.test', { implementation: 'planned', steps: undefined, reason: 'No hosted fixture' }),
    ])));
    await assert.rejects(execute(process.execPath, [path.join(sdkRoot, 'scripts/integration/suite.mjs'), '--profile', 'local', '--manifest', file, '--output', output]), error => error.code === 1);
    const report = JSON.parse(await readFile(path.join(output, 'report.json')));
    assert.equal(report.passed, false);
    assert.deepEqual(report.results.map(row => row.status), ['failed', 'blocked', 'passed', 'not-implemented']);
    assert.equal(report.results[0].steps[0].exitCode, 3);
    assert.equal(report.results[1].steps.length, 0);
    await assert.rejects(execute(process.execPath, [path.join(sdkRoot, 'scripts/integration/suite.mjs'), '--profile', 'local', '--manifest', file, '--output', output]), /Refusing to overwrite/);
  } finally { await rm(directory, { recursive: true, force: true }); }
});
