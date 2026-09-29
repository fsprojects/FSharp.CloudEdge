#!/usr/bin/env python3
"""Check pristine upstream evidence and require valid bodies under the generated schema."""
import argparse
import hashlib
import importlib.util
import json
from pathlib import Path

from jsonschema import Draft4Validator
from referencing import Registry
from referencing.exceptions import NoSuchResource

ROOT = Path(__file__).resolve().parents[2]


def digest(raw):
    return hashlib.sha256(raw).hexdigest()


def inside(directory, name):
    if not isinstance(name, str) or not name or Path(name).is_absolute():
        raise ValueError("Evidence paths must be nonempty relative paths")
    target = (directory / name).resolve()
    if not target.is_relative_to(directory.resolve()) or ".." in Path(name).parts:
        raise ValueError(f"Evidence path escapes its directory: {name}")
    return target


def load_schema_pair(schema_path, pins_path, provenance_path):
    """Reconstruct the exact selected schema with the authenticated generation pipeline."""
    pins_path = pins_path.resolve()
    root = pins_path.parents[2]
    raw = schema_path.read_bytes()
    pins_raw = pins_path.read_bytes()
    pins = json.loads(pins_raw)
    provenance = json.loads(provenance_path.read_bytes())
    if digest(raw) != pins["schema"]["sha256"] or provenance.get("schemaOriginalSha256") != digest(raw):
        raise ValueError("Pristine schema differs from pinned generation evidence")
    if provenance.get("status") != "compiled" or provenance.get("pinsSha256") != digest(pins_raw):
        raise ValueError("Pins differ from a completed generation receipt")
    pipeline_paths = pins.get("pipelineInputs", [])
    if not pipeline_paths or len(set(pipeline_paths)) != len(pipeline_paths):
        raise ValueError("Generation pipeline inputs must be explicit and unique")
    pipeline_bytes = {name: inside(root, name).read_bytes() for name in pipeline_paths}
    pipeline_sources = {name: digest(content) for name, content in pipeline_bytes.items()}
    if pipeline_sources != provenance.get("pipelineSources"):
        raise ValueError("Generation pipeline sources differ from accepted evidence")
    generator_path = inside(root, "generators/hawaii/generate.py")
    if "generators/hawaii/generate.py" not in pipeline_sources:
        raise ValueError("Overlay implementation is not authenticated by pipeline evidence")
    overlay_paths = pins.get("overlays")
    if not isinstance(overlay_paths, list) or len(set(overlay_paths)) != len(overlay_paths):
        raise ValueError("Pinned overlay paths must be an ordered unique list")
    overlay_bytes = [(name, inside(pins_path.parent, name).read_bytes()) for name in overlay_paths]
    overlay_evidence = [{"path": name, "sha256": digest(content)} for name, content in overlay_bytes]
    if overlay_evidence != provenance.get("overlays"):
        raise ValueError("Ordered overlay inputs differ from accepted generation evidence")
    spec = importlib.util.spec_from_file_location("cloudedge_fixture_generation", generator_path)
    generation = importlib.util.module_from_spec(spec)
    exec(compile(pipeline_bytes["generators/hawaii/generate.py"], str(generator_path), "exec"), generation.__dict__)
    original = json.loads(raw)
    effective = original
    for _, content in overlay_bytes:
        effective = generation.apply_overlay(effective, json.loads(content))
    policy = {"operations": []}
    if pins.get("operationPolicy"):
        policy_raw = inside(pins_path.parent, pins["operationPolicy"]).read_bytes()
        policy = json.loads(policy_raw)
        if (digest(policy_raw) != provenance.get("lifecyclePolicySha256")
                or policy.get("schemaSha256") != digest(raw)):
            raise ValueError("Operation policy differs from accepted generation evidence")
    selected, _ = generation.select_lifecycle(effective, policy)
    selected_path = provenance_path.parent / "schema-selected.json"
    selected_raw = selected_path.read_bytes()
    if digest(selected_raw) != provenance.get("schemaSelectedSha256"):
        raise ValueError("Effective schema file differs from accepted generation evidence")
    if selected != json.loads(selected_raw):
        raise ValueError("Effective schema does not match ordered overlays and operation selection")
    return original, selected, {
        "pristineSchemaSha256": digest(raw), "effectiveSchemaSha256": digest(selected_raw),
        "pinsSha256": digest(pins_raw), "generationProvenanceSha256": digest(provenance_path.read_bytes()),
        "pipelineSources": pipeline_sources, "overlays": overlay_evidence,
    }


def normalize(value):
    """OpenAPI 3.0 uses nullable rather than JSON Schema's null type."""
    if isinstance(value, list):
        return [normalize(item) for item in value]
    if not isinstance(value, dict):
        return value
    result = {key: normalize(item) for key, item in value.items()}
    if result.pop("nullable", False):
        if isinstance(result.get("type"), str):
            result["type"] = [result["type"], "null"]
        else:
            result = {"anyOf": [result, {"type": "null"}]}
    return result


def no_external_reference(uri):
    raise NoSuchResource(ref=uri)


def body_errors(document, selected, body):
    # Keep components at the root for local JSON Pointers, while refusing network retrieval.
    validator = Draft4Validator({**document, **selected}, registry=Registry(retrieve=no_external_reference))
    return [{"path": list(error.path), "validator": error.validator, "message": error.message}
            for error in validator.iter_errors(body)]


def error_keys(errors):
    return sorted(({"path": "/".join(map(str, error["path"])), "validator": error["validator"]} for error in errors),
                  key=lambda item: (item["path"], item["validator"]))


