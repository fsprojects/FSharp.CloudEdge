module TrialEmails

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

type Email = {| user: string; template: string |}

type Env =
    abstract EMAILS: Workers.Queue<Email>
    abstract DB: Workers.D1Database
    abstract TRIALS: Workers.Workflow<obj>

type TrialEmails(ctx: Workers.ExecutionContext<obj>, env: Env) =
    inherit Runtime.WorkflowEntrypoint<Env, obj>(ctx, env)

    override _.run(event, step) =
        let user = event.instanceId
        async {
            do! step.``do``("welcome email", fun _ -> env.EMAILS.send {| user = user; template = "welcome" |})
                |> Async.AwaitPromise |> Async.Ignore
            do! step.sleep.Invoke("trial period", U2.Case2 "7 days") |> Async.AwaitPromise
            let! plan =
                step.``do``("check plan", fun _ ->
                    env.DB.prepare("SELECT plan FROM users WHERE id = ?").bind(user).first<string>("plan"))
                |> Async.AwaitPromise
            let stillTrial = (plan = Some "trial")
            if stillTrial then
                do! step.``do``("reminder email", fun _ -> env.EMAILS.send {| user = user; template = "trial-ending" |})
                    |> Async.AwaitPromise |> Async.Ignore
            return box {| user = user; reminded = stillTrial |}
        }
        |> Async.StartAsPromise

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
