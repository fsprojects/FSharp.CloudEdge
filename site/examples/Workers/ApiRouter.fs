module ApiRouter

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

let jsonWithStatus (status: float) body =
    let init = Workers.ResponseInit.Create(status = status)
    Workers.Exports.Response.json(body, U2.Case2 init)

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request _ _ ->
        let url = Workers.Exports.URL(U2.Case1 request.url)
        let response =
            match request.``method``, url.pathname with
            | "GET", "/" -> Workers.Exports.Response.Create("Welcome to the API")
            | "GET", "/api/greeting" ->
                let name = url.searchParams.get "name" |> Option.defaultValue "world"
                Workers.Exports.Response.json {| greeting = $"Hello, {name}" |}
            | _, "/api/greeting" -> jsonWithStatus 405. {| error = "Use GET" |}
            | _ -> jsonWithStatus 404. {| error = "Not found" |}
        U2.Case2 response)
