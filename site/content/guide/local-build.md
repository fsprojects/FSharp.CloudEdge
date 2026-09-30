---
title: Local Build
description: Get the libraries by building the repository.
order: 3
---

<div class="ce-block-head">
<p class="ce-block-lead">This is the contributor workflow for regenerating and building the libraries from source. See <a href="/FSharp.CloudEdge/guide/packages/">Packages</a> for the planned NuGet release and its current status. The build generates the runtime bindings from exact releases of Cloudflare's npm packages and the control-plane clients from a pinned copy of Cloudflare's OpenAPI document.</p>
<ul class="ce-facts">
<li><span>You need</span> .NET SDK 10.0.401, Node.js with npm, Python 3, git</li>
<li><span>You get</span> Compiled bindings and a local package feed</li>
</ul>
</div>

## Sibling Checkouts

The build requires three more repositories beside FSharp.CloudEdge. Xantham and Hawaii are the generators: Xantham reads Cloudflare's TypeScript declarations for the runtime bindings, and Hawaii reads Cloudflare's OpenAPI document for the control-plane clients. The third checkout, BAREWire, is a dependency of one test in the solution.

1. Make a parent folder named `repos` for the checkouts.

   ```bash
   mkdir repos
   cd repos
   ```

2. Clone the four repositories, with Hawaii on its `fsharp-cloudedge-support` branch.

   ```bash
   git clone https://github.com/fsprojects/FSharp.CloudEdge.git
   git clone https://github.com/shayanhabibi/Xantham.git
   git clone --branch fsharp-cloudedge-support https://github.com/FidelityFramework/Hawaii.git
   git clone https://github.com/FidelityFramework/BAREWire.git
   ```

3. Switch Xantham to commit `c7e2fa0`. The runtime bindings in the repository were generated with a Xantham tool packed from that commit.

   ```bash
   git -C Xantham switch --detach c7e2fa0daa2ed3ec662cd453ae4c287a6a28673b
   ```

   ```text
   HEAD is now at c7e2fa0 Flatten interface bases emitted as abstract classes
   ```

The parent folder now holds four checkouts. The package-based First Worker guide does not require this folder layout.

```bash
ls
```

```text
BAREWire  FSharp.CloudEdge  Hawaii  Xantham
```

## Generator Tools

Xantham and Hawaii run as .NET tools that you pack from your checkouts. Enter the remaining commands on this page in the FSharp.CloudEdge folder.

1. Install the repository's npm dependencies. They include the pinned TypeScript compiler, which Xantham uses to read the declarations.

   ```bash
   cd FSharp.CloudEdge
   npm ci
   ```

   ```text
   added 3 packages, and audited 4 packages in 330ms

   found 0 vulnerabilities
   ```

2. Pack the generators and Xantham's support packages.

   ```bash
   npm run tools:bootstrap
   ```

   ```text
   xantham: installed xantham 0.1.0-local.4c8eab44ac350e5cf178
   hawaii-unofficial: installed Hawaii.Unofficial 1.0.0-local.8d94cc643f84f47d4848
   Local tools are pinned in .config/dotnet-tools.json; package hashes are in config/tool-packages.json.
   ```

   The bootstrap packs the Xantham and Hawaii checkouts with `dotnet pack` in Release and writes the packages to `artifacts/tool-feed`. It also packs `Xantham.Fable.Core` and `Xantham.Fable.Core.TS`, the two support packages that every runtime binding requires. Each version number ends in a digest of the package's source and build recipe, and the digest also covers your SDK version. With another SDK patch, your digests differ from the ones above. The bootstrap then installs both tools in the repository's tool manifest, `.config/dotnet-tools.json`, and records the package hashes in `config/tool-packages.json`.

3. Restore the tools that the manifest lists.

   ```bash
   npm run tools:restore
   ```

   The command wraps `dotnet tool restore`, which fetches Fable 5.13.0 from nuget.org and the two generators from `artifacts/tool-feed`. When the restore prints a notice about a newer Fable release, keep the manifest at 5.13.0. On First Worker you install Fable 5.13.0 as well.

