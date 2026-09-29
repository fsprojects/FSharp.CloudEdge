module NightlyCleanup

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let schedule (compute: ComputeClient) accountId =
    task {
        let nightly = workers_schedule.Create "0 3 * * *"
        match! compute.WorkerCronTriggerUpdateCronTriggers(accountId, "guestbook", [ nightly ]) with
        | WorkerCronTriggerUpdateCronTriggers.OK payload ->
            for trigger in payload.result.schedules do
                printfn "guestbook runs on %s" trigger.cron
        | WorkerCronTriggerUpdateCronTriggers.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
    }
