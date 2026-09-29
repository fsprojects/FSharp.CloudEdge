---
title: Background Work
description: Queues and Workflows for work that outlives a request.
order: 5
---

<div class="ce-block-head">
<p class="ce-block-lead">Reply to the visitor right away and do the slow work afterwards. Cloudflare redelivers a queue message or retries a Workflow step after a failure, and cron triggers run your Worker on a schedule.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Workers</code></li>
<li><span>npm</span> <code>@cloudflare/workers-types</code> 5.20260906.1</li>
<li><span>Free plan</span> 10,000 Queues operations and 3,000 Workflow steps a day</li>
</ul>
</div>

## Webhook Inbox

The `fetch` handler puts each incoming webhook on a queue and answers 202 at once. The `queue` handler posts the payloads to your order service, with `ack` for each delivered message and `retry` for a failed one.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract WEBHOOKS: Workers.Queue<string>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, string, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = (fun request env _ ->
            async {
                let! payload = request.text () |> Async.AwaitPromise
                let! _ = env.WEBHOOKS.send payload |> Async.AwaitPromise
                return Workers.Exports.Response.Create("accepted", Workers.ResponseInit.Create(status = 202.))
            }
            |> Async.StartAsPromise
            |> U2.Case1),
        queue = (fun batch _ _ ->
            async {
                for message in batch.messages do
                    let! response =
                        Workers.Exports.fetch(U3.Case1 "https://orders.example.com/hooks", Workers.RequestInit.Create(``method`` = "POST", body = message.body))
                        |> Async.AwaitPromise
                    if response.ok then message.ack ()
                    else message.retry (Workers.QueueRetryOptions.Create(delaySeconds = 30. * message.attempts))
            }
            |> Async.StartAsPromise
            |> Some)
    )
```

The retry delay is 30 seconds multiplied by the attempt count. By default, Cloudflare [redelivers a message three times](https://developers.cloudflare.com/queues/configuration/batching-retries/) before it marks the delivery failed.

<div class="ce-needs"><p><strong>Needs</strong> a queue binding named <code>WEBHOOKS</code>, with this Worker as the queue's consumer. <code>ComputeClient.QueuesCreateConsumer</code> creates the consumer, and <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare the binding.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";

export const worker = {
    fetch: (request, env, _arg) => startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(request.text()), (_arg_1) => singleton.Bind(awaitPromise(env.WEBHOOKS.send(_arg_1)), (_arg_2) => singleton.Return(new globalThis.Response("accepted", ({
        status: 202,
    }))))))),
    queue: (batch, _arg_3, _arg_4) => startAsPromise(singleton.Delay(() => singleton.For(batch.messages, (_arg_5) => {
        const message = _arg_5;
        return singleton.Bind(awaitPromise(fetch("https://orders.example.com/hooks", {
            method: "POST",
            body: message.body,
        })), (_arg_6) => {
            if (_arg_6.ok) {
                message.ack();
                return singleton.Zero();
            }
            else {
                message.retry({
                    delaySeconds: 30 * message.attempts,
                });
                return singleton.Zero();
            }
        });
    }))),
};

export default worker;
```

</details>

## Cart Reminder

When a visitor saves a cart, the Worker queues a reminder for an hour later. `delaySeconds` in `QueueSendOptions` sets the delay, up to 24 hours.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Cart = {| cartId: string; email: string |}

type Env =
    abstract REMINDERS: Workers.Queue<Cart>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, Cart, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! cart = request.json<Cart>() |> Async.AwaitPromise
                let later = Workers.QueueSendOptions.Create(delaySeconds = 3600.)
                let! _ = env.REMINDERS.send(cart, later) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| saved = cart.cartId |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";

export const worker = {
    fetch: (request, env, _arg) => startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(request.json()), (_arg_1) => {
        const cart = _arg_1;
        const later = {
            delaySeconds: 3600,
        };
        return singleton.Bind(awaitPromise(env.REMINDERS.send(cart, later)), (_arg_2) => singleton.Return(globalThis.Response.json({
            saved: cart.cartId,
        })));
    }))),
};

