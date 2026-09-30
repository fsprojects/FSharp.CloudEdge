---
title: AI
description: Workers AI, AI Gateway and AI Search from F#.
order: 7
---

<div class="ce-block-head">
<p class="ce-block-lead">Your Worker can summarize text and answer questions from your own documents. Workers AI hosts the models, and AI Gateway routes requests from your Worker to other providers.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.WorkersAIProvider</code> <code>Runtime.AIGatewayProvider</code> <code>Runtime.AISearchProvider</code> <code>Runtime.AIUtils</code> <code>Runtime.AIChat</code> <code>Runtime.Think</code></li>
<li><span>npm</span> <code>workers-ai-provider</code>, <code>ai-gateway-provider</code>, <code>ai-search-provider</code> and four <code>@cloudflare/</code> packages</li>
<li><span>Binding</span> <code>Ai</code> in <code>Runtime.Workers</code></li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/workers-ai/platform/pricing/">10,000 Workers AI Neurons a day</a></li>
</ul>
</div>

## Summarizer

Post an article and get a two-sentence summary back. `createWorkersAI` wraps the Worker's `AI` binding as a model provider, and `chat` takes a model id from the [Workers AI catalog](https://developers.cloudflare.com/workers-ai/models/).

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module WorkersAI = FSharp.CloudEdge.Runtime.WorkersAIProvider
module V4 = FSharp.CloudEdge.Support.AI.V4.Provider

type Env =
    abstract AI: Workers.Ai<Workers.AiModels>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! article = request.text () |> Async.AwaitPromise
                let settings = WorkersAI.WorkersAISettings2.Create(binding = env.AI)
                let workersai = WorkersAI.Exports.createWorkersAI (U2.Case1 settings)
                let model = workersai.chat<string> (U2.Case1 "@cf/zai-org/glm-4.7-flash")
                let articlePart = V4.LanguageModelV4Message3.Content.Item.Text(article, None)
                let prompt =
                    [| V4.LanguageModelV4Message.System("Summarize in two sentences.", None)
                       V4.LanguageModelV4Message.User([| articlePart |], None) |]
                let options = V4.LanguageModelV4CallOptions.Create prompt
                let! result = model.doGenerate options |> Async.AwaitPromise
                let summary =
                    result.content
                    |> Array.choose (function
                        | V4.LanguageModelV4Content.Text(text, _) -> Some text
                        | _ -> None)
                    |> String.concat ""
                return Workers.Exports.Response.json {| summary = summary |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> a Workers AI binding named <code>AI</code>. <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a> shows how to declare it.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { createWorkersAI } from "workers-ai-provider";
import { join } from "./fable_modules/fable-library-js.5.13.0/String.js";
import { choose } from "./fable_modules/fable-library-js.5.13.0/Array.js";

export const worker = {
    fetch: (request, env, _arg) => startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(request.text()), (_arg_1) => {
        const workersai = createWorkersAI({
            binding: env.AI,
        });
        const model = workersai.chat("@cf/zai-org/glm-4.7-flash");
        const prompt = [{
            role: "system",
            content: "Summarize in two sentences.",
        }, {
            role: "user",
            content: [{
                type: "text",
                text: _arg_1,
            }],
        }];
        const options = {
            prompt: prompt,
        };
        return singleton.Bind(awaitPromise(model.doGenerate(options)), (_arg_2) => {
            const summary = join("", choose((_arg_3) => {
                if (_arg_3.type === "text") {
                    return _arg_3.text;
                }
                else {
                    return undefined;
                }
            }, _arg_2.content));
            return singleton.Return(globalThis.Response.json({
                summary: summary,
            }));
        });
    }))),
};

