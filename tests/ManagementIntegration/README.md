# Management client diagnostics for Conclave

This consumer compiles against the real generated Compute, Networking and
Security clients, then exercises them over loopback HTTP. It diagnoses the
management operations Conclave needs for native services and private network
deployments. Real Cloudflare deployment and teardown remain separate acceptance
stages.

```sh
python3 -m pip install -r tests/ManagementIntegration/requirements.txt
node scripts/integration/management.mjs
```

The runner uses an authenticated original OpenAPI from the existing Hawaii run
artifacts. A clean environment can supply the pristine pinned file with
`--schema /path/to/openapi.json`. `--run-directory` accepts a new directory.
No Cloudflare credentials are read. The client only accepts an IPv4 loopback
endpoint, disables proxies and redirects, and uses a fixed fixture token.

The 41 cases cover these contracts:

| Surface | Diagnostic coverage |
| --- | --- |
| Compute | DO-managed container request discriminator and namespace ownership, scheduled and DO response branches, malformed discriminator and required-field controls, a separate scheduler application rollout with digest-pinned image and numeric step percentage, forbidden response, container deletion and subsequent absence, Worker deletion without force |
| Tunnel | Remotely managed creation, encoded list filters, ingress and WARP routing configuration, configuration version, private IPv6 route create/delete, tunnel delete, actual 403/429 status, error details, null result and retry headers across Tunnel, combined listing, virtual networks and routes |
| WARP connector | HA creation, local HA configuration with IPv4 and IPv6 VIPs, configuration version and tunnel identity, delete, rate-limit decoding |
| Access | Email inclusion and device posture requirement preservation, session duration, create/read/delete, actual 403/429 status retention, undeclared status refusal, malformed JSON refusal, in-flight cancellation |
| Private network security | Hostname-route rate limits and WARP-subnet access denial through generated Security clients |

The fixtures specify expected HTTP methods, exact paths and queries, JSON bodies,
media types and authorization. F# checks response cases and meaningful result
fields. The runner rejects duplicate requests, verifies that every fixture was
observed, bounds request size and command duration, and closes the server and all
connections. It retains build/runtime logs, requests, per-case observations,
fixture schema results, input hashes and loaded assembly hashes.

`validate_fixtures.py` checks each body against both the pristine pinned OpenAPI
and the effective schema consumed by the generated clients. It authenticates the
generation receipt, pipeline sources, ordered overlays and operation policy, then
reconstructs the selected schema and compares it with the recorded schema bytes.
`--pins` and `--provenance` can select those evidence files explicitly. Defaults
use this repository's pins and current Hawaii ownership receipt.

`expectedSchemaErrors` records exact pristine upstream conflicts only. Positive
bodies must have zero errors under the effective schema. Missing corrections,
new errors or changed upstream conflict evidence fail validation. Deliberately
invalid JSON bodies declare `negativeFixture: true` and `negativeSchemaErrors`.
both schemas must reject them with exactly that evidence. These observations
are marked `negative-response` and `expected-rejection`, and the F# client must
independently reject the response. They never count as conforming operations.
Transport negatives such as malformed JSON syntax, undeclared status and
cancellation are listed separately as excluded response bodies. The report
retains both sets of observations and their input hashes.

Validation translates OpenAPI 3.0 `nullable` to the JSON Schema null type and uses
Draft 4 without optional format checks. External reference retrieval is disabled.
The HTTP assertions cover method, path, query and media separately. Schema
consistency does not establish acceptance by the deployed Cloudflare service.

Run validator regressions with
`python3 -m unittest discover -s tests/ManagementIntegration -p 'test_fixture_validation.py'`.

The final Hawaii correction run passes **41 of 41 HTTP cases**, with every
request observed and no transport failures. Its [report](../../artifacts/integration/management-hawaii-complete/report.json)
identifies the installed generator output and loaded assemblies. The accompanying
[schema report](../../artifacts/integration/management-hawaii-complete/fixture-schema.json)
records 42 conforming request/response bodies and five deliberately invalid
responses with the expected rejection evidence. Three transport response cases
are excluded from body validation. These are local diagnostics.

## Regressions exposed by this suite

The initial run recorded **22 passing diagnostics and five failing contracts**
in `artifacts/integration/management-OJHoAJ/report.json`. Authentic object
responses exposed a Hawaii generation defect even where Conclave's transport
adapter compensated for it.

- `compute.create-native`, `compute.get-native`,
  `compute.rollout-scheduled` and `compute.delete-native`: generated success
  cases carried `string`, while the OpenAPI response declared an object envelope.
  The generated decoder threw `JsonException` on those response objects.
- `tunnel.rate-limit`: the generated `Status4XX` payload also carried `string`
  and rejected an object failure envelope.

