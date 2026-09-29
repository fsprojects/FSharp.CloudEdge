---
title: Credentials
description: The account ID and API token your F# programs use.
order: 2
---

<div class="ce-block-head">
<p class="ce-block-lead">Your F# programs call Cloudflare's API with two values: your account ID and an API token. You find both in the Cloudflare dashboard, and your programs read them from environment variables.</p>
<ul class="ce-facts">
<li><span>You need</span> A Cloudflare account</li>
<li><span>You get</span> <code>CLOUDFLARE_ACCOUNT_ID</code> and <code>CLOUDFLARE_API_TOKEN</code>, kept in <code>.env</code></li>
</ul>
</div>

## Account ID

The account ID identifies your Cloudflare account. Most account-level operations in the generated clients take it as an `accountId` argument, `StorageClient.D1CreateDatabase` among them.

1. Open the Cloudflare dashboard and go to **Account home**.
2. Select **Search**, or press `Ctrl+K` (`Cmd+K` on a Mac).
3. Enter `Copy account ID` and choose the result. The ID is now on your clipboard.

The **Account Details** section of **Workers & Pages** shows the same ID, with a copy button beside **Account ID**.

## Scoped API Token

Your programs send the token with every request, and Cloudflare checks each request against the token's permissions. Create a user token with the five permissions that the later examples require.

1. Go to **My Profile** > **API Tokens** and select **Create Token**.
2. In the **Custom token** section, click **Get started** beside **Create Custom Token**.
3. Under **Token name**, enter `hello-worker`.
4. Under **Permissions**, add a row for each permission in the table below. Use **Add more** to start each new row.
5. In each row, set the first menu to **Account** and the last one to **Edit**. In the middle menu, pick the permission group from the table, such as **D1**.
6. Under **Account Resources**, keep **Include** and choose your account.
7. Select **Continue to summary**, review the five permissions, and finish with **Create Token**.
8. Copy the token. The same page has a `curl` command in its **Test this token** section. Run it. A reply with `"status": "active"` means the token works.

:::warning
Cloudflare shows the token once. Before you leave the page, save it in a password manager for the `.env` steps.
:::

## Token Permissions

All five are **Account** permissions at the **Edit** level. On the API tab of Cloudflare's [permissions reference](https://developers.cloudflare.com/fundamentals/api/reference/permissions/), the same permission names end in **Write** where the dashboard uses **Edit**.

| Permission | Covers | Used on |
| --- | --- | --- |
| D1 Edit | D1 databases | [Account Setup](../libraries/control-plane/account-setup.md) |
| Workers R2 Storage Edit | R2 buckets | [Account Setup](../libraries/control-plane/account-setup.md) |
| Workers KV Storage Edit | KV namespaces | [Account Setup](../libraries/control-plane/account-setup.md) |
| Queues Edit | Queues | [Account Setup](../libraries/control-plane/account-setup.md) |
| Workers Scripts Edit | Worker scripts | [Worker Upload](../libraries/control-plane/worker-upload.md), [First Deploy](first-deploy.md) |

Other operations in the generated clients may require other permissions. You can add them later, since Cloudflare lets you edit an existing token.

## Token Safety

- **Use a scoped token, never the Global API Key.** That key has the same permissions as your user, on all of your resources. Anyone holding your new token can perform the actions its five permissions grant.
- **Keep tokens out of source control.** The token belongs in `.env`, which Git ignores in the FSharp.CloudEdge folder. GitHub scans public repositories for Cloudflare tokens. When it finds one, Cloudflare revokes the token and notifies you by email.
- **Roll a lost or leaked token.** On **My Profile** > **API Tokens**, open the three-dot menu next to the token and choose **Roll**, then **Confirm**. Cloudflare invalidates the old secret, and the new one has the same permissions.

## The .env File

You clone FSharp.CloudEdge into your `repos` folder on [Local Build](local-build.md), the next page. Its root holds a template for this file, `.env.template`, and its `.gitignore` lists `.env`. Run the steps below after Local Build and before [First Deploy](first-deploy.md).

