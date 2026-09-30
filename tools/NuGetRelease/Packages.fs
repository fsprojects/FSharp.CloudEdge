module NuGetRelease.Packages

open System
open System.Collections.Generic
open System.IO
open System.IO.Compression
open System.Net
open System.Net.Http
open System.Text.Json
open System.Xml.Linq
open NuGetRelease.Common

let order (projects: Package array) =
    let byId = projects |> Array.map (fun p -> p.Id, p) |> Map.ofArray
    ensure (byId.Count = projects.Length) "Duplicate package IDs"
    let ordered = ResizeArray<Package>()
    let visiting = HashSet<string>()
    let visited = HashSet<string>()
    let rec visit name =
        if not (visited.Contains name) then
            ensure (byId.ContainsKey name) $"Dependency is outside the release: {name}"
            ensure (visiting.Add name) $"Dependency cycle: {name}"
            let project = byId[name]
            project.Dependencies |> Map.iter (fun dependency _ -> visit dependency)
            visiting.Remove name |> ignore
            visited.Add name |> ignore
            ordered.Add project
    byId |> Map.iter (fun name _ -> visit name)
    ordered.ToArray()

// Staged projects consume dependencies as NuGet packages. Finish each dependency
// layer before restoring the next, then let MSBuild parallelize independent projects.
let packBatches (projects: Package array) =
    let mutable remaining = order projects
    let mutable completed = Set.empty
    let batches = ResizeArray<Package array>()
    while remaining.Length > 0 do
        let ready = remaining |> Array.filter (fun p -> p.Dependencies |> Map.keys |> Seq.forall completed.Contains)
        ensure (ready.Length > 0) "No package is ready to build"
        // This generated assembly uses several GB during compilation. Give it the
        // runner's memory before starting concurrent builds of the smaller projects.
        let batch =
            match ready |> Array.tryFind (fun p -> p.Id = "FSharp.CloudEdge.Core.Api") with
            | Some core -> [|core|]
            | None -> ready
        batches.Add batch
        completed <- Set.union completed (batch |> Array.map _.Id |> Set.ofArray)
        remaining <- remaining |> Array.filter (fun p -> not (completed.Contains p.Id))
    batches.ToArray()

let inventory () =
    XDocument.Load(Path.Combine(root, config.Solution))
    |> elements "Project"
    |> Array.map (fun item ->
        let path = Path.Combine(root, attr "Path" item)
        let doc = XDocument.Load path
        let name = Path.GetFileNameWithoutExtension path
        ensure (name.StartsWith "FSharp.CloudEdge.") $"Not a CloudEdge package: {name}"
        let pathInRepo = relative path
        let references = elements "PackageReference" doc
        {
            Id = name; Path = pathInRepo; Framework = (element "TargetFramework" doc).Value
            Fable = pathInRepo.StartsWith "src/Runtime/" || pathInRepo.StartsWith "src/Support/"
            Sources = elements "Compile" doc |> Array.map (attr "Include")
            Dependencies = elements "ProjectReference" doc |> Array.map (fun n -> Path.GetFileNameWithoutExtension(attr "Include" n), config.Version) |> Map.ofArray
            External = references |> Array.map (fun n ->
                let id = attr "Include" n
                id, match config.PublicDependencies.TryGetValue id with true, version -> version | _ -> attr "Version" n) |> Map.ofArray
        })
    |> order

let packageUrl name = $"https://www.nuget.org/packages/{name}/{config.Version}"
let dependencies (project: Package) = Map.fold (fun all key value -> Map.add key value all) project.Dependencies project.External

let dependencyRange (id: string) version =
    if id.StartsWith("FSharp.CloudEdge.") || config.PublicDependencies.ContainsKey id then patchRange version
    else version

