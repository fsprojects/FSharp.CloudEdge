# First-party Cloudflare build checkpoint — pencils down

Historical checkpoint at Xantham `35feabd`. The user subsequently authorized the scoped
library closeout. Current selection, commands and evidence paths are documented in
[Selected Cloudflare library](SDK-LIBRARY.md): 112 public inputs in 31 SDK libraries,
including AI Gateway, plus explicit support owners and Hawaii management.

The selected first-party bindings are generated and compile through one command. This
checkpoint is ready for review and targeted consumer work. Shared dependency ownership and
runtime validation for the selected resources remain incomplete, so it is not yet an
accepted release of that selected surface.

```sh
npm run build:sdk -- --resume
```

The current packed Xantham tool is `0.1.0-local.80272a2344056b6a0b9a`, built from
[`35feabd`](https://github.com/shayanhabibi/Xantham/commit/35feabd5a975f626215762fa3365416b778d4fcd).
The fixes are pushed on `fix/pr84-catalogs-and-mixed-roots` in
[draft PR 85](https://github.com/shayanhabibi/Xantham/pull/85).

## Delivery boundary after user scope clarification

Build and validate the Cloudflare resource APIs the project intends to use. The full
inventory is available for community contributions; completing it is not a delivery
commitment. This supersedes the earlier ambition to accept every inventoried SDK in the
first delivery.

- Prioritize server-side Workers resource access, Agents, Sandbox, Containers, the selected
  OpenAPI → Hawaii management APIs, and AI Gateway for Workers AI and external model access.
- React, React Native, UI frameworks and web-client behavior are outside the delivery scope.
  Keep their existing inputs and evidence available without pursuing their generation
  failures or behavior tests. Browser automation wrapper/package repairs also wait for a
  concrete application need; the native Workers Browser binding remains in the inventory.
- Firebase and other optional integration-specific work remain deferred. Dependencies
  required by a selected server-side API still need correct type ownership; that does not
  create a commitment to validate the dependency's unrelated integrations.
- Apply the same acceptance bar to every selected API: reproducible generation, coherent
  shared types, compilation, representative typed consumers, explicit fidelity findings,
  and bounded setup/exercise/cleanup checks where applicable.

AI Gateway is an explicit delivery priority. Native access is already present in the
selected Workers declarations: `Ai.gateway(id)` and the `AiGateway` surface, with Workers AI
and external-provider request types. Hawaii's accepted management selection includes 66
AI Gateway operations, including lifecycle operations. The separately selectable
`ai-gateway-provider@4.0.0` program is the selected wrapper integration. A subsequent
bounded probe regenerated all 19 inputs with the final tool and compiled against net8.0
and the exact project support pins with zero warnings/errors. It was outside the earlier
four-library default; it is included in the current scoped build. Runtime acceptance remains separate. The
[SDK selection and version audit](AI-GATEWAY-SDK-SELECTION-20260913.md) explains why the
existing pin is suitable even after the September 11 `4.0.1` release. Further investigation
of the separate `@cloudflare/ai-gateway@0.0.6` candidate is shelved; its failure does not
establish that AI Gateway product access is unavailable.

Product acceptance still needs a typed consumer exercising Workers AI and an external
provider, response/error checks, and bounded gateway creation, deletion and absence
verification. Evidence for the existing declarations and management selection is recorded
in `artifacts/workers/declarations.json` and
`artifacts/hawaii/runs/20260909T152527/operation-coverage.json`; provider probe evidence is in
`artifacts/one-shot-20260913/ai-gateway-provider-final-probe/results.json` and its `build.log`.
These are separate binding/build
checkpoints, not AI Gateway runtime acceptance.

## Broader inventory snapshot — community backlog

These counts describe the retained broad inventory. They are not the remaining delivery
denominator, and excluded client/integration programs do not block the selected release.

The four-library build below is the repeatable selected checkpoint, not the total generated
surface. Recorded broad runs have successfully generated **72 of 81 SDK compiler programs**,
covering every planned program for **51 of 60 package-version records**, and **188 of 214
canonical inputs**. These successes span recorded tool revisions; they are not a single
full-inventory acceptance run on the final tool.

| Inventory area | Programs generated / planned |
|---|---:|
| Native Workers runtime declarations | 2 / 2 |
| Agents family | 14 / 14 |
| Compute, including Sandbox and Computer | 8 / 8 |
| Resource/service SDKs, including Containers | 7 / 7 |
| Voice providers | 7 / 7 |
| AI SDKs/providers | 10 / 12 |
| Browser and Angular integrations | 8 / 12 |
| Browser automation | 1 / 3 |
| Mobile SDKs | 2 / 3 |
| Pages plugins | 10 / 10 |
| RPC, feature flags and tooling | 3 / 3 |

Management has separate accepted generation/build evidence for **3,437 selected OpenAPI
operations across 12 Hawaii projects**. Its pinned source contains 3,448 operations; the
selection excludes 8 retired and 3 internal routes. That is the selected management scope,
with its documented JSON-preservation exceptions, not a new run in this final verification.

Nine SDK programs still lack successful generation evidence: Playwright, Puppeteer,
RealtimeKit core/UI/React Native UI/UI addons, Think React, the malformed
`@cloudflare/ai-gateway@0.0.6` candidate, and the explicitly deferred Firebase integration.
The other AI Gateway provider, Workers AI provider and AI Search provider programs have
successfully generated.

Program counts are not API-method percentages. The full 1,106-input denominator includes
892 additional wildcard inputs, concentrated in the blocked Puppeteer and RealtimeKit UI
programs. Strict full-input generated coverage is therefore 188 / 1,106, even though most
programs and the primary server-side families have generation evidence. Those wildcard
inputs remain visible and unaccepted. The machine-readable breakdown is
`artifacts/one-shot-20260913/generation-overview-at-handoff.json`.

## What is in hand

| Selected compiler program | Retained package pin | Public inputs | Final compile |
|---|---|---:|---|
| Stable Workers | `@cloudflare/workers-types@5.20260906.1` | 1 | 0 warnings, 0 errors |
| Agents worker-root partition | `agents@0.22.0` | 12 | 0 warnings, 0 errors |
| Containers | `@cloudflare/containers@0.3.7` | 1 | 0 warnings, 0 errors |
| Sandbox worker-root partition | `@cloudflare/sandbox@0.12.9` | 2 | 0 warnings, 0 errors |

These are **16 selected public inputs in four libraries**. Their exact maps and authenticated
build receipts are saved in `artifacts/sdk-build/`; `SDK.Partitions.slnx` contains the selected
projects. Other Agents/Sandbox environments, provider wrappers and the broader SDK inventory
remain separately selectable. Compilation includes the reported widened and escape mappings;
it does not establish complete type fidelity.

The full inventory remains 1,106 production inputs planned into 81 compiler programs. No
deferred input has been counted as a successful binding. Management continues through the
separate OpenAPI → Hawaii pipeline; no TypeScript management SDK conversion was added.

## Validation completed

| Check | Result |
|---|---|
| Combined Xantham regression gate | 869 generator tests, 90 Wire tests, compile gate and 461 Fable checks pass |
| CloudEdge orchestration tests | 155 tests pass |
| Selected one-command generation, receipt checks and compilation | All four libraries pass with the final packed tool |
| Authenticated resume | Passed at the preceding checkpoint; changed tool fingerprints triggered fresh generation in the final run |
| Original Agents browser entry with shipped Workers declarations | Generates and compiles; 6 FS1104 identifier warnings, 0 errors |
| Workers/BAREWire Fable consumer in Node Fetch | 13 runtime checks pass; final Workers source is byte-identical to that run |
| Live Cloudflare/Firebase resources created | 0 |

The Agents browser probe is retained regression evidence; further web-client validation is
outside the delivery scope. Agents, Containers, Sandbox and AI Gateway runtime consumers and
setup/exercise/cleanup checks remain further work within the selected scope.

## The next blocking boundary

Workers → Containers catalog reuse still fails before a shared typed consumer can be built:

```text
source hash mismatch for OutboundHandlerParams
(FSharp.CloudEdge.Support.ContainersWorkers.Cloudflare.Pipelines.PipelineRecord)
```

The earlier primitive-alias defect is fixed and has a minimal typed regression. This further
record-alias provenance conflict is recorded for the next pass; the catalog guard remains
enforced. Independently compiled Workers and Containers libraries do not prove their types
are interchangeable. Reproduce with a fresh run label using
`node artifacts/one-shot-20260913/probe-core-catalogs.mjs followup`.

A final bounded source comparison also found two emitted compiler-private brand names in
Agents whose numeric suffixes changed between checkpoints. The four changed lines are in
`artifacts/one-shot-20260913/agents-final-binding.diff`; aggregate findings and line counts are
unchanged. Their visibility and naming need a regression before claiming stable public output.
No further generator changes were made after these final observations.

When implementation resumes, address that ownership boundary, connect validated support
owners to the one-command build, validate the selected AI Gateway access path, and add typed
runtime consumers for the intended first-party resources. The broader inventory is available
for demand-driven community work. This scope update does not restart generator repairs or
live validation after the pencils-down checkpoint.

## Evidence and local changes

- Final summary: `artifacts/one-shot-20260913/first-delivery-final-summary.json`.
- Final selected build: `artifacts/one-shot-20260913/build-sdk-first-delivery-35feabd.json` and its `.log`.
- Browser result: `artifacts/one-shot-20260913/agents-browser-runs/35feabd-original/results.json`.
- Shared-owner failure: `artifacts/one-shot-20260913/core-catalogs-35feabd/containers/generate.log`.
- Runtime check: `artifacts/one-shot-20260913/bridge-ff35b89.log`.
- [Build command and saved configuration](sdk-build.md).
- [Complete inventory and delivery policy](CLOUDFLARE-COMPLETENESS-SCOPE-20260913.md).

Xantham changes are committed and pushed. CloudEdge's build flow, generated libraries and
reports remain in this local working directory, which has no Git repository initialized.
The transferable findings are also recorded in Composer's
`docs/javascript-targeting/07_dependency_identity_and_validation.md`; those documentation
changes remain uncommitted under Composer's local repository instructions.
