module TrialEmails

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

type Email = {| user: string; template: string |}

type Env =
    abstract EMAILS: Workers.Queue<Email>
    abstract DB: Workers.D1Database

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
            if plan = Some "trial" then
                do! step.``do``("reminder email", fun _ -> env.EMAILS.send {| user = user; template = "trial-ending" |})
                    |> Async.AwaitPromise |> Async.Ignore
            return box {| user = user; plan = plan |}
        }
        |> Async.StartAsPromise
