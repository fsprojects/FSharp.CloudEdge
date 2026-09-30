---
title: Computer workspaces
description: Keep a durable working directory while choosing how its commands and code execute.
---

<div class="ce-block-head">
<p class="ce-block-lead">Keep a durable working directory while choosing how its commands and code execute.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Computer</code> <code>Runtime.ComputerArtifacts</code></li>
<li><span>npm</span> <code>@cloudflare/computer 0.2.1</code></li>
<li><span>Upstream/API</span> Preview; upstream does not recommend production use</li>
</ul>
</div>

## Files first, execution second

Computer provides a SQLite-backed filesystem inside a Durable Object. The workspace can store files without any execution backend. Add a backend when the agent also needs to run commands against those files.

The package is a workspace abstraction, not a desktop or GUI automation service. Its pinned [package guide](https://www.npmjs.com/package/@cloudflare/computer/v/0.2.1) describes the filesystem, execution backends, Git integration, and RPC lifetime.

## Save a plan

This helper accepts the package's `DurableObjectStorageLike` storage interface, creates a workspace over it, writes a plan, and reads it back. It needs durable storage but no command runner.

```fsharp
open Fable.Core

module Computer = FSharp.CloudEdge.Runtime.Computer

let savePlan (storage: Computer.DurableObjectStorageLike) (text: string) =
    let workspace = Computer.Exports.Workspace(Computer.WorkspaceOptions.Create storage)
    async {
        do! workspace.fs.writeFile("/plan.md", text) |> Async.AwaitPromise
        return! workspace.fs.readFile("/plan.md", "utf8") |> Async.AwaitPromise
    }
    |> Async.StartAsPromise
```

## Choose an execution backend

| Backend | Work it runs | Configuration |
| --- | --- | --- |
| None | File reads and writes only | SQLite-backed Durable Object storage |
| Container | Linux commands and installed binaries | A compatible container running `computerd` |
| Worker shell | Supported shell commands through `just-bash` | Worker Loader binding and experimental flag |
| Worker JavaScript | ECMAScript modules with structured inputs and results | Worker Loader binding and experimental flag |

The package also requires `nodejs_compat`. Register the appropriate backend and select it through the runtime API. A container backend has a separate filesystem synchronization lifecycle; file changes and command completion must be handled together. Worker-backed execution accesses the workspace through its Durable Object.

The F# constructors live under `Runtime.Computer.Backends`: `Container.Exports.CloudflareContainerBackend`, `WorkerShell.Exports.WorkerShellBackend`, and `WorkerJavascript.Exports.WorkerJavaScriptBackend`. Pass the selected backend through `WorkspaceOptions.backends`. The pinned package guide includes the host setup; Worker-backed execution requires `"worker_loaders": [{ "binding": "LOADER" }]` and `"compatibility_flags": ["nodejs_compat", "experimental"]` in Wrangler configuration.

The F# example above exercises the filesystem API only. Backend construction, execution, synchronization, and recovery need separate consumer fixtures and runtime verification.

## Git and published work

Enable the optional Git client to inspect and commit local workspace files. Use the Artifacts helper when the task should publish or share repositories. Local durable files, local Git history, and remote repository storage remain independently useful parts of the workflow.

## Remote handles

When accessing a workspace across the Worker–Durable Object boundary, follow the package's disposal rules for returned workspace and execution handles. Keep durable task identifiers in application state rather than treating a live RPC handle as a saved workspace.

## Related Pages

- [Artifacts](artifacts.md)
- [Git tools](git.md)
- [Choosing an execution environment](../compute.md)

## Help verify these bindings

The examples on this page are checked against F# source projects. Compilation and emitted JavaScript checks do not establish hosted service behavior. Useful targets for community verification include storage interface compatibility, file encodings, backend events and results, filesystem synchronization, and handle disposal.

See [Verify bindings](../../guide/verify-bindings.md) for the existing evidence, reproducible checks, and the [binding issue form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include the pinned package version and the specific behavior exercised; successful reproductions are useful evidence too.
