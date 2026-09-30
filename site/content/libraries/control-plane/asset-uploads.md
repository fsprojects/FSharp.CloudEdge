---
title: Asset Uploads
description: Upload only the files that changed.
order: 24
---

<div class="ce-block-head">
<p class="ce-block-lead">Serve static files, such as your site's pages and images, from hello-worker. Your program sends Cloudflare a manifest of file hashes and uploads only the new and changed files. It then uploads hello-worker with a completion token, and Cloudflare attaches the files to the new version.</p>
<ul class="ce-facts">
<li><span>Library</span> <code>Management.Compute</code></li>
<li><span>Operations</span> <code>WorkerScriptUpdateCreateAssetsUploadSession</code> <code>WorkerAssetsUpload</code> <code>WorkerScriptUploadWorkerModule</code></li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/workers/static-assets/billing-and-limitations/">Requests to static assets are free and unlimited</a></li>
<li><span>Files</span> <a href="https://developers.cloudflare.com/workers/platform/limits/">20,000 per Worker version on Free, 25 MiB each</a></li>
</ul>
</div>

## File Hash

Each file in the manifest has a hash of 32 hexadecimal characters. `hashFile` computes SHA-256 over the file's base64 text followed by its extension, such as `html`, and keeps the first 16 bytes. The example in Cloudflare's [direct-upload guide](https://developers.cloudflare.com/workers/static-assets/direct-upload/) hashes the same inputs. With the extension in the input, `about.txt` and `about.html` with the same bytes are two assets, each uploaded with its own content type.

```fsharp
module FileHash

open System
open System.IO
open System.Security.Cryptography
open System.Text

let hashFile (path: string) =
    let base64 = Convert.ToBase64String(File.ReadAllBytes path)
    let extension = Path.GetExtension(path).TrimStart '.'
    let digest = SHA256.HashData(Encoding.UTF8.GetBytes(base64 + extension))
    Convert.ToHexString(digest, 0, 16).ToLowerInvariant()
```

## Manifest

`scan` returns an `Asset` record for each file under a folder. The key is the URL path of the file, such as `/index.html`. `manifest` builds the request's map from each key to the file's hash and its size in bytes. In generated type names such as `workers_manifest_u002D_value`, `_u002D_` stands for a hyphen in the schema name.

```fsharp
module Manifest

open System.IO
open FSharp.CloudEdge.Core.Api.Types

type Asset = { Key: string; Path: string; Hash: string; Size: int }

let scan (folder: string) =
    [ for path in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories) ->
          { Key = "/" + Path.GetRelativePath(folder, path).Replace('\\', '/')
            Path = path
            Hash = FileHash.hashFile path
            Size = int (FileInfo path).Length } ]

let manifest (assets: Asset list) =
    assets
    |> List.map (fun asset -> asset.Key, workers_manifest_u002D_value.Create(asset.Hash, asset.Size))
    |> Map.ofList
```

## Missing-File Check

`WorkerScriptUpdateCreateAssetsUploadSession` posts the manifest for hello-worker. The reply contains an upload token and `buckets`, which are groups of hashes to upload together. Files already uploaded for a recent version of hello-worker are left out of the buckets. When Cloudflare already has every file, the buckets are empty and the token is the completion token for the deployment.

```fsharp
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
```

:::warning
The example response in the direct-upload guide has `"errors": null` and `"messages": null`. The 0.1.0 client reads both fields as lists, and the call throws a `JsonException` when either one is `null`. The client decodes a response that has empty lists in both fields.
:::

## Batched Upload

`upload` sends each bucket in one `WorkerAssetsUpload` request. These requests require the upload token in place of your API token, so `upload` creates a second `HttpClient` with that bearer token. Each file is one form field. The field name is the file's hash, and the value is the base64 contents. Cloudflare serves each file with the content type of its field.

The reply to the last bucket, HTTP 201, contains the completion token. The upload token and the completion token are each valid for one hour.

```fsharp
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
```

## Deployment

The deployment is the Worker upload from [First Deploy](../../guide/first-deploy.md), with the completion token in the metadata's `assets.jwt`. By default, Cloudflare [serves a request whose URL matches a file](https://developers.cloudflare.com/workers/static-assets/) from the assets and invokes the Worker for every other request.

```fsharp
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
```

<div class="ce-needs"><p><strong>Needs</strong> <code>dist/worker.js</code> from <a href="/FSharp.CloudEdge/guide/first-worker/">First Worker</a>.</p></div>

## Code-Only Redeploy

When only `dist/worker.js` has changed, call `redeploy`. It sets `keep_assets` in the metadata in place of a completion token. The new version then has the same assets as the current one, and the program skips the upload session and the buckets.

```fsharp
module CodeOnlyRedeploy

open System.Text.Json

let redeploy compute accountId =
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           keep_assets = true |}
    Deployment.uploadWorker compute accountId (JsonSerializer.SerializeToNode(settings).AsObject())
```

## Site Publisher

`publish` calls the modules above in order for the `public` folder in hello-worker. With `--code-only`, it calls the code-only redeploy.

```fsharp
module Program

open FSharp.CloudEdge.Management.Compute

let publish (args: string[]) =
    task {
        use http = ClientSetup.cloudflareHttp ()
        let compute = ComputeClient http
        let accountId = ClientSetup.accountId ()
        if args = [| "--code-only" |] then
            do! CodeOnlyRedeploy.redeploy compute accountId
        else
            let assets = Manifest.scan "public"
            match! MissingFiles.check compute accountId assets with
            | Some(completionToken, []) -> do! Deployment.deploy compute accountId completionToken
            | Some(uploadToken, buckets) ->
                match! BatchedUpload.upload accountId uploadToken assets buckets with
                | Some completionToken -> do! Deployment.deploy compute accountId completionToken
                | None -> printfn "The upload returned no completion token"
            | None -> ()
    }

[<EntryPoint>]
let main args =
    (publish args).GetAwaiter().GetResult()
    0
```

<div class="ce-needs"><p><strong>Needs</strong> a <code>public</code> folder in hello-worker and the <code>ClientSetup</code> module from <a href="/FSharp.CloudEdge/guide/credentials/">Credentials</a>. The publisher's project file is like the one on <a href="/FSharp.CloudEdge/guide/first-deploy/">First Deploy</a>, and its <code>Compile</code> list contains <code>ClientSetup.fs</code>, then the seven files of this page in order. Start it from hello-worker, since it reads <code>public</code> and <code>dist/worker.js</code> relative to the working directory.</p></div>

On the first run, all four files in `public` are new.

```text
4 of 4 files need uploading
Uploading bucket 1 of 2
Uploading bucket 2 of 2
Deployed hello-worker, has assets: true
```

On the second run, the four files are the same as before.

```text
0 of 4 files need uploading
Deployed hello-worker, has assets: true
```

On the third run, `style.css` has new contents.

```text
1 of 4 files need uploading
Uploading bucket 1 of 1
Deployed hello-worker, has assets: true
```

These runs were recorded against a local test server in place of Cloudflare's API. That server returned buckets of two hashes.

## Library Table

| Library | Operations | What it covers |
| --- | ---: | --- |
| `Management.Compute` | 304 | Asset and Worker uploads |
| `Core.Api` | 0 | Multipart fields and response types |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/first-deploy/"><strong>First Deploy</strong><span>hello-worker's first upload</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Bindings and schedules</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/hybrid-search/"><strong>Hybrid Search</strong><span>Reindexing only changed chunks</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/clients/"><strong>Client Catalog</strong><span>Every control-plane client</span></a>
</div>

## NuGet packages

[Core.Api 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Core.Api/0.1.0), [Management.Compute 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Compute/0.1.0).

See [installation and release availability](../../guide/packages.md).
