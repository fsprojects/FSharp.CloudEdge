module StorefrontSettings

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract FLAGS: Workers.Flagship

let standardPrices = {| monthly = 12.; yearly = 120. |}

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun _ env _ ->
            async {
                let! banner = env.FLAGS.getStringValue("banner-text", "Welcome back!") |> Async.AwaitPromise
                let! trialDays = env.FLAGS.getNumberValue("trial-days", 14.) |> Async.AwaitPromise
                let! prices = env.FLAGS.getObjectValue("prices", standardPrices) |> Async.AwaitPromise
                let settings = {| banner = banner; trialDays = trialDays; prices = prices |}
                return Workers.Exports.Response.json settings
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
