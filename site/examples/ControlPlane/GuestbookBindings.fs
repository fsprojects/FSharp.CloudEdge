module GuestbookBindings

open System.Text.Json
open System.Text.Json.Nodes

let metadata (databaseId: string) (namespaceId: string) =
    let bindings: JsonNode[] =
        [| JsonSerializer.SerializeToNode {| ``type`` = "d1"; name = "DB"; database_id = databaseId |}
           JsonSerializer.SerializeToNode {| ``type`` = "kv_namespace"; name = "SESSIONS"; namespace_id = namespaceId |}
           JsonSerializer.SerializeToNode {| ``type`` = "r2_bucket"; name = "PHOTOS"; bucket_name = "photos" |}
           JsonSerializer.SerializeToNode {| ``type`` = "queue"; name = "ORDERS"; queue_name = "orders" |}
           JsonSerializer.SerializeToNode {| ``type`` = "plain_text"; name = "SITE_TITLE"; text = "Guestbook" |} |]
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           bindings = bindings |}
    JsonSerializer.SerializeToNode(settings).AsObject()
