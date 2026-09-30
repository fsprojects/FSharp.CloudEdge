---
title: Containers
description: Run containerized services and route requests to them from an F# Worker.
---

<div class="ce-block-head">
<p class="ce-block-lead">Use an existing Linux service or toolchain behind a Worker, with a Durable Object controlling the container instance.</p>
<ul class="ce-facts">
<li><span>Library</span> <code>Runtime.Containers</code></li>
<li><span>npm</span> <code>@cloudflare/containers</code> 0.3.7</li>
<li><span>Plan</span> Workers Paid</li>
<li><span>Binding gap</span> Direct F# subclassing of <code>Container</code> is unsupported</li>
</ul>
</div>

## What Containers provides

[Cloudflare Containers](https://developers.cloudflare.com/containers/) runs a container image alongside the Workers platform. A Worker routes traffic to a container-backed Durable Object; the container supplies the application server or executable environment. This is useful for image processing, native dependencies, and existing services, as well as agent execution.

Choose the image, listening port, and instance identity for the service. Decide how it should start, become ready, sleep, and recover from failure. The container's lifecycle and filesystem are separate from the Durable Object's durable application state.

## Containers and Sandbox

| Capability | Use it for |
| --- | --- |
| Containers | Deploying and operating your own containerized service |
| Sandbox SDK | Command execution, files, processes, and development tooling on top of Containers |
| Computer container backend | Running commands against a Computer workspace through its synchronization model |

A service that exposes HTTP from a container does not need an agent framework. Use the [Sandbox page](agents/sandbox.md) when the desired API is execute, read files, or manage processes rather than your container's own application protocol.

## Image Resizer

The Worker posts each upload to a container and returns the container's response. `switchPort` copies the request and sets its target port to 8080. `getFetchByName`, from the [Support Libraries](support.md), produces a fetch stub for the container's Durable Object.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Containers = FSharp.CloudEdge.Runtime.Containers
module Transport = FSharp.CloudEdge.Support.Workers.DurableObjects

type Env =
    abstract RESIZER: Workers.DurableObjectNamespace<Containers.Container<obj>>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            let resize =
                Workers.Exports.Request.Create(
                    U3.Case1 "http://resizer/thumbnail?width=320",
                    Workers.RequestInit.Create(``method`` = "POST", body = request.body))
            let resizer = Transport.getFetchByName env.RESIZER "resizer"
            resizer.fetch (Containers.Exports.switchPort(resize, 8080.)) |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> the Workers Paid plan and a container image whose server listens on port 8080. The <code>RESIZER</code> binding refers to a Durable Object class that extends <code>Container</code>. F# code cannot subclass <code>Container</code> in 0.1.0, so that class is written in JavaScript. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how an F# program declares a Worker's bindings.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { getFetchByName } from "./src/Support/FSharp.CloudEdge.Support.Workers/DurableObjects.js";
import { switchPort } from "@cloudflare/containers";

export const worker = {
    fetch: (request, env, _arg) => {
        const resize = new globalThis.Request("http://resizer/thumbnail?width=320", ({
            method: "POST",
            body: request.body,
        }));
        const resizer = getFetchByName(env.RESIZER, "resizer");
        return resizer.fetch(switchPort(resize, 8080));
    },
};

export default worker;
```

</details>

## F# integration boundary

The current bindings let F# consumers use container interfaces and helpers such as `switchPort`. They do not support deriving an F# class directly from the imported `Container` class. The example therefore calls an existing container-backed Durable Object whose class is implemented in JavaScript.

This is a known binding limitation, documented in the [delivery acceptance record](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/SDK-DELIVERY-ACCEPTANCE-20260913.md). A compiling fetch consumer establishes type composition; it does not verify image deployment, readiness, routing, or recovery on Cloudflare.

## What needs runtime verification

A useful container fixture pins the image, starts the instance, waits for readiness, sends a request through the F# consumer, observes restart or version-change behavior, and removes its resources. Record the response and the cleanup outcome separately. The [integration acceptance plan](https://github.com/fsprojects/FSharp.CloudEdge/blob/main/docs/INTEGRATION-ACCEPTANCE.md) lists the hosted evidence still needed.

## Related Pages

- [Sandbox](agents/sandbox.md)
- [Execution environments](compute.md)
- [Computer workspaces](agents/computer.md)
- [Worker deployment and bindings](control-plane/worker-upload.md)

## Help verify these bindings

Contributions that reproduce a type mismatch, incorrect emitted call, or a hosted lifecycle result help mature this binding. See [Verify bindings](../guide/verify-bindings.md) for the evidence to collect and the [binding issue form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml).
