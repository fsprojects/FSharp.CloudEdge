module ChatRoomWorker

open System.Text.Json
open System.Text.Json.Nodes

let metadata () =
    let rooms: JsonNode =
        JsonSerializer.SerializeToNode {| ``type`` = "durable_object_namespace"; name = "ROOMS"; class_name = "ChatRoom" |}
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           bindings = [| rooms |]
           migrations = {| new_tag = "v1"; new_sqlite_classes = [| "ChatRoom" |] |} |}
    JsonSerializer.SerializeToNode(settings).AsObject()
