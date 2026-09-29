#!/usr/bin/env python3
"""Generate and compile a canonical Cloudflare API model from pinned source."""
import argparse
import copy
from datetime import date
import hashlib
import json
import os
import re
import xml.etree.ElementTree as ET
from pathlib import Path
import resource
import subprocess
import time
import urllib.request

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[1]
METHODS = {"get", "post", "put", "patch", "delete", "head", "options", "trace", "query"}


def digest(data):
    return hashlib.sha256(data).hexdigest()


def write_json(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2) + "\n")


def pointer_parent(document, pointer):
    if not pointer.startswith("/"):
        raise ValueError(f"Expected JSON pointer: {pointer}")
    parts = [p.replace("~1", "/").replace("~0", "~") for p in pointer[1:].split("/")]
    current = document
    for part in parts[:-1]:
        if isinstance(current, list):
            if not part.isdecimal() or str(int(part)) != part:
                raise ValueError(f"Expected array index at {pointer}")
            current = current[int(part)]
        else:
            current = current[part]
    if not isinstance(current, dict):
        raise ValueError(f"Expected object at {pointer}")
    return current, parts[-1]


def apply_overlay(document, patch):
    """Apply object patches, requiring an exact adjacent test before replacement."""
    result = copy.deepcopy(document)
    tested_path = None
    for item in patch:
        if item["op"] not in {"add", "move", "test", "replace"}:
            raise ValueError(f"Unsupported overlay operation: {item['op']}")
        parent, key = pointer_parent(result, item["path"])
        if item["op"] == "test":
            # Exact source-value matching deliberately distinguishes JSON booleans
            # from numbers and integer spellings from floating point spellings.
            if key not in parent or json.dumps(parent[key], sort_keys=True) != json.dumps(item["value"], sort_keys=True):
                raise ValueError(f"Overlay source test failed: {item['path']}")
            tested_path = item["path"]
            continue
        if item["op"] == "replace":
            if tested_path != item["path"] or key not in parent:
                raise ValueError(f"Overlay replacement requires an adjacent source test: {item['path']}")
            parent[key] = copy.deepcopy(item["value"])
        else:
            if key in parent:
                raise ValueError(f"Overlay destination already exists: {item['path']}")
            if item["op"] == "add":
                parent[key] = copy.deepcopy(item["value"])
            else:
                source, name = pointer_parent(result, item["from"])
                parent[key] = source.pop(name)
        tested_path = None
    return result


def operations(document):
    return {(method, path): value for path, item in document["paths"].items()
            for method, value in item.items() if method in METHODS}


def select_lifecycle(document, policy):
    """Apply reviewed operation decisions at an explicit, reproducible date."""
    result = copy.deepcopy(document)
    source = operations(document)
    excluded = {}
    decisions = policy.get("operations", [])

    def policy_date(value):
        if not isinstance(value, str) or len(value) != 10:
            raise ValueError("Operation policy dates must use YYYY-MM-DD")
        parsed = date.fromisoformat(value)
        if parsed.isoformat() != value:
            raise ValueError("Operation policy dates must use YYYY-MM-DD")
        return parsed

    reviewed_at = policy_date(policy.get("asOf")) if decisions else None
    for entry in decisions:
        key = (entry["method"], entry["path"])
        if key not in source or key in excluded:
            raise ValueError(f"Unknown or duplicate lifecycle operation: {key}")
        if source[key].get("operationId") != entry["operationId"]:
            raise ValueError(f"Operation identity changed: {key}")
        if entry.get("disposition") not in {"exclude-retired", "exclude-internal"}:
            raise ValueError(f"Unsupported operation disposition: {key}")
        if not isinstance(entry.get("reason"), str) or not entry["reason"].strip() or not isinstance(entry.get("sources"), list) or not entry["sources"]:
            raise ValueError(f"Operation exclusion needs a reason and primary sources: {key}")
        if any(not isinstance(url, str) or not url.startswith("https://") for url in entry["sources"]):
            raise ValueError(f"Operation source must be an HTTPS primary-source URL: {key}")
        if entry["disposition"] == "exclude-retired":
            if policy_date(entry.get("retiredAt")) > reviewed_at:
                raise ValueError(f"Operation has not retired at the policy review date: {key}")
        elif "retiredAt" in entry:
            raise ValueError(f"Internal exposure is not an operation retirement: {key}")
        excluded[key] = entry
        del result["paths"][key[1]][key[0]]
    result["paths"] = {p: item for p, item in result["paths"].items() if any(m in METHODS for m in item)}
    # Keep the shared component graph intact. Reference-closed component pruning
    # and service facades are separate steps; no per-service type copies here.
    coverage = [{"method": method, "path": path, "operationId": value.get("operationId"),
                 "deprecated": value.get("deprecated", False), "tags": value.get("tags", []),
                 "disposition": excluded[(method, path)]["disposition"].replace("exclude-", "excluded-", 1)
                                if (method, path) in excluded else "included",
                 **({"selectionEvidence": copy.deepcopy(excluded[(method, path)]), "selectionAsOf": policy["asOf"]}
                    if (method, path) in excluded else {})}
                for (method, path), value in sorted(source.items())]
    return result, coverage


