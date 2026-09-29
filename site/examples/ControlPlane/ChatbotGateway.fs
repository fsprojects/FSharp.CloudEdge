module ChatbotGateway

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.AI

let createGateway (ai: AIClient) accountId =
    task {
        let gateway =
            { AigConfigCreateGatewayPayload.Create(cache_invalidate_on_update = true, collect_logs = true, id = "chatbot") with
                cache_ttl = Some 300
                rate_limiting_limit = Some 100
                rate_limiting_interval = Some 60
                rate_limiting_technique = Some AigConfigCreateGatewayPayloadRate_limiting_technique.Fixed }
        match! ai.AigConfigCreateGateway(accountId, body = gateway) with
        | AigConfigCreateGateway.OK payload ->
            printfn "Gateway %s created at %O" payload.result.id payload.result.created_at
        | AigConfigCreateGateway.BadRequest failure ->
            printfn "Gateway rejected: %A" failure.errors
    }
