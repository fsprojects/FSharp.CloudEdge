# SDK delivery acceptance — September 13, 2026

**The agreed selected delivery is complete. There are no remaining delivery blockers.**

The accepted scope is 31 server SDK libraries covering 112 public inputs, three generated
AI support owners, one small handwritten Workers support library, and 12 Hawaii projects
covering 3,437 selected OpenAPI operations. This produces 47 library projects and a
50-project solution including the required consumers. Existing package pins were retained.
The [scope table](SDK-DELIVERY-SCOPE.md) lists the selected SDKs.

## Definition of done and evidence

| Required outcome | Result |
|---|---|
| Locally built generators | Xantham `c7e2fa0`, packed as `0.1.0-local.8e4c7b11b0ac90236c25`; pinned local Hawaii tool retained. |
| Generator regression gate | 910 Generator and 90 Wire tests in both phases; compile gate and 465 Fable checks passed. |
| Complete selected SDK generation | All 34 generated SDK/support libraries authenticated and compiled. |
| Cohesive complete hierarchy | All 50 projects built in Release with zero reported warnings/errors. |
| Typed library composition | Workers → Containers; Workers AI/Gateway v4 contracts; separate AI Search v3; direct F# DurableObject consumer passed. |
| Tooling and byte transport | `npm test` passed: 178 tooling tests, 28 Hawaii tooling tests and 13 ByteBridge runtime checks. |
| Durable Object usability | Final receipt-authenticated workerd run passed seven groups covering subclassing, both fetch lookup helpers, state/transactions, alarm retry, restart recovery and verified cleanup. |
| Agents workflow | Normal configure/build/test commands passed against the final library, including state, restart, destroy and cleanup. |
| Retired output cleanup | 183 verified generated directories archived; all retained project references resolve. Three historical probe solutions were also archived. |
| Review and transferable findings | [Xantham PR85](https://github.com/shayanhabibi/Xantham/pull/85) updated; Composer JSIR notes updated locally. |

The final combined build reused authenticated Hawaii output. Its earlier accepted staged
Hawaii build had 127 warnings and zero errors, and passed the local HTTP consumer. The
combined build's zero-warning result does not replace that earlier warning record.
ByteBridge used clean BAREWire commit `5af58d8f32b93d0fbc631c4bd884d806aaae3e43`.

The immutable [acceptance snapshot](../artifacts/one-shot-20260913/final-acceptance/acceptance.json)
records tool, source, receipt and log hashes. The [build guide](SDK-LIBRARY.md) explains the
repeatable commands; clean-checkout setup uses `install:profiles -- --delivery` to select
24 dependency profiles. The final runtime reports are
[Durable Objects](../../FSharp.CloudEdge.Validation/artifacts/local/durable-objects-C1VeIu/report.json)
and [Agents](../../FSharp.CloudEdge.Validation/artifacts/local/agents-g9QYKR/report.json).

## Recorded non-blocking limitations

- Generic Durable Object RPC stub computation still widens to `obj`. The public
  `FSharp.CloudEdge.Support.Workers.DurableObjects` helpers provide checked, typed fetch
  access by ID or name. Arbitrary RPC method typing remains outside that helper's contract.
- Ordinary Container bindings retain imported constructors and inherited members, with
  SI006 recording the omitted nominal base relation. Direct F# subclassing of ordinary
  Container is unsupported; the ambient DurableObject base is directly subclassable.
- The Agents runtime sample retains its existing JavaScript subclass adapter.
- Other reported TypeScript mapping losses remain visible. The [fidelity audit](../artifacts/one-shot-20260913/final-fidelity-audit.md)
  records 1,444 exact, 3,366 ergonomic, 3,713 widened and 790 escape symbol occurrences.
  These counts include repeated dependencies and are not unique API or defect counts.
- Explicit callback injection retains its documented curried/generic alias limitations.
  Ordinary callback generation and the tested delegate/inline adapter pass their controls.
- Browser/UI and optional integrations remain deferred. Local runtime checks establish SDK
  usability; production deployment, eviction, hibernation and distributed behavior have
  separate acceptance. Conclave actor and supervisor implementation is outside this delivery.

The remaining items are documented boundaries, not additional work in this closeout.
