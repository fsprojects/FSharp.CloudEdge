---
title: Support Libraries
description: Shared AI contracts and Workers helpers.
order: 13
---

<div class="ce-block-head">
<p class="ce-block-lead">Write an AI feature once and choose its model for each request. A model created by one library is a valid argument for another, because both use the contract types defined here. <code>Support.Workers</code> adds checked <code>fetch</code> helpers for Durable Objects.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Support.AI.V4.Provider</code> <code>Support.AI.V4.OpenAICompatible</code> <code>Support.AI.V3.Provider</code> <code>Support.Workers</code></li>
<li><span>npm</span> <code>@ai-sdk/provider</code> 4.0.10 and 3.0.15, <code>@ai-sdk/openai-compatible</code> 3.0.44</li>
</ul>
</div>

## Model Switch

`workersModel` and `endpointModel` use different libraries to create their models. `createOpenAICompatible` builds a provider for an endpoint that exposes an OpenAI-compatible API. Both functions have the return type `LanguageModelV4`, so either model can be the result of `modelFor`. Prompt code written against `LanguageModelV4`, like the Summarizer's on the [AI](ai.md) page, type-checks with either model.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module WorkersAI = FSharp.CloudEdge.Runtime.WorkersAIProvider
module Compatible = FSharp.CloudEdge.Support.AI.V4.OpenAICompatible
module V4 = FSharp.CloudEdge.Support.AI.V4.Provider

type Env =
    abstract AI: Workers.Ai<Workers.AiModels>
    abstract LLM_URL: string
    abstract LLM_KEY: string
    abstract LLM_MODEL: string

let workersModel (env: Env) : V4.LanguageModelV4 =
    let settings = WorkersAI.WorkersAISettings2.Create(binding = env.AI)
    let workersai = WorkersAI.Exports.createWorkersAI(U2.Case1 settings)
    workersai.chat(U2.Case1 "@cf/meta/llama-3.3-70b-instruct-fp8-fast")

let endpointModel (env: Env) : V4.LanguageModelV4 =
    let settings =
        Compatible.OpenAICompatibleProviderSettings.Create(
            baseURL = env.LLM_URL,
            name = "own-llm",
            apiKey = env.LLM_KEY
        )
    let endpoint = Compatible.Exports.createOpenAICompatible settings
    endpoint.chatModel env.LLM_MODEL

let modelFor (env: Env) (plan: string) =
    if plan = "pro" then endpointModel env else workersModel env
```

<div class="ce-needs"><p><strong>Needs</strong> a Workers AI binding named <code>AI</code> and a secret named <code>LLM_KEY</code>. The variables <code>LLM_URL</code> and <code>LLM_MODEL</code> hold the endpoint's address and its model. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare them.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { createWorkersAI } from "workers-ai-provider";
import { createOpenAICompatible } from "@ai-sdk/openai-compatible";

export function workersModel(env) {
    const workersai = createWorkersAI({
        binding: env.AI,
    });
    return workersai.chat("@cf/meta/llama-3.3-70b-instruct-fp8-fast");
}

export function endpointModel(env) {
    const endpoint = createOpenAICompatible({
        baseURL: env.LLM_URL,
        name: "own-llm",
        apiKey: env.LLM_KEY,
    });
    return endpoint.chatModel(env.LLM_MODEL);
}

export function modelFor(env, plan) {
    if (plan === "pro") {
        return endpointModel(env);
    }
    else {
        return workersModel(env);
    }
}
```

</details>

## Usage Meter

AI Search models implement version 3 of the AI SDK contract, and `Support.AI.V3.Provider` defines its F# types. Workers AI and AI Gateway models implement version 4, and the two versions are separate F# types. `askWithUsage` takes any `LanguageModelV3`, such as the AI Search model in Help Center Answers on the [AI](ai.md) page. The result contains the answer and the call's token counts. `finish` is the reason that generation stopped, such as `stop` or `length`.

