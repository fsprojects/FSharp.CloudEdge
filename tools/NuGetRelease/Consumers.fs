module NuGetRelease.Consumers

open System
open System.IO
open System.Net
open System.Text.RegularExpressions
open System.Xml.Linq
open NuGetRelease.Common
open NuGetRelease.Packages

type ConsumerResult = { Project: string; Build: string; Fable: string; Error: string }

let exampleProjects () =
    Directory.GetDirectories(Path.Combine(root, "site/examples"))
    |> Array.collect (fun dir -> Directory.GetFiles(dir, "*.fsproj"))
    |> Array.sort

let audit packages =
    let ids = packages |> Array.map (fun (p: Package) -> p.Id) |> Set.ofArray
    let projects = exampleProjects ()
    let mutable count = 0
    for project in projects do
        let doc = XDocument.Load project
        let refs = elements "PackageReference" doc |> Array.filter (fun n -> (attr "Include" n).StartsWith "FSharp.CloudEdge.")
        let sources = elements "ProjectReference" doc |> Array.map (fun n -> Path.GetFileNameWithoutExtension(attr "Include" n), n) |> Map.ofArray
        ensure (refs |> Array.map (attr "Include") |> Set.ofArray = (sources |> Map.keys |> Set.ofSeq)) $"Package/source references differ: {project}"
        for node in refs do
            let id = attr "Include" node
            ensure (ids.Contains id && attr "Version" node = sampleVersion) $"Unexpected package or version: {id} in {project}; expected {sampleVersion}"
            ensure (attr "Condition" node = "'$(CloudEdgeUseSource)' != 'true'") $"Package must be the default: {id}"
            ensure (attr "Condition" sources[id] = "'$(CloudEdgeUseSource)' == 'true'") $"Source must be opt-in: {id}"
            count <- count + 1
    let catalog = File.ReadAllText(Path.Combine(root, "site/content/libraries/packages.md"))
    for id in ids do ensure (catalog.Contains(packageUrl id)) $"Missing versioned catalog URL: {id}"
    for name in ["first-worker"; "first-deploy"] do
        let text = File.ReadAllText(Path.Combine(root, $"site/content/guide/{name}.md"))
        ensure (not (text.Contains "ProjectReference" || text.Contains "artifacts/tool-feed")) $"Source checkout still required: {name}"
    printfn "Audited %d examples, %d package references, %d catalog URLs" projects.Length count ids.Count
    projects

let installTool directory packageId version command =
    let tool = Path.Combine(directory, "tools", if OperatingSystem.IsWindows() then command + ".exe" else command)
    let source = Path.Combine(directory, "Tools.NuGet.Config")
    writeNuGetConfig source (Path.Combine(directory, "tool-cache")) false
    if not (File.Exists tool) then
        run "dotnet" ["tool"; "install"; packageId; "--version"; version; "--tool-path"; Path.GetDirectoryName tool; "--configfile"; source] (Path.Combine(directory, "logs", command + "-install.log"))
    tool

let installCompiler directory =
    use manifest = System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(root, ".config/dotnet-tools.json")))
    let version = manifest.RootElement.GetProperty("tools").GetProperty("fickl").GetProperty("version").GetString()
    installTool directory "fickl" version "fickl"

