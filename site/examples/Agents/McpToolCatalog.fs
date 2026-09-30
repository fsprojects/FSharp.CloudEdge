module McpToolCatalog

open Fable.Core

module Mcp = FSharp.CloudEdge.Runtime.AgentsMcpClient.Mcp.Client

let availableTools (manager: Mcp.MCPClientManager) =
    async {
        let wait = Mcp.MCPClientManager.WaitForConnections.Options.Create(timeout = 5000.)
        do! manager.waitForConnections wait |> Async.AwaitPromise
        return manager.listTools () |> Array.map (fun tool -> tool.name)
    }
    |> Async.StartAsPromise
