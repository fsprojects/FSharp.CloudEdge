import { Agent, routeAgentRequest } from 'agents';
import { Harness, createRegistry } from '@earendil-works/pi-durable';
import { createModels, fauxProvider, fauxAssistantMessage } from '@earendil-works/pi-ai';
import { createHarness, contextName, structuredInput, explicitUndefinedInput, submitText, submitInput, promptInput, submitSessionInput, promptSessionInput } from 'cloudedge-pi-client';

export class PiProbe extends Agent {
  constructor(state, env) {
    super(state, env);
    const faux = fauxProvider({ api: 'cloudedge-fixture', models: [{ id: 'fixture', input: ['text', 'image'] }] });
    faux.setResponses(Array.from({ length: 20 }, () => fauxAssistantMessage('fixture reply')));
    const models = createModels();
    models.setProvider(faux.provider);
    this.pi = createHarness(({ storage, context }) => {
      this.invocationContext = contextName({ storage, context });
      return Harness.open(storage, { models, registry: createRegistry() }, context);
    }, faux.getModel().provider, faux.getModel().id);
    this.lifecycle.use(this.pi);
  }

  async onRequest(request) {
    const input = structuredInput('visual question');
    if (new URL(request.url).searchParams.get('action') === 'inspect') {
      const duplicate = await submitInput(this.pi, input, 'shared-submit');
      return Response.json({ duplicate, messages: await this.pi.messages() });
    }
    const captured = [];
    const original = this.pi.submit.bind(this.pi);
    this.pi.submit = (value, options) => { captured.push(value); return original(value, options); };
    let nullRejected = false;
    try { await submitText(this.pi, null, 'invalid-null'); } catch { nullRejected = captured.length === 0; }
    const text = await submitText(this.pi, 'plain text', 'text-submit');
    await this.pi.wait(text.operationId);
    const receipt = await submitInput(this.pi, input, 'shared-submit');
    const duplicate = await submitInput(this.pi, input, 'shared-submit');
    await this.pi.wait(receipt.operationId);
    const response = await promptInput(this.pi, input, 'harness-prompt');
    const session = this.pi.session();
    const sessionReceipt = await submitSessionInput(session, input, 'session-submit');
    await session.wait(sessionReceipt.operationId);
    const sessionResponse = await promptSessionInput(session, input, 'session-prompt');
    const undefinedReceipt = await submitInput(this.pi, explicitUndefinedInput('explicit undefined'), 'undefined-submit');
    await this.pi.wait(undefinedReceipt.operationId);
    const image = captured[1];
    const explicit = captured.at(-1)[0];
    return Response.json({ text, receipt, duplicate, response, sessionReceipt, sessionResponse,
      context: this.invocationContext, messages: await this.pi.messages(),
      wire: { nullRejected, plain: captured[0], image, omittedSignature: !Object.hasOwn(image[0], 'textSignature'),
        ownUndefinedSignature: Object.hasOwn(explicit, 'textSignature') && explicit.textSignature === undefined } });
  }
}

export default {
  async fetch(request, env) { return await routeAgentRequest(request, env) ?? new Response('missing', { status: 404 }); }
};
