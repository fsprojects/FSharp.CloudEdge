module ShopAssistant

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module AIUtils = FSharp.CloudEdge.Runtime.AIUtils

type Env =
    abstract AI: Workers.Ai<Workers.AiModels>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! question = request.text () |> Async.AwaitPromise
                let spec = "https://shop.example.com/openapi.json"
                let! tools = AIUtils.Exports.createToolsFromOpenAPISpec spec |> Async.AwaitPromise
                let messages: obj[] = [| {| role = "user"; content = question |} |]
                let input = AIUtils.RunWithTools.Input.Create(messages, tools)
                let model = "@hf/nousresearch/hermes-2-pro-mistral-7b"
                let! answer =
                    AIUtils.Exports.runWithTools (env.AI, model, input) |> Async.AwaitPromise
                return Workers.Exports.Response.json answer
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
