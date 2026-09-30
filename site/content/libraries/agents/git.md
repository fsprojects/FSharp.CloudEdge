---
title: Git tools
description: Inspect and version an agent working tree, then synchronize it with a Git remote.
---

<div class="ce-block-head">
<p class="ce-block-lead">Inspect and version an agent working tree, then synchronize it with a Git remote.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.ShellGit</code> <code>Runtime.Computer</code></li>
<li><span>npm</span> <code>@cloudflare/shell 0.4.3; @cloudflare/computer 0.2.1</code></li>
<li><span>Upstream/API</span> Shell experimental; Computer preview</li>
</ul>
</div>

## A Git client for workspace files

The Shell package's `createGit(filesystem)` operates on a supplied virtual filesystem. Its command set includes clone, status, add, commit, branch, checkout, fetch, pull, push, and diff. It can connect to a Git remote such as [Artifacts](artifacts.md).

Computer has its own `createGitClient` factory and `workspace.git` interface. These are distinct bindings with their own option records. Use the client that belongs to the workspace implementation you selected.

## Inspect an existing working tree

After initializing or cloning a repository in a Shell filesystem, this helper returns the paths reported by Git status. The caller supplies the same filesystem the agent edits.

```fsharp
open Fable.Core

module ShellGit = FSharp.CloudEdge.Runtime.ShellGit

let changedFiles (filesystem: ShellGit.FileSystem) =
    let git = ShellGit.Git.Exports.createGit filesystem
    async {
        let! changes = git.status () |> Async.AwaitPromise
        return changes |> Array.map (fun entry -> entry.filepath)
    }
    |> Async.StartAsPromise
```

## Record an attempt

Start from a known revision, give the attempt a branch or repository of its own, edit the files, and run checks in the chosen execution environment. Review the status and diff before staging and committing. Publish the commit to a remote if another agent or a later session needs it.

A successful commit records files locally. The task is not published until the remote has received the relevant commit and ref. Keep the repository, branch, and revision in the task's result record so a reviewer can reproduce the state.

## Tools and credentials

For Code Mode, `gitTools(workspace, options)` exposes Git commands as a tool provider. Configure remote authentication on the host; the [Shell Git guide](https://github.com/cloudflare/agents/blob/main/packages/shell/README.md#git-toolprovider-for-codemode) describes how default credentials are supplied to network operations.

A workspace's Git client performs repository operations. [Artifacts](artifacts.md) supplies the hosted repository and repository-scoped tokens. A [Sandbox](sandbox.md) can instead run the ordinary Git executable when the task needs a Linux toolchain.

## Related Pages

- [Shell workspaces](shell.md)
- [Artifacts repositories](artifacts.md)
- [Computer workspaces](computer.md)

## Help verify these bindings

The examples on this page are checked against F# source projects. Compilation and emitted JavaScript checks do not establish hosted service behavior. Useful targets for community verification include status records, option unions, commits and refs, remote authentication, and push/pull results.

See [Verify bindings](../../guide/verify-bindings.md) for the existing evidence, reproducible checks, and the [binding issue form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include the pinned package version and the specific behavior exercised; successful reproductions are useful evidence too.
