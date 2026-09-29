module ApiReference

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Chanfana = FSharp.CloudEdge.Runtime.Chanfana

let listTasks = {| summary = "List tasks"; responses = {| ``200`` = {| description = "The task list" |} |} |}

let spec =
    {| openapi = "3.1.0"
       info = {| title = "Tasks API"; version = "1.0.0" |}
       paths = {| ``/tasks`` = {| get = listTasks |} |} |}

let html = Workers.ResponseInit.Create(headers = {| ``content-type`` = "text/html; charset=utf-8" |})

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request _ _ ->
            match Workers.Exports.URL(U2.Case1 request.url).pathname with
            | "/openapi.json" -> U2.Case2(Workers.Exports.Response.json spec)
            | "/redoc" -> U2.Case2(Workers.Exports.Response.Create(Chanfana.Exports.getReDocUI "/openapi.json", html))
            | _ -> U2.Case2(Workers.Exports.Response.Create(Chanfana.Exports.getSwaggerUI "/openapi.json", html))
    )
