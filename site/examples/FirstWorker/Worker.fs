module Worker

open System
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request _ _ ->
        let url = Workers.Exports.URL(U2.Case1 request.url)
        let response =
            match url.pathname with
            | "/" ->
                let name = url.searchParams.get "name" |> Option.defaultValue "world"
                Workers.Exports.Response.json {| greeting = $"Hello, {name}" |}
            | "/time" ->
                Workers.Exports.Response.json {| utc = DateTime.UtcNow.ToString "o" |}
            | _ ->
                let init = Workers.ResponseInit.Create(status = 404.)
                Workers.Exports.Response.json({| error = "Not found" |}, U2.Case2 init)
        U2.Case2 response)
