import assert from 'node:assert/strict';
import { createHash } from 'node:crypto';
import { spawn, spawnSync } from 'node:child_process';
import { once } from 'node:events';
import { createRequire } from 'node:module';
import { existsSync, mkdirSync, mkdtempSync, readFileSync, rmSync, writeFileSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { planProjects, checkProjects } from '../../scripts/projects.mjs';

const directory = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(directory, '../..');
const require = createRequire(path.join(root, 'profiles/agents-pi/package.json'));
const { build } = require('esbuild');
const hash = file => createHash('sha256').update(readFileSync(file)).digest('hex');
const configuration = JSON.parse(readFileSync(path.join(root, 'artifacts/sdk-build/targets.json'), 'utf8'));
const plan = planProjects(configuration, root);
plan.solution = path.join(root, 'SDK.Partitions.slnx');
const checked = checkProjects(plan, ['agents-pi']);
const target = configuration.targets.find(target => target.id === 'agents-pi');
assert.ok(target?.projection, 'Generate the strict agents-pi delivery first');
const inputFiles = ['tests/AgentsPi/Client.fs', 'tests/AgentsPi/worker.mjs', 'tests/AgentsPi/check.mjs', 'tests/AgentsPi/AgentsPi.fsproj', 'tests/AgentsPi/.config/dotnet-tools.json', 'tests/SDKComposition/SDKComposition.fsproj', 'tests/SDKComposition/Pi.fs', 'profiles/agents-pi/package.json', 'profiles/agents-pi/package-lock.json', 'artifacts/agents-pi/generation.json', ...target.files.map(file => target.outputDirectory + '/' + file)];
const inputs = () => Object.fromEntries(inputFiles.map(file => [file, hash(path.join(root, file))]));
const before = inputs();
const scratch = path.join(root, 'tests/.scratch/agents-pi');
mkdirSync(scratch, { recursive: true });
const run = mkdtempSync(path.join(scratch, 'run-'));
const report = { qualification: 'Actual Agents PiHarness and pi-durable SQLite Durable Object, SDK fauxProvider model', checked, inputSha256: before, assertions: [] };
const command = (arguments_, cwd = root, expected = 0) => {
  const result = spawnSync('dotnet', arguments_, { cwd, encoding: 'utf8', timeout: 120000 });
  writeFileSync(path.join(run, `command-${command.index++}.log`), result.stdout + result.stderr);
  assert.equal(result.error, undefined);
  assert.equal(result.status, expected, result.stdout + result.stderr);
  return result.stdout + result.stderr;
};
command.index = 0;
let child, output = '', url;
const start = async () => {
  output = '';
  child = spawn(process.execPath, [path.join(root, 'profiles/agents-pi/node_modules/wrangler/bin/wrangler.js'), 'dev', '--local', '--config', path.join(run, 'wrangler.json'), '--ip', '127.0.0.1', '--port', '0', '--inspector-port', '0', '--persist-to', path.join(run, 'state')], { cwd: root, detached: true, stdio: ['ignore', 'pipe', 'pipe'], env: { ...process.env, WRANGLER_SEND_METRICS: 'false', NO_COLOR: '1' } });
  for (const stream of [child.stdout, child.stderr]) stream.on('data', chunk => { output += chunk; });
  for (let i = 0; i < 300; i++) {
    const match = output.match(/Ready on (http:\/\/127\.0\.0\.1:\d+)/);
    if (match) { url = match[1]; return; }
    if (child.exitCode !== null) throw Error(output);
    await new Promise(resolve => setTimeout(resolve, 100));
  }
  throw Error('Local workerd startup timed out: ' + output);
};
const stop = async () => {
  if (!child || child.exitCode !== null) return;
  const exited = once(child, 'exit');
  process.kill(-child.pid, 'SIGTERM');
  let timer;
  try { await Promise.race([exited, new Promise((_, reject) => { timer = setTimeout(() => { process.kill(-child.pid, 'SIGKILL'); reject(Error('Local runtime shutdown timed out')); }, 10000); })]); }
  finally { clearTimeout(timer); writeFileSync(path.join(run, 'worker.log'), output); }
};
const request = async suffix => {
  const response = await fetch(url + '/agents/pi-probe/validation' + suffix, { signal: AbortSignal.timeout(20000) });
  const text = await response.text();
  assert.equal(response.status, 200, text);
  return JSON.parse(text);
};
try {
  for (const enabled of [false, true]) {
    const evaluated = JSON.parse(command(['msbuild', 'tests/SDKComposition/SDKComposition.fsproj', '-getItem:ProjectReference,Compile', `-p:CloudEdgeIncludeAgentsPi=${enabled}`]));
    assert.equal(evaluated.Items.ProjectReference.some(item => item.Identity.includes('AgentsPi')), enabled);
    assert.equal(evaluated.Items.Compile.some(item => item.Identity === 'Pi.fs'), enabled);
  }
  report.assertions.push('Default SDK composition excludes Agents Pi; its explicit property enables both the project and consumer');
  command(['build', 'tests/SDKComposition/SDKComposition.fsproj', '-c', 'Release', '-p:CloudEdgeIncludeAgentsPi=true']);
  command(['build', 'tests/AgentsPi/AgentsPi.fsproj', '-c', 'Release']);
  report.assertions.push('Shared Input compiles across both submit and prompt methods on PiHarness and PiSession');
  const negative = path.join(run, 'negative');
  mkdirSync(negative);
  for (const file of ['Directory.Build.props', 'Directory.Build.targets']) writeFileSync(path.join(negative, file), '<Project />');
  writeFileSync(path.join(negative, 'Bad.fs'), 'module Bad\nmodule Input = FSharp.CloudEdge.Pi.HarnessSubmit\nlet invalid : Input.InputItemsItemImage = { Text = "wrong payload"; TextSignature = None }\n');
  writeFileSync(path.join(negative, 'Bad.fsproj'), `<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include="Bad.fs" /><ProjectReference Include="${path.join(root, target.outputDirectory, target.generator.module + '.fsproj')}" /><PackageReference Include="Fable.Core" Version="5.2.0" /></ItemGroup></Project>`);
  const negativeLog = command(['build', 'Bad.fsproj', '-c', 'Release'], negative, 1);
  assert.match(negativeLog, /FS1129|FS0764/);
  report.assertions.push('Image constructor rejects text payload fields at F# compile time');
  command(['tool', 'restore'], directory);
  command(['fable', 'AgentsPi.fsproj', '--outDir', path.join(run, 'fable'), '--noCache'], directory);
  mkdirSync(path.join(run, 'bundle'));
  const bundle = path.join(run, 'bundle/worker.mjs');
  await build({ entryPoints: [path.join(directory, 'worker.mjs')], outfile: bundle, bundle: true, format: 'esm', platform: 'node', target: 'es2022', banner: { js: 'import { createRequire } from "node:module"; const require = createRequire("/worker.mjs");' }, conditions: ['workerd', 'worker', 'browser'], mainFields: ['module', 'main'], external: ['cloudflare:*', 'node:*'], nodePaths: [path.join(root, 'profiles/agents-pi/node_modules')], alias: { 'cloudedge-pi-client': path.join(run, 'fable/Client.js') }, logLevel: 'warning' });
  report.bundleSha256 = hash(bundle);
  writeFileSync(path.join(run, 'wrangler.json'), JSON.stringify({ name: 'cloudedge-pi-test', main: bundle, no_bundle: true, compatibility_date: '2026-09-01', compatibility_flags: ['nodejs_compat'], durable_objects: { bindings: [{ name: 'PiProbe', class_name: 'PiProbe' }] }, migrations: [{ tag: 'v1', new_sqlite_classes: ['PiProbe'] }] }));
  await start();
  const result = await request('');
  writeFileSync(path.join(run, 'response.json'), JSON.stringify(result, null, 2));
  assert.equal(result.text.accepted, true); assert.equal(result.wire.plain, 'plain text');
  assert.equal(result.wire.nullRejected, true);
  report.assertions.push('Ordinary F# null strings are rejected before invoking the SDK');
  report.assertions.push('F# generated PiHarness constructor and typed factory context enter the actual lifecycle');
  assert.match(result.context, /Context/);
  assert.deepEqual(result.wire.image, [{ text: 'visual question', type: 'text' }, { data: 'aGVsbG8=', mimeType: 'image/png', type: 'image' }]);
  report.assertions.push('Generated closed text/image payloads encode SDK discriminator tags and fields');
  assert.equal(result.wire.omittedSignature, true); assert.equal(result.wire.ownUndefinedSignature, true);
  report.assertions.push('Omitted optional property and explicitly present undefined stay distinct at the SDK boundary');
  assert.equal(result.receipt.accepted, true); assert.equal(result.duplicate.accepted, false);
  assert.equal(result.sessionReceipt.accepted, true);
  report.assertions.push('Both typed submit operations persist input and preserve idempotent receipts');
  for (const response of [result.response, result.sessionResponse]) { assert.equal(response.status, 'done'); assert.equal(response.text, 'fixture reply'); }
  report.assertions.push('Both typed prompt operations run actual Pi model tasks and return typed terminal responses');
  const messages = result.messages.filter(entry => entry.kind === 'pi.user');
  assert.equal(messages.length, 6);
  assert.deepEqual(messages[1].model[0].content, result.wire.image);
  report.assertions.push('SQLite transcript contains the encoded mixed text/image input');
  await stop(); await start();
  const restarted = await request('?action=inspect');
  assert.equal(restarted.duplicate.accepted, false);
  assert.equal(restarted.messages.filter(entry => entry.kind === 'pi.user').length, 6);
  report.assertions.push('Durable transcript and idempotency survive a workerd restart');
  assert.deepEqual(inputs(), before, 'Gate inputs changed during execution');
  report.passed = true;
} catch (error) { report.passed = false; report.error = error.stack; process.exitCode = 1; }
finally {
  try { await stop(); report.runtimeStopped = true; } catch (error) { report.cleanupError = error.message; process.exitCode = 1; }
  rmSync(path.join(run, 'state'), { recursive: true, force: true });
  report.stateRemoved = !existsSync(path.join(run, 'state'));
  report.passed = report.passed && report.runtimeStopped && report.stateRemoved;
  writeFileSync(path.join(run, 'report.json'), JSON.stringify(report, null, 2) + '\n');
  console.log(JSON.stringify(report, null, 2));
  console.log('Evidence: ' + run);
}
