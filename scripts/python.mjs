import { spawnSync } from 'node:child_process';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

/** Resolve an actual Python 3 interpreter, rejecting non-executing Store aliases. */
export function resolvePython({ env = process.env, root = process.cwd(), platform = process.platform, probe = spawnSync } = {}) {
  const candidates = env.CLOUDEDGE_PYTHON ? [[env.CLOUDEDGE_PYTHON, []]]
    : platform === 'win32' ? [['py', ['-3']], ['python', []], ['python3', []]] : [['python3', []], ['python', []]];
  for (const [command, prefix] of candidates) {
    const result = probe(command, [...prefix, '-c', 'import json,sys; print(json.dumps({"major":sys.version_info.major,"executable":sys.executable}))'],
      { cwd: root, env, encoding: 'utf8', timeout: 5000, windowsHide: true });
    if (result.error || result.status !== 0) continue;
    try {
      const identity = JSON.parse(result.stdout.trim());
      if (identity.major === 3 && typeof identity.executable === 'string' && path.isAbsolute(identity.executable)) return identity.executable;
    } catch { /* A Store alias or unrelated command is not interpreter evidence. */ }
  }
  throw Error('Python 3 is unavailable; set CLOUDEDGE_PYTHON to an installed interpreter executable');
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  try {
    const result = spawnSync(resolvePython(), process.argv.slice(2), { stdio: 'inherit', env: process.env, windowsHide: true });
    if (result.error) throw result.error;
    process.exitCode = result.status ?? 1;
  } catch (error) {
    console.error(error.message);
    process.exitCode = 1;
  }
}
