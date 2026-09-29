module VectorQuery

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module WorkersAI = FSharp.CloudEdge.Runtime.WorkersAIProvider
module V4 = FSharp.CloudEdge.Support.AI.V4.Provider

let embed (ai: Workers.Ai<Workers.AiModels>) (texts: string[]) =
    async {
        let settings = WorkersAI.WorkersAISettings2.Create(binding = ai)
        let workersai = WorkersAI.Exports.createWorkersAI (U2.Case1 settings)
        let model = workersai.textEmbedding "@cf/baai/bge-base-en-v1.5"
        let! result = model.doEmbed (V4.EmbeddingModelV4CallOptions.Create texts) |> Async.AwaitPromise
        return result.embeddings
    }

let vectorSearch (ai: Workers.Ai<Workers.AiModels>) (index: Workers.Vectorize) (query: string) =
    async {
        let! embeddings = embed ai [| query |]
        let options = Workers.VectorizeQueryOptions.Create(topK = 20.)
        let! found = index.query (U3.Case1 embeddings[0], options) |> Async.AwaitPromise
        return found.matches |> Array.map (fun hit -> hit.id)
    }
