module GameLobby

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Agents = FSharp.CloudEdge.Runtime.Agents
module Lifecycle = FSharp.CloudEdge.Runtime.Agents.Lifecycle

[<AttachMembers>]
type GameLobby(ctx: Workers.DurableObjectState<obj>, env: obj) as this =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    let lifecycle = Lifecycle.Lifecycle.install<obj, obj> this

    member _.onConnect(player: Agents.Connection<obj>, _context: Agents.ConnectionContext) =
        player.send (U3.Case1 $"Welcome to {lifecycle.name}")

    member _.onMessage(player: Agents.Connection<obj>, message: Agents.WSMessage) =
        lifecycle.broadcast (message, [| player.id |])
