module RoomStatus

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Transport = FSharp.CloudEdge.Support.Workers.DurableObjects

type Env =
    abstract ROOMS: Workers.DurableObjectNamespace<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            let room = Workers.Exports.URL(U2.Case1 request.url).pathname.Substring(1)
            let transport = Transport.getFetchByName env.ROOMS room
            transport.fetch(Workers.Exports.Request.Create(U3.Case1 request.url)) |> U2.Case1
    )
