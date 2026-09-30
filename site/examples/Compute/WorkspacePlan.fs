module WorkspacePlan

open Fable.Core

module Computer = FSharp.CloudEdge.Runtime.Computer

let savePlan (storage: Computer.DurableObjectStorageLike) (text: string) =
    let workspace = Computer.Exports.Workspace(Computer.WorkspaceOptions.Create storage)
    async {
        do! workspace.fs.writeFile("/plan.md", text) |> Async.AwaitPromise
        return! workspace.fs.readFile("/plan.md", "utf8") |> Async.AwaitPromise
    }
    |> Async.StartAsPromise
