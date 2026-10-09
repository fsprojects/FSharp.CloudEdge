# Build selected SDK partitions

After restoring the pinned profiles and Hawaii tool package, and placing the
primary Xantham checkout at the exact `xantham.source.xanthamRevision` pin:

```sh
npm run build -- --resume
```

This builds the selected SDK/support libraries and the Hawaii management hierarchy together.
The [library guide](SDK-LIBRARY.md) describes the layout, dependency injection and fidelity
contract. Run `npm run build` for fresh generation through both tools.

Every delivered SDK target uses the source adapter in `generators/xantham`.
`config/targets.json` owns its project path and exact Xantham revision once under
`xantham.source`; individual targets only select optional projections. A target without a projection runs `Pipeline.run`, the same pipeline as the Xantham CLI, so library compilation is its compile gate. A target with a projection runs `Pipeline.runProjectedWith`, which also compiles the generated sources against the adapter's Xantham support assemblies before writing them; that validation has no catalog producer assemblies, so a projected target cannot import catalogs. The default `XanthamProjectRoot` is the primary sibling `../Xantham`; set `CLOUDEDGE_XANTHAM_ROOT` for another primary checkout location.

Generation requires clean pinned generator/support sources. Explicit development may set `CLOUDEDGE_ALLOW_DIRTY_XANTHAM=1`, recording the source diff and untracked-source hashes as `source.dirty` in each receipt. Such receipts are development evidence: they authenticate only while the variable is set and the same diff persists. Acceptance regenerates at a committed `xanthamRevision` with the variable unset. Generated projects compile against the `Xantham.Fable.Core` and `Xantham.Fable.Core.TS` packages in `projectDefaults`; `npm run tools:bootstrap -- --only xantham --xantham-source ../Xantham` packs them from the checked-out Xantham revision, so bootstrap at the pin before regenerating.

The generation receipt binds the source adapter configuration, compiled payload,
TypeScript pin, profile and output hashes. Its build receipt associates source and
ancestor build inputs with that payload; `--no-build` refuses stale associations.
One frozen adapter build serves the selected generation closure. Project checks
authenticate it again before compilation. Configurations without `xantham.source`
retain the legacy pinned local Xantham CLI mode.

For SDK-only work:

```sh
npm run build:sdk -- --plan
npm run build:sdk -- --resume
npm run build:sdk -- workers containers
npm run build:sdk -- --all --plan
```

The default is the server selection in [`config/sdk-delivery.json`](../config/sdk-delivery.json) plus every configured support target, including `agents-pi`. A default build therefore materializes the complete delivered inventory that `test:contracts` and `test:agents-pi` read. It includes Agents, Sandbox, Containers, AI Gateway, Workers AI, AI Search and the selected server helpers. React/web-client behavior and optional integration work remain outside the default build. Management stays OpenAPI → Hawaii.

Each library selects exact public subpaths from the pinned compiler partition. Inputs left out require a reason; newly inventoried inputs require an explicit disposition. Included declaration files retain their full exported APIs. Support catalogs are supplied through `catalogImports`. Delivery configuration rejects an import unless the consumer and producer have identical inference settings (normally one named `inferenceProfiles` entry) and the consumer ships the producer's package; Xantham then validates source and API identities. A consumer that generates a shipped declaration absent from every referenced catalog emits it in `groups/<module>.fs`; that file must appear in the target's `files` list, ahead of the entry file.

The command retains canonical inventory metadata in `config/targets.json`. It saves the
materialized configuration in `artifacts/sdk-build/targets.json`, complete disposition
accounting in `artifacts/sdk-build/scope.json`, and the broad partition plan in
`artifacts/sdk-build/partitions.json`. `SDK.Partitions.slnx` contains the requested selected
libraries and their support dependencies. A later default build restores the full server
selection after a narrower explicit build.

`artifacts/sdk-build/latest.json` distinguishes SDK inputs, support projects, generation and
compilation. A failed generation or catalog check prevents compilation. `--resume` verifies
the compiler, tool payload, profile, input overlays, catalog references and source hashes
before reusing generation. Compilation still runs against the entire selected solution.

`--all` deliberately attempts the broad inventory, including deferred integrations and
known incomplete declaration surfaces. That command does not apply the default selection's
support injection. It is available for deliberate inventory work and does not define
the default build. The [scope table](SDK-DELIVERY-SCOPE.md) lists the selected libraries
and versions; `config/sdk-delivery.json` records reasons for excluded inputs.


## Agents Pi operations

`agents-pi` adds Agents 0.27.0's `agents/harness/pi` and `agents/models/pi-ai`
entrypoints beside the existing Agents 0.22 delivery. Its isolated profile pins
pi-ai, pi-durable and Chord at 1.1.0. The generated project owns their reached
binding types in a recursive namespace source, so references between packages
compile together.

```sh
npm run install:profiles -- agents-pi
npm run build:sdk -- agents-pi
npm run test:agents-pi
```

This support target adds early operation projections to the shared source runner. It calls Xantham's projection API and actual Myriad plugin before shaping. Its receipt additionally hashes the exact projection selection and projection provenance in the manifest. It keeps its own profile and imports no catalogs: its Workers types pin `5.20261009.1`, which wrangler 4.130.0 requires, and its projection validation compiles without producer assemblies.

