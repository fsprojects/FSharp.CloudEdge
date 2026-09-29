module SupportPackageSmoke

open Fable.Core.JS

type Settings = { Count: int }

let typedKeyValue () =
    let key: typekeyof<Settings, int> = unbox "Count"
    key.Value

let keyValue () =
    let key: keyof<Settings> = unbox "Count"
    key.Value

let readonlyIndexer () =
    let values: ReadonlyRecord<string, int> = unbox {| count = 42 |}
    values.Item "count"

let mutableIndexer () =
    let values: Fable.Core.JS.JS.Record<string, int> = unbox {| count = 0 |}
    values.Item "count" <- 42
    values.Item "count"
