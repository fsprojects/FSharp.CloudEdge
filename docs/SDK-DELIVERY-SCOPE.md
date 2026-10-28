# Selected SDK scope

This is the explicit `server-resources` selection from `config/sdk-delivery.json`:
30 SDK libraries covering 106 public entrypoints, plus eight generated dependency support libraries and the Agents Pi operations library.
One handwritten Workers support project provides checked Durable Object fetch adapters.
Versions retain the installed package pins. The tables describe the selection; successful
generation and compilation are recorded separately in `artifacts/library-build/latest.json`.

Management is generated separately from the pinned Cloudflare OpenAPI specification by Hawaii.
The management selection contains 3,437 operations across 12 projects, including shared models.

## Runtime libraries

| Library | npm package | Retained version | Public entrypoints |
|---|---|---|---:|
| `Workers` | `@cloudflare/workers-types` | `5.20261025.1` | 1 |
| `Agents` | `agents` | `0.30.1` | 9 |
| `AgentsAiChatAgent` | `agents` | `0.30.1` | 14 |
| `AgentsMcpClient` | `agents` | `0.30.1` | 1 |
| `AIChat` | `@cloudflare/ai-chat` | `0.12.2` | 3 |
| `CodeMode` | `@cloudflare/codemode` | `0.5.5` | 3 |
| `Shell` | `@cloudflare/shell` | `0.5.0` | 1 |
| `ShellGit` | `@cloudflare/shell` | `0.5.0` | 2 |
| `Voice` | `@cloudflare/voice` | `0.5.0` | 1 |
| `VoiceErrors` | `@cloudflare/voice` | `0.5.0` | 1 |
| `Computer` | `@cloudflare/computer` | `0.7.0` | 6 |
| `ComputerArtifacts` | `@cloudflare/computer` | `0.7.0` | 13 |
| `Sandbox` | `@cloudflare/sandbox` | `1.0.3` | 1 |
| `Containers` | `@cloudflare/containers` | `0.3.7` | 1 |
| `Actors` | `@cloudflare/actors` | `0.0.1-beta.6` | 3 |
| `Cabidela` | `@cloudflare/cabidela` | `0.2.4` | 1 |
| `DynamicWorkflows` | `@cloudflare/dynamic-workflows` | `0.1.1` | 1 |
| `WorkersOauthProvider` | `@cloudflare/workers-oauth-provider` | `1.2.4` | 1 |
| `KvAssetHandler` | `@cloudflare/kv-asset-handler` | `0.5.0` | 1 |
| `Chanfana` | `chanfana` | `3.4.0` | 1 |
| `Capnweb` | `capnweb` | `0.12.0` | 1 |
| `AIGatewayProvider` | `ai-gateway-provider` | `4.0.2` | 19 |
| `WorkersAIProvider` | `workers-ai-provider` | `4.0.1` | 5 |
| `AISearchProvider` | `ai-search-provider` | `0.1.2` | 1 |
| `AIUtils` | `@cloudflare/ai-utils` | `1.0.1` | 1 |
| `Think` | `@cloudflare/think` | `0.21.0` | 8 |
| `Flagship` | `@cloudflare/flagship` | `0.6.0` | 2 |
| `PagesPluginCloudflareAccess` | `@cloudflare/pages-plugin-cloudflare-access` | `1.0.5` | 2 |
| `PagesPluginTurnstile` | `@cloudflare/pages-plugin-turnstile` | `1.0.2` | 1 |
| `PagesPluginStaticForms` | `@cloudflare/pages-plugin-static-forms` | `1.0.3` | 1 |

## Shared dependency contracts

| Library | npm package | Retained version |
|---|---|---|
| `Support.AI.V4.Provider` | `@ai-sdk/provider` | `4.0.10` |
| `Support.AI.V4.OpenAICompatible` | `@ai-sdk/openai-compatible` | `3.0.44` |
| `Support.AI.V4.ProviderUtils` | `@ai-sdk/provider-utils` | `5.0.36` |
| `Support.AI.V7` | `ai` | `7.0.93` |
| `Support.AI.V3.Provider` | `@ai-sdk/provider` | `3.0.15` |
| `Support.Zod.V4` | `zod` (`.`, `./v3`, `./v4/core`) | `4.6.7` |
| `Support.Hono.V4` | `hono` (`.`, `./types`) | `4.13.2` |
| `Support.JsonSchema` | `@types/json-schema` | `7.0.15` |

AI Gateway and Workers AI share the v4 contract. The retained AI Search package uses v3, which has its own owner. Runtime Workers is the single owner of the native Workers declarations for every library in the `workers-node` catalog family described in the [library guide](SDK-LIBRARY.md).

## Selection boundaries

The configuration lists every selected public import and a reason for each omitted import
or deferred partition. Upstream removals are recorded separately in the selected inventory refresh; Sandbox no longer exports the retired bridge partition. Selected declaration files retain their complete exports and
transitive type dependencies. AI Gateway retains its outward provider adapters.

The broader inventory contains 1,099 public inputs in 79 compiler programs. Its browser UI,
React, mobile clients, optional integrations, developer tooling and alternate preview
surfaces remain opt-in. The selected MCP client is the server-side `agents/mcp/client` import.

See [the library guide](SDK-LIBRARY.md) for commands, catalog ownership, fidelity findings
and the separate runtime/resource lifecycle validation boundary.
