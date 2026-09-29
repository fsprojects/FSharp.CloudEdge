# Durable Object SDK acceptance

Durable Objects are a required first-party resource surface. The delivery should support a
straightforward F# subclass, typed state and alarm calls, and request forwarding from a Worker.
Conclave's actor designs motivate this requirement; implementing its Olivier/Prospero overlay,
mailbox policy or supervisor is outside this SDK delivery.

## Binding checks

| Contract | Evidence required |
|---|---|
| Native base class | A direct F# subclass uses the generated ambient import and constructor. Generated fetch/alarm interfaces preserve the host's calling convention. |
| Namespace and fetch | Named identity, ID lookup and real stub request forwarding use generated Workers types. |
| Durable state | Typed reads/writes, transactions, initialization gating and alarms use generated members. |
| Facet startup | `DurableObjectClass<T>` retains its parameter through `FacetStartupOptions<T>`; an incompatible marker argument fails compilation. |
| Shared ownership | The final generated libraries agree on source identity, API, arity and generic constraints, and compile with the required SDKComposition consumer. |

`tests/SDKComposition/DurableObjects.fs` is the consumer. The hand-written public module
`FSharp.CloudEdge.Support.Workers.DurableObjects` supplies `FetchTransport`, its checked
projection, and get-by-ID/get-by-name helpers. This is a small, visible boundary over the current generic RPC binding limitation: generated namespace
`get`/`getByName` return `obj` (`SA002`/`TR018`). The projection checks that the returned object
exposes fetch, then provides typed request/response forwarding. It does not claim typing for
arbitrary RPC methods. Storage, transaction and alarm APIs retain their generated typing.

The same consumer and public helper sources are compiled and run by the sibling validation repository. The JavaScript entry
exports the compiled F# Durable Object class directly; it does not supply a subclass adapter.

## Bounded local runtime checks

The workerd probe uses an isolated SQLite state directory and ephemeral local ports. It checks:

- Initialization completes before fetch is served.
- A real namespace stub forwards a typed request; an invalid fetch projection is rejected.
- Typed transactional updates suppress duplicate message IDs and preserve concurrent increments.
- Named objects have independent state.
- A scheduled alarm commits progress, deliberately throws, then completes on host retry without
  duplicating the durable effect.
- Acknowledged state and duplicate receipts survive a runtime process restart.
- Generated alarm/state deletion succeeds; processes stop, ports close and isolated state is removed.

These are SDK usability and local runtime checks. They establish neither application mailbox
ordering nor Facet supervision. Production eviction, hibernation, distributed delivery and
migration behavior require separate deployment evidence. No Cloudflare resources are created
by the local smoke.

## Commands and evidence

The validation repository's `scripts/build-durable-objects.mjs` builds the linked F# source,
Fable output and Worker bundle. It records source/project hashes and validates the supplied
Workers generation receipt. `--run` starts `scripts/local-durable-objects.mjs`, which records
its assertions and cleanup result. Each label creates a new evidence directory.

```
node scripts/build-durable-objects.mjs --label final-durable-objects \
  --receipt /absolute/path/to/final/workers/generation.json --run
```

Use `--workers-project /absolute/path/to/workers.fsproj` for a retained producer project.
`--candidate` explicitly marks an intermediate run without final receipt authentication;
that result does not substitute for the final packed-tool library build.

The final packed tool `0.1.0-local.8e4c7b11b0ac90236c25` generated the Workers input. Its
receipt was authenticated before the standalone consumer and public helper built in Release
with zero warnings/errors and compiled through Fable. The bounded local smoke passed all seven
assertion groups with Wrangler `4.130.0` / workerd `1.20260908.1`, including both public
fetch lookup helpers, and verified process, both-port and state cleanup.

Report: `../FSharp.CloudEdge.Validation/artifacts/local/durable-objects-C1VeIu/report.json`.
Bundle and build provenance:
`../FSharp.CloudEdge.Validation/artifacts/durable-object-builds/final-durable-objects/`.
This result has `candidate: false` and records the final generation receipt and source hashes.
The earlier incompatible Facet marker probe failed with `FS0001`, as required.

The complete selected SDK/library build also passed: all 34 generated SDK/support libraries
compiled, followed by the full 50-project Release build with zero warnings/errors. Its required
SDKComposition consumer and hand-written Workers helper are included. Build receipt:
`artifacts/library-build/latest.json`.

The final Agents smoke passed separately at
`../FSharp.CloudEdge.Validation/artifacts/local/agents-g9QYKR/report.json` and retains its
JavaScript subclass adapter qualification. Direct F# Durable Object subclassing is the result
established by the Durable Object report.
