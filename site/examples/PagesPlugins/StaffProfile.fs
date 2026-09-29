module StaffProfile

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Access = FSharp.CloudEdge.Runtime.PagesPluginCloudflareAccess

let onRequestGet (context: Workers.EventContext<obj, string, Access.PluginData>) =
    async {
        let! identity = context.data.cloudflareAccess.JWT.getIdentity () |> Async.AwaitPromise
        match identity with
        | Some person ->
            let profile = {| name = person.name; email = person.email; groups = person.groups |}
            return Workers.Exports.Response.json profile
        | None ->
            let notFound = Workers.ResponseInit.Create(status = 404.)
            return Workers.Exports.Response.Create("No identity for this sign-in", notFound)
    }
    |> Async.StartAsPromise
