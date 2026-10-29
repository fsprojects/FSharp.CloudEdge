module WorkspacePlan

open Fable.Core

module Computer = FSharp.CloudEdge.Runtime.Computer

let savePlan (storage: Computer.DurableObjectStorageLike) (text: string) =
    let workspace = Computer.Exports.Workspace(Computer.WorkspaceOptions.Create storage)
    async {
        do! workspace.fs.writeFile("/plan.md", U3.Case1 text) |> Async.AwaitPromise
        let options: Computer.WorkspaceFilesystem.ReadFile.Options2 = unbox {| encoding = "utf8" |}
        return! workspace.fs.readFile("/plan.md", options) |> Async.AwaitPromise
    }
    |> Async.StartAsPromise
