---
title: Agents SDK
description: Give each agent an identity, durable state, connections, and a lifecycle.
---

<div class="ce-block-head">
<p class="ce-block-lead">Give each agent an identity, durable state, connections, and a lifecycle.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Agents</code></li>
<li><span>npm</span> <code>agents 0.22.0</code></li>
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
        lifecycle.broadcast (message, [| player.id |])
```

<div class="ce-needs"><p><strong>Needs</strong> a Durable Object binding for <code>GameLobby</code> and a Worker that forwards each connection to a lobby by name, as Room Lookup on the <a href="/FSharp.CloudEdge/libraries/platform/durable-objects/">Durable Objects</a> page does. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare the binding.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { concat } from "./fable_modules/fable-library-js.5.13.0/String.js";
import { defaultOf } from "./fable_modules/fable-library-js.5.13.0/Util.js";
import { FSharpRef } from "./fable_modules/fable-library-js.5.13.0/Types.js";
import { Lifecycle } from "agents/lifecycle";
import { DurableObject } from "cloudflare:workers";
import { class_type, obj_type } from "./fable_modules/fable-library-js.5.13.0/Reflection.js";

export class GameLobby extends DurableObject {
    constructor(ctx, env) {
        super(ctx, env);
        const this$ = new FSharpRef(defaultOf());
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
        _.lifecycle.broadcast(message, [player.id]);
    }
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
            lifecycle.broadcast (message, [| sender.id |])
        | _ -> ()
```

<div class="ce-needs"><p><strong>Needs</strong> a SQLite-backed Durable Object binding for <code>TeamChannel</code>, with channels addressed by name.</p></div>

## Related Pages

- [MCP connections and tools](mcp.md)
- [Chat agents](chat.md)
- [Durable Objects](../platform/durable-objects.md)

## Help verify these bindings

The examples on this page are checked against F# source projects. Compilation and emitted JavaScript checks do not establish hosted service behavior. Useful targets for community verification include agent identity, callback signatures, connection lifecycle, state recovery, and the F# lifecycle integration.

See [Verify bindings](../../guide/verify-bindings.md) for the existing evidence, reproducible checks, and the [binding issue form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include the pinned package version and the specific behavior exercised; successful reproductions are useful evidence too.
