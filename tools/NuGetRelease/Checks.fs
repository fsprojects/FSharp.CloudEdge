module NuGetRelease.Checks

open System
open System.IO
open System.IO.Compression
open System.Net
open NuGetRelease.Common
open NuGetRelease.Packages
open NuGetRelease.Consumers

let checkAvailabilityPolling () =
    let seconds (value: float) = TimeSpan.FromSeconds value
    let mutable elapsed = TimeSpan.Zero
    let now () = elapsed
    let pauses = ResizeArray<TimeSpan>()
    let pause duration = pauses.Add duration; elapsed <- elapsed + duration
    let ignoreReport _ _ = ()
    let calls = ResizeArray<string>()
    let eventual id _ =
        calls.Add id
        if id = "First" || elapsed >= seconds 30. then Available else Pending "indexing"
    let pending = waitForAvailability (seconds 90.) now pause eventual ignoreReport [|"First"; "Second"|]
    ensure (pending.Length = 0 && calls.ToArray() = [|"First"; "Second"; "Second"|]) "Availability must retry pending packages and stop checking available packages"
    ensure (pauses.ToArray() = [|seconds 30.|]) "Availability must stop waiting as soon as all packages are visible"

    elapsed <- TimeSpan.Zero
    pauses.Clear()
    calls.Clear()
    let absent id _ = calls.Add id; Pending "indexing"
    let pending = waitForAvailability (seconds 45.) now pause absent ignoreReport [|"First"|]
    ensure (pending = [|"First", "indexing"|] && calls.Count = 2) "Availability must retain pending packages at the deadline"
    ensure (elapsed = seconds 45. && pauses.ToArray() = [|seconds 30.; seconds 15.|]) "Availability polling exceeded its time budget"

    elapsed <- TimeSpan.Zero
    pauses.Clear()
    calls.Clear()
    let pending = waitForAvailability TimeSpan.Zero now pause absent ignoreReport [|"First"; "Second"|]
    ensure (pending.Length = 2 && calls.Count = 2 && pauses.Count = 0) "Zero wait must check every package once without sleeping"

    elapsed <- TimeSpan.Zero
    calls.Clear()
    let slow id timeout =
        calls.Add id
        ensure (timeout = seconds 5.) "Request timeout must fit within the remaining availability budget"
        elapsed <- elapsed + timeout
        Pending "request timed out"
    let pending = waitForAvailability (seconds 5.) now pause slow ignoreReport [|"First"; "Second"|]
    ensure (pending.Length = 2 && calls.ToArray() = [|"First"|] && elapsed = seconds 5.) "Requests must not continue after the availability deadline"

    for status in [HttpStatusCode.NotFound; HttpStatusCode.RequestTimeout; HttpStatusCode.TooManyRequests; HttpStatusCode.InternalServerError; HttpStatusCode.ServiceUnavailable] do
        ensure (retryAvailabilityStatus status) $"Availability should retry HTTP {int status}"
    for status in [HttpStatusCode.OK; HttpStatusCode.BadRequest; HttpStatusCode.Unauthorized; HttpStatusCode.Forbidden] do
        ensure (not (retryAvailabilityStatus status)) $"Availability should not retry HTTP {int status}"
    pauses.Clear()
    let mutable permanentFailure = false
    try
        waitForAvailability (seconds 90.) now pause (fun _ _ -> invalidOp "HTTP 403") ignoreReport [|"First"|] |> ignore
    with :? InvalidOperationException as error -> permanentFailure <- error.Message = "HTTP 403"
    ensure (permanentFailure && pauses.Count = 0) "Permanent availability failures must propagate immediately"

let runChecks () =
    checkAvailabilityPolling ()
    let gitRef = Environment.GetEnvironmentVariable "GITHUB_REF"
    if not (isNull gitRef) && gitRef.StartsWith "refs/tags/" then
        ensure (gitRef = "refs/tags/v" + config.Version) $"Release tag must be v{config.Version}; received {gitRef}"
    let projects = inventory ()
    ensure (sampleVersion = "0.1.*") "Samples must follow the current patch line"
    ensure (patchRange "0.1.1" = "[0.1.1,0.2.0)") "Patch dependency range is incorrect"
    ensure (projects.Length = 47 && (projects |> Array.filter _.Fable |> Array.length) = 35) "Unexpected release scope"
    let mutable seen = Set.empty
    for project in projects do
        ensure (project.Dependencies |> Map.keys |> Seq.forall seen.Contains) $"Invalid dependency order: {project.Id}"
        ensure (project.External |> Map.values |> Seq.forall (fun v -> not (v.Contains "-local."))) "Local dependency survived release planning"
        seen <- seen.Add project.Id
    let fixture: Package = { Id = "FSharp.CloudEdge.Test"; Path = ""; Framework = "net8.0"; Fable = true; Sources = [|"Test.fs"|]; Dependencies = Map.empty; External = Map.ofList ["Xantham.Fable.Core", "0.1.0"] }
    let left = { fixture with Id = "Left" }
    let right = { fixture with Id = "Right" }
    let joined = { fixture with Id = "Joined"; Dependencies = Map.ofList [left.Id, config.Version; right.Id, config.Version] }
    let leaf = { fixture with Id = "Leaf"; Dependencies = Map.ofList [joined.Id, config.Version] }
    let layers = packBatches [|leaf; joined; right; left|]
    ensure (layers |> Array.map Array.length = [|2; 1; 1|]) "Independent packages must share a batch; consumers must wait"
    ensure (layers.[1].[0].Id = joined.Id && layers.[2].[0].Id = leaf.Id) "Dependency layers are out of order"
    let batches = packBatches projects
    ensure (batches[0].Length = 1 && batches.[0].[0].Id = "FSharp.CloudEdge.Core.Api") "The large API assembly must build alone"
    let mutable packed = Set.empty
    for batch in batches do
        ensure (batch |> Array.forall (fun p -> p.Dependencies |> Map.keys |> Seq.forall packed.Contains)) "A package would restore before its dependency was packed"
        packed <- Set.union packed (batch |> Array.map _.Id |> Set.ofArray)
    ensure (packed.Count = projects.Length) "Package batching omitted projects"
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
        create "[0.1.0,0.2.0)" true
        let entry = inspectPackage directory fixture
        ensure (entry.Sha256.Length = 64) "Missing package hash"
        create "0.1.0-local.test" true
        expectFailure (fun () -> inspectPackage directory fixture |> ignore)
        create "[0.1.0]" true
        expectFailure (fun () -> inspectPackage directory fixture |> ignore)
        create "[0.1.0,0.2.0)" false
        expectFailure (fun () -> inspectPackage directory fixture |> ignore)
        expectFailure (fun () -> order [| { fixture with Dependencies = Map.ofList [fixture.Id, config.Version] } |] |> ignore)
        ensure (isExcerpt "  let x = 1\n  let y = 2" "module Test\nlet x = 1\nlet y = 2\n") "Uniform snippet indentation rejected"
        ensure (not (isExcerpt "let x = 1\n    let y = 2" "let x = 1\nlet y = 2")) "Broken snippet indentation accepted"
        printfn "Release graph, package rejection, hash, snippet and availability checks passed"
    finally
        Directory.Delete(directory, true)
