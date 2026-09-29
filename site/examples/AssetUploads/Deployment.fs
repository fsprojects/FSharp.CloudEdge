module Deployment

open System.IO
open System.Text.Json
open System.Text.Json.Nodes
open FSharp.CloudEdge.Core.Api.Http
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let uploadWorker (compute: ComputeClient) accountId (metadata: JsonObject) =
    task {
        let worker: MultipartFile =
            { Bytes = File.ReadAllBytes "dist/worker.js"
              FileName = "worker.js"
              ContentType = Some "application/javascript+module"
              PartName = Some "worker.js" }
        match! compute.WorkerScriptUploadWorkerModule(accountId, "hello-worker", metadata, files = [ worker ]) with
        | WorkerScriptUploadWorkerModule.OK payload ->
            printfn "Deployed hello-worker, has assets: %b" (payload.result.has_assets = Some true)
        | WorkerScriptUploadWorkerModule.Status4XX(status, failure) ->
            printfn "Deployment rejected with HTTP %d: %O" status failure.JsonValue
    }

let deploy compute accountId (completionToken: string) =
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           assets = {| jwt = completionToken |} |}
    uploadWorker compute accountId (JsonSerializer.SerializeToNode(settings).AsObject())
