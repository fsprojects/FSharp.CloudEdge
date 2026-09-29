module SessionStore

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