def assign_taxonomy(document, coverage, taxonomy):
    source = operations(document)
    unresolved = []
    for operation in coverage:
        path = operation["path"]
        candidates = [r for r in taxonomy["rules"]
                      if path == r["path"] or (r["match"] == "prefix" and path.startswith(r["path"] + "/"))]
        if candidates:
            longest = max(len(r["path"]) for r in candidates)
            candidates = [r for r in candidates if len(r["path"]) == longest]
        if len(candidates) > 1:
            raise ValueError(f"Ambiguous taxonomy ownership: {operation['method']} {path}")
        declaration = source[(operation["method"], path)]
        operation["sourceSdkGroup"] = declaration.get("x-fern-sdk-group-name")
        operation["taxonomy"] = ({k: candidates[0][k] for k in ["area", "purpose", "family"]}
                                 if candidates else None)
        if not candidates:
            unresolved.append({k: operation[k] for k in ["method", "path", "operationId", "tags", "sourceSdkGroup"]})
    return unresolved


def grouped_clients(coverage):
    """Assign selected operations to one explicit taxonomy owner; reject gaps."""
    groups = {}
    seen = set()
    for operation in coverage:
        if operation["disposition"] != "included":
            continue
        name = operation.get("operationId")
        if not isinstance(name, str) or not name or name in seen:
            raise ValueError(f"Missing or duplicate selected operation ID: {name}")
        seen.add(name)
        taxonomy = operation.get("taxonomy")
        if not taxonomy or taxonomy.get("area") not in {"Management", "Tenancy"}:
            raise ValueError(f"Selected operation lacks a supported taxonomy owner: {name}")
        purpose = taxonomy.get("purpose")
        if not isinstance(purpose, str) or not re.fullmatch(r"[A-Z][A-Za-z0-9]*", purpose):
            raise ValueError(f"Invalid client purpose for {name}: {purpose}")
        if taxonomy["area"] == "Tenancy":
            if purpose != "Tenancy":
                raise ValueError(f"Tenancy operation has another purpose: {name}")
            project = "FSharp.CloudEdge.Tenancy"
        else:
            project = "FSharp.CloudEdge.Management." + purpose
        groups.setdefault(project, []).append(name)
    if not groups:
        raise ValueError("No operations selected for client generation")
    return [{"project": project, "operationIds": sorted(ids)} for project, ids in sorted(groups.items())]


def checked_relative(root, name):
    """Validate artifact/receipt paths before reading or replacing any file."""
    path = Path(name)
    if not name or path.is_absolute() or ".." in path.parts or "\\" in name:
        raise ValueError(f"Expected a contained relative path: {name}")
    result = root / path
    if not result.resolve().is_relative_to(root.resolve()) or result.is_symlink():
        raise ValueError(f"Path leaves its owned directory: {name}")
    return result


