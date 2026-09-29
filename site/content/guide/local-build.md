---
title: Local Build
description: Get the libraries by building the repository.
order: 3
---

<div class="ce-block-head">
<p class="ce-block-lead">Today you get the libraries by cloning the FSharp.CloudEdge repository and building it on your machine. The build generates each binding from exact versions of Cloudflare's npm packages and OpenAPI document, then compiles it. You add the result to your Worker project on the next page.</p>
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

3. Switch Xantham to commit `c7e2fa0`. The pinned Xantham tool was packed from that commit.

   ```bash
   git -C Xantham switch --detach c7e2fa0daa2ed3ec662cd453ae4c287a6a28673b
   ```

   ```text
   HEAD is now at c7e2fa0 Flatten interface bases emitted as abstract classes
   ```

The parent folder now holds four checkouts. On the First Worker page you create `hello-worker` in this same folder.

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

   The bootstrap packs the Xantham and Hawaii checkouts with `dotnet pack` in Release and writes the packages to `artifacts/tool-feed`. It also packs `Xantham.Fable.Core` and `Xantham.Fable.Core.TS`, the two support packages that every runtime binding requires. Each version number ends in a digest of the package's source and build recipe, and your SDK version is part of that digest. The bootstrap then installs both tools in the repository's tool manifest, `.config/dotnet-tools.json`, and records the package hashes in `config/tool-packages.json`. Its last line on success starts `Local tools are pinned in`.

3. Restore the tools that the manifest lists.

   ```bash
   npm run tools:restore
   ```

   The command wraps `dotnet tool restore`, which fetches Fable 5.13.0 from nuget.org and the two generators from `artifacts/tool-feed`.

## Library Build

1. Install Cloudflare's npm packages.

   ```bash
   npm run install:profiles -- --delivery
   ```

   Each folder under `profiles/` is a dependency profile: a set of npm packages with its own lock file. With `--delivery`, the installer calls `npm ci --ignore-scripts` in each of the 24 profiles that hold this release's inputs. It prints one line per profile, such as `agents: installed`.

2. Check the support package.

   ```bash
   npm run test:support-package
   ```

   This builds a small F# project with `Xantham.Fable.Core`, restored from the local feed. Fable compiles that project to JavaScript, and Node verifies the output. It finishes with `Package-only support smoke passed` and the package version.

3. Generate and compile the libraries.

   ```bash
   npm run build
   ```

   - Xantham generates the 31 runtime libraries, and the 3 AI contract libraries they share, from the declarations in the profiles. An initial compile checks them.
   - A Python runner downloads Cloudflare's OpenAPI document at a pinned commit and verifies its SHA-256. Hawaii generates the 11 control-plane clients and their shared model project from the downloaded file. The runner builds those 12 projects and exercises them with a loopback HTTP server.
   - The build compiles the solution, `FSharp.CloudEdge.slnx`, in Release.

   On success, the final line reads `Complete hierarchy compiled`, followed by the project counts. The build writes a log for each stage under `artifacts/`, and `artifacts/library-build/latest.json` records how the last attempt ended. In the recorded generation that produced the committed clients, building those 12 projects took 350 seconds and peaked at 10.5 GiB of memory.

4. Finish with the ByteBridge test.

   ```bash
   npm run test:bridge
   ```

   The ByteBridge test combines the Workers bindings with BAREWire. Fable translates it to JavaScript. Node then runs its 13 checks and prints a JSON object with the number that passed.

## Rebuilds

A second build can reuse the first. With `--resume`, the build compares each library's input and output hashes with the previous build. Where they match, the build skips generation for that library. Where they differ, the generator runs again. Compilation covers every library either way.

```bash
npm run build -- --resume
```

The bootstrap compares hashes too. When a generator's source is unchanged, it reuses the package in `artifacts/tool-feed`, provided the package still matches its receipt.

## Build Output

Your projects on the next pages use two folders from FSharp.CloudEdge.

- `src/` holds the bindings, compiled in Release. Your Worker project on First Worker references `src/Runtime/Platform/FSharp.CloudEdge.Runtime.Workers`, and the upload program on First Deploy references the control-plane projects in `src/Core` and `src/Management`.
- `artifacts/tool-feed/` is the local package feed. The runtime bindings reference `Xantham.Fable.Core` and `Xantham.Fable.Core.TS`, so a project that uses the bindings restores those two packages from this folder.

## Next Step

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/first-worker/"><strong>First Worker</strong><span>Worker.fs, compiled and run on your machine</span></a>
</div>
