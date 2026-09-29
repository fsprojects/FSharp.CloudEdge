module InviteLinks

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
