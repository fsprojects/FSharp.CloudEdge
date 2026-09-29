module Leaderboard

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

type Score = {| player: string; points: float |}

type Leaderboard(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    do ctx.storage.sql.exec("CREATE TABLE IF NOT EXISTS scores (player TEXT PRIMARY KEY, points REAL)") |> ignore

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch request =
            async {
                if request.``method`` = "POST" then
                    let! score = request.json<Score>() |> Async.AwaitPromise
                    ctx.storage.sql.exec(
                        "INSERT INTO scores VALUES (?, ?) ON CONFLICT (player) DO UPDATE SET points = MAX(points, excluded.points)",
                        score.player, score.points) |> ignore
                let top = ctx.storage.sql.exec<Score>("SELECT player, points FROM scores ORDER BY points DESC LIMIT 10").toArray()
                return Workers.Exports.Response.json top
            }
            |> Async.StartAsPromise
            |> U2.Case1
