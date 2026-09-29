namespace rec FSharp.CloudEdge.Management.Messaging

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
type MessagingClient(httpClient: HttpClient) =
    ///<summary>
    ///Gets a list of all alert types for which an account is eligible.
    ///</summary>
    member this.NotificationAlertTypesGetAlertTypes(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/available_alerts"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationAlertTypesGetAlertTypes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationAlertTypesGetAlertTypes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationAlertTypesGetAlertTypes" (int status)
        }

    ///<summary>
    ///Get a list of all delivery mechanism types for which an account is eligible.
    ///</summary>
    member this.NotificationMechanismEligibilityGetDeliveryMechanismEligibility
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/destinations/eligible"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    NotificationMechanismEligibilityGetDeliveryMechanismEligibility.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationMechanismEligibilityGetDeliveryMechanismEligibility.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for NotificationMechanismEligibilityGetDeliveryMechanismEligibility"
                        (int status)
        }

    ///<summary>
    ///Deletes all the PagerDuty Services connected to the account.
    ///</summary>
    member this.NotificationDestinationsWithPagerDutyDeletePagerDutyServices
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/destinations/pagerduty"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return NotificationDestinationsWithPagerDutyDeletePagerDutyServices.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationDestinationsWithPagerDutyDeletePagerDutyServices.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for NotificationDestinationsWithPagerDutyDeletePagerDutyServices"
                        (int status)
        }

    ///<summary>
    ///Get a list of all configured PagerDuty services.
    ///</summary>
    member this.NotificationDestinationsWithPagerDutyListPagerDutyServices
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/destinations/pagerduty"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return NotificationDestinationsWithPagerDutyListPagerDutyServices.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationDestinationsWithPagerDutyListPagerDutyServices.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for NotificationDestinationsWithPagerDutyListPagerDutyServices"
                        (int status)
        }

    ///<summary>
    ///Creates a new token for integrating with PagerDuty.
    ///</summary>
    member this.NotificationDestinationsWithPagerDutyConnectPagerDuty
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/destinations/pagerduty/connect"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 ->
                return NotificationDestinationsWithPagerDutyConnectPagerDuty.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationDestinationsWithPagerDutyConnectPagerDuty.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for NotificationDestinationsWithPagerDutyConnectPagerDuty"
                        (int status)
        }

    ///<summary>
    ///Links PagerDuty with the account using the integration token.
    ///</summary>
    member this.NotificationDestinationsWithPagerDutyConnectPagerDutyToken
        (accountId: string, tokenId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("token_id", tokenId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/destinations/pagerduty/connect/{token_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return NotificationDestinationsWithPagerDutyConnectPagerDutyToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationDestinationsWithPagerDutyConnectPagerDutyToken.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for NotificationDestinationsWithPagerDutyConnectPagerDutyToken"
                        (int status)
        }

    ///<summary>
    ///Gets a list of all configured webhook destinations.
    ///</summary>
    member this.NotificationWebhooksListWebhooks(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/destinations/webhooks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationWebhooksListWebhooks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationWebhooksListWebhooks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationWebhooksListWebhooks" (int status)
        }

    ///<summary>
    ///Creates a new webhook destination.
    ///</summary>
    member this.NotificationWebhooksCreateAWebhook
        (accountId: string, body: NotificationWebhooksCreateAWebhookPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/destinations/webhooks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return NotificationWebhooksCreateAWebhook.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationWebhooksCreateAWebhook.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationWebhooksCreateAWebhook" (int status)
        }

    ///<summary>
    ///Delete a configured webhook destination.
    ///</summary>
    member this.NotificationWebhooksDeleteAWebhook
        (webhookId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("webhook_id", webhookId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/destinations/webhooks/{webhook_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationWebhooksDeleteAWebhook.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationWebhooksDeleteAWebhook.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationWebhooksDeleteAWebhook" (int status)
        }

    ///<summary>
    ///Get details for a single webhooks destination.
    ///</summary>
    member this.NotificationWebhooksGetAWebhook
        (accountId: string, webhookId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("webhook_id", webhookId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/destinations/webhooks/{webhook_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationWebhooksGetAWebhook.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationWebhooksGetAWebhook.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationWebhooksGetAWebhook" (int status)
        }

    ///<summary>
    ///Update a webhook destination.
    ///</summary>
    member this.NotificationWebhooksUpdateAWebhook
        (
            webhookId: string,
            accountId: string,
            body: NotificationWebhooksUpdateAWebhookPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("webhook_id", webhookId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/destinations/webhooks/{webhook_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationWebhooksUpdateAWebhook.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationWebhooksUpdateAWebhook.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationWebhooksUpdateAWebhook" (int status)
        }

    ///<summary>
    ///Gets a list of history records for notifications sent to an account. The records are displayed for last `x` number of days based on the zone plan (free = 30, pro = 30, biz = 30, ent = 90).
    ///</summary>
    member this.NotificationHistoryListHistory
        (
            accountId: string,
            ?perPage: float,
            ?before: System.DateTimeOffset,
            ?page: float,
            ?since: System.DateTimeOffset,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if before.IsSome then
                      RequestPart.query ("before", before.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if since.IsSome then
                      RequestPart.query ("since", since.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/history"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationHistoryListHistory.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationHistoryListHistory.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationHistoryListHistory" (int status)
        }

    ///<summary>
    ///Get a list of all Notification policies.
    ///</summary>
    member this.NotificationPoliciesListNotificationPolicies(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/policies"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationPoliciesListNotificationPolicies.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationPoliciesListNotificationPolicies.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for NotificationPoliciesListNotificationPolicies" (int status)
        }

    ///<summary>
    ///Creates a new Notification policy.
    ///</summary>
    member this.NotificationPoliciesCreateANotificationPolicy
        (
            accountId: string,
            body: NotificationPoliciesCreateANotificationPolicyPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/policies"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationPoliciesCreateANotificationPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationPoliciesCreateANotificationPolicy.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for NotificationPoliciesCreateANotificationPolicy" (int status)
        }

    ///<summary>
    ///Delete a Notification policy.
    ///</summary>
    member this.NotificationPoliciesDeleteANotificationPolicy
        (accountId: string, policyId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("policy_id", policyId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/policies/{policy_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationPoliciesDeleteANotificationPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationPoliciesDeleteANotificationPolicy.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for NotificationPoliciesDeleteANotificationPolicy" (int status)
        }

    ///<summary>
    ///Get details for a single policy.
    ///</summary>
    member this.NotificationPoliciesGetANotificationPolicy
        (accountId: string, policyId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("policy_id", policyId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/policies/{policy_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationPoliciesGetANotificationPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationPoliciesGetANotificationPolicy.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for NotificationPoliciesGetANotificationPolicy" (int status)
        }

    ///<summary>
    ///Update a Notification policy.
    ///</summary>
    member this.NotificationPoliciesUpdateANotificationPolicy
        (
            accountId: string,
            policyId: string,
            body: NotificationPoliciesUpdateANotificationPolicyPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("policy_id", policyId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/policies/{policy_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationPoliciesUpdateANotificationPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationPoliciesUpdateANotificationPolicy.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for NotificationPoliciesUpdateANotificationPolicy" (int status)
        }

    ///<summary>
    ///Shows details for unsubscribing an email address from a notification policy.
    ///</summary>
    member this.NotificationPoliciesShowEmailUnsubscribeDetails
        (accountId: string, policyId: string, email: string, token: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("policy_id", policyId)
                  RequestPart.query ("email", email)
                  RequestPart.query ("token", token) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/policies/{policy_id}/email/unsubscribe"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationPoliciesShowEmailUnsubscribeDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationPoliciesShowEmailUnsubscribeDetails.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for NotificationPoliciesShowEmailUnsubscribeDetails"
                        (int status)
        }

    ///<summary>
    ///Unsubscribes an email address from a notification policy.
    ///</summary>
    member this.NotificationPoliciesUnsubscribeEmailFromNotificationPolicy
        (accountId: string, policyId: string, email: string, token: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("policy_id", policyId)
                  RequestPart.query ("email", email)
                  RequestPart.query ("token", token) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/policies/{policy_id}/email/unsubscribe"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return NotificationPoliciesUnsubscribeEmailFromNotificationPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationPoliciesUnsubscribeEmailFromNotificationPolicy.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for NotificationPoliciesUnsubscribeEmailFromNotificationPolicy"
                        (int status)
        }

    ///<summary>
    ///Send a test notification for a policy to verify delivery mechanisms are working as expected.
    ///</summary>
    member this.NotificationPoliciesTestANotificationPolicy
        (
            accountId: string,
            policyId: string,
            ?cancellationToken: CancellationToken,
            ?body: NotificationPoliciesTestANotificationPolicyPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("policy_id", policyId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/policies/{policy_id}/test"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationPoliciesTestANotificationPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NotificationPoliciesTestANotificationPolicy.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for NotificationPoliciesTestANotificationPolicy" (int status)
        }

    ///<summary>
    ///Gets a list of silences for an account.
    ///</summary>
    member this.NotificationSilencesListSilences(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/silences"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationSilencesListSilences.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationSilencesListSilences.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationSilencesListSilences" (int status)
        }

    ///<summary>
    ///Creates a new silence for an account.
    ///</summary>
    member this.NotificationSilencesCreateSilences
        (accountId: string, body: list<aaa_silence_create_request>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/silences"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationSilencesCreateSilences.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationSilencesCreateSilences.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationSilencesCreateSilences" (int status)
        }

    ///<summary>
    ///Updates existing silences for an account.
    ///</summary>
    member this.NotificationSilencesUpdateSilences
        (accountId: string, body: list<aaa_silence_update_request>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/silences"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationSilencesUpdateSilences.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationSilencesUpdateSilences.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationSilencesUpdateSilences" (int status)
        }

    ///<summary>
    ///Deletes an existing silence for an account.
    ///</summary>
    member this.NotificationSilencesDeleteSilences
        (accountId: string, silenceId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("silence_id", silenceId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/silences/{silence_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationSilencesDeleteSilences.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationSilencesDeleteSilences.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationSilencesDeleteSilences" (int status)
        }

    ///<summary>
    ///Gets a specific silence for an account.
    ///</summary>
    member this.NotificationSilencesGetSilence
        (accountId: string, silenceId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("silence_id", silenceId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/alerting/v3/silences/{silence_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NotificationSilencesGetSilence.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NotificationSilencesGetSilence.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NotificationSilencesGetSilence" (int status)
        }

    ///<summary>
    ///Lists existing destination addresses.
    ///</summary>
    member this.EmailRoutingDestinationAddressesListDestinationAddresses
        (
            accountId: string,
            ?page: float,
            ?perPage: float,
            ?direction: string,
            ?verified: bool,
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
                  if verified.IsSome then
                      RequestPart.query ("verified", verified.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/addresses"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return EmailRoutingDestinationAddressesListDestinationAddresses.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailRoutingDestinationAddressesListDestinationAddresses"
                        (int status)
        }

    ///<summary>
    ///Create a destination address to forward your emails to. Destination addresses need to be verified before they can be used.
    ///</summary>
    member this.EmailRoutingDestinationAddressesCreateADestinationAddress
        (accountId: string, body: email_create_destination_address_properties, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/addresses"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return EmailRoutingDestinationAddressesCreateADestinationAddress.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailRoutingDestinationAddressesCreateADestinationAddress"
                        (int status)
        }

    ///<summary>
    ///Deletes a specific destination address.
    ///</summary>
    member this.EmailRoutingDestinationAddressesDeleteDestinationAddress
        (destinationAddressIdentifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("destination_address_identifier", destinationAddressIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/addresses/{destination_address_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return EmailRoutingDestinationAddressesDeleteDestinationAddress.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailRoutingDestinationAddressesDeleteDestinationAddress"
                        (int status)
        }

    ///<summary>
    ///Gets information for a specific destination email already created.
    ///</summary>
    member this.EmailRoutingDestinationAddressesGetADestinationAddress
        (destinationAddressIdentifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("destination_address_identifier", destinationAddressIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/addresses/{destination_address_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingDestinationAddressesGetADestinationAddress.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailRoutingDestinationAddressesGetADestinationAddress"
                        (int status)
        }

    ///<summary>
    ///Updates the status of a specific destination address.
    ///</summary>
    member this.EmailRoutingDestinationAddressesUpdateDestinationAddress
        (
            destinationAddressIdentifier: string,
            accountId: string,
            body: email_update_destination_address_properties,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("destination_address_identifier", destinationAddressIdentifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/addresses/{destination_address_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return EmailRoutingDestinationAddressesUpdateDestinationAddress.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailRoutingDestinationAddressesUpdateDestinationAddress"
                        (int status)
        }

    ///<summary>
    ///Lists existing routing rules across all zones in the account.
    ///</summary>
    member this.EmailRoutingRoutingRulesListAccountRoutingRules
        (accountId: string, ?page: float, ?perPage: float, ?enabled: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if enabled.IsSome then
                      RequestPart.query ("enabled", enabled.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingRoutingRulesListAccountRoutingRules.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailRoutingRoutingRulesListAccountRoutingRules"
                        (int status)
        }

    ///<summary>
    ///Computes the Email Routing rule changes that would be needed to reconcile a Wrangler-managed desired ruleset. This endpoint is read-only and does not create, update, or delete rules.
    ///</summary>
    member this.EmailRoutingRoutingRulesPlanAccountRoutingRules
        (accountId: string, body: email_account_rules_plan_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/rules/plan"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingRoutingRulesPlanAccountRoutingRules.OK((Serializer.deserialize content))
            | 400 -> return EmailRoutingRoutingRulesPlanAccountRoutingRules.BadRequest
            | 403 -> return EmailRoutingRoutingRulesPlanAccountRoutingRules.Forbidden
            | 422 -> return EmailRoutingRoutingRulesPlanAccountRoutingRules.UnprocessableEntity
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailRoutingRoutingRulesPlanAccountRoutingRules"
                        (int status)
        }

    ///<summary>
    ///Lists email suppressions for the specified account.
    ///</summary>
    member this.GetPublicListSuppressionRouting
        (
            accountId: string,
            ?page: int,
            ?perPage: int,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/suppression"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPublicListSuppressionRouting.OK((Serializer.deserialize content))
            | 400 -> return GetPublicListSuppressionRouting.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPublicListSuppressionRouting" (int status)
        }

    ///<summary>
    ///Creates a new email suppression for the specified account.
    ///</summary>
    member this.PostPublicNewSuppressionRouting
        (accountId: string, ?cancellationToken: CancellationToken, ?body: PostPublicNewSuppressionRoutingPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/suppression"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostPublicNewSuppressionRouting.OK((Serializer.deserialize content))
            | 400 -> return PostPublicNewSuppressionRouting.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostPublicNewSuppressionRouting" (int status)
        }

    ///<summary>
    ///Deletes an email suppression for the specified account.
    ///</summary>
    member this.DeletePublicDeleteSuppressionRouting
        (accountId: string, suppressionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("suppression_id", suppressionId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/suppression/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeletePublicDeleteSuppressionRouting.OK((Serializer.deserialize content))
            | 404 -> return DeletePublicDeleteSuppressionRouting.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeletePublicDeleteSuppressionRouting" (int status)
        }

    ///<summary>
    ///Retrieves a single email suppression for the specified account.
    ///</summary>
    member this.GetPublicGetSuppressionRouting
        (accountId: string, suppressionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("suppression_id", suppressionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/routing/suppression/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPublicGetSuppressionRouting.OK((Serializer.deserialize content))
            | 404 -> return GetPublicGetSuppressionRouting.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPublicGetSuppressionRouting" (int status)
        }

    ///<summary>
    ///Returns the current daily sending quota for the account and, when a quota is resolved, the account's current usage against it. Quota is null when not yet available; usage is null when there is no resolved quota or usage is temporarily unavailable.
    ///</summary>
    member this.EmailSendingGetSendingLimits(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/limits"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingGetSendingLimits.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EmailSendingGetSendingLimits" (int status)
        }

    ///<summary>
    ///Returns the raw RFC 5322 MIME message for the given account and message id.
    ///</summary>
    member this.EmailSendingGetEmailMessage
        (accountId: string, messageId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("message_id", messageId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/messages/{message_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingGetEmailMessage.OK
            | 404 -> return EmailSendingGetEmailMessage.NotFound((Serializer.deserialize content))
            | 500 -> return EmailSendingGetEmailMessage.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EmailSendingGetEmailMessage" (int status)
        }

    ///<summary>
    ///Returns the authoritative Email Sending reputation state and active evaluation policy. Accounts without an evaluation are Healthy with null evaluation timestamps.
    ///</summary>
    member this.EmailSendingGetReputation(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/reputation"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingGetReputation.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EmailSendingGetReputation" (int status)
        }

    ///<summary>
    ///Send an email for the specified account using the structured builder. Provide the sender, recipients, subject, and at least one of text or html; attachments are optional.
    ///</summary>
    ///<param name="accountId">Identifier of the account.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.EmailSendingAccountSendBuilder
        (accountId: string, body: email_u002D_sending_EmailBuilder, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/send"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingAccountSendBuilder.OK((Serializer.deserialize content))
            | 400 -> return EmailSendingAccountSendBuilder.BadRequest((Serializer.deserialize content))
            | 403 -> return EmailSendingAccountSendBuilder.Forbidden((Serializer.deserialize content))
            | 429 -> return EmailSendingAccountSendBuilder.TooManyRequests((Serializer.deserialize content))
            | 500 -> return EmailSendingAccountSendBuilder.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EmailSendingAccountSendBuilder" (int status)
        }

    ///<summary>
    ///Send a raw RFC 5322 (MIME) email for the specified account. Provide the full MIME message plus the SMTP envelope (from and recipients).
    ///</summary>
    ///<param name="accountId">Identifier of the account.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.EmailSendingAccountSendRawMessage
        (accountId: string, body: email_u002D_sending_SendRawRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/send_raw"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingAccountSendRawMessage.OK((Serializer.deserialize content))
            | 400 -> return EmailSendingAccountSendRawMessage.BadRequest((Serializer.deserialize content))
            | 403 -> return EmailSendingAccountSendRawMessage.Forbidden((Serializer.deserialize content))
            | 429 -> return EmailSendingAccountSendRawMessage.TooManyRequests((Serializer.deserialize content))
            | 500 -> return EmailSendingAccountSendRawMessage.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EmailSendingAccountSendRawMessage" (int status)
        }

    ///<summary>
    ///Lists email suppressions for the specified account.
    ///</summary>
    member this.GetPublicListSuppressionSending
        (
            accountId: string,
            ?page: int,
            ?perPage: int,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/suppression"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPublicListSuppressionSending.OK((Serializer.deserialize content))
            | 400 -> return GetPublicListSuppressionSending.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPublicListSuppressionSending" (int status)
        }

    ///<summary>
    ///Creates a new email suppression for the specified account.
    ///</summary>
    member this.PostPublicNewSuppressionSending
        (accountId: string, ?cancellationToken: CancellationToken, ?body: PostPublicNewSuppressionSendingPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/suppression"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostPublicNewSuppressionSending.OK((Serializer.deserialize content))
            | 400 -> return PostPublicNewSuppressionSending.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostPublicNewSuppressionSending" (int status)
        }

    ///<summary>
    ///Deletes an email suppression for the specified account.
    ///</summary>
    member this.DeletePublicDeleteSuppressionSending
        (accountId: string, suppressionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("suppression_id", suppressionId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/suppression/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeletePublicDeleteSuppressionSending.OK((Serializer.deserialize content))
            | 404 -> return DeletePublicDeleteSuppressionSending.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeletePublicDeleteSuppressionSending" (int status)
        }

    ///<summary>
    ///Retrieves a single email suppression for the specified account.
    ///</summary>
    member this.GetPublicGetSuppressionSending
        (accountId: string, suppressionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("suppression_id", suppressionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/suppression/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPublicGetSuppressionSending.OK((Serializer.deserialize content))
            | 404 -> return GetPublicGetSuppressionSending.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPublicGetSuppressionSending" (int status)
        }

    ///<summary>
    ///Lists every active Email Sending suppression owned by the account, including legacy rows with internal zone memberships.
    ///</summary>
    member this.GetPublicListSendingSuppressions
        (
            accountId: string,
            ?perPage: int,
            ?cursor: string,
            ?email: string,
            ?search: string,
            ?reason: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if email.IsSome then
                      RequestPart.query ("email", email.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if reason.IsSome then
                      RequestPart.query ("reason", reason.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/suppressions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPublicListSendingSuppressions.OK((Serializer.deserialize content))
            | 400 -> return GetPublicListSendingSuppressions.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPublicListSendingSuppressions" (int status)
        }

    ///<summary>
    ///Creates an account-wide suppression. If a mutable legacy zone-linked row already exists, it is promoted without changing its identifier.
    ///</summary>
    member this.PostPublicCreateSendingSuppression
        (accountId: string, ?cancellationToken: CancellationToken, ?body: PostPublicCreateSendingSuppressionPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/suppressions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostPublicCreateSendingSuppression.OK((Serializer.deserialize content))
            | 400 -> return PostPublicCreateSendingSuppression.BadRequest((Serializer.deserialize content))
            | 403 -> return PostPublicCreateSendingSuppression.Forbidden((Serializer.deserialize content))
            | 404 -> return PostPublicCreateSendingSuppression.NotFound((Serializer.deserialize content))
            | 409 -> return PostPublicCreateSendingSuppression.Conflict((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostPublicCreateSendingSuppression" (int status)
        }

    ///<summary>
    ///Imports up to 1,000 account-level Email Sending suppressions in one request.
    ///</summary>
    member this.PostPublicBulkCreateSendingSuppressions
        (accountId: string, ?cancellationToken: CancellationToken, ?body: PostPublicBulkCreateSendingSuppressionsPayload) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/suppressions/bulk"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostPublicBulkCreateSendingSuppressions.OK((Serializer.deserialize content))
            | 429 -> return PostPublicBulkCreateSendingSuppressions.TooManyRequests((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostPublicBulkCreateSendingSuppressions" (int status)
        }

    ///<summary>
    ///Deletes the suppression, its note, and every legacy internal zone membership, allowing future delivery attempts to the address.
    ///</summary>
    member this.DeletePublicDeleteSendingSuppression
        (accountId: string, suppressionId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("suppression_id", suppressionId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/suppressions/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeletePublicDeleteSendingSuppression.OK((Serializer.deserialize content))
            | 400 -> return DeletePublicDeleteSendingSuppression.BadRequest((Serializer.deserialize content))
            | 403 -> return DeletePublicDeleteSendingSuppression.Forbidden((Serializer.deserialize content))
            | 404 -> return DeletePublicDeleteSendingSuppression.NotFound((Serializer.deserialize content))
            | 409 -> return DeletePublicDeleteSendingSuppression.Conflict((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeletePublicDeleteSendingSuppression" (int status)
        }

    ///<summary>
    ///Gets an Email Sending suppression owned by the account.
    ///</summary>
    member this.GetPublicGetSendingSuppression
        (accountId: string, suppressionId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("suppression_id", suppressionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/suppressions/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPublicGetSendingSuppression.OK((Serializer.deserialize content))
            | 404 -> return GetPublicGetSendingSuppression.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPublicGetSendingSuppression" (int status)
        }

    ///<summary>
    ///Updates expiry or advisory note fields without changing legacy internal zone memberships.
    ///</summary>
    member this.PatchPublicUpdateSendingSuppression
        (
            accountId: string,
            suppressionId: System.Guid,
            ?cancellationToken: CancellationToken,
            ?body: PatchPublicUpdateSendingSuppressionPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("suppression_id", suppressionId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/email/sending/suppressions/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PatchPublicUpdateSendingSuppression.OK((Serializer.deserialize content))
            | 400 -> return PatchPublicUpdateSendingSuppression.BadRequest((Serializer.deserialize content))
            | 403 -> return PatchPublicUpdateSendingSuppression.Forbidden((Serializer.deserialize content))
            | 404 -> return PatchPublicUpdateSendingSuppression.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PatchPublicUpdateSendingSuppression" (int status)
        }

    ///<summary>
    ///List all event notification rules for a bucket.
    ///</summary>
    member this.R2GetEventNotificationConfigs
        (bucketName: string, accountId: string, ?cfR2Jurisdiction: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/event_notifications/r2/{bucket_name}/configuration"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetEventNotificationConfigs.OK((Serializer.deserialize content))
            | 404 -> return R2GetEventNotificationConfigs.NotFound((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetEventNotificationConfigs.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetEventNotificationConfigs" (int status)
        }

    ///<summary>
    ///Delete an event notification rule. **If no body is provided, all rules for specified queue will be deleted**.
    ///</summary>
    member this.R2EventNotificationDeleteConfig
        (
            queueId: string,
            bucketName: string,
            accountId: string,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken,
            ?body: R2EventNotificationDeleteConfigPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/event_notifications/r2/{bucket_name}/configuration/queues/{queue_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2EventNotificationDeleteConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2EventNotificationDeleteConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2EventNotificationDeleteConfig" (int status)
        }

    ///<summary>
    ///Get a single event notification rule.
    ///</summary>
    member this.R2GetEventNotificationConfig
        (
            queueId: string,
            bucketName: string,
            accountId: string,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/event_notifications/r2/{bucket_name}/configuration/queues/{queue_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetEventNotificationConfig.OK((Serializer.deserialize content))
            | 404 -> return R2GetEventNotificationConfig.NotFound((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetEventNotificationConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetEventNotificationConfig" (int status)
        }

    ///<summary>
    ///Create event notification rule.
    ///</summary>
    member this.R2PutEventNotificationConfig
        (
            queueId: string,
            bucketName: string,
            accountId: string,
            body: R2PutEventNotificationConfigPayload,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/event_notifications/r2/{bucket_name}/configuration/queues/{queue_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2PutEventNotificationConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2PutEventNotificationConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2PutEventNotificationConfig" (int status)
        }

    ///<summary>
    ///Get a paginated list of event subscriptions with optional sorting and filtering
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="page">Page number for pagination</param>
    ///<param name="perPage">Number of items per page</param>
    ///<param name="order">Field to sort by</param>
    ///<param name="direction">Sort direction</param>
    ///<param name="cancellationToken"></param>
    member this.SubscriptionsList
        (
            accountId: string,
            ?page: int,
            ?perPage: int,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/event_subscriptions/subscriptions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SubscriptionsList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SubscriptionsList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SubscriptionsList" (int status)
        }

    ///<summary>
    ///Create a new event subscription for a queue
    ///</summary>
    member this.SubscriptionsCreate
        (accountId: string, body: SubscriptionsCreatePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/event_subscriptions/subscriptions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SubscriptionsCreate.OK((Serializer.deserialize content))
            | 400 -> return SubscriptionsCreate.BadRequest((Serializer.deserialize content))
            | 404 -> return SubscriptionsCreate.NotFound((Serializer.deserialize content))
            | 405 -> return SubscriptionsCreate.MethodNotAllowed((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SubscriptionsCreate" (int status)
        }

    ///<summary>
    ///Delete an existing event subscription
    ///</summary>
    member this.SubscriptionsDelete(accountId: string, subscriptionId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("subscription_id", subscriptionId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/event_subscriptions/subscriptions/{subscription_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SubscriptionsDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SubscriptionsDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SubscriptionsDelete" (int status)
        }

    ///<summary>
    ///Get details about an existing event subscription
    ///</summary>
    member this.SubscriptionsGet(accountId: string, subscriptionId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("subscription_id", subscriptionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/event_subscriptions/subscriptions/{subscription_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SubscriptionsGet.OK((Serializer.deserialize content))
            | 404 -> return SubscriptionsGet.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SubscriptionsGet" (int status)
        }

    ///<summary>
    ///Update an existing event subscription
    ///</summary>
    member this.SubscriptionsPatch
        (
            accountId: string,
            subscriptionId: string,
            body: SubscriptionsPatchPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("subscription_id", subscriptionId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/event_subscriptions/subscriptions/{subscription_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SubscriptionsPatch.OK((Serializer.deserialize content))
            | 400 -> return SubscriptionsPatch.BadRequest((Serializer.deserialize content))
            | 404 -> return SubscriptionsPatch.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SubscriptionsPatch" (int status)
        }

    ///<summary>
    ///Retrieves the current DMARC report configuration and status for a zone.
    ///Returns the RUA prefix, enabled status, approved sources, and DNS records.
    ///</summary>
    ///<param name="zoneId">Zone identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.GetDmarcReportsStatus(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/auth/dmarc-reports"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetDmarcReportsStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetDmarcReportsStatus.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetDmarcReportsStatus" (int status)
        }

    ///<summary>
    ///Updates the DMARC report configuration for a zone.
    ///At least one of `enabled` or `skip_wizard` must be provided.
    ///When enabling, the handler will ensure the DMARC RUA record exists in DNS.
    ///</summary>
    ///<param name="zoneId">Zone identifier.</param>
    ///<param name="body">Request body for PATCH /dmarc-reports</param>
    ///<param name="cancellationToken"></param>
    member this.ConfigureDmarcReports
        (zoneId: string, body: email_u002D_auth_ConfigureDmarcReportsRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/email/auth/dmarc-reports"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ConfigureDmarcReports.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ConfigureDmarcReports.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ConfigureDmarcReports" (int status)
        }

    ///<summary>
    ///Inspects a specific SPF TXT record and returns a parsed tree structure
    ///in the spflimit-worker format.
    ///The record ID must be provided via the `id` query parameter.
    ///Returns a recursive tree showing:
    ///- Parsed components with their qualifiers and types
    ///- Nested includes recursively resolved within components
    ///- Per-component and total lookup counts
    ///- Detailed error information with context
    ///</summary>
    ///<param name="zoneId">Zone identifier.</param>
    ///<param name="id">DNS record ID (rec_tag) to inspect</param>
    ///<param name="cancellationToken"></param>
    member this.InspectSpf(zoneId: string, id: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.query ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/email/auth/spf/inspect" requestParts cancellationToken

            match (int status) with
            | 200 -> return InspectSpf.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return InspectSpf.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for InspectSpf" (int status)
        }

    ///<summary>
    ///Get information about the settings for your Email Routing zone.
    ///</summary>
    member this.EmailRoutingSettingsGetEmailRoutingSettings(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/email/routing" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingSettingsGetEmailRoutingSettings.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for EmailRoutingSettingsGetEmailRoutingSettings" (int status)
        }

    ///<summary>
    ///Update the settings for your Email Routing zone.
    ///</summary>
    member this.EmailRoutingSettingsUpdateEmailRoutingSettings
        (zoneId: string, body: email_update_email_routing_settings_properties, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/email/routing" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingSettingsUpdateEmailRoutingSettings.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailRoutingSettingsUpdateEmailRoutingSettings"
                        (int status)
        }

    ///<summary>
    ///Update the settings for your Email Routing zone.
    ///</summary>
    member this.EmailRoutingSettingsReplaceEmailRoutingSettings
        (zoneId: string, body: email_update_email_routing_settings_properties, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/email/routing" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingSettingsReplaceEmailRoutingSettings.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailRoutingSettingsReplaceEmailRoutingSettings"
                        (int status)
        }

    ///<summary>
    ///Disable your Email Routing zone. Also removes additional MX records previously required for Email Routing to work.
    ///</summary>
    member this.EmailRoutingSettingsDisableEmailRouting
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/email/routing/disable" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingSettingsDisableEmailRouting.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EmailRoutingSettingsDisableEmailRouting" (int status)
        }

    ///<summary>
    ///Disable your Email Routing zone. Also removes additional MX records previously required for Email Routing to work.
    ///</summary>
    member this.EmailRoutingSettingsDisableEmailRoutingDns
        (zoneId: string, ?cancellationToken: CancellationToken, ?body: email_email_setting_dns_request_body)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/zones/{zone_id}/email/routing/dns" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingSettingsDisableEmailRoutingDns.OK((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for EmailRoutingSettingsDisableEmailRoutingDns" (int status)
        }

    ///<summary>
    ///Show the DNS records needed to configure your Email Routing zone.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="subdomain">Deprecated. When supplied, the response shape differs from the documented default and is not modeled in generated SDKs. Do not rely on this parameter.</param>
    ///<param name="cancellationToken"></param>
    member this.EmailRoutingSettingsEmailRoutingDnsSettings
        (zoneId: string, ?subdomain: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if subdomain.IsSome then
                      RequestPart.query ("subdomain", subdomain.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/email/routing/dns" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingSettingsEmailRoutingDnsSettings.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for EmailRoutingSettingsEmailRoutingDnsSettings" (int status)
        }

    ///<summary>
    ///Unlock MX Records previously locked by Email Routing.
    ///</summary>
    member this.EmailRoutingSettingsUnlockEmailRoutingDns
        (zoneId: string, ?cancellationToken: CancellationToken, ?body: email_email_setting_dns_request_body)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/email/routing/dns" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingSettingsUnlockEmailRoutingDns.OK((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for EmailRoutingSettingsUnlockEmailRoutingDns" (int status)
        }

    ///<summary>
    ///Enable you Email Routing zone. Add and lock the necessary MX and SPF records.
    ///</summary>
    member this.EmailRoutingSettingsEnableEmailRoutingDns
        (zoneId: string, ?cancellationToken: CancellationToken, ?body: email_email_setting_dns_request_body)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/email/routing/dns" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingSettingsEnableEmailRoutingDns.OK((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for EmailRoutingSettingsEnableEmailRoutingDns" (int status)
        }

    ///<summary>
    ///Enable you Email Routing zone. Add and lock the necessary MX and SPF records.
    ///</summary>
    member this.EmailRoutingSettingsEnableEmailRouting
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/email/routing/enable" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingSettingsEnableEmailRouting.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EmailRoutingSettingsEnableEmailRouting" (int status)
        }

    ///<summary>
    ///Lists existing routing rules.
    ///</summary>
    member this.EmailRoutingRoutingRulesListRoutingRules
        (zoneId: string, ?page: float, ?perPage: float, ?enabled: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if enabled.IsSome then
                      RequestPart.query ("enabled", enabled.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/email/routing/rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingRoutingRulesListRoutingRules.OK((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for EmailRoutingRoutingRulesListRoutingRules" (int status)
        }

    ///<summary>
    ///Rules consist of a set of criteria for matching emails (such as an email being sent to a specific custom email address) plus a set of actions to take on the email (like forwarding it to a specific destination address). Forward actions require exactly one verified destination address.
    ///</summary>
    member this.EmailRoutingRoutingRulesCreateRoutingRule
        (zoneId: string, body: email_create_rule_properties, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/email/routing/rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingRoutingRulesCreateRoutingRule.OK((Serializer.deserialize content))
            | 400 -> return EmailRoutingRoutingRulesCreateRoutingRule.BadRequest
            | 422 -> return EmailRoutingRoutingRulesCreateRoutingRule.UnprocessableEntity
            | _ ->
                return failwithf "Unexpected HTTP status %d for EmailRoutingRoutingRulesCreateRoutingRule" (int status)
        }

    ///<summary>
    ///Get information on the default catch-all routing rule.
    ///</summary>
    member this.EmailRoutingRoutingRulesGetCatchAllRule(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/routing/rules/catch_all"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingRoutingRulesGetCatchAllRule.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EmailRoutingRoutingRulesGetCatchAllRule" (int status)
        }

    ///<summary>
    ///Enable or disable catch-all routing rule, or change action to forward to a specific destination address. Forward actions require exactly one verified destination address.
    ///</summary>
    member this.EmailRoutingRoutingRulesUpdateCatchAllRule
        (zoneId: string, body: email_update_catch_all_rule_properties, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/email/routing/rules/catch_all"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingRoutingRulesUpdateCatchAllRule.OK((Serializer.deserialize content))
            | 400 -> return EmailRoutingRoutingRulesUpdateCatchAllRule.BadRequest
            | 422 -> return EmailRoutingRoutingRulesUpdateCatchAllRule.UnprocessableEntity
            | _ ->
                return failwithf "Unexpected HTTP status %d for EmailRoutingRoutingRulesUpdateCatchAllRule" (int status)
        }

    ///<summary>
    ///Delete a specific routing rule.
    ///</summary>
    member this.EmailRoutingRoutingRulesDeleteRoutingRule
        (ruleIdentifier: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("rule_identifier", ruleIdentifier)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/email/routing/rules/{rule_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingRoutingRulesDeleteRoutingRule.OK((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for EmailRoutingRoutingRulesDeleteRoutingRule" (int status)
        }

    ///<summary>
    ///Get information for a specific routing rule already created.
    ///</summary>
    member this.EmailRoutingRoutingRulesGetRoutingRule
        (ruleIdentifier: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("rule_identifier", ruleIdentifier)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/routing/rules/{rule_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingRoutingRulesGetRoutingRule.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EmailRoutingRoutingRulesGetRoutingRule" (int status)
        }

    ///<summary>
    ///Update actions and matches, or enable/disable specific routing rules. Forward actions require exactly one verified destination address.
    ///</summary>
    member this.EmailRoutingRoutingRulesUpdateRoutingRule
        (
            ruleIdentifier: string,
            zoneId: string,
            body: email_update_rule_properties,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("rule_identifier", ruleIdentifier)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/email/routing/rules/{rule_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingRoutingRulesUpdateRoutingRule.OK((Serializer.deserialize content))
            | 400 -> return EmailRoutingRoutingRulesUpdateRoutingRule.BadRequest
            | 422 -> return EmailRoutingRoutingRulesUpdateRoutingRule.UnprocessableEntity
            | _ ->
                return failwithf "Unexpected HTTP status %d for EmailRoutingRoutingRulesUpdateRoutingRule" (int status)
        }

    ///<summary>
    ///Lists email suppressions for the specified zone.
    ///</summary>
    member this.GetPublicListSuppressionZoneRouting
        (
            zoneId: string,
            ?page: int,
            ?perPage: int,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/routing/suppression"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPublicListSuppressionZoneRouting.OK((Serializer.deserialize content))
            | 400 -> return GetPublicListSuppressionZoneRouting.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPublicListSuppressionZoneRouting" (int status)
        }

    ///<summary>
    ///Creates a new email suppression for the specified zone.
    ///</summary>
    member this.PostPublicNewSuppressionZoneRouting
        (zoneId: string, ?cancellationToken: CancellationToken, ?body: PostPublicNewSuppressionZoneRoutingPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/email/routing/suppression"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostPublicNewSuppressionZoneRouting.OK((Serializer.deserialize content))
            | 400 -> return PostPublicNewSuppressionZoneRouting.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostPublicNewSuppressionZoneRouting" (int status)
        }

    ///<summary>
    ///Deletes an email suppression for the specified zone.
    ///</summary>
    member this.DeletePublicDeleteSuppressionZoneRouting
        (zoneId: string, suppressionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("suppression_id", suppressionId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/email/routing/suppression/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeletePublicDeleteSuppressionZoneRouting.OK((Serializer.deserialize content))
            | 404 -> return DeletePublicDeleteSuppressionZoneRouting.NotFound((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for DeletePublicDeleteSuppressionZoneRouting" (int status)
        }

    ///<summary>
    ///Retrieves a single email suppression for the specified zone.
    ///</summary>
    member this.GetPublicGetSuppressionZoneRouting
        (zoneId: string, suppressionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("suppression_id", suppressionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/routing/suppression/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPublicGetSuppressionZoneRouting.OK((Serializer.deserialize content))
            | 404 -> return GetPublicGetSuppressionZoneRouting.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPublicGetSuppressionZoneRouting" (int status)
        }

    ///<summary>
    ///Unlock MX records previously locked by Email Routing. Deprecated - use PATCH /zones/{zone_id}/email/routing/dns instead.
    ///</summary>
    member this.EmailRoutingSettingsUnlockEmailRouting
        (zoneId: string, ?cancellationToken: CancellationToken, ?body: email_email_setting_dns_request_body)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/email/routing/unlock" requestParts cancellationToken

            match (int status) with
            | 200 -> return EmailRoutingSettingsUnlockEmailRouting.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EmailRoutingSettingsUnlockEmailRouting" (int status)
        }

    ///<summary>
    ///Lists all sending-enabled subdomains for the zone.
    ///</summary>
    member this.EmailSendingSubdomainsListSendingSubdomains(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/subdomains"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingSubdomainsListSendingSubdomains.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for EmailSendingSubdomainsListSendingSubdomains" (int status)
        }

    ///<summary>
    ///Creates a new sending subdomain or re-enables sending on an existing subdomain that had it disabled. If zone-level Email Sending has not been enabled yet, the zone flag is automatically set when the entitlement is present. A leftmost wildcard such as `*.example.com` is accepted only for accounts with wildcard Email Sending enabled. Wildcard senders share the base domain's DKIM signing identity and `cf-bounce.&amp;lt;base&amp;gt;` return path.
    ///</summary>
    member this.EmailSendingSubdomainsCreateSendingSubdomain
        (zoneId: string, body: email_create_sending_subdomain_properties, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/subdomains"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingSubdomainsCreateSendingSubdomain.OK((Serializer.deserialize content))
            | 403 -> return EmailSendingSubdomainsCreateSendingSubdomain.Forbidden
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for EmailSendingSubdomainsCreateSendingSubdomain" (int status)
        }

    ///<summary>
    ///Returns the DNS records that would be created for a sending subdomain, flags which records are missing, and reports any conflicts with existing DNS records. This is a read-only dry-run — no records are created or modified. Use before or after creating a subdomain to check DNS status. A leftmost wildcard requires wildcard Email Sending to be enabled for the account and previews base-scoped DKIM and return-path records.
    ///</summary>
    member this.EmailSendingSubdomainsPreviewSendingSubdomain
        (zoneId: string, body: email_create_sending_subdomain_properties, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/subdomains/preview"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingSubdomainsPreviewSendingSubdomain.OK((Serializer.deserialize content))
            | 403 -> return EmailSendingSubdomainsPreviewSendingSubdomain.Forbidden
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for EmailSendingSubdomainsPreviewSendingSubdomain" (int status)
        }

    ///<summary>
    ///Disables sending on a subdomain and removes its DNS records. If routing is still active on the subdomain, only sending is disabled.
    ///</summary>
    member this.EmailSendingSubdomainsDeleteSendingSubdomain
        (subdomainId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subdomain_id", subdomainId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/subdomains/{subdomain_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingSubdomainsDeleteSendingSubdomain.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for EmailSendingSubdomainsDeleteSendingSubdomain" (int status)
        }

    ///<summary>
    ///Gets information for a specific sending subdomain.
    ///</summary>
    member this.EmailSendingSubdomainsGetSendingSubdomain
        (subdomainId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subdomain_id", subdomainId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/subdomains/{subdomain_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingSubdomainsGetSendingSubdomain.OK((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for EmailSendingSubdomainsGetSendingSubdomain" (int status)
        }

    ///<summary>
    ///Updates the activity-log preview preference for a sending subdomain.
    ///</summary>
    ///<param name="subdomainId"></param>
    ///<param name="zoneId"></param>
    ///<param name="body">
    ///At least one of `preview_enabled` or `drop_suppressed_recipients` must
    ///be provided. A field omitted from the request body is left unchanged.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.EmailSendingSubdomainsUpdateSendingSubdomain
        (
            subdomainId: string,
            zoneId: string,
            body: email_update_sending_subdomain_properties,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subdomain_id", subdomainId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/subdomains/{subdomain_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingSubdomainsUpdateSendingSubdomain.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for EmailSendingSubdomainsUpdateSendingSubdomain" (int status)
        }

    ///<summary>
    ///Returns the expected DNS records for a sending subdomain.
    ///</summary>
    member this.EmailSendingSubdomainsGetSendingSubdomainDns
        (subdomainId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subdomain_id", subdomainId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/subdomains/{subdomain_id}/dns"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingSubdomainsGetSendingSubdomainDns.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for EmailSendingSubdomainsGetSendingSubdomainDns" (int status)
        }

    ///<summary>
    ///Idempotently re-applies the sending DNS records (creates missing records, re-applies the email_routing lock on records whose lock has been cleared). Refuses with a 409 if foreign MX, multiple SPF, multiple DMARC, or multiple DKIM records exist at the relevant DNS names — those require manual cleanup.
    ///</summary>
    member this.EmailSendingSubdomainsFixSendingSubdomainDns
        (subdomainId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subdomain_id", subdomainId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/subdomains/{subdomain_id}/dns"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingSubdomainsFixSendingSubdomainDns.OK((Serializer.deserialize content))
            | 400 -> return EmailSendingSubdomainsFixSendingSubdomainDns.BadRequest
            | 403 -> return EmailSendingSubdomainsFixSendingSubdomainDns.Forbidden
            | 404 -> return EmailSendingSubdomainsFixSendingSubdomainDns.NotFound
            | 409 -> return EmailSendingSubdomainsFixSendingSubdomainDns.Conflict
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for EmailSendingSubdomainsFixSendingSubdomainDns" (int status)
        }

    ///<summary>
    ///Returns the desired DNS records for a sending subdomain along with a live diff against actual DNS state. Use this to detect missing, unlocked, foreign, or multi-record conflicts before deciding whether to call the fix endpoint. For wildcard sending rows, each call also rechecks the governing organizational-domain DMARC policy and reports policy drift that DNS Fix cannot repair.
    ///</summary>
    member this.EmailSendingSubdomainsGetSendingSubdomainDnsStatus
        (subdomainId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subdomain_id", subdomainId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/subdomains/{subdomain_id}/dns/status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EmailSendingSubdomainsGetSendingSubdomainDnsStatus.OK((Serializer.deserialize content))
            | 400 -> return EmailSendingSubdomainsGetSendingSubdomainDnsStatus.BadRequest
            | 404 -> return EmailSendingSubdomainsGetSendingSubdomainDnsStatus.NotFound
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailSendingSubdomainsGetSendingSubdomainDnsStatus"
                        (int status)
        }

    ///<summary>
    ///Returns the matched complaint count for a sending subdomain in a half-open time window of up to seven days.
    ///</summary>
    member this.EmailSendingSubdomainsGetSendingSubdomainReputationComplaints
        (
            subdomainId: string,
            zoneId: string,
            startAt: System.DateTimeOffset,
            endAt: System.DateTimeOffset,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("subdomain_id", subdomainId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.query ("start_at", startAt)
                  RequestPart.query ("end_at", endAt) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/subdomains/{subdomain_id}/reputation/complaints"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    EmailSendingSubdomainsGetSendingSubdomainReputationComplaints.OK((Serializer.deserialize content))
            | 400 -> return EmailSendingSubdomainsGetSendingSubdomainReputationComplaints.BadRequest
            | 404 -> return EmailSendingSubdomainsGetSendingSubdomainReputationComplaints.NotFound
            | 422 -> return EmailSendingSubdomainsGetSendingSubdomainReputationComplaints.UnprocessableEntity
            | 502 -> return EmailSendingSubdomainsGetSendingSubdomainReputationComplaints.BadGateway
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for EmailSendingSubdomainsGetSendingSubdomainReputationComplaints"
                        (int status)
        }

    ///<summary>
    ///Lists email suppressions for the specified zone.
    ///</summary>
    member this.GetPublicListSuppressionZoneSending
        (
            zoneId: string,
            ?page: int,
            ?perPage: int,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/suppression"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPublicListSuppressionZoneSending.OK((Serializer.deserialize content))
            | 400 -> return GetPublicListSuppressionZoneSending.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPublicListSuppressionZoneSending" (int status)
        }

    ///<summary>
    ///Creates a new email suppression for the specified zone.
    ///</summary>
    member this.PostPublicNewSuppressionZoneSending
        (zoneId: string, ?cancellationToken: CancellationToken, ?body: PostPublicNewSuppressionZoneSendingPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/suppression"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostPublicNewSuppressionZoneSending.OK((Serializer.deserialize content))
            | 400 -> return PostPublicNewSuppressionZoneSending.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostPublicNewSuppressionZoneSending" (int status)
        }

    ///<summary>
    ///Deletes an email suppression for the specified zone.
    ///</summary>
    member this.DeletePublicDeleteSuppressionZoneSending
        (zoneId: string, suppressionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("suppression_id", suppressionId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/suppression/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeletePublicDeleteSuppressionZoneSending.OK((Serializer.deserialize content))
            | 404 -> return DeletePublicDeleteSuppressionZoneSending.NotFound((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for DeletePublicDeleteSuppressionZoneSending" (int status)
        }

    ///<summary>
    ///Retrieves a single email suppression for the specified zone.
    ///</summary>
    member this.GetPublicGetSuppressionZoneSending
        (zoneId: string, suppressionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("suppression_id", suppressionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/email/sending/suppression/{suppression_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPublicGetSuppressionZoneSending.OK((Serializer.deserialize content))
            | 404 -> return GetPublicGetSuppressionZoneSending.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPublicGetSuppressionZoneSending" (int status)
        }
