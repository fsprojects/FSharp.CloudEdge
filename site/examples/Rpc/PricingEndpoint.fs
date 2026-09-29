module PricingEndpoint

module Workers = FSharp.CloudEdge.Runtime.Workers
module Capnweb = FSharp.CloudEdge.Runtime.Capnweb

let onRequest (context: Workers.EventContext<obj, string, obj>) =
    Capnweb.Exports.newWorkersRpcResponse(context.request, Pricing.PricingApi())
