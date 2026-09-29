module HelpCenterAnswers

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Search = FSharp.CloudEdge.Runtime.AISearchProvider
module V3 = FSharp.CloudEdge.Support.AI.V3.Provider

type Env =
    abstract AI_SEARCH: Workers.AiSearchNamespace

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! question = request.text () |> Async.AwaitPromise
                let settings = Search.AISearchNamespaceSettings.Create env.AI_SEARCH
                let search = Search.Exports.createAISearchNamespace settings
                let model = search.get("help-center").chat ()
                let questionPart = V3.LanguageModelV3Message3.Content.Item.Text(question, None)
                let prompt = [| V3.LanguageModelV3Message.User([| questionPart |], None) |]
                let options = V3.LanguageModelV3CallOptions.Create prompt
                let! result = model.doGenerate options |> Async.AwaitPromise
                let answer =
                    result.content
                    |> Array.choose (function
                        | V3.LanguageModelV3Content.Text(text, _) -> Some text
                        | _ -> None)
                    |> String.concat ""
                return Workers.Exports.Response.json {| answer = answer |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
