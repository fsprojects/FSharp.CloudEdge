---
title: Control Plane
description: Cloudflare's REST API as F# clients.
order: 20
---

<div class="ce-block-head">
<p class="ce-block-lead">Account setup and deployment are F# programs that call Cloudflare's REST API. Eleven .NET clients have a method for each of 3,437 operations in that API, from creating a database to uploading a Worker.</p>
<ul class="ce-facts">
<li><span>Clients</span> 10 <code>Management</code>, 1 <code>Tenancy</code></li>
<li><span>Shared types</span> <code>Core.Api</code></li>
<li><span>Operations</span> 3,437</li>
<li><span>Target</span> <code>netstandard2.0</code></li>
</ul>
</div>

## API Client

Each client's constructor takes an `HttpClient`. The one below has the API root as its base address and sends a bearer token from the `CLOUDFLARE_API_TOKEN` environment variable with every request.

```fsharp
open System
open System.Net.Http
open System.Net.Http.Headers
open FSharp.CloudEdge.Management.Compute
open FSharp.CloudEdge.Management.Storage
open FSharp.CloudEdge.Tenancy

let connect () =
    let token = Environment.GetEnvironmentVariable "CLOUDFLARE_API_TOKEN"
    let http = new HttpClient(BaseAddress = Uri "https://api.cloudflare.com/client/v4")
    http.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", token)
    http

let accountId = Environment.GetEnvironmentVariable "CLOUDFLARE_ACCOUNT_ID"
let http = connect ()
let tenancy = TenancyClient http
let storage = StorageClient http
let compute = ComputeClient http
```

<div class="ce-needs"><p><strong>Needs</strong> an API token in <code>CLOUDFLARE_API_TOKEN</code> and your account ID in <code>CLOUDFLARE_ACCOUNT_ID</code>. <a href="/FSharp.CloudEdge/guide/credentials/">Credentials</a> covers both values.</p></div>

## Token Check

Every operation returns a `Task` of a union with one case for each response the operation declares. `UserApiTokensVerifyToken` declares `OK` and `Status4XX`, and `match!` handles both.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Tenancy

let checkToken (tenancy: TenancyClient) =
    task {
        match! tenancy.UserApiTokensVerifyToken() with
        | UserApiTokensVerifyToken.OK payload ->
            printfn "Token accepted: %b" payload.success
            return true
        | UserApiTokensVerifyToken.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d, code %d: %s" status error.code error.message
            return false
    }
```

The unions and payload types are in `FSharp.CloudEdge.Core.Api.Types`. A type for a named schema has the schema's name with `_u002D_` in place of each hyphen, as in `workers_u002D_kv_create_namespace_body`. Unions and inline request bodies are named after their operation, such as `D1CreateDatabasePayload`.

## Account Lookup

The client raises an exception for a status outside the declared cases, and for a response body whose JSON differs from its declared type. This lookup returns every outcome as a `Result`, and `try ... with` turns exceptions into `Error` values.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Tenancy

let findAccount (tenancy: TenancyClient) (name: string) =
    task {
        try
            match! tenancy.AccountsListAccounts(name = name) with
            | AccountsListAccounts.OK payload ->
                match Option.defaultValue [] payload.result with
                | account :: _ -> return Ok account.id
                | [] -> return Error $"No account named {name}"
            | AccountsListAccounts.Status4XX(status, failure) ->
                let details = failure.errors |> List.map (fun error -> error.message) |> String.concat "; "
                return Error $"HTTP {status}: {details}"
        with error ->
            return Error error.Message
    }
```

## Setup Program

This entry point starts with Token Check and then calls examples from [Account Setup](account-setup.md) and [Worker Upload](worker-upload.md) in order. It creates the storage and the queue, then uploads the Worker with bindings to them. After the upload it attaches the queue consumer and sets the schedule, and the last call turns on the workers.dev address.

```fsharp
open System.IO
open FSharp.CloudEdge.Core.Api.Http
open FSharp.CloudEdge.Core.Api.Types
open ApiClient

let uploadGuestbook databaseId namespaceId =
    let worker: MultipartFile =
        { Bytes = File.ReadAllBytes "dist/worker.js"
          FileName = "worker.js"
          ContentType = Some "application/javascript+module"
          PartName = Some "worker.js" }
    let metadata = GuestbookBindings.metadata databaseId namespaceId
    compute.WorkerScriptUploadWorkerModule(accountId, "guestbook", metadata, files = [ worker ])

let setup () =
    task {
        let! tokenAccepted = TokenCheck.checkToken tenancy
        let! databaseId = GuestbookDatabase.createDatabase storage accountId
        let! namespaceId = SessionStore.createNamespace storage accountId
        let! bucketCreated = PhotoBucket.createBucket storage accountId
        let! queueId = OrderQueue.createQueue compute accountId
        match databaseId, namespaceId, queueId with
        | Some databaseId, Some namespaceId, Some queueId when tokenAccepted && bucketCreated ->
            match! uploadGuestbook databaseId namespaceId with
            | WorkerScriptUploadWorkerModule.OK _ ->
                do! OrderConsumer.attach compute accountId queueId
                do! NightlyCleanup.schedule compute accountId
                do! PublicUrl.publish compute accountId "guestbook"
                return 0
            | WorkerScriptUploadWorkerModule.Status4XX(status, _) ->
                printfn "Upload rejected with HTTP %d" status
                return 1
        | _ -> return 1
    }

[<EntryPoint>]
let main _ = setup().GetAwaiter().GetResult()
```

## Library Table

| Library | Operations | What it covers |
| --- | ---: | --- |
| `Core.Api` | 0 | Payload types and HTTP transport |
| `Management.<Purpose>` | 3,137 | Ten clients, listed in the [Client Catalog](clients.md) |
| `Tenancy` | 300 | Accounts, members, API tokens, zones |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/account-setup/"><strong>Account Setup</strong><span>Storage and queues for a Worker</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Modules, bindings, schedules</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/clients/"><strong>Client Catalog</strong><span>Eleven clients by purpose</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/credentials/"><strong>Credentials</strong><span>Account ID and API token</span></a>
</div>

## NuGet packages

[Core.Api 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Core.Api/0.1.0), [Management.Compute 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Compute/0.1.0), [Management.Storage 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Storage/0.1.0), [Tenancy 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Tenancy/0.1.0).

See [installation and release availability](../../guide/packages.md).
