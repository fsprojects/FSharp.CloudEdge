module DarkMode

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract FLAGS: Workers.Flagship

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun _ env _ ->
            async {
                let! darkMode = env.FLAGS.getBooleanValue("dark-mode", false) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| darkMode = darkMode |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
