import { spawn } from 'node:child_process';

// The Windows owner assigns this waiting process to its Job Object before
// releasing the structured command. No requested process starts outside it.
let payload = '';
process.stdin.setEncoding('utf8');
for await (const chunk of process.stdin) payload += chunk;
const { executable, args } = JSON.parse(payload);
const child = spawn(executable, args, { stdio: ['ignore', 'pipe', 'pipe'], windowsHide: true });
child.stdout.pipe(process.stdout, { end: false });
child.stderr.pipe(process.stderr, { end: false });
child.on('error', error => { console.error(error.message); process.exitCode = 1; });
child.on('close', (code, signal) => { process.exitCode = code ?? (signal ? 1 : 0); });
