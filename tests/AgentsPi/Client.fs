module CloudEdgePiClient

open Fable.Core

module Pi = FSharp.CloudEdge.Runtime.AgentsPi.Harness.Pi
module Submit = FSharp.CloudEdge.Pi.HarnessSubmit
module Prompt = FSharp.CloudEdge.Pi.HarnessPrompt
module SessionSubmit = FSharp.CloudEdge.Pi.SessionSubmit
module SessionPrompt = FSharp.CloudEdge.Pi.SessionPrompt

let createHarness (factory: Pi.PiHarnessFactory) provider modelId : Pi.PiHarness =
    let model = Pi.PiModel.Create(provider = provider, id = modelId)
    let defaults = Pi.PiSessionDefaults.Create(model = model)
    Pi.Exports.PiHarness(Pi.PiHarnessOptions.Create(factory, defaults = defaults))

let contextName (context: Pi.PiHarnessContext) : string = context.context.toString()

let structuredInput text : Submit.Input =
    Submit.Input.Items [|
        Submit.InputItemsItem.Text { Text = text; TextSignature = None }
        Submit.InputItemsItem.Image { Data = "aGVsbG8="; MimeType = "image/png" }
    |]

let explicitUndefinedInput text : Submit.Input =
    Submit.Input.Items [|
        Submit.InputItemsItem.Text { Text = text; TextSignature = Some Submit.InputItemsItemTextTextSignature.Undefined }
    |]

let submitText harness text operationId : JS.Promise<Pi.PiReceipt> =
    Submit.submit harness (Submit.Input.Text text) (Some(Pi.PiSubmitOptions.Create(operationId = operationId)))

let submitInput harness (input: Submit.Input) operationId : JS.Promise<Pi.PiReceipt> =
    Submit.submit harness input (Some(Pi.PiSubmitOptions.Create(operationId = operationId)))

let promptInput harness (input: Submit.Input) operationId : JS.Promise<Pi.PiPromptResponse> =
    Prompt.prompt harness input (Some(Pi.PiSubmitOptions.Create(operationId = operationId)))

let submitSessionInput session (input: Submit.Input) operationId : JS.Promise<Pi.PiReceipt> =
    SessionSubmit.submit session input (Some(Pi.PiSubmitOptions.Create(operationId = operationId)))

let promptSessionInput session (input: Submit.Input) operationId : JS.Promise<Pi.PiPromptResponse> =
    SessionPrompt.prompt session input (Some(Pi.PiSubmitOptions.Create(operationId = operationId)))
