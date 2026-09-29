module Counter

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

type Counter(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch _ =
            async {
                let! current = ctx.storage.get<float>("count") |> Async.AwaitPromise
                let count = Option.defaultValue 0. current + 1.
                do! ctx.storage.put("count", count) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| count = count |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
