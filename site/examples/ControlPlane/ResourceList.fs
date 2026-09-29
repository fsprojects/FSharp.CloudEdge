module ResourceList

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute
open FSharp.CloudEdge.Management.Storage

let listNamespaces (storage: StorageClient) accountId =
    task {
        match! storage.WorkersKvNamespaceListNamespaces(accountId, perPage = 100.) with
        | WorkersKvNamespaceListNamespaces.OK payload ->
            for kv in Option.defaultValue [] payload.result do
                printfn "KV     %s  %s" kv.id kv.title
        | WorkersKvNamespaceListNamespaces.Status4XX(status, _) ->
            printfn "HTTP %d" status
    }

let listQueues (compute: ComputeClient) accountId =
    task {
        match! compute.QueuesList(accountId) with
        | QueuesList.OK payload ->
            for queue in Option.defaultValue [] payload.result do
                printfn "Queue  %A  %A" queue.queue_id queue.queue_name
        | QueuesList.Status4XX(status, _) ->
            printfn "HTTP %d" status
    }
