---
title: Worker Upload
description: Upload a Worker and its bindings from an F# program.
order: 22
---

<div class="ce-block-head">
<p class="ce-block-lead">From an F# program, upload the JavaScript module that your Worker compiles to. The upload's metadata declares the Worker's bindings, and separate calls set its schedule and its workers.dev address.</p>
<ul class="ce-facts">
<li><span>Library</span> <code>Management.Compute</code></li>
<li><span>Operation</span> <code>WorkerScriptUploadWorkerModule</code></li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/workers/platform/pricing/">100,000 requests a day</a></li>
<li><span>Cron Triggers</span> <a href="https://developers.cloudflare.com/workers/platform/limits/">5 per account on Free</a></li>
</ul>
</div>

## Hello Worker

`WorkerScriptUploadWorkerModule` sends a multipart request with a `metadata` part and one part per module. In the metadata, `main_module` identifies the part with the entry module, and the file's `PartName` matches it.

```fsharp
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
```

<div class="ce-needs"><p><strong>Needs</strong> your Worker's JavaScript module at <code>dist/worker.js</code>. <a href="/FSharp.CloudEdge/guide/first-worker/">First Worker</a> covers the compile step.</p></div>

## Guestbook Bindings

Bindings are JSON objects in the metadata's `bindings` array. Anonymous records serialize with their field names as JSON keys, and double backticks make the keyword `type` usable as a field. Each binding's `name` is its JavaScript variable name inside the Worker.

```fsharp
open System.Text.Json
open System.Text.Json.Nodes

let metadata (databaseId: string) (namespaceId: string) =
    let bindings: JsonNode[] =
        [| JsonSerializer.SerializeToNode {| ``type`` = "d1"; name = "DB"; database_id = databaseId |}
           JsonSerializer.SerializeToNode {| ``type`` = "kv_namespace"; name = "SESSIONS"; namespace_id = namespaceId |}
           JsonSerializer.SerializeToNode {| ``type`` = "r2_bucket"; name = "PHOTOS"; bucket_name = "photos" |}
           JsonSerializer.SerializeToNode {| ``type`` = "queue"; name = "ORDERS"; queue_name = "orders" |}
           JsonSerializer.SerializeToNode {| ``type`` = "plain_text"; name = "SITE_TITLE"; text = "Guestbook" |} |]
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           bindings = bindings |}
    JsonSerializer.SerializeToNode(settings).AsObject()
```

A D1 binding takes `database_id`, which Cloudflare's [upload reference](https://developers.cloudflare.com/api/resources/workers/subresources/scripts/methods/update/) lists in place of the deprecated `id`.

<div class="ce-needs"><p><strong>Needs</strong> the storage and the queue from <a href="/FSharp.CloudEdge/libraries/control-plane/account-setup/">Account Setup</a>.</p></div>

## Chat Room

A Durable Object binding refers to the exported class by `class_name`. The upload that introduces the class includes a migration, and Cloudflare creates a SQLite-backed namespace for each class in `new_sqlite_classes`.

```fsharp
open System.Text.Json
open System.Text.Json.Nodes

let metadata () =
    let rooms: JsonNode =
        JsonSerializer.SerializeToNode {| ``type`` = "durable_object_namespace"; name = "ROOMS"; class_name = "ChatRoom" |}
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           bindings = [| rooms |]
           migrations = {| new_tag = "v1"; new_sqlite_classes = [| "ChatRoom" |] |} |}
    JsonSerializer.SerializeToNode(settings).AsObject()
```

For a later migration, set `old_tag` to the current migration tag and `new_tag` to the next one. Cloudflare rejects the upload when `old_tag` differs from the latest one.

<div class="ce-needs"><p><strong>Needs</strong> a Worker module that exports the <code>ChatRoom</code> class. <a href="/FSharp.CloudEdge/libraries/platform/durable-objects/">Durable Objects</a> has such classes in F#.</p></div>

## Payment Key

The program reads the key from an environment variable and uploads it in a `secret_text` binding. `keep_bindings` lists the binding types to keep from the previous upload, and with `secret_text` in that list, only the first upload includes the key.

