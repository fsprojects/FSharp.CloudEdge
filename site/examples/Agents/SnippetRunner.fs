module SnippetRunner

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module CodeMode = FSharp.CloudEdge.Runtime.CodeMode

type Env =
    abstract LOADER: Workers.WorkerLoader

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! code = request.text () |> Async.AwaitPromise
                let options = CodeMode.DynamicWorkerExecutorOptions.Create(loader = env.LOADER, timeout = 5000.)
                let executor = CodeMode.Exports.DynamicWorkerExecutor options
                let! outcome = executor.execute (code, U2.Case1 [||]) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| result = outcome.result; error = outcome.error; logs = outcome.logs |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
