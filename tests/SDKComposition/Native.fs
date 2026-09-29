module CloudEdgeNativeComposition

module Workers = FSharp.CloudEdge.Runtime.Workers
module Containers = FSharp.CloudEdge.Runtime.Containers

let switchPort (request: Workers.Request<_, _>) (port: float) : Workers.Request<_, _> =
    Containers.Exports.switchPort(request, port)
