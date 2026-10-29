# Fickl inline unions and Durable Object RPC

Fickl 0.7.2 always created a proxy for an inline-interface union case. A proxy cannot cross a structured-clone boundary, and an F#-constructed value therefore could fail when passed to a Durable Object RPC method. In Fickl 0.7.3, [PR #138](https://github.com/shayanhabibi/fickl/pull/138) passes through a payload whose discriminator already equals the case tag. It also adds opt-in `Copy` and `CopyPlainObjects` representations for payloads that do not already carry the tag.

The tracked [RPC probe](../tests/FicklRpcProbe/check.mjs) compiles a small F# union with the selected Fickl compiler, then calls a SQLite-backed `CloneProbe` Durable Object in celld. With the 0.7.3 packages from the successful Fickl release job, the F#-constructed, already-tagged value crossed RPC successfully. A plain control and an untagged value passed through the runtime copy helper also crossed; missing or conflicting tags used the default proxy and failed with `DataCloneError`, as expected. The probe verifies the discriminator, field contents, and reference identity for the already-tagged case.

To run the probe, install the locked `profiles/agents-pi` npm dependencies, Fickl 0.7.3 and celld:

```bash
npm ci --prefix profiles/agents-pi
CLOUDEDGE_CELLD=/path/to/celld CLOUDEDGE_FICKL=/path/to/fickl node tests/FicklRpcProbe/check.mjs
```

The probe uses `tests/.scratch/` only for disposable runtime state and removes that state after the run. The Fickl runtime owns the representation; the probe does not duplicate its implementation.

The pass-through avoids introducing a proxy but does not guarantee that an arbitrary caller-supplied object or its nested fields can be serialized. For an untagged payload, the default `View` keeps the proxy's live forwarding and F# payload identity but cannot cross structured clone. The opt-in copy representations produce snapshots of own enumerable properties; JavaScript reads and writes to the copy can diverge from the original. Copies can still contain non-serializable nested values or unsupported prototypes. JSON conversion is not a general solution: it changes values such as `undefined`, binary data, and dates.

Ordinary Fable 5.13 also emitted the wrong inline-union representation for CloudEdge's AI Summarizer example. CloudEdge's JavaScript examples use Fickl so its tagged-union annotations are handled correctly.

The checked-in Think binding contains a documented one-line compatibility correction pending full regeneration after its separate Agent catalog mismatch is resolved; see [nuget-release.md](nuget-release.md).
