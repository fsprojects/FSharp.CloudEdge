module SignupForm

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Turnstile = FSharp.CloudEdge.Runtime.PagesPluginTurnstile

let verifyHuman =
    Turnstile.Exports.pagesPluginTurnstile(
        Turnstile.PluginArgs.Create(secret = "1x0000000000000000000000000000000AA"))

let register (context: Workers.EventContext<obj, string, Turnstile.PluginData>) =
    async {
        let! form = context.request.formData () |> Async.AwaitPromise
        match form.get "email" with
        | Some (U2.Case1 email) -> return Workers.Exports.Response.Create($"Thanks for signing up, {email}.")
        | _ ->
            let badRequest = Workers.ResponseInit.Create(status = 400.)
            return Workers.Exports.Response.Create("An email address is required.", badRequest)
    }
    |> Async.StartAsPromise

let onRequestPost: obj[] = [| verifyHuman; register |]
