module WorkerUpload

open System.Text.Json
open FSharp.CloudEdge.Core.Api.Http
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let metadata =
    JsonSerializer.SerializeToNode({| main_module = "worker.js"; compatibility_date = "2026-09-06" |}).AsObject()

let upload (compute: ComputeClient) accountId (bundle: byte[]) =
    task {
        let worker: MultipartFile =
            { Bytes = bundle
              FileName = "worker.js"
              ContentType = Some "application/javascript+module"
              PartName = Some "worker.js" }
        match! compute.WorkerScriptUploadWorkerModule(accountId, "hello-worker", metadata, files = [ worker ]) with
        | WorkerScriptUploadWorkerModule.OK payload ->
            printfn "Uploaded hello-worker, startup time %d ms" payload.result.startup_time_ms
            return true
        | WorkerScriptUploadWorkerModule.Status4XX(status, failure) ->
            printfn "Upload rejected with HTTP %d: %O" status failure.JsonValue
            return false
    }
