import { DurableObject } from 'cloudflare:workers';
import { withInlineInterfaceTag, copyWithInlineInterfaceTag } from 'fickl-tagged-union';
import { createTaggedText, createTextPart } from 'fsharp-probe';

export class CloneProbe extends DurableObject {
  echo(value) {
    return { type: value.type, text: value.text };
  }
}

export default {
  async fetch(request, env) {
    const stub = env.PROBE.get(env.PROBE.idFromName('probe'));
    const mode = new URL(request.url).pathname;
    const plain = mode === '/missing' || mode === '/copy'
      ? { text: 'example' }
      : { type: mode === '/conflicting' ? 'image' : 'text', text: 'example' };
    const fsharpPayload = mode === '/fsharp' ? createTextPart('example') : undefined;
    const input = mode === '/fsharp' ? createTaggedText(fsharpPayload)
      : mode === '/copy' ? copyWithInlineInterfaceTag(plain, 'type', 'text')
      : mode === '/plain' ? plain
      : withInlineInterfaceTag(plain, 'type', 'text');
    const identity = input === (mode === '/fsharp' ? fsharpPayload : plain);
    const tag = input.type;
    try {
      return Response.json({ ok: true, value: await stub.echo(input), identity, tag });
    } catch (error) {
      return Response.json({ ok: false, name: error.name, identity, tag });
    }
  }
};
