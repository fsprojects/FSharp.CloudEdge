---
title: Feature Flags
description: Flagship feature flags through OpenFeature.
order: 11
---

<div class="ce-block-head">
<p class="ce-block-lead">Turn features on and off, and change their settings, without redeploying your Worker. The Flagship binding returns each flag's current value, or the default that your code passes with the call.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Workers</code> <code>Runtime.Flagship</code></li>
<li><span>Binding</span> <code>Flagship</code> in <code>Runtime.Workers</code></li>
<li><span>npm</span> <code>@cloudflare/workers-types</code> 5.20260906.1, <code>@cloudflare/flagship</code> 0.5.0</li>
</ul>
</div>

:::info
Flagship has been in [public beta](https://developers.cloudflare.com/changelog/post/2026-05-26-public-beta/) since May 26, 2026.
:::

## Dark Mode

The site's script requests this JSON and applies the dark theme when `darkMode` is `true`. `getBooleanValue` takes the flag key and a default. The binding returns that default when the flag is missing or evaluation fails, and also for a flag of another type.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract FLAGS: Workers.Flagship

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun _ env _ ->
            async {
                let! darkMode = env.FLAGS.getBooleanValue("dark-mode", false) |> Async.AwaitPromise
                return Workers.Exports.Response.json {| darkMode = darkMode |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> a <code>flagship</code> binding named <code>FLAGS</code>, with your Flagship app's ID as <code>app_id</code>. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare bindings.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";

export const worker = {
    fetch: (_arg, env, _arg_1) => startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(env.FLAGS.getBooleanValue("dark-mode", false)), (_arg_2) => singleton.Return(globalThis.Response.json({
        darkMode: _arg_2,
    }))))),
};

export default worker;
```

</details>

## Storefront Settings

Each flag type has its own method. The promises from `getStringValue` and `getNumberValue` resolve to an F# `string` and a `float`. The result of `getObjectValue` has the F# type of its default, so the flag's JSON value should have the fields of `standardPrices`.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract FLAGS: Workers.Flagship

let standardPrices = {| monthly = 12.; yearly = 120. |}

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun _ env _ ->
            async {
                let! banner = env.FLAGS.getStringValue("banner-text", "Welcome back!") |> Async.AwaitPromise
                let! trialDays = env.FLAGS.getNumberValue("trial-days", 14.) |> Async.AwaitPromise
                let! prices = env.FLAGS.getObjectValue("prices", standardPrices) |> Async.AwaitPromise
                let settings = {| banner = banner; trialDays = trialDays; prices = prices |}
                return Workers.Exports.Response.json settings
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

## Flag Inspector

An admin endpoint shows why `new-checkout` has its current value. `getBooleanDetails` reports the value with its `variant` and `reason`. Both are F# options, and `Option.defaultValue` supplies `"none"` for the JSON.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract FLAGS: Workers.Flagship

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun _ env _ ->
            async {
                let! details = env.FLAGS.getBooleanDetails("new-checkout", false) |> Async.AwaitPromise
                return
                    Workers.Exports.Response.json
                        {| newCheckout = details.value
                           variant = Option.defaultValue "none" details.variant
                           reason = Option.defaultValue "none" details.reason |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

## Library Table

`Runtime.Flagship` binds Flagship's own SDK. Its `FlagshipClient` evaluates flags over HTTP, and its `FlagshipServerProvider` is an OpenFeature provider for server code. From .NET, `Management.Compute` creates Flagship apps and flags with `FlagshipCreateApp` and `FlagshipCreateFlag`.

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.Workers` | `@cloudflare/workers-types` 5.20260906.1 | `Flagship` binding |
| `Runtime.Flagship` | `@cloudflare/flagship` 0.5.0 | HTTP client, OpenFeature provider |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/"><strong>Workers</strong><span>Request handlers</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Declare bindings</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/clients/"><strong>Client Catalog</strong><span>Flagship apps from .NET</span></a>
</div>
