# Cloudflare management and tenancy clients

This consumer generates management and tenancy clients with the separately
packaged Onur-based Hawaii softfork. TypeScript runtime bindings remain in the
Xantham pipeline. The OpenAPI input, source overlays, lifecycle decisions, and
logical client taxonomy are maintained here.

Until the enhanced tool is published, bootstrap its local package from a Hawaii
source checkout. This installs a pinned version in the consumer's local tool
manifest; routine generation does not build or load a sibling source checkout.

```sh
node scripts/bootstrap-tools.mjs --only hawaii --hawaii-source ../Hawaii
python3 generators/hawaii/generate.py
python3 -m unittest discover -s generators/hawaii/tests -p 'test_*.py'
```

Run these commands from the FSharp.CloudEdge root. The [tool bootstrap walkthrough](../../README.md)
covers the local package feed and manifest. Generator-development regressions
remain in the Hawaii repository, separate from this consumer workflow.

`pins.json` fixes the original Cloudflare schema commit and SHA-256 and selects
the local `hawaii` tool. The runner resolves its package through
`scripts/tool-packages.mjs`, verifies the tool fingerprint before and after
execution, applies the explicit [source overlays](overlays/README.md), and
generates into a new artifact directory. Provenance records the package ID,
version, package and payload hashes, and tool manifest hash. Compilation must
succeed before that run is marked compiled. The runner does not rewrite
generated F# or publish a partly working client into `src`.

Use `--schema /path/to/openapi.json` to reuse a downloaded pristine input;
the same hash check applies. Each run retains generation/build logs, timing and
memory observations, full operation coverage, and provenance under
`artifacts/hawaii/runs`.

The original input has 3,448 operations. Coverage assigns each operation its
source identity, logical service family and purpose, and lifecycle disposition.
`taxonomy.json` organizes ten Management clients by purpose and one Tenancy
client. Every schema and response type belongs to `FSharp.CloudEdge.Core.Api`;
the client projects reference that shared assembly and never copy its types.
Logical families do not imply one project per SDK tag.

Generated F# stays byte-identical to Hawaii's output. The consumer projects
preserve compilation order and relocate only project references into:

- `src/Core/FSharp.CloudEdge.Core.Api`
- `src/Management/FSharp.CloudEdge.Management.<Purpose>`
- `src/Tenancy/FSharp.CloudEdge.Tenancy`

`Hawaii.Bindings.slnx` contains this complete set. The default runner first builds
that layout in its artifact directory and exercises Compute and Tenancy clients
against a local HTTP server. Only then does it install the owned files into
`src`. The ownership receipt at `inventory/hawaii-output-ownership.json` records
file hashes, the validated package identity, source hash, and run directory.
Subsequent runs refuse to overwrite unowned files or edited generated output;
the previous owned files remain backed up in the new run directory. Unclassified
selected operations fail grouping instead of disappearing from a partial SDK.

Use `--single-client` for a one-project compilation probe that stays entirely in
its artifact directory. It does not replace the organized consumer projects.
The pinned schema currently maps to 164 family owners with no unclassified
operations; [the coverage inventory](inventory/taxonomy-coverage.json) records
the schema and taxonomy hashes. Future unclassified operations remain in the
coverage report with no fabricated owner.

Ownership does not establish public SDK eligibility. The source also contains
internal test and health routes, and SDK-ignore flags can have different
purposes. Those signals require explicit selection decisions alongside the
lifecycle review; assigning a route to a family does not make it a supported
customer API.

The source marks 285 operations deprecated. Deprecation alone does not establish
retirement. The pinned [operation policy](lifecycle/operation-policy.json) reviews
eligibility as of September 6, 2026: it excludes eight retired operations and
three explicitly internal test or infrastructure routes, leaving 3,437 operations.
Every exclusion matches a source operation ID and supplies a reason and primary
sources. Retirement dates must fall on or before the policy's review date.
`--lifecycle-policy` overrides this policy with another explicit, source-matched
review; generation never silently advances the review date with the machine clock.

The [review notes](lifecycle/review-notes.json) explain retained ambiguous cases,
future retirements, and changes that affect only a field or an SDK method form.
Publisher `hidden`, SDK-ignore, and `deprecated` flags never remove an operation
on their own. Internal exposure exclusions have a separate disposition from
retirement, generation failure, and unknown classification. Shared component
definitions remain available to included operations. The full
[selection coverage](inventory/operation-selection-coverage.json) records each
decision; the [deprecation inventory](lifecycle/deprecated-operations.json)
remains review evidence.

The exact [JSON preservation policy](preserve-json-schemas.json) lists named
union and composed-union exceptions against the pinned source. The generator otherwise rejects
unsupported named unions. Selected exceptions retain their payload in a named
JSON wrapper and appear in `schema-fidelity.json`; this does not assert union
membership or complete schema validation. The policy also records source
contradictions in four resource-tagging request alternatives, where inherited
and specialized enum constraints have an empty intersection.

The selected 3,437-operation surface now passes generation through the packaged
tool, a Release build of all twelve projects, and the grouped-client loopback
sample. The pipeline records those results in its run provenance and output
ownership receipt. The 217 explicit JSON-preservation exceptions remain part of
the generated contract; successful compilation does not establish complete
schema validation or deployed Cloudflare behavior. These are .NET management
clients, separate from the future native Clef/JSIR path.
