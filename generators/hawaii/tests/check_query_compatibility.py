#!/usr/bin/env python3
"""Compare generated query transport with recorded, pinned official SDK output."""
import argparse
import importlib.util
import json
from pathlib import Path
import subprocess
import tempfile

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
spec = importlib.util.spec_from_file_location("hawaii_consumer", HERE.parent / "generate.py")
pipeline = importlib.util.module_from_spec(spec)
spec.loader.exec_module(pipeline)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.parse_args()
    work = Path(tempfile.mkdtemp(prefix="hawaii-sdk-query-compatibility-"))

    def run(command):
        result = subprocess.run(command, cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        if result.returncode:
            raise RuntimeError(result.stdout)
        return result.stdout

    schema = {"openapi": "3.0.3", "info": {"title": "Query encoding", "version": "1"},
              "paths": {}, "components": {"schemas": {"Dummy": {"type": "string"}}}}
    (work / "schema.json").write_text(json.dumps(schema))
    config = {"schema": str(work / "schema.json"), "output": str(work / "Generated"),
              "project": "QueryCompatibility", "target": "fsharp"}
    (work / "hawaii.json").write_text(json.dumps(config))
    pins = json.loads((HERE.parent / "pins.json").read_text())
    provenance = {}
    try:
        pipeline.run_hawaii(pins["hawaii"], work / "hawaii.json", work, provenance)
    finally:
        pipeline.write_json(work / "provenance.json", provenance)
    (work / "Check.fs").write_text('''module Check
open System
open System.Text.Json.Nodes
open QueryCompatibility.Http
[<EntryPoint>]
let main args =
    let fixtures = JsonNode.Parse(System.IO.File.ReadAllText(args.[0]))
    for entry in fixtures.["cases"].AsArray() do
        let request = RequestPart.queryDotted("filters", entry.["input"].["filters"])
        let path = OpenApiHttp.applyQueryStringParameters "/check" [request]
        let actual = if path.Contains "?" then path.Substring(path.IndexOf('?') + 1) else ""
        let expected = entry.["expected"].GetValue<string>()
        if actual <> expected then failwithf "SDK query mismatch: expected %s, got %s" expected actual
    printfn "Passed %d pinned SDK outgoing-query comparisons." (fixtures.["cases"].AsArray().Count)
    0
''')
    (work / "Check.fsproj").write_text('''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup>
<ItemGroup><ProjectReference Include="Generated/QueryCompatibility.fsproj" /><Compile Include="Check.fs" /></ItemGroup>
</Project>
''')
    print(run(["dotnet", "run", "--project", str(work / "Check.fsproj"), "--", str(HERE / "sdk-query-compatibility.json")]).strip())
    print(f"Generated check: {work}")


if __name__ == "__main__":
    main()
