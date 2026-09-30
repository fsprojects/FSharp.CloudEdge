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

let takeSupportFeed args =
    let value, remaining = takeOption "--support-feed" args
    let path = value |> Option.map (fun path ->
        let full = Path.GetFullPath(path: string)
        ensure (Directory.Exists full) $"Support feed does not exist: {full}"
        full)
    path, remaining

[<EntryPoint>]
let main args =
    try
        match args |> Array.toList with
        | ["plan"] -> Packages.inventory () |> fun packages -> printfn "%s" (System.Text.Json.JsonSerializer.Serialize(packages, jsonOptions))
        | "pack" :: options ->
            let feed, rest = takeSupportFeed options
            let count, unknown = takeOption "--jobs" rest
            ensure (List.isEmpty unknown) "Usage: pack [--jobs N] [--support-feed PATH]"
            let jobs = count |> Option.map Int32.Parse |> Option.defaultValue (max 1 (min 2 Environment.ProcessorCount))
            Packages.inventory () |> Packages.pack jobs feed
        | ["verify"] -> Packages.inventory () |> Packages.verify
        | ["availability"] -> Packages.inventory () |> Packages.availability
        | ["audit"] -> Packages.inventory () |> Consumers.audit |> ignore
        | ["check"] -> Checks.runChecks ()
        | "support" :: options ->
            let feed, unknown = takeSupportFeed options
            ensure (List.isEmpty unknown) "Usage: support [--support-feed PATH]"
            Consumers.checkSupport feed
        | ["publish"; "--confirm"; version] -> Packages.publish (Packages.inventory ()) version
        | "examples" :: options ->
            let feed, options = takeSupportFeed options
            ensure (options |> List.forall (fun x -> x = "--public" || x = "--fable")) "Usage: examples [--public] [--fable] [--support-feed PATH]"
            let projects = Packages.inventory () |> Consumers.audit
            Consumers.buildExamples projects (List.contains "--public" options) (List.contains "--fable" options) feed
        | ["snippets"] -> Consumers.checkSnippets None
        | ["snippets"; "--js-root"; path] -> Consumers.checkSnippets (Some(Path.GetFullPath path))
        | _ -> failwith "Commands: plan | pack [--jobs N] [--support-feed PATH] | verify | availability | audit | check | support [--support-feed PATH] | examples [--public] [--fable] [--support-feed PATH] | snippets [--js-root PATH] | publish --confirm VERSION"
        0
    with ex ->
        eprintfn "%s" ex.Message
        1
