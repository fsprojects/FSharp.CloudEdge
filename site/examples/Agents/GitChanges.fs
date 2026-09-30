module GitChanges

open Fable.Core

module ShellGit = FSharp.CloudEdge.Runtime.ShellGit

let changedFiles (filesystem: ShellGit.FileSystem) =
    let git = ShellGit.Git.Exports.createGit filesystem
    async {
        let! changes = git.status () |> Async.AwaitPromise
        return changes |> Array.map (fun entry -> entry.filepath)
    }
    |> Async.StartAsPromise
