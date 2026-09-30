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

let installFable directory =
    let tool = Path.Combine(directory, "tools", if OperatingSystem.IsWindows() then "fable.exe" else "fable")
    let source = Path.Combine(directory, "Tools.NuGet.Config")
    writeNuGetConfig source (Path.Combine(directory, "tool-cache")) false
    if not (File.Exists tool) then
        run "dotnet" ["tool"; "install"; "fable"; "--version"; "5.13.0"; "--tool-path"; Path.GetDirectoryName tool; "--configfile"; source] (Path.Combine(directory, "logs/fable-install.log"))
    tool

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

let buildExamples (projects: string array) publicFeed emit =
    let mode, directory = prepareConsumerDirectory publicFeed
    let tool = if emit then installFable directory else ""
    let results = ResizeArray<ConsumerResult>()
    for index, original in Array.indexed projects do
        let name = Path.GetFileName(Path.GetDirectoryName original)
        let project = Path.Combine(directory, "examples", name, Path.GetFileName original)
        printfn "[%d/%d] %s package consumer: %s" (index + 1) projects.Length mode name
        let mutable built = false
        let mutable emitted = "not run"
        let mutable error = ""
        try
            run "dotnet" ["build"; project; "-c"; "Release"; "--nologo"] (Path.Combine(directory, "logs", name + "-build.log"))
            recordResolvedPackages directory name project
            built <- true
            let kind = elements "ExampleKind" (XDocument.Load project) |> Array.tryHead
            if emit && (kind |> Option.forall (fun n -> n.Value <> "DotNet")) then
                emitted <- "failed"
                run tool [project; "--outDir"; Path.Combine(directory, "js", name)] (Path.Combine(directory, "logs", name + "-fable.log"))
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
    let tool = installFable directory
    for file in ["Smoke.fs"; "check.mjs"] do
        copyFile (Path.Combine(root, "tests/SupportPackage", file)) (Path.Combine(directory, file))
    let version = config.PublicDependencies["Xantham.Fable.Core"]
    let tsVersion = config.PublicDependencies["Xantham.Fable.Core.TS"]
    let project = Path.Combine(directory, "PublicSupport.fsproj")
    xml "Project" ["Sdk", "Microsoft.NET.Sdk"] [
        xml "PropertyGroup" [] [valueElement "TargetFramework" "net8.0"]
        xml "ItemGroup" [] [
            xml "Compile" ["Include", "Smoke.fs"] []
            xml "PackageReference" ["Include", "Fable.Core"; "Version", "5.2.0"] []
            xml "PackageReference" ["Include", "Xantham.Fable.Core"; "Version", $"[{version}]"] []
            xml "PackageReference" ["Include", "Xantham.Fable.Core.TS"; "Version", $"[{tsVersion}]"] []
        ]
    ] |> saveXml project
    run "dotnet" ["build"; project; "--nologo"] (Path.Combine(directory, "logs/build.log"))
    run tool [project; "--outDir"; Path.Combine(directory, "fable-out")] (Path.Combine(directory, "logs/fable.log"))
    run "node" [Path.Combine(directory, "check.mjs")] (Path.Combine(directory, "logs/runtime.log"))
    printfn "Public support %s builds, emits through Fable and passes binding runtime assertions" version

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
    let fs = sources "*.fs" (Path.Combine(root, "site/examples"))
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
