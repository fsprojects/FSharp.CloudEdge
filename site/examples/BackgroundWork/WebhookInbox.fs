module WebhookInbox

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract WEBHOOKS: Workers.Queue<string>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, string, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = (fun request env _ ->
            async {
                let! payload = request.text () |> Async.AwaitPromise
                let! _ = env.WEBHOOKS.send payload |> Async.AwaitPromise
                return Workers.Exports.Response.Create("accepted", Workers.ResponseInit.Create(status = 202.))
            }
            |> Async.StartAsPromise
            |> U2.Case1),
        queue = (fun batch _ _ ->
            async {
                for message in batch.messages do
                    let! response =
                        Workers.Exports.fetch(U3.Case1 "https://orders.example.com/hooks", Workers.RequestInit.Create(``method`` = "POST", body = message.body))
                        |> Async.AwaitPromise
                    if response.ok then message.ack ()
                    else message.retry (Workers.QueueRetryOptions.Create(delaySeconds = 30. * message.attempts))
            }
            |> Async.StartAsPromise
            |> Some)
    )
