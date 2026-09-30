---
title: Chat agents
description: Maintain conversation history, stream replies, and coordinate model and tool calls.
---

<div class="ce-block-head">
<p class="ce-block-lead">Maintain conversation history, stream replies, and coordinate model and tool calls.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.AIChat</code> <code>Runtime.Think</code></li>
<li><span>npm</span> <code>@cloudflare/ai-chat 0.11.0</code></li>
<li><span>Upstream/API</span> Think experimental; see the F# binding boundary below</li>
</ul>
</div>

## Conversation and execution

The chat layer owns the conversation, message persistence, and streaming response lifecycle. A [model provider](../ai.md) supplies inference; [MCP](mcp.md) and [Code Mode](code-mode.md) supply ways to use tools. Files and repository history can live in their own workspaces and Artifacts repositories.

The upstream [AI Chat guide](https://github.com/cloudflare/agents/tree/main/packages/ai-chat) describes resumable streams, persistent messages, tool approval, and recovery. Configure conversation identity and history retention separately from the execution environment used by a tool call.

## F# binding boundary

The current bindings describe `AIChatAgent` and `Think` as F# interfaces. They cannot directly be used as imported base classes for an F# class. Exported helper functions are available from F#, including the history migration below. The [Agents SDK page](sdk.md) demonstrates the supported F# Durable Object lifecycle pattern.

## Chat History Upgrade

`autoTransformMessages` converts a conversation saved in the AI SDK v4 message format to v5 `UIMessage` objects. Apps built on v5 can then read chat history stored before the upgrade.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Migration = FSharp.CloudEdge.Runtime.AIChat.AiChatV5Migration

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request _ _ ->
            async {
                let! stored = request.json<obj[]> () |> Async.AwaitPromise
                let upgraded = Migration.Exports.autoTransformMessages stored
                return Workers.Exports.Response.json upgraded
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { awaitPromise, startAsPromise } from "./fable_modules/fable-library-js.5.13.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.5.13.0/AsyncBuilder.js";
import { autoTransformMessages } from "@cloudflare/ai-chat/ai-chat-v5-migration";

export const worker = {
    fetch: (request, _arg, _arg_1) => startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(request.json()), (_arg_2) => {
        const upgraded = autoTransformMessages(_arg_2);
        return singleton.Return(globalThis.Response.json(upgraded));
    }))),
};

export default worker;
```

</details>

## History migrations

Treat persisted conversation history as application data with a version. When upgrading the message representation, test the conversion against existing conversations before changing the code that reads them. The example demonstrates the v4-to-v5 conversion helper; it is not a complete chat agent or a promise that arbitrary later message formats need no migration.

## Related Pages

- [Agents SDK](sdk.md)
- [Model providers](../ai.md)
- [MCP connections](mcp.md)
- [Voice](voice.md)

## Help verify these bindings

The examples on this page are checked against F# source projects. Compilation and emitted JavaScript checks do not establish hosted service behavior. Useful targets for community verification include the imported-class binding gap, helper compatibility with stored messages, and chat lifecycle behavior through any adapter used.

See [Verify bindings](../../guide/verify-bindings.md) for the existing evidence, reproducible checks, and the [binding issue form](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include the pinned package version and the specific behavior exercised; successful reproductions are useful evidence too.

## NuGet packages

[Runtime.AIChat 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.AIChat/0.1.0), [Runtime.Think 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Think/0.1.0), [Runtime.Workers 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Workers/0.1.0).

See [installation and release availability](../../guide/packages.md).
