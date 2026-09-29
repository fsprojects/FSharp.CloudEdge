module Checkout

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Order = {| email: string; items: {| sku: string; quantity: float |}[] |}

type Env =
    abstract DB: Workers.D1Database

let orderSql = "INSERT INTO orders (id, email) VALUES (?, ?)"
let itemSql = "INSERT INTO order_items (order_id, sku, quantity) VALUES (?, ?, ?)"

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request env _ ->
        async {
            let! order = request.json<Order>() |> Async.AwaitPromise
            let orderId = Workers.Exports.crypto.randomUUID()
            let addItem = env.DB.prepare itemSql
            let statements =
                [| env.DB.prepare(orderSql).bind(orderId, order.email)
                   for item in order.items do
                       addItem.bind(orderId, item.sku, item.quantity) |]
            let! _ = env.DB.batch statements |> Async.AwaitPromise
            return Workers.Exports.Response.json {| orderId = orderId |}
        }
        |> Async.StartAsPromise
        |> U2.Case1)
