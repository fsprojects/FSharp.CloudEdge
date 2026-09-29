---
title: Hybrid Search
description: Keyword and vector search over your content, indexed incrementally.
order: 8
---

<div class="ce-block-head">
<p class="ce-block-lead">Give your site a search box that ranks results by the words they share with the query and by closeness in meaning. One Worker serves search requests and updates a keyword index and a vector index, embedding a chunk again only when its hash changes.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Workers</code> <code>Runtime.WorkersAIProvider</code> <code>Support.AI.V4.Provider</code></li>
<li><span>npm</span> <code>@cloudflare/workers-types</code> 5.20260906.1, <code>workers-ai-provider</code> 4.0.0, <code>@ai-sdk/provider</code> 4.0.10</li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/d1/platform/pricing/">D1: 5 million rows read and 100,000 rows written a day</a></li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/workers-ai/platform/pricing/">Workers AI: 10,000 Neurons a day</a></li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/vectorize/platform/pricing/">Vectorize: 5 million stored vector dimensions, 30 million queried a month</a></li>
</ul>
</div>

## Keyword Query

The `chunks` table uses FTS5, the SQLite full-text search module that [D1 supports](https://developers.cloudflare.com/d1/sql-api/sql-statements/). `ORDER BY bm25(chunks)` puts the best match first, because `bm25` scores better matches lower. `keywordSearch` quotes each word and joins the words with `OR`. Unquoted, `d1: store` is FTS5 syntax for a column filter and fails with `no such column: d1`.

```fsharp
open System
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

let schema =
    "CREATE VIRTUAL TABLE IF NOT EXISTS chunks USING fts5(id UNINDEXED, page UNINDEXED, title, text, hash UNINDEXED)"

let searchSql = "SELECT id FROM chunks WHERE chunks MATCH ? ORDER BY bm25(chunks) LIMIT 20"

let keywordSearch (db: Workers.D1Database) (query: string) =
    async {
        let terms =
            query.Replace("\"", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries)
            |> Array.map (fun term -> "\"" + term + "\"")
        if terms.Length = 0 then
            return [||]
        else
            let statement = db.prepare(searchSql).bind(String.concat " OR " terms)
            let! found = statement.all<{| id: string |}>() |> Async.AwaitPromise
            return found.results |> Array.map (fun row -> row.id)
    }
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { map } from "./fable_modules/fable-library-js.5.13.0/Array.js";
import { join, replace, split } from "./fable_modules/fable-library-js.5.13.0/String.js";
import { awaitPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";

export const schema = "CREATE VIRTUAL TABLE IF NOT EXISTS chunks USING fts5(id UNINDEXED, page UNINDEXED, title, text, hash UNINDEXED)";

export const searchSql = "SELECT id FROM chunks WHERE chunks MATCH ? ORDER BY bm25(chunks) LIMIT 20";

export function keywordSearch(db, query) {
    return singleton.Delay(() => {
        const terms = map((term) => (("\"" + term) + "\""), split(replace(query, "\"", " "), [" "], undefined, 1));
        if (terms.length === 0) {
            return singleton.Return([]);
        }
        else {
            const statement = db.prepare(searchSql).bind(join(" OR ", terms));
            return singleton.Bind(awaitPromise(statement.all()), (_arg) => singleton.Return(map((row) => row.id, _arg.results)));
        }
    });
}
```

</details>

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

## Rank Fusion

BM25 scores and vector similarities are on different scales, so `fuse` combines the two lists by position alone. An id's score is the sum of 1 / (60 + rank) across every list that includes the id, counting ranks from 1. With at most 20 results in each list, any id in both lists ranks above every id in a single list.

```fsharp
let k = 60.0

let fuse (rankings: string[][]) =
    rankings
    |> Array.collect (Array.mapi (fun rank id -> id, 1.0 / (k + float rank + 1.0)))
    |> Array.groupBy fst
    |> Array.map (fun (id, scores) -> id, Array.sumBy snd scores)
    |> Array.sortByDescending snd
    |> Array.map fst
```

## Chunk Hash

Split each page into chunks, such as one per heading, and keep each chunk within the 512 input tokens that bge-base-en-v1.5 accepts. A chunk's id is also its Vectorize id, and [Vectorize limits](https://developers.cloudflare.com/vectorize/platform/limits/) ids to 64 bytes. `chunkHash` computes a SHA-256 digest with `crypto.subtle` and returns the chunk with a `hash` field of 64 hex characters. The digest input is the title and the text, so an edited title also counts as a change.

```fsharp
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
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { awaitPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { printf, toText, join } from "./fable_modules/fable-library-js.5.13.0/String.js";
import { initialize } from "./fable_modules/fable-library-js.5.13.0/Array.js";

export function chunkHash(chunk) {
    return singleton.Delay(() => {
        const bytes = (new TextEncoder()).encode((chunk.title + "\n") + chunk.text);
        return singleton.Bind(awaitPromise(crypto.subtle.digest("SHA-256", bytes.buffer)), (_arg) => {
            const view = new Uint8Array(_arg);
            const hash = join("", initialize(view.length, (i) => {
                const arg = view[i];
                return toText(printf("%02x"))(arg);
            }));
            return singleton.Return({
                hash: hash,
                id: chunk.id,
                text: chunk.text,
                title: chunk.title,
            });
        });
    });
}
```

</details>

## Changed-Chunk Filter

The body of each index request is one `Page`. `changedChunks` compares each chunk's hash with the hash stored under the same id. `write` holds new and edited chunks, and `delete` holds the ids of chunks removed from the page. To remove a page from the index, send it with an empty `chunks` array.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Page = {| path: string; chunks: ChunkHash.Chunk[] |}

let storedSql = "SELECT id, hash FROM chunks WHERE page = ?"

let changedChunks (db: Workers.D1Database) (page: Page) =
    async {
        let! hashed = page.chunks |> Array.map ChunkHash.chunkHash |> Async.Parallel
        let statement = db.prepare(storedSql).bind(page.path)
        let! stored = statement.all<{| id: string; hash: string |}>() |> Async.AwaitPromise
        let known = stored.results |> Array.map (fun row -> row.id, row.hash) |> Map.ofArray
        let storedIds = stored.results |> Array.map (fun row -> row.id) |> Set.ofArray
        let incomingIds = page.chunks |> Array.map (fun chunk -> chunk.id) |> Set.ofArray
        let changed = hashed |> Array.filter (fun chunk -> Map.tryFind chunk.id known <> Some chunk.hash)
        let stale = Set.difference storedIds incomingIds |> Set.toArray
        return {| write = changed; delete = stale |}
    }
```

## Index Update

`reindex` embeds only the chunks in `write`, so for an unchanged page the only binding calls are two D1 queries. The D1 batch runs last and stores the new hashes in one transaction. After a failed run, D1 still holds the old hashes, so the next run repeats the work. An upsert with an existing id replaces that vector, so the repeat leaves one vector per chunk.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

let deleteSql = "DELETE FROM chunks WHERE id = ?"
let insertSql = "INSERT INTO chunks (id, page, title, text, hash) VALUES (?, ?, ?, ?, ?)"

let reindex (db: Workers.D1Database) (index: Workers.Vectorize) ai (page: ChangedChunks.Page) =
    async {
        do! db.exec KeywordQuery.schema |> Async.AwaitPromise |> Async.Ignore
        let! changes = ChangedChunks.changedChunks db page
        if changes.write.Length > 0 then
            let texts = changes.write |> Array.map (fun chunk -> chunk.title + "\n" + chunk.text)
            let! embeddings = VectorQuery.embed ai texts
            let vectors =
                Array.zip changes.write embeddings
                |> Array.map (fun (chunk, values) -> Workers.VectorizeVector.Create(chunk.id, U3.Case1 values))
            do! index.upsert vectors |> Async.AwaitPromise |> Async.Ignore
        if changes.delete.Length > 0 then
            do! index.deleteByIds changes.delete |> Async.AwaitPromise |> Async.Ignore
        let removed = Array.append (changes.write |> Array.map (fun chunk -> chunk.id)) changes.delete
        let statements =
            [| for id in removed -> db.prepare(deleteSql).bind id
               for chunk in changes.write ->
                   db.prepare(insertSql).bind(chunk.id, page.path, chunk.title, chunk.text, chunk.hash) |]
        if statements.Length > 0 then
            do! db.batch statements |> Async.AwaitPromise |> Async.Ignore
        return
            {| written = changes.write.Length
               deleted = changes.delete.Length
               unchanged = page.chunks.Length - changes.write.Length |}
    }
```

## Search Endpoint

For `GET /?q=...`, the Worker runs both queries in parallel and responds with the ten best ids after fusion. To index a page, send it as the body of a `POST` with the header `Authorization: Bearer <secret>`, where `<secret>` is the value of `UPLOAD_SECRET`. Vectorize applies upserts asynchronously, so vector results typically include a new chunk within a few seconds.

```fsharp
open System
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract DB: Workers.D1Database
    abstract VECTORS: Workers.Vectorize
    abstract AI: Workers.Ai<Workers.AiModels>
    abstract UPLOAD_SECRET: string

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let url = Workers.Exports.URL(U2.Case1 request.url)
                let authorized =
                    not (String.IsNullOrEmpty env.UPLOAD_SECRET)
                    && request.headers.get "Authorization" = Some("Bearer " + env.UPLOAD_SECRET)
                match request.``method``, url.searchParams.get "q" with
                | "POST", _ when authorized ->
                    let! page = request.json<ChangedChunks.Page>() |> Async.AwaitPromise
                    let! counts = IndexUpdate.reindex env.DB env.VECTORS env.AI page
                    return Workers.Exports.Response.json counts
                | "POST", _ ->
                    return Workers.Exports.Response.Create("Unauthorized", Workers.ResponseInit.Create(status = 401.))
                | _, Some query when query.Trim() <> "" ->
                    let! rankings =
                        Async.Parallel
                            [| KeywordQuery.keywordSearch env.DB query
                               VectorQuery.vectorSearch env.AI env.VECTORS query |]
                    let results = RankFusion.fuse rankings |> Array.truncate 10
                    return Workers.Exports.Response.json {| query = query; results = results |}
                | _ ->
                    return Workers.Exports.Response.Create("Add ?q= to the URL", Workers.ResponseInit.Create(status = 400.))
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> a D1 binding named <code>DB</code> and a Vectorize binding named <code>VECTORS</code>. The Worker also uses the Workers AI binding <code>AI</code> and the secret <code>UPLOAD_SECRET</code>. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare bindings.</p></div>

## Library Table

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.Workers` | `@cloudflare/workers-types` 5.20260906.1 | D1, Vectorize, `crypto`, `AI` binding |
| `Runtime.WorkersAIProvider` | `workers-ai-provider` 4.0.0 | Workers AI embedding models |
| `Support.AI.V4.Provider` | `@ai-sdk/provider` 4.0.10 | Embedding call options and results |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/storage/"><strong>Storage</strong><span>SQL in D1, files in R2</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/ai/"><strong>AI</strong><span>Workers AI models from F#</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/asset-uploads/"><strong>Asset Uploads</strong><span>Only the files that changed</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>A Worker and its bindings</span></a>
</div>