The shared public input is `FSharp.CloudEdge.Pi.HarnessSubmit.Input`. Its `Text`
case takes a string; its `Items` case takes typed text/image payloads. The same
value works with `HarnessSubmit.submit`, `HarnessPrompt.prompt`,
`SessionSubmit.submit` and `SessionPrompt.prompt`. Required `type` tags are
encoded by the generated operation. Image payloads require `Data` and `MimeType`;
text payloads require `Text`. `TextSignature = None` omits the property, while
`Some InputItemsItemTextTextSignature.Undefined` passes an explicit JavaScript
`undefined`. Options and return types come from each actual SDK method.

The raw SDK surface remains available. Its UserInput already has an erased `U2`
representation; the strict operations add closed payload construction and shared
F# types. The public factory context now resolves to the shipped Chord `Context`.
Other lossy mappings remain recorded in the manifest. Reached dependency types
are included; standalone pi-ai/pi-durable value exports remain outside this
Agents entrypoint selection.

`test:agents-pi` pins Fable 5.17.2 and Fable.Core 5.2.0. It enables the optional
SDKComposition consumer with `CloudEdgeIncludeAgentsPi=true`; the default
composition excludes this project and its Pi consumer. It rejects malformed image
payloads at compile time, then runs the generated F# constructor and operations
inside a local SQLite Durable Object. The JavaScript host installs the real
PiHarness lifecycle and supplies pi-ai's deterministic `fauxProvider` through
`createModels`, `createRegistry` and `Harness.open`. The gate checks both submit
and prompt receiver types, encoded mixed content, optional-property presence,
model replies, durable transcripts and idempotency across runtime restart.
Receipts and local runtime state live under ignored `tests/.scratch/agents-pi`;
state is removed when the gate ends. This gate proves local SDK behavior with a
scripted provider; hosted provider credentials and deployed Cloudflare behavior
remain separate integration checks.

### Agents Pi acceptance, 2026-10-09

The delivery pins clean Xantham `2ecbd73cff0ee95b46ebc68ade560a473307e974`.
`npm run test:tooling` passed 184 tests. The explicit generated project and
SDKComposition consumer compiled; the malformed image control failed with F#
field/type diagnostics as expected. The Fable/workerd gate passed all 11
assertions, including restart persistence, with unchanged input hashes. The
source-to-payload control changed the adapter source while retaining its DLL:
reuse was refused, and restoring the source restored the accepted fingerprint.
Generation, build and runtime receipts remain under `artifacts/agents-pi`,
`generators/xantham/bin/Release/net10.0/cloudedge-build.json` and
`tests/.scratch/agents-pi`. The retained binding index is
`tests/.scratch/strict-operation-audit/final-binding.json`.

The shape controls cover string input, arrays of tagged text/image records,
optional omission versus explicit undefined, shared input across four methods,
invalid image fields and invalid null strings. The raw manifest records
84 exact, 308 ergonomic, 294 widened and 12 escape symbols; these grades describe
the selected raw closure, not just the four strict operations. Remaining raw
widenings and standalone dependency value exports are outside this increment.

Retrieval used fresh/latest snapshot
`0cbe94d87d33e4cd8d2f3f0437fa2b9860db04e798b93d6efa5d56ca09edcb97`.
Xantham and CloudEdge were absent from that snapshot, so bounded owning-file
reads used the unindexed-repository exception; uncommitted changes were reviewed
as diffs. F# semantic tools were unavailable; compiler and Fable evidence supplied
the semantic checks. The reviewer verified receipt binding and the default
composition exclusion. No baseline suite was repeated to rediscover accepted
results, and no alternate checkout or Bozzetto gate was used.

Growth from `3bf08618837910e433b779cf6c0396fc900a1080` to this increment's
commit: 29 files changed, 15616 insertions(+), 30 deletions(-). Existing generation, catalog, project and tool
owners were extended. The 16 new files have these owners and reasons:

| New files | Owner | Reason |
| --- | --- | --- |
| `generators/xantham/Program.fs`, `Xantham.CloudEdge.fsproj` | Generation orchestration | Invoke the existing early Xantham/Myriad projection API from the pinned primary checkout. |
| `profiles/agents-pi/package.json`, `package-lock.json` | Profile installation | Pin the isolated Agents 0.27/Pi 1.1 dependency closure. |
| `src/Runtime/Agents/FSharp.CloudEdge.Runtime.AgentsPi/groups/FSharp.CloudEdge.Runtime.fs` | Xantham raw generation | Emit the reached package types together so recursive references compile. |
| The same project's four `FSharp.CloudEdge.Pi.{HarnessSubmit,HarnessPrompt,SessionSubmit,SessionPrompt}.fs` files | Xantham/Myriad operation generation | Emit authenticated typed operations sharing one input definition. |
| The same project's `FSharp.CloudEdge.Runtime.AgentsPi.fsproj` | Project emission | Compile the authenticated ordered source closure. |
| `tests/SDKComposition/Pi.fs` | SDKComposition | Compile the optional API alongside existing library owners. |
| `tests/AgentsPi/{AgentsPi.fsproj,Client.fs,worker.mjs,check.mjs,.config/dotnet-tools.json}` | Agents Pi acceptance | Exercise generated F# calls against the actual pinned SDK and local Durable Object lifecycle. |
