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
                let sandbox: FSharp.CloudEdge.Runtime.SandboxBridge.ISandbox = SandboxApi.getSandbox env.Sandbox runId
                let checkout = FSharp.CloudEdge.Runtime.SandboxBridge.ISandbox.GitCheckout.Options.Create(targetDir = "/workspace/app", depth = 1.)
                let! _ = sandbox.gitCheckout(repository, checkout) |> Async.AwaitPromise
                let! log = sandbox.execStream "cd /workspace/app && npm ci && npm test" |> Async.AwaitPromise
                let headers = Workers.Exports.Headers()
                headers.set("Content-Type", "text/event-stream")
                headers.set("Cache-Control", "no-cache")
                return Workers.Exports.Response.Create(unbox<Workers.BodyInit> log, Workers.ResponseInit.Create(headers = U3.Case1 headers))
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
