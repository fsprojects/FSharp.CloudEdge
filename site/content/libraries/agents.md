---
title: Agents & Tools
description: Agent lifecycles, tools, sandboxes, persistent workspaces, and versioned repositories.
order: 6
---

An agent needs somewhere to keep its state, tools it can call, an environment in which to run code, and a way to keep or share its results. Choose those parts independently. The pages below explain each part, its F# bindings, and its configuration.

## Build the agent

| Capability | Start here |
| --- | --- |
| Agent identity, state, connections, and lifecycle | [Agents SDK](agents/sdk.md) |
| Conversation history and streamed replies | [Chat agents](agents/chat.md) |
| Remote tool discovery and calls | [MCP](agents/mcp.md) |
| Generated code that composes supplied tools | [Code Mode](agents/code-mode.md) |
| Speech providers and audio output | [Voice](agents/voice.md) |
| Inbound email events | [Email agents](agents/email.md) |

## Testing and contribution

Several upstream packages are experimental or preview, and some imported base classes cannot yet be subclassed directly from F#. Each capability page describes its current limitations.

Try the examples in your application and share what you find. [Verify bindings](../guide/verify-bindings.md) explains how to run checks and report a reproducible problem or a working integration.

## Give it a working environment

| Capability | Start here |
| --- | --- |
| Your own containerized service | [Containers](containers.md) |
| Linux commands, installed packages, and tests | [Sandbox](agents/sandbox.md) |
| Durable files with configurable execution backends | [Computer workspaces](agents/computer.md) |
| Structured file and state operations for Code Mode | [Shell workspaces](agents/shell.md) |
| Local commits, branches, diffs, and remote synchronization | [Git tools](agents/git.md) |
| Hosted Git-compatible repositories and versioned work | [Artifacts](agents/artifacts.md) |

The [execution environments guide](compute.md) compares the runtime choices. Artifacts holds published file history; a workspace holds working files; a sandbox executes a task. An agent's conversation and application state have their own lifecycle.

## Example: a coding task

Create an agent identity for the task, give it a branch or repository for its work, and make those files available in a workspace or sandbox. Expose the tools the task needs. After execution, inspect the diff and test results, publish the chosen revision, and save its identity with the task result. The [Artifacts workflow](agents/artifacts.md#a-versioned-agent-workflow) walks through those boundaries.

## Examples

- <span id="game-lobby"></span>[Game Lobby — Agents SDK](agents/sdk.md#game-lobby)
- <span id="team-channel"></span>[Team Channel — Agents SDK](agents/sdk.md#team-channel)
- <span id="snippet-runner"></span>[Snippet Runner — Code Mode](agents/code-mode.md#snippet-runner)
- <span id="drafts-folder"></span>[Drafts Folder — Shell](agents/shell.md#drafts-folder)
- <span id="audio-preview"></span>[Audio Preview — Voice](agents/voice.md#audio-preview)
- <span id="support-inbox"></span>[Support Inbox — Email](agents/email.md#support-inbox)

## Library Table

The [library line-up](index.md) maps services to packages. Each dedicated page lists its own bindings and pinned package versions.

## Related Pages

- [Model inference and providers](ai.md)
- [Durable Objects](platform/durable-objects.md)
- [Worker deployment and bindings](control-plane/worker-upload.md)
