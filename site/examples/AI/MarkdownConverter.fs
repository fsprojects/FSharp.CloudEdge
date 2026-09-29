module MarkdownConverter

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract AI: Workers.Ai<Workers.AiModels>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let name = request.url.Split('/') |> Array.last
                let! file = request.blob () |> Async.AwaitPromise
                let upload = Workers.MarkdownDocument.Create(name, file)
                let! converted = env.AI.toMarkdown upload |> Async.AwaitPromise
                return Workers.Exports.Response.json converted
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
