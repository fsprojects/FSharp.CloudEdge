# Build selected SDK partitions

After bootstrapping the reviewed local generator/support packages and restoring
the pinned dependency profiles:

```sh
npm run build -- --resume
```

This builds the selected SDK/support libraries and the Hawaii management hierarchy together.
The [library guide](SDK-LIBRARY.md) describes the layout, dependency injection and fidelity
contract. Run `npm run build` for fresh generation through both tools.

Ordinary delivered SDK targets use the authenticated packaged Xantham CLI.
The optional Agents Pi projection uses the source adapter in `generators/xantham`,
whose project and exact source revision are recorded with the projection configuration.
Projected generation runs `Pipeline.runProjectedWith` and compiles its output against
the adapter's support assemblies and freshly compiled catalog owners before accepting it.
The existing project builder authenticates and compiles those owners first; their DLL
hashes participate in projection receipts and resume checks. Set `CLOUDEDGE_XANTHAM_ROOT`
to the reviewed checkout when its location differs from the default sibling layout.

Generation requires clean pinned generator/support sources. Explicit development may set `CLOUDEDGE_ALLOW_DIRTY_XANTHAM=1`, recording the source diff and untracked-source hashes as `source.dirty` in each receipt. Such receipts are development evidence: they authenticate only while the variable is set and the same diff persists. Acceptance regenerates at a committed `xanthamRevision` with the variable unset. Generated projects compile against the `Xantham.Fable.Core` and `Xantham.Fable.Core.TS` packages in `projectDefaults`; `npm run tools:bootstrap -- --only xantham --xantham-source ../Xantham` packs them from the checked-out Xantham revision, so bootstrap at the pin before regenerating.

Generation receipts bind the selected tool payload, TypeScript pin, installed profile,
catalog references and output hashes. Project checks authenticate those inputs again
before compilation. Projected targets additionally bind the adapter configuration,
source and ancestor build inputs; `--no-build` refuses stale build associations.
That option also authenticates the catalog owners' existing DLLs against their
checked source, project and inherited build inputs without compiling or rewriting
projects. Dependency builds record `<target>-dependencies.json` under
`artifacts/projects/`; `latest.json` continues to describe the explicit bindings build.

For SDK-only work:

```sh
npm run build:sdk -- --plan
npm run build:sdk -- --resume
npm run build:sdk -- workers containers
npm run build:sdk -- --all --plan
```

The default is the server selection in [`config/sdk-delivery.json`](../config/sdk-delivery.json) plus every configured support target, including `agents-pi`. It includes Agents, Sandbox, Containers, AI Gateway, Workers AI, AI Search and the selected server helpers. Pi's additional composition and runtime gate remains opt-in. React/web-client behavior and optional integration work remain outside the default build. Management stays OpenAPI → Hawaii.

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

`agents-pi` adds Agents 0.30.1's `agents/harness/pi` and `agents/models/pi-ai`
entrypoints in an isolated profile alongside the main Agents 0.30.1 delivery. It pins
pi-ai, pi-durable and Chord at 1.4.0. The `pi-ai` and `pi-durable` producers select
their public factory exports and own their types. AgentsPi imports those catalogs
and the main Agents catalog, including its Lifecycle contracts. Every owner uses
the same inference profile and Node/Workers declarations.

The current F# host and celld gate are under validation. The coverage described
below is the gate's intended contract; generation, compilation and runtime
acceptance for this revision are still pending.

```sh
bun scripts/build-sdk.mjs agents-pi
bun tests/AgentsPi/check.mjs
```

This support target adds early operation projections to the source runner. It calls Xantham's projection API and actual Myriad plugin before shaping. Its receipt additionally hashes the exact projection selection and projection provenance in the manifest. Its profile uses Workers types `5.20261025.1` and Node declarations `26.4.1`; imported catalog owners are compiled before projection validation.

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
F# types. Fresh-generation compilation must verify that the constructor and factory
callback share the same canonical Chord context type.
Other lossy mappings remain recorded in the manifest. The PiAI producer also
selects `api/openai-completions`, allowing an explicit provider to call a local
OpenAI-compatible inference endpoint such as Magnitude. Workers AI remains a
separate provider through `agents/models/pi-ai`.

The Pi gate pins public Fickl 0.7.2 and Fable.Core 5.6.0. It checks the optional
SDKComposition inclusion property and links that existing Pi witness into the
selected Pi consumer project; full SDK composition remains a separate build.
It rejects malformed image payloads at compile time, then runs the generated
bindings in a local SQLite cell on celld. The F# host subclasses the native
DurableObject, installs Lifecycle and PiHarness, and supplies pi-ai's deterministic
`fauxProvider` through generated `createModels`, `createRegistry` and `Harness.open`
exports. It forwards `onWake` so Lifecycle owns the physical alarm. The gate checks both submit
and prompt receiver types, encoded mixed content, optional-property presence,
model replies, durable transcripts and idempotency across runtime restart.
Receipts and local runtime state live under ignored `tests/.scratch/agents-pi`;
state is removed when the gate ends. This gate is intended to prove local SDK behavior with a
scripted provider; hosted provider credentials and deployed Cloudflare behavior
remain separate integration checks.

### Historical Agents Pi acceptance, 2026-10-09

This earlier delivery used clean Xantham `2ecbd73cff0ee95b46ebc68ade560a473307e974`.
Its counts and retained receipts below do not qualify the current SDK versions.
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
