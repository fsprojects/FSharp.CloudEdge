"""Regression checks for fixture evidence, not live Cloudflare acceptance."""
import copy
import json
from pathlib import Path
import shutil
import tempfile
import unittest

from referencing.exceptions import Unresolvable

import validate_fixtures as validation


def document_fixture():
    message = {"type": "object", "required": ["code", "message"],
               "properties": {"code": {"type": "integer"}, "message": {"type": "string"}}}
    common = {"type": "object", "required": ["success", "errors", "messages", "result"], "properties": {
        "success": {"type": "boolean", "enum": [True]},
        "errors": {"type": "array", "items": message}, "messages": {"type": "array", "items": message},
        "result": {"anyOf": [{"type": "object"}, {"type": "array"}, {"type": "string"}]}}}
    failure = copy.deepcopy(common)
    failure["properties"]["success"]["enum"] = [False]
    failure["properties"]["result"] = {"type": "object", "nullable": True, "enum": [None]}
    conflicting = {"allOf": [{"$ref": "#/components/schemas/Success"}, {"$ref": "#/components/schemas/Failure"}]}
    document = {"openapi": "3.0.3", "info": {"title": "fixture", "version": "1"}, "components": {"schemas": {
        "Common": common, "Success": {"allOf": [{"$ref": "#/components/schemas/Common"},
                                                   {"type": "object", "properties": {"result": {"type": "object"}}}]},
        "Failure": failure}}, "paths": {"/tunnel/{id}": {"get": {"operationId": "getTunnel", "responses": {
            "4XX": {"description": "failure", "content": {"application/json": {"schema": conflicting}}}}}}}}
    fixtures = {"schemaVersion": 1, "cases": [{"id": "tunnel.failure", "operationId": "getTunnel", "method": "GET",
        "pathTemplate": "/tunnel/{id}", "responseStatus": 429,
        "responseBody": {"success": False, "errors": [{"code": 10000, "message": "denied"}], "messages": [], "result": None},
        "expectedSchemaErrors": [{"path": "result", "validator": "anyOf"}, {"path": "result", "validator": "type"},
                                 {"path": "success", "validator": "enum"}]}]}
    effective = copy.deepcopy(document)
    effective["paths"]["/tunnel/{id}"]["get"]["responses"]["4XX"]["content"]["application/json"]["schema"] = {
        "$ref": "#/components/schemas/Failure"}
    return document, effective, fixtures


