import copy
import importlib.util
import json
from pathlib import Path
import tempfile
import unittest
import xml.etree.ElementTree as ET

spec = importlib.util.spec_from_file_location("hawaii_groups", Path(__file__).resolve().parents[1] / "generate.py")
pipeline = importlib.util.module_from_spec(spec)
spec.loader.exec_module(pipeline)


class GroupedOutput(unittest.TestCase):
    def setUp(self):
        temporary = tempfile.TemporaryDirectory(prefix="hawaii-grouped-output-")
        self.addCleanup(temporary.cleanup)
        self.work = Path(temporary.name)
        self.output = self.work / "Generated"
        self.output.mkdir()
        self.root = self.work / "Consumer"
        self.root.mkdir()
        self.stage = self.work / "Staged"
        self.run = self.work / "run"
        self.run.mkdir()
        self.groups = [{"project": "FSharp.CloudEdge.Management.Compute", "operationIds": ["Build"]},
                       {"project": "FSharp.CloudEdge.Tenancy", "operationIds": ["Account"]}]
        core = "FSharp.CloudEdge.Core.Api"
        self.write(self.output / f"{core}.fsproj", '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>netstandard2.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include="OpenApiHttp.fs" /><Compile Include="Types.fs" /></ItemGroup></Project>')
        self.write(self.output / "Types.fs", "namespace Canonical\ntype Model = { Value: string }\n")
        self.write(self.output / "OpenApiHttp.fs", "namespace Canonical\nmodule Http = let value = 1\n")
        clients = []
        for group in self.groups:
            name = group["project"]
            self.write(self.output / name / f"{name}.fsproj", f'<Project Sdk="Microsoft.NET.Sdk"><ItemGroup><Compile Include="Client.fs" /><ProjectReference Include="../{core}.fsproj" /></ItemGroup></Project>')
            self.write(self.output / name / "Client.fs", f"namespace {name}\nopen Canonical\n")
            clients.append(group | {"projectFile": f"{name}/{name}.fsproj", "sourceFile": f"{name}/Client.fs"})
        self.manifest = {"schemaVersion": 1, "sharedProject": core, "clients": clients}
        self.write(self.output / "client-groups.json", json.dumps(self.manifest))

    def write(self, path, text):
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(text)

    def stage_output(self):
        return pipeline.stage_grouped_projects(self.output, self.groups, self.stage)

    def test_taxonomy_is_an_exact_partition_and_exclusions_are_retained_outside_clients(self):
        coverage = [{"operationId": "Build", "disposition": "included", "taxonomy": {"area": "Management", "purpose": "Compute"}},
                    {"operationId": "Account", "disposition": "included", "taxonomy": {"area": "Tenancy", "purpose": "Tenancy"}},
                    {"operationId": "Retired", "disposition": "excluded-retired", "taxonomy": None}]
        self.assertEqual(pipeline.grouped_clients(coverage), self.groups)
        for bad in [coverage + [coverage[0]], [coverage[0] | {"taxonomy": None}], [coverage[0] | {"operationId": ""}]]:
            with self.assertRaises(ValueError):
                pipeline.grouped_clients(bad)

    def test_layout_preserves_source_bytes_order_and_one_core_reference(self):
        receipt = self.stage_output()
        self.assertEqual((self.stage / "src/Core/FSharp.CloudEdge.Core.Api/Types.fs").read_bytes(), (self.output / "Types.fs").read_bytes())
        for group in self.groups:
            name = group["project"]
            target = self.stage / pipeline.project_directory(name)
            self.assertEqual((target / "Client.fs").read_bytes(), (self.output / name / "Client.fs").read_bytes())
            xml = ET.parse(target / f"{name}.fsproj")
            refs = xml.findall(".//ProjectReference")
            self.assertEqual(len(refs), 1)
            self.assertEqual((target / refs[0].get("Include")).resolve(), self.stage / "src/Core/FSharp.CloudEdge.Core.Api/FSharp.CloudEdge.Core.Api.fsproj")
        self.assertEqual(len(receipt["files"]), 8)
        self.assertEqual([folder.get("Name") for folder in ET.parse(self.stage / "Hawaii.Bindings.slnx").findall("Folder")], ["/Core/", "/Management/", "/Tenancy/"])

    def test_wrong_or_missing_generated_owner_is_rejected(self):
        for mutation in [lambda x: x["clients"].pop(), lambda x: x["clients"][0].update(operationIds=["Account"]),
                         lambda x: x["clients"].append(x["clients"][0])]:
            manifest = copy.deepcopy(self.manifest)
            mutation(manifest)
            self.write(self.output / "client-groups.json", json.dumps(manifest))
            with self.assertRaises(ValueError):
                self.stage_output()

    def test_escaping_artifact_path_is_rejected(self):
        self.manifest["clients"][0]["projectFile"] = "../unowned.fsproj"
        self.write(self.output / "client-groups.json", json.dumps(self.manifest))
        with self.assertRaises(ValueError):
            self.stage_output()

    def test_install_refuses_unowned_or_user_edited_output(self):
        receipt = self.stage_output()
        name = "src/Core/FSharp.CloudEdge.Core.Api/Types.fs"
        user = self.root / name
        self.write(user, "user source")
        with self.assertRaisesRegex(ValueError, "unowned"):
            pipeline.install_grouped_projects(self.stage, self.root, receipt, self.run)
        self.assertEqual(user.read_text(), "user source")
        user.unlink()
        pipeline.install_grouped_projects(self.stage, self.root, receipt, self.run)
        self.write(user, "user changed generated source")
        with self.assertRaisesRegex(ValueError, "edited or removed"):
            pipeline.install_grouped_projects(self.stage, self.root, receipt, self.work / "another-run")
        self.assertEqual(user.read_text(), "user changed generated source")

    def test_reinstall_preserves_previous_owned_bytes_and_unrelated_files(self):
        receipt = self.stage_output()
        unrelated = self.root / "user.fs"
        self.write(unrelated, "user code")
        pipeline.install_grouped_projects(self.stage, self.root, receipt, self.run)
        installed_times = {name: (self.root / name).stat().st_mtime_ns for name in receipt["files"]}
        next_run = self.work / "next-run"
        next_run.mkdir()
        pipeline.install_grouped_projects(self.stage, self.root, receipt, next_run)
        for name in receipt["files"]:
            self.assertEqual((next_run / "previous-output" / name).read_bytes(), (self.root / name).read_bytes())
            self.assertEqual((self.root / name).stat().st_mtime_ns, installed_times[name])
        self.assertEqual(unrelated.read_text(), "user code")

    def test_post_validation_source_change_is_rejected_before_install(self):
        receipt = self.stage_output()
        (self.stage / "src/Core/FSharp.CloudEdge.Core.Api/Types.fs").write_text("changed after build")
        with self.assertRaisesRegex(ValueError, "changed after validation"):
            pipeline.install_grouped_projects(self.stage, self.root, receipt, self.run)
        self.assertFalse((self.root / "src").exists())

    def test_receipt_cannot_claim_unrelated_consumer_files(self):
        receipt = self.stage_output()
        readme = self.root / "README.md"
        self.write(readme, "user README")
        self.write(self.root / "inventory/hawaii-output-ownership.json", json.dumps({"schemaVersion": 1, "files": {"README.md": pipeline.digest(readme.read_bytes())}}))
        with self.assertRaises(ValueError):
            pipeline.install_grouped_projects(self.stage, self.root, receipt, self.run)
        self.assertEqual(readme.read_text(), "user README")


if __name__ == "__main__":
    unittest.main()
