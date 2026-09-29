module AnnouncementBanner

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

let banner = """<p class="banner">Orders ship free this week.</p>"""

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request _ _ ->
            async {
                let! page = Workers.Exports.fetch(U3.Case1 request.url) |> Async.AwaitPromise
                let addBanner =
                    Workers.HTMLRewriterElementContentHandlers.Create(
                        element = fun body ->
                            body.prepend(U3.Case1 banner, Workers.ContentOptions.Create(html = true)) |> ignore
                            None
                    )
                return Workers.Exports.HTMLRewriter().on("body", addBanner).transform page
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
