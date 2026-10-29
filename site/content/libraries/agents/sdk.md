---
title: Agents SDK
description: Give each agent an identity, durable state, connections, and a lifecycle.
---

<div class="ce-block-head">
<p class="ce-block-lead">Give each agent an identity, durable state, connections, and a lifecycle.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Agents</code></li>
<li><span>npm</span> <code>agents 0.30.1</code></li>
<li><span>Upstream/API</span> Lifecycle API experimental</li>
</ul>
</div>

## Agent identity and state

An agent instance has a name and Durable Object identity. Choose that identity around the unit that owns the work: a conversation, project, or team. The examples below use the SDK's lifecycle integration with an F# Durable Object.

Connection state, application tables, and scheduling belong to the agent. Files it edits can live in a [workspace](computer.md); code it runs can execute in a [Sandbox](sandbox.md). Those resources have their own persistence and lifecycle rules.

`agents/lifecycle` is experimental in the pinned package. The examples compile against that version; consult the [Agents SDK documentation](https://developers.cloudflare.com/agents/) when changing versions.

## Game Lobby

Players join the lobby over WebSockets. `Lifecycle.install` adds the Agents SDK's connection handling to a plain Durable Object, and `broadcast` sends each message to every player except the sender.

:::info
`Runtime.Agents` describes the SDK's `Agent` class as an F# interface. F# agents therefore extend `DurableObject` from the Workers runtime and install `Lifecycle`, the pattern Cloudflare documents for plain Durable Objects. With `[<AttachMembers>]`, Fable emits `onConnect` and `onMessage` as methods of the JavaScript class, where Lifecycle calls them by name.
:::

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Agents = FSharp.CloudEdge.Runtime.Agents
module Lifecycle = FSharp.CloudEdge.Runtime.Agents.Lifecycle

[<AttachMembers>]
type GameLobby(ctx: Workers.DurableObjectState<obj>, env: obj) as this =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    let lifecycle = Lifecycle.Lifecycle.install<obj, obj> this

    member _.onConnect(player: Agents.Connection<obj>, _context: Agents.ConnectionContext) =
        player.send (U3.Case1 $"Welcome to {lifecycle.name}")

    member _.onMessage(player: Agents.Connection<obj>, message: Agents.WSMessage) =
        for socket in ctx.getWebSockets() do
            if not (obj.ReferenceEquals(socket, player)) then socket.send message
```

<div class="ce-needs"><p><strong>Needs</strong> a Durable Object binding for <code>GameLobby</code> and a Worker that forwards each connection to a lobby by name, as Room Lookup on the <a href="/FSharp.CloudEdge/libraries/platform/durable-objects/">Durable Objects</a> page does. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare the binding.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript

import { concat } from "./fable_modules/fable-library-js.5.24.0/String.js";
import { item } from "./fable_modules/fable-library-js.5.24.0/Array.js";
import { defaultOf } from "./fable_modules/fable-library-js.5.24.0/Util.js";
import { FSharpRef } from "./fable_modules/fable-library-js.5.24.0/Types.js";
import { Lifecycle } from "agents/lifecycle";
import { DurableObject } from "cloudflare:workers";
import { class_type, obj_type } from "./fable_modules/fable-library-js.5.24.0/Reflection.js";

export class GameLobby extends DurableObject {
    constructor(ctx, env) {
        super(ctx, env);
        const this$ = new FSharpRef(defaultOf());
        this.ctx = ctx;
        this$.contents = this;
        this.lifecycle = Lifecycle.install(this$.contents);
        this["init@11"] = 1;
    }
    onConnect(player, _context) {
        const _ = this;
        player.send(concat("Welcome to ", _.lifecycle.name));
    }
    onMessage(player, message) {
        const _ = this;
        const arr = _.ctx.getWebSockets();
        for (let idx = 0; idx <= (arr.length - 1); idx++) {
            const socket = item(idx, arr);
            if (!(socket === player)) {
                socket.send(message);
            }
        }
    }
}

export function GameLobby_$reflection() {
    return class_type("GameLobby.GameLobby", undefined, GameLobby, class_type("FSharp.CloudEdge.Runtime.Workers.CloudflareModule.Workers.DurableObject`2", [obj_type, obj_type], DurableObject));
}

