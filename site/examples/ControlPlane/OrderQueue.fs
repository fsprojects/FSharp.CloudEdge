module OrderQueue

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let createQueue (compute: ComputeClient) accountId =
    task {
        match! compute.QueuesCreate(accountId, body = QueuesCreatePayload.Create "orders") with
        | QueuesCreate.OK payload ->
            let queueId = payload.result |> Option.bind (fun queue -> queue.queue_id)
            printfn "Queue orders: %A" queueId
            return queueId
        | QueuesCreate.Status4XX(status, failure) ->
            for error in Option.defaultValue [] failure.errors do
                printfn "HTTP %d: %s" status error.message
            return None
    }
