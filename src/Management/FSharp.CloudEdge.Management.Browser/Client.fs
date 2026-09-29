namespace rec FSharp.CloudEdge.Management.Browser

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
type BrowserClient(httpClient: HttpClient) =
    ///<summary>
    ///Deletes the browser extension configuration for an account.
    ///The shard mapping is preserved and not deleted.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.AccountsBrowserExtensionConfigDelete(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/browser-extension/config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountsBrowserExtensionConfigDelete.OK((Serializer.deserialize content))
            | 400 -> return AccountsBrowserExtensionConfigDelete.BadRequest((Serializer.deserialize content))
            | 401 -> return AccountsBrowserExtensionConfigDelete.Unauthorized((Serializer.deserialize content))
            | 403 -> return AccountsBrowserExtensionConfigDelete.Forbidden((Serializer.deserialize content))
            | 404 -> return AccountsBrowserExtensionConfigDelete.NotFound((Serializer.deserialize content))
            | 500 -> return AccountsBrowserExtensionConfigDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsBrowserExtensionConfigDelete" (int status)
        }

    ///<summary>
    ///Returns the browser extension configuration for an account.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.AccountsBrowserExtensionConfigGet(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/browser-extension/config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountsBrowserExtensionConfigGet.OK((Serializer.deserialize content))
            | 401 -> return AccountsBrowserExtensionConfigGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return AccountsBrowserExtensionConfigGet.Forbidden((Serializer.deserialize content))
            | 404 -> return AccountsBrowserExtensionConfigGet.NotFound((Serializer.deserialize content))
            | 500 -> return AccountsBrowserExtensionConfigGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsBrowserExtensionConfigGet" (int status)
        }

    ///<summary>
    ///Creates the browser extension configuration for an account.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AccountsBrowserExtensionConfigPost
        (accountId: string, body: brex_CreateConfigRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/browser-extension/config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountsBrowserExtensionConfigPost.OK((Serializer.deserialize content))
            | 400 -> return AccountsBrowserExtensionConfigPost.BadRequest((Serializer.deserialize content))
            | 401 -> return AccountsBrowserExtensionConfigPost.Unauthorized((Serializer.deserialize content))
            | 403 -> return AccountsBrowserExtensionConfigPost.Forbidden((Serializer.deserialize content))
            | 409 -> return AccountsBrowserExtensionConfigPost.Conflict((Serializer.deserialize content))
            | 500 -> return AccountsBrowserExtensionConfigPost.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsBrowserExtensionConfigPost" (int status)
        }

    ///<summary>
    ///Replaces the browser extension configuration for an account.
    ///The configuration must already exist. The shard mapping is immutable.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AccountsBrowserExtensionConfigPut
        (accountId: string, body: brex_UpdateConfigRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/browser-extension/config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountsBrowserExtensionConfigPut.OK((Serializer.deserialize content))
            | 400 -> return AccountsBrowserExtensionConfigPut.BadRequest((Serializer.deserialize content))
            | 401 -> return AccountsBrowserExtensionConfigPut.Unauthorized((Serializer.deserialize content))
            | 403 -> return AccountsBrowserExtensionConfigPut.Forbidden((Serializer.deserialize content))
            | 404 -> return AccountsBrowserExtensionConfigPut.NotFound((Serializer.deserialize content))
            | 500 -> return AccountsBrowserExtensionConfigPut.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountsBrowserExtensionConfigPut" (int status)
        }
