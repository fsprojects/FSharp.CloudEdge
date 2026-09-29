# AI Gateway SDK selection

Use Cloudflare's `ai-gateway-provider` for the selected provider-wrapper integration.
Retain the existing `4.0.0` pin. Native Workers `Ai.gateway(id)` access and Hawaii's
OpenAPI-generated management operations remain complementary parts of the project.
Further investigation of the separate `@cloudflare/ai-gateway` package is shelved.
The provider is included in the current default [selected library build](SDK-LIBRARY.md),
alongside explicit AI SDK support owners. The isolated probe below predates that hierarchy.

## Why this package

[Cloudflare's integration guide](https://developers.cloudflare.com/ai-gateway/integrations/vercel-ai-sdk/)
uses `ai-gateway-provider`, maintained in the
[Cloudflare AI repository](https://github.com/cloudflare/ai/tree/main/packages/ai-gateway-provider).
The guide covers Workers AI and external model providers. This server-side integration
does not require taking on React or web-client behavior.

The provider was already in this repository's inventory and the earlier 19-input
generation/diagnostic compilation sweep. Highlighting the separate package's failure as
the outstanding AI Gateway item obscured that positive evidence. The delivery now tracks
the selected provider and its actual consumer requirements.

## Stable and prerelease check

Registry metadata was fetched on 2026-09-13. `ai-gateway-provider` has 29 published
versions, no published prerelease versions, and `latest=4.0.1`.
[Release 4.0.1](https://github.com/cloudflare/ai/releases/tag/ai-gateway-provider%404.0.1)
was published on September 11 and raises the minimum OpenAI-compatible provider version
to preserve Gemini thought signatures across unified tool-call turns.

The registry tarballs for `4.0.0` and `4.0.1` were checked against their SRI digests and
compared. Both contain 125 files. **Only `package.json` differs**: all runtime JavaScript
and all 40 declaration files are byte-identical. The raised peer minimum is
`@ai-sdk/openai-compatible@^3.0.32`; this project's existing profile already locks
`3.0.44`. Retaining `4.0.0` therefore does not miss that dependency correction in the
current profile. This comparison does not substitute for runtime acceptance.

| Existing profile dependency | Exact installed version |
|---|---|
| `ai-gateway-provider` | `4.0.0` |
| `ai` | `7.0.93` |
| `@ai-sdk/provider` | `4.0.10` |
| `@ai-sdk/provider-utils` | `5.0.36` |
| `@ai-sdk/openai-compatible` | `3.0.44` |

The separate `@cloudflare/ai-gateway` registry has ten published versions with tags
`latest=0.0.6`, `beta=0.0.1` and `bin=0.0.3-bin`; no newer prerelease was found. Its
historical releases were not installed or tested. That investigation stopped when the
user confirmed the provider package as the focus; there is no claim that every historical
release is broken.

## Binding evidence and remaining acceptance

The exact **19-input** `aigatewayprovider` partition has now been regenerated with the
final immutable Xantham tool `0.1.0-local.80272a2344056b6a0b9a` (`35feabd`) and compiled
against **net8.0 and the project's exact support packages: zero warnings, zero errors**.
This was an isolated diagnostic probe under the earlier four-library default. The current
selection includes this provider and has separate shared-type composition checks.
Findings remain explicit: 17 exact, 8 ergonomic, 34 widened and 1 escape.

A small consumer also compiles with zero warnings/errors and a clean FCS check. It constructs
typed gateway settings and unified-provider options and calls `createAiGateway` and
`createUnified`. These functions were compiled, not executed. `createUnified` returns
`obj`, and gateway model arguments/results remain `obj`: the partition does not own the
external `OpenAICompatibleProvider` and `LanguageModelV4` types. The consumer establishes
typed configuration and factory access, not a fully typed model selector.

Shared ownership of those external types and live inference/lifecycle acceptance remain
work for the selected integration. The intended live check is bounded Workers AI and
external-provider inference, response/error handling, and gateway creation/deletion with
verified cleanup. No live resources were created for this SDK/version investigation.

## Local evidence

- Registry snapshots and verified package comparison:
  `artifacts/one-shot-20260913/ai-gateway-alternatives-audit/`.
- File comparison:
  `artifacts/one-shot-20260913/ai-gateway-alternatives-audit/provider-package-comparison.json`.
- Final tool generation/compile probe:
  `artifacts/one-shot-20260913/ai-gateway-provider-final-probe/results.json` and `build.log`.
- Consumer compilation and final provenance checks:
  `artifacts/one-shot-20260913/ai-gateway-provider-final-probe/consumer/results.json`.
- Stopped historical-package investigation:
  `artifacts/one-shot-20260913/ai-gateway-version-audit/README.md`.
