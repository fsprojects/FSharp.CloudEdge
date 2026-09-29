import assert from "node:assert/strict";
import { keyValue, mutableIndexer, readonlyIndexer, typedKeyValue } from "./fable-out/Smoke.js";

assert.equal(typedKeyValue(), "Count");
assert.equal(keyValue(), "Count");
assert.equal(readonlyIndexer(), 42);
assert.equal(mutableIndexer(), 42);
console.log("Xantham.Fable.Core package-only Fable types/indexers/Emit smoke: 4 checks passed.");
