module SandboxApi

open Fable.Core
open Fable.Core.JsInterop

module Workers = FSharp.CloudEdge.Runtime.Workers
module Bridge = FSharp.CloudEdge.Runtime.SandboxBridge

// The package exports getSandbox; its binding is pending regeneration.
[<Import("getSandbox", "@cloudflare/sandbox")>]
let getSandbox (binding: Workers.DurableObjectNamespace<obj>) (id: string) : Bridge.ISandbox = jsNative
