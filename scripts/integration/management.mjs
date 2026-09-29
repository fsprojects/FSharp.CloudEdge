import assert from 'node:assert/strict';
import { createHash } from 'node:crypto';
import { spawn } from 'node:child_process';
import { once } from 'node:events';
import { createServer } from 'node:http';
import { mkdir, mkdtemp, open, readFile, readdir, writeFile } from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { currentHawaii } from '../build-library.mjs';

const ROOT = fileURLToPath(new URL('../../', import.meta.url));
const sha256 = data => createHash('sha256').update(data).digest('hex');
const loadJson = async file => JSON.parse(await readFile(file, 'utf8'));

async function command(executable, args, cwd, logPath, timeoutMs = 300_000) {
  const log = await open(logPath, 'w');
  try {
    const child = spawn(executable, args, { cwd, stdio: ['ignore', log.fd, log.fd], detached: true });
    let timedOut = false;
    const deadline = setTimeout(() => {
      timedOut = true;
      try { process.kill(-child.pid, 'SIGKILL'); } catch { /* child already exited */ }
    }, timeoutMs);
    try {
      const [code, signal] = await once(child, 'exit');
      assert.equal(code, 0, `${executable} ${args[0]} failed (${timedOut ? 'timeout' : signal ?? code}); see ${logPath}`);
    } finally { clearTimeout(deadline); }
  } finally { await log.close(); }
}

async function pinnedSchema(root, override, expectedHash) {
  if (override) {
    const bytes = await readFile(path.resolve(override));
    assert.equal(sha256(bytes), expectedHash, 'Provided OpenAPI does not match Hawaii pins');
    return { document: JSON.parse(bytes), file: path.resolve(override) };
  }
  const runs = path.join(root, 'artifacts/hawaii/runs');
  for (const entry of (await readdir(runs, { withFileTypes: true })).reverse()) {
    if (!entry.isDirectory()) continue;
    const file = path.join(runs, entry.name, 'schema-original.json');
    try {
      const bytes = await readFile(file);
      if (sha256(bytes) === expectedHash) return { document: JSON.parse(bytes), file };
    } catch (error) { if (error.code !== 'ENOENT') throw error; }
  }
  throw new Error('No authenticated original OpenAPI found. Supply --schema <pristine pinned schema>.');
}

