---
title: Agents
description: Stateful agents with tools, schedules and MCP.
order: 6
---

<div class="ce-block-head">
<p class="ce-block-lead">Each agent is a Durable Object with its own SQLite database and WebSocket connections, so a game lobby or a team channel is one small F# class. Your Worker can also run model-generated code in a sandbox and respond with synthesized speech.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Agents</code> <code>Runtime.AgentsAiChatAgent</code> <code>Runtime.AgentsMcpClient</code> <code>Runtime.CodeMode</code> <code>Runtime.Shell</code> <code>Runtime.ShellGit</code> <code>Runtime.Voice</code> <code>Runtime.VoiceErrors</code></li>
<li><span>npm</span> <code>agents</code> 0.22.0, <code>@cloudflare/codemode</code> 0.5.1, <code>@cloudflare/shell</code> 0.4.3, <code>@cloudflare/voice</code> 0.4.0</li>
<li><span>Free plan</span> 100,000 Durable Object requests a day</li>
</ul>
</div>

:::warning
Cloudflare marks four of the modules on this page as experimental, and their documentation states that their APIs can change between releases:

- `agents/lifecycle`, in Game Lobby and Team Channel
- `@cloudflare/codemode`, in Snippet Runner
- `@cloudflare/shell`, in Drafts Folder
- `@cloudflare/voice`, in Audio Preview
:::

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

<div class="ce-needs"><p><strong>Needs</strong> a Durable Object binding for <code>GameLobby</code>, with each lobby addressed by name. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare the binding.</p></div>

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

<div class="ce-needs"><p><strong>Needs</strong> a Durable Object binding for <code>TeamChannel</code> on the SQLite storage backend.</p></div>

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

<div class="ce-needs"><p><strong>Needs</strong> a Worker Loader binding named <code>LOADER</code>. <a href="https://developers.cloudflare.com/dynamic-workers/pricing/">Dynamic Workers</a> are available on the Workers Paid plan.</p></div>

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

## Drafts Folder

`Workspace` from `@cloudflare/shell` is a file system stored in the Durable Object's SQLite database. On a `PUT`, the object saves the request body as a Markdown file at the request path, and every response lists the drafts with their sizes.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Shell = FSharp.CloudEdge.Runtime.Shell

