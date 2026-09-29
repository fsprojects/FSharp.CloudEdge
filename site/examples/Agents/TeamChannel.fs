module TeamChannel

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Agents = FSharp.CloudEdge.Runtime.Agents
module Lifecycle = FSharp.CloudEdge.Runtime.Agents.Lifecycle

let history (sql: Workers.SqlStorage) =
    Lifecycle.DurableObjectCapability.Create(onStart = fun _ ->
        sql.exec "CREATE TABLE IF NOT EXISTS messages (body TEXT)" |> ignore
        None)

[<AttachMembers>]
type TeamChannel(ctx: Workers.DurableObjectState<obj>, env: obj) as this =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    let lifecycle = Lifecycle.Lifecycle.install<obj, obj>(this).``use`` (history ctx.storage.sql)

    member _.onConnect(teammate: Agents.Connection<obj>, _context: Agents.ConnectionContext) =
        let recent = ctx.storage.sql.exec<{| body: string |}>("SELECT body FROM messages ORDER BY rowid DESC LIMIT 20").toArray ()
        for message in Array.rev recent do
            teammate.send (U3.Case1 message.body)

    member _.onMessage(sender: Agents.Connection<obj>, message: Agents.WSMessage) =
        match message with
        | U3.Case1 text ->
            ctx.storage.sql.exec ("INSERT INTO messages (body) VALUES (?)", text) |> ignore
            lifecycle.broadcast (message, [| sender.id |])
        | _ -> ()
