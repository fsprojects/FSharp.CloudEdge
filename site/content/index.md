---
title: FSharp.CloudEdge
description: Build web products on Cloudflare with F#.
layout: splash
---

<section class="ce-hero">
<div class="ce-hero__copy">
<span class="ce-chip"><b>0.1.0</b> MIT licensed</span>
<h1 class="ce-wordmark">FSharp<span class="ce-dot">.</span>CloudEdge</h1>
<p class="ce-line">Build intelligent products on Cloudflare with F#.</p>
<p class="ce-why">F# developers deserve a first-class experience for building on Cloudflare. And Cloudflare gets a first-class functional language that can honor its contracts and make the Fable Compiler's original promise of “<a href="https://fable.io">JavaScript you can be proud of</a>” a new level of integrity.</p>
<p class="ce-sub">Cloudflare runs your code <i>at cloud's edge,</i> close to your users, and provides modern, responsive agentic tools. You can start with a free plan that covers 100,000 Worker requests per day. FSharp.CloudEdge makes the full array of Cloudflare products and services available in F#, from dynamic web pages to agentic AI capabilities.</p>
<div class="ce-actions">
<a class="ce-btn ce-btn--primary" href="#building-blocks">See what you can build <svg aria-hidden="true" viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 5v14"/><path d="m19 12-7 7-7-7"/></svg></a>
<a class="ce-btn ce-btn--secondary" href="/FSharp.CloudEdge/libraries/">Library line-up</a>
<a class="ce-btn ce-btn--secondary" href="https://www.nuget.org/packages?q=FSharp.CloudEdge">Browse packages on NuGet</a>
</div>
</div>
<figure class="ce-code" aria-label="A Durable Object that counts requests, written in F#">
<div class="ce-code__bar"><span>Counter.fs</span><span>Durable Object</span></div>
<pre><code><span class="k">module</span> Counter
<span></span>
<span class="k">open</span> Fable.Core
<span></span>
<span class="k">module</span> Workers = FSharp.CloudEdge.Runtime.Workers
<span class="k">module</span> Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
<span></span>
<span class="k">type</span> Counter(ctx: Workers.DurableObjectState&lt;<span class="t">obj</span>&gt;, env: <span class="t">obj</span>) =
    <span class="k">inherit</span> Runtime.DurableObject&lt;<span class="t">obj</span>, <span class="t">obj</span>&gt;(ctx, env)
<span></span>
    <span class="k">interface</span> Runtime.DurableObject.IFetchHandler <span class="k">with</span>
        <span class="k">member</span> _.fetch _ =
            <span class="k">async</span> {
                <span class="k">let!</span> current = ctx.storage.get&lt;<span class="t">float</span>&gt;(<span class="s">"count"</span>) |&gt; Async.AwaitPromise
                <span class="k">let</span> count = Option.defaultValue <span class="n">0.</span> current + <span class="n">1.</span>
                <span class="k">do!</span> ctx.storage.put(<span class="s">"count"</span>, count) |&gt; Async.AwaitPromise
                <span class="k">return</span> Workers.Exports.Response.json {| count = count |}
            }
            |&gt; Async.StartAsPromise
            |&gt; U2.Case1</code></pre>
<figcaption>Compiles against FSharp.CloudEdge 0.1.0.</figcaption>
</figure>
</section>

## Building Blocks

Each block is a Cloudflare service with F# bindings in this library, with the free-plan allowance where Cloudflare offers one.

<div class="ce-grid">
<div class="ce-card"><h3><a href="/FSharp.CloudEdge/libraries/platform/durable-objects/">Real-time rooms</a></h3><p>Chat rooms and live dashboards, each room a Durable Object with its own storage and WebSockets.</p><div class="ce-card__libs"><code>Runtime.Workers</code></div><span class="ce-free">Free: 100,000 requests a day</span></div>
<div class="ce-card"><h3><a href="/FSharp.CloudEdge/libraries/ai/">AI features</a></h3><p>Summaries, chat and embeddings from Workers AI models, or from other providers through AI Gateway.</p><div class="ce-card__libs"><code>Runtime.WorkersAIProvider</code> <code>Runtime.AIGatewayProvider</code></div><span class="ce-free">Free: 10,000 Neurons a day</span></div>
<div class="ce-card"><h3><a href="/FSharp.CloudEdge/libraries/platform/storage/">Data and files</a></h3><p>Key-value data in KV, SQL in D1 and files in R2.</p><div class="ce-card__libs"><code>Runtime.Workers</code></div><span class="ce-free">Free: 5 GB in D1, 10 GB in R2</span></div>
<div class="ce-card"><h3><a href="/FSharp.CloudEdge/libraries/agents/">Agents</a></h3><p>Stateful agents with tools, schedules and MCP servers.</p><div class="ce-card__libs"><code>Runtime.Agents</code> <code>Runtime.Think</code></div></div>
<div class="ce-card"><h3><a href="/FSharp.CloudEdge/libraries/platform/background-work/">Background work</a></h3><p>Queues and Workflows for jobs that run after the response goes out.</p><div class="ce-card__libs"><code>Runtime.Workers</code></div><span class="ce-free">Free: 100,000 Workflow requests a day</span></div>
<div class="ce-card"><h3><a href="/FSharp.CloudEdge/libraries/compute/">Sandboxes and containers</a></h3><p>Isolated sandboxes for untrusted code, and Linux containers beside your Worker.</p><div class="ce-card__libs"><code>Runtime.Sandbox</code> <code>Runtime.Containers</code></div></div>
<div class="ce-card"><h3><a href="/FSharp.CloudEdge/libraries/control-plane/account-setup/">Account setup</a></h3><p>Create D1 databases and R2 buckets and upload Workers from an F# program, through Cloudflare's REST API.</p><div class="ce-card__libs"><code>Management.Storage</code> <code>Management.Compute</code></div></div>
</div>

<div class="ce-credits">
<p>The runtime bindings are generated by <a href="https://github.com/shayanhabibi/Xantham">Xantham</a> from Cloudflare's own TypeScript declarations, and the account-setup clients by <a href="https://github.com/Zaid-Ajaj/Hawaii">Hawaii</a> from Cloudflare's OpenAPI document. <a href="https://github.com/fsprojects/FSharp.CloudEdge#upstream-foundations">How the library is made</a>.</p>
</div>
