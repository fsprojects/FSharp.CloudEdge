import assert from 'node:assert/strict';
import { replyToView, replyToBody } from './fable-out/Bridge.js';

const ask = Uint8Array.from([1, 0x78, 0x56, 0x34, 0x12, 1, 2, 3, 4]);
const expectedReply = [2, 0x78, 0x56, 0x34, 0x12, 1, 2, 3, 4];
let passed = 0;

function success(result, label) {
  assert.equal(result.tag, 0, label);
  assert.deepEqual(Array.from(result.fields[0]), expectedReply, label);
  passed++;
}

function failure(result, expected, label) {
  assert.equal(result.tag, 1, label);
  assert.equal(result.fields[0], expected, label);
  passed++;
}

success(replyToView(ask), 'whole byte view');
const backing = Uint8Array.from([91, 92, ...ask, 93, 94]);
const window = backing.subarray(2, 2 + ask.length);
success(replyToView(window), 'nonzero offset excludes surrounding bytes');
success(replyToView(Buffer.from(backing).subarray(2, 2 + ask.length)), 'Node Buffer view normalized');

const body = new Response(window);
success(await replyToBody(body), 'generated Workers Body.bytes into BAREWire');
failure(await replyToBody(body), 'Unreadable body', 'consumed Body rejection becomes a typed failure');
failure(replyToView(Uint8Array.from([1, 2])), 'Malformed envelope', 'short envelope');
failure(replyToView(Uint8Array.from([255, ...ask.subarray(1)])), 'Malformed envelope', 'unknown envelope kind');
failure(replyToView(Uint8Array.from([0, ...ask.subarray(1)])), 'Expected Ask', 'valid unexpected envelope kind');
failure(replyToView(ask.subarray(0, ask.length - 1)), 'Payload must contain exactly one u32', 'truncated payload');
failure(replyToView(Uint8Array.from([...ask, 99])), 'Payload must contain exactly one u32', 'trailing payload');
failure(replyToView(new Uint8Array(0)), 'Malformed envelope', 'empty view');

const detached = ask.slice();
structuredClone(detached.buffer, { transfer: [detached.buffer] });
failure(replyToView(detached), 'Unreadable byte view', 'detached buffer');
assert.deepEqual(Array.from(backing), [91, 92, ...ask, 93, 94], 'input remains unchanged');
passed++;

console.log(JSON.stringify({
  passed,
  runtime: process.version,
  scope: 'Fable, generated Workers Body bindings and BAREWire codecs in Node Fetch',
}));
