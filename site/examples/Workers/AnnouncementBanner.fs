module AnnouncementBanner

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract ORIGIN: string

let banner = """<p class="banner">Orders ship free this week.</p>"""
let asHtml = Workers.ContentOptions.Create(html = true)

let addBanner =
    Workers.HTMLRewriterElementContentHandlers.Create(element = fun body ->
        body.prepend(U3.Case1 banner, asHtml) |> ignore
        None)

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let url = Workers.Exports.URL(U2.Case1 request.url)
            let pageUrl = env.ORIGIN + url.pathname + url.search
            let! page = Workers.Exports.fetch(U3.Case1 pageUrl) |> Async.AwaitPromise
            return Workers.Exports.HTMLRewriter().on("body", addBanner).transform page
        }
        |> Async.StartAsPromise
        |> U2.Case1)