```fsharp
open System
open System.Text.Json
open System.Text.Json.Nodes

let firstUpload () =
    let apiKey = Environment.GetEnvironmentVariable "PAYMENTS_API_KEY"
    let secret: JsonNode =
        JsonSerializer.SerializeToNode {| ``type`` = "secret_text"; name = "PAYMENTS_API_KEY"; text = apiKey |}
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           bindings = [| secret |] |}
    JsonSerializer.SerializeToNode(settings).AsObject()

let laterUpload () =
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           keep_bindings = [| "secret_text" |] |}
    JsonSerializer.SerializeToNode(settings).AsObject()
```

## Nightly Cleanup

`WorkerCronTriggerUpdateCronTriggers` takes the schedules as cron expressions. Cron Triggers run on UTC time, so this schedule fires at 03:00 UTC every day.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let schedule (compute: ComputeClient) accountId =
    task {
        let nightly = workers_schedule.Create "0 3 * * *"
        match! compute.WorkerCronTriggerUpdateCronTriggers(accountId, "guestbook", [ nightly ]) with
        | WorkerCronTriggerUpdateCronTriggers.OK payload ->
            for trigger in payload.result.schedules do
                printfn "guestbook runs on %s" trigger.cron
        | WorkerCronTriggerUpdateCronTriggers.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
    }
```

The Worker receives each run in the `scheduled` handler of its `ExportedHandler`. Per Cloudflare's documentation, a schedule change can take up to 15 minutes to propagate.

## Public URL

`WorkerScriptPostSubdomain` turns on the Worker's workers.dev route. The address has the form `<worker>.<subdomain>.workers.dev`, and `WorkerSubdomainGetSubdomain` returns the account's subdomain.

```fsharp
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
```

## Order Consumer

A Worker consumer receives the queue's messages in its `queue` handler. `QueuesCreateConsumer` takes the queue ID from [Account Setup](account-setup.md) and a consumer request, where `Variant1` is the Worker form and `Variant2` the HTTP pull form.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let attach (compute: ComputeClient) accountId queueId =
    task {
        let worker =
            mq_worker_u002D_consumer_u002D_request.Create("guestbook", mq_worker_u002D_consumer_u002D_requestType.Worker)
        match! compute.QueuesCreateConsumer(queueId, accountId, body = mq_consumer_u002D_request.Variant1 worker) with
        | QueuesCreateConsumer.OK _ ->
            printfn "guestbook now consumes the orders queue"
        | QueuesCreateConsumer.Status4XX(status, failure) ->
            for error in Option.defaultValue [] failure.errors do
                printfn "HTTP %d: %s" status error.message
    }
```

The request refers to the Worker by `script_name`, so the program attaches the consumer after the upload.

## Binding Kinds

Each row is one entry in the `bindings` array. The keys follow the binding schemas in Cloudflare's OpenAPI document.

| Binding | `type` | Keys |
| --- | --- | --- |
| D1 database | `d1` | `name`, `database_id` |
| KV namespace | `kv_namespace` | `name`, `namespace_id` |
| R2 bucket | `r2_bucket` | `name`, `bucket_name` |
| Queue producer | `queue` | `name`, `queue_name` |
| Durable Object | `durable_object_namespace` | `name`, `class_name` |
| Workflow | `workflow` | `name`, `workflow_name`, `class_name` |
| Workers AI | `ai` | `name` |
| AI Search namespace | `ai_search_namespace` | `name`, `namespace` |
| Vectorize index | `vectorize` | `name`, `index_name` |
| Analytics Engine | `analytics_engine` | `name`, `dataset` |
| Hyperdrive | `hyperdrive` | `name`, `id` |
| Service | `service` | `name`, `service` |
| Browser Rendering | `browser` | `name` |
| Images | `images` | `name` |
| Environment variable | `plain_text` | `name`, `text` |
| Secret | `secret_text` | `name`, `text` |

## Library Table

| Library | Operations | What it covers |
| --- | ---: | --- |
| `Management.Compute` | 304 | Uploads, schedules, queues, workers.dev |
| `Core.Api` | 0 | `MultipartFile` and the response types |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/account-setup/"><strong>Account Setup</strong><span>Storage and queues to bind</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/asset-uploads/"><strong>Asset Uploads</strong><span>Static files for a Worker</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/durable-objects/"><strong>Durable Objects</strong><span>Classes behind a binding</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/background-work/"><strong>Background Work</strong><span>Queue and scheduled handlers</span></a>
</div>