def project_directory(project):
    if project == "FSharp.CloudEdge.Core.Api":
        area = "Core"
    elif project == "FSharp.CloudEdge.Tenancy":
        area = "Tenancy"
    elif re.fullmatch(r"FSharp\.CloudEdge\.Management\.[A-Z][A-Za-z0-9]*", project):
        area = "Management"
    else:
        raise ValueError(f"Unexpected Hawaii project name: {project}")
    return Path("src") / area / project


def stage_grouped_projects(output, groups, stage):
    """Copy generated F# unchanged; project XML alone adapts the consumer layout."""
    manifest = json.loads((output / "client-groups.json").read_bytes())
    core = "FSharp.CloudEdge.Core.Api"
    if manifest.get("schemaVersion") != 1 or manifest.get("sharedProject") != core:
        raise ValueError("Generated ownership manifest has an unexpected shared model")
    expected = {group["project"]: group["operationIds"] for group in groups}
    actual = {}
    for client in manifest.get("clients", []):
        project = client.get("project")
        ids = client.get("operationIds", [])
        if project in actual or project not in expected or len(ids) != len(set(ids)) or sorted(ids) != sorted(expected[project]):
            raise ValueError(f"Generated operation ownership differs for {project}")
        actual[project] = client
    if set(actual) != set(expected):
        raise ValueError("Generated client manifest omits a configured project")
    stage.mkdir(parents=True, exist_ok=False)
    projects = [(core, f"{core}.fsproj", None)] + [
        (name, entry["projectFile"], entry["sourceFile"]) for name, entry in sorted(actual.items())]
    solution = ET.Element("Solution")
    folders = {}
    owned = {}
    for name, project_file, client_source in projects:
        generated_project = checked_relative(output, project_file)
        xml = ET.fromstring(generated_project.read_bytes())
        if xml.tag != "Project" or xml.get("Sdk") != "Microsoft.NET.Sdk":
            raise ValueError(f"Unexpected generated project document: {name}")
        directory = project_directory(name)
        destination = stage / directory
        destination.mkdir(parents=True)
        refs = xml.findall(".//ProjectReference")
        if name == core and refs or name != core and len(refs) != 1:
            raise ValueError(f"Unexpected shared-model dependency count: {name}")
        for reference in refs:
            source_ref = (generated_project.parent / reference.attrib["Include"]).resolve()
            if source_ref != (output / f"{core}.fsproj").resolve():
                raise ValueError(f"Client has a dependency outside the shared model: {name}")
            reference.set("Include", os.path.relpath(stage / project_directory(core) / f"{core}.fsproj", destination))
        includes = xml.findall(".//Compile")
        expected_sources = ["OpenApiHttp.fs", "Types.fs"] if name == core else ["Client.fs"]
        if [item.get("Include") for item in includes] != expected_sources:
            raise ValueError(f"Unexpected generated compilation order for {name}")
        for item in includes:
            source = checked_relative(output, (generated_project.parent.relative_to(output) / item.attrib["Include"]).as_posix())
            if name != core and source.resolve() != checked_relative(output, client_source).resolve():
                raise ValueError(f"Client source differs from ownership manifest: {name}")
            data = source.read_bytes()
            target = destination / item.attrib["Include"]
            target.write_bytes(data)
            owned[target.relative_to(stage).as_posix()] = digest(data)
        ET.indent(xml, space="  ")
        project_bytes = ET.tostring(xml, encoding="utf-8", xml_declaration=True) + b"\n"
        target = destination / f"{name}.fsproj"
        target.write_bytes(project_bytes)
        owned[target.relative_to(stage).as_posix()] = digest(project_bytes)
        area = directory.parts[1]
        if area not in folders:
            folders[area] = ET.SubElement(solution, "Folder", Name=f"/{area}/")
        ET.SubElement(folders[area], "Project", Path=target.relative_to(stage).as_posix())
    ET.indent(solution, space="  ")
    solution_bytes = ET.tostring(solution, encoding="utf-8") + b"\n"
    (stage / "Hawaii.Bindings.slnx").write_bytes(solution_bytes)
    owned["Hawaii.Bindings.slnx"] = digest(solution_bytes)
    return {"schemaVersion": 1, "sharedProject": core, "clients": groups, "files": dict(sorted(owned.items()))}


