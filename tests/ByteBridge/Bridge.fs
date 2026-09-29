module FSharp.CloudEdge.Tests.ByteBridge

open Fable.Core
open Fable.Core.JsInterop
open BAREWire.Encoding
open BAREWire.Framing
module Workers = FSharp.CloudEdge.Runtime.Workers

// This fixture accepts an Ask carrying exactly one u32 and echoes it in a Reply.
let replyToView (bytes: JS.Uint8Array) : Result<byte array, string> =
    try
        let normalized: byte array =
            emitJsExpr bytes "new Uint8Array($0.buffer, $0.byteOffset, $0.byteLength)"

        let frame, payloadAt = Envelope.decodeMessage normalized

        if Cursor.isFault payloadAt then
            Error "Malformed envelope"
        elif frame.Kind <> FrameKind.Ask then
            Error "Expected Ask"
        else
            let value, consumed = Codec.decode Decoder.readU32 frame.Payload

            if Cursor.isFault consumed then
                Error "Payload must contain exactly one u32"
            else
                let payload, written = Codec.encode 4 Encoder.writeU32 value

                if Cursor.isFault written then
                    Error "Payload encoding failed"
                else
                    Ok(Envelope.encodeMessage (Envelope.reply frame.Correlation payload))
    with _ ->
        Error "Unreadable byte view"

let replyToBody (body: Workers.Body) : JS.Promise<Result<byte array, string>> =
    async {
        try
            let! bytes = body.bytes() |> Async.AwaitPromise
            return replyToView bytes
        with _ ->
            return Error "Unreadable body"
    }
    |> Async.StartAsPromise
