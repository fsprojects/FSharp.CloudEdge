# Grouped Hawaii client smoke test

The default `generators/hawaii/generate.py` pipeline runs this sample against its
staged output before installation. To check an installed set independently:

```sh
python3 generators/hawaii/tests/check_grouped_clients.py --run-directory artifacts/hawaii/manual-http-smoke
```

The sample uses real loopback HTTP exchanges with generated Tenancy and Compute
clients: account listing, dynamic multipart asset upload, a 429 status-class
response, and rejection of an undeclared 599 status. It checks that response
models share the single Core assembly, account IDs flow into a different client,
query values are encoded, and submitted base64 text and part media survive on
the wire.

No credentials or Cloudflare network requests are used. This checks generated
client transport and assembly ownership, not deployment or server acceptance of
an arbitrary application payload. The fixture runs in Release after the full
staged solution has compiled.
