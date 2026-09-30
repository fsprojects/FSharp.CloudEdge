---
title: Packages
description: Consume FSharp.CloudEdge 0.1.0 from NuGet, with a separate path for release candidates and source contributors.
order: 3
---

Application projects reference the FSharp.CloudEdge libraries by NuGet package ID and version. The release version is **0.1.0**. Each ID matches its assembly name; the [package catalog](../libraries/packages.md) lists the 47 selected packages and their versioned NuGet URLs.

## Release availability

The first `0.1.0` release is being prepared. Package links and examples name the intended public versions; they do not assert that publication has completed. Public Xantham support packages currently need a packaging correction for Fable inline helpers; see the [release blocker and reproduction](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/nuget-release.md#current-release-blocker). Until that is resolved, use the [contributor source build](local-build.md). Locally packed candidates are for release testing and share this upstream limitation.

The release order is: validate candidate packages and consumers, publish dependencies before their consumers, confirm every `0.1.0` package restores from nuget.org, then announce availability. A package page appearing in search is separate from a clean restore succeeding.

## Add the package your application uses

For a Worker:

```bash
dotnet add package FSharp.CloudEdge.Runtime.Workers --version 0.1.0
```

For a .NET program that manages Workers:

```bash
dotnet add package FSharp.CloudEdge.Management.Compute --version 0.1.0
```

The latter brings in `FSharp.CloudEdge.Core.Api` transitively. You can list it explicitly when your project uses its types. [First Worker](first-worker.md) and [First Deploy](first-deploy.md) show complete project files.

The NuGet feed is `https://api.nuget.org/v3/index.json`. A package page such as `https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Workers/0.1.0` is for browsing; it is not a package source URL.

## Fable and npm dependencies

CloudEdge's runtime and Fable support candidates contain their F# source as well as .NET assemblies. Fable uses that source when compiling the application to JavaScript. The Xantham support dependencies are restored by NuGet and also need those source assets; their current packaging issue is described above. Application developers do not run the binding generators.

NuGet does not install the upstream JavaScript SDKs. Where an example imports an npm SDK, install the exact package version shown on its library page. Worker-native APIs such as `Request`, D1, and R2 are supplied by the Workers runtime. The platform type declarations describe those APIs; they are not a JavaScript runtime implementation to bundle.

## Verify a candidate before publication

Maintainers can build the selected packages from the repository's checked-in source:

```bash
dotnet run --project tools/NuGetRelease -- pack
dotnet run --project tools/NuGetRelease -- examples --fable
```

Packing writes `artifacts/nuget-release/0.1.0/packages` and a dependency-ordered manifest with package hashes. The consumer checks use that feed for CloudEdge and nuget.org for external dependencies, with an isolated cache. This is package consumption testing before any public upload.

After publication, run the same consumers against nuget.org alone:

```bash
dotnet run --project tools/NuGetRelease -- availability
dotnet run --project tools/NuGetRelease -- examples --fable --public
```

The [maintainer release guide](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/nuget-release.md) covers ownership, release checks, and publication. Package validation is distinct from [hosted runtime verification](verify-bindings.md).
