module TrialSignup

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract TRIALS: Workers.Workflow<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                match Workers.Exports.URL(U2.Case1 request.url).searchParams.get "user" with
                | None ->
                    return Workers.Exports.Response.Create("user is required", Workers.ResponseInit.Create(status = 400.))
                | Some user ->
                    let! trial =
                        if request.``method`` = "POST" then
                            env.TRIALS.create(Workers.WorkflowInstanceCreateOptions.Create(id = user)) |> Async.AwaitPromise
                        else
                            env.TRIALS.get user |> Async.AwaitPromise
                    let! state = trial.status () |> Async.AwaitPromise
                    return Workers.Exports.Response.json {| trial = trial.id; status = state.status |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
