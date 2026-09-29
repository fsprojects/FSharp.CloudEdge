# Worker body to BAREWire regression fixture

This fixture exercises freshly generated Worker `Body.bytes()` bindings through Fable and the
actual BAREWire Fable codecs. An Ask frame carries exactly one unsigned 32-bit value. Its Reply
preserves the correlation identifier and payload. Malformed frames, unreadable views and rejected
body reads produce explicit `Result` errors.

From the repository root, after generating the bindings:

```sh
dotnet tool restore
dotnet build FSharp.CloudEdge.slnx
dotnet fable tests/ByteBridge/ByteBridge.fsproj --outDir tests/ByteBridge/fable-out --noCache
node tests/ByteBridge/check.mjs
```

The default project references the sibling `BAREWire` checkout. Set the MSBuild `BareWireRoot`
property to use another checkout. The test requires Node with `Response.bytes()` and
`structuredClone` transfer support; Node 25.1.0 was used for the initial verification.

The byte-view tests include a nonzero backing-buffer offset, a Node `Buffer` slice, an empty
view and a detached buffer. A plain `Uint8Array` over the supplied window preserves its bounds
and uses typed-array copy semantics inside BAREWire. The fixture also checks exact payload
consumption and a second read of an already consumed body.

This is a Node Fetch integration test. Workerd compatibility, concurrent shared-buffer ownership,
schema negotiation, application range constraints and dimensional contracts require their own
checks. The u32 echo fixture establishes byte transport and framing behavior for the bridge.
