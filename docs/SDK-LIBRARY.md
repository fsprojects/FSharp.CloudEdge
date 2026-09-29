# Selected Cloudflare library

The [September 13 acceptance record](SDK-DELIVERY-ACCEPTANCE-20260913.md) closes the selected
delivery and lists its verified gates and remaining limitations.

The delivery scope is the explicit server-side selection in
[`config/sdk-delivery.json`](../config/sdk-delivery.json): 112 public SDK entrypoints in
31 libraries, plus three generated AI support owners, a small handwritten Workers adapter,
and the separate Hawaii management
projects. The [scope table](SDK-DELIVERY-SCOPE.md) lists every selected library and retained
package version. The broader inventory remains available for community extensions.

## Build

```sh
npm run build -- --plan
npm run build -- --resume
```

`build` generates the selected SDKs with the pinned local Xantham tool, generates management
from the pinned OpenAPI specification with Hawaii, and compiles the complete hierarchy in
`FSharp.CloudEdge.slnx`. Hawaii validates its staged Release build and local HTTP consumer
before installing generated files. `--resume` reuses SDK output only after authenticating
its inputs, catalogs, tool and source hashes. It also authenticates existing Hawaii output
against the generator package, schema, policies, overlays and owned files before reuse.
Pipeline and HTTP consumer source hashes are checked too; the installed ownership set must
match the accepted staged run in full.
Compilation still runs.

`npm run build:sdk -- --resume` builds just the selected SDK/support hierarchy.
`npm run build:sdk -- workers containers` selects a smaller dependency closure.
`npm run build:sdk -- --all --plan` inspects the larger inventory; `--all` attempts it,
including deferred integrations. Its results do not define this delivery's completion.

## Library hierarchy

| Directory | Contents |
|---|---|
| `src/Core` | Hawaii's shared API models |
| `src/Support` | Generated AI model contract owners and the handwritten Durable Object fetch adapter |
| `src/Runtime/Platform` | Native Workers resource declarations |
| `src/Runtime/Agents` | Agents, server MCP client, CodeMode, Shell and server Voice |
| `src/Runtime/AI` | AI Gateway, Workers AI, AI Search, AI Chat, AI Utils and Think |
| `src/Runtime/Compute` | Sandbox and Computer |
| `src/Runtime/Services` | Containers, Actors, Dynamic Workflows, OAuth and resource helpers |
| `src/Runtime/RPC` | Capnweb |
| `src/Runtime/FeatureFlags` | General and server Flagship APIs |
| `src/Runtime/Pages` | Cloudflare Access, Turnstile and Static Forms server middleware |
| `src/Management` | Hawaii clients grouped by service purpose |
| `src/Tenancy` | Hawaii account, membership and access-management APIs |

`FSharp.CloudEdge.Bindings.slnx` contains the selected generated hierarchy and its handwritten
Workers support project.
`FSharp.CloudEdge.slnx` also contains the consumer projects. `SDK.Partitions.slnx` and
`Hawaii.Bindings.slnx` expose the two generator pipelines separately.

## Exact scope and catalog injection

Each delivery library lists its public imports explicitly. Omitted imports and whole
partitions require a recorded reason. A newly inventoried import cannot silently enter
the delivery. Selection preserves each included declaration file's complete exports and
transitive type dependencies; it does not edit generated F# or prune difficult root members.
React, web clients, mobile UI, separate optional integrations and alternate preview surfaces
remain outside this selection. `agents/mcp/client` is retained as the server-side
`AgentsMcpClient` library.

`supportTargets` defines ordinary generated libraries for dependency types. `catalogImports`
injects their authenticated declaration catalogs into consumers and orders producer generation
before consumption. For example:

```json
{
  "catalogImports": {
    "containers": ["workers"],
    "ai-openai-compatible-v4": ["ai-provider-v4"],
    "aigatewayprovider": ["ai-provider-v4", "ai-openai-compatible-v4"],
    "workersaiprovider": ["ai-provider-v4"],
    "aisearchprovider": ["ai-provider-v3"]
  }
}
```

The producer package must be directly pinned in its installation profile. A transitive
dependency may be promoted at its existing locked version; doing so must retain resolved
versions and tarball integrities. Generator inference settings, including the complete
`groups` map, must match the intended catalog context. Catalog source and F# API checks remain
enforced. The build authenticates owner provenance and derives the corresponding project
references; sharing a package name alone is insufficient.

