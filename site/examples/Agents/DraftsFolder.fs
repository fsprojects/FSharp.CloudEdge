module DraftsFolder

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Shell = FSharp.CloudEdge.Runtime.Shell

type DraftsFolder(ctx: Workers.DurableObjectState<obj>, env: obj) =
    inherit Runtime.DurableObject<obj, obj>(ctx, env)

    let workspace = Shell.Exports.Workspace(Shell.WorkspaceOptions.Create(sql = ctx.storage.sql))

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch request =
            async {
                let path = Workers.Exports.URL(U2.Case1 request.url).pathname
                if request.``method`` = "PUT" then
                    let! text = request.text () |> Async.AwaitPromise
                    do! workspace.writeFile (path, text, "text/markdown") |> Async.AwaitPromise
                let! drafts = workspace.glob "/**/*.md" |> Async.AwaitPromise
                let listing = drafts |> Array.map (fun draft -> {| path = draft.path; size = draft.size |})
                return Workers.Exports.Response.json listing
            }
            |> Async.StartAsPromise
            |> U2.Case1