class FixtureValidationTests(unittest.TestCase):
    def test_original_three_errors_remain_evidence_and_effective_body_has_zero_errors(self):
        original, effective, fixtures = document_fixture()
        report = validation.validate_documents(original, effective, fixtures)
        self.assertEqual(report["status"], "checked")
        observed = report["observations"][0]
        self.assertEqual(observed["upstream"]["status"], "recorded-conflict")
        self.assertEqual(len(observed["upstream"]["errors"]), 3)
        self.assertEqual(observed["effective"], {"status": "passed", "errors": []})

    def test_known_upstream_errors_cannot_bypass_an_uncorrected_effective_schema(self):
        original, _, fixtures = document_fixture()
        report = validation.validate_documents(original, original, fixtures)
        self.assertEqual(report["status"], "failed")
        self.assertEqual(report["observations"][0]["upstream"]["status"], "recorded-conflict")
        self.assertEqual(len(report["observations"][0]["effective"]["errors"]), 3)

    def test_new_invalid_body_fails_even_when_all_upstream_errors_are_acknowledged(self):
        original, effective, fixtures = document_fixture()
        fixtures["cases"][0]["responseBody"]["errors"][0]["code"] = "not-a-number"
        first = validation.validate_documents(original, effective, fixtures)
        fixtures["cases"][0]["expectedSchemaErrors"] = validation.error_keys(first["observations"][0]["upstream"]["errors"])
        report = validation.validate_documents(original, effective, fixtures)
        self.assertEqual(report["observations"][0]["upstream"]["status"], "recorded-conflict")
        self.assertEqual(report["status"], "failed")
        self.assertEqual(report["observations"][0]["effective"]["errors"][0]["path"], ["errors", 0, "code"])

    def test_upstream_conflict_disappearance_requires_evidence_review(self):
        original, effective, fixtures = document_fixture()
        report = validation.validate_documents(effective, effective, fixtures)
        self.assertEqual(report["status"], "failed")
        self.assertEqual(report["observations"][0]["upstream"]["status"], "failed")

    def test_request_validation_never_uses_response_error_metadata(self):
        original, effective, fixtures = document_fixture()
        for document in [original, effective]:
            document["paths"]["/tunnel/{id}"]["get"]["requestBody"] = {"content": {"application/json": {
                "schema": {"type": "object", "required": ["config"], "properties": {"config": {"type": "object"}}}}}}
        fixtures["cases"][0]["requestBody"] = {"config": 3}
        report = validation.validate_documents(original, effective, fixtures)
        self.assertEqual(report["status"], "failed")
        self.assertEqual(report["observations"][0]["direction"], "request")
        self.assertEqual(report["observations"][0]["upstream"]["expectedErrors"], [])

    def test_operation_identity_and_duplicate_fixtures_fail_closed(self):
        original, effective, fixtures = document_fixture()
        effective["paths"]["/tunnel/{id}"]["get"]["operationId"] = "differentOperation"
        with self.assertRaisesRegex(ValueError, "operation differs"):
            validation.validate_documents(original, effective, fixtures)
        effective["paths"]["/tunnel/{id}"]["get"]["operationId"] = "getTunnel"
        fixtures["cases"].append(copy.deepcopy(fixtures["cases"][0]))
        with self.assertRaisesRegex(ValueError, "Duplicate fixture"):
            validation.validate_documents(original, effective, fixtures)

    def test_negative_response_is_explicitly_excluded_and_cannot_hide_recorded_conflicts(self):
        original, effective, fixtures = document_fixture()
        fixture = fixtures["cases"][0]
        fixture["negativeFixture"] = True
        with self.assertRaisesRegex(ValueError, "cannot bypass"):
            validation.validate_documents(original, effective, fixtures)
        del fixture["expectedSchemaErrors"]
        report = validation.validate_documents(original, effective, fixtures)
        self.assertEqual(report["negativeResponsesExcluded"], [fixture["id"]])
        self.assertEqual(report["observations"], [])

    def test_schema_reference_resolution_never_fetches_external_documents(self):
        with self.assertRaisesRegex(Unresolvable, "https://external.invalid/schema"):
            validation.body_errors({}, {"$ref": "https://external.invalid/schema"}, {})

    def negative_fixture(self):
        _, document, fixtures = document_fixture()
        fixture = fixtures["cases"][0]
        del fixture["expectedSchemaErrors"]
        fixture["negativeFixture"] = True
        fixture["responseBody"]["errors"][0]["code"] = "not-a-number"
        fixture["negativeSchemaErrors"] = [{"path": "errors/0/code", "validator": "type"}]
        return document, fixtures

    def test_malformed_body_is_rejection_evidence_in_both_schemas_never_a_conforming_positive(self):
        document, fixtures = self.negative_fixture()
        report = validation.validate_documents(document, document, fixtures)
        self.assertEqual(report["status"], "checked")
        self.assertEqual(report["negativeResponsesExcluded"], [])
        observed = report["observations"][0]
        self.assertEqual(observed["kind"], "negative-response")
        for source in ["upstream", "effective"]:
            self.assertEqual(observed[source]["status"], "expected-rejection")
            self.assertEqual(validation.error_keys(observed[source]["errors"]), fixtures["cases"][0]["negativeSchemaErrors"])

    def test_negative_evidence_fails_when_either_schema_stops_rejecting(self):
        document, fixtures = self.negative_fixture()
        relaxed = copy.deepcopy(document)
        relaxed["components"]["schemas"]["Failure"]["properties"]["errors"]["items"]["properties"]["code"] = {"type": "string"}
        for original, effective, changed in [(relaxed, document, "upstream"), (document, relaxed, "effective")]:
            with self.subTest(changed=changed):
                report = validation.validate_documents(original, effective, fixtures)
                self.assertEqual(report["status"], "failed")
                self.assertEqual(report["observations"][0][changed]["errors"], [])

    def test_negative_body_cannot_accumulate_unrecorded_errors(self):
        document, fixtures = self.negative_fixture()
        del fixtures["cases"][0]["responseBody"]["messages"]
        report = validation.validate_documents(document, document, fixtures)
        self.assertEqual(report["status"], "failed")
        self.assertEqual(len(report["observations"][0]["effective"]["errors"]), 2)

    def test_negative_evidence_cannot_be_empty_or_disguise_a_positive_or_transport_case(self):
        document, fixtures = self.negative_fixture()
        for changes in [{"negativeSchemaErrors": []}, {"negativeFixture": False},
                        {"responseRaw": "{broken"}, {"holdResponse": True},
                        {"negativeSchemaErrors": [{"path": "errors", "validator": 1}]}]:
            with self.subTest(changes=changes):
                changed = copy.deepcopy(fixtures)
                changed["cases"][0].update(changes)
                with self.assertRaisesRegex(ValueError, "negativeSchemaErrors"):
                    validation.validate_documents(document, document, changed)

    def test_negative_response_evidence_cannot_exempt_an_invalid_request(self):
        document, fixtures = self.negative_fixture()
        document["paths"]["/tunnel/{id}"]["get"]["requestBody"] = {"content": {"application/json": {
            "schema": {"type": "object", "required": ["config"], "properties": {"config": {"type": "object"}}}}}}
        fixtures["cases"][0]["requestBody"] = {"config": 3}
        report = validation.validate_documents(document, document, fixtures)
        self.assertEqual(report["status"], "failed")
        self.assertEqual(report["observations"][0]["direction"], "request")
        self.assertEqual(report["observations"][0]["upstream"]["expectedErrors"], [])
        self.assertEqual(report["observations"][1]["effective"]["status"], "expected-rejection")


class GenerationEvidenceTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory(prefix="cloudedge-schema-evidence-")
        self.addCleanup(self.directory.cleanup)
        self.root = Path(self.directory.name)
        self.hawaii = self.root / "generators/hawaii"
        self.hawaii.mkdir(parents=True)
        self.run = self.root / "artifacts/run"
        self.run.mkdir(parents=True)
        self.original, self.effective, self.fixtures = document_fixture()
        self.schema = self.root / "upstream.json"
        self.write(self.schema, self.original)
        self.schema_hash = validation.digest(self.schema.read_bytes())
        self.generator = self.hawaii / "generate.py"
        shutil.copyfile(validation.ROOT / "generators/hawaii/generate.py", self.generator)
        self.first = self.hawaii / "first.json"
        self.second = self.hawaii / "second.json"
        self.write(self.first, [{"op": "add", "path": "/info/x-correction-profile", "value": "test"}])
        pointer = "/paths/~1tunnel~1{id}/get/responses/4XX/content/application~1json/schema"
        selected = self.original["paths"]["/tunnel/{id}"]["get"]["responses"]["4XX"]["content"]["application/json"]["schema"]
        self.write(self.second, [{"op": "test", "path": pointer, "value": selected},
                                {"op": "replace", "path": pointer, "value": {"$ref": "#/components/schemas/Failure"}}])
        self.effective["info"]["x-correction-profile"] = "test"
        self.write(self.run / "schema-selected.json", self.effective)
        self.pins = {"schemaVersion": 1, "schema": {"sha256": self.schema_hash},
                     "pipelineInputs": ["generators/hawaii/generate.py"], "overlays": ["first.json", "second.json"]}
        self.pins_path = self.hawaii / "pins.json"
        self.write(self.pins_path, self.pins)
        self.provenance = {"status": "compiled", "pinsSha256": validation.digest(self.pins_path.read_bytes()),
                           "schemaOriginalSha256": self.schema_hash,
                           "schemaSelectedSha256": validation.digest((self.run / "schema-selected.json").read_bytes()),
                           "pipelineSources": {"generators/hawaii/generate.py": validation.digest(self.generator.read_bytes())},
                           "overlays": [{"path": file.name, "sha256": validation.digest(file.read_bytes())} for file in [self.first, self.second]]}
        self.provenance_path = self.run / "provenance.json"
        self.write(self.provenance_path, self.provenance)

    @staticmethod
    def write(file, value):
        file.write_text(json.dumps(value, indent=2) + "\n")

    def load(self):
        return validation.load_schema_pair(self.schema, self.pins_path, self.provenance_path)

    def test_exact_pipeline_reconstruction_matches_accepted_effective_schema(self):
        original, effective, evidence = self.load()
        self.assertEqual(original, self.original)
        self.assertEqual(effective, self.effective)
        self.assertEqual(evidence["overlays"], self.provenance["overlays"])
        self.assertEqual(validation.validate_documents(original, effective, self.fixtures)["status"], "checked")

    def test_tampered_overlay_and_reordered_evidence_are_rejected(self):
        original_bytes = self.second.read_bytes()
        self.second.write_bytes(original_bytes + b" ")
        with self.assertRaisesRegex(ValueError, "Ordered overlay inputs"):
            self.load()
        self.second.write_bytes(original_bytes)
        self.provenance["overlays"].reverse()
        self.write(self.provenance_path, self.provenance)
        with self.assertRaisesRegex(ValueError, "Ordered overlay inputs"):
            self.load()

    def test_tampered_pipeline_is_rejected_before_python_import(self):
        with self.generator.open("a") as stream:
            stream.write("\nraise RuntimeError('must not execute unauthenticated pipeline')\n")
        with self.assertRaisesRegex(ValueError, "pipeline sources differ"):
            self.load()

    def test_stale_pins_and_original_schema_are_rejected(self):
        self.pins["overlays"].reverse()
        self.write(self.pins_path, self.pins)
        with self.assertRaisesRegex(ValueError, "Pins differ"):
            self.load()
        self.schema.write_text("{}")
        with self.assertRaisesRegex(ValueError, "Pristine schema differs"):
            self.load()

    def test_effective_schema_requires_both_byte_authentication_and_reconstruction(self):
        changed = copy.deepcopy(self.effective)
        changed["info"]["title"] = "unexpected"
        selected = self.run / "schema-selected.json"
        self.write(selected, changed)
        with self.assertRaisesRegex(ValueError, "Effective schema file differs"):
            self.load()
        self.provenance["schemaSelectedSha256"] = validation.digest(selected.read_bytes())
        self.write(self.provenance_path, self.provenance)
        with self.assertRaisesRegex(ValueError, "does not match ordered overlays"):
            self.load()

    def test_missing_overlay_correction_still_fails_body_validation(self):
        self.pins["overlays"] = ["first.json"]
        self.write(self.pins_path, self.pins)
        self.provenance["pinsSha256"] = validation.digest(self.pins_path.read_bytes())
        self.provenance["overlays"] = self.provenance["overlays"][:1]
        uncorrected = copy.deepcopy(self.original)
        uncorrected["info"]["x-correction-profile"] = "test"
        self.write(self.run / "schema-selected.json", uncorrected)
        self.provenance["schemaSelectedSha256"] = validation.digest((self.run / "schema-selected.json").read_bytes())
        self.write(self.provenance_path, self.provenance)
        original, effective, _ = self.load()
        report = validation.validate_documents(original, effective, self.fixtures)
        self.assertEqual(report["status"], "failed")
        self.assertEqual(len(report["observations"][0]["effective"]["errors"]), 3)


if __name__ == "__main__":
    unittest.main()
