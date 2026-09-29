# Cloudflare completeness and first-delivery boundary

The executable delivery now uses the explicit 112-input, 31-library selection in
[`config/sdk-delivery.json`](../config/sdk-delivery.json), including AI Gateway and its
support owners. See [Selected Cloudflare library](SDK-LIBRARY.md) for current build commands
and acceptance reports. The four-library measurements below are historical checkpoints.

Updated 2026-09-13 with an additive audit of the 2026-09-06 pinned inventory and the user's accepted first-delivery priority. Every existing package pin is retained. Six published voice providers have been added and their tarballs verified against registry SRI. This records the inventory and delivery boundaries; generation, compilation and runtime coverage are tracked separately.

The inventory contains 63 package-version records. The broader canonical configuration contains 60 package-version records across 55 install profiles and 214 canonical entries. Management TypeScript provenance and two deprecated SDKs remain outside Xantham production generation.

## Accepted first-delivery priority

The delivery commitment is to build and validate the Cloudflare resource APIs the project intends to use: native Workers/product bindings, resource SDKs such as Agents, Sandbox and Containers, AI Gateway for Workers AI and external model access, and management access generated directly from the pinned OpenAPI specification by Hawaii. The user has explicitly excluded React and web-client behavior and proposed leaving the broader surface to community demand. This supersedes the earlier first-delivery ambition to accept the entire SDK inventory. General compatibility remains a generator design objective, not a commitment to repair or validate every integration here.

- Fix generator defects required by the selected resource APIs. A general fix may also benefit optional integrations, but their failures alone do not initiate further work.
- Preserve each integration's pins, inputs and explicit validation status. An unvalidated or deferred input is never counted as a successful binding.
- Leave React, React Native, UI framework bindings and web-client behavior outside the delivery scope. Keep them available for community contributions; no further generation repair or behavior validation is planned for them.
- Defer optional integration-specific package repairs, workarounds and deployment validation. Browser automation wrapper/package repairs also require a concrete application need before work resumes; this does not remove the native Browser binding from the inventory. Delivery priority is based on what a surface enables, not only on whether its npm package has a Cloudflare name.
- Preserve correct ownership for dependency types exposed by selected server-side APIs. Supporting those signatures does not imply validating unrelated third-party integrations.
- Report first-delivery acceptance separately from full-inventory acceptance. This policy does not change production target flags or create silent exclusions in the inventory. The generation/compile checkpoint below has an explicit selection; cross-library consumers and runtime acceptance remain outstanding.

The concrete deferral is further validation of `@cloudflare/turnstile-firebase-app-check@0.0.5`. It is an optional browser integration between Turnstile and Firebase App Check. Its Firebase dependency exposed a general catalog ownership defect: a nested module manifest without a version was mistaken for an independent installed package. The general fix has passed the Xantham regression gate (854 generator tests, 90 Wire tests and 461 Fable runtime checks). That evidence validates the generator fix, not the Firebase integration. Integration-specific generation, compilation, package repair and live Firebase deployment work are deferred for the first delivery. The package and its public input remain in the inventory and in the broader planned coverage.

AI Gateway is explicitly retained as a priority. Native `Ai.gateway(id)` / `AiGateway` access is already declared in the selected Workers bindings, and Hawaii's accepted management selection includes 66 AI Gateway operations. Use `ai-gateway-provider@4.0.0` as the selected provider-wrapper integration: a subsequent bounded probe regenerated its 19 inputs with the final tool and compiled net8.0/exact support pins with zero warnings/errors. It was outside the earlier four-library default; it is included in the current scoped build. The [SDK selection and version audit](AI-GATEWAY-SDK-SELECTION-20260913.md) checks the newer 4.0.1 release and records why no pin change is necessary for the current profile. Further investigation of the separate `@cloudflare/ai-gateway` candidate is shelved. The selected route must demonstrate Workers AI access and outbound model-provider access with bounded setup, response/error checks and verified cleanup; this runtime acceptance remains outstanding. The [handoff](FIRST-DELIVERY-HANDOFF-20260913.md) records the evidence paths.

The authorized closeout focuses on shared type ownership and selected resource consumers,
including AI Gateway. The broad inventory remains available for community demand and does
not define the selected delivery's completion percentage. It supersedes the earlier pause
on generator changes.

## Executable first-delivery generation and compilation checkpoint