let stage commit (project: Package) =
    let directory = Path.Combine(output, "staged", project.Id)
    mkdir directory
    let original = Path.Combine(root, project.Path)
    for source in project.Sources do
        ensure (not (Path.IsPathRooted source) && not (source.Split('/', '\\') |> Array.contains "..")) $"Unsupported source path: {source}"
        copyFile (Path.Combine(Path.GetDirectoryName original, source)) (Path.Combine(directory, source))
    let properties = [
        "TargetFramework", project.Framework; "Version", config.Version; "PackageId", project.Id
        "Authors", config.Authors; "Description", $"{project.Id}: F# bindings and clients for Cloudflare. See the documentation for supported APIs and verification status."
        "PackageLicenseExpression", "MIT"; "PackageReadmeFile", "README.md"
        "PackageProjectUrl", config.ProjectUrl; "RepositoryUrl", config.Repository
        "RepositoryType", "git"; "RepositoryCommit", commit
        "PackageTags", (if project.Fable then "FSharp Cloudflare Fable" else "FSharp Cloudflare")
        "NoWarn", "$(NoWarn);FS1104"; "OtherFlags", "$(OtherFlags) --maxerrors:20"
    ]
    let name = project.Id + ".fsproj"
    let assets = [
        for file in ["README.md"; "LICENSE"] do
            xml "None" ["Include", file; "Pack", "true"; "PackagePath", "/"] []
        if project.Fable then
            for file in Array.append project.Sources [|name|] do
                xml "Content" ["Include", file; "Pack", "true"; "PackagePath", "fable/" + Path.GetDirectoryName(file).Replace('\\', '/')] []
    ]
    let file = Path.Combine(directory, name)
    xml "Project" ["Sdk", "Microsoft.NET.Sdk"] [
        xml "PropertyGroup" [] (properties |> List.map (fun (name, value) -> valueElement name value))
        xml "ItemGroup" [] (project.Sources |> Array.map (fun source -> xml "Compile" ["Include", source] []) |> Array.toList)
        xml "ItemGroup" [] (dependencies project |> Map.toList |> List.map (fun (id, version) -> xml "PackageReference" ["Include", id; "Version", dependencyRange id version] []))
        xml "ItemGroup" [] assets
    ] |> saveXml file
    let readme = $"# {project.Id}\n\nFSharp.CloudEdge {config.Version}.\n\n[Documentation]({config.ProjectUrl}) · [Source]({config.Repository})\n\nConsult the documentation for pinned upstream SDK versions, setup, binding limitations, and runtime verification status. "
    let consumer = if project.Fable then "Compile with Fable; install the corresponding npm SDK separately where required. " else "Use this client from a .NET application. "
    File.WriteAllText(Path.Combine(directory, "README.md"), readme + consumer + "Package availability does not imply every hosted workflow has been validated.\n")
    copyFile (Path.Combine(root, "LICENSE")) (Path.Combine(directory, "LICENSE"))
    file

let inspectPackage folder (project: Package) =
    let file = Path.Combine(folder, $"{project.Id}.{config.Version}.nupkg")
    use archive = ZipFile.OpenRead file
    let names = archive.Entries |> Seq.map _.FullName |> Set.ofSeq
    let specs = archive.Entries |> Seq.filter (fun e -> e.FullName.EndsWith ".nuspec") |> Seq.toArray
    ensure (specs.Length = 1) $"Expected one nuspec: {file}"
    use stream = specs[0].Open()
    let doc = XDocument.Load stream
    let ns = doc.Root.Name.Namespace
    let metadata = doc.Root.Element(ns + "metadata")
    ensure (metadata.Element(ns + "id").Value = project.Id) $"Wrong package ID: {file}"
    ensure (metadata.Element(ns + "version").Value = config.Version) $"Wrong version: {file}"
    for asset in ["README.md"; "LICENSE"] do ensure (names.Contains asset) $"Missing {asset}: {file}"
    ensure (names |> Seq.exists (fun n -> n.StartsWith "lib/" && n.EndsWith ".dll")) $"Missing assembly: {file}"
    let deps = Dictionary<string, string>()
    for node in doc.Descendants(ns + "dependency") do deps[attr "id" node] <- attr "version" node
    ensure (deps.Values |> Seq.forall (fun v -> not (v.Contains "-local."))) $"Local-only dependency: {file}"
    for KeyValue(name, version) in dependencies project do
        ensure (deps.ContainsKey name) $"Missing dependency {name}: {file}"
        let actual = deps[name].Replace(" ", "")
        let expected = dependencyRange name version
        let matches = actual = expected || (expected = version && actual = $"[{version},)")
        ensure matches $"Wrong dependency {name} {actual}; expected {expected}: {file}"
    if project.Fable then
        for source in Array.append project.Sources [|project.Id + ".fsproj"|] do
            ensure (names.Contains("fable/" + source.Replace('\\', '/'))) $"Missing Fable source {source}: {file}"
    { Id = project.Id; Version = config.Version; File = Path.GetFileName file; Sha256 = sha256 file; Dependencies = deps; Url = packageUrl project.Id }

