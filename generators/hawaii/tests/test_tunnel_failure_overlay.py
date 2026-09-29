"""Offline contracts for the exact Tunnel/One/WARP failure-response correction."""
import copy
import hashlib
import importlib.util
import json
from pathlib import Path
import unittest

from jsonschema import Draft4Validator


HERE = Path(__file__).resolve().parent
HAWAII = HERE.parent
FIXTURE = HERE / "fixtures/tunnel-failure-responses.openapi.json"
FIXTURE_SHA256 = "5363b6f10ab3c3e05017b2826c0284b34aa2aeb6a0b3b90ecfa27b4ad71e9351"
COMMON = "#/components/schemas/tunnel_api-response-common"
FAILURE = "#/components/schemas/tunnel_api-response-common-failure"
spec = importlib.util.spec_from_file_location("hawaii_tunnel_overlay", HAWAII / "generate.py")
pipeline = importlib.util.module_from_spec(spec)
spec.loader.exec_module(pipeline)


def json_bytes(value):
    return json.dumps(value, sort_keys=True, separators=(",", ":")).encode()


def pointer(parts):
    return "/" + "/".join(str(part).replace("~", "~0").replace("/", "~1") for part in parts)


def read_pointer(document, path):
    node = document
    for part in path.removeprefix("#").split("/")[1:]:
        node = node[part.replace("~1", "/").replace("~0", "~")]
    return node


def references(value):
    if isinstance(value, dict):
        if "$ref" in value:
            yield value["$ref"]
        for item in value.values():
            yield from references(item)
    elif isinstance(value, list):
        for item in value:
            yield from references(item)


def success_constraints(document, schema, seen=None):
    seen = set() if seen is None else seen
    if "$ref" in schema:
        ref = schema["$ref"]
        if ref not in seen:
            seen.add(ref)
            yield from success_constraints(document, read_pointer(document, ref), seen)
    success = schema.get("properties", {}).get("success", {})
    if "enum" in success:
        yield success["enum"]
    for operand in schema.get("allOf", []):
        yield from success_constraints(document, operand, seen)


def normalize_nullable(value):
    """Translate OpenAPI 3.0 nullable into JSON Schema's null type for validation."""
    if isinstance(value, list):
        return [normalize_nullable(item) for item in value]
    if not isinstance(value, dict):
        return value
    result = {key: normalize_nullable(item) for key, item in value.items()}
    if result.pop("nullable", False):
        if isinstance(result.get("type"), str):
            result["type"] = [result["type"], "null"]
        else:
            result = {"anyOf": [result, {"type": "null"}]}
    return result


