module LinkShortener

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract LINKS: Workers.KVNamespace<string>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let slug = Workers.Exports.URL(U2.Case1 request.url).pathname.Substring(1)
            match request.``method`` with
            | "PUT" ->
                let! target = request.text() |> Async.AwaitPromise
                do! env.LINKS.put(slug, U4.Case1 target) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| slug = slug; target = target |}
            | _ ->
                let! target =
                    env.LINKS.get(slug, Workers.KVNamespace.Text) |> Async.AwaitPromise
                match target with
                | Some url -> return Workers.Exports.Response.redirect(url, 302.)
                | None ->
                    let notFound = Workers.ResponseInit.Create(status = 404.)
                    return Workers.Exports.Response.Create("No such link", notFound)
        }
        |> Async.StartAsPromise
        |> U2.Case1)
