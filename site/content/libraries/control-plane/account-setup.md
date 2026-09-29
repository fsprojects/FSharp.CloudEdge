---
title: Account Setup
description: Create databases, buckets and namespaces from an F# program.
order: 21
---

<div class="ce-block-head">
<p class="ce-block-lead">From an F# program, create the storage your Worker binds to and list what the account already has. <code>Management.Storage</code> has the storage operations, and <code>Management.Compute</code> has the queue operations.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Management.Storage</code> <code>Management.Compute</code></li>
<li><span>D1 free</span> <a href="https://developers.cloudflare.com/workers/platform/pricing/">5 GB of storage</a></li>
<li><span>R2 free</span> <a href="https://developers.cloudflare.com/r2/pricing/">10 GB-month of storage</a></li>
<li><span>KV free</span> <a href="https://developers.cloudflare.com/workers/platform/pricing/">1 GB of storage</a></li>
<li><span>Queues free</span> <a href="https://developers.cloudflare.com/workers/platform/pricing/">10,000 operations a day</a></li>
</ul>
</div>

## Guestbook Database

`D1CreateDatabase` returns the new database's details as a `JsonObject`. Its `uuid` field is the `database_id` of a D1 binding.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Storage

let createDatabase (storage: StorageClient) accountId =
    task {
        match! storage.D1CreateDatabase(accountId, D1CreateDatabasePayload.Create "guestbook") with
        | D1CreateDatabase.OK payload ->
            let databaseId = string payload.result["uuid"]
            printfn "D1 database guestbook: %s" databaseId
            return Some databaseId
        | D1CreateDatabase.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
            return None
    }
```

<div class="ce-needs"><p><strong>Needs</strong> a <code>StorageClient</code> and your account ID, as in the <a href="/FSharp.CloudEdge/libraries/control-plane/">Control Plane</a> API Client.</p></div>

## Photo Bucket

Request records have a `Create` method that takes the required fields and sets the rest to `None`. A `with` expression then sets one of those fields. Here it adds a location hint for Western North America, which Cloudflare treats as best effort.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Storage

let createBucket (storage: StorageClient) accountId =
    task {
        let bucket = { R2CreateBucketPayload.Create "photos" with locationHint = Some r2_bucket_location.Wnam }
        match! storage.R2CreateBucket(accountId, bucket) with
        | R2CreateBucket.OK payload ->
            printfn "R2 bucket %O in %O" payload.result["name"] payload.result["location"]
            return true
        | R2CreateBucket.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
            return false
    }
```

## Session Store

The new namespace's `id` is the `namespace_id` of a `kv_namespace` binding. Cloudflare returns a 400 when the account already has a namespace with the same title, so running it twice gives the `Status4XX` case.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Storage

let createNamespace (storage: StorageClient) accountId =
    task {
        let body = workers_u002D_kv_create_namespace_body.Create "sessions"
        match! storage.WorkersKvNamespaceCreateANamespace(accountId, body) with
        | WorkersKvNamespaceCreateANamespace.OK payload ->
            let namespaceId = payload.result |> Option.map (fun created -> created.id)
            printfn "KV namespace sessions: %A" namespaceId
            return namespaceId
        | WorkersKvNamespaceCreateANamespace.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
            return None
    }
```

## Order Queue

`QueuesCreate` takes its body as an optional argument, so the call passes it by name. The consumer example on [Worker Upload](worker-upload.md) takes the `queue_id` from this result.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute

let createQueue (compute: ComputeClient) accountId =
    task {
        match! compute.QueuesCreate(accountId, body = QueuesCreatePayload.Create "orders") with
        | QueuesCreate.OK payload ->
            let queueId = payload.result |> Option.bind (fun queue -> queue.queue_id)
            printfn "Queue orders: %A" queueId
            return queueId
        | QueuesCreate.Status4XX(status, failure) ->
            for error in Option.defaultValue [] failure.errors do
                printfn "HTTP %d: %s" status error.message
            return None
    }
```

## Bucket List

`R2ListBuckets` returns its result as a `JsonObject` with a `buckets` array, and the loop reads each bucket's `name` and `creation_date`.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Storage

let listBuckets (storage: StorageClient) accountId =
    task {
        match! storage.R2ListBuckets(accountId) with
        | R2ListBuckets.OK payload ->
            for bucket in payload.result["buckets"].AsArray() do
                printfn "%O  created %O" bucket["name"] bucket["creation_date"]
        | R2ListBuckets.Status4XX(status, _) ->
            printfn "HTTP %d" status
    }
```

## Resource Inventory

`WorkersKvNamespaceListNamespaces` and `QueuesList` return F# records. Optional schema fields are `option` values, such as a queue's `queue_id`.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Compute
open FSharp.CloudEdge.Management.Storage

let listNamespaces (storage: StorageClient) accountId =
    task {
        match! storage.WorkersKvNamespaceListNamespaces(accountId, perPage = 100.) with
        | WorkersKvNamespaceListNamespaces.OK payload ->
            for kv in Option.defaultValue [] payload.result do
                printfn "KV     %s  %s" kv.id kv.title
        | WorkersKvNamespaceListNamespaces.Status4XX(status, _) ->
            printfn "HTTP %d" status
    }

let listQueues (compute: ComputeClient) accountId =
    task {
        match! compute.QueuesList(accountId) with
        | QueuesList.OK payload ->
            for queue in Option.defaultValue [] payload.result do
                printfn "Queue  %A  %A" queue.queue_id queue.queue_name
        | QueuesList.Status4XX(status, _) ->
            printfn "HTTP %d" status
    }
```

## Library Table

| Library | Operations | What it covers |
| --- | ---: | --- |
| `Management.Storage` | 153 | R2, D1, Workers KV, Vectorize, Hyperdrive |
| `Management.Compute` | 304 | Workers, Queues, Workflows, Pages |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Bindings for these resources</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/storage/"><strong>Storage</strong><span>KV, R2 and D1 in a Worker</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/background-work/"><strong>Background Work</strong><span>Queue messages in a Worker</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/"><strong>Control Plane</strong><span>Clients and the API token</span></a>
</div>
