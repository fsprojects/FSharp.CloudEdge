module MissingFiles

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let check (compute: ComputeClient) accountId (assets: Manifest.Asset list) =
    task {
        let request = workers_create_u002D_assets_u002D_upload_u002D_session_u002D_object.Create(Manifest.manifest assets)
        match! compute.WorkerScriptUpdateCreateAssetsUploadSession(accountId, "hello-worker", request) with
        | WorkerScriptUpdateCreateAssetsUploadSession.OK response ->
            match response.result with
            | Some { jwt = Some token; buckets = buckets } ->
                let buckets = defaultArg buckets []
                printfn "%d of %d files need uploading" (List.sumBy List.length buckets) assets.Length
                return Some(token, buckets)
            | _ -> return None
        | WorkerScriptUpdateCreateAssetsUploadSession.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "Upload session refused with HTTP %d: %s" status error.message
            return None
    }
