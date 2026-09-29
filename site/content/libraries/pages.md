---
title: Pages Plugins
description: Access, Turnstile and Static Forms for Pages Functions.
order: 12
---

<div class="ce-block-head">
<p class="ce-block-lead">Put a staff area behind Cloudflare Access and check a signup form with Turnstile on your Pages site. Each plugin returns a Pages Function, which an F# module exports under a handler name such as <code>onRequest</code>.</p>
<ul class="ce-facts">
<li><span>Libraries</span> <code>Runtime.PagesPluginCloudflareAccess</code> <code>Runtime.PagesPluginTurnstile</code> <code>Runtime.PagesPluginStaticForms</code> <code>Runtime.Workers</code></li>
<li><span>npm</span> <code>@cloudflare/pages-plugin-cloudflare-access</code> 1.0.5, <code>@cloudflare/pages-plugin-turnstile</code> 1.0.2, <code>@cloudflare/pages-plugin-static-forms</code> 1.0.3</li>
<li><span>Free plan</span> <a href="https://developers.cloudflare.com/pages/functions/pricing/">unlimited static asset requests, and 100,000 Functions and Workers requests a day</a></li>
</ul>
</div>

## Staff Area

Compiled to `functions/staff/_middleware.js`, this module is the middleware for `/staff` and every path below it. It validates the Access token in each request's `Cf-Access-Jwt-Assertion` header. When validation fails, it redirects the browser to your team's Access login page with a `302` status. `aud` is your Access application's audience tag, and `domain` is your team domain.

```fsharp
module Access = FSharp.CloudEdge.Runtime.PagesPluginCloudflareAccess

let onRequest =
    Access.Exports.pagesPluginCloudflareAccess(
        Access.PluginArgs.Create(
            aud = "your-application-aud-tag",
            domain = "https://your-team.cloudflareaccess.com"
        )
    )
```

<div class="ce-needs"><p><strong>Needs</strong> a Cloudflare Access application that covers <code>/staff</code>. Use its audience tag as <code>aud</code>.</p></div>

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import pages_plugin_cloudflare_access from "@cloudflare/pages-plugin-cloudflare-access";

export const onRequest = pages_plugin_cloudflare_access({
    aud: "your-application-aud-tag",
    domain: "https://your-team.cloudflareaccess.com",
});
```

</details>

## Staff Profile

After the middleware, `context.data.cloudflareAccess.JWT` holds the token's payload and a `getIdentity` function. `getIdentity` requests the user's identity from your team domain with the token. It resolves to `Some` identity when the reply has a success status, and to `None` for any other status. The identity includes the user's name and email, and its `groups` array lists the user's groups.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Access = FSharp.CloudEdge.Runtime.PagesPluginCloudflareAccess

let onRequestGet (context: Workers.EventContext<obj, string, Access.PluginData>) =
    async {
        let! identity = context.data.cloudflareAccess.JWT.getIdentity () |> Async.AwaitPromise
        match identity with
        | Some person ->
            let profile = {| name = person.name; email = person.email; groups = person.groups |}
            return Workers.Exports.Response.json profile
        | None ->
            let notFound = Workers.ResponseInit.Create(status = 404.)
            return Workers.Exports.Response.Create("No identity for this sign-in", notFound)
    }
    |> Async.StartAsPromise
```

<div class="ce-needs"><p><strong>Needs</strong> the Staff Area middleware in the same directory or a parent directory.</p></div>

## Sign-out Link

`generateLogoutURL` builds the Access logout address for your team domain. The function responds with a `302` redirect to that address.

```fsharp
module Workers = FSharp.CloudEdge.Runtime.Workers
module Access = FSharp.CloudEdge.Runtime.PagesPluginCloudflareAccess

let onRequestGet (_: Workers.EventContext<obj, string, obj>) =
    let logout =
        Access.Api.Exports.generateLogoutURL(
            Access.Api.GenerateLogoutURL0.Create(domain = "https://your-team.cloudflareaccess.com"))
    Workers.Exports.Response.redirect(logout, 302.)
```

<details class="ce-js"><summary>Emitted JavaScript</summary>

```javascript
import { generateLogoutURL } from "@cloudflare/pages-plugin-cloudflare-access/api";

export function onRequestGet(_arg) {
    const logout = generateLogoutURL({
        domain: "https://your-team.cloudflareaccess.com",
    });
    return globalThis.Response.redirect(logout, 302);
}
```

</details>

## Signup Form

Pages calls the functions of an exported array in order. The Turnstile plugin validates the token in the form's `cf-turnstile-response` field. When the token is valid, `register` handles the request next. For an invalid token, the plugin returns a `400` response with Turnstile's error descriptions.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Turnstile = FSharp.CloudEdge.Runtime.PagesPluginTurnstile

let verifyHuman =
    Turnstile.Exports.pagesPluginTurnstile(
        Turnstile.PluginArgs.Create(secret = "1x0000000000000000000000000000000AA"))

let register (context: Workers.EventContext<obj, string, Turnstile.PluginData>) =
    async {
        let! form = context.request.formData () |> Async.AwaitPromise
        match form.get "email" with
        | Some (U2.Case1 email) -> return Workers.Exports.Response.Create($"Thanks for signing up, {email}.")
        | _ ->
            let badRequest = Workers.ResponseInit.Create(status = 400.)
            return Workers.Exports.Response.Create("An email address is required.", badRequest)
    }
    |> Async.StartAsPromise

let onRequestPost: obj[] = [| verifyHuman; register |]
```

The type of a form value is `U2<string, File>`, and `U2.Case1` matches the text of the email field.

<div class="ce-needs"><p><strong>Needs</strong> a Turnstile widget on the signup form. The secret in the code is Turnstile's <a href="https://developers.cloudflare.com/turnstile/troubleshooting/testing/">test secret key</a>, which passes every validation. A live site uses the widget's own secret key.</p></div>

## Library Table

`Runtime.PagesPluginStaticForms` binds the plugin that handles submissions from HTML forms with a `data-static-form-name` attribute. From .NET, `Management.Security` creates Access applications with `AccessApplicationsAddAnApplication` and Turnstile widgets with `AccountsTurnstileWidgetCreate`.

| Library | npm package | What it covers |
| --- | --- | --- |
| `Runtime.PagesPluginCloudflareAccess` | `@cloudflare/pages-plugin-cloudflare-access` 1.0.5 | Access middleware, identity |
| `Runtime.PagesPluginTurnstile` | `@cloudflare/pages-plugin-turnstile` 1.0.2 | Turnstile validation |
| `Runtime.PagesPluginStaticForms` | `@cloudflare/pages-plugin-static-forms` 1.0.3 | Static form replies |
| `Runtime.Workers` | `@cloudflare/workers-types` 5.20260906.1 | Pages Functions context |

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/rpc/"><strong>RPC</strong><span>An API in a Pages Function</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/platform/"><strong>Workers</strong><span>Request handlers</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/clients/"><strong>Client Catalog</strong><span>Access and Turnstile setup</span></a>
</div>
