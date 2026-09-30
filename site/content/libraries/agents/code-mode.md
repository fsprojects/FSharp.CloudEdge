---
title: Code Mode
description: Let an agent write code that composes the tools your application exposes.
---

<div class="ce-block-head">
<p class="ce-block-lead">Let an agent write code that composes the tools your application exposes.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.CodeMode</code></li>
<li><span>npm</span> <code>@cloudflare/codemode 0.5.1</code></li>
<li><span>Upstream/API</span> Experimental</li>
</ul>
</div>

## Tool orchestration

Code Mode executes generated JavaScript against supplied tool providers. A generated program can call several tools, process intermediate results, and return a compact answer. It is useful when a task needs a sequence of tool operations rather than a complete Linux environment.

The example uses a Dynamic Worker executor and therefore needs a Worker Loader binding. The filesystem and Git capabilities on the [Shell](shell.md) and [Git tools](git.md) pages can be exposed as additional providers. Native binaries and package-manager workflows belong on the [Sandbox](sandbox.md) page.

## Snippet Runner

POST a JavaScript snippet, such as one a model generated, and `DynamicWorkerExecutor` runs it in a new Dynamic Worker with outbound `fetch` and `connect` blocked. The JSON response contains the console output with either the return value or, after a throw or a five-second timeout, an `error` field.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module CodeMode = FSharp.CloudEdge.Runtime.CodeMode

type Env =
    abstract LOADER: Workers.WorkerLoader

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! code = request.text () |> Async.AwaitPromise
                let options = CodeMode.DynamicWorkerExecutorOptions.Create(loader = env.LOADER, timeout = 5000.)
                let executor = CodeMode.Exports.DynamicWorkerExecutor options
                let! outcome = executor.execute (code, U2.Case1 [||]) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| result = outcome.result; error = outcome.error; logs = outcome.logs |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> a Worker Loader binding named <code>LOADER</code>. <a href="https://developers.cloudflare.com/dynamic-workers/pricing/">Dynamic Workers</a> require the Workers Paid plan.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { DynamicWorkerExecutor } from "@cloudflare/codemode";
import { unwrap } from "./fable_modules/fable-library-js.5.13.0/Option.js";

export const worker = {
    fetch: (request, env, _arg) => startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(request.text()), (_arg_1) => {
        const options = {
            loader: env.LOADER,
            timeout: 5000,
        };
        const executor = new DynamicWorkerExecutor(options);
        return singleton.Bind(awaitPromise(executor.execute(_arg_1, [])), (_arg_2) => {
            let result;
            const outcome = _arg_2;
            return singleton.Return(globalThis.Response.json((result = outcome.result, {
                error: unwrap(outcome.error),
                logs: unwrap(outcome.logs),
                result: result,
            })));
        });
    }))),
};

export default worker;
```

</details>

## Define the tools the run can use

Decide which operations a task needs, implement them on the host, and expose those providers to the executor. Keep credentials in the host's tool configuration. The generated program receives the capabilities you supply; a durable workspace or an external MCP connection must be provided explicitly.

Code execution and model inference are separate steps. Configure the model through a [model provider](../ai.md), then pass the generated code to the executor. Inspect execution errors and results before deciding whether the agent should retry.

The pinned package's [source and usage guide](https://github.com/cloudflare/agents/tree/main/packages/codemode) describe provider resolution and executor options.

## Related Pages

- [Shell state tools](shell.md)
- [Git tool providers](git.md)
- [MCP tool discovery](mcp.md)

## Help verify these bindings

The examples on this page are checked against F# source projects. Compilation and emitted JavaScript checks do not establish hosted service behavior. Useful targets for community verification include provider type compatibility, argument and result marshalling, execution failures, and cancellation.

See [Verify bindings](../../guide/verify-bindings.md) for the existing evidence, reproducible checks, and the [binding issue form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include the pinned package version and the specific behavior exercised; successful reproductions are useful evidence too.

## NuGet packages

[Runtime.CodeMode 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.CodeMode/0.1.0), [Runtime.Workers 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Workers/0.1.0).

See [installation and release availability](../../guide/packages.md).
