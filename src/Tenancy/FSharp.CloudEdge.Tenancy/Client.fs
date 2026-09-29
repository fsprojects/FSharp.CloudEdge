namespace rec FSharp.CloudEdge.Tenancy

open System.Net
open System.Net.Http
open System.Text
open System.Threading
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Core.Api.Http

///Welcome to Cloudflare's API documentation site. We are experimenting with an updated version of our API documentation - check out [developers.cloudflare.com/api-next/](https://developers.cloudflare.com/api-next/) to test out the new experience.
///To get started using Cloudflare's products and services via the API, refer to [how to interact with Cloudflare](https://developers.cloudflare.com/fundamentals/basic-tasks/interact-with-cloudflare/), which covers using tools like [Terraform](https://developers.cloudflare.com/terraform/#cloudflare-terraform) and the [official SDKs](https://developers.cloudflare.com/fundamentals/api/reference/sdks/) to maintain your Cloudflare resources.
///Using the Cloudflare API requires authentication so that Cloudflare knows who is making requests and what permissions you have. Create an API token to grant access to the API to perform actions. You can also authenticate with [API keys](https://developers.cloudflare.com/fundamentals/api/get-started/keys/), but these keys have [several limitations](https://developers.cloudflare.com/fundamentals/api/get-started/keys/#limitations) that make them less secure than API tokens. Whenever possible, use API tokens to interact with the Cloudflare API.
///To create an API token, from the Cloudflare dashboard, go to My Profile &amp;gt; API Tokens and select Create Token. For more information on how to create and troubleshoot API tokens, refer to
///our [API fundamentals](https://developers.cloudflare.com/fundamentals/api/).
///For information regarding rate limits, refer to our [API Rate Limits](https://developers.cloudflare.com/cloudflare-for-platforms/workers-for-platforms/platform/limits/#api-rate-limits).
///Totally new to Cloudflare? [Start here](https://developers.cloudflare.com/fundamentals/get-started/).
type TenancyClient(httpClient: HttpClient) =
    ///<summary>
    ///List all accounts you have ownership or verified access to.
    ///</summary>
    member this.AccountsListAccounts
        (?name: string, ?page: float, ?perPage: float, ?direction: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) = OpenApiHttp.getAsync httpClient "/accounts" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountsListAccounts.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountsListAccounts.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsListAccounts" (int status)
        }

    ///<summary>
    ///Create an account (only available for tenant admins at this time)
    ///</summary>
    member this.AccountCreation(body: iam_create_u002D_account, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]
            let! (status, _, content) = OpenApiHttp.postAsync httpClient "/accounts" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountCreation.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountCreation.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountCreation" (int status)
        }

    ///<summary>
    ///Batch move a collection of accounts to a specific organization. ⚠️ Not implemented.
    ///</summary>
    member this.AccountsBatchMoveAccounts
        (body: AccountsBatchMoveAccountsPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]
            let! (status, _, content) = OpenApiHttp.postAsync httpClient "/accounts/move" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountsBatchMoveAccounts.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountsBatchMoveAccounts.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsBatchMoveAccounts" (int status)
        }

    ///<summary>
    ///Get the current transformation flows configuration for a zone.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="zoneId">Zone identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.GetFlowsGet(accountId: string, zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("accountId", accountId)
                  RequestPart.path ("zoneId", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{accountId}/zones/{zoneId}/v1/images/flows"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetFlowsGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetFlowsGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetFlowsGet" (int status)
        }

    ///<summary>
    ///Replace the entire transformation flows configuration for a zone.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="zoneId">Zone identifier.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.PutFlowsUpdate
        (accountId: string, zoneId: string, ?cancellationToken: CancellationToken, ?body: PutFlowsUpdatePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("accountId", accountId)
                  RequestPart.path ("zoneId", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{accountId}/zones/{zoneId}/v1/images/flows"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PutFlowsUpdate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PutFlowsUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PutFlowsUpdate" (int status)
        }

    ///<summary>
    ///Delete a specific account (only available for tenant admins at this time). This is a permanent operation that will delete any zones or other resources under the account
    ///</summary>
    member this.AccountDeletion(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/accounts/{account_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountDeletion.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountDeletion.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountDeletion" (int status)
        }

    ///<summary>
    ///Get information about a specific account that you are a member of.
    ///</summary>
    member this.AccountsAccountDetails(accountId: iam_account_identifier, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountsAccountDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountsAccountDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsAccountDetails" (int status)
        }

    ///<summary>
    ///Update an existing account.
    ///</summary>
    member this.AccountsUpdateAccount
        (
            accountId: iam_account_identifier,
            body: iam_components_u002D_schemas_u002D_account,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountsUpdateAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountsUpdateAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsUpdateAccount" (int status)
        }

    ///<summary>
    ///Returns billable usage data for the account.
    ///When no query parameters are provided, returns usage for the current
    ///billing period.
    ///</summary>
    ///<param name="accountId">Identifies the Cloudflare account.</param>
    ///<param name="from">Start date for the usage query (ISO 8601). The provided time range must include the subscription billing cycle anchor day, otherwise no usage data is returned. Use the info endpoint to retrieve the subscription anchor day.</param>
    ///<param name="to">End date for the usage query (ISO 8601).</param>
    ///<param name="cancellationToken"></param>
    member this.BillableUsageGetV1AccountUsage
        (accountId: string, ?from: string, ?``to``: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if from.IsSome then
                      RequestPart.query ("from", from.Value)
                  if ``to``.IsSome then
                      RequestPart.query ("to", ``to``.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/billable-usage" requestParts cancellationToken

            match (int status) with
            | 200 -> return BillableUsageGetV1AccountUsage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return BillableUsageGetV1AccountUsage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BillableUsageGetV1AccountUsage" (int status)
        }

    ///<summary>
    ///Returns high-level usage information for the account, including coverage,
    ///and subscription metadata.
    ///</summary>
    ///<param name="accountId">Identifies the Cloudflare account.</param>
    ///<param name="cancellationToken"></param>
    member this.BillableUsageGetV1AccountUsageInfo(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/billable-usage/info"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BillableUsageGetV1AccountUsageInfo.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return BillableUsageGetV1AccountUsageInfo.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BillableUsageGetV1AccountUsageInfo" (int status)
        }

    ///<summary>
    ///Returns cost and usage data for a single Cloudflare account, aligned
    ///with the [FinOps FOCUS v1.3](https://focus.finops.org/focus-specification/v1-3/)
    ///Cost and Usage dataset specification.
    ///Each record represents one billable metric for one account on one day.
    ///This includes all metered usage, including usage that falls within
    ///free-tier allowances and may result in zero cost.
    ///**Note:** Cost and pricing fields are not yet populated and
    ///will be absent from responses until billing integration is complete.
    ///When `from` and `to` are omitted, defaults to the start of the current
    ///month through today. The maximum date range is 31 days.
    ///</summary>
    ///<param name="accountId">Identifies the Cloudflare account.</param>
    ///<param name="from">Start date for the usage query (ISO 8601). Required if `to` is set. When omitted along with `to`, defaults to the start of the current month. Filters by charge period (when consumption happened), not billing period. The maximum date range is 31 days.</param>
    ///<param name="to">End date for the usage query (ISO 8601). Required if `from` is set. When omitted along with `from`, defaults to today. Filters by charge period (when consumption happened), not billing period. The maximum date range is 31 days.</param>
    ///<param name="cancellationToken"></param>
    member this.BillableUsageV2GetAccountUsage
        (accountId: string, ?from: string, ?``to``: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if from.IsSome then
                      RequestPart.query ("from", from.Value)
                  if ``to``.IsSome then
                      RequestPart.query ("to", ``to``.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/billable/usage" requestParts cancellationToken

            match (int status) with
            | 200 -> return BillableUsageV2GetAccountUsage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return BillableUsageV2GetAccountUsage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BillableUsageV2GetAccountUsage" (int status)
        }

    ///<summary>
    ///Returns cost and usage data for a single Cloudflare account, aligned
    ///with the [FinOps FOCUS v1.3](https://focus.finops.org/focus-specification/v1-3/)
    ///Cost and Usage dataset specification.
    ///This is the filterable counterpart to `GET` on the same path. It is a
    ///read-only operation and requires only the `#billing:read` permission;
    ///`POST` is used so that filter criteria can be supplied in a request body
    ///rather than in the query string.
    ///Each record represents one billable metric for one account on one day.
    ///This includes all metered usage, including usage that falls within
    ///free-tier allowances and may result in zero cost.
    ///**Note:** Cost and pricing fields are not yet populated and
    ///will be absent from responses until billing integration is complete.
    ///The request body is optional. When it is omitted, or when `TimePeriod`
    ///is omitted, the range defaults to the start of the current month through
    ///today. The maximum date range is 31 days.
    ///Filters of different kinds are combined with AND. Values within one tag
    ///filter are combined with OR. Filter values that do not match usage
    ///produce an empty result set.
    ///Results can be grouped by up to two customer resource-tag keys. Grouped
    ///values are returned in the `Tags` field. Usage without a requested tag
    ///remains in an untagged group, with that key omitted from `Tags`.
    ///Requests using tag filtering or grouping return HTTP 400 when tag-aware
    ///usage data is unavailable.
    ///</summary>
    ///<param name="accountId">Identifies the Cloudflare account.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body">Filter criteria for a usage query. Every field is optional; an empty object is equivalent to omitting the body entirely. Unknown fields are rejected.</param>
    member this.BillableUsageV2QueryAccountUsage
        (accountId: string, ?cancellationToken: CancellationToken, ?body: billable_u002D_usage_u002D_api_v2_usage_query)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/billable/usage" requestParts cancellationToken

            match (int status) with
            | 200 -> return BillableUsageV2QueryAccountUsage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return BillableUsageV2QueryAccountUsage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BillableUsageV2QueryAccountUsage" (int status)
        }

    ///<summary>
    ///Gets bad debt information for an account, including outstanding invoices and total debt amount.
    ///</summary>
    member this.AccountBillingGetBadDebt(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/billing/bad-debt" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingGetBadDebt.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingGetBadDebt.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingGetBadDebt" (int status)
        }

    ///<summary>
    ///Gets the credit balance and eligibility for an account.
    ///</summary>
    member this.AccountBillingGetCredits(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/billing/credits" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingGetCredits.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingGetCredits.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingGetCredits" (int status)
        }

    ///<summary>
    ///Gets the billing history for an account.
    ///</summary>
    member this.AccountBillingHistoryGetBillingHistory
        (accountId: string, ?page: int, ?perPage: int, ?status: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/billing/history" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingHistoryGetBillingHistory.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingHistoryGetBillingHistory.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingHistoryGetBillingHistory" (int status)
        }

    ///<summary>
    ///Deletes the billing profile for an account.
    ///</summary>
    member this.AccountBillingProfileDeleteBillingProfile(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/billing/profile"
                    requestParts
                    cancellationToken

            match (int status) with
            | 204 -> return AccountBillingProfileDeleteBillingProfile.NoContent
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingProfileDeleteBillingProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountBillingProfileDeleteBillingProfile" (int status)
        }

    ///<summary>
    ///Gets the current billing profile for the account.
    ///</summary>
    member this.AccountBillingProfileGetBillingProfile(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/billing/profile" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingProfileGetBillingProfile.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingProfileGetBillingProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingProfileGetBillingProfile" (int status)
        }

    ///<summary>
    ///Updates the billing email addresses and preferred locale for an account.
    ///</summary>
    member this.AccountBillingProfileUpdateBillingEmail
        (accountId: string, body: AccountBillingProfileUpdateBillingEmailPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/billing/profile"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountBillingProfileUpdateBillingEmail.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingProfileUpdateBillingEmail.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingProfileUpdateBillingEmail" (int status)
        }

    ///<summary>
    ///Creates a billing profile for an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body">Request body for creating or updating a billing profile.</param>
    ///<param name="cancellationToken"></param>
    member this.AccountBillingProfileCreateBillingProfile
        (
            accountId: string,
            body: bill_u002D_subs_u002D_api_billing_profile_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/billing/profile" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingProfileCreateBillingProfile.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingProfileCreateBillingProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountBillingProfileCreateBillingProfile" (int status)
        }

    ///<summary>
    ///Updates the billing profile for an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body">Request body for creating or updating a billing profile.</param>
    ///<param name="cancellationToken"></param>
    member this.AccountBillingProfileUpdateBillingProfile
        (
            accountId: string,
            body: bill_u002D_subs_u002D_api_billing_profile_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/billing/profile" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingProfileUpdateBillingProfile.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingProfileUpdateBillingProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountBillingProfileUpdateBillingProfile" (int status)
        }

    ///<summary>
    ///Creates a Stripe payment intent for adding or updating a payment method on the account's billing profile. Returns a client secret for frontend payment method collection.
    ///</summary>
    member this.AccountBillingCreatePaymentIntent(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/billing/profile/payment-method"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountBillingCreatePaymentIntent.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingCreatePaymentIntent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingCreatePaymentIntent" (int status)
        }

    ///<summary>
    ///Gets unpaid invoice information for an account.
    ///</summary>
    member this.AccountBillingGetUnpaidInvoices(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/billing/unpaid-invoice"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountBillingGetUnpaidInvoices.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingGetUnpaidInvoices.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingGetUnpaidInvoices" (int status)
        }

    ///<summary>
    ///Retrieve billing usage analytics for an account. Returns time-series data for all billable product metrics including Stream, Media (Images), Rate Limiting, Load Balancing, Argo, Workers, Workers KV, Image Resizing, and Spectrum.
    ///</summary>
    ///<param name="accountId">Standard Cloudflare hex account identifier. The API gateway translates this to an internal numeric ID before forwarding to the backend service.</param>
    ///<param name="metrics">Comma-separated list of metrics to include in the response. Available metrics depend on the endpoint. Billing usage supports: streamMinutesViewed, rateLimitingRequestsAllowed, loadBalancingQueries, argoAcceleratedBytes, workersRequests, workersKVReads, imageResizingRequests, spectrumBytesTransferred, mediaUniqueTransformations. Stream/media usage supports: streamMinutesViewed.</param>
    ///<param name="since">Start of the time range for the query (inclusive). ISO 8601 timestamp. Defaults to 6 hours before the current time.</param>
    ///<param name="until">End of the time range for the query (inclusive). ISO 8601 timestamp. Defaults to the current time.</param>
    ///<param name="timeDelta">Time unit to aggregate usage observations into. Data retention is approximately 18 months. The effective number of data points returned depends on the time range and granularity selected. For example, requesting hourly granularity over 18 months could produce up to ~13,000 data points; use the limit parameter to cap results and be aware that responses may be truncated.</param>
    ///<param name="limit">Maximum number of data points to return. The actual number of results depends on the interaction between the time range (since/until) and time_delta granularity. Results are truncated to this limit without error if the time range produces more data points than the limit allows.</param>
    ///<param name="filters">Filter expressions to apply to the query. Format: field==value. Multiple filters can be combined.</param>
    ///<param name="cancellationToken"></param>
    member this.UsageAnalyticsGetAccountBillingUsage
        (
            accountId: string,
            ?metrics: string,
            ?since: System.DateTimeOffset,
            ?until: System.DateTimeOffset,
            ?timeDelta: string,
            ?limit: int,
            ?filters: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if metrics.IsSome then
                      RequestPart.query ("metrics", metrics.Value)
                  if since.IsSome then
                      RequestPart.query ("since", since.Value)
                  if until.IsSome then
                      RequestPart.query ("until", until.Value)
                  if timeDelta.IsSome then
                      RequestPart.query ("time_delta", timeDelta.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if filters.IsSome then
                      RequestPart.query ("filters", filters.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/billing/usage" requestParts cancellationToken

            match (int status) with
            | 200 -> return UsageAnalyticsGetAccountBillingUsage.OK((Serializer.deserialize content))
            | 400 -> return UsageAnalyticsGetAccountBillingUsage.BadRequest((Serializer.deserialize content))
            | 401 -> return UsageAnalyticsGetAccountBillingUsage.Unauthorized((Serializer.deserialize content))
            | 403 -> return UsageAnalyticsGetAccountBillingUsage.Forbidden((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UsageAnalyticsGetAccountBillingUsage" (int status)
        }

    ///<summary>
    ///Creates multiple subscriptions for an account in a single request.
    ///</summary>
    member this.AccountSubscriptionsBulkCreateSubscription
        (
            accountId: string,
            body: AccountSubscriptionsBulkCreateSubscriptionPayload,
            ?idempKey: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if idempKey.IsSome then
                      RequestPart.query ("idemp_key", idempKey.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/bulk/subscriptions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSubscriptionsBulkCreateSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountSubscriptionsBulkCreateSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountSubscriptionsBulkCreateSubscription" (int status)
        }

    ///<summary>
    ///Creates a Stripe setup intent for adding a payment method to an account. Returns a client secret for frontend payment method collection.
    ///</summary>
    member this.AccountBillingCreateSetupIntent(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/client-secret" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingCreateSetupIntent.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingCreateSetupIntent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingCreateSetupIntent" (int status)
        }

    ///<summary>
    ///Returns the list of entitlements (features and their allocations) for a given account. Each entitlement describes a product feature the account is permitted to use and the allocation value (boolean, count, range, enum, or string) that governs its behaviour.
    ///</summary>
    ///<param name="accountId">Identifier of the account.</param>
    ///<param name="cancellationToken"></param>
    member this.EntitlementsGetAccountEntitlements(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/entitlements" requestParts cancellationToken

            match (int status) with
            | 200 -> return EntitlementsGetAccountEntitlements.OK((Serializer.deserialize content))
            | 204 -> return EntitlementsGetAccountEntitlements.NoContent
            | 400 -> return EntitlementsGetAccountEntitlements.BadRequest((Serializer.deserialize content))
            | 401 -> return EntitlementsGetAccountEntitlements.Unauthorized((Serializer.deserialize content))
            | 403 -> return EntitlementsGetAccountEntitlements.Forbidden((Serializer.deserialize content))
            | 404 -> return EntitlementsGetAccountEntitlements.NotFound((Serializer.deserialize content))
            | 405 -> return EntitlementsGetAccountEntitlements.MethodNotAllowed((Serializer.deserialize content))
            | 500 -> return EntitlementsGetAccountEntitlements.InternalServerError((Serializer.deserialize content))
            | 503 -> return EntitlementsGetAccountEntitlements.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EntitlementsGetAccountEntitlements" (int status)
        }

    ///<summary>
    ///List all the permissions groups for an account.
    ///</summary>
    member this.AccountPermissionGroupList
        (
            accountId: iam_account_identifier,
            ?id: string,
            ?name: string,
            ?label: string,
            ?page: float,
            ?perPage: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if id.IsSome then
                      RequestPart.query ("id", id.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if label.IsSome then
                      RequestPart.query ("label", label.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/iam/permission_groups"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountPermissionGroupList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountPermissionGroupList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountPermissionGroupList" (int status)
        }

    ///<summary>
    ///Get information about a specific permission group in an account.
    ///</summary>
    member this.AccountPermissionGroupDetails
        (
            accountId: iam_account_identifier,
            permissionGroupId: iam_permission_group_identifier,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("permission_group_id", permissionGroupId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/iam/permission_groups/{permission_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountPermissionGroupDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountPermissionGroupDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountPermissionGroupDetails" (int status)
        }

    ///<summary>
    ///List all the resource groups for an account.
    ///</summary>
    member this.AccountResourceGroupList
        (accountId: iam_account_identifier, ?id: string, ?name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if id.IsSome then
                      RequestPart.query ("id", id.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/iam/resource_groups"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountResourceGroupList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountResourceGroupList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountResourceGroupList" (int status)
        }

    ///<summary>
    ///Create a new Resource Group under the specified account.
    ///</summary>
    member this.AccountResourceGroupCreate
        (
            accountId: iam_account_identifier,
            body: iam_request_create_resource_group,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/iam/resource_groups"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountResourceGroupCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountResourceGroupCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountResourceGroupCreate" (int status)
        }

    ///<summary>
    ///Remove a resource group from an account.
    ///</summary>
    member this.AccountResourceGroupDelete
        (
            accountId: iam_account_identifier,
            resourceGroupId: iam_resource_group_identifier,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("resource_group_id", resourceGroupId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/iam/resource_groups/{resource_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountResourceGroupDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountResourceGroupDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountResourceGroupDelete" (int status)
        }

    ///<summary>
    ///Get information about a specific resource group in an account.
    ///</summary>
    member this.AccountResourceGroupDetails
        (
            accountId: iam_account_identifier,
            resourceGroupId: iam_resource_group_identifier,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("resource_group_id", resourceGroupId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/iam/resource_groups/{resource_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountResourceGroupDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountResourceGroupDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountResourceGroupDetails" (int status)
        }

    ///<summary>
    ///Modify an existing resource group.
    ///</summary>
    member this.AccountResourceGroupUpdate
        (
            accountId: iam_account_identifier,
            resourceGroupId: iam_resource_group_identifier,
            body: iam_request_update_resource_group,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("resource_group_id", resourceGroupId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/iam/resource_groups/{resource_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountResourceGroupUpdate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountResourceGroupUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountResourceGroupUpdate" (int status)
        }

    ///<summary>
    ///List all the user groups for an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="id">ID of the user group to be fetched.</param>
    ///<param name="name"></param>
    ///<param name="fuzzyName"></param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="direction"></param>
    ///<param name="cancellationToken"></param>
    member this.AccountUserGroupList
        (
            accountId: iam_account_identifier,
            ?id: iam_user_group_identifier,
            ?name: string,
            ?fuzzyName: string,
            ?page: float,
            ?perPage: float,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if id.IsSome then
                      RequestPart.query ("id", id.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if fuzzyName.IsSome then
                      RequestPart.query ("fuzzyName", fuzzyName.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/iam/user_groups" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountUserGroupList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountUserGroupList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountUserGroupList" (int status)
        }

    ///<summary>
    ///Create a new user group under the specified account.
    ///</summary>
    member this.AccountUserGroupCreate
        (accountId: iam_account_identifier, body: iam_create_user_group_body, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/iam/user_groups" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountUserGroupCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountUserGroupCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountUserGroupCreate" (int status)
        }

    ///<summary>
    ///Remove a user group from an account.
    ///</summary>
    member this.AccountUserGroupDelete
        (
            accountId: iam_account_identifier,
            userGroupId: iam_user_group_identifier,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("user_group_id", userGroupId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/iam/user_groups/{user_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountUserGroupDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountUserGroupDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountUserGroupDelete" (int status)
        }

    ///<summary>
    ///Get information about a specific user group in an account.
    ///</summary>
    member this.AccountUserGroupDetails
        (
            accountId: iam_account_identifier,
            userGroupId: iam_user_group_identifier,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("user_group_id", userGroupId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/iam/user_groups/{user_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountUserGroupDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountUserGroupDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountUserGroupDetails" (int status)
        }

    ///<summary>
    ///Modify an existing user group.
    ///</summary>
    member this.AccountUserGroupUpdate
        (
            accountId: iam_account_identifier,
            userGroupId: iam_user_group_identifier,
            body: iam_update_user_group_body,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("user_group_id", userGroupId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/iam/user_groups/{user_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountUserGroupUpdate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountUserGroupUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountUserGroupUpdate" (int status)
        }

    ///<summary>
    ///List all the members attached to a user group.
    ///</summary>
    member this.AccountUserGroupMemberList
        (
            accountId: iam_account_identifier,
            userGroupId: iam_user_group_identifier,
            ?page: float,
            ?perPage: float,
            ?fuzzyEmail: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("user_group_id", userGroupId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if fuzzyEmail.IsSome then
                      RequestPart.query ("fuzzyEmail", fuzzyEmail.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/iam/user_groups/{user_group_id}/members"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountUserGroupMemberList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountUserGroupMemberList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountUserGroupMemberList" (int status)
        }

    ///<summary>
    ///Add members to a User Group.
    ///</summary>
    member this.AccountUserGroupMemberCreate
        (
            accountId: iam_account_identifier,
            userGroupId: iam_user_group_identifier,
            body: AccountUserGroupMemberCreatePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("user_group_id", userGroupId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/iam/user_groups/{user_group_id}/members"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountUserGroupMemberCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountUserGroupMemberCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountUserGroupMemberCreate" (int status)
        }

    ///<summary>
    ///Replace the set of members attached to a User Group.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="userGroupId"></param>
    ///<param name="body">Set/Replace members to a user group.</param>
    ///<param name="cancellationToken"></param>
    member this.AccountUserGroupMembersUpdate
        (
            accountId: iam_account_identifier,
            userGroupId: iam_user_group_identifier,
            body: AccountUserGroupMembersUpdatePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("user_group_id", userGroupId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/iam/user_groups/{user_group_id}/members"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountUserGroupMembersUpdate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountUserGroupMembersUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountUserGroupMembersUpdate" (int status)
        }

    ///<summary>
    ///Remove a member from User Group
    ///</summary>
    member this.AccountUserGroupMemberDelete
        (
            accountId: iam_account_identifier,
            userGroupId: iam_user_group_identifier,
            memberId: iam_user_group_member_identifier,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("user_group_id", userGroupId)
                  RequestPart.path ("member_id", memberId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/iam/user_groups/{user_group_id}/members/{member_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountUserGroupMemberDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountUserGroupMemberDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountUserGroupMemberDelete" (int status)
        }

    ///<summary>
    ///Get details of a specific member in a user group.
    ///</summary>
    member this.AccountUserGroupMemberGet
        (
            accountId: iam_account_identifier,
            userGroupId: iam_user_group_identifier,
            memberId: iam_user_group_member_identifier,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("user_group_id", userGroupId)
                  RequestPart.path ("member_id", memberId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/iam/user_groups/{user_group_id}/members/{member_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountUserGroupMemberGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountUserGroupMemberGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountUserGroupMemberGet" (int status)
        }

    ///<summary>
    ///Toggles PDF invoice generation for an account.
    ///</summary>
    member this.AccountBillingTogglePdfInvoices
        (accountId: string, body: AccountBillingTogglePdfInvoicesPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/accounts/{account_id}/invoices" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingTogglePdfInvoices.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingTogglePdfInvoices.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingTogglePdfInvoices" (int status)
        }

    ///<summary>
    ///List all members of an account.
    ///</summary>
    member this.AccountMembersListMembers
        (
            accountId: iam_account_identifier,
            ?order: string,
            ?status: string,
            ?page: float,
            ?perPage: float,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/members" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountMembersListMembers.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountMembersListMembers.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountMembersListMembers" (int status)
        }

    ///<summary>
    ///Add a user to the list of members for this account.
    ///</summary>
    member this.AccountMembersAddMember
        (
            accountId: iam_account_identifier,
            body: InlineUnion_be10f3bc1b712c5b9809a775,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/members" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountMembersAddMember.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountMembersAddMember.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountMembersAddMember" (int status)
        }

    ///<summary>
    ///Remove a member from an account.
    ///</summary>
    member this.AccountMembersRemoveMember
        (
            memberId: string,
            accountId: iam_account_identifier,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("member_id", memberId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/members/{member_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountMembersRemoveMember.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountMembersRemoveMember.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountMembersRemoveMember" (int status)
        }

    ///<summary>
    ///Get information about a specific member of an account.
    ///</summary>
    member this.AccountMembersMemberDetails
        (memberId: string, accountId: iam_account_identifier, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("member_id", memberId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/members/{member_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountMembersMemberDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountMembersMemberDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountMembersMemberDetails" (int status)
        }

    ///<summary>
    ///Modify an account member.
    ///</summary>
    member this.AccountMembersUpdateMember
        (
            memberId: string,
            accountId: iam_account_identifier,
            body: InlineUnion_2df0627403a42737936c0185,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("member_id", memberId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/members/{member_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountMembersUpdateMember.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountMembersUpdateMember.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountMembersUpdateMember" (int status)
        }

    ///<summary>
    ///Move an account within an organization hierarchy or an account outside an organization. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    member this.AccountsMoveAccounts
        (accountId: string, body: AccountsMoveAccountsPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/move" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountsMoveAccounts.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountsMoveAccounts.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsMoveAccounts" (int status)
        }

    ///<summary>
    ///Retrieve a list of the organizations that "contain" this account or are
    ///managing it.
    ///The returned list will be in order from "root" to "leaf", where the "leaf"
    ///will be the organization that _immediately_ contains the specified
    ///account.
    ///</summary>
    member this.AccountsListAccountOrganizations(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/organizations" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountsListAccountOrganizations.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountsListAccountOrganizations.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsListAccountOrganizations" (int status)
        }

    ///<summary>
    ///Pays outstanding bad debt for an account. Discovers all debt automatically and handles invoice deduplication.
    ///</summary>
    member this.AccountBillingPayBadDebt
        (accountId: string, body: AccountBillingPayBadDebtPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/pay-bad-debt" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingPayBadDebt.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingPayBadDebt.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingPayBadDebt" (int status)
        }

    ///<summary>
    ///Pays an outstanding invoice for an account. Returns a Stripe client secret when Strong Customer Authentication (SCA) is required to complete the payment.
    ///</summary>
    member this.AccountBillingPayInvoice
        (accountId: string, body: AccountBillingPayInvoicePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/pay-invoice" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingPayInvoice.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingPayInvoice.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingPayInvoice" (int status)
        }

    ///<summary>
    ///Lists all payment methods for an account.
    ///</summary>
    member this.AccountBillingListPaymentMethods
        (accountId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/payment-methods" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingListPaymentMethods.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingListPaymentMethods.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingListPaymentMethods" (int status)
        }

    ///<summary>
    ///Creates a new payment method for an account.
    ///</summary>
    member this.AccountBillingCreatePaymentMethod
        (accountId: string, body: bill_u002D_subs_u002D_api_payment_method, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/payment-methods" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountBillingCreatePaymentMethod.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingCreatePaymentMethod.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingCreatePaymentMethod" (int status)
        }

    ///<summary>
    ///Deletes a payment method from an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="paymentMethodId">Payment method identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.AccountBillingDeletePaymentMethod
        (accountId: string, paymentMethodId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("payment_method_id", paymentMethodId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/payment-methods/{payment_method_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountBillingDeletePaymentMethod.OK((Serializer.deserialize content))
            | 204 -> return AccountBillingDeletePaymentMethod.NoContent
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingDeletePaymentMethod.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingDeletePaymentMethod" (int status)
        }

    ///<summary>
    ///Gets a specific payment method for an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="paymentMethodId">Payment method identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.AccountBillingGetPaymentMethod
        (accountId: string, paymentMethodId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("payment_method_id", paymentMethodId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/payment-methods/{payment_method_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountBillingGetPaymentMethod.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingGetPaymentMethod.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingGetPaymentMethod" (int status)
        }

    ///<summary>
    ///Updates a payment method for an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="paymentMethodId">Payment method identifier.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AccountBillingUpdatePaymentMethod
        (
            accountId: string,
            paymentMethodId: string,
            body: bill_u002D_subs_u002D_api_payment_method,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("payment_method_id", paymentMethodId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/payment-methods/{payment_method_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountBillingUpdatePaymentMethod.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingUpdatePaymentMethod.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingUpdatePaymentMethod" (int status)
        }

    ///<summary>
    ///Sets a payment method as the default for an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="paymentMethodId">Payment method identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.AccountBillingSetDefaultPaymentMethod
        (accountId: string, paymentMethodId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("payment_method_id", paymentMethodId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/payment-methods/{payment_method_id}/set-as-default"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountBillingSetDefaultPaymentMethod.OK((Serializer.deserialize content))
            | 204 -> return AccountBillingSetDefaultPaymentMethod.NoContent
            | _ when (((int status) / 100) = 4) ->
                return AccountBillingSetDefaultPaymentMethod.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingSetDefaultPaymentMethod" (int status)
        }

    ///<summary>
    ///Retrieves the profile information for a specific Cloudflare account, including organization details, settings, and metadata. This endpoint is commonly used to verify account access and retrieve account-level configuration.
    ///</summary>
    member this.AccountsGetAccountProfile(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/profile" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountsGetAccountProfile.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountsGetAccountProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsGetAccountProfile" (int status)
        }

    ///<summary>
    ///Updates the profile information for a Cloudflare account. Allows modification of account-level settings and organizational details. Requires Account Settings Write permission.
    ///</summary>
    member this.AccountsModifyAccountProfile
        (accountId: string, body: organizations_u002D_api_Profile, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/profile" requestParts cancellationToken

            match (int status) with
            | 204 -> return AccountsModifyAccountProfile.NoContent
            | _ when (((int status) / 100) = 4) ->
                return AccountsModifyAccountProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsModifyAccountProfile" (int status)
        }

    ///<summary>
    ///Downloads a receipt as a PDF document.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="receiptId">Receipt identifier.</param>
    ///<param name="doctype"></param>
    ///<param name="cancellationToken"></param>
    member this.AccountBillingGetReceiptPdf
        (accountId: string, receiptId: string, ?doctype: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("receipt_id", receiptId)
                  if doctype.IsSome then
                      RequestPart.query ("doctype", doctype.Value) ]

            let! (status, _, contentBinary) =
                OpenApiHttp.getBinaryAsync
                    httpClient
                    "/accounts/{account_id}/receipts/{receipt_id}/pdf"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountBillingGetReceiptPdf.OK(contentBinary)
            | _ when (((int status) / 100) = 4) ->
                let content = Encoding.UTF8.GetString contentBinary
                return AccountBillingGetReceiptPdf.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountBillingGetReceiptPdf" (int status)
        }

    ///<summary>
    ///Performs real-time, authoritative availability checks directly against domain
    ///registries. Use this endpoint to verify a domain is available before attempting
    ///registration via `POST /registrations`.
    ///**Important:** Unlike the Search endpoint, these results are authoritative and
    ///reflect current registry status. Always check availability immediately before
    ///registration as domain status can change rapidly.
    ///**Note:** This endpoint uses POST to accept a list of domains in the request
    ///body. It is a read-only operation — it does not create, modify, or reserve
    ///any domains.
    ///### Extension support
    ///Only domains on extensions supported for programmatic registration by this API
    ///can be registered. If you check a domain on an unsupported extension, the response
    ///will include `registrable: false` with a `reason` field explaining why:
    ///- `extension_not_supported_via_api` — Cloudflare Registrar supports this extension
    ///  in the dashboard, but it is not yet available for programmatic registration via
    ///  this API. Register via `https://dash.cloudflare.com/{account_id}/domains/registrations` instead.
    ///- `extension_not_supported` — This extension is not supported by Cloudflare
    ///  Registrar.
    ///- `extension_disallows_registration` — The extension's registry has temporarily
    ///  or permanently frozen new registrations. No registrar can register domains on
    ///  this extension at this time.
    ///- `domain_premium` — The domain is premium priced. Premium registration is not
    ///  currently supported by this API.
    ///- `domain_unavailable` — The domain is already registered, reserved, or otherwise
    ///  not available for registration on a supported extension.
    ///The `reason` field is only present when `registrable` is `false`.
    ///### Behavior
    ///- Maximum 20 domains per request
    ///- Pricing is only returned for domains where `registrable: true`
    ///- Results are not cached; each request queries the registry
    ///### Workflow
    ///1. Call this endpoint with domains the user wants to register.
    ///2. For each domain where `registrable: true`, present pricing to the user.
    ///3. If `tier: premium`, note that premium registration is not currently
    ///   supported by this API and do not proceed to `POST /registrations`.
    ///4. Proceed to `POST /registrations` only for supported non-premium domains.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID. Required for all Registrar API operations.</param>
    ///<param name="body">Request body for checking domain availability.</param>
    ///<param name="cancellationToken"></param>
    member this.SandboxRegistrarDomainDiscoveryCheck
        (
            accountId: string,
            body: registrar_u002D_api_u002D_sandbox_domain_check_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/registrar-sandbox/domain-check"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SandboxRegistrarDomainDiscoveryCheck.OK((Serializer.deserialize content))
            | 400 -> return SandboxRegistrarDomainDiscoveryCheck.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SandboxRegistrarDomainDiscoveryCheck" (int status)
        }

    ///<summary>
    ///Searches for domain name suggestions based on a keyword, phrase, or partial domain name.
    ///Returns a list of potentially available domains with pricing information.
    ///**Important:** Results are non-authoritative and based on cached data. Always use the
    ///`/domain-check` endpoint to verify real-time availability before attempting registration.
    ///Suggestions are scoped to extensions supported for programmatic registration
    ///via this API (`POST /registrations`). Domains on unsupported extensions will
    ///not appear in results, even if they are available at the registry level.
    ///### Use cases
    ///- Brand name discovery (e.g., "acme corp" → acmecorp.com, acmecorp.dev)
    ///- Keyword-based suggestions (e.g., "coffee shop" → coffeeshop.com, mycoffeeshop.net)
    ///- Alternative extension discovery (e.g., "example.com" → example.com, example.app, example.xyz)
    ///### Workflow
    ///1. Call this endpoint with a keyword or domain name.
    ///2. Present suggestions to the user.
    ///3. Call `/domain-check` with the user's chosen domains to confirm real-time availability and pricing.
    ///4. Proceed to `POST /registrations` only for supported non-premium domains
    ///   where the Check response returns `registrable: true`.
    ///**Note:** Searching with just a domain extension (e.g., "com" or ".app") is not supported. Provide a keyword or domain name.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID. Required for all Registrar API operations.</param>
    ///<param name="q">
    ///The search term to find domain suggestions. Accepts keywords, phrases, or full domain names.
    ///- Phrases: "coffee shop" returns coffeeshop.com, mycoffeeshop.net, etc.
    ///- Domain names: "example.com" returns example.com and variations across extensions
    ///</param>
    ///<param name="extensions">
    ///Limits results to specific domain extensions from the supported set. If not specified,
    ///returns results across all supported extensions. Extensions not in the supported
    ///set are silently ignored.
    ///</param>
    ///<param name="limit">Maximum number of domain suggestions to return. Defaults to 20 if not specified.</param>
    ///<param name="cancellationToken"></param>
    member this.SandboxRegistrarDomainDiscoverySearch
        (accountId: string, q: string, ?extensions: list<string>, ?limit: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.query ("q", q)
                  if extensions.IsSome then
                      RequestPart.queryComma ("extensions", extensions.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar-sandbox/domain-search"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SandboxRegistrarDomainDiscoverySearch.OK((Serializer.deserialize content))
            | 400 -> return SandboxRegistrarDomainDiscoverySearch.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SandboxRegistrarDomainDiscoverySearch" (int status)
        }

    ///<summary>
    ///Returns metadata and JSON Schema documents describing the expected input
    ///structure for registration operations on each supported
    ///extension (TLD).
    ///This endpoint uses cursor-based pagination. Results are ordered by
    ///extension name by default. To fetch the next page, pass the `cursor`
    ///value from the `result_info` object in the response as the `cursor`
    ///query parameter in your next request. An empty `cursor` string
    ///indicates there are no more pages.
    ///Supports HTTP conditional GET via `ETag`. Include the `ETag` value
    ///from a previous response in an `If-None-Match` header to receive a
    ///`304 Not Modified` when the data has not changed.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="name">
    ///Filter extensions by exact name match.
    ///For example, `name=com` returns only the `com` extension.
    ///</param>
    ///<param name="cursor">
    ///Opaque token from a previous response's `result_info.cursor`.
    ///Pass this value to fetch the next page of results. Omit (or
    ///pass an empty string) for the first page.
    ///</param>
    ///<param name="perPage">Number of items to return per page.</param>
    ///<param name="direction">Sort direction for results. Defaults to ascending order.</param>
    ///<param name="sortBy">Column to sort results by. Defaults to `name` when omitted.</param>
    ///<param name="cancellationToken"></param>
    member this.SandboxRegistrarExtensionList
        (
            accountId: string,
            ?name: string,
            ?cursor: string,
            ?perPage: int,
            ?direction: string,
            ?sortBy: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if sortBy.IsSome then
                      RequestPart.query ("sort_by", sortBy.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar-sandbox/extensions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SandboxRegistrarExtensionList.OK((Serializer.deserialize content))
            | 304 -> return SandboxRegistrarExtensionList.NotModified
            | _ when (((int status) / 100) = 4) ->
                return SandboxRegistrarExtensionList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SandboxRegistrarExtensionList" (int status)
        }

    ///<summary>
    ///Returns metadata and JSON Schema documents describing the expected input
    ///structure for registration operations on a specific
    ///extension (TLD).
    ///Supports HTTP conditional GET via `ETag`. Include the `ETag` value
    ///from a previous response in an `If-None-Match` header to receive a
    ///`304 Not Modified` when the data has not changed.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="extension">The extension name (e.g., `com`, `co.uk`).</param>
    ///<param name="cancellationToken"></param>
    member this.SandboxRegistrarExtensionGet
        (accountId: string, extension: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("extension", extension) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar-sandbox/extensions/{extension}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SandboxRegistrarExtensionGet.OK((Serializer.deserialize content))
            | 304 -> return SandboxRegistrarExtensionGet.NotModified
            | _ when (((int status) / 100) = 4) ->
                return SandboxRegistrarExtensionGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SandboxRegistrarExtensionGet" (int status)
        }

    ///<summary>
    ///Returns a paginated list of domain registrations owned by the account.
    ///This endpoint uses cursor-based pagination. Results are ordered by registration
    ///date by default. To fetch the next page, pass the `cursor` value from the
    ///`result_info` object in the response as the `cursor` query parameter in
    ///your next request. An empty `cursor` string indicates there are no more
    ///pages.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cursor">
    ///Opaque token from a previous response's `result_info.cursor`.
    ///Pass this value to fetch the next page of results. Omit (or
    ///pass an empty string) for the first page.
    ///</param>
    ///<param name="perPage">Number of items to return per page.</param>
    ///<param name="direction">Sort direction for results. Defaults to ascending order.</param>
    ///<param name="sortBy">Column to sort results by. Defaults to registration date (`registry_created_at`) when omitted.</param>
    ///<param name="cancellationToken"></param>
    member this.SandboxRegistrarDomainRegistrationList
        (
            accountId: string,
            ?cursor: string,
            ?perPage: int,
            ?direction: string,
            ?sortBy: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if sortBy.IsSome then
                      RequestPart.query ("sort_by", sortBy.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar-sandbox/registrations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SandboxRegistrarDomainRegistrationList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SandboxRegistrarDomainRegistrationList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SandboxRegistrarDomainRegistrationList" (int status)
        }

    ///<summary>
    ///Starts a domain registration workflow.
    ///### Prerequisites
    ///- The account must not already be at the maximum supported domain limit.
    ///    A single account may own up to 500 domains in total across registrations
    ///    created through either the dashboard or this API.
    ///- The domain must be on a supported extension for programmatic registration.
    ///- Use `POST /domain-check` immediately before calling this endpoint to confirm
    ///    real-time availability and pricing.
    ///### Defaults
    ///- `years`: defaults to the extension's minimum registration period (1 year for
    ///    most extensions, but varies — for example, `.ai` (if supported) requires a minimum of 2 years).
    ///- `auto_renew`: defaults to `false`. Setting it to `true` is an explicit
    ///    opt-in authorizing Cloudflare to charge the account's default payment
    ///    method up to 30 days before domain expiry to renew the registration.
    ///    Renewal pricing may change over time based on registry pricing.
    ///- `privacy_mode`: defaults to `redaction`.
    ///### Premium domains
    ///Premium domain registration is not currently supported by this API.
    ///If `POST /domain-check` returns `tier: premium`, do not call this
    ///endpoint for that domain.
    ///### Response behavior
    ///By default, the server holds the connection for a bounded, server-defined
    ///amount of time while the registration completes. Most registrations finish
    ///within this window and return `201 Created` with a completed workflow status.
    ///If the registration is still processing after this synchronous wait window,
    ///the server returns `202 Accepted`. Poll the URL in `links.self` to track progress.
    ///To skip the wait and receive an immediate `202`, send `Prefer: respond-async`.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID. Required for all Registrar API operations.</param>
    ///<param name="body"></param>
    ///<param name="prefer">
    ///Set to `respond-async` to receive an immediate `202 Accepted` without
    ///waiting for the operation to complete (RFC 7240).
    ///The header may be combined with other preferences using standard
    ///comma-separated syntax.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.SandboxRegistrarDomainRegistrationCreate
        (
            accountId: string,
            body: registrar_u002D_api_u002D_sandbox_registration_create_request,
            ?prefer: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if prefer.IsSome then
                      RequestPart.header ("Prefer", prefer.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/registrar-sandbox/registrations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return SandboxRegistrarDomainRegistrationCreate.Created((Serializer.deserialize content))
            | 202 -> return SandboxRegistrarDomainRegistrationCreate.Accepted((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SandboxRegistrarDomainRegistrationCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for SandboxRegistrarDomainRegistrationCreate" (int status)
        }

    ///<summary>
    ///Returns the current state of a domain registration.
    ///This is the canonical read endpoint for a domain you own. It returns
    ///the full registration resource including current settings and expiration.
    ///When the registration resource is ready, both `created_at` and `expires_at`
    ///are present in the response.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="domainName">Domain name to retrieve.</param>
    ///<param name="cancellationToken"></param>
    member this.SandboxRegistrarDomainRegistrationGet
        (accountId: string, domainName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain_name", domainName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar-sandbox/registrations/{domain_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SandboxRegistrarDomainRegistrationGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SandboxRegistrarDomainRegistrationGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SandboxRegistrarDomainRegistrationGet" (int status)
        }

    ///<summary>
    ///Updates an existing domain registration.
    ///By default, the server holds the connection for a bounded, server-defined
    ///amount of time while the update completes. Most updates finish within this
    ///window and return `200 OK` with a completed workflow status.
    ///If the update is still processing after this synchronous wait window, the
    ///server returns `202 Accepted`. Poll the URL in `links.self` to track progress.
    ///To skip the wait and receive an immediate `202`, send `Prefer: respond-async`.
    ///This endpoint currently supports updating `auto_renew` only.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="domainName">Domain name to update.</param>
    ///<param name="body">
    ///Request to update an existing domain registration.
    ///This endpoint currently supports updating `auto_renew` only.
    ///</param>
    ///<param name="prefer">
    ///Set to `respond-async` to receive an immediate `202 Accepted` without
    ///waiting for the operation to complete (RFC 7240).
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.SandboxRegistrarDomainRegistrationUpdate
        (
            accountId: string,
            domainName: string,
            body: registrar_u002D_api_u002D_sandbox_registration_update_request,
            ?prefer: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain_name", domainName)
                  RequestPart.jsonContent body
                  if prefer.IsSome then
                      RequestPart.header ("Prefer", prefer.Value) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/registrar-sandbox/registrations/{domain_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SandboxRegistrarDomainRegistrationUpdate.OK((Serializer.deserialize content))
            | 202 -> return SandboxRegistrarDomainRegistrationUpdate.Accepted((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SandboxRegistrarDomainRegistrationUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for SandboxRegistrarDomainRegistrationUpdate" (int status)
        }

    ///<summary>
    ///Returns the current status of a domain registration workflow.
    ///Use this endpoint to poll for completion when the POST response
    ///returned `202 Accepted`. The URL is provided in the `links.self`
    ///field of the workflow status response.
    ///Poll this endpoint until the workflow reaches a terminal state or a
    ///state that requires user attention.
    ///**Terminal states:** `succeeded` and `failed` are terminal and always
    ///have `completed: true`.
    ///**Non-terminal states:**
    ///- `action_required` has `completed: false` and will not resolve on its
    ///  own. The workflow is paused pending user intervention.
    ///- `blocked` has `completed: false` and indicates the workflow is waiting
    ///  on a third party such as the extension registry or losing registrar.
    ///  Continue polling while informing the user of the delay.
    ///Use increasing backoff between polls. When `state: blocked`, use a
    ///longer polling interval and do not poll indefinitely.
    ///A naive polling loop that only checks `completed` can run indefinitely
    ///when `state: action_required`. Break explicitly on `action_required`:
    ///```js
    ///let status;
    ///do {
    ///  await new Promise(r =&amp;gt; setTimeout(r, 2000));
    ///  status = await cloudflare.request({
    ///    method: 'GET',
    ///    path: reg.result.links.self,
    ///  });
    ///} while (
    ///  !status.result.completed &amp;&amp;
    ///  status.result.state !== 'action_required'
    ///);
    ///if (status.result.state === 'action_required') {
    ///  // Surface context.action and context.confirmation_sent_to to the user.
    ///  // Do not re-submit the registration request.
    ///}
    ///```
    ///</summary>
    ///<param name="accountId">Cloudflare account ID. Required for all Registrar API operations.</param>
    ///<param name="domainName"></param>
    ///<param name="cancellationToken"></param>
    member this.SandboxRegistrarDomainRegistrationGetStatus
        (accountId: string, domainName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain_name", domainName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar-sandbox/registrations/{domain_name}/registration-status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SandboxRegistrarDomainRegistrationGetStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SandboxRegistrarDomainRegistrationGetStatus.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for SandboxRegistrarDomainRegistrationGetStatus" (int status)
        }

    ///<summary>
    ///Returns the current status of a domain update workflow.
    ///Use this endpoint to poll for completion when the PATCH response
    ///returned `202 Accepted`. The URL is provided in the `links.self`
    ///field of the workflow status response.
    ///Poll this endpoint until the workflow reaches a terminal state or a
    ///state that requires user attention.
    ///Use increasing backoff between polls. When the workflow remains blocked
    ///on a third party, use a longer polling interval and do not poll indefinitely.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="domainName"></param>
    ///<param name="cancellationToken"></param>
    member this.SandboxRegistrarDomainRegistrationGetUpdateStatus
        (accountId: string, domainName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain_name", domainName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar-sandbox/registrations/{domain_name}/update-status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SandboxRegistrarDomainRegistrationGetUpdateStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SandboxRegistrarDomainRegistrationGetUpdateStatus.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SandboxRegistrarDomainRegistrationGetUpdateStatus"
                        (int status)
        }

    ///<summary>
    ///Performs real-time, authoritative availability checks directly against domain
    ///registries. Use this endpoint to verify a domain is available before attempting
    ///registration via `POST /registrations`.
    ///**Important:** Unlike the Search endpoint, these results are authoritative and
    ///reflect current registry status. Always check availability immediately before
    ///registration as domain status can change rapidly.
    ///**Note:** This endpoint uses POST to accept a list of domains in the request
    ///body. It is a read-only operation — it does not create, modify, or reserve
    ///any domains.
    ///### Extension support
    ///Only domains on extensions supported for programmatic registration by this API
    ///can be registered. If you check a domain on an unsupported extension, the response
    ///will include `registrable: false` with a `reason` field explaining why:
    ///- `extension_not_supported_via_api` — Cloudflare Registrar supports this extension
    ///  in the dashboard, but it is not yet available for programmatic registration via
    ///  this API. Register via `https://dash.cloudflare.com/{account_id}/domains/registrations` instead.
    ///- `extension_not_supported` — This extension is not supported by Cloudflare
    ///  Registrar.
    ///- `extension_disallows_registration` — The extension's registry has temporarily
    ///  or permanently frozen new registrations. No registrar can register domains on
    ///  this extension at this time.
    ///- `domain_premium` — The domain is premium priced. Premium registration is not
    ///  currently supported by this API.
    ///- `domain_unavailable` — The domain is already registered, reserved, or otherwise
    ///  not available for registration on a supported extension.
    ///The `reason` field is only present when `registrable` is `false`.
    ///### Behavior
    ///- Maximum 20 domains per request
    ///- Pricing is only returned for domains where `registrable: true`
    ///- Results are not cached; each request queries the registry
    ///### Workflow
    ///1. Call this endpoint with domains the user wants to register.
    ///2. For each domain where `registrable: true`, present pricing to the user.
    ///3. If `tier: premium`, note that premium registration is not currently
    ///   supported by this API and do not proceed to `POST /registrations`.
    ///4. Proceed to `POST /registrations` only for supported non-premium domains.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID. Required for all Registrar API operations.</param>
    ///<param name="body">Request body for checking domain availability.</param>
    ///<param name="cancellationToken"></param>
    member this.RegistrarDomainDiscoveryCheck
        (accountId: string, body: registrar_u002D_api_domain_check_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/registrar/domain-check"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarDomainDiscoveryCheck.OK((Serializer.deserialize content))
            | 400 -> return RegistrarDomainDiscoveryCheck.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarDomainDiscoveryCheck" (int status)
        }

    ///<summary>
    ///Searches for domain name suggestions based on a keyword, phrase, or partial domain name.
    ///Returns a list of potentially available domains with pricing information.
    ///**Important:** Results are non-authoritative and based on cached data. Always use the
    ///`/domain-check` endpoint to verify real-time availability before attempting registration.
    ///Suggestions are scoped to extensions supported for programmatic registration
    ///via this API (`POST /registrations`). Domains on unsupported extensions will
    ///not appear in results, even if they are available at the registry level.
    ///### Use cases
    ///- Brand name discovery (e.g., "acme corp" → acmecorp.com, acmecorp.dev)
    ///- Keyword-based suggestions (e.g., "coffee shop" → coffeeshop.com, mycoffeeshop.net)
    ///- Alternative extension discovery (e.g., "example.com" → example.com, example.app, example.xyz)
    ///### Workflow
    ///1. Call this endpoint with a keyword or domain name.
    ///2. Present suggestions to the user.
    ///3. Call `/domain-check` with the user's chosen domains to confirm real-time availability and pricing.
    ///4. Proceed to `POST /registrations` only for supported non-premium domains
    ///   where the Check response returns `registrable: true`.
    ///**Note:** Searching with just a domain extension (e.g., "com" or ".app") is not supported. Provide a keyword or domain name.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID. Required for all Registrar API operations.</param>
    ///<param name="q">
    ///The search term to find domain suggestions. Accepts keywords, phrases, or full domain names.
    ///- Phrases: "coffee shop" returns coffeeshop.com, mycoffeeshop.net, etc.
    ///- Domain names: "example.com" returns example.com and variations across extensions
    ///</param>
    ///<param name="extensions">
    ///Limits results to specific domain extensions from the supported set. If not specified,
    ///returns results across all supported extensions. Extensions not in the supported
    ///set are silently ignored.
    ///</param>
    ///<param name="limit">Maximum number of domain suggestions to return. Defaults to 20 if not specified.</param>
    ///<param name="cancellationToken"></param>
    member this.RegistrarDomainDiscoverySearch
        (accountId: string, q: string, ?extensions: list<string>, ?limit: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.query ("q", q)
                  if extensions.IsSome then
                      RequestPart.queryComma ("extensions", extensions.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar/domain-search"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarDomainDiscoverySearch.OK((Serializer.deserialize content))
            | 400 -> return RegistrarDomainDiscoverySearch.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarDomainDiscoverySearch" (int status)
        }

    ///<summary>
    ///Lists domains handled by Registrar.
    ///</summary>
    member this.RegistrarDomainsListDomains(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar/domains"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarDomainsListDomains.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return RegistrarDomainsListDomains.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarDomainsListDomains" (int status)
        }

    ///<summary>
    ///Show individual domain.
    ///</summary>
    member this.RegistrarDomainsGetDomain
        (domainName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("domain_name", domainName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar/domains/{domain_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarDomainsGetDomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return RegistrarDomainsGetDomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarDomainsGetDomain" (int status)
        }

    ///<summary>
    ///Updates an individual domain.
    ///</summary>
    member this.RegistrarDomainsUpdateDomain
        (
            domainName: string,
            accountId: string,
            body: registrar_u002D_api_domain_update_properties,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("domain_name", domainName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/registrar/domains/{domain_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarDomainsUpdateDomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return RegistrarDomainsUpdateDomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarDomainsUpdateDomain" (int status)
        }

    ///<summary>
    ///Returns metadata and JSON Schema documents describing the expected input
    ///structure for registration operations on each supported
    ///extension (TLD).
    ///This endpoint uses cursor-based pagination. Results are ordered by
    ///extension name by default. To fetch the next page, pass the `cursor`
    ///value from the `result_info` object in the response as the `cursor`
    ///query parameter in your next request. An empty `cursor` string
    ///indicates there are no more pages.
    ///Supports HTTP conditional GET via `ETag`. Include the `ETag` value
    ///from a previous response in an `If-None-Match` header to receive a
    ///`304 Not Modified` when the data has not changed.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="name">
    ///Filter extensions by exact name match.
    ///For example, `name=com` returns only the `com` extension.
    ///</param>
    ///<param name="cursor">
    ///Opaque token from a previous response's `result_info.cursor`.
    ///Pass this value to fetch the next page of results. Omit (or
    ///pass an empty string) for the first page.
    ///</param>
    ///<param name="perPage">Number of items to return per page.</param>
    ///<param name="direction">Sort direction for results. Defaults to ascending order.</param>
    ///<param name="sortBy">Column to sort results by. Defaults to `name` when omitted.</param>
    ///<param name="cancellationToken"></param>
    member this.RegistrarExtensionList
        (
            accountId: string,
            ?name: string,
            ?cursor: string,
            ?perPage: int,
            ?direction: string,
            ?sortBy: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if sortBy.IsSome then
                      RequestPart.query ("sort_by", sortBy.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar/extensions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarExtensionList.OK((Serializer.deserialize content))
            | 304 -> return RegistrarExtensionList.NotModified
            | _ when (((int status) / 100) = 4) ->
                return RegistrarExtensionList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarExtensionList" (int status)
        }

    ///<summary>
    ///Returns metadata and JSON Schema documents describing the expected input
    ///structure for registration operations on a specific
    ///extension (TLD).
    ///Supports HTTP conditional GET via `ETag`. Include the `ETag` value
    ///from a previous response in an `If-None-Match` header to receive a
    ///`304 Not Modified` when the data has not changed.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="extension">The extension name (e.g., `com`, `co.uk`).</param>
    ///<param name="cancellationToken"></param>
    member this.RegistrarExtensionGet(accountId: string, extension: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("extension", extension) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar/extensions/{extension}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarExtensionGet.OK((Serializer.deserialize content))
            | 304 -> return RegistrarExtensionGet.NotModified
            | _ when (((int status) / 100) = 4) ->
                return RegistrarExtensionGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarExtensionGet" (int status)
        }

    ///<summary>
    ///Returns a paginated list of domain registrations owned by the account.
    ///This endpoint uses cursor-based pagination. Results are ordered by registration
    ///date by default. To fetch the next page, pass the `cursor` value from the
    ///`result_info` object in the response as the `cursor` query parameter in
    ///your next request. An empty `cursor` string indicates there are no more
    ///pages.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cursor">
    ///Opaque token from a previous response's `result_info.cursor`.
    ///Pass this value to fetch the next page of results. Omit (or
    ///pass an empty string) for the first page.
    ///</param>
    ///<param name="perPage">Number of items to return per page.</param>
    ///<param name="direction">Sort direction for results. Defaults to ascending order.</param>
    ///<param name="sortBy">Column to sort results by. Defaults to registration date (`registry_created_at`) when omitted.</param>
    ///<param name="cancellationToken"></param>
    member this.RegistrarDomainRegistrationList
        (
            accountId: string,
            ?cursor: string,
            ?perPage: int,
            ?direction: string,
            ?sortBy: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if sortBy.IsSome then
                      RequestPart.query ("sort_by", sortBy.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar/registrations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarDomainRegistrationList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return RegistrarDomainRegistrationList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarDomainRegistrationList" (int status)
        }

    ///<summary>
    ///Starts a domain registration workflow. This is a billable operation — successful
    ///registration charges the account's default payment method. All successful
    ///domain registrations are non-refundable — once the workflow completes with
    ///`state: succeeded`, the charge cannot be reversed.
    ///### Prerequisites
    ///- The account must have a billing profile with a valid default payment method.
    ///  Set this up at `https://dash.cloudflare.com/{account_id}/billing/payment-info`.
    ///- The account must not already be at the maximum supported domain limit.
    ///  A single account may own up to 500 domains in total across registrations
    ///  created through either the dashboard or this API.
    ///- The domain must be on a supported extension for programmatic registration.
    ///- Use `POST /domain-check` immediately before calling this endpoint to confirm
    ///  real-time availability and pricing.
    ///### Express mode
    ///The only required field is `domain_name`. If `contacts` is omitted, the system
    ///uses the account's default address book entry as the registrant. If no default
    ///exists and no contact is provided, the request fails. Set up a default address
    ///book entry and accept the required agreement at
    ///`https://dash.cloudflare.com/{account_id}/domains/registrations`.
    ///### Defaults
    ///- `years`: defaults to the extension's minimum registration period (1 year for
    ///  most extensions, but varies — for example, `.ai` (if supported) requires a minimum of 2 years).
    ///- `auto_renew`: defaults to `false`. Setting it to `true` is an explicit
    ///  opt-in authorizing Cloudflare to charge the account's default payment
    ///  method up to 30 days before domain expiry to renew the registration.
    ///  Renewal pricing may change over time based on registry pricing.
    ///- `privacy_mode`: defaults to `redaction`.
    ///### Premium domains
    ///Premium domain registration is not currently supported by this API.
    ///If `POST /domain-check` returns `tier: premium`, do not call this
    ///endpoint for that domain.
    ///### Response behavior
    ///By default, the server holds the connection for a bounded, server-defined
    ///amount of time while the registration completes. Most registrations finish
    ///within this window and return `201 Created` with a completed workflow status.
    ///If the registration is still processing after this synchronous wait window,
    ///the server returns `202 Accepted`. Poll the URL in `links.self` to track progress.
    ///To skip the wait and receive an immediate `202`, send `Prefer: respond-async`.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID. Required for all Registrar API operations.</param>
    ///<param name="body"></param>
    ///<param name="prefer">
    ///Set to `respond-async` to receive an immediate `202 Accepted` without
    ///waiting for the operation to complete (RFC 7240).
    ///The header may be combined with other preferences using standard
    ///comma-separated syntax.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.RegistrarDomainRegistrationCreate
        (
            accountId: string,
            body: registrar_u002D_api_registration_create_request,
            ?prefer: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if prefer.IsSome then
                      RequestPart.header ("Prefer", prefer.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/registrar/registrations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return RegistrarDomainRegistrationCreate.Created((Serializer.deserialize content))
            | 202 -> return RegistrarDomainRegistrationCreate.Accepted((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return RegistrarDomainRegistrationCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarDomainRegistrationCreate" (int status)
        }

    ///<summary>
    ///Returns the current state of a domain registration.
    ///This is the canonical read endpoint for a domain you own. It returns
    ///the full registration resource including current settings and expiration.
    ///When the registration resource is ready, both `created_at` and `expires_at`
    ///are present in the response.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="domainName">Domain name to retrieve.</param>
    ///<param name="cancellationToken"></param>
    member this.RegistrarDomainRegistrationGet
        (accountId: string, domainName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain_name", domainName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar/registrations/{domain_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarDomainRegistrationGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return RegistrarDomainRegistrationGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarDomainRegistrationGet" (int status)
        }

    ///<summary>
    ///Updates an existing domain registration.
    ///By default, the server holds the connection for a bounded, server-defined
    ///amount of time while the update completes. Most updates finish within this
    ///window and return `200 OK` with a completed workflow status.
    ///If the update is still processing after this synchronous wait window, the
    ///server returns `202 Accepted`. Poll the URL in `links.self` to track progress.
    ///To skip the wait and receive an immediate `202`, send `Prefer: respond-async`.
    ///This endpoint currently supports updating `auto_renew` only.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="domainName">Domain name to update.</param>
    ///<param name="body">
    ///Request to update an existing domain registration.
    ///This endpoint currently supports updating `auto_renew` only.
    ///</param>
    ///<param name="prefer">
    ///Set to `respond-async` to receive an immediate `202 Accepted` without
    ///waiting for the operation to complete (RFC 7240).
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.RegistrarDomainRegistrationUpdate
        (
            accountId: string,
            domainName: string,
            body: registrar_u002D_api_registration_update_request,
            ?prefer: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain_name", domainName)
                  RequestPart.jsonContent body
                  if prefer.IsSome then
                      RequestPart.header ("Prefer", prefer.Value) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/registrar/registrations/{domain_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarDomainRegistrationUpdate.OK((Serializer.deserialize content))
            | 202 -> return RegistrarDomainRegistrationUpdate.Accepted((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return RegistrarDomainRegistrationUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarDomainRegistrationUpdate" (int status)
        }

    ///<summary>
    ///Returns the current status of a domain registration workflow.
    ///Use this endpoint to poll for completion when the POST response
    ///returned `202 Accepted`. The URL is provided in the `links.self`
    ///field of the workflow status response.
    ///Poll this endpoint until the workflow reaches a terminal state or a
    ///state that requires user attention.
    ///**Terminal states:** `succeeded` and `failed` are terminal and always
    ///have `completed: true`.
    ///**Non-terminal states:**
    ///- `action_required` has `completed: false` and will not resolve on its
    ///  own. The workflow is paused pending user intervention.
    ///- `blocked` has `completed: false` and indicates the workflow is waiting
    ///  on a third party such as the extension registry or losing registrar.
    ///  Continue polling while informing the user of the delay.
    ///Use increasing backoff between polls. When `state: blocked`, use a
    ///longer polling interval and do not poll indefinitely.
    ///A naive polling loop that only checks `completed` can run indefinitely
    ///when `state: action_required`. Break explicitly on `action_required`:
    ///```js
    ///let status;
    ///do {
    ///  await new Promise(r =&amp;gt; setTimeout(r, 2000));
    ///  status = await cloudflare.request({
    ///    method: 'GET',
    ///    path: reg.result.links.self,
    ///  });
    ///} while (
    ///  !status.result.completed &amp;&amp;
    ///  status.result.state !== 'action_required'
    ///);
    ///if (status.result.state === 'action_required') {
    ///  // Surface context.action and context.confirmation_sent_to to the user.
    ///  // Do not re-submit the registration request.
    ///}
    ///```
    ///</summary>
    ///<param name="accountId">Cloudflare account ID. Required for all Registrar API operations.</param>
    ///<param name="domainName"></param>
    ///<param name="cancellationToken"></param>
    member this.RegistrarDomainRegistrationGetStatus
        (accountId: string, domainName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain_name", domainName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar/registrations/{domain_name}/registration-status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarDomainRegistrationGetStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return RegistrarDomainRegistrationGetStatus.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegistrarDomainRegistrationGetStatus" (int status)
        }

    ///<summary>
    ///Returns the current status of a domain update workflow.
    ///Use this endpoint to poll for completion when the PATCH response
    ///returned `202 Accepted`. The URL is provided in the `links.self`
    ///field of the workflow status response.
    ///Poll this endpoint until the workflow reaches a terminal state or a
    ///state that requires user attention.
    ///Use increasing backoff between polls. When the workflow remains blocked
    ///on a third party, use a longer polling interval and do not poll indefinitely.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="domainName"></param>
    ///<param name="cancellationToken"></param>
    member this.RegistrarDomainRegistrationGetUpdateStatus
        (accountId: string, domainName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain_name", domainName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/registrar/registrations/{domain_name}/update-status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegistrarDomainRegistrationGetUpdateStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    RegistrarDomainRegistrationGetUpdateStatus.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for RegistrarDomainRegistrationGetUpdateStatus" (int status)
        }

    ///<summary>
    ///Get all available roles for an account.
    ///</summary>
    member this.AccountRolesListRoles
        (accountId: iam_account_identifier, ?page: float, ?perPage: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/roles" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountRolesListRoles.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountRolesListRoles.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountRolesListRoles" (int status)
        }

    ///<summary>
    ///Get information about a specific role for an account.
    ///</summary>
    member this.AccountRolesRoleDetails
        (roleId: string, accountId: iam_account_identifier, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("role_id", roleId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/roles/{role_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountRolesRoleDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountRolesRoleDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountRolesRoleDetails" (int status)
        }

    ///<summary>
    ///Returns a list of Image Resizing configurations across all zones for the account.
    ///This endpoint is useful for retrieving the transformations (image_resizing) state
    ///for all zones belonging to an account.
    ///</summary>
    member this.AccountSettingsListTransformations(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/settings/transformations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSettingsListTransformations.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountSettingsListTransformations.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountSettingsListTransformations" (int status)
        }

    ///<summary>
    ///Retrieves the Unique Transformations billing configuration for an account.
    ///When enabled, billing data is directed to the Transformations pipeline.
    ///</summary>
    member this.AccountSettingsGetUtBillingSetting(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/settings/ut-billing"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSettingsGetUtBillingSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountSettingsGetUtBillingSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountSettingsGetUtBillingSetting" (int status)
        }

    ///<summary>
    ///Updates the Unique Transformations billing configuration for an account.
    ///When enabled, billing data is directed to the Transformations pipeline.
    ///Note: setting the value to "off" is not permitted once enabled.
    ///</summary>
    member this.AccountSettingsChangeUtBillingSetting
        (accountId: string, body: AccountSettingsChangeUtBillingSettingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/settings/ut-billing"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSettingsChangeUtBillingSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountSettingsChangeUtBillingSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountSettingsChangeUtBillingSetting" (int status)
        }

    ///<summary>
    ///Lists all account shares.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="status">Filter shares by status.</param>
    ///<param name="kind">Filter shares by kind.</param>
    ///<param name="targetType">Filter shares by target_type.</param>
    ///<param name="resourceTypes">Filter share resources by resource_types.</param>
    ///<param name="order">Order shares by values in the given field.</param>
    ///<param name="direction">Direction to sort objects.</param>
    ///<param name="page">
    ///Page number. Defaults to `1` when `per_page` is supplied without
    ///`page`. May be omitted entirely along with `per_page` to receive a
    ///non-paginated response.
    ///</param>
    ///<param name="perPage">
    ///Number of objects to return per page. Defaults to `20` when `page`
    ///is supplied without `per_page`. May be omitted entirely along with
    ///`page` to receive a non-paginated response.
    ///</param>
    ///<param name="includeResources">Include resources in the response.</param>
    ///<param name="includeRecipientCounts">Include recipient counts in the response.</param>
    ///<param name="tag">Filter shares by tag. Each value is either `key=value` (matches shares whose tags contain that key/value pair) or `key` alone (matches shares that have any value for that key). May be repeated; multiple `tag` parameters are ANDed together. Maximum 20 `tag` parameters per request.</param>
    ///<param name="cancellationToken"></param>
    member this.SharesList
        (
            accountId: string,
            ?status: string,
            ?kind: string,
            ?targetType: string,
            ?resourceTypes: list<string>,
            ?order: string,
            ?direction: string,
            ?page: int,
            ?perPage: int,
            ?includeResources: bool,
            ?includeRecipientCounts: bool,
            ?tag: list<string>,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if kind.IsSome then
                      RequestPart.query ("kind", kind.Value)
                  if targetType.IsSome then
                      RequestPart.query ("target_type", targetType.Value)
                  if resourceTypes.IsSome then
                      RequestPart.query ("resource_types", resourceTypes.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if includeResources.IsSome then
                      RequestPart.query ("include_resources", includeResources.Value)
                  if includeRecipientCounts.IsSome then
                      RequestPart.query ("include_recipient_counts", includeRecipientCounts.Value)
                  if tag.IsSome then
                      RequestPart.query ("tag", tag.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/shares" requestParts cancellationToken

            match (int status) with
            | 200 -> return SharesList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SharesList.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return SharesList.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SharesList" (int status)
        }

    ///<summary>
    ///Creates a new resource share for sharing Cloudflare resources with other accounts or organizations.
    ///</summary>
    member this.ShareCreate
        (accountId: string, body: resource_u002D_sharing_create_share_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/shares" requestParts cancellationToken

            match (int status) with
            | 201 -> return ShareCreate.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareCreate.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareCreate" (int status)
        }

    ///<summary>
    ///Deletion is not immediate, an updated share object with a new status will be returned.
    ///</summary>
    member this.ShareDelete(accountId: string, shareId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareDelete.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareDelete" (int status)
        }

    ///<summary>
    ///Fetches share by ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="shareId"></param>
    ///<param name="includeResources">Include resources in the response.</param>
    ///<param name="includeRecipientCounts">Include recipient counts in the response.</param>
    ///<param name="cancellationToken"></param>
    member this.SharesGetById
        (
            accountId: string,
            shareId: string,
            ?includeResources: bool,
            ?includeRecipientCounts: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  if includeResources.IsSome then
                      RequestPart.query ("include_resources", includeResources.Value)
                  if includeRecipientCounts.IsSome then
                      RequestPart.query ("include_recipient_counts", includeRecipientCounts.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SharesGetById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SharesGetById.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return SharesGetById.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SharesGetById" (int status)
        }

    ///<summary>
    ///Updates the share's display name and tags. This endpoint does **not**
    ///modify recipients or resources — those are managed via dedicated
    ///subresource endpoints:
    ///- **Recipients**: Use `POST /accounts/{account_id}/shares/{share_id}/recipients`
    ///  to add a single recipient, `PUT /accounts/{account_id}/shares/{share_id}/recipients`
    ///  to replace the full recipient list, or
    ///  `DELETE /accounts/{account_id}/shares/{share_id}/recipients/{recipient_id}`
    ///  to remove a recipient.
    ///- **Resources**: Use the share's resource subresource endpoints.
    ///Updating is not immediate; an updated share object with a new status
    ///will be returned.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="shareId"></param>
    ///<param name="body">
    ///Request body for `PUT /accounts/{account_id}/shares/{share_id}`. The
    ///share's display `name` and `tags` can be updated via this endpoint. To
    ///modify recipients, use the
    ///`/accounts/{account_id}/shares/{share_id}/recipients` subresource
    ///endpoints (POST, PUT, DELETE).
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.ShareUpdate
        (
            accountId: string,
            shareId: string,
            body: resource_u002D_sharing_update_share_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareUpdate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareUpdate.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareUpdate" (int status)
        }

    ///<summary>
    ///Lists the accounts excluded from an organization-targeted share. Only valid for shares with `target_type=organization`.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="shareId"></param>
    ///<param name="page">
    ///Page number. Defaults to `1` when `per_page` is supplied without
    ///`page`. May be omitted entirely along with `per_page` to receive a
    ///non-paginated response.
    ///</param>
    ///<param name="perPage">
    ///Number of objects to return per page. Defaults to `20` when `page`
    ///is supplied without `per_page`. May be omitted entirely along with
    ///`page` to receive a non-paginated response.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.ShareExcludedRecipientsList
        (accountId: string, shareId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/excluded-recipients"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareExcludedRecipientsList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareExcludedRecipientsList.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareExcludedRecipientsList.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareExcludedRecipientsList" (int status)
        }

    ///<summary>
    ///Excludes a single account from an organization-targeted share. The share continues to target the entire organization, but the excluded account is skipped during reconciliation.
    ///</summary>
    member this.ShareExcludedRecipientCreate
        (
            accountId: string,
            shareId: string,
            body: resource_u002D_sharing_create_share_excluded_recipient_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/excluded-recipients"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return ShareExcludedRecipientCreate.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareExcludedRecipientCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareExcludedRecipientCreate.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareExcludedRecipientCreate" (int status)
        }

    ///<summary>
    ///Reconciles a share's excluded recipients to match the given list. Only valid for shares with `target_type=organization`.
    ///</summary>
    member this.ShareExcludedRecipientsUpdate
        (
            accountId: string,
            shareId: string,
            body: resource_u002D_sharing_update_share_excluded_recipients_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/excluded-recipients"
                    requestParts
                    cancellationToken

            match (int status) with
            | 204 -> return ShareExcludedRecipientsUpdate.NoContent
            | _ when (((int status) / 100) = 4) ->
                return ShareExcludedRecipientsUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareExcludedRecipientsUpdate.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareExcludedRecipientsUpdate" (int status)
        }

    ///<summary>
    ///Removes a single account from a share's excluded-recipient list. The account becomes eligible to receive the share again on the next reconciliation cycle.
    ///</summary>
    member this.ShareExcludedRecipientDelete
        (accountId: string, shareId: string, excludedRecipientId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.path ("excluded_recipient_id", excludedRecipientId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/excluded-recipients/{excluded_recipient_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareExcludedRecipientDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareExcludedRecipientDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareExcludedRecipientDelete.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareExcludedRecipientDelete" (int status)
        }

    ///<summary>
    ///Gets a single excluded recipient of a share by its identifier tag.
    ///</summary>
    member this.ShareExcludedRecipientsGetById
        (accountId: string, shareId: string, excludedRecipientId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.path ("excluded_recipient_id", excludedRecipientId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/excluded-recipients/{excluded_recipient_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareExcludedRecipientsGetById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareExcludedRecipientsGetById.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareExcludedRecipientsGetById.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareExcludedRecipientsGetById" (int status)
        }

    ///<summary>
    ///List share recipients by share ID. Returns **all** recipients
    ///regardless of their `association_status` (associating, associated,
    ///disassociating, disassociated). Callers that want only "active"
    ///recipients must filter client-side on the `association_status` field.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="shareId"></param>
    ///<param name="includeResources">Include resources in the response.</param>
    ///<param name="page">
    ///Page number. Defaults to `1` when `per_page` is supplied without
    ///`page`. May be omitted entirely along with `per_page` to receive a
    ///non-paginated response.
    ///</param>
    ///<param name="perPage">
    ///Number of objects to return per page. Defaults to `20` when `page`
    ///is supplied without `per_page`. May be omitted entirely along with
    ///`page` to receive a non-paginated response.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.ShareRecipientsList
        (
            accountId: string,
            shareId: string,
            ?includeResources: bool,
            ?page: int,
            ?perPage: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  if includeResources.IsSome then
                      RequestPart.query ("include_resources", includeResources.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/recipients"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareRecipientsList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareRecipientsList.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareRecipientsList.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareRecipientsList" (int status)
        }

    ///<summary>
    ///Adds a single recipient to an account-targeted resource share, granting
    ///them access to the shared resources. The recipient account must belong
    ///to the same organization as the share owner.
    ///To replace the entire recipient list in one call, use
    ///`PUT /accounts/{account_id}/shares/{share_id}/recipients` instead.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="shareId"></param>
    ///<param name="body">
    ///Optionally specify `recipient_account_id` to target a specific account, or `organization_id` to target the caller's whole organization. If neither is provided, the caller's organization is used.
    ///The legacy field `account_id` is accepted as a synonym for `recipient_account_id` during the deprecation period (see `x-sunset` on that field).
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.ShareRecipientCreate
        (
            accountId: string,
            shareId: string,
            body: resource_u002D_sharing_create_share_recipient_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/recipients"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return ShareRecipientCreate.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareRecipientCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareRecipientCreate.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareRecipientCreate" (int status)
        }

    ///<summary>
    ///Replaces the full recipient list for an account-targeted share. The
    ///server computes the diff between the current and desired recipients:
    ///new accounts are added, accounts no longer in the list are marked for
    ///disassociation, and unchanged accounts are left as-is.
    ///Returns an error if the share targets an organization
    ///(`target_type=organization`); org-targeted shares derive their
    ///recipients dynamically from org membership and cannot be modified
    ///through this endpoint.
    ///</summary>
    member this.ShareRecipientsUpdate
        (
            accountId: string,
            shareId: string,
            body: resource_u002D_sharing_update_share_recipients_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/recipients"
                    requestParts
                    cancellationToken

            match (int status) with
            | 204 -> return ShareRecipientsUpdate.NoContent
            | _ when (((int status) / 100) = 4) ->
                return ShareRecipientsUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareRecipientsUpdate.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareRecipientsUpdate" (int status)
        }

    ///<summary>
    ///Performs a **soft delete**: sets the recipient's
    ///`desired_association_status` to `disassociated`, which signals the
    ///background reconciliation workflow (Temporal) to remove the shared
    ///resources from the recipient account. The recipient record remains in
    ///the database for audit purposes and is still returned by
    ///`GET /accounts/{account_id}/shares/{share_id}/recipients` with its
    ///updated status.
    ///Resource access is not fully removed until the workflow completes and
    ///`current_association_status` transitions to `disassociated`. The
    ///recipient record itself is never physically deleted.
    ///</summary>
    member this.ShareRecipientDelete
        (accountId: string, shareId: string, recipientId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.path ("recipient_id", recipientId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/recipients/{recipient_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareRecipientDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareRecipientDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareRecipientDelete.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareRecipientDelete" (int status)
        }

    ///<summary>
    ///Get share recipient by ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="shareId"></param>
    ///<param name="recipientId"></param>
    ///<param name="includeResources">Include resources in the response.</param>
    ///<param name="cancellationToken"></param>
    member this.ShareRecipientsGetById
        (
            accountId: string,
            shareId: string,
            recipientId: string,
            ?includeResources: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.path ("recipient_id", recipientId)
                  if includeResources.IsSome then
                      RequestPart.query ("include_resources", includeResources.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/recipients/{recipient_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareRecipientsGetById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareRecipientsGetById.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareRecipientsGetById.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareRecipientsGetById" (int status)
        }

    ///<summary>
    ///List share resources by share ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="shareId"></param>
    ///<param name="status">Filter share resources by status.</param>
    ///<param name="resourceType">Filter share resources by resource_type.</param>
    ///<param name="page">
    ///Page number. Defaults to `1` when `per_page` is supplied without
    ///`page`. May be omitted entirely along with `per_page` to receive a
    ///non-paginated response.
    ///</param>
    ///<param name="perPage">
    ///Number of objects to return per page. Defaults to `20` when `page`
    ///is supplied without `per_page`. May be omitted entirely along with
    ///`page` to receive a non-paginated response.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.ShareResourcesList
        (
            accountId: string,
            shareId: string,
            ?status: string,
            ?resourceType: string,
            ?page: int,
            ?perPage: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if resourceType.IsSome then
                      RequestPart.query ("resource_type", resourceType.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/resources"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareResourcesList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareResourcesList.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareResourcesList.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareResourcesList" (int status)
        }

    ///<summary>
    ///Adds a resource to an existing share, making it available to share recipients.
    ///</summary>
    member this.ShareResourceCreate
        (
            accountId: string,
            shareId: string,
            body: resource_u002D_sharing_create_share_resource_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/resources"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return ShareResourceCreate.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareResourceCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareResourceCreate.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareResourceCreate" (int status)
        }

    ///<summary>
    ///Deletion is not immediate, an updated share resource object with a new status will be returned.
    ///</summary>
    member this.ShareResourceDelete
        (accountId: string, shareId: string, shareResourceId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.path ("share_resource_id", shareResourceId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/resources/{share_resource_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareResourceDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareResourceDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareResourceDelete.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareResourceDelete" (int status)
        }

    ///<summary>
    ///Get share resource by ID.
    ///</summary>
    member this.ShareResourcesGetById
        (accountId: string, shareId: string, shareResourceId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.path ("share_resource_id", shareResourceId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/resources/{share_resource_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareResourcesGetById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareResourcesGetById.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareResourcesGetById.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareResourcesGetById" (int status)
        }

    ///<summary>
    ///Update is not immediate, an updated share resource object with a new status will be returned.
    ///</summary>
    member this.ShareResourceUpdate
        (
            accountId: string,
            shareId: string,
            shareResourceId: string,
            body: resource_u002D_sharing_update_share_resource_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("share_id", shareId)
                  RequestPart.path ("share_resource_id", shareResourceId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/shares/{share_id}/resources/{share_resource_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ShareResourceUpdate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ShareResourceUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ShareResourceUpdate.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ShareResourceUpdate" (int status)
        }

    ///<summary>
    ///Lists all of an account's subscriptions.
    ///</summary>
    member this.AccountSubscriptionsListSubscriptions(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/subscriptions" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountSubscriptionsListSubscriptions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountSubscriptionsListSubscriptions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountSubscriptionsListSubscriptions" (int status)
        }

    ///<summary>
    ///Creates an account subscription.
    ///</summary>
    member this.AccountSubscriptionsCreateSubscription
        (accountId: string, body: bill_u002D_subs_u002D_api_subscription_u002D_v2, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/subscriptions" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountSubscriptionsCreateSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountSubscriptionsCreateSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountSubscriptionsCreateSubscription" (int status)
        }

    ///<summary>
    ///Cancels pending delayed downgrades for the specified subscriptions.
    ///</summary>
    member this.AccountSubscriptionsCancelDelayedDowngrade
        (
            accountId: string,
            body: AccountSubscriptionsCancelDelayedDowngradePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/subscriptions/cancel-downgrade"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSubscriptionsCancelDelayedDowngrade.OK((Serializer.deserialize content))
            | 204 -> return AccountSubscriptionsCancelDelayedDowngrade.NoContent
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountSubscriptionsCancelDelayedDowngrade.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountSubscriptionsCancelDelayedDowngrade" (int status)
        }

    ///<summary>
    ///Deletes an account's subscription.
    ///</summary>
    member this.AccountSubscriptionsDeleteSubscription
        (
            subscriptionIdentifier: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subscription_identifier", subscriptionIdentifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/subscriptions/{subscription_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSubscriptionsDeleteSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountSubscriptionsDeleteSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountSubscriptionsDeleteSubscription" (int status)
        }

    ///<summary>
    ///Gets an account subscription by identifier.
    ///</summary>
    member this.AccountSubscriptionsGetSubscription
        (subscriptionIdentifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subscription_identifier", subscriptionIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/subscriptions/{subscription_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSubscriptionsGetSubscription.OK((Serializer.deserialize content))
            | 404 -> return AccountSubscriptionsGetSubscription.NotFound((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountSubscriptionsGetSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountSubscriptionsGetSubscription" (int status)
        }

    ///<summary>
    ///Updates an account subscription.
    ///</summary>
    member this.AccountSubscriptionsUpdateSubscription
        (
            subscriptionIdentifier: string,
            accountId: string,
            body: bill_u002D_subs_u002D_api_subscription_u002D_v2,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subscription_identifier", subscriptionIdentifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/subscriptions/{subscription_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSubscriptionsUpdateSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountSubscriptionsUpdateSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountSubscriptionsUpdateSubscription" (int status)
        }

    ///<summary>
    ///Smartly applies the incoming subscription into the lifecycle of the subscription.
    ///</summary>
    member this.AccountSubscriptionsActionAppendSubscription
        (
            subscriptionIdentifier: string,
            accountId: string,
            body: bill_u002D_subs_u002D_api_subscription_u002D_v2,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subscription_identifier", subscriptionIdentifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/subscriptions/{subscription_identifier}/action/append"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSubscriptionsActionAppendSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountSubscriptionsActionAppendSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for AccountSubscriptionsActionAppendSubscription" (int status)
        }

    ///<summary>
    ///Gets the cancellation reason for an account subscription.
    ///</summary>
    member this.AccountSubscriptionsGetCancelReason
        (subscriptionIdentifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subscription_identifier", subscriptionIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/subscriptions/{subscription_identifier}/cancel-reason"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSubscriptionsGetCancelReason.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountSubscriptionsGetCancelReason.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountSubscriptionsGetCancelReason" (int status)
        }

    ///<summary>
    ///Records a cancellation reason for an account subscription.
    ///</summary>
    member this.AccountSubscriptionsCreateCancelReason
        (
            subscriptionIdentifier: string,
            accountId: string,
            body: bill_u002D_subs_u002D_api_cancel_reason_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subscription_identifier", subscriptionIdentifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/subscriptions/{subscription_identifier}/cancel-reason"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountSubscriptionsCreateCancelReason.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountSubscriptionsCreateCancelReason.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountSubscriptionsCreateCancelReason" (int status)
        }

    ///<summary>
    ///Removes all tags from a specific account-level resource.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body">Request body schema for deleting tags from account-level resources. Use /zones/{zone_id}/tags for zone-level resources.</param>
    ///<param name="ifMatch">
    ///ETag value for optimistic concurrency control. When provided, the server will
    ///verify the current resource ETag matches before applying the write. Returns
    ///412 Precondition Failed if the resource has been modified since the ETag was
    ///obtained. Omit this header for unconditional writes.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.TagsDelete
        (
            accountId: resource_u002D_tagging_account_id,
            body: resource_u002D_tagging_delete_tags_request_account_level,
            ?ifMatch: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if ifMatch.IsSome then
                      RequestPart.header ("If-Match", ifMatch.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/accounts/{account_id}/tags" requestParts cancellationToken

            match (int status) with
            | 204 -> return TagsDelete.NoContent
            | 412 -> return TagsDelete.PreconditionFailed((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TagsDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return TagsDelete.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TagsDelete" (int status)
        }

    ///<summary>
    ///Retrieves tags for a specific account-level resource.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="resourceId">The ID of the resource to retrieve tags for.</param>
    ///<param name="resourceType">The type of the resource.</param>
    ///<param name="workerId">Worker identifier. Required for worker_version resources.</param>
    ///<param name="cancellationToken"></param>
    member this.TagsGet
        (
            accountId: resource_u002D_tagging_account_id,
            resourceId: string,
            resourceType: string,
            ?workerId: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.query ("resource_id", resourceId)
                  RequestPart.query ("resource_type", resourceType)
                  if workerId.IsSome then
                      RequestPart.query ("worker_id", workerId.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/tags" requestParts cancellationToken

            match (int status) with
            | 200 -> return TagsGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TagsGet.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return TagsGet.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TagsGet" (int status)
        }

    ///<summary>
    ///Creates or updates tags for a specific account-level resource.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body">Request body schema for setting tags on account-level resources.</param>
    ///<param name="ifMatch">
    ///ETag value for optimistic concurrency control. When provided, the server will
    ///verify the current resource ETag matches before applying the write. Returns
    ///412 Precondition Failed if the resource has been modified since the ETag was
    ///obtained. Omit this header for unconditional writes.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.TagsSet
        (
            accountId: resource_u002D_tagging_account_id,
            body: resource_u002D_tagging_set_tags_request_account_level,
            ?ifMatch: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if ifMatch.IsSome then
                      RequestPart.header ("If-Match", ifMatch.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/tags" requestParts cancellationToken

            match (int status) with
            | 200 -> return TagsSet.OK((Serializer.deserialize content))
            | 412 -> return TagsSet.PreconditionFailed((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TagsSet.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return TagsSet.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TagsSet" (int status)
        }

    ///<summary>
    ///Lists all distinct tag keys used across resources in an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="cursor">Cursor for pagination.</param>
    ///<param name="cancellationToken"></param>
    member this.TagsListKeys
        (accountId: resource_u002D_tagging_account_id, ?cursor: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/tags/keys" requestParts cancellationToken

            match (int status) with
            | 200 -> return TagsListKeys.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TagsListKeys.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return TagsListKeys.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TagsListKeys" (int status)
        }

    ///<summary>
    ///Lists all tagged resources for an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="type">Filter by resource type. Can be repeated to filter by multiple types (OR logic). Example: ?type=zone&amp;type=worker</param>
    ///<param name="name">Filter by resource name. Performs a case-insensitive substring match. Example: ?name=my-zone</param>
    ///<param name="id">Filter by resource ID. Can be repeated up to 50 times to filter by multiple IDs. Example: ?id=abc&amp;id=def</param>
    ///<param name="caseInsensitive">Match `tag` keys and values case-insensitively. Stored casing is unchanged. Example: ?tag=environment=production&amp;case_insensitive=true</param>
    ///<param name="tag">
    ///Filter resources by tag criteria. This parameter can be repeated multiple times, with AND logic between parameters.
    ///Supported syntax:
    ///- **Key-only**: `tag=&amp;lt;key&amp;gt;` - Resource must have the tag key (e.g., `tag=production`)
    ///- **Key-value**: `tag=&amp;lt;key&amp;gt;=&amp;lt;value&amp;gt;` - Resource must have the tag with specific value (e.g., `tag=env=prod`)
    ///- **Multiple values (OR)**: `tag=&amp;lt;key&amp;gt;=&amp;lt;v1&amp;gt;,&amp;lt;v2&amp;gt;` - Resource must have tag with any of the values (e.g., `tag=env=prod,staging`)
    ///- **Negate key-only**: `tag=!&amp;lt;key&amp;gt;` - Resource must not have the tag key (e.g., `tag=!archived`)
    ///- **Negate key-value**: `tag=&amp;lt;key&amp;gt;!=&amp;lt;value&amp;gt;` - Resource must not have the tag with specific value (e.g., `tag=region!=us-west-1`)
    ///Multiple tag parameters are combined with AND logic.
    ///</param>
    ///<param name="cursor">Cursor for pagination.</param>
    ///<param name="cancellationToken"></param>
    member this.TagsList
        (
            accountId: resource_u002D_tagging_account_id,
            ?``type``: list<string>,
            ?name: string,
            ?id: list<string>,
            ?caseInsensitive: bool,
            ?tag: list<string>,
            ?cursor: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if id.IsSome then
                      RequestPart.query ("id", id.Value)
                  if caseInsensitive.IsSome then
                      RequestPart.query ("case_insensitive", caseInsensitive.Value)
                  if tag.IsSome then
                      RequestPart.query ("tag", tag.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/tags/resources" requestParts cancellationToken

            match (int status) with
            | 200 -> return TagsList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TagsList.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return TagsList.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TagsList" (int status)
        }

    ///<summary>
    ///Lists all distinct tag keys and their distinct values across resources in an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="cursor">Cursor for pagination.</param>
    ///<param name="cancellationToken"></param>
    member this.TagsListKeySummary
        (accountId: resource_u002D_tagging_account_id, ?cursor: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/tags/summary" requestParts cancellationToken

            match (int status) with
            | 200 -> return TagsListKeySummary.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TagsListKeySummary.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return TagsListKeySummary.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TagsListKeySummary" (int status)
        }

    ///<summary>
    ///Lists all distinct values for a given tag key, optionally filtered by resource type.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="tagKey">The tag key to retrieve values for.</param>
    ///<param name="type">Filter by resource type.</param>
    ///<param name="cursor">Cursor for pagination.</param>
    ///<param name="cancellationToken"></param>
    member this.TagsListValues
        (
            accountId: resource_u002D_tagging_account_id,
            tagKey: string,
            ?``type``: string,
            ?cursor: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tag_key", tagKey)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/tags/values/{tag_key}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TagsListValues.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TagsListValues.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return TagsListValues.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TagsListValues" (int status)
        }

    ///<summary>
    ///List all Account Owned API tokens created for this account. Results include active, disabled, and recently-expired tokens when include_expired is set to true.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="direction"></param>
    ///<param name="includeExpired">When true, includes recently-expired tokens in the response.</param>
    ///<param name="cancellationToken"></param>
    member this.AccountApiTokensListTokens
        (
            accountId: iam_account_identifier,
            ?page: float,
            ?perPage: float,
            ?direction: string,
            ?includeExpired: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if includeExpired.IsSome then
                      RequestPart.query ("include_expired", includeExpired.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/tokens" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountApiTokensListTokens.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountApiTokensListTokens.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountApiTokensListTokens" (int status)
        }

    ///<summary>
    ///Create a new Account Owned API token.
    ///</summary>
    member this.AccountApiTokensCreateToken
        (accountId: iam_account_identifier, body: iam_create_payload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/tokens" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountApiTokensCreateToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountApiTokensCreateToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountApiTokensCreateToken" (int status)
        }

    ///<summary>
    ///Find all available permission groups for Account Owned API Tokens
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="name">
    ///Filter by the name of the permission group.
    ///The value must be URL-encoded.
    ///</param>
    ///<param name="scope">
    ///Filter by the scope of the permission group.
    ///The value must be URL-encoded.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.AccountApiTokensListPermissionGroups
        (accountId: iam_account_identifier, ?name: string, ?scope: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if scope.IsSome then
                      RequestPart.query ("scope", scope.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/tokens/permission_groups"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountApiTokensListPermissionGroups.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountApiTokensListPermissionGroups.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountApiTokensListPermissionGroups" (int status)
        }

    ///<summary>
    ///Test whether a token works.
    ///</summary>
    member this.AccountApiTokensVerifyToken(accountId: iam_account_identifier, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/tokens/verify" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountApiTokensVerifyToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountApiTokensVerifyToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountApiTokensVerifyToken" (int status)
        }

    ///<summary>
    ///Destroy an Account Owned API token.
    ///</summary>
    member this.AccountApiTokensDeleteToken
        (
            accountId: iam_account_identifier,
            tokenId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("token_id", tokenId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/tokens/{token_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountApiTokensDeleteToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountApiTokensDeleteToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountApiTokensDeleteToken" (int status)
        }

    ///<summary>
    ///Get information about a specific Account Owned API token.
    ///</summary>
    member this.AccountApiTokensTokenDetails
        (accountId: iam_account_identifier, tokenId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("token_id", tokenId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/tokens/{token_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountApiTokensTokenDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountApiTokensTokenDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountApiTokensTokenDetails" (int status)
        }

    ///<summary>
    ///Update an existing token.
    ///</summary>
    member this.AccountApiTokensUpdateToken
        (accountId: iam_account_identifier, tokenId: string, body: iam_token_body, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("token_id", tokenId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/tokens/{token_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountApiTokensUpdateToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountApiTokensUpdateToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountApiTokensUpdateToken" (int status)
        }

    ///<summary>
    ///Roll the Account Owned API token secret.
    ///</summary>
    member this.AccountApiTokensRollToken
        (
            accountId: iam_account_identifier,
            tokenId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("token_id", tokenId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/tokens/{token_id}/value"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountApiTokensRollToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountApiTokensRollToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountApiTokensRollToken" (int status)
        }

    ///<summary>
    ///Validates a billing address and returns validated address suggestions. Authentication is not enforced to support pre-signup address validation flows, so credentials are accepted but not required.
    ///</summary>
    member this.BillingValidateAddress
        (body: bill_u002D_subs_u002D_api_address_validation_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/billing/address-validation" requestParts cancellationToken

            match (int status) with
            | 200 -> return BillingValidateAddress.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return BillingValidateAddress.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BillingValidateAddress" (int status)
        }

    ///<summary>
    ///Gets a rate plan's details by its public key (e.g., 'teams_free', 'cf_pro_20_20'). This is a public catalog endpoint, so authentication is not enforced and credentials are accepted but not required.
    ///</summary>
    ///<param name="publicKey">The public key identifier for the rate plan.</param>
    ///<param name="cancellationToken"></param>
    member this.BillingGetRatePlan(publicKey: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("public_key", publicKey) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/billing/rate_plans/{public_key}" requestParts cancellationToken

            match (int status) with
            | 200 -> return BillingGetRatePlan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return BillingGetRatePlan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BillingGetRatePlan" (int status)
        }

    ///<summary>
    ///List memberships of accounts the user can access.
    ///</summary>
    member this.User'SAccountMembershipsListMemberships
        (
            ?accountName: string,
            ?page: float,
            ?perPage: float,
            ?order: string,
            ?direction: string,
            ?name: string,
            ?status: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if accountName.IsSome then
                      RequestPart.query ("account.name", accountName.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value) ]

            let! (status, _, content) = OpenApiHttp.getAsync httpClient "/memberships" requestParts cancellationToken

            match (int status) with
            | 200 -> return User'SAccountMembershipsListMemberships.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return User'SAccountMembershipsListMemberships.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for User'SAccountMembershipsListMemberships" (int status)
        }

    ///<summary>
    ///Remove the associated member from an account.
    ///</summary>
    member this.User'SAccountMembershipsDeleteMembership
        (membershipId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("membership_id", membershipId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/memberships/{membership_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return User'SAccountMembershipsDeleteMembership.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return User'SAccountMembershipsDeleteMembership.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for User'SAccountMembershipsDeleteMembership" (int status)
        }

    ///<summary>
    ///Get a specific membership.
    ///</summary>
    member this.User'SAccountMembershipsMembershipDetails(membershipId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("membership_id", membershipId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/memberships/{membership_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return User'SAccountMembershipsMembershipDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return User'SAccountMembershipsMembershipDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for User'SAccountMembershipsMembershipDetails" (int status)
        }

    ///<summary>
    ///Accept or reject this account invitation.
    ///</summary>
    member this.User'SAccountMembershipsUpdateMembership
        (
            membershipId: string,
            body: User'SAccountMembershipsUpdateMembershipPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("membership_id", membershipId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/memberships/{membership_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return User'SAccountMembershipsUpdateMembership.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return User'SAccountMembershipsUpdateMembership.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for User'SAccountMembershipsUpdateMembership" (int status)
        }

    ///<summary>
    ///List all available OAuth scopes. This endpoint requires authentication but has no authorization role requirements.
    ///</summary>
    member this.OauthScopesList(?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = []
            let! (status, _, content) = OpenApiHttp.getAsync httpClient "/oauth/scopes" requestParts cancellationToken

            match (int status) with
            | 200 -> return OauthScopesList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OauthScopesList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OauthScopesList" (int status)
        }

    ///<summary>
    ///Retrieve a list of organizations a particular user has access to. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    ///<param name="id">
    ///Only return organizations with the specified IDs (ex. id=foo&amp;id=bar). Send multiple elements
    ///by repeating the query value.
    ///</param>
    ///<param name="name">
    ///(case-sensitive) Filter the list of organizations to where the name is equal to a
    ///particular string.
    ///</param>
    ///<param name="nameStartsWith">
    ///(case-insensitive) Filter the list of organizations to where the name starts with a
    ///particular string.
    ///</param>
    ///<param name="nameEndsWith">
    ///(case-insensitive) Filter the list of organizations to where the name ends with a particular
    ///string.
    ///</param>
    ///<param name="nameContains">
    ///(case-insensitive) Filter the list of organizations to where the name contains a particular
    ///string.
    ///</param>
    ///<param name="containingAccount">
    ///Filter the list of organizations to the ones that contain this particular
    ///account.
    ///</param>
    ///<param name="containingUser">
    ///Filter the list of organizations to the ones that contain this particular
    ///user.
    ///IMPORTANT: Just because an organization "contains" a user is not a
    ///representation of any authorization or privilege to manage any resources
    ///therein. An organization "containing" a user simply means the user is managed by
    ///that organization.
    ///</param>
    ///<param name="containingOrganization">
    ///Filter the list of organizations to the ones that contain this particular
    ///organization.
    ///</param>
    ///<param name="parentId">
    ///Filter the list of organizations to the ones that are a sub-organization
    ///of the specified organization.
    ///"null" is a valid value to provide for this parameter. It means "where
    ///an organization has no parent (i.e. it is a 'root' organization)."
    ///</param>
    ///<param name="pageToken">
    ///An opaque token returned from the last list response that when
    ///provided will retrieve the next page.
    ///Parameters used to filter the retrieved list must remain in subsequent
    ///requests with a page token.
    ///</param>
    ///<param name="pageSize">The amount of items to return. Defaults to 10.</param>
    ///<param name="cancellationToken"></param>
    member this.OrganizationListOrganizations
        (
            ?id: list<string>,
            ?name: string,
            ?nameStartsWith: string,
            ?nameEndsWith: string,
            ?nameContains: string,
            ?containingAccount: string,
            ?containingUser: string,
            ?containingOrganization: string,
            ?parentId: InlineUnion_238c0f85c1cf54ddecc1c91c,
            ?pageToken: string,
            ?pageSize: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if id.IsSome then
                      RequestPart.queryComma ("id", id.Value)
                  if name.IsSome then
                      RequestPart.queryComma ("name", name.Value)
                  if nameStartsWith.IsSome then
                      RequestPart.queryComma ("name.startsWith", nameStartsWith.Value)
                  if nameEndsWith.IsSome then
                      RequestPart.queryComma ("name.endsWith", nameEndsWith.Value)
                  if nameContains.IsSome then
                      RequestPart.queryComma ("name.contains", nameContains.Value)
                  if containingAccount.IsSome then
                      RequestPart.queryComma ("containing.account", containingAccount.Value)
                  if containingUser.IsSome then
                      RequestPart.queryComma ("containing.user", containingUser.Value)
                  if containingOrganization.IsSome then
                      RequestPart.queryComma ("containing.organization", containingOrganization.Value)
                  if parentId.IsSome then
                      RequestPart.queryComma ("parent.id", parentId.Value)
                  if pageToken.IsSome then
                      RequestPart.queryComma ("page_token", pageToken.Value)
                  if pageSize.IsSome then
                      RequestPart.queryComma ("page_size", pageSize.Value) ]

            let! (status, _, content) = OpenApiHttp.getAsync httpClient "/organizations" requestParts cancellationToken

            match (int status) with
            | 200 -> return OrganizationListOrganizations.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OrganizationListOrganizations.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OrganizationListOrganizations" (int status)
        }

    ///<summary>
    ///Create a new organization for a user. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    ///<param name="body">References an Organization in the Cloudflare data model.</param>
    ///<param name="cancellationToken"></param>
    member this.OrganizationsCreateUserOrganization
        (body: organizations_u002D_api_Organization, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]
            let! (status, _, content) = OpenApiHttp.postAsync httpClient "/organizations" requestParts cancellationToken

            match (int status) with
            | 200 -> return OrganizationsCreateUserOrganization.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OrganizationsCreateUserOrganization.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OrganizationsCreateUserOrganization" (int status)
        }

    ///<summary>
    ///Delete an organization. The organization MUST be empty before deleting.
    ///It must not contain any sub-organizations, accounts, members or users. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///**Access Control:** Restricted to enterprise organizations.
    ///</summary>
    ///<param name="organizationId">The ID of the organization to delete.</param>
    ///<param name="cancellationToken"></param>
    member this.OrganizationsDelete(organizationId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("organization_id", organizationId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/organizations/{organization_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return OrganizationsDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OrganizationsDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OrganizationsDelete" (int status)
        }

    ///<summary>
    ///Retrieve the details of a certain organization. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    ///<param name="organizationId">The ID of the organization to retrieve.</param>
    ///<param name="cancellationToken"></param>
    member this.OrganizationsRetrieve(organizationId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("organization_id", organizationId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/organizations/{organization_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return OrganizationsRetrieve.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OrganizationsRetrieve.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OrganizationsRetrieve" (int status)
        }

    ///<summary>
    ///Modify organization. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    ///<param name="organizationId">The ID of the organization to modify.</param>
    ///<param name="body">References an Organization in the Cloudflare data model.</param>
    ///<param name="cancellationToken"></param>
    member this.OrganizationsModify
        (organizationId: string, body: organizations_u002D_api_Organization, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/organizations/{organization_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return OrganizationsModify.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OrganizationsModify.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OrganizationsModify" (int status)
        }

    ///<summary>
    ///Retrieve a list of accounts that belong to a specific organization. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    ///<param name="organizationId">The ID of the organization to retrieve a list of accounts for.</param>
    ///<param name="accountPubname">
    ///(case-insensitive) Filter the list of accounts to where the account_pubname is equal to
    ///a particular string.
    ///</param>
    ///<param name="accountPubnameStartsWith">
    ///(case-insensitive) Filter the list of accounts to where the account_pubname starts with
    ///a particular string.
    ///</param>
    ///<param name="accountPubnameEndsWith">
    ///(case-insensitive) Filter the list of accounts to where the account_pubname ends with
    ///a particular string.
    ///</param>
    ///<param name="accountPubnameContains">
    ///(case-insensitive) Filter the list of accounts to where the account_pubname contains
    ///a particular string.
    ///</param>
    ///<param name="name">
    ///(case-insensitive) Filter the list of accounts to where the name is equal to a
    ///particular string.
    ///</param>
    ///<param name="nameStartsWith">
    ///(case-insensitive) Filter the list of accounts to where the name starts with a
    ///particular string.
    ///</param>
    ///<param name="nameEndsWith">
    ///(case-insensitive) Filter the list of accounts to where the name ends with a particular
    ///string.
    ///</param>
    ///<param name="nameContains">
    ///(case-insensitive) Filter the list of accounts to where the name contains a particular
    ///string.
    ///</param>
    ///<param name="orderBy">
    ///Field to order results by. Currently supported values: `account_name`.
    ///When not specified, results are ordered by internal account ID.
    ///</param>
    ///<param name="direction">
    ///Sort direction for the order_by field. Valid values: `asc`, `desc`.
    ///Defaults to `asc` when order_by is specified.
    ///</param>
    ///<param name="includeTags">Include Account tags from the resource tag mirror. Omit this parameter to preserve the existing Account response shape.</param>
    ///<param name="pageToken">
    ///An opaque token returned from the last list response that when
    ///provided will retrieve the next page.
    ///Parameters used to filter the retrieved list must remain in subsequent
    ///requests with a page token.
    ///</param>
    ///<param name="pageSize">The amount of items to return. Defaults to 10.</param>
    ///<param name="cancellationToken"></param>
    member this.OrganizationsGetAccounts
        (
            organizationId: string,
            ?accountPubname: string,
            ?accountPubnameStartsWith: string,
            ?accountPubnameEndsWith: string,
            ?accountPubnameContains: string,
            ?name: string,
            ?nameStartsWith: string,
            ?nameEndsWith: string,
            ?nameContains: string,
            ?orderBy: string,
            ?direction: string,
            ?includeTags: bool,
            ?pageToken: string,
            ?pageSize: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  if accountPubname.IsSome then
                      RequestPart.queryComma ("account_pubname", accountPubname.Value)
                  if accountPubnameStartsWith.IsSome then
                      RequestPart.queryComma ("account_pubname.startsWith", accountPubnameStartsWith.Value)
                  if accountPubnameEndsWith.IsSome then
                      RequestPart.queryComma ("account_pubname.endsWith", accountPubnameEndsWith.Value)
                  if accountPubnameContains.IsSome then
                      RequestPart.queryComma ("account_pubname.contains", accountPubnameContains.Value)
                  if name.IsSome then
                      RequestPart.queryComma ("name", name.Value)
                  if nameStartsWith.IsSome then
                      RequestPart.queryComma ("name.startsWith", nameStartsWith.Value)
                  if nameEndsWith.IsSome then
                      RequestPart.queryComma ("name.endsWith", nameEndsWith.Value)
                  if nameContains.IsSome then
                      RequestPart.queryComma ("name.contains", nameContains.Value)
                  if orderBy.IsSome then
                      RequestPart.queryComma ("order_by", orderBy.Value)
                  if direction.IsSome then
                      RequestPart.queryComma ("direction", direction.Value)
                  if includeTags.IsSome then
                      RequestPart.queryComma ("include_tags", includeTags.Value)
                  if pageToken.IsSome then
                      RequestPart.queryComma ("page_token", pageToken.Value)
                  if pageSize.IsSome then
                      RequestPart.queryComma ("page_size", pageSize.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/organizations/{organization_id}/accounts"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OrganizationsGetAccounts.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OrganizationsGetAccounts.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OrganizationsGetAccounts" (int status)
        }

    ///<summary>
    ///Returns cost and usage data for all accounts within an organization,
    ///aligned with the [FinOps FOCUS v1.3](https://focus.finops.org/focus-specification/v1-3/)
    ///Cost and Usage dataset specification.
    ///Each record represents one billable metric for one account on one day.
    ///This includes all metered usage, including usage that falls within
    ///free-tier allowances and may result in zero cost. The response
    ///includes usage for every account belonging to the specified
    ///organization.
    ///**Note:** Cost and pricing fields are not yet populated and
    ///will be absent from responses until billing integration is complete.
    ///When `from` and `to` are omitted, defaults to the start of the current
    ///month through today. The maximum date range is 31 days.
    ///</summary>
    ///<param name="organizationId">Identifies the Cloudflare organization.</param>
    ///<param name="from">Start date for the usage query (ISO 8601). Required if `to` is set. When omitted along with `to`, defaults to the start of the current month. Filters by charge period (when consumption happened), not billing period. The maximum date range is 31 days.</param>
    ///<param name="to">End date for the usage query (ISO 8601). Required if `from` is set. When omitted along with `from`, defaults to today. Filters by charge period (when consumption happened), not billing period. The maximum date range is 31 days.</param>
    ///<param name="cancellationToken"></param>
    member this.BillableUsageV2GetOrganizationUsage
        (organizationId: string, ?from: string, ?``to``: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  if from.IsSome then
                      RequestPart.query ("from", from.Value)
                  if ``to``.IsSome then
                      RequestPart.query ("to", ``to``.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/organizations/{organization_id}/billable/usage"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BillableUsageV2GetOrganizationUsage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return BillableUsageV2GetOrganizationUsage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BillableUsageV2GetOrganizationUsage" (int status)
        }

    ///<summary>
    ///Gets a list of audit logs for an organization.
    ///</summary>
    ///<param name="organizationId"></param>
    ///<param name="since">Limits the returned results to logs newer than the specified date. This can be a date string 2019-04-30 (interpreted in UTC) or an absolute timestamp that conforms to RFC3339.</param>
    ///<param name="before">Limits the returned results to logs older than the specified date. This can be a date string 2019-04-30 (interpreted in UTC) or an absolute timestamp that conforms to RFC3339.</param>
    ///<param name="actionResult"></param>
    ///<param name="actionType"></param>
    ///<param name="actorContext"></param>
    ///<param name="actorEmail"></param>
    ///<param name="actorId"></param>
    ///<param name="actorIpAddress"></param>
    ///<param name="actorTokenId"></param>
    ///<param name="actorTokenName"></param>
    ///<param name="actorType"></param>
    ///<param name="id"></param>
    ///<param name="rawCfRayId"></param>
    ///<param name="rawMethod"></param>
    ///<param name="rawStatusCode"></param>
    ///<param name="rawUri"></param>
    ///<param name="resourceId"></param>
    ///<param name="resourceProduct"></param>
    ///<param name="resourceType"></param>
    ///<param name="resourceScope"></param>
    ///<param name="actionResultNot"></param>
    ///<param name="actionTypeNot"></param>
    ///<param name="actorContextNot"></param>
    ///<param name="actorEmailNot"></param>
    ///<param name="actorIdNot"></param>
    ///<param name="actorIpAddressNot"></param>
    ///<param name="actorTokenIdNot"></param>
    ///<param name="actorTokenNameNot"></param>
    ///<param name="actorTypeNot"></param>
    ///<param name="idNot"></param>
    ///<param name="rawCfRayIdNot"></param>
    ///<param name="rawMethodNot"></param>
    ///<param name="rawStatusCodeNot"></param>
    ///<param name="rawUriNot"></param>
    ///<param name="resourceIdNot"></param>
    ///<param name="resourceProductNot"></param>
    ///<param name="resourceTypeNot"></param>
    ///<param name="resourceScopeNot"></param>
    ///<param name="direction"></param>
    ///<param name="limit"></param>
    ///<param name="cursor"></param>
    ///<param name="cancellationToken"></param>
    member this.AuditLogsV2GetOrganizationAuditLogs
        (
            organizationId: string,
            since: string,
            before: string,
            ?actionResult: list<string>,
            ?actionType: list<string>,
            ?actorContext: list<string>,
            ?actorEmail: list<string>,
            ?actorId: list<string>,
            ?actorIpAddress: list<string>,
            ?actorTokenId: list<string>,
            ?actorTokenName: list<string>,
            ?actorType: list<string>,
            ?id: list<string>,
            ?rawCfRayId: list<string>,
            ?rawMethod: list<string>,
            ?rawStatusCode: list<int>,
            ?rawUri: list<string>,
            ?resourceId: list<string>,
            ?resourceProduct: list<string>,
            ?resourceType: list<string>,
            ?resourceScope: list<string>,
            ?actionResultNot: list<string>,
            ?actionTypeNot: list<string>,
            ?actorContextNot: list<string>,
            ?actorEmailNot: list<string>,
            ?actorIdNot: list<string>,
            ?actorIpAddressNot: list<string>,
            ?actorTokenIdNot: list<string>,
            ?actorTokenNameNot: list<string>,
            ?actorTypeNot: list<string>,
            ?idNot: list<string>,
            ?rawCfRayIdNot: list<string>,
            ?rawMethodNot: list<string>,
            ?rawStatusCodeNot: list<int>,
            ?rawUriNot: list<string>,
            ?resourceIdNot: list<string>,
            ?resourceProductNot: list<string>,
            ?resourceTypeNot: list<string>,
            ?resourceScopeNot: list<string>,
            ?direction: string,
            ?limit: float,
            ?cursor: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  RequestPart.query ("since", since)
                  RequestPart.query ("before", before)
                  if actionResult.IsSome then
                      RequestPart.query ("action_result", actionResult.Value)
                  if actionType.IsSome then
                      RequestPart.query ("action_type", actionType.Value)
                  if actorContext.IsSome then
                      RequestPart.query ("actor_context", actorContext.Value)
                  if actorEmail.IsSome then
                      RequestPart.query ("actor_email", actorEmail.Value)
                  if actorId.IsSome then
                      RequestPart.query ("actor_id", actorId.Value)
                  if actorIpAddress.IsSome then
                      RequestPart.query ("actor_ip_address", actorIpAddress.Value)
                  if actorTokenId.IsSome then
                      RequestPart.query ("actor_token_id", actorTokenId.Value)
                  if actorTokenName.IsSome then
                      RequestPart.query ("actor_token_name", actorTokenName.Value)
                  if actorType.IsSome then
                      RequestPart.query ("actor_type", actorType.Value)
                  if id.IsSome then
                      RequestPart.query ("id", id.Value)
                  if rawCfRayId.IsSome then
                      RequestPart.query ("raw_cf_ray_id", rawCfRayId.Value)
                  if rawMethod.IsSome then
                      RequestPart.query ("raw_method", rawMethod.Value)
                  if rawStatusCode.IsSome then
                      RequestPart.query ("raw_status_code", rawStatusCode.Value)
                  if rawUri.IsSome then
                      RequestPart.query ("raw_uri", rawUri.Value)
                  if resourceId.IsSome then
                      RequestPart.query ("resource_id", resourceId.Value)
                  if resourceProduct.IsSome then
                      RequestPart.query ("resource_product", resourceProduct.Value)
                  if resourceType.IsSome then
                      RequestPart.query ("resource_type", resourceType.Value)
                  if resourceScope.IsSome then
                      RequestPart.query ("resource_scope", resourceScope.Value)
                  if actionResultNot.IsSome then
                      RequestPart.query ("action_result.not", actionResultNot.Value)
                  if actionTypeNot.IsSome then
                      RequestPart.query ("action_type.not", actionTypeNot.Value)
                  if actorContextNot.IsSome then
                      RequestPart.query ("actor_context.not", actorContextNot.Value)
                  if actorEmailNot.IsSome then
                      RequestPart.query ("actor_email.not", actorEmailNot.Value)
                  if actorIdNot.IsSome then
                      RequestPart.query ("actor_id.not", actorIdNot.Value)
                  if actorIpAddressNot.IsSome then
                      RequestPart.query ("actor_ip_address.not", actorIpAddressNot.Value)
                  if actorTokenIdNot.IsSome then
                      RequestPart.query ("actor_token_id.not", actorTokenIdNot.Value)
                  if actorTokenNameNot.IsSome then
                      RequestPart.query ("actor_token_name.not", actorTokenNameNot.Value)
                  if actorTypeNot.IsSome then
                      RequestPart.query ("actor_type.not", actorTypeNot.Value)
                  if idNot.IsSome then
                      RequestPart.query ("id.not", idNot.Value)
                  if rawCfRayIdNot.IsSome then
                      RequestPart.query ("raw_cf_ray_id.not", rawCfRayIdNot.Value)
                  if rawMethodNot.IsSome then
                      RequestPart.query ("raw_method.not", rawMethodNot.Value)
                  if rawStatusCodeNot.IsSome then
                      RequestPart.query ("raw_status_code.not", rawStatusCodeNot.Value)
                  if rawUriNot.IsSome then
                      RequestPart.query ("raw_uri.not", rawUriNot.Value)
                  if resourceIdNot.IsSome then
                      RequestPart.query ("resource_id.not", resourceIdNot.Value)
                  if resourceProductNot.IsSome then
                      RequestPart.query ("resource_product.not", resourceProductNot.Value)
                  if resourceTypeNot.IsSome then
                      RequestPart.query ("resource_type.not", resourceTypeNot.Value)
                  if resourceScopeNot.IsSome then
                      RequestPart.query ("resource_scope.not", resourceScopeNot.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/organizations/{organization_id}/logs/audit"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AuditLogsV2GetOrganizationAuditLogs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AuditLogsV2GetOrganizationAuditLogs.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AuditLogsV2GetOrganizationAuditLogs" (int status)
        }

    ///<summary>
    ///Returns the chronological change history for the resource identified by the given organization-scoped audit log entry.
    ///The endpoint first locates the source audit log entry by `id` (using `action_time` to narrow the lookup window), derives identifying filters from that entry, and then returns matching audit logs within the `since`/`before` window.
    ///The `result_info.history_status` field indicates the quality of the resource identification used:
    ///- `exact`: Resource was identified by the resource URI.
    ///- `approximate`: Resource was identified without the resource URI.
    ///- `unavailable`: The source audit log entry did not contain enough information to identify the resource; an empty result is returned.
    ///</summary>
    ///<param name="organizationId"></param>
    ///<param name="id"></param>
    ///<param name="actionTime">RFC3339 timestamp of the source audit log entry's action time. Used to narrow the source-entry lookup window. Provide the `action.time` value from the audit log identified by `id`.</param>
    ///<param name="since">Limits the returned results to logs newer than the specified date. This can be a date string 2019-04-30 (interpreted in UTC) or an absolute timestamp that conforms to RFC3339.</param>
    ///<param name="before">Limits the returned results to logs older than the specified date. This can be a date string 2019-04-30 (interpreted in UTC) or an absolute timestamp that conforms to RFC3339.</param>
    ///<param name="direction"></param>
    ///<param name="limit"></param>
    ///<param name="cursor"></param>
    ///<param name="cancellationToken"></param>
    member this.AuditLogsV2GetOrganizationAuditLogHistory
        (
            organizationId: string,
            id: System.Guid,
            actionTime: System.DateTimeOffset,
            since: string,
            before: string,
            ?direction: string,
            ?limit: float,
            ?cursor: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  RequestPart.path ("id", id)
                  RequestPart.query ("action_time", actionTime)
                  RequestPart.query ("since", since)
                  RequestPart.query ("before", before)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/organizations/{organization_id}/logs/audit/{id}/history"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AuditLogsV2GetOrganizationAuditLogHistory.OK((Serializer.deserialize content))
            | 404 -> return AuditLogsV2GetOrganizationAuditLogHistory.NotFound((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AuditLogsV2GetOrganizationAuditLogHistory.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AuditLogsV2GetOrganizationAuditLogHistory" (int status)
        }

    ///<summary>
    ///List memberships for an Organization. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    ///<param name="organizationId"></param>
    ///<param name="status">Filter the list of memberships by membership status.</param>
    ///<param name="userEmail">Filter the list of memberships for a specific email.</param>
    ///<param name="userEmailContains">Filter the list of memberships for a specific email that contains a substring.</param>
    ///<param name="userEmailStartsWith">Filter the list of memberships for a specific email that starts with a substring.</param>
    ///<param name="userEmailEndsWith">Filter the list of memberships for a specific email that ends with a substring.</param>
    ///<param name="pageToken">
    ///An opaque token returned from the last list response that when
    ///provided will retrieve the next page.
    ///Parameters used to filter the retrieved list must remain in subsequent
    ///requests with a page token.
    ///</param>
    ///<param name="pageSize">The amount of items to return. Defaults to 10.</param>
    ///<param name="cancellationToken"></param>
    member this.MembersList
        (
            organizationId: string,
            ?status: list<string>,
            ?userEmail: string,
            ?userEmailContains: string,
            ?userEmailStartsWith: string,
            ?userEmailEndsWith: string,
            ?pageToken: string,
            ?pageSize: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if userEmail.IsSome then
                      RequestPart.queryComma ("user.email", userEmail.Value)
                  if userEmailContains.IsSome then
                      RequestPart.queryComma ("user.email.contains", userEmailContains.Value)
                  if userEmailStartsWith.IsSome then
                      RequestPart.queryComma ("user.email.startsWith", userEmailStartsWith.Value)
                  if userEmailEndsWith.IsSome then
                      RequestPart.queryComma ("user.email.endsWith", userEmailEndsWith.Value)
                  if pageToken.IsSome then
                      RequestPart.queryComma ("page_token", pageToken.Value)
                  if pageSize.IsSome then
                      RequestPart.queryComma ("page_size", pageSize.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/organizations/{organization_id}/members"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MembersList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MembersList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MembersList" (int status)
        }

    ///<summary>
    ///Create a membership that grants access to a specific Organization. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    member this.MembersCreate
        (
            organizationId: string,
            body: organizations_u002D_api_CreateMemberRequest,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/organizations/{organization_id}/members"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MembersCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MembersCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MembersCreate" (int status)
        }

    ///<summary>
    ///Delete a membership to a particular Organization. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    member this.MembersDelete
        (organizationId: string, memberId: string, body: MembersDeletePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  RequestPart.path ("member_id", memberId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/organizations/{organization_id}/members/{member_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 204 -> return MembersDelete.NoContent
            | _ when (((int status) / 100) = 4) ->
                return MembersDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MembersDelete" (int status)
        }

    ///<summary>
    ///Retrieve a single membership from an Organization. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    member this.MembersRetrieve(organizationId: string, memberId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  RequestPart.path ("member_id", memberId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/organizations/{organization_id}/members/{member_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MembersRetrieve.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MembersRetrieve.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MembersRetrieve" (int status)
        }

    ///<summary>
    ///Batch create multiple memberships that grant access to a specific Organization.
    ///</summary>
    member this.MembersBatchCreate
        (
            organizationId: string,
            body: organizations_u002D_api_BatchCreateMembersRequest,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/organizations/{organization_id}/members:batchCreate"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MembersBatchCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MembersBatchCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MembersBatchCreate" (int status)
        }

    ///<summary>
    ///Get an organizations profile if it exists. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    ///<param name="organizationId">The ID of the organization to retrieve a profile for.</param>
    ///<param name="cancellationToken"></param>
    member this.OrganizationsGetProfile(organizationId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("organization_id", organizationId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/organizations/{organization_id}/profile"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OrganizationsGetProfile.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OrganizationsGetProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OrganizationsGetProfile" (int status)
        }

    ///<summary>
    ///Modify organization profile. (Currently in Public Beta - see https://developers.cloudflare.com/fundamentals/organizations/)
    ///</summary>
    member this.OrganizationsModifyProfile
        (organizationId: string, body: organizations_u002D_api_Profile, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/organizations/{organization_id}/profile"
                    requestParts
                    cancellationToken

            match (int status) with
            | 204 -> return OrganizationsModifyProfile.NoContent
            | _ when (((int status) / 100) = 4) ->
                return OrganizationsModifyProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OrganizationsModifyProfile" (int status)
        }

    ///<summary>
    ///Lists all organization shares.
    ///</summary>
    ///<param name="organizationId"></param>
    ///<param name="status">Filter shares by status.</param>
    ///<param name="kind">Filter shares by kind.</param>
    ///<param name="targetType">Filter shares by target_type.</param>
    ///<param name="resourceTypes">Filter share resources by resource_types.</param>
    ///<param name="order">Order shares by values in the given field.</param>
    ///<param name="direction">Direction to sort objects.</param>
    ///<param name="page">
    ///Page number. Defaults to `1` when `per_page` is supplied without
    ///`page`. May be omitted entirely along with `per_page` to receive a
    ///non-paginated response.
    ///</param>
    ///<param name="perPage">
    ///Number of objects to return per page. Defaults to `20` when `page`
    ///is supplied without `per_page`. May be omitted entirely along with
    ///`page` to receive a non-paginated response.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.OrganizationSharesList
        (
            organizationId: string,
            ?status: string,
            ?kind: string,
            ?targetType: string,
            ?resourceTypes: list<string>,
            ?order: string,
            ?direction: string,
            ?page: int,
            ?perPage: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if kind.IsSome then
                      RequestPart.query ("kind", kind.Value)
                  if targetType.IsSome then
                      RequestPart.query ("target_type", targetType.Value)
                  if resourceTypes.IsSome then
                      RequestPart.query ("resource_types", resourceTypes.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/organizations/{organization_id}/shares" requestParts cancellationToken

            match (int status) with
            | 200 -> return OrganizationSharesList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OrganizationSharesList.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return OrganizationSharesList.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OrganizationSharesList" (int status)
        }

    ///<summary>
    ///Retrieves a Tenant by Tenant ID.
    ///</summary>
    member this.TenantsRetrieveTenant(tenantId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("tenant_id", tenantId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/tenants/{tenant_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return TenantsRetrieveTenant.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TenantsRetrieveTenant.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TenantsRetrieveTenant" (int status)
        }

    ///<summary>
    ///List of account types available for the Tenant to provision accounts.
    ///</summary>
    member this.TenantsValidAccountTypes(tenantId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("tenant_id", tenantId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/tenants/{tenant_id}/account_types" requestParts cancellationToken

            match (int status) with
            | 200 -> return TenantsValidAccountTypes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TenantsValidAccountTypes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TenantsValidAccountTypes" (int status)
        }

    ///<summary>
    ///List of accounts for the Tenant.
    ///</summary>
    member this.TenantsListAccounts(tenantId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("tenant_id", tenantId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/tenants/{tenant_id}/accounts" requestParts cancellationToken

            match (int status) with
            | 200 -> return TenantsListAccounts.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TenantsListAccounts.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TenantsListAccounts" (int status)
        }

    ///<summary>
    ///List of innate entitlements available for the Tenant.
    ///</summary>
    member this.TenantsListEntitlements(tenantId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("tenant_id", tenantId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/tenants/{tenant_id}/entitlements" requestParts cancellationToken

            match (int status) with
            | 200 -> return TenantsListEntitlements.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TenantsListEntitlements.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TenantsListEntitlements" (int status)
        }

    ///<summary>
    ///List of active members (Cloudflare users) for the Tenant.
    ///</summary>
    member this.TenantsListMemberships(tenantId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("tenant_id", tenantId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/tenants/{tenant_id}/memberships" requestParts cancellationToken

            match (int status) with
            | 200 -> return TenantsListMemberships.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TenantsListMemberships.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TenantsListMemberships" (int status)
        }

    ///<summary>
    ///Lists a tenant's custom nameservers.
    ///</summary>
    member this.TenantLevelCustomNameserversListTenantCustomNameservers
        (tenantTag: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("tenant_tag", tenantTag) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/tenants/{tenant_tag}/custom_ns" requestParts cancellationToken

            match (int status) with
            | 200 -> return TenantLevelCustomNameserversListTenantCustomNameservers.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    TenantLevelCustomNameserversListTenantCustomNameservers.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for TenantLevelCustomNameserversListTenantCustomNameservers"
                        (int status)
        }

    ///<summary>
    ///Adds a custom nameserver for a tenant.
    ///</summary>
    member this.TenantLevelCustomNameserversAddTenantCustomNameserver
        (
            tenantTag: string,
            body: dns_u002D_custom_u002D_nameservers_CustomNSInput,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("tenant_tag", tenantTag); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/tenants/{tenant_tag}/custom_ns" requestParts cancellationToken

            match (int status) with
            | 200 -> return TenantLevelCustomNameserversAddTenantCustomNameserver.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    TenantLevelCustomNameserversAddTenantCustomNameserver.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for TenantLevelCustomNameserversAddTenantCustomNameserver"
                        (int status)
        }

    ///<summary>
    ///Deletes a tenant's custom nameserver.
    ///</summary>
    member this.TenantLevelCustomNameserversDeleteTenantCustomNameserver
        (customNsId: string, tenantTag: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("custom_ns_id", customNsId)
                  RequestPart.path ("tenant_tag", tenantTag) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/tenants/{tenant_tag}/custom_ns/{custom_ns_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return TenantLevelCustomNameserversDeleteTenantCustomNameserver.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    TenantLevelCustomNameserversDeleteTenantCustomNameserver.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for TenantLevelCustomNameserversDeleteTenantCustomNameserver"
                        (int status)
        }

    ///<summary>
    ///Retrieves detailed information about the currently authenticated user, including email, name, and account memberships.
    ///</summary>
    member this.UserUserDetails(?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = []
            let! (status, _, content) = OpenApiHttp.getAsync httpClient "/user" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserUserDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserUserDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserUserDetails" (int status)
        }

    ///<summary>
    ///Edit part of your user details.
    ///</summary>
    member this.UserEditUser(body: UserEditUserPayload, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]
            let! (status, _, content) = OpenApiHttp.patchAsync httpClient "/user" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserEditUser.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserEditUser.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserEditUser" (int status)
        }

    ///<summary>
    ///The user analytics dashboard provides totals and timeseries data aggregated
    ///across all zones owned by the authenticated user for the given time period.
    ///Only zones for which the user has the `#analytics:read` permission are included.
    ///This endpoint is deprecated. Please use the GraphQL Analytics API instead:
    ///https://developers.cloudflare.com/analytics/graphql-api/
    ///</summary>
    member this.UserAnalyticsGetDashboard
        (
            ?since: zone_u002D_analytics_u002D_api_since,
            ?until: zone_u002D_analytics_u002D_api_until,
            ?continuous: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if since.IsSome then
                      RequestPart.query ("since", since.Value)
                  if until.IsSome then
                      RequestPart.query ("until", until.Value)
                  if continuous.IsSome then
                      RequestPart.query ("continuous", continuous.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/analytics/dashboard" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserAnalyticsGetDashboard.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserAnalyticsGetDashboard.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserAnalyticsGetDashboard" (int status)
        }

    ///<summary>
    ///Gets a list of audit logs for a user account. Can be filtered by who made the change, on which zone, and the timeframe of the change.
    ///</summary>
    member this.AuditLogsGetUserAuditLogs
        (
            ?id: string,
            ?export: bool,
            ?actionType: string,
            ?actorIp: string,
            ?actorEmail: string,
            ?since: InlineUnion_789e8c731edb2f2bd04d64be,
            ?before: InlineUnion_96dd0b3e2bc6f4d7d0183513,
            ?zoneName: string,
            ?direction: string,
            ?perPage: float,
            ?page: float,
            ?hideUserLogs: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if id.IsSome then
                      RequestPart.query ("id", id.Value)
                  if export.IsSome then
                      RequestPart.query ("export", export.Value)
                  if actionType.IsSome then
                      RequestPart.query ("action.type", actionType.Value)
                  if actorIp.IsSome then
                      RequestPart.query ("actor.ip", actorIp.Value)
                  if actorEmail.IsSome then
                      RequestPart.query ("actor.email", actorEmail.Value)
                  if since.IsSome then
                      RequestPart.query ("since", since.Value)
                  if before.IsSome then
                      RequestPart.query ("before", before.Value)
                  if zoneName.IsSome then
                      RequestPart.query ("zone.name", zoneName.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if hideUserLogs.IsSome then
                      RequestPart.query ("hide_user_logs", hideUserLogs.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/audit_logs" requestParts cancellationToken

            match (int status) with
            | 200 -> return AuditLogsGetUserAuditLogs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AuditLogsGetUserAuditLogs.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AuditLogsGetUserAuditLogs" (int status)
        }

    ///<summary>
    ///Accesses your billing history object.
    ///</summary>
    member this.``UserBillingHistory(Deprecated)BillingHistoryDetails``
        (
            ?page: float,
            ?perPage: float,
            ?order: string,
            ?occurredAt: System.DateTimeOffset,
            ?``type``: string,
            ?action: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if occurredAt.IsSome then
                      RequestPart.query ("occurred_at", occurredAt.Value)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value)
                  if action.IsSome then
                      RequestPart.query ("action", action.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/billing/history" requestParts cancellationToken

            match (int status) with
            | 200 -> return ``UserBillingHistory(Deprecated)BillingHistoryDetails``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``UserBillingHistory(Deprecated)BillingHistoryDetails``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for UserBillingHistory(Deprecated)BillingHistoryDetails"
                        (int status)
        }

    ///<summary>
    ///Accesses your billing profile object.
    ///</summary>
    member this.``UserBillingProfile(Deprecated)BillingProfileDetails``(?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = []

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/billing/profile" requestParts cancellationToken

            match (int status) with
            | 200 -> return ``UserBillingProfile(Deprecated)BillingProfileDetails``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``UserBillingProfile(Deprecated)BillingProfileDetails``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for UserBillingProfile(Deprecated)BillingProfileDetails"
                        (int status)
        }

    ///<summary>
    ///Retrieve the communication preferences for the authenticated user, including email verification status, marketing subscription opt-in/opt-out state, and language locale. Callers authenticate with standard Cloudflare API tokens or keys.
    ///</summary>
    member this.UserCommunicationPreferencesGet(?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = []

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/communication_preferences" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserCommunicationPreferencesGet.OK((Serializer.deserialize content))
            | 400 -> return UserCommunicationPreferencesGet.BadRequest((Serializer.deserialize content))
            | 401 -> return UserCommunicationPreferencesGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return UserCommunicationPreferencesGet.Forbidden((Serializer.deserialize content))
            | 404 -> return UserCommunicationPreferencesGet.NotFound((Serializer.deserialize content))
            | 500 -> return UserCommunicationPreferencesGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserCommunicationPreferencesGet" (int status)
        }

    ///<summary>
    ///Set one or more communication preferences for the authenticated user. Supply a map of preference keys to subscription states, and an optional language locale. This endpoint does not modify email verification settings. Callers authenticate with standard Cloudflare API tokens or keys.
    ///</summary>
    ///<param name="body">Request body for updating communication preferences. An email field in the payload is ignored; email verification state is managed separately, so the field is excluded from this schema.</param>
    ///<param name="cancellationToken"></param>
    member this.UserCommunicationPreferencesUpdate
        (body: cps_update_communication_preferences_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/user/communication_preferences" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserCommunicationPreferencesUpdate.OK((Serializer.deserialize content))
            | 400 -> return UserCommunicationPreferencesUpdate.BadRequest((Serializer.deserialize content))
            | 401 -> return UserCommunicationPreferencesUpdate.Unauthorized((Serializer.deserialize content))
            | 403 -> return UserCommunicationPreferencesUpdate.Forbidden((Serializer.deserialize content))
            | 404 -> return UserCommunicationPreferencesUpdate.NotFound((Serializer.deserialize content))
            | 500 -> return UserCommunicationPreferencesUpdate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserCommunicationPreferencesUpdate" (int status)
        }

    ///<summary>
    ///Fetches IP Access rules of the user. You can filter the results using several optional parameters.
    ///</summary>
    member this.IpAccessRulesForAUserListIpAccessRules
        (
            ?mode: string,
            ?configurationTarget: string,
            ?configurationValue: string,
            ?notes: string,
            ?``match``: string,
            ?page: float,
            ?perPage: float,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if mode.IsSome then
                      RequestPart.query ("mode", mode.Value)
                  if configurationTarget.IsSome then
                      RequestPart.query ("configuration.target", configurationTarget.Value)
                  if configurationValue.IsSome then
                      RequestPart.query ("configuration.value", configurationValue.Value)
                  if notes.IsSome then
                      RequestPart.query ("notes", notes.Value)
                  if ``match``.IsSome then
                      RequestPart.query ("match", ``match``.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/firewall/access_rules/rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return IpAccessRulesForAUserListIpAccessRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAccessRulesForAUserListIpAccessRules.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for IpAccessRulesForAUserListIpAccessRules" (int status)
        }

    ///<summary>
    ///Creates a new IP Access rule for all zones owned by the current user.
    ///Note: To create an IP Access rule that applies to a specific zone, refer to the [IP Access rules for a zone](#ip-access-rules-for-a-zone) endpoints.
    ///</summary>
    member this.IpAccessRulesForAUserCreateAnIpAccessRule
        (body: IpAccessRulesForAUserCreateAnIpAccessRulePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/user/firewall/access_rules/rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return IpAccessRulesForAUserCreateAnIpAccessRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAccessRulesForAUserCreateAnIpAccessRule.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for IpAccessRulesForAUserCreateAnIpAccessRule" (int status)
        }

    ///<summary>
    ///Deletes an IP Access rule at the user level.
    ///Note: Deleting a user-level rule will affect all zones owned by the user.
    ///</summary>
    member this.IpAccessRulesForAUserDeleteAnIpAccessRule(ruleId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("rule_id", ruleId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/user/firewall/access_rules/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAccessRulesForAUserDeleteAnIpAccessRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAccessRulesForAUserDeleteAnIpAccessRule.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for IpAccessRulesForAUserDeleteAnIpAccessRule" (int status)
        }

    ///<summary>
    ///Fetches the details of an IP Access rule defined at the user level.
    ///</summary>
    member this.IpAccessRulesForAUserGetAnIpAccessRule(ruleId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("rule_id", ruleId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/user/firewall/access_rules/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAccessRulesForAUserGetAnIpAccessRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAccessRulesForAUserGetAnIpAccessRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for IpAccessRulesForAUserGetAnIpAccessRule" (int status)
        }

    ///<summary>
    ///Updates an IP Access rule defined at the user level. You can only update the rule action (`mode` parameter) and notes.
    ///</summary>
    member this.IpAccessRulesForAUserUpdateAnIpAccessRule
        (ruleId: string, body: IpAccessRulesForAUserUpdateAnIpAccessRulePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("rule_id", ruleId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/user/firewall/access_rules/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAccessRulesForAUserUpdateAnIpAccessRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAccessRulesForAUserUpdateAnIpAccessRule.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for IpAccessRulesForAUserUpdateAnIpAccessRule" (int status)
        }

    ///<summary>
    ///Lists all invitations associated with my user.
    ///</summary>
    member this.User'SInvitesListInvitations(?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = []
            let! (status, _, content) = OpenApiHttp.getAsync httpClient "/user/invites" requestParts cancellationToken

            match (int status) with
            | 200 -> return User'SInvitesListInvitations.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return User'SInvitesListInvitations.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for User'SInvitesListInvitations" (int status)
        }

    ///<summary>
    ///Gets the details of an invitation.
    ///</summary>
    member this.User'SInvitesInvitationDetails(inviteId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("invite_id", inviteId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/invites/{invite_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return User'SInvitesInvitationDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return User'SInvitesInvitationDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for User'SInvitesInvitationDetails" (int status)
        }

    ///<summary>
    ///Responds to an invitation.
    ///</summary>
    member this.User'SInvitesRespondToInvitation
        (inviteId: string, body: User'SInvitesRespondToInvitationPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("invite_id", inviteId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/user/invites/{invite_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return User'SInvitesRespondToInvitation.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return User'SInvitesRespondToInvitation.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for User'SInvitesRespondToInvitation" (int status)
        }

    ///<summary>
    ///List configured monitors for a user.
    ///</summary>
    member this.LoadBalancerMonitorsListMonitors(?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = []

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/load_balancers/monitors" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerMonitorsListMonitors.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerMonitorsListMonitors.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerMonitorsListMonitors" (int status)
        }

    ///<summary>
    ///Create a configured monitor.
    ///</summary>
    member this.LoadBalancerMonitorsCreateMonitor
        (body: load_u002D_balancing_monitor_u002D_editable, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/user/load_balancers/monitors" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerMonitorsCreateMonitor.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerMonitorsCreateMonitor.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerMonitorsCreateMonitor" (int status)
        }

    ///<summary>
    ///Delete a configured monitor.
    ///</summary>
    member this.LoadBalancerMonitorsDeleteMonitor
        (monitorId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_id", monitorId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/user/load_balancers/monitors/{monitor_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerMonitorsDeleteMonitor.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerMonitorsDeleteMonitor.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerMonitorsDeleteMonitor" (int status)
        }

    ///<summary>
    ///List a single configured monitor for a user.
    ///</summary>
    member this.LoadBalancerMonitorsMonitorDetails(monitorId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("monitor_id", monitorId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/user/load_balancers/monitors/{monitor_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerMonitorsMonitorDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerMonitorsMonitorDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerMonitorsMonitorDetails" (int status)
        }

    ///<summary>
    ///Apply changes to an existing monitor, overwriting the supplied properties.
    ///</summary>
    member this.LoadBalancerMonitorsPatchMonitor
        (monitorId: string, body: load_u002D_balancing_monitor_u002D_editable, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_id", monitorId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/user/load_balancers/monitors/{monitor_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerMonitorsPatchMonitor.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerMonitorsPatchMonitor.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerMonitorsPatchMonitor" (int status)
        }

    ///<summary>
    ///Modify a configured monitor.
    ///</summary>
    member this.LoadBalancerMonitorsUpdateMonitor
        (monitorId: string, body: load_u002D_balancing_monitor_u002D_editable, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_id", monitorId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/user/load_balancers/monitors/{monitor_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerMonitorsUpdateMonitor.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerMonitorsUpdateMonitor.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerMonitorsUpdateMonitor" (int status)
        }

    ///<summary>
    ///Preview pools using the specified monitor with provided monitor details. The returned preview_id can be used in the preview endpoint to retrieve the results.
    ///</summary>
    member this.LoadBalancerMonitorsPreviewMonitor
        (monitorId: string, body: load_u002D_balancing_monitor_u002D_editable, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_id", monitorId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/user/load_balancers/monitors/{monitor_id}/preview"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerMonitorsPreviewMonitor.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerMonitorsPreviewMonitor.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerMonitorsPreviewMonitor" (int status)
        }

    ///<summary>
    ///Get the list of resources that reference the provided monitor.
    ///</summary>
    member this.LoadBalancerMonitorsListMonitorReferences(monitorId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("monitor_id", monitorId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/user/load_balancers/monitors/{monitor_id}/references"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerMonitorsListMonitorReferences.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerMonitorsListMonitorReferences.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for LoadBalancerMonitorsListMonitorReferences" (int status)
        }

    ///<summary>
    ///List configured pools.
    ///</summary>
    member this.LoadBalancerPoolsListPools(?monitor: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if monitor.IsSome then
                      RequestPart.query ("monitor", monitor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/load_balancers/pools" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerPoolsListPools.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerPoolsListPools.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerPoolsListPools" (int status)
        }

    ///<summary>
    ///Apply changes to a number of existing pools, overwriting the supplied properties. Pools are ordered by ascending `name`. Returns the list of affected pools. Supports the standard pagination query parameters, either `limit`/`offset` or `per_page`/`page`.
    ///</summary>
    member this.LoadBalancerPoolsPatchPools
        (body: LoadBalancerPoolsPatchPoolsPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/user/load_balancers/pools" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerPoolsPatchPools.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerPoolsPatchPools.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerPoolsPatchPools" (int status)
        }

    ///<summary>
    ///Create a new pool.
    ///</summary>
    member this.LoadBalancerPoolsCreatePool
        (body: LoadBalancerPoolsCreatePoolPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/user/load_balancers/pools" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerPoolsCreatePool.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerPoolsCreatePool.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerPoolsCreatePool" (int status)
        }

    ///<summary>
    ///Delete a configured pool.
    ///</summary>
    member this.LoadBalancerPoolsDeletePool
        (poolId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/user/load_balancers/pools/{pool_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerPoolsDeletePool.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerPoolsDeletePool.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerPoolsDeletePool" (int status)
        }

    ///<summary>
    ///Fetch a single configured pool.
    ///</summary>
    member this.LoadBalancerPoolsPoolDetails(poolId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("pool_id", poolId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/load_balancers/pools/{pool_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerPoolsPoolDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerPoolsPoolDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerPoolsPoolDetails" (int status)
        }

    ///<summary>
    ///Apply changes to an existing pool, overwriting the supplied properties.
    ///</summary>
    member this.LoadBalancerPoolsPatchPool
        (poolId: string, body: LoadBalancerPoolsPatchPoolPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/user/load_balancers/pools/{pool_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerPoolsPatchPool.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerPoolsPatchPool.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerPoolsPatchPool" (int status)
        }

    ///<summary>
    ///Modify a configured pool.
    ///</summary>
    member this.LoadBalancerPoolsUpdatePool
        (poolId: string, body: LoadBalancerPoolsUpdatePoolPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/user/load_balancers/pools/{pool_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerPoolsUpdatePool.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerPoolsUpdatePool.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerPoolsUpdatePool" (int status)
        }

    ///<summary>
    ///Fetch the latest pool health status for a single pool.
    ///</summary>
    member this.LoadBalancerPoolsPoolHealthDetails(poolId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("pool_id", poolId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/user/load_balancers/pools/{pool_id}/health"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerPoolsPoolHealthDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerPoolsPoolHealthDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerPoolsPoolHealthDetails" (int status)
        }

    ///<summary>
    ///Preview pool health using provided monitor details. The returned preview_id can be used in the preview endpoint to retrieve the results.
    ///</summary>
    member this.LoadBalancerPoolsPreviewPool
        (poolId: string, body: load_u002D_balancing_monitor_u002D_editable, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/user/load_balancers/pools/{pool_id}/preview"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerPoolsPreviewPool.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerPoolsPreviewPool.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerPoolsPreviewPool" (int status)
        }

    ///<summary>
    ///Get the list of resources that reference the provided pool.
    ///</summary>
    member this.LoadBalancerPoolsListPoolReferences(poolId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("pool_id", poolId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/user/load_balancers/pools/{pool_id}/references"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerPoolsListPoolReferences.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerPoolsListPoolReferences.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerPoolsListPoolReferences" (int status)
        }

    ///<summary>
    ///Get the result of a previous preview operation using the provided preview_id.
    ///</summary>
    member this.LoadBalancerMonitorsPreviewResult
        (previewId: load_u002D_balancing_preview_id, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("preview_id", previewId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/user/load_balancers/preview/{preview_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerMonitorsPreviewResult.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerMonitorsPreviewResult.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerMonitorsPreviewResult" (int status)
        }

    ///<summary>
    ///List all region mappings in the user context.
    ///</summary>
    member this.UserLoadBalancerRegionsListRegions
        (?subdivisionCode: string, ?countryCode: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if subdivisionCode.IsSome then
                      RequestPart.query ("subdivision_code", subdivisionCode.Value)
                  if countryCode.IsSome then
                      RequestPart.query ("country_code", countryCode.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/load_balancers/regions" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserLoadBalancerRegionsListRegions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserLoadBalancerRegionsListRegions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserLoadBalancerRegionsListRegions" (int status)
        }

    ///<summary>
    ///List origin health changes.
    ///</summary>
    member this.LoadBalancerHealthcheckEventsListHealthcheckEvents
        (
            ?until: System.DateTimeOffset,
            ?poolName: string,
            ?originHealthy: bool,
            ?poolId: string,
            ?since: System.DateTimeOffset,
            ?originName: string,
            ?poolHealthy: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if until.IsSome then
                      RequestPart.query ("until", until.Value)
                  if poolName.IsSome then
                      RequestPart.query ("pool_name", poolName.Value)
                  if originHealthy.IsSome then
                      RequestPart.query ("origin_healthy", originHealthy.Value)
                  if poolId.IsSome then
                      RequestPart.query ("pool_id", poolId.Value)
                  if since.IsSome then
                      RequestPart.query ("since", since.Value)
                  if originName.IsSome then
                      RequestPart.query ("origin_name", originName.Value)
                  if poolHealthy.IsSome then
                      RequestPart.query ("pool_healthy", poolHealthy.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/load_balancing_analytics/events" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerHealthcheckEventsListHealthcheckEvents.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    LoadBalancerHealthcheckEventsListHealthcheckEvents.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for LoadBalancerHealthcheckEventsListHealthcheckEvents"
                        (int status)
        }

    ///<summary>
    ///Get a specific membership for the currently authenticated user.
    ///</summary>
    member this.UserMembershipsGet(membershipId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("membership_id", membershipId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/memberships/{membership_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserMembershipsGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserMembershipsGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserMembershipsGet" (int status)
        }

    ///<summary>
    ///Lists organizations the user is associated with.
    ///</summary>
    member this.User'SOrganizationsListOrganizations
        (
            ?name: string,
            ?page: float,
            ?perPage: float,
            ?order: string,
            ?direction: string,
            ?``match``: string,
            ?status: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if ``match``.IsSome then
                      RequestPart.query ("match", ``match``.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/organizations" requestParts cancellationToken

            match (int status) with
            | 200 -> return User'SOrganizationsListOrganizations.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return User'SOrganizationsListOrganizations.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for User'SOrganizationsListOrganizations" (int status)
        }

    ///<summary>
    ///Removes association to an organization.
    ///</summary>
    member this.User'SOrganizationsLeaveOrganization
        (organizationId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("organization_id", organizationId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/user/organizations/{organization_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return User'SOrganizationsLeaveOrganization.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return User'SOrganizationsLeaveOrganization.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for User'SOrganizationsLeaveOrganization" (int status)
        }

    ///<summary>
    ///Gets a specific organization the user is associated with.
    ///</summary>
    member this.User'SOrganizationsOrganizationDetails(organizationId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("organization_id", organizationId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/organizations/{organization_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return User'SOrganizationsOrganizationDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return User'SOrganizationsOrganizationDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for User'SOrganizationsOrganizationDetails" (int status)
        }

    ///<summary>
    ///Retrieves a list of total bandwidth by zone over a given time period.
    ///</summary>
    member this.SpectrumAnalyticsGetZonesReport
        (
            ?since: spectrum_u002D_analytics_since,
            ?until: spectrum_u002D_analytics_until,
            ?cdnTraffic: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if since.IsSome then
                      RequestPart.query ("since", since.Value)
                  if until.IsSome then
                      RequestPart.query ("until", until.Value)
                  if cdnTraffic.IsSome then
                      RequestPart.query ("cdn_traffic", cdnTraffic.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/spectrum_analytics/zones/report" requestParts cancellationToken

            match (int status) with
            | 200 -> return SpectrumAnalyticsGetZonesReport.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SpectrumAnalyticsGetZonesReport.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SpectrumAnalyticsGetZonesReport" (int status)
        }

    ///<summary>
    ///Lists all of a user's subscriptions.
    ///</summary>
    member this.UserSubscriptionGetUserSubscriptions(?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = []

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/subscriptions" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserSubscriptionGetUserSubscriptions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserSubscriptionGetUserSubscriptions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserSubscriptionGetUserSubscriptions" (int status)
        }

    ///<summary>
    ///Creates a user subscription.
    ///</summary>
    member this.UserSubscriptionCreateUserSubscription
        (body: bill_u002D_subs_u002D_api_subscription_u002D_v2, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/user/subscriptions" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserSubscriptionCreateUserSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserSubscriptionCreateUserSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserSubscriptionCreateUserSubscription" (int status)
        }

    ///<summary>
    ///Deletes a user's subscription.
    ///</summary>
    member this.UserSubscriptionDeleteUserSubscription
        (identifier: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/user/subscriptions/{identifier}" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserSubscriptionDeleteUserSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserSubscriptionDeleteUserSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserSubscriptionDeleteUserSubscription" (int status)
        }

    ///<summary>
    ///Updates a user's subscriptions.
    ///</summary>
    member this.UserSubscriptionUpdateUserSubscription
        (
            identifier: string,
            body: bill_u002D_subs_u002D_api_subscription_u002D_v2,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/user/subscriptions/{identifier}" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserSubscriptionUpdateUserSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserSubscriptionUpdateUserSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserSubscriptionUpdateUserSubscription" (int status)
        }

    ///<summary>
    ///Retrieves list of tenants the authenticated user / method has access to.
    ///</summary>
    member this.UserListUserTenants(?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = []
            let! (status, _, content) = OpenApiHttp.getAsync httpClient "/user/tenants" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserListUserTenants.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserListUserTenants.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserListUserTenants" (int status)
        }

    ///<summary>
    ///List all access tokens you created. Results include active, disabled, and recently-expired tokens when include_expired is set to true.
    ///</summary>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="direction"></param>
    ///<param name="includeExpired">When true, includes recently-expired tokens in the response.</param>
    ///<param name="cancellationToken"></param>
    member this.UserApiTokensListTokens
        (?page: float, ?perPage: float, ?direction: string, ?includeExpired: bool, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if includeExpired.IsSome then
                      RequestPart.query ("include_expired", includeExpired.Value) ]

            let! (status, _, content) = OpenApiHttp.getAsync httpClient "/user/tokens" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserApiTokensListTokens.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserApiTokensListTokens.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserApiTokensListTokens" (int status)
        }

    ///<summary>
    ///Create a new access token.
    ///</summary>
    member this.UserApiTokensCreateToken(body: iam_create_payload, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]
            let! (status, _, content) = OpenApiHttp.postAsync httpClient "/user/tokens" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserApiTokensCreateToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserApiTokensCreateToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserApiTokensCreateToken" (int status)
        }

    ///<summary>
    ///Find all available permission groups for API Tokens.
    ///</summary>
    ///<param name="name">
    ///Filter by the name of the permission group.
    ///The value must be URL-encoded.
    ///</param>
    ///<param name="scope">
    ///Filter by the scope of the permission group.
    ///The value must be URL-encoded.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.PermissionGroupsListPermissionGroups
        (?name: string, ?scope: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if scope.IsSome then
                      RequestPart.query ("scope", scope.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/tokens/permission_groups" requestParts cancellationToken

            match (int status) with
            | 200 -> return PermissionGroupsListPermissionGroups.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PermissionGroupsListPermissionGroups.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PermissionGroupsListPermissionGroups" (int status)
        }

    ///<summary>
    ///Test whether a token works.
    ///</summary>
    member this.UserApiTokensVerifyToken(?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = []

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/tokens/verify" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserApiTokensVerifyToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserApiTokensVerifyToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserApiTokensVerifyToken" (int status)
        }

    ///<summary>
    ///Destroy a token.
    ///</summary>
    member this.UserApiTokensDeleteToken
        (tokenId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("token_id", tokenId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/user/tokens/{token_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserApiTokensDeleteToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserApiTokensDeleteToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserApiTokensDeleteToken" (int status)
        }

    ///<summary>
    ///Get information about a specific token.
    ///</summary>
    member this.UserApiTokensTokenDetails(tokenId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("token_id", tokenId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/user/tokens/{token_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserApiTokensTokenDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserApiTokensTokenDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserApiTokensTokenDetails" (int status)
        }

    ///<summary>
    ///Update an existing token.
    ///</summary>
    member this.UserApiTokensUpdateToken(tokenId: string, body: iam_token_body, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("token_id", tokenId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/user/tokens/{token_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserApiTokensUpdateToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserApiTokensUpdateToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserApiTokensUpdateToken" (int status)
        }

    ///<summary>
    ///Roll the token secret.
    ///</summary>
    member this.UserApiTokensRollToken
        (tokenId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("token_id", tokenId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/user/tokens/{token_id}/value" requestParts cancellationToken

            match (int status) with
            | 200 -> return UserApiTokensRollToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UserApiTokensRollToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UserApiTokensRollToken" (int status)
        }

    ///<summary>
    ///Lists, searches, sorts, and filters your zones. Listing zones across more than 500 accounts
    ///is currently not allowed.
    ///</summary>
    member this.ZonesGet
        (
            ?name: string,
            ?status: string,
            ?``type``: list<string>,
            ?accountId: string,
            ?accountName: string,
            ?page: float,
            ?perPage: float,
            ?order: string,
            ?direction: string,
            ?``match``: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if ``type``.IsSome then
                      RequestPart.queryComma ("type", ``type``.Value)
                  if accountId.IsSome then
                      RequestPart.query ("account.id", accountId.Value)
                  if accountName.IsSome then
                      RequestPart.query ("account.name", accountName.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if ``match``.IsSome then
                      RequestPart.query ("match", ``match``.Value) ]

            let! (status, _, content) = OpenApiHttp.getAsync httpClient "/zones" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZonesGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZonesGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZonesGet" (int status)
        }

    ///<summary>
    ///Creates a new zone (domain) in your Cloudflare account.
    ///The zone is created in a pending state and must be activated by updating your domain's
    ///nameservers to point to Cloudflare, or by completing the verification process for partial
    ///(CNAME) setups.
    ///</summary>
    member this.ZonesPost(body: ZonesPostPayload, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]
            let! (status, _, content) = OpenApiHttp.postAsync httpClient "/zones" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZonesPost.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZonesPost.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZonesPost" (int status)
        }

    ///<summary>
    ///Deletes an existing zone.
    ///</summary>
    member this.Zones0Delete(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/zones/{zone_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return Zones0Delete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Zones0Delete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Zones0Delete" (int status)
        }

    ///<summary>
    ///Retrieves detailed information about a specific zone identified by its zone ID.
    ///Returns zone configuration, status, nameservers, and associated metadata.
    ///</summary>
    member this.Zones0Get(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return Zones0Get.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Zones0Get.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Zones0Get" (int status)
        }

    ///<summary>
    ///Edits a zone. Only one zone property can be changed at a time.
    ///</summary>
    member this.Zones0Patch(zoneId: string, body: Zones0PatchPayload, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return Zones0Patch.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Zones0Patch.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Zones0Patch" (int status)
        }

    ///<summary>
    ///Triggeres a new activation check for a PENDING Zone. This can be
    ///triggered every 5 min for paygo/ent customers, every hour for FREE
    ///Zones.
    ///</summary>
    ///<param name="zoneId">Zone ID</param>
    ///<param name="cancellationToken"></param>
    member this.PutZonesZoneIdActivationCheck(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/activation_check" requestParts cancellationToken

            match (int status) with
            | 200 -> return PutZonesZoneIdActivationCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PutZonesZoneIdActivationCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PutZonesZoneIdActivationCheck" (int status)
        }

    ///<summary>
    ///Lists available plans the zone can subscribe to.
    ///</summary>
    member this.ZoneRatePlanListAvailablePlans(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/available_plans" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneRatePlanListAvailablePlans.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneRatePlanListAvailablePlans.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneRatePlanListAvailablePlans" (int status)
        }

    ///<summary>
    ///Details of the available plan that the zone can subscribe to.
    ///</summary>
    member this.ZoneRatePlanAvailablePlanDetails
        (planIdentifier: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("plan_identifier", planIdentifier)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/available_plans/{plan_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneRatePlanAvailablePlanDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneRatePlanAvailablePlanDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneRatePlanAvailablePlanDetails" (int status)
        }

    ///<summary>
    ///Lists all rate plans the zone can subscribe to.
    ///</summary>
    member this.ZoneRatePlanListAvailableRatePlans(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/available_rate_plans" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneRatePlanListAvailableRatePlans.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneRatePlanListAvailableRatePlans.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneRatePlanListAvailableRatePlans" (int status)
        }

    ///<summary>
    ///Returns the list of entitlements (features and their allocations) for a given zone. Each entitlement describes a product feature the zone is permitted to use and the allocation value (boolean, count, range, enum, or string) that governs its behaviour.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="cancellationToken"></param>
    member this.EntitlementsGetZoneEntitlements(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/entitlements" requestParts cancellationToken

            match (int status) with
            | 200 -> return EntitlementsGetZoneEntitlements.OK((Serializer.deserialize content))
            | 204 -> return EntitlementsGetZoneEntitlements.NoContent
            | 400 -> return EntitlementsGetZoneEntitlements.BadRequest((Serializer.deserialize content))
            | 401 -> return EntitlementsGetZoneEntitlements.Unauthorized((Serializer.deserialize content))
            | 403 -> return EntitlementsGetZoneEntitlements.Forbidden((Serializer.deserialize content))
            | 404 -> return EntitlementsGetZoneEntitlements.NotFound((Serializer.deserialize content))
            | 405 -> return EntitlementsGetZoneEntitlements.MethodNotAllowed((Serializer.deserialize content))
            | 500 -> return EntitlementsGetZoneEntitlements.InternalServerError((Serializer.deserialize content))
            | 503 -> return EntitlementsGetZoneEntitlements.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EntitlementsGetZoneEntitlements" (int status)
        }

    ///<summary>
    ///Stop enforcement of a zone hold on the zone, permanently or temporarily, allowing the
    ///creation and activation of zones with this zone's hostname.
    ///Existing zone holds can be removed from CDN-only zones when `hold_after` is not provided.
    ///Active holds are automatically disabled when a zone transitions to CDN-only mode.
    ///</summary>
    ///<param name="zoneId">Zone ID</param>
    ///<param name="holdAfter">
    ///If `hold_after` is provided, the hold will be temporarily disabled,
    ///then automatically re-enabled by the system at the time specified
    ///in this RFC3339-formatted timestamp. Otherwise, the hold will be
    ///disabled indefinitely. `hold_after` cannot be provided for CDN-only zones.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.Zones0HoldDelete(zoneId: string, ?holdAfter: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if holdAfter.IsSome then
                      RequestPart.query ("hold_after", holdAfter.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/zones/{zone_id}/hold" requestParts cancellationToken

            match (int status) with
            | 200 -> return Zones0HoldDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Zones0HoldDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Zones0HoldDelete" (int status)
        }

    ///<summary>
    ///Retrieve whether the zone is subject to a zone hold, and metadata about the hold.
    ///</summary>
    ///<param name="zoneId">Zone ID</param>
    ///<param name="cancellationToken"></param>
    member this.Zones0HoldGet(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/hold" requestParts cancellationToken

            match (int status) with
            | 200 -> return Zones0HoldGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Zones0HoldGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Zones0HoldGet" (int status)
        }

    ///<summary>
    ///Update the `hold_after` and/or `include_subdomains` values on an existing zone hold.
    ///The hold is enabled if the `hold_after` date-time value is in the past.
    ///Existing zone holds can be removed from CDN-only zones by setting `hold_after` to `null`.
    ///Other zone hold updates cannot be made on CDN-only zones.
    ///Active holds are automatically disabled when a zone transitions to CDN-only mode.
    ///</summary>
    ///<param name="zoneId">Zone ID</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.Zones0HoldPatch(zoneId: string, body: Zones0HoldPatchPayload, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/hold" requestParts cancellationToken

            match (int status) with
            | 200 -> return Zones0HoldPatch.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Zones0HoldPatch.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Zones0HoldPatch" (int status)
        }

    ///<summary>
    ///Enforce a zone hold on the zone, blocking the creation and activation of zones with this zone's hostname.
    ///Zone holds cannot be enabled on CDN-only zones.
    ///</summary>
    ///<param name="zoneId">Zone ID</param>
    ///<param name="includeSubdomains">
    ///If provided, the zone hold will extend to block any subdomain of the given zone, as well
    ///as SSL4SaaS Custom Hostnames. For example, a zone hold on a zone with the hostname
    ///'example.com' and include_subdomains=true will block 'example.com',
    ///'staging.example.com', 'api.staging.example.com', etc.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.Zones0HoldPost(zoneId: string, ?includeSubdomains: bool, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if includeSubdomains.IsSome then
                      RequestPart.query ("include_subdomains", includeSubdomains.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/hold" requestParts cancellationToken

            match (int status) with
            | 200 -> return Zones0HoldPost.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Zones0HoldPost.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Zones0HoldPost" (int status)
        }

    ///<summary>
    ///Retrieve whether a given hostname is subject to a zone hold, and metadata about the hold.
    ///This endpoint checks whether the given hostname (or any of its ancestor domains) is blocked
    ///by an active zone hold. If a hold with `include_subdomains` is active on an ancestor domain,
    ///that hold is returned. This endpoint is used internally by SSL/COMS to check hold status
    ///during zone activation.
    ///</summary>
    ///<param name="zoneId">
    ///Zone identifier. Consumed by the API gateway for routing; the backend
    ///handler does not use this value directly.
    ///</param>
    ///<param name="zoneName">
    ///The hostname to check for a zone hold. May be a subdomain (e.g. `subdomain.example.com`)
    ///or an apex domain (e.g. `example.com`). The service checks the hostname and its ancestor
    ///domains for active holds with `include_subdomains` enabled.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.Zones0HoldZoneNameGet(zoneId: string, zoneName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("zone_name", zoneName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/hold/{zone_name}" requestParts cancellationToken

            match (int status) with
            | 200 -> return Zones0HoldZoneNameGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Zones0HoldZoneNameGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Zones0HoldZoneNameGet" (int status)
        }

    ///<summary>
    ///Available settings for your user in relation to a zone.
    ///</summary>
    member this.ZoneSettingsGetAllZoneSettings(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetAllZoneSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsGetAllZoneSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsGetAllZoneSettings" (int status)
        }

    ///<summary>
    ///Edit settings for a zone.
    ///</summary>
    member this.ZoneSettingsEditZoneSettingsInfo
        (zoneId: string, body: zones_multiple_settings, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/settings" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsEditZoneSettingsInfo.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsEditZoneSettingsInfo.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsEditZoneSettingsInfo" (int status)
        }

    ///<summary>
    ///Aegis provides dedicated egress IPs (from Cloudflare to your origin) for your layer 7 WAF and CDN services. The egress IPs are reserved exclusively for your account so that you can increase your origin security by only allowing traffic from a small list of IP addresses.
    ///</summary>
    member this.ZoneCacheSettingsGetAegisSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/aegis" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsGetAegisSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneCacheSettingsGetAegisSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneCacheSettingsGetAegisSetting" (int status)
        }

    ///<summary>
    ///Aegis provides dedicated egress IPs (from Cloudflare to your origin) for your layer 7 WAF and CDN services. The egress IPs are reserved exclusively for your account so that you can increase your origin security by only allowing traffic from a small list of IP addresses.
    ///</summary>
    member this.ZoneCacheSettingsChangeAegisSetting
        (zoneId: string, body: ZoneCacheSettingsChangeAegisSettingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/settings/aegis" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsChangeAegisSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneCacheSettingsChangeAegisSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneCacheSettingsChangeAegisSetting" (int status)
        }

    ///<summary>
    ///When enabled, Cloudflare automatically selects the preferred TLS key-exchange algorithm to use when establishing the TLS connection to the zone's origin, picking from the algorithms permitted by the zone's `origin_tls_compliance_modes` setting. When disabled, the default key-exchange ordering is used.
    ///</summary>
    member this.SslDetectorAutoOriginTlsKexGetEnrollment(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/auto_origin_tls_kex"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SslDetectorAutoOriginTlsKexGetEnrollment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SslDetectorAutoOriginTlsKexGetEnrollment.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for SslDetectorAutoOriginTlsKexGetEnrollment" (int status)
        }

    ///<summary>
    ///Enable or disable Auto-Origin TLS KEX selection for the zone by sending `{"enabled": true}` or `{"enabled": false}`. When enabled, Cloudflare runs a periodic scan of the zone's origins to determine the preferred key-exchange algorithm and writes that preference to the edge so it is sent first in the TLS ClientHello to the origin.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Update enablement of Auto-Origin TLS KEX selection.</param>
    ///<param name="cancellationToken"></param>
    member this.SslDetectorAutoOriginTlsKexPatchEnrollment
        (zoneId: string, body: cache_auto_origin_tls_kex_patch, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/auto_origin_tls_kex"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SslDetectorAutoOriginTlsKexPatchEnrollment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SslDetectorAutoOriginTlsKexPatchEnrollment.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for SslDetectorAutoOriginTlsKexPatchEnrollment" (int status)
        }

    ///<summary>
    ///Automatic Platform Optimization (APO) for WordPress is a performance feature that serves your
    ///WordPress site from Cloudflare's edge network, reducing load times for visitors.
    ///Refer to the APO documentation for more information.
    ///</summary>
    member this.ZoneSettingsGetAutomaticPlatformOptimizationSetting
        (zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/automatic_platform_optimization"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetAutomaticPlatformOptimizationSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneSettingsGetAutomaticPlatformOptimizationSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneSettingsGetAutomaticPlatformOptimizationSetting"
                        (int status)
        }

    ///<summary>
    ///Automatic Platform Optimization (APO) for WordPress is a performance feature that serves your
    ///WordPress site from Cloudflare's edge network, reducing load times for visitors.
    ///Refer to the APO documentation for more information.
    ///</summary>
    member this.ZoneSettingsChangeAutomaticPlatformOptimizationSetting
        (
            zoneId: string,
            body: ZoneSettingsChangeAutomaticPlatformOptimizationSettingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/automatic_platform_optimization"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsChangeAutomaticPlatformOptimizationSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneSettingsChangeAutomaticPlatformOptimizationSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneSettingsChangeAutomaticPlatformOptimizationSetting"
                        (int status)
        }

    ///<summary>
    ///Binary AST is a new binary encoding for JavaScript that enables faster parsing of scripts.
    ///When enabled, Cloudflare will serve a binary-encoded version of JavaScript to compatible browsers.
    ///</summary>
    member this.ZoneSettingsGetBinaryAstSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/binary_ast" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetBinaryAstSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsGetBinaryAstSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsGetBinaryAstSetting" (int status)
        }

    ///<summary>
    ///Binary AST is a new binary encoding for JavaScript that enables faster parsing of scripts.
    ///When enabled, Cloudflare will serve a binary-encoded version of JavaScript to compatible browsers.
    ///</summary>
    member this.ZoneSettingsChangeBinaryAstSetting
        (zoneId: string, body: ZoneSettingsChangeBinaryAstSettingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/settings/binary_ast" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsChangeBinaryAstSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsChangeBinaryAstSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsChangeBinaryAstSetting" (int status)
        }

    ///<summary>
    ///Retrieve the current CSAM Scanner configuration for a zone.
    ///The notification email is masked by default in responses.
    ///</summary>
    ///<param name="zoneId">Identifier for the zone.</param>
    ///<param name="cancellationToken"></param>
    member this.CsamScannerGetSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/csam_scanner_third_party"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CsamScannerGetSetting.OK((Serializer.deserialize content))
            | 400 -> return CsamScannerGetSetting.BadRequest((Serializer.deserialize content))
            | 401 -> return CsamScannerGetSetting.Unauthorized((Serializer.deserialize content))
            | 403 -> return CsamScannerGetSetting.Forbidden((Serializer.deserialize content))
            | 404 -> return CsamScannerGetSetting.NotFound((Serializer.deserialize content))
            | 500 -> return CsamScannerGetSetting.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CsamScannerGetSetting" (int status)
        }

    ///<summary>
    ///Update the CSAM Scanner configuration for a zone. Allows enabling or
    ///disabling CSAM scanning, updating the notification email, and
    ///configuring scanning sources.
    ///When a new email is provided, email verification is triggered
    ///automatically. The `enabled` field is a toggle; the server may
    ///adjust it based on whether the notification email is verified.
    ///Returns 403 if the zone or account is locked by Trust &amp; Safety.
    ///</summary>
    ///<param name="zoneId">Identifier for the zone.</param>
    ///<param name="body">Request body for updating CSAM Scanner configuration.</param>
    ///<param name="cancellationToken"></param>
    member this.CsamScannerUpdateSetting
        (
            zoneId: string,
            body: csam_u002D_config_u002D_service_csam_scanner_third_party_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/csam_scanner_third_party"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CsamScannerUpdateSetting.OK((Serializer.deserialize content))
            | 400 -> return CsamScannerUpdateSetting.BadRequest((Serializer.deserialize content))
            | 401 -> return CsamScannerUpdateSetting.Unauthorized((Serializer.deserialize content))
            | 403 -> return CsamScannerUpdateSetting.Forbidden((Serializer.deserialize content))
            | 404 -> return CsamScannerUpdateSetting.NotFound((Serializer.deserialize content))
            | 500 -> return CsamScannerUpdateSetting.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CsamScannerUpdateSetting" (int status)
        }

    ///<summary>
    ///Enhance your website's font delivery with Cloudflare Fonts. Deliver Google Hosted fonts from your own domain,
    ///boost performance, and enhance user privacy. Refer to the Cloudflare Fonts documentation for more information.
    ///</summary>
    member this.ZoneSettingsGetFontsSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/fonts" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetFontsSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsGetFontsSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsGetFontsSetting" (int status)
        }

    ///<summary>
    ///Enhance your website's font delivery with Cloudflare Fonts. Deliver Google Hosted fonts from your own domain,
    ///boost performance, and enhance user privacy. Refer to the Cloudflare Fonts documentation for more information.
    ///</summary>
    member this.ZoneSettingsChangeFontsSetting
        (zoneId: string, body: ZoneSettingsChangeFontsSettingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/settings/fonts" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsChangeFontsSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsChangeFontsSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsChangeFontsSetting" (int status)
        }

    ///<summary>
    ///Gets the Google Tag Gateway configuration for a zone.
    ///</summary>
    member this.ZoneSettingsGetGoogleTagGatewayConfig(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/google-tag-gateway/config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetGoogleTagGatewayConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsGetGoogleTagGatewayConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsGetGoogleTagGatewayConfig" (int status)
        }

    ///<summary>
    ///Updates the Google Tag Gateway configuration for a zone.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Google Tag Gateway configuration for a zone.</param>
    ///<param name="cancellationToken"></param>
    member this.ZoneSettingsChangeGoogleTagGatewayConfig
        (
            zoneId: string,
            body: google_u002D_tag_u002D_gateway_google_u002D_tag_u002D_gateway_u002D_config,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/settings/google-tag-gateway/config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsChangeGoogleTagGatewayConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsChangeGoogleTagGatewayConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for ZoneSettingsChangeGoogleTagGatewayConfig" (int status)
        }

    ///<summary>
    ///HTTP/2 Prioritization controls the order in which assets are delivered to browsers.
    ///Cloudflare's HTTP/2 Prioritization overrides the default browser prioritization order,
    ///improving the loading performance of web pages.
    ///</summary>
    member this.ZoneSettingsGetH2PrioritizationSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/h2_prioritization"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetH2PrioritizationSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsGetH2PrioritizationSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsGetH2PrioritizationSetting" (int status)
        }

    ///<summary>
    ///HTTP/2 Prioritization controls the order in which assets are delivered to browsers.
    ///Cloudflare's HTTP/2 Prioritization overrides the default browser prioritization order,
    ///improving the loading performance of web pages.
    ///</summary>
    member this.ZoneSettingsChangeH2PrioritizationSetting
        (zoneId: string, body: ZoneSettingsChangeH2PrioritizationSettingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/h2_prioritization"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsChangeH2PrioritizationSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsChangeH2PrioritizationSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for ZoneSettingsChangeH2PrioritizationSetting" (int status)
        }

    ///<summary>
    ///Gets the Image Transformations setting for a zone. Accepted values are `off`, `on`, `open`, and `latest`.
    ///</summary>
    member this.ZoneSettingsGetImageResizingSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/image_resizing"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetImageResizingSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsGetImageResizingSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsGetImageResizingSetting" (int status)
        }

    ///<summary>
    ///Sets the Image Transformations setting for a zone. Accepted values are `off`, `on`, `open`, and `latest`.
    ///</summary>
    member this.ZoneSettingsChangeImageResizingSetting
        (zoneId: string, body: ZoneSettingsChangeImageResizingSettingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/image_resizing"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsChangeImageResizingSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsChangeImageResizingSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsChangeImageResizingSetting" (int status)
        }

    ///<summary>
    ///Fetches the Network Error Logging (NEL) setting for a zone. NEL allows browsers to report network errors to a configured endpoint. The setting is enabled by default for free and pro zones, and disabled by default for business and enterprise zones unless the NEL product feature is enabled.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="cancellationToken"></param>
    member this.NelSettingsGet(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/nel" requestParts cancellationToken

            match (int status) with
            | 200 -> return NelSettingsGet.OK((Serializer.deserialize content))
            | 401 -> return NelSettingsGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return NelSettingsGet.Forbidden((Serializer.deserialize content))
            | 500 -> return NelSettingsGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NelSettingsGet" (int status)
        }

    ///<summary>
    ///Updates the Network Error Logging (NEL) setting for a zone. Requires the NEL product feature to be enabled for the zone. The setting controls whether browsers report network errors to Cloudflare's NEL endpoint.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="body">Request body for updating the NEL setting.</param>
    ///<param name="cancellationToken"></param>
    member this.NelSettingsEdit
        (zoneId: string, body: nel_u002D_config_nel_setting_patch, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/settings/nel" requestParts cancellationToken

            match (int status) with
            | 200 -> return NelSettingsEdit.OK((Serializer.deserialize content))
            | 400 -> return NelSettingsEdit.BadRequest((Serializer.deserialize content))
            | 401 -> return NelSettingsEdit.Unauthorized((Serializer.deserialize content))
            | 403 -> return NelSettingsEdit.Forbidden((Serializer.deserialize content))
            | 500 -> return NelSettingsEdit.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NelSettingsEdit" (int status)
        }

    ///<summary>
    ///Origin H2 Max Streams configures the max number of concurrent requests that Cloudflare will send within the same connection when communicating with the origin server, if the origin supports it. Note that if your origin does not support H2 multiplexing, 5xx errors may be observed, particularly 520s. Also note that the default value is `100` for all plan types except Enterprise where it is `1`. `1` means that H2 multiplexing is disabled.
    ///</summary>
    member this.ZoneCacheSettingsGetOriginH2MaxStreamsSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/origin_h2_max_streams"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsGetOriginH2MaxStreamsSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsGetOriginH2MaxStreamsSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for ZoneCacheSettingsGetOriginH2MaxStreamsSetting" (int status)
        }

    ///<summary>
    ///Origin H2 Max Streams configures the max number of concurrent requests that Cloudflare will send within the same connection when communicating with the origin server, if the origin supports it. Note that if your origin does not support H2 multiplexing, 5xx errors may be observed, particularly 520s. Also note that the default value is `100` for all plan types except Enterprise where it is `1`. `1` means that H2 multiplexing is disabled.
    ///</summary>
    member this.ZoneCacheSettingsChangeOriginH2MaxStreamsSetting
        (
            zoneId: string,
            body: ZoneCacheSettingsChangeOriginH2MaxStreamsSettingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/origin_h2_max_streams"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsChangeOriginH2MaxStreamsSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsChangeOriginH2MaxStreamsSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneCacheSettingsChangeOriginH2MaxStreamsSetting"
                        (int status)
        }

    ///<summary>
    ///Origin Max HTTP Setting Version sets the highest HTTP version Cloudflare will attempt to use with your origin. This setting allows Cloudflare to make HTTP/2 requests to your origin. (Refer to [Enable HTTP/2 to Origin](https://developers.cloudflare.com/cache/how-to/enable-http2-to-origin/), for more information.). The default value is "2" for all plan types except Enterprise where it is "1".
    ///</summary>
    member this.ZoneCacheSettingsGetOriginMaxHttpVersionSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/origin_max_http_version"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsGetOriginMaxHttpVersionSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsGetOriginMaxHttpVersionSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneCacheSettingsGetOriginMaxHttpVersionSetting"
                        (int status)
        }

    ///<summary>
    ///Origin Max HTTP Setting Version sets the highest HTTP version Cloudflare will attempt to use with your origin. This setting allows Cloudflare to make HTTP/2 requests to your origin. (Refer to [Enable HTTP/2 to Origin](https://developers.cloudflare.com/cache/how-to/enable-http2-to-origin/), for more information.). The default value is "2" for all plan types except Enterprise where it is "1".
    ///</summary>
    member this.ZoneCacheSettingsChangeOriginMaxHttpVersionSetting
        (
            zoneId: string,
            body: ZoneCacheSettingsChangeOriginMaxHttpVersionSettingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/origin_max_http_version"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsChangeOriginMaxHttpVersionSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsChangeOriginMaxHttpVersionSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneCacheSettingsChangeOriginMaxHttpVersionSetting"
                        (int status)
        }

    ///<summary>
    ///Delete the Origin TLS Compliance Modes setting for the zone, removing any configured compliance constraint. After deletion, Cloudflare's default behavior applies (no compliance filtering of the key-exchange algorithm list sent to the origin).
    ///</summary>
    member this.ZoneCacheSettingsDeleteOriginTlsComplianceModesSetting
        (zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/settings/origin_tls_compliance_modes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsDeleteOriginTlsComplianceModesSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsDeleteOriginTlsComplianceModesSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneCacheSettingsDeleteOriginTlsComplianceModesSetting"
                        (int status)
        }

    ///<summary>
    ///Origin TLS Compliance Modes constrains the set of TLS key-exchange algorithms Cloudflare may use when establishing the TLS connection to the zone's origin. The value is a list of named compliance modes (currently `fips` and `pqh`). Multiple modes are combined as the intersection of their permitted algorithm lists. An empty list (or no rule configured) means no compliance constraint is applied.
    ///</summary>
    member this.ZoneCacheSettingsGetOriginTlsComplianceModesSetting
        (zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/origin_tls_compliance_modes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsGetOriginTlsComplianceModesSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsGetOriginTlsComplianceModesSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneCacheSettingsGetOriginTlsComplianceModesSetting"
                        (int status)
        }

    ///<summary>
    ///Update the set of TLS compliance modes for the zone. PATCH performs a full replace of the modes list, not a merge — the request body is treated as the complete new list, and any modes not present in it are removed. (To remove a single mode from an existing configuration, send the updated list without it.) The request body must be of the form `{"value": ["fips", "pqh"]}`. Currently supported modes are `fips` and `pqh`; an empty list clears the constraint. Future modes (e.g. `cnsa2`) may be added; clients should treat unknown values as opaque strings. Invalid mode values are rejected with a 4xx response.
    ///</summary>
    member this.ZoneCacheSettingsChangeOriginTlsComplianceModesSetting
        (
            zoneId: string,
            body: ZoneCacheSettingsChangeOriginTlsComplianceModesSettingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/origin_tls_compliance_modes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsChangeOriginTlsComplianceModesSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsChangeOriginTlsComplianceModesSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneCacheSettingsChangeOriginTlsComplianceModesSetting"
                        (int status)
        }

    ///<summary>
    ///Replace the entire set of TLS compliance modes for the zone with the list provided in the request body. PUT performs a full replace, not a merge — any modes not present in the request body are removed. The request body must be of the form `{"value": ["fips", "pqh"]}`. Currently supported modes are `fips` and `pqh`; an empty list clears the constraint. Future modes (e.g. `cnsa2`) may be added; clients should treat unknown values as opaque strings. Invalid mode values are rejected with a 4xx response.
    ///</summary>
    member this.ZoneCacheSettingsReplaceOriginTlsComplianceModesSetting
        (
            zoneId: string,
            body: ZoneCacheSettingsReplaceOriginTlsComplianceModesSettingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/settings/origin_tls_compliance_modes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsReplaceOriginTlsComplianceModesSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsReplaceOriginTlsComplianceModesSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneCacheSettingsReplaceOriginTlsComplianceModesSetting"
                        (int status)
        }

    ///<summary>
    ///Retrieves RUM status for a zone.
    ///</summary>
    member this.WebAnalyticsGetRumStatus(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/rum" requestParts cancellationToken

            match (int status) with
            | 200 -> return WebAnalyticsGetRumStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WebAnalyticsGetRumStatus.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WebAnalyticsGetRumStatus" (int status)
        }

    ///<summary>
    ///Toggles RUM on/off for an existing zone.
    ///</summary>
    member this.WebAnalyticsToggleRum
        (zoneId: string, body: rum_toggle_u002D_rum_u002D_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/settings/rum" requestParts cancellationToken

            match (int status) with
            | 200 -> return WebAnalyticsToggleRum.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WebAnalyticsToggleRum.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WebAnalyticsToggleRum" (int status)
        }

    ///<summary>
    ///Speed Brain lets compatible browsers speculate on content which can be prefetched or preloaded, making website
    ///navigation faster. Refer to the Cloudflare Speed Brain documentation for more information.
    ///</summary>
    member this.ZoneSettingsGetSpeedBrainSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/speed_brain" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetSpeedBrainSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsGetSpeedBrainSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsGetSpeedBrainSetting" (int status)
        }

    ///<summary>
    ///Speed Brain lets compatible browsers speculate on content which can be prefetched or preloaded, making website
    ///navigation faster. Refer to the Cloudflare Speed Brain documentation for more information.
    ///</summary>
    member this.ZoneSettingsChangeSpeedBrainSetting
        (zoneId: string, body: ZoneSettingsChangeSpeedBrainSettingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/settings/speed_brain" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsChangeSpeedBrainSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsChangeSpeedBrainSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsChangeSpeedBrainSetting" (int status)
        }

    ///<summary>
    ///If the system is enabled, the response will include next_scheduled_scan, representing the next time this zone will be scanned and the zone's ssl/tls encryption mode is potentially upgraded by the system. If the system is disabled, next_scheduled_scan will not be present in the response body.
    ///</summary>
    member this.SslDetectorAutomaticModeGetEnrollment(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/ssl_automatic_mode"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SslDetectorAutomaticModeGetEnrollment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SslDetectorAutomaticModeGetEnrollment.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SslDetectorAutomaticModeGetEnrollment" (int status)
        }

    ///<summary>
    ///The automatic system is enabled when this endpoint is hit with value in the request body is set to "auto", and disabled when the request body value is set to "custom".
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Update enablement of Automatic SSL/TLS.</param>
    ///<param name="cancellationToken"></param>
    member this.SslDetectorAutomaticModePatchEnrollment
        (zoneId: string, body: cache_schemas_patch, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/ssl_automatic_mode"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SslDetectorAutomaticModePatchEnrollment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SslDetectorAutomaticModePatchEnrollment.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SslDetectorAutomaticModePatchEnrollment" (int status)
        }

    ///<summary>
    ///Media Transformations Allowed Origins restricts transformations for images and video served through
    ///Cloudflare's network to requests originating from specified domains. Refer to the
    ///Image Transformations and Video Transformations documentation for more information.
    ///</summary>
    member this.ZoneSettingsGetTransformationsAllowedOriginsSetting
        (zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/transformations_allowed_origins"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetTransformationsAllowedOriginsSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneSettingsGetTransformationsAllowedOriginsSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneSettingsGetTransformationsAllowedOriginsSetting"
                        (int status)
        }

    ///<summary>
    ///Media Transformations Allowed Origins restricts transformations for images and video served through
    ///Cloudflare's network to requests originating from specified domains. Refer to the
    ///Image Transformations and Video Transformations documentation for more information.
    ///</summary>
    member this.ZoneSettingsChangeTransformationsAllowedOriginsSetting
        (
            zoneId: string,
            body: ZoneSettingsChangeTransformationsAllowedOriginsSettingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/transformations_allowed_origins"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsChangeTransformationsAllowedOriginsSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneSettingsChangeTransformationsAllowedOriginsSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneSettingsChangeTransformationsAllowedOriginsSetting"
                        (int status)
        }

    ///<summary>
    ///C2PA (Coalition for Content Provenance and Authenticity) signing adds cryptographic metadata
    ///to images processed through Cloudflare Image Transformations, enabling verification of image
    ///authenticity and provenance.
    ///</summary>
    member this.ZoneSettingsGetTransformationsC2paSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/transformations_c2pa"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetTransformationsC2paSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsGetTransformationsC2paSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for ZoneSettingsGetTransformationsC2paSetting" (int status)
        }

    ///<summary>
    ///C2PA (Coalition for Content Provenance and Authenticity) signing adds cryptographic metadata
    ///to images processed through Cloudflare Image Transformations, enabling verification of image
    ///authenticity and provenance.
    ///</summary>
    member this.ZoneSettingsChangeTransformationsC2paSetting
        (
            zoneId: string,
            body: ZoneSettingsChangeTransformationsC2paSettingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/transformations_c2pa"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsChangeTransformationsC2paSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneSettingsChangeTransformationsC2paSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for ZoneSettingsChangeTransformationsC2paSetting" (int status)
        }

    ///<summary>
    ///Returns the combined Transformations configuration for a zone, including the transformations
    ///toggle, allowed origins, and C2PA signing settings in a single response.
    ///</summary>
    member this.ZoneSettingsGetTransformationsConfig(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/transformations_config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetTransformationsConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsGetTransformationsConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsGetTransformationsConfig" (int status)
        }

    ///<summary>
    ///Updates one or more fields of the combined Transformations configuration for a zone.
    ///Omitted fields are left unchanged. The response always returns the full current state
    ///of all three sub-settings.
    ///</summary>
    member this.ZoneSettingsChangeTransformationsConfig
        (zoneId: string, body: ZoneSettingsChangeTransformationsConfigPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/transformations_config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsChangeTransformationsConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsChangeTransformationsConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsChangeTransformationsConfig" (int status)
        }

    ///<summary>
    ///Gets latest Zaraz configuration for a zone. It can be preview or published configuration, whichever was the last updated. Secret variables values will not be included.
    ///</summary>
    member this.GetZonesZoneIdentifierZarazConfig(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/zaraz/config" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetZonesZoneIdentifierZarazConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetZonesZoneIdentifierZarazConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetZonesZoneIdentifierZarazConfig" (int status)
        }

    ///<summary>
    ///Updates Zaraz configuration for a zone.
    ///</summary>
    member this.PutZonesZoneIdentifierZarazConfig
        (zoneId: string, body: zaraz_zaraz_u002D_config_u002D_body, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/settings/zaraz/config" requestParts cancellationToken

            match (int status) with
            | 200 -> return PutZonesZoneIdentifierZarazConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PutZonesZoneIdentifierZarazConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PutZonesZoneIdentifierZarazConfig" (int status)
        }

    ///<summary>
    ///Gets default Zaraz configuration for a zone.
    ///</summary>
    member this.GetZonesZoneIdentifierZarazDefault(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/zaraz/default" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetZonesZoneIdentifierZarazDefault.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetZonesZoneIdentifierZarazDefault.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetZonesZoneIdentifierZarazDefault" (int status)
        }

    ///<summary>
    ///Exports full current published Zaraz configuration for a zone, secret variables included.
    ///</summary>
    member this.GetZonesZoneIdentifierZarazExport(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/zaraz/export" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetZonesZoneIdentifierZarazExport.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetZonesZoneIdentifierZarazExport.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetZonesZoneIdentifierZarazExport" (int status)
        }

    ///<summary>
    ///Lists a history of published Zaraz configuration records for a zone.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="offset">Ordinal number to start listing the results with. Default value is 0.</param>
    ///<param name="limit">Maximum amount of results to list. Default value is 10.</param>
    ///<param name="sortField">The field to sort by. Default is updated_at.</param>
    ///<param name="sortOrder">Sorting order. Default is DESC.</param>
    ///<param name="cancellationToken"></param>
    member this.GetZonesZoneIdentifierZarazHistory
        (
            zoneId: string,
            ?offset: int,
            ?limit: int,
            ?sortField: string,
            ?sortOrder: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if sortField.IsSome then
                      RequestPart.query ("sortField", sortField.Value)
                  if sortOrder.IsSome then
                      RequestPart.query ("sortOrder", sortOrder.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/zaraz/history" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetZonesZoneIdentifierZarazHistory.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetZonesZoneIdentifierZarazHistory.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetZonesZoneIdentifierZarazHistory" (int status)
        }

    ///<summary>
    ///Restores a historical published Zaraz configuration by ID for a zone.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">ID of the Zaraz configuration to restore.</param>
    ///<param name="cancellationToken"></param>
    member this.PutZonesZoneIdentifierZarazHistory(zoneId: string, body: int, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/settings/zaraz/history" requestParts cancellationToken

            match (int status) with
            | 200 -> return PutZonesZoneIdentifierZarazHistory.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PutZonesZoneIdentifierZarazHistory.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PutZonesZoneIdentifierZarazHistory" (int status)
        }

    ///<summary>
    ///Gets a history of published Zaraz configurations by ID(s) for a zone.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="ids">Comma separated list of Zaraz configuration IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.GetZonesZoneIdentifierZarazConfigHistory
        (zoneId: string, ids: list<int>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.queryComma ("ids", ids) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/zaraz/history/configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetZonesZoneIdentifierZarazConfigHistory.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetZonesZoneIdentifierZarazConfigHistory.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for GetZonesZoneIdentifierZarazConfigHistory" (int status)
        }

    ///<summary>
    ///Publish current Zaraz preview configuration for a zone.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="body">Zaraz configuration description.</param>
    member this.PostZonesZoneIdentifierZarazPublish
        (zoneId: string, ?cancellationToken: CancellationToken, ?body: string)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/settings/zaraz/publish"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostZonesZoneIdentifierZarazPublish.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PostZonesZoneIdentifierZarazPublish.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostZonesZoneIdentifierZarazPublish" (int status)
        }

    ///<summary>
    ///Gets Zaraz workflow for a zone.
    ///</summary>
    member this.GetZonesZoneIdentifierZarazWorkflow(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/settings/zaraz/workflow"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetZonesZoneIdentifierZarazWorkflow.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetZonesZoneIdentifierZarazWorkflow.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetZonesZoneIdentifierZarazWorkflow" (int status)
        }

    ///<summary>
    ///Updates Zaraz workflow for a zone.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Zaraz workflow.</param>
    ///<param name="cancellationToken"></param>
    member this.PutZonesZoneIdentifierZarazWorkflow
        (zoneId: string, body: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/settings/zaraz/workflow"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PutZonesZoneIdentifierZarazWorkflow.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PutZonesZoneIdentifierZarazWorkflow.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PutZonesZoneIdentifierZarazWorkflow" (int status)
        }

    ///<summary>
    ///Fetch a single zone setting by name
    ///</summary>
    member this.ZoneSettingsGetSingleSetting(zoneId: string, settingId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("setting_id", settingId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/settings/{setting_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsGetSingleSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsGetSingleSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsGetSingleSetting" (int status)
        }

    ///<summary>
    ///Updates a single zone setting by the identifier
    ///</summary>
    member this.ZoneSettingsEditSingleSetting
        (
            zoneId: string,
            settingId: string,
            body: zones_zone_settings_single_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("setting_id", settingId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/settings/{setting_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneSettingsEditSingleSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSettingsEditSingleSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSettingsEditSingleSetting" (int status)
        }

    ///<summary>
    ///Deletes a zone's subscription.
    ///</summary>
    member this.ZoneSubscriptionDeleteZoneSubscription(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/zones/{zone_id}/subscription" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSubscriptionDeleteZoneSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSubscriptionDeleteZoneSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSubscriptionDeleteZoneSubscription" (int status)
        }

    ///<summary>
    ///Lists zone subscription details.
    ///</summary>
    member this.ZoneSubscriptionZoneSubscriptionDetails(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/subscription" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSubscriptionZoneSubscriptionDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSubscriptionZoneSubscriptionDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSubscriptionZoneSubscriptionDetails" (int status)
        }

    ///<summary>
    ///Create a zone subscription, either plan or add-ons.
    ///</summary>
    member this.ZoneSubscriptionCreateZoneSubscription
        (zoneId: string, body: bill_u002D_subs_u002D_api_subscription_u002D_v2, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/subscription" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSubscriptionCreateZoneSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSubscriptionCreateZoneSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSubscriptionCreateZoneSubscription" (int status)
        }

    ///<summary>
    ///Updates zone subscriptions, either plan or add-ons.
    ///</summary>
    member this.ZoneSubscriptionUpdateZoneSubscription
        (zoneId: string, body: bill_u002D_subs_u002D_api_subscription_u002D_v2, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/subscription" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSubscriptionUpdateZoneSubscription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSubscriptionUpdateZoneSubscription.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSubscriptionUpdateZoneSubscription" (int status)
        }

    ///<summary>
    ///Deletes a zone's subscription. Retained for audit-log coverage. Use the singular `/zones/{zone_id}/subscription` path instead.
    ///</summary>
    member this.ZoneSubscriptionDeleteZoneSubscriptions(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/zones/{zone_id}/subscriptions" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSubscriptionDeleteZoneSubscriptions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSubscriptionDeleteZoneSubscriptions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSubscriptionDeleteZoneSubscriptions" (int status)
        }

    ///<summary>
    ///Create a zone subscription, either plan or add-ons. Retained for audit-log coverage. Use the singular `/zones/{zone_id}/subscription` path instead.
    ///</summary>
    member this.ZoneSubscriptionCreateZoneSubscriptions
        (zoneId: string, body: bill_u002D_subs_u002D_api_subscription_u002D_v2, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/subscriptions" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSubscriptionCreateZoneSubscriptions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSubscriptionCreateZoneSubscriptions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSubscriptionCreateZoneSubscriptions" (int status)
        }

    ///<summary>
    ///Updates zone subscriptions, either plan or add-ons. Retained for audit-log coverage. Use the singular `/zones/{zone_id}/subscription` path instead.
    ///</summary>
    member this.ZoneSubscriptionUpdateZoneSubscriptions
        (zoneId: string, body: bill_u002D_subs_u002D_api_subscription_u002D_v2, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/subscriptions" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneSubscriptionUpdateZoneSubscriptions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneSubscriptionUpdateZoneSubscriptions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneSubscriptionUpdateZoneSubscriptions" (int status)
        }

    ///<summary>
    ///Removes all tags from a specific zone-level resource.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Request body schema for deleting tags from zone-level resources.</param>
    ///<param name="ifMatch">
    ///ETag value for optimistic concurrency control. When provided, the server will
    ///verify the current resource ETag matches before applying the write. Returns
    ///412 Precondition Failed if the resource has been modified since the ETag was
    ///obtained. Omit this header for unconditional writes.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.TagsZoneDelete
        (
            zoneId: resource_u002D_tagging_zone_id,
            body: resource_u002D_tagging_delete_tags_request_zone_level,
            ?ifMatch: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body
                  if ifMatch.IsSome then
                      RequestPart.header ("If-Match", ifMatch.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/zones/{zone_id}/tags" requestParts cancellationToken

            match (int status) with
            | 204 -> return TagsZoneDelete.NoContent
            | 412 -> return TagsZoneDelete.PreconditionFailed((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TagsZoneDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return TagsZoneDelete.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TagsZoneDelete" (int status)
        }

    ///<summary>
    ///Retrieves tags for a specific zone-level resource.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="resourceId">The ID of the resource to retrieve tags for.</param>
    ///<param name="resourceType">The type of the resource.</param>
    ///<param name="accessApplicationId">Access application ID identifier. Required for access_application_policy resources.</param>
    ///<param name="cancellationToken"></param>
    member this.TagsZoneGet
        (
            zoneId: resource_u002D_tagging_zone_id,
            resourceId: string,
            resourceType: string,
            ?accessApplicationId: System.Guid,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.query ("resource_id", resourceId)
                  RequestPart.query ("resource_type", resourceType)
                  if accessApplicationId.IsSome then
                      RequestPart.query ("access_application_id", accessApplicationId.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/tags" requestParts cancellationToken

            match (int status) with
            | 200 -> return TagsZoneGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TagsZoneGet.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return TagsZoneGet.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TagsZoneGet" (int status)
        }

    ///<summary>
    ///Creates or updates tags for a specific zone-level resource. Replaces all existing tags for the resource.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Request body schema for setting tags on zone-level resources.</param>
    ///<param name="ifMatch">
    ///ETag value for optimistic concurrency control. When provided, the server will
    ///verify the current resource ETag matches before applying the write. Returns
    ///412 Precondition Failed if the resource has been modified since the ETag was
    ///obtained. Omit this header for unconditional writes.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.TagsZoneSet
        (
            zoneId: resource_u002D_tagging_zone_id,
            body: resource_u002D_tagging_set_tags_request_zone_level,
            ?ifMatch: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body
                  if ifMatch.IsSome then
                      RequestPart.header ("If-Match", ifMatch.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/tags" requestParts cancellationToken

            match (int status) with
            | 200 -> return TagsZoneSet.OK((Serializer.deserialize content))
            | 412 -> return TagsZoneSet.PreconditionFailed((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TagsZoneSet.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return TagsZoneSet.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TagsZoneSet" (int status)
        }
