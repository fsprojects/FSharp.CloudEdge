module ChatHistoryUpgrade

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Migration = FSharp.CloudEdge.Runtime.AIChat.AiChatV5Migration

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request _ _ ->
            async {
                let! stored = request.json<obj[]> () |> Async.AwaitPromise
                let upgraded = Migration.Exports.autoTransformMessages stored
                return Workers.Exports.Response.json upgraded
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
