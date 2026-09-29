---
title: Services
description: Containers, Actors, OAuth, asset serving and validation.
order: 9
---

<div class="ce-block-head">
<p class="ce-block-lead">An F# Worker can send heavy jobs to a container and require an OAuth 2.1 token for its API. Other libraries in this block serve a single-page app from KV and check incoming JSON against a schema.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Containers</code> <code>Runtime.Actors</code> <code>Runtime.DynamicWorkflows</code> <code>Runtime.WorkersOauthProvider</code> <code>Runtime.KvAssetHandler</code> <code>Runtime.Cabidela</code> <code>Runtime.Chanfana</code></li>
<li><span>npm</span> six <code>@cloudflare/</code> packages and <code>chanfana</code></li>
<li><span>Free plan</span> 100,000 Worker requests a day</li>
<li><span>Containers</span> <a href="https://developers.cloudflare.com/containers/">Workers Paid plan</a></li>
</ul>
</div>

## Image Resizer

The Worker posts each upload to a container and returns the container's response. `switchPort` copies the request and sets its target port to 8080. `getFetchByName`, from the [Support Libraries](support.md), produces a fetch stub for the container's Durable Object.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Containers = FSharp.CloudEdge.Runtime.Containers
module Transport = FSharp.CloudEdge.Support.Workers.DurableObjects

type Env =
    abstract RESIZER: Workers.DurableObjectNamespace<Containers.Container<obj>>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            let resize =
                Workers.Exports.Request.Create(
                    U3.Case1 "http://resizer/thumbnail?width=320",
                    Workers.RequestInit.Create(``method`` = "POST", body = request.body))
            let resizer = Transport.getFetchByName env.RESIZER "resizer"
            resizer.fetch (Containers.Exports.switchPort(resize, 8080.)) |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> the Workers Paid plan and a Durable Object binding named <code>RESIZER</code> whose class extends <code>Container</code>. F# code cannot subclass <code>Container</code> in 0.1.0, so that class is written in JavaScript. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how an F# program declares a Worker's bindings.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { getFetchByName } from "./src/Support/FSharp.CloudEdge.Support.Workers/DurableObjects.js";
import { switchPort } from "@cloudflare/containers";

export const worker = {
    fetch: (request, env, _arg) => {
        const resize = new globalThis.Request("http://resizer/thumbnail?width=320", ({
            method: "POST",
            body: request.body,
        }));
        const resizer = getFetchByName(env.RESIZER, "resizer");
        return resizer.fetch(switchPort(resize, 8080));
    },
};

export default worker;
```

</details>

## Leaderboard

The leaderboard stores scores in SQLite and lists the top ten. `Actor` cannot be subclassed from F# in 0.1.0, so the leaderboard inherits the Workers `DurableObject` class. The Actors `Storage` helper applies each numbered migration once.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Actors = FSharp.CloudEdge.Runtime.Actors
module ActorStorage = FSharp.CloudEdge.Runtime.Actors.Storage

type Score = {| player: string; points: float |}

type Leaderboard(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    let schema = ActorStorage.Exports.Storage ctx.storage

    do schema.migrations <- [|
        Actors.SQLSchemaMigration.Create(1., "Create scores", "CREATE TABLE IF NOT EXISTS scores (player TEXT, points REAL)")
        Actors.SQLSchemaMigration.Create(2., "Index points", "CREATE INDEX IF NOT EXISTS by_points ON scores (points)")
    |]

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch request =
            async {
                let! _ = schema.runMigrations() |> Async.AwaitPromise
                if request.``method`` = "POST" then
                    let! score = request.json<Score>() |> Async.AwaitPromise
                    ctx.storage.sql.exec("INSERT INTO scores VALUES (?, ?)", score.player, score.points) |> ignore
                let top = ctx.storage.sql.exec<Score>("SELECT player, points FROM scores ORDER BY points DESC LIMIT 10")
                return Workers.Exports.Response.json (top.toArray())
            }
            |> Async.StartAsPromise
            |> U2.Case1
```

<div class="ce-needs"><p><strong>Needs</strong> a Durable Object binding for <code>Leaderboard</code> with the <a href="https://developers.cloudflare.com/durable-objects/api/sqlite-storage-api/">SQLite storage backend</a>, which <code>ctx.storage.sql</code> requires.</p></div>

:::info
`@cloudflare/actors` 0.0.1-beta.6 is a beta release, and its README states that the project is in active development.
:::

## Account API

`OAuthProvider` publishes its OAuth metadata and implements the token and client registration endpoints. It checks the bearer token on every request under `/api/` before it calls `account`, and `home` answers all other paths. `ctx.props` holds the `props` passed to `completeAuthorization` at sign-in.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module OAuth = FSharp.CloudEdge.Runtime.WorkersOauthProvider

type User = {| userId: string |}

