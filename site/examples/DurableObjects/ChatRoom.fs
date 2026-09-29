module ChatRoom

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
