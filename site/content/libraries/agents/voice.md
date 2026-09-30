---
title: Voice
description: Build speech input and output around an agent conversation.
---

<div class="ce-block-head">
<p class="ce-block-lead">Build speech input and output around an agent conversation.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Voice</code> <code>Runtime.VoiceErrors</code></li>
<li><span>npm</span> <code>@cloudflare/voice 0.4.0</code></li>
<li><span>Upstream/API</span> Experimental</li>
</ul>
</div>

## Speech is a separate pipeline

Voice connects speech providers and audio handling to the rest of an agent. Choose transcription and synthesis providers, define when a turn begins and ends, and decide how partial text and interruptions affect the conversation.

The example below covers text-to-speech only: it turns part of an article into audio using Workers AI. It can be used independently of a chat agent. The pinned [Voice package](https://www.npmjs.com/package/@cloudflare/voice/v/0.4.0) contains the wider provider and pipeline interfaces.

## Audio Preview

The Worker responds to a POSTed article with MPEG audio of its first two sentences, which `SentenceChunker` splits out and `WorkersAITTS` synthesizes with Workers AI. If the `synthesize` call throws, `logVoiceError` writes one structured log entry and the caller receives the error text, cut to at most 300 characters.

```fsharp
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
```

<div class="ce-needs"><p><strong>Needs</strong> a Workers AI binding named <code>AI</code>. <code>Voice.AiLike</code> is the Voice package's own type for that binding, and <code>WorkersAITTS</code> calls the <code>@cf/deepgram/aura-1</code> model unless its options set another. The free plan includes 10,000 Workers AI Neurons a day.</p></div>

## Integrating a conversation

Keep conversation identity and history in the [chat layer](chat.md). Connect text from speech recognition to that conversation, then stream or synthesize the reply through the chosen output provider. Audio encoding, chunking, cancellation, and provider errors belong to this integration and should be handled explicitly.

## Related Pages

- [Chat agents](chat.md)
- [Workers AI providers](../ai.md)
- [Agents SDK](sdk.md)

## Help verify these bindings

The examples on this page are checked against F# source projects. Compilation and emitted JavaScript checks do not establish hosted service behavior. Useful targets for community verification include audio and stream types, provider callbacks, synthesis results, cancellation, and error handling.

See [Verify bindings](../../guide/verify-bindings.md) for the existing evidence, reproducible checks, and the [binding issue form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include the pinned package version and the specific behavior exercised; successful reproductions are useful evidence too.
