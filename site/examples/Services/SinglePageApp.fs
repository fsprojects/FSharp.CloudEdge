module SinglePageApp

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Assets = FSharp.CloudEdge.Runtime.KvAssetHandler

type Env =
    abstract SITE: Workers.KVNamespace<string>

let options (env: Env) =
    Assets.Options.MapRequestToAsset.Options.Create(
        ASSET_NAMESPACE = env.SITE,
        mapRequestToAsset = Assets.Options.MapRequestToAsset(fun request _ -> Assets.Exports.serveSinglePageApp request))

[<ExportDefault>]
let worker =
    {| fetch = fun (request: obj) (env: Env) (ctx: Workers.ExecutionContext<obj>) ->
        let lookup = Assets.GetAssetFromKV.Event.Create(request, fun work -> ctx.waitUntil work)
        Assets.Exports.getAssetFromKV(lookup, options env) |}