def validate_owned_path(name):
    if name == "Hawaii.Bindings.slnx":
        return
    parts = Path(name).parts
    if len(parts) != 4 or Path(*parts[:3]) != project_directory(parts[2]):
        raise ValueError(f"Path is outside Hawaii-owned project files: {name}")
    allowed = {parts[2] + ".fsproj"} | ({"Types.fs", "OpenApiHttp.fs"} if parts[2] == "FSharp.CloudEdge.Core.Api" else {"Client.fs"})
    if parts[3] not in allowed:
        raise ValueError(f"Path is outside Hawaii-owned project files: {name}")


def install_grouped_projects(stage, root, receipt, run):
    """Replace only previously owned, unchanged files after the staged build passes."""
    receipt_name = "inventory/hawaii-output-ownership.json"
    receipt_path = checked_relative(root, receipt_name)
    previous = json.loads(receipt_path.read_bytes()) if receipt_path.exists() else {"schemaVersion": 1, "files": {}}
    if previous.get("schemaVersion") != 1 or not isinstance(previous.get("files"), dict):
        raise ValueError("Unknown previous Hawaii output ownership format")
    old = previous["files"]
    new = receipt["files"]
    for name, fingerprint in old.items():
        validate_owned_path(name)
        path = checked_relative(root, name)
        if not path.is_file() or digest(path.read_bytes()) != fingerprint:
            raise ValueError(f"Owned Hawaii output was edited or removed: {name}")
    for name, fingerprint in new.items():
        validate_owned_path(name)
        source = checked_relative(stage, name)
        if not source.is_file() or digest(source.read_bytes()) != fingerprint:
            raise ValueError(f"Staged Hawaii output changed after validation: {name}")
        destination = checked_relative(root, name)
        if destination.exists() and name not in old:
            raise ValueError(f"Refusing to overwrite an unowned consumer file: {name}")
    backup = run / "previous-output"
    backup.mkdir()
    for name in old:
        target = checked_relative(backup, name)
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(checked_relative(root, name).read_bytes())
    if receipt_path.exists():
        (backup / "ownership.json").write_bytes(receipt_path.read_bytes())
    written = []
    removed = []
    try:
        for name in new:
            if old.get(name) == new[name]:
                continue
            target = checked_relative(root, name)
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_bytes(checked_relative(stage, name).read_bytes())
            written.append(name)
        for name in old.keys() - new.keys():
            checked_relative(root, name).unlink()
            removed.append(name)
        write_json(receipt_path, receipt)
    except Exception:
        # Recover only bytes written by this installation; never overwrite a
        # concurrent edit. The complete previous output also remains in run.
        for name in reversed(written):
            target = checked_relative(root, name)
            if target.exists() and digest(target.read_bytes()) == new[name]:
                if name in old:
                    target.write_bytes(checked_relative(backup, name).read_bytes())
                else:
                    target.unlink()
        for name in removed:
            target = checked_relative(root, name)
            if not target.exists():
                target.write_bytes(checked_relative(backup, name).read_bytes())
        raise


def command(args, cwd, run, name):
    start = time.monotonic()
    with (run / f"{name}.log").open("w") as log:
        completed = subprocess.run(args, cwd=cwd, stdout=log, stderr=subprocess.STDOUT)
    metrics = {"command": [str(a) for a in args], "exitCode": completed.returncode,
               "elapsedSeconds": time.monotonic() - start,
               "processTreeMaxRssKiB": resource.getrusage(resource.RUSAGE_CHILDREN).ru_maxrss}
    # ru_maxrss is the maximum child-process peak observed in this runner so far.
    write_json(run / f"{name}-metrics.json", metrics)
    if completed.returncode:
        raise RuntimeError(f"{name} failed ({completed.returncode}); see {run / (name + '.log')}")
    return metrics


