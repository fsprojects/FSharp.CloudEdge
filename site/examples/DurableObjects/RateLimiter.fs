module RateLimiter

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
