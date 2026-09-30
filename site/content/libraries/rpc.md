---
title: RPC
description: Cap'n Web remote procedure calls.
order: 10
---

<div class="ce-block-head">
<p class="ce-block-lead">Serve an API whose clients call your F# methods by name. Cap'n Web sends each call over HTTP or a WebSocket, and it can send a chain of dependent calls in one round trip.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Capnweb</code> <code>Runtime.Workers</code></li>
<li><span>npm</span> <code>capnweb</code> 0.12.0</li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/pages/functions/pricing/">100,000 requests a day, shared by Pages Functions and Workers</a></li>
</ul>
</div>

## Pricing API

The script on a pricing page calls `plans` and `quote` on this API. Cap'n Web exposes the class methods of an object that inherits `RpcTarget`, and on Workers its `RpcTarget` is the class exported by `cloudflare:workers`. With `[<AttachMembers>]`, Fable compiles the F# members as methods of that JavaScript class.

```fsharp
module Pricing

open Fable.Core

module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

[<AttachMembers>]
type PricingApi() =
    inherit Runtime.RpcTarget()

    member _.plans() = [| "starter"; "team"; "business" |]

    member _.quote(plan: string, seats: float) =
        let perSeat =
            match plan with
            | "team" -> 8.
            | "business" -> 15.
            | _ -> 0.
        {| plan = plan; seats = seats; monthly = perSeat * seats |}
```

`quote` builds its result as an anonymous record. Fable compiles it to a plain JavaScript object, and Cap'n Web sends plain objects to the client by value.

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { RpcTarget } from "cloudflare:workers";
import { class_type } from "./fable_modules/fable-library-js.5.13.0/Reflection.js";

export class PricingApi extends RpcTarget {
    constructor() {
        super();
    }
    plans() {
        return ["starter", "team", "business"];
    }
    quote(plan, seats) {
        return {
            monthly: ((plan === "team") ? 8 : ((plan === "business") ? 15 : 0)) * seats,
            plan: plan,
            seats: seats,
        };
    }
}
```

</details>

## Pricing Endpoint

A Pages Function serves the API. `newWorkersRpcResponse` returns the response for either kind of Cap'n Web request, an HTTP batch or a WebSocket upgrade. The function creates a new `PricingApi` for each request.

```fsharp
module Workers = FSharp.CloudEdge.Runtime.Workers
module Capnweb = FSharp.CloudEdge.Runtime.Capnweb

let onRequest (context: Workers.EventContext<obj, string, obj>) =
    Capnweb.Exports.newWorkersRpcResponse(context.request, Pricing.PricingApi())
```

<div class="ce-needs"><p><strong>Needs</strong> a Pages project with this module compiled to <code>functions/api.js</code>. Pages serves that file's <code>onRequest</code> export at <code>/api</code>, following its <a href="https://developers.cloudflare.com/pages/functions/routing/">file-based routing</a>.</p></div>

Browser code opens a session against that route with Cap'n Web's `newHttpBatchRpcSession` or `newWebSocketRpcSession`.

:::warning
`newWorkersRpcResponse` accepts cross-origin requests. To serve only your own site, check the request's `Origin` header before the call. According to the Cap'n Web documentation, cross-origin requests should be safe for an API that authorizes callers in-band. Such an API has a method that takes credentials and produces the authorized API.
:::

## Guarded Endpoint

`newWorkersRpcResponse` takes session options as its third argument. Cap'n Web calls `onSendError` with each error that it serializes for a client. This handler writes the error with `console.error` and returns `None`, so Cap'n Web applies its default serialization, which omits the stack trace.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Capnweb = FSharp.CloudEdge.Runtime.Capnweb

let options =
    Capnweb.RpcSessionOptions.Create(
        onSendError = (fun error ->
            JS.console.error error
            None),
        limits = Capnweb.RpcSessionOptions.Limits.Create(maxMessageSize = 65536.)
    )

let onRequest (context: Workers.EventContext<obj, string, obj>) =
    Capnweb.Exports.newWorkersRpcResponse(context.request, Pricing.PricingApi(), options)
```

In this example the maximum incoming message size is 65,536 UTF-16 code units, set through `limits`. The default is 33,554,432. Cap'n Web rejects a larger message and aborts the session.

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { newWorkersRpcResponse } from "capnweb";
import { PricingApi } from "./PricingApi.js";

export const options = {
    onSendError: (error) => {
        console.error(error);
        return undefined;
    },
    limits: {
        maxMessageSize: 65536,
    },
};

export function onRequest(context) {
    return newWorkersRpcResponse(context.request, new PricingApi(), options);
}
```

</details>

## Library Table

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.Capnweb` | `capnweb` 0.12.0 | Sessions, responses, options |
| `Runtime.Workers` | `@cloudflare/workers-types` 5.20260906.1 | `RpcTarget`, Pages Functions context |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/pages/"><strong>Pages Plugins</strong><span>Access and Turnstile</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/"><strong>Workers</strong><span>Request handlers</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/durable-objects/"><strong>Durable Objects</strong><span>Stateful rooms</span></a>
</div>

## NuGet packages

[Runtime.Capnweb 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Capnweb/0.1.0), [Runtime.Workers 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Workers/0.1.0).

See [installation and release availability](../guide/packages.md).