let prepareConsumerDirectory publicFeed =
    let mode = if publicFeed then "public" else "candidate"
    let directory = Path.Combine(output, "consumers-" + mode)
    if Directory.Exists directory then Directory.Delete(directory, true)
    mkdir directory
    let examples = Path.Combine(directory, "examples")
    copyTree (Path.Combine(root, "site/examples")) examples
    for project in Directory.GetFiles(examples, "*.fsproj", SearchOption.AllDirectories) do
        let doc = XDocument.Load project
        elements "ProjectReference" doc |> Array.iter _.Remove()
        elements "PackageReference" doc |> Array.iter (fun n ->
            n.SetAttributeValue(xn "Condition", null)
            if (attr "Include" n).StartsWith "FSharp.CloudEdge." then
                n.SetAttributeValue(xn "Version", $"[{config.Version}]"))
        doc.Save project
    let props = Path.Combine(examples, "Examples.props")
    let doc = XDocument.Load props
    doc.Root.Elements()
    |> Seq.collect _.Elements()
    |> Seq.filter (fun n -> n.Name.LocalName.Contains "CloudEdge" || n.Name.LocalName = "BuildProjectReferences" || n.Name.LocalName = "RestoreAdditionalProjectSources" || n.Attribute(xn "Condition") <> null)
    |> Seq.toArray |> Array.iter _.Remove()
    doc.Save props
    let supportPins = xml "ItemGroup" [] [
        for KeyValue(id, version) in config.PublicDependencies do
            xml "PackageReference" ["Include", id; "Version", $"[{version}]"; "Condition", "'$(ExampleKind)' != 'DotNet'"] []
    ]
    doc.Root.Add supportPins
    doc.Save props
    writeNuGetConfig (Path.Combine(directory, "NuGet.Config")) (Path.Combine(directory, "cache")) (not publicFeed)
    mode, directory

// Reuse the source composition witness against packages only, so the published
// Pi contracts must preserve the same shared input and factory context types.
let preparePiConsumer directory =
    let witness = Path.Combine(root, "tests/SDKComposition/Pi.fs")
    let client = Path.Combine(root, "tests/AgentsPi/Client.fs")
    let original = Path.Combine(root, "tests/AgentsPi/AgentsPi.fsproj")
    let compilerManifest = Path.Combine(root, "tests/AgentsPi/.config/dotnet-tools.json")
    let doc = XDocument.Load original
    let framework = (element "TargetFramework" doc).Value
    let coreVersion = elements "PackageReference" doc |> Array.find (fun n -> attr "Include" n = "Fable.Core") |> attr "Version"
    let destination = Path.Combine(directory, "examples/agents-pi")
    copyFile witness (Path.Combine(destination, "Pi.fs"))
    copyFile client (Path.Combine(destination, "Client.fs"))
    let project = Path.Combine(destination, "AgentsPi.PackageConsumer.fsproj")
    xml "Project" ["Sdk", "Microsoft.NET.Sdk"] [
        xml "PropertyGroup" [] [valueElement "TargetFramework" framework]
        xml "ItemGroup" [] [
            xml "Compile" ["Include", "Pi.fs"] []
            xml "Compile" ["Include", "Client.fs"] []
            xml "PackageReference" ["Include", "Fable.Core"; "Version", coreVersion] []
            xml "PackageReference" ["Include", "FSharp.CloudEdge.Runtime.AgentsPi"; "Version", $"[{config.Version}]"] []
            xml "PackageReference" ["Include", "FSharp.CloudEdge.Support.Workers"; "Version", $"[{config.Version}]"] []
            for KeyValue(id, version) in config.PublicDependencies do
                xml "PackageReference" ["Include", id; "Version", $"[{version}]"] []
        ]
    ] |> saveXml project
    mkdir (Path.Combine(directory, "logs"))
    [witness; client; original; compilerManifest]
    |> List.map (fun file -> relative file, sha256 file)
    |> Map.ofList
    |> writeJson (Path.Combine(directory, "logs/agents-pi-inputs.json"))
    project

let requirePiPackages (packages: string array) =
    for name in ["AgentsPi"; "PiAi"; "PiDurable"] do
        let package = $"FSharp.CloudEdge.Runtime.{name}/{config.Version}"
        ensure (Array.contains package packages) $"Pi consumer did not resolve {package}"

let recordResolvedPackages directory name project =
    use assets = System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(Path.GetDirectoryName(project: string), "obj/project.assets.json")))
    let packages = assets.RootElement.GetProperty("libraries").EnumerateObject()
                   |> Seq.filter (fun p -> p.Value.GetProperty("type").GetString() = "package")
                   |> Seq.map _.Name |> Seq.sort |> Seq.toArray
    for package in packages do
        let parts = package.Split('/')
        let id, version = parts[0], parts[1]
        if id.StartsWith "FSharp.CloudEdge." then ensure (version = config.Version) $"Consumer resolved {package}; expected {config.Version}"
        match config.PublicDependencies.TryGetValue id with
        | true, expected -> ensure (version = expected) $"Consumer resolved {package}; expected {expected}"
        | _ -> ()
    let destination = Path.Combine(directory, "resolved-packages")
    mkdir destination
    writeJson (Path.Combine(destination, name + ".json")) packages
    packages

