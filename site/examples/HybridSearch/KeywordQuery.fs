module KeywordQuery

open System
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

let schema =
    "CREATE VIRTUAL TABLE IF NOT EXISTS chunks USING fts5(id UNINDEXED, page UNINDEXED, title, text, hash UNINDEXED)"

let searchSql = "SELECT id FROM chunks WHERE chunks MATCH ? ORDER BY bm25(chunks) LIMIT 20"

let keywordSearch (db: Workers.D1Database) (query: string) =
    async {
        let terms =
            query.Replace("\"", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries)
            |> Array.map (fun term -> "\"" + term + "\"")
        if terms.Length = 0 then
            return [||]
        else
            let statement = db.prepare(searchSql).bind(String.concat " OR " terms)
            let! found = statement.all<{| id: string |}>() |> Async.AwaitPromise
            return found.results |> Array.map (fun row -> row.id)
    }
