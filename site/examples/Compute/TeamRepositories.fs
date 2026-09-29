module TeamRepositories

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Artifacts = FSharp.CloudEdge.Runtime.ComputerArtifacts.Artifacts

type Env =
    abstract ARTIFACTS: Workers.Artifacts

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let url = Workers.Exports.URL(U2.Case1 request.url)
                let team = url.searchParams.get "team" |> Option.defaultValue "demo"
                let repositories = Artifacts.Exports.createArtifact(env.ARTIFACTS, team)
                if request.``method`` = "POST" then
                    let! name = request.text () |> Async.AwaitPromise
                    do! repositories.create name |> Async.AwaitPromise |> Async.Ignore
                let! all = repositories.list () |> Async.AwaitPromise
                return Workers.Exports.Response.json (all |> Array.map (fun repo -> {| name = repo.name; branch = repo.defaultBranch |}))
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
