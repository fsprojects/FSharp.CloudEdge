module SessionCleanup

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract DB: Workers.D1Database

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        scheduled = fun controller env _ ->
            env.DB.prepare("DELETE FROM sessions WHERE expires_at < ?").bind(controller.scheduledTime).run()
            |> Async.AwaitPromise
            |> Async.Ignore
            |> Async.StartAsPromise
            |> Some
    )
