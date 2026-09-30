---
title: Start Here
description: From an empty folder to a Worker on Cloudflare, in F#.
order: 1
---

<div class="ce-block-head">
<p class="ce-block-lead">At the end of this path you have a Worker written in F#, running on Cloudflare's free plan. Set up the account and tools on this page, then follow the package-based path below.</p>
<ul class="ce-facts">
<li><span>You need</span> A Cloudflare account, .NET SDK 10.0.401, Node.js with npm, curl</li>
<li><span>You get</span> <code>hello-worker</code>, a Worker on Cloudflare</li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/workers/platform/pricing/">100,000 requests a day</a></li>
</ul>
</div>

## The Path

You write the Worker and its deployment in F#. Fable compiles the Worker to JavaScript, and an F# program uploads it to Cloudflare through the REST API. Your application restores the versioned libraries from NuGet. Source builds are a separate contributor workflow.

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/credentials/"><strong>Credentials</strong><span>Account ID and API token in .env</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/packages/"><strong>Packages</strong><span>Install the 0.1.0 libraries</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/first-worker/"><strong>First Worker</strong><span>Worker.fs, compiled and run locally</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/first-deploy/"><strong>First Deploy</strong><span>hello-worker, uploaded from F#</span></a>
</div>

## Account and Toolchain

- **A Cloudflare account.** You [sign up](https://developers.cloudflare.com/fundamentals/account/create-account/) with an email address and a password. The free plan is enough for this path. It includes 100,000 Worker requests a day and 10 ms of CPU time per invocation.
- **The .NET SDK 10.0.401** or a later 10.0.4xx patch, as the repository's `global.json` requires. [Download .NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).
- **Node.js with npm.** The repository's build scripts run on Node, and on First Worker you install esbuild and workerd with npm. [Node.js installers](https://nodejs.org/en/download).
- **curl**, to send requests to your Worker from a terminal. [curl packages](https://curl.se/download.html).

Check your versions from a terminal.

```bash
dotnet --version
node --version
npm --version
```

On the machine used to write these pages, the output is:

```text
10.0.401
v25.1.0
11.6.2
```

Building the libraries themselves requires additional tools and sibling repositories. See [Local Build](local-build.md) when contributing to the bindings.

## Next Step

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/credentials/"><strong>Credentials</strong><span>Your account ID and API token</span></a>
</div>