let buildExamples (projects: string array) publicFeed emit =
    let mode, directory = prepareConsumerDirectory publicFeed
    // Generated tagged unions throughout the bindings require Fickl's inline-interface support.
    let tool = if emit then installCompiler directory else ""
    let piProject = preparePiConsumer directory
    let consumers = Array.append (projects |> Array.map (fun original ->
        let name = Path.GetFileName(Path.GetDirectoryName original)
        name, Path.Combine(directory, "examples", name, Path.GetFileName original), false)) [|"agents-pi", piProject, true|]
    let results = ResizeArray<ConsumerResult>()
    for index, (name, project, pi) in Array.indexed consumers do
        printfn "[%d/%d] %s package consumer: %s" (index + 1) consumers.Length mode name
        let mutable built = false
        let mutable emitted = "not run"
        let mutable error = ""
        try
            run "dotnet" ["build"; project; "-c"; "Release"; "--nologo"] (Path.Combine(directory, "logs", name + "-build.log"))
            let resolved = recordResolvedPackages directory name project
            if pi then requirePiPackages resolved
            built <- true
            let kind = elements "ExampleKind" (XDocument.Load project) |> Array.tryHead
            if emit && (kind |> Option.forall (fun n -> n.Value <> "DotNet")) then
                emitted <- "failed"
                let arguments = [project; "--outDir"; Path.Combine(directory, "js", name)] @ (if pi then ["--noCache"] else [])
                run tool arguments (Path.Combine(directory, "logs", name + "-fable.log"))
                emitted <- "passed"
        with ex -> error <- ex.Message
        results.Add { Project = name; Build = (if built then "passed" else "failed"); Fable = emitted; Error = error }
        writeJson (Path.Combine(directory, "results.json")) (results.ToArray())
    let failures = results |> Seq.filter (fun r -> r.Error <> "") |> Seq.length
    ensure (failures = 0) $"{failures} package consumers failed; see {directory}/results.json"
    printfn "Passed %d isolated %s consumers" results.Count mode

// Exercise the erased keys and indexers consumed by CloudEdge bindings. The
// separate upstream-helper repro covers APIs these bindings do not call.
let checkSupport () =
    let directory = Path.Combine(output, "public-support")
    if Directory.Exists directory then Directory.Delete(directory, true)
    mkdir directory
    writeNuGetConfig (Path.Combine(directory, "NuGet.Config")) (Path.Combine(directory, "cache")) false
    let tool = installCompiler directory
    for file in ["Smoke.fs"; "check.mjs"] do
        copyFile (Path.Combine(root, "tests/SupportPackage", file)) (Path.Combine(directory, file))
    let version = config.PublicDependencies["Xantham.Fable.Core"]
    let tsVersion = config.PublicDependencies["Xantham.Fable.Core.TS"]
    let project = Path.Combine(directory, "PublicSupport.fsproj")
    xml "Project" ["Sdk", "Microsoft.NET.Sdk"] [
        xml "PropertyGroup" [] [valueElement "TargetFramework" "net8.0"]
        xml "ItemGroup" [] [
            xml "Compile" ["Include", "Smoke.fs"] []
            xml "PackageReference" ["Include", "Fable.Core"; "Version", "5.6.0"] []
            xml "PackageReference" ["Include", "Xantham.Fable.Core"; "Version", $"[{version}]"] []
            xml "PackageReference" ["Include", "Xantham.Fable.Core.TS"; "Version", $"[{tsVersion}]"] []
        ]
    ] |> saveXml project
    run "dotnet" ["build"; project; "--nologo"] (Path.Combine(directory, "logs/build.log"))
    use receipt = System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "inventory/pi-lifecycle-acceptance.json")))
    let expected = receipt.RootElement.GetProperty("supportFableSources")
    for KeyValue(id, version) in config.PublicDependencies do
        let package = Path.Combine(directory, "cache", id.ToLowerInvariant(), version.ToLowerInvariant())
        let fable = Path.Combine(package, "fable")
        let sources = expected.GetProperty(id).EnumerateObject() |> Seq.map (fun item -> item.Name, item.Value.GetString()) |> Map.ofSeq
        let actual = Directory.GetFiles(fable, "*.fs", SearchOption.AllDirectories) |> Array.map (fun file -> "fable/" + Path.GetRelativePath(fable, file).Replace('\\', '/'), sha256 file) |> Map.ofArray
        ensure (not sources.IsEmpty && sources = actual) $"Published {id} Fable sources differ from the Pi lifecycle gate"
    run tool [project; "--outDir"; Path.Combine(directory, "fable-out")] (Path.Combine(directory, "logs/fable.log"))
    run "node" [Path.Combine(directory, "check.mjs")] (Path.Combine(directory, "logs/runtime.log"))
    printfn "Public support %s builds, emits through Fickl and passes binding runtime assertions" version

