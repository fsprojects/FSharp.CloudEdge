import assert from 'node:assert/strict';
import { createHash } from 'node:crypto';
import { spawnSync } from 'node:child_process';
import { spawnOwned, stopOwned } from '../../scripts/integration/process-lifetime.mjs';
import { createRequire } from 'node:module';
import { createServer } from 'node:net';
import { accessSync, constants, existsSync, mkdirSync, mkdtempSync, readFileSync, readdirSync, realpathSync, renameSync, rmSync, writeFileSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { planProjects, checkProjects } from '../../scripts/projects.mjs';
import { packageCache } from '../../scripts/tool-packages.mjs';
import { unzipSync } from 'fflate';

const directory = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(directory, '../..');
// A failed or interrupted rerun must not leave an earlier pass available for release.
const acceptanceReceipt = path.join(root, 'inventory/pi-lifecycle-acceptance.json');
rmSync(acceptanceReceipt, { force: true });
const require = createRequire(path.join(root, 'profiles/agents-pi/package.json'));
const { build } = require('esbuild');
const hash = file => createHash('sha256').update(readFileSync(file)).digest('hex');
const localSupport = JSON.parse(readFileSync(path.join(root, 'config/targets.json'), 'utf8')).projectDefaults.packages;
const supportArchives = ['Xantham.Fable.Core', 'Xantham.Fable.Core.TS'].map(id => {
  const version = localSupport[id];
  const local = path.join(root, 'artifacts/tool-feed', `${id}.${version}.nupkg`);
  const cached = path.join(packageCache(process.env, root), id.toLowerCase(), version.toLowerCase(), `${id.toLowerCase()}.${version.toLowerCase()}.nupkg`);
  assert.equal(hash(local), hash(cached), `${id} restored package differs from the local gate package`);
  return { id, local, cached, sha256: hash(local) };
});
const supportFableSources = Object.fromEntries(supportArchives.map(({ id, local }) => {
  const entries = unzipSync(readFileSync(local), { filter: entry => entry.name.startsWith('fable/') && entry.name.endsWith('.fs') });
  const sources = Object.fromEntries(Object.entries(entries).map(([name, bytes]) => [name, createHash('sha256').update(bytes).digest('hex')]));
  assert.ok(Object.keys(sources).length, `${id} has no Fable sources`);
  return [id, sources];
}));
const xanthamRoot = path.resolve(process.env.CLOUDEDGE_XANTHAM_ROOT ?? path.join(root, '../Xantham'));
const git = args => {
  const result = spawnSync('git', ['-C', xanthamRoot, ...args], { encoding: 'utf8', timeout: 10000 });
  assert.equal(result.status, 0, result.stderr);
  return result.stdout.trim();
};
const xanthamRevision = git(['rev-parse', 'HEAD']);
assert.equal(git(['status', '--porcelain']), '', 'Pi gate requires clean Xantham sources');
const celldCommand = process.env.CLOUDEDGE_CELLD ?? 'celld';
const celldCandidates = celldCommand.includes(path.sep)
  ? [path.resolve(root, celldCommand)]
  : (process.env.PATH ?? '').split(path.delimiter).map(directory => path.resolve(directory, celldCommand));
const celld = celldCandidates.find(file => { try { accessSync(file, constants.X_OK); return true; } catch { return false; } });
assert.ok(celld, 'celld executable is unavailable');
const celldVersion = spawnSync(celld, ['--version'], { encoding: 'utf8', timeout: 10000 });
assert.equal(celldVersion.status, 0, celldVersion.stderr);
const runtime = { executable: realpathSync(celld), sha256: hash(celld), version: celldVersion.stdout.trim() };
const configuration = JSON.parse(readFileSync(path.join(root, 'artifacts/sdk-build/targets.json'), 'utf8'));
const plan = planProjects(configuration, root);
plan.solution = path.join(root, 'SDK.Partitions.slnx');
const checked = checkProjects(plan, ['agents-pi']);
const target = configuration.targets.find(target => target.id === 'agents-pi');
assert.ok(target?.projection, 'Generate the strict agents-pi delivery first');
const selectedIds = new Set(checked.map(project => project.target));
const selectedProjects = plan.projects.filter(project => selectedIds.has(project.id));
const selectedProfiles = new Set(checked.map(project => project.profile));
const inputFiles = [
  'tests/AgentsPi/Client.fs', 'tests/AgentsPi/check.mjs', 'tests/AgentsPi/AgentsPi.fsproj',
  'tests/AgentsPi/.config/dotnet-tools.json', 'tests/SDKComposition/SDKComposition.fsproj', 'tests/SDKComposition/Pi.fs',
  'src/Support/FSharp.CloudEdge.Support.Workers/DurableObjects.fs',
  'src/Support/FSharp.CloudEdge.Support.Workers/FSharp.CloudEdge.Support.Workers.fsproj',
  'artifacts/sdk-build/targets.json', 'Directory.Build.props', '.config/dotnet-tools.json', 'config/tool-packages.json',
  'config/sdk-delivery.json', 'config/targets.json', 'config/nuget-release.json',
  ...selectedProjects.flatMap(project => [project.file, ...project.sources].map(file => path.relative(root, file))),
  ...checked.map(project => `artifacts/${project.target}/generation.json`),
  ...configuration.profiles.filter(profile => selectedProfiles.has(profile.id)).flatMap(profile => [`${profile.directory}/package.json`, `${profile.directory}/package-lock.json`]),
  ...supportArchives.map(archive => path.relative(root, archive.local)),
];
const inputs = () => Object.fromEntries(inputFiles.map(file => [file, hash(path.join(root, file))]));
inputFiles.push('scripts/integration/process-lifetime.mjs', 'scripts/integration/step-job.ps1', 'scripts/integration/step-process.mjs');
const before = inputs();
const scratch = path.join(root, 'tests/.scratch/agents-pi');
mkdirSync(scratch, { recursive: true });
const run = mkdtempSync(path.join(scratch, 'run-'));
const stateRoot = path.join(run, '.celld');
const durableState = path.join(stateRoot, 'dev/runtime');
const sqliteFiles = directory => readdirSync(directory, { withFileTypes: true }).flatMap(entry => {
  const file = path.join(directory, entry.name);
  if (entry.isDirectory()) return sqliteFiles(file);
  if (!entry.isFile() || entry.name === 'objects.sqlite3') return [];
  const bytes = readFileSync(file);
  if (bytes.length < 100 || !bytes.subarray(0, 16).equals(Buffer.from('SQLite format 3\0'))) return [];
  return [{ file: path.relative(run, file), bytes: bytes.length, sha256: createHash('sha256').update(bytes).digest('hex') }];
});
const report = { qualification: 'Actual Agents PiHarness and pi-durable SQLite cell on celld, SDK fauxProvider model; restart after completed operations', xanthamRevision, xanthamClean: true, supportFableSources, runtime, checked, inputSha256: before, assertions: [] };
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
  const listener = createServer();
  await new Promise((resolve, reject) => { listener.once('error', reject); listener.listen(0, '127.0.0.1', resolve); });
  const port = listener.address().port;
  await new Promise(resolve => listener.close(resolve));
  url = `http://127.0.0.1:${port}`;
  child = spawnOwned(celld, ['dev', run, '--host', '127.0.0.1', '--port', String(port), '--no-watch'], { cwd: run, stdio: ['ignore', 'pipe', 'pipe'], env: { ...process.env, CELLD_OTEL: '0', NO_COLOR: '1' } });
  let launchError;
  child.once('error', error => { launchError = error; });
  const requireRunning = () => {
    if (launchError) throw launchError;
    if (child.exitCode !== null || child.signalCode !== null) throw Error('Local celld exited during startup: ' + output);
  };
  for (const stream of [child.stdout, child.stderr]) stream.on('data', chunk => { output += chunk; });
  const deadline = Date.now() + 30000;
  while (Date.now() < deadline) {
    requireRunning();
    let ready = false;
    try {
      // The existing F# worker's fallback route is side-effect free.
      const response = await fetch(url + '/__cloudedge_pi_readiness', { redirect: 'error', signal: AbortSignal.timeout(Math.min(500, Math.max(1, deadline - Date.now()))) });
      const body = await response.json();
      ready = response.status === 404 && body?.error === 'missing' && Object.keys(body).length === 1;
    } catch { /* The worker may not be serving yet. */ }
    requireRunning();
    if (ready) return;
    await new Promise(resolve => setTimeout(resolve, 100));
  }
  throw Error('Local celld startup timed out: ' + output);
};
const stop = async () => {
  try { await stopOwned(child, 10000); }
  finally { writeFileSync(path.join(run, 'worker.log'), output); }
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
  command(['build', 'tests/AgentsPi/AgentsPi.fsproj', '-c', 'Release']);
  report.assertions.push('Existing Pi composition witness and F# cell host compile against the selected generated dependency closure');
  const negative = path.join(run, 'negative');
  mkdirSync(negative);
  for (const file of ['Directory.Build.props', 'Directory.Build.targets']) writeFileSync(path.join(negative, file), '<Project />');
  writeFileSync(path.join(negative, 'Bad.fs'), 'module Bad\nmodule Input = FSharp.CloudEdge.Pi.HarnessSubmit\nlet invalid : Input.InputItemsItemImage = { Text = "wrong payload"; TextSignature = None }\n');
  writeFileSync(path.join(negative, 'Bad.fsproj'), `<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include="Bad.fs" /><ProjectReference Include="${path.join(root, target.outputDirectory, target.generator.module + '.fsproj')}" /><PackageReference Include="Fable.Core" Version="5.6.0" /></ItemGroup></Project>`);
  const negativeLog = command(['build', 'Bad.fsproj', '-c', 'Release'], negative, 1);
  assert.match(negativeLog, /FS1129|FS0764/);
  report.assertions.push('Image constructor rejects text payload fields at F# compile time');
  command(['tool', 'restore'], directory);
  command(['fickl', 'AgentsPi.fsproj', '--outDir', path.join(run, 'fable'), '--noCache'], directory);
  mkdirSync(path.join(run, 'bundle'));
  const bundle = path.join(run, 'bundle/worker.mjs');
  await build({ entryPoints: [path.join(run, 'fable/Client.js')], outfile: bundle, bundle: true, format: 'esm', platform: 'node', target: 'es2022', conditions: ['workerd', 'worker', 'browser'], mainFields: ['module', 'main'], external: ['cloudflare:*', 'node:*'], nodePaths: [path.join(root, 'profiles/agents-pi/node_modules')], logLevel: 'warning' });
  report.bundleSha256 = hash(bundle);
  writeFileSync(path.join(run, 'wrangler.json'), JSON.stringify({ name: 'cloudedge-pi-test', main: 'bundle/worker.mjs', no_bundle: true, compatibility_date: '2026-09-01', compatibility_flags: ['nodejs_compat'], durable_objects: { bindings: [{ name: 'PiProbe', class_name: 'PiProbe' }] }, migrations: [{ tag: 'v1', new_sqlite_classes: ['PiProbe'] }] }));
  await start();
  const result = await request('');
  writeFileSync(path.join(run, 'response.json'), JSON.stringify(result, null, 2));
  assert.equal(result.text.accepted, true); assert.equal(result.wire.plain, 'plain text');
  assert.equal(result.wire.nullRejected, true);
  assert.match(result.context, /Context/);
  report.assertions.push('Ordinary F# null strings are rejected before invoking the SDK');
  report.assertions.push('F# generated PiHarness constructor and typed factory context enter the actual lifecycle');
  assert.deepEqual(result.wire.image, [{ text: 'visual question', type: 'text' }, { data: 'aGVsbG8=', mimeType: 'image/png', type: 'image' }]);
  report.assertions.push('Generated closed text/image payloads encode SDK discriminator tags and fields');
  assert.equal(result.wire.omittedSignature, true); assert.equal(result.wire.ownUndefinedSignature, true);
  report.assertions.push('Omitted optional property and explicitly present undefined stay distinct at the SDK boundary');
  assert.equal(result.receipt.accepted, true); assert.equal(result.duplicate.accepted, false);
  assert.equal(result.duplicate.operationId, result.receipt.operationId);
  assert.equal(result.sessionReceipt.accepted, true);
  assert.equal(result.sessionDuplicate.accepted, false);
  assert.equal(result.sessionDuplicate.operationId, result.sessionReceipt.operationId);
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
  assert.equal(restarted.duplicate.operationId, result.receipt.operationId);
  assert.equal(restarted.sessionDuplicate.accepted, false);
  assert.equal(restarted.sessionDuplicate.operationId, result.sessionReceipt.operationId);
  const restartedMessages = restarted.messages.filter(entry => entry.kind === 'pi.user');
  assert.equal(restartedMessages.length, 6);
  assert.deepEqual(restartedMessages, messages);
  report.assertions.push('Durable transcript and idempotency survive a celld restart');
  assert.deepEqual(inputs(), before, 'Gate inputs changed during execution');
  assert.equal(hash(celld), runtime.sha256, 'celld executable changed during execution');
  for (const archive of supportArchives) assert.equal(hash(archive.cached), archive.sha256, `${archive.id} cached package changed during execution`);
  assert.equal(git(['rev-parse', 'HEAD']), xanthamRevision, 'Xantham revision changed during execution');
  assert.equal(git(['status', '--porcelain']), '', 'Xantham sources changed during execution');
  report.passed = true;
} catch (error) { report.passed = false; report.error = error.stack; process.exitCode = 1; }
finally {
  try { await stop(); report.runtimeStopped = true; } catch (error) { report.cleanupError = error.message; process.exitCode = 1; }
  if (report.runtimeStopped) {
    const stateExisted = existsSync(stateRoot);
    try {
      // celld dev keeps its bucket in objects.sqlite3; require cell SQLite in runtime instead.
      assert.ok(existsSync(durableState), 'celld Durable Object state directory was not created');
      report.durableSqlite = sqliteFiles(durableState);
      assert.ok(report.durableSqlite.length > 0, 'No Durable Object SQLite database found under .celld/dev/runtime');
      report.durableStateObserved = true;
      report.assertions.push('Actual celld Durable Object SQLite files were observed and hashed after stopping the runtime');
    } catch (error) { report.stateError = error.message; process.exitCode = 1; }
    try {
      rmSync(stateRoot, { recursive: true, force: true });
      assert.equal(existsSync(stateRoot), false, 'celld state remained after cleanup');
      report.stateRemoved = stateExisted;
      if (report.durableStateObserved && report.stateRemoved) report.assertions.push('The observed celld Durable Object SQLite state was removed');
    } catch (error) { report.cleanupError = error.message; process.exitCode = 1; }
  }
  report.passed = Boolean(report.passed && report.runtimeStopped && report.durableStateObserved && report.stateRemoved);
  writeFileSync(path.join(run, 'report.json'), JSON.stringify(report, null, 2) + '\n');
  if (report.passed) {
    const candidate = path.join(run, 'pi-lifecycle-acceptance.tmp');
    writeFileSync(candidate, JSON.stringify(report, null, 2) + '\n');
    renameSync(candidate, acceptanceReceipt);
  }
  console.log(JSON.stringify(report, null, 2));
  console.log('Evidence: ' + run);
}
