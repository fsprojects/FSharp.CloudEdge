---
title: Email agents
description: Handle inbound email as an event in an agent workflow.
---

<div class="ce-block-head">
<p class="ce-block-lead">Handle inbound email as an event in an agent workflow.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.AgentsAiChatAgent</code></li>
<li><span>npm</span> <code>agents 0.22.0</code></li>
<li><span>Upstream/API</span> Requires an Email Routing configuration</li>
</ul>
</div>

## Email as an input

An Email Worker receives a message through an Email Routing rule. Your handler can inspect it, forward it, or connect it to application state. The example below filters automatic replies before forwarding support mail.

The `agents/email` helpers are delivered in `Runtime.AgentsAiChatAgent`; the assembly name does not mean this example creates an AI chat agent or performs model inference.

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

## Connecting to an agent

Decide which application identity and conversation own the inbound message before invoking an agent. Forwarding alone needs no model. If an agent drafts a reply or takes an action, preserve the relationship between the incoming message, the task record, and the outgoing result.

## Related Pages

- [Agents SDK](sdk.md)
- [Chat agents](chat.md)

## Help verify these bindings

The examples on this page are checked against F# source projects. Compilation and emitted JavaScript checks do not establish hosted service behavior. Useful targets for community verification include header and message shapes, optional async handlers, forwarding results, and delivery through Email Routing.

See [Verify bindings](../../guide/verify-bindings.md) for the existing evidence, reproducible checks, and the [binding issue form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include the pinned package version and the specific behavior exercised; successful reproductions are useful evidence too.
