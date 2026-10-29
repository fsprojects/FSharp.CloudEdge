module WorkspaceFiles

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
                let sandbox: FSharp.CloudEdge.Runtime.SandboxBridge.ISandbox = SandboxApi.getSandbox env.Sandbox "files"
                match url.searchParams.get "path" |> Option.bind Bridge.Exports.resolveWorkspacePath with
                | None ->
                    return Workers.Exports.Response.Create(U9.Case1 "Paths must stay inside /workspace", Workers.ResponseInit.Create(status = 400.))
                | Some path when request.``method`` = "PUT" ->
                    let! body = request.text () |> Async.AwaitPromise
                    let! saved = sandbox.writeFile(path, body) |> Async.AwaitPromise
                    return Workers.Exports.Response.json {| saved = saved.path |}
                | Some path ->
                    let! file = sandbox.readFile path |> Async.AwaitPromise
                    return Workers.Exports.Response.Create(U9.Case1 file.content)
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
