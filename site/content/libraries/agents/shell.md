---
title: Shell workspaces
description: Give generated programs structured file operations over ephemeral or durable storage.
---

<div class="ce-block-head">
<p class="ce-block-lead">Give generated programs structured file operations over ephemeral or durable storage.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Shell</code> <code>Runtime.ShellGit</code></li>
<li><span>npm</span> <code>@cloudflare/shell 0.4.3</code></li>
<li><span>Upstream/API</span> Experimental</li>
</ul>
</div>

## Files and state

Despite its name, this package's execution model is JavaScript with structured filesystem operations. It does not provide a Bash interpreter. Its [package guide](https://github.com/cloudflare/agents/blob/main/packages/shell/README.md) describes `StateBackend`, filesystem implementations, and the `stateTools` integration with Code Mode.

Choose an in-memory filesystem for disposable work, or a `Workspace` backed by the Durable Object's SQLite storage for files that must survive another request. The example below stores Markdown drafts in the latter.

## Drafts Folder

`Workspace` from `@cloudflare/shell` is a file system stored in the Durable Object's SQLite database. On a `PUT`, the object saves the request body as a Markdown file at the request path, and every response lists the `.md` files with their sizes.

```fsharp
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
```

<div class="ce-needs"><p><strong>Needs</strong> a SQLite-backed Durable Object binding for <code>DraftsFolder</code>.</p></div>

## Expose files to an agent

`stateTools(workspace)` provides file operations to Code Mode. The host owns the workspace and supplies the provider; generated code operates through the exposed `state` API. Use [Git tools](git.md) when those files also need commits, branches, and remote synchronization.

Workspace persistence and version history serve different purposes. Saving a draft makes the current file durable. Committing it records a revision. Pushing that revision to [Artifacts](artifacts.md) makes it available independently of this workspace.

## Choosing between Shell and Computer

Shell supplies state backends and Code Mode providers. [Computer](computer.md) provides a workspace filesystem with a configurable execution backend. Choose the API around how the agent will interact with its files; both deserve an explicit storage and ownership decision.

## Related Pages

- [Code Mode](code-mode.md)
- [Git tools](git.md)
- [Computer workspaces](computer.md)

## Help verify these bindings

The examples on this page are checked against F# source projects. Compilation and emitted JavaScript checks do not establish hosted service behavior. Useful targets for community verification include persistence across Durable Object restarts, file metadata, filesystem adapters, and state-tool arguments and results.

See [Verify bindings](../../guide/verify-bindings.md) for the existing evidence, reproducible checks, and the [binding issue form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include the pinned package version and the specific behavior exercised; successful reproductions are useful evidence too.
