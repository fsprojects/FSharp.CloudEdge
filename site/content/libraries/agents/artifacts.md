---
title: Artifacts
description: Store, fork, and exchange versioned file trees through Git-compatible repositories.
---

<div class="ce-block-head">
<p class="ce-block-lead">Store, fork, and exchange versioned file trees through Git-compatible repositories.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Workers</code> <code>Runtime.ComputerArtifacts</code></li>
<li><span>npm</span> <code>@cloudflare/computer 0.2.1</code></li>
<li><span>Upstream/API</span> Artifacts closed beta; Computer helper preview</li>
</ul>
</div>

## What is versioned

[Cloudflare Artifacts](https://developers.cloudflare.com/artifacts/) gives each repository its own history, refs, remote URL, and access tokens. Workers and the REST API manage repositories; Git clients clone, fetch, and push their contents. An agent can work against the same remote from a local machine, a virtual workspace, or a Linux sandbox.

Keep these three resources distinct:

| Resource | What it holds | Typical use |
| --- | --- | --- |
| Artifacts repository | Versioned files and Git history | Branch, fork, review, and merge agent work |
| Sandbox directory backup | A saved directory tree | Restore a prepared execution environment |
| Agent Durable Object | Conversation and application state | Resume the agent's workflow |

## A versioned agent workflow

1. Create or import a repository containing the task's starting files.
2. Allocate a branch or fork for an independent attempt and grant the worker access to that repository.
3. Clone the files into a sandbox or workspace, make changes, and run the relevant checks.
4. Commit and push the result. Store the resulting repository and revision alongside the task record.
5. Compare competing attempts, review the diff, and merge the accepted result.

The [repository interfaces](https://developers.cloudflare.com/artifacts/concepts/repositories/) explain how the Workers binding, REST API, and Git remote address the same repository. Repository versioning lets the execution environment be replaced without losing the published work.

## Team Repositories

[Cloudflare Artifacts](https://developers.cloudflare.com/artifacts/) stores Git repositories that any Git client can clone. `createArtifact` from `Runtime.ComputerArtifacts` returns a client that prefixes each repository name with a session ID. The Worker uses the team name as that ID, so each team's list contains only its own repositories. It creates a repository from a `POST` body and responds with the team's list as JSON.

:::warning
`@cloudflare/computer` 0.2.1 is a preview package. Its README states that the APIs are unstable and that the package is not suitable for production use at this time.
:::

```fsharp
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
```

<div class="ce-needs"><p><strong>Needs</strong> an Artifacts binding named <code>ARTIFACTS</code>. Artifacts is in closed beta, and access is by request.</p></div>

The team query parameter in this example demonstrates naming. An application should derive the team from its authenticated identity before granting access to that team's repositories.

## Repository access

Use repository-scoped tokens for Git operations. Give a reader read access and a publishing task write access, with a lifetime appropriate to the task. The Computer helper exposes `createToken`, `listTokens`, and `revokeToken`; repository creation and token management remain separate from Git operations on file contents.

## Large working trees

[ArtifactFS](https://developers.cloudflare.com/artifacts/guides/artifact-fs/) can mount a repository as a local filesystem without waiting for a full clone. Treat that as an execution-environment integration: choose how the files become available to the sandbox independently of which agent owns the task and which revision it should use.

## Related Pages

- [Sandbox execution and backups](sandbox.md)
- [Git tools inside a workspace](git.md)
- [Persistent Computer workspaces](computer.md)

## Help verify these bindings

The examples on this page are checked against F# source projects. Compilation and emitted JavaScript checks do not establish hosted service behavior. Useful targets for community verification include repository response shapes, namespace and session naming, scoped tokens, and interoperability with a standard Git client.

See [Verify bindings](../../guide/verify-bindings.md) for the existing evidence, reproducible checks, and the [binding issue form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include the pinned package version and the specific behavior exercised; successful reproductions are useful evidence too.
