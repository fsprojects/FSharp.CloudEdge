---
title: Packages
description: Consume FSharp.CloudEdge from NuGet, with a separate path for release candidates and source contributors.
order: 3
---

Application projects reference the FSharp.CloudEdge libraries by NuGet package ID and version. Each ID matches its assembly name; the [package catalog](../libraries/packages.md) lists the selected packages and their versioned NuGet URLs.

`Runtime` packages expose bindings for Workers, Agents, and other runtime APIs. `Support` packages provide shared bindings for upstream dependencies used by those APIs, such as Zod and the AI SDK. They may be installed transitively, and applications can reference them directly when they use their types.

The version segment in a support name identifies an upstream API family, not the FSharp.CloudEdge package version. For example, `Support.AI.V4.Provider` binds `@ai-sdk/provider` 4.x, while `Support.AI.V7` binds the `ai` package 7.x. Related upstream packages do not all share a major version: `Support.AI.V4.ProviderUtils` binds `@ai-sdk/provider-utils` 5.x alongside the V4 provider family. See each package's npm version in the catalog when choosing JavaScript dependencies.

## Release availability

All **47 packages** in the **0.1.0** release are published on nuget.org. [Browse all FSharp.CloudEdge packages on NuGet](https://www.nuget.org/packages?q=FSharp.CloudEdge), or use the [package catalog](../libraries/packages.md) to find a specific library.

The 0.1.0 release uses the published `Xantham.Fable.Core` and `Xantham.Fable.Core.TS` **0.1.0** packages. Contributors can use the [source build](local-build.md) when regenerating or changing the bindings.

## Add the package your application uses

For a Worker:

```bash
dotnet add package FSharp.CloudEdge.Runtime.Workers --version '0.1.*'
```

For a .NET program that manages Workers:

```bash
dotnet add package FSharp.CloudEdge.Management.Compute --version '0.1.*'
```

The latter brings in `FSharp.CloudEdge.Core.Api` transitively. You can list it explicitly when your project uses its types. [First Worker](first-worker.md) and [First Deploy](first-deploy.md) show complete project files.

Samples use `0.1.*` to accept stable patches in the `0.1` series without moving to `0.2`. Catalog links identify concrete package versions. For repeatable application builds, commit a NuGet lock file and use locked restore in CI; deliberately refresh it when adopting a patch. Release validation pins the candidate and support versions exactly and records every resolved package.

The NuGet feed is `https://api.nuget.org/v3/index.json`. A package page such as `https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Workers/0.1.0` is for browsing; it is not a package source URL.

**0.1.1** keeps FSharp.CloudEdge packages on nuget.org and uses newer Xantham support packages from Xantham's public Cloudsmith feed. To restore this version, add `https://nuget.cloudsmith.io/shayanhabibi/xantham/v3/index.json` as a NuGet package source alongside nuget.org.

The regenerated 0.1.1 bindings are more precise in some places and may require source changes when upgrading from 0.1.0. For example, the Workers `EventContext.Create` parameters for `env` and `params` now use their generated intersection and `Params<'P>` types rather than `obj`. Check code that constructs generated option and context objects when upgrading.

## Fable and npm dependencies

CloudEdge's runtime and Fable support packages contain their F# source as well as .NET assemblies. Fable uses that source when compiling the application to JavaScript. NuGet restores the published Xantham support dependencies transitively. Release validation compiles the site's actual examples against this package combination. Application developers do not run the binding generators.

Use the `fickl` compiler, version 0.7.3, for these bindings. It supports the tagged unions used by the generated APIs; ordinary Fable 5.13 does not correctly emit every inline union case. Install it in a local tool manifest with `dotnet tool install fickl --version 0.7.3` and compile with `dotnet fickl <project.fsproj> --outDir <output-directory>`.

Fickl 0.7.3 passes an already correctly tagged inline-interface value through unchanged. When the tag is missing or different, the default representation is still a JavaScript proxy, which cannot cross a Durable Object RPC or other structured-clone boundary. Fickl also offers opt-in copy representations; see the [RPC probe and representation tradeoffs](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/fickl-inline-union-rpc.md). Values and nested fields must still be valid for the receiving API.

NuGet does not install the upstream JavaScript SDKs. Where an example imports an npm SDK, install the exact package version shown on its library page. Worker-native APIs such as `Request`, D1, and R2 are supplied by the Workers runtime. The platform type declarations describe those APIs; they are not a JavaScript runtime implementation to bundle.

## Verify a candidate before publication

Maintainers can build the selected packages from the repository's checked-in source:

```bash
dotnet run --project tools/NuGetRelease -- pack
dotnet run --project tools/NuGetRelease -- examples --fable
```

Packing writes `artifacts/nuget-release/0.1.1/packages` and a dependency-ordered manifest with package hashes. The consumer checks use that feed for CloudEdge, Cloudsmith for Xantham, and nuget.org for other dependencies, with an isolated cache. This is package consumption testing before any public upload.

After publication, run the same consumers against the public package sources:

```bash
dotnet run --project tools/NuGetRelease -- availability
dotnet run --project tools/NuGetRelease -- examples --fable --public
```

The [maintainer release guide](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/nuget-release.md) covers ownership, release checks, and publication. Package validation is distinct from [hosted runtime verification](verify-bindings.md).
