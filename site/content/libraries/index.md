---
title: Library Line-Up
description: The Cloudflare SDKs these libraries cover.
order: 1
---

The 31 runtime libraries bind the Cloudflare SDKs below, and 11 control-plane clients call Cloudflare's REST API. Project names start with `FSharp.CloudEdge.`, and [`docs/SDK-DELIVERY-SCOPE.md`](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/SDK-DELIVERY-SCOPE.md) lists each project with its npm package and pinned version.

| Area | Cloudflare SDKs |
| --- | --- |
| Workers platform | Workers runtime types and native bindings |
| [Data & Analytics](data/index.md) | [D1](data/d1.md), [R2](data/r2.md), [KV](data/kv.md), [Vectorize](data/vectorize.md), [Analytics Engine](data/analytics-engine.md) through runtime bindings and management clients |
| [Agents & Tools](agents.md) | [Agents SDK](agents/sdk.md), [Chat and Think](agents/chat.md), [MCP](agents/mcp.md), [Code Mode](agents/code-mode.md), [Shell](agents/shell.md), [Git](agents/git.md), [Voice](agents/voice.md), [Email](agents/email.md) |
| [AI](ai.md) | AI Utils, and the AI Gateway, Workers AI and AI Search providers |
| [Compute](compute.md) | [Containers](containers.md), [Sandbox](agents/sandbox.md), [Computer](agents/computer.md) |
| Versioned repositories | [Artifacts](agents/artifacts.md) through the Workers runtime binding |
| [Services](services.md) | Actors, Dynamic Workflows, Workers OAuth Provider, KV Asset Handler, Cabidela, Chanfana |
| RPC | Cap'n Web |
| Feature flags | Flagship |
| Pages | Cloudflare Access, Turnstile and Static Forms plugins |
| Control plane | Ten management clients grouped by purpose and one tenancy client, 3,437 REST operations over shared `Core.Api` types |

Client-side adapters and optional integrations are left out of this selection, and [`config/sdk-delivery.json`](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/config/sdk-delivery.json) records the reason for each.

The capability pages describe integration boundaries and areas that need runtime evidence. See [Verify bindings](../guide/verify-bindings.md) to contribute a reproducible issue or a successful check.
