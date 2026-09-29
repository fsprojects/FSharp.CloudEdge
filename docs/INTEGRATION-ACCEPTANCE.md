# Integration acceptance

Cloudflare acceptance follows **build, deploy, exercise, clear, verify absence**. Each run creates its own fixtures and returns a machine-readable result after cleanup. FSharp.CloudEdge owns the SDK contracts. Conclave owns the application behaviors assembled from them.

Status on September 13, 2026: the accepted library delivery has compilation, generator, HTTP loopback and bounded local runtime evidence. A complete remote lifecycle runner is unfinished. The new suite catalog records required hosted families as planned. Selecting those families returns an incomplete result.

## Existing evidence

| Existing component | Established scope | Hosted acceptance still required |
|---|---|---|
| [Delivery acceptance](SDK-DELIVERY-ACCEPTANCE-20260913.md) | Selected generation and the 50-project Release build | Deployed behavior of the selected APIs |
| [SDKComposition](../tests/SDKComposition/README.md) | Actual F# consumers compose Workers, Containers and AI provider types | Service responses and deployment lifecycle |
| [ByteBridge](../tests/ByteBridge/README.md) | Generated Workers byte views through Fable and BAREWire under Node | Hosted transport and native endpoint profiles |
| [HawaiiApi](../tests/HawaiiApi/README.md) | Actual generated clients against loopback HTTP fixtures | Cloudflare response behavior and resource lifecycle |
| [ManagementIntegration](../tests/ManagementIntegration/README.md) | Compute, Tunnel, WARP and Access request/response diagnostics, including the inline response regression | Resource provisioning, service behavior, policy enforcement and verified removal |
| [DO acceptance](DURABLE-OBJECTS-ACCEPTANCE.md) | Direct F# subclass, storage, alarm retry and local workerd restart | Hosted migration, hibernation, eviction and distributed delivery |
| [Validation Agents runner](../../FSharp.CloudEdge.Validation/scripts/local-agents.mjs) | SDK calls through the retained JS subclass adapter, local state and restart | Hosted Agent lifecycle and external integrations |
| [Validation management CLI](../../FSharp.CloudEdge.Validation/management/Program.fs) | Partial Worker upload/enable/delete and Gateway create/get/delete commands through generated clients | Journal, staged actions, restartable cleanup and absence verification |
| Validation Sandbox and Gateway projects | Project reference scaffolding | Scenario implementations and hosted execution |

The [validation handoff](../../FSharp.CloudEdge.Validation/docs/HANDOFF.md) explicitly records live lifecycle journaling, recovery, cleanup and acceptance as unfinished. Its accepted closeout created no Cloudflare resources. Local process termination, closed ports and deleted SQLite directories establish local cleanup.

The management diagnostics exposed a Hawaii defect: an inline response assembled
through `allOf` could be emitted as `string` when its outer schema omitted `type`.
The generator fix preserves its inherited fields and named result types. A related
YAML regression now produces an explicit diagnostic when anonymous response union
branches would otherwise be discarded. These are management OpenAPI generation
changes. Xantham's TypeScript runtime bindings use a separate generation path.

The maintained Hawaii overlays also correct 47 Tunnel-related error contracts
that combined incompatible success and failure envelopes. The correction selects
the existing failure schema under exact source preconditions. The management
suite checks error status, details, nullable result and retry metadata across
seven affected API families. Its schema validator requires positive bodies to
have zero errors against the same effective schema used to generate the clients.
Deliberately malformed bodies must produce their exact declared rejection evidence.

Additional generator regressions retain canonical error-list types and restore
required-string rejection. The actual container consumer exercises both scheduling
variants, malformed discriminators and absent identity. These checks use the
schema, generated types and decoder behavior together to identify the layer that
needs correction.

