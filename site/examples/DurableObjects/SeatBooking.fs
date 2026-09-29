module SeatBooking

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

type Booking = {| seat: string; email: string |}

type SeatMap(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    let book (booking: Booking) (tx: Workers.DurableObjectTransaction) =
        async {
            let! holder = tx.get<string>("seat:" + booking.seat) |> Async.AwaitPromise
            if holder.IsSome then
                return false
            else
                let! sold = tx.get<float>("sold") |> Async.AwaitPromise
                do! tx.put("seat:" + booking.seat, booking.email) |> Async.AwaitPromise
                do! tx.put("sold", Option.defaultValue 0. sold + 1.) |> Async.AwaitPromise
                return true
        }
        |> Async.StartAsPromise

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch request =
            async {
                let! booking = request.json<Booking>() |> Async.AwaitPromise
                let! booked = ctx.storage.transaction(book booking) |> Async.AwaitPromise
                let status = if booked then 201. else 409.
                return Workers.Exports.Response.json({| seat = booking.seat; booked = booked |}, U2.Case2(Workers.ResponseInit.Create(status = status)))
            }
            |> Async.StartAsPromise
            |> U2.Case1
