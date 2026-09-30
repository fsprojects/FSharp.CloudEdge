---
title: Client Catalog
description: The management and tenancy clients and what each covers.
order: 23
---

<div class="ce-block-head">
<p class="ce-block-lead">Each of the eleven clients covers one purpose in Cloudflare's REST API, with a method for every operation in it. The program that uploads a Worker can also configure the domain it serves, from DNS records to a Turnstile widget on a signup form.</p>
<ul class="ce-facts">
<li><span>Clients</span> 11</li>
<li><span>Operations</span> 3,437</li>
<li><span>Service families</span> 163</li>
<li><span>Schema</span> <code>cloudflare/api-schemas</code> at <code>f2df0ca</code></li>
</ul>
</div>

## Signup Widget

`AccountsTurnstileWidgetCreate` creates a Turnstile widget for the listed domains and their subdomains. The result has the site key, which your site uses to show the widget, and the secret key for [server-side token validation](https://developers.cloudflare.com/turnstile/get-started/).

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Security

let createWidget (security: SecurityClient) accountId =
    task {
        let widget =
            AccountsTurnstileWidgetCreatePayload.Create([ "example.com" ], turnstile_widget_mode.Managed, "signup-form")
        match! security.AccountsTurnstileWidgetCreate(accountId, widget) with
        | AccountsTurnstileWidgetCreate.OK payload ->
            return payload.result |> Option.map (fun created -> created.sitekey, created.secret)
        | AccountsTurnstileWidgetCreate.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
            return None
    }
```

For the Worker that validates tokens, upload the secret key in a `secret_text` binding as on [Worker Upload](worker-upload.md).

## Chatbot Gateway

`AigConfigCreateGateway` creates an AI Gateway, here with a 300-second cache and a [rate limit](https://developers.cloudflare.com/ai-gateway/features/rate-limiting/) of 100 requests in each 60-second window. The operation declares two responses, `OK` and `BadRequest`, and the match covers both.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.AI

let createGateway (ai: AIClient) accountId =
    task {
        let gateway =
            { AigConfigCreateGatewayPayload.Create(cache_invalidate_on_update = true, collect_logs = true, id = "chatbot") with
                cache_ttl = Some 300
                rate_limiting_limit = Some 100
                rate_limiting_interval = Some 60
                rate_limiting_technique = Some AigConfigCreateGatewayPayloadRate_limiting_technique.Fixed }
        match! ai.AigConfigCreateGateway(accountId, body = gateway) with
        | AigConfigCreateGateway.OK payload ->
            printfn "Gateway %s created at %O" payload.result.id payload.result.created_at
        | AigConfigCreateGateway.BadRequest failure ->
            printfn "Gateway rejected: %A" failure.errors
    }
```

## Avatar Upload

`CloudflareImagesCreateAuthenticatedDirectUploadUrlV2` returns a URL for one unauthenticated upload, a single multipart `POST` of the image. A browser can send a user's picture to that URL. The `creator` argument is the user's ID, which Cloudflare saves in the image's creator field. The expiry must be between 2 minutes and 6 hours after the call.

```fsharp
open System
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Media

let uploadUrl (media: MediaClient) accountId (userId: string) =
    task {
        let expiry = DateTimeOffset.UtcNow.AddMinutes 30.
        match! media.CloudflareImagesCreateAuthenticatedDirectUploadUrlV2(accountId, creator = userId, expiry = expiry) with
        | CloudflareImagesCreateAuthenticatedDirectUploadUrlV2.OK payload ->
            return Some(string payload.result["uploadURL"])
        | CloudflareImagesCreateAuthenticatedDirectUploadUrlV2.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
            return None
    }
```

## Page View Report

`AnalyticsEngineSqlQueryPost` sends SQL to Workers Analytics Engine as a plain-text body. The query ends in `FORMAT JSON`, since the client decodes the `OK` response as one JSON object and the default output is newline-delimited JSON.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Observability

let topPages (observability: ObservabilityClient) accountId =
    task {
        let sql =
            "SELECT blob1 AS path, SUM(_sample_interval) AS views FROM page_views "
            + "WHERE timestamp > NOW() - INTERVAL '7' DAY "
            + "GROUP BY path ORDER BY views DESC LIMIT 10 FORMAT JSON"
        match! observability.AnalyticsEngineSqlQueryPost(accountId, sql) with
        | AnalyticsEngineSqlQueryPost.OK report ->
            for row in report.data do
                printfn "%O  %O" row["path"] row["views"]
        | AnalyticsEngineSqlQueryPost.BadRequest message ->
            printfn "Query rejected: %s" message
        | other ->
            printfn "%A" other
    }
```

<div class="ce-needs"><p><strong>Needs</strong> a Worker that writes page paths to <code>blob1</code> of a dataset named <code>page_views</code>, through an <code>analytics_engine</code> binding as listed on <a href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/">Worker Upload</a>.</p></div>

## Contact Address

An Email Routing rule forwards mail for one address on your domain to another inbox. Forward actions require a verified destination address. The operation's two error responses are union cases without a payload: `BadRequest` for an unverified destination and `UnprocessableEntity` for invalid input.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Messaging

let forwardContact (messaging: MessagingClient) zoneId =
    task {
        let toContact =
            { email_rule_matcher.Create email_rule_matcherType.Literal with
                field = Some Field.To
                value = Some "hello@example.com" }
        let forward = { email_rule_action.Create email_rule_actionType.Forward with value = Some [ "owner@example.net" ] }
        let rule = { email_create_rule_properties.Create([ forward ], [ toContact ]) with name = Some "Contact address" }
        match! messaging.EmailRoutingRoutingRulesCreateRoutingRule(zoneId, rule) with
        | EmailRoutingRoutingRulesCreateRoutingRule.OK _ ->
            printfn "hello@example.com forwards to owner@example.net"
        | EmailRoutingRoutingRulesCreateRoutingRule.BadRequest ->
            printfn "Rule rejected with HTTP 400"
        | EmailRoutingRoutingRulesCreateRoutingRule.UnprocessableEntity ->
            printfn "Rule rejected with HTTP 422"
    }
```

<div class="ce-needs"><p><strong>Needs</strong> a verified destination address and the <a href="https://developers.cloudflare.com/fundamentals/account/find-account-and-zone-ids/">zone ID</a> from the domain's Overview page. <code>EmailRoutingSettingsEnableEmailRouting</code> turns on Email Routing for the zone and adds its MX and SPF records.</p></div>

## Tiered Cache

With Smart Tiered Cache, Cloudflare picks the upper-tier data center for each of a site's origins from its latency data. This call turns it on for one zone.

```fsharp
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.ContentDelivery

let enable (delivery: ContentDeliveryClient) zoneId =
    task {
        let turnOn = cache_u002D_rules_smart_tiered_cache_patch.Create cache_u002D_rules_smart_tiered_cache_patchValue.On
        match! delivery.SmartTieredCachePatchSmartTieredCacheSetting(zoneId, turnOn) with
        | SmartTieredCachePatchSmartTieredCacheSetting.OK _ ->
            printfn "Smart Tiered Cache is on"
        | SmartTieredCachePatchSmartTieredCacheSetting.Status4XX(status, failure) ->
            printfn "HTTP %d: %O" status failure.errors
    }
```

## Shop Record

The request type for a DNS record is a JSON wrapper, one of 217 such types in `Core.Api`. Its `FromJson` method takes a `JsonElement`, which `JsonSerializer.SerializeToElement` produces from an anonymous record. This CNAME points `shop.example.com` at another host through Cloudflare's proxy. The `name` is the full name including the zone, and a `ttl` of 1 means automatic.

```fsharp
open System.Text.Json
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Networking

let addRecord (networking: NetworkingClient) zoneId =
    task {
        let record =
            JsonSerializer.SerializeToElement
                {| ``type`` = "CNAME"; name = "shop.example.com"; content = "shop.example.net"; proxied = true; ttl = 1 |}
            |> dns_u002D_records_dns_u002D_record_u002D_post.FromJson
        match! networking.DnsRecordsForAZoneCreateDnsRecord(zoneId, record) with
        | DnsRecordsForAZoneCreateDnsRecord.OK payload ->
            printfn "DNS record created: %b" payload.success
        | DnsRecordsForAZoneCreateDnsRecord.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
    }
```

## Library Table

| Library | Operations | What it covers |
| --- | ---: | --- |
| **Application Platform** | | |
| `Management.Compute` | 304 | Workers, Pages, Queues, Workflows, Containers |
| `Management.Storage` | 153 | R2, D1, Workers KV, Vectorize, Hyperdrive |
| `Management.AI` | 152 | AI Gateway, AI Search, Workers AI |
| `Management.Media` | 175 | Stream, Images, Realtime, Calls |
| **Network and Security** | | |
| `Management.Networking` | 505 | DNS, load balancing, Tunnel, Spectrum |
| `Management.ContentDelivery` | 65 | Cache settings, Pay per crawl, Smart Shield |
| `Management.Security` | 1,295 | Zero Trust, rulesets, API Shield, Turnstile |
| **Insight and Messaging** | | |
| `Management.Observability` | 386 | Logs, Logpush, analytics, Radar |
| `Management.Messaging` | 98 | Email Routing, email sending, notifications |
| **Account Administration** | | |
| `Tenancy` | 300 | Accounts, members, API tokens, zones |
| `Management.Browser` | 4 | Browser extension settings |

`Core.Api` contains every payload type and response union. The clients also share its HTTP transport. Cloudflare's OpenAPI document has 3,448 operations. The difference is eight retired operations and three internal routes.

<details><summary>Service families in each client</summary>

- `Management.Compute`, 11 families: `workers` 114, `builds` 31, `browser-rendering` 28, `pages` 26, `queues` 23, `workflows` 23, `pipelines` 19, `containers` 14, `flagship` 14, `snippets` 8, `triggers` 4
- `Management.Storage`, 9 families: `r2` 46, `vectorize` 24, `artifacts` 17, `storage` 14, `r2-catalog` 13, `d1` 12, `secrets-store` 12, `hyperdrive` 8, `resource-library` 7
- `Management.AI`, 6 families: `ai-gateway` 66, `ai-search` 49, `agent-memory` 14, `ai` 14, `autorag` 7, `ai-audit` 2
- `Management.Media`, 6 families: `realtime` 61, `stream` 50, `images` 44, `calls` 10, `moq` 8, `media` 2
- `Management.Networking`, 29 families: `magic` 177, `load-balancers` 42, `addressing` 41, `secondary-dns` 28, `waiting-rooms` 24, `custom-pages` 18, `cni` 16, `mnm` 16, `dns-records` 15, `teamnet` 14, `cfd-tunnel` 12, `web3` 12, `warp-connector` 11, `dns-firewall` 9, `dns-settings` 9, `healthchecks` 9, `spectrum` 9, `data-localization` 7, `origin` 7, `connectivity` 5, `custom-ns` 5, `argo` 4, `dnssec` 4, `hostnames` 4, `cloud-connector` 2, `dns-analytics` 2, `cache` 1, `ips` 1, `tunnels` 1
- `Management.ContentDelivery`, 6 families: `cache` 21, `pay-per-crawl` 17, `smart-shield` 10, `environments` 7, `pagerules` 7, `url-normalization` 3
- `Management.Security`, 53 families: `cloudforce-one` 231, `access` 165, `dlp` 93, `email-security` 73, `devices` 70, `gateway` 57, `api-gateway` 44, `firewall` 42, `brand-protection` 41, `intel` 39, `data-security` 36, `rulesets` 32, `dex` 31, `security-center` 26, `vuln-scanner` 22, `scim` 16, `schema-validation` 15, `token-validation` 15, `zerotrust` 15, `urlscanner` 14, `origin-tls-client-auth` 13, `page-shield` 13, `ssl` 12, `custom-hostnames` 11, `one` 11, `rules` 11, `zt-risk-scoring` 11, `abuse-reports` 10, `pcaps` 9, `content-upload-scan` 8, `custom-csrs` 8, `infrastructure` 8, `filters` 7, `leaked-credential-checks` 7, `oauth-clients` 7, `advanced-certificates` 6, `challenges` 6, `custom-certificates` 6, `sso-connectors` 6, `client-certificates` 5, `keyless-certificates` 5, `mtls-certificates` 5, `rate-limits` 5, `ai-security` 4, `bot-management` 4, `botnet-feed` 4, `certificates` 4, `managed-headers` 3, `certificate-authorities` 2, `ct` 2, `fraud-detection` 2, `precursor` 2, `dcv-delegation` 1
- `Management.Observability`, 12 families: `radar` 273, `logpush` 34, `logs` 29, `rum` 13, `analytics` 10, `speed-api` 10, `diagnostics` 6, `reporting` 6, `analytics-engine` 2, `audit-logs` 1, `rate-limit-analytics` 1, `request-tracer` 1
- `Management.Messaging`, 4 families: `email` 64, `alerting` 25, `event-subscriptions` 5, `event-notifications` 4
- `Tenancy`, 26 families: `settings` 57, `user` 55, `shares` 20, `billing` 18, `organizations` 18, `iam` 17, `zones` 16, `registrar` 13, `subscriptions` 13, `registrar-sandbox` 10, `tags` 10, `tenants` 8, `tokens` 8, `accounts` 7, `payment-methods` 6, `members` 5, `memberships` 4, `subscription` 4, `entitlements` 2, `profile` 2, `roles` 2, `invoices` 1, `oauth` 1, `pay-bad-debt` 1, `pay-invoice` 1, `receipts` 1
- `Management.Browser`, 1 family: `browser-extension` 4

</details>

## Related Pages

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/"><strong>Control Plane</strong><span>Clients and responses</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/account-setup/"><strong>Account Setup</strong><span>Storage and queues</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/control-plane/worker-upload/"><strong>Worker Upload</strong><span>Modules and bindings</span></a>
<a class="ce-next__card" href="/FSharp.CloudEdge/libraries/ai/"><strong>AI</strong><span>Models and gateways in a Worker</span></a>
</div>

## NuGet packages

[Core.Api 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Core.Api/0.1.0), [Management.AI 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.AI/0.1.0), [Management.Browser 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Browser/0.1.0), [Management.Compute 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Compute/0.1.0), [Management.ContentDelivery 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.ContentDelivery/0.1.0), [Management.Media 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Media/0.1.0), [Management.Messaging 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Messaging/0.1.0), [Management.Networking 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Networking/0.1.0), [Management.Observability 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Observability/0.1.0), [Management.Security 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Security/0.1.0), [Management.Storage 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Management.Storage/0.1.0), [Tenancy 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Tenancy/0.1.0).

See [installation and release availability](../../guide/packages.md).