The earlier `npm run build:sdk` default selected stable Workers, the Agents worker root partition, Containers,
and the Sandbox worker root partition. A packed local Xantham build at `ff35b89` generated
and authenticated all four libraries and compiled each with zero warnings and zero errors.
The exact maps cover **16 public inputs (1 + 12 + 1 + 2)** from the broader 1,106-input
inventory. This does not include every Agents or Sandbox environment/subpath.

The command derives the partition configuration without changing canonical inventory
metadata, and composes staging, receipt validation, project reconciliation and compilation.
The [build guide](sdk-build.md) documents `--plan`, `--resume`, explicit partition selection,
the selected `SDK.Partitions.slnx`, and recorded stage status. All 155 Node tooling tests pass.

The `ff35b89` index-signature identity fix passes 857 generator tests, 90 Wire tests,
the compile gate and 461 Fable runtime checks. The actual Containers standalone output
also compiles with zero warnings/errors; its findings remain visible (9 exact, 9 ergonomic,
24 widened, 1 escape). Neither that result nor the four-library build establishes full
type fidelity or shared dependency ownership.

The first Workers → Containers catalog consumer remains blocked by a general primitive
identity defect, now reduced independently of the SDK: an otherwise unused exported string
alias changes a nested object's identity. Browser dependency placement has a separate
small DOM-augmentation reproducer. Both are generator work within the first-party remit.
No Cloudflare or Firebase resources were created for this checkpoint.

The existing Workers/BAREWire consumer also passed all 13 Fable/JavaScript runtime checks
against the regenerated Workers body bindings in Node Fetch (`bridge-ff35b89.log`). This
validates that recorded consumer boundary; Agents, Containers and Sandbox runtime checks
remain outstanding.

Evidence: `artifacts/one-shot-20260913/build-sdk-first-delivery-ff35b89.log`, the dated
authenticated reports under `artifacts/projects/`, and
`artifacts/one-shot-20260913/containers-compile-results-ff35b89.json`.

## Final verification and stopping point

The [first-delivery handoff](FIRST-DELIVERY-HANDOFF-20260913.md) records the pencils-down
checkpoint at Xantham `35feabd`. The final packed tool again generated, authenticated and
compiled all four selected libraries (16 inputs) with zero warnings/errors. Its composed
regression gate passes 869 generator tests, 90 Wire tests and 461 Fable checks. The original
Agents browser entry with shipped Workers declarations now compiles (6 identifier warnings,
0 errors).

Workers → Containers catalog reuse now reaches a further provenance conflict between
`OutboundHandlerParams` and `Cloudflare.Pipelines.PipelineRecord`. It remains blocked and
unaccepted. A bounded source comparison also records compiler-private brand naming in Agents
for follow-up. These observations close this pass; no further generator fixes are implied by
the checkpoint. Optional integration deferrals remain in force.

## Published SDK inputs currently selected

