module CloudEdge.Generation

open System
open System.IO
open System.Text.Json
open Xantham.Generator
open Xantham.Generator.Customization
open Xantham.Generator.Myriad

[<EntryPoint>]
let main arguments =
    try
        let option name =
            let index = arguments |> Array.findIndex ((=) name)
            arguments[index + 1]

        if arguments.Length < 2 || arguments[0] <> "generate" then
            invalidArg "arguments" "Expected generate <package> --config <json> --out <directory> [--workspace <directory> --projection <json>]"

        let config = GeneratorConfig.loadFile (option "--config")

        // Ordinary targets run the Xantham CLI pipeline; projected targets also compile-validate their projections.
        let run =
            if arguments |> Array.contains "--projection" then
                let text = File.ReadAllText(option "--projection")
                use document = JsonDocument.Parse text
                let selections =
                    document.RootElement.GetProperty("selections").EnumerateArray()
                    |> Seq.map (fun selection ->
                        let string (name: string) = selection.GetProperty(name).GetString()
                        { Package = string "package"
                          ReceiverPath = selection.GetProperty("receiverPath").EnumerateArray() |> Seq.map _.GetString() |> Seq.toList
                          MethodName = string "methodName"
                          ParameterName = string "parameterName"
                          FieldName = match selection.TryGetProperty "fieldName" with true, field when field.ValueKind = JsonValueKind.String -> Some(field.GetString()) | _ -> None
                          ReceiverType = string "receiverType"
                          ModuleName = string "moduleName"
                          TypeName = string "typeName"
                          FunctionName = string "functionName" })
                    |> Seq.toList
                let workspace = option "--workspace"
                let identity = { Id = "cloudedge.operations"; Version = "1"; Configuration = Map.ofList [ "selections", text ] }
                let references = [ "Xantham.Fable.Core"; "Xantham.Fable.Core.TS" ] |> List.map (fun name -> Path.Combine(AppContext.BaseDirectory, name + ".dll"))
                let compiler = Compiler.dotnet workspace references
                Pipeline.runProjectedWith compiler [ Operations.createShared identity workspace selections ] [] config arguments[1] (option "--out")
            else
                Pipeline.run config arguments[1] (option "--out")
        let report = run |> Async.RunSynchronously
        for file in report.OutputFiles do printfn "%s" file
        printfn "Exact %d; ergonomic %d; widened %d; escape %d" report.Counts.Exact report.Counts.Ergonomic report.Counts.Widened report.Counts.Escape
        0
    with error ->
        eprintfn "%O" error
        1
