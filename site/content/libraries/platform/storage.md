---
title: Storage
description: Key-value data in KV, files in R2 and SQL in D1.
order: 3
---

<div class="ce-block-head">
<p class="ce-block-lead">Save data from your Worker in Cloudflare's storage services. Each service is a binding in the Worker's environment, and <code>Runtime.Workers</code> declares F# types for its methods and results.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Workers</code></li>
<li><span>npm</span> <code>@cloudflare/workers-types</code> 5.20260906.1</li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/workers/platform/pricing/">1 GB in KV, 10 GB-month in R2, 5 GB in D1</a></li>
</ul>
</div>

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

## File Drop

On a `PUT`, this Worker saves the request body as a file in R2, with HTTP metadata such as `Content-Type`. On a `GET`, `writeHttpMetadata` copies that metadata into the response headers. Uploads count toward the free plan's 1 million Class A operations a month, and downloads toward its 10 million Class B operations. [Egress from R2](https://developers.cloudflare.com/r2/pricing/) is free.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract FILES: Workers.R2Bucket

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let key = Workers.Exports.URL(U2.Case1 request.url).pathname.Substring(1)
            match request.``method`` with
            | "PUT" ->
                let! bytes = request.arrayBuffer() |> Async.AwaitPromise
                let metadata = U2.Case1 request.headers
                let options = Workers.R2PutOptions.Create(httpMetadata = metadata)
                let! stored =
                    env.FILES.put(key, U5.Case2 bytes, options) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| key = key; size = stored.size |}
            | _ ->
                let! found = env.FILES.get key |> Async.AwaitPromise
                match found with
                | Some file ->
                    let headers = Workers.Exports.Headers()
                    file.writeHttpMetadata headers
                    headers.set("etag", file.httpEtag)
                    let init = Workers.ResponseInit.Create(headers = headers)
                    return Workers.Exports.Response.Create(file.body, init)
                | None ->
                    let notFound = Workers.ResponseInit.Create(status = 404.)
                    return Workers.Exports.Response.Create("Not found", notFound)
        }
        |> Async.StartAsPromise
        |> U2.Case1)
```

<div class="ce-needs"><p><strong>Needs</strong> an R2 bucket binding named <code>FILES</code>.</p></div>

## Guestbook

D1 is a SQL database. `prepare` and `bind` build a statement with `?` placeholders. `run` executes the insert, and `all` returns the rows as `Entry` records. The daily D1 allowance on the free plan is 5 million rows read and 100,000 rows written.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Entry = {| name: string; message: string |}

type Env =
    abstract DB: Workers.D1Database

let insertSql = "INSERT INTO entries (name, message) VALUES (?, ?)"
let latestSql = "SELECT name, message FROM entries ORDER BY id DESC LIMIT 20"

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            match request.``method`` with
            | "POST" ->
                let! entry = request.json<Entry>() |> Async.AwaitPromise
                let insert = env.DB.prepare(insertSql).bind(entry.name, entry.message)
                let! result = insert.run() |> Async.AwaitPromise
                return Workers.Exports.Response.json {| id = result.meta.last_row_id |}
            | _ ->
                let! result = env.DB.prepare(latestSql).all<Entry>() |> Async.AwaitPromise
                return Workers.Exports.Response.json result.results
        }
        |> Async.StartAsPromise
        |> U2.Case1)
```

<div class="ce-needs"><p><strong>Needs</strong> a <code>DB</code> binding to a D1 database, with the <code>entries</code> table the queries use.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";

export const insertSql = "INSERT INTO entries (name, message) VALUES (?, ?)";

export const latestSql = "SELECT name, message FROM entries ORDER BY id DESC LIMIT 20";

export const worker = {
    fetch: (request, env, _arg) => startAsPromise(singleton.Delay(() => ((request.method === "POST") ? singleton.Bind(awaitPromise(request.json()), (_arg_1) => {
        const entry = _arg_1;
        const insert = env.DB.prepare(insertSql).bind(entry.name, entry.message);
        return singleton.Bind(awaitPromise(insert.run()), (_arg_2) => singleton.Return(globalThis.Response.json({
            id: _arg_2.meta.last_row_id,
        })));
    }) : singleton.Bind(awaitPromise(env.DB.prepare(latestSql).all()), (_arg_3) => singleton.Return(globalThis.Response.json(_arg_3.results)))))),
};

export default worker;
```

</details>

## Order Checkout

`batch` sends an order and its items to D1 in one call. Cloudflare's [D1 documentation](https://developers.cloudflare.com/d1/worker-api/d1-database/) defines batched statements as SQL transactions, so if one insert fails, D1 rolls back the order and all of its items. The item statement is prepared once and bound once per item.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Order = {| email: string; items: {| sku: string; quantity: float |}[] |}

type Env =
    abstract DB: Workers.D1Database

let orderSql = "INSERT INTO orders (id, email) VALUES (?, ?)"
let itemSql = "INSERT INTO order_items (order_id, sku, quantity) VALUES (?, ?, ?)"

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let! order = request.json<Order>() |> Async.AwaitPromise
            let orderId = Workers.Exports.crypto.randomUUID()
            let addItem = env.DB.prepare itemSql
            let statements =
                [| env.DB.prepare(orderSql).bind(orderId, order.email)
                   for item in order.items do
                       addItem.bind(orderId, item.sku, item.quantity) |]
            let! _ = env.DB.batch statements |> Async.AwaitPromise
            return Workers.Exports.Response.json {| orderId = orderId |}
        }
        |> Async.StartAsPromise
        |> U2.Case1)
```

<div class="ce-needs"><p><strong>Needs</strong> the same <code>DB</code> binding, with <code>orders</code> and <code>order_items</code> tables.</p></div>

## Library Table

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.Workers` | `@cloudflare/workers-types` 5.20260906.1 | KV, R2 and D1 binding types |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/"><strong>Workers</strong><span>Handlers and responses</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/durable-objects/"><strong>Durable Objects</strong><span>Storage per room</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/account-setup/"><strong>Account Setup</strong><span>Create databases and buckets</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Upload with bindings</span></a>
</div>