def resolve_hawaii_tool(selection):
    """Read the authenticated local-tool package identity without consulting source checkouts."""
    if selection != {"tool": "hawaii"}:
        raise ValueError("pins.hawaii must select the packaged hawaii tool")
    completed = subprocess.run(["node", str(ROOT / "scripts/tool-packages.mjs"), selection["tool"]],
                               cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    if completed.returncode:
        raise RuntimeError(f"Cannot resolve packaged Hawaii tool: {completed.stderr.strip() or completed.stdout.strip()}")
    tool = json.loads(completed.stdout)
    fingerprint = tool.get("fingerprint")
    if (not isinstance(fingerprint, dict)
            or not all(key in fingerprint for key in ["packageId", "command", "version", "packageSha256", "payloadSha256", "manifestSha256"])
            or tool.get("args") != ["tool", "run", fingerprint.get("command"), "--"]):
        raise ValueError("Hawaii tool resolver returned incomplete package evidence")
    return tool


def run_hawaii(selection, config_path, run, provenance):
    tool = resolve_hawaii_tool(selection)
    provenance["hawaiiTool"] = tool["fingerprint"]
    command(["dotnet", *tool["args"], "--config", str(config_path), "--no-logo"], ROOT, run, "generate")
    after = resolve_hawaii_tool(selection)
    if after["fingerprint"] != tool["fingerprint"]:
        raise RuntimeError("Packaged Hawaii tool changed during generation; output was not accepted")


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--schema", type=Path, help="Existing pristine schema; exact pinned SHA-256 required")
    parser.add_argument("--lifecycle-policy", type=Path, help="Override the pinned operation lifecycle and exposure policy")
    parser.add_argument("--run-directory", type=Path)
    parser.add_argument("--single-client", action="store_true", help="Compile a single-client artifact probe without updating organized consumer output")
    args = parser.parse_args(argv)
    pins_bytes = (HERE / "pins.json").read_bytes()
    pins = json.loads(pins_bytes)
    run = (args.run_directory or ROOT / "artifacts/hawaii/runs" / time.strftime("%Y%m%dT%H%M%S")).resolve()
    if run.exists():
        raise ValueError(f"Run directory already exists: {run}")
    run.mkdir(parents=True)
    pipeline_sources = {name: digest((ROOT / name).read_bytes()) for name in pins["pipelineInputs"]}
    provenance = {"status": "started", "pinsSha256": digest(pins_bytes), "pipelineSources": pipeline_sources}
    try:
        raw = args.schema.read_bytes() if args.schema else urllib.request.urlopen(pins["schema"]["url"]).read()
        if digest(raw) != pins["schema"]["sha256"]:
            raise ValueError("Pristine Cloudflare schema hash differs from pins.json")
        (run / "schema-original.json").write_bytes(raw)
        source = json.loads(raw)
        if len(operations(source)) != pins["schema"]["operationCount"]:
            raise ValueError("Pinned schema operation inventory changed")
        provenance["schemaOriginalSha256"] = digest(raw)
        provenance["overlays"] = []
        for path in pins["overlays"]:
            patch = (HERE / path).read_bytes()
            source = apply_overlay(source, json.loads(patch))
            provenance["overlays"].append({"path": path, "sha256": digest(patch)})
        policy = {"operations": []}
        policy_path = args.lifecycle_policy or (HERE / pins["operationPolicy"] if pins.get("operationPolicy") else None)
        if policy_path:
            policy_bytes = policy_path.read_bytes()
            policy = json.loads(policy_bytes)
            if policy.get("schemaSha256") != pins["schema"]["sha256"]:
                raise ValueError("Lifecycle policy belongs to a different source schema")
            provenance["lifecyclePolicySha256"] = digest(policy_bytes)
            provenance["lifecyclePolicyAsOf"] = policy.get("asOf")
        selected, coverage = select_lifecycle(source, policy)
        taxonomy_bytes = (HERE / pins["taxonomy"]).read_bytes()
        taxonomy = json.loads(taxonomy_bytes)
        if taxonomy["schemaSha256"] != pins["schema"]["sha256"]:
            raise ValueError("Taxonomy belongs to a different schema")
        provenance["taxonomySha256"] = digest(taxonomy_bytes)
        unresolved = assign_taxonomy(source, coverage, taxonomy)
        write_json(run / "taxonomy-unresolved.json", unresolved)
        provenance["unresolvedTaxonomyOperations"] = len(unresolved)
        write_json(run / "operation-coverage.json", coverage)
        write_json(run / "schema-selected.json", selected)
        provenance["schemaSelectedSha256"] = digest((run / "schema-selected.json").read_bytes())
        provenance["sourceOperations"] = len(coverage)
        provenance["includedOperations"] = sum(item["disposition"] == "included" for item in coverage)
        output = run / "Generated"
        groups = [] if args.single_client else grouped_clients(coverage)
        config = {"schema": str(run / "schema-selected.json"),
                  "project": "FSharp.CloudEdge.Management" if args.single_client else "FSharp.CloudEdge.Core.Api",
                  "output": str(output), "target": "fsharp", "asyncReturnType": "task", "emptyDefinitions": "free-form"}
        if groups:
            config["clientGroups"] = groups
        policy_bytes = (HERE / pins["schemaPolicy"]).read_bytes()
        schema_policy = json.loads(policy_bytes)
        if schema_policy["schemaSha256"] != pins["schema"]["sha256"]:
            raise ValueError("Schema fidelity policy belongs to a different source schema")
        provenance["schemaPolicySha256"] = digest(policy_bytes)
        config["preserveJsonSchemas"] = schema_policy["schemaIds"]
        write_json(run / "hawaii.json", config)
        run_hawaii(pins["hawaii"], run / "hawaii.json", run, provenance)
        if {name: digest((ROOT / name).read_bytes()) for name in pins["pipelineInputs"]} != pipeline_sources:
            raise RuntimeError("Hawaii pipeline sources changed during generation; output was not accepted")
        provenance["generatedSources"] = {p.relative_to(output).as_posix(): digest(p.read_bytes()) for p in sorted(output.rglob("*.fs"))}
        if args.single_client:
            command(["dotnet", "build", str(output / "FSharp.CloudEdge.Management.fsproj"), "--configuration", "Release", "--nologo", "-m:1"], ROOT, run, "compile")
        else:
            stage = run / "Staged"
            receipt = stage_grouped_projects(output, groups, stage)
            write_json(run / "output-ownership.json", receipt)
            command(["dotnet", "build", str(stage / "Hawaii.Bindings.slnx"), "--configuration", "Release", "--nologo", "-m:1"], ROOT, run, "compile")
            command(["python3", str(HERE / "tests/check_grouped_clients.py"), "--bindings-root", str(stage), "--run-directory", str(run / "http-smoke")], ROOT, run, "http-smoke")
            if resolve_hawaii_tool(pins["hawaii"])["fingerprint"] != provenance["hawaiiTool"]:
                raise RuntimeError("Packaged Hawaii tool changed during validation; output was not installed")
            if {name: digest((ROOT / name).read_bytes()) for name in pins["pipelineInputs"]} != pipeline_sources:
                raise RuntimeError("Hawaii pipeline sources changed during validation; output was not installed")
            receipt["validation"] = {"run": run.relative_to(ROOT).as_posix() if run.is_relative_to(ROOT) else str(run),
                                     "tool": provenance["hawaiiTool"], "schemaSha256": provenance["schemaSelectedSha256"],
                                     "configuration": "Release", "selectedOperations": provenance["includedOperations"]}
            install_grouped_projects(stage, ROOT, receipt, run)
            provenance["installedProjects"] = ["FSharp.CloudEdge.Core.Api"] + [group["project"] for group in groups]
        provenance["status"] = "compiled"
        print(f"Canonical model and clients compiled; {provenance['includedOperations']} selected operations. Artifacts: {run}")
    except Exception as error:
        provenance["status"] = "failed"
        provenance["failure"] = str(error)
        raise
    finally:
        write_json(run / "provenance.json", provenance)


if __name__ == "__main__":
    main()
