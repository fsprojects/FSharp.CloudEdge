module NuGetRelease.Program

open System
open System.IO
open NuGetRelease.Common

let rec takeOption name args =
    match args with
    | option :: value :: rest when option = name -> Some value, rest
    | [option] when option = name -> failwith $"{name} needs a value"
    | first :: rest ->
        let value, remaining = takeOption name rest
        value, first :: remaining
    | [] -> None, []

[<EntryPoint>]
let main args =
    try
        match args |> Array.toList with
        | ["plan"] -> Packages.inventory () |> fun packages -> printfn "%s" (System.Text.Json.JsonSerializer.Serialize(packages, jsonOptions))
        | "pack" :: options ->
            let count, unknown = takeOption "--jobs" options
            ensure (List.isEmpty unknown) "Usage: pack [--jobs N]"
            let jobs = count |> Option.map Int32.Parse |> Option.defaultValue (max 1 (min 2 Environment.ProcessorCount))
            Packages.inventory () |> Packages.pack jobs
        | ["verify"] -> Packages.inventory () |> Packages.verify
        | "availability" :: options ->
            let duration, unknown = takeOption "--wait-minutes" options
            ensure (List.isEmpty unknown) "Usage: availability [--wait-minutes N]"
            let minutes = duration |> Option.map Int32.Parse |> Option.defaultValue 0
            ensure (minutes >= 0) "The availability wait cannot be negative"
            Packages.inventory () |> Packages.availability minutes
        | ["audit"] -> Packages.inventory () |> Consumers.audit |> ignore
        | ["check"] -> Checks.runChecks ()
        | ["support"] -> Consumers.checkSupport ()
        | ["publish"; "--confirm"; version] -> Packages.publish (Packages.inventory ()) version
        | "examples" :: options ->
            ensure (options |> List.forall (fun x -> x = "--public" || x = "--fable")) "Usage: examples [--public] [--fable]"
            let projects = Packages.inventory () |> Consumers.audit
            Consumers.buildExamples projects (List.contains "--public" options) (List.contains "--fable" options)
        | ["snippets"] -> Consumers.checkSnippets None
        | ["snippets"; "--js-root"; path] -> Consumers.checkSnippets (Some(Path.GetFullPath path))
        | _ -> failwith "Commands: plan | pack [--jobs N] | verify | availability [--wait-minutes N] | audit | check | support | examples [--public] [--fable] | snippets [--js-root PATH] | publish --confirm VERSION"
        0
    with ex ->
        eprintfn "%s" ex.Message
        1
