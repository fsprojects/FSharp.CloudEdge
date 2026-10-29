module Summarizer

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module WorkersAI = FSharp.CloudEdge.Runtime.WorkersAIProvider
module V4 = FSharp.CloudEdge.Support.AI.V4.Provider

type Env =
    abstract AI: Workers.Ai<Workers.AiModels>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! article = request.text () |> Async.AwaitPromise
                let settings = WorkersAI.WorkersAISettings2.Create(binding = env.AI)
                let workersai = WorkersAI.Exports.createWorkersAI (U2.Case1 settings)
                let model = workersai.chat<string> (U2.Case1 "@cf/zai-org/glm-4.7-flash")
                let articlePart = V4.LanguageModelV4Message.Cases.User.Content.Item.Text(V4.LanguageModelV4TextPart.Create(article))
                let prompt =
                    [| V4.LanguageModelV4Message.System("Summarize in two sentences.", None)
                       V4.LanguageModelV4Message.User([| articlePart |], None) |]
                let options = V4.LanguageModelV4CallOptions.Create prompt
                let! result = model.doGenerate options |> Async.AwaitPromise
                let summary =
                    result.content
                    |> Array.choose (function
                        | V4.LanguageModelV4Content.Text part -> Some part.text
                        | _ -> None)
                    |> String.concat ""
                return Workers.Exports.Response.json {| summary = summary |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