def expected_error_keys(value, field):
    if not isinstance(value, list) or any(not isinstance(item, dict) or set(item) != {"path", "validator"}
                                          or not all(isinstance(part, str) for part in item.values()) for item in value):
        raise ValueError(f"{field} must list exact path/validator evidence")
    return sorted(value, key=lambda item: (item["path"], item["validator"]))


def validate_documents(original, effective, fixtures):
    upstream, corrected = normalize(original), normalize(effective)
    observations = []
    skipped = []
    identities = set()
    failed = False
    for fixture in fixtures["cases"]:
        if fixture["id"] in identities:
            raise ValueError(f"Duplicate fixture identity: {fixture['id']}")
        identities.add(fixture["id"])
        operations = [document["paths"][fixture["pathTemplate"]][fixture["method"].lower()]
                      for document in [upstream, corrected]]
        if any(operation.get("operationId") != fixture["operationId"] for operation in operations):
            raise ValueError(f"Fixture operation differs from schema: {fixture['id']}")
        expected = expected_error_keys(fixture.get("expectedSchemaErrors", []), "expectedSchemaErrors")
        negative = None
        if "negativeSchemaErrors" in fixture:
            negative = expected_error_keys(fixture["negativeSchemaErrors"], "negativeSchemaErrors")
            if not fixture.get("negativeFixture") or not negative:
                raise ValueError("negativeSchemaErrors requires a negative fixture and nonempty rejection evidence")
            if "responseRaw" in fixture or fixture.get("holdResponse"):
                raise ValueError("negativeSchemaErrors requires the actual JSON response body, not a transport negative")
        if fixture.get("negativeFixture") and expected:
            raise ValueError("Negative fixtures cannot bypass expected upstream conflict verification")
        bodies = []
        if "requestBody" in fixture:
            bodies.append(("request", [operation["requestBody"]["content"]["application/json"]["schema"]
                                       for operation in operations], fixture["requestBody"]))
        if not fixture.get("negativeFixture") or negative is not None:
            status = str(fixture["responseStatus"])
            responses = [operation["responses"].get(status) or operation["responses"][status[0] + "XX"] for operation in operations]
            bodies.append(("response", [response["content"]["application/json"]["schema"] for response in responses], fixture["responseBody"]))
        else:
            skipped.append(fixture["id"])
        for direction, selections, body in bodies:
            upstream_errors = body_errors(upstream, selections[0], body)
            effective_errors = body_errors(corrected, selections[1], body)
            if direction == "response" and negative is not None:
                upstream_matches = error_keys(upstream_errors) == negative
                effective_matches = error_keys(effective_errors) == negative
                passed = upstream_matches and effective_matches
                failed = failed or not passed
                observations.append({"id": fixture["id"], "direction": direction, "kind": "negative-response",
                                     "status": "passed" if passed else "failed",
                                     "upstream": {"status": "expected-rejection" if upstream_matches else "failed",
                                                  "expectedErrors": negative, "errors": upstream_errors},
                                     "effective": {"status": "expected-rejection" if effective_matches else "failed",
                                                   "expectedErrors": negative, "errors": effective_errors}})
                continue
            expected_errors = expected if direction == "response" else []
            upstream_matches = error_keys(upstream_errors) == expected_errors
            passed = upstream_matches and not effective_errors
            failed = failed or not passed
            observations.append({"id": fixture["id"], "direction": direction, "status": "passed" if passed else "failed",
                                 "upstream": {"status": "recorded-conflict" if upstream_errors and upstream_matches else "passed" if upstream_matches else "failed",
                                              "expectedErrors": expected_errors, "errors": upstream_errors},
                                 "effective": {"status": "failed" if effective_errors else "passed", "errors": effective_errors}})
    return {"schemaVersion": 2, "status": "failed" if failed else "checked", "observations": observations,
            "negativeResponsesExcluded": skipped}


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--schema", type=Path, required=True)
    parser.add_argument("--fixtures", type=Path, required=True)
    parser.add_argument("--report", type=Path, required=True)
    parser.add_argument("--pins", type=Path, default=ROOT / "generators/hawaii/pins.json")
    parser.add_argument("--provenance", type=Path)
    args = parser.parse_args(argv)
    root = args.pins.resolve().parents[2]
    provenance_path = args.provenance
    if provenance_path is None:
        ownership = json.loads((root / "inventory/hawaii-output-ownership.json").read_bytes())
        provenance_path = inside(root, ownership["validation"]["run"]) / "provenance.json"
    original, effective, evidence = load_schema_pair(args.schema, args.pins, provenance_path)
    fixtures = json.loads(args.fixtures.read_bytes())
    if evidence["pristineSchemaSha256"] != fixtures["schemaSha256"]:
        raise ValueError("Fixture schema hash does not match original OpenAPI")
    report = validate_documents(original, effective, fixtures)
    report["evidence"] = evidence
    args.report.write_text(json.dumps(report, indent=2) + "\n")
    if report["status"] != "checked":
        raise SystemExit("Fixture validation failed: positive bodies require zero effective errors; negative bodies require exact rejection evidence")
    negative_count = sum(item.get("kind") == "negative-response" for item in report["observations"])
    print(f"Checked {len(report['observations'])} fixture bodies against pristine and effective schemas; "
          f"positive bodies have zero effective errors, {negative_count} negative bodies have exact rejection evidence.")


if __name__ == "__main__":
    main()
