module SignupWidget

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Security

let createWidget (security: SecurityClient) accountId =
    task {
        let widget =
            AccountsTurnstileWidgetCreatePayload.Create([ "example.com" ], turnstile_widget_mode.Managed, "signup-form")
        match! security.AccountsTurnstileWidgetCreate(accountId, widget) with
        | AccountsTurnstileWidgetCreate.OK payload ->
            return payload.result |> Option.map (fun created -> created.sitekey, created.secret)
        | AccountsTurnstileWidgetCreate.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
            return None
    }
