module FicklRpcProbe

open Fable.Core
open Fable.Core.JsInterop
open Fickl.Core

[<Interface>]
type TextPart =
    abstract ``type``: string
    abstract text: string
    [<Emit("({ type: 'text', text: $0 })")>]
    static member Create (text: string) : TextPart = jsNative

[<RequireQualifiedAccess; TypeScriptTaggedUnion("type")>]
type Input =
    | [<InlineInterface>] Text of TextPart

let createTextPart text = TextPart.Create text
let createTaggedText payload = Input.Text payload
