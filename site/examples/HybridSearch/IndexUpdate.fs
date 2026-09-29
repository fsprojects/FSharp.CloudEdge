module IndexUpdate

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

let deleteSql = "DELETE FROM chunks WHERE id = ?"
let insertSql = "INSERT INTO chunks (id, page, title, text, hash) VALUES (?, ?, ?, ?, ?)"

let reindex (db: Workers.D1Database) (index: Workers.Vectorize) ai (page: ChangedChunks.Page) =
    async {
        do! db.exec KeywordQuery.schema |> Async.AwaitPromise |> Async.Ignore
        let! changes = ChangedChunks.changedChunks db page
        if changes.write.Length > 0 then
            let texts = changes.write |> Array.map (fun chunk -> chunk.title + "\n" + chunk.text)
            let! embeddings = VectorQuery.embed ai texts
            let vectors =
                Array.zip changes.write embeddings
                |> Array.map (fun (chunk, values) -> Workers.VectorizeVector.Create(chunk.id, U3.Case1 values))
            do! index.upsert vectors |> Async.AwaitPromise |> Async.Ignore
        if changes.delete.Length > 0 then
            do! index.deleteByIds changes.delete |> Async.AwaitPromise |> Async.Ignore
        let removed = Array.append (changes.write |> Array.map (fun chunk -> chunk.id)) changes.delete
        let statements =
            [| for id in removed -> db.prepare(deleteSql).bind id
               for chunk in changes.write ->
                   db.prepare(insertSql).bind(chunk.id, page.path, chunk.title, chunk.text, chunk.hash) |]
        if statements.Length > 0 then
            do! db.batch statements |> Async.AwaitPromise |> Async.Ignore
        return
            {| written = changes.write.Length
               deleted = changes.delete.Length
               unchanged = page.chunks.Length - changes.write.Length |}
    }