let account: OAuth.OAuthProviderOptions.ApiHandler<obj> =
    OAuth.OAuthProviderOptions.ApiHandler.Create(
        fetch = fun (_: obj) (_: obj) (ctx: Workers.ExecutionContext<User>) ->
            Workers.Exports.Response.json {| userId = ctx.props.userId |})

let home: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun _ _ _ -> U2.Case2(Workers.Exports.Response.Create("Sign in to continue")))

[<ExportDefault>]
let provider =
    OAuth.Exports.OAuthProvider(
        OAuth.OAuthProviderOptions.Create(
            defaultHandler = home,
            authorizeEndpoint = "/authorize",
            tokenEndpoint = "/oauth/token",
            clientRegistrationEndpoint = "/oauth/register",
            apiRoute = U2.Case1 "/api/",
            apiHandler = U2.Case2 account))
```

<div class="ce-needs"><p><strong>Needs</strong> a KV namespace bound as <code>OAUTH_KV</code>, where the provider stores grants and tokens.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { OAuthProvider } from "@cloudflare/workers-oauth-provider";

export const account = {
    fetch: (_arg, _arg_1, ctx) => globalThis.Response.json({
        userId: ctx.props.userId,
    }),
};

export const home = {
    fetch: (_arg, _arg_1, _arg_2) => (new globalThis.Response("Sign in to continue")),
};

export const provider = new OAuthProvider({
    defaultHandler: home,
    authorizeEndpoint: "/authorize",
    tokenEndpoint: "/oauth/token",
    apiRoute: "/api/",
    apiHandler: account,
    clientRegistrationEndpoint: "/oauth/register",
});

export default provider;
```

</details>

## Consent Page

Set `consent` as the provider's `defaultHandler` in place of `home`. The provider adds `OAUTH_PROVIDER` to `env`. `parseAuthRequest` validates the client and its redirect URI. After your own sign-in and consent steps, `completeAuthorization` stores the grant and returns the `redirectTo` URL.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module OAuth = FSharp.CloudEdge.Runtime.WorkersOauthProvider

type Env =
    abstract OAUTH_PROVIDER: OAuth.OAuthHelpers

let consent: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let url = Workers.Exports.URL(U2.Case1 request.url)
                if url.pathname <> "/authorize" then
                    return Workers.Exports.Response.Create("Not found", Workers.ResponseInit.Create(status = 404.))
                else
                    let! authRequest = env.OAUTH_PROVIDER.parseAuthRequest request |> Async.AwaitPromise
                    let! client = env.OAUTH_PROVIDER.lookupClient authRequest.clientId |> Async.AwaitPromise
                    let name = client |> Option.bind (fun c -> c.clientName) |> Option.defaultValue authRequest.clientId
                    let scopes = String.concat ", " authRequest.scope
                    return Workers.Exports.Response.Create($"{name} asks for: {scopes}")
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

## Single-Page App

The Worker serves a built front end from Workers KV. `serveSinglePageApp` maps page routes such as `/settings` to `index.html`, so the front end's own router handles them. In 0.1.0 the result of `getAssetFromKV` is typed `obj`, so this Worker is a plain record whose `fetch` returns that result unchanged.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Assets = FSharp.CloudEdge.Runtime.KvAssetHandler

type Env =
    abstract SITE: Workers.KVNamespace<string>

let options (env: Env) =
    Assets.Options.MapRequestToAsset.Options.Create(
        ASSET_NAMESPACE = env.SITE,
        mapRequestToAsset = Assets.Options.MapRequestToAsset(fun request _ -> Assets.Exports.serveSinglePageApp request))

[<ExportDefault>]
let worker =
    {| fetch = fun (request: obj) (env: Env) (ctx: Workers.ExecutionContext<obj>) ->
        let lookup = Assets.GetAssetFromKV.Event.Create(request, fun work -> ctx.waitUntil work)
        Assets.Exports.getAssetFromKV(lookup, options env) |}
```

<div class="ce-needs"><p><strong>Needs</strong> a KV namespace bound as <code>SITE</code> that holds each file under its path without the leading slash, such as <code>index.html</code> or <code>assets/app.js</code>.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { getAssetFromKV, serveSinglePageApp } from "@cloudflare/kv-asset-handler";

export function options(env) {
    return {
        ASSET_NAMESPACE: env.SITE,
        mapRequestToAsset: (request, _arg) => serveSinglePageApp(request),
    };
}

export const worker = {
    fetch: (request, env, ctx) => getAssetFromKV({
        request: request,
        waitUntil: (work) => {
            ctx.waitUntil(work);
        },
    }, options(env)),
};

export default worker;
```

</details>

## Signup Check

The schema is an anonymous record, which Fable emits as a plain JSON Schema object. With `applyDefaults`, Cabidela sets `plan` to `free` on a submission that has none. The Worker answers invalid JSON or a failed `validate` with status 400 and the error message.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Cabidela = FSharp.CloudEdge.Runtime.Cabidela

