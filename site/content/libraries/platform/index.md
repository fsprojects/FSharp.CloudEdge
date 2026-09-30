---
title: Workers
description: Answer requests at the edge with F# handlers.
order: 2
---

<div class="ce-block-head">
<p class="ce-block-lead">Serve an API from F#, or place F# code in front of an existing site to rewrite and cache its pages. A Worker is a module that exports a handler with a <code>fetch</code> function, and Fable compiles that handler to the default export of a JavaScript module.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Workers</code></li>
<li><span>npm</span> <code>@cloudflare/workers-types</code> 5.20260906.1</li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/workers/platform/pricing/">100,000 requests a day</a></li>
</ul>
</div>

## Health Check

The smallest Worker responds to every request with `ok`. `[<ExportDefault>]` marks the handler as the module's default export, and `U2.Case2` wraps a finished response. Here all four type arguments of `ExportedHandler` are `obj`, the F# type for any value.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env ctx ->
        U2.Case2(Workers.Exports.Response.Create("ok")))
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
export const worker = {
    fetch: (request, env, ctx) => (new globalThis.Response("ok")),
};

export default worker;
```

</details>

## API Router

The router matches the request method and path together. `Response.json` serializes an anonymous record, and `ResponseInit` sets the status code of each error response.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

let jsonWithStatus (status: float) body =
    let init = Workers.ResponseInit.Create(status = status)
    Workers.Exports.Response.json(body, U2.Case2 init)

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request _ _ ->
        let url = Workers.Exports.URL(U2.Case1 request.url)
        let response =
            match request.``method``, url.pathname with
            | "GET", "/" -> Workers.Exports.Response.Create("Welcome to the API")
            | "GET", "/api/greeting" ->
                let name = url.searchParams.get "name" |> Option.defaultValue "world"
                Workers.Exports.Response.json {| greeting = $"Hello, {name}" |}
            | _, "/api/greeting" -> jsonWithStatus 405. {| error = "Use GET" |}
            | _ -> jsonWithStatus 404. {| error = "Not found" |}
        U2.Case2 response)
```

## Visitor Greeting

`request.cf` holds properties that Cloudflare's network provides about each incoming request, such as its city. Both `request.cf` and its `city` property are F# options. Cloudflare's [Request documentation](https://developers.cloudflare.com/workers/runtime-apis/request/) states that `request.cf` is unavailable in the Workers dashboard and the Playground preview editor.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request _ _ ->
        let city =
            request.cf
            |> Option.bind (fun cf -> cf.city)
            |> Option.defaultValue "your city"
        U2.Case2(Workers.Exports.Response.Create($"Hello to everyone in {city}")))
```

## API Proxy

The handler's second argument is the Worker's environment. Declare its type as an F# interface such as `Env`, and pass that interface as the first type argument of `ExportedHandler`. This proxy sends a `GET` to `UPSTREAM_URL` with the path and query string of each request. That `GET` has an `Authorization` header with the `API_KEY` secret, so only the upstream API receives the key. `fetch` returns a promise, which the handler wraps in `U2.Case1`.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract UPSTREAM_URL: string
    abstract API_KEY: string

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        let url = Workers.Exports.URL(U2.Case1 request.url)
        let upstream = env.UPSTREAM_URL + url.pathname + url.search
        let auth = {| Authorization = $"Bearer {env.API_KEY}" |}
        let init = Workers.RequestInit.Create(headers = auth)
        Workers.Exports.fetch(U3.Case1 upstream, init) |> U2.Case1)
```

<div class="ce-needs"><p><strong>Needs</strong> a secret named <code>API_KEY</code> and an environment variable named <code>UPSTREAM_URL</code>. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare these bindings.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { concat } from "./fable_modules/fable-library-js.5.13.0/String.js";

export const worker = {
    fetch: (request, env, _arg) => {
        const url = new URL(request.url);
        const upstream = (env.UPSTREAM_URL + url.pathname) + url.search;
        const auth = {
            Authorization: concat("Bearer ", env.API_KEY),
        };
        return fetch(upstream, {
            headers: auth,
        });
    },
};

export default worker;
```

</details>

## Announcement Banner

[`HTMLRewriter`](https://developers.cloudflare.com/workers/runtime-apis/html-rewriter/) runs a handler on each element that matches a selector. This Worker fetches the requested page from `ORIGIN` and adds a banner at the start of its `<body>`. With `html = true`, `prepend` inserts the banner as HTML instead of escaped text. The element handler is synchronous, so it returns `None` in place of a promise.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract ORIGIN: string

let banner = """<p class="banner">Orders ship free this week.</p>"""
let asHtml = Workers.ContentOptions.Create(html = true)

let addBanner =
    Workers.HTMLRewriterElementContentHandlers.Create(element = fun body ->
        body.prepend(U3.Case1 banner, asHtml) |> ignore
        None)

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let url = Workers.Exports.URL(U2.Case1 request.url)
            let pageUrl = env.ORIGIN + url.pathname + url.search
            let! page = Workers.Exports.fetch(U3.Case1 pageUrl) |> Async.AwaitPromise
            return Workers.Exports.HTMLRewriter().on("body", addBanner).transform page
        }
        |> Async.StartAsPromise
        |> U2.Case1)
```

<div class="ce-needs"><p><strong>Needs</strong> an environment variable named <code>ORIGIN</code> that holds your site's address, such as <code>https://www.example.com</code>.</p></div>

## Page Cache

`caches.default` is the global cache of the Cache API. This Worker returns the cached copy of a page when `match` finds one. On a miss it fetches the page from `ORIGIN` and stores a copy with `put`. According to Cloudflare's [Cache documentation](https://developers.cloudflare.com/workers/runtime-apis/cache/), each data center has its own cache contents.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract ORIGIN: string

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let cache = Workers.Exports.caches.``default``
            let! cached = cache.``match``(U3.Case1 request.url) |> Async.AwaitPromise
            match cached with
            | Some response -> return response
            | None ->
                let url = Workers.Exports.URL(U2.Case1 request.url)
                let pageUrl = env.ORIGIN + url.pathname + url.search
                let! page = Workers.Exports.fetch(U3.Case1 pageUrl) |> Async.AwaitPromise
                do! cache.put(U3.Case1 request.url, page.clone()) |> Async.AwaitPromise
                return page
        }
        |> Async.StartAsPromise
        |> U2.Case1)
```

<div class="ce-needs"><p><strong>Needs</strong> the same <code>ORIGIN</code> environment variable as Announcement Banner.</p></div>

## Library Table

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.Workers` | `@cloudflare/workers-types` 5.20260906.1 | Runtime API and binding types |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/data/"><strong>Data &amp; Analytics</strong><span>D1, R2, KV, Vectorize and Analytics Engine</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/durable-objects/"><strong>Durable Objects</strong><span>Stateful rooms</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/background-work/"><strong>Background Work</strong><span>Queues and Workflows</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Upload with bindings</span></a>
</div>

## NuGet packages

[Runtime.Workers 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Workers/0.1.0).

See [installation and release availability](../../guide/packages.md).
