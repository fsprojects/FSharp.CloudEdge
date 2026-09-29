module SupportInbox

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Email = FSharp.CloudEdge.Runtime.AgentsAiChatAgent.Email

type Env =
    abstract TEAM_ADDRESS: string

let isAutoReply (message: Workers.ForwardableEmailMessage) =
    [| "auto-submitted"; "x-auto-response-suppress"; "precedence" |]
    |> Array.choose (fun key ->
        message.headers.get key
        |> Option.map (fun value -> Email.EmailHeader.Create(key = key, value = value)))
    |> Email.Exports.isAutoReplyEmail

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        email = fun message env _ ->
            if isAutoReply message then None
            else
                message.forward env.TEAM_ADDRESS
                |> Async.AwaitPromise
                |> Async.Ignore
                |> Async.StartAsPromise
                |> Some
    )
