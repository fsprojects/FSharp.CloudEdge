module TestRunner

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
                let! repository = request.text () |> Async.AwaitPromise
                let runId = string (System.Guid.NewGuid())
                let sandbox: Sandbox.ISandbox = Sandbox.Exports.getSandbox<Sandbox.Sandbox<obj>>(env.Sandbox, runId)
                let checkout = Sandbox.ISandbox.GitCheckout.Options.Create(targetDir = "/workspace/app", depth = 1.)
                let! _ = sandbox.gitCheckout(repository, checkout) |> Async.AwaitPromise
                let! log = sandbox.execStream "cd /workspace/app && npm ci && npm test" |> Async.AwaitPromise
                let headers = {| ``Content-Type`` = "text/event-stream"; ``Cache-Control`` = "no-cache" |}
                return Workers.Exports.Response.Create(log, Workers.ResponseInit.Create(headers = headers))
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
