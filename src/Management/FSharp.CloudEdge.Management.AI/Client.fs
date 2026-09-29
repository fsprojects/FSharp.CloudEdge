namespace rec FSharp.CloudEdge.Management.AI

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
type AIClient(httpClient: HttpClient) =
    ///<summary>
    ///Lists all namespaces for the given account. Results are paginated.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="perPage"></param>
    ///<param name="order"></param>
    ///<param name="direction"></param>
    ///<param name="cursor"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryNamespaceList
        (
            accountId: string,
            ?perPage: int,
            ?order: string,
            ?direction: string,
            ?cursor: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryNamespaceList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryNamespaceList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryNamespaceList" (int status)
        }

    ///<summary>
    ///Creates a new memory namespace owned by the account.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryNamespaceCreate
        (accountId: string, body: AgentMemoryNamespaceCreatePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return AgentMemoryNamespaceCreate.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryNamespaceCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryNamespaceCreate" (int status)
        }

    ///<summary>
    ///Deletes a namespace.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryNamespaceDelete
        (accountId: string, namespaceName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryNamespaceDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryNamespaceDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryNamespaceDelete" (int status)
        }

    ///<summary>
    ///Gets a namespace by name.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryNamespaceGet
        (accountId: string, namespaceName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryNamespaceGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryNamespaceGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryNamespaceGet" (int status)
        }

    ///<summary>
    ///Lists the profiles of a namespace, ordered by name. A profile appears once it has been used.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="perPage"></param>
    ///<param name="cursor"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryProfileList
        (accountId: string, namespaceName: string, ?perPage: int, ?cursor: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}/profiles"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryProfileList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryProfileList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryProfileList" (int status)
        }

    ///<summary>
    ///Marks a profile for deletion.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="profileName"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryProfileDelete
        (accountId: string, namespaceName: string, profileName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName)
                  RequestPart.path ("profile_name", profileName) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}/profiles/{profile_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryProfileDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryProfileDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryProfileDelete" (int status)
        }

    ///<summary>
    ///Processes a conversation and extracts structured memories from it. Agent Memory identifies facts, events, instructions, and tasks automatically.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="profileName"></param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryIngest
        (
            accountId: string,
            namespaceName: string,
            profileName: string,
            body: AgentMemoryIngestPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName)
                  RequestPart.path ("profile_name", profileName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}/profiles/{profile_name}/ingest"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryIngest.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryIngest.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryIngest" (int status)
        }

    ///<summary>
    ///List memories stored in a profile.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="profileName"></param>
    ///<param name="perPage"></param>
    ///<param name="cursor"></param>
    ///<param name="sessionId"></param>
    ///<param name="type"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryMemoryList
        (
            accountId: string,
            namespaceName: string,
            profileName: string,
            ?perPage: int,
            ?cursor: string,
            ?sessionId: string,
            ?``type``: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName)
                  RequestPart.path ("profile_name", profileName)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if sessionId.IsSome then
                      RequestPart.query ("session_id", sessionId.Value)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}/profiles/{profile_name}/memories"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryMemoryList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryMemoryList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryMemoryList" (int status)
        }

    ///<summary>
    ///Deletes a memory by ID. Removes the memory and any source messages linked to it. Returns the deleted memory.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="profileName"></param>
    ///<param name="memoryId"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryMemoryDelete
        (
            accountId: string,
            namespaceName: string,
            profileName: string,
            memoryId: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName)
                  RequestPart.path ("profile_name", profileName)
                  RequestPart.path ("memory_id", memoryId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}/profiles/{profile_name}/memories/{memory_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryMemoryDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryMemoryDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryMemoryDelete" (int status)
        }

    ///<summary>
    ///Retrieves a memory by ID.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="profileName"></param>
    ///<param name="memoryId"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryMemoryGet
        (
            accountId: string,
            namespaceName: string,
            profileName: string,
            memoryId: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName)
                  RequestPart.path ("profile_name", profileName)
                  RequestPart.path ("memory_id", memoryId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}/profiles/{profile_name}/memories/{memory_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryMemoryGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryMemoryGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryMemoryGet" (int status)
        }

    ///<summary>
    ///Retrieves memories relevant to the query and returns a synthesized answer.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="profileName"></param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryRecall
        (
            accountId: string,
            namespaceName: string,
            profileName: string,
            body: AgentMemoryRecallPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName)
                  RequestPart.path ("profile_name", profileName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}/profiles/{profile_name}/recall"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryRecall.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryRecall.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryRecall" (int status)
        }

    ///<summary>
    ///Stores a single memory explicitly.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="profileName"></param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemoryRemember
        (
            accountId: string,
            namespaceName: string,
            profileName: string,
            body: AgentMemoryRememberPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName)
                  RequestPart.path ("profile_name", profileName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}/profiles/{profile_name}/remember"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemoryRemember.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemoryRemember.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemoryRemember" (int status)
        }

    ///<summary>
    ///Marks all memories and messages in a profile that are tagged with the given session ID for deletion.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="profileName"></param>
    ///<param name="sessionId">Session identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemorySessionDelete
        (
            accountId: string,
            namespaceName: string,
            profileName: string,
            sessionId: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName)
                  RequestPart.path ("profile_name", profileName)
                  RequestPart.path ("session_id", sessionId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}/profiles/{profile_name}/sessions/{session_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemorySessionDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemorySessionDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemorySessionDelete" (int status)
        }

    ///<summary>
    ///Generates a paste-ready prompt block summarizing everything stored in a memory profile.
    ///</summary>
    ///<param name="accountId">Cloudflare Account ID.</param>
    ///<param name="namespaceName"></param>
    ///<param name="profileName"></param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AgentMemorySummary
        (
            accountId: string,
            namespaceName: string,
            profileName: string,
            body: AgentMemorySummaryPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace_name", namespaceName)
                  RequestPart.path ("profile_name", profileName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/agent-memory/namespaces/{namespace_name}/profiles/{profile_name}/summary"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AgentMemorySummary.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AgentMemorySummary.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AgentMemorySummary" (int status)
        }

    ///<summary>
    ///Retrieve the current credit balance, payment method info, and top-up configuration.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingGetCreditBalance(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/credit-balance"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingGetCreditBalance.OK((Serializer.deserialize content))
            | 400 -> return AigBillingGetCreditBalance.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingGetCreditBalance.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingGetCreditBalance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingGetCreditBalance" (int status)
        }

    ///<summary>
    ///Retrieve a list of past invoices with pagination, optionally filtered by type.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="type">Filter invoice type: auto, manual, or all.</param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingGetInvoiceHistory
        (accountId: string, ?``type``: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/invoice-history"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingGetInvoiceHistory.OK((Serializer.deserialize content))
            | 400 -> return AigBillingGetInvoiceHistory.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingGetInvoiceHistory.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingGetInvoiceHistory.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingGetInvoiceHistory" (int status)
        }

    ///<summary>
    ///Retrieve a preview of the upcoming invoice including line items and tax.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingGetInvoicePreview(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/invoice-preview"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingGetInvoicePreview.OK((Serializer.deserialize content))
            | 400 -> return AigBillingGetInvoicePreview.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingGetInvoicePreview.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingGetInvoicePreview.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingGetInvoicePreview" (int status)
        }

    ///<summary>
    ///Remove the spending limit for the account.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingDeleteSpendingLimit(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/spending-limit"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingDeleteSpendingLimit.OK((Serializer.deserialize content))
            | 400 -> return AigBillingDeleteSpendingLimit.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingDeleteSpendingLimit.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingDeleteSpendingLimit.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingDeleteSpendingLimit" (int status)
        }

    ///<summary>
    ///Retrieve the current spending limit configuration for the account.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingGetSpendingLimit(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/spending-limit"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingGetSpendingLimit.OK((Serializer.deserialize content))
            | 400 -> return AigBillingGetSpendingLimit.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingGetSpendingLimit.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingGetSpendingLimit.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingGetSpendingLimit" (int status)
        }

    ///<summary>
    ///Deprecated: spending limits can no longer be created, enabled, or modified and this endpoint always responds 403. Use the new AI Gateway spend limits instead: https://developers.cloudflare.com/ai-gateway/features/spend-limits/. Existing limits can be removed via DELETE /spending-limit.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingSetSpendingLimit
        (accountId: string, body: AigBillingSetSpendingLimitPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/spending-limit"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return AigBillingSetSpendingLimit.Created((Serializer.deserialize content))
            | 400 -> return AigBillingSetSpendingLimit.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingSetSpendingLimit.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingSetSpendingLimit.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingSetSpendingLimit" (int status)
        }

    ///<summary>
    ///Create a credit top-up for the given account, charged to the account's default payment method.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingCreateTopup
        (accountId: string, body: AigBillingCreateTopupPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/topup"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingCreateTopup.OK((Serializer.deserialize content))
            | 400 -> return AigBillingCreateTopup.BadRequest((Serializer.deserialize content))
            | 402 -> return AigBillingCreateTopup.PaymentRequired((Serializer.deserialize content))
            | 403 -> return AigBillingCreateTopup.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingCreateTopup.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingCreateTopup" (int status)
        }

    ///<summary>
    ///Remove the auto top-up configuration for the account.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingDeleteTopupConfig(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/topup/config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingDeleteTopupConfig.OK((Serializer.deserialize content))
            | 400 -> return AigBillingDeleteTopupConfig.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingDeleteTopupConfig.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingDeleteTopupConfig.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingDeleteTopupConfig" (int status)
        }

    ///<summary>
    ///Retrieve the current auto top-up threshold, amount, and any error state.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingGetTopupConfig(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/topup/config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingGetTopupConfig.OK((Serializer.deserialize content))
            | 400 -> return AigBillingGetTopupConfig.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingGetTopupConfig.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingGetTopupConfig.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingGetTopupConfig" (int status)
        }

    ///<summary>
    ///Configure auto top-up with a balance threshold and top-up amount.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingSetTopupConfig
        (accountId: string, body: AigBillingSetTopupConfigPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/topup/config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingSetTopupConfig.OK((Serializer.deserialize content))
            | 400 -> return AigBillingSetTopupConfig.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingSetTopupConfig.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingSetTopupConfig.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingSetTopupConfig" (int status)
        }

    ///<summary>
    ///Determine whether an account can self-serve a credit top-up, and if not, why. The dashboard forwards the account's payment-method list in the body; the reason logic is owned here.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AigBillingTopupEligibility
        (accountId: string, ?cancellationToken: CancellationToken, ?body: AigBillingTopupEligibilityPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/topup/eligibility"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingTopupEligibility.OK((Serializer.deserialize content))
            | 400 -> return AigBillingTopupEligibility.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingTopupEligibility.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingTopupEligibility.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingTopupEligibility" (int status)
        }

    ///<summary>
    ///Retrieve the minimum and maximum allowed top-up amounts (in cents) for this account.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingGetTopupLimits(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/topup/limits"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingGetTopupLimits.OK((Serializer.deserialize content))
            | 400 -> return AigBillingGetTopupLimits.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingGetTopupLimits.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingGetTopupLimits.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingGetTopupLimits" (int status)
        }

    ///<summary>
    ///Get the payment processing status of a top-up by its invoice ID.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingCheckTopupStatus
        (accountId: string, body: AigBillingCheckTopupStatusPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/topup/status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingCheckTopupStatus.OK((Serializer.deserialize content))
            | 400 -> return AigBillingCheckTopupStatus.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingCheckTopupStatus.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingCheckTopupStatus.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingCheckTopupStatus" (int status)
        }

    ///<summary>
    ///Retrieve aggregated usage meter event summaries for the given time range.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="valueGroupingWindow">Grouping window for usage data.</param>
    ///<param name="startTime">Start time as Unix timestamp in milliseconds.</param>
    ///<param name="endTime">End time as Unix timestamp in milliseconds.</param>
    ///<param name="cancellationToken"></param>
    member this.AigBillingGetUsageHistory
        (
            accountId: string,
            valueGroupingWindow: string,
            ?startTime: float,
            ?endTime: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.query ("value_grouping_window", valueGroupingWindow)
                  if startTime.IsSome then
                      RequestPart.query ("start_time", startTime.Value)
                  if endTime.IsSome then
                      RequestPart.query ("end_time", endTime.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/billing/usage-history"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigBillingGetUsageHistory.OK((Serializer.deserialize content))
            | 400 -> return AigBillingGetUsageHistory.BadRequest((Serializer.deserialize content))
            | 403 -> return AigBillingGetUsageHistory.Forbidden((Serializer.deserialize content))
            | 404 -> return AigBillingGetUsageHistory.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigBillingGetUsageHistory" (int status)
        }

    ///<summary>
    ///Lists all AI Gateway evaluator types configured for the account.
    ///</summary>
    member this.AigConfigListAccountProvider
        (
            accountId: string,
            ?page: int,
            ?perPage: int,
            ?beta: bool,
            ?enable: bool,
            ?search: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if beta.IsSome then
                      RequestPart.query ("beta", beta.Value)
                  if enable.IsSome then
                      RequestPart.query ("enable", enable.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/custom-providers"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListAccountProvider.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListAccountProvider.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigListAccountProvider" (int status)
        }

    ///<summary>
    ///Creates a new AI Gateway.
    ///</summary>
    member this.AigConfigCreateAccountProvider
        (accountId: string, ?cancellationToken: CancellationToken, ?body: AigConfigCreateAccountProviderPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/custom-providers"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigCreateAccountProvider.OK((Serializer.deserialize content))
            | 400 -> return AigConfigCreateAccountProvider.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigCreateAccountProvider" (int status)
        }

    ///<summary>
    ///Lists all AI Gateway evaluator types configured for the account.
    ///</summary>
    member this.AigConfigListAccountProviderCost
        (
            accountId: string,
            ?page: int,
            ?perPage: int,
            ?enable: bool,
            ?accountProviderId: System.Guid,
            ?modelRule: string,
            ?costType: string,
            ?search: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if enable.IsSome then
                      RequestPart.query ("enable", enable.Value)
                  if accountProviderId.IsSome then
                      RequestPart.query ("account_provider_id", accountProviderId.Value)
                  if modelRule.IsSome then
                      RequestPart.query ("model_rule", modelRule.Value)
                  if costType.IsSome then
                      RequestPart.query ("cost_type", costType.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/custom-providers/costs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListAccountProviderCost.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListAccountProviderCost.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigListAccountProviderCost" (int status)
        }

    ///<summary>
    ///Creates a new AI Gateway.
    ///</summary>
    member this.AigConfigCreateAccountProviderCost
        (accountId: string, ?cancellationToken: CancellationToken, ?body: AigConfigCreateAccountProviderCostPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/custom-providers/costs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigCreateAccountProviderCost.OK((Serializer.deserialize content))
            | 400 -> return AigConfigCreateAccountProviderCost.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigCreateAccountProviderCost" (int status)
        }

    ///<summary>
    ///Deletes an AI Gateway dataset.
    ///</summary>
    member this.AigConfigDeleteAccountProviderCost
        (accountId: string, id: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/custom-providers/costs/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigDeleteAccountProviderCost.OK((Serializer.deserialize content))
            | 404 -> return AigConfigDeleteAccountProviderCost.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigDeleteAccountProviderCost" (int status)
        }

    ///<summary>
    ///Retrieves details for a specific AI Gateway dataset.
    ///</summary>
    member this.AigConfigFetchAccountProviderCost
        (accountId: string, id: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/custom-providers/costs/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigFetchAccountProviderCost.OK((Serializer.deserialize content))
            | 404 -> return AigConfigFetchAccountProviderCost.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigFetchAccountProviderCost" (int status)
        }

    ///<summary>
    ///Updates an existing AI Gateway dataset.
    ///</summary>
    member this.AigConfigUpdateAccountProviderCost
        (
            accountId: string,
            id: System.Guid,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigUpdateAccountProviderCostPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/custom-providers/costs/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigUpdateAccountProviderCost.OK((Serializer.deserialize content))
            | 400 -> return AigConfigUpdateAccountProviderCost.BadRequest((Serializer.deserialize content))
            | 404 -> return AigConfigUpdateAccountProviderCost.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigUpdateAccountProviderCost" (int status)
        }

    ///<summary>
    ///Deletes an AI Gateway dataset.
    ///</summary>
    member this.AigConfigDeleteAccountProvider
        (accountId: string, id: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/custom-providers/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigDeleteAccountProvider.OK((Serializer.deserialize content))
            | 404 -> return AigConfigDeleteAccountProvider.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigDeleteAccountProvider" (int status)
        }

    ///<summary>
    ///Retrieves details for a specific AI Gateway dataset.
    ///</summary>
    member this.AigConfigFetchAccountProvider
        (accountId: string, id: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/custom-providers/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigFetchAccountProvider.OK((Serializer.deserialize content))
            | 404 -> return AigConfigFetchAccountProvider.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigFetchAccountProvider" (int status)
        }

    ///<summary>
    ///Updates an existing AI Gateway dataset.
    ///</summary>
    member this.AigConfigUpdateAccountProvider
        (
            accountId: string,
            id: System.Guid,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigUpdateAccountProviderPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/custom-providers/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigUpdateAccountProvider.OK((Serializer.deserialize content))
            | 400 -> return AigConfigUpdateAccountProvider.BadRequest((Serializer.deserialize content))
            | 404 -> return AigConfigUpdateAccountProvider.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigUpdateAccountProvider" (int status)
        }

    ///<summary>
    ///Lists all available evaluator types for scoring AI gateway responses.
    ///</summary>
    member this.AigConfigListEvaluators
        (
            accountId: string,
            ?page: int,
            ?perPage: int,
            ?orderBy: string,
            ?orderByDirection: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if orderByDirection.IsSome then
                      RequestPart.query ("order_by_direction", orderByDirection.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/evaluation-types"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListEvaluators.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListEvaluators.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigListEvaluators" (int status)
        }

    ///<summary>
    ///Lists all AI Gateway evaluator types configured for the account.
    ///</summary>
    member this.AigConfigListGateway
        (accountId: string, ?page: int, ?perPage: int, ?search: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListGateway.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListGateway.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigListGateway" (int status)
        }

    ///<summary>
    ///Creates a new AI Gateway.
    ///</summary>
    member this.AigConfigCreateGateway
        (accountId: string, ?cancellationToken: CancellationToken, ?body: AigConfigCreateGatewayPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigCreateGateway.OK((Serializer.deserialize content))
            | 400 -> return AigConfigCreateGateway.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigCreateGateway" (int status)
        }

    ///<summary>
    ///Lists all AI Gateway evaluator types configured for the account.
    ///</summary>
    member this.AigConfigListCustomDomain
        (
            accountId: string,
            gatewayId: string,
            ?page: int,
            ?perPage: int,
            ?status: string,
            ?search: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/custom-domains"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListCustomDomain.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListCustomDomain.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigListCustomDomain" (int status)
        }

    ///<summary>
    ///Provisions a Cloudflare-for-SaaS custom hostname and returns the CNAME target to point DNS at.
    ///</summary>
    member this.AigConfigCreateCustomDomain
        (
            accountId: string,
            gatewayId: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigCreateCustomDomainPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/custom-domains"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigCreateCustomDomain.OK((Serializer.deserialize content))
            | 400 -> return AigConfigCreateCustomDomain.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigCreateCustomDomain" (int status)
        }

    ///<summary>
    ///Deletes an AI Gateway dataset.
    ///</summary>
    member this.AigConfigDeleteCustomDomain
        (accountId: string, gatewayId: string, hostname: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("hostname", hostname) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/custom-domains/{hostname}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigDeleteCustomDomain.OK((Serializer.deserialize content))
            | 404 -> return AigConfigDeleteCustomDomain.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigDeleteCustomDomain" (int status)
        }

    ///<summary>
    ///Retrieves details for a specific AI Gateway dataset.
    ///</summary>
    member this.AigConfigFetchCustomDomain
        (accountId: string, gatewayId: string, hostname: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("hostname", hostname) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/custom-domains/{hostname}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigFetchCustomDomain.OK((Serializer.deserialize content))
            | 404 -> return AigConfigFetchCustomDomain.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigFetchCustomDomain" (int status)
        }

    ///<summary>
    ///Lists all AI Gateway evaluator types configured for the account.
    ///</summary>
    member this.AigConfigListDataset
        (
            accountId: string,
            gatewayId: string,
            ?page: int,
            ?perPage: int,
            ?name: string,
            ?enable: bool,
            ?search: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if enable.IsSome then
                      RequestPart.query ("enable", enable.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/datasets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListDataset.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListDataset.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigListDataset" (int status)
        }

    ///<summary>
    ///Creates a new AI Gateway.
    ///</summary>
    member this.AigConfigCreateDataset
        (
            gatewayId: string,
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigCreateDatasetPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/datasets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigCreateDataset.OK((Serializer.deserialize content))
            | 400 -> return AigConfigCreateDataset.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigCreateDataset" (int status)
        }

    ///<summary>
    ///Deletes an AI Gateway dataset.
    ///</summary>
    member this.AigConfigDeleteDataset
        (accountId: string, gatewayId: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/datasets/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigDeleteDataset.OK((Serializer.deserialize content))
            | 404 -> return AigConfigDeleteDataset.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigDeleteDataset" (int status)
        }

    ///<summary>
    ///Retrieves details for a specific AI Gateway dataset.
    ///</summary>
    member this.AigConfigFetchDataset
        (accountId: string, gatewayId: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/datasets/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigFetchDataset.OK((Serializer.deserialize content))
            | 404 -> return AigConfigFetchDataset.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigFetchDataset" (int status)
        }

    ///<summary>
    ///Updates an existing AI Gateway dataset.
    ///</summary>
    member this.AigConfigUpdateDataset
        (
            accountId: string,
            gatewayId: string,
            id: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigUpdateDatasetPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/datasets/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigUpdateDataset.OK((Serializer.deserialize content))
            | 400 -> return AigConfigUpdateDataset.BadRequest((Serializer.deserialize content))
            | 404 -> return AigConfigUpdateDataset.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigUpdateDataset" (int status)
        }

    ///<summary>
    ///Lists all AI Gateway evaluator types configured for the account.
    ///</summary>
    member this.AigConfigListEvaluations
        (
            accountId: string,
            gatewayId: string,
            ?page: int,
            ?perPage: int,
            ?name: string,
            ?processed: bool,
            ?search: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if processed.IsSome then
                      RequestPart.query ("processed", processed.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/evaluations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListEvaluations.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListEvaluations.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigListEvaluations" (int status)
        }

    ///<summary>
    ///Creates a new AI Gateway.
    ///</summary>
    member this.AigConfigCreateEvaluations
        (
            gatewayId: string,
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigCreateEvaluationsPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/evaluations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigCreateEvaluations.OK((Serializer.deserialize content))
            | 400 -> return AigConfigCreateEvaluations.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigCreateEvaluations" (int status)
        }

    ///<summary>
    ///Deletes an AI Gateway dataset.
    ///</summary>
    member this.AigConfigDeleteEvaluations
        (accountId: string, gatewayId: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/evaluations/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigDeleteEvaluations.OK((Serializer.deserialize content))
            | 404 -> return AigConfigDeleteEvaluations.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigDeleteEvaluations" (int status)
        }

    ///<summary>
    ///Retrieves details for a specific AI Gateway dataset.
    ///</summary>
    member this.AigConfigFetchEvaluations
        (accountId: string, gatewayId: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/evaluations/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigFetchEvaluations.OK((Serializer.deserialize content))
            | 404 -> return AigConfigFetchEvaluations.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigFetchEvaluations" (int status)
        }

    ///<summary>
    ///Deletes gateway log entries matching the specified criteria.
    ///</summary>
    member this.AigConfigDeleteGatewayLogs
        (
            accountId: string,
            gatewayId: string,
            ?orderBy: string,
            ?orderByDirection: string,
            ?filters: list<System.Text.Json.Nodes.JsonObject>,
            ?limit: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if orderByDirection.IsSome then
                      RequestPart.query ("order_by_direction", orderByDirection.Value)
                  if filters.IsSome then
                      RequestPart.queryDotted ("filters", filters.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/logs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigDeleteGatewayLogs.OK((Serializer.deserialize content))
            | 400 -> return AigConfigDeleteGatewayLogs.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigDeleteGatewayLogs" (int status)
        }

    ///<summary>
    ///Lists request/response log entries for the AI gateway with filtering and pagination.
    ///</summary>
    member this.AigConfigListGatewayLogs
        (
            accountId: string,
            gatewayId: string,
            ?search: string,
            ?page: int,
            ?perPage: int,
            ?orderBy: string,
            ?orderByDirection: string,
            ?filters: list<System.Text.Json.Nodes.JsonObject>,
            ?metaInfo: bool,
            ?direction: string,
            ?startDate: System.DateTimeOffset,
            ?endDate: System.DateTimeOffset,
            ?minCost: float,
            ?maxCost: float,
            ?minTokensIn: float,
            ?maxTokensIn: float,
            ?minTokensOut: float,
            ?maxTokensOut: float,
            ?minTotalTokens: float,
            ?maxTotalTokens: float,
            ?minDuration: float,
            ?maxDuration: float,
            ?feedback: InlineUnion_84f40b027476b6dd90ade672,
            ?success: bool,
            ?cached: bool,
            ?model: string,
            ?modelType: string,
            ?provider: string,
            ?requestContentType: string,
            ?responseContentType: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if orderByDirection.IsSome then
                      RequestPart.query ("order_by_direction", orderByDirection.Value)
                  if filters.IsSome then
                      RequestPart.queryDotted ("filters", filters.Value)
                  if metaInfo.IsSome then
                      RequestPart.query ("meta_info", metaInfo.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if startDate.IsSome then
                      RequestPart.query ("start_date", startDate.Value)
                  if endDate.IsSome then
                      RequestPart.query ("end_date", endDate.Value)
                  if minCost.IsSome then
                      RequestPart.query ("min_cost", minCost.Value)
                  if maxCost.IsSome then
                      RequestPart.query ("max_cost", maxCost.Value)
                  if minTokensIn.IsSome then
                      RequestPart.query ("min_tokens_in", minTokensIn.Value)
                  if maxTokensIn.IsSome then
                      RequestPart.query ("max_tokens_in", maxTokensIn.Value)
                  if minTokensOut.IsSome then
                      RequestPart.query ("min_tokens_out", minTokensOut.Value)
                  if maxTokensOut.IsSome then
                      RequestPart.query ("max_tokens_out", maxTokensOut.Value)
                  if minTotalTokens.IsSome then
                      RequestPart.query ("min_total_tokens", minTotalTokens.Value)
                  if maxTotalTokens.IsSome then
                      RequestPart.query ("max_total_tokens", maxTotalTokens.Value)
                  if minDuration.IsSome then
                      RequestPart.query ("min_duration", minDuration.Value)
                  if maxDuration.IsSome then
                      RequestPart.query ("max_duration", maxDuration.Value)
                  if feedback.IsSome then
                      RequestPart.query ("feedback", feedback.Value)
                  if success.IsSome then
                      RequestPart.query ("success", success.Value)
                  if cached.IsSome then
                      RequestPart.query ("cached", cached.Value)
                  if model.IsSome then
                      RequestPart.query ("model", model.Value)
                  if modelType.IsSome then
                      RequestPart.query ("model_type", modelType.Value)
                  if provider.IsSome then
                      RequestPart.query ("provider", provider.Value)
                  if requestContentType.IsSome then
                      RequestPart.query ("request_content_type", requestContentType.Value)
                  if responseContentType.IsSome then
                      RequestPart.query ("response_content_type", responseContentType.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/logs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListGatewayLogs.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListGatewayLogs.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigListGatewayLogs" (int status)
        }

    ///<summary>
    ///Retrieves detailed information for a specific AI Gateway log entry.
    ///</summary>
    member this.AigConfigGetGatewayLogDetail
        (id: string, gatewayId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/logs/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigGetGatewayLogDetail.OK((Serializer.deserialize content))
            | 404 -> return AigConfigGetGatewayLogDetail.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigGetGatewayLogDetail" (int status)
        }

    ///<summary>
    ///Updates metadata for an AI Gateway log entry.
    ///</summary>
    member this.AigConfigPatchGatewayLog
        (
            id: string,
            gatewayId: string,
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigPatchGatewayLogPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/logs/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigPatchGatewayLog.OK((Serializer.deserialize content))
            | 404 -> return AigConfigPatchGatewayLog.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigPatchGatewayLog" (int status)
        }

    ///<summary>
    ///Retrieves the original request payload for an AI Gateway log entry.
    ///</summary>
    member this.AigConfigGetGatewayLogRequest
        (id: string, gatewayId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/logs/{id}/request"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigGetGatewayLogRequest.OK((Serializer.deserialize content))
            | 404 -> return AigConfigGetGatewayLogRequest.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigGetGatewayLogRequest" (int status)
        }

    ///<summary>
    ///Retrieves the response payload for an AI Gateway log entry.
    ///</summary>
    member this.AigConfigGetGatewayLogResponse
        (id: string, gatewayId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/logs/{id}/response"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigGetGatewayLogResponse.OK((Serializer.deserialize content))
            | 404 -> return AigConfigGetGatewayLogResponse.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigGetGatewayLogResponse" (int status)
        }

    ///<summary>
    ///Lists all AI Gateway evaluator types configured for the account.
    ///</summary>
    member this.AigConfigListProviders
        (accountId: string, gatewayId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/provider_configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListProviders.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListProviders.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigListProviders" (int status)
        }

    ///<summary>
    ///Creates a new AI Gateway.
    ///</summary>
    member this.AigConfigCreateProviders
        (
            accountId: string,
            gatewayId: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigCreateProvidersPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/provider_configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigCreateProviders.OK((Serializer.deserialize content))
            | 400 -> return AigConfigCreateProviders.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigCreateProviders" (int status)
        }

    ///<summary>
    ///Deletes an AI Gateway dataset.
    ///</summary>
    member this.AigConfigDeleteProviders
        (accountId: string, gatewayId: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/provider_configs/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigDeleteProviders.OK((Serializer.deserialize content))
            | 404 -> return AigConfigDeleteProviders.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigDeleteProviders" (int status)
        }

    ///<summary>
    ///Updates an existing AI Gateway dataset.
    ///</summary>
    member this.AigConfigUpdateProviders
        (
            accountId: string,
            gatewayId: string,
            id: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigUpdateProvidersPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/provider_configs/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigUpdateProviders.OK((Serializer.deserialize content))
            | 400 -> return AigConfigUpdateProviders.BadRequest((Serializer.deserialize content))
            | 404 -> return AigConfigUpdateProviders.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigUpdateProviders" (int status)
        }

    ///<summary>
    ///List all AI Gateway Dynamic Routes.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="gatewayId"></param>
    ///<param name="page">Page number</param>
    ///<param name="perPage">Number of routes per page</param>
    ///<param name="cancellationToken"></param>
    member this.AigConfigListGatewayDynamicRoutes
        (accountId: string, gatewayId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/routes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListGatewayDynamicRoutes.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListGatewayDynamicRoutes.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigListGatewayDynamicRoutes" (int status)
        }

    ///<summary>
    ///Create a new AI Gateway Dynamic Route.
    ///</summary>
    member this.AigConfigPostGatewayDynamicRoute
        (
            accountId: string,
            gatewayId: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigPostGatewayDynamicRoutePayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/routes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigPostGatewayDynamicRoute.OK((Serializer.deserialize content))
            | 400 -> return AigConfigPostGatewayDynamicRoute.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigPostGatewayDynamicRoute" (int status)
        }

    ///<summary>
    ///Delete an AI Gateway Dynamic Route.
    ///</summary>
    member this.AigConfigDeleteGatewayDynamicRoute
        (accountId: string, gatewayId: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/routes/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigDeleteGatewayDynamicRoute.OK((Serializer.deserialize content))
            | 400 -> return AigConfigDeleteGatewayDynamicRoute.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigDeleteGatewayDynamicRoute" (int status)
        }

    ///<summary>
    ///Get an AI Gateway Dynamic Route.
    ///</summary>
    member this.AigConfigGetGatewayDynamicRoute
        (accountId: string, gatewayId: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/routes/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigGetGatewayDynamicRoute.OK((Serializer.deserialize content))
            | 400 -> return AigConfigGetGatewayDynamicRoute.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigGetGatewayDynamicRoute" (int status)
        }

    ///<summary>
    ///Update an AI Gateway Dynamic Route.
    ///</summary>
    member this.AigConfigUpdateGatewayDynamicRoute
        (
            accountId: string,
            gatewayId: string,
            id: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigUpdateGatewayDynamicRoutePayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/routes/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigUpdateGatewayDynamicRoute.OK((Serializer.deserialize content))
            | 400 -> return AigConfigUpdateGatewayDynamicRoute.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigUpdateGatewayDynamicRoute" (int status)
        }

    ///<summary>
    ///List all AI Gateway Dynamic Route Deployments.
    ///</summary>
    member this.AigConfigListGatewayDynamicRouteDeployments
        (accountId: string, gatewayId: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/routes/{id}/deployments"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListGatewayDynamicRouteDeployments.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListGatewayDynamicRouteDeployments.BadRequest((Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for AigConfigListGatewayDynamicRouteDeployments" (int status)
        }

    ///<summary>
    ///Create a new AI Gateway Dynamic Route Deployment.
    ///</summary>
    member this.AigConfigPostGatewayDynamicRouteDeployment
        (
            accountId: string,
            gatewayId: string,
            id: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigPostGatewayDynamicRouteDeploymentPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/routes/{id}/deployments"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigPostGatewayDynamicRouteDeployment.OK((Serializer.deserialize content))
            | 400 -> return AigConfigPostGatewayDynamicRouteDeployment.BadRequest((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AigConfigPostGatewayDynamicRouteDeployment" (int status)
        }

    ///<summary>
    ///List all AI Gateway Dynamic Route Versions.
    ///</summary>
    member this.AigConfigListGatewayDynamicRouteVersions
        (accountId: string, gatewayId: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/routes/{id}/versions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigListGatewayDynamicRouteVersions.OK((Serializer.deserialize content))
            | 400 -> return AigConfigListGatewayDynamicRouteVersions.BadRequest((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AigConfigListGatewayDynamicRouteVersions" (int status)
        }

    ///<summary>
    ///Create a new AI Gateway Dynamic Route Version.
    ///</summary>
    member this.AigConfigPostGatewayDynamicRouteVersion
        (
            accountId: string,
            gatewayId: string,
            id: string,
            ?cancellationToken: CancellationToken,
            ?body: AigConfigPostGatewayDynamicRouteVersionPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/routes/{id}/versions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigPostGatewayDynamicRouteVersion.OK((Serializer.deserialize content))
            | 400 -> return AigConfigPostGatewayDynamicRouteVersion.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigPostGatewayDynamicRouteVersion" (int status)
        }

    ///<summary>
    ///Get an AI Gateway Dynamic Route Version.
    ///</summary>
    member this.AigConfigGetGatewayDynamicRouteVersion
        (accountId: string, gatewayId: string, id: string, versionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("id", id)
                  RequestPart.path ("version_id", versionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/routes/{id}/versions/{version_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigGetGatewayDynamicRouteVersion.OK((Serializer.deserialize content))
            | 400 -> return AigConfigGetGatewayDynamicRouteVersion.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigGetGatewayDynamicRouteVersion" (int status)
        }

    ///<summary>
    ///Retrieves the endpoint URL for an AI Gateway.
    ///</summary>
    member this.AigConfigGetGatewayUrl
        (gatewayId: string, accountId: string, provider: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("gateway_id", gatewayId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("provider", provider) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{gateway_id}/url/{provider}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigGetGatewayUrl.OK((Serializer.deserialize content))
            | 400 -> return AigConfigGetGatewayUrl.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigGetGatewayUrl" (int status)
        }

    ///<summary>
    ///Deletes an AI Gateway dataset.
    ///</summary>
    member this.AigConfigDeleteGateway(accountId: string, id: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigDeleteGateway.OK((Serializer.deserialize content))
            | 404 -> return AigConfigDeleteGateway.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigDeleteGateway" (int status)
        }

    ///<summary>
    ///Retrieves details for a specific AI Gateway dataset.
    ///</summary>
    member this.AigConfigFetchGateway(accountId: string, id: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigFetchGateway.OK((Serializer.deserialize content))
            | 404 -> return AigConfigFetchGateway.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigFetchGateway" (int status)
        }

    ///<summary>
    ///Updates an existing AI Gateway dataset.
    ///</summary>
    member this.AigConfigUpdateGateway
        (accountId: string, id: string, ?cancellationToken: CancellationToken, ?body: AigConfigUpdateGatewayPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/gateways/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigUpdateGateway.OK((Serializer.deserialize content))
            | 400 -> return AigConfigUpdateGateway.BadRequest((Serializer.deserialize content))
            | 404 -> return AigConfigUpdateGateway.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigUpdateGateway" (int status)
        }

    ///<summary>
    ///Returns the canonical logging platform and migration availability for an account.
    ///</summary>
    member this.AigConfigGetLoggingState(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/logging-state"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigGetLoggingState.OK((Serializer.deserialize content))
            | 400 -> return AigConfigGetLoggingState.BadRequest((Serializer.deserialize content))
            | 503 -> return AigConfigGetLoggingState.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigGetLoggingState" (int status)
        }

    ///<summary>
    ///Irreversibly migrates an eligible account to Workers Observability logging.
    ///</summary>
    member this.AigConfigPatchLoggingState(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/ai-gateway/logging-state"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AigConfigPatchLoggingState.OK((Serializer.deserialize content))
            | 409 -> return AigConfigPatchLoggingState.Conflict((Serializer.deserialize content))
            | 503 -> return AigConfigPatchLoggingState.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AigConfigPatchLoggingState" (int status)
        }

    ///<summary>
    ///List all AI Search instances in the account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="page">Page number (1-indexed).</param>
    ///<param name="perPage">Number of results per page.</param>
    ///<param name="search">Filter instances whose id contains this string (case-insensitive).</param>
    ///<param name="namespace">Filter by namespace.</param>
    ///<param name="orderBy">Field to order results by.</param>
    ///<param name="orderByDirection">Order direction.</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchListInstances
        (
            accountId: string,
            ?page: int,
            ?perPage: int,
            ?search: string,
            ?``namespace``: string,
            ?orderBy: string,
            ?orderByDirection: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if ``namespace``.IsSome then
                      RequestPart.query ("namespace", ``namespace``.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if orderByDirection.IsSome then
                      RequestPart.query ("order_by_direction", orderByDirection.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchListInstances.OK((Serializer.deserialize content))
            | 400 -> return AiSearchListInstances.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchListInstances" (int status)
        }

    ///<summary>
    ///Create a new AI Search instance with the given configuration.
    ///</summary>
    member this.AiSearchCreateInstance
        (accountId: string, ?cancellationToken: CancellationToken, ?body: AiSearchCreateInstancePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return AiSearchCreateInstance.Created((Serializer.deserialize content))
            | 400 -> return AiSearchCreateInstance.BadRequest((Serializer.deserialize content))
            | 403 -> return AiSearchCreateInstance.Forbidden((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchCreateInstance" (int status)
        }

    ///<summary>
    ///Permanently delete an AI Search instance and all its indexed data.
    ///</summary>
    member this.AiSearchDeleteInstance(accountId: string, id: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchDeleteInstance.OK((Serializer.deserialize content))
            | 404 -> return AiSearchDeleteInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchDeleteInstance" (int status)
        }

    ///<summary>
    ///Retrieve the configuration and status of an AI Search instance.
    ///</summary>
    member this.AiSearchFetchInstance(accountId: string, id: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchFetchInstance.OK((Serializer.deserialize content))
            | 404 -> return AiSearchFetchInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchFetchInstance" (int status)
        }

    ///<summary>
    ///Update the configuration of an AI Search instance.
    ///</summary>
    member this.AiSearchUpdateInstance
        (accountId: string, id: string, ?cancellationToken: CancellationToken, ?body: AiSearchUpdateInstancePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchUpdateInstance.OK((Serializer.deserialize content))
            | 400 -> return AiSearchUpdateInstance.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchUpdateInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchUpdateInstance" (int status)
        }

    ///<summary>
    ///Performs a chat completion request against an AI Search instance, using indexed content as context for generating responses.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchInstanceChatCompletion
        (
            id: string,
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchInstanceChatCompletionPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}/chat/completions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchInstanceChatCompletion.OK((Serializer.deserialize content))
            | 400 -> return AiSearchInstanceChatCompletion.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchInstanceChatCompletion.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchInstanceChatCompletion" (int status)
        }

    ///<summary>
    ///Lists indexing jobs for an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchInstanceListJobs
        (id: string, accountId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}/jobs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchInstanceListJobs.OK((Serializer.deserialize content))
            | 400 -> return AiSearchInstanceListJobs.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchInstanceListJobs.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchInstanceListJobs.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchInstanceListJobs" (int status)
        }

    ///<summary>
    ///Creates a new indexing job for an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchInstanceCreateJob
        (id: string, accountId: string, ?cancellationToken: CancellationToken, ?body: AiSearchInstanceCreateJobPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}/jobs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchInstanceCreateJob.OK((Serializer.deserialize content))
            | 400 -> return AiSearchInstanceCreateJob.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchInstanceCreateJob.NotFound((Serializer.deserialize content))
            | 429 -> return AiSearchInstanceCreateJob.TooManyRequests((Serializer.deserialize content))
            | 503 -> return AiSearchInstanceCreateJob.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchInstanceCreateJob" (int status)
        }

    ///<summary>
    ///Retrieves details for a specific AI Search indexing job.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="jobId"></param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchInstanceGetJob
        (id: string, jobId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("job_id", jobId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}/jobs/{job_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchInstanceGetJob.OK((Serializer.deserialize content))
            | 404 -> return AiSearchInstanceGetJob.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchInstanceGetJob.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchInstanceGetJob" (int status)
        }

    ///<summary>
    ///Cancel an in-progress indexing job for an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="jobId"></param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchInstanceChangeJobStatus
        (
            id: string,
            jobId: string,
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchInstanceChangeJobStatusPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("job_id", jobId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}/jobs/{job_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchInstanceChangeJobStatus.OK((Serializer.deserialize content))
            | 400 -> return AiSearchInstanceChangeJobStatus.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchInstanceChangeJobStatus.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchInstanceChangeJobStatus.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchInstanceChangeJobStatus" (int status)
        }

    ///<summary>
    ///Lists log entries for an AI Search indexing job.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="jobId"></param>
    ///<param name="accountId"></param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchInstanceListJobLogs
        (id: string, jobId: string, accountId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("job_id", jobId)
                  RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}/jobs/{job_id}/logs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchInstanceListJobLogs.OK((Serializer.deserialize content))
            | 400 -> return AiSearchInstanceListJobLogs.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchInstanceListJobLogs.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchInstanceListJobLogs.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchInstanceListJobLogs" (int status)
        }

    ///<summary>
    ///Executes a semantic search query against an AI Search instance to find relevant indexed content.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchInstanceSearch
        (id: string, accountId: string, ?cancellationToken: CancellationToken, ?body: AiSearchInstanceSearchPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}/search"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchInstanceSearch.OK((Serializer.deserialize content))
            | 400 -> return AiSearchInstanceSearch.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchInstanceSearch.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchInstanceSearch" (int status)
        }

    ///<summary>
    ///Retrieve usage and indexing statistics for an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchStats(id: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id); RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/instances/{id}/stats"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchStats.OK((Serializer.deserialize content))
            | 404 -> return AiSearchStats.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchStats" (int status)
        }

    ///<summary>
    ///List namespaces in the account, including their descriptions and creation times.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="page">Page number (1-indexed).</param>
    ///<param name="perPage">Number of results per page.</param>
    ///<param name="search">Filter namespaces whose name or description contains this string (case-insensitive).</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchListNamespaces
        (accountId: string, ?page: int, ?perPage: int, ?search: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchListNamespaces.OK((Serializer.deserialize content))
            | 400 -> return AiSearchListNamespaces.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchListNamespaces" (int status)
        }

    ///<summary>
    ///Create a namespace for organizing AI Search instances.
    ///</summary>
    member this.AiSearchCreateNamespace
        (accountId: string, ?cancellationToken: CancellationToken, ?body: AiSearchCreateNamespacePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return AiSearchCreateNamespace.Created((Serializer.deserialize content))
            | 400 -> return AiSearchCreateNamespace.BadRequest((Serializer.deserialize content))
            | 403 -> return AiSearchCreateNamespace.Forbidden((Serializer.deserialize content))
            | 409 -> return AiSearchCreateNamespace.Conflict((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchCreateNamespace" (int status)
        }

    ///<summary>
    ///Permanently delete a namespace. The namespace must be empty (no instances), and the default namespace cannot be deleted.
    ///</summary>
    member this.AiSearchDeleteNamespace(accountId: string, name: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchDeleteNamespace.OK((Serializer.deserialize content))
            | 400 -> return AiSearchDeleteNamespace.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchDeleteNamespace.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchDeleteNamespace" (int status)
        }

    ///<summary>
    ///Retrieve a namespace and its description.
    ///</summary>
    member this.AiSearchFetchNamespace(accountId: string, name: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchFetchNamespace.OK((Serializer.deserialize content))
            | 404 -> return AiSearchFetchNamespace.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchFetchNamespace" (int status)
        }

    ///<summary>
    ///Update the description and/or the public endpoint configuration of an existing namespace. The default namespace's description cannot be modified, but its public endpoint can.
    ///</summary>
    member this.AiSearchUpdateNamespace
        (accountId: string, name: string, ?cancellationToken: CancellationToken, ?body: AiSearchUpdateNamespacePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchUpdateNamespace.OK((Serializer.deserialize content))
            | 400 -> return AiSearchUpdateNamespace.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchUpdateNamespace.NotFound((Serializer.deserialize content))
            | 409 -> return AiSearchUpdateNamespace.Conflict((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchUpdateNamespace" (int status)
        }

    ///<summary>
    ///Performs a chat completion request against multiple AI Search instances in parallel, merging retrieved content as context for generating a response.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchNamespaceMultiInstanceChatCompletion
        (
            accountId: string,
            name: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchNamespaceMultiInstanceChatCompletionPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/chat/completions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceMultiInstanceChatCompletion.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceMultiInstanceChatCompletion.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceMultiInstanceChatCompletion.NotFound((Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for AiSearchNamespaceMultiInstanceChatCompletion" (int status)
        }

    ///<summary>
    ///List all AI Search instances in the account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="page">Page number (1-indexed).</param>
    ///<param name="perPage">Number of results per page.</param>
    ///<param name="search">Filter instances whose id contains this string (case-insensitive).</param>
    ///<param name="namespace">Filter by namespace.</param>
    ///<param name="orderBy">Field to order results by.</param>
    ///<param name="orderByDirection">Order direction.</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceListInstances
        (
            accountId: string,
            name: string,
            ?page: int,
            ?perPage: int,
            ?search: string,
            ?``namespace``: string,
            ?orderBy: string,
            ?orderByDirection: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if ``namespace``.IsSome then
                      RequestPart.query ("namespace", ``namespace``.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if orderByDirection.IsSome then
                      RequestPart.query ("order_by_direction", orderByDirection.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceListInstances.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceListInstances.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceListInstances" (int status)
        }

    ///<summary>
    ///Create a new AI Search instance with the given configuration.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchNamespaceCreateInstance
        (
            accountId: string,
            name: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchNamespaceCreateInstancePayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return AiSearchNamespaceCreateInstance.Created((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceCreateInstance.BadRequest((Serializer.deserialize content))
            | 403 -> return AiSearchNamespaceCreateInstance.Forbidden((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceCreateInstance" (int status)
        }

    ///<summary>
    ///Permanently delete an AI Search instance and all its indexed data.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="id"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceDeleteInstance
        (accountId: string, id: string, name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceDeleteInstance.OK((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceDeleteInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceDeleteInstance" (int status)
        }

    ///<summary>
    ///Retrieve the configuration and status of an AI Search instance.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="id"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceFetchInstance
        (accountId: string, id: string, name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceFetchInstance.OK((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceFetchInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceFetchInstance" (int status)
        }

    ///<summary>
    ///Moves an instance from its current namespace to the specified target namespace. Use 'default' with --destination-namespace to move the instance back to the default namespace. Fails with 400 if the target namespace already has an instance with the same id (ids must be unique within a namespace — the same id can exist in different namespaces).
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="name">Current namespace of the instance.</param>
    ///<param name="id">Instance id.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchMoveInstance
        (
            accountId: string,
            name: string,
            id: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchMoveInstancePayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchMoveInstance.OK((Serializer.deserialize content))
            | 400 -> return AiSearchMoveInstance.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchMoveInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchMoveInstance" (int status)
        }

    ///<summary>
    ///Update the configuration of an AI Search instance.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="id"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchNamespaceUpdateInstance
        (
            accountId: string,
            id: string,
            name: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchNamespaceUpdateInstancePayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceUpdateInstance.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceUpdateInstance.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceUpdateInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceUpdateInstance" (int status)
        }

    ///<summary>
    ///Performs a chat completion request against an AI Search instance, using indexed content as context for generating responses.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchNamespaceInstanceChatCompletion
        (
            id: string,
            accountId: string,
            name: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchNamespaceInstanceChatCompletionPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/chat/completions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceChatCompletion.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceChatCompletion.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceChatCompletion.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceChatCompletion" (int status)
        }

    ///<summary>
    ///Lists indexed items in an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="search"></param>
    ///<param name="sortBy">Sort order for items. "status" (default) sorts by status priority then last_seen_at. "modified_at" sorts by file modification time (most recent first), falling back to created_at.</param>
    ///<param name="status"></param>
    ///<param name="source">Filter items by source_id. Use "builtin" for uploaded files, or a source identifier like "web-crawler:https://example.com".</param>
    ///<param name="metadataFilter">JSON-encoded metadata filter using Vectorize filter syntax. Examples: {"folder":"reports/"}, {"timestamp":{"$gte":1700000000000}}, {"folder":{"$in":["docs/","reports/"]}}</param>
    ///<param name="itemId">Filter items by their unique ID. Returns at most one item.</param>
    ///<param name="key">Filter items by their exact key (object key / filename). Keys are unique per source, so combine with `source` to disambiguate across data sources.</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceInstanceListItems
        (
            id: string,
            accountId: string,
            name: string,
            ?page: int,
            ?perPage: int,
            ?search: string,
            ?sortBy: string,
            ?status: string,
            ?source: string,
            ?metadataFilter: string,
            ?itemId: string,
            ?key: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if sortBy.IsSome then
                      RequestPart.query ("sort_by", sortBy.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if source.IsSome then
                      RequestPart.query ("source", source.Value)
                  if metadataFilter.IsSome then
                      RequestPart.query ("metadata_filter", metadataFilter.Value)
                  if itemId.IsSome then
                      RequestPart.query ("item_id", itemId.Value)
                  if key.IsSome then
                      RequestPart.query ("key", key.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/items"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceListItems.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceListItems.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceListItems.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchNamespaceInstanceListItems.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceListItems" (int status)
        }

    ///<summary>
    ///Uploads a file to a managed AI Search instance via multipart/form-data.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="file">The file to upload. Filename must not exceed 128 characters.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="waitForCompletion">Wait for indexing to fully complete before responding. On RAGs with vector indexing enabled, this additionally waits for Vectorize ingestion confirmation (up to 40s) so the returned item reflects a queryable state. On timeout the item is returned in `running` state and the background alarm continues polling. Defaults to false.</param>
    ///<param name="metadata">JSON string of custom metadata key-value pairs.</param>
    member this.AiSearchNamespaceInstanceUploadItem
        (
            id: string,
            accountId: string,
            name: string,
            file: MultipartFile,
            ?cancellationToken: CancellationToken,
            ?waitForCompletion: bool,
            ?metadata: string
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartDeclaredFields [ "file"; "wait_for_completion"; "metadata" ]
                  RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  RequestPart.multipartBinary ("file", "application/octet-stream", file)
                  if waitForCompletion.IsSome then
                      RequestPart.multipartScalar ("wait_for_completion", "text/plain", waitForCompletion.Value)
                  if metadata.IsSome then
                      RequestPart.multipartScalar ("metadata", "text/plain", metadata.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/items"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceUploadItem.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceUploadItem.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceUploadItem.NotFound((Serializer.deserialize content))
            | 409 -> return AiSearchNamespaceInstanceUploadItem.Conflict((Serializer.deserialize content))
            | 413 -> return AiSearchNamespaceInstanceUploadItem.RequestEntityTooLarge((Serializer.deserialize content))
            | 429 -> return AiSearchNamespaceInstanceUploadItem.TooManyRequests((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceUploadItem" (int status)
        }

    ///<summary>
    ///Creates or updates an indexed item in an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchNamespaceInstanceCreateOrUpdateItem
        (
            id: string,
            accountId: string,
            name: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchNamespaceInstanceCreateOrUpdateItemPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/items"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceCreateOrUpdateItem.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceCreateOrUpdateItem.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceCreateOrUpdateItem.NotFound((Serializer.deserialize content))
            | 409 -> return AiSearchNamespaceInstanceCreateOrUpdateItem.Conflict((Serializer.deserialize content))
            | 503 ->
                return AiSearchNamespaceInstanceCreateOrUpdateItem.ServiceUnavailable((Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceCreateOrUpdateItem" (int status)
        }

    ///<summary>
    ///Deletes a file from a managed AI Search instance and triggers a reindex.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="itemId"></param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceInstanceDeleteItem
        (id: string, itemId: string, accountId: string, name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("item_id", itemId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/items/{item_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceDeleteItem.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceDeleteItem.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceDeleteItem.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceDeleteItem" (int status)
        }

    ///<summary>
    ///Retrieves a specific indexed item from an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="itemId"></param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceInstanceGetItem
        (id: string, itemId: string, accountId: string, name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("item_id", itemId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/items/{item_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceGetItem.OK((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceGetItem.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchNamespaceInstanceGetItem.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceGetItem" (int status)
        }

    ///<summary>
    ///Syncs an item to an AI Search instance index.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="itemId"></param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchNamespaceInstanceSyncItem
        (
            id: string,
            itemId: string,
            accountId: string,
            name: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchNamespaceInstanceSyncItemPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("item_id", itemId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/items/{item_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceSyncItem.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceSyncItem.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceSyncItem.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchNamespaceInstanceSyncItem.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceSyncItem" (int status)
        }

    ///<summary>
    ///Lists chunks for a specific item in an AI Search instance, including their text content.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="itemId"></param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="limit"></param>
    ///<param name="offset"></param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceInstanceListItemChunks
        (
            id: string,
            itemId: string,
            accountId: string,
            name: string,
            ?limit: int,
            ?offset: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("item_id", itemId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/items/{item_id}/chunks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceListItemChunks.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceListItemChunks.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceListItemChunks.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchNamespaceInstanceListItemChunks.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceListItemChunks" (int status)
        }

    ///<summary>
    ///Downloads the raw file content for a specific item from the managed AI Search instance storage.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="itemId"></param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceInstanceGetItemContent
        (id: string, itemId: string, accountId: string, name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("item_id", itemId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name) ]

            let! (status, _, contentBinary) =
                OpenApiHttp.getBinaryAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/items/{item_id}/download"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceGetItemContent.OK(contentBinary)
            | 400 ->
                let content = Encoding.UTF8.GetString contentBinary
                return AiSearchNamespaceInstanceGetItemContent.BadRequest((Serializer.deserialize content))
            | 403 ->
                let content = Encoding.UTF8.GetString contentBinary
                return AiSearchNamespaceInstanceGetItemContent.Forbidden((Serializer.deserialize content))
            | 404 ->
                let content = Encoding.UTF8.GetString contentBinary
                return AiSearchNamespaceInstanceGetItemContent.NotFound((Serializer.deserialize content))
            | 503 ->
                let content = Encoding.UTF8.GetString contentBinary
                return AiSearchNamespaceInstanceGetItemContent.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceGetItemContent" (int status)
        }

    ///<summary>
    ///Lists processing logs for a specific item in an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="itemId"></param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="limit"></param>
    ///<param name="cursor"></param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceInstanceLogsItem
        (
            id: string,
            itemId: string,
            accountId: string,
            name: string,
            ?limit: int,
            ?cursor: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("item_id", itemId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/items/{item_id}/logs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceLogsItem.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceLogsItem.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceLogsItem.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchNamespaceInstanceLogsItem.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceLogsItem" (int status)
        }

    ///<summary>
    ///Lists indexing jobs for an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceInstanceListJobs
        (id: string, accountId: string, name: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/jobs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceListJobs.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceListJobs.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceListJobs.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchNamespaceInstanceListJobs.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceListJobs" (int status)
        }

    ///<summary>
    ///Creates a new indexing job for an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchNamespaceInstanceCreateJob
        (
            id: string,
            accountId: string,
            name: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchNamespaceInstanceCreateJobPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/jobs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceCreateJob.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceCreateJob.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceCreateJob.NotFound((Serializer.deserialize content))
            | 429 -> return AiSearchNamespaceInstanceCreateJob.TooManyRequests((Serializer.deserialize content))
            | 503 -> return AiSearchNamespaceInstanceCreateJob.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceCreateJob" (int status)
        }

    ///<summary>
    ///Retrieves details for a specific AI Search indexing job.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="jobId"></param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceInstanceGetJob
        (id: string, jobId: string, accountId: string, name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("job_id", jobId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/jobs/{job_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceGetJob.OK((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceGetJob.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchNamespaceInstanceGetJob.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceGetJob" (int status)
        }

    ///<summary>
    ///Cancel an in-progress indexing job for an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="jobId"></param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchNamespaceInstanceChangeJobStatus
        (
            id: string,
            jobId: string,
            accountId: string,
            name: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchNamespaceInstanceChangeJobStatusPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("job_id", jobId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/jobs/{job_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceChangeJobStatus.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceChangeJobStatus.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceChangeJobStatus.NotFound((Serializer.deserialize content))
            | 503 ->
                return AiSearchNamespaceInstanceChangeJobStatus.ServiceUnavailable((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceChangeJobStatus" (int status)
        }

    ///<summary>
    ///Lists log entries for an AI Search indexing job.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="jobId"></param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceInstanceListJobLogs
        (
            id: string,
            jobId: string,
            accountId: string,
            name: string,
            ?page: int,
            ?perPage: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("job_id", jobId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/jobs/{job_id}/logs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceListJobLogs.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceListJobLogs.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceListJobLogs.NotFound((Serializer.deserialize content))
            | 503 -> return AiSearchNamespaceInstanceListJobLogs.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceListJobLogs" (int status)
        }

    ///<summary>
    ///Purges all cached search results for an AI Search instance. A new internal cache key is generated, immediately orphaning all prior cached entries.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespacePurgeInstanceCache
        (accountId: string, id: string, name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/purge_cache"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespacePurgeInstanceCache.OK((Serializer.deserialize content))
            | 404 -> return AiSearchNamespacePurgeInstanceCache.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespacePurgeInstanceCache" (int status)
        }

    ///<summary>
    ///Executes a semantic search query against an AI Search instance to find relevant indexed content.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchNamespaceInstanceSearch
        (
            id: string,
            accountId: string,
            name: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchNamespaceInstanceSearchPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/search"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceInstanceSearch.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceInstanceSearch.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceInstanceSearch.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceInstanceSearch" (int status)
        }

    ///<summary>
    ///Retrieve usage and indexing statistics for an AI Search instance.
    ///</summary>
    ///<param name="id">AI Search instance ID. Lowercase alphanumeric, hyphens, and underscores.</param>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchNamespaceStats
        (id: string, accountId: string, name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/instances/{id}/stats"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceStats.OK((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceStats.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceStats" (int status)
        }

    ///<summary>
    ///Performs a semantic search query against multiple AI Search instances in parallel, merging the retrieved results into a single ranked response.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="name">Namespace name</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AiSearchNamespaceMultiInstanceSearch
        (
            accountId: string,
            name: string,
            ?cancellationToken: CancellationToken,
            ?body: AiSearchNamespaceMultiInstanceSearchPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("name", name)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/namespaces/{name}/search"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchNamespaceMultiInstanceSearch.OK((Serializer.deserialize content))
            | 400 -> return AiSearchNamespaceMultiInstanceSearch.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchNamespaceMultiInstanceSearch.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchNamespaceMultiInstanceSearch" (int status)
        }

    ///<summary>
    ///List stored AI Search credentials in the account without exposing their secrets.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="page">Page number (1-indexed).</param>
    ///<param name="perPage">Number of results per page.</param>
    ///<param name="search">Filter tokens whose name contains this string (case-insensitive).</param>
    ///<param name="cancellationToken"></param>
    member this.AiSearchListTokens
        (accountId: string, ?page: int, ?perPage: int, ?search: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/ai-search/tokens" requestParts cancellationToken

            match (int status) with
            | 200 -> return AiSearchListTokens.OK((Serializer.deserialize content))
            | 400 -> return AiSearchListTokens.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchListTokens" (int status)
        }

    ///<summary>
    ///Create a stored Cloudflare credential for an AI Search instance to access its data source.
    ///</summary>
    member this.AiSearchCreateTokens
        (accountId: string, ?cancellationToken: CancellationToken, ?body: AiSearchCreateTokensPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/tokens"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return AiSearchCreateTokens.Created((Serializer.deserialize content))
            | 400 -> return AiSearchCreateTokens.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchCreateTokens" (int status)
        }

    ///<summary>
    ///Permanently delete a stored AI Search credential. Credentials in use by an instance cannot be deleted.
    ///</summary>
    member this.AiSearchDeleteTokens(accountId: string, id: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/tokens/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchDeleteTokens.OK((Serializer.deserialize content))
            | 400 -> return AiSearchDeleteTokens.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchDeleteTokens.NotFound((Serializer.deserialize content))
            | 409 -> return AiSearchDeleteTokens.Conflict((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchDeleteTokens" (int status)
        }

    ///<summary>
    ///Retrieve a stored AI Search credential without exposing its secret.
    ///</summary>
    member this.AiSearchFetchTokens(accountId: string, id: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/tokens/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchFetchTokens.OK((Serializer.deserialize content))
            | 400 -> return AiSearchFetchTokens.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchFetchTokens.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchFetchTokens" (int status)
        }

    ///<summary>
    ///Replace a stored AI Search credential and invalidate cached credentials for instances that use it.
    ///</summary>
    member this.AiSearchUpdateTokens
        (accountId: string, id: System.Guid, ?cancellationToken: CancellationToken, ?body: AiSearchUpdateTokensPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/ai-search/tokens/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AiSearchUpdateTokens.OK((Serializer.deserialize content))
            | 400 -> return AiSearchUpdateTokens.BadRequest((Serializer.deserialize content))
            | 404 -> return AiSearchUpdateTokens.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiSearchUpdateTokens" (int status)
        }

    ///<summary>
    ///Searches Workers AI models by author or organization name.
    ///</summary>
    member this.WorkersAiSearchAuthor(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai/authors/search"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersAiSearchAuthor.OK((Serializer.deserialize content))
            | 400 -> return WorkersAiSearchAuthor.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiSearchAuthor" (int status)
        }

    ///<summary>
    ///Lists all fine-tuning jobs created by the account, including status and metrics.
    ///</summary>
    member this.WorkersAiListFinetunes(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/ai/finetunes" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkersAiListFinetunes.OK((Serializer.deserialize content))
            | 400 -> return WorkersAiListFinetunes.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiListFinetunes" (int status)
        }

    ///<summary>
    ///Creates a new fine-tuning job for a Workers AI model using custom training data.
    ///</summary>
    member this.WorkersAiCreateFinetune
        (accountId: string, ?cancellationToken: CancellationToken, ?body: WorkersAiCreateFinetunePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/ai/finetunes" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkersAiCreateFinetune.OK((Serializer.deserialize content))
            | 400 -> return WorkersAiCreateFinetune.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiCreateFinetune" (int status)
        }

    ///<summary>
    ///Lists publicly available fine-tuned models that can be used with Workers AI.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="limit">Pagination Limit.</param>
    ///<param name="offset">Pagination Offset.</param>
    ///<param name="orderBy">Order By Column Name.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkersAiListPublicFinetunes
        (accountId: string, ?limit: float, ?offset: float, ?orderBy: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("orderBy", orderBy.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai/finetunes/public"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersAiListPublicFinetunes.OK((Serializer.deserialize content))
            | 400 -> return WorkersAiListPublicFinetunes.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiListPublicFinetunes" (int status)
        }

    ///<summary>
    ///Delete a finetune. Any in-flight requests referencing the lora will fail after the files are deleted.
    ///</summary>
    member this.WorkersAiDeleteFinetune(accountId: string, finetuneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("finetune_id", finetuneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/ai/finetunes/{finetune_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersAiDeleteFinetune.OK((Serializer.deserialize content))
            | 404 -> return WorkersAiDeleteFinetune.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiDeleteFinetune" (int status)
        }

    ///<summary>
    ///Uploads training data assets for a Workers AI fine-tuning job.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="finetuneId"></param>
    ///<param name="fileName">Name of the file (adapter_config.json or adapter_model.safetensors).</param>
    ///<param name="file">File to upload.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkersAiUploadFinetuneAsset
        (
            accountId: string,
            finetuneId: string,
            fileName: string,
            file: MultipartFile,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartDeclaredFields [ "file_name"; "file" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("finetune_id", finetuneId)
                  RequestPart.multipartScalar ("file_name", "text/plain", fileName)
                  RequestPart.multipartBinary ("file", "application/octet-stream", file) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai/finetunes/{finetune_id}/finetune-assets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersAiUploadFinetuneAsset.OK((Serializer.deserialize content))
            | 400 -> return WorkersAiUploadFinetuneAsset.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiUploadFinetuneAsset" (int status)
        }

    ///<summary>
    ///Returns a pre-signed R2 URL for downloading a finetune asset file.
    ///</summary>
    member this.WorkersAiDownloadFinetuneAsset
        (accountId: string, finetuneId: string, fileName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("finetune_id", finetuneId)
                  RequestPart.path ("file_name", fileName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai/finetunes/{finetune_id}/finetune-assets/{file_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersAiDownloadFinetuneAsset.OK((Serializer.deserialize content))
            | 404 -> return WorkersAiDownloadFinetuneAsset.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiDownloadFinetuneAsset" (int status)
        }

    ///<summary>
    ///Retrieves the input and output JSON schema definition for a Workers AI model.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="model">Model Name</param>
    ///<param name="cancellationToken"></param>
    member this.WorkersAiGetModelSchema(accountId: string, model: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.query ("model", model) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/ai/models/schema" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkersAiGetModelSchema.OK((Serializer.deserialize content))
            | 400 -> return WorkersAiGetModelSchema.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiGetModelSchema" (int status)
        }

    ///<summary>
    ///Searches Workers AI models by name or description.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="perPage"></param>
    ///<param name="page"></param>
    ///<param name="task">Filter by Task Name.</param>
    ///<param name="author">Filter by Author.</param>
    ///<param name="source">Filter by Source Id.</param>
    ///<param name="hideExperimental">Filter to hide experimental models.</param>
    ///<param name="search">Search.</param>
    ///<param name="includeDeprecated">If true, include models for up to three months after their deprecation date. Defaults to false.</param>
    ///<param name="format">If set, return models in the requested marketplace format instead of the default response.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkersAiSearchModel
        (
            accountId: string,
            ?perPage: int,
            ?page: int,
            ?task: string,
            ?author: string,
            ?source: float,
            ?hideExperimental: bool,
            ?search: string,
            ?includeDeprecated: bool,
            ?format: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if task.IsSome then
                      RequestPart.query ("task", task.Value)
                  if author.IsSome then
                      RequestPart.query ("author", author.Value)
                  if source.IsSome then
                      RequestPart.query ("source", source.Value)
                  if hideExperimental.IsSome then
                      RequestPart.query ("hide_experimental", hideExperimental.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if includeDeprecated.IsSome then
                      RequestPart.query ("include_deprecated", includeDeprecated.Value)
                  if format.IsSome then
                      RequestPart.query ("format", format.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/ai/models/search" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkersAiSearchModel.OK((Serializer.deserialize content))
            | 404 -> return WorkersAiSearchModel.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiSearchModel" (int status)
        }

    ///<summary>
    ///Execute an AI model by specifying the model name in the request body.
    ///This endpoint provides a generic interface for running AI models where the model name is part of the request payload rather than the URL path. It supports all AI Gateway features including caching, custom headers, and request options.
    ///Model-specific inputs available in [Cloudflare Docs](https://developers.cloudflare.com/workers-ai/models/).
    ///</summary>
    member this.WorkersAiPostRunGeneric
        (accountId: string, ?cancellationToken: CancellationToken, ?body: WorkersAiPostRunGenericPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/ai/run" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkersAiPostRunGeneric.OK((Serializer.deserialize content))
            | 400 -> return WorkersAiPostRunGeneric.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiPostRunGeneric" (int status)
        }

    ///<summary>
    ///This endpoint provides users with the capability to run specific AI models on-demand.
    ///By submitting the required input data, users can receive real-time predictions or results generated by the chosen AI
    ///model. The endpoint supports various AI model types, ensuring flexibility and adaptability for diverse use cases.
    ///Model specific inputs available in [Cloudflare Docs](https://developers.cloudflare.com/workers-ai/models/).
    ///</summary>
    member this.WorkersAiPostRunModel
        (
            accountId: string,
            modelName: string,
            ?cancellationToken: CancellationToken,
            ?body: InlineUnion_bf9bd547d728bd702d6446e0
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("model_name", modelName)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/ai/run/{model_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersAiPostRunModel.OK((Serializer.deserialize content))
            | 400 -> return WorkersAiPostRunModel.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiPostRunModel" (int status)
        }

    ///<summary>
    ///Searches Workers AI models by task type (e.g., text-generation, embeddings).
    ///</summary>
    member this.WorkersAiSearchTask(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/ai/tasks/search" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkersAiSearchTask.OK((Serializer.deserialize content))
            | 404 -> return WorkersAiSearchTask.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiSearchTask" (int status)
        }

    ///<summary>
    ///Converts uploaded files into Markdown format using Workers AI.
    ///</summary>
    member this.WorkersAiPostToMarkdown
        (accountId: string, files: list<MultipartFile>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartDeclaredFields [ "files" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.multipartBinary ("files", "application/octet-stream", files) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/ai/tomarkdown" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkersAiPostToMarkdown.OK((Serializer.deserialize content))
            | 400 -> return WorkersAiPostToMarkdown.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiPostToMarkdown" (int status)
        }

    ///<summary>
    ///Lists all file formats supported for conversion to Markdown.
    ///</summary>
    member this.WorkersAiGetToMarkdownSupported(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/ai/tomarkdown/supported"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersAiGetToMarkdownSupported.OK((Serializer.deserialize content))
            | 400 -> return WorkersAiGetToMarkdownSupported.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersAiGetToMarkdownSupported" (int status)
        }

    ///<summary>
    ///Runs an AI Search query against an AutoRAG.
    ///</summary>
    ///<param name="id">rag id</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AutoragConfigAiSearch
        (id: string, accountId: string, ?cancellationToken: CancellationToken, ?body: AutoragConfigAiSearchPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/autorag/rags/{id}/ai-search"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AutoragConfigAiSearch.OK((Serializer.deserialize content))
            | 404 -> return AutoragConfigAiSearch.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AutoragConfigAiSearch" (int status)
        }

    ///<summary>
    ///Lists files indexed by an AutoRAG.
    ///</summary>
    ///<param name="id">rag id</param>
    ///<param name="accountId"></param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="search"></param>
    ///<param name="status"></param>
    ///<param name="cancellationToken"></param>
    member this.AutoragConfigFiles
        (
            id: string,
            accountId: string,
            ?page: int,
            ?perPage: int,
            ?search: string,
            ?status: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/autorag/rags/{id}/files"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AutoragConfigFiles.OK((Serializer.deserialize content))
            | 404 -> return AutoragConfigFiles.NotFound((Serializer.deserialize content))
            | 503 -> return AutoragConfigFiles.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AutoragConfigFiles" (int status)
        }

    ///<summary>
    ///Lists jobs for an AutoRAG.
    ///</summary>
    ///<param name="id">rag id</param>
    ///<param name="accountId"></param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="cancellationToken"></param>
    member this.AutoragConfigListJobs
        (id: string, accountId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/autorag/rags/{id}/jobs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AutoragConfigListJobs.OK((Serializer.deserialize content))
            | 404 -> return AutoragConfigListJobs.NotFound((Serializer.deserialize content))
            | 503 -> return AutoragConfigListJobs.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AutoragConfigListJobs" (int status)
        }

    ///<summary>
    ///Returns details for an AutoRAG job.
    ///</summary>
    ///<param name="id">rag id</param>
    ///<param name="jobId"></param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.AutoragConfigGetJob
        (id: string, jobId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("job_id", jobId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/autorag/rags/{id}/jobs/{job_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AutoragConfigGetJob.OK((Serializer.deserialize content))
            | 404 -> return AutoragConfigGetJob.NotFound((Serializer.deserialize content))
            | 503 -> return AutoragConfigGetJob.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AutoragConfigGetJob" (int status)
        }

    ///<summary>
    ///Lists logs for an AutoRAG job.
    ///</summary>
    ///<param name="id">rag id</param>
    ///<param name="jobId"></param>
    ///<param name="accountId"></param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="cancellationToken"></param>
    member this.AutoragConfigListJobLogs
        (id: string, jobId: string, accountId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("job_id", jobId)
                  RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/autorag/rags/{id}/jobs/{job_id}/logs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AutoragConfigListJobLogs.OK((Serializer.deserialize content))
            | 404 -> return AutoragConfigListJobLogs.NotFound((Serializer.deserialize content))
            | 503 -> return AutoragConfigListJobLogs.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AutoragConfigListJobLogs" (int status)
        }

    ///<summary>
    ///Searches an AutoRAG.
    ///</summary>
    ///<param name="id">rag id</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.AutoragConfigSearch
        (id: string, accountId: string, ?cancellationToken: CancellationToken, ?body: AutoragConfigSearchPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/autorag/rags/{id}/search"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AutoragConfigSearch.OK((Serializer.deserialize content))
            | 404 -> return AutoragConfigSearch.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AutoragConfigSearch" (int status)
        }

    ///<summary>
    ///Starts synchronization for an AutoRAG.
    ///</summary>
    ///<param name="id">rag id</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.AutoragConfigSync(id: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("id", id); RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/autorag/rags/{id}/sync"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AutoragConfigSync.OK((Serializer.deserialize content))
            | 400 -> return AutoragConfigSync.BadRequest((Serializer.deserialize content))
            | 404 -> return AutoragConfigSync.NotFound((Serializer.deserialize content))
            | 429 -> return AutoragConfigSync.TooManyRequests((Serializer.deserialize content))
            | 503 -> return AutoragConfigSync.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AutoragConfigSync" (int status)
        }

    ///<summary>
    ///Fetches and parses the robots.txt file for a zone or a specific subdomain within the zone. Returns parsed user-agent rules, content signals, and sitemaps.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="subdomain">Optional subdomain to fetch robots.txt for. If omitted, fetches robots.txt for the zone apex domain.</param>
    ///<param name="cancellationToken"></param>
    member this.AiAuditGetRobots(zoneId: string, ?subdomain: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if subdomain.IsSome then
                      RequestPart.query ("subdomain", subdomain.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/ai-audit/robots" requestParts cancellationToken

            match (int status) with
            | 200 -> return AiAuditGetRobots.OK((Serializer.deserialize content))
            | 400 -> return AiAuditGetRobots.BadRequest((Serializer.deserialize content))
            | 401 -> return AiAuditGetRobots.Unauthorized((Serializer.deserialize content))
            | 403 -> return AiAuditGetRobots.Forbidden((Serializer.deserialize content))
            | 404 -> return AiAuditGetRobots.NotFound((Serializer.deserialize content))
            | 500 -> return AiAuditGetRobots.InternalServerError((Serializer.deserialize content))
            | 503 -> return AiAuditGetRobots.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiAuditGetRobots" (int status)
        }

    ///<summary>
    ///Fetches and parses robots.txt files for multiple domains within a zone in a single request. Each domain must belong to the specified zone. Results are keyed by hostname.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="body">Array of domain hostnames to fetch robots.txt for. Each domain must end with the zone name. Maximum 25 domains per request.</param>
    ///<param name="cancellationToken"></param>
    member this.AiAuditBulkGetRobots(zoneId: string, body: list<string>, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/ai-audit/robots/bulk" requestParts cancellationToken

            match (int status) with
            | 200 -> return AiAuditBulkGetRobots.OK((Serializer.deserialize content))
            | 400 -> return AiAuditBulkGetRobots.BadRequest((Serializer.deserialize content))
            | 401 -> return AiAuditBulkGetRobots.Unauthorized((Serializer.deserialize content))
            | 403 -> return AiAuditBulkGetRobots.Forbidden((Serializer.deserialize content))
            | 404 -> return AiAuditBulkGetRobots.NotFound((Serializer.deserialize content))
            | 408 -> return AiAuditBulkGetRobots.RequestTimeout((Serializer.deserialize content))
            | 500 -> return AiAuditBulkGetRobots.InternalServerError((Serializer.deserialize content))
            | 503 -> return AiAuditBulkGetRobots.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AiAuditBulkGetRobots" (int status)
        }