export function GameLobby_$ctor_6A0EF749(ctx, env) {
    return new GameLobby(ctx, env);
}
```

</details>

## Team Channel

`DurableObjectCapability.Create` builds a lifecycle capability from F# functions. The `history` capability creates the messages table before the channel handles any work, and each teammate who connects receives the last 20 messages.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Agents = FSharp.CloudEdge.Runtime.Agents
module Lifecycle = FSharp.CloudEdge.Runtime.Agents.Lifecycle

let history (sql: Workers.SqlStorage) =
    Lifecycle.DurableObjectCapability.Create(onStart = fun _ ->
        sql.exec "CREATE TABLE IF NOT EXISTS messages (body TEXT)" |> ignore
        None)

[<AttachMembers>]
type TeamChannel(ctx: Workers.DurableObjectState<obj>, env: obj) as this =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    let lifecycle = Lifecycle.Lifecycle.install<obj, obj>(this).``use`` (history ctx.storage.sql)

    member _.onConnect(teammate: Agents.Connection<obj>, _context: Agents.ConnectionContext) =
        let recent = ctx.storage.sql.exec<{| body: string |}>("SELECT body FROM messages ORDER BY rowid DESC LIMIT 20").toArray ()
        for message in Array.rev recent do
            teammate.send (U3.Case1 message.body)

    member _.onMessage(sender: Agents.Connection<obj>, message: Agents.WSMessage) =
        match message with
        | U3.Case1 text ->
            ctx.storage.sql.exec ("INSERT INTO messages (body) VALUES (?)", text) |> ignore
            for socket in ctx.getWebSockets() do
                if not (obj.ReferenceEquals(socket, sender)) then socket.send message
        | _ -> ()
```

<div class="ce-needs"><p><strong>Needs</strong> a SQLite-backed Durable Object binding for <code>TeamChannel</code>, with channels addressed by name.</p></div>

## Pi lifecycle

The 0.1.1 package set includes `Runtime.AgentsPi`, `Runtime.PiAi`, and `Runtime.PiDurable` for the Agents Pi lifecycle. The Pi bindings are generated against `agents` 0.30.1 and the `@earendil-works/pi-ai`, `@earendil-works/pi-durable`, and `@earendil-works/chord` 1.4.0 packages. Install the JavaScript packages alongside the NuGet libraries; the [package guide](../../guide/packages.md) covers the required NuGet sources.

### Assemble the pieces

| Package | Role in a Pi Worker |
| --- | --- |
| `Runtime.AgentsPi` | Pi lifecycle capability and typed submit/prompt projections. |
| `Runtime.Agents` | Installs the capability on the Durable Object lifecycle. |
| `Runtime.PiAi` | Model registry, providers, model definitions, and the deterministic test provider. |
| `Runtime.PiDurable` | Opens the durable harness using lifecycle storage and context; it also exposes the Chord context types. |
| `Runtime.Workers` and `Support.Workers` | Worker and Durable Object types and, when routing requests to an object by name, the transport helper. |
| `Support.AI.V4.Provider`, `Support.AI.V4.ProviderUtils`, `Support.AI.V7`, `Support.JsonSchema`, and `Support.Zod.V4` | Shared types used by the Agents and Pi dependency graph. NuGet restores these transitively. |

Add the public Xantham Cloudsmith source described in the [package guide](../../guide/packages.md), then add the Pi package to an F# project and install its JavaScript dependencies. Add `FSharp.CloudEdge.Support.Workers` directly if you use the routing helper:

```bash
dotnet add package FSharp.CloudEdge.Runtime.AgentsPi --version 0.1.1
dotnet add package FSharp.CloudEdge.Support.Workers --version 0.1.1
npm install agents@0.30.1 @earendil-works/pi-ai@1.4.0 @earendil-works/pi-durable@1.4.0 @earendil-works/chord@1.4.0
```

The typed Pi projections use Fickl. With a local .NET tool manifest, install `fickl` version `0.7.3` and compile the project with `dotnet fickl <project.fsproj> --outDir <output-directory>`. Configure and export a SQLite-backed Durable Object in the Worker and declare its binding and migration in your Worker configuration. The [Durable Objects guide](../platform/durable-objects.md) covers the hosting setup.

### Create and install a harness

A harness factory receives the lifecycle context and opens the durable Pi harness with its storage and context. The model provider and model ID select the default session model:

```fsharp
let createHarness (factory: Pi.PiHarnessFactory) provider modelId : Pi.PiHarness =
    let model = Pi.PiModel.Create(provider = provider, id = modelId)
    let defaults = Pi.PiSessionDefaults.Create(model = model)
    Pi.Exports.PiHarness(Pi.PiHarnessOptions.Create(factory, defaults = defaults))
```

The following host wiring is from the tested Pi client. It creates a **faux** model that always replies `fixture reply`, so it is suitable for learning and local checks without provider credentials. Replace the faux provider and model with your configured Pi model provider before using live inference. The `createHarness` and `contextName` functions and module aliases are defined in the [complete tested client](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/tests/AgentsPi/Client.fs).

