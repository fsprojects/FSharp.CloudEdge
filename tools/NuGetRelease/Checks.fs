module NuGetRelease.Checks

open System
open System.IO
open System.IO.Compression
open NuGetRelease.Common
open NuGetRelease.Packages
open NuGetRelease.Consumers

let runChecks () =
    let gitRef = Environment.GetEnvironmentVariable "GITHUB_REF"
    if not (isNull gitRef) && gitRef.StartsWith "refs/tags/" then
        ensure (gitRef = "refs/tags/v" + config.Version) $"Release tag must be v{config.Version}; received {gitRef}"
    let projects = inventory ()
    ensure (projects.Length = 47 && (projects |> Array.filter _.Fable |> Array.length) = 35) "Unexpected release scope"
    let mutable seen = Set.empty
    for project in projects do
        ensure (project.Dependencies |> Map.keys |> Seq.forall seen.Contains) $"Invalid dependency order: {project.Id}"
        ensure (project.External |> Map.values |> Seq.forall (fun v -> not (v.Contains "-local."))) "Local dependency survived release planning"
        seen <- seen.Add project.Id
    let fixture: Package = { Id = "FSharp.CloudEdge.Test"; Path = ""; Framework = "net8.0"; Fable = true; Sources = [|"Test.fs"|]; Dependencies = Map.empty; External = Map.ofList ["Xantham.Fable.Core", "0.1.0"] }
    let directory = Path.Combine(Path.GetTempPath(), "cloudedge-release-tests-" + Guid.NewGuid().ToString("N"))
    mkdir directory
    let expectFailure action =
        let failed = try action (); false with _ -> true
        ensure failed "Expected validation to reject the fixture"
    let create dependency source =
        let file = Path.Combine(directory, $"{fixture.Id}.{config.Version}.nupkg")
        if File.Exists file then File.Delete file
        use archive = ZipFile.Open(file, ZipArchiveMode.Create)
        let add name text =
            use writer = new StreamWriter(archive.CreateEntry(name).Open())
            writer.Write(text: string)
        add (fixture.Id + ".nuspec") $"<package xmlns=\"http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd\"><metadata><id>{fixture.Id}</id><version>{config.Version}</version><dependencies><dependency id=\"Xantham.Fable.Core\" version=\"{dependency}\"/></dependencies></metadata></package>"
        for name in ["README.md"; "LICENSE"; "lib/net8.0/Test.dll"; "fable/" + fixture.Id + ".fsproj"] do add name "fixture"
        if source then add "fable/Test.fs" "module Test"
    try
        create "[0.1.0]" true
        let entry = inspectPackage directory fixture
        ensure (entry.Sha256.Length = 64) "Missing package hash"
        create "0.1.0-local.test" true
        expectFailure (fun () -> inspectPackage directory fixture |> ignore)
        create "[0.1.0]" false
        expectFailure (fun () -> inspectPackage directory fixture |> ignore)
        expectFailure (fun () -> order [| { fixture with Dependencies = Map.ofList [fixture.Id, config.Version] } |] |> ignore)
        ensure (isExcerpt "  let x = 1\n  let y = 2" "module Test\nlet x = 1\nlet y = 2\n") "Uniform snippet indentation rejected"
        ensure (not (isExcerpt "let x = 1\n    let y = 2" "let x = 1\nlet y = 2")) "Broken snippet indentation accepted"
        printfn "Release graph, package rejection, hash and snippet checks passed"
    finally
        Directory.Delete(directory, true)
