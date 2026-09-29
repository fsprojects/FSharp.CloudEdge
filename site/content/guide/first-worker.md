---
title: First Worker
description: Write a Worker in F#, compile it with Fable and run it on your machine.
order: 4
---

<div class="ce-block-head">
<p class="ce-block-lead">Your first Worker responds to HTTP requests with JSON. You write it in 22 lines of F# and compile it with Fable. Then you run it on your own machine in workerd, Cloudflare's open-source Workers runtime.</p>
<ul class="ce-facts">
<li><span>You need</span> The FSharp.CloudEdge folder from <a href="/FSharp.CloudEdge/guide/local-build/">Local Build</a>, Node.js with npm, curl</li>
<li><span>You get</span> <code>hello-worker</code> on <code>localhost:8787</code></li>
</ul>
</div>

## Project Folder

With `hello-worker` next to FSharp.CloudEdge, the project file can reference the Workers library through the relative path `../FSharp.CloudEdge`.

1. Open a terminal in the `repos` folder from [Local Build](local-build.md).
2. Create `hello-worker` and enter it.

   ```bash
   mkdir hello-worker
   cd hello-worker
   ```

Stay in `hello-worker` for the remaining commands.

## Project File

Save this as `hello-worker.fsproj`. It declares `Worker.fs` and one package. It also references the Workers library in FSharp.CloudEdge.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Worker.fs" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Fable.Core" Version="5.2.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="../FSharp.CloudEdge/src/Runtime/Platform/FSharp.CloudEdge.Runtime.Workers/FSharp.CloudEdge.Runtime.Workers.fsproj" />
  </ItemGroup>
</Project>
```

Fable.Core 5.2.0 matches the Workers library's own dependency. The Workers library also references two support packages, Xantham.Fable.Core and Xantham.Fable.Core.TS. NuGet restores both through your project reference. Their version numbers end in a digest that the bootstrap step on Local Build computes on your machine, so Fable.Core is the only package `hello-worker.fsproj` lists.

Next to it, add `NuGet.Config` with the package sources NuGet restores from. The first is the feed in `FSharp.CloudEdge/artifacts/tool-feed`, which holds the two support packages. The second, nuget.org, hosts Fable.Core and the Fable compiler.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local-generators" value="../FSharp.CloudEdge/artifacts/tool-feed" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

## Worker Source

Put the handler in `Worker.fs`. It responds to `/` with a JSON greeting and to `/time` with the current UTC time. For any other route it returns a 404.

```fsharp
module Worker

open System
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(fetch = fun request _ _ ->
        let url = Workers.Exports.URL(U2.Case1 request.url)
        let response =
            match url.pathname with
            | "/" ->
                let name = url.searchParams.get "name" |> Option.defaultValue "world"
                Workers.Exports.Response.json {| greeting = $"Hello, {name}" |}
            | "/time" ->
                Workers.Exports.Response.json {| utc = DateTime.UtcNow.ToString "o" |}
            | _ ->
                let init = Workers.ResponseInit.Create(status = 404.)
                Workers.Exports.Response.json({| error = "Not found" |}, U2.Case2 init)
        U2.Case2 response)
```

- `[<ExportDefault>]` declares `worker` as the default export of the JavaScript module. The Workers runtime passes each incoming request to the `fetch` handler of `worker`.
- `ExportedHandler.Create` builds the export from a `fetch` function. The function's first argument is the request, and the two `_` patterns discard the environment and the execution context. All four type arguments of `ExportedHandler` are `obj`, the F# type for any value.
- `Workers.Exports` contains the globals your Worker can call, so `Workers.Exports.URL` and `Workers.Exports.Response` are JavaScript's `URL` and `Response`.
- An anonymous record such as `{| greeting = ... |}` compiles to a plain JavaScript object, and `Response.json` serializes it as the response body.
- `U2.Case1` and `U2.Case2` wrap a value as the first or second type of a TypeScript union. The `URL` constructor takes a `U2<string, URL>`, and the `fetch` function returns a `U2<JS.Promise<Response>, Response>`.

## Fable Compilation

1. Create a tool manifest to pin .NET tool versions for `hello-worker`.

   ```bash
   dotnet new tool-manifest
   ```

   ```text
   The template "Dotnet local tool manifest file" was created successfully.
   ```

2. Install Fable 5.13.0, the release that the tool manifest of FSharp.CloudEdge pins.

   ```bash
   dotnet tool install fable --version 5.13.0
   ```

   ```text
   You can invoke the tool from this directory using the following commands: 'dotnet tool run fable' or 'dotnet fable'.
   ```

3. Compile `hello-worker.fsproj` into the `build` folder.

   ```bash
   dotnet fable hello-worker.fsproj -o build
   ```

   ```text
   Parsing hello-worker.fsproj...
   Project and references (6 source files) parsed in 1970ms

   Started Fable compilation...

   Fable compilation finished in 30684ms
   ```

The six source files are `Worker.fs` plus the sources of the Workers library and its two support packages. Fable compiles all six, so this step takes about half a minute. `build/Worker.js` contains your handler, and the Fable library code it imports is in `build/fable_modules`.

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { defaultArg } from "./fable_modules/fable-library-js.5.13.0/Option.js";
import { concat } from "./fable_modules/fable-library-js.5.13.0/String.js";
import { utcNow, toString } from "./fable_modules/fable-library-js.5.13.0/Date.js";

export const worker = {
    fetch: (request, _arg, _arg_1) => {
        const url = new URL(request.url);
        const matchValue = url.pathname;
        switch (matchValue) {
            case "/": {
                const name = defaultArg(url.searchParams.get("name"), "world");
                return globalThis.Response.json({
                    greeting: concat("Hello, ", name),
                });
            }
            case "/time":
                return globalThis.Response.json({
                    utc: toString(utcNow(), "o"),
                });
            default: {
                const init = {
                    status: 404,
                };
                return globalThis.Response.json({
                    error: "Not found",
                }, init);
            }
        }
    },
};

export default worker;
```