```fsharp
open Fable.Core

module V3 = FSharp.CloudEdge.Support.AI.V3.Provider

let askWithUsage (model: V3.LanguageModelV3) (question: string) =
    async {
        let part = V3.LanguageModelV3Message3.Content.Item.Text(question, None)
        let prompt = [| V3.LanguageModelV3Message.User([| part |], None) |]
        let options = V3.LanguageModelV3CallOptions.Create(prompt = prompt, maxOutputTokens = 400.)
        let! result = model.doGenerate options |> Async.AwaitPromise
        let answer =
            result.content
            |> Array.choose (function
                | V3.LanguageModelV3Content.Text(text, _) -> Some text
                | _ -> None)
            |> String.concat ""
        return
            {| answer = answer
               inputTokens = result.usage.inputTokens.total |> Option.defaultValue 0.
               outputTokens = result.usage.outputTokens.total |> Option.defaultValue 0.
               finish = result.finishReason.unified |}
    }
    |> Async.StartAsPromise
```

## Room Status

The Worker reads a room's name from the URL path. `getFetchByName` derives the Durable Object's ID from that name and returns the object's stub as a `FetchTransport`, whose `fetch` method forwards a request to the room. Here the Worker sends a GET request for the same URL and answers with the room's response.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Transport = FSharp.CloudEdge.Support.Workers.DurableObjects

type Env =
    abstract ROOMS: Workers.DurableObjectNamespace<obj>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            let room = Workers.Exports.URL(U2.Case1 request.url).pathname.Substring(1)
            let transport = Transport.getFetchByName env.ROOMS room
            transport.fetch(Workers.Exports.Request.Create(U3.Case1 request.url)) |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> a Durable Object binding named <code>ROOMS</code>. <a href="/FSharp.CloudEdge/libraries/platform/durable-objects/">Durable Objects</a> covers the room's class, and <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare the binding.</p></div>

## Regional Room

`get` takes options for the stub, such as a location hint. With `weur`, the Worker requests western Europe for a new room. The hint takes effect only on the first `get` for each room, as Cloudflare's [data location](https://developers.cloudflare.com/durable-objects/reference/data-location/) reference describes.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Transport = FSharp.CloudEdge.Support.Workers.DurableObjects

type Env =
    abstract ROOMS: Workers.DurableObjectNamespace<obj>

let westernEurope =
    Workers.DurableObjectNamespaceGetDurableObjectOptions.Create(
        locationHint = Workers.DurableObjectLocationHint.Weur
    )

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            let room = Workers.Exports.URL(U2.Case1 request.url).pathname.Substring(1)
            let stub = env.ROOMS.get(env.ROOMS.idFromName room, westernEurope)
            let transport = Transport.requireFetchTransport stub
            transport.fetch(Workers.Exports.Request.Create(U3.Case1 request.url)) |> U2.Case1
    )
```

`requireFetchTransport` checks that the stub has a `fetch` function and returns it as a `FetchTransport`. For any other value, it throws a `TypeError`. `getFetchByName` and `getFetchById` run the same check.

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { substring } from "./fable_modules/fable-library-js.5.13.0/String.js";
import { requireFetchTransport } from "./src/Support/FSharp.CloudEdge.Support.Workers/DurableObjects.js";

export const westernEurope = {
    locationHint: "weur",
};

export const worker = {
    fetch: (request, env, _arg) => {
        const room = substring((new URL(request.url)).pathname, 1);
        const transport = requireFetchTransport(env.ROOMS.get(env.ROOMS.idFromName(room), westernEurope));
        return transport.fetch(new globalThis.Request(request.url));
    },
};

export default worker;
```

</details>

## Library Table

| Library | npm package | What it covers |
| --- | --- | --- |
| `Support.AI.V4.Provider` | `@ai-sdk/provider` 4.0.10 | v4 model contracts |
| `Support.AI.V4.OpenAICompatible` | `@ai-sdk/openai-compatible` 3.0.44 | OpenAI-compatible models |
| `Support.AI.V3.Provider` | `@ai-sdk/provider` 3.0.15 | v3 model contracts |
| `Support.Workers` | Handwritten, no package | Durable Object `fetch` |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/ai/"><strong>AI</strong><span>Models and prompts</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/durable-objects/"><strong>Durable Objects</strong><span>Room classes</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Declare bindings</span></a>
</div>
