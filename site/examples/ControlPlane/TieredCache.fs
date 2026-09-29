module TieredCache

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.ContentDelivery

let enable (delivery: ContentDeliveryClient) zoneId =
    task {
        let turnOn = cache_u002D_rules_smart_tiered_cache_patch.Create cache_u002D_rules_smart_tiered_cache_patchValue.On
        match! delivery.SmartTieredCachePatchSmartTieredCacheSetting(zoneId, turnOn) with
        | SmartTieredCachePatchSmartTieredCacheSetting.OK _ ->
            printfn "Smart Tiered Cache is on"
        | SmartTieredCachePatchSmartTieredCacheSetting.Status4XX(status, failure) ->
            printfn "HTTP %d: %O" status failure.errors
    }
