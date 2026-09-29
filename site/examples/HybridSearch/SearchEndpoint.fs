module SearchEndpoint

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
