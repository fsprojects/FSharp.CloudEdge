module CloudEdgePiComposition

open Fable.Core

module Raw = FSharp.CloudEdge.Runtime.AgentsPi.Harness.Pi
module Input = FSharp.CloudEdge.Pi.HarnessSubmit
module HarnessPrompt = FSharp.CloudEdge.Pi.HarnessPrompt
module SessionSubmit = FSharp.CloudEdge.Pi.SessionSubmit
module SessionPrompt = FSharp.CloudEdge.Pi.SessionPrompt

let imageAndText text data mimeType : Input.Input =
    Input.Input.Items [|
        Input.InputItemsItem.Text { Text = text; TextSignature = None }
        Input.InputItemsItem.Image { Data = data; MimeType = mimeType }
    |]

let invocationContext (factory: Raw.PiHarnessContext) : FSharp.CloudEdge.Runtime.Chord.Context = factory.context

let submit harness input options : JS.Promise<Raw.PiReceipt> = Input.submit harness input options
let prompt harness (input: Input.Input) options : JS.Promise<Raw.PiPromptResponse> = HarnessPrompt.prompt harness input options
let submitSession session (input: Input.Input) options : JS.Promise<Raw.PiReceipt> = SessionSubmit.submit session input options
let promptSession session (input: Input.Input) options : JS.Promise<Raw.PiPromptResponse> = SessionPrompt.prompt session input options
