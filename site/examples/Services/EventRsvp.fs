module EventRsvp

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Actors = FSharp.CloudEdge.Runtime.Actors
module ActorStorage = FSharp.CloudEdge.Runtime.Actors.Storage

type Rsvp = {| email: string; plusOnes: float |}
type Headcount = {| attending: float |}

type Invitation(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    let schema = ActorStorage.Exports.Storage ctx.storage

    do schema.migrations <- [|
        Actors.SQLSchemaMigration.Create(1., "RSVPs", "CREATE TABLE IF NOT EXISTS rsvps (email TEXT PRIMARY KEY)")
        Actors.SQLSchemaMigration.Create(2., "Plus-ones", "ALTER TABLE rsvps ADD COLUMN plus_ones INTEGER DEFAULT 0")
    |]

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch request =
            async {
                let! _ = schema.runMigrations() |> Async.AwaitPromise
                if request.``method`` = "POST" then
                    let! rsvp = request.json<Rsvp>() |> Async.AwaitPromise
                    ctx.storage.sql.exec("INSERT OR REPLACE INTO rsvps VALUES (?, ?)", rsvp.email, rsvp.plusOnes) |> ignore
                let headcount = ctx.storage.sql.exec<Headcount>("SELECT COUNT(*) + TOTAL(plus_ones) AS attending FROM rsvps").one()
                return Workers.Exports.Response.json headcount
            }
            |> Async.StartAsPromise
            |> U2.Case1
