module UsageMeter

open Fable.Core

module V3 = FSharp.CloudEdge.Support.AI.V3.Provider

let askWithUsage (model: V3.LanguageModelV3) (question: string) =
    async {
        let part = V3.LanguageModelV3Message3.Content.Item.Text(question, None)
        let prompt = [| V3.LanguageModelV3Message.User([| part |], None) |]
        let options = V3.LanguageModelV3CallOptions.Create(prompt = prompt, maxOutputTokens = 400.)
        let! result = model.doGenerate options |> Async.AwaitPromise
        let answer =
            result.content
            |> Array.choose (function
                | V3.LanguageModelV3Content.Text(text, _) -> Some text
                | _ -> None)
            |> String.concat ""
        return
            {| answer = answer
               inputTokens = result.usage.inputTokens.total |> Option.defaultValue 0.
               outputTokens = result.usage.outputTokens.total |> Option.defaultValue 0.
               finish = result.finishReason.unified |}
    }
    |> Async.StartAsPromise
