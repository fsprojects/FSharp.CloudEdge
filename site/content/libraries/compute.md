---
title: Execution environments
description: Choose where your application runs code and where its files persist.
order: 8
---

Choose an execution environment from the work it must perform and the files it must retain. A containerized HTTP service, an agent editing durable documents, and a task that installs dependencies and runs a repository's test suite have different requirements.

## Choose an environment

| Requirement | Capability | Dedicated page |
| --- | --- | --- |
| An existing containerized service or custom Linux image | Containers with a Worker-facing Durable Object | [Containers](containers.md) |
| Linux binaries, package installation, or an existing toolchain | Container execution through Sandbox | [Sandbox](agents/sandbox.md) |
| Generated JavaScript composing application tools | Dynamic Worker execution with supplied providers | [Code Mode](agents/code-mode.md) |
| A durable filesystem with a choice of command backends | Computer workspace | [Computer](agents/computer.md) |
| Structured file operations exposed to generated code | Shell workspace and state tools | [Shell](agents/shell.md) |

## Keep work across runs

Use workspace storage for files that should remain available to the same application identity. Use [Artifacts](agents/artifacts.md) when the work needs a Git history and a remote that other agents or tools can access. Use [Sandbox backups](agents/sandbox.md#backup-restore-and-version-history) to reuse a prepared directory in a later execution environment.

Those choices can be combined. A coding agent can clone an Artifacts repository into a sandbox, use a backup to avoid reinstalling unchanged dependencies, and publish its changes as a new commit.

## Examples from the original page

- <span id="code-runner"></span>[Code Runner](agents/sandbox.md#code-runner)
- <span id="workspace-files"></span>[Workspace Files](agents/sandbox.md#workspace-files)
- <span id="notebook"></span>[Notebook](agents/sandbox.md#notebook)
- <span id="test-runner"></span>[Test Runner](agents/sandbox.md#test-runner)
- <span id="text-search"></span>[Text Search](agents/sandbox.md#text-search)
- <span id="team-repositories"></span>[Team Repositories — Artifacts](agents/artifacts.md#team-repositories)

## Library Table

| Library | Dedicated documentation |
| --- | --- |
| `Runtime.Containers` | [Containers](containers.md) |
| `Runtime.Sandbox`, `Runtime.SandboxBridge` | [Sandbox](agents/sandbox.md) |
| `Runtime.Computer` | [Computer workspaces](agents/computer.md) |
| `Runtime.ComputerArtifacts` | [Artifacts](agents/artifacts.md) |

## Related Pages

- [Agents & Tools overview](agents.md)
- [Git tools](agents/git.md)
- [Worker deployment](control-plane/worker-upload.md)
