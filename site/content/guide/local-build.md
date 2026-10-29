---
title: Local Build
description: Regenerate and build the libraries from source.
order: 3
---

This is the contributor workflow for regenerating and building FSharp.CloudEdge. Application developers can use the [NuGet packages](packages.md) without the generator checkouts.

The build needs .NET SDK 10.0.401, Node.js and npm, Bun, Python 3, and Git. Xantham generates the TypeScript bindings; Hawaii generates the control-plane clients. The Pi projection also uses the Xantham source revision recorded in `config/sdk-delivery.json`.

## Sibling checkouts

Place the checkouts beside one another:

```text
repos/
  BAREWire/
  FSharp.CloudEdge/
  Hawaii/
  Xantham/
```

The default bootstrap paths are `../Xantham` and `../Hawaii`. BAREWire is needed by the ByteBridge test. Check out the Xantham revision pinned by the Pi projection before generating it; use the Hawaii `fsharp-cloudedge-support` branch for the control-plane generator. The bootstrap also accepts `--xantham-source` and `--hawaii-source` when the checkouts live elsewhere.

## Install generator tools

From the FSharp.CloudEdge checkout, install the root dependencies, build both local generator packages, and restore the tools:

```bash
npm ci
npm run tools:bootstrap
npm run tools:restore
npm run tools:check
```

The bootstrap writes the local packages to `artifacts/tool-feed`, pins them in `.config/dotnet-tools.json`, and records package hashes in `config/tool-packages.json`. It also packs the Xantham Fable support sources into the local feed and updates the source-build package pins. Re-run the bootstrap after changing either generator. To rebuild one tool, pass `--only xantham` or `--only hawaii` after `--`.

## Generate and build

Install the npm profiles and build the selected libraries:

```bash
npm run install:profiles -- --delivery
npm run build -- --resume
```

The build generates the selected SDK bindings and control-plane clients, verifies their generation receipts, and compiles `FSharp.CloudEdge.slnx`. `--resume` reuses outputs only when their input, generator, and output hashes match. Logs and the latest build report are under `artifacts/`.

For the Pi lifecycle check, install `celld` and make it available on `PATH`, or set `CLOUDEDGE_CELLD` to its executable. With the pinned Xantham checkout clean, run:

```bash
npm run test:agents-pi
```

The check runs the generated Pi harness in a local SQLite-backed Durable Object, exercises submission and prompt operations, and verifies persisted state after a runtime restart. It uses a local model fixture; it does not test a hosted model provider.

Other focused checks include:

```bash
npm run test:support-package
npm run test:bridge
```

The support check restores the locally packed `Xantham.Fable.Core` package and exercises its Fable output. ByteBridge needs the sibling BAREWire checkout.

## Build examples from source

Site examples normally use NuGet packages from the current `0.1` patch line. To use the generated projects in this checkout instead:

```bash
dotnet build site/examples/FirstWorker/FirstWorker.fsproj -c Release -p:CloudEdgeUseSource=true
```

The repository's `NuGet.Config` supplies `artifacts/tool-feed` for the local Xantham support packages and nuget.org for public dependencies. Published 0.1.1 packages use the Xantham Cloudsmith feed described in the [package guide](packages.md).

The release tool checks the site's F# snippets against their source examples. Once candidate examples have been compiled to JavaScript, it can also check the emitted JavaScript blocks:

```bash
dotnet run --project tools/NuGetRelease -- snippets
dotnet run --project tools/NuGetRelease -- snippets --js-root artifacts/nuget-release/0.1.1/consumers-candidate/js
```
