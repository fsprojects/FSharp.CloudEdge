module FSharp.CloudEdge.Support.Workers.DurableObjects

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type NativeRequest = Workers.Request<obj, U2<Workers.RequestInitCfProperties, Workers.IncomingRequestCfProperties<obj>>>

/// Fetch transport over the generated opaque Durable Object stub result.
[<Interface>]
type FetchTransport =
    abstract fetch: request: NativeRequest -> JS.Promise<Workers.Response>

[<Emit("(() => { if ($0 == null || typeof $0.fetch !== 'function') throw new TypeError('Durable Object stub must expose fetch'); return $0; })()")>]
let private checkedFetchTransport (stub: obj) : FetchTransport = jsNative

/// Requires a callable fetch member and retains the original stub as the receiver.
let requireFetchTransport (stub: obj) : FetchTransport = checkedFetchTransport stub

let getFetchById (namespaceBinding: Workers.DurableObjectNamespace<'T>) (id: Workers.DurableObjectId) : FetchTransport =
    namespaceBinding.get id |> requireFetchTransport

let getFetchByName (namespaceBinding: Workers.DurableObjectNamespace<'T>) (name: string) : FetchTransport =
    namespaceBinding.idFromName name |> getFetchById namespaceBinding
