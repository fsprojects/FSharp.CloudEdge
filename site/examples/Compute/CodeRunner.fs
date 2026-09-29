module CodeRunner

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Sandbox = FSharp.CloudEdge.Runtime.Sandbox

type Env =
    abstract Sandbox: Workers.DurableObjectNamespace<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let sandbox: Sandbox.ISandbox = Sandbox.Exports.getSandbox<Sandbox.Sandbox<obj>>(env.Sandbox, "playground")
                let! code = request.text () |> Async.AwaitPromise
                let! run = sandbox.runCode code |> Async.AwaitPromise
                return Workers.Exports.Response.json {| output = run.logs.stdout; errors = run.logs.stderr |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
