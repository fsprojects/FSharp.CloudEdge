module VisitorGreeting

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request _ _ ->
        let city =
            request.cf
            |> Option.bind (fun cf -> cf.city)
            |> Option.defaultValue "your city"
        U2.Case2(Workers.Exports.Response.Create($"Hello to everyone in {city}")))
