module CartReminder

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Cart = {| cartId: string; email: string |}

type Env =
    abstract REMINDERS: Workers.Queue<Cart>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! cart = request.json<Cart>() |> Async.AwaitPromise
                let later = Workers.QueueSendOptions.Create(delaySeconds = 3600.)
                let! _ = env.REMINDERS.send(cart, later) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| saved = cart.cartId |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
