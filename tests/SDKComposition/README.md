This consumer checks that the selected native and AI libraries compose through shared dependency types. `Native.fs` passes a `FSharp.CloudEdge.Runtime.Workers.Request` into Containers' `switchPort` helper and returns the same Workers-owned request type. It uses the generated API directly, without casts.

`AI.fs` requires the Gateway Unified factory to return the configured OpenAI-compatible provider, passes Workers AI models into AI Gateway through `LanguageModelV4`, and calls generation with typed options and results. AI Search retains its separate `LanguageModelV3` owner because its pinned package uses that version.

`DurableObjects.fs` defines a direct F# subclass of the generated ambient Durable Object base, using its fetch/alarm interfaces, state initialization, storage, transactions, namespace identities and generic Facet startup types. The generated generic RPC stub result is currently `obj`; the public hand-written `FSharp.CloudEdge.Support.Workers.DurableObjects` helper exposes a separate, runtime-checked `FetchTransport` projection for fetch. It does not provide arbitrary RPC typing. The same F# source is linked into the sibling validation repository for a bounded local workerd lifecycle test.

The final selected delivery passed: all 34 generated SDK/support libraries compiled, and the complete 50-project Release build—including this required consumer, Hawaii management, and the hand-written Workers helper—finished with zero warnings and errors. The build receipt is `artifacts/library-build/latest.json`; the packed Xantham tool is `0.1.0-local.8e4c7b11b0ac90236c25`.

The project targets .NET 8 and uses the exact support package pins in `config/targets.json`. Its generated project references point to the selected delivery destinations in `config/sdk-delivery.json`; the hand-written Workers support project is referenced explicitly.

After generation, run `dotnet build tests/SDKComposition/SDKComposition.fsproj --configuration Release`. The complete `npm run build -- --resume` command also requires this consumer and compiles it in the final library solution.

Compilation establishes that native requests, factory results, model interfaces, and generation arguments compose across the generated assemblies. The separate direct-F# Durable Object smoke passed initialization, typed fetch forwarding, transactions, alarm failure/retry, persisted restart recovery and cleanup against the final Workers receipt: `../FSharp.CloudEdge.Validation/artifacts/local/durable-objects-C1VeIu/report.json`. The final Agents smoke also passed, using its existing JavaScript subclass adapter: `../FSharp.CloudEdge.Validation/artifacts/local/agents-g9QYKR/report.json`.

Provider network behavior, inference responses and Cloudflare deployment lifecycle remain separate runtime validation. These local checks created no Cloudflare resources.