let verify (projects: Package array) =
    let actual = projects |> Array.map (inspectPackage feed)
    let expected = readJson<ManifestEntry array> (Path.Combine(output, "manifest.json"))
    ensure (actual.Length = expected.Length) "Manifest package count differs"
    Array.iter2 (fun a b -> ensure (a.Id = b.Id && a.Version = b.Version && a.File = b.File && a.Sha256 = b.Sha256) $"Candidate differs from manifest: {a.Id}") actual expected
    printfn "Verified %d packages and manifest hashes" actual.Length

let pack jobs (projects: Package array) =
    ensure (jobs > 0) "The build process count must be positive"
    mkdir feed
    let restoreConfig = Path.Combine(output, "NuGet.Config")
    let cache = Path.Combine(output, "cache")
    if Directory.Exists cache then Directory.Delete(cache, true)
    writeNuGetConfig restoreConfig cache true
    let commit = capture "git" ["rev-parse"; "HEAD"]
    let logDirectory = Path.Combine(output, "pack-logs")
    mkdir logDirectory
    let batches = packBatches projects
    for index, batch in Array.indexed batches do
        printfn "[Batch %d/%d] Packing %d packages with up to %d MSBuild processes: %s" (index + 1) batches.Length batch.Length jobs (batch |> Array.map _.Id |> String.concat ", ")
        let solution = Path.Combine(logDirectory, $"batch-{index + 1}.slnx")
        let binaryLog = Path.Combine(logDirectory, $"batch-{index + 1}.binlog")
        let paths = batch |> Array.map (stage commit)
        xml "Solution" [] (paths |> Array.map (fun path -> xml "Project" ["Path", Path.GetRelativePath(logDirectory, path).Replace('\\', '/')] []) |> Array.toList)
        |> saveXml solution
        run "dotnet" ["pack"; solution; "-c"; "Release"; $"-p:PackageOutputPath={feed}";
                      $"-p:RestoreConfigFile={restoreConfig}"; $"-maxcpucount:{jobs}"; "-p:BuildInParallel=true";
                      $"-bl:{binaryLog}"; "--verbosity"; "normal"; "--nologo"]
            (Path.Combine(logDirectory, $"batch-{index + 1}.log"))
    writeJson (Path.Combine(output, "manifest.json")) (projects |> Array.map (inspectPackage feed))
    verify projects

type PackageAvailability = Available | Pending of string

let retryAvailabilityStatus (status: HttpStatusCode) =
    status = HttpStatusCode.NotFound || status = HttpStatusCode.RequestTimeout ||
    int status = 429 || (int status >= 500 && int status <= 599)

