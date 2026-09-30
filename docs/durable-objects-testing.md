# Durable Object testing

The Durable Object consumer checks direct F# subclassing, typed state and alarm calls, and
request forwarding from a Worker. Application mailbox and supervision policies belong in
consumer projects such as Conclave.

## Binding checks

| Contract | Check |
|---|---|
| Native base class | A direct F# subclass uses the generated ambient import and constructor. Generated fetch/alarm interfaces preserve the host's calling convention. |
| Namespace and fetch | Named identity, ID lookup and real stub request forwarding use generated Workers types. |
| Durable state | Typed reads/writes, transactions, initialization gating and alarms use generated members. |
| Facet startup | `DurableObjectClass<T>` retains its parameter through `FacetStartupOptions<T>`; an incompatible marker argument fails compilation. |
| Shared ownership | Generated libraries agree on source identity, API, arity and generic constraints, and compile with the SDKComposition consumer. |

`tests/SDKComposition/DurableObjects.fs` is the consumer. The hand-written public module
`FSharp.CloudEdge.Support.Workers.DurableObjects` supplies `FetchTransport`, its checked
projection, and get-by-ID/get-by-name helpers. This is a small, visible boundary over the current generic RPC binding limitation: generated namespace
`get`/`getByName` return `obj` (`SA002`/`TR018`). The projection checks that the returned object
exposes fetch, then provides typed request/response forwarding. It does not claim typing for
arbitrary RPC methods. Storage, transaction and alarm APIs retain their generated typing.

The same consumer and public helper sources are linked into the sibling validation repository. The JavaScript entry
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

Production eviction, hibernation, distributed delivery and migration behavior need hosted
tests. Application mailbox ordering and Facet supervision have their own consumer scenarios.
The local smoke uses isolated workerd state and creates no Cloudflare resources.

## Running the checks

After generation, compile the consumer from the CloudEdge repository:

```sh
dotnet build tests/SDKComposition/SDKComposition.fsproj --configuration Release
```

The validation repository's `scripts/build-durable-objects.mjs` builds the linked F# source,
Fable output and Worker bundle. It records source/project hashes and validates the supplied
Workers generation receipt. `--run` starts `scripts/local-durable-objects.mjs`, which records
its assertions and cleanup result. Run this command from the validation repository, using
the receipt for the Workers source being tested and a new label for each build:

```sh
node scripts/build-durable-objects.mjs --label durable-objects-check \
  --receipt /absolute/path/to/workers/generation.json --run
```

Use `--workers-project /absolute/path/to/workers.fsproj` for a retained producer project.
`--candidate` permits an intermediate run without generation-receipt authentication; the
report records that distinction. Builds are written under `artifacts/durable-object-builds/`
and local runtime reports under `artifacts/local/` in the validation repository.

Inspect the current report for assertions, tool versions, source hashes and cleanup results.
The separate Agents runner retains a JavaScript subclass adapter; the Durable Object runner
exports the compiled F# class directly. The [integration guide](integration-testing.md)
describes the other local cases and planned hosted tests.
