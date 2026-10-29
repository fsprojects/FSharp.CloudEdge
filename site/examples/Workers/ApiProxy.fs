module ApiProxy

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract UPSTREAM_URL: string
    abstract API_KEY: string

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        let url = Workers.Exports.URL(U2.Case1 request.url)
        let upstream = env.UPSTREAM_URL + url.pathname + url.search
        let headers = Workers.Exports.Headers()
        headers.set("Authorization", $"Bearer {env.API_KEY}")
        let init = Workers.RequestInit<Workers.RequestInitCfProperties>.Create(headers = U3.Case1 headers)
        Workers.Exports.fetch(U3.Case1 upstream, init) |> U2.Case1)
