module PublicUrl

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let publish (compute: ComputeClient) accountId (scriptName: string) =
    task {
        let enable = WorkerScriptPostSubdomainPayload.Create true
        match! compute.WorkerScriptPostSubdomain(accountId, scriptName, enable) with
        | WorkerScriptPostSubdomain.OK _ ->
            match! compute.WorkerSubdomainGetSubdomain(accountId) with
            | WorkerSubdomainGetSubdomain.OK account ->
                printfn "https://%s.%s.workers.dev" scriptName account.result.subdomain
            | WorkerSubdomainGetSubdomain.Status4XX(status, _) ->
                printfn "Subdomain lookup returned HTTP %d" status
        | WorkerScriptPostSubdomain.Status4XX(status, _) ->
            printfn "workers.dev setting returned HTTP %d" status
    }
