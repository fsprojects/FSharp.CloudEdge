import childProcess from "node:child_process";
import { syncBuiltinESMExports } from "node:module";

// Run an existing JavaScript process double through Node on every platform.
// Only its exact command is redirected; argv, environment, stdio and exit status
// still cross the real process boundary used by the production caller.
export function executableScript(t, script) {
  const spawnSync = childProcess.spawnSync;
  const replacement = t.mock.method(childProcess, "spawnSync", (command, args, options) =>
    command === script
      ? spawnSync(process.execPath, [script, ...args], options)
      : spawnSync(command, args, options));
  syncBuiltinESMExports();
  t.after(() => { replacement.mock.restore(); syncBuiltinESMExports(); });
  return script;
}