1. From your `repos` folder, copy the template and make `.env` readable by your user alone.

   ```bash
   cd FSharp.CloudEdge
   cp .env.template .env
   chmod 600 .env
   cat .env
   ```

   ```text
   # Cloudflare dashboard: Account home > Search > "Copy account ID"
   CLOUDFLARE_ACCOUNT_ID=
   # Cloudflare dashboard: My Profile > API Tokens > Create Token
   CLOUDFLARE_API_TOKEN=
   ```

2. Open `.env` in your editor. Paste each value straight after its `=` sign: the account ID on the `CLOUDFLARE_ACCOUNT_ID=` line and the token on the `CLOUDFLARE_API_TOKEN=` line. Save your changes.

3. Confirm that Git ignores `.env`.

   ```bash
   git check-ignore .env
   ```

   ```text
   .env
   ```

## Shell Variables

`Environment.GetEnvironmentVariable` reads the environment that your terminal passes to each program it starts. Load `.env` into that environment from the FSharp.CloudEdge folder.

```bash
set -a
source .env
set +a
```

`set -a` marks every variable that `source` reads from `.env` for export, and `set +a` turns the marking off. Check the result:

```bash
printenv CLOUDFLARE_ACCOUNT_ID
```

It prints your account ID. If the output is empty, check that you saved your account ID in `.env`, then run the three lines again in this terminal.

In each new terminal, load `.env` again. From your hello-worker folder, the path is `../FSharp.CloudEdge/.env`:

```bash
set -a
source ../FSharp.CloudEdge/.env
set +a
```

## Client Setup

Every generated client takes an `HttpClient`. `cloudflareHttp` returns one with Cloudflare's API base address and your token in its `Authorization` header. When a variable is missing, `fromEnvironment` reports the name and exits.

```fsharp
module ClientSetup

open System
open System.Net.Http
open System.Net.Http.Headers

let fromEnvironment name =
    match Environment.GetEnvironmentVariable name with
    | null | "" ->
        eprintfn "%s is not set. Load .env into this terminal first." name
        exit 1
    | value -> value

let accountId () = fromEnvironment "CLOUDFLARE_ACCOUNT_ID"

let cloudflareHttp () =
    let token = fromEnvironment "CLOUDFLARE_API_TOKEN"
    let http = new HttpClient()
    http.BaseAddress <- Uri "https://api.cloudflare.com/client/v4"
    let headers = http.DefaultRequestHeaders
    headers.Authorization <- AuthenticationHeaderValue("Bearer", token)
    http
```

A program that calls `cloudflareHttp` before you load `.env` prints this line:

```text
CLOUDFLARE_API_TOKEN is not set. Load .env into this terminal first.
```

## Token Check

`UserApiTokensVerifyToken` on the Tenancy client sends `GET /user/tokens/verify`, the same request as the **Test this token** command. The result is a union. `OK` holds the reply to HTTP 200, and `Status4XX` holds the status code and Cloudflare's errors for a 4xx response. `checkToken` takes the `HttpClient` from Client Setup.

```fsharp
module TokenCheck

open System.Net.Http
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Tenancy

let checkToken (http: HttpClient) =
    task {
        let tenancy = TenancyClient http
        match! tenancy.UserApiTokensVerifyToken() with
        | UserApiTokensVerifyToken.OK payload ->
            let tokenStatus =
                match payload.result with
                | Some result -> string result["status"]
                | None -> "unknown"
            printfn "Token status: %s" tokenStatus
            return tokenStatus = "active"
        | UserApiTokensVerifyToken.Status4XX(httpStatus, failure) ->
            for error in failure.errors do
                eprintfn "HTTP %d: %s" httpStatus error.message
            return false
    }
```

With a working token, `checkToken` returns `true`, and the output is:

```text
Token status: active
```

For a rejected token, the `Status4XX` branch prints the HTTP status with each error message from Cloudflare's reply, and `checkToken` returns `false`.

## Next Step

<div class="ce-next">
<a class="ce-next__card" href="/FSharp.CloudEdge/guide/local-build/"><strong>Local Build</strong><span>Clone and build the libraries</span></a>
</div>
