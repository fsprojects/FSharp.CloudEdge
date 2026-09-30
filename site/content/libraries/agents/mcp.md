---
title: MCP connections and tools
description: Connect an agent to remote tools and expose application capabilities through MCP.
---

<div class="ce-block-head">
<p class="ce-block-lead">Connect an agent to remote tools and expose application capabilities through MCP.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.Agents</code> <code>Runtime.AgentsAiChatAgent</code> <code>Runtime.AgentsMcpClient</code></li>
<li><span>npm</span> <code>agents 0.22.0</code></li>
<li><span>Upstream/API</span> Uses the pinned Agents SDK interfaces</li>
</ul>
</div>

## Client and server roles

An MCP client discovers and calls tools offered by another service. An MCP server publishes tools, resources, and prompts for other clients. Decide which role your application needs before configuring a connection.

The SDK binds server capabilities through its agent modules and client management through `Runtime.AgentsMcpClient.Mcp.Client`. Cloudflare's [MCP documentation](https://developers.cloudflare.com/agents/model-context-protocol/) covers the supported integration patterns.

## Connection lifecycle

For new client integrations, use `registerServer` followed by `connectToServer`. Registration and establishing a live connection are separate operations. Authentication can return a continuation that the user must complete before discovery succeeds.

On recovery, restore the configured connections and wait for discovery before constructing the model's tool catalog. Use `listServers` to inspect server state and `listTools` or `getAITools` to obtain the currently available tools.

## Read the discovered catalog

This helper is called after the manager has registered and started connecting to its servers. It waits up to five seconds for pending connection and discovery work, then returns the names currently available. Waiting does not make a failed connection successful; check individual server states when a required tool is missing.

```fsharp
open Fable.Core

module Mcp = FSharp.CloudEdge.Runtime.AgentsMcpClient.Mcp.Client

let availableTools (manager: Mcp.MCPClientManager) =
    async {
        let wait = Mcp.MCPClientManager.WaitForConnections.Options.Create(timeout = 5000.)
        do! manager.waitForConnections wait |> Async.AwaitPromise
        return manager.listTools () |> Array.map (fun tool -> tool.name)
    }
    |> Async.StartAsPromise
```

## Match tool access to the task

Choose which connected servers and tools are available to a particular agent or user. Keep OAuth callbacks and connection state with the owning application identity. A discovered tool can still require authorization or fail at execution time, so handle tool errors separately from discovery.

Close connections that are no longer needed. For persistent agents, use the SDK's restoration lifecycle rather than assuming an in-memory network connection survives hibernation.

## Related Pages

- [Agents SDK lifecycle](sdk.md)
- [Code Mode orchestration](code-mode.md)
- [Chat agents](chat.md)

## Testing and feedback

Useful test cases include connection and OAuth states, discovery after recovery, namespaced tool shapes, and tool error decoding.

[Verify bindings](../../guide/verify-bindings.md) explains how to run checks and [report an issue](https://github.com/fsprojects/FSharp.CloudEdge/issues/new?template=binding-report.yml). Include a small reproduction and the package versions used; working examples are welcome too.

## NuGet packages

[Runtime.Agents 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Agents/0.1.0), [Runtime.AgentsAiChatAgent 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.AgentsAiChatAgent/0.1.0), [Runtime.AgentsMcpClient 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.AgentsMcpClient/0.1.0).

See [installation and release availability](../../guide/packages.md).
