---
title: Library Line-Up
description: The Cloudflare SDKs these libraries cover.
order: 1
---

The 31 runtime libraries bind the Cloudflare SDKs below, and 11 control-plane clients call Cloudflare's REST API. Project names start with `FSharp.CloudEdge.`, and [`docs/SDK-DELIVERY-SCOPE.md`](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/SDK-DELIVERY-SCOPE.md) lists each project with its npm package and pinned version.

| Area | Cloudflare SDKs |
| --- | --- |
| Workers platform | Workers runtime types and native bindings |
| Agents | Agents SDK, Code Mode, Shell, Voice |
| AI | AI Chat, Think, AI Utils, and the AI Gateway, Workers AI and AI Search providers |
| Compute | Computer, Sandbox |
| Services | Containers, Actors, Dynamic Workflows, Workers OAuth Provider, KV Asset Handler, Cabidela, Chanfana |
| RPC | Cap'n Web |
| Feature flags | Flagship |
| Pages | Cloudflare Access, Turnstile and Static Forms plugins |
| Control plane | Ten management clients grouped by purpose and one tenancy client, 3,437 REST operations over shared `Core.Api` types |

Client-side adapters and optional integrations are left out of this selection, and [`config/sdk-delivery.json`](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/config/sdk-delivery.json) records the reason for each.