// The clock and probe are injected so deadline handling can be checked without
// network requests or real delays. Packages already visible need no more polling.
let waitForAvailability waitDuration now pause probe report (ids: string array) =
    ensure (waitDuration >= TimeSpan.Zero) "The availability wait must not be negative"
    let interval = TimeSpan.FromSeconds 30.
    let started = now ()
    let elapsed () = now () - started
    let remaining () = waitDuration - elapsed ()
    let mutable pending = ids |> Array.map (fun id -> id, "not checked before the deadline")
    let mutable finished = false
    while not finished do
        pending <- pending |> Array.choose (fun (id, reason) ->
            let budget = remaining ()
            if waitDuration > TimeSpan.Zero && budget <= TimeSpan.Zero then Some (id, reason)
            else
                let timeout = if waitDuration = TimeSpan.Zero then interval else min interval budget
                match probe id timeout with
                | Available -> None
                | Pending detail -> Some (id, detail))
        report (elapsed ()) pending
        finished <- pending.Length = 0 || waitDuration = TimeSpan.Zero || remaining () <= TimeSpan.Zero
        if not finished then
            let delay = min interval (remaining ())
            if delay > TimeSpan.Zero then pause delay
    pending

let availability waitMinutes (projects: Package array) =
    ensure (waitMinutes >= 0) "The availability wait must not be negative"
    use client = new HttpClient(Timeout = Threading.Timeout.InfiniteTimeSpan)
    let probe (id: string) timeout =
        let url = $"https://api.nuget.org/v3-flatcontainer/{id.ToLowerInvariant()}/index.json"
        use cancellation = new Threading.CancellationTokenSource(timeout: TimeSpan)
        try
            use response = client.GetAsync(url, cancellation.Token).GetAwaiter().GetResult()
            if retryAvailabilityStatus response.StatusCode then
                Pending $"HTTP {int response.StatusCode}"
            else
                response.EnsureSuccessStatusCode() |> ignore
                use doc = JsonDocument.Parse(response.Content.ReadAsStringAsync(cancellation.Token).GetAwaiter().GetResult())
                if doc.RootElement.GetProperty("versions").EnumerateArray() |> Seq.exists (fun v -> v.GetString() = config.Version) then
                    printfn "available %s %s" id config.Version
                    Available
                else Pending "version not yet in the public index"
        with
        | :? OperationCanceledException -> Pending "request timed out"
        | :? HttpRequestException as error when not error.StatusCode.HasValue -> Pending $"network error: {error.Message}"
    let clock = Diagnostics.Stopwatch.StartNew()
    let report (elapsed: TimeSpan) pending =
        printfn "%d/%d packages available after %.0f seconds; %d pending" (projects.Length - Array.length pending) projects.Length elapsed.TotalSeconds (Array.length pending)
    let pending =
        waitForAvailability (TimeSpan.FromMinutes(float waitMinutes)) (fun () -> clock.Elapsed)
            (fun duration -> Threading.Thread.Sleep(duration: TimeSpan)) probe report (projects |> Array.map _.Id)
    for id, reason in pending do printfn "PENDING %s %s: %s" id config.Version reason
    ensure (pending.Length = 0)
        $"{pending.Length} packages were not confirmed available on nuget.org within {waitMinutes} minute(s). Uploads may already have succeeded; do not republish or bump the version. Retry the availability check, then public-consumer verification."

let publish (projects: Package array) confirmedVersion =
    ensure (confirmedVersion = config.Version) "Publication confirmation must match the configured release version"
    let key = Environment.GetEnvironmentVariable "NUGET_API_KEY"
    ensure (not (String.IsNullOrWhiteSpace key)) "NUGET_API_KEY is not set; the publishing workflow obtains it through NuGet/login and Trusted Publishing"
    verify projects
    for project in projects do
        printfn "Publishing %s %s" project.Id config.Version
        run "dotnet" ["nuget"; "push"; Path.Combine(feed, $"{project.Id}.{config.Version}.nupkg");
                      "--api-key"; key; "--source"; "https://api.nuget.org/v3/index.json"]
            (Path.Combine(output, "publish-logs", project.Id + ".log"))
