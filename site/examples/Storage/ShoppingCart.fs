module ShoppingCart

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Cart = {| items: string[] |}

type Env =
    abstract CARTS: Workers.KVNamespace<string>

let sevenDays = Workers.KVNamespacePutOptions.Create(expirationTtl = 604800.)

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let cartId = Workers.Exports.URL(U2.Case1 request.url).pathname.Substring(1)
                match request.``method`` with
                | "PUT" ->
                    let! cart = request.json<Cart>() |> Async.AwaitPromise
                    do! env.CARTS.put(cartId, U4.Case1(JS.JSON.stringify cart), sevenDays) |> Async.AwaitPromise
                    return Workers.Exports.Response.json cart
                | _ ->
                    let! cart = env.CARTS.get<Cart>(cartId, Workers.KVNamespace.Json) |> Async.AwaitPromise
                    return Workers.Exports.Response.json(cart |> Option.defaultValue {| items = [||] |})
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
