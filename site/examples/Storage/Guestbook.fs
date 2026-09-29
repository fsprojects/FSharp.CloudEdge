module Guestbook

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Entry = {| name: string; message: string |}

type Env =
    abstract DB: Workers.D1Database

let insertSql = "INSERT INTO entries (name, message) VALUES (?, ?)"
let latestSql = "SELECT name, message FROM entries ORDER BY id DESC LIMIT 20"

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            match request.``method`` with
            | "POST" ->
                let! entry = request.json<Entry>() |> Async.AwaitPromise
                let insert = env.DB.prepare(insertSql).bind(entry.name, entry.message)
                let! result = insert.run() |> Async.AwaitPromise
                return Workers.Exports.Response.json {| id = result.meta.last_row_id |}
            | _ ->
                let! result = env.DB.prepare(latestSql).all<Entry>() |> Async.AwaitPromise
                return Workers.Exports.Response.json result.results
        }
        |> Async.StartAsPromise
        |> U2.Case1)
