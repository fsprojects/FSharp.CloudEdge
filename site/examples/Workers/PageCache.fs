module PageCache

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract ORIGIN: string

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let cache = Workers.Exports.caches.``default``
            let! cached = cache.``match``(U3.Case1 request.url) |> Async.AwaitPromise
            match cached with
            | Some response -> return response
            | None ->
                let url = Workers.Exports.URL(U2.Case1 request.url)
                let pageUrl = env.ORIGIN + url.pathname + url.search
                let! page = Workers.Exports.fetch(U3.Case1 pageUrl) |> Async.AwaitPromise
                do! cache.put(U3.Case1 request.url, page.clone()) |> Async.AwaitPromise
                return page
        }
        |> Async.StartAsPromise
        |> U2.Case1)
