import contextlib
import importlib.util
import io
import json
import os
from pathlib import Path
import sys
import tempfile
import unittest
from unittest import mock

spec = importlib.util.spec_from_file_location("hawaii_tool_consumer", Path(__file__).resolve().parents[1] / "generate.py")
pipeline = importlib.util.module_from_spec(spec)
spec.loader.exec_module(pipeline)


class PackagedToolInvocation(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix="cloudedge-hawaii-package-")
        self.addCleanup(self.temporary.cleanup)
        self.root = Path(self.temporary.name)
        self.here = self.root / "generators/hawaii"
        self.here.mkdir(parents=True)
        (self.root / "scripts").mkdir()
        (self.root / "bin").mkdir()
        self.addCleanup(mock.patch.stopall)
        mock.patch.object(pipeline, "ROOT", self.root).start()
        mock.patch.object(pipeline, "HERE", self.here).start()
        mock.patch.dict(os.environ, {"PATH": str(self.root / "bin") + os.pathsep + os.environ["PATH"]}).start()
        schema = {"paths": {"/old": {"get": {"operationId": "old", "deprecated": True}},
                            "/current": {"post": {"operationId": "current"}}},
                  "components": {"schemas": {"Shared": {"type": "string"}}}}
        self.schema = self.root / "original.json"
        self.schema.write_text(json.dumps(schema))
        schema_hash = pipeline.digest(self.schema.read_bytes())
        self.write("generators/hawaii/pins.json", {"schemaVersion": 1, "schema": {"url": "https://example.invalid/schema.json", "sha256": schema_hash, "operationCount": 2},
                   "hawaii": {"tool": "hawaii"}, "pipelineInputs": ["scripts/tool-packages.mjs"],
                   "overlays": [], "taxonomy": "taxonomy.json", "schemaPolicy": "policy.json", "operationPolicy": "lifecycle.json"})
        self.write("generators/hawaii/taxonomy.json", {"schemaSha256": schema_hash, "rules": [
            {"path": path, "match": "exact", "area": "Management", "purpose": "Fixture", "family": "sample"} for path in schema["paths"]]})
        self.write("generators/hawaii/policy.json", {"schemaSha256": schema_hash, "schemaIds": []})
        self.write("generators/hawaii/lifecycle.json", {"schemaSha256": schema_hash, "asOf": "2026-09-06", "operations": [
            {"method": "get", "path": "/old", "operationId": "old", "disposition": "exclude-retired", "retiredAt": "2025-01-01", "reason": "Fixture", "sources": ["https://example.invalid/retirement"]}]})
        self.fingerprint = {"packageId": "Hawaii.Unofficial", "command": "hawaii-unofficial", "version": "1.0.0-local.fixture",
                            "packageSha256": "a" * 64, "payloadSha256": {"Hawaii.dll": "b" * 64}, "manifestSha256": "c" * 64}
        self.write("tool-evidence.json", {"args": ["tool", "run", "hawaii-unofficial", "--"], "fingerprint": self.fingerprint,
                   "assembly": str(self.root / "isolated-package-cache/Hawaii.dll")})
        (self.root / "scripts/tool-packages.mjs").write_text('''import fs from 'node:fs';
fs.appendFileSync('calls.jsonl', JSON.stringify({kind:'resolve',args:process.argv.slice(2),cwd:process.cwd()})+'\\n');
if(fs.existsSync('resolver-failure')) {process.stderr.write('Bootstrap generator tools first: npm run tools:bootstrap');process.exit(1);}
process.stdout.write(fs.readFileSync('tool-evidence.json','utf8'));
''')
        dotnet = self.root / "bin/dotnet"
        dotnet.write_text(f"#!{sys.executable}\n" + '''import json, pathlib, sys
root=pathlib.Path.cwd()
args=sys.argv[1:]
with (root/'calls.jsonl').open('a') as stream:stream.write(json.dumps({'kind':'dotnet','args':args,'cwd':str(root)})+'\\n')
if args[:4]==['tool','run','hawaii-unofficial','--']:
    config=json.loads(pathlib.Path(args[args.index('--config')+1]).read_text())
    output=pathlib.Path(config['output']);output.mkdir()
    (output/'Client.fs').write_text('module Generated\\n')
    (output/'FSharp.CloudEdge.Management.fsproj').write_text('<Project />')
    if (root/'mutate-tool').exists():
        field=(root/'mutate-tool').read_text()
        evidence=json.loads((root/'tool-evidence.json').read_text())
        evidence['fingerprint'][field]={'Hawaii.dll':'changed'} if field=='payloadSha256' else 'changed'
        (root/'tool-evidence.json').write_text(json.dumps(evidence))
    if (root/'fail-generation').exists():sys.exit(7)
    if (root/'mutate-pipeline').exists():
        source=root/'scripts/tool-packages.mjs'
        source.write_text(source.read_text()+'\\n')
elif not (args and args[0]=='build' and '/Generated/' in args[1]):
    sys.exit('Unexpected source build or direct assembly invocation')
''')
        dotnet.chmod(0o755)
        self.run_directory = self.root / "artifacts/run"

    def write(self, file, value):
        (self.root / file).write_text(json.dumps(value))

    def run_pipeline(self):
        with contextlib.redirect_stdout(io.StringIO()):
            pipeline.main(["--schema", str(self.schema), "--run-directory", str(self.run_directory), "--single-client"])

    def calls(self):
        file = self.root / "calls.jsonl"
        return [json.loads(line) for line in file.read_text().splitlines()] if file.exists() else []

    def provenance(self):
        return json.loads((self.run_directory / "provenance.json").read_text())

    def test_installed_tool_invocation_and_receipt_preserve_lifecycle_and_config_paths(self):
        self.run_pipeline()
        calls = self.calls()
        self.assertEqual([call["kind"] for call in calls], ["resolve", "dotnet", "resolve", "dotnet"])
        self.assertTrue(all(call["cwd"] == str(self.root) for call in calls))
        self.assertEqual(calls[0]["args"], ["hawaii"])
        self.assertEqual(calls[1]["args"], ["tool", "run", "hawaii-unofficial", "--", "--config", str(self.run_directory / "hawaii.json"), "--no-logo"])
        self.assertEqual(calls[-1]["args"][0], "build")
        self.assertEqual(self.provenance()["hawaiiTool"], self.fingerprint)
        self.assertEqual(self.provenance()["status"], "compiled")
        self.assertEqual(self.provenance()["pipelineSources"], {
            "scripts/tool-packages.mjs": pipeline.digest((self.root / "scripts/tool-packages.mjs").read_bytes())})
        self.assertEqual((self.provenance()["sourceOperations"], self.provenance()["includedOperations"]), (2, 1))
        self.assertFalse(any(key in self.provenance() for key in ["hawaiiHead", "hawaiiSources", "hawaiiAssemblies"]))
        config = json.loads((self.run_directory / "hawaii.json").read_text())
        self.assertEqual(config["schema"], str(self.run_directory / "schema-selected.json"))
        self.assertEqual(config["project"], "FSharp.CloudEdge.Management")
        self.assertNotIn("clientGroups", config)
        selected = json.loads((self.run_directory / "schema-selected.json").read_text())
        self.assertEqual(set(selected["paths"]), {"/current"})
        self.assertIn("Shared", selected["components"]["schemas"])

    def test_changed_pipeline_rejects_output_before_compilation(self):
        (self.root / "mutate-pipeline").write_text("yes")
        with self.assertRaisesRegex(RuntimeError, "pipeline sources changed"):
            self.run_pipeline()
        self.assertEqual(self.provenance()["status"], "failed")
        self.assertFalse(any(call["kind"] == "dotnet" and call["args"][0] == "build" for call in self.calls()))

    def test_changed_package_payload_or_manifest_rejects_output_before_compilation(self):
        for field in ["packageSha256", "payloadSha256", "manifestSha256"]:
            with self.subTest(field=field):
                self.write("tool-evidence.json", {"args": ["tool", "run", "hawaii-unofficial", "--"], "fingerprint": self.fingerprint})
                (self.root / "mutate-tool").write_text(field)
                self.run_directory = self.root / f"artifacts/{field}"
                with self.assertRaisesRegex(RuntimeError, "tool changed during generation"):
                    self.run_pipeline()
                self.assertEqual(self.provenance()["status"], "failed")
                self.assertNotIn("generatedSources", self.provenance())
        self.assertFalse(any(call["args"][0] == "build" for call in self.calls() if call["kind"] == "dotnet"))

    def test_missing_bootstrap_fails_without_attempting_a_source_build(self):
        (self.root / "resolver-failure").touch()
        with self.assertRaisesRegex(RuntimeError, "Bootstrap generator tools first"):
            self.run_pipeline()
        self.assertEqual([call["kind"] for call in self.calls()], ["resolve"])
        self.assertEqual(self.provenance()["status"], "failed")

    def test_incomplete_resolver_evidence_is_not_accepted(self):
        self.write("tool-evidence.json", {"args": ["tool", "run", "hawaii-unofficial", "--"], "fingerprint": {"command": "hawaii-unofficial"}})
        with self.assertRaisesRegex(ValueError, "incomplete package evidence"):
            self.run_pipeline()
        self.assertEqual([call["kind"] for call in self.calls()], ["resolve"])

    def test_failed_tool_remains_failed_and_does_not_compile_partial_output(self):
        (self.root / "fail-generation").touch()
        with self.assertRaisesRegex(RuntimeError, "generate failed"):
            self.run_pipeline()
        self.assertEqual([call["kind"] for call in self.calls()], ["resolve", "dotnet"])
        self.assertEqual(self.provenance()["status"], "failed")
        self.assertEqual(self.provenance()["hawaiiTool"], self.fingerprint)

    def test_source_root_override_is_no_longer_a_generation_argument(self):
        with contextlib.redirect_stderr(io.StringIO()), self.assertRaises(SystemExit) as result:
            pipeline.main(["--hawaii-root", "/source/checkout"])
        self.assertEqual(result.exception.code, 2)
        self.assertFalse(self.run_directory.exists())

    def test_legacy_source_pins_cannot_reenable_source_generation(self):
        pins = json.loads((self.here / "pins.json").read_text())
        pins["hawaii"] = {"sourceDirectory": "../Hawaii", "baseCommit": "old", "project": "src/Hawaii.fsproj"}
        self.write("generators/hawaii/pins.json", pins)
        with self.assertRaisesRegex(ValueError, "must select the packaged hawaii tool"):
            self.run_pipeline()
        self.assertEqual(self.calls(), [])


if __name__ == "__main__":
    unittest.main()