type DraftsFolder(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    let workspace = Shell.Exports.Workspace(Shell.WorkspaceOptions.Create(sql = ctx.storage.sql))

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch request =
            async {
                let path = Workers.Exports.URL(U2.Case1 request.url).pathname
                if request.``method`` = "PUT" then
                    let! text = request.text () |> Async.AwaitPromise
                    do! workspace.writeFile (path, text, "text/markdown") |> Async.AwaitPromise
                let! drafts = workspace.glob "/**/*.md" |> Async.AwaitPromise
                let listing = drafts |> Array.map (fun draft -> {| path = draft.path; size = draft.size |})
                return Workers.Exports.Response.json listing
            }
            |> Async.StartAsPromise
            |> U2.Case1
```

<div class="ce-needs"><p><strong>Needs</strong> a SQLite-backed Durable Object binding for <code>DraftsFolder</code>.</p></div>

## Audio Preview

The Worker responds to a POSTed article with MPEG audio of its first two sentences, which `SentenceChunker` splits out and `WorkersAITTS` synthesizes with Workers AI. If the `synthesize` call throws, `logVoiceError` writes one structured log entry and the caller receives the error text, cut to at most 300 characters.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Voice = FSharp.CloudEdge.Runtime.Voice
module Errors = FSharp.CloudEdge.Runtime.VoiceErrors.Errors

type Env =
    abstract AI: Voice.AiLike

let opening (article: string) =
    let chunker = Voice.Exports.SentenceChunker()
    Array.append (chunker.add article) (chunker.flush ()) |> Array.truncate 2 |> String.concat " "

let failed (message: string) =
    Workers.Exports.Response.json ({| error = message |}, U2.Case2(Workers.ResponseInit.Create(status = 502.)))

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                try
                    let! article = request.text () |> Async.AwaitPromise
                    let tts = Voice.Exports.WorkersAITTS env.AI
                    let! audio = tts.synthesize (opening article) |> Async.AwaitPromise
                    match audio with
                    | Some bytes ->
                        let headers = [| [| "content-type"; "audio/mpeg" |] |]
                        return Workers.Exports.Response.Create(bytes, Workers.ResponseInit.Create(headers = headers))
                    | None -> return failed "No audio"
                with caught ->
                    let error = Errors.Exports.toVoiceError (caught, "Speech is unavailable")
                    Errors.Exports.logVoiceError (Errors.VoiceErrorLogOptions.Create(``component`` = "AudioPreview", stage = "synthesize", message = "Preview failed", error = error))
                    return failed (Errors.Exports.voiceErrorMessage (error, "Speech is unavailable"))
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> a Workers AI binding named <code>AI</code>. <code>Voice.AiLike</code> is the Voice package's own type for that binding, and <code>WorkersAITTS</code> calls the <code>@cf/deepgram/aura-1</code> model unless its options set another. The free plan includes 10,000 Workers AI Neurons a day.</p></div>

## Support Inbox

This Email Worker forwards support mail to your team and skips automatic replies. `isAutoReplyEmail` from the Agents SDK's email module returns true when one of three headers marks a message as an automatic reply, such as an out-of-office notice.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Email = FSharp.CloudEdge.Runtime.AgentsAiChatAgent.Email

type Env =
    abstract TEAM_ADDRESS: string

let isAutoReply (message: Workers.ForwardableEmailMessage) =
    [| "auto-submitted"; "x-auto-response-suppress"; "precedence" |]
    |> Array.choose (fun key ->
        message.headers.get key
        |> Option.map (fun value -> Email.EmailHeader.Create(key = key, value = value)))
    |> Email.Exports.isAutoReplyEmail

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        email = fun message env _ ->
            if isAutoReply message then None
            else
                message.forward env.TEAM_ADDRESS
                |> Async.AwaitPromise
                |> Async.Ignore
                |> Async.StartAsPromise
                |> Some
    )
```

<div class="ce-needs"><p><strong>Needs</strong> an Email Routing rule whose action sends mail to this Worker, and a <code>TEAM_ADDRESS</code> variable set to a verified destination address. Cloudflare's <a href="https://developers.cloudflare.com/email-routing/email-workers/enable-email-workers/">Email Workers guide</a> covers the rule.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { isAutoReplyEmail } from "agents/email";
import { choose } from "./fable_modules/fable-library-js.5.13.0/Array.js";
import { awaitPromise, ignore, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";

export function isAutoReply(message) {
    return isAutoReplyEmail(choose((key) => {
        let value;
        const option_1 = message.headers.get(key);
        if (option_1 != null) {
            return (value = option_1, {
                key: key,
                value: value,
            });
        }
        else {
            return undefined;
        }
    }, ["auto-submitted", "x-auto-response-suppress", "precedence"]));
}

export const worker = {
    email: (message, env, _arg) => (isAutoReply(message) ? undefined : startAsPromise(ignore(awaitPromise(message.forward(env.TEAM_ADDRESS))))),
};

export default worker;
```

</details>

## Library Table

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.Agents` | `agents` 0.22.0 | Routing, lifecycle and MCP |
| `Runtime.AgentsAiChatAgent` | `agents` 0.22.0 | Email, schedules and MCP servers |
| `Runtime.AgentsMcpClient` | `agents` 0.22.0 | MCP client manager |
| `Runtime.CodeMode` | `@cloudflare/codemode` 0.5.1 | Sandboxed runs of generated code |
| `Runtime.Shell` | `@cloudflare/shell` 0.4.3 | Workspace files and state |
| `Runtime.ShellGit` | `@cloudflare/shell` 0.4.3 | Git and Code Mode tools |
| `Runtime.Voice` | `@cloudflare/voice` 0.4.0 | Speech pipeline and providers |
| `Runtime.VoiceErrors` | `@cloudflare/voice` 0.4.0 | Voice error helpers |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/durable-objects/"><strong>Durable Objects</strong><span>Objects with storage and alarms</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/ai/"><strong>AI</strong><span>Chat agents and model providers</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/compute/"><strong>Compute</strong><span>Linux sandboxes for user code</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Bindings and uploads</span></a>
</div>
