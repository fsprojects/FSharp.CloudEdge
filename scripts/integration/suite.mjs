import { spawn } from 'node:child_process';
import { createHash } from 'node:crypto';
import { createWriteStream, existsSync, readFileSync, mkdirSync, writeFileSync, realpathSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { parseArgs } from 'node:util';

export const sdkRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../..');
const hash = file => createHash('sha256').update(readFileSync(file)).digest('hex');
export function validateManifest(manifest) {
  if (manifest.schemaVersion !== 1 || !Array.isArray(manifest.cases) || !manifest.cases.length) throw Error('Expected a nonempty schema-1 suite');
  const ids = new Set();
  for (const entry of manifest.cases) {
    if (!/^[a-z][a-z0-9.-]+$/.test(entry.id) || ids.has(entry.id)) throw Error(`Invalid or duplicate case: ${entry.id}`);
    ids.add(entry.id);
    if (!entry.contract || !entry.owner || !entry.profiles?.length || !['implemented', 'planned'].includes(entry.implementation)) throw Error(`Incomplete contract: ${entry.id}`);
    if (entry.implementation === 'planned' ? !entry.reason : !entry.steps?.length) throw Error(`Missing execution or reason: ${entry.id}`);
    for (const step of entry.steps ?? []) {
      if (!['sdk', 'validation', 'conclave'].includes(step.root) || !['node', 'npm', 'dotnet', 'python3'].includes(step.command)
        || !Array.isArray(step.args) || step.args.some(arg => typeof arg !== 'string')) throw Error(`Invalid command: ${entry.id}`);
    }
  }
  const visit = (id, stack = []) => {
    if (stack.includes(id)) throw Error(`Dependency cycle: ${[...stack, id].join(' -> ')}`);
    const entry = manifest.cases.find(entry => entry.id === id);
    if (!entry) throw Error(`Unknown dependency: ${id}`);
    for (const dependency of entry.dependsOn ?? []) visit(dependency, [...stack, id]);
  };
  for (const id of ids) visit(id);
  return manifest;
}
export function selectCases(manifest, profile, only = []) {
  validateManifest(manifest);
  if (!manifest.profiles.includes(profile)) throw Error(`Unknown profile: ${profile}`);
  const selected = new Map();
  const visit = id => {
    const entry = manifest.cases.find(entry => entry.id === id);
    if (!entry) throw Error(`Unknown case: ${id}`);
    for (const dependency of entry.dependsOn ?? []) visit(dependency);
    selected.set(id, entry);
  };
  for (const entry of manifest.cases) if (!only.length && (profile === 'release' || entry.profiles.includes(profile))) visit(entry.id);
  for (const id of only) visit(id);
  return [...selected.values()];
}
export function summarize(results) {
  const counts = Object.fromEntries(['passed', 'failed', 'blocked', 'not-implemented'].map(status => [status, results.filter(row => row.status === status).length]));
  return { passed: results.length > 0 && results.every(row => row.status === 'passed'), counts };
}

export function junitReport(report) {
  const xml = value => String(value ?? '').replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('"', '&quot;').replaceAll("'", '&apos;');
  const failed = report.results.filter(row => row.status !== 'passed').length;
  const cases = report.results.map(row => {
    const seconds = Math.max(0, (Date.parse(row.finishedAt) - Date.parse(row.startedAt)) / 1000) || 0;
    const failure = row.status === 'passed' ? '' : `<failure type="${xml(row.status)}" message="${xml(row.reason ?? 'Contract did not pass')}">${xml(row.contract)}</failure>`;
    const evidence = row.steps.map(step => step.log).filter(Boolean).join('\n');
    return `  <testcase name="${xml(row.id)}" classname="${xml(row.owner)}" time="${seconds}">${failure}<system-out>${xml(evidence)}</system-out></testcase>`;
  });
  return `<?xml version="1.0" encoding="UTF-8"?>\n<testsuite name="CloudEdge.${xml(report.profile)}" tests="${report.results.length}" failures="${failed}" errors="0" skipped="0">\n${cases.join('\n')}\n</testsuite>\n`;
}

export function validateLibraryCoverage(manifest, delivery) {
  const selected = new Set(delivery.libraries.map(entry => entry.partition));
  const covered = new Set(manifest.cases.flatMap(entry => entry.libraries ?? []));
  const missing = [...selected].filter(id => !covered.has(id));
  const unknown = [...covered].filter(id => !selected.has(id));
  if (missing.length || unknown.length) throw Error(`Runtime contract ownership drift: missing=${missing.join(',')} unknown=${unknown.join(',')}`);
  return { selectedLibraries: selected.size, mappedLibraries: covered.size,
    qualification: 'Mapping assigns required hosted test families; a planned family is not passing coverage.' };
}

export async function executeStep(step, roots, log, timeoutMs = 1_800_000, signal) {
  const cwd = roots[step.root];
  if (!cwd || !existsSync(cwd)) return { status: 'blocked', reason: `Missing ${step.root} repository: ${cwd}` };
  return new Promise(resolve => {
    const output = createWriteStream(log);
    const executable = step.command === 'node' ? process.execPath : step.command === 'dotnet' ? (process.env.CLOUDEDGE_DOTNET ?? 'dotnet') : step.command;
    const child = spawn(executable, step.args, { cwd, detached: true, stdio: ['ignore', 'pipe', 'pipe'],
      env: { ...process.env, CloudEdgeRoot: roots.sdk, CLOUDEDGE_ROOT: roots.sdk, WRANGLER_SEND_METRICS: 'false', NO_COLOR: '1' } });
    child.stdout.pipe(output, { end: false }); child.stderr.pipe(output, { end: false });
    let error, killTimer, timedOut = false;
    const stop = () => {
      if (!child.pid || child.exitCode !== null || child.signalCode !== null) return;
      try { process.kill(-child.pid, 'SIGTERM'); } catch (e) { if (e.code !== 'ESRCH') error = e.message; }
      killTimer ??= setTimeout(() => { try { process.kill(-child.pid, 'SIGKILL'); } catch {} }, 5000);
    };
    const timer = setTimeout(() => { timedOut = true; stop(); }, timeoutMs);
    signal?.addEventListener('abort', stop, { once: true });
    if (signal?.aborted) stop();
    child.on('error', e => { error = e.message; });
    child.on('close', (code, terminationSignal) => {
      clearTimeout(timer); clearTimeout(killTimer); signal?.removeEventListener('abort', stop);
      output.end(() => resolve({ status: !error && !timedOut && !signal?.aborted && code === 0 ? 'passed' : 'failed',
        exitCode: code, terminationSignal, timedOut, ...(error ? { error } : {}), log, logSha256: hash(log) }));
    });
  });
}

async function main() {
  const { values } = parseArgs({ options: {
    profile: { type: 'string', default: 'cloudflare' }, case: { type: 'string', multiple: true },
    list: { type: 'boolean', default: false }, 'conclave-root': { type: 'string' }, 'validation-root': { type: 'string' },
    'manifest': { type: 'string' }, 'output': { type: 'string' },
  } });
  const manifestFile = path.resolve(values.manifest ?? path.join(sdkRoot, 'tests/integration/suite.json'));
  const manifest = validateManifest(JSON.parse(readFileSync(manifestFile, 'utf8')));
  if (!values.manifest) validateLibraryCoverage(manifest, JSON.parse(readFileSync(path.join(sdkRoot, 'config/sdk-delivery.json'), 'utf8')));
  const selected = selectCases(manifest, values.profile, values.case);
  if (values.list) { console.log(JSON.stringify(selected, null, 2)); return; }
  const roots = { sdk: sdkRoot, conclave: path.resolve(values['conclave-root'] ?? process.env.CLOUDEDGE_CONCLAVE_ROOT ?? path.join(sdkRoot, '../Conclave')),
    validation: path.resolve(values['validation-root'] ?? process.env.CLOUDEDGE_VALIDATION_ROOT ?? path.join(sdkRoot, '../FSharp.CloudEdge.Validation')) };
  const directory = path.resolve(values.output ?? path.join(sdkRoot, 'artifacts/integration', new Date().toISOString().replaceAll(/[:.]/g, '-') + '-' + process.pid));
  if (existsSync(directory)) throw Error(`Refusing to overwrite run evidence: ${directory}`);
  mkdirSync(directory, { recursive: true });
  const controller = new AbortController();
  const interrupt = () => controller.abort();
  process.once('SIGTERM', interrupt); process.once('SIGINT', interrupt);
  const results = [], report = { schemaVersion: 1, profile: values.profile, requestedCases: values.case ?? [],
    selectionKind: values.case?.length ? 'explicit-cases' : 'profile',
    startedAt: new Date().toISOString(), node: process.version, roots, manifestSha256: hash(manifestFile), runnerSha256: hash(fileURLToPath(import.meta.url)), results };
  try {
    for (const entry of selected) {
      const result = { id: entry.id, owner: entry.owner, contract: entry.contract, profiles: entry.profiles, startedAt: new Date().toISOString(), steps: [] };
      results.push(result);
      if (entry.implementation === 'planned') Object.assign(result, { status: 'not-implemented', reason: entry.reason });
      else if (controller.signal.aborted) Object.assign(result, { status: 'blocked', reason: 'Run interrupted' });
      else if ((entry.dependsOn ?? []).some(id => results.find(row => row.id === id)?.status !== 'passed'))
        Object.assign(result, { status: 'blocked', reason: 'A prerequisite did not pass' });
      else {
        console.log(`RUN ${entry.id}: ${entry.contract}`);
        for (const [index, step] of entry.steps.entries()) {
          const outcome = await executeStep(step, roots, path.join(directory, `${entry.id}.${index + 1}.log`), entry.timeoutMs, controller.signal);
          result.steps.push({ command: step.command, args: step.args, root: step.root, ...outcome });
          if (outcome.status !== 'passed') break;
        }
        result.status = result.steps.at(-1)?.status ?? 'failed';
      }
      result.finishedAt = new Date().toISOString();
      console.log(`${result.status.toUpperCase()} ${result.id}${result.reason ? ': ' + result.reason : ''}`);
      writeFileSync(path.join(directory, 'report.json'), JSON.stringify(report, null, 2) + '\n');
    }
  } finally {
    process.removeListener('SIGTERM', interrupt); process.removeListener('SIGINT', interrupt);
    Object.assign(report, summarize(results), { finishedAt: new Date().toISOString(), selectedCases: selected.length,
      coverageScope: 'Passing local cases establish only their named contracts. Release requires every catalogued case, including live, native, client and transport profiles.' });
    if (hash(manifestFile) !== report.manifestSha256) { report.passed = false; report.inputChanged = 'Suite manifest changed during the run'; }
    if (results.length !== selected.length) report.passed = false;
    report.profileComplete = report.passed && report.selectionKind === 'profile';
    writeFileSync(path.join(directory, 'report.json'), JSON.stringify(report, null, 2) + '\n');
    writeFileSync(path.join(directory, 'junit.xml'), junitReport(report));
    console.log(JSON.stringify({ passed: report.passed, counts: report.counts, evidence: path.join(directory, 'report.json') }, null, 2));
    if (!report.passed || results.length !== selected.length) process.exitCode = 1;
  }
}
if (process.argv[1] && realpathSync(process.argv[1]) === realpathSync(fileURLToPath(import.meta.url))) await main();
