import assert from "node:assert/strict";
import { brandRoundTrip, optionalValue, propertyName, propertyValue } from "./fable-out/UpstreamHelpers.js";

assert.equal(propertyName(), "Count");
assert.equal(propertyValue(), 42);
assert.equal(optionalValue(), 42);
assert.equal(brandRoundTrip("identifier"), "identifier");
console.log("Upstream support module helpers: 4 checks passed; the recorded issue can be reviewed for closure.");
