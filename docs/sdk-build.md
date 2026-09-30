# Build selected SDK partitions

After restoring the pinned profiles and local tool packages:

```sh
npm run build -- --resume
```

This builds the selected SDK/support libraries and the Hawaii management hierarchy together.
The [library guide](SDK-LIBRARY.md) describes the layout, dependency injection and fidelity
contract. Run `npm run build` for fresh generation through both tools.

For SDK-only work:

```sh
npm run build:sdk -- --plan
npm run build:sdk -- --resume
npm run build:sdk -- workers containers
npm run build:sdk -- --all --plan
```

The default is the server selection in
[`config/sdk-delivery.json`](../config/sdk-delivery.json), plus its explicit support owners.
It includes Agents, Sandbox, Containers, AI Gateway, Workers AI, AI Search and the selected
server helpers. React/web-client behavior and optional integration work remain outside the
default build. Management stays OpenAPI → Hawaii.

Each library selects exact public subpaths from the pinned compiler partition. Inputs left
out require a reason; newly inventoried inputs require an explicit disposition. Included
declaration files retain their full exported APIs. Support catalogs are supplied through
`catalogImports`; compatible inference profiles and source/API identities are validated.

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