Cloudflare also distinguishes deployed tests from local runtime tests in its [testing documentation](https://developers.cloudflare.com/workers/testing/). Local results count where their environment supplies the contract being asserted.

## Ownership

| Contract | Test owner | Example |
|---|---|---|
| Generated symbol, overload, public input and shared type identity | FSharp.CloudEdge | Workers Request remains the owner accepted by Containers |
| Generated management request and response | FSharp.CloudEdge | Container application response decodes from the actual API object |
| Hosted runtime binding behavior | FSharp.CloudEdge | Loader loads a compiled child, Facet abort invalidates its stub and retains its storage |
| Application orchestration | Conclave | Prospero restarts the affected siblings and preserves the job's accepted contributions |
| Resource API security settings | FSharp.CloudEdge | Access policy or Tunnel route creation, reading and removal |
| Application authority | Conclave | Tenant/scope admission, child grants and revocation during an operation |
| Binary view interoperation | FSharp.CloudEdge | Generated body bytes preserve a nonzero-offset payload |
| Application protocol | Conclave | BAREWire job identity and cancellation survive Worker, native and client boundaries |
| Native placement | Conclave | An admitted Clef image executes the required service contract |
| Network overlay behavior | Conclave | An enrolled runner reaches the admitted service through One/WARP and an unauthorized runner is refused |

The SDK's Facet test uses a minimal supervisor to exercise the binding. Prospero's strategy, restart budget and durable join are Conclave policy. Fable compiles and assembles that topology through CloudEdge's DO bindings. Fable's MailboxProcessor implementation is excluded from the actor path.

The existing Validation repository supplies reusable fixture sources and earlier evidence. Common lifecycle infrastructure belongs with CloudEdge's suite. Conclave imports that infrastructure while retaining its scenarios and assertions in its own repository.

## Remote lifecycle

Wrangler is not a dependency of the proposed live path. F# consumers compile through .NET and Fable. Bundling uses the existing JavaScript build tools. A host-side controller uses generated management clients to provision resources and upload the actual emitted module with its bindings and migrations.

```mermaid
flowchart LR
    Build[Compile candidate consumer] --> Plan[Persist run and resource intent]
    Plan --> Deploy[Provision and upload]
    Deploy --> Exercise[Run staged assertions]
    Exercise --> Collect[Collect bounded evidence]
    Collect --> Clear[Clear resources in dependency order]
    Clear --> Absent[Verify absence]
    Absent --> Result[Return test and cleanup verdicts]
    Deploy -. failure .-> Clear
    Exercise -. failure .-> Clear
    Collect -. failure .-> Clear
    Journal[Durable journal] --> Resume[Resume cleanup or expiry janitor]
    Resume --> Clear
```

The controller records intended resources before each creation request. A creation can succeed while its response is lost or undecodable. Recovery reconciles the exact planned identity before clearing the resource. A broad name prefix is insufficient authority to delete another run's resources.

The journal records a run ID, an account fingerprint and an expiry. Resource records contain exact fixture names and returned IDs. The ordered plan creates prerequisites before dependent resources and cleanup reverses that order. The journal also records candidate artifact hashes and lifecycle progress. Credentials remain outside the journal and evidence bundle.

Use a separately pinned controller build for resource administration and cleanup. The candidate library runs in the test consumer. This permits the suite to report a candidate response-decoding failure while a working controller clears the resource. Record both revisions in the result.

Clearing includes dependent state: pending work, subscriptions and alarms where applicable, then service resources, then the Worker and its owned namespaces or deployment state. Product adapters must implement the correct lifecycle for their resource. DO namespace removal needs the supported class lifecycle mechanism, including removal of the shipped class. A successful Worker delete alone is insufficient evidence of complete cleanup. See [DO class lifecycle](https://developers.cloudflare.com/durable-objects/reference/durable-objects-migrations/).

Every resource adapter supplies bounded polling for readiness and absence. Transient errors use a finite retry policy. Cleanup attempts continue for independent resources after one removal fails. The final result preserves assertion failure and cleanup failure separately.

A killed CI runner cannot execute a finally block. Resume-cleanup and an expiry janitor therefore consume the same ownership journal. Journals must be uploaded to durable CI storage before mutation, or persisted in an equivalent control store. A local journal alone covers process restart on that machine.

The current [lifecycle module](../scripts/integration/lifecycle.mjs) supplies adapter-neutral journal mechanics and fault-path tests. Product-specific Cloudflare adapters, durable CI journal publication and the expiry janitor remain implementation work. Unit tests of those mechanics establish no remote deployment behavior.

## Coverage structure

The [suite catalog](../tests/integration/suite.json) records 14 implemented local diagnostic commands and 32 required hosted families. The hosted entries are specifications awaiting executable adapters. They are not 32 completed test implementations.

Twelve runtime families cover the selected SDK libraries: Workers I/O, DO/Loader/Facets, Workflows, storage bindings, Agents/chat/MCP, execution tools, Voice, Containers/Sandbox, AI providers, HTTP/RPC, supporting utilities and Pages plugins. Eleven management families cover the ten purpose clients plus Tenancy. Nine Conclave families cover orchestration, authority, workflow/MCP, native services, IoT, WREN, protocols, One/WARP and performance.

A family contains staged cases. For Containers, those stages include creating an application from a pinned image, awaiting readiness, performing a service request, changing a deployment version and observing recovery, then removing the application and checking absence. The candidate binding must decode the real responses at every step.

Represent each standing feature with its public entry, relevant symbols or operation IDs, consuming case IDs and required execution profile. The authenticated API baseline tracks all selected declarations and operations. Family assignment provides initial ownership. Member-to-case mappings and per-operation behavioral coverage still need to be completed.

The 3,437 selected management operations do not imply 3,437 independent fixture deployments. Related operations share a resource lifecycle. Every operation needs an explicit coverage disposition. Structural coverage, authenticated read-only checks and hosted mutations have different evidence. Account-wide administrative operations require a suitable test-account profile. Missing product entitlements remain visible coverage gaps.

Conclave's release profile selects the feature contracts required by its admitted deployment manifests. Any additional customer deployment capability adds its contracts to that profile. A passing reduced profile must retain its exact selection in the result.

## SDK advances

[Contract tracking](../tests/integration/baselines/README.md) authenticates the standing selected delivery and compares it with a newly generated candidate. The baseline covers 31 server libraries, 112 public inputs and 7,794 owned declaration entries. It also fingerprints 3,437 management operations and their referenced schemas.

The check records TypeScript inputs as well as generated F# APIs. An upstream change can require review even when a widened F# binding has the same signature. The baseline is never refreshed implicitly.

An upgrade requires the following evidence:

1. The candidate's SDK pins, declaration inputs and generator receipts authenticate.
2. The difference report accounts for added, changed and removed contracts.
3. Affected F# consumers compile against the candidate.
4. Required hosted fixtures pass their staged assertions.
5. Conclave's affected consumer scenarios pass.
6. Every run's cleanup and absence checks pass.
7. The reviewed baseline and coverage mappings advance together.

Changes to shared owners or common HTTP serialization affect more consumers than a single public entry. The suite must expand the affected selection through those dependencies.

## Conclave scenarios

The [Prospero acceptance design](../../Conclave/docs/roadmap/INTEGRATION-ACCEPTANCE-20260913.md) specifies the fan-out and durable join test derived from the Fearless Parallelism articles. Its first executable fixture uses bounded integer contributions to give the orchestration a known result contract. Compiler-generated arithmetic proofs remain owned by the separate Clef roadmap.

Native tests use the actual compiled Clef artifact and retain its compiler, target profile and image hashes. Current local subprocess evidence and a hosted image lifecycle have separate results. Native protocol coverage must preserve BAREWire representations at both endpoints.

WREN tests distinguish hosted Solid clients from the native host and its authenticated loopback bridge. A browser HTTP driver establishes neither a WREN native launch nor its client security policy.

IoT tests identify the physical device or gateway profile. Hardware fixture prerequisites remain explicit while per-run sessions, credentials and cloud resources are ephemeral. Existing hardware is a test rig, not a resource created by the test.

One/WARP tests require an enrolled runner and a negative-path runner. The fixture owns its routes and policies, revokes its temporary credentials and clears its cloud resources. Run the same application protocol checks over the admitted paths. The overlay's presence establishes neither application authority nor correct native decoding by itself.

Performance cases retain worker count, payload sizes and useful work. They measure latency distributions and throughput alongside coordination traffic and resource use. Barrier-based overlap tests establish outstanding work, while CPU speedup requires separate measured evidence.

## Commands and results

```sh
# Inspect required hosted families without mutation.
npm run test:integration -- --profile cloudflare --list
npm run test:integration -- --profile conclave-cloudflare --list

# Fast checks independent of Wrangler.
npm run test:integration-tooling
npm run test:contracts

# Explicit local diagnostics, including existing optional workerd runners.
npm run test:integration -- --profile local

# Select the direct Miniflare Prospero fixture.
npm run test:integration -- --case conclave.prospero-local
```

The runner defaults to the Cloudflare profile. Hosted families remain incomplete until their adapters exist. A planned, blocked or failed selected case causes a nonzero result. Existing Wrangler-based local probes remain individually selectable diagnostics.

Each run writes JSON, JUnit XML and command logs under a new `artifacts/integration/` directory. Reports retain the selected profile, manifest and runner hashes, timings and prerequisite outcomes. The live adapter must add deployed artifact identity, staged observations, resource receipts and verified absence. CI consumers should expose the assertion verdict and cleanup verdict separately.

No Cloudflare resources were created during this audit and initial implementation.
