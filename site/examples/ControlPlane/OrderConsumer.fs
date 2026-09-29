module OrderConsumer

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let attach (compute: ComputeClient) accountId queueId =
    task {
        let worker =
            mq_worker_u002D_consumer_u002D_request.Create("guestbook", mq_worker_u002D_consumer_u002D_requestType.Worker)
        match! compute.QueuesCreateConsumer(queueId, accountId, body = mq_consumer_u002D_request.Variant1 worker) with
        | QueuesCreateConsumer.OK _ ->
            printfn "guestbook now consumes the orders queue"
        | QueuesCreateConsumer.Status4XX(status, failure) ->
            for error in Option.defaultValue [] failure.errors do
                printfn "HTTP %d: %s" status error.message
    }
