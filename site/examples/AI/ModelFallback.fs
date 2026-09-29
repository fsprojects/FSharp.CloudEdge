module ModelFallback

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Gateway = FSharp.CloudEdge.Runtime.AIGatewayProvider
module Unified = FSharp.CloudEdge.Runtime.AIGatewayProvider.Providers.Unified
module V4 = FSharp.CloudEdge.Support.AI.V4.Provider

type Env =
    abstract ACCOUNT_ID: string
    abstract GATEWAY_TOKEN: string

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! question = request.text () |> Async.AwaitPromise
                let settings =
                    Gateway.AiGatewayAPISettings.Create(
                        gateway = "support",
                        accountId = env.ACCOUNT_ID,
                        apiKey = env.GATEWAY_TOKEN
                    )
                let gateway = Gateway.Exports.createAiGateway (U2.Case1 settings)
                let models =
                    [| "openai/gpt-5.2"; "workers-ai/@cf/meta/llama-3.3-70b-instruct-fp8-fast" |]
                    |> Array.map Unified.Exports.unified
                let model = gateway.chat (U2.Case1 models)
                let questionPart = V4.LanguageModelV4Message3.Content.Item.Text(question, None)
                let prompt = [| V4.LanguageModelV4Message.User([| questionPart |], None) |]
                let options = V4.LanguageModelV4CallOptions.Create prompt
                let! result = model.doGenerate options |> Async.AwaitPromise
                let answer =
                    result.content
                    |> Array.choose (function
                        | V4.LanguageModelV4Content.Text(text, _) -> Some text
                        | _ -> None)
                    |> String.concat ""
                return Workers.Exports.Response.json {| answer = answer |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