let schema =
    {| ``type`` = "object"
       required = [| "email" |]
       properties =
        {| email = {| ``type`` = "string"; minLength = 3 |}
           plan = {| ``type`` = "string"; enum = [| "free"; "team" |]; ``default`` = "free" |} |} |}

let signup = Cabidela.Exports.Cabidela(schema, Cabidela.CabidelaOptions.Create(applyDefaults = true))

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request _ _ ->
            async {
                try
                    let! form = request.json<obj>() |> Async.AwaitPromise
                    signup.validate form |> ignore
                    return Workers.Exports.Response.json form
                with error ->
                    let status = Workers.ResponseInit.Create(status = 400.)
                    return Workers.Exports.Response.json({| error = error.Message |}, U2.Case2 status)
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
export const schema = {
    properties: {
        email: {
            minLength: 3,
            type: "string",
        },
        plan: {
            default: "free",
            enum: ["free", "team"],
            type: "string",
        },
    },
    required: ["email"],
    type: "object",
};

export const signup = new Cabidela(schema, ({
    applyDefaults: true,
}));
```

</details>

## API Reference

`getSwaggerUI` and `getReDocUI` each build a standalone HTML page. That HTML loads its viewer from jsDelivr and fetches the OpenAPI document at the URL you pass. This Worker serves the document itself at `/openapi.json` and ReDoc at `/redoc`. Swagger UI is the response for every other path.

Chanfana's routes are `OpenAPIRoute` subclasses on a Hono or itty-router app. In 0.1.0 `OpenAPIRoute` is an interface that F# code cannot subclass, and neither router is bound. Neither is required for the documentation pages.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Chanfana = FSharp.CloudEdge.Runtime.Chanfana

let listTasks = {| summary = "List tasks"; responses = {| ``200`` = {| description = "The task list" |} |} |}

let spec =
    {| openapi = "3.1.0"
       info = {| title = "Tasks API"; version = "1.0.0" |}
       paths = {| ``/tasks`` = {| get = listTasks |} |} |}

let html = Workers.ResponseInit.Create(headers = {| ``content-type`` = "text/html; charset=utf-8" |})

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request _ _ ->
            match Workers.Exports.URL(U2.Case1 request.url).pathname with
            | "/openapi.json" -> U2.Case2(Workers.Exports.Response.json spec)
            | "/redoc" -> U2.Case2(Workers.Exports.Response.Create(Chanfana.Exports.getReDocUI "/openapi.json", html))
            | _ -> U2.Case2(Workers.Exports.Response.Create(Chanfana.Exports.getSwaggerUI "/openapi.json", html))
    )
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { getSwaggerUI, getReDocUI } from "chanfana";

export const listTasks = {
    responses: {
        "200": {
            description: "The task list",
        },
    },
    summary: "List tasks",
};

export const spec = {
    info: {
        title: "Tasks API",
        version: "1.0.0",
    },
    openapi: "3.1.0",
    paths: {
        "/tasks": {
            get: listTasks,
        },
    },
};

export const html = {
    headers: {
        "content-type": "text/html; charset=utf-8",
    },
};

export const worker = {
    fetch: (request, _arg, _arg_1) => {
        const matchValue = (new URL(request.url)).pathname;
        return (matchValue === "/openapi.json") ? globalThis.Response.json(spec) : ((matchValue === "/redoc") ? (new globalThis.Response(getReDocUI("/openapi.json"), html)) : (new globalThis.Response(getSwaggerUI("/openapi.json"), html)));
    },
};

export default worker;
```

</details>

## Library Table

| Library | npm package and version | What it covers |
| --- | --- | --- |
| `Runtime.Containers` | `@cloudflare/containers` 0.3.7 | Container-backed Durable Objects |
| `Runtime.Actors` | `@cloudflare/actors` 0.0.1-beta.6 | Durable Object helpers |
| `Runtime.DynamicWorkflows` | `@cloudflare/dynamic-workflows` 0.1.1 | Workflows per tenant |
| `Runtime.WorkersOauthProvider` | `@cloudflare/workers-oauth-provider` 0.10.3 | OAuth 2.1 provider |
| `Runtime.KvAssetHandler` | `@cloudflare/kv-asset-handler` 0.5.0 | Static files from KV |
| `Runtime.Cabidela` | `@cloudflare/cabidela` 0.2.4 | JSON Schema validation |
| `Runtime.Chanfana` | `chanfana` 3.4.0 | OpenAPI schemas and docs |

Using Dynamic Workflows from F# requires a cast in 0.1.0, so the library has no example here. A Worker Loader stub's `getEntrypoint` is typed `obj`, and `createDynamicWorkflowEntrypoint` takes a loader that must produce a typed `WorkflowRunner`.

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/durable-objects/"><strong>Durable Objects</strong><span>Rooms with storage and alarms</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/storage/"><strong>Storage</strong><span>KV, R2 and D1</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/support/"><strong>Support Libraries</strong><span>Workers helpers</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/compute/"><strong>Compute</strong><span>Sandboxes for running code</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>A Worker and its bindings</span></a>
</div>
