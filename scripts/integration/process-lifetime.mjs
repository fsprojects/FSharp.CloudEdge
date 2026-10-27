import { spawn } from 'node:child_process';
import { fileURLToPath } from 'node:url';

const completions = new WeakMap();

/** Spawn with a Windows kill-on-close Job Object or a POSIX process group. */
export function spawnOwned(executable, args, options = {}) {
  const windows = process.platform === 'win32';
  const command = windows ? 'powershell.exe' : executable;
  const arguments_ = windows
    ? ['-NoProfile', '-NonInteractive', '-ExecutionPolicy', 'Bypass', '-File', fileURLToPath(new URL('./step-job.ps1', import.meta.url)),
      '-Node', process.execPath, '-Wrapper', fileURLToPath(new URL('./step-process.mjs', import.meta.url)),
      '-Payload', Buffer.from(JSON.stringify({ executable, args })).toString('base64')] : args;
  const child = spawn(command, arguments_, { ...options, detached: !windows, windowsHide: true });
  const lifetime = { closed: false };
  lifetime.completion = new Promise(resolve => child.once('close', (code, signal) => {
    lifetime.closed = true;
    resolve({ code, signal });
  }));
  completions.set(child, lifetime);
  return child;
}

/** Stop only this launcher's owned processes, then wait for captured pipes to close. */
export async function stopOwned(child, graceMs = 5000) {
  if (!child) return;
  const lifetime = completions.get(child);
  if (!lifetime) throw Error('Process was not started by the owned launcher');
  if (lifetime.closed) return;
  let timer;
  let escalation = new Promise(() => {});
  if (process.platform === 'win32') {
    // ChildProcess.kill uses the retained OS handle, avoiding PID reuse. The
    // helper's exit closes its Job Object even if the actual command exited first.
    if (child.exitCode === null && child.signalCode === null) child.kill('SIGKILL');
  } else if (child.pid) {
    const kill = signal => { try { process.kill(-child.pid, signal); } catch (error) { if (error.code !== 'ESRCH') throw error; } };
    kill('SIGTERM');
    escalation = new Promise((_, reject) => {
      timer = setTimeout(() => { try { kill('SIGKILL'); } catch (error) { reject(error); } }, graceMs);
    });
  }
  try { await Promise.race([lifetime.completion, escalation]); } finally { clearTimeout(timer); }
}
