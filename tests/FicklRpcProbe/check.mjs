import assert from 'node:assert/strict';
import { mkdtempSync, mkdirSync, readdirSync, rmSync, writeFileSync } from 'node:fs';
import { spawnSync } from 'node:child_process';
import { createServer } from 'node:net';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { build } from '../../profiles/agents-pi/node_modules/esbuild/lib/main.js';
import { spawnOwned, stopOwned } from '../../scripts/integration/process-lifetime.mjs';

const root = fileURLToPath(new URL('../..', import.meta.url));
const scratch = path.join(root, 'tests/.scratch/fickl-rpc-probe');
mkdirSync(scratch, { recursive: true });
const run = mkdtempSync(path.join(scratch, 'run-'));
let child;
try {
  const compilerPath = process.env.CLOUDEDGE_FICKL ?? 'fickl';
  const version = spawnSync(compilerPath, ['--help'], { cwd: root, encoding: 'utf8', timeout: 10000 });
  assert.equal(version.status, 0, version.stderr);
  const banner = version.stdout.replace(/\x1b\[[0-9;]*m/g, '').split('\n')[0];
  assert.match(banner, /^Fickl 0\.7\.3\b/, 'Expected the release compiler');
  const fable = path.join(run, 'fable');
  const compiler = spawnSync(compilerPath,
    [path.join(root, 'tests/FicklRpcProbe/FicklRpcProbe.fsproj'), '--outDir', fable, '--noCache'],
    { cwd: root, encoding: 'utf8', timeout: 180000 });
  assert.equal(compiler.status, 0, compiler.stdout + compiler.stderr);
  const libraries = readdirSync(path.join(fable, 'fable_modules')).filter(name => name.startsWith('fable-library-js.'));
  assert.equal(libraries.length, 1, 'Expected one Fickl runtime library');
  const helper = path.join(fable, 'fable_modules', libraries[0], 'TaggedUnion.js');
  await build({
    entryPoints: [path.join(root, 'tests/FicklRpcProbe/worker.mjs')],
    outfile: path.join(run, 'bundle.mjs'), bundle: true, format: 'esm', platform: 'browser',
    external: ['cloudflare:*'], alias: { 'fickl-tagged-union': helper, 'fsharp-probe': path.join(fable, 'Probe.js') }
  });
  writeFileSync(path.join(run, 'wrangler.json'), JSON.stringify({
    name: 'cloudedge-fickl-rpc-probe', main: 'bundle.mjs', no_bundle: true,
    compatibility_date: '2026-09-01',
    durable_objects: { bindings: [{ name: 'PROBE', class_name: 'CloneProbe' }] },
    migrations: [{ tag: 'v1', new_sqlite_classes: ['CloneProbe'] }]
  }));
  const socket = createServer();
  await new Promise(resolve => socket.listen(0, '127.0.0.1', resolve));
  const port = socket.address().port;
  await new Promise(resolve => socket.close(resolve));
  child = spawnOwned(process.env.CLOUDEDGE_CELLD ?? 'celld',
    ['dev', run, '--host', '127.0.0.1', '--port', String(port), '--no-watch'],
    { cwd: run, stdio: ['ignore', 'pipe', 'pipe'], env: { ...process.env, CELLD_OTEL: '0', NO_COLOR: '1' } });
  let output = '';
  for (const stream of [child.stdout, child.stderr]) stream.on('data', value => { output += value; });
  let plain;
  for (let i = 0; i < 300; i++) {
    try { plain = await (await fetch(`http://127.0.0.1:${port}/plain`)).json(); break; }
    catch { await new Promise(resolve => setTimeout(resolve, 100)); }
  }
  assert.deepEqual(plain, { ok: true, value: { type: 'text', text: 'example' }, identity: true, tag: 'text' }, `Plain RPC control failed: ${output}`);
  const request = async mode => (await fetch(`http://127.0.0.1:${port}/${mode}`)).json();
  const tagged = await request('tagged');
  assert.deepEqual(tagged, plain, 'Already-tagged payload must retain identity and pass RPC');
  const fsharp = await request('fsharp');
  assert.equal(fsharp.ok, true, 'F#-constructed inline union must pass RPC');
  assert.deepEqual(fsharp.value, plain.value);
  assert.equal(fsharp.identity, true, 'F#-constructed union must retain its payload identity');
  const missing = await request('missing');
  assert.deepEqual(missing, { ok: false, name: 'DataCloneError', identity: false, tag: 'text' });
  const conflicting = await request('conflicting');
  assert.deepEqual(conflicting, missing, 'Conflicting discriminator must not pass through');
  const copy = await request('copy');
  assert.deepEqual(copy, { ok: true, value: plain.value, identity: false, tag: 'text' });
  console.log(JSON.stringify({ compiler: banner, plain, tagged, fsharp, missing, conflicting, copy }));
} finally {
  if (child) await stopOwned(child, 10000);
  rmSync(run, { recursive: true, force: true });
}
