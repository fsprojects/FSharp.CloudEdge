# Selected delivery contracts

`server-resources-20260913.json` captures the standing selected delivery from
`docs/SDK-DELIVERY-ACCEPTANCE-20260913.md`. It tracks 31 server SDK libraries,
112 public inputs, three generated support libraries, the handwritten Workers
support sources, 7,794 owned catalog declarations and 3,437 management operations.
It also records the explicit dispositions of deferred public inputs.

The reviewed Hawaii delivery uses `1.0.0-local.8b77c5534f22eccbbb0f`.
The final [comparison](../../../artifacts/integration/hawaii-complete-contracts-diff.json)
and [review](../../../artifacts/integration/hawaii-complete-contracts-review.json)
account for the tool identity, hashes of `Core.Api/Types.fs` and `OpenApiHttp.fs`,
and 47 operation fingerprints matching the guarded Tunnel failure overlay.
The pristine upstream schema and Xantham SDK delivery are unchanged. The original
snapshot remains under `artifacts/integration/hawaii-inline-response-before/`.

The initial source review identified 1,393 response cases across 1,264 methods
corrected from strings to typed envelopes. The final generator corrections retain
145 canonical error-list fields and restore required-string rejection. The
management diagnostics now pass all 41 cases, including the corrected Tunnel
failures and deliberately malformed container responses. See the
[management diagnostics and evidence](../../ManagementIntegration/README.md).

This is declaration inventory. It does not establish that every declaration has
a runtime test or that every management operation has been called. Behavioral
integration results remain separate, including Conclave actor orchestration,
native transports and live Cloudflare environments.

Run from the repository root:

```sh
node --test tests/integration/contracts.test.mjs
node scripts/integration/contracts.mjs check --output artifacts/integration/contracts.json
```

The check recomputes delivery selection from the current configuration and refuses
stale materialized targets or public input dispositions. It reuses `checkProjects`
to authenticate generated sources, project references, catalogs, profile lockfiles,
overlay fingerprints and pinned generator identity. It checks the current bytes of
173 declaration files belonging to selected packages against the generation catalogs,
including internal files reached from public entry points. All catalog input hashes
are retained for comparison. Other transitive package files are represented by their
generation evidence and profile locks, rather than independently rehashed here.

Hawaii authentication reuses `currentHawaii`: schema and policy inputs, tool identity,
generation pipeline sources, output ownership and all 26 owned files must match the
accepted generation. The selected schema, operation coverage registry and generated
client ownership must agree. Each operation fingerprint includes path parameters,
effective security and servers, and the full closure of local schema references.
A response schema change therefore flags its consumers even when the operation ID
has not changed. Cyclic local references are supported. Unresolved or remote
references fail the check.

An SDK or generator upgrade follows this sequence:

1. Update pins and regenerate using the existing SDK and Hawaii delivery commands.
2. Capture a candidate at a new path:
   `node scripts/integration/contracts.mjs snapshot --output artifacts/integration/candidate.json`.
3. Compare it with the standing baseline:
   `node scripts/integration/contracts.mjs diff tests/integration/baselines/server-resources-20260913.json artifacts/integration/candidate.json --output artifacts/integration/upgrade-diff.json`.
4. Review additions, removals, changed API fingerprints, TypeScript inputs, ownership,
   support sources and selection changes. Give new or changed behavior an integration
   test disposition. Run the affected consumer and runtime suites.
5. Promote the reviewed candidate through the repository's normal change review.
   Keep the comparison report and integration receipts with the upgrade evidence.

The CLI never implicitly refreshes the baseline. `snapshot` requires a new output
path and refuses an existing file. `check` and `diff` cannot overwrite their input
snapshots with a report. Exit code `0` means unchanged, `2` means review is required,
and `1` means missing, inconsistent or invalid evidence. Additions require review too.
Source offsets and generation timestamps are excluded from declaration identities.
No tool runs a generator, builds a library, installs packages or contacts Cloudflare.

The JSON baseline is deliberately explicit. Declaration keys are
`role:fully-qualified-FSharp-name:generic-arity`. API and constraint changes are
reported separately from TypeScript input hashes, so an upstream change hidden by
an existing widened binding still causes review. Management keys are `METHOD /path`.
The report is a conservative change inventory, not a semantic compatibility verdict.
