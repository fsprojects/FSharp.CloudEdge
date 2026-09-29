module AudioPreview

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Voice = FSharp.CloudEdge.Runtime.Voice
module Errors = FSharp.CloudEdge.Runtime.VoiceErrors.Errors

type Env =
    abstract AI: Voice.AiLike

let opening (article: string) =
    let chunker = Voice.Exports.SentenceChunker()
    Array.append (chunker.add article) (chunker.flush ()) |> Array.truncate 2 |> String.concat " "

let failed (message: string) =
    Workers.Exports.Response.json ({| error = message |}, U2.Case2(Workers.ResponseInit.Create(status = 502.)))

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                try
                    let! article = request.text () |> Async.AwaitPromise
                    let tts = Voice.Exports.WorkersAITTS env.AI
                    let! audio = tts.synthesize (opening article) |> Async.AwaitPromise
                    match audio with
                    | Some bytes ->
                        let headers = [| [| "content-type"; "audio/mpeg" |] |]
                        return Workers.Exports.Response.Create(bytes, Workers.ResponseInit.Create(headers = headers))
                    | None -> return failed "No audio"
                with caught ->
                    let error = Errors.Exports.toVoiceError (caught, "Speech is unavailable")
                    Errors.Exports.logVoiceError (Errors.VoiceErrorLogOptions.Create(``component`` = "AudioPreview", stage = "synthesize", message = "Preview failed", error = error))
                    return failed (Errors.Exports.voiceErrorMessage (error, "Speech is unavailable"))
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
