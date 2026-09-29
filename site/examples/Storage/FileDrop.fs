module FileDrop

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract FILES: Workers.R2Bucket

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let key = Workers.Exports.URL(U2.Case1 request.url).pathname.Substring(1)
            match request.``method`` with
            | "PUT" ->
                let! bytes = request.arrayBuffer() |> Async.AwaitPromise
                let metadata = U2.Case1 request.headers
                let options = Workers.R2PutOptions.Create(httpMetadata = metadata)
                let! stored =
                    env.FILES.put(key, U5.Case2 bytes, options) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| key = key; size = stored.size |}
            | _ ->
                let! found = env.FILES.get key |> Async.AwaitPromise
                match found with
                | Some file ->
                    let headers = Workers.Exports.Headers()
                    file.writeHttpMetadata headers
                    headers.set("etag", file.httpEtag)
                    let init = Workers.ResponseInit.Create(headers = headers)
                    return Workers.Exports.Response.Create(file.body, init)
                | None ->
                    let notFound = Workers.ResponseInit.Create(status = 404.)
                    return Workers.Exports.Response.Create("Not found", notFound)
        }
        |> Async.StartAsPromise
        |> U2.Case1)
