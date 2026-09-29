module TextSearch

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Sandbox = FSharp.CloudEdge.Runtime.Sandbox
module Bridge = FSharp.CloudEdge.Runtime.SandboxBridge.Bridge

type Env =
    abstract Sandbox: Workers.DurableObjectNamespace<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let url = Workers.Exports.URL(U2.Case1 request.url)
                let term = url.searchParams.get "q" |> Option.defaultValue ""
                let sandbox: Sandbox.ISandbox = Sandbox.Exports.getSandbox<Sandbox.Sandbox<obj>>(env.Sandbox, "files")
                let! found = sandbox.exec $"grep -rn -- {Bridge.Exports.shellQuote term} /workspace" |> Async.AwaitPromise
                return Workers.Exports.Response.Create(found.stdout)
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
