module BatchedUpload

open System
open System.IO
open System.Net.Http
open System.Net.Http.Headers
open FSharp.CloudEdge.Core.Api.Http
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let contentType (path: string) =
    match Path.GetExtension(path).ToLowerInvariant() with
    | ".html" -> "text/html"
    | ".css" -> "text/css"
    | ".js" -> "text/javascript"
    | ".svg" -> "image/svg+xml"
    | ".png" -> "image/png"
    | _ -> "application/octet-stream"

let upload accountId (uploadToken: string) (assets: Manifest.Asset list) (buckets: string list list) =
    task {
        use http = new HttpClient(BaseAddress = Uri "https://api.cloudflare.com/client/v4")
        http.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", uploadToken)
        let uploads = ComputeClient http
        let pathOf = assets |> List.map (fun asset -> asset.Hash, asset.Path) |> Map.ofList
        let mutable completionToken = None
        for index, bucket in List.indexed buckets do
            printfn "Uploading bucket %d of %d" (index + 1) buckets.Length
            let files: MultipartTextField list =
                [ for hash in bucket ->
                      { Name = hash
                        Value = Convert.ToBase64String(File.ReadAllBytes pathOf[hash])
                        ContentType = contentType pathOf[hash] } ]
            match! uploads.WorkerAssetsUpload(accountId, true, files) with
            | WorkerAssetsUpload.Created finished -> completionToken <- finished.result |> Option.bind (fun result -> result.jwt)
            | WorkerAssetsUpload.Accepted _ -> ()
            | WorkerAssetsUpload.Status4XX(status, _) -> printfn "Bucket %d refused with HTTP %d" (index + 1) status
        return completionToken
    }
