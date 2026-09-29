module ChangedChunks

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Page = {| path: string; chunks: ChunkHash.Chunk[] |}

let storedSql = "SELECT id, hash FROM chunks WHERE page = ?"

let changedChunks (db: Workers.D1Database) (page: Page) =
    async {
        let! hashed = page.chunks |> Array.map ChunkHash.chunkHash |> Async.Parallel
        let statement = db.prepare(storedSql).bind(page.path)
        let! stored = statement.all<{| id: string; hash: string |}>() |> Async.AwaitPromise
        let known = stored.results |> Array.map (fun row -> row.id, row.hash) |> Map.ofArray
        let storedIds = stored.results |> Array.map (fun row -> row.id) |> Set.ofArray
        let incomingIds = page.chunks |> Array.map (fun chunk -> chunk.id) |> Set.ofArray
        let changed = hashed |> Array.filter (fun chunk -> Map.tryFind chunk.id known <> Some chunk.hash)
        let stale = Set.difference storedIds incomingIds |> Set.toArray
        return {| write = changed; delete = stale |}
    }