## Library Build

1. Install the npm packages that provide the TypeScript declarations.

   ```bash
   npm run install:profiles -- --delivery
   ```

   Each folder under `profiles/` is a dependency profile: a set of npm packages with its own lock file. With `--delivery`, the installer runs `npm ci --ignore-scripts` in the 24 profiles that `npm run build` reads. It prints one line per profile, such as `agents: installed`.

2. Check the support package.

   ```bash
   npm run test:support-package
   ```

   This builds a small F# project with `Xantham.Fable.Core`, restored from the local feed. Fable compiles that project to JavaScript, and a Node script checks the result. It finishes with `Package-only support smoke passed` and the package version.

3. Generate and compile the libraries.

   ```bash
   npm run build
   ```

   - Xantham generates the 31 runtime libraries from the declarations in the profiles, along with the three AI SDK contract libraries they share. The build then compiles those 34 libraries.
   - A Python runner downloads Cloudflare's OpenAPI document at a pinned commit and verifies its SHA-256. Hawaii generates the 11 control-plane clients and their shared model project from the downloaded file. The runner builds those 12 projects. A test program then sends four requests through the Tenancy and Compute clients to a loopback HTTP server.
   - The build compiles the solution, `FSharp.CloudEdge.slnx`, in Release.

   On success, the last line of output starts with `Complete hierarchy compiled` and lists the project counts. The build writes a log for each stage under `artifacts/`, and `artifacts/library-build/latest.json` records how the most recent attempt ended.

4. Finish with the ByteBridge test.

   ```bash
   npm run test:bridge
   ```

   ```text
   {"passed":13,"runtime":"v25.1.0","scope":"Fable, generated Workers Body bindings and BAREWire codecs in Node Fetch"}
   ```

   The ByteBridge test project references the Workers bindings and BAREWire. Fable translates it to JavaScript, and Node runs its 13 checks. The JSON line reports the number of checks that passed and the Node runtime.

## Rebuilds

A second build can reuse the first. With `--resume`, the build compares each library's input, generator and output hashes with the values that the previous build recorded. Where they match, the build skips generation for that library. Where they differ, the generator runs again. Compilation covers every library either way.

```bash
npm run build -- --resume
```

The bootstrap compares hashes too. When a generator's source is unchanged, the bootstrap reuses the package in `artifacts/tool-feed` after it verifies the package against its build receipt.

In a new clone on the same machine, the bootstrap packs the tools again with the same version numbers. NuGet still holds the packages from the first clone in its cache at `~/.nuget/packages`, so the bootstrap installs those copies and stops with `installed cache differs from the local package`. Delete the cached tool packages, then repeat the bootstrap.

```bash
rm -rf ~/.nuget/packages/xantham ~/.nuget/packages/hawaii.unofficial
npm run tools:bootstrap
```

## Build Output

Source-based consumers use two folders from FSharp.CloudEdge.

- `src/` holds the bindings, compiled in Release. A source-based Worker consumer can reference `src/Runtime/Platform/FSharp.CloudEdge.Runtime.Workers`, and a source-based upload program can reference the control-plane projects in `src/Core` and `src/Management`.
- `artifacts/tool-feed/` is the local package feed. The runtime bindings reference `Xantham.Fable.Core` and `Xantham.Fable.Core.TS`, so a project that uses the bindings restores those two packages from this folder.

To run site examples against these source builds instead of packages, pass `-p:CloudEdgeUseSource=true`. The normal example mode uses NuGet `0.1.0`.

```bash
dotnet build site/examples/FirstWorker/FirstWorker.fsproj -c Release -p:CloudEdgeUseSource=true
```

## Next Step

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/first-worker/"><strong>First Worker</strong><span>Worker.fs, compiled and run on your machine</span></a>
</div>
