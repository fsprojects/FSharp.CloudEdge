# FSharp.CloudEdge

The [integration acceptance architecture](docs/INTEGRATION-ACCEPTANCE.md) records the existing tests, the ephemeral Cloudflare lifecycle, and the boundary between SDK and Conclave acceptance. Hosted test families remain explicitly planned until their deployment adapters are implemented.

Cloudflare bindings for F# applications, generated from pinned SDK declarations and OpenAPI schemas. This repository owns package selection, compatible dependency profiles, service taxonomy, lifecycle policy and consumer validation. Xantham and Hawaii are separate generators, invoked as pinned local .NET tools.

Workers and Cloudflare's AI and agent SDKs provide the immediate Fable bridge, with BAREWire as a binary backplane. `Fidelity.CloudEdge` is reserved for the future Clef/Composer JavaScript lowering path.

The delivery scope follows the server-side Cloudflare resource APIs this project intends to
use, including AI Gateway for Workers AI and external model access. React, web-client
behavior and optional integration work are outside that commitment. The broader pinned
inventory is available for community contributions driven by concrete demand.

The [accepted delivery](docs/SDK-DELIVERY-ACCEPTANCE-20260913.md) records the completed gates
and remaining non-blocking limitations. The [selected library guide](docs/SDK-LIBRARY.md)
describes the build, hierarchy, exact scope and dependency catalog injection. The [earlier checkpoint](docs/FIRST-DELIVERY-HANDOFF-20260913.md)
retains the preceding validation history.

## Organization

| Area | Responsibility |
| --- | --- |
| Core | Shared Hawaii API models with one declared owner. |
| Support | Generated AI contract owners and the small handwritten Workers fetch adapter. |
| Runtime | Selected server SDK bindings, grouped into platform, agents, AI, compute and resource services. |
| Management | Service APIs generated from Cloudflare's OpenAPI schema through Hawaii. |
| Tenancy | Account, membership and access-management APIs. |

[config/sdk-surfaces.json](config/sdk-surfaces.json) retains the broad inventory and
[config/targets.json](config/targets.json) its canonical targets and pinned profiles.
[config/sdk-delivery.json](config/sdk-delivery.json) selects the delivered public inputs,
records deferrals, and supplies support owners and catalog imports. Management uses the
separate [Hawaii pipeline](generators/hawaii/), with operation ownership and lifecycle
disposition recorded independently of upstream tags.

The selected delivery has passed generation, the complete 50-project Release build, typed cross-library consumers, and bounded local Durable Object and Agents lifecycle checks with verified cleanup. Hawaii contributes 3,437 selected operations across twelve projects, with shared-model and HTTP checks. Broader inventory and production deployment acceptance remain separate from this completed delivery.

## Generate and compile

Use Node.js with npm and .NET SDK 10.0.400 or a compatible .NET 10 SDK. Until the required generator versions are published, build their tool packages from separate checkouts. The [local tools walkthrough](docs/local-tools.md) covers setup, version changes and the later move to published packages. The default checkout layout is:

```text
repos/
  FSharp.CloudEdge/
  Xantham/
  Hawaii/
  BAREWire/
```

```sh
npm ci
npm run tools:bootstrap
npm run tools:restore
npm run install:profiles -- --delivery
npm run test:support-package
npm run build
npm run test:bridge
```

`install:profiles -- --delivery` installs the profiles selected by the delivery build,
including its generated support owners and profile overrides. Explicit profile IDs remain
available (`npm run install:profiles -- agents sandbox`); without arguments the installer
retains its broad-inventory behavior. `--delivery` cannot be combined with profile IDs.

`build` generates and compiles the complete selected hierarchy through both local tools.
The SDK selection contains **112 public inputs in 31 libraries**, including
`ai-gateway-provider`, plus explicit support libraries. Hawaii contributes 3,437 selected
OpenAPI operations across twelve projects. `FSharp.CloudEdge.slnx` includes the generated
hierarchy and consumer projects; `FSharp.CloudEdge.Bindings.slnx` includes the generated
projects and their handwritten Workers support project.

