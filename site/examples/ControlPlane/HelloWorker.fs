module HelloWorker

open System.IO
open System.Text.Json
open FSharp.CloudEdge.Core.Api.Http
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let upload (compute: ComputeClient) accountId =
    task {
        let workerModule: MultipartFile =
            { Bytes = File.ReadAllBytes "dist/worker.js"
              FileName = "worker.js"
              ContentType = Some "application/javascript+module"
              PartName = Some "worker.js" }
        let metadata =
            JsonSerializer.SerializeToNode({| main_module = "worker.js"; compatibility_date = "2026-09-06" |}).AsObject()
        match! compute.WorkerScriptUploadWorkerModule(accountId, "hello", metadata, files = [ workerModule ]) with
        | WorkerScriptUploadWorkerModule.OK payload ->
            printfn "hello uploaded, startup %d ms" payload.result.startup_time_ms
            printfn "Handlers: %A" payload.result.handlers
        | WorkerScriptUploadWorkerModule.Status4XX(status, _) ->
            printfn "Upload rejected with HTTP %d" status
    }
