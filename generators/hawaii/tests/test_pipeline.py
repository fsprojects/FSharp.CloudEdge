import importlib.util
from pathlib import Path
import unittest

spec = importlib.util.spec_from_file_location("hawaii_consumer", Path(__file__).resolve().parents[1] / "generate.py")
pipeline = importlib.util.module_from_spec(spec)
spec.loader.exec_module(pipeline)


class ConsumerContracts(unittest.TestCase):
    def test_overlay_preserves_source_and_moves_entire_response(self):
        source = {"responses": {"4xx": {"content": {"application/json": {"schema": {"type": "string"}}}}}}
        result = pipeline.apply_overlay(source, [{"op": "move", "from": "/responses/4xx", "path": "/responses/4XX"}])
        self.assertEqual(result["responses"]["4XX"], source["responses"]["4xx"])
        self.assertIn("4xx", source["responses"])
        self.assertNotIn("4xx", result["responses"])

    def test_overlay_refuses_overwrite_or_missing_source(self):
        with self.assertRaises(ValueError):
            pipeline.apply_overlay({"a": 1}, [{"op": "add", "path": "/a", "value": 2}])
        with self.assertRaises(KeyError):
            pipeline.apply_overlay({}, [{"op": "move", "from": "/missing", "path": "/a"}])

    def test_json_pointer_escaping(self):
        result = pipeline.apply_overlay({"a/b~c": {}}, [{"op": "add", "path": "/a~1b~0c/value", "value": 1}])
        self.assertEqual(result, {"a/b~c": {"value": 1}})

    def test_query_encoding_overlay_keeps_schema_through_parameter_array(self):
        source = {"parameters": [{"name": "filter", "schema": {"type": "array", "items": {"type": "object"}}}]}
        result = pipeline.apply_overlay(source, [
            {"op": "add", "path": "/parameters/0/content", "value": {"application/json": {}}},
            {"op": "move", "from": "/parameters/0/schema", "path": "/parameters/0/content/application~1json/schema"},
        ])
        self.assertEqual(result["parameters"][0]["content"]["application/json"]["schema"], source["parameters"][0]["schema"])
        self.assertIn("schema", source["parameters"][0])
        for index in ["-1", "00", "-"]:
            with self.assertRaises(ValueError):
                pipeline.apply_overlay(source, [{"op": "add", "path": f"/parameters/{index}/content", "value": {}}])

    def test_unknown_patch_operation_is_not_ignored(self):
        with self.assertRaises(ValueError):
            pipeline.apply_overlay({}, [{"op": "remove", "path": "/a"}])

    def test_response_correction_requires_exact_source_and_preserves_original(self):
        original = {"response": {"allOf": [{"$ref": "success"}, {"$ref": "failure"}]}}
        patch = [{"op": "test", "path": "/response", "value": original["response"]},
                 {"op": "replace", "path": "/response", "value": {"$ref": "failure"}}]
        corrected = pipeline.apply_overlay(original, patch)
        self.assertEqual(corrected, {"response": {"$ref": "failure"}})
        self.assertIn("allOf", original["response"])
        with self.assertRaisesRegex(ValueError, "source test failed"):
            pipeline.apply_overlay(corrected, patch)

    def test_replacement_rejects_unguarded_wrong_and_reused_tests(self):
        source = {"response": {"type": "object"}, "other": 1}
        replacement = {"op": "replace", "path": "/response", "value": {"type": "string"}}
        for prefix in [[], [{"op": "test", "path": "/other", "value": 1}],
                       [{"op": "test", "path": "/response", "value": source["response"]},
                        {"op": "add", "path": "/unrelated", "value": 2}]]:
            with self.subTest(prefix=prefix), self.assertRaisesRegex(ValueError, "adjacent source test"):
                pipeline.apply_overlay(source, prefix + [replacement])
        with self.assertRaisesRegex(ValueError, "adjacent source test"):
            pipeline.apply_overlay(source, [{"op": "test", "path": "/response", "value": source["response"]}, replacement, replacement])

    def test_source_test_rejects_changed_absent_and_differently_typed_values(self):
        for source, expected in [({}, 1), ({"value": 2}, 1), ({"value": True}, 1),
                                 ({"value": {"required": ["result", "success"]}}, {"required": ["success", "result"]})]:
            with self.subTest(source=source), self.assertRaisesRegex(ValueError, "source test failed"):
                pipeline.apply_overlay(source, [{"op": "test", "path": "/value", "value": expected}])

    def test_failed_later_correction_does_not_mutate_source(self):
        source = {"first": 1, "second": 2}
        with self.assertRaisesRegex(ValueError, "source test failed"):
            pipeline.apply_overlay(source, [{"op": "test", "path": "/first", "value": 1},
                                            {"op": "replace", "path": "/first", "value": 3},
                                            {"op": "test", "path": "/second", "value": 99}])
        self.assertEqual(source, {"first": 1, "second": 2})

    def source(self):
        return {"paths": {"/old": {"get": {"operationId": "old", "deprecated": True}},
                          "/current": {"post": {"operationId": "current"}}},
                "components": {"schemas": {"Shared": {"type": "string"}}}}

    def retirement(self):
        return {"method": "get", "path": "/old", "operationId": "old", "disposition": "exclude-retired",
                "retiredAt": "2025-01-01", "reason": "Fixture retirement",
                "sources": ["https://developers.cloudflare.com/example-retirement"]}

    def policy(self, *decisions):
        return {"asOf": "2026-09-06", "operations": list(decisions)}

    def test_deprecation_is_not_automatic_retirement(self):
        selected, coverage = pipeline.select_lifecycle(self.source(), {"operations": []})
        self.assertEqual(len(pipeline.operations(selected)), 2)
        self.assertTrue(all(op["disposition"] == "included" for op in coverage))

    def test_explicit_retirement_is_accounted_for_without_deleting_shared_types(self):
        source = self.source()
        selected, coverage = pipeline.select_lifecycle(source, self.policy(self.retirement()))
        self.assertEqual(set(selected["paths"]), {"/current"})
        self.assertEqual(selected["components"], source["components"])
        self.assertIn("/old", source["paths"])
        self.assertEqual(sum(op["disposition"] == "excluded-retired" for op in coverage), 1)
        excluded = next(op for op in coverage if op["path"] == "/old")
        self.assertEqual(excluded["selectionEvidence"], self.retirement())
        self.assertEqual(excluded["selectionAsOf"], "2026-09-06")

    def test_missing_evidence_changed_identity_and_duplicate_decisions_fail(self):
        for modification in [{"sources": []}, {"operationId": "another"}, {"disposition": "deprecated"}]:
            entry = self.retirement() | modification
            with self.subTest(modification=modification), self.assertRaises(ValueError):
                pipeline.select_lifecycle(self.source(), self.policy(entry))
        with self.assertRaises(ValueError):
            pipeline.select_lifecycle(self.source(), self.policy(self.retirement(), self.retirement()))

    def test_retirement_requires_a_valid_date_no_later_than_review(self):
        for retired_at in [None, "2026-09-07", "2026-02-30", "20260101", "2026-01-01T00:00:00Z"]:
            with self.subTest(retired_at=retired_at), self.assertRaises(ValueError):
                pipeline.select_lifecycle(self.source(), self.policy(self.retirement() | {"retiredAt": retired_at}))
        for as_of in [None, "tomorrow", "2024-12-31"]:
            with self.subTest(as_of=as_of), self.assertRaises(ValueError):
                pipeline.select_lifecycle(self.source(), self.policy(self.retirement()) | {"asOf": as_of})
        selected, _ = pipeline.select_lifecycle(self.source(), self.policy(self.retirement()) | {"asOf": "2025-01-01"})
        self.assertNotIn("/old", selected["paths"])

    def test_internal_route_exclusion_has_its_own_evidence_without_retirement(self):
        internal = self.retirement() | {"disposition": "exclude-internal", "reason": "Publisher identifies an internal test route"}
        del internal["retiredAt"]
        selected, coverage = pipeline.select_lifecycle(self.source(), self.policy(internal))
        self.assertNotIn("/old", selected["paths"])
        self.assertEqual(next(op for op in coverage if op["path"] == "/old")["disposition"], "excluded-internal")
        with self.assertRaises(ValueError):
            pipeline.select_lifecycle(self.source(), self.policy(internal | {"retiredAt": "2025-01-01"}))

    def test_publisher_flags_do_not_implicitly_remove_customer_endpoints(self):
        source = self.source()
        source["paths"]["/old"]["get"].update({"x-fern-ignore": True, "x-forge-hidden": True,
                                               "x-cfDeprecation": {"eol": "2020-01-01T00:00:00Z"}})
        selected, _ = pipeline.select_lifecycle(source, self.policy())
        self.assertEqual(selected, source)

    def test_taxonomy_has_one_owner_and_retains_unknown_operations(self):
        source = self.source()
        _, coverage = pipeline.select_lifecycle(source, {"operations": []})
        rule = {"match": "exact", "path": "/current", "area": "Management", "purpose": "Compute", "family": "sample"}
        unresolved = pipeline.assign_taxonomy(source, coverage, {"rules": [rule]})
        self.assertEqual([entry["path"] for entry in unresolved], ["/old"])
        self.assertEqual(next(op for op in coverage if op["path"] == "/current")["taxonomy"]["family"], "sample")
        self.assertEqual(len(coverage), 2)
        with self.assertRaises(ValueError):
            pipeline.assign_taxonomy(source, coverage, {"rules": [rule, rule]})


if __name__ == "__main__":
    unittest.main()
