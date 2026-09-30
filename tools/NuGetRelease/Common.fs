module NuGetRelease.Common

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Security.Cryptography
open System.Text.Json
open System.Xml.Linq

type ReleaseConfig = {
    Version: string
    Solution: string
    Repository: string
    ProjectUrl: string
    Authors: string
    PublicDependencies: Dictionary<string, string>
}

type Package = {
    Id: string
    Path: string
    Framework: string
    Fable: bool
    Sources: string array
    Dependencies: Map<string, string>
    External: Map<string, string>
}

type ManifestEntry = {
    Id: string
    Version: string
    File: string
    Sha256: string
    Dependencies: Dictionary<string, string>
    Url: string
}

let root = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "../.."))
let jsonOptions = JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true, WriteIndented = true)
let readJson<'T> path = JsonSerializer.Deserialize<'T>(File.ReadAllText path, jsonOptions)
let writeJson path value = File.WriteAllText(path, JsonSerializer.Serialize(value, jsonOptions) + "\n")
let config = readJson<ReleaseConfig> (Path.Combine(root, "config/nuget-release.json"))
let output = Path.Combine(root, "artifacts/nuget-release", config.Version)
let feed = Path.Combine(output, "packages")
let ensure condition message = if not condition then failwith message
let mkdir path = Directory.CreateDirectory path |> ignore
let xn name = XName.Get name
let attr name (node: XElement) = node.Attribute(xn name).Value
let elements name (doc: XDocument) = doc.Descendants(xn name) |> Seq.toArray
let element name (doc: XDocument) = elements name doc |> Array.exactlyOne
let xml name attrs children =
    let node = XElement(xn name)
    for key, value in attrs do node.SetAttributeValue(xn key, value)
    for child: XElement in children do node.Add child
    node
let valueElement (name: string) (value: string) = XElement(xn name, value)
let saveXml path node = XDocument(node: XElement).Save(path: string)
let sha256 path = File.ReadAllBytes path |> SHA256.HashData |> Convert.ToHexString |> _.ToLowerInvariant()
let relative path = Path.GetRelativePath(root, path).Replace('\\', '/')

// ArgumentList preserves each argument verbatim; credentials never enter a shell command string.
let run (executable: string) (arguments: string list) (log: string) =
    mkdir (Path.GetDirectoryName log)
    let start = ProcessStartInfo(executable, WorkingDirectory = root, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false)
    arguments |> Seq.iter start.ArgumentList.Add
    use proc = Process.Start start
    let stdout = proc.StandardOutput.ReadToEndAsync()
    let stderr = proc.StandardError.ReadToEndAsync()
    proc.WaitForExit()
    let text = stdout.Result + stderr.Result
    File.WriteAllText(log, text)
    if proc.ExitCode <> 0 then
        text.Split('\n') |> Seq.rev |> Seq.truncate 25 |> Seq.rev |> Seq.iter (eprintfn "%s")
        failwith $"{Path.GetFileName executable} failed ({proc.ExitCode}); see {log}"

let capture executable arguments =
    let start = ProcessStartInfo(executable, WorkingDirectory = root, RedirectStandardOutput = true, UseShellExecute = false)
    arguments |> Seq.iter start.ArgumentList.Add
    use proc = Process.Start start
    let result = proc.StandardOutput.ReadToEnd()
    proc.WaitForExit()
    ensure (proc.ExitCode = 0) $"{executable} failed"
    result.Trim()

let writeNuGetConfig path cache candidate =
    let source name url = xml "add" ["key", name; "value", url] []
    let sources = [
        xml "clear" [] []
        if candidate then source "candidate" feed
        source "nuget.org" "https://api.nuget.org/v3/index.json"
    ]
    let mapping name pattern = xml "packageSource" ["key", name] [xml "package" ["pattern", pattern] []]
    xml "configuration" [] [
        xml "config" [] [source "globalPackagesFolder" cache]
        xml "packageSources" [] sources
        xml "packageSourceMapping" [] [
            if candidate then mapping "candidate" "FSharp.CloudEdge.*"
            mapping "nuget.org" "*"
        ]
    ] |> saveXml path

let copyFile (source: string) (destination: string) =
    mkdir (Path.GetDirectoryName destination)
    File.Copy(source, destination, true)

let rec copyTree source destination =
    mkdir destination
    for file in Directory.GetFiles source do
        if Path.GetFileName file <> "NuGet.Config" then copyFile file (Path.Combine(destination, Path.GetFileName file))
    for directory in Directory.GetDirectories source do
        if not (Set.contains (Path.GetFileName directory) (set ["bin"; "obj"; "fable_modules"])) then
            copyTree directory (Path.Combine(destination, Path.GetFileName directory))