let normalized (text: string) =
    text.Replace("\r\n", "\n").Split('\n') |> Array.map _.TrimEnd() |> Array.skipWhile String.IsNullOrWhiteSpace |> Array.rev |> Array.skipWhile String.IsNullOrWhiteSpace |> Array.rev

let isExcerpt (block: string) (source: string) =
    let wanted, lines = normalized block, normalized source
    if wanted.Length = 0 then true
    elif wanted.Length > lines.Length then false
    else
        let indent (s: string) = s.Length - s.TrimStart(' ').Length
        [0 .. lines.Length - wanted.Length]
        |> List.exists (fun start ->
            let window = lines[start .. start + wanted.Length - 1]
            Array.forall2 (fun (a: string) (b: string) -> a.Trim() = b.Trim()) window wanted
            && (Array.zip window wanted |> Array.filter (fun (_, b) -> not (String.IsNullOrWhiteSpace b)) |> Array.map (fun (a, b) -> indent a - indent b) |> Array.distinct |> Array.length) <= 1)

let checkSnippets jsRoot =
    let sources extension folder =
        Directory.GetFiles(folder, extension, SearchOption.AllDirectories)
        |> Array.filter (fun p -> p.Split(Path.DirectorySeparatorChar) |> Array.exists (fun x -> x = "bin" || x = "obj" || x = "fable_modules") |> not)
        |> Array.map File.ReadAllText
    let fs = Array.append (sources "*.fs" (Path.Combine(root, "site/examples"))) [| File.ReadAllText(Path.Combine(root, "tests/AgentsPi/Client.fs")) |]
    let js = jsRoot |> Option.map (sources "*.js") |> Option.defaultValue [||]
    let fence = Regex("^```(\\w+)[^\\n]*\\n(.*?)^```[ \\t]*$", RegexOptions.Multiline ||| RegexOptions.Singleline)
    let pre = Regex("<pre([^>]*)><code[^>]*>(.*?)</code></pre>", RegexOptions.Singleline)
    let pages = Directory.GetFiles(Path.Combine(root, "site/content"), "*.md", SearchOption.AllDirectories)
    let mutable count, failures = 0, 0
    for page in pages do
        let text = File.ReadAllText page
        let blocks = seq {
            for m in fence.Matches text do yield m.Groups[1].Value.ToLowerInvariant(), m.Groups[2].Value
            for m in pre.Matches text do
                let language = if m.Groups[1].Value.Contains "data-lang=\"javascript\"" then "javascript" else "fsharp"
                yield language, WebUtility.HtmlDecode(Regex.Replace(m.Groups[2].Value, "<[^>]+>", ""))
        }
        for language, block in blocks do
            if language = "fsharp" || (language = "javascript" && jsRoot.IsSome) then
                count <- count + 1
                let candidates = if language = "fsharp" then fs else js
                if not (candidates |> Array.exists (isExcerpt block)) then
                    failures <- failures + 1
                    eprintfn "Unmatched %s snippet: %s" language (relative page)
    ensure (failures = 0) $"{failures} snippets do not match the checked sources"
    printfn "Checked %d snippets on %d pages" count pages.Length
