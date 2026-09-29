module Notebook

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
                let url = Workers.Exports.URL(U2.Case1 request.url)
                let notebook = url.searchParams.get "notebook" |> Option.defaultValue "scratch"
                let sandbox: Sandbox.ISandbox = Sandbox.Exports.getSandbox<Sandbox.Sandbox<obj>>(env.Sandbox, notebook)
                let! contexts = sandbox.listCodeContexts () |> Async.AwaitPromise
                let! context =
                    match Array.tryHead contexts with
                    | Some context -> async { return context }
                    | None -> sandbox.createCodeContext () |> Async.AwaitPromise
                let! cell = request.text () |> Async.AwaitPromise
                let! result = sandbox.runCode(cell, Sandbox.RunCodeOptions.Create(context = context)) |> Async.AwaitPromise
                let values = result.results |> Array.choose (fun item -> item.text)
                return Workers.Exports.Response.json {| output = result.logs.stdout; values = values |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
