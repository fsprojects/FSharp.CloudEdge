# Selected SDK scope

This is the explicit `server-resources` selection from `config/sdk-delivery.json`:
31 SDK libraries covering 112 public entrypoints, plus three generated AI support libraries.
One handwritten Workers support project provides checked Durable Object fetch adapters.
Versions retain the installed package pins. The tables describe the selection; successful
generation and compilation are recorded separately in `artifacts/library-build/latest.json`.

Management is generated separately from the pinned Cloudflare OpenAPI specification by Hawaii.
The management selection contains 3,437 operations across 12 projects, including shared models.

## Runtime libraries

| Library | npm package | Retained version | Public entrypoints |
|---|---|---|---:|
| `Workers` | `@cloudflare/workers-types` | `5.20260906.1` | 1 |
| `Agents` | `agents` | `0.22.0` | 11 |
| `AgentsAiChatAgent` | `agents` | `0.22.0` | 14 |
| `AgentsMcpClient` | `agents` | `0.22.0` | 1 |
| `AIChat` | `@cloudflare/ai-chat` | `0.11.0` | 3 |
| `CodeMode` | `@cloudflare/codemode` | `0.5.1` | 3 |
| `Shell` | `@cloudflare/shell` | `0.4.3` | 1 |
| `ShellGit` | `@cloudflare/shell` | `0.4.3` | 2 |
| `Voice` | `@cloudflare/voice` | `0.4.0` | 1 |
| `VoiceErrors` | `@cloudflare/voice` | `0.4.0` | 1 |
| `Computer` | `@cloudflare/computer` | `0.2.1` | 6 |
| `ComputerArtifacts` | `@cloudflare/computer` | `0.2.1` | 13 |
| `Sandbox` | `@cloudflare/sandbox` | `0.12.9` | 2 |
| `SandboxBridge` | `@cloudflare/sandbox` | `0.12.9` | 2 |
| `Containers` | `@cloudflare/containers` | `0.3.7` | 1 |
| `Actors` | `@cloudflare/actors` | `0.0.1-beta.6` | 3 |
| `Cabidela` | `@cloudflare/cabidela` | `0.2.4` | 1 |
| `DynamicWorkflows` | `@cloudflare/dynamic-workflows` | `0.1.1` | 1 |
| `WorkersOauthProvider` | `@cloudflare/workers-oauth-provider` | `0.10.3` | 1 |
| `KvAssetHandler` | `@cloudflare/kv-asset-handler` | `0.5.0` | 1 |
| `Chanfana` | `chanfana` | `3.4.0` | 1 |
| `Capnweb` | `capnweb` | `0.12.0` | 1 |
| `AIGatewayProvider` | `ai-gateway-provider` | `4.0.0` | 19 |
| `WorkersAIProvider` | `workers-ai-provider` | `4.0.0` | 5 |
| `AISearchProvider` | `ai-search-provider` | `0.1.1` | 1 |
| `AIUtils` | `@cloudflare/ai-utils` | `1.0.1` | 1 |
| `Think` | `@cloudflare/think` | `0.17.0` | 9 |
| `Flagship` | `@cloudflare/flagship` | `0.5.0` | 2 |
| `PagesPluginCloudflareAccess` | `@cloudflare/pages-plugin-cloudflare-access` | `1.0.5` | 2 |
| `PagesPluginTurnstile` | `@cloudflare/pages-plugin-turnstile` | `1.0.2` | 1 |
| `PagesPluginStaticForms` | `@cloudflare/pages-plugin-static-forms` | `1.0.3` | 1 |

## Shared AI contracts

| Library | npm package | Retained version |
|---|---|---|
| `V4.Provider` | `@ai-sdk/provider` | `4.0.10` |
| `V4.OpenAICompatible` | `@ai-sdk/openai-compatible` | `3.0.44` |
| `V3.Provider` | `@ai-sdk/provider` | `3.0.15` |

AI Gateway and Workers AI share the v4 contract. The retained AI Search package uses v3,
which has its own owner. Runtime Workers is the native resource owner used by Containers.

## Selection boundaries

The configuration lists every selected public import and a reason for each omitted import
or deferred partition. Selected declaration files retain their complete exports and
transitive type dependencies. AI Gateway retains its outward provider adapters.

The broader inventory contains 1,106 public inputs in 81 compiler programs. Its browser UI,
React, mobile clients, optional integrations, developer tooling and alternate preview
surfaces remain opt-in. The selected MCP client is the server-side `agents/mcp/client` import.

See [the library guide](SDK-LIBRARY.md) for commands, catalog ownership, fidelity findings
and the separate runtime/resource lifecycle validation boundary.
