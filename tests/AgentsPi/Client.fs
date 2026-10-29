module CloudEdgePiClient

open System
open Fable.Core
open Fable.Core.JsInterop
open Fickl.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Lifecycle = FSharp.CloudEdge.Runtime.Agents.Lifecycle
module Pi = FSharp.CloudEdge.Runtime.AgentsPi.Harness.Pi
module PiAi = FSharp.CloudEdge.Runtime.PiAi
module PiDurable = FSharp.CloudEdge.Runtime.PiDurable
module Transport = FSharp.CloudEdge.Support.Workers.DurableObjects
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

type ProbeEnv =
    abstract PiProbe: Workers.DurableObjectNamespace<obj>

type private WireInput = U2<string, obj[]>

let private items = function
    | U2.Case2 values -> values
    | U2.Case1 _ -> invalidOp "Expected encoded item input"

[<AttachMembers>]
type PiProbe(state: Workers.DurableObjectState<obj>, env: ProbeEnv) as this =
    inherit Runtime.DurableObject<ProbeEnv, obj>(state, env)

    let installedLifecycle = Lifecycle.Lifecycle<ProbeEnv, obj>.install<ProbeEnv, obj>(this)
    let mutable invocationContext = ""
    let faux =
        PiAi.Exports.fauxProvider(
            PiAi.RegisterFauxProviderOptions.Create(
                api = "cloudedge-fixture",
                models = [| PiAi.FauxModelDefinition.Create("fixture", input = [| PiAi.ModelInputModality.Text; PiAi.ModelInputModality.Image |]) |]))
    let models = PiAi.Exports.createModels()

    do
        faux.setResponses(Array.init 20 (fun _ -> U2.Case1(PiAi.Exports.fauxAssistantMessage(U5.Case1 "fixture reply"))))
        models.setProvider(faux.provider)

    let pi =
        createHarness
            (fun context ->
                invocationContext <- contextName context
                let registry = PiDurable.Exports.createRegistry()
                let options = PiDurable.HarnessOptions.Create(U2.Case1 models, registry, onWake = context.onWake)
                PiDurable.Exports.Harness.``open``(context.storage, options, context.context) |> U2.Case2)
            (faux.getModel().provider)
            (faux.getModel().id)

    do installedLifecycle.``use``(unbox<Lifecycle.DurableObjectCapability<obj>> pi) |> ignore

    member _.lifecycle = installedLifecycle

    member _.onRequest(request: Transport.NativeRequest) : JS.Promise<Workers.Response> = jsAsync {
        let input = structuredInput "visual question"
        let url = Workers.Exports.URL(U2.Case1 request.url)
        if url.searchParams.get("action") = Some "inspect" then
            let! duplicate = submitInput pi input "shared-submit"
            let! sessionDuplicate = submitSessionInput (pi.session()) input "session-submit"
            let! messages = pi.messages()
            return Workers.Exports.Response.json {| duplicate = duplicate; sessionDuplicate = sessionDuplicate; messages = messages |}
        else
            let captured = ResizeArray<WireInput>()
            let original: Func<WireInput, Pi.PiSubmitOptions option, JS.Promise<Pi.PiReceipt>> = pi?submit?bind(pi)
            pi?submit <- Func<WireInput, Pi.PiSubmitOptions option, JS.Promise<Pi.PiReceipt>>(fun value options ->
                captured.Add value
                original.Invoke(value, options))
            try
                let mutable nullRejected = false
                try
                    let! _ = submitText pi null "invalid-null"
                    ()
                with _ -> nullRejected <- captured.Count = 0
                let! text = submitText pi "plain text" "text-submit"
                let! _ = pi.wait(text.operationId)
                let! receipt = submitInput pi input "shared-submit"
                let! duplicate = submitInput pi input "shared-submit"
                let! _ = pi.wait(receipt.operationId)
                let! response = promptInput pi input "harness-prompt"
                let session = pi.session()
                let! sessionReceipt = submitSessionInput session input "session-submit"
                let! sessionDuplicate = submitSessionInput session input "session-submit"
                let! _ = session.wait(sessionReceipt.operationId)
                let! sessionResponse = promptSessionInput session input "session-prompt"
                let! undefinedReceipt = submitInput pi (explicitUndefinedInput "explicit undefined") "undefined-submit"
                let! _ = pi.wait(undefinedReceipt.operationId)
                let! messages = pi.messages()
                let plain =
                    match captured[0] with
                    | U2.Case1 value -> value
                    | U2.Case2 _ -> invalidOp "Expected encoded plain text"
                let image = items captured[1]
                let explicit = (items captured[captured.Count - 1])[0]
                let ownSignature value = JS.Constructors.Object.getOwnPropertyNames(value) |> Seq.contains "textSignature"
                return Workers.Exports.Response.json
                    {| text = text; receipt = receipt; duplicate = duplicate; response = response
                       sessionReceipt = sessionReceipt; sessionDuplicate = sessionDuplicate; sessionResponse = sessionResponse
                       context = invocationContext; messages = messages
                       wire = {| nullRejected = nullRejected; plain = plain; image = image
                                 omittedSignature = not (ownSignature image[0])
                                 ownUndefinedSignature = ownSignature explicit && JS.Constructors.Object.is(explicit?textSignature, JS.undefined<obj>) |} |}
            finally
                pi?submit <- original
    }

[<ExportDefault>]
let worker =
    {| fetch = Func<Transport.NativeRequest, ProbeEnv, Workers.ExecutionContext<obj>, JS.Promise<Workers.Response>>(fun request env _ -> jsAsync {
        let url = Workers.Exports.URL(U2.Case1 request.url)
        if url.pathname = "/agents/pi-probe/validation" then
            return! (Transport.getFetchByName env.PiProbe "validation").fetch request
        else
            return Workers.Exports.Response.json({| error = "missing" |}, U2.Case2(Workers.ResponseInit.Create(status = 404.)))
    }) |}