class TunnelFailureOverlay(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.source = json.loads(FIXTURE.read_text())
        cls.patch_bytes = (HAWAII / "overlays/tunnel-failure-responses.json").read_bytes()
        cls.patch = json.loads(cls.patch_bytes)
        cls.inventory = json.loads((HAWAII / "overlays/tunnel-failure-responses.inventory.json").read_text())
        cls.pins = json.loads((HAWAII / "pins.json").read_text())
        cls.sites = []
        for path, methods in cls.source["paths"].items():
            for method, operation in methods.items():
                parts = ["paths", path, method, "responses", "4XX", "content", "application/json", "schema"]
                cls.sites.append({"path": path, "method": method.upper(), "operationId": operation["operationId"],
                                  "responseSchemaPath": pointer(parts)})

    def test_fixture_is_pinned_pristine_projection_with_complete_references(self):
        self.assertEqual(hashlib.sha256(FIXTURE.read_bytes()).hexdigest(), FIXTURE_SHA256)
        source = self.source["x-fixture-source"]
        self.assertEqual(source["sha256"], self.pins["schema"]["sha256"])
        self.assertEqual(source["commit"], self.pins["schema"]["commit"])
        self.assertEqual(len(self.sites), 47)
        self.assertEqual(len(self.source["components"]["schemas"]), 97)
        for ref in references(self.source):
            with self.subTest(reference=ref):
                self.assertTrue(ref.startswith("#/components/schemas/"))
                self.assertIsInstance(read_pointer(self.source, ref), dict)

    def test_inventory_and_patch_identify_the_same_exact_operations(self):
        inventory = self.inventory
        self.assertEqual(inventory["overlaySha256"], hashlib.sha256(self.patch_bytes).hexdigest())
        self.assertEqual(inventory["sourceSchema"]["sha256"], self.source["x-fixture-source"]["sha256"])
        fields = ("operationId", "method", "path", "responseSchemaPath")
        expected = {tuple(site[key] for key in fields) for site in self.sites}
        actual = {tuple(site[key] for key in fields) for site in inventory["affectedOperations"]}
        self.assertEqual(actual, expected)
        self.assertEqual(len(inventory["affectedOperations"]), len(actual))
        replacements = [item for item in self.patch if item["op"] == "replace"]
        self.assertEqual(len(replacements), 47)
        self.assertEqual({item["path"] for item in replacements}, {site["responseSchemaPath"] for site in self.sites})
        self.assertEqual(sum(item["op"] == "test" for item in self.patch), 96)
        for index, item in enumerate(self.patch):
            if item["op"] != "replace":
                continue
            with self.subTest(path=item["path"]):
                self.assertEqual(item["value"], {"$ref": FAILURE})
                self.assertEqual(self.patch[index - 1], {"op": "test", "path": item["path"],
                                                       "value": read_pointer(self.source, item["path"])})
                self.assertEqual(self.patch[index - 2]["value"], next(site["operationId"] for site in self.sites if site["responseSchemaPath"] == item["path"]))

    def test_every_original_failure_intersects_conflicting_success_values(self):
        success_envelopes = set()
        for site in self.sites:
            with self.subTest(operation=site["operationId"]):
                schema = read_pointer(self.source, site["responseSchemaPath"])
                self.assertEqual(set(schema), {"allOf"})
                self.assertEqual(len(schema["allOf"]), 2)
                self.assertEqual(schema["allOf"][1], {"$ref": FAILURE})
                constraints = list(success_constraints(self.source, schema))
                self.assertIn([True], constraints)
                self.assertIn([False], constraints)
                success_envelopes.add(schema["allOf"][0]["$ref"])
        self.assertEqual(len(success_envelopes), 21)

    def test_every_corrected_failure_accepts_null_result_and_preserves_constraints(self):
        corrected = normalize_nullable(pipeline.apply_overlay(self.source, self.patch))
        failure = {"success": False, "errors": [{"code": 10000, "message": "fixture denied"}],
                   "messages": [], "result": None}
        for site in self.sites:
            with self.subTest(operation=site["operationId"]):
                schema = read_pointer(corrected, site["responseSchemaPath"])
                validator = Draft4Validator({**corrected, **schema})
                self.assertEqual(list(validator.iter_errors(failure)), [])
                self.assertFalse(validator.is_valid({**failure, "success": True}))
                self.assertFalse(validator.is_valid({**failure, "result": {}}))
                self.assertFalse(validator.is_valid({**failure, "errors": "not an error array"}))
                self.assertFalse(validator.is_valid({key: value for key, value in failure.items() if key != "result"}))

    def test_only_the_47_response_leaves_change_and_input_is_unchanged(self):
        before = json_bytes(self.source)
        corrected = pipeline.apply_overlay(self.source, self.patch)
        self.assertEqual(json_bytes(self.source), before)
        self.assertEqual(json_bytes(corrected["components"]), json_bytes(self.source["components"]))
        restored = copy.deepcopy(corrected)
        for site in self.sites:
            original = self.source["paths"][site["path"]][site["method"].lower()]
            changed = corrected["paths"][site["path"]][site["method"].lower()]
            with self.subTest(operation=site["operationId"]):
                self.assertEqual(json_bytes({k: v for k, v in original.items() if k != "responses"}),
                                 json_bytes({k: v for k, v in changed.items() if k != "responses"}))
                for status, response in original["responses"].items():
                    if status != "4XX":
                        self.assertEqual(json_bytes(response), json_bytes(changed["responses"][status]))
                parent, key = pipeline.pointer_parent(restored, site["responseSchemaPath"])
                parent[key] = copy.deepcopy(read_pointer(self.source, site["responseSchemaPath"]))
        self.assertEqual(json_bytes(restored), before)

    def test_every_operation_identity_change_is_rejected(self):
        for site in self.sites:
            with self.subTest(operation=site["operationId"]):
                changed = copy.deepcopy(self.source)
                changed["paths"][site["path"]][site["method"].lower()]["operationId"] += "-upstream-changed"
                before = json_bytes(changed)
                with self.assertRaisesRegex(ValueError, "source test failed"):
                    pipeline.apply_overlay(changed, self.patch)
                self.assertEqual(json_bytes(changed), before)

    def test_every_response_shape_change_is_rejected(self):
        for site in self.sites:
            with self.subTest(operation=site["operationId"]):
                changed = copy.deepcopy(self.source)
                read_pointer(changed, site["responseSchemaPath"])["allOf"].reverse()
                before = json_bytes(changed)
                with self.assertRaisesRegex(ValueError, "source test failed"):
                    pipeline.apply_overlay(changed, self.patch)
                self.assertEqual(json_bytes(changed), before)

    def test_shared_schema_changes_require_review(self):
        for reference in [COMMON, FAILURE]:
            with self.subTest(reference=reference):
                changed = copy.deepcopy(self.source)
                read_pointer(changed, reference)["description"] = "Changed upstream definition"
                with self.assertRaisesRegex(ValueError, "source test failed"):
                    pipeline.apply_overlay(changed, self.patch)

    def test_every_replacement_requires_its_immediately_preceding_guard(self):
        for index, item in enumerate(self.patch):
            if item["op"] != "replace":
                continue
            with self.subTest(path=item["path"]):
                unguarded = self.patch[:index - 1] + self.patch[index:]
                with self.assertRaisesRegex(ValueError, "adjacent source test"):
                    pipeline.apply_overlay(self.source, unguarded)

    def test_already_corrected_source_is_not_silently_repatched(self):
        corrected = pipeline.apply_overlay(self.source, self.patch)
        before = json_bytes(corrected)
        with self.assertRaisesRegex(ValueError, "source test failed"):
            pipeline.apply_overlay(corrected, self.patch)
        self.assertEqual(json_bytes(corrected), before)


if __name__ == "__main__":
    unittest.main()
