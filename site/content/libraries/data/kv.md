---
title: Workers KV
description: Store values by key for read-heavy application data.
---

<div class="ce-block-head">
<p class="ce-block-lead">Store values by key for read-heavy application data.</p>
<ul class="ce-facts">
<li><span>Library</span> <code>Runtime.Workers</code></li>
<li><span>npm</span> <code>@cloudflare/workers-types</code> 5.20260906.1</li>
</ul>
</div>

## Data model and setup

Create a KV namespace and attach it to a Worker environment as a `KVNamespace` binding. Choose a key scheme, value format, and expiration policy. JSON values need an application schema even though the service stores values without enforcing one.

KV is eventually consistent across locations. Read the [consistency model](https://developers.cloudflare.com/kv/concepts/how-kv-works/) before using it for data that several writers update concurrently. The shopping-cart example below illustrates serialization; concurrent read-modify-write operations need separate coordination.

## Link Shortener

Workers KV stores a value under a string key. On a `PUT`, this Worker saves the request body as the target URL for that path. On a `GET`, it responds with a redirect to the target. The free plan includes 100,000 KV reads and 1,000 writes a day. A write can take up to 60 seconds to be visible in other locations, according to Cloudflare's [KV documentation](https://developers.cloudflare.com/kv/api/write-key-value-pairs/).

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract LINKS: Workers.KVNamespace<string>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let slug = Workers.Exports.URL(U2.Case1 request.url).pathname.Substring(1)
            match request.``method`` with
            | "PUT" ->
                let! target = request.text() |> Async.AwaitPromise
                do! env.LINKS.put(slug, U4.Case1 target) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| slug = slug; target = target |}
            | _ ->
                let! target =
                    env.LINKS.get(slug, Workers.KVNamespace.Text) |> Async.AwaitPromise
                match target with
                | Some url -> return Workers.Exports.Response.redirect(url, 302.)
                | None ->
                    let notFound = Workers.ResponseInit.Create(status = 404.)
                    return Workers.Exports.Response.Create("No such link", notFound)
        }
        |> Async.StartAsPromise
        |> U2.Case1)
```

<div class="ce-needs"><p><strong>Needs</strong> a KV namespace binding named <code>LINKS</code>. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare the binding.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { substring } from "./fable_modules/fable-library-js.5.13.0/String.js";

export const worker = {
    fetch: (request, env, _arg) => startAsPromise(singleton.Delay(() => {
        const slug = substring((new URL(request.url)).pathname, 1);
        return (request.method === "PUT") ? singleton.Bind(awaitPromise(request.text()), (_arg_1) => {
            const target = _arg_1;
            return singleton.Bind(awaitPromise(env.LINKS.put(slug, target)), () => singleton.Return(globalThis.Response.json({
                slug: slug,
                target: target,
            })));
        }) : singleton.Bind(awaitPromise(env.LINKS.get(slug, "text")), (_arg_3) => {
            const target_1 = _arg_3;
            if (target_1 == null) {
                const notFound = {
                    status: 404,
                };
                return singleton.Return(new globalThis.Response("No such link", notFound));
            }
            else {
                const url = target_1;
                return singleton.Return(globalThis.Response.redirect(url, 302));
            }
        });
    })),
};

export default worker;
```

</details>

## Shopping Cart

`JS.JSON.stringify` serializes the cart for KV, and `get` with `KVNamespace.Json` parses the stored value back into a `Cart`. `expirationTtl` is in seconds, so a cart expires seven days after its last update.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Cart = {| items: string[] |}

type Env =
    abstract CARTS: Workers.KVNamespace<string>

let sevenDays = Workers.KVNamespacePutOptions.Create(expirationTtl = 604800.)

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let cartId = Workers.Exports.URL(U2.Case1 request.url).pathname.Substring(1)
            match request.``method`` with
            | "PUT" ->
                let! cart = request.json<Cart>() |> Async.AwaitPromise
                let json = JS.JSON.stringify cart
                do! env.CARTS.put(cartId, U4.Case1 json, sevenDays) |> Async.AwaitPromise
                return Workers.Exports.Response.json cart
            | _ ->
                let! saved =
                    env.CARTS.get<Cart>(cartId, Workers.KVNamespace.Json)
                    |> Async.AwaitPromise
                let cart = saved |> Option.defaultValue {| items = [||] |}
                return Workers.Exports.Response.json cart
        }
        |> Async.StartAsPromise
        |> U2.Case1)
```

<div class="ce-needs"><p><strong>Needs</strong> a <code>CARTS</code> binding to a KV namespace.</p></div>


## Help verify the binding

Useful checks include missing keys, JSON decoding, expiration, pagination, and visibility from different locations. A compiling consumer does not establish hosted behavior. Record the package version, configuration, and observed result using the [verification guide](../../guide/verify-bindings.md).

## Related Pages

- [Data & Analytics](index.md)
- [Account setup](../control-plane/account-setup.md)
- [Worker bindings](../control-plane/worker-upload.md)
