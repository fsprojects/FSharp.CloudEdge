---
title: First Deploy
description: Upload your Worker to Cloudflare from an F# program.
order: 5
---

<div class="ce-block-head">
<p class="ce-block-lead">A short F# program uploads <code>dist/worker.js</code> as the Worker hello-worker and prints its workers.dev address. It saves a fingerprint of each upload and skips the upload on a later run with an unchanged bundle.</p>
<ul class="ce-facts">
<li><span>You need</span> <code>dist/worker.js</code> from <a href="/FSharp.CloudEdge/guide/first-worker/">First Worker</a>, <code>.env</code> from <a href="/FSharp.CloudEdge/guide/credentials/">Credentials</a></li>
<li><span>You get</span> A deploy program for hello-worker that skips unchanged bundles</li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/workers/platform/pricing/">100,000 requests a day</a></li>
</ul>
</div>

## Project Folder

The upload program is a .NET console project in a `deploy` folder inside hello-worker. It references two NuGet packages at version `0.1.0`: `Core.Api` for the request and response types, and `Management.Compute` for `ComputeClient`.

1. From your hello-worker folder, create `deploy`.

   ```bash
   mkdir deploy
   ```

2. Save the `ClientSetup` module from [Credentials](credentials.md) as `deploy/ClientSetup.fs`. It reads your two variables and returns the `HttpClient` for `ComputeClient`.

3. Create `deploy/Deploy.fsproj` with this content.

   ```xml
   <Project Sdk="Microsoft.NET.Sdk">

     <PropertyGroup>
       <OutputType>Exe</OutputType>
       <TargetFramework>net10.0</TargetFramework>
     </PropertyGroup>

     <ItemGroup>
       <Compile Include="ClientSetup.fs" />
       <Compile Include="ChangeCheck.fs" />
       <Compile Include="WorkerUpload.fs" />
       <Compile Include="WorkerAddress.fs" />
       <Compile Include="Program.fs" />
     </ItemGroup>

     <ItemGroup>
       <PackageReference Include="FSharp.CloudEdge.Core.Api" Version="0.1.0" />
       <PackageReference Include="FSharp.CloudEdge.Management.Compute" Version="0.1.0" />
     </ItemGroup>

   </Project>
   ```

The F# compiler processes the files in the order of the `Compile` list, so each module can call only the modules listed above it. Save the four files below in `deploy`.

## Change Check

`fingerprint` hashes the upload's metadata and the bundle together with SHA-256. A changed bundle or a new compatibility date means a new fingerprint. After a successful upload, `remember` writes the fingerprint to `dist/last-upload.txt`. On the next run, `unchanged` compares the new fingerprint with that file.

```fsharp
module ChangeCheck

open System
open System.IO
open System.Security.Cryptography
open System.Text

let lastUpload = "dist/last-upload.txt"

let fingerprint (metadata: string) (bundle: byte[]) =
    Array.append (Encoding.UTF8.GetBytes metadata) bundle
    |> SHA256.HashData
    |> Convert.ToHexString

let unchanged (fingerprint: string) =
    File.Exists lastUpload && File.ReadAllText lastUpload = fingerprint

let remember (fingerprint: string) =
    File.WriteAllText(lastUpload, fingerprint)
```

## Worker Upload

`upload` sends the metadata and the bundle in one multipart request. In the metadata, `main_module` is the name of the part that contains the entry module, and the bundle's `PartName` is that name. Cloudflare's [infrastructure-as-code guide](https://developers.cloudflare.com/workers/platform/infrastructure-as-code/) states that this upload creates a version and a deployment.

Per Cloudflare's [metadata reference](https://developers.cloudflare.com/workers/configuration/multipart-upload-metadata/), the default for an API upload without `compatibility_date` is the oldest compatibility date, 2021-11-02. `metadata` has the date 2026-09-06, the same as `compatibilityDate` in `config.capnp` on [First Worker](first-worker.md).

```fsharp
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
```

## Worker Address

A Worker's workers.dev address has the form `<worker>.<subdomain>.workers.dev`, and the subdomain belongs to your account. `WorkerScriptGetSubdomain` returns whether the address of hello-worker is on. When it is off, `show` turns it on with `WorkerScriptPostSubdomain`. `WorkerSubdomainGetSubdomain` returns your account's subdomain, which the dashboard also shows under **Your subdomain** on the **Workers & Pages** page.

```fsharp
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
```

## Program Flow

`deploy` calls functions from the three modules above, in order. When the fingerprint matches `dist/last-upload.txt`, `deploy` skips the upload and prints the address. When Cloudflare rejects an upload, `upload` prints the HTTP status and Cloudflare's JSON reply, and the program exits with code 1.

```fsharp
module Program

open System.IO
open FSharp.CloudEdge.Management.Compute

let deploy () =
    task {
        use http = ClientSetup.cloudflareHttp ()
        let compute = ComputeClient http
        let accountId = ClientSetup.accountId ()
        let bundle = File.ReadAllBytes "dist/worker.js"
        let fingerprint = ChangeCheck.fingerprint (WorkerUpload.metadata.ToJsonString()) bundle
        if ChangeCheck.unchanged fingerprint then
            printfn "No change since the last upload"
        else
            let! uploaded = WorkerUpload.upload compute accountId bundle
            if not uploaded then exit 1
            ChangeCheck.remember fingerprint
        do! WorkerAddress.show compute accountId
    }

deploy().GetAwaiter().GetResult()
```

## First Run

1. Open a terminal in your hello-worker folder and load `.env`, as on [Credentials](credentials.md).

   ```bash
   set -a
   source .env
   set +a
   ```

   If you skip this step, `ClientSetup` reports the name of the missing variable and the program exits.

2. Start the program from hello-worker, since it reads `dist/worker.js` relative to the working directory.

   ```bash
   dotnet run --project deploy -c Release
   ```

   NuGet restores the versioned client packages. `-c Release` selects the build configuration for your deploy program.

   ```text
   Uploaded hello-worker, startup time 4 ms
   Turned on the workers.dev address
   https://hello-worker.your-subdomain.workers.dev
   ```

   This output was recorded against a local test server in place of Cloudflare's API. Your subdomain and startup time will differ, and the second line appears only when the workers.dev address was off.

3. Open the printed address in a browser and compare the reply with the one workerd returned on [First Worker](first-worker.md).

## Repeat Run

Repeat the command.

```bash
dotnet run --project deploy -c Release
```

```text
No change since the last upload
https://hello-worker.your-subdomain.workers.dev
```

On the second run, the program sends only two read requests, both for the address. After you change `Worker.fs`, build `dist/worker.js` again as on [First Worker](first-worker.md), and the program uploads the new bundle on the following run.

## Next Step

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/asset-uploads/"><strong>Asset Uploads</strong><span>Static files for hello-worker</span></a>
</div>
