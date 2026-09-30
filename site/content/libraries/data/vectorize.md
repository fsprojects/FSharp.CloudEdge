---
title: Vectorize
description: Index vectors and retrieve nearby results for semantic search and retrieval.
---

<div class="ce-block-head">
<p class="ce-block-lead">Index vectors and retrieve nearby results for semantic search and retrieval.</p>
<ul class="ce-facts">
<li><span>Library</span> <code>Runtime.Workers</code></li>
<li><span>npm</span> <code>@cloudflare/workers-types</code> 5.20260906.1</li>
</ul>
</div>

## Index, model, and data

Vectorize stores vectors; an embedding model produces them. Choose the model before creating the index, and use matching dimensions and a compatible distance metric. Keep document IDs stable so upserts and deletions can follow changes in the source content.

Create an index and expose it through the Worker's `Vectorize` binding. The example below uses Workers AI for embeddings, but the index and model have separate configuration and lifecycles.

## Vector Query

`embed` runs the [bge-base-en-v1.5](https://developers.cloudflare.com/workers-ai/models/bge-base-en-v1.5/) model through your Worker's `AI` binding and returns 768 numbers for each text. `vectorSearch` embeds the query with the same model and returns the ids of the 20 nearest vectors in Vectorize.

```fsharp
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
```

<div class="ce-needs"><p><strong>Needs</strong> a Vectorize index created with 768 dimensions and the cosine metric. An index's dimensions and metric are fixed at creation, so choose the embedding model first. <code>StorageClient.VectorizeCreateVectorizeIndex</code> in <code>Management.Storage</code> creates the index from an F# program. <a href="/FSharp.CloudEdge/libraries/control-plane/account-setup/">Account Setup</a> shows the same client creating a D1 database.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { createWorkersAI } from "workers-ai-provider";
import { awaitPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { map, item } from "./fable_modules/fable-library-js.5.13.0/Array.js";

export function embed(ai, texts) {
    return singleton.Delay(() => {
        const workersai = createWorkersAI({
            binding: ai,
        });
        const model = workersai.textEmbedding("@cf/baai/bge-base-en-v1.5");
        return singleton.Bind(awaitPromise(model.doEmbed({
            values: texts,
        })), (_arg) => singleton.Return(_arg.embeddings));
    });
}

export function vectorSearch(ai, index, query) {
    return singleton.Delay(() => singleton.Bind(embed(ai, [query]), (_arg) => {
        const options = {
            topK: 20,
        };
        return singleton.Bind(awaitPromise(index.query(item(0, _arg), options)), (_arg_1) => singleton.Return(map((hit) => hit.id, _arg_1.matches)));
    }));
}
```

</details>

## Mutation visibility

[Index mutations are asynchronous](https://developers.cloudflare.com/vectorize/reference/client-api/). A returned mutation ID acknowledges the operation; do not treat it as proof that a query immediately sees the change. Runtime checks should observe processing and then assert retrieval results.

Use metadata filters where the application needs to narrow retrieval. Similarity scores depend on the model and metric; evaluate them with representative queries instead of treating a score as a universal confidence value.

## Combine keyword and vector search

[Hybrid Search](../hybrid-search.md) shows the combined recipe: D1 keyword retrieval, vector retrieval, rank fusion, and incremental indexing. This page covers the Vectorize service itself.


## Help verify the binding

Useful checks include vector dimensions, metadata filters, result shapes, mutation visibility, and deletion. A compiling consumer does not establish hosted behavior. Record the package version, configuration, and observed result using the [verification guide](../../guide/verify-bindings.md).

## Related Pages

- [Data & Analytics](index.md)
- [Account setup](../control-plane/account-setup.md)
- [Worker bindings](../control-plane/worker-upload.md)