AI Gateway and Workers AI retain AI SDK v4 model contracts. The pinned AI Search provider
retains v3 contracts in a separate support library. These interfaces are not interchangeable.
Workers supplies the native resource owner directly to Containers. Both generation programs
use the retained Containers profile's exact Workers pin and matching inference settings;
there is no second generated Workers support assembly in this selection.
The [AI Gateway selection note](AI-GATEWAY-SDK-SELECTION-20260913.md) records the retained
provider version and the comparison with its newer patch release.

## Evidence and fidelity

`artifacts/sdk-build/scope.json` records every selected and deferred public input.
`artifacts/sdk-build/targets.json` is the exact materialized generation configuration.
`artifacts/sdk-build/latest.json` records SDK generation and compilation;
`artifacts/library-build/latest.json` records the complete hierarchy build.
`artifacts/library-build/fidelity.json` presents the selected targets' symbol grades and
links to their detailed findings. Those per-target counts are not a deduplicated total
across the libraries.
Each target retains `manifest.json`, per-symbol findings, `declarations.json`, input
fingerprints and generation receipts under `artifacts/<target>/`.
SDK projects retain the existing `FS1104` suppression for generated identifiers containing
`@`. Compiler warning totals reflect that project setting; type-mapping losses remain
visible in the fidelity findings.

The fidelity bar is useful typed consumers and consistent shared contracts, with every
remaining loss reported. It is not zero widened declarations. A deliberately opaque
boundary remains visible in the findings. Consumer helpers belong in separate F# sources;
generated files stay reproducible generator output. Catalog source/API mismatches are
failures to resolve, not something an injection setting can waive.

The bounded widening policy is to repair a mapping when it prevents a selected consumer
from passing a resource or model directly between libraries. Unknown values, unsupported
TypeScript constructs and deliberate recursion cutoffs retain their reported losses.
Reducing the aggregate count alone is not a delivery requirement.

There are two extension mechanisms. Generated support types use the catalog imports above,
which authenticate both ownership and the exposed F# API. Xantham also supports explicit
`groups` mappings from a TypeScript name to a supplied F# type and its generic arity. That
mapping is a caller-provided contract: the caller supplies the support library, project
reference and typed consumer checks. It does not establish catalog compatibility by itself.
Use a separate helper or support source for a deliberately narrowed boundary, and keep its
assumptions documented beside its consumer checks. Neither mechanism requires editing
generated bindings.

The callback injection probe demonstrates why the runtime check matters. A named
two-argument F# delegate passes callbacks in both directions through Fable. A curried
function alias compiles, but a returned JavaScript callback fails when invoked without an
adapter. Generic callback mapping also exposes a current `TR053` arity limitation. See
`artifacts/one-shot-20260913/callback-injection-probe/` for the isolated evidence; these
results qualify the extension mechanism and do not expand the selected SDK repair scope.
Keeping the imported value typed as a delegate and exposing
`fun value count -> callback.Invoke(value, count)` gives a tested F# function adapter,
including partial application of a callback returned from JavaScript.
Ordinary Xantham generation of that callback already chooses a delegate and passes its
runtime control. An inline adapter disappears entirely for direct calls; retained partial
application still creates a closure.

Generation, compilation, typed composition and runtime/resource lifecycle checks are distinct
gates. A compiled library does not establish Cloudflare deployment behavior. Live inference
and setup/exercise/verified-cleanup checks remain a separate validation phase.
The [Durable Object acceptance criteria](DURABLE-OBJECTS-ACCEPTANCE.md) cover Conclave's
critical mailbox and supervision dependencies explicitly.

## Retired local output

After a successful complete library build, archive generated directories left by the broader
inventory runs:

```sh
node scripts/archive-sdk-output.mjs --dry-run
node scripts/archive-sdk-output.mjs
```

The archive requires matching SDK and complete-library build evidence, verifies ownership
and source hashes, and checks retained project references. It moves eligible directories
into a dated `artifacts/sdk-output-archive/` directory with an inventory and build receipts.
Edited or unowned files are retained and reported. Canonical inventory configuration and
generation evidence remain available for subsequent opt-in builds.
