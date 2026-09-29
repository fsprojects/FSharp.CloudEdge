---
title: Durable Objects
description: Stateful rooms with storage, WebSockets and alarms.
order: 4
---

<div class="ce-block-head">
<p class="ce-block-lead">Give each chat room or game its own Durable Object: one instance per name, with private storage and live WebSocket connections. Cloudflare routes every request for that name to the same instance, and its storage is strongly consistent.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Workers</code> <code>Support.Workers</code></li>
<li><span>npm</span> <code>@cloudflare/workers-types</code> 5.20260906.1</li>
<li><span>Free plan</span> 100,000 requests a day, 5 GB stored</li>
</ul>
</div>

## Chat Room

Each room is one `ChatRoom` object, and its `webSocketMessage` handler sends every text message to all sockets in the room. Sockets accepted with `acceptWebSocket`, part of the [WebSocket Hibernation API](https://developers.cloudflare.com/durable-objects/best-practices/websockets/), stay open while Cloudflare evicts an idle room from memory. Cloudflare bills duration only for the time a room is in memory.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

type ChatRoom(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch request =
            if request.headers.get "Upgrade" = Some "websocket" then
                let pair = Workers.Exports.WebSocketPair.Create()
                let client = pair.``0``
                let server = pair.``1``
                ctx.acceptWebSocket server
                let upgrade = Workers.ResponseInit.Create(status = 101., webSocket = client)
                Workers.Exports.Response.Create(init = upgrade) |> U2.Case2
            else
                Workers.Exports.Response.Create("Expected a WebSocket", Workers.ResponseInit.Create(status = 426.))
                |> U2.Case2

    interface Runtime.DurableObject.IWebSocketMessageHandler with
        member _.webSocketMessage(_, message) =
            match message with
            | U2.Case1 text ->
                for socket in ctx.getWebSockets() do
                    socket.send (U3.Case1 text)
            | _ -> ()
            None

    interface Runtime.DurableObject.IWebSocketCloseHandler with
        member _.webSocketClose(socket, code, reason, _) =
            socket.close(code, reason)
            None
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { equals } from "./fable_modules/fable-library-js.5.13.0/Util.js";
import { item } from "./fable_modules/fable-library-js.5.13.0/Array.js";
import { WorkerEntrypoint, DurableObject } from "cloudflare:workers";
import { class_type, obj_type } from "./fable_modules/fable-library-js.5.13.0/Reflection.js";
import { getFetchByName } from "./src/Support/FSharp.CloudEdge.Support.Workers/DurableObjects.js";

export class ChatRoom extends DurableObject {
    constructor(ctx, env) {
        super(ctx, env);
        this.ctx = ctx;
    }
    fetch(request) {
        const _ = this;
        if (equals(request.headers.get("Upgrade"), "websocket")) {
            const pair = new WebSocketPair();
            const client = pair["0"];
            const server = pair["1"];
            _.ctx.acceptWebSocket(server);
            const upgrade = {
                status: 101,
                webSocket: client,
            };
            return new globalThis.Response(undefined, upgrade);
        }
        else {
            return new globalThis.Response("Expected a WebSocket", ({
                status: 426,
            }));
        }
    }
    webSocketMessage(_arg, message) {
        const _ = this;
        if (typeof message === "string") {
            const arr = _.ctx.getWebSockets();
            for (let idx = 0; idx <= (arr.length - 1); idx++) {
                const socket = item(idx, arr);
                socket.send(message);
            }
        }
        return undefined;
    }
    webSocketClose(socket, code, reason, _arg) {
        socket.close(code, reason);
        return undefined;
    }
}
```

</details>

## Room Lookup

Clients connect to a room through this Worker, which uses the URL path as the room name. `getFetchByName` from `Support.Workers` returns a `FetchTransport` for the matching `ChatRoom` object, so everyone who opens `/general` joins the same room. Put `RoomLookup` in the same file as `ChatRoom`, and the compiled module exports both classes.

```fsharp
open Fable.Core.JsInterop

module Transport = FSharp.CloudEdge.Support.Workers.DurableObjects

type Env =
    abstract ROOMS: Workers.DurableObjectNamespace<obj>

type RoomLookup(ctx: Workers.ExecutionContext<obj>, env: Env) =
    inherit Runtime.WorkerEntrypoint<Env, obj>(ctx, env)

    interface Runtime.WorkerEntrypoint.IFetchHandler with
        member _.fetch request =
            let room = Workers.Exports.URL(U2.Case1 request.url).pathname
            (Transport.getFetchByName env.ROOMS room).fetch request |> U2.Case1

exportDefault jsConstructor<RoomLookup>
```

The Worker is a `WorkerEntrypoint` class. Its `fetch` receives the request as the type that `FetchTransport.fetch` takes, so the Worker forwards it as it arrived. `exportDefault` makes `RoomLookup` the Worker's default export.

<div class="ce-needs"><p><strong>Needs</strong> a Durable Object binding named <code>ROOMS</code> for the <code>ChatRoom</code> class, and a migration that lists <code>ChatRoom</code> in <code>new_sqlite_classes</code>. Durable Objects on the free plan use the SQLite storage backend. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare the binding and the migration.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
export class RoomLookup extends WorkerEntrypoint {
    constructor(ctx, env) {
        super(ctx, env);
        this.env = env;
    }
    fetch(request) {
        const _ = this;
        const room = (new URL(request.url)).pathname;
        return getFetchByName(_.env.ROOMS, room).fetch(request);
    }
}
```

</details>

## Invite Links

For rooms with unguessable addresses, use this Worker instead of `RoomLookup`. With a `room` parameter, `idFromString` parses the ID and `getFetchById` returns a `FetchTransport` for that room. Otherwise the Worker creates an ID with `newUniqueId` and answers with it as JSON.

```fsharp
open Fable.Core
open Fable.Core.JsInterop

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Transport = FSharp.CloudEdge.Support.Workers.DurableObjects

type Env =
    abstract ROOMS: Workers.DurableObjectNamespace<obj>

type InviteLinks(ctx: Workers.ExecutionContext<obj>, env: Env) =
    inherit Runtime.WorkerEntrypoint<Env, obj>(ctx, env)

    interface Runtime.WorkerEntrypoint.IFetchHandler with
        member _.fetch request =
            match Workers.Exports.URL(U2.Case1 request.url).searchParams.get "room" with
            | Some room ->
                let id = env.ROOMS.idFromString room
                (Transport.getFetchById env.ROOMS id).fetch request |> U2.Case1
            | None ->
                let id = env.ROOMS.newUniqueId()
                Workers.Exports.Response.json {| room = id.toString() |} |> U2.Case2

exportDefault jsConstructor<InviteLinks>
```

## Seat Booking

A `SeatMap` object holds the bookings for one show. `storage.transaction` runs the seat check and both writes as one transaction that either commits or aborts. The object answers a second booking for a taken seat with 409.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

type Booking = {| seat: string; email: string |}

type SeatMap(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    let book (booking: Booking) (tx: Workers.DurableObjectTransaction) =
        async {
            let! holder = tx.get<string>("seat:" + booking.seat) |> Async.AwaitPromise
            if holder.IsSome then
                return false
            else
                let! sold = tx.get<float>("sold") |> Async.AwaitPromise
                do! tx.put("seat:" + booking.seat, booking.email) |> Async.AwaitPromise
                do! tx.put("sold", Option.defaultValue 0. sold + 1.) |> Async.AwaitPromise
                return true
        }
        |> Async.StartAsPromise

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch request =
            async {
                let! booking = request.json<Booking>() |> Async.AwaitPromise
                let! booked = ctx.storage.transaction(book booking) |> Async.AwaitPromise
                let status = if booked then 201. else 409.
                return Workers.Exports.Response.json({| seat = booking.seat; booked = booked |}, U2.Case2(Workers.ResponseInit.Create(status = status)))
            }
            |> Async.StartAsPromise
            |> U2.Case1
```

## Rate Limiter

A `RateLimiter` object allows one API key 100 requests a minute. On the first request of a window it sets an [alarm](https://developers.cloudflare.com/durable-objects/api/alarms/) for 60 seconds later, and the `alarm` handler deletes the count. Call `getFetchByName` with the caller's key to get the limiter for that key.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

type RateLimiter(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch _ =
            async {
                let! used = ctx.storage.get<float>("used") |> Async.AwaitPromise
                let used = Option.defaultValue 0. used + 1.
                if used = 1. then
                    do! ctx.storage.setAlarm(U2.Case1(JS.Constructors.Date.now () + 60_000.)) |> Async.AwaitPromise
                do! ctx.storage.put("used", used) |> Async.AwaitPromise
                let status = if used > 100. then 429. else 200.
                return Workers.Exports.Response.json({| used = used; limit = 100 |}, U2.Case2(Workers.ResponseInit.Create(status = status)))
            }
            |> Async.StartAsPromise
            |> U2.Case1

    interface Runtime.DurableObject.IAlarmHandler with
        member _.alarm _ = Some(ctx.storage.deleteAll ())
```

## Leaderboard

One `Leaderboard` object per game stores scores in its own SQLite database. The [SQL API](https://developers.cloudflare.com/durable-objects/api/sqlite-storage-api/) is synchronous, and `toArray` reads the rows of a cursor as `Score` records.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

type Score = {| player: string; points: float |}

type Leaderboard(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    do ctx.storage.sql.exec("CREATE TABLE IF NOT EXISTS scores (player TEXT PRIMARY KEY, points REAL)") |> ignore

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch request =
            async {
                if request.``method`` = "POST" then
                    let! score = request.json<Score>() |> Async.AwaitPromise
                    ctx.storage.sql.exec(
                        "INSERT INTO scores VALUES (?, ?) ON CONFLICT (player) DO UPDATE SET points = MAX(points, excluded.points)",
                        score.player, score.points) |> ignore
                let top = ctx.storage.sql.exec<Score>("SELECT player, points FROM scores ORDER BY points DESC LIMIT 10").toArray()
                return Workers.Exports.Response.json top
            }
            |> Async.StartAsPromise
            |> U2.Case1
```

<div class="ce-needs"><p><strong>Needs</strong> the <code>Leaderboard</code> class in a <code>new_sqlite_classes</code> migration, as for the chat room.</p></div>

## Library Table

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.Workers` | `@cloudflare/workers-types` 5.20260906.1 | Durable Object classes, storage and WebSockets |
| `Support.Workers` | Hand-written, no npm package | Typed `fetch` for Durable Object stubs |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/"><strong>Workers</strong><span>Handlers and bindings</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/background-work/"><strong>Background Work</strong><span>Queues, Workflows and cron</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/storage/"><strong>Storage</strong><span>KV, D1 and R2</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Bindings and migrations</span></a>
</div>