Hawaii now emits records for inline response envelopes whose `allOf` branches
add object properties without repeating an outer `type`. Inherited envelope
fields and the named nested result types survive generation. The consumer checks
the generated DO application variant, namespace identity and rollout result
directly. Deletion checks its typed asynchronous acknowledgement. No response
body is double-encoded to satisfy a string decoder.

The [Tunnel correction](../../generators/hawaii/overlays/tunnel-failure-responses.json)
also repairs 47 related `4XX` contracts that combined incompatible success and
failure envelopes. It selects the existing canonical failure schema. Exact source
tests guard every replacement, and requests and success responses remain intact.
The fixtures retain the three original upstream violations while requiring their
failure bodies to pass the corrected schema.

Nine HTTP cases exercise that failure shape across seven API families. They
check actual 403/429 status, exact error code and message, null result, and
`Retry-After` preservation without altering response bodies. The numeric error
code is explicitly synthetic. A later response without a retry header must also
clear the previous response's metadata.

The generated error collection retains the canonical `tunnel_messages` list
type. A multiple-error response checks item order, codes and Unicode messages.
Hawaii ignores string-length assertions only when the referenced type provably
excludes strings, so a misplaced `minLength` cannot widen that list to arbitrary
JSON. Meaningful constraints retain their existing generation path.

The required-identity case also tests Hawaii's serializer configuration. Global
null omission previously caused the JSON dependency to accept a missing required
string as null. The corrected configuration rejects the absent application ID
while retaining optional-field omission and explicit-null representation.

Positive bodies pass the effective corrected schema. The nine failure responses
retain their recorded conflicts against the pristine pinned schema. No quoted
object fixtures hide string widening, and an expected exception never counts as a
successful management operation. Changes to Hawaii schema pins require fixture
review. Regenerated clients must compile this consumer and pass the wire contracts.

## Implications for real deployment acceptance

These generated methods provide the management entry points for a separate,
ephemeral Cloudflare fixture. Their successful use still needs live evidence.

| Resource | Provision or configure | Tear down and verify |
| --- | --- | --- |
| Fixture Worker and actor classes | `ComputeClient.WorkerScriptUploadWorkerModule`, multipart modules and metadata including DO migrations | `WorkerScriptDeleteWorker`, then read settings and list namespaces to verify absence |
| Native container application | `CreateApplication` using the DO namespace returned for the fixture Worker | `DeleteApplication`, then poll `GetApplication` until `NotFound` |
| Cloudflare Tunnel | `NetworkingClient.CloudflareTunnelCreateACloudflareTunnel`, `CloudflareTunnelConfigurationPutConfiguration` | Stop the owned connector process, remove owned routes, clean up its connections, then `CloudflareTunnelDeleteACloudflareTunnel` and verify absence |
| Private route | `TunnelRouteCreateATunnelRoute` with the owned tunnel ID and isolated test CIDR | `TunnelRouteDeleteATunnelRoute` with its returned ID and verify absence |
| WARP connector | `CloudflareTunnelCreateAWarpConnectorTunnel`, `CloudflareTunnelConfigurationUpdateWarpConnectorConfiguration` | Stop the owned connector host, `CloudflareTunnelDeleteAWarpConnectorTunnel`, verify absence |
| Access policy | `SecurityClient.AccessPoliciesCreateAnAccessPolicy` on an owned test application | `AccessPoliciesDeleteAnAccessPolicy`, verify absence and revocation propagation through the deployed endpoint |

Use a unique run ID in supported resource names and a durable ownership receipt
containing account, exact returned IDs, Worker name, DO namespace IDs, image
digest and each completed lifecycle step. Record creation intent before sending
requests so a lost create response can be reconciled within that exact run.
Do not infer ownership from a broad name prefix. Limit reconciliation to the
designated test account and exact run identity.

Container deletion is explicitly asynchronous in the pinned response schema.
A successful delete response is a start acknowledgement. The teardown stage
must retain the receipt until absence is observed. Worker deletion without force
can refuse associated bindings. Any forced deletion must establish ownership of
every affected resource first. The Worker delete operation description says no
response body while its response schema declares an object envelope. This suite
tests the declared envelope. The deployed fixture must record which wire behavior
Cloudflare actually supplies before selecting a cleanup decoder.

Actor fan-out belongs inside the deployed fixture's Prospero topology. Management
provisions its Worker, namespaces and capabilities. Creating each actor task is a
runtime operation through the DO bindings and the admitted execution profile.
This loopback suite does not establish actor orchestration, BAREWire transport,
Access enforcement, WARP connectivity, native startup, revocation propagation,
or resource cleanup on Cloudflare.
