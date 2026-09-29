module Checkout

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Order = {| email: string; items: {| sku: string; quantity: float |}[] |}

type Env =
    abstract DB: Workers.D1Database

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! order = request.json<Order>() |> Async.AwaitPromise
                let orderId = Workers.Exports.crypto.randomUUID()
                let addOrder = env.DB.prepare("INSERT INTO orders (id, email) VALUES (?, ?)")
                let addItem = env.DB.prepare("INSERT INTO order_items (order_id, sku, quantity) VALUES (?, ?, ?)")
                let statements =
                    Array.append
                        [| addOrder.bind(orderId, order.email) |]
                        (order.items |> Array.map (fun item -> addItem.bind(orderId, item.sku, item.quantity)))
                let! _ = env.DB.batch statements |> Async.AwaitPromise
                return Workers.Exports.Response.json {| orderId = orderId |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