export default worker;
```

</details>

The prompt and the result are AI SDK types from the [Support Libraries](support.md). Every AI SDK language model implements `doGenerate`, which the AI Search and AI Gateway examples also use.

## Duplicate Detector

Before a forum accepts a new question, compare it with one already posted. The embedding model produces a 768-dimension vector for each text, and `cosine` reduces the pair to one score that is higher for closer meanings.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module WorkersAI = FSharp.CloudEdge.Runtime.WorkersAIProvider
module V4 = FSharp.CloudEdge.Support.AI.V4.Provider

type Env =
    abstract AI: Workers.Ai<Workers.AiModels>

type Pair = {| question: string; existing: string |}

let cosine (a: float[]) (b: float[]) =
    let dot = Array.map2 ( * ) a b |> Array.sum
    let norm (v: float[]) = v |> Array.sumBy (fun x -> x * x) |> sqrt
    dot / (norm a * norm b)

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! pair = request.json<Pair> () |> Async.AwaitPromise
                let settings = WorkersAI.WorkersAISettings2.Create(binding = env.AI)
                let workersai = WorkersAI.Exports.createWorkersAI (U2.Case1 settings)
                let model = workersai.textEmbedding "@cf/baai/bge-base-en-v1.5"
                let options =
                    V4.EmbeddingModelV4CallOptions.Create [| pair.question; pair.existing |]
                let! result = model.doEmbed options |> Async.AwaitPromise
                let similarity = cosine result.embeddings[0] result.embeddings[1]
                return Workers.Exports.Response.json {| similarity = similarity |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> the Workers AI binding <code>AI</code>.</p></div>

## Help Center Answers

The Worker answers questions from the documents in an [AI Search](https://developers.cloudflare.com/ai-search/) instance. The instance's `chat` model generates each reply from the passages that AI Search retrieves for the question.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Search = FSharp.CloudEdge.Runtime.AISearchProvider
module V3 = FSharp.CloudEdge.Support.AI.V3.Provider

type Env =
    abstract AI_SEARCH: Workers.AiSearchNamespace

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! question = request.text () |> Async.AwaitPromise
                let settings = Search.AISearchNamespaceSettings.Create env.AI_SEARCH
                let search = Search.Exports.createAISearchNamespace settings
                let model = search.get("help-center").chat ()
                let questionPart = V3.LanguageModelV3Message3.Content.Item.Text(question, None)
                let prompt = [| V3.LanguageModelV3Message.User([| questionPart |], None) |]
                let options = V3.LanguageModelV3CallOptions.Create prompt
                let! result = model.doGenerate options |> Async.AwaitPromise
                let answer =
                    result.content
                    |> Array.choose (function
                        | V3.LanguageModelV3Content.Text(text, _) -> Some text
                        | _ -> None)
                    |> String.concat ""
                return Workers.Exports.Response.json {| answer = answer |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> an AI Search namespace binding named <code>AI_SEARCH</code> and an instance called <code>help-center</code>. Every account has a <code>default</code> namespace.</p></div>

AI Search models implement version 3 of the AI SDK contract, so this example uses the `V3` types. The Workers AI and AI Gateway models implement version 4.

## Shop Assistant

The assistant replies to customers with live data from your own API. `createToolsFromOpenAPISpec` builds one tool per operation in an OpenAPI document, and `runWithTools` executes the tool calls that the model returns.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module AIUtils = FSharp.CloudEdge.Runtime.AIUtils

type Env =
    abstract AI: Workers.Ai<Workers.AiModels>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! question = request.text () |> Async.AwaitPromise
                let spec = "https://shop.example.com/openapi.json"
                let! tools = AIUtils.Exports.createToolsFromOpenAPISpec spec |> Async.AwaitPromise
                let messages: obj[] = [| {| role = "user"; content = question |} |]
                let input = AIUtils.RunWithTools.Input.Create(messages, tools)
                let model = "@hf/nousresearch/hermes-2-pro-mistral-7b"
                let! answer =
                    AIUtils.Exports.runWithTools (env.AI, model, input) |> Async.AwaitPromise
                return Workers.Exports.Response.json answer
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> the <code>AI</code> binding, and an OpenAPI spec with an absolute URL in its <code>servers</code> entry. The tools send their requests to that URL.</p></div>

The tool functions run inside the Worker that sends the prompt. Cloudflare documents this as [embedded function calling](https://developers.cloudflare.com/workers-ai/features/function-calling/embedded/).

## Model Fallback

The gateway's `chat` takes two models from different providers, and AI Gateway returns the reply from the first one that succeeds. `unified` turns each `provider/model` string into a model for the gateway's [OpenAI-compatible endpoint](https://developers.cloudflare.com/ai-gateway/usage/chat-completion/).

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Gateway = FSharp.CloudEdge.Runtime.AIGatewayProvider
module Unified = FSharp.CloudEdge.Runtime.AIGatewayProvider.Providers.Unified
module V4 = FSharp.CloudEdge.Support.AI.V4.Provider

type Env =
    abstract ACCOUNT_ID: string
    abstract GATEWAY_TOKEN: string

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let! question = request.text () |> Async.AwaitPromise
                let settings =
                    Gateway.AiGatewayAPISettings.Create(
                        gateway = "support",
                        accountId = env.ACCOUNT_ID,
                        apiKey = env.GATEWAY_TOKEN
                    )
                let gateway = Gateway.Exports.createAiGateway (U2.Case1 settings)
                let models =
                    [| "openai/gpt-5.2"; "workers-ai/@cf/meta/llama-3.3-70b-instruct-fp8-fast" |]
                    |> Array.map Unified.Exports.unified
                let model = gateway.chat (U2.Case1 models)
                let questionPart = V4.LanguageModelV4Message3.Content.Item.Text(question, None)
                let prompt = [| V4.LanguageModelV4Message.User([| questionPart |], None) |]
                let options = V4.LanguageModelV4CallOptions.Create prompt
                let! result = model.doGenerate options |> Async.AwaitPromise
                let answer =
                    result.content
                    |> Array.choose (function
                        | V4.LanguageModelV4Content.Text(text, _) -> Some text
                        | _ -> None)
                    |> String.concat ""
                return Workers.Exports.Response.json {| answer = answer |}
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> an AI Gateway named <code>support</code> that stores the provider keys. The Worker's <code>ACCOUNT_ID</code> and <code>GATEWAY_TOKEN</code> hold the account ID and the gateway's authentication token.</p></div>

AI Gateway is available on every Cloudflare plan, and caching and rate limiting are among its [free core features](https://developers.cloudflare.com/ai-gateway/reference/pricing/). With the `gateway` option of `WorkersAISettings2.Create`, the Workers AI provider sends its requests through a gateway as well.

## Markdown Converter

Upload a PDF or another file, and the Worker returns its content as Markdown in a JSON result. `toMarkdown` is a method of the Worker's `AI` binding, and the upload's name is the last segment of the request URL.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

type Env =
    abstract AI: Workers.Ai<Workers.AiModels>

[<ExportDefault>]
let worker: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let name = request.url.Split('/') |> Array.last
                let! file = request.blob () |> Async.AwaitPromise
                let upload = Workers.MarkdownDocument.Create(name, file)
                let! converted = env.AI.toMarkdown upload |> Async.AwaitPromise
                return Workers.Exports.Response.json converted
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<div class="ce-needs"><p><strong>Needs</strong> the Workers AI binding <code>AI</code>.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { last } from "./fable_modules/fable-library-js.5.13.0/Array.js";
import { split } from "./fable_modules/fable-library-js.5.13.0/String.js";

export const worker = {
    fetch: (request, env, _arg) => startAsPromise(singleton.Delay(() => {
        const name = last(split(request.url, ["/"], undefined, 0));
        return singleton.Bind(awaitPromise(request.blob()), (_arg_1) => {
            const upload = {
                name: name,
                blob: _arg_1,
            };
            return singleton.Bind(awaitPromise(env.AI.toMarkdown(upload)), (_arg_2) => singleton.Return(globalThis.Response.json(_arg_2)));
        });
    })),
};

export default worker;
```

