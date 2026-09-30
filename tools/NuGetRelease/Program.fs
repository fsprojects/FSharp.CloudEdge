module NuGetRelease.Program

open System
open System.IO
open NuGetRelease.Common

[<EntryPoint>]
let main args =
    try
        match args |> Array.toList with
        | ["plan"] -> Packages.inventory () |> fun packages -> printfn "%s" (System.Text.Json.JsonSerializer.Serialize(packages, jsonOptions))
        | ["pack"] -> Packages.inventory () |> Packages.pack
        | ["verify"] -> Packages.inventory () |> Packages.verify
        | ["availability"] -> Packages.inventory () |> Packages.availability
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
        | _ -> failwith "Commands: plan | pack | verify | availability | audit | check | support | examples [--public] [--fable] | snippets [--js-root PATH] | publish --confirm VERSION"
        0
    with ex ->
        eprintfn "%s" ex.Message
        1