| Package | Version | Canonical entries | Layer |
|---|---|---:|---|
| `@cloudflare/workers-types` | `5.20260906.1` | 2 | Runtime/Platform |
| `agents` | `0.22.0` | 36 | Runtime/Agents |
| `@cloudflare/ai-chat` | `0.11.0` | 4 | Runtime/AI |
| `@cloudflare/codemode` | `0.5.1` | 6 | Runtime/Agents |
| `@cloudflare/think` | `0.17.0` | 12 | Runtime/AI |
| `@cloudflare/voice` | `0.4.0` | 4 | Runtime/Agents |
| `@cloudflare/shell` | `0.4.3` | 3 | Runtime/Agents |
| `@cloudflare/worker-bundler` | `0.2.3` | 2 | Runtime/Tooling |
| `hono-agents` | `3.0.12` | 1 | Runtime/Agents |
| `@cloudflare/sandbox` | `0.12.9` | 5 | Runtime/Compute |
| `@cloudflare/containers` | `0.3.7` | 1 | Runtime/Services |
| `@cloudflare/dynamic-workflows` | `0.1.1` | 1 | Runtime/Services |
| `@cloudflare/computer` | `0.2.1` | 19 | Runtime/Compute |
| `workers-ai-provider` | `4.0.0` | 5 | Runtime/AI |
| `ai-gateway-provider` | `4.0.0` | 19 | Runtime/AI |
| `ai-search-provider` | `0.1.1` | 1 | Runtime/AI |
| `@cloudflare/tanstack-ai` | `0.2.1` | 11 | Runtime/AI |
| `@cloudflare/ai-utils` | `1.0.1` | 1 | Runtime/AI |
| `@cloudflare/playwright` | `1.3.6` | 3 | Runtime/BrowserAutomation |
| `@cloudflare/puppeteer` | `1.4.0` | 1 | Runtime/BrowserAutomation |
| `@cloudflare/playwright-mcp` | `0.0.5` | 1 | Runtime/BrowserAutomation |
| `@cloudflare/workers-oauth-provider` | `0.10.3` | 1 | Runtime/Services |
| `capnweb` | `0.12.0` | 1 | Runtime/RPC |
| `@cloudflare/flagship` | `0.5.0` | 3 | Runtime/FeatureFlags |
| `@cloudflare/sandbox` | `0.13.0-next.751.1` | 10 | Runtime/Compute |
| `@cloudflare/stream-react` | `1.9.3` | 1 | Runtime/Browser |
| `@cloudflare/stream-angular` | `1.0.1` | 1 | Runtime/Browser/Angular |
| `@cloudflare/turnstile-firebase-app-check` | `0.0.5` | 1 | Runtime/Browser |
| `@cloudflare/realtimekit` | `2.0.2` | 5 | Runtime/Browser |
| `@cloudflare/realtimekit-react` | `2.0.2` | 1 | Runtime/Browser |
| `@cloudflare/realtimekit-ui` | `2.0.2` | 2 | Runtime/Browser |
| `@cloudflare/realtimekit-react-ui` | `2.0.2` | 2 | Runtime/Browser |
| `@cloudflare/realtimekit-angular-ui` | `2.0.2` | 1 | Runtime/Browser/Angular |
| `@cloudflare/realtimekit-react-native` | `2.0.0` | 1 | Runtime/Mobile |
| `@cloudflare/realtimekit-react-native-ui` | `2.0.0` | 1 | Runtime/Mobile |
| `@cloudflare/react-native-webrtc` | `137.0.1` | 1 | Runtime/Mobile |
| `@cloudflare/realtimekit-recording-sdk` | `0.0.3` | 1 | Runtime/Browser |
| `@cloudflare/realtimekit-ui-addons` | `0.1.0` | 11 | Runtime/Browser |
| `@cloudflare/realtimekit-virtual-background` | `0.0.2` | 1 | Runtime/Browser |
| `@cloudflare/actors` | `0.0.1-beta.6` | 3 | Runtime/Services |
| `chanfana` | `3.4.0` | 1 | Runtime/Services |
| `@cloudflare/cabidela` | `0.2.4` | 1 | Runtime/Services |
| `@cloudflare/kv-asset-handler` | `0.5.0` | 1 | Runtime/Services |
| `@cloudflare/ai-gateway` | `0.0.6` | 2 | Runtime/AI |
| `@cloudflare/pages-plugin-cloudflare-access` | `1.0.5` | 2 | Runtime/Pages |
| `@cloudflare/pages-plugin-google-chat` | `1.0.4` | 2 | Runtime/Pages |
| `@cloudflare/pages-plugin-graphql` | `1.0.4` | 1 | Runtime/Pages |
| `@cloudflare/pages-plugin-hcaptcha` | `1.0.4` | 1 | Runtime/Pages |
| `@cloudflare/pages-plugin-honeycomb` | `1.0.4` | 1 | Runtime/Pages |
| `@cloudflare/pages-plugin-sentry` | `1.1.4` | 1 | Runtime/Pages |
| `@cloudflare/pages-plugin-static-forms` | `1.0.3` | 1 | Runtime/Pages |
| `@cloudflare/pages-plugin-stytch` | `1.0.3` | 2 | Runtime/Pages |
| `@cloudflare/pages-plugin-turnstile` | `1.0.2` | 1 | Runtime/Pages |
| `@cloudflare/pages-plugin-vercel-og` | `0.1.2` | 2 | Runtime/Pages |

## Additive voice provider coverage

| Package | Pinned version | Public entries |
|---|---|---|
| `@cloudflare/voice-assemblyai` | `0.2.0` | root |
| `@cloudflare/voice-deepgram` | `0.2.0` | root |
| `@cloudflare/voice-elevenlabs` | `0.2.0` | root |
| `@cloudflare/voice-plivo` | `0.2.0` | root |
| `@cloudflare/voice-telnyx` | `0.2.0` | root, stt, tts, browser |
| `@cloudflare/voice-twilio` | `0.1.0` | root |

These packages import `agents/voice` and require Agents >=0.23.0. The separate `voice-providers` profile pins Agents 0.23.0 as their dependency; the existing canonical Agents SDK remains 0.22.0. Shared voice type ownership must stay coherent within the new profile, with an explicit browser partition for Telnyx.

