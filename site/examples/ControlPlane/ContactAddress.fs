module ContactAddress

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Messaging

let forwardContact (messaging: MessagingClient) zoneId =
    task {
        let toContact =
            { email_rule_matcher.Create email_rule_matcherType.Literal with
                field = Some Field.To
                value = Some "hello@example.com" }
        let forward = { email_rule_action.Create email_rule_actionType.Forward with value = Some [ "owner@example.net" ] }
        let rule = { email_create_rule_properties.Create([ forward ], [ toContact ]) with name = Some "Contact address" }
        match! messaging.EmailRoutingRoutingRulesCreateRoutingRule(zoneId, rule) with
        | EmailRoutingRoutingRulesCreateRoutingRule.OK _ ->
            printfn "hello@example.com forwards to owner@example.net"
        | EmailRoutingRoutingRulesCreateRoutingRule.BadRequest ->
            printfn "Rule rejected with HTTP 400"
        | EmailRoutingRoutingRulesCreateRoutingRule.UnprocessableEntity ->
            printfn "Rule rejected with HTTP 422"
    }
