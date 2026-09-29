namespace rec FSharp.CloudEdge.Management.Compute

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
type ComputeClient(httpClient: HttpClient) =
    ///<summary>
    ///Returns the page's accessibility tree. Use `interestingOnly` to only return semantically meaningful nodes; use `root` to scope the tree to a CSS-selector-anchored subtree. Control page loading with `gotoOptions` and `waitFor*` options.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPostAccessibilityTree
        (
            accountId: string,
            body: InlineUnion_9a191a90e352f560123ce344,
            ?cacheTTL: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/accessibilityTree"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostAccessibilityTree.OK((Serializer.deserialize content))
            | 400 -> return BrapiPostAccessibilityTree.BadRequest((Serializer.deserialize content))
            | 422 -> return BrapiPostAccessibilityTree.UnprocessableEntity((Serializer.deserialize content))
            | 429 -> return BrapiPostAccessibilityTree.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiPostAccessibilityTree.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostAccessibilityTree" (int status)
        }

    ///<summary>
    ///Fetches rendered HTML content from provided URL or HTML. Check available options like `gotoOptions` and `waitFor*` to control page load behaviour.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPostContent
        (
            accountId: string,
            body: InlineUnion_78a8fd6c85fdcce464712cb3,
            ?cacheTTL: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/content"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostContent.OK((Serializer.deserialize content))
            | 400 -> return BrapiPostContent.BadRequest((Serializer.deserialize content))
            | 422 -> return BrapiPostContent.UnprocessableEntity((Serializer.deserialize content))
            | 429 -> return BrapiPostContent.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiPostContent.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostContent" (int status)
        }

    ///<summary>
    ///Starts a crawl job for the provided URL and its children. Check available options like `gotoOptions` and `waitFor*` to control page load behaviour.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPostCrawl
        (
            accountId: string,
            body: InlineUnion_f5dd374bc2be6d2ff06e2ac5,
            ?cacheTTL: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/crawl"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostCrawl.OK((Serializer.deserialize content))
            | 400 -> return BrapiPostCrawl.BadRequest((Serializer.deserialize content))
            | 429 -> return BrapiPostCrawl.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiPostCrawl.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostCrawl" (int status)
        }

    ///<summary>
    ///Cancels an ongoing crawl job by setting its status to cancelled and stopping all queued URLs.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="jobId">The ID of the crawl job to cancel.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiDeleteCancelCrawl(accountId: string, jobId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("job_id", jobId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/crawl/{job_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiDeleteCancelCrawl.OK((Serializer.deserialize content))
            | 400 -> return BrapiDeleteCancelCrawl.BadRequest((Serializer.deserialize content))
            | 404 -> return BrapiDeleteCancelCrawl.NotFound((Serializer.deserialize content))
            | 500 -> return BrapiDeleteCancelCrawl.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiDeleteCancelCrawl" (int status)
        }

    ///<summary>
    ///Returns the result of a crawl job.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="jobId">Crawl job ID.</param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="status">Filter by URL status.</param>
    ///<param name="cursor">Cursor for pagination.</param>
    ///<param name="limit">Limit for pagination.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetCrawlResult
        (
            accountId: string,
            jobId: string,
            ?cacheTTL: float,
            ?status: string,
            ?cursor: float,
            ?limit: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("job_id", jobId)
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/crawl/{job_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiGetCrawlResult.OK((Serializer.deserialize content))
            | 400 -> return BrapiGetCrawlResult.BadRequest((Serializer.deserialize content))
            | 500 -> return BrapiGetCrawlResult.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetCrawlResult" (int status)
        }

    ///<summary>
    ///Acquires and establishes a WebSocket connection to a browser session. Session guardrails may be supplied in the `cf-brapi-guardrails` header as base64url-encoded JSON of the same `guardrails` object the POST body accepts (for example `{"allowedDomains":["*.example.com"]}`).
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="keepAlive">Keep-alive time in ms (only valid when acquiring new session).</param>
    ///<param name="lab">Use experimental browser.</param>
    ///<param name="recording"></param>
    ///<param name="cfBrapiGuardrails">Optional base64url-encoded JSON session guardrails (allowedDomains and allowedDomainSets)</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsBrowserAcquire
        (
            accountId: string,
            ?keepAlive: float,
            ?lab: bool,
            ?recording: bool,
            ?cfBrapiGuardrails: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if keepAlive.IsSome then
                      RequestPart.query ("keep_alive", keepAlive.Value)
                  if lab.IsSome then
                      RequestPart.query ("lab", lab.Value)
                  if recording.IsSome then
                      RequestPart.query ("recording", recording.Value)
                  if cfBrapiGuardrails.IsSome then
                      RequestPart.header ("cf-brapi-guardrails", cfBrapiGuardrails.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser"
                    requestParts
                    cancellationToken

            match (int status) with
            | 101 -> return BrapiGetDevtoolsBrowserAcquire.SwitchingProtocols
            | 400 -> return BrapiGetDevtoolsBrowserAcquire.BadRequest((Serializer.deserialize content))
            | 429 -> return BrapiGetDevtoolsBrowserAcquire.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsBrowserAcquire.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsBrowserAcquire" (int status)
        }

    ///<summary>
    ///Acquires a browser and returns its session ID and websocket URL. Optionally accepts a JSON body with session guardrails to restrict outbound HTTP/S traffic.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="keepAlive">Keep-alive time in milliseconds.</param>
    ///<param name="lab">Use experimental browser.</param>
    ///<param name="targets">Include browser targets in response.</param>
    ///<param name="liveViewUrlExpiresInMs">How long the live view URL remains valid, in milliseconds (max 60 minutes). Only used when targets is true.</param>
    ///<param name="recording"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.BrapiPostDevtoolsAcquire
        (
            accountId: string,
            ?keepAlive: float,
            ?lab: bool,
            ?targets: bool,
            ?liveViewUrlExpiresInMs: float,
            ?recording: bool,
            ?cancellationToken: CancellationToken,
            ?body: BrapiPostDevtoolsAcquirePayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if keepAlive.IsSome then
                      RequestPart.query ("keep_alive", keepAlive.Value)
                  if lab.IsSome then
                      RequestPart.query ("lab", lab.Value)
                  if targets.IsSome then
                      RequestPart.query ("targets", targets.Value)
                  if liveViewUrlExpiresInMs.IsSome then
                      RequestPart.query ("liveViewUrlExpiresInMs", liveViewUrlExpiresInMs.Value)
                  if recording.IsSome then
                      RequestPart.query ("recording", recording.Value)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostDevtoolsAcquire.OK((Serializer.deserialize content))
            | 429 -> return BrapiPostDevtoolsAcquire.TooManyRequests((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostDevtoolsAcquire" (int status)
        }

    ///<summary>
    ///Closes an existing browser session.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Browser session ID to close.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiDeleteDevtoolsBrowserDelete
        (accountId: string, sessionId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiDeleteDevtoolsBrowserDelete.OK((Serializer.deserialize content))
            | 404 -> return BrapiDeleteDevtoolsBrowserDelete.NotFound
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiDeleteDevtoolsBrowserDelete" (int status)
        }

    ///<summary>
    ///Establishes a WebSocket connection to an existing browser session.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Browser session ID to connect to.</param>
    ///<param name="keepAlive">Keep-alive time in ms (only valid when acquiring new session).</param>
    ///<param name="lab">Use experimental browser.</param>
    ///<param name="recording"></param>
    ///<param name="cfBrapiGuardrails">Optional base64url-encoded JSON connection guardrails (mode)</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsBrowser
        (
            accountId: string,
            sessionId: System.Guid,
            ?keepAlive: float,
            ?lab: bool,
            ?recording: bool,
            ?cfBrapiGuardrails: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId)
                  if keepAlive.IsSome then
                      RequestPart.query ("keep_alive", keepAlive.Value)
                  if lab.IsSome then
                      RequestPart.query ("lab", lab.Value)
                  if recording.IsSome then
                      RequestPart.query ("recording", recording.Value)
                  if cfBrapiGuardrails.IsSome then
                      RequestPart.header ("cf-brapi-guardrails", cfBrapiGuardrails.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 101 -> return BrapiGetDevtoolsBrowser.SwitchingProtocols
            | 400 -> return BrapiGetDevtoolsBrowser.BadRequest((Serializer.deserialize content))
            | 429 -> return BrapiGetDevtoolsBrowser.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsBrowser.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsBrowser" (int status)
        }

    ///<summary>
    ///Returns a list of all debuggable targets including tabs, pages, service workers, and other browser contexts.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Browser session ID.</param>
    ///<param name="liveViewUrlExpiresInMs">How long the live view URLs remain valid, in milliseconds (max 60 minutes)</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsJson
        (
            accountId: string,
            sessionId: System.Guid,
            ?liveViewUrlExpiresInMs: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId)
                  if liveViewUrlExpiresInMs.IsSome then
                      RequestPart.query ("liveViewUrlExpiresInMs", liveViewUrlExpiresInMs.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}/json"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiGetDevtoolsJson.OK((Serializer.deserialize content))
            | 400 -> return BrapiGetDevtoolsJson.BadRequest((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsJson.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsJson" (int status)
        }

    ///<summary>
    ///Activates (brings to front) a specific browser target by its ID.
    ///</summary>
    ///<param name="sessionId">Browser session ID.</param>
    ///<param name="accountId">Account ID.</param>
    ///<param name="targetId">Target ID to activate.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsJsonActivate
        (sessionId: System.Guid, accountId: string, targetId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("session_id", sessionId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("target_id", targetId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}/json/activate/{target_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiGetDevtoolsJsonActivate.OK((Serializer.deserialize content))
            | 400 -> return BrapiGetDevtoolsJsonActivate.BadRequest((Serializer.deserialize content))
            | 404 -> return BrapiGetDevtoolsJsonActivate.NotFound((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsJsonActivate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsJsonActivate" (int status)
        }

    ///<summary>
    ///Closes a specific browser target (tab, page, etc.) by its ID. Returns 'Target is closing' on success or an error if the target is not found.
    ///</summary>
    ///<param name="sessionId">Browser session ID.</param>
    ///<param name="accountId">Account ID.</param>
    ///<param name="targetId">Target ID to close.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsJsonClose
        (sessionId: System.Guid, accountId: string, targetId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("session_id", sessionId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("target_id", targetId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}/json/close/{target_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiGetDevtoolsJsonClose.OK((Serializer.deserialize content))
            | 400 -> return BrapiGetDevtoolsJsonClose.BadRequest((Serializer.deserialize content))
            | 404 -> return BrapiGetDevtoolsJsonClose.NotFound((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsJsonClose.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsJsonClose" (int status)
        }

    ///<summary>
    ///Returns a list of all debuggable targets including tabs, pages, service workers, and other browser contexts.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Browser session ID.</param>
    ///<param name="liveViewUrlExpiresInMs">How long the live view URLs remain valid, in milliseconds (max 60 minutes)</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsJsonList
        (
            accountId: string,
            sessionId: System.Guid,
            ?liveViewUrlExpiresInMs: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId)
                  if liveViewUrlExpiresInMs.IsSome then
                      RequestPart.query ("liveViewUrlExpiresInMs", liveViewUrlExpiresInMs.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}/json/list"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiGetDevtoolsJsonList.OK((Serializer.deserialize content))
            | 400 -> return BrapiGetDevtoolsJsonList.BadRequest((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsJsonList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsJsonList" (int status)
        }

    ///<summary>
    ///Returns the debuggable target with the given ID.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Browser session ID.</param>
    ///<param name="targetId">Target ID.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsJsonTarget
        (accountId: string, sessionId: System.Guid, targetId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId)
                  RequestPart.path ("target_id", targetId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}/json/list/{target_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiGetDevtoolsJsonTarget.OK((Serializer.deserialize content))
            | 400 -> return BrapiGetDevtoolsJsonTarget.BadRequest((Serializer.deserialize content))
            | 404 -> return BrapiGetDevtoolsJsonTarget.NotFound((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsJsonTarget.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsJsonTarget" (int status)
        }

    ///<summary>
    ///Opens a new tab in the browser. Optionally specify a URL to navigate to.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Browser session ID.</param>
    ///<param name="url"></param>
    ///<param name="liveViewUrlExpiresInMs">How long the live view URL remains valid, in milliseconds (max 60 minutes)</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPutDevtoolsJsonNew
        (
            accountId: string,
            sessionId: System.Guid,
            ?url: string,
            ?liveViewUrlExpiresInMs: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId)
                  if url.IsSome then
                      RequestPart.query ("url", url.Value)
                  if liveViewUrlExpiresInMs.IsSome then
                      RequestPart.query ("liveViewUrlExpiresInMs", liveViewUrlExpiresInMs.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}/json/new"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPutDevtoolsJsonNew.OK((Serializer.deserialize content))
            | 400 -> return BrapiPutDevtoolsJsonNew.BadRequest((Serializer.deserialize content))
            | 500 -> return BrapiPutDevtoolsJsonNew.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPutDevtoolsJsonNew" (int status)
        }

    ///<summary>
    ///Returns the complete Chrome DevTools Protocol schema including all domains, commands, events, and types. This schema describes the entire CDP API surface.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Browser session ID.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsJsonProtocol
        (accountId: string, sessionId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}/json/protocol"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiGetDevtoolsJsonProtocol.OK((Serializer.deserialize content))
            | 400 -> return BrapiGetDevtoolsJsonProtocol.BadRequest((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsJsonProtocol.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsJsonProtocol" (int status)
        }

    ///<summary>
    ///Get browser version metadata.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Browser session ID.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsJsonVersion
        (accountId: string, sessionId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}/json/version"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiGetDevtoolsJsonVersion.OK((Serializer.deserialize content))
            | 400 -> return BrapiGetDevtoolsJsonVersion.BadRequest((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsJsonVersion.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsJsonVersion" (int status)
        }

    ///<summary>
    ///Generates time-limited URLs to view a remote browser session. Set `guardrails: { mode: 'readonly' }` to create a view-only link.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Browser session ID</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.BrapiPostDevtoolsLiveView
        (
            accountId: string,
            sessionId: System.Guid,
            ?cancellationToken: CancellationToken,
            ?body: BrapiPostDevtoolsLiveViewPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}/live_view"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostDevtoolsLiveView.OK((Serializer.deserialize content))
            | 400 -> return BrapiPostDevtoolsLiveView.BadRequest((Serializer.deserialize content))
            | 404 -> return BrapiPostDevtoolsLiveView.NotFound((Serializer.deserialize content))
            | 500 -> return BrapiPostDevtoolsLiveView.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostDevtoolsLiveView" (int status)
        }

    ///<summary>
    ///Establishes a WebSocket connection to a specific Chrome DevTools target or page.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Browser session ID.</param>
    ///<param name="targetId">Target ID, e.g. page ID.</param>
    ///<param name="cfBrapiGuardrails">Optional base64url-encoded JSON connection guardrails (mode)</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsPage
        (
            accountId: string,
            sessionId: System.Guid,
            targetId: string,
            ?cfBrapiGuardrails: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId)
                  RequestPart.path ("target_id", targetId)
                  if cfBrapiGuardrails.IsSome then
                      RequestPart.header ("cf-brapi-guardrails", cfBrapiGuardrails.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/browser/{session_id}/page/{target_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 101 -> return BrapiGetDevtoolsPage.SwitchingProtocols
            | 400 -> return BrapiGetDevtoolsPage.BadRequest((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsPage.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsPage" (int status)
        }

    ///<summary>
    ///List active browser sessions.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="limit"></param>
    ///<param name="offset"></param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsSessionList
        (accountId: string, ?limit: float, ?offset: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/session"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiGetDevtoolsSessionList.OK((Serializer.deserialize content))
            | 500 -> return BrapiGetDevtoolsSessionList.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsSessionList" (int status)
        }

    ///<summary>
    ///Get details for a specific browser session.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="sessionId">Session ID.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiGetDevtoolsSessionDetails
        (accountId: string, sessionId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("session_id", sessionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/devtools/session/{session_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiGetDevtoolsSessionDetails.OK((Serializer.deserialize content))
            | 404 -> return BrapiGetDevtoolsSessionDetails.NotFound
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiGetDevtoolsSessionDetails" (int status)
        }

    ///<summary>
    ///Gets json from a webpage from a provided URL or HTML. Pass `prompt` or `schema` in the body. Control page loading with `gotoOptions` and `waitFor*` options.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPostJson
        (
            accountId: string,
            body: InlineUnion_9a6adabcd10359309a4f8db7,
            ?cacheTTL: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/json"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostJson.OK((Serializer.deserialize content))
            | 400 -> return BrapiPostJson.BadRequest((Serializer.deserialize content))
            | 422 -> return BrapiPostJson.UnprocessableEntity((Serializer.deserialize content))
            | 429 -> return BrapiPostJson.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiPostJson.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostJson" (int status)
        }

    ///<summary>
    ///Get links from a web page.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPostLinks
        (
            accountId: string,
            body: InlineUnion_67b9c47821ba3c28b391c2fd,
            ?cacheTTL: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/links"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostLinks.OK((Serializer.deserialize content))
            | 400 -> return BrapiPostLinks.BadRequest((Serializer.deserialize content))
            | 422 -> return BrapiPostLinks.UnprocessableEntity((Serializer.deserialize content))
            | 429 -> return BrapiPostLinks.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiPostLinks.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostLinks" (int status)
        }

    ///<summary>
    ///Gets markdown of a webpage from provided URL or HTML. Control page loading with `gotoOptions` and `waitFor*` options.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPostMarkdown
        (
            accountId: string,
            body: InlineUnion_3e407f5198d828a72350ca51,
            ?cacheTTL: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/markdown"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostMarkdown.OK((Serializer.deserialize content))
            | 400 -> return BrapiPostMarkdown.BadRequest((Serializer.deserialize content))
            | 422 -> return BrapiPostMarkdown.UnprocessableEntity((Serializer.deserialize content))
            | 429 -> return BrapiPostMarkdown.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiPostMarkdown.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostMarkdown" (int status)
        }

    ///<summary>
    ///Fetches rendered PDF from provided URL or HTML. Check available options like `gotoOptions` and `waitFor*` to control page load behaviour.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPostPdf
        (
            accountId: string,
            body: InlineUnion_138890d107228fb17f3273ec,
            ?cacheTTL: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value) ]

            let! (status, _, contentBinary) =
                OpenApiHttp.postBinaryAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/pdf"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostPdf.OK(contentBinary)
            | 400 ->
                let content = Encoding.UTF8.GetString contentBinary
                return BrapiPostPdf.BadRequest((Serializer.deserialize content))
            | 422 ->
                let content = Encoding.UTF8.GetString contentBinary
                return BrapiPostPdf.UnprocessableEntity((Serializer.deserialize content))
            | 429 ->
                let content = Encoding.UTF8.GetString contentBinary
                return BrapiPostPdf.TooManyRequests((Serializer.deserialize content))
            | 500 ->
                let content = Encoding.UTF8.GetString contentBinary
                return BrapiPostPdf.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostPdf" (int status)
        }

    ///<summary>
    ///Get meta attributes like height, width, text and others of selected elements.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPostScrape
        (
            accountId: string,
            body: InlineUnion_eefdeba68cafd7b731e99db2,
            ?cacheTTL: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/scrape"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostScrape.OK((Serializer.deserialize content))
            | 400 -> return BrapiPostScrape.BadRequest((Serializer.deserialize content))
            | 422 -> return BrapiPostScrape.UnprocessableEntity((Serializer.deserialize content))
            | 429 -> return BrapiPostScrape.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiPostScrape.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostScrape" (int status)
        }

    ///<summary>
    ///Takes a screenshot of a webpage from provided URL or HTML. Control page loading with `gotoOptions` and `waitFor*` options. Customize screenshots with `viewport`, `fullPage`, `clip` and others.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPostScreenshot
        (
            accountId: string,
            body: InlineUnion_0aa1ba48bfe5292f91790d3a,
            ?cacheTTL: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/screenshot"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostScreenshot.OK((Serializer.deserialize content))
            | 400 -> return BrapiPostScreenshot.BadRequest((Serializer.deserialize content))
            | 422 -> return BrapiPostScreenshot.UnprocessableEntity((Serializer.deserialize content))
            | 429 -> return BrapiPostScreenshot.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiPostScreenshot.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostScreenshot" (int status)
        }

    ///<summary>
    ///Returns the page's HTML content and screenshot. Control page loading with `gotoOptions` and `waitFor*` options. Customize screenshots with `viewport`, `fullPage`, `clip` and others.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cacheTTL">Cache TTL default is 5s. Set to 0 to disable.</param>
    ///<param name="cancellationToken"></param>
    member this.BrapiPostSnapshot
        (
            accountId: string,
            body: InlineUnion_5c2d22d159419a7d46a2dd9e,
            ?cacheTTL: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cacheTTL.IsSome then
                      RequestPart.query ("cacheTTL", cacheTTL.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-rendering/snapshot"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BrapiPostSnapshot.OK((Serializer.deserialize content))
            | 400 -> return BrapiPostSnapshot.BadRequest((Serializer.deserialize content))
            | 422 -> return BrapiPostSnapshot.UnprocessableEntity((Serializer.deserialize content))
            | 429 -> return BrapiPostSnapshot.TooManyRequests((Serializer.deserialize content))
            | 500 -> return BrapiPostSnapshot.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BrapiPostSnapshot" (int status)
        }

    ///<summary>
    ///Retrieve account limits and usage information
    ///</summary>
    member this.GetAccountLimits(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/account/limits"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetAccountLimits.OK((Serializer.deserialize content))
            | 401 -> return GetAccountLimits.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetAccountLimits" (int status)
        }

    ///<summary>
    ///Retrieve builds for specific version IDs
    ///</summary>
    member this.GetBuildsByVersionIds(accountId: string, versionIds: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.query ("version_ids", versionIds) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/builds/builds" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetBuildsByVersionIds.OK((Serializer.deserialize content))
            | 401 -> return GetBuildsByVersionIds.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetBuildsByVersionIds" (int status)
        }

    ///<summary>
    ///Retrieve the most recent builds for multiple worker scripts
    ///</summary>
    member this.GetLatestBuildsByScripts
        (accountId: string, externalScriptIds: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.query ("external_script_ids", externalScriptIds) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/builds/latest"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetLatestBuildsByScripts.OK((Serializer.deserialize content))
            | 401 -> return GetLatestBuildsByScripts.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetLatestBuildsByScripts" (int status)
        }

    ///<summary>
    ///Retrieve detailed information about a specific build
    ///</summary>
    member this.GetBuildByUuid(accountId: string, buildUuid: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("build_uuid", buildUuid) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/builds/{build_uuid}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetBuildByUuid.OK((Serializer.deserialize content))
            | 401 -> return GetBuildByUuid.Unauthorized((Serializer.deserialize content))
            | 404 -> return GetBuildByUuid.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetBuildByUuid" (int status)
        }

    ///<summary>
    ///Cancel a running or queued build
    ///</summary>
    member this.CancelBuildByUuid(accountId: string, buildUuid: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("build_uuid", buildUuid) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/builds/builds/{build_uuid}/cancel"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CancelBuildByUuid.OK((Serializer.deserialize content))
            | 401 -> return CancelBuildByUuid.Unauthorized((Serializer.deserialize content))
            | 404 -> return CancelBuildByUuid.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CancelBuildByUuid" (int status)
        }

    ///<summary>
    ///Retrieve logs for a specific build with cursor-based pagination
    ///</summary>
    member this.GetBuildLogs
        (accountId: string, buildUuid: System.Guid, ?cursor: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("build_uuid", buildUuid)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/builds/{build_uuid}/logs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetBuildLogs.OK((Serializer.deserialize content))
            | 401 -> return GetBuildLogs.Unauthorized((Serializer.deserialize content))
            | 404 -> return GetBuildLogs.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetBuildLogs" (int status)
        }

    ///<summary>
    ///Upsert a repository connection for CI/CD integration
    ///</summary>
    member this.UpsertRepoConnection
        (accountId: string, body: builds_UpsertRepoConnectionRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/builds/repos/connections"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpsertRepoConnection.OK((Serializer.deserialize content))
            | 401 -> return UpsertRepoConnection.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpsertRepoConnection" (int status)
        }

    ///<summary>
    ///Remove a repository connection
    ///</summary>
    member this.DeleteRepoConnection
        (accountId: string, repoConnectionUuid: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("repo_connection_uuid", repoConnectionUuid) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/builds/repos/connections/{repo_connection_uuid}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteRepoConnection.OK((Serializer.deserialize content))
            | 401 -> return DeleteRepoConnection.Unauthorized((Serializer.deserialize content))
            | 404 -> return DeleteRepoConnection.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteRepoConnection" (int status)
        }

    ///<summary>
    ///Analyze repository for automatic configuration detection
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="providerType">SCM provider type</param>
    ///<param name="providerAccountId"></param>
    ///<param name="repoId"></param>
    ///<param name="branch"></param>
    ///<param name="rootDirectory"></param>
    ///<param name="cancellationToken"></param>
    member this.GetWorkerConfigAutofill
        (
            accountId: string,
            providerType: string,
            providerAccountId: string,
            repoId: string,
            branch: string,
            ?rootDirectory: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("provider_type", providerType)
                  RequestPart.path ("provider_account_id", providerAccountId)
                  RequestPart.path ("repo_id", repoId)
                  RequestPart.query ("branch", branch)
                  if rootDirectory.IsSome then
                      RequestPart.query ("root_directory", rootDirectory.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/repos/{provider_type}/{provider_account_id}/{repo_id}/config_autofill"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetWorkerConfigAutofill.OK((Serializer.deserialize content))
            | 401 -> return GetWorkerConfigAutofill.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetWorkerConfigAutofill" (int status)
        }

    ///<summary>
    ///Get all build tokens with pagination
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="page">Page number for pagination</param>
    ///<param name="perPage">Number of items per page</param>
    ///<param name="cancellationToken"></param>
    member this.ListBuildTokens(accountId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/builds/tokens" requestParts cancellationToken

            match (int status) with
            | 200 -> return ListBuildTokens.OK((Serializer.deserialize content))
            | 401 -> return ListBuildTokens.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListBuildTokens" (int status)
        }

    ///<summary>
    ///Create a new build authentication token
    ///</summary>
    member this.CreateBuildToken
        (accountId: string, body: builds_CreateBuildTokenRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/builds/tokens" requestParts cancellationToken

            match (int status) with
            | 200 -> return CreateBuildToken.OK((Serializer.deserialize content))
            | 401 -> return CreateBuildToken.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateBuildToken" (int status)
        }

    ///<summary>
    ///Remove a build authentication token
    ///</summary>
    member this.DeleteBuildToken
        (accountId: string, buildTokenUuid: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("build_token_uuid", buildTokenUuid) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/builds/tokens/{build_token_uuid}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteBuildToken.OK((Serializer.deserialize content))
            | 401 -> return DeleteBuildToken.Unauthorized((Serializer.deserialize content))
            | 404 -> return DeleteBuildToken.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteBuildToken" (int status)
        }

    ///<summary>
    ///Create a new CI/CD trigger
    ///</summary>
    member this.CreateTrigger
        (accountId: string, body: builds_CreateTriggerRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/builds/triggers" requestParts cancellationToken

            match (int status) with
            | 200 -> return CreateTrigger.OK((Serializer.deserialize content))
            | 401 -> return CreateTrigger.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateTrigger" (int status)
        }

    ///<summary>
    ///Remove a CI/CD trigger
    ///</summary>
    member this.DeleteTrigger(accountId: string, triggerUuid: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("trigger_uuid", triggerUuid) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/builds/triggers/{trigger_uuid}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteTrigger.OK((Serializer.deserialize content))
            | 401 -> return DeleteTrigger.Unauthorized((Serializer.deserialize content))
            | 404 -> return DeleteTrigger.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteTrigger" (int status)
        }

    ///<summary>
    ///Update an existing CI/CD trigger
    ///</summary>
    member this.UpdateTrigger
        (
            accountId: string,
            triggerUuid: System.Guid,
            body: builds_UpdateTriggerRequest,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("trigger_uuid", triggerUuid)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/builds/triggers/{trigger_uuid}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateTrigger.OK((Serializer.deserialize content))
            | 401 -> return UpdateTrigger.Unauthorized((Serializer.deserialize content))
            | 404 -> return UpdateTrigger.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateTrigger" (int status)
        }

    ///<summary>
    ///Trigger a manual build for a specific trigger
    ///</summary>
    member this.CreateManualBuild
        (
            accountId: string,
            triggerUuid: System.Guid,
            body: builds_CreateBuildRequest,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("trigger_uuid", triggerUuid)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/builds/triggers/{trigger_uuid}/builds"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateManualBuild.OK((Serializer.deserialize content))
            | 401 -> return CreateManualBuild.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateManualBuild" (int status)
        }

    ///<summary>
    ///Get all environment variables for a trigger
    ///</summary>
    member this.ListEnvironmentVariables
        (accountId: string, triggerUuid: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("trigger_uuid", triggerUuid) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/triggers/{trigger_uuid}/environment_variables"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListEnvironmentVariables.OK((Serializer.deserialize content))
            | 401 -> return ListEnvironmentVariables.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListEnvironmentVariables" (int status)
        }

    ///<summary>
    ///Create or update environment variables for a trigger
    ///</summary>
    member this.UpsertEnvironmentVariables
        (
            accountId: string,
            triggerUuid: System.Guid,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("trigger_uuid", triggerUuid)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/builds/triggers/{trigger_uuid}/environment_variables"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpsertEnvironmentVariables.OK((Serializer.deserialize content))
            | 401 -> return UpsertEnvironmentVariables.Unauthorized((Serializer.deserialize content))
            | 404 -> return UpsertEnvironmentVariables.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpsertEnvironmentVariables" (int status)
        }

    ///<summary>
    ///Remove a specific environment variable from a trigger
    ///</summary>
    member this.DeleteEnvironmentVariable
        (
            accountId: string,
            triggerUuid: System.Guid,
            environmentVariableKey: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("trigger_uuid", triggerUuid)
                  RequestPart.path ("environment_variable_key", environmentVariableKey) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/builds/triggers/{trigger_uuid}/environment_variables/{environment_variable_key}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteEnvironmentVariable.OK((Serializer.deserialize content))
            | 401 -> return DeleteEnvironmentVariable.Unauthorized((Serializer.deserialize content))
            | 404 -> return DeleteEnvironmentVariable.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteEnvironmentVariable" (int status)
        }

    ///<summary>
    ///Clear the build cache for a specific trigger
    ///</summary>
    member this.PurgeBuildCache(accountId: string, triggerUuid: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("trigger_uuid", triggerUuid) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/builds/triggers/{trigger_uuid}/purge_build_cache"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PurgeBuildCache.OK((Serializer.deserialize content))
            | 401 -> return PurgeBuildCache.Unauthorized((Serializer.deserialize content))
            | 404 -> return PurgeBuildCache.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PurgeBuildCache" (int status)
        }

    ///<summary>
    ///Create a new build configuration for a Worker script, linking it to a git repository with CI/CD triggers.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body">Request body for creating a Worker build configuration.</param>
    ///<param name="cancellationToken"></param>
    member this.CreateWorkerBuild
        (accountId: string, body: builds_CreateWorkerRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/builds/workers" requestParts cancellationToken

            match (int status) with
            | 201 -> return CreateWorkerBuild.Created((Serializer.deserialize content))
            | 400 -> return CreateWorkerBuild.BadRequest((Serializer.deserialize content))
            | 401 -> return CreateWorkerBuild.Unauthorized((Serializer.deserialize content))
            | 404 -> return CreateWorkerBuild.NotFound((Serializer.deserialize content))
            | 409 -> return CreateWorkerBuild.Conflict((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateWorkerBuild" (int status)
        }

    ///<summary>
    ///Get all builds for a specific worker script with pagination
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="externalScriptId"></param>
    ///<param name="page">Page number for pagination</param>
    ///<param name="perPage">Number of items per page</param>
    ///<param name="cancellationToken"></param>
    member this.ListBuildsByScript
        (accountId: string, externalScriptId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("external_script_id", externalScriptId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/workers/{external_script_id}/builds"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListBuildsByScript.OK((Serializer.deserialize content))
            | 401 -> return ListBuildsByScript.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListBuildsByScript" (int status)
        }

    ///<summary>
    ///Get all triggers for a specific worker script
    ///</summary>
    member this.ListTriggersByScript
        (accountId: string, externalScriptId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("external_script_id", externalScriptId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/workers/{external_script_id}/triggers"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListTriggersByScript.OK((Serializer.deserialize content))
            | 401 -> return ListTriggersByScript.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListTriggersByScript" (int status)
        }

    ///<summary>
    ///Get all deploy hooks for a specific worker script.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName">Human-readable name of the worker.</param>
    ///<param name="cancellationToken"></param>
    member this.ListDeployHooks(accountId: string, scriptName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/workers/{script_name}/deploy_hooks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListDeployHooks.OK((Serializer.deserialize content))
            | 401 -> return ListDeployHooks.Unauthorized((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListDeployHooks" (int status)
        }

    ///<summary>
    ///Create a new deploy hook for a worker script.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName">Human-readable name of the worker.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreateDeployHook
        (
            accountId: string,
            scriptName: string,
            body: builds_CreateDeployHookRequest,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/builds/workers/{script_name}/deploy_hooks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateDeployHook.OK((Serializer.deserialize content))
            | 400 -> return CreateDeployHook.BadRequest((Serializer.deserialize content))
            | 401 -> return CreateDeployHook.Unauthorized((Serializer.deserialize content))
            | 404 -> return CreateDeployHook.NotFound((Serializer.deserialize content))
            | 409 -> return CreateDeployHook.Conflict((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateDeployHook" (int status)
        }

    ///<summary>
    ///Delete a deploy hook.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName">Human-readable name of the worker.</param>
    ///<param name="deployHookUuid">Deploy hook UUID</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteDeployHook
        (accountId: string, scriptName: string, deployHookUuid: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("deploy_hook_uuid", deployHookUuid) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/builds/workers/{script_name}/deploy_hooks/{deploy_hook_uuid}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteDeployHook.OK((Serializer.deserialize content))
            | 401 -> return DeleteDeployHook.Unauthorized((Serializer.deserialize content))
            | 404 -> return DeleteDeployHook.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteDeployHook" (int status)
        }

    ///<summary>
    ///Get details of a specific deploy hook.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName">Human-readable name of the worker.</param>
    ///<param name="deployHookUuid">Deploy hook UUID</param>
    ///<param name="cancellationToken"></param>
    member this.GetDeployHook
        (accountId: string, scriptName: string, deployHookUuid: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("deploy_hook_uuid", deployHookUuid) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/workers/{script_name}/deploy_hooks/{deploy_hook_uuid}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetDeployHook.OK((Serializer.deserialize content))
            | 401 -> return GetDeployHook.Unauthorized((Serializer.deserialize content))
            | 404 -> return GetDeployHook.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetDeployHook" (int status)
        }

    ///<summary>
    ///Update an existing deploy hook.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName">Human-readable name of the worker.</param>
    ///<param name="deployHookUuid">Deploy hook UUID</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateDeployHook
        (
            accountId: string,
            scriptName: string,
            deployHookUuid: System.Guid,
            body: builds_CreateDeployHookRequest,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("deploy_hook_uuid", deployHookUuid)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/builds/workers/{script_name}/deploy_hooks/{deploy_hook_uuid}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateDeployHook.OK((Serializer.deserialize content))
            | 400 -> return UpdateDeployHook.BadRequest((Serializer.deserialize content))
            | 401 -> return UpdateDeployHook.Unauthorized((Serializer.deserialize content))
            | 404 -> return UpdateDeployHook.NotFound((Serializer.deserialize content))
            | 409 -> return UpdateDeployHook.Conflict((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateDeployHook" (int status)
        }

    ///<summary>
    ///Delete the build configuration for a Worker script.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptTag">The Worker script tag (external script ID)</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteWorkerBuild(accountId: string, scriptTag: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_tag", scriptTag) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/builds/workers/{script_tag}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteWorkerBuild.OK((Serializer.deserialize content))
            | 401 -> return DeleteWorkerBuild.Unauthorized((Serializer.deserialize content))
            | 404 -> return DeleteWorkerBuild.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteWorkerBuild" (int status)
        }

    ///<summary>
    ///Retrieve the build configuration for a specific Worker script, including git repository details and production settings.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptTag">The Worker script tag (external script ID)</param>
    ///<param name="cancellationToken"></param>
    member this.GetWorkerBuild(accountId: string, scriptTag: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_tag", scriptTag) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/builds/workers/{script_tag}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetWorkerBuild.OK((Serializer.deserialize content))
            | 401 -> return GetWorkerBuild.Unauthorized((Serializer.deserialize content))
            | 404 -> return GetWorkerBuild.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetWorkerBuild" (int status)
        }

    ///<summary>
    ///Update the build configuration for a Worker script. Supports partial updates to git repository settings and production build settings.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptTag">The Worker script tag (external script ID)</param>
    ///<param name="body">Request body for updating a Worker build configuration. At least one field must be provided.</param>
    ///<param name="cancellationToken"></param>
    member this.UpdateWorkerBuild
        (accountId: string, scriptTag: string, body: builds_UpdateWorkerRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_tag", scriptTag)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/builds/workers/{script_tag}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateWorkerBuild.OK((Serializer.deserialize content))
            | 400 -> return UpdateWorkerBuild.BadRequest((Serializer.deserialize content))
            | 401 -> return UpdateWorkerBuild.Unauthorized((Serializer.deserialize content))
            | 404 -> return UpdateWorkerBuild.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateWorkerBuild" (int status)
        }

    ///<summary>
    ///Lists all the applications that are associated with your account.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="name">Filter applications by name.</param>
    ///<param name="image">Filter applications by image.</param>
    ///<param name="cancellationToken"></param>
    member this.ListApplications
        (accountId: string, ?name: string, ?image: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if image.IsSome then
                      RequestPart.query ("image", image.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/containers/applications"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListApplications.OK((Serializer.deserialize content))
            | 401 -> return ListApplications.Unauthorized((Serializer.deserialize content))
            | 500 -> return ListApplications.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListApplications" (int status)
        }

    ///<summary>
    ///Create a Containers application.
    ///Use `scheduling_policy: "default"` for a scheduler-backed application. The
    ///Containers scheduler maintains the requested instance count and manages
    ///deployment configuration, placement, scaling, versions, and rollouts.
    ///Use `scheduling_policy: "durable_object"` for a Durable Object-managed
    ///application. Each Durable Object creates and manages the lifecycle of its
    ///container instance. This request accepts only `name`, `scheduling_policy`,
    ///and `durable_objects`. Application-level configuration, scaling, constraints,
    ///versions, and rollouts do not apply.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="body">
    ///Create a Containers application. Set `scheduling_policy` to `default` for a
    ///scheduler-backed application with deployment configuration, instance counts,
    ///constraints, versions, and rollouts. Set it to `durable_object` for a
    ///Durable Object-managed application where each Durable Object creates and manages
    ///the lifecycle of its container instance. For `durable_object` requests, supply
    ///only `name`, `scheduling_policy`, and `durable_objects`.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.CreateApplication
        (accountId: string, body: cc_ContainersCreateApplicationRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/containers/applications"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return CreateApplication.Created((Serializer.deserialize content))
            | 400 -> return CreateApplication.BadRequest((Serializer.deserialize content))
            | 401 -> return CreateApplication.Unauthorized((Serializer.deserialize content))
            | 403 -> return CreateApplication.Forbidden((Serializer.deserialize content))
            | 500 -> return CreateApplication.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateApplication" (int status)
        }

    ///<summary>
    ///Deletes a single application by id.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="applicationId"></param>
    ///<param name="cancellationToken"></param>
    member this.DeleteApplication(accountId: string, applicationId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("application_id", applicationId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/containers/applications/{application_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteApplication.OK((Serializer.deserialize content))
            | 401 -> return DeleteApplication.Unauthorized((Serializer.deserialize content))
            | 404 -> return DeleteApplication.NotFound((Serializer.deserialize content))
            | 500 -> return DeleteApplication.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteApplication" (int status)
        }

    ///<summary>
    ///Returns a single application by id.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="applicationId"></param>
    ///<param name="cancellationToken"></param>
    member this.GetApplication(accountId: string, applicationId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("application_id", applicationId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/containers/applications/{application_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetApplication.OK((Serializer.deserialize content))
            | 401 -> return GetApplication.Unauthorized((Serializer.deserialize content))
            | 404 -> return GetApplication.NotFound((Serializer.deserialize content))
            | 500 -> return GetApplication.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetApplication" (int status)
        }

    ///<summary>
    ///Modifies a single application by id. Changes that replace instance deployment
    ///configuration, including the image, must be applied with a rollout.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="applicationId"></param>
    ///<param name="body">
    ///Request body for modifying a Containers application without replacing its instances.
    ///Deployment configuration changes such as image, resource allocation, command, and
    ///environment variables require an application rollout.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.ModifyApplication
        (
            accountId: string,
            applicationId: string,
            body: cc_ContainersModifyApplicationRequestBody,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("application_id", applicationId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/containers/applications/{application_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ModifyApplication.OK((Serializer.deserialize content))
            | 400 -> return ModifyApplication.BadRequest((Serializer.deserialize content))
            | 401 -> return ModifyApplication.Unauthorized((Serializer.deserialize content))
            | 403 -> return ModifyApplication.Forbidden((Serializer.deserialize content))
            | 404 -> return ModifyApplication.NotFound((Serializer.deserialize content))
            | 500 -> return ModifyApplication.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ModifyApplication" (int status)
        }

    ///<summary>
    ///Lists container instances belonging to an application.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="applicationId"></param>
    ///<param name="perPage">Maximum number of instances to return per page. Defaults to all.</param>
    ///<param name="pageToken">Opaque token from a previous response to retrieve the next page.</param>
    ///<param name="cancellationToken"></param>
    member this.ListContainerInstances
        (
            accountId: string,
            applicationId: string,
            ?perPage: int,
            ?pageToken: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("application_id", applicationId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if pageToken.IsSome then
                      RequestPart.query ("page_token", pageToken.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/containers/applications/{application_id}/instances"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListContainerInstances.OK((Serializer.deserialize content))
            | 400 -> return ListContainerInstances.BadRequest((Serializer.deserialize content))
            | 401 -> return ListContainerInstances.Unauthorized((Serializer.deserialize content))
            | 404 -> return ListContainerInstances.NotFound((Serializer.deserialize content))
            | 500 -> return ListContainerInstances.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListContainerInstances" (int status)
        }

    ///<summary>
    ///Returns a container instance belonging to an application.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="applicationId"></param>
    ///<param name="instanceId"></param>
    ///<param name="cancellationToken"></param>
    member this.GetContainerInstance
        (accountId: string, applicationId: string, instanceId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("application_id", applicationId)
                  RequestPart.path ("instance_id", instanceId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/containers/applications/{application_id}/instances/{instance_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetContainerInstance.OK((Serializer.deserialize content))
            | 400 -> return GetContainerInstance.BadRequest((Serializer.deserialize content))
            | 401 -> return GetContainerInstance.Unauthorized((Serializer.deserialize content))
            | 404 -> return GetContainerInstance.NotFound((Serializer.deserialize content))
            | 500 -> return GetContainerInstance.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetContainerInstance" (int status)
        }

    ///<summary>
    ///Creates a rollout to update the application's configuration across instances
    ///with minimal downtime. Rollouts apply only to scheduler-backed applications
    ///with `scheduling_policy: "default"`. Versions and rollouts do not apply to
    ///applications with `scheduling_policy: "durable_object"`.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="applicationId"></param>
    ///<param name="body">Request body to create a new rollout for a scheduler-backed application.</param>
    ///<param name="cancellationToken"></param>
    member this.CreateApplicationRollout
        (
            accountId: string,
            applicationId: string,
            body: cc_ContainersCreateApplicationRolloutRequest,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("application_id", applicationId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/containers/applications/{application_id}/rollouts"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return CreateApplicationRollout.Created((Serializer.deserialize content))
            | 400 -> return CreateApplicationRollout.BadRequest((Serializer.deserialize content))
            | 401 -> return CreateApplicationRollout.Unauthorized((Serializer.deserialize content))
            | 403 -> return CreateApplicationRollout.Forbidden((Serializer.deserialize content))
            | 404 -> return CreateApplicationRollout.NotFound((Serializer.deserialize content))
            | 500 -> return CreateApplicationRollout.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateApplicationRollout" (int status)
        }

    ///<summary>
    ///Returns all versions for a scheduler-backed application with
    ///`scheduling_policy: "default"`. Versions and rollouts do not apply to
    ///applications with `scheduling_policy: "durable_object"`.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="applicationId"></param>
    ///<param name="cancellationToken"></param>
    member this.ListApplicationVersions
        (accountId: string, applicationId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("application_id", applicationId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/containers/applications/{application_id}/versions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListApplicationVersions.OK((Serializer.deserialize content))
            | 401 -> return ListApplicationVersions.Unauthorized((Serializer.deserialize content))
            | 404 -> return ListApplicationVersions.NotFound((Serializer.deserialize content))
            | 500 -> return ListApplicationVersions.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListApplicationVersions" (int status)
        }

    ///<summary>
    ///Idempotently starts or observes preparation of the runtime artifacts required to run one digest-pinned managed container image on Cloudflare's network. Returns 202 while durable preparation continues and 200 when the image is ready or preparation has reached a terminal error.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="body">A digest-pinned managed container image to prepare for the new runtime.</param>
    ///<param name="cancellationToken"></param>
    member this.PrepareContainerImage
        (accountId: string, body: cc_PrepareContainerImageRequestBody, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/containers/image-preparations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PrepareContainerImage.OK((Serializer.deserialize content))
            | 202 -> return PrepareContainerImage.Accepted((Serializer.deserialize content))
            | 400 -> return PrepareContainerImage.BadRequest((Serializer.deserialize content))
            | 401 -> return PrepareContainerImage.Unauthorized((Serializer.deserialize content))
            | 403 -> return PrepareContainerImage.Forbidden((Serializer.deserialize content))
            | 500 -> return PrepareContainerImage.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PrepareContainerImage" (int status)
        }

    ///<summary>
    ///Get the list of configured registries in the account.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.ListImageRegistries(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/containers/registries"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListImageRegistries.OK((Serializer.deserialize content))
            | 401 -> return ListImageRegistries.Unauthorized((Serializer.deserialize content))
            | 500 -> return ListImageRegistries.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListImageRegistries" (int status)
        }

    ///<summary>
    ///Registers credentials for a supported private external image registry so
    ///Containers can pull images from it. This endpoint does not create a registry
    ///or upload an image. Public Docker Hub images and images in the Cloudflare
    ///managed registry do not require this configuration.
    ///Refer to [Image management](https://developers.cloudflare.com/containers/platform-details/image-management/)
    ///for supported registries and instructions for storing registry credentials.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="body">
    ///Configuration for connecting Containers to a supported private external image
    ///registry. See [Image management](https://developers.cloudflare.com/containers/platform-details/image-management/)
    ///for supported providers and credential setup instructions.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.CreateImageRegistry
        (accountId: string, body: cc_ContainersCreateImageRegistryRequestBody, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/containers/registries"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return CreateImageRegistry.Created((Serializer.deserialize content))
            | 400 -> return CreateImageRegistry.BadRequest((Serializer.deserialize content))
            | 403 -> return CreateImageRegistry.Forbidden((Serializer.deserialize content))
            | 409 -> return CreateImageRegistry.Conflict((Serializer.deserialize content))
            | 500 -> return CreateImageRegistry.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateImageRegistry" (int status)
        }

    ///<summary>
    ///Delete a registry from the account, this will prevent Containers from pulling images from the registry.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="domain"></param>
    ///<param name="cancellationToken"></param>
    member this.DeleteImageRegistry(accountId: string, domain: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain", domain) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/containers/registries/{domain}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteImageRegistry.OK((Serializer.deserialize content))
            | 403 -> return DeleteImageRegistry.Forbidden((Serializer.deserialize content))
            | 404 -> return DeleteImageRegistry.NotFound((Serializer.deserialize content))
            | 409 -> return DeleteImageRegistry.Conflict((Serializer.deserialize content))
            | 500 -> return DeleteImageRegistry.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteImageRegistry" (int status)
        }

    ///<summary>
    ///Generates credentials for accessing a configured container image registry.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="domain"></param>
    ///<param name="body">Specifies the configuration for the image registry credential to create. Cloudflare requires both fields for managed registries. For external registries, Cloudflare ignores both fields, and the registry provider determines the returned credentials' permissions and lifetime.</param>
    ///<param name="cancellationToken"></param>
    member this.GenerateImageRegistryCredentials
        (
            accountId: string,
            domain: string,
            body: cc_ImageRegistryCredentialsConfiguration,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain", domain)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/containers/registries/{domain}/credentials"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return GenerateImageRegistryCredentials.Created((Serializer.deserialize content))
            | 400 -> return GenerateImageRegistryCredentials.BadRequest((Serializer.deserialize content))
            | 403 -> return GenerateImageRegistryCredentials.Forbidden((Serializer.deserialize content))
            | 404 -> return GenerateImageRegistryCredentials.NotFound((Serializer.deserialize content))
            | 409 -> return GenerateImageRegistryCredentials.Conflict((Serializer.deserialize content))
            | 500 -> return GenerateImageRegistryCredentials.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GenerateImageRegistryCredentials" (int status)
        }

    ///<summary>
    ///Lists all apps in the account. Returns identity and audit fields only — flag definitions are not included.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipListApps(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/flagship/apps" requestParts cancellationToken

            match (int status) with
            | 200 -> return FlagshipListApps.OK((Serializer.deserialize content))
            | 400 -> return FlagshipListApps.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipListApps.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipListApps.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipListApps.NotFound((Serializer.deserialize content))
            | 429 -> return FlagshipListApps.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipListApps.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipListApps" (int status)
        }

    ///<summary>
    ///Creates an app. The returned `id` is used in all subsequent flag, changelog, and evaluation requests.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipCreateApp
        (accountId: string, body: FlagshipCreateAppPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/flagship/apps" requestParts cancellationToken

            match (int status) with
            | 201 -> return FlagshipCreateApp.Created((Serializer.deserialize content))
            | 400 -> return FlagshipCreateApp.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipCreateApp.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipCreateApp.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipCreateApp.NotFound((Serializer.deserialize content))
            | 429 -> return FlagshipCreateApp.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipCreateApp.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipCreateApp" (int status)
        }

    ///<summary>
    ///Deletes an app and all its flags and changelog history. Returns 409 if any Worker still references this app via a Flagship binding.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipDeleteApp(accountId: string, appId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipDeleteApp.OK((Serializer.deserialize content))
            | 400 -> return FlagshipDeleteApp.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipDeleteApp.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipDeleteApp.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipDeleteApp.NotFound((Serializer.deserialize content))
            | 409 -> return FlagshipDeleteApp.Conflict((Serializer.deserialize content))
            | 429 -> return FlagshipDeleteApp.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipDeleteApp.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipDeleteApp" (int status)
        }

    ///<summary>
    ///Returns an app's name and audit fields. Flag definitions are not included.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipGetApp(accountId: string, appId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipGetApp.OK((Serializer.deserialize content))
            | 400 -> return FlagshipGetApp.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipGetApp.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipGetApp.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipGetApp.NotFound((Serializer.deserialize content))
            | 429 -> return FlagshipGetApp.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipGetApp.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipGetApp" (int status)
        }

    ///<summary>
    ///Updates an app. Only `name` is mutable.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipUpdateApp
        (accountId: string, appId: string, body: FlagshipUpdateAppPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipUpdateApp.OK((Serializer.deserialize content))
            | 400 -> return FlagshipUpdateApp.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipUpdateApp.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipUpdateApp.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipUpdateApp.NotFound((Serializer.deserialize content))
            | 429 -> return FlagshipUpdateApp.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipUpdateApp.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipUpdateApp" (int status)
        }

    ///<summary>
    ///Returns an app's evaluation-only flag definitions for SDKs that evaluate locally. Send the returned `ETag` in `If-None-Match` to avoid downloading unchanged definitions.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="ifNoneMatch">Previously returned ETag, or `*`.</param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipGetDefinitions
        (accountId: string, appId: string, ?ifNoneMatch: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  if ifNoneMatch.IsSome then
                      RequestPart.header ("If-None-Match", ifNoneMatch.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}/definitions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipGetDefinitions.OK((Serializer.deserialize content))
            | 304 -> return FlagshipGetDefinitions.NotModified
            | 401 -> return FlagshipGetDefinitions.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipGetDefinitions.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipGetDefinitions.NotFound((Serializer.deserialize content))
            | 500 -> return FlagshipGetDefinitions.InternalServerError((Serializer.deserialize content))
            | 503 -> return FlagshipGetDefinitions.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipGetDefinitions" (int status)
        }

    ///<summary>
    ///Evaluates a flag against the provided context. Pass context attributes as query parameters; values are forwarded as strings. For low-latency in-Worker evaluation, prefer the Flagship binding over this endpoint.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="flagKey">The flag key to evaluate.</param>
    ///<param name="targetingKey">Context targeting key (per OpenFeature spec); used for percentage rollout bucketing.</param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipEvaluateFlag
        (accountId: string, appId: string, flagKey: string, ?targetingKey: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.query ("flagKey", flagKey)
                  if targetingKey.IsSome then
                      RequestPart.query ("targetingKey", targetingKey.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}/evaluate"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipEvaluateFlag.OK((Serializer.deserialize content))
            | 400 -> return FlagshipEvaluateFlag.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipEvaluateFlag.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipEvaluateFlag.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipEvaluateFlag.NotFound((Serializer.deserialize content))
            | 500 -> return FlagshipEvaluateFlag.InternalServerError((Serializer.deserialize content))
            | 503 -> return FlagshipEvaluateFlag.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipEvaluateFlag" (int status)
        }

    ///<summary>
    ///Evaluates a flag against the provided context, passed as a JSON request body (OFREP-shaped) rather than query parameters. Returns the same response shape as the GET variant.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipEvaluateFlagPost
        (accountId: string, appId: string, body: FlagshipEvaluateFlagPostPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}/evaluate"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipEvaluateFlagPost.OK((Serializer.deserialize content))
            | 400 -> return FlagshipEvaluateFlagPost.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipEvaluateFlagPost.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipEvaluateFlagPost.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipEvaluateFlagPost.NotFound((Serializer.deserialize content))
            | 500 -> return FlagshipEvaluateFlagPost.InternalServerError((Serializer.deserialize content))
            | 503 -> return FlagshipEvaluateFlagPost.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipEvaluateFlagPost" (int status)
        }

    ///<summary>
    ///Lists an app's flags ordered by key. Pass `cursor` from `result_info` to page forward; a null cursor indicates the last page.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="limit">Max items to return (1–200).</param>
    ///<param name="cursor">Pagination cursor from a previous response.</param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipListFlags
        (accountId: string, appId: string, ?limit: string, ?cursor: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}/flags"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipListFlags.OK((Serializer.deserialize content))
            | 400 -> return FlagshipListFlags.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipListFlags.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipListFlags.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipListFlags.NotFound((Serializer.deserialize content))
            | 429 -> return FlagshipListFlags.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipListFlags.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipListFlags" (int status)
        }

    ///<summary>
    ///Creates a flag. Returns 409 if the key already exists. `type` is inferred from variation values and may be omitted.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipCreateFlag
        (accountId: string, appId: string, body: FlagshipCreateFlagPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}/flags"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return FlagshipCreateFlag.Created((Serializer.deserialize content))
            | 400 -> return FlagshipCreateFlag.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipCreateFlag.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipCreateFlag.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipCreateFlag.NotFound((Serializer.deserialize content))
            | 429 -> return FlagshipCreateFlag.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipCreateFlag.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipCreateFlag" (int status)
        }

    ///<summary>
    ///Deletes a flag permanently. Subsequent evaluations fall back to the caller-supplied default. Cannot be undone.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="flagKey">Flag key (slug).</param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipDeleteFlag
        (accountId: string, appId: string, flagKey: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("flag_key", flagKey) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}/flags/{flag_key}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipDeleteFlag.OK((Serializer.deserialize content))
            | 400 -> return FlagshipDeleteFlag.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipDeleteFlag.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipDeleteFlag.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipDeleteFlag.NotFound((Serializer.deserialize content))
            | 429 -> return FlagshipDeleteFlag.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipDeleteFlag.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipDeleteFlag" (int status)
        }

    ///<summary>
    ///Returns the full flag definition including rules, variations, and audit fields.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="flagKey">Flag key (slug).</param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipGetFlag
        (accountId: string, appId: string, flagKey: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("flag_key", flagKey) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}/flags/{flag_key}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipGetFlag.OK((Serializer.deserialize content))
            | 400 -> return FlagshipGetFlag.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipGetFlag.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipGetFlag.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipGetFlag.NotFound((Serializer.deserialize content))
            | 429 -> return FlagshipGetFlag.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipGetFlag.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipGetFlag" (int status)
        }

    ///<summary>
    ///Replaces the entire flag definition. Omitted fields are dropped, not preserved — read before writing. Each update appends a changelog entry.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="flagKey">Flag key (slug).</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipUpdateFlag
        (
            accountId: string,
            appId: string,
            flagKey: string,
            body: FlagshipUpdateFlagPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("flag_key", flagKey)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}/flags/{flag_key}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipUpdateFlag.OK((Serializer.deserialize content))
            | 400 -> return FlagshipUpdateFlag.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipUpdateFlag.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipUpdateFlag.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipUpdateFlag.NotFound((Serializer.deserialize content))
            | 429 -> return FlagshipUpdateFlag.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipUpdateFlag.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipUpdateFlag" (int status)
        }

    ///<summary>
    ///Returns the audit history for a flag, newest first. Each entry includes the event type and full flag state after the change; `update` entries include a field-level diff. Capped at 200 entries per flag.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="appId">App identifier.</param>
    ///<param name="flagKey">Flag key (slug).</param>
    ///<param name="limit">Max items to return (1–200).</param>
    ///<param name="cursor">Pagination cursor from a previous response.</param>
    ///<param name="cancellationToken"></param>
    member this.FlagshipGetFlagChangelog
        (
            accountId: string,
            appId: string,
            flagKey: string,
            ?limit: string,
            ?cursor: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("flag_key", flagKey)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/flagship/apps/{app_id}/flags/{flag_key}/changelog"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FlagshipGetFlagChangelog.OK((Serializer.deserialize content))
            | 400 -> return FlagshipGetFlagChangelog.BadRequest((Serializer.deserialize content))
            | 401 -> return FlagshipGetFlagChangelog.Unauthorized((Serializer.deserialize content))
            | 403 -> return FlagshipGetFlagChangelog.Forbidden((Serializer.deserialize content))
            | 404 -> return FlagshipGetFlagChangelog.NotFound((Serializer.deserialize content))
            | 429 -> return FlagshipGetFlagChangelog.TooManyRequests((Serializer.deserialize content))
            | 500 -> return FlagshipGetFlagChangelog.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FlagshipGetFlagChangelog" (int status)
        }

    ///<summary>
    ///Fetch a list of all user projects.
    ///</summary>
    member this.PagesProjectGetProjects
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
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/pages/projects" requestParts cancellationToken

            match (int status) with
            | 200 -> return PagesProjectGetProjects.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesProjectGetProjects.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesProjectGetProjects" (int status)
        }

    ///<summary>
    ///Create a new project.
    ///</summary>
    member this.PagesProjectCreateProject
        (accountId: string, body: PagesProjectCreateProjectPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/pages/projects" requestParts cancellationToken

            match (int status) with
            | 200 -> return PagesProjectCreateProject.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesProjectCreateProject.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesProjectCreateProject" (int status)
        }

    ///<summary>
    ///Delete a project by name.
    ///</summary>
    member this.PagesProjectDeleteProject
        (projectName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesProjectDeleteProject.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesProjectDeleteProject.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesProjectDeleteProject" (int status)
        }

    ///<summary>
    ///Fetch a project by name.
    ///</summary>
    member this.PagesProjectGetProject(projectName: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesProjectGetProject.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesProjectGetProject.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesProjectGetProject" (int status)
        }

    ///<summary>
    ///Set new attributes for an existing project. Modify environment variables. To delete an environment variable, set the key to null.
    ///</summary>
    member this.PagesProjectUpdateProject
        (
            projectName: string,
            accountId: string,
            body: PagesProjectUpdateProjectPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesProjectUpdateProject.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesProjectUpdateProject.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesProjectUpdateProject" (int status)
        }

    ///<summary>
    ///Fetch a list of project deployments.
    ///</summary>
    member this.PagesDeploymentGetDeployments
        (
            projectName: string,
            accountId: string,
            ?env: string,
            ?page: int,
            ?perPage: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId)
                  if env.IsSome then
                      RequestPart.query ("env", env.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/deployments"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDeploymentGetDeployments.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDeploymentGetDeployments.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDeploymentGetDeployments" (int status)
        }

    ///<summary>
    ///Start a new deployment from production. The repository and account must have already been authorized on the Cloudflare Pages dashboard.
    ///</summary>
    ///<param name="projectName"></param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="wranglerConfigHash">Hash of the Wrangler configuration file used for this deployment.</param>
    ///<param name="pagesBuildOutputDir">The build output directory path.</param>
    ///<param name="manifest">
    ///JSON string containing a manifest of files to deploy. Maps file paths to their content hashes.
    ///Required for direct upload deployments. Maximum 20,000 entries.
    ///</param>
    ///<param name="functionsFilepathRoutingConfigJson">Functions routing configuration file.</param>
    ///<param name="commitMessage">Git commit message associated with this deployment.</param>
    ///<param name="commitHash">Git commit SHA associated with this deployment.</param>
    ///<param name="commitDirty">Boolean string indicating if the working directory has uncommitted changes.</param>
    ///<param name="branch">The branch to build the new deployment from. The `HEAD` of the branch will be used. If omitted, the production branch will be used by default.</param>
    ///<param name="workerJs">
    ///Worker JavaScript file. Mutually exclusive with `_worker.bundle`.
    ///Cannot specify both `_worker.js` and `_worker.bundle` in the same request.
    ///</param>
    ///<param name="workerBundle">
    ///Worker bundle file in multipart/form-data format. Mutually exclusive with `_worker.js`.
    ///Cannot specify both `_worker.js` and `_worker.bundle` in the same request.
    ///Maximum size: 25 MiB.
    ///</param>
    ///<param name="routesJson">Routes configuration file defining routing rules.</param>
    ///<param name="redirects">Redirects configuration file for the deployment.</param>
    ///<param name="headers">Headers configuration file for the deployment.</param>
    member this.PagesDeploymentCreateDeployment
        (
            projectName: string,
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?wranglerConfigHash: string,
            ?pagesBuildOutputDir: string,
            ?manifest: string,
            ?functionsFilepathRoutingConfigJson: MultipartFile,
            ?commitMessage: string,
            ?commitHash: string,
            ?commitDirty: string,
            ?branch: string,
            ?workerJs: MultipartFile,
            ?workerBundle: MultipartFile,
            ?routesJson: MultipartFile,
            ?redirects: MultipartFile,
            ?headers: MultipartFile
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields
                      [ "wrangler_config_hash"
                        "pages_build_output_dir"
                        "manifest"
                        "functions-filepath-routing-config.json"
                        "commit_message"
                        "commit_hash"
                        "commit_dirty"
                        "branch"
                        "_worker.js"
                        "_worker.bundle"
                        "_routes.json"
                        "_redirects"
                        "_headers" ]
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId)
                  if wranglerConfigHash.IsSome then
                      RequestPart.multipartScalar ("wrangler_config_hash", "text/plain", wranglerConfigHash.Value)
                  if pagesBuildOutputDir.IsSome then
                      RequestPart.multipartScalar ("pages_build_output_dir", "text/plain", pagesBuildOutputDir.Value)
                  if manifest.IsSome then
                      RequestPart.multipartJson ("manifest", "application/json", manifest.Value)
                  if functionsFilepathRoutingConfigJson.IsSome then
                      RequestPart.multipartBinary (
                          "functions-filepath-routing-config.json",
                          "application/json",
                          functionsFilepathRoutingConfigJson.Value
                      )
                  if commitMessage.IsSome then
                      RequestPart.multipartScalar ("commit_message", "text/plain", commitMessage.Value)
                  if commitHash.IsSome then
                      RequestPart.multipartScalar ("commit_hash", "text/plain", commitHash.Value)
                  if commitDirty.IsSome then
                      RequestPart.multipartScalar ("commit_dirty", "text/plain", commitDirty.Value)
                  if branch.IsSome then
                      RequestPart.multipartScalar ("branch", "text/plain", branch.Value)
                  if workerJs.IsSome then
                      RequestPart.multipartBinary (
                          "_worker.js",
                          "application/javascript+module, text/javascript+module, application/javascript, text/javascript",
                          workerJs.Value
                      )
                  if workerBundle.IsSome then
                      RequestPart.multipartBinary ("_worker.bundle", "multipart/form-data", workerBundle.Value)
                  if routesJson.IsSome then
                      RequestPart.multipartBinary ("_routes.json", "application/json", routesJson.Value)
                  if redirects.IsSome then
                      RequestPart.multipartBinary ("_redirects", "text/plain", redirects.Value)
                  if headers.IsSome then
                      RequestPart.multipartBinary ("_headers", "text/plain", headers.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/deployments"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDeploymentCreateDeployment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDeploymentCreateDeployment.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDeploymentCreateDeployment" (int status)
        }

    ///<summary>
    ///Delete a deployment.
    ///</summary>
    member this.PagesDeploymentDeleteDeployment
        (
            deploymentId: string,
            projectName: string,
            accountId: string,
            ?force: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("deployment_id", deploymentId)
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId)
                  if force.IsSome then
                      RequestPart.query ("force", force.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/deployments/{deployment_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDeploymentDeleteDeployment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDeploymentDeleteDeployment.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDeploymentDeleteDeployment" (int status)
        }

    ///<summary>
    ///Fetch information about a deployment.
    ///</summary>
    member this.PagesDeploymentGetDeploymentInfo
        (deploymentId: string, projectName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("deployment_id", deploymentId)
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/deployments/{deployment_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDeploymentGetDeploymentInfo.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDeploymentGetDeploymentInfo.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDeploymentGetDeploymentInfo" (int status)
        }

    ///<summary>
    ///Fetch deployment logs for a project.
    ///</summary>
    member this.PagesDeploymentGetDeploymentLogs
        (deploymentId: string, projectName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("deployment_id", deploymentId)
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/deployments/{deployment_id}/history/logs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDeploymentGetDeploymentLogs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDeploymentGetDeploymentLogs.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDeploymentGetDeploymentLogs" (int status)
        }

    ///<summary>
    ///Retry a previous deployment.
    ///</summary>
    member this.PagesDeploymentRetryDeployment
        (deploymentId: string, projectName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("deployment_id", deploymentId)
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/deployments/{deployment_id}/retry"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDeploymentRetryDeployment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDeploymentRetryDeployment.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDeploymentRetryDeployment" (int status)
        }

    ///<summary>
    ///Rollback the production deployment to a previous deployment. You can only rollback to succesful builds on production.
    ///</summary>
    member this.PagesDeploymentRollbackDeployment
        (deploymentId: string, projectName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("deployment_id", deploymentId)
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/deployments/{deployment_id}/rollback"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDeploymentRollbackDeployment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDeploymentRollbackDeployment.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDeploymentRollbackDeployment" (int status)
        }

    ///<summary>
    ///Start a tail that receives logs and exception data.
    ///</summary>
    member this.PagesDeploymentCreateTail
        (
            deploymentId: string,
            projectName: string,
            accountId: string,
            body: PagesDeploymentCreateTailPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("deployment_id", deploymentId)
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/deployments/{deployment_id}/tails"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDeploymentCreateTail.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDeploymentCreateTail.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDeploymentCreateTail" (int status)
        }

    ///<summary>
    ///Deletes a tail from a Pages deployment.
    ///</summary>
    member this.PagesDeploymentDeleteTail
        (
            tailId: string,
            deploymentId: string,
            projectName: string,
            accountId: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("tail_id", tailId)
                  RequestPart.path ("deployment_id", deploymentId)
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/deployments/{deployment_id}/tails/{tail_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDeploymentDeleteTail.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDeploymentDeleteTail.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDeploymentDeleteTail" (int status)
        }

    ///<summary>
    ///Fetch a list of all domains associated with a Pages project.
    ///</summary>
    member this.PagesDomainsGetDomains(projectName: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/domains"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDomainsGetDomains.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDomainsGetDomains.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDomainsGetDomains" (int status)
        }

    ///<summary>
    ///Add a new domain for the Pages project.
    ///</summary>
    member this.PagesDomainsAddDomain
        (
            projectName: string,
            accountId: string,
            body: PagesDomainsAddDomainPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/domains"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDomainsAddDomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDomainsAddDomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDomainsAddDomain" (int status)
        }

    ///<summary>
    ///Delete a Pages project's domain.
    ///</summary>
    member this.PagesDomainsDeleteDomain
        (domainName: string, projectName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("domain_name", domainName)
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/domains/{domain_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDomainsDeleteDomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDomainsDeleteDomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDomainsDeleteDomain" (int status)
        }

    ///<summary>
    ///Fetch a single domain.
    ///</summary>
    member this.PagesDomainsGetDomain
        (domainName: string, projectName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("domain_name", domainName)
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/domains/{domain_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDomainsGetDomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDomainsGetDomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDomainsGetDomain" (int status)
        }

    ///<summary>
    ///Retry the validation status of a single domain.
    ///</summary>
    member this.PagesDomainsPatchDomain
        (domainName: string, projectName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("domain_name", domainName)
                  RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/domains/{domain_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesDomainsPatchDomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesDomainsPatchDomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesDomainsPatchDomain" (int status)
        }

    ///<summary>
    ///Purge all cached build artifacts for a Pages project
    ///</summary>
    member this.PagesPurgeBuildCache(projectName: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/purge_build_cache"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesPurgeBuildCache.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesPurgeBuildCache.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesPurgeBuildCache" (int status)
        }

    ///<summary>
    ///Disconnect the Git repository source from an existing Pages project.
    ///</summary>
    member this.PagesProjectDisconnectProjectSource
        (projectName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/source"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesProjectDisconnectProjectSource.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesProjectDisconnectProjectSource.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesProjectDisconnectProjectSource" (int status)
        }

    ///<summary>
    ///Connect a Git repository source to an existing Pages project.
    ///</summary>
    ///<param name="projectName"></param>
    ///<param name="accountId"></param>
    ///<param name="body">Configs for the project source control.</param>
    ///<param name="cancellationToken"></param>
    member this.PagesProjectConnectProjectSource
        (projectName: string, accountId: string, body: pages_source, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/source"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesProjectConnectProjectSource.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesProjectConnectProjectSource.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesProjectConnectProjectSource" (int status)
        }

    ///<summary>
    ///Get a short-lived JWT for Pages Direct Upload asset operations.
    ///</summary>
    member this.PagesProjectGetUploadToken
        (projectName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("project_name", projectName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pages/projects/{project_name}/upload-token"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PagesProjectGetUploadToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesProjectGetUploadToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesProjectGetUploadToken" (int status)
        }

    ///<summary>
    ///[DEPRECATED] List, filter, and paginate pipelines in an account. Use the new /pipelines/v1/pipelines endpoint instead.
    ///</summary>
    member this.GetV4AccountsByAccountIdPipelinesDeprecated
        (accountId: string, ?search: string, ?page: string, ?perPage: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/pipelines" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetV4AccountsByAccountIdPipelinesDeprecated.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    GetV4AccountsByAccountIdPipelinesDeprecated.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for GetV4AccountsByAccountIdPipelinesDeprecated" (int status)
        }

    ///<summary>
    ///[DEPRECATED] Create a new pipeline. Use the new /pipelines/v1/pipelines endpoint instead.
    ///</summary>
    member this.PostV4AccountsByAccountIdPipelinesDeprecated
        (
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?body: PostV4AccountsByAccountIdPipelinesDeprecatedPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/pipelines" requestParts cancellationToken

            match (int status) with
            | 200 -> return PostV4AccountsByAccountIdPipelinesDeprecated.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    PostV4AccountsByAccountIdPipelinesDeprecated.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for PostV4AccountsByAccountIdPipelinesDeprecated" (int status)
        }

    ///<summary>
    ///List/Filter Pipelines in Account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="name">Filters pipelines by name (case-insensitive substring).</param>
    ///<param name="cancellationToken"></param>
    member this.GetV4AccountsByAccountIdPipelinesV1Pipelines
        (accountId: string, ?page: float, ?perPage: float, ?name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/pipelines"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetV4AccountsByAccountIdPipelinesV1Pipelines.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetV4AccountsByAccountIdPipelinesV1Pipelines.Status4XX(int status)
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for GetV4AccountsByAccountIdPipelinesV1Pipelines" (int status)
        }

    ///<summary>
    ///Create a new Pipeline.
    ///</summary>
    member this.PostV4AccountsByAccountIdPipelinesV1Pipelines
        (
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?body: PostV4AccountsByAccountIdPipelinesV1PipelinesPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/pipelines"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostV4AccountsByAccountIdPipelinesV1Pipelines.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PostV4AccountsByAccountIdPipelinesV1Pipelines.Status4XX(int status)
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for PostV4AccountsByAccountIdPipelinesV1Pipelines" (int status)
        }

    ///<summary>
    ///Delete Pipeline in Account.
    ///</summary>
    member this.DeleteV4AccountsByAccountIdPipelinesV1PipelinesByPipelineId
        (accountId: string, pipelineId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("pipeline_id", pipelineId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/pipelines/{pipeline_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return DeleteV4AccountsByAccountIdPipelinesV1PipelinesByPipelineId.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteV4AccountsByAccountIdPipelinesV1PipelinesByPipelineId.Status4XX(int status)
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for DeleteV4AccountsByAccountIdPipelinesV1PipelinesByPipelineId"
                        (int status)
        }

    ///<summary>
    ///Get Pipeline details.
    ///</summary>
    member this.GetV4AccountsByAccountIdPipelinesV1PipelinesByPipelineId
        (accountId: string, pipelineId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("pipeline_id", pipelineId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/pipelines/{pipeline_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return GetV4AccountsByAccountIdPipelinesV1PipelinesByPipelineId.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetV4AccountsByAccountIdPipelinesV1PipelinesByPipelineId.Status4XX(int status)
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for GetV4AccountsByAccountIdPipelinesV1PipelinesByPipelineId"
                        (int status)
        }

    ///<summary>
    ///List/Filter Sinks in Account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="pipelineId"></param>
    ///<param name="name">Filters sinks by name (case-insensitive substring).</param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="cancellationToken"></param>
    member this.GetV4AccountsByAccountIdPipelinesV1Sinks
        (
            accountId: string,
            ?pipelineId: string,
            ?name: string,
            ?page: float,
            ?perPage: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if pipelineId.IsSome then
                      RequestPart.query ("pipeline_id", pipelineId.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/sinks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetV4AccountsByAccountIdPipelinesV1Sinks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) -> return GetV4AccountsByAccountIdPipelinesV1Sinks.Status4XX(int status)
            | _ ->
                return failwithf "Unexpected HTTP status %d for GetV4AccountsByAccountIdPipelinesV1Sinks" (int status)
        }

    ///<summary>
    ///Create a new Sink.
    ///</summary>
    member this.PostV4AccountsByAccountIdPipelinesV1Sinks
        (
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?body: PostV4AccountsByAccountIdPipelinesV1SinksPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/sinks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostV4AccountsByAccountIdPipelinesV1Sinks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PostV4AccountsByAccountIdPipelinesV1Sinks.Status4XX(int status)
            | _ ->
                return failwithf "Unexpected HTTP status %d for PostV4AccountsByAccountIdPipelinesV1Sinks" (int status)
        }

    ///<summary>
    ///Delete Sink in Account.
    ///</summary>
    member this.DeleteV4AccountsByAccountIdPipelinesV1SinksBySinkId
        (accountId: string, sinkId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("sink_id", sinkId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/sinks/{sink_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteV4AccountsByAccountIdPipelinesV1SinksBySinkId.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteV4AccountsByAccountIdPipelinesV1SinksBySinkId.Status4XX(int status)
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for DeleteV4AccountsByAccountIdPipelinesV1SinksBySinkId"
                        (int status)
        }

    ///<summary>
    ///Get Sink Details.
    ///</summary>
    member this.GetV4AccountsByAccountIdPipelinesV1SinksBySinkId
        (accountId: string, sinkId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("sink_id", sinkId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/sinks/{sink_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetV4AccountsByAccountIdPipelinesV1SinksBySinkId.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetV4AccountsByAccountIdPipelinesV1SinksBySinkId.Status4XX(int status)
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for GetV4AccountsByAccountIdPipelinesV1SinksBySinkId"
                        (int status)
        }

    ///<summary>
    ///List/Filter Streams in Account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="pipelineId"></param>
    ///<param name="name">Filters streams by name (case-insensitive substring).</param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="cancellationToken"></param>
    member this.GetV4AccountsByAccountIdPipelinesV1Streams
        (
            accountId: string,
            ?pipelineId: string,
            ?name: string,
            ?page: float,
            ?perPage: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if pipelineId.IsSome then
                      RequestPart.query ("pipeline_id", pipelineId.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/streams"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetV4AccountsByAccountIdPipelinesV1Streams.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetV4AccountsByAccountIdPipelinesV1Streams.Status4XX(int status)
            | _ ->
                return failwithf "Unexpected HTTP status %d for GetV4AccountsByAccountIdPipelinesV1Streams" (int status)
        }

    ///<summary>
    ///Create a new Stream.
    ///</summary>
    member this.PostV4AccountsByAccountIdPipelinesV1Streams
        (
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?body: PostV4AccountsByAccountIdPipelinesV1StreamsPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/streams"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostV4AccountsByAccountIdPipelinesV1Streams.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PostV4AccountsByAccountIdPipelinesV1Streams.Status4XX(int status)
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for PostV4AccountsByAccountIdPipelinesV1Streams" (int status)
        }

    ///<summary>
    ///Delete Stream in Account.
    ///</summary>
    member this.DeleteV4AccountsByAccountIdPipelinesV1StreamsByStreamId
        (accountId: string, streamId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("stream_id", streamId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/streams/{stream_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteV4AccountsByAccountIdPipelinesV1StreamsByStreamId.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteV4AccountsByAccountIdPipelinesV1StreamsByStreamId.Status4XX(int status)
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for DeleteV4AccountsByAccountIdPipelinesV1StreamsByStreamId"
                        (int status)
        }

    ///<summary>
    ///Get Stream Details.
    ///</summary>
    member this.GetV4AccountsByAccountIdPipelinesV1StreamsByStreamId
        (accountId: string, streamId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("stream_id", streamId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/streams/{stream_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetV4AccountsByAccountIdPipelinesV1StreamsByStreamId.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetV4AccountsByAccountIdPipelinesV1StreamsByStreamId.Status4XX(int status)
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for GetV4AccountsByAccountIdPipelinesV1StreamsByStreamId"
                        (int status)
        }

    ///<summary>
    ///Update a Stream.
    ///</summary>
    member this.PatchV4AccountsByAccountIdPipelinesV1StreamsByStreamId
        (
            accountId: string,
            streamId: string,
            ?cancellationToken: CancellationToken,
            ?body: PatchV4AccountsByAccountIdPipelinesV1StreamsByStreamIdPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("stream_id", streamId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/streams/{stream_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PatchV4AccountsByAccountIdPipelinesV1StreamsByStreamId.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PatchV4AccountsByAccountIdPipelinesV1StreamsByStreamId.Status4XX(int status)
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for PatchV4AccountsByAccountIdPipelinesV1StreamsByStreamId"
                        (int status)
        }

    ///<summary>
    ///Validates that the Pipelines SQL is correct.
    ///</summary>
    member this.PostV4AccountsByAccountIdPipelinesV1ValidateSql
        (
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?body: PostV4AccountsByAccountIdPipelinesV1ValidateSqlPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/v1/validate_sql"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostV4AccountsByAccountIdPipelinesV1ValidateSql.OK((Serializer.deserialize content))
            | 422 ->
                return
                    PostV4AccountsByAccountIdPipelinesV1ValidateSql.UnprocessableEntity(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return PostV4AccountsByAccountIdPipelinesV1ValidateSql.Status4XX(int status)
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for PostV4AccountsByAccountIdPipelinesV1ValidateSql"
                        (int status)
        }

    ///<summary>
    ///[DEPRECATED] Delete a pipeline. Use the new /pipelines/v1/pipelines endpoint instead.
    ///</summary>
    member this.DeleteV4AccountsByAccountIdPipelinesByPipelineNameDeprecated
        (accountId: string, pipelineName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("pipeline_name", pipelineName) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/{pipeline_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteV4AccountsByAccountIdPipelinesByPipelineNameDeprecated.OK
            | _ when (((int status) / 100) = 4) ->
                return DeleteV4AccountsByAccountIdPipelinesByPipelineNameDeprecated.Status4XX(int status)
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for DeleteV4AccountsByAccountIdPipelinesByPipelineNameDeprecated"
                        (int status)
        }

    ///<summary>
    ///[DEPRECATED] Get configuration of a pipeline. Use the new /pipelines/v1/pipelines endpoint instead.
    ///</summary>
    member this.GetV4AccountsByAccountIdPipelinesByPipelineNameDeprecated
        (accountId: string, pipelineName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("pipeline_name", pipelineName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/{pipeline_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return GetV4AccountsByAccountIdPipelinesByPipelineNameDeprecated.OK((Serializer.deserialize content))
            | 404 ->
                return
                    GetV4AccountsByAccountIdPipelinesByPipelineNameDeprecated.NotFound((Serializer.deserialize content))
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for GetV4AccountsByAccountIdPipelinesByPipelineNameDeprecated"
                        (int status)
        }

    ///<summary>
    ///[DEPRECATED] Update an existing pipeline. Use the new /pipelines/v1/pipelines endpoint instead.
    ///</summary>
    member this.PutV4AccountsByAccountIdPipelinesByPipelineNameDeprecated
        (
            accountId: string,
            pipelineName: string,
            ?cancellationToken: CancellationToken,
            ?body: PutV4AccountsByAccountIdPipelinesByPipelineNameDeprecatedPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("pipeline_name", pipelineName)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/pipelines/{pipeline_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return PutV4AccountsByAccountIdPipelinesByPipelineNameDeprecated.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    PutV4AccountsByAccountIdPipelinesByPipelineNameDeprecated.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for PutV4AccountsByAccountIdPipelinesByPipelineNameDeprecated"
                        (int status)
        }

    ///<summary>
    ///Returns the queues owned by an account.
    ///</summary>
    member this.QueuesList(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/queues" requestParts cancellationToken

            match (int status) with
            | 200 -> return QueuesList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesList" (int status)
        }

    ///<summary>
    ///Create a new queue
    ///</summary>
    member this.QueuesCreate(accountId: string, ?cancellationToken: CancellationToken, ?body: QueuesCreatePayload) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/queues" requestParts cancellationToken

            match (int status) with
            | 200 -> return QueuesCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesCreate" (int status)
        }

    ///<summary>
    ///Deletes a queue
    ///</summary>
    member this.QueuesDelete(queueId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesDelete" (int status)
        }

    ///<summary>
    ///Get details about a specific queue.
    ///</summary>
    member this.QueuesGet(queueId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesGet" (int status)
        }

    ///<summary>
    ///Updates a Queue.
    ///</summary>
    member this.QueuesUpdatePartial
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: mq_queue)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesUpdatePartial.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesUpdatePartial.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesUpdatePartial" (int status)
        }

    ///<summary>
    ///Updates a Queue. Note that this endpoint does not support partial updates. If successful, the Queue's configuration is overwritten with the supplied configuration.
    ///</summary>
    member this.QueuesUpdate
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: mq_queue)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesUpdate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesUpdate" (int status)
        }

    ///<summary>
    ///Returns the consumers for a Queue
    ///</summary>
    member this.QueuesListConsumers(queueId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/consumers"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesListConsumers.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesListConsumers.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesListConsumers" (int status)
        }

    ///<summary>
    ///Creates a new consumer for a Queue
    ///</summary>
    ///<param name="queueId"></param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="body">Request body for creating or updating a consumer</param>
    member this.QueuesCreateConsumer
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: mq_consumer_u002D_request)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/consumers"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesCreateConsumer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesCreateConsumer.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesCreateConsumer" (int status)
        }

    ///<summary>
    ///Deletes the consumer for a queue.
    ///</summary>
    member this.QueuesDeleteConsumer
        (consumerId: string, queueId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("consumer_id", consumerId)
                  RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/consumers/{consumer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesDeleteConsumer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesDeleteConsumer.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesDeleteConsumer" (int status)
        }

    ///<summary>
    ///Fetches the consumer for a queue by consumer id
    ///</summary>
    member this.QueuesGetConsumer
        (consumerId: string, queueId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("consumer_id", consumerId)
                  RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/consumers/{consumer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesGetConsumer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesGetConsumer.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesGetConsumer" (int status)
        }

    ///<summary>
    ///Updates the consumer for a queue, or creates one if it does not exist.
    ///</summary>
    ///<param name="consumerId"></param>
    ///<param name="queueId"></param>
    ///<param name="accountId"></param>
    ///<param name="body">Request body for creating or updating a consumer</param>
    ///<param name="cancellationToken"></param>
    member this.QueuesUpdateConsumer
        (
            consumerId: string,
            queueId: string,
            accountId: string,
            body: mq_consumer_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("consumer_id", consumerId)
                  RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/consumers/{consumer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesUpdateConsumer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesUpdateConsumer.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesUpdateConsumer" (int status)
        }

    ///<summary>
    ///Push a message to a Queue
    ///</summary>
    member this.QueuesPushMessage
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: mq_queue_u002D_message)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/messages"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesPushMessage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesPushMessage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesPushMessage" (int status)
        }

    ///<summary>
    ///Acknowledge + Retry messages from a Queue
    ///</summary>
    member this.QueuesAckMessages
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: QueuesAckMessagesPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/messages/ack"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesAckMessages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesAckMessages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesAckMessages" (int status)
        }

    ///<summary>
    ///Push a batch of message to a Queue
    ///</summary>
    member this.QueuesPushMessages
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: mq_queue_u002D_batch)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/messages/batch"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesPushMessages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesPushMessages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesPushMessages" (int status)
        }

    ///<summary>
    ///Extend the lease on a message. This creates a new lease ID on your message without incrementing the message's `attempts` counter.
    ///</summary>
    member this.QueuesExtendMessages
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: QueuesExtendMessagesPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/messages/extend"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesExtendMessages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesExtendMessages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesExtendMessages" (int status)
        }

    ///<summary>
    ///Peek messages from a Queue without leasing them. Messages remain available for subsequent peek or pull operations.
    ///</summary>
    member this.QueuesPeekMessages
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: QueuesPeekMessagesPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/messages/peek"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesPeekMessages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesPeekMessages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesPeekMessages" (int status)
        }

    ///<summary>
    ///Preview messages from a Queue without leasing them. Messages remain available for subsequent preview or pull operations.
    ///</summary>
    member this.QueuesPreviewMessages
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: QueuesPreviewMessagesPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/messages/preview"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesPreviewMessages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesPreviewMessages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesPreviewMessages" (int status)
        }

    ///<summary>
    ///Delete previewed messages from a Queue. Note that messages acknowledged this way aren't considered delivered, they are instantly deleted from this queue and do not affect metrics.
    ///</summary>
    member this.QueuesAckPreviewMessages
        (
            queueId: string,
            accountId: string,
            body: QueuesAckPreviewMessagesPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/messages/preview/ack"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesAckPreviewMessages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesAckPreviewMessages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesAckPreviewMessages" (int status)
        }

    ///<summary>
    ///Pull a batch of messages from a Queue
    ///</summary>
    member this.QueuesPullMessages
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: QueuesPullMessagesPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/messages/pull"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesPullMessages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesPullMessages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesPullMessages" (int status)
        }

    ///<summary>
    ///Delete peeked messages from a Queue by their ref. Purged messages aren't considered delivered, they are instantly deleted from this queue and do not affect metrics.
    ///</summary>
    member this.QueuesPurgeMessages
        (queueId: string, accountId: string, body: QueuesPurgeMessagesPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/messages/purge"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesPurgeMessages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesPurgeMessages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesPurgeMessages" (int status)
        }

    ///<summary>
    ///Return best-effort metrics for a queue. Values may be approximate due to the distributed nature of queues.
    ///</summary>
    member this.QueuesGetMetrics(queueId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/metrics"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesGetMetrics.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesGetMetrics.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesGetMetrics" (int status)
        }

    ///<summary>
    ///Get details about a Queue's purge status.
    ///</summary>
    member this.QueuesPurgeGet(queueId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/purge"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesPurgeGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesPurgeGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesPurgeGet" (int status)
        }

    ///<summary>
    ///Deletes all messages from the Queue.
    ///</summary>
    member this.QueuesPurge
        (queueId: string, accountId: string, ?cancellationToken: CancellationToken, ?body: QueuesPurgePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("queue_id", queueId)
                  RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/queues/{queue_id}/purge"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueuesPurge.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return QueuesPurge.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueuesPurge" (int status)
        }

    ///<summary>
    ///Deletes all event trigger declarations owned by a Worker script.
    ///</summary>
    member this.WorDeleteScriptTriggers(scriptName: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/triggers/{script_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorDeleteScriptTriggers.OK((Serializer.deserialize content))
            | 400 -> return WorDeleteScriptTriggers.BadRequest((Serializer.deserialize content))
            | 404 -> return WorDeleteScriptTriggers.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorDeleteScriptTriggers" (int status)
        }

    ///<summary>
    ///Returns the event trigger declarations owned by a Worker script.
    ///</summary>
    member this.WorGetScriptTriggers(scriptName: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/triggers/{script_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorGetScriptTriggers.OK((Serializer.deserialize content))
            | 400 -> return WorGetScriptTriggers.BadRequest((Serializer.deserialize content))
            | 404 -> return WorGetScriptTriggers.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorGetScriptTriggers" (int status)
        }

    ///<summary>
    ///Adds event trigger declarations without removing existing declarations owned by the script.
    ///</summary>
    member this.WorAddScriptTriggers
        (scriptName: string, accountId: string, body: WorAddScriptTriggersPayload, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/triggers/{script_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorAddScriptTriggers.OK((Serializer.deserialize content))
            | 400 -> return WorAddScriptTriggers.BadRequest((Serializer.deserialize content))
            | 404 -> return WorAddScriptTriggers.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorAddScriptTriggers" (int status)
        }

    ///<summary>
    ///Replaces all event trigger declarations owned by a Worker script.
    ///</summary>
    member this.WorReplaceScriptTriggers
        (
            scriptName: string,
            accountId: string,
            body: WorReplaceScriptTriggersPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/triggers/{script_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorReplaceScriptTriggers.OK((Serializer.deserialize content))
            | 400 -> return WorReplaceScriptTriggers.BadRequest((Serializer.deserialize content))
            | 404 -> return WorReplaceScriptTriggers.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorReplaceScriptTriggers" (int status)
        }

    ///<summary>
    ///Fetches Worker account settings for an account.
    ///</summary>
    member this.WorkerAccountSettingsFetchWorkerAccountSettings
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/account-settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerAccountSettingsFetchWorkerAccountSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    WorkerAccountSettingsFetchWorkerAccountSettings.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for WorkerAccountSettingsFetchWorkerAccountSettings"
                        (int status)
        }

    ///<summary>
    ///Creates Worker account settings for an account.
    ///</summary>
    member this.WorkerAccountSettingsCreateWorkerAccountSettings
        (accountId: string, body: workers_account_u002D_settings, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/account-settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerAccountSettingsCreateWorkerAccountSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    WorkerAccountSettingsCreateWorkerAccountSettings.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for WorkerAccountSettingsCreateWorkerAccountSettings"
                        (int status)
        }

    ///<summary>
    ///Upload assets ahead of creating a Worker version.  To learn more about the direct uploads of assets, see https://developers.cloudflare.com/workers/static-assets/direct-upload/.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="base64"></param>
    ///<param name="body">Base-64 encoded contents of the file. The content type of the file should be included to ensure a valid "Content-Type" header is included in asset responses.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkerAssetsUpload
        (accountId: string, base64: bool, body: list<MultipartTextField>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields []
                  RequestPart.path ("account_id", accountId)
                  RequestPart.query ("base64", base64)
                  RequestPart.multipartFields ("body", "*/*", body) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/assets/upload"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return WorkerAssetsUpload.Created((Serializer.deserialize content))
            | 202 -> return WorkerAssetsUpload.Accepted((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerAssetsUpload.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerAssetsUpload" (int status)
        }

    ///<summary>
    ///Fetch a list of Workers for Platforms namespaces.
    ///</summary>
    member this.NamespaceWorkerList(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerList" (int status)
        }

    ///<summary>
    ///Create a new Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerCreate
        (accountId: string, body: NamespaceWorkerCreatePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerCreate" (int status)
        }

    ///<summary>
    ///Delete a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerDeleteNamespace
        (accountId: string, dispatchNamespace: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerDeleteNamespace.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerDeleteNamespace.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerDeleteNamespace" (int status)
        }

    ///<summary>
    ///Get a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerGetNamespace
        (accountId: string, dispatchNamespace: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerGetNamespace.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerGetNamespace.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerGetNamespace" (int status)
        }

    ///<summary>
    ///Patch a Workers for Platforms namespace. Omitted fields are left unchanged.
    ///</summary>
    member this.NamespaceWorkerPatchNamespace
        (
            accountId: string,
            dispatchNamespace: string,
            body: NamespaceWorkerPatchNamespacePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerPatchNamespace.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerPatchNamespace.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerPatchNamespace" (int status)
        }

    ///<summary>
    ///Update a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerPutNamespace
        (
            accountId: string,
            dispatchNamespace: string,
            body: NamespaceWorkerPutNamespacePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerPutNamespace.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerPutNamespace.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerPutNamespace" (int status)
        }

    ///<summary>
    ///Delete multiple scripts from a Workers for Platforms namespace based on optional tag filters.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="dispatchNamespace"></param>
    ///<param name="tags">Filter scripts by tags before deletion. Format: comma-separated list of tag:allowed pairs where allowed is 'yes' or 'no'.</param>
    ///<param name="limit">Limit the number of scripts to delete.</param>
    ///<param name="cancellationToken"></param>
    member this.NamespaceWorkerDeleteScripts
        (accountId: string, dispatchNamespace: string, ?tags: string, ?limit: int, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  if tags.IsSome then
                      RequestPart.query ("tags", tags.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerDeleteScripts.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerDeleteScripts.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerDeleteScripts" (int status)
        }

    ///<summary>
    ///Fetch a list of scripts uploaded to a Workers for Platforms namespace.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="dispatchNamespace"></param>
    ///<param name="tags">Filter scripts by tags. Format: comma-separated list of tag:allowed pairs where allowed is 'yes' or 'no'.</param>
    ///<param name="cancellationToken"></param>
    member this.NamespaceWorkerListScripts
        (accountId: string, dispatchNamespace: string, ?tags: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  if tags.IsSome then
                      RequestPart.query ("tags", tags.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerListScripts.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerListScripts.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerListScripts" (int status)
        }

    ///<summary>
    ///Delete a worker from a Workers for Platforms namespace. This call has no response body on a successful delete.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="dispatchNamespace"></param>
    ///<param name="scriptName"></param>
    ///<param name="force">If set to true, delete will not be stopped by associated service binding, durable object, or other binding. Any of these associated bindings/durable objects will be deleted along with the script.</param>
    ///<param name="cancellationToken"></param>
    member this.NamespaceWorkerScriptDeleteWorker
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            ?force: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  if force.IsSome then
                      RequestPart.query ("force", force.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerScriptDeleteWorker.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerScriptDeleteWorker.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerScriptDeleteWorker" (int status)
        }

    ///<summary>
    ///Fetch information about a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerScriptWorkerDetails
        (accountId: string, dispatchNamespace: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerScriptWorkerDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerScriptWorkerDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerScriptWorkerDetails" (int status)
        }

    ///<summary>
    ///Upload a worker module to a Workers for Platforms namespace. You can find more about the multipart metadata on our docs: https://developers.cloudflare.com/workers/configuration/multipart-upload-metadata/.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="dispatchNamespace"></param>
    ///<param name="scriptName"></param>
    ///<param name="metadata">JSON-encoded metadata about the uploaded parts and Worker configuration.</param>
    ///<param name="bindingsInherit">When set to "strict", the upload will fail if any `inherit` type bindings cannot be resolved against the previous version of the script. Without this, unresolvable inherit bindings are silently dropped.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="files">An array of modules (often JavaScript files) comprising a Worker script. At least one module must be present and referenced in the metadata as `main_module` or `body_part` by filename.&amp;lt;br/&amp;gt;Possible Content-Type(s) are: `application/javascript+module`, `text/javascript+module`, `application/javascript`, `text/javascript`, `text/x-python`, `text/x-python-requirement`, `application/wasm`, `text/plain`, `application/octet-stream`, `application/source-map`.</param>
    member this.NamespaceWorkerScriptUploadWorkerModule
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            metadata: System.Text.Json.Nodes.JsonObject,
            ?bindingsInherit: string,
            ?cancellationToken: CancellationToken,
            ?files: list<MultipartFile>
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "metadata"; "files" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.multipartJson ("metadata", "application/json", metadata)
                  if bindingsInherit.IsSome then
                      RequestPart.query ("bindings_inherit", bindingsInherit.Value)
                  if files.IsSome then
                      RequestPart.multipartBinary (
                          "files",
                          "application/javascript+module, text/javascript+module, application/javascript, text/javascript, text/x-python, text/x-python-requirement, application/wasm, text/plain, application/octet-stream, application/source-map",
                          files.Value
                      ) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerScriptUploadWorkerModule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerScriptUploadWorkerModule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerScriptUploadWorkerModule" (int status)
        }

    ///<summary>
    ///Start uploading a collection of assets for use in a Worker version. To learn more about the direct uploads of assets, see https://developers.cloudflare.com/workers/static-assets/direct-upload/.
    ///</summary>
    member this.NamespaceWorkerScriptUpdateCreateAssetsUploadSession
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            body: workers_create_u002D_assets_u002D_upload_u002D_session_u002D_object,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/assets-upload-session"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerScriptUpdateCreateAssetsUploadSession.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    NamespaceWorkerScriptUpdateCreateAssetsUploadSession.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for NamespaceWorkerScriptUpdateCreateAssetsUploadSession"
                        (int status)
        }

    ///<summary>
    ///Fetch script bindings from a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerGetScriptBindings
        (accountId: string, dispatchNamespace: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/bindings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerGetScriptBindings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerGetScriptBindings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerGetScriptBindings" (int status)
        }

    ///<summary>
    ///Fetch script content from a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerGetScriptContent
        (accountId: string, dispatchNamespace: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/content"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerGetScriptContent.OK
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerGetScriptContent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerGetScriptContent" (int status)
        }

    ///<summary>
    ///Put script content for a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="dispatchNamespace"></param>
    ///<param name="scriptName"></param>
    ///<param name="metadata">JSON-encoded metadata about the uploaded parts and Worker configuration.</param>
    ///<param name="cFWORKERBODYPART">The multipart name of a script upload part containing script content in service worker format. Alternative to including in a metadata part.</param>
    ///<param name="cFWORKERMAINMODULEPART">The multipart name of a script upload part containing script content in es module format. Alternative to including in a metadata part.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="files">An array of modules (often JavaScript files) comprising a Worker script. At least one module must be present and referenced in the metadata as `main_module` or `body_part` by filename.&amp;lt;br/&amp;gt;Possible Content-Type(s) are: `application/javascript+module`, `text/javascript+module`, `application/javascript`, `text/javascript`, `text/x-python`, `text/x-python-requirement`, `application/wasm`, `text/plain`, `application/octet-stream`, `application/source-map`.</param>
    member this.NamespaceWorkerPutScriptContent
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            metadata: System.Text.Json.Nodes.JsonObject,
            ?cFWORKERBODYPART: string,
            ?cFWORKERMAINMODULEPART: string,
            ?cancellationToken: CancellationToken,
            ?files: list<MultipartFile>
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "metadata"; "files" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.multipartJson ("metadata", "application/json", metadata)
                  if cFWORKERBODYPART.IsSome then
                      RequestPart.header ("CF-WORKER-BODY-PART", cFWORKERBODYPART.Value)
                  if cFWORKERMAINMODULEPART.IsSome then
                      RequestPart.header ("CF-WORKER-MAIN-MODULE-PART", cFWORKERMAINMODULEPART.Value)
                  if files.IsSome then
                      RequestPart.multipartBinary (
                          "files",
                          "application/javascript+module, text/javascript+module, application/javascript, text/javascript, text/x-python, text/x-python-requirement, application/wasm, text/plain, application/octet-stream, application/source-map",
                          files.Value
                      ) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/content"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerPutScriptContent.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerPutScriptContent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerPutScriptContent" (int status)
        }

    ///<summary>
    ///List secrets bound to a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerListScriptSecrets
        (accountId: string, dispatchNamespace: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/secrets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerListScriptSecrets.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerListScriptSecrets.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerListScriptSecrets" (int status)
        }

    ///<summary>
    ///Add a secret to a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="dispatchNamespace"></param>
    ///<param name="scriptName"></param>
    ///<param name="body">A secret value accessible through a binding.</param>
    ///<param name="cancellationToken"></param>
    member this.NamespaceWorkerPutScriptSecrets
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            body: workers_secret,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/secrets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerPutScriptSecrets.OK((Serializer.deserialize content))
            | 429 -> return NamespaceWorkerPutScriptSecrets.TooManyRequests((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerPutScriptSecrets.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerPutScriptSecrets" (int status)
        }

    ///<summary>
    ///Create, update, or delete multiple secrets on a script in a single operation using JSON Merge Patch (RFC 7396).
    ///Usage:
    ///- To create or update a secret, set its value to a secret object.
    ///- To delete a secret, set its value to `null`.
    ///- Secrets not included in the request are left unchanged.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="dispatchNamespace"></param>
    ///<param name="scriptName"></param>
    ///<param name="body">JSON Merge Patch (RFC 7396) request body for bulk secret changes.</param>
    ///<param name="cancellationToken"></param>
    member this.NamespaceWorkerPatchScriptSecretsBulk
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            body: workers_secret_u002D_patch_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/secrets-bulk"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerPatchScriptSecretsBulk.OK((Serializer.deserialize content))
            | 429 -> return NamespaceWorkerPatchScriptSecretsBulk.TooManyRequests((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerPatchScriptSecretsBulk.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerPatchScriptSecretsBulk" (int status)
        }

    ///<summary>
    ///Remove a secret from a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerDeleteScriptSecret
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            secretName: string,
            ?urlEncoded: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("secret_name", secretName)
                  if urlEncoded.IsSome then
                      RequestPart.query ("url_encoded", urlEncoded.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/secrets/{secret_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerDeleteScriptSecret.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerDeleteScriptSecret.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerDeleteScriptSecret" (int status)
        }

    ///<summary>
    ///Get a given secret binding (value omitted) on a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerGetScriptSecrets
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            secretName: string,
            ?urlEncoded: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("secret_name", secretName)
                  if urlEncoded.IsSome then
                      RequestPart.query ("url_encoded", urlEncoded.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/secrets/{secret_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerGetScriptSecrets.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerGetScriptSecrets.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerGetScriptSecrets" (int status)
        }

    ///<summary>
    ///Get script settings from a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerGetScriptSettings
        (accountId: string, dispatchNamespace: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerGetScriptSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerGetScriptSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerGetScriptSettings" (int status)
        }

    ///<summary>
    ///Patch script metadata, such as bindings.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="dispatchNamespace"></param>
    ///<param name="scriptName"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="settings">Script and version settings for Workers for Platforms namespace scripts. Same as script-and-version-settings-item but without annotations, which are not supported for namespace scripts.</param>
    member this.NamespaceWorkerPatchScriptSettings
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            ?cancellationToken: CancellationToken,
            ?settings: workers_namespace_u002D_script_u002D_and_u002D_version_u002D_settings_u002D_item
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "settings" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  if settings.IsSome then
                      RequestPart.multipartJson ("settings", "application/json", settings.Value) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerPatchScriptSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerPatchScriptSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerPatchScriptSettings" (int status)
        }

    ///<summary>
    ///Fetch tags from a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerGetScriptTags
        (accountId: string, dispatchNamespace: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/tags"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerGetScriptTags.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerGetScriptTags.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerGetScriptTags" (int status)
        }

    ///<summary>
    ///Put script tags for a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="dispatchNamespace"></param>
    ///<param name="scriptName"></param>
    ///<param name="body">Tags associated with the Worker.</param>
    ///<param name="cancellationToken"></param>
    member this.NamespaceWorkerPutScriptTags
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            body: workers_tags,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/tags"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerPutScriptTags.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerPutScriptTags.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerPutScriptTags" (int status)
        }

    ///<summary>
    ///Delete script tag for a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerDeleteScriptTag
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            tag: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("tag", tag) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/tags/{tag}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerDeleteScriptTag.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerDeleteScriptTag.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerDeleteScriptTag" (int status)
        }

    ///<summary>
    ///Put a single tag on a script uploaded to a Workers for Platforms namespace.
    ///</summary>
    member this.NamespaceWorkerPutScriptTag
        (
            accountId: string,
            dispatchNamespace: string,
            scriptName: string,
            tag: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("dispatch_namespace", dispatchNamespace)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("tag", tag) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/dispatch/namespaces/{dispatch_namespace}/scripts/{script_name}/tags/{tag}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return NamespaceWorkerPutScriptTag.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return NamespaceWorkerPutScriptTag.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for NamespaceWorkerPutScriptTag" (int status)
        }

    ///<summary>
    ///Lists all domains for an account.
    ///</summary>
    member this.WorkersDomainsList
        (
            accountId: string,
            ?zoneId: string,
            ?zoneName: string,
            ?service: string,
            ?hostname: string,
            ?environment: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if zoneId.IsSome then
                      RequestPart.query ("zone_id", zoneId.Value)
                  if zoneName.IsSome then
                      RequestPart.query ("zone_name", zoneName.Value)
                  if service.IsSome then
                      RequestPart.query ("service", service.Value)
                  if hostname.IsSome then
                      RequestPart.query ("hostname", hostname.Value)
                  if environment.IsSome then
                      RequestPart.query ("environment", environment.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/workers/domains" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkersDomainsList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersDomainsList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersDomainsList" (int status)
        }

    ///<summary>
    ///Attaches a domain that routes traffic to a Worker.
    ///</summary>
    member this.WorkersDomainsUpdate
        (accountId: string, body: WorkersDomainsUpdatePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/workers/domains" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkersDomainsUpdate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersDomainsUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersDomainsUpdate" (int status)
        }

    ///<summary>
    ///Detaches a domain from a Worker. Both the Worker and all of its previews are no longer routable using this domain.
    ///</summary>
    member this.WorkersDomainsDelete(accountId: string, domainId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain_id", domainId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/domains/{domain_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersDomainsDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersDomainsDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersDomainsDelete" (int status)
        }

    ///<summary>
    ///Gets information about a domain.
    ///</summary>
    member this.WorkersDomainsGet(accountId: string, domainId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain_id", domainId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/domains/{domain_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersDomainsGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersDomainsGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersDomainsGet" (int status)
        }

    ///<summary>
    ///Returns the Durable Object namespaces owned by an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="page">Current page.</param>
    ///<param name="perPage">Items per-page.</param>
    ///<param name="cancellationToken"></param>
    member this.DurableObjectsNamespaceListNamespaces
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
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/durable_objects/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DurableObjectsNamespaceListNamespaces.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DurableObjectsNamespaceListNamespaces.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DurableObjectsNamespaceListNamespaces" (int status)
        }

    ///<summary>
    ///Returns the Durable Objects in a given namespace.
    ///</summary>
    member this.DurableObjectsNamespaceListObjects
        (accountId: string, id: string, ?limit: float, ?cursor: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/durable_objects/namespaces/{id}/objects"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DurableObjectsNamespaceListObjects.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DurableObjectsNamespaceListObjects.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DurableObjectsNamespaceListObjects" (int status)
        }

    ///<summary>
    ///List your Workers Observability Telemetry Destinations.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="order"></param>
    ///<param name="orderBy"></param>
    ///<param name="cancellationToken"></param>
    member this.DestinationList
        (
            accountId: string,
            ?page: float,
            ?perPage: float,
            ?order: string,
            ?orderBy: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("perPage", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("orderBy", orderBy.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/destinations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DestinationList.OK((Serializer.deserialize content))
            | 401 -> return DestinationList.Unauthorized((Serializer.deserialize content))
            | 404 -> return DestinationList.NotFound((Serializer.deserialize content))
            | 500 -> return DestinationList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DestinationList" (int status)
        }

    ///<summary>
    ///Create a new Workers Observability Telemetry Destination.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.DestinationCreate
        (accountId: string, ?cancellationToken: CancellationToken, ?body: DestinationCreatePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/destinations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return DestinationCreate.Created((Serializer.deserialize content))
            | 400 -> return DestinationCreate.BadRequest((Serializer.deserialize content))
            | 401 -> return DestinationCreate.Unauthorized((Serializer.deserialize content))
            | 403 -> return DestinationCreate.Forbidden((Serializer.deserialize content))
            | 500 -> return DestinationCreate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DestinationCreate" (int status)
        }

    ///<summary>
    ///Delete a Workers Observability Telemetry Destination.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="slug"></param>
    ///<param name="cancellationToken"></param>
    member this.DestinationsDelete(accountId: string, slug: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("slug", slug) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/destinations/{slug}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DestinationsDelete.OK((Serializer.deserialize content))
            | 401 -> return DestinationsDelete.Unauthorized((Serializer.deserialize content))
            | 404 -> return DestinationsDelete.NotFound((Serializer.deserialize content))
            | 500 -> return DestinationsDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DestinationsDelete" (int status)
        }

    ///<summary>
    ///Update an existing Workers Observability Telemetry Destination.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="slug"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.DestinationUpdate
        (accountId: string, slug: string, ?cancellationToken: CancellationToken, ?body: DestinationUpdatePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("slug", slug)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/destinations/{slug}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DestinationUpdate.OK((Serializer.deserialize content))
            | 400 -> return DestinationUpdate.BadRequest((Serializer.deserialize content))
            | 401 -> return DestinationUpdate.Unauthorized((Serializer.deserialize content))
            | 404 -> return DestinationUpdate.NotFound((Serializer.deserialize content))
            | 500 -> return DestinationUpdate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DestinationUpdate" (int status)
        }

    ///<summary>
    ///Delete one resource configured for Workers Observability metrics export.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.MetricsExportDelete
        (accountId: string, ?cancellationToken: CancellationToken, ?body: MetricsExportDeletePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/metricsexport"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MetricsExportDelete.OK((Serializer.deserialize content))
            | 400 -> return MetricsExportDelete.BadRequest((Serializer.deserialize content))
            | 401 -> return MetricsExportDelete.Unauthorized((Serializer.deserialize content))
            | 404 -> return MetricsExportDelete.NotFound((Serializer.deserialize content))
            | 500 -> return MetricsExportDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MetricsExportDelete" (int status)
        }

    ///<summary>
    ///List resources configured for Workers Observability metrics export.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.MetricsExportList(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/metricsexport"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MetricsExportList.OK((Serializer.deserialize content))
            | 401 -> return MetricsExportList.Unauthorized((Serializer.deserialize content))
            | 500 -> return MetricsExportList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MetricsExportList" (int status)
        }

    ///<summary>
    ///Create or replace resources configured for Workers Observability metrics export.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.MetricsExportUpsert
        (accountId: string, ?cancellationToken: CancellationToken, ?body: InlineUnion_57313af0e98224f0948ea31d)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/metricsexport"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return MetricsExportUpsert.Created((Serializer.deserialize content))
            | 400 -> return MetricsExportUpsert.BadRequest((Serializer.deserialize content))
            | 401 -> return MetricsExportUpsert.Unauthorized((Serializer.deserialize content))
            | 409 -> return MetricsExportUpsert.Conflict((Serializer.deserialize content))
            | 500 -> return MetricsExportUpsert.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MetricsExportUpsert" (int status)
        }

    ///<summary>
    ///List saved queries.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="order"></param>
    ///<param name="orderBy"></param>
    ///<param name="cancellationToken"></param>
    member this.QueriesList
        (
            accountId: string,
            ?page: float,
            ?perPage: float,
            ?order: string,
            ?orderBy: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("perPage", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("orderBy", orderBy.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/queries"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueriesList.OK((Serializer.deserialize content))
            | 401 -> return QueriesList.Unauthorized((Serializer.deserialize content))
            | 404 -> return QueriesList.NotFound((Serializer.deserialize content))
            | 500 -> return QueriesList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueriesList" (int status)
        }

    ///<summary>
    ///Persist query for later use.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.QueriesPost(accountId: string, body: QueriesPostPayload, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/queries"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueriesPost.OK((Serializer.deserialize content))
            | 401 -> return QueriesPost.Unauthorized((Serializer.deserialize content))
            | 409 -> return QueriesPost.Conflict((Serializer.deserialize content))
            | 500 -> return QueriesPost.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueriesPost" (int status)
        }

    ///<summary>
    ///Delete a saved query.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="queryId"></param>
    ///<param name="cancellationToken"></param>
    member this.QueriesDelete(accountId: string, queryId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("queryId", queryId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/queries/{queryId}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueriesDelete.OK((Serializer.deserialize content))
            | 401 -> return QueriesDelete.Unauthorized((Serializer.deserialize content))
            | 404 -> return QueriesDelete.NotFound((Serializer.deserialize content))
            | 500 -> return QueriesDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueriesDelete" (int status)
        }

    ///<summary>
    ///Retrieve a saved query.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="queryId"></param>
    ///<param name="cancellationToken"></param>
    member this.QueriesGet(accountId: string, queryId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("queryId", queryId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/queries/{queryId}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueriesGet.OK((Serializer.deserialize content))
            | 401 -> return QueriesGet.Unauthorized((Serializer.deserialize content))
            | 404 -> return QueriesGet.NotFound((Serializer.deserialize content))
            | 500 -> return QueriesGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueriesGet" (int status)
        }

    ///<summary>
    ///Update saved query.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="queryId"></param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.QueriesPatch
        (accountId: string, queryId: string, body: QueriesPatchPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("queryId", queryId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/queries/{queryId}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return QueriesPatch.OK((Serializer.deserialize content))
            | 401 -> return QueriesPatch.Unauthorized((Serializer.deserialize content))
            | 404 -> return QueriesPatch.NotFound((Serializer.deserialize content))
            | 500 -> return QueriesPatch.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for QueriesPatch" (int status)
        }

    ///<summary>
    ///Shared queries store the results of a previously run query, allowing you to share the results with others.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.SharedQueryPost
        (accountId: string, body: SharedQueryPostPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/shared/query"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SharedQueryPost.OK((Serializer.deserialize content))
            | 400 -> return SharedQueryPost.BadRequest((Serializer.deserialize content))
            | 401 -> return SharedQueryPost.Unauthorized((Serializer.deserialize content))
            | 429 -> return SharedQueryPost.TooManyRequests((Serializer.deserialize content))
            | 500 -> return SharedQueryPost.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SharedQueryPost" (int status)
        }

    ///<summary>
    ///Shared queries store the results of a previously run query, allowing you to share the results with others.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="id">Specify the ID of the shared query.</param>
    ///<param name="view">Select the view of the query result to return, defaults to events.</param>
    ///<param name="cancellationToken"></param>
    member this.SharedQueryGet(accountId: string, id: string, ?view: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  if view.IsSome then
                      RequestPart.query ("view", view.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/shared/query/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SharedQueryGet.OK((Serializer.deserialize content))
            | 400 -> return SharedQueryGet.BadRequest((Serializer.deserialize content))
            | 401 -> return SharedQueryGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return SharedQueryGet.Forbidden((Serializer.deserialize content))
            | 500 -> return SharedQueryGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SharedQueryGet" (int status)
        }

    ///<summary>
    ///List all the keys in your telemetry events.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.TelemetryKeysList
        (accountId: string, body: TelemetryKeysListPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/telemetry/keys"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TelemetryKeysList.OK((Serializer.deserialize content))
            | 400 -> return TelemetryKeysList.BadRequest((Serializer.deserialize content))
            | 401 -> return TelemetryKeysList.Unauthorized((Serializer.deserialize content))
            | 403 -> return TelemetryKeysList.Forbidden((Serializer.deserialize content))
            | 429 -> return TelemetryKeysList.TooManyRequests((Serializer.deserialize content))
            | 500 -> return TelemetryKeysList.InternalServerError((Serializer.deserialize content))
            | 504 -> return TelemetryKeysList.GatewayTimeout((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TelemetryKeysList" (int status)
        }

    ///<summary>
    ///Prepare websocket server for live tail.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.TelemetryLiveTailPost
        (accountId: string, body: TelemetryLiveTailPostPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/telemetry/live-tail"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TelemetryLiveTailPost.OK((Serializer.deserialize content))
            | 401 -> return TelemetryLiveTailPost.Unauthorized((Serializer.deserialize content))
            | 403 -> return TelemetryLiveTailPost.Forbidden((Serializer.deserialize content))
            | 500 -> return TelemetryLiveTailPost.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TelemetryLiveTailPost" (int status)
        }

    ///<summary>
    ///Notify live tail that user is still eligible to receive live events.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.TelemetryLiveTailHeartbeatGet
        (accountId: string, body: TelemetryLiveTailHeartbeatGetPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/telemetry/live-tail/heartbeat"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TelemetryLiveTailHeartbeatGet.OK((Serializer.deserialize content))
            | 401 -> return TelemetryLiveTailHeartbeatGet.Unauthorized((Serializer.deserialize content))
            | 500 -> return TelemetryLiveTailHeartbeatGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TelemetryLiveTailHeartbeatGet" (int status)
        }

    ///<summary>
    ///Run a temporary or saved query.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.TelemetryQuery(accountId: string, body: TelemetryQueryPayload, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/telemetry/query"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TelemetryQuery.OK((Serializer.deserialize content))
            | 400 -> return TelemetryQuery.BadRequest((Serializer.deserialize content))
            | 401 -> return TelemetryQuery.Unauthorized((Serializer.deserialize content))
            | 403 -> return TelemetryQuery.Forbidden((Serializer.deserialize content))
            | 429 -> return TelemetryQuery.TooManyRequests((Serializer.deserialize content))
            | 500 -> return TelemetryQuery.InternalServerError((Serializer.deserialize content))
            | 504 -> return TelemetryQuery.GatewayTimeout((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TelemetryQuery" (int status)
        }

    ///<summary>
    ///List unique values found in your events.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.TelemetryValuesList
        (accountId: string, body: TelemetryValuesListPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/telemetry/values"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TelemetryValuesList.OK((Serializer.deserialize content))
            | 400 -> return TelemetryValuesList.BadRequest((Serializer.deserialize content))
            | 401 -> return TelemetryValuesList.Unauthorized((Serializer.deserialize content))
            | 403 -> return TelemetryValuesList.Forbidden((Serializer.deserialize content))
            | 429 -> return TelemetryValuesList.TooManyRequests((Serializer.deserialize content))
            | 500 -> return TelemetryValuesList.InternalServerError((Serializer.deserialize content))
            | 504 -> return TelemetryValuesList.GatewayTimeout((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TelemetryValuesList" (int status)
        }

    ///<summary>
    ///Event counts broken down by dataset and service, bucketed by day, for up to 90 days. The top-level events field is the sum of all breakdown counts.
    ///</summary>
    ///<param name="accountId">Your Cloudflare account ID.</param>
    ///<param name="from">Unix timestamp in milliseconds for the start of the range.</param>
    ///<param name="to">Unix timestamp in milliseconds for the end of the range.</param>
    ///<param name="cancellationToken"></param>
    member this.UsageGet(accountId: string, from: string, ``to``: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.query ("from", from)
                  RequestPart.query ("to", ``to``) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/observability/usage"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UsageGet.OK((Serializer.deserialize content))
            | 400 -> return UsageGet.BadRequest((Serializer.deserialize content))
            | 401 -> return UsageGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return UsageGet.Forbidden((Serializer.deserialize content))
            | 429 -> return UsageGet.TooManyRequests((Serializer.deserialize content))
            | 500 -> return UsageGet.InternalServerError((Serializer.deserialize content))
            | 504 -> return UsageGet.GatewayTimeout((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UsageGet" (int status)
        }

    ///<summary>
    ///Returns a list of available placement regions organized by cloud provider. These regions can be used to configure Smart Placement for Workers.
    ///</summary>
    member this.WorkerPlacementListRegions(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/placement/regions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerPlacementListRegions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerPlacementListRegions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerPlacementListRegions" (int status)
        }

    ///<summary>
    ///Fetch a list of uploaded workers.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="tags">Filter scripts by tags. Format: comma-separated list of tag:allowed pairs where allowed is 'yes' or 'no'.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkerScriptListWorkers(accountId: string, ?tags: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if tags.IsSome then
                      RequestPart.query ("tags", tags.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/workers/scripts" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptListWorkers.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptListWorkers.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptListWorkers" (int status)
        }

    ///<summary>
    ///Search for Workers in an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="name"></param>
    ///<param name="id"></param>
    ///<param name="orderBy"></param>
    ///<param name="page">Current page.</param>
    ///<param name="perPage">Items per page.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkerScriptSearchWorkers
        (
            accountId: string,
            ?name: string,
            ?id: string,
            ?orderBy: string,
            ?page: int,
            ?perPage: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if id.IsSome then
                      RequestPart.query ("id", id.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts-search"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptSearchWorkers.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptSearchWorkers.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptSearchWorkers" (int status)
        }

    ///<summary>
    ///Delete your worker. This call has no response body on a successful delete.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName"></param>
    ///<param name="force">If set to true, delete will not be stopped by associated service binding, durable object, or other binding. Any of these associated bindings/durable objects will be deleted along with the script.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkerScriptDeleteWorker
        (accountId: string, scriptName: string, ?force: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  if force.IsSome then
                      RequestPart.query ("force", force.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptDeleteWorker.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptDeleteWorker.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptDeleteWorker" (int status)
        }

    ///<summary>
    ///Fetch raw script content for your worker. Note this is the original script content, not JSON encoded.
    ///</summary>
    member this.WorkerScriptDownloadWorker
        (accountId: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptDownloadWorker.OK
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptDownloadWorker.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptDownloadWorker" (int status)
        }

    ///<summary>
    ///Upload a worker module. You can find more about the multipart metadata on our docs: https://developers.cloudflare.com/workers/configuration/multipart-upload-metadata/.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName"></param>
    ///<param name="metadata">JSON-encoded metadata about the uploaded parts and Worker configuration.</param>
    ///<param name="bindingsInherit">When set to "strict", the upload will fail if any `inherit` type bindings cannot be resolved against the previous version of the Worker. Without this, unresolvable inherit bindings are silently dropped.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="files">An array of modules (often JavaScript files) comprising a Worker script. At least one module must be present and referenced in the metadata as `main_module` or `body_part` by filename.&amp;lt;br/&amp;gt;Possible Content-Type(s) are: `application/javascript+module`, `text/javascript+module`, `application/javascript`, `text/javascript`, `text/x-python`, `text/x-python-requirement`, `application/wasm`, `text/plain`, `application/octet-stream`, `application/source-map`.</param>
    member this.WorkerScriptUploadWorkerModule
        (
            accountId: string,
            scriptName: string,
            metadata: System.Text.Json.Nodes.JsonObject,
            ?bindingsInherit: string,
            ?cancellationToken: CancellationToken,
            ?files: list<MultipartFile>
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "metadata"; "files" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.multipartJson ("metadata", "application/json", metadata)
                  if bindingsInherit.IsSome then
                      RequestPart.query ("bindings_inherit", bindingsInherit.Value)
                  if files.IsSome then
                      RequestPart.multipartBinary (
                          "files",
                          "application/javascript+module, text/javascript+module, application/javascript, text/javascript, text/x-python, text/x-python-requirement, application/wasm, text/plain, application/octet-stream, application/source-map",
                          files.Value
                      ) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptUploadWorkerModule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptUploadWorkerModule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptUploadWorkerModule" (int status)
        }

    ///<summary>
    ///Start uploading a collection of assets for use in a Worker version. To learn more about the direct uploads of assets, see https://developers.cloudflare.com/workers/static-assets/direct-upload/.
    ///</summary>
    member this.WorkerScriptUpdateCreateAssetsUploadSession
        (
            accountId: string,
            scriptName: string,
            body: workers_create_u002D_assets_u002D_upload_u002D_session_u002D_object,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/assets-upload-session"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptUpdateCreateAssetsUploadSession.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    WorkerScriptUpdateCreateAssetsUploadSession.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for WorkerScriptUpdateCreateAssetsUploadSession" (int status)
        }

    ///<summary>
    ///Put script content without touching config or metadata.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName"></param>
    ///<param name="metadata">JSON-encoded metadata about the uploaded parts and Worker configuration.</param>
    ///<param name="cFWORKERBODYPART">The multipart name of a script upload part containing script content in service worker format. Alternative to including in a metadata part.</param>
    ///<param name="cFWORKERMAINMODULEPART">The multipart name of a script upload part containing script content in es module format. Alternative to including in a metadata part.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="files">An array of modules (often JavaScript files) comprising a Worker script. At least one module must be present and referenced in the metadata as `main_module` or `body_part` by filename.&amp;lt;br/&amp;gt;Possible Content-Type(s) are: `application/javascript+module`, `text/javascript+module`, `application/javascript`, `text/javascript`, `text/x-python`, `text/x-python-requirement`, `application/wasm`, `text/plain`, `application/octet-stream`, `application/source-map`.</param>
    member this.WorkerScriptPutContent
        (
            accountId: string,
            scriptName: string,
            metadata: System.Text.Json.Nodes.JsonObject,
            ?cFWORKERBODYPART: string,
            ?cFWORKERMAINMODULEPART: string,
            ?cancellationToken: CancellationToken,
            ?files: list<MultipartFile>
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "metadata"; "files" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.multipartJson ("metadata", "application/json", metadata)
                  if cFWORKERBODYPART.IsSome then
                      RequestPart.header ("CF-WORKER-BODY-PART", cFWORKERBODYPART.Value)
                  if cFWORKERMAINMODULEPART.IsSome then
                      RequestPart.header ("CF-WORKER-MAIN-MODULE-PART", cFWORKERMAINMODULEPART.Value)
                  if files.IsSome then
                      RequestPart.multipartBinary (
                          "files",
                          "application/javascript+module, text/javascript+module, application/javascript, text/javascript, text/x-python, text/x-python-requirement, application/wasm, text/plain, application/octet-stream, application/source-map",
                          files.Value
                      ) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/content"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptPutContent.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptPutContent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptPutContent" (int status)
        }

    ///<summary>
    ///Fetch script content only.
    ///</summary>
    member this.WorkerScriptGetContent(accountId: string, scriptName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/content/v2"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptGetContent.OK
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptGetContent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptGetContent" (int status)
        }

    ///<summary>
    ///List of Worker Deployments. The first deployment in the list is the latest deployment actively serving traffic.
    ///</summary>
    member this.WorkerDeploymentsListDeployments
        (accountId: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/deployments"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerDeploymentsListDeployments.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerDeploymentsListDeployments.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerDeploymentsListDeployments" (int status)
        }

    ///<summary>
    ///Deployments configure how [Worker Versions](https://developers.cloudflare.com/api/operations/worker-versions-list-versions) are deployed to traffic. A deployment can consist of one or two versions of a Worker.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName"></param>
    ///<param name="body"></param>
    ///<param name="force">If set to true, the deployment will be created even if normally blocked by something such rolling back to an older version when a secret has changed.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkerDeploymentsCreateDeployment
        (
            accountId: string,
            scriptName: string,
            body: workers_deployment,
            ?force: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body
                  if force.IsSome then
                      RequestPart.query ("force", force.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/deployments"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerDeploymentsCreateDeployment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerDeploymentsCreateDeployment.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerDeploymentsCreateDeployment" (int status)
        }

    ///<summary>
    ///Delete a Worker Deployment. The latest deployment, which is actively serving traffic, cannot be deleted. All other deployments can be deleted.
    ///</summary>
    member this.WorkerDeploymentsDeleteDeployment
        (accountId: string, scriptName: string, deploymentId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("deployment_id", deploymentId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/deployments/{deployment_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerDeploymentsDeleteDeployment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerDeploymentsDeleteDeployment.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerDeploymentsDeleteDeployment" (int status)
        }

    ///<summary>
    ///Get information about a Worker Deployment.
    ///</summary>
    member this.WorkerDeploymentsGetDeployment
        (accountId: string, scriptName: string, deploymentId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("deployment_id", deploymentId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/deployments/{deployment_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerDeploymentsGetDeployment.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerDeploymentsGetDeployment.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerDeploymentsGetDeployment" (int status)
        }

    ///<summary>
    ///Fetches Cron Triggers for a Worker.
    ///</summary>
    member this.WorkerCronTriggerGetCronTriggers
        (accountId: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/schedules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerCronTriggerGetCronTriggers.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerCronTriggerGetCronTriggers.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerCronTriggerGetCronTriggers" (int status)
        }

    ///<summary>
    ///Updates Cron Triggers for a Worker.
    ///</summary>
    member this.WorkerCronTriggerUpdateCronTriggers
        (accountId: string, scriptName: string, body: list<workers_schedule>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/schedules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerCronTriggerUpdateCronTriggers.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerCronTriggerUpdateCronTriggers.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerCronTriggerUpdateCronTriggers" (int status)
        }

    ///<summary>
    ///Get script-level settings when using [Worker Versions](https://developers.cloudflare.com/api/operations/worker-versions-list-versions). Includes Logpush and Tail Consumers.
    ///</summary>
    member this.WorkerScriptSettingsGetSettings
        (accountId: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/script-settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptSettingsGetSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptSettingsGetSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptSettingsGetSettings" (int status)
        }

    ///<summary>
    ///Patch script-level settings when using [Worker Versions](https://developers.cloudflare.com/api/operations/worker-versions-list-versions). Including but not limited to Logpush and Tail Consumers.
    ///</summary>
    member this.WorkerScriptSettingsPatchSettings
        (
            accountId: string,
            scriptName: string,
            body: workers_script_u002D_settings_u002D_item,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/script-settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptSettingsPatchSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptSettingsPatchSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptSettingsPatchSettings" (int status)
        }

    ///<summary>
    ///List secrets bound to a script.
    ///</summary>
    member this.WorkerListScriptSecrets(accountId: string, scriptName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/secrets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerListScriptSecrets.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerListScriptSecrets.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerListScriptSecrets" (int status)
        }

    ///<summary>
    ///Add a secret to a script.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName"></param>
    ///<param name="body">A secret value accessible through a binding.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkerPutScriptSecret
        (accountId: string, scriptName: string, body: workers_secret, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/secrets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerPutScriptSecret.OK((Serializer.deserialize content))
            | 429 -> return WorkerPutScriptSecret.TooManyRequests((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerPutScriptSecret.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerPutScriptSecret" (int status)
        }

    ///<summary>
    ///Create, update, or delete multiple secrets on a script in a single operation using JSON Merge Patch (RFC 7396).
    ///Usage:
    ///- To create or update a secret, set its value to a secret object.
    ///- To delete a secret, set its value to `null`.
    ///- Secrets not included in the request are left unchanged.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName"></param>
    ///<param name="body">JSON Merge Patch (RFC 7396) request body for bulk secret changes.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkerPatchScriptSecretsBulk
        (
            accountId: string,
            scriptName: string,
            body: workers_secret_u002D_patch_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/secrets-bulk"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerPatchScriptSecretsBulk.OK((Serializer.deserialize content))
            | 429 -> return WorkerPatchScriptSecretsBulk.TooManyRequests((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerPatchScriptSecretsBulk.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerPatchScriptSecretsBulk" (int status)
        }

    ///<summary>
    ///Remove a secret from a script.
    ///</summary>
    member this.WorkerDeleteScriptSecret
        (
            accountId: string,
            scriptName: string,
            secretName: string,
            ?urlEncoded: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("secret_name", secretName)
                  if urlEncoded.IsSome then
                      RequestPart.query ("url_encoded", urlEncoded.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/secrets/{secret_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerDeleteScriptSecret.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerDeleteScriptSecret.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerDeleteScriptSecret" (int status)
        }

    ///<summary>
    ///Get a given secret binding (value omitted) on a script.
    ///</summary>
    member this.WorkerGetScriptSecret
        (
            accountId: string,
            scriptName: string,
            secretName: string,
            ?urlEncoded: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("secret_name", secretName)
                  if urlEncoded.IsSome then
                      RequestPart.query ("url_encoded", urlEncoded.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/secrets/{secret_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerGetScriptSecret.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerGetScriptSecret.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerGetScriptSecret" (int status)
        }

    ///<summary>
    ///Get metadata and config, such as bindings or usage model.
    ///</summary>
    member this.WorkerScriptGetSettings(accountId: string, scriptName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptGetSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptGetSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptGetSettings" (int status)
        }

    ///<summary>
    ///Patch metadata or config, such as bindings or usage model.
    ///</summary>
    member this.WorkerScriptPatchSettings
        (
            accountId: string,
            scriptName: string,
            ?cancellationToken: CancellationToken,
            ?settings: workers_script_u002D_and_u002D_version_u002D_settings_u002D_item
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "settings" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  if settings.IsSome then
                      RequestPart.multipartJson ("settings", "application/json", settings.Value) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptPatchSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptPatchSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptPatchSettings" (int status)
        }

    ///<summary>
    ///Disable all workers.dev subdomains for a Worker.
    ///</summary>
    member this.WorkerScriptDeleteSubdomain
        (accountId: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/subdomain"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptDeleteSubdomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptDeleteSubdomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptDeleteSubdomain" (int status)
        }

    ///<summary>
    ///Get if the Worker is available on the workers.dev subdomain.
    ///</summary>
    member this.WorkerScriptGetSubdomain(accountId: string, scriptName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/subdomain"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptGetSubdomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptGetSubdomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptGetSubdomain" (int status)
        }

    ///<summary>
    ///Enable or disable the Worker on the workers.dev subdomain.
    ///</summary>
    member this.WorkerScriptPostSubdomain
        (
            accountId: string,
            scriptName: string,
            body: WorkerScriptPostSubdomainPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/subdomain"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptPostSubdomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptPostSubdomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptPostSubdomain" (int status)
        }

    ///<summary>
    ///Get list of tails currently deployed on a Worker.
    ///</summary>
    member this.WorkerTailLogsListTails(accountId: string, scriptName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/tails"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerTailLogsListTails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerTailLogsListTails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerTailLogsListTails" (int status)
        }

    ///<summary>
    ///Starts a tail that receives logs and exception from a Worker.
    ///</summary>
    member this.WorkerTailLogsStartTail(accountId: string, scriptName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/tails"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerTailLogsStartTail.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerTailLogsStartTail.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerTailLogsStartTail" (int status)
        }

    ///<summary>
    ///Deletes a tail from a Worker.
    ///</summary>
    member this.WorkerTailLogsDeleteTail
        (accountId: string, scriptName: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/tails/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerTailLogsDeleteTail.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerTailLogsDeleteTail.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerTailLogsDeleteTail" (int status)
        }

    ///<summary>
    ///Fetches the Usage Model for a given Worker.
    ///</summary>
    member this.WorkerScriptFetchUsageModel
        (accountId: string, scriptName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/usage-model"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptFetchUsageModel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptFetchUsageModel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptFetchUsageModel" (int status)
        }

    ///<summary>
    ///Updates the Usage Model for a given Worker. Requires a Workers Paid subscription.
    ///</summary>
    member this.WorkerScriptUpdateUsageModel
        (
            accountId: string,
            scriptName: string,
            body: WorkerScriptUpdateUsageModelPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/usage-model"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptUpdateUsageModel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptUpdateUsageModel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptUpdateUsageModel" (int status)
        }

    ///<summary>
    ///List of Worker Versions. The first version in the list is the latest version.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName"></param>
    ///<param name="deployable">Only return versions that can be used in a deployment. Ignores pagination.</param>
    ///<param name="page">Current page.</param>
    ///<param name="perPage">Items per-page.</param>
    ///<param name="cancellationToken"></param>
    member this.WorkerVersionsListVersions
        (
            accountId: string,
            scriptName: string,
            ?deployable: bool,
            ?page: int,
            ?perPage: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  if deployable.IsSome then
                      RequestPart.query ("deployable", deployable.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/versions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerVersionsListVersions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerVersionsListVersions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerVersionsListVersions" (int status)
        }

    ///<summary>
    ///Upload a Worker Version without deploying to Cloudflare's network. You can find more about the multipart metadata on our docs: https://developers.cloudflare.com/workers/configuration/multipart-upload-metadata/.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="scriptName"></param>
    ///<param name="metadata">JSON-encoded metadata about the uploaded parts and Worker configuration.</param>
    ///<param name="bindingsInherit">When set to "strict", the upload will fail if any `inherit` type bindings cannot be resolved against the previous version of the Worker. Without this, unresolvable inherit bindings are silently dropped.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="files">An array of modules (often JavaScript files) comprising a Worker script. At least one module must be present and referenced in the metadata as `main_module` or `body_part` by filename.&amp;lt;br/&amp;gt;Possible Content-Type(s) are: `application/javascript+module`, `text/javascript+module`, `application/javascript`, `text/javascript`, `text/x-python`, `text/x-python-requirement`, `application/wasm`, `text/plain`, `application/octet-stream`, `application/source-map`.</param>
    member this.WorkerVersionsUploadVersion
        (
            accountId: string,
            scriptName: string,
            metadata: System.Text.Json.Nodes.JsonObject,
            ?bindingsInherit: string,
            ?cancellationToken: CancellationToken,
            ?files: list<MultipartFile>
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "metadata"; "files" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.multipartJson ("metadata", "application/json", metadata)
                  if bindingsInherit.IsSome then
                      RequestPart.query ("bindings_inherit", bindingsInherit.Value)
                  if files.IsSome then
                      RequestPart.multipartBinary (
                          "files",
                          "application/javascript+module, text/javascript+module, application/javascript, text/javascript, `text/x-python`, text/x-python-requirement, application/wasm, text/plain, application/octet-stream, application/source-map",
                          files.Value
                      ) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/versions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerVersionsUploadVersion.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerVersionsUploadVersion.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerVersionsUploadVersion" (int status)
        }

    ///<summary>
    ///Retrieves detailed information about a specific version of a Workers script.
    ///</summary>
    member this.WorkerVersionsGetVersionDetail
        (accountId: string, scriptName: string, versionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("script_name", scriptName)
                  RequestPart.path ("version_id", versionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/scripts/{script_name}/versions/{version_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerVersionsGetVersionDetail.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerVersionsGetVersionDetail.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerVersionsGetVersionDetail" (int status)
        }

    ///<summary>
    ///Get script content from a worker with an environment.
    ///</summary>
    member this.WorkerEnvironmentGetScriptContent
        (accountId: string, serviceName: string, environmentName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("service_name", serviceName)
                  RequestPart.path ("environment_name", environmentName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/services/{service_name}/environments/{environment_name}/content"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerEnvironmentGetScriptContent.OK
            | _ when (((int status) / 100) = 4) ->
                return WorkerEnvironmentGetScriptContent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerEnvironmentGetScriptContent" (int status)
        }

    ///<summary>
    ///Put script content from a worker with an environment.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="serviceName"></param>
    ///<param name="environmentName"></param>
    ///<param name="metadata">JSON-encoded metadata about the uploaded parts and Worker configuration.</param>
    ///<param name="cFWORKERBODYPART">The multipart name of a script upload part containing script content in service worker format. Alternative to including in a metadata part.</param>
    ///<param name="cFWORKERMAINMODULEPART">The multipart name of a script upload part containing script content in es module format. Alternative to including in a metadata part.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="files">An array of modules (often JavaScript files) comprising a Worker script. At least one module must be present and referenced in the metadata as `main_module` or `body_part` by filename.&amp;lt;br/&amp;gt;Possible Content-Type(s) are: `application/javascript+module`, `text/javascript+module`, `application/javascript`, `text/javascript`, `text/x-python`, `text/x-python-requirement`, `application/wasm`, `text/plain`, `application/octet-stream`, `application/source-map`.</param>
    member this.WorkerEnvironmentPutScriptContent
        (
            accountId: string,
            serviceName: string,
            environmentName: string,
            metadata: System.Text.Json.Nodes.JsonObject,
            ?cFWORKERBODYPART: string,
            ?cFWORKERMAINMODULEPART: string,
            ?cancellationToken: CancellationToken,
            ?files: list<MultipartFile>
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "metadata"; "files" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("service_name", serviceName)
                  RequestPart.path ("environment_name", environmentName)
                  RequestPart.multipartJson ("metadata", "application/json", metadata)
                  if cFWORKERBODYPART.IsSome then
                      RequestPart.header ("CF-WORKER-BODY-PART", cFWORKERBODYPART.Value)
                  if cFWORKERMAINMODULEPART.IsSome then
                      RequestPart.header ("CF-WORKER-MAIN-MODULE-PART", cFWORKERMAINMODULEPART.Value)
                  if files.IsSome then
                      RequestPart.multipartBinary (
                          "files",
                          "application/javascript+module, text/javascript+module, application/javascript, text/javascript, text/x-python, text/x-python-requirement, application/wasm, text/plain, application/octet-stream, application/source-map",
                          files.Value
                      ) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/services/{service_name}/environments/{environment_name}/content"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerEnvironmentPutScriptContent.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerEnvironmentPutScriptContent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerEnvironmentPutScriptContent" (int status)
        }

    ///<summary>
    ///Get script settings from a worker with an environment.
    ///</summary>
    member this.WorkerScriptEnvironmentGetSettings
        (accountId: string, serviceName: string, environmentName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("service_name", serviceName)
                  RequestPart.path ("environment_name", environmentName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/services/{service_name}/environments/{environment_name}/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptEnvironmentGetSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptEnvironmentGetSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptEnvironmentGetSettings" (int status)
        }

    ///<summary>
    ///Patch script metadata, such as bindings.
    ///</summary>
    member this.WorkerScriptEnvironmentPatchSettings
        (
            accountId: string,
            serviceName: string,
            environmentName: string,
            body: workers_script_u002D_settings_u002D_response,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("service_name", serviceName)
                  RequestPart.path ("environment_name", environmentName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/services/{service_name}/environments/{environment_name}/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerScriptEnvironmentPatchSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerScriptEnvironmentPatchSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerScriptEnvironmentPatchSettings" (int status)
        }

    ///<summary>
    ///Deletes a Workers subdomain for an account.
    ///</summary>
    member this.WorkerSubdomainDeleteSubdomain(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/subdomain"
                    requestParts
                    cancellationToken

            match (int status) with
            | 204 -> return WorkerSubdomainDeleteSubdomain.NoContent
            | _ when (((int status) / 100) = 4) ->
                return WorkerSubdomainDeleteSubdomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerSubdomainDeleteSubdomain" (int status)
        }

    ///<summary>
    ///Returns a Workers subdomain for an account.
    ///</summary>
    member this.WorkerSubdomainGetSubdomain(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/subdomain"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerSubdomainGetSubdomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerSubdomainGetSubdomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerSubdomainGetSubdomain" (int status)
        }

    ///<summary>
    ///Creates a Workers subdomain for an account.
    ///</summary>
    member this.WorkerSubdomainCreateSubdomain
        (accountId: string, body: workers_subdomain_u002D_2, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/subdomain"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerSubdomainCreateSubdomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerSubdomainCreateSubdomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerSubdomainCreateSubdomain" (int status)
        }

    ///<summary>
    ///List all Workers for an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="page">Current page.</param>
    ///<param name="perPage">Items per-page.</param>
    ///<param name="orderBy">Property to sort results by.</param>
    ///<param name="order">Sort direction.</param>
    ///<param name="cancellationToken"></param>
    member this.ListWorkers
        (
            accountId: string,
            ?page: int,
            ?perPage: int,
            ?orderBy: string,
            ?order: string,
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
                  if order.IsSome then
                      RequestPart.query ("order", order.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/workers/workers" requestParts cancellationToken

            match (int status) with
            | 200 -> return ListWorkers.OK((Serializer.deserialize content))
            | 401 -> return ListWorkers.Unauthorized((Serializer.deserialize content))
            | 500 -> return ListWorkers.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListWorkers" (int status)
        }

    ///<summary>
    ///Create a new Worker.
    ///</summary>
    member this.CreateWorker(accountId: string, body: CreateWorkerPayload, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/workers/workers" requestParts cancellationToken

            match (int status) with
            | 200 -> return CreateWorker.OK((Serializer.deserialize content))
            | 400 -> return CreateWorker.BadRequest((Serializer.deserialize content))
            | 401 -> return CreateWorker.Unauthorized((Serializer.deserialize content))
            | 403 -> return CreateWorker.Forbidden((Serializer.deserialize content))
            | 409 -> return CreateWorker.Conflict((Serializer.deserialize content))
            | 500 -> return CreateWorker.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateWorker" (int status)
        }

    ///<summary>
    ///Delete a Worker and all its associated resources (versions, deployments, etc.).
    ///</summary>
    member this.DeleteWorker(accountId: string, workerId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("worker_id", workerId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/workers/{worker_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteWorker.OK((Serializer.deserialize content))
            | 400 -> return DeleteWorker.BadRequest((Serializer.deserialize content))
            | 401 -> return DeleteWorker.Unauthorized((Serializer.deserialize content))
            | 404 -> return DeleteWorker.NotFound((Serializer.deserialize content))
            | 500 -> return DeleteWorker.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteWorker" (int status)
        }

    ///<summary>
    ///Get details about a specific Worker.
    ///</summary>
    member this.GetWorker(accountId: string, workerId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("worker_id", workerId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/workers/{worker_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetWorker.OK((Serializer.deserialize content))
            | 400 -> return GetWorker.BadRequest((Serializer.deserialize content))
            | 404 -> return GetWorker.NotFound((Serializer.deserialize content))
            | 500 -> return GetWorker.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetWorker" (int status)
        }

    ///<summary>
    ///Perform a partial update on a Worker, where omitted properties are left unchanged from their current values.
    ///</summary>
    member this.EditWorker
        (accountId: string, workerId: string, body: workers_Worker, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("worker_id", workerId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/workers/{worker_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EditWorker.OK((Serializer.deserialize content))
            | 400 -> return EditWorker.BadRequest((Serializer.deserialize content))
            | 401 -> return EditWorker.Unauthorized((Serializer.deserialize content))
            | 403 -> return EditWorker.Forbidden((Serializer.deserialize content))
            | 404 -> return EditWorker.NotFound((Serializer.deserialize content))
            | 409 -> return EditWorker.Conflict((Serializer.deserialize content))
            | 500 -> return EditWorker.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EditWorker" (int status)
        }

    ///<summary>
    ///Perform a complete replacement of a Worker, where omitted properties are set to their default values. This is the exact same as the Create Worker endpoint, but operates on an existing Worker. To perform a partial update instead, use the Edit Worker endpoint.
    ///</summary>
    member this.UpdateWorker
        (accountId: string, workerId: string, body: UpdateWorkerPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("worker_id", workerId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workers/workers/{worker_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateWorker.OK((Serializer.deserialize content))
            | 400 -> return UpdateWorker.BadRequest((Serializer.deserialize content))
            | 401 -> return UpdateWorker.Unauthorized((Serializer.deserialize content))
            | 403 -> return UpdateWorker.Forbidden((Serializer.deserialize content))
            | 404 -> return UpdateWorker.NotFound((Serializer.deserialize content))
            | 409 -> return UpdateWorker.Conflict((Serializer.deserialize content))
            | 500 -> return UpdateWorker.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateWorker" (int status)
        }

    ///<summary>
    ///List all versions for a Worker.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="workerId"></param>
    ///<param name="page">Current page.</param>
    ///<param name="perPage">Items per-page.</param>
    ///<param name="cancellationToken"></param>
    member this.ListWorkerVersions
        (accountId: string, workerId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("worker_id", workerId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/workers/{worker_id}/versions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListWorkerVersions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListWorkerVersions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListWorkerVersions" (int status)
        }

    ///<summary>
    ///Create a new version.
    ///</summary>
    member this.CreateWorkerVersion
        (
            accountId: string,
            workerId: string,
            body: workers_Version,
            ?deploy: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("worker_id", workerId)
                  RequestPart.jsonContent body
                  if deploy.IsSome then
                      RequestPart.query ("deploy", deploy.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workers/workers/{worker_id}/versions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateWorkerVersion.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreateWorkerVersion.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateWorkerVersion" (int status)
        }

    ///<summary>
    ///Only `/versions/latest` is supported. Creates a new version by applying a JSON Merge Patch (RFC 7396) to the latest version. Patching a specific version ID is not supported. Omitted fields are inherited from the latest version.
    ///</summary>
    member this.PatchLatestWorkerVersion
        (
            accountId: string,
            workerId: string,
            ?deploy: bool,
            ?cancellationToken: CancellationToken,
            ?body: workers_Version
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("worker_id", workerId)
                  if deploy.IsSome then
                      RequestPart.query ("deploy", deploy.Value)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workers/workers/{worker_id}/versions/latest"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PatchLatestWorkerVersion.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PatchLatestWorkerVersion.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PatchLatestWorkerVersion" (int status)
        }

    ///<summary>
    ///Delete a version.
    ///</summary>
    member this.DeleteWorkerVersion
        (accountId: string, workerId: string, versionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("worker_id", workerId)
                  RequestPart.path ("version_id", versionId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workers/workers/{worker_id}/versions/{version_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteWorkerVersion.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteWorkerVersion.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteWorkerVersion" (int status)
        }

    ///<summary>
    ///Get details about a specific version.
    ///</summary>
    member this.GetWorkerVersion
        (accountId: string, workerId: string, versionId: string, ?include: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("worker_id", workerId)
                  RequestPart.path ("version_id", versionId)
                  if include.IsSome then
                      RequestPart.query ("include", include.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workers/workers/{worker_id}/versions/{version_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetWorkerVersion.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetWorkerVersion.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetWorkerVersion" (int status)
        }

    ///<summary>
    ///Lists all workflows configured for the account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="perPage"></param>
    ///<param name="page"></param>
    ///<param name="search">Allows filtering workflows` name.</param>
    ///<param name="cancellationToken"></param>
    member this.WorListWorkflows
        (accountId: string, ?perPage: float, ?page: float, ?search: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/workflows" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorListWorkflows.OK((Serializer.deserialize content))
            | 400 -> return WorListWorkflows.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorListWorkflows" (int status)
        }

    ///<summary>
    ///Retrieves account-level Workflows settings, such as the default instance retention.
    ///</summary>
    member this.WorGetWorkflowSettings(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorGetWorkflowSettings.OK((Serializer.deserialize content))
            | 400 -> return WorGetWorkflowSettings.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorGetWorkflowSettings" (int status)
        }

    ///<summary>
    ///Updates only the account-level Workflows settings fields present in the request body.
    ///</summary>
    member this.WorUpdateWorkflowSettings
        (accountId: string, body: WorUpdateWorkflowSettingsPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workflows/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorUpdateWorkflowSettings.OK((Serializer.deserialize content))
            | 400 -> return WorUpdateWorkflowSettings.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorUpdateWorkflowSettings" (int status)
        }

    ///<summary>
    ///Deletes a Workflow. This only deletes the Workflow and does not delete or modify any Worker associated to this Workflow or bounded to it.
    ///</summary>
    member this.WorDeleteWorkflow(workflowName: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorDeleteWorkflow.OK((Serializer.deserialize content))
            | 400 -> return WorDeleteWorkflow.BadRequest((Serializer.deserialize content))
            | 404 -> return WorDeleteWorkflow.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorDeleteWorkflow" (int status)
        }

    ///<summary>
    ///Retrieves configuration and metadata for a specific workflow.
    ///</summary>
    member this.WorGetWorkflowDetails(workflowName: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorGetWorkflowDetails.OK((Serializer.deserialize content))
            | 400 -> return WorGetWorkflowDetails.BadRequest((Serializer.deserialize content))
            | 404 -> return WorGetWorkflowDetails.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorGetWorkflowDetails" (int status)
        }

    ///<summary>
    ///Creates a new workflow or updates an existing workflow definition.
    ///</summary>
    member this.WorCreateOrModifyWorkflow
        (
            workflowName: string,
            accountId: string,
            body: WorCreateOrModifyWorkflowPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorCreateOrModifyWorkflow.OK((Serializer.deserialize content))
            | 400 -> return WorCreateOrModifyWorkflow.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorCreateOrModifyWorkflow" (int status)
        }

    ///<summary>
    ///Lists all instances of a workflow with their execution status.
    ///</summary>
    ///<param name="workflowName"></param>
    ///<param name="accountId"></param>
    ///<param name="page">Deprecated: use `cursor` for pagination instead.</param>
    ///<param name="perPage"></param>
    ///<param name="cursor">Opaque token for cursor-based pagination. Mutually exclusive with `page`.</param>
    ///<param name="direction">Defines the direction for cursor-based pagination.</param>
    ///<param name="status"></param>
    ///<param name="dateStart">Accepts ISO 8601 with no timezone offsets and in UTC.</param>
    ///<param name="dateEnd">Accepts ISO 8601 with no timezone offsets and in UTC.</param>
    ///<param name="cancellationToken"></param>
    member this.WorListWorkflowInstances
        (
            workflowName: string,
            accountId: string,
            ?page: float,
            ?perPage: float,
            ?cursor: string,
            ?direction: string,
            ?status: string,
            ?dateStart: System.DateTimeOffset,
            ?dateEnd: System.DateTimeOffset,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if dateStart.IsSome then
                      RequestPart.query ("date_start", dateStart.Value)
                  if dateEnd.IsSome then
                      RequestPart.query ("date_end", dateEnd.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorListWorkflowInstances.OK((Serializer.deserialize content))
            | 400 -> return WorListWorkflowInstances.BadRequest((Serializer.deserialize content))
            | 404 -> return WorListWorkflowInstances.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorListWorkflowInstances" (int status)
        }

    ///<summary>
    ///Creates a new instance of a workflow, starting its execution.
    ///</summary>
    member this.WorCreateNewWorkflowInstance
        (
            workflowName: string,
            accountId: string,
            body: WorCreateNewWorkflowInstancePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorCreateNewWorkflowInstance.OK((Serializer.deserialize content))
            | 400 -> return WorCreateNewWorkflowInstance.BadRequest((Serializer.deserialize content))
            | 404 -> return WorCreateNewWorkflowInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorCreateNewWorkflowInstance" (int status)
        }

    ///<summary>
    ///Creates multiple workflow instances in a single batch operation.
    ///</summary>
    member this.WorBatchCreateWorkflowInstance
        (
            workflowName: string,
            accountId: string,
            body: WorBatchCreateWorkflowInstancePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/batch"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorBatchCreateWorkflowInstance.OK((Serializer.deserialize content))
            | 400 -> return WorBatchCreateWorkflowInstance.BadRequest((Serializer.deserialize content))
            | 404 -> return WorBatchCreateWorkflowInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorBatchCreateWorkflowInstance" (int status)
        }

    ///<summary>
    ///Deletes multiple workflow instances in a single batch operation.
    ///</summary>
    member this.WorBatchDeleteWorkflowInstances
        (
            workflowName: string,
            accountId: string,
            body: WorBatchDeleteWorkflowInstancesPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/batch/delete"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorBatchDeleteWorkflowInstances.OK((Serializer.deserialize content))
            | 400 -> return WorBatchDeleteWorkflowInstances.BadRequest((Serializer.deserialize content))
            | 404 -> return WorBatchDeleteWorkflowInstances.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorBatchDeleteWorkflowInstances" (int status)
        }

    ///<summary>
    ///Performs a batch termination of multiple workflow instances.
    ///</summary>
    member this.WorBatchTerminateWorkflowInstances
        (workflowName: string, accountId: string, body: list<string>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/batch/terminate"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorBatchTerminateWorkflowInstances.OK((Serializer.deserialize content))
            | 400 -> return WorBatchTerminateWorkflowInstances.BadRequest((Serializer.deserialize content))
            | 404 -> return WorBatchTerminateWorkflowInstances.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorBatchTerminateWorkflowInstances" (int status)
        }

    ///<summary>
    ///Gets the status of a bulk workflow instance termination job.
    ///</summary>
    member this.WorStatusTerminateWorkflowInstances
        (workflowName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/terminate"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorStatusTerminateWorkflowInstances.OK((Serializer.deserialize content))
            | 400 -> return WorStatusTerminateWorkflowInstances.BadRequest((Serializer.deserialize content))
            | 404 -> return WorStatusTerminateWorkflowInstances.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorStatusTerminateWorkflowInstances" (int status)
        }

    ///<summary>
    ///Deletes a workflow instance and its stored state.
    ///</summary>
    ///<param name="workflowName"></param>
    ///<param name="instanceId">Instance identifier. User-created instances match `^[a-zA-Z0-9_][a-zA-Z0-9-_]*$` (max 100 characters); cron-triggered instances can use a longer, system-generated id derived from the cron expression.</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.WorDeleteWorkflowInstance
        (workflowName: string, instanceId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("instance_id", instanceId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/{instance_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorDeleteWorkflowInstance.OK((Serializer.deserialize content))
            | 404 -> return WorDeleteWorkflowInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorDeleteWorkflowInstance" (int status)
        }

    ///<summary>
    ///Retrieves logs and execution status for a specific workflow instance.
    ///</summary>
    ///<param name="workflowName"></param>
    ///<param name="instanceId">Instance identifier. User-created instances match `^[a-zA-Z0-9_][a-zA-Z0-9-_]*$` (max 100 characters); cron-triggered instances can use a longer, system-generated id derived from the cron expression.</param>
    ///<param name="accountId"></param>
    ///<param name="simple">When true, omits step details and returns only metadata with step_count.</param>
    ///<param name="order">Step ordering: "asc" (default, oldest first) or "desc" (newest first).</param>
    ///<param name="cancellationToken"></param>
    member this.WorDescribeWorkflowInstance
        (
            workflowName: string,
            instanceId: string,
            accountId: string,
            ?simple: string,
            ?order: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("instance_id", instanceId)
                  RequestPart.path ("account_id", accountId)
                  if simple.IsSome then
                      RequestPart.query ("simple", simple.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/{instance_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorDescribeWorkflowInstance.OK((Serializer.deserialize content))
            | 400 -> return WorDescribeWorkflowInstance.BadRequest((Serializer.deserialize content))
            | 404 -> return WorDescribeWorkflowInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorDescribeWorkflowInstance" (int status)
        }

    ///<summary>
    ///Sends an event to a running workflow instance to trigger state transitions.
    ///</summary>
    ///<param name="workflowName"></param>
    ///<param name="instanceId">Instance identifier. User-created instances match `^[a-zA-Z0-9_][a-zA-Z0-9-_]*$` (max 100 characters); cron-triggered instances can use a longer, system-generated id derived from the cron expression.</param>
    ///<param name="eventType"></param>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.WorSendEventWorkflowInstance
        (
            workflowName: string,
            instanceId: string,
            eventType: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("instance_id", instanceId)
                  RequestPart.path ("event_type", eventType)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/{instance_id}/events/{event_type}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorSendEventWorkflowInstance.OK((Serializer.deserialize content))
            | 400 -> return WorSendEventWorkflowInstance.BadRequest((Serializer.deserialize content))
            | 404 -> return WorSendEventWorkflowInstance.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorSendEventWorkflowInstance" (int status)
        }

    ///<summary>
    ///Changes the execution status of a workflow instance (e.g., pause, resume, terminate).
    ///</summary>
    ///<param name="workflowName"></param>
    ///<param name="instanceId">Instance identifier. User-created instances match `^[a-zA-Z0-9_][a-zA-Z0-9-_]*$` (max 100 characters); cron-triggered instances can use a longer, system-generated id derived from the cron expression.</param>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.WorChangeStatusWorkflowInstance
        (
            workflowName: string,
            instanceId: string,
            accountId: string,
            body: InlineUnion_fb923d8f3dad4ef22f9bf0c5,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("instance_id", instanceId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/{instance_id}/status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorChangeStatusWorkflowInstance.OK((Serializer.deserialize content))
            | 400 -> return WorChangeStatusWorkflowInstance.BadRequest((Serializer.deserialize content))
            | 404 -> return WorChangeStatusWorkflowInstance.NotFound((Serializer.deserialize content))
            | 409 -> return WorChangeStatusWorkflowInstance.Conflict((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorChangeStatusWorkflowInstance" (int status)
        }

    ///<summary>
    ///Retrieves the full, untruncated output for a specific step on a workflow instance. Returns a flat status-shaped JSON body with step `status` ('running' | 'waiting' | 'complete' | 'errored'), `error` (nullable), and `output` (the step value, or null while running/waiting/errored). When the step returned a ReadableStream from step.do, the response is served as 'application/octet-stream' with the raw bytes as the body instead of JSON. A `status='running'` response with non-null `error` indicates the step is currently retrying after a prior attempt failed.
    ///</summary>
    ///<param name="workflowName"></param>
    ///<param name="instanceId">Instance identifier. User-created instances match `^[a-zA-Z0-9_][a-zA-Z0-9-_]*$` (max 100 characters); cron-triggered instances can use a longer, system-generated id derived from the cron expression.</param>
    ///<param name="name">Exact step name from the instance logs response, including the generated counter suffix.</param>
    ///<param name="type">Step type to disambiguate step.do and waitForEvent entries that share the same name.</param>
    ///<param name="accountId"></param>
    ///<param name="attempt">Specific attempt number to retrieve output or error for.</param>
    ///<param name="cancellationToken"></param>
    member this.WorGetWorkflowInstanceStep
        (
            workflowName: string,
            instanceId: string,
            name: string,
            ``type``: string,
            accountId: string,
            ?attempt: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("instance_id", instanceId)
                  RequestPart.query ("name", name)
                  RequestPart.query ("type", ``type``)
                  RequestPart.path ("account_id", accountId)
                  if attempt.IsSome then
                      RequestPart.query ("attempt", attempt.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/{instance_id}/step"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorGetWorkflowInstanceStep.OK((Serializer.deserialize content))
            | 400 -> return WorGetWorkflowInstanceStep.BadRequest((Serializer.deserialize content))
            | 404 -> return WorGetWorkflowInstanceStep.NotFound((Serializer.deserialize content))
            | 429 -> return WorGetWorkflowInstanceStep.TooManyRequests((Serializer.deserialize content))
            | 500 -> return WorGetWorkflowInstanceStep.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorGetWorkflowInstanceStep" (int status)
        }

    ///<summary>
    ///Opens a WebSocket that streams workflow instance events.
    ///</summary>
    ///<param name="workflowName"></param>
    ///<param name="instanceId">Instance identifier. User-created instances match `^[a-zA-Z0-9_][a-zA-Z0-9-_]*$` (max 100 characters); cron-triggered instances can use a longer, system-generated id derived from the cron expression.</param>
    ///<param name="accountId"></param>
    ///<param name="cursor">Last event ID already received.</param>
    ///<param name="filter">Comma-separated workflow event types to include.</param>
    ///<param name="cancellationToken"></param>
    member this.WorSubscribeWorkflowInstanceEvents
        (
            workflowName: string,
            instanceId: string,
            accountId: string,
            ?cursor: int,
            ?filter: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("instance_id", instanceId)
                  RequestPart.path ("account_id", accountId)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if filter.IsSome then
                      RequestPart.query ("filter", filter.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/{instance_id}/subscribe"
                    requestParts
                    cancellationToken

            match (int status) with
            | 101 -> return WorSubscribeWorkflowInstanceEvents.SwitchingProtocols
            | 400 -> return WorSubscribeWorkflowInstanceEvents.BadRequest((Serializer.deserialize content))
            | 404 -> return WorSubscribeWorkflowInstanceEvents.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorSubscribeWorkflowInstanceEvents" (int status)
        }

    ///<summary>
    ///Creates a short-lived token for connecting to the workflow instance event WebSocket.
    ///</summary>
    ///<param name="workflowName"></param>
    ///<param name="instanceId">Instance identifier. User-created instances match `^[a-zA-Z0-9_][a-zA-Z0-9-_]*$` (max 100 characters); cron-triggered instances can use a longer, system-generated id derived from the cron expression.</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.WorCreateWorkflowInstanceSubscriptionToken
        (workflowName: string, instanceId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("instance_id", instanceId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/instances/{instance_id}/subscribe/token"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorCreateWorkflowInstanceSubscriptionToken.OK((Serializer.deserialize content))
            | 400 -> return WorCreateWorkflowInstanceSubscriptionToken.BadRequest((Serializer.deserialize content))
            | 404 -> return WorCreateWorkflowInstanceSubscriptionToken.NotFound((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for WorCreateWorkflowInstanceSubscriptionToken" (int status)
        }

    ///<summary>
    ///Lists all deployed versions of a workflow.
    ///</summary>
    member this.WorListWorkflowVersions
        (workflowName: string, accountId: string, ?perPage: float, ?page: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("account_id", accountId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/versions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorListWorkflowVersions.OK((Serializer.deserialize content))
            | 400 -> return WorListWorkflowVersions.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorListWorkflowVersions" (int status)
        }

    ///<summary>
    ///Retrieves details for a specific deployed workflow version.
    ///</summary>
    member this.WorDescribeWorkflowVersions
        (workflowName: string, versionId: System.Guid, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("version_id", versionId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/versions/{version_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorDescribeWorkflowVersions.OK((Serializer.deserialize content))
            | 400 -> return WorDescribeWorkflowVersions.BadRequest((Serializer.deserialize content))
            | 404 -> return WorDescribeWorkflowVersions.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorDescribeWorkflowVersions" (int status)
        }

    ///<summary>
    ///Retrieves the directed acyclic graph (DAG) representation of a workflow version.
    ///</summary>
    member this.WorDescribeWorkflowVersionsDag
        (workflowName: string, versionId: System.Guid, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("version_id", versionId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/versions/{version_id}/dag"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorDescribeWorkflowVersionsDag.OK((Serializer.deserialize content))
            | 404 -> return WorDescribeWorkflowVersionsDag.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorDescribeWorkflowVersionsDag" (int status)
        }

    ///<summary>
    ///Retrieves the graph visualization of a workflow version.
    ///</summary>
    member this.WorDescribeWorkflowVersionsGraph
        (workflowName: string, versionId: System.Guid, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("workflow_name", workflowName)
                  RequestPart.path ("version_id", versionId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/workflows/{workflow_name}/versions/{version_id}/graph"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorDescribeWorkflowVersionsGraph.OK((Serializer.deserialize content))
            | 404 -> return WorDescribeWorkflowVersionsGraph.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorDescribeWorkflowVersionsGraph" (int status)
        }

    ///<summary>
    ///Check which of the provided file hashes are missing from the Pages
    ///asset store. Returns a list of missing hashes that need to be uploaded.
    ///Used as part of the Pages Direct Upload workflow.
    ///Authenticate with the JWT obtained from the upload-token endpoint:
    ///GET /accounts/{account_id}/pages/projects/{project_name}/upload-token
    ///</summary>
    member this.PagesAssetsCheckMissing
        (body: pages_pages_assets_check_missing_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/pages/assets/check-missing" requestParts cancellationToken

            match (int status) with
            | 200 -> return PagesAssetsCheckMissing.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesAssetsCheckMissing.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesAssetsCheckMissing" (int status)
        }

    ///<summary>
    ///Upload one or more files to the Pages asset store. Each file is
    ///identified by its content hash and is uploaded using the same JSON shape
    ///as the Cloudflare KV bulk write API. Used as part of the Pages Direct
    ///Upload workflow.
    ///Authenticate with the JWT obtained from the upload-token endpoint:
    ///GET /accounts/{account_id}/pages/projects/{project_name}/upload-token
    ///</summary>
    member this.PagesAssetsUpload(body: pages_pages_assets_upload_request, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/pages/assets/upload" requestParts cancellationToken

            match (int status) with
            | 200 -> return PagesAssetsUpload.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesAssetsUpload.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesAssetsUpload" (int status)
        }

    ///<summary>
    ///Register the provided file hashes as recently uploaded to the Pages
    ///asset store. Used as part of the Pages Direct Upload workflow so future
    ///deployments can avoid re-uploading files that are already present.
    ///Authenticate with the JWT obtained from the upload-token endpoint:
    ///GET /accounts/{account_id}/pages/projects/{project_name}/upload-token
    ///</summary>
    member this.PagesAssetsUpsertHashes
        (body: pages_pages_assets_upsert_hashes_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/pages/assets/upsert-hashes" requestParts cancellationToken

            match (int status) with
            | 200 -> return PagesAssetsUpsertHashes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PagesAssetsUpsertHashes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PagesAssetsUpsertHashes" (int status)
        }

    ///<summary>
    ///Trigger a build using a deploy hook. This endpoint does not require authentication - the deploy_hook_uuid acts as a secret token.
    ///</summary>
    ///<param name="deployHookUuid">Deploy hook UUID</param>
    ///<param name="cancellationToken"></param>
    member this.TriggerDeployHook(deployHookUuid: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("deploy_hook_uuid", deployHookUuid) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/workers/builds/deploy_hooks/{deploy_hook_uuid}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TriggerDeployHook.OK((Serializer.deserialize content))
            | 404 -> return TriggerDeployHook.NotFound((Serializer.deserialize content))
            | 429 -> return TriggerDeployHook.TooManyRequests((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TriggerDeployHook" (int status)
        }

    ///<summary>
    ///Fetches all snippets belonging to the zone.
    ///</summary>
    member this.ListZoneSnippets(zoneId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/snippets" requestParts cancellationToken

            match (int status) with
            | 200 -> return ListZoneSnippets.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListZoneSnippets.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ListZoneSnippets.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListZoneSnippets" (int status)
        }

    ///<summary>
    ///Deletes all snippet rules belonging to the zone.
    ///</summary>
    member this.DeleteZoneSnippetRules(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/snippets/snippet_rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteZoneSnippetRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteZoneSnippetRules.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return DeleteZoneSnippetRules.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteZoneSnippetRules" (int status)
        }

    ///<summary>
    ///Fetches all snippet rules belonging to the zone.
    ///</summary>
    member this.ListZoneSnippetRules(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/snippets/snippet_rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return ListZoneSnippetRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListZoneSnippetRules.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ListZoneSnippetRules.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListZoneSnippetRules" (int status)
        }

    ///<summary>
    ///Updates all snippet rules belonging to the zone.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Define a snippet rules object.</param>
    ///<param name="cancellationToken"></param>
    member this.UpdateZoneSnippetRules
        (zoneId: string, body: UpdateZoneSnippetRulesPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/snippets/snippet_rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return UpdateZoneSnippetRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateZoneSnippetRules.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return UpdateZoneSnippetRules.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateZoneSnippetRules" (int status)
        }

    ///<summary>
    ///Deletes a snippet belonging to the zone. Returns a 4XX response if the zone or snippet no longer exists.
    ///</summary>
    member this.DeleteZoneSnippet(zoneId: string, snippetName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("snippet_name", snippetName) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/snippets/{snippet_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteZoneSnippet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteZoneSnippet.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return DeleteZoneSnippet.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteZoneSnippet" (int status)
        }

    ///<summary>
    ///Fetches a snippet belonging to the zone.
    ///</summary>
    member this.GetZoneSnippet(zoneId: string, snippetName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("snippet_name", snippetName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/snippets/{snippet_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetZoneSnippet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetZoneSnippet.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return GetZoneSnippet.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetZoneSnippet" (int status)
        }

    ///<summary>
    ///Creates or updates a snippet belonging to the zone.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="snippetName"></param>
    ///<param name="files">Contain files belonging to the snippet.</param>
    ///<param name="metadata">Provide metadata about the snippet.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body">Contain files belonging to the snippet.</param>
    member this.UpdateZoneSnippet
        (
            zoneId: string,
            snippetName: string,
            files: list<MultipartFile>,
            metadata: System.Text.Json.Nodes.JsonObject,
            ?cancellationToken: CancellationToken,
            ?body: list<MultipartFileField>
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "files"; "metadata" ]
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("snippet_name", snippetName)
                  RequestPart.multipartBinary ("files", "application/octet-stream", files)
                  RequestPart.multipartJson ("metadata", "application/json", metadata)
                  if body.IsSome then
                      RequestPart.multipartFileFields ("body", "application/octet-stream", body.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/snippets/{snippet_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateZoneSnippet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateZoneSnippet.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return UpdateZoneSnippet.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateZoneSnippet" (int status)
        }

    ///<summary>
    ///Fetches the content of a snippet belonging to the zone.
    ///</summary>
    member this.GetZoneSnippetContent(zoneId: string, snippetName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("snippet_name", snippetName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/snippets/{snippet_name}/content"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetZoneSnippetContent.OK
            | _ when (((int status) / 100) = 4) ->
                return GetZoneSnippetContent.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return GetZoneSnippetContent.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetZoneSnippetContent" (int status)
        }

    ///<summary>
    ///Returns routes for a zone.
    ///</summary>
    member this.WorkerRoutesListRoutes(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/workers/routes" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkerRoutesListRoutes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerRoutesListRoutes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerRoutesListRoutes" (int status)
        }

    ///<summary>
    ///Creates a route that maps a URL pattern to a Worker.
    ///</summary>
    member this.WorkerRoutesCreateRoute(zoneId: string, body: workers_route, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/workers/routes" requestParts cancellationToken

            match (int status) with
            | 200 -> return WorkerRoutesCreateRoute.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerRoutesCreateRoute.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerRoutesCreateRoute" (int status)
        }

    ///<summary>
    ///Deletes a route.
    ///</summary>
    member this.WorkerRoutesDeleteRoute(routeId: string, zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("route_id", routeId); RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/workers/routes/{route_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerRoutesDeleteRoute.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerRoutesDeleteRoute.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerRoutesDeleteRoute" (int status)
        }

    ///<summary>
    ///Returns information about a route, including URL pattern and Worker.
    ///</summary>
    member this.WorkerRoutesGetRoute(routeId: string, zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("route_id", routeId); RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/workers/routes/{route_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerRoutesGetRoute.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerRoutesGetRoute.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerRoutesGetRoute" (int status)
        }

    ///<summary>
    ///Updates the URL pattern or Worker associated with a route.
    ///</summary>
    member this.WorkerRoutesUpdateRoute
        (routeId: string, zoneId: string, body: workers_route, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("route_id", routeId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/workers/routes/{route_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkerRoutesUpdateRoute.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkerRoutesUpdateRoute.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkerRoutesUpdateRoute" (int status)
        }
