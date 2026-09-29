The pinned Cloudflare schema uses `assets_jwt` once and `pages_upload_token`
three times without defining either security scheme. The JSON Patch beside
this file adds those two omitted declarations. It does not change operations,
payload schemas, or their existing security requirements.

Both are temporary upload JWTs carried as HTTP bearer tokens. They are distinct
from the account API token used to request an upload session:

- Workers: [Cloudflare's direct-upload guide](https://developers.cloudflare.com/workers/static-assets/direct-upload/)
  specifies the session JWT in the `Authorization` header for asset upload.
- Pages: [Cloudflare's upload implementation](https://github.com/cloudflare/workers-sdk/blob/42df9bbf07e37032a3e61027e33d504d74a25ccd/packages/wrangler/src/pages/upload.ts#L104)
  obtains the project upload JWT and supplies it to `check-missing`, `upload`,
  and `upsert-hashes` as a bearer token.

The generator keeps the downloaded schema unchanged and checks its SHA-256.
The overlay applies only when both names are absent. An upstream addition
requires review instead of silently replacing its definition. Generation
provenance records both the original and overlaid schema hashes.

The Spectrum analytics operations use `4xx` in three response maps. The second
patch moves those entries to `4XX`, retaining each complete response body and
its position in the status class. [OpenAPI 3.0.3's Responses Object](https://spec.openapis.org/oas/v3.0.3#responses-object)
defines status classes with uppercase `XX`. Applying this patch requires the
lowercase entry to exist and the uppercase destination to be absent.

The query-encoding patch covers nine parameter sites listed in
[the parameter inventory](../inventory/object-array-query-parameters.json).
Seven Cloudforce One descriptions in the pinned official schema explicitly
specify a JSON array. Those parameters retain their schemas under
`content: application/json`. The [tag API documentation](https://developers.cloudflare.com/api/resources/cloudforce_one/subresources/threat_events/subresources/tags/methods/list/)
also describes this form.

The two AI Gateway log filters use the explicit `dotted-repeat` compatibility
encoding. Its source is [Cloudflare SDK 7.1.0's query transport](https://github.com/cloudflare/cloudflare-typescript/blob/faaaf89ed8064a9fb54de538ec3e89487f1302b0/src/internal/utils/query.ts),
not an inferred OpenAPI serialization rule. Nested objects use dotted keys and
arrays repeat keys. This encoding loses array-element boundaries and maps null
to the same text as an empty string. Matching the SDK is not a promise of
complete JSON payload preservation. The underlying SDK source and outgoing
query fixtures are pinned in the consumer tests.

The [Tunnel failure correction](tunnel-failure-responses.json) repairs 47
operation-level `4XX` schemas listed in its
[inventory](tunnel-failure-responses.inventory.json). Each faulty schema combines
a success envelope with `tunnel_api-response-common-failure`. Those branches
require both `success: true` and `success: false`, and impose incompatible result
constraints. The correction selects the existing failure component, retaining its
errors, messages and nullable result. Request schemas and success responses remain
unchanged.

The patch tests the shared definitions and each operation identity. Every
replacement also requires an immediately preceding exact test of the response
schema. Changed source definitions or identities stop generation for review.
The pristine source remains retained, and provenance authenticates the ordered
overlay bytes and the effective schema used by Hawaii.

[Cloudflare's rate-limit contract](https://developers.cloudflare.com/fundamentals/api/reference/limits/)
uses HTTP `429` and `Retry-After`. The generated-client fixtures check those
values and preserve error details with `success: false` and `result: null`.
Their numeric error code is explicitly synthetic. The original source violations
are retained as evidence, while the corrected fixture bodies must pass the
effective schema with zero errors.
