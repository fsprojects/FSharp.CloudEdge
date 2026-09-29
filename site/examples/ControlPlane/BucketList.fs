module BucketList

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Storage

let listBuckets (storage: StorageClient) accountId =
    task {
        match! storage.R2ListBuckets(accountId) with
        | R2ListBuckets.OK payload ->
            for bucket in payload.result["buckets"].AsArray() do
                printfn "%O  created %O" bucket["name"] bucket["creation_date"]
        | R2ListBuckets.Status4XX(status, _) ->
            printfn "HTTP %d" status
    }