/** Diagnostic HTTP fixtures. This is never real Cloudflare deployment acceptance. */
export async function runManagement({ root = ROOT, runDirectory, schema } = {}) {
  root = path.resolve(root);
  const fixturePath = path.join(root, 'tests/ManagementIntegration/fixtures.json');
  const fixtures = await loadJson(fixturePath);
  const pins = await loadJson(path.join(root, 'generators/hawaii/pins.json'));
  const packages = await loadJson(path.join(root, 'config/tool-packages.json'));
  const accepted = currentHawaii(root, packages.tools[pins.hawaii.tool]);
  const provenancePath = path.join(root, accepted.run, 'provenance.json');
  assert.equal(fixtures.schemaSha256, pins.schema.sha256, 'Pinned OpenAPI changed: review and revalidate management fixtures before updating their schemaSha256');
  const original = await pinnedSchema(root, schema, pins.schema.sha256);
  const ids = new Set();
  for (const fixture of fixtures.cases) {
    assert.ok(!ids.has(fixture.id), `Duplicate fixture ${fixture.id}`);
    ids.add(fixture.id);
    const operation = original.document.paths[fixture.pathTemplate]?.[fixture.method.toLowerCase()];
    assert.equal(operation?.operationId, fixture.operationId, `Pinned operation moved or disappeared for ${fixture.id}`);
    const status = String(fixture.responseStatus);
    if (!fixture.negativeFixture) assert.ok(operation.responses[status] ?? operation.responses[`${status[0]}XX`], `Fixture status is undeclared for ${fixture.id}`);
  }
  const base = path.join(root, 'artifacts/integration');
  await mkdir(base, { recursive: true });
  const run = runDirectory ? path.resolve(runDirectory) : await mkdtemp(path.join(base, 'management-'));
  if (runDirectory) await mkdir(run, { recursive: false });
  const startedAt = new Date().toISOString();
  const project = path.join(root, 'tests/ManagementIntegration/ManagementIntegration.fsproj');
  await command('python3', [path.join(root, 'tests/ManagementIntegration/validate_fixtures.py'),
    '--schema', original.file, '--fixtures', fixturePath, '--report', path.join(run, 'fixture-schema.json'),
    '--pins', path.join(root, 'generators/hawaii/pins.json'), '--provenance', provenancePath], root, path.join(run, 'fixture-schema.log'));
  // Cold compilation of the full shared response model approaches five minutes.
  // Keep a separate build bound without extending the HTTP client's ten-second bound.
  await command('dotnet', ['build', project, '--configuration', 'Release', '--nologo'], root, path.join(run, 'build.log'), 900_000);

  const requests = [];
  const transportFailures = [];
  const seen = new Set();
  const byId = new Map(fixtures.cases.map(f => [f.id, f]));
  const sockets = new Set();
  const server = createServer(async (request, response) => {
    const id = request.headers['x-integration-case'];
    try {
      const fixture = byId.get(id);
      assert.ok(fixture, `Unknown fixture ID ${id}`);
      assert.ok(!seen.has(id), `Unexpected retry or duplicate request ${id}`);
      seen.add(id);
      const chunks = [];
      let size = 0;
      for await (const chunk of request) {
        size += chunk.length;
        assert.ok(size <= 64 * 1024, 'Management fixture exceeded body bound');
        chunks.push(chunk);
      }
      const body = Buffer.concat(chunks).toString('utf8');
      const record = { id, method: request.method, path: request.url, body: body ? JSON.parse(body) : null };
      requests.push(record);
      assert.equal(request.method, fixture.method, `${id} method`);
      assert.equal(request.url, fixture.requestPath, `${id} path/query encoding`);
      assert.equal(request.headers.authorization, 'Bearer fixture-only-token', `${id} authorization`);
      assert.deepEqual(record.body, fixture.requestBody ?? null, `${id} request body`);
      if (fixture.requestBody) assert.match(request.headers['content-type'], /^application\/json\b/, `${id} media type`);
      if (fixture.holdResponse) return; // The client must cancel this outstanding request.
      const payload = fixture.responseRaw ?? JSON.stringify(fixture.responseBody);
      response.writeHead(fixture.responseStatus, { ...fixture.responseHeaders, 'content-type': 'application/json', 'content-length': Buffer.byteLength(payload) });
      response.end(payload);
    } catch (error) {
      transportFailures.push({ id: id ?? null, detail: error.message });
      response.writeHead(500, { 'content-type': 'application/json' });
      response.end(JSON.stringify({ error: 'Management fixture request contract failed' }));
    }
  });
  server.on('connection', socket => { sockets.add(socket); socket.on('close', () => sockets.delete(socket)); });
  server.listen(0, '127.0.0.1');
  await once(server, 'listening');
  let processFailure;
  try {
    await command('dotnet', [path.join(root, 'tests/ManagementIntegration/bin/Release/net10.0/ManagementIntegration.dll'),
      `http://127.0.0.1:${server.address().port}`, fixturePath, path.join(run, 'observations.json')], root, path.join(run, 'runtime.log'));
  } catch (error) { processFailure = error.message; }
  finally {
    for (const socket of sockets) socket.destroy();
    await new Promise(resolve => server.close(resolve));
    await writeFile(path.join(run, 'requests.json'), JSON.stringify(requests, null, 2) + '\n');
  }
  let observations = [];
  try { observations = await loadJson(path.join(run, 'observations.json')); }
  catch (error) { if (error.code !== 'ENOENT') throw error; }
  const results = fixtures.cases.map(fixture => {
    const observation = observations.find(item => item.id === fixture.id);
    const transport = transportFailures.filter(item => item.id === fixture.id);
    const passed = observation?.status === 'passed' && seen.has(fixture.id) && transport.length === 0;
    return { id: fixture.id, operationId: fixture.operationId, status: passed ? 'passed' : 'failed',
      ...(fixture.knownGap ? { knownGap: fixture.knownGap } : {}),
      ...(fixture.regression ? { regression: fixture.regression } : {}),
      ...(fixture.schemaCorrection ? { schemaCorrection: fixture.schemaCorrection } : {}),
      detail: observation?.detail ?? 'Client did not report this case', transportFailures: transport,
      requestObserved: seen.has(fixture.id),
      schemaPointer: `/paths/${fixture.pathTemplate.replaceAll('~', '~0').replaceAll('/', '~1')}/${fixture.method.toLowerCase()}/responses` };
  });
  const files = ['tests/ManagementIntegration/Program.fs', 'tests/ManagementIntegration/ManagementIntegration.fsproj',
    'tests/ManagementIntegration/fixtures.json', 'tests/ManagementIntegration/validate_fixtures.py',
    'tests/ManagementIntegration/requirements.txt', 'scripts/integration/management.mjs',
    'generators/hawaii/pins.json', 'inventory/hawaii-output-ownership.json', 'config/tool-packages.json',
    'scripts/build-library.mjs', ...pins.pipelineInputs,
    ...pins.overlays.map(file => `generators/hawaii/${file}`),
    `${accepted.run}/provenance.json`, 'src/Core/FSharp.CloudEdge.Core.Api/Types.fs',
    'src/Core/FSharp.CloudEdge.Core.Api/OpenApiHttp.fs',
    ...['Compute', 'Networking', 'Security'].flatMap(purpose => [
      `src/Management/FSharp.CloudEdge.Management.${purpose}/Client.fs`,
      `tests/ManagementIntegration/bin/Release/net10.0/FSharp.CloudEdge.Management.${purpose}.dll`]),
    'tests/ManagementIntegration/bin/Release/net10.0/FSharp.CloudEdge.Core.Api.dll',
    'tests/ManagementIntegration/bin/Release/net10.0/ManagementIntegration.dll'];
  const sourceHashes = Object.fromEntries(await Promise.all(files.map(async file => [file, sha256(await readFile(path.join(root, file)))])));
  const failed = results.filter(r => r.status === 'failed').length;
  const report = { schemaVersion: 1, suite: 'management', status: failed || processFailure || transportFailures.length ? 'failed' : 'passed',
    profile: 'local-loopback', liveCloudflare: false, startedAt, completedAt: new Date().toISOString(),
    schema: { commit: pins.schema.commit, sha256: pins.schema.sha256, file: original.file, acceptedGeneration: accepted.run,
      validationReport: 'fixture-schema.json' }, sourceHashes,
    counts: { total: results.length, passed: results.length - failed, failed, requests: requests.length },
    results, transportFailures, ...(processFailure ? { processFailure } : {}),
    acceptanceBoundary: 'Generated HTTP client diagnostics only. No Cloudflare resource, Access enforcement, WARP tunnel, native container, or network propagation was exercised.' };
  await writeFile(path.join(run, 'report.json'), JSON.stringify(report, null, 2) + '\n');
  return { ...report, runDirectory: run };
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  const options = {};
  for (let index = 2; index < process.argv.length; index += 2) {
    const key = { '--run-directory': 'runDirectory', '--schema': 'schema', '--root': 'root' }[process.argv[index]];
    assert.ok(key && process.argv[index + 1], 'Usage: node scripts/integration/management.mjs [--run-directory NEW_DIRECTORY] [--schema ORIGINAL_OPENAPI] [--root BINDINGS_ROOT]');
    options[key] = process.argv[index + 1];
  }
  const report = await runManagement(options);
  console.log(`Management diagnostics: ${report.counts.passed}/${report.counts.total} passed, ${report.counts.failed} failed. ${report.runDirectory}/report.json`);
  process.exitCode = report.status === 'passed' ? 0 : 1;
}
