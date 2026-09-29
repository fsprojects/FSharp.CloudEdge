module FlagInspector

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract FLAGS: Workers.Flagship

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun _ env _ ->
            async {
                let! details = env.FLAGS.getBooleanDetails("new-checkout", false) |> Async.AwaitPromise
                return
                    Workers.Exports.Response.json
                        {| newCheckout = details.value
                           variant = Option.defaultValue "none" details.variant
                           reason = Option.defaultValue "none" details.reason |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