export default worker;
```

</details>

## Trial Emails

The `TrialEmails` Workflow sends a welcome email, then sleeps for seven days. After the sleep it sends a reminder to a user whose plan is still `trial`. Both emails are messages on the `EMAILS` queue, and the instance ID is the user's ID.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

type Email = {| user: string; template: string |}

type Env =
    abstract EMAILS: Workers.Queue<Email>
    abstract DB: Workers.D1Database

type TrialEmails(ctx: Workers.ExecutionContext<obj>, env: Env) =
    inherit Runtime.WorkflowEntrypoint<Env, obj>(ctx, env)

    override _.run(event, step) =
        let user = event.instanceId
        async {
            do! step.``do``("welcome email", fun _ -> env.EMAILS.send {| user = user; template = "welcome" |})
                |> Async.AwaitPromise |> Async.Ignore
            do! step.sleep.Invoke("trial period", U2.Case2 "7 days") |> Async.AwaitPromise
            let! plan =
                step.``do``("check plan", fun _ ->
                    env.DB.prepare("SELECT plan FROM users WHERE id = ?").bind(user).first<string>("plan"))
                |> Async.AwaitPromise
            if plan = Some "trial" then
                do! step.``do``("reminder email", fun _ -> env.EMAILS.send {| user = user; template = "trial-ending" |})
                    |> Async.AwaitPromise |> Async.Ignore
            return box {| user = user; plan = plan |}
        }
        |> Async.StartAsPromise
```

Workflows cache the value each `step.do` returns, keyed by the step's name, so give steps fixed names. By default, Cloudflare [retries a failed step five times](https://developers.cloudflare.com/workflows/build/sleeping-and-retrying/) with exponential backoff.

<div class="ce-needs"><p><strong>Needs</strong> a Workflow for the <code>TrialEmails</code> class, plus the <code>EMAILS</code> queue and <code>DB</code> database bindings. <code>ComputeClient.WorCreateOrModifyWorkflow</code> creates the Workflow from the class name and the Worker's script name.</p></div>

## Trial Signup

This Worker takes the user's ID from the `user` query parameter. On a POST, it creates a `TrialEmails` instance under that ID, and on a GET it finds the instance with `get`. Either way, the response contains the instance status.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract TRIALS: Workers.Workflow<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                match Workers.Exports.URL(U2.Case1 request.url).searchParams.get "user" with
                | None ->
                    return Workers.Exports.Response.Create("user is required", Workers.ResponseInit.Create(status = 400.))
                | Some user ->
                    let! trial =
                        if request.``method`` = "POST" then
                            env.TRIALS.create(Workers.WorkflowInstanceCreateOptions.Create(id = user)) |> Async.AwaitPromise
                        else
                            env.TRIALS.get user |> Async.AwaitPromise
                    let! state = trial.status () |> Async.AwaitPromise
                    return Workers.Exports.Response.json {| trial = trial.id; status = state.status |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> a Workflow binding named <code>TRIALS</code> for the <code>TrialEmails</code> class. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare bindings.</p></div>

## Session Cleanup

Each of the Worker's [cron triggers](https://developers.cloudflare.com/workers/configuration/cron-triggers/) runs the `scheduled` handler on its schedule, in UTC. The `DELETE` removes every session that expired before `scheduledTime`.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract DB: Workers.D1Database

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        scheduled = fun controller env _ ->
            env.DB.prepare("DELETE FROM sessions WHERE expires_at < ?").bind(controller.scheduledTime).run()
            |> Async.AwaitPromise
            |> Async.Ignore
            |> Async.StartAsPromise
            |> Some
    )
```

<div class="ce-needs"><p><strong>Needs</strong> a cron trigger on the Worker, such as <code>0 3 * * *</code> for 03:00 UTC every day. <code>ComputeClient.WorkerCronTriggerUpdateCronTriggers</code> sets the triggers, as <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows. The free plan limit is 5 per account.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, ignore, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";

export const worker = {
    scheduled: (controller, env, _arg) => startAsPromise(ignore(awaitPromise(env.DB.prepare("DELETE FROM sessions WHERE expires_at < ?").bind(controller.scheduledTime).run()))),
};

export default worker;
```

</details>

## Library Table

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.Workers` | `@cloudflare/workers-types` 5.20260906.1 | Queues, Workflows and cron handlers |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/durable-objects/"><strong>Durable Objects</strong><span>Rooms with storage and alarms</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/"><strong>Workers</strong><span>Handlers and bindings</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/storage/"><strong>Storage</strong><span>KV, D1 and R2</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>A Worker and its bindings</span></a>
</div>
