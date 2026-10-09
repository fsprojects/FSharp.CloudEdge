module FSharp.CloudEdge.Pi.SessionSubmit

open Fable.Core

type InputItemsItemImage = FSharp.CloudEdge.Pi.HarnessSubmit.InputItemsItemImage
type InputItemsItemTextTextSignature = FSharp.CloudEdge.Pi.HarnessSubmit.InputItemsItemTextTextSignature
type InputItemsItemText = FSharp.CloudEdge.Pi.HarnessSubmit.InputItemsItemText
type InputItemsItem = FSharp.CloudEdge.Pi.HarnessSubmit.InputItemsItem
type Input = FSharp.CloudEdge.Pi.HarnessSubmit.Input

[<Emit("typeof $0 === 'string'")>]
let private isString (_: string) : bool = jsNative

let private stringValue (value: string) : obj =
    if isString value then box value
    else invalidArg "value" "The operation requires a JavaScript string"

[<Emit("undefined")>]
let private undefinedValue () : obj = jsNative

[<Emit("({})")>]
let private newObject () : obj = jsNative

[<Emit("Object.defineProperty($0, $1, {value: $2, enumerable: true, writable: true, configurable: true})")>]
let private setField (_: obj) (_: string) (_: obj) : unit = jsNative

[<Emit("({[$0]: $1})")>]
let private fieldArgument (_: string) (_: obj) : obj = jsNative

let private encodeInputItemsItemImage (value: InputItemsItemImage) : obj =
    let result = newObject ()
    setField result "data" (stringValue value.Data)
    setField result "mimeType" (stringValue value.MimeType)
    setField result "type" (box "image")
    result

let private encodeInputItemsItemTextTextSignature (value: InputItemsItemTextTextSignature) : obj =
    match value with
    | InputItemsItemTextTextSignature.Text value -> stringValue value
    | InputItemsItemTextTextSignature.Undefined -> undefinedValue ()

let private encodeInputItemsItemText (value: InputItemsItemText) : obj =
    let result = newObject ()
    setField result "text" (stringValue value.Text)
    match value.TextSignature with
    | None -> ()
    | Some field -> setField result "textSignature" (encodeInputItemsItemTextTextSignature field)
    setField result "type" (box "text")
    result

let private encodeInputItemsItem (value: InputItemsItem) : obj =
    match value with
    | InputItemsItem.Image value -> encodeInputItemsItemImage value
    | InputItemsItem.Text value -> encodeInputItemsItemText value

let private encodeInput (value: Input) : obj =
    match value with
    | Input.Items value -> (fun values -> values |> Array.map encodeInputItemsItem |> box) value
    | Input.Text value -> stringValue value

let submit (receiver: FSharp.CloudEdge.Runtime.AgentsPi.Harness.Pi.PiSession) (value: Input) argument1 =
    receiver.submit(unbox (encodeInput value), ?options = argument1)
