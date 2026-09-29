module GuestbookDatabase

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Storage

let createDatabase (storage: StorageClient) accountId =
    task {
        match! storage.D1CreateDatabase(accountId, D1CreateDatabasePayload.Create "guestbook") with
        | D1CreateDatabase.OK payload ->
            let databaseId = string payload.result["uuid"]
            printfn "D1 database guestbook: %s" databaseId
            return Some databaseId
        | D1CreateDatabase.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
            return None
    }