Use `npm run build -- --plan` to review the selection and `npm run build -- --resume` to
reuse authenticated output while compiling the hierarchy. `build:sdk` runs just the SDK
portion and accepts explicit selected library IDs. Its `--all --plan` lists the broad
inventory; `--all` attempts its generation and build, including deferred integrations.
The [SDK build guide](docs/sdk-build.md) explains saved configurations and evidence.

The root npm manifest pins the TypeScript compiler. Each folder under `profiles/` pins an SDK and its compatible declaration providers; lockfiles preserve nested dependency versions. These profiles are intentionally separate because current SDK releases can require different AI, React and other dependency versions.

Routine generation invokes the pinned packaged tools and does not build or read sibling
generator checkouts. Catalog input hashes, owner graphs, profile fingerprints and generated
source hashes are authenticated before project references are accepted. Set
`CLOUDEDGE_DOTNET` to override the dotnet executable. The older `generate`, `plan:sdk` and
direct `projects.mjs` commands operate on the broad canonical inventory; use `build` or
`build:sdk` consistently for the delivery configuration.

`npm run build:bindings` authenticates and builds the selected SDK hierarchy.
`npm run test:tooling` checks selection, ownership and orchestration. `npm test` runs those
checks, the complete hierarchy build with authenticated reuse, and the BAREWire runtime
consumer. Cloudflare inference and resource lifecycle validation remain separate gates.

The F# projects pin Fable.Core 5.2.0 and the generated compiler-library bindings used by Xantham's compile gate. `Xantham.Fable.Core` and `Xantham.Fable.Core.TS` are exact PackageReferences. Bootstrap packages their source with the Fable source assets required by consumers. [Upstream issues](docs/upstream-issues.md) records the helper-function correction and its retained runtime regression.

## Contributing and publication

The intended home after validation is the fsprojects organization, with community maintenance. The selected local delivery is accepted; repository transfer, licensing and release ownership remain publication decisions. Generator fixes belong in Xantham or Hawaii. Cloudflare selection, input policies, library organization and consumer tests belong here.

Changes to input pins or generator packages should include regenerated output and the corresponding validation results. Reports under `artifacts/` record what was checked; a successful build alone does not establish complete service coverage. The local setup and acceptance commands are intended to let another contributor reproduce those checks without relying on the original development machine.

## Inputs and evidence

Regeneration does not patch generated F# text. Selection rules and any documented source corrections belong to the input pipelines, with original and transformed hashes retained. Unexpected output, stale pins and failed generator processes remain failures.

[config/declaration-overlays.json](config/declaration-overlays.json) contains reviewed repairs for published TypeScript declaration files. For example, the pinned RealtimeKit callstats package contains an unresolved internal import alias; its other shipped declarations identify the intended relative import. The runner verifies the package and source hashes, applies that correction to a cached input copy, and records the rule in the generation receipt. `npm ci` remains reproducible, and changed upstream files require a fresh review of the rule. These repairs belong to the consumer; general type and rendering fixes belong in Xantham.

Reproducible reports live under `artifacts/`: per-symbol fidelity findings, effective configuration, generation receipts, compiler and generator hashes, source hashes, dependency audits and build results. Exact, ergonomic, widened and escape findings describe the precision of Xantham mappings. Successful compilation does not by itself establish complete SDK fidelity.

The [SDK inventory](inventory/cloudflare-sdk-inventory-20260906.json) records the primary-source discovery snapshot. Its historical probe notes describe that snapshot's checks; current input pins and acceptance receipts belong to the configuration and artifacts above.

The [ByteBridge consumer](tests/ByteBridge/README.md) exercises generated Workers body bindings and actual BAREWire codecs through Fable and Node Fetch. The September 13 acceptance passed all 13 checks with the final packed Xantham bindings, Fable 5.13.0, Node 25.1.0 and clean BAREWire commit `5af58d8f32b93d0fbc631c4bd884d806aaae3e43`. Those checks cover that boundary and recorded input state.