Sources: [Cloudflare voice provider sources](https://github.com/cloudflare/agents/tree/main/voice-providers), [voice documentation](https://developers.cloudflare.com/agents/communication-channels/voice/). Exact registry metadata and verified archive hashes are recorded in `inventory/cloudflare-sdk-inventory-20260913.json` and `config/sdk-surfaces.json`.

## Public export coverage beyond selected generation inputs

The existing catalog also records 892 concrete wildcard entries marked for production coverage: 618 in Puppeteer and 274 in RealtimeKit UI. Together with the 214 canonical entries, these make 1,106 production public inputs. The partition plan assigns all of them to 81 compiler programs; this is planned coverage, not successful generation. They are not yet reconciled against emitted public names. Root import reachability does not establish completeness, and the 214 selected generation inputs do not count the wildcard inputs as passed. Runtime aliases, declaration-only paths and conditional variants require separate accounting.

## Native/application bindings

These require explicit compatibility dates, flags and binding configurations. Listing them in this inventory does not prove that generated library coverage exists.

- Workers platform core and product bindings: cloudflare:workers, cloudflare:sockets, cloudflare:email, Durable Objects, Workflows, Worker Loader, KV, R2, D1, Queues, Workers AI, Vectorize, Hyperdrive, Pipelines, Browser binding, AI Gateway, Service bindings, Rate Limiting, Secrets Store, Analytics Engine, Images, Static Assets, Dynamic Dispatch, Media Transformations, mTLS, Stream, Version Metadata
- Agent Memory: Native env binding with profile ingestion, remember and recall; optional Agents session integration.
- Artifacts: Native env binding for repositories, files, forks and tokens; optional @cloudflare/computer/artifacts wrapper.
- Flagship: Native env binding plus separate @cloudflare/flagship OpenFeature SDK.
- Email Service: Native Workers send/route APIs and REST operations.

## Management API

Official pinned OpenAPI through Hawaii: 3,448 source operations; the previous accepted selection was 3,437 operations in 12 projects after 8 retired and 3 internal exclusions. The TypeScript `cloudflare` package is provenance, not an additional production management implementation.

## Inventory records outside production generation

- `cloudflare@7.1.0`: management_rest_sdk.
- `agents-sdk@0.0.36`: deprecated.
- `@cloudflare/ai@1.2.2`: deprecated.

## Other exclusions and dependencies

Tooling outside SDK binding generation: `wrangler`, `@cloudflare/vite-plugin`, `@cloudflare/vitest-pool-workers`, `miniflare`, `workerd`, `create-cloudflare`.

Private, unpublished, or invalid names as classified on 2026-09-06:

- `@cloudflare/channels`: Private source package; not an independently published SDK.
- `@cloudflare/computer-rpc`: Private internal Computer RPC source.
- `@cloudflare/dofs`: Private Computer filesystem source.
- `@cloudflare/computerd`: Private source daemon; binary distribution is a separate tooling question.
- `@cloudflare/gateway-core`: Private source bundled into consumers.
- `@cloudflare/realtimekit-web`: Name appears in some documentation; actual web core package is @cloudflare/realtimekit.
- `@cloudflare/realtimekit-angular`: Name appears in some documentation; core is @cloudflare/realtimekit, UI wrapper is @cloudflare/realtimekit-angular-ui.
- `@cloudflare/hono-agents`: Actual official published name is hono-agents.

Third-party dependencies are not additional Cloudflare SDKs, but their types still require explicit ownership when exposed by public signatures: `ai and @ai-sdk/*`, `@modelcontextprotocol/*`, `@tanstack/ai`, `hono`, `react`, `zod`, `mppx and viem`, `@x402/*`, `@browserbasehq/stagehand`, `@openai/agents`, `@opencode-ai/sdk`, `@xterm/*`, `@openfeature/*`.

Go and Python SDKs are outside this TypeScript-to-F# input inventory.

## Acceptance required for the selected delivery

- Record the exact server-side resource and AI Gateway inputs selected for the release, with pinned compatibility profiles. Keep the larger inventory separate from the release denominator.
- Produce coherent shared support/dependency owners and deterministic manifests and compile order for that selection.
- Generate and compile the selected libraries together, execute representative typed Fable consumers, and report widened or escaped signatures explicitly.
- Validate the intended Workers AI and external-provider access paths through AI Gateway.
- Exercise bounded resource setup, use and teardown where applicable, verifying cleanup as part of the test.

## Broader inventory work available to contributors

These are requirements for any future claim of full-inventory acceptance, not blockers for
the selected delivery. A contributor can instead accept a smaller, explicit addition using
the same generation, type ownership, compilation and consumer checks.

- Re-audit package and product discovery before calling the complement complete. Current evidence is a dated snapshot.
- Reconcile every public subpath and concrete wildcard expansion with an emitted surface or an explicit, reasoned status. Full-inventory acceptance cannot count deferred first-delivery integrations as passed, and successful job counts alone do not prove input coverage.
- Cover native bindings through representative declared compatibility profiles.
- Produce the common support/dependency owners and separated runtime/browser/mobile/management projects with deterministic manifests and compile order.
- Compile all generated projects together, execute representative Fable consumers, and report remaining widened or escaped signatures rather than equating compilation with complete fidelity.
- Keep live setup/exercise/teardown validation as a separate acceptance stage.

## September 13 generation evidence

### Latest partition checkpoints

These are diagnostic probes using immutable local tool builds, not production acceptance receipts. The production configuration still has 214 canonical generation entries; adopting the 81-program partition plan into the production one-shot pipeline remains work to do.

| Checkpoint | Evidence | What it establishes |
|---|---|---|
| Partition plan | 81 compiler programs account for all 1,106 production public inputs | Planned coverage only. |
| Xantham `14a1849` generation probe | 71 programs generated, covering 187 public inputs; 10 programs failed, containing the remaining 919 inputs | Successful generation for the reported inputs only. The large Puppeteer and RealtimeKit UI wildcard programs are among the failures. |
| Xantham `14a1849` compilation probe | 66 of the 71 generated programs compiled; five failed; aggregate build reported 316 warnings and 24 errors | Partial compilation evidence for that tool version. |
| Xantham `4787a98` targeted recovery | All five previously failing programs regenerated and compiled: `agents`, `computer`, `computer--artifacts`, `sandboxpreview--bridge`, `think`; 63 warnings, zero errors | The public-subpath class-shape fix resolves those failures in a separate run. This is not a fresh unified 71-program acceptance run. |

Evidence: [partition plan](../artifacts/one-shot-20260913/partitions-plan.json), [generation probe at `14a1849`](../artifacts/one-shot-20260913/partition-probe-results-14a1849.json), [compilation log at `14a1849`](../artifacts/one-shot-20260913/build-partition-probes-14a1849.log), and [five-program recovery at `4787a98`](../artifacts/one-shot-20260913/class-partition-results-4787a98.json).

The 10 generation failures in the `14a1849` snapshot are `aigatewaycandidate`, `containers`, `playwright`, `puppeteer`, `realtimekit`, `realtimekitreactnativeui`, `realtimekitui`, `realtimekituiaddons--camera-host-control`, `think--react` and `turnstilefirebaseappcheck`. These include both generator/catalog defects and unresolved published package inputs; they are not all attributable to the same cause. The subsequent general package-ownership regression fix does not establish that the Firebase program now passes. Its further integration validation is deferred under the first-delivery boundary above.

The CloudEdge tooling suite subsequently passed 150 tests, including partition-planning checks. A green Xantham or CloudEdge tooling regression gate does not establish successful bindings for the full inventory. Shared dependency ownership, a unified production generation/compile run, representative Fable consumers and bounded live setup/exercise/teardown remain separate acceptance requirements.

### Earlier checkpoints retained for context

Local Xantham checkpoint `1706c1e`: the original 205-entry pass produced 147 accepted outputs, 15 failures and 43 blocked entries. This was generation evidence only. Package-wide Shell generation succeeded in an isolated experiment; package-wide Agents failed catalog identity construction at that checkpoint. The dated generation report and logs live under `artifacts/one-shot-20260913/`.

The nine added voice entries all generated and compiled against the pinned local support packages. The original 57 inventory records were compared as JSON values and are unchanged. At that checkpoint, 144 CloudEdge tooling tests passed. These results do not establish shared dependency identity or runtime conformance. A separate Deepgram probe also generated and compiled a reachable Agents dependency module; that module needs version-specific ownership before integration.

Further local generator probes (after checkpoint `cfd00d3`): the existing Agents browser entry compiled after correcting merged DOM/value handling and colliding exclusive-arm factories. Its manifest changed from 28/39/16/4 to 33/37/21/4 (exact/ergonomic/widened/escape); missing augmented dependency types were reported explicitly. Shipping the reachable Workers dependency into a browser compatibility module exposed references back to the later root module, so that stratification experiment was not accepted as a compilable library. Package-wide Agents reached a constructor identity conflict after the subpath naming fixes; later generator fixes and the partition checkpoints above supersede that particular failure. The ownership-cycle experiment remains distinct from those later compile results.
