module PhotoBucket

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Storage

let createBucket (storage: StorageClient) accountId =
    task {
        let bucket = { R2CreateBucketPayload.Create "photos" with locationHint = Some r2_bucket_location.Wnam }
        match! storage.R2CreateBucket(accountId, bucket) with
        | R2CreateBucket.OK payload ->
            printfn "R2 bucket %O in %O" payload.result["name"] payload.result["location"]
            return true
        | R2CreateBucket.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
            return false
    }
