module UpstreamSupportHelpers

open Fable.Core.JS

type Settings = { Count: int }

[<Measure>]
type Identifier

let propertyName () =
    TypeKeyOf.create (fun (settings: Settings) -> settings.Count)
    |> TypeKeyOf.value

let propertyValue () =
    let key = TypeKeyOf.create (fun (settings: Settings) -> settings.Count)
    TypeKeyOf.item key { Count = 42 }

let optionalValue () =
    let key = TypeKeyOf.create (fun (settings: Settings) -> settings.Count) |> TypeKeyOf.box
    KeyOf.item key { Count = 42 } |> Option.map unbox<int>

let brandRoundTrip (value: string) = value |> Brand.tagString<Identifier> |> Brand.untagString
