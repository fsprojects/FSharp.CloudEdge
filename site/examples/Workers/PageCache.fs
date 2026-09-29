module PageCache

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request _ _ ->
            async {
                let cache = Workers.Exports.caches.``default``
                let! cached = cache.``match``(U3.Case1 request.url) |> Async.AwaitPromise
                match cached with
                | Some response -> return response
                | None ->
                    let! response = Workers.Exports.fetch(U3.Case1 request.url) |> Async.AwaitPromise
                    do! cache.put(U3.Case1 request.url, response.clone()) |> Async.AwaitPromise
                    return response
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
