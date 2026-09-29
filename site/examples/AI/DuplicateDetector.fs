module DuplicateDetector

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module WorkersAI = FSharp.CloudEdge.Runtime.WorkersAIProvider
module V4 = FSharp.CloudEdge.Support.AI.V4.Provider

type Env =
    abstract AI: Workers.Ai<Workers.AiModels>

type Pair = {| question: string; existing: string |}

let cosine (a: float[]) (b: float[]) =
    let dot = Array.map2 ( * ) a b |> Array.sum
    let norm (v: float[]) = v |> Array.sumBy (fun x -> x * x) |> sqrt
    dot / (norm a * norm b)

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! pair = request.json<Pair> () |> Async.AwaitPromise
                let settings = WorkersAI.WorkersAISettings2.Create(binding = env.AI)
                let workersai = WorkersAI.Exports.createWorkersAI (U2.Case1 settings)
                let model = workersai.textEmbedding "@cf/baai/bge-base-en-v1.5"
                let options =
                    V4.EmbeddingModelV4CallOptions.Create [| pair.question; pair.existing |]
                let! result = model.doEmbed options |> Async.AwaitPromise
                let similarity = cosine result.embeddings[0] result.embeddings[1]
                return Workers.Exports.Response.json {| similarity = similarity |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
