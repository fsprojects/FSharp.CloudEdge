import assert from 'node:assert/strict';
import test from 'node:test';
import { resolvePython } from '../scripts/python.mjs';
import { spawnSync } from 'node:child_process';
import { mkdtempSync, mkdirSync, rmSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import path from 'node:path';

const valid = () => ({ status: 0, stdout: JSON.stringify({ major: 3, executable: process.execPath }) });
test('Python resolver honors the explicit interpreter and returns its verified executable', () => {
  const env = { CLOUDEDGE_PYTHON: 'chosen-python' };
  const calls = [];
  assert.equal(resolvePython({ env, probe(command, args, options) {
    calls.push(command); assert.equal(args[0], '-c'); assert.equal(options.env, env); return valid();
  } }), process.execPath);
  assert.deepEqual(calls, ['chosen-python']);
});
test('an invalid explicit interpreter fails without silently choosing a different Python', () => {
  const calls = [];
  assert.throws(() => resolvePython({ env: { CLOUDEDGE_PYTHON: 'bad-python' }, probe(command) {
    calls.push(command); return { status: 0, stdout: '' };
  } }), /CLOUDEDGE_PYTHON/);
  assert.deepEqual(calls, ['bad-python']);
});
test('Windows skips nonexecuting aliases and verifies the Python 3 launcher', () => {
  const calls = [];
  assert.equal(resolvePython({ env: {}, platform: 'win32', probe(command, args) {
    calls.push(command);
    if (command === 'py') { assert.equal(args[0], '-3'); return { status: 1 }; }
    return valid();
  } }), process.execPath);
  assert.deepEqual(calls, ['py', 'python']);
});
test('wrong Python versions and unverified executable paths are rejected', () => {
  for (const identity of [{ major: 2, executable: process.execPath }, { major: 3, executable: 'relative/python' }]) {
    assert.throws(() => resolvePython({ env: {}, probe: () => ({ status: 0, stdout: JSON.stringify(identity) }) }), /unavailable/);
  }
});

test('Python CLI forwards literal arguments and environment and retains a nonzero exit code', () => {
  const scratch = fileURLToPath(new URL('.scratch/python-cli/', import.meta.url));
  mkdirSync(scratch, { recursive: true });
  const directory = mkdtempSync(path.join(scratch, 'run with spaces-'));
  try {
    const script = 'import json,os,sys; print(json.dumps({"args":sys.argv[1:],"cwd":os.getcwd(),"value":os.environ["CLOUDEDGE_PYTHON_TEST_VALUE"]})); sys.exit(7)';
    const result = spawnSync(process.execPath, [fileURLToPath(new URL('../scripts/python.mjs', import.meta.url)), '-c', script, 'literal argument with spaces'], {
      cwd: directory, encoding: 'utf8', env: { ...process.env, CLOUDEDGE_PYTHON: resolvePython(), CLOUDEDGE_PYTHON_TEST_VALUE: 'kept value' },
    });
    assert.ifError(result.error);
    assert.equal(result.status, 7, result.stderr);
    assert.deepEqual(JSON.parse(result.stdout), { args: ['literal argument with spaces'], cwd: directory, value: 'kept value' });
  } finally {
    assert.ok(directory.startsWith(scratch));
    rmSync(directory, { recursive: true, force: true });
  }
});
