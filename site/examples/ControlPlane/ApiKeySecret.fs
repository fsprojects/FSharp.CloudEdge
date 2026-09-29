module ApiKeySecret

open System
open System.Text.Json
open System.Text.Json.Nodes

let firstUpload () =
    let apiKey = Environment.GetEnvironmentVariable "PAYMENTS_API_KEY"
    let secret: JsonNode =
        JsonSerializer.SerializeToNode {| ``type`` = "secret_text"; name = "PAYMENTS_API_KEY"; text = apiKey |}
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           bindings = [| secret |] |}
    JsonSerializer.SerializeToNode(settings).AsObject()

let laterUpload () =
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           keep_bindings = [| "secret_text" |] |}
    JsonSerializer.SerializeToNode(settings).AsObject()
