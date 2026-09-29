module GuardedEndpoint

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Capnweb = FSharp.CloudEdge.Runtime.Capnweb

let options =
    Capnweb.RpcSessionOptions.Create(
        onSendError = (fun error ->
            JS.console.error error
            None),
        limits = Capnweb.RpcSessionOptions.Limits.Create(maxMessageSize = 65536.)
    )

let onRequest (context: Workers.EventContext<obj, string, obj>) =
    Capnweb.Exports.newWorkersRpcResponse(context.request, Pricing.PricingApi(), options)
