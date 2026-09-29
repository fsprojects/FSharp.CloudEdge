module WorkerAddress

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let show (compute: ComputeClient) accountId =
    task {
        match! compute.WorkerScriptGetSubdomain(accountId, "hello-worker") with
        | WorkerScriptGetSubdomain.OK route when not route.result.enabled ->
            let enable = WorkerScriptPostSubdomainPayload.Create true
            match! compute.WorkerScriptPostSubdomain(accountId, "hello-worker", enable) with
            | WorkerScriptPostSubdomain.OK _ -> printfn "Turned on the workers.dev address"
            | WorkerScriptPostSubdomain.Status4XX(status, _) -> printfn "workers.dev setting returned HTTP %d" status
        | _ -> ()
        match! compute.WorkerSubdomainGetSubdomain accountId with
        | WorkerSubdomainGetSubdomain.OK account ->
            printfn "https://hello-worker.%s.workers.dev" account.result.subdomain
        | WorkerSubdomainGetSubdomain.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "Subdomain lookup returned HTTP %d: %s" status error.message
    }