```fsharp
[<AttachMembers>]
type PiProbe(state: Workers.DurableObjectState<obj>, env: ProbeEnv) as this =
    inherit Runtime.DurableObject<ProbeEnv, obj>(state, env)

    let installedLifecycle = Lifecycle.Lifecycle<ProbeEnv, obj>.install<ProbeEnv, obj>(this)
    let mutable invocationContext = ""
    let faux =
        PiAi.Exports.fauxProvider(
            PiAi.RegisterFauxProviderOptions.Create(
                api = "cloudedge-fixture",
                models = [| PiAi.FauxModelDefinition.Create("fixture", input = [| PiAi.ModelInputModality.Text; PiAi.ModelInputModality.Image |]) |]))
    let models = PiAi.Exports.createModels()

    do
        faux.setResponses(Array.init 20 (fun _ -> U2.Case1(PiAi.Exports.fauxAssistantMessage(U5.Case1 "fixture reply"))))
        models.setProvider(faux.provider)

    let pi =
        createHarness
            (fun context ->
                invocationContext <- contextName context
                let registry = PiDurable.Exports.createRegistry()
                let options = PiDurable.HarnessOptions.Create(U2.Case1 models, registry, onWake = context.onWake)
                PiDurable.Exports.Harness.``open``(context.storage, options, context.context) |> U2.Case2)
            (faux.getModel().provider)
            (faux.getModel().id)

    do installedLifecycle.``use``(unbox<Lifecycle.DurableObjectCapability<obj>> pi) |> ignore

    member _.lifecycle = installedLifecycle

```

The `PiProbe` binding in this example is a Durable Object namespace. The factory creates a Pi registry, opens the durable harness with `context.storage` and `context.context`, and registers it on the Agents lifecycle. `onWake` lets the durable harness reconnect work after the object wakes. The cast at `use` reflects a structural JavaScript capability accepted by the SDK.

### Send work and get a response

Use the typed submission projection for text or structured text and image inputs. An operation ID lets callers recognize a repeated submission:

```fsharp
let submitText harness text operationId : JS.Promise<Pi.PiReceipt> =
    Submit.submit harness (Submit.Input.Text text) (Some(Pi.PiSubmitOptions.Create(operationId = operationId)))
```

`submitText` returns a receipt; use its operation ID with `harness.wait` when you need to await completion. For an immediate terminal response, use the prompt projection:

```fsharp
let promptInput harness (input: Submit.Input) operationId : JS.Promise<Pi.PiPromptResponse> =
    Prompt.prompt harness input (Some(Pi.PiSubmitOptions.Create(operationId = operationId)))
```

`Submit.Input.Items` supports typed text and image parts; image data is base64 with a MIME type. Both the harness and `harness.session()` have submit and prompt projections. Choose stable operation IDs for retries: the tested lifecycle preserves receipts and its SQLite transcript across a restart.

`Pi` here is `FSharp.CloudEdge.Runtime.AgentsPi.Harness.Pi`, `Submit` is `FSharp.CloudEdge.Pi.HarnessSubmit`, and `Prompt` is `FSharp.CloudEdge.Pi.HarnessPrompt`. Install the harness with the agent lifecycle, configure its model provider, and bind the host to a SQLite-backed Durable Object. The JavaScript SDK accepts the harness structurally as a capability; in F#, pass it to the lifecycle’s `use` method as `unbox<Lifecycle.DurableObjectCapability<obj>> harness`. The repository's `tests/AgentsPi/Client.fs` shows the complete local lifecycle, including session submission, prompts, and restart recovery.

## Related Pages

- [MCP connections and tools](mcp.md)
- [Chat agents](chat.md)
- [Durable Objects](../platform/durable-objects.md)

## Testing and feedback

Useful test cases include agent identity, callback signatures, connection lifecycle, state recovery, and the F# lifecycle integration.

[Verify bindings](../../guide/verify-bindings.md) explains how to run checks and [report an issue](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include a small reproduction and the package versions used; working examples are welcome too.

## NuGet packages

[Runtime.Agents 0.1.1](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Agents/0.1.1), [Runtime.AgentsPi 0.1.1](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.AgentsPi/0.1.1), [Runtime.PiAi 0.1.1](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.PiAi/0.1.1), [Runtime.PiDurable 0.1.1](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.PiDurable/0.1.1), [Runtime.Workers 0.1.1](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Workers/0.1.1).

See [installation and release availability](../../guide/packages.md).
