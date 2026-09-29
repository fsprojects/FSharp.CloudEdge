The generic response/status, recursive type, and schema simplification
regressions live with the Hawaii softfork:

```sh
python3 ../Hawaii/tests/response-contracts/run.py
```

Run this command from the FSharp.CloudEdge repository root. Consumer-specific
schema pinning, upload-security overlays, and operation coverage belong here.
Generated clients are never repaired by editing their F# output.

Install the pinned schema validator and run the consumer pipeline and fixture
validation regressions from that same root:

```sh
python3 -m pip install -r tests/ManagementIntegration/requirements.txt
npm run test:hawaii-tooling
```

The Tunnel overlay tests use a retained subset of the pristine pinned schema.
They check all 47 corrected error contracts and preserve the source request and
success definitions. Fixture-validator tests authenticate generation inputs and
require zero schema errors under the effective generated contract. Recording an
upstream defect never substitutes for that successful validation.
