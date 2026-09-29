module ChunkHash

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Chunk = {| id: string; title: string; text: string |}

let chunkHash (chunk: Chunk) =
    async {
        let bytes = Workers.Exports.TextEncoder().encode (chunk.title + "\n" + chunk.text)
        let! digest =
            Workers.Exports.crypto.subtle.digest (U2.Case1 "SHA-256", U2.Case1 bytes.buffer)
            |> Async.AwaitPromise
        let view = JS.Constructors.Uint8Array.Create digest
        let hash = Array.init view.length (fun i -> sprintf "%02x" view[i]) |> String.concat ""
        return {| chunk with hash = hash |}
    }
