---
title: Compute
description: Sandboxes and workspaces for running code.
order: 8
---

<div class="ce-block-head">
<p class="ce-block-lead">Run your users' code in a Linux container and reply with the output in the same request. Your Worker gets a sandbox by ID, and each ID maps to a separate container.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Sandbox</code> <code>Runtime.SandboxBridge</code> <code>Runtime.Computer</code> <code>Runtime.ComputerArtifacts</code></li>
<li><span>npm</span> <code>@cloudflare/sandbox</code> 0.12.9, <code>@cloudflare/computer</code> 0.2.1</li>
<li><span>Plan</span> Workers Paid for Sandbox</li>
<li><span>Status</span> Sandbox beta, Computer preview</li>
</ul>
</div>

## Code Runner

Visitors POST Python source. `runCode` executes it in the sandbox's code interpreter, and the Worker sends back stdout and stderr as JSON.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Sandbox = FSharp.CloudEdge.Runtime.Sandbox

type Env =
    abstract Sandbox: Workers.DurableObjectNamespace<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let sandbox: Sandbox.ISandbox = Sandbox.Exports.getSandbox<Sandbox.Sandbox<obj>>(env.Sandbox, "playground")
                let! code = request.text () |> Async.AwaitPromise
                let! run = sandbox.runCode code |> Async.AwaitPromise
                return Workers.Exports.Response.json {| output = run.logs.stdout; errors = run.logs.stderr |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> a Durable Object binding named <code>Sandbox</code> for the <code>Sandbox</code> class of <code>@cloudflare/sandbox</code>, and a container image built on <code>cloudflare/sandbox</code>. For Python, use the image tag that ends in <code>-python</code>. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare the binding.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { getSandbox } from "@cloudflare/sandbox";

export const worker = {
    fetch: (request, env, _arg) => startAsPromise(singleton.Delay(() => {
        const sandbox = getSandbox(env.Sandbox, "playground");
        return singleton.Bind(awaitPromise(request.text()), (_arg_1) => singleton.Bind(awaitPromise(sandbox.runCode(_arg_1)), (_arg_2) => {
            let output;
            const run = _arg_2;
            return singleton.Return(globalThis.Response.json((output = run.logs.stdout, {
                errors: run.logs.stderr,
                output: output,
            })));
        }));
    })),
};

export default worker;
```

</details>

## Workspace Files

The Worker saves a `PUT` body to a file in the sandbox and serves the file on `GET`. `resolveWorkspacePath` from `Runtime.SandboxBridge` returns `None` for any path outside `/workspace`. The Worker responds to `?path=../etc/passwd` with a 400.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Sandbox = FSharp.CloudEdge.Runtime.Sandbox
module Bridge = FSharp.CloudEdge.Runtime.SandboxBridge.Bridge

type Env =
    abstract Sandbox: Workers.DurableObjectNamespace<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let url = Workers.Exports.URL(U2.Case1 request.url)
                let sandbox: Sandbox.ISandbox = Sandbox.Exports.getSandbox<Sandbox.Sandbox<obj>>(env.Sandbox, "files")
                match url.searchParams.get "path" |> Option.bind Bridge.Exports.resolveWorkspacePath with
                | None ->
                    return Workers.Exports.Response.Create("Paths must stay inside /workspace", Workers.ResponseInit.Create(status = 400.))
                | Some path when request.``method`` = "PUT" ->
                    let! body = request.text () |> Async.AwaitPromise
                    let! saved = sandbox.writeFile(path, body) |> Async.AwaitPromise
                    return Workers.Exports.Response.json {| saved = saved.path |}
                | Some path ->
                    let! file = sandbox.readFile path |> Async.AwaitPromise
                    return Workers.Exports.Response.Create(file.content)
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

## Notebook

Each notebook is a sandbox, and the Worker runs one Python cell per request. It reuses the notebook's code context, so a variable set in one cell is still defined in the next.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Sandbox = FSharp.CloudEdge.Runtime.Sandbox

type Env =
    abstract Sandbox: Workers.DurableObjectNamespace<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let url = Workers.Exports.URL(U2.Case1 request.url)
                let notebook = url.searchParams.get "notebook" |> Option.defaultValue "scratch"
                let sandbox: Sandbox.ISandbox = Sandbox.Exports.getSandbox<Sandbox.Sandbox<obj>>(env.Sandbox, notebook)
                let! contexts = sandbox.listCodeContexts () |> Async.AwaitPromise
                let! context =
                    match Array.tryHead contexts with
                    | Some context -> async { return context }
                    | None -> sandbox.createCodeContext () |> Async.AwaitPromise
                let! cell = request.text () |> Async.AwaitPromise
                let! result = sandbox.runCode(cell, Sandbox.RunCodeOptions.Create(context = context)) |> Async.AwaitPromise
                let values = result.results |> Array.choose (fun item -> item.text)
                return Workers.Exports.Response.json {| output = result.logs.stdout; values = values |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

## Test Runner

POST a repository URL, and the Worker clones it with `gitCheckout` into a new sandbox for that run. `execStream` returns the output of `npm ci` and `npm test` as Server-Sent Events. The Worker sends that stream as the response body, so the browser receives output while the tests run.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Sandbox = FSharp.CloudEdge.Runtime.Sandbox

type Env =
    abstract Sandbox: Workers.DurableObjectNamespace<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! repository = request.text () |> Async.AwaitPromise
                let runId = string (System.Guid.NewGuid())
                let sandbox: Sandbox.ISandbox = Sandbox.Exports.getSandbox<Sandbox.Sandbox<obj>>(env.Sandbox, runId)
                let checkout = Sandbox.ISandbox.GitCheckout.Options.Create(targetDir = "/workspace/app", depth = 1.)
                let! _ = sandbox.gitCheckout(repository, checkout) |> Async.AwaitPromise
                let! log = sandbox.execStream "cd /workspace/app && npm ci && npm test" |> Async.AwaitPromise
                let headers = {| ``Content-Type`` = "text/event-stream"; ``Cache-Control`` = "no-cache" |}
                return Workers.Exports.Response.Create(log, Workers.ResponseInit.Create(headers = headers))
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { newGuid } from "./fable_modules/fable-library-js.5.13.0/Guid.js";
import { getSandbox } from "@cloudflare/sandbox";

export const worker = {
    fetch: (request, env, _arg) => startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(request.text()), (_arg_1) => {
        const runId = newGuid();
        const sandbox = getSandbox(env.Sandbox, runId);
        const checkout = {
            targetDir: "/workspace/app",
            depth: 1,
        };
        return singleton.Bind(awaitPromise(sandbox.gitCheckout(_arg_1, checkout)), (_arg_2) => singleton.Bind(awaitPromise(sandbox.execStream("cd /workspace/app && npm ci && npm test")), (_arg_3) => {
            const headers = {
                "Cache-Control": "no-cache",
                "Content-Type": "text/event-stream",
            };
            return singleton.Return(new globalThis.Response(_arg_3, ({
                headers: headers,
            })));
        }));
    }))),
};

export default worker;
```

</details>

## Text Search

This Worker searches the `files` sandbox that Workspace Files uses. `shellQuote` from `Runtime.SandboxBridge` quotes the search term. Whatever the visitor typed, the shell passes it to `grep` as one argument.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Sandbox = FSharp.CloudEdge.Runtime.Sandbox
module Bridge = FSharp.CloudEdge.Runtime.SandboxBridge.Bridge

type Env =
    abstract Sandbox: Workers.DurableObjectNamespace<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let url = Workers.Exports.URL(U2.Case1 request.url)
                let term = url.searchParams.get "q" |> Option.defaultValue ""
                let sandbox: Sandbox.ISandbox = Sandbox.Exports.getSandbox<Sandbox.Sandbox<obj>>(env.Sandbox, "files")
                let! found = sandbox.exec $"grep -rn -- {Bridge.Exports.shellQuote term} /workspace" |> Async.AwaitPromise
                return Workers.Exports.Response.Create(found.stdout)
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { defaultArg } from "./fable_modules/fable-library-js.5.13.0/Option.js";
import { getSandbox } from "@cloudflare/sandbox";
import { concat } from "./fable_modules/fable-library-js.5.13.0/String.js";
import { shellQuote } from "@cloudflare/sandbox/bridge";

export const worker = {
    fetch: (request, env, _arg) => startAsPromise(singleton.Delay(() => {
        const url = new URL(request.url);
        const term = defaultArg(url.searchParams.get("q"), "");
        const sandbox = getSandbox(env.Sandbox, "files");
        return singleton.Bind(awaitPromise(sandbox.exec(concat("grep -rn -- ", shellQuote(term), " /workspace"))), (_arg_1) => singleton.Return(new globalThis.Response(_arg_1.stdout)));
    })),
};

export default worker;
```

</details>

## Team Repositories

[Cloudflare Artifacts](https://developers.cloudflare.com/artifacts/) stores Git repositories that any Git client can clone. `createArtifact` from `Runtime.ComputerArtifacts` returns a client that prefixes each repository name with a session ID. The Worker uses the team name as that ID, so each team's list contains only its own repositories. It creates a repository from a `POST` body and responds with the team's list as JSON.

:::warning
`@cloudflare/computer` 0.2.1 is a preview package. Its README states that the APIs are unstable and that the package is not suitable for production use at this time.
:::

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Artifacts = FSharp.CloudEdge.Runtime.ComputerArtifacts.Artifacts

type Env =
    abstract ARTIFACTS: Workers.Artifacts

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let url = Workers.Exports.URL(U2.Case1 request.url)
                let team = url.searchParams.get "team" |> Option.defaultValue "demo"
                let repositories = Artifacts.Exports.createArtifact(env.ARTIFACTS, team)
                if request.``method`` = "POST" then
                    let! name = request.text () |> Async.AwaitPromise
                    do! repositories.create name |> Async.AwaitPromise |> Async.Ignore
                let! all = repositories.list () |> Async.AwaitPromise
                return Workers.Exports.Response.json (all |> Array.map (fun repo -> {| name = repo.name; branch = repo.defaultBranch |}))
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> an Artifacts binding named <code>ARTIFACTS</code>. Artifacts is in closed beta, and access is by request.</p></div>

## Library Table

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.Sandbox` | `@cloudflare/sandbox` 0.12.9 | Commands, files and code in a sandbox |
| `Runtime.SandboxBridge` | `@cloudflare/sandbox` 0.12.9 | Bridge helpers and OpenAI Agents tools |
| `Runtime.Computer` | `@cloudflare/computer` 0.2.1 | Workspace filesystems for Durable Objects |
| `Runtime.ComputerArtifacts` | `@cloudflare/computer` 0.2.1 | Artifacts and shell command modules |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/durable-objects/"><strong>Durable Objects</strong><span>Rooms with storage and alarms</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/services/"><strong>Services</strong><span>Containers, Actors and OAuth</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>A Worker and its bindings</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/ai/"><strong>AI</strong><span>Models for your Worker</span></a>
</div>
