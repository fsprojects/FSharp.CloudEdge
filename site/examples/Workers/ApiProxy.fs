module ApiProxy

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract UPSTREAM_URL: string
    abstract API_KEY: string

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            let url = Workers.Exports.URL(U2.Case1 request.url)
            let headers = {| Authorization = $"Bearer {env.API_KEY}" |}
            Workers.Exports.fetch(U3.Case1(env.UPSTREAM_URL + url.pathname + url.search), Workers.RequestInit.Create(headers = headers))
            |> U2.Case1
    )