</details>

## Bundled Module

esbuild resolves the imports in `build/Worker.js` and writes one ES module, `dist/worker.js`. You serve that bundle with workerd below and upload it on [First Deploy](first-deploy.md).

1. Add a `package.json` for the npm tools.

   ```bash
   npm init -y
   ```

2. Install esbuild 0.28.2 and workerd 1.20260906.1, the releases that produced the output on this page. npm installs both with a binary for your platform.

   ```bash
   npm install --save-dev esbuild@0.28.2 workerd@1.20260906.1
   ```

   ```text
   added 4 packages, and audited 5 packages in 2s

   found 0 vulnerabilities
   ```

3. Bundle `build/Worker.js`.

   ```bash
   npx esbuild build/Worker.js --bundle --format=esm '--external:cloudflare:*' --outfile=dist/worker.js
   ```

   ```text
     dist/worker.js  24.0kb
   ```

- `--bundle`: esbuild copies the imported `fable_modules` code into the output.
- `--format=esm`: the output is an ES module, with the same default export as `build/Worker.js`.
- `--external:cloudflare:*`: esbuild leaves `cloudflare:` imports in place, for workerd and Cloudflare to resolve. A Durable Object class, for example, extends a base class from `cloudflare:workers`.
- `--outfile`: the path of the bundle.

## Runtime Configuration

workerd reads its settings from a Cap'n Proto text file. Put this one in `hello-worker` as `config.capnp`.

```text
using Workerd = import "/workerd/workerd.capnp";

const config :Workerd.Config = (
  services = [(name = "hello-worker", worker = .helloWorker)],
  sockets = [(address = "localhost:8787", service = "hello-worker")],
);

const helloWorker :Workerd.Worker = (
  modules = [(name = "worker.js", esModule = embed "dist/worker.js")],
  compatibilityDate = "2026-09-06",
);
```

- `using Workerd` loads the configuration schema built into workerd.
- `const config` is the configuration that `workerd serve` starts from.
- `services` defines one service, `hello-worker`, whose Worker is the `helloWorker` constant.
- With `sockets`, workerd listens on port 8787 of `localhost` and passes requests to that service. HTTP is the default protocol for a socket.
- `const helloWorker` defines the Worker.
- `modules` lists one ES module, `worker.js`, and `embed` reads its contents from `dist/worker.js`.
- `compatibilityDate` is required. With it, you opt into the runtime changes up to that day. 2026-09-06 is the date in 5.20260906.1, the version of `@cloudflare/workers-types` used to generate the Workers library.

workerd exits at startup when the compatibility date is later than the newest date it supports, and the error message states that date. Cloudflare explains the scheme in [Compatibility dates](https://developers.cloudflare.com/workers/configuration/compatibility-dates/).

## Local Run

1. Start workerd. With `--watch`, it reloads the Worker after each new bundle.

   ```bash
   npx workerd serve config.capnp --watch
   ```

   workerd prints nothing at startup and keeps serving until you stop it.

2. In a second terminal, call the Worker.

   ```bash
   curl http://localhost:8787/
   ```

   ```text
   {"greeting":"Hello, world"}
   ```

3. Add a name to the query string. Quote the URL, because `?` is a wildcard character in your shell.

   ```bash
   curl 'http://localhost:8787/?name=Ada'
   ```

   ```text
   {"greeting":"Hello, Ada"}
   ```

4. Ask for the time.

   ```bash
   curl http://localhost:8787/time
   ```

   ```text
   {"utc":"2026-09-29T21:16:55.773Z"}
   ```

5. Try any other path. With `-i`, curl also prints the status line and headers.

   ```bash
   curl -i http://localhost:8787/nope
   ```

   ```text
   HTTP/1.1 404 Not Found
   Content-Length: 21
   Content-Type: application/json

   {"error":"Not found"}
   ```

## Edit Loop

In watch mode, Fable keeps the project loaded and recompiles whenever you save `Worker.fs`. With `--runWatch`, Fable starts esbuild after each compile, and workerd reloads the new bundle.

1. In a third terminal, go to `hello-worker` and start the watch.

   ```bash
   dotnet fable watch hello-worker.fsproj -o build --runWatch npx esbuild build/Worker.js --bundle --format=esm '--external:cloudflare:*' --outfile=dist/worker.js
   ```

   The initial compile takes as long as before. Fable prints `Watching ..` when it is ready.

2. In `Worker.fs`, change `Hello` to `Howdy` and save. Fable reports the recompile:

   ```text
   Fable compilation finished in 127ms
   ```

   workerd reports the reload:

   ```text
   Reloading due to config change...
   ```

3. Call the Worker again.

   ```bash
   curl http://localhost:8787/
   ```

   ```text
   {"greeting":"Howdy, world"}
   ```

Stop Fable and workerd with Ctrl+C.

## Next Step

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/first-deploy/"><strong>First Deploy</strong><span>dist/worker.js, uploaded to Cloudflare</span></a>
</div>
