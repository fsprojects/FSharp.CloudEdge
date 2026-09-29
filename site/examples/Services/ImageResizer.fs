module ImageResizer

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Containers = FSharp.CloudEdge.Runtime.Containers
module Transport = FSharp.CloudEdge.Support.Workers.DurableObjects

type Env =
    abstract RESIZER: Workers.DurableObjectNamespace<Containers.Container<obj>>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            let resize =
                Workers.Exports.Request.Create(
                    U3.Case1 "http://resizer/thumbnail?width=320",
                    Workers.RequestInit.Create(``method`` = "POST", body = request.body))
            let resizer = Transport.getFetchByName env.RESIZER "resizer"
            resizer.fetch (Containers.Exports.switchPort(resize, 8080.)) |> U2.Case1
    )
