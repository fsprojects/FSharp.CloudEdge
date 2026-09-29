module HealthCheck

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env ctx ->
        U2.Case2(Workers.Exports.Response.Create("ok")))