</details>

[Markdown conversion](https://developers.cloudflare.com/workers-ai/features/markdown-conversion/) is free for most formats. For images, the service can run Workers AI models, and those runs count toward the daily Neuron allocation. `env.AI.toMarkdown().supported()` lists the formats the service accepts.

## Chat History Upgrade

Conversation persistence, history migration, and the F# `AIChatAgent` binding boundary have their own treatment on the [Chat agents page](agents/chat.md#chat-history-upgrade).

## Library Table

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.WorkersAIProvider` | `workers-ai-provider` 4.0.0 | Workers AI models |
| `Runtime.AIGatewayProvider` | `ai-gateway-provider` 4.0.0 | Routing through AI Gateway |
| `Runtime.AISearchProvider` | `ai-search-provider` 0.1.1 | AI Search as a chat model |
| `Runtime.AIUtils` | `@cloudflare/ai-utils` 1.0.1 | Tool calling |
| `Runtime.AIChat` | `@cloudflare/ai-chat` 0.11.0 | Chat agent and history upgrade |
| `Runtime.Think` | `@cloudflare/think` 0.17.0 | Experimental chat agent |
| `Runtime.Workers` | `@cloudflare/workers-types` 5.20260906.1 | The `Ai` binding |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/support/"><strong>Support Libraries</strong><span>The AI SDK types</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/agents/"><strong>Agents</strong><span>Stateful agents and MCP</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/"><strong>Workers</strong><span>Handlers and bindings</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Workers and their bindings</span></a>
</div>

## NuGet packages

[Runtime.AIChat 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.AIChat/0.1.0), [Runtime.AIGatewayProvider 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.AIGatewayProvider/0.1.0), [Runtime.AISearchProvider 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.AISearchProvider/0.1.0), [Runtime.AIUtils 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.AIUtils/0.1.0), [Runtime.Think 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Think/0.1.0), [Runtime.Workers 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Workers/0.1.0), [Runtime.WorkersAIProvider 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.WorkersAIProvider/0.1.0), [Support.AI.V3.Provider 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Support.AI.V3.Provider/0.1.0), [Support.AI.V4.Provider 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Support.AI.V4.Provider/0.1.0).

See [installation and release availability](../guide/packages.md).
