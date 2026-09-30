---
title: Verify Bindings
description: Help establish which Cloudflare workflows work from F#, and report reproducible binding issues.
order: 6
---

Cloudflare's newer execution and agent APIs open up useful possibilities for F# applications. These bindings need community testing to establish where that promise holds in practice. A small reproducible failure, a documented workaround, or a successful runtime check can all help mature the implementation.

## What the evidence establishes

| Check | What it establishes |
| --- | --- |
| F# compilation | The selected types and calls compose in the checked consumer |
| Fable output inspection | Imports, arguments, and emitted calls have the expected shape |
| Local execution | The exercised behavior works in the named local runtime |
| Hosted execution | The exercised behavior works with the recorded Cloudflare configuration |
| Lifecycle verification | Creation, readiness, recovery, and cleanup work for the tested scenario |

A passing check applies to the scenario and versions recorded. It does not establish coverage of the whole SDK. The [integration acceptance record](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/INTEGRATION-ACCEPTANCE.md) describes existing local evidence and the hosted lifecycle work still needed. The [suite catalog](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/tests/integration/suite.json) distinguishes implemented checks from planned coverage.

Known boundaries include direct F# subclassing of `Container`, and imported interfaces such as `AIChatAgent` and `Think` that cannot serve as F# base classes. Read the capability page and the [delivery acceptance record](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/SDK-DELIVERY-ACCEPTANCE-20260913.md) before choosing a fixture. Upstream preview status and binding implementation status are separate concerns.

## Start with one behavior

Choose a small operation: a container request after startup, a Sandbox command with a failing exit code, a workspace file read after wake-up, an Artifacts fork and push, or MCP discovery after reconnect. State the expected result before running it.

Follow [Packages](packages.md) to prepare package consumption, or [Local Build](local-build.md) when testing changes to generated source. From the repository root, compile the relevant site example project. For example:

```bash
dotnet build site/examples/Agents/Agents.fsproj --nologo
dotnet build site/examples/Compute/Compute.fsproj --nologo
dotnet build site/examples/Services/Services.fsproj --nologo
```

Run these builds sequentially. They restore `0.1.0` packages by default; they do not deploy a Worker or prove that a service call succeeds. For contributor source builds, add `-c Release -p:CloudEdgeUseSource=true`. Before publication, use the candidate checks described on [Packages](packages.md). Agents contains the SDK and tool examples, Compute contains workspace and Sandbox examples, and Services contains the Containers consumer.

The documentation excerpt check is separate:

```bash
dotnet run --project tools/NuGetRelease -- snippets
```

It compares the site's F# snippets with source files under `site/examples`; it does not compile or execute them. After running the package consumer checks, compare the displayed JavaScript with their Fable output:

```bash
dotnet run --project tools/NuGetRelease -- snippets --js-root artifacts/nuget-release/0.1.0/consumers-candidate/js
```

For example, emit the Agents consumer with the repository's pinned Fable tool:

```bash
dotnet fable site/examples/Agents/Agents.fsproj --outDir artifacts/site-examples/Agents
```

Use the matching project and output directory for Compute, Services, or Storage. Inspect the relevant generated file before moving on to runtime checks. Emission is a separate check from executing that JavaScript.

## Exercise the runtime boundary

Run the smallest consumer in the environment required by the capability. Record the Fable and npm versions, compatibility date and flags, binding configuration, and whether the test used Node, a local Worker runtime, or Cloudflare. Inspect the emitted import and call when the failure crosses from F# into JavaScript.

Where practical, compare with an equivalent JavaScript or TypeScript call against the same pinned upstream package and configuration. That helps distinguish a binding defect from an upstream behavior or setup issue. An incomplete comparison is still worth reporting; say what you could and could not reproduce.

For hosted lifecycle tests, record startup and readiness, the operation's result, recovery if exercised, and resource deletion. Confirm cleanup separately from the main assertion. Keep tokens and private application data out of published fixtures and logs.

## Share a result

Use the [binding report form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml) for a defect, missing API, or successful verification result. Include:

- Repository commit, binding library, upstream npm package and version, and the affected symbol.
- Minimal F# source, relevant configuration, and commands to reproduce.
- Expected behavior and actual output, including errors or assertions.
- Runtime environment and the checks actually performed.
- Any equivalent upstream example, workaround, or cleanup result.

Check [existing issues](https://github.com/fsprojects/FSharp.CloudEdge/issues) and the [upstream issue notes](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/upstream-issues.md) for related work. If you contribute a fix, retain a focused regression fixture that demonstrates the behavior. Successful reports with reproducible fixtures can help turn planned coverage into repeatable checks.
