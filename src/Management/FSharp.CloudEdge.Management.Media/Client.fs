namespace rec FSharp.CloudEdge.Management.Media

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
type MediaClient(httpClient: HttpClient) =
    ///<summary>
    ///List images for an account.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="perPage">Number of items per page</param>
    ///<param name="continuationToken">Continuation token for a next page. List images V2 returns continuation_token</param>
    ///<param name="creator">Internal user ID set within the creator field. Setting to empty string will return images where creator field is not set</param>
    ///<param name="sortOrder">Sorting order by upload time</param>
    ///<param name="cancellationToken"></param>
    member this.GetImageList
        (
            accountId: string,
            ?perPage: int,
            ?continuationToken: string,
            ?creator: string,
            ?sortOrder: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("accountId", accountId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if continuationToken.IsSome then
                      RequestPart.query ("continuation_token", continuationToken.Value)
                  if creator.IsSome then
                      RequestPart.query ("creator", creator.Value)
                  if sortOrder.IsSome then
                      RequestPart.query ("sort_order", sortOrder.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{accountId}/v1/images" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetImageList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetImageList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetImageList" (int status)
        }

    ///<summary>
    ///Upload a new image.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="url">A URL to fetch an image from origin. Only needed when type is uploading from a URL.</param>
    ///<param name="requireSignedURLs">Indicates whether the image requires a signature token for the access.</param>
    ///<param name="metadata">User modifiable key-value store. Can use used for keeping references to another system of record for managing images.</param>
    ///<param name="id">Optional Image Custom ID. Up to 1024 chars. Can include any number of subpaths, and utf8 characters. Cannot start nor end with a / (forward slash). Cannot be a UUID.</param>
    ///<param name="file">An image binary data. Only needed when type is uploading a file.</param>
    ///<param name="creator">Can set the creator field with an internal user ID.</param>
    member this.PostImageUpload
        (
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?url: string,
            ?requireSignedURLs: bool,
            ?metadata: System.Text.Json.Nodes.JsonObject,
            ?id: string,
            ?file: MultipartFile,
            ?creator: string
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartDeclaredFields
                      [ "url"; "requireSignedURLs"; "metadata"; "id"; "file"; "creator" ]
                  RequestPart.path ("accountId", accountId)
                  if url.IsSome then
                      RequestPart.multipartScalar ("url", "text/plain", url.Value)
                  if requireSignedURLs.IsSome then
                      RequestPart.multipartScalar ("requireSignedURLs", "text/plain", requireSignedURLs.Value)
                  if metadata.IsSome then
                      RequestPart.multipartJson ("metadata", "application/json", metadata.Value)
                  if id.IsSome then
                      RequestPart.multipartScalar ("id", "text/plain", id.Value)
                  if file.IsSome then
                      RequestPart.multipartBinary ("file", "application/octet-stream", file.Value)
                  if creator.IsSome then
                      RequestPart.multipartScalar ("creator", "text/plain", creator.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{accountId}/v1/images" requestParts cancellationToken

            match (int status) with
            | 200 -> return PostImageUpload.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PostImageUpload.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostImageUpload" (int status)
        }

    ///<summary>
    ///Delete an image.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="imageId">Image identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteImageDelete(accountId: string, imageId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("accountId", accountId)
                  RequestPart.path ("imageId", imageId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{accountId}/v1/images/{imageId}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteImageDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteImageDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteImageDelete" (int status)
        }

    ///<summary>
    ///Get image metadata.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="imageId">Image identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.GetImageGet(accountId: string, imageId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("accountId", accountId)
                  RequestPart.path ("imageId", imageId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{accountId}/v1/images/{imageId}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetImageGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetImageGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetImageGet" (int status)
        }

    ///<summary>
    ///Update an image's metadata.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="imageId">Image identifier.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.PatchImageEdit
        (accountId: string, imageId: string, ?cancellationToken: CancellationToken, ?body: PatchImageEditPayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("accountId", accountId)
                  RequestPart.path ("imageId", imageId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{accountId}/v1/images/{imageId}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PatchImageEdit.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PatchImageEdit.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PatchImageEdit" (int status)
        }

    ///<summary>
    ///Upload an image to a specific image ID.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="imageId">Image identifier.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="url">A URL to fetch an image from origin. Only needed when type is uploading from a URL.</param>
    ///<param name="file">An image binary data. Only needed when type is uploading a file.</param>
    member this.PutImageDirectUpload
        (accountId: string, imageId: string, ?cancellationToken: CancellationToken, ?url: string, ?file: MultipartFile)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartDeclaredFields [ "url"; "file" ]
                  RequestPart.path ("accountId", accountId)
                  RequestPart.path ("imageId", imageId)
                  if url.IsSome then
                      RequestPart.multipartScalar ("url", "text/plain", url.Value)
                  if file.IsSome then
                      RequestPart.multipartBinary ("file", "application/octet-stream", file.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{accountId}/v1/images/{imageId}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PutImageDirectUpload.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PutImageDirectUpload.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PutImageDirectUpload" (int status)
        }

    ///<summary>
    ///Get image as a blob.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="imageId">Image identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.GetImageGetBlob(accountId: string, imageId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("accountId", accountId)
                  RequestPart.path ("imageId", imageId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{accountId}/v1/images/{imageId}/blob"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetImageGetBlob.OK
            | _ when (((int status) / 100) = 4) ->
                return GetImageGetBlob.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetImageGetBlob" (int status)
        }

    ///<summary>
    ///List images for an account.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="perPage">Number of items per page</param>
    ///<param name="continuationToken">Continuation token for a next page. List images V2 returns continuation_token</param>
    ///<param name="creator">Internal user ID set within the creator field. Setting to empty string will return images where creator field is not set</param>
    ///<param name="sortOrder">Sorting order by upload time</param>
    ///<param name="cancellationToken"></param>
    member this.GetAccountsAccountIdV2Images
        (
            accountId: string,
            ?perPage: int,
            ?continuationToken: string,
            ?creator: string,
            ?sortOrder: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("accountId", accountId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if continuationToken.IsSome then
                      RequestPart.query ("continuation_token", continuationToken.Value)
                  if creator.IsSome then
                      RequestPart.query ("creator", creator.Value)
                  if sortOrder.IsSome then
                      RequestPart.query ("sort_order", sortOrder.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{accountId}/v2/images" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetAccountsAccountIdV2Images.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetAccountsAccountIdV2Images.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetAccountsAccountIdV2Images" (int status)
        }

    ///<summary>
    ///Create an authenticated direct upload URL for an image.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="requireSignedURLs">Indicates whether the image requires a signature token for the access.</param>
    ///<param name="metadata">User modifiable key-value store. Can use used for keeping references to another system of record for managing images.</param>
    ///<param name="id">Optional Image Custom ID. Up to 1024 chars. Can include any number of subpaths, and utf8 characters. Cannot start nor end with a / (forward slash). Cannot be a UUID.</param>
    ///<param name="expiry">The date after which the upload will not be accepted. Minimum: Now + 2 minutes.</param>
    member this.PostDirectUploadLink
        (
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?requireSignedURLs: bool,
            ?metadata: System.Text.Json.Nodes.JsonObject,
            ?id: string,
            ?expiry: string
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartDeclaredFields [ "requireSignedURLs"; "metadata"; "id"; "expiry" ]
                  RequestPart.path ("accountId", accountId)
                  if requireSignedURLs.IsSome then
                      RequestPart.multipartScalar ("requireSignedURLs", "text/plain", requireSignedURLs.Value)
                  if metadata.IsSome then
                      RequestPart.multipartJson ("metadata", "application/json", metadata.Value)
                  if id.IsSome then
                      RequestPart.multipartScalar ("id", "text/plain", id.Value)
                  if expiry.IsSome then
                      RequestPart.multipartScalar ("expiry", "text/plain", expiry.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{accountId}/v2/images/direct_upload"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostDirectUploadLink.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PostDirectUploadLink.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostDirectUploadLink" (int status)
        }

    ///<summary>
    ///Lists all apps in the Cloudflare account
    ///</summary>
    member this.CallsAppsList(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/calls/apps" requestParts cancellationToken

            match (int status) with
            | 200 -> return CallsAppsList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CallsAppsList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CallsAppsList" (int status)
        }

    ///<summary>
    ///Creates a new Cloudflare calls app. An app is an unique enviroment where each Session can access all Tracks within the app.
    ///</summary>
    member this.CallsAppsCreateANewApp
        (accountId: string, body: calls_app_editable_fields, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/calls/apps" requestParts cancellationToken

            match (int status) with
            | 201 -> return CallsAppsCreateANewApp.Created((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CallsAppsCreateANewApp" (int status)
        }

    ///<summary>
    ///Deletes an app from Cloudflare Calls
    ///</summary>
    member this.CallsAppsDeleteApp(appId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("app_id", appId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/calls/apps/{app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CallsAppsDeleteApp.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) -> return CallsAppsDeleteApp.Status4XX(int status)
            | _ -> return failwithf "Unexpected HTTP status %d for CallsAppsDeleteApp" (int status)
        }

    ///<summary>
    ///Fetches details for a single Calls app.
    ///</summary>
    member this.CallsAppsRetrieveAppDetails(appId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("app_id", appId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/calls/apps/{app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CallsAppsRetrieveAppDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CallsAppsRetrieveAppDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CallsAppsRetrieveAppDetails" (int status)
        }

    ///<summary>
    ///Edit details for a single app.
    ///</summary>
    member this.CallsAppsUpdateAppDetails
        (appId: string, accountId: string, body: calls_app_editable_fields, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("app_id", appId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/calls/apps/{app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CallsAppsUpdateAppDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CallsAppsUpdateAppDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CallsAppsUpdateAppDetails" (int status)
        }

    ///<summary>
    ///Lists all TURN keys in the Cloudflare account
    ///</summary>
    member this.CallsTurnKeyList(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/calls/turn_keys" requestParts cancellationToken

            match (int status) with
            | 200 -> return CallsTurnKeyList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CallsTurnKeyList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CallsTurnKeyList" (int status)
        }

    ///<summary>
    ///Creates a new Cloudflare Calls TURN key.
    ///</summary>
    member this.CallsTurnKeyCreate
        (accountId: string, body: calls_turn_key_editable_fields, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/calls/turn_keys" requestParts cancellationToken

            match (int status) with
            | 201 -> return CallsTurnKeyCreate.Created((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CallsTurnKeyCreate" (int status)
        }

    ///<summary>
    ///Deletes a TURN key from Cloudflare Calls
    ///</summary>
    member this.CallsDeleteTurnKey(keyId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("key_id", keyId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/calls/turn_keys/{key_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CallsDeleteTurnKey.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) -> return CallsDeleteTurnKey.Status4XX(int status)
            | _ -> return failwithf "Unexpected HTTP status %d for CallsDeleteTurnKey" (int status)
        }

    ///<summary>
    ///Fetches details for a single TURN key.
    ///</summary>
    member this.CallsRetrieveTurnKeyDetails(keyId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("key_id", keyId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/calls/turn_keys/{key_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CallsRetrieveTurnKeyDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CallsRetrieveTurnKeyDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CallsRetrieveTurnKeyDetails" (int status)
        }

    ///<summary>
    ///Edit details for a single TURN key.
    ///</summary>
    member this.CallsUpdateTurnKey
        (keyId: string, accountId: string, body: calls_turn_key_editable_fields, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("key_id", keyId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/calls/turn_keys/{key_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CallsUpdateTurnKey.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CallsUpdateTurnKey.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CallsUpdateTurnKey" (int status)
        }

    ///<summary>
    ///List up to 100 images with one request. Use the optional parameters below to get a specific range of images.
    ///</summary>
    member this.CloudflareImagesListImages
        (accountId: string, ?page: float, ?perPage: float, ?creator: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if creator.IsSome then
                      RequestPart.query ("creator", creator.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/images/v1" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesListImages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesListImages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesListImages" (int status)
        }

    ///<summary>
    ///Upload an image to CF Images. Images up to 10 Megabytes can be uploaded using a
    ///single HTTP POST (multipart/form-data) request by sending an image file or
    ///passing a URL accessible to the API.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="url">A URL to fetch an image from origin. Only needed when type is uploading from a URL.</param>
    ///<param name="requireSignedURLs">Indicates whether the image requires a signature token for the access.</param>
    ///<param name="metadata">User modifiable key-value store. Can use used for keeping references to another system of record for managing images.</param>
    ///<param name="id">An optional custom unique identifier for your image.</param>
    ///<param name="file">An image binary data. Only needed when type is uploading a file.</param>
    ///<param name="creator">Can set the creator field with an internal user ID.</param>
    member this.CloudflareImagesUploadAnImageViaUrl
        (
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?url: string,
            ?requireSignedURLs: bool,
            ?metadata: System.Text.Json.Nodes.JsonObject,
            ?id: string,
            ?file: MultipartFile,
            ?creator: string
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields
                      [ "url"; "requireSignedURLs"; "metadata"; "id"; "file"; "creator" ]
                  RequestPart.path ("account_id", accountId)
                  if url.IsSome then
                      RequestPart.multipartScalar ("url", "text/plain", url.Value)
                  if requireSignedURLs.IsSome then
                      RequestPart.multipartScalar ("requireSignedURLs", "text/plain", requireSignedURLs.Value)
                  if metadata.IsSome then
                      RequestPart.multipartJson ("metadata", "application/json", metadata.Value)
                  if id.IsSome then
                      RequestPart.multipartScalar ("id", "text/plain", id.Value)
                  if file.IsSome then
                      RequestPart.multipartBinary ("file", "application/octet-stream", file.Value)
                  if creator.IsSome then
                      RequestPart.multipartScalar ("creator", "text/plain", creator.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/images/v1" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesUploadAnImageViaUrl.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesUploadAnImageViaUrl.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesUploadAnImageViaUrl" (int status)
        }

    ///<summary>
    ///Direct uploads allow users to upload images without API keys. A common use
    ///case are web apps, client-side applications, or mobile devices where users
    ///upload content directly to Cloudflare Images. This method creates a one-time
    ///upload URL. Use the V2 endpoint for additional features such as custom IDs and
    ///metadata.
    ///</summary>
    member this.CloudflareImagesCreateAuthenticatedDirectUploadUrlV1
        (accountId: string, ?cancellationToken: CancellationToken, ?body: images_image_direct_upload_request_v1)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/direct_upload"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesCreateAuthenticatedDirectUploadUrlV1.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareImagesCreateAuthenticatedDirectUploadUrlV1.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareImagesCreateAuthenticatedDirectUploadUrlV1"
                        (int status)
        }

    ///<summary>
    ///List your CF Images signing keys.
    ///</summary>
    member this.CloudflareImagesKeysListSigningKeys(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/images/v1/keys" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesKeysListSigningKeys.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesKeysListSigningKeys.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesKeysListSigningKeys" (int status)
        }

    ///<summary>
    ///Delete a CF Images signing key with specified name. Returns all keys available.
    ///When the last key is removed, a new default signing key will be generated.
    ///</summary>
    member this.CloudflareImagesKeysDeleteSigningKey
        (signingKeyName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("signing_key_name", signingKeyName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/keys/{signing_key_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesKeysDeleteSigningKey.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesKeysDeleteSigningKey.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesKeysDeleteSigningKey" (int status)
        }

    ///<summary>
    ///Create a new CF Images signing key with specified name. Returns all keys available.
    ///</summary>
    member this.CloudflareImagesKeysAddSigningKey
        (signingKeyName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("signing_key_name", signingKeyName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/keys/{signing_key_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesKeysAddSigningKey.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesKeysAddSigningKey.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesKeysAddSigningKey" (int status)
        }

    ///<summary>
    ///Fetch image statistics details for Cloudflare Images. The returned statistics detail storage usage, including the current image count vs this account's allowance.
    ///</summary>
    member this.CloudflareImagesImagesUsageStatistics(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/images/v1/stats" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesImagesUsageStatistics.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesImagesUsageStatistics.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesImagesUsageStatistics" (int status)
        }

    ///<summary>
    ///List existing CF Images variants.
    ///</summary>
    member this.CloudflareImagesVariantsListVariants(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/variants"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesVariantsListVariants.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesVariantsListVariants.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesVariantsListVariants" (int status)
        }

    ///<summary>
    ///Create a CF Images variant that allows you to resize images for different use cases.
    ///</summary>
    member this.CloudflareImagesVariantsCreateAVariant
        (accountId: string, body: images_image_variant_definition, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/variants"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesVariantsCreateAVariant.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesVariantsCreateAVariant.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesVariantsCreateAVariant" (int status)
        }

    ///<summary>
    ///Delete a CF Images variant. This will purge the cache for all images associated with the variant.
    ///</summary>
    member this.CloudflareImagesVariantsDeleteAVariant
        (
            variantId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("variant_id", variantId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/variants/{variant_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesVariantsDeleteAVariant.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesVariantsDeleteAVariant.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesVariantsDeleteAVariant" (int status)
        }

    ///<summary>
    ///Fetch details for a CF Images variant.
    ///</summary>
    member this.CloudflareImagesVariantsVariantDetails
        (variantId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("variant_id", variantId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/variants/{variant_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesVariantsVariantDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesVariantsVariantDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesVariantsVariantDetails" (int status)
        }

    ///<summary>
    ///Update a CF Images variant. This will purge the cache for all images associated with the variant.
    ///</summary>
    member this.CloudflareImagesVariantsUpdateAVariant
        (
            variantId: string,
            accountId: string,
            body: images_image_variant_patch_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("variant_id", variantId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/variants/{variant_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesVariantsUpdateAVariant.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesVariantsUpdateAVariant.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesVariantsUpdateAVariant" (int status)
        }

    ///<summary>
    ///Fetch details for a single variant with properties at the top level of the result.
    ///</summary>
    member this.CloudflareImagesVariantsVariantDetailsFlat
        (variantId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("variant_id", variantId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/variants/{variant_id}/flat"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesVariantsVariantDetailsFlat.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareImagesVariantsVariantDetailsFlat.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareImagesVariantsVariantDetailsFlat" (int status)
        }

    ///<summary>
    ///Delete an image on Cloudflare Images. On success, all copies of the image are deleted and purged from cache.
    ///</summary>
    member this.CloudflareImagesDeleteImage
        (
            imageId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("image_id", imageId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/{image_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesDeleteImage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesDeleteImage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesDeleteImage" (int status)
        }

    ///<summary>
    ///Fetch details for a CF Images image.
    ///</summary>
    member this.CloudflareImagesImageDetails
        (imageId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("image_id", imageId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/{image_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesImageDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesImageDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesImageDetails" (int status)
        }

    ///<summary>
    ///Update a CF Images image's metadata, creator, or access control. On access control change, all copies of the image are purged from cache.
    ///</summary>
    member this.CloudflareImagesUpdateImage
        (imageId: string, accountId: string, body: images_image_patch_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("image_id", imageId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/{image_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesUpdateImage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesUpdateImage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesUpdateImage" (int status)
        }

    ///<summary>
    ///Download an image from CF Images. For most images this will be the originally uploaded file. For larger images it can be a near-lossless version of the original.
    ///</summary>
    member this.CloudflareImagesBaseImage(imageId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("image_id", imageId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v1/{image_id}/blob"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesBaseImage.OK
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesBaseImage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesBaseImage" (int status)
        }

    ///<summary>
    ///List up to 10000 images from CF Images, with up to 1000 results per page. Use the optional parameters below to get a specific range of images.
    ///Pagination is supported via continuation_token.
    ///**Metadata Filtering (Optional):**
    ///You can optionally filter images by custom metadata fields using the `meta.&amp;lt;field&amp;gt;[&amp;lt;operator&amp;gt;]=&amp;lt;value&amp;gt;` syntax.
    ///**Supported Operators:**
    ///- `eq` / `eq:string` / `eq:number` / `eq:boolean` - Exact match
    ///- `gt` / `gt:number` - Greater than (number only)
    ///- `gte` / `gte:number` - Greater than or equal (number only)
    ///- `lt` / `lt:number` - Less than (number only)
    ///- `lte` / `lte:number` - Less than or equal (number only)
    ///- `in` / `in:string` / `in:number` - Match any value in list (pipe-separated)
    ///**Metadata Filter Constraints:**
    ///- Maximum 5 metadata filters per request
    ///- Maximum 5 levels of nesting (e.g., `meta.first.second.third.fourth.fifth`)
    ///- Maximum 10 elements for list operators (`in`)
    ///- Supports string, number, and boolean value types
    ///- Range operators (`gt`, `gte`, `lt`, `lte`) only accept numeric values
    ///**Filter Consistency:**
    ///Filters are combined with AND logic. The system does not validate whether filter combinations are logically consistent. For example, `meta.priority[eq:number]=5&amp;meta.priority[lte:number]=3` will return zero results because no value can satisfy both conditions simultaneously. It is the caller's responsibility to ensure filter combinations make sense.
    ///**Examples:**
    ///```
    ///# List all images
    ////images/v2
    ///# Filter by metadata [eq]
    ////images/v2?meta.status[eq:string]=active
    ///# Filter by metadata [in]
    ////images/v2?meta.status[in]=pending|deleted|flagged
    ///# Filter by metadata [in:number]
    ////images/v2?meta.ratings[in:number]=4|5
    ///# Filter by metadata range [gte:number]
    ////images/v2?meta.priority[gte:number]=1
    ///# Filter by bounded range
    ////images/v2?meta.priority[gte:number]=1&amp;meta.priority[lte:number]=5
    ///# Filter by nested metadata
    ////images/v2?meta.region.name[eq]=eu-west
    ///# Combine metadata filters with creator
    ////images/v2?meta.status[eq]=active&amp;creator=user123
    ///# Multiple metadata filters (AND logic)
    ////images/v2?meta.status[eq]=active&amp;meta.priority[eq:number]=5
    ///```
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="continuationToken"></param>
    ///<param name="perPage"></param>
    ///<param name="sortOrder"></param>
    ///<param name="creator"></param>
    ///<param name="meta<field><operator>">
    ///Optional metadata filter(s). Multiple filters can be combined with AND logic.
    ///**Operators:**
    ///- `eq`, `eq:string`, `eq:number`, `eq:boolean` - Exact match
    ///- `gt`, `gt:number` - Greater than (number only)
    ///- `gte`, `gte:number` - Greater than or equal (number only)
    ///- `lt`, `lt:number` - Less than (number only)
    ///- `lte`, `lte:number` - Less than or equal (number only)
    ///- `in`, `in:string`, `in:number` - Match any value in pipe-separated list
    ///**Examples:**
    ///- `meta.status[eq]=active`
    ///- `meta.priority[eq:number]=5`
    ///- `meta.enabled[eq:boolean]=true`
    ///- `meta.priority[gte:number]=1`
    ///- `meta.score[lt:number]=100`
    ///- `meta.region[in]=us-east|us-west|eu-west`
    ///**Note:** Filter consistency is not validated. Contradictory filters (e.g., `meta.priority[eq:number]=5&amp;meta.priority[lte:number]=3`) will return zero results.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.CloudflareImagesListImagesV2
        (
            accountId: string,
            ?continuationToken: string,
            ?perPage: float,
            ?sortOrder: string,
            ?creator: string,
            ?``meta<field><operator>``: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if continuationToken.IsSome then
                      RequestPart.query ("continuation_token", continuationToken.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if sortOrder.IsSome then
                      RequestPart.query ("sort_order", sortOrder.Value)
                  if creator.IsSome then
                      RequestPart.query ("creator", creator.Value)
                  if ``meta<field><operator>``.IsSome then
                      RequestPart.query ("meta.<field>[<operator>]", ``meta<field><operator>``.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/images/v2" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesListImagesV2.OK((Serializer.deserialize content))
            | 400 -> return CloudflareImagesListImagesV2.BadRequest((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesListImagesV2.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesListImagesV2" (int status)
        }

    ///<summary>
    ///Direct uploads allow users to upload images without API keys. A common use case are web apps, client-side applications, or mobile devices where users upload content directly to Cloudflare Images. This method creates a draft record for a future image. It returns an upload URL and an image identifier. To verify if the image itself has been uploaded, send an image details request (accounts/:account_identifier/images/v1/:identifier), and check that the `draft: true` property is not present.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="requireSignedURLs">Indicates whether the image requires a signature token to be accessed.</param>
    ///<param name="metadata">User modifiable key-value store. Can be used for keeping references to another system of record, for managing images.</param>
    ///<param name="id">Optional Image Custom ID. Up to 1024 chars. Can include any number of subpaths, and utf8 characters. Cannot start nor end with a / (forward slash). Cannot be a UUID.</param>
    ///<param name="expiry">The date after which the upload will not be accepted. Minimum: Now + 2 minutes. Maximum: Now + 6 hours.</param>
    ///<param name="creator">Can set the creator field with an internal user ID.</param>
    member this.CloudflareImagesCreateAuthenticatedDirectUploadUrlV2
        (
            accountId: string,
            ?cancellationToken: CancellationToken,
            ?requireSignedURLs: bool,
            ?metadata: System.Text.Json.Nodes.JsonObject,
            ?id: string,
            ?expiry: System.DateTimeOffset,
            ?creator: string
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "requireSignedURLs"; "metadata"; "id"; "expiry"; "creator" ]
                  RequestPart.path ("account_id", accountId)
                  if requireSignedURLs.IsSome then
                      RequestPart.multipartScalar ("requireSignedURLs", "text/plain", requireSignedURLs.Value)
                  if metadata.IsSome then
                      RequestPart.multipartJson ("metadata", "application/json", metadata.Value)
                  if id.IsSome then
                      RequestPart.multipartScalar ("id", "text/plain", id.Value)
                  if expiry.IsSome then
                      RequestPart.multipartScalar ("expiry", "text/plain", expiry.Value)
                  if creator.IsSome then
                      RequestPart.multipartScalar ("creator", "text/plain", creator.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/direct_upload"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesCreateAuthenticatedDirectUploadUrlV2.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareImagesCreateAuthenticatedDirectUploadUrlV2.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareImagesCreateAuthenticatedDirectUploadUrlV2"
                        (int status)
        }

    ///<summary>
    ///Lists filterable metadata keys used by images for an account.
    ///</summary>
    member this.CloudflareImagesListMetadataKeys(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/metadata/keys"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesListMetadataKeys.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesListMetadataKeys.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesListMetadataKeys" (int status)
        }

    ///<summary>
    ///List all migrations for the account.
    ///</summary>
    member this.CloudflareImagesSourcingkitListMigrations
        (accountId: string, ?offset: int, ?limit: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/migrations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitListMigrations.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesSourcingkitListMigrations.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitListMigrations" (int status)
        }

    ///<summary>
    ///Create a new migration from an existing source. The migration will import
    ///objects from the source bucket into Cloudflare Images.
    ///</summary>
    member this.CloudflareImagesSourcingkitCreateMigration
        (accountId: string, body: images_sourcingkit_migration_create_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/migrations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitCreateMigration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareImagesSourcingkitCreateMigration.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitCreateMigration" (int status)
        }

    ///<summary>
    ///Delete an existing migration. Only completed, errored, or aborted migrations can be deleted.
    ///</summary>
    member this.CloudflareImagesSourcingkitDeleteMigration
        (accountId: string, migrationId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("migration_id", migrationId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/migrations/{migration_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitDeleteMigration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareImagesSourcingkitDeleteMigration.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitDeleteMigration" (int status)
        }

    ///<summary>
    ///Fetch details for a single migration.
    ///</summary>
    member this.CloudflareImagesSourcingkitGetMigration
        (accountId: string, migrationId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("migration_id", migrationId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/migrations/{migration_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitGetMigration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesSourcingkitGetMigration.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitGetMigration" (int status)
        }

    ///<summary>
    ///Get the current progress of a migration including counts of scanned, imported,
    ///skipped, and errored objects.
    ///</summary>
    member this.CloudflareImagesSourcingkitGetMigrationProgress
        (accountId: string, migrationId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("migration_id", migrationId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/migrations/{migration_id}/lifecycle"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitGetMigrationProgress.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareImagesSourcingkitGetMigrationProgress.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareImagesSourcingkitGetMigrationProgress"
                        (int status)
        }

    ///<summary>
    ///Abort a running migration. Objects already imported will not be removed.
    ///</summary>
    member this.CloudflareImagesSourcingkitAbortMigration
        (accountId: string, migrationId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("migration_id", migrationId) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/migrations/{migration_id}/lifecycle/abort"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitAbortMigration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesSourcingkitAbortMigration.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitAbortMigration" (int status)
        }

    ///<summary>
    ///Start a pending migration. The migration will begin importing objects from the configured source.
    ///</summary>
    member this.CloudflareImagesSourcingkitStartMigration
        (accountId: string, migrationId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("migration_id", migrationId) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/migrations/{migration_id}/lifecycle/start"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitStartMigration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesSourcingkitStartMigration.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitStartMigration" (int status)
        }

    ///<summary>
    ///List log entries for a specific migration.
    ///</summary>
    member this.CloudflareImagesSourcingkitListMigrationLogs
        (accountId: string, migrationId: System.Guid, ?offset: int, ?limit: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("migration_id", migrationId)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/migrations/{migration_id}/logs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitListMigrationLogs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareImagesSourcingkitListMigrationLogs.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitListMigrationLogs" (int status)
        }

    ///<summary>
    ///List all configured migration sources for the account.
    ///</summary>
    member this.CloudflareImagesSourcingkitListSources
        (accountId: string, ?offset: int, ?limit: int, ?name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/sources"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitListSources.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesSourcingkitListSources.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitListSources" (int status)
        }

    ///<summary>
    ///Create a new migration source by providing storage credentials. The service
    ///will verify connectivity to the bucket before accepting the source.
    ///</summary>
    member this.CloudflareImagesSourcingkitCreateSource
        (accountId: string, body: images_sourcingkit_source_create_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/sources"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitCreateSource.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesSourcingkitCreateSource.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitCreateSource" (int status)
        }

    ///<summary>
    ///Verify connectivity to a storage bucket before creating a source. Returns
    ///connectivity status without persisting any state.
    ///</summary>
    member this.CloudflareImagesSourcingkitPrecheckSourceConnectivity
        (
            accountId: string,
            body: images_sourcingkit_connectivity_precheck_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/sources/connectivity-precheck"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitPrecheckSourceConnectivity.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareImagesSourcingkitPrecheckSourceConnectivity.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareImagesSourcingkitPrecheckSourceConnectivity"
                        (int status)
        }

    ///<summary>
    ///Delete an existing migration source. Sources with active migrations cannot be deleted.
    ///</summary>
    member this.CloudflareImagesSourcingkitDeleteSource
        (accountId: string, sourceId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("source_id", sourceId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/sources/{source_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitDeleteSource.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesSourcingkitDeleteSource.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitDeleteSource" (int status)
        }

    ///<summary>
    ///Fetch details for a single migration source.
    ///</summary>
    member this.CloudflareImagesSourcingkitGetSource
        (accountId: string, sourceId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("source_id", sourceId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/sources/{source_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitGetSource.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesSourcingkitGetSource.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitGetSource" (int status)
        }

    ///<summary>
    ///Update the name of an existing migration source.
    ///</summary>
    member this.CloudflareImagesSourcingkitUpdateSource
        (
            accountId: string,
            sourceId: System.Guid,
            body: images_sourcingkit_source_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("source_id", sourceId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/sources/{source_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitUpdateSource.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareImagesSourcingkitUpdateSource.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareImagesSourcingkitUpdateSource" (int status)
        }

    ///<summary>
    ///Check the current connectivity status of an existing migration source.
    ///</summary>
    member this.CloudflareImagesSourcingkitGetSourceConnectivity
        (accountId: string, sourceId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("source_id", sourceId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/images/v2/sourcingkit/sources/{source_id}/connectivity"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareImagesSourcingkitGetSourceConnectivity.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareImagesSourcingkitGetSourceConnectivity.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareImagesSourcingkitGetSourceConnectivity"
                        (int status)
        }

    ///<summary>
    ///Retrieve Media usage analytics for an account. This endpoint shares the same backend handler as the Stream usage endpoint and returns identical Stream metrics (streamMinutesViewed). The gateway rewrites this path to the shared usage handler.
    ///</summary>
    ///<param name="accountId">Standard Cloudflare hex account identifier. The API gateway translates this to an internal numeric ID before forwarding to the backend service.</param>
    ///<param name="metrics">Comma-separated list of metrics to include in the response. Available metrics depend on the endpoint. Billing usage supports: streamMinutesViewed, rateLimitingRequestsAllowed, loadBalancingQueries, argoAcceleratedBytes, workersRequests, workersKVReads, imageResizingRequests, spectrumBytesTransferred, mediaUniqueTransformations. Stream/media usage supports: streamMinutesViewed.</param>
    ///<param name="since">Start of the time range for the query (inclusive). ISO 8601 timestamp. Defaults to 6 hours before the current time.</param>
    ///<param name="until">End of the time range for the query (inclusive). ISO 8601 timestamp. Defaults to the current time.</param>
    ///<param name="timeDelta">Time unit to aggregate usage observations into. Data retention is approximately 18 months. The effective number of data points returned depends on the time range and granularity selected. For example, requesting hourly granularity over 18 months could produce up to ~13,000 data points; use the limit parameter to cap results and be aware that responses may be truncated.</param>
    ///<param name="limit">Maximum number of data points to return. The actual number of results depends on the interaction between the time range (since/until) and time_delta granularity. Results are truncated to this limit without error if the time range produces more data points than the limit allows.</param>
    ///<param name="filters">Filter expressions to apply to the query. Format: field==value. Multiple filters can be combined.</param>
    ///<param name="cancellationToken"></param>
    member this.UsageAnalyticsGetAccountMediaUsage
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
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/media/usage" requestParts cancellationToken

            match (int status) with
            | 200 -> return UsageAnalyticsGetAccountMediaUsage.OK((Serializer.deserialize content))
            | 400 -> return UsageAnalyticsGetAccountMediaUsage.BadRequest((Serializer.deserialize content))
            | 401 -> return UsageAnalyticsGetAccountMediaUsage.Unauthorized((Serializer.deserialize content))
            | 403 -> return UsageAnalyticsGetAccountMediaUsage.Forbidden((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UsageAnalyticsGetAccountMediaUsage" (int status)
        }

    ///<summary>
    ///Lists all MoQ relays for the account. Returns only metadata.
    ///Config, status, and tokens are omitted.
    ///Results are cursor-paginated (keyset on the `created` timestamp).
    ///Use `created_before` / `created_after` with the `created` value of the
    ///first/last item in a page to fetch the adjacent page. `result_info`
    ///reports the page `count` and the `total` matching the cursor filters.
    ///</summary>
    ///<param name="accountId">Cloudflare account identifier.</param>
    ///<param name="createdBefore">
    ///Cursor for pagination. Returns relays created strictly before this
    ///RFC 3339 timestamp (typically the `created` value of the first item
    ///on the current page, to fetch the previous page).
    ///</param>
    ///<param name="createdAfter">
    ///Cursor for pagination. Returns relays created strictly after this
    ///RFC 3339 timestamp (typically the `created` value of the last item
    ///on the current page, to fetch the next page).
    ///</param>
    ///<param name="perPage">
    ///Maximum number of relays to return per page. Values above the maximum are
    ///clamped to it rather than rejected.
    ///</param>
    ///<param name="asc">
    ///Sort order by `created`. When true, results are returned oldest-first
    ///(ascending); otherwise newest-first (descending, the default).
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.MoqRelaysList
        (
            accountId: string,
            ?createdBefore: System.DateTimeOffset,
            ?createdAfter: System.DateTimeOffset,
            ?perPage: int,
            ?asc: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if createdBefore.IsSome then
                      RequestPart.query ("created_before", createdBefore.Value)
                  if createdAfter.IsSome then
                      RequestPart.query ("created_after", createdAfter.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if asc.IsSome then
                      RequestPart.query ("asc", asc.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/moq/relays" requestParts cancellationToken

            match (int status) with
            | 200 -> return MoqRelaysList.OK((Serializer.deserialize content))
            | 500 -> return MoqRelaysList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MoqRelaysList" (int status)
        }

    ///<summary>
    ///Provisions a new MoQ relay instance. Auto-creates a publish+subscribe
    ///token and a subscribe-only token. Token values are included in the
    ///response (shown once). Config is always set to defaults (upstreams
    ///off) and cannot be supplied here — sending a non-empty `config` is
    ///rejected (21014); `null` or `{}` is accepted as absent. Use PUT to
    ///configure the relay after it exists.
    ///</summary>
    ///<param name="accountId">Cloudflare account identifier.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.MoqRelaysCreate
        (accountId: string, body: MoqRelaysCreatePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/moq/relays" requestParts cancellationToken

            match (int status) with
            | 201 -> return MoqRelaysCreate.Created((Serializer.deserialize content))
            | 400 -> return MoqRelaysCreate.BadRequest((Serializer.deserialize content))
            | 409 -> return MoqRelaysCreate.Conflict((Serializer.deserialize content))
            | 413 -> return MoqRelaysCreate.RequestEntityTooLarge((Serializer.deserialize content))
            | 500 -> return MoqRelaysCreate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MoqRelaysCreate" (int status)
        }

    ///<summary>
    ///Soft-deletes a MoQ relay. The relay ID goes in the URL path —
    ///`DELETE /accounts/{account_id}/moq/relays/{relay_id}` — not the
    ///request body; there is no collection-level delete endpoint.
    ///</summary>
    ///<param name="accountId">Cloudflare account identifier.</param>
    ///<param name="relayId">Relay unique identifier (32 hex characters).</param>
    ///<param name="cancellationToken"></param>
    member this.MoqRelaysDelete(accountId: string, relayId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("relay_id", relayId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/moq/relays/{relay_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MoqRelaysDelete.OK((Serializer.deserialize content))
            | 400 -> return MoqRelaysDelete.BadRequest((Serializer.deserialize content))
            | 404 -> return MoqRelaysDelete.NotFound((Serializer.deserialize content))
            | 500 -> return MoqRelaysDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MoqRelaysDelete" (int status)
        }

    ///<summary>
    ///Retrieves a single MoQ relay including config and status.
    ///Tokens are NOT included.
    ///</summary>
    ///<param name="accountId">Cloudflare account identifier.</param>
    ///<param name="relayId">Relay unique identifier (32 hex characters).</param>
    ///<param name="cancellationToken"></param>
    member this.MoqRelaysGet(accountId: string, relayId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("relay_id", relayId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/moq/relays/{relay_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MoqRelaysGet.OK((Serializer.deserialize content))
            | 400 -> return MoqRelaysGet.BadRequest((Serializer.deserialize content))
            | 404 -> return MoqRelaysGet.NotFound((Serializer.deserialize content))
            | 500 -> return MoqRelaysGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MoqRelaysGet" (int status)
        }

    ///<summary>
    ///Updates a relay's name and/or configuration. The relay ID goes in
    ///the URL path — `PUT /accounts/{account_id}/moq/relays/{relay_id}` —
    ///not the request body; there is no collection-level update endpoint.
    ///This is also the only way to set a relay's config (config cannot be
    ///set at create time). Partial updates: omitted fields are preserved;
    ///config sub-objects replace as whole objects when present.
    ///</summary>
    ///<param name="accountId">Cloudflare account identifier.</param>
    ///<param name="relayId">Relay unique identifier (32 hex characters).</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.MoqRelaysUpdate
        (accountId: string, relayId: string, body: MoqRelaysUpdatePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("relay_id", relayId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/moq/relays/{relay_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MoqRelaysUpdate.OK((Serializer.deserialize content))
            | 400 -> return MoqRelaysUpdate.BadRequest((Serializer.deserialize content))
            | 404 -> return MoqRelaysUpdate.NotFound((Serializer.deserialize content))
            | 500 -> return MoqRelaysUpdate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MoqRelaysUpdate" (int status)
        }

    ///<summary>
    ///Returns metadata for every token the relay accepts. Secrets are never
    ///returned, so a token that has been lost cannot be recovered here. There
    ///is no expiry filter: compare each token's `expires` to the current time
    ///to tell which ones have lapsed.
    ///</summary>
    ///<param name="accountId">Cloudflare account identifier.</param>
    ///<param name="relayId">Relay unique identifier (32 hex characters).</param>
    ///<param name="cancellationToken"></param>
    member this.MoqRelaysTokensList(accountId: string, relayId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("relay_id", relayId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/moq/relays/{relay_id}/tokens"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MoqRelaysTokensList.OK((Serializer.deserialize content))
            | 400 -> return MoqRelaysTokensList.BadRequest((Serializer.deserialize content))
            | 404 -> return MoqRelaysTokensList.NotFound((Serializer.deserialize content))
            | 500 -> return MoqRelaysTokensList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MoqRelaysTokensList" (int status)
        }

    ///<summary>
    ///Mints a new relay-scoped token and adds it to the relay's accepted-auth
    ///registry. The token value (secret) is shown once in the response. A relay
    ///may hold up to 10 tokens; creating an 11th is rejected.
    ///</summary>
    ///<param name="accountId">Cloudflare account identifier.</param>
    ///<param name="relayId">Relay unique identifier (32 hex characters).</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.MoqRelaysTokensCreate
        (accountId: string, relayId: string, body: MoqRelaysTokensCreatePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("relay_id", relayId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/moq/relays/{relay_id}/tokens"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return MoqRelaysTokensCreate.Created((Serializer.deserialize content))
            | 400 -> return MoqRelaysTokensCreate.BadRequest((Serializer.deserialize content))
            | 404 -> return MoqRelaysTokensCreate.NotFound((Serializer.deserialize content))
            | 409 -> return MoqRelaysTokensCreate.Conflict((Serializer.deserialize content))
            | 500 -> return MoqRelaysTokensCreate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MoqRelaysTokensCreate" (int status)
        }

    ///<summary>
    ///Revokes a token by removing it from the set the relay accepts. Relays
    ///cache that set, so revocation takes effect within seconds rather than
    ///instantly, and connections already established with the token are not
    ///closed. Revoking an unknown token succeeds, so the call is idempotent.
    ///</summary>
    ///<param name="accountId">Cloudflare account identifier.</param>
    ///<param name="relayId">Relay unique identifier (32 hex characters).</param>
    ///<param name="jti">Token identifier (jti — 32 hex characters).</param>
    ///<param name="cancellationToken"></param>
    member this.MoqRelaysTokensDelete
        (accountId: string, relayId: string, jti: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("relay_id", relayId)
                  RequestPart.path ("jti", jti) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/moq/relays/{relay_id}/tokens/{jti}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MoqRelaysTokensDelete.OK((Serializer.deserialize content))
            | 400 -> return MoqRelaysTokensDelete.BadRequest((Serializer.deserialize content))
            | 404 -> return MoqRelaysTokensDelete.NotFound((Serializer.deserialize content))
            | 500 -> return MoqRelaysTokensDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MoqRelaysTokensDelete" (int status)
        }

    ///<summary>
    ///Fetch all apps for your account
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="perPage">Number of results per page.</param>
    ///<param name="search">Search string that matches apps by name.</param>
    ///<param name="sortOrder">Sort order for apps by creation time.</param>
    ///<param name="cancellationToken"></param>
    member this.GetApps
        (
            accountId: string,
            ?pageNo: int,
            ?perPage: int,
            ?search: string,
            ?sortOrder: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if sortOrder.IsSome then
                      RequestPart.query ("sort_order", sortOrder.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/apps"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetApps.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetApps" (int status)
        }

    ///<summary>
    ///Create new app for your account
    ///</summary>
    member this.CreateApp(accountId: string, body: CreateAppPayload, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/apps"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateApp.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateApp" (int status)
        }

    ///<summary>
    ///Fetch details for an app in your account.
    ///</summary>
    member this.GetApp(accountId: string, appId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/apps/{app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetApp.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetApp" (int status)
        }

    ///<summary>
    ///Returns day-wise session and recording analytics data of an App for the specified time range start_date to end_date. If start_date and end_date are not provided, the default time range is set from 30 days ago to the current date.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="startDate">start date in YYYY-MM-DD format</param>
    ///<param name="endDate">end date in YYYY-MM-DD format</param>
    ///<param name="cancellationToken"></param>
    member this.GetOrgAnalytics
        (accountId: string, appId: string, ?startDate: string, ?endDate: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  if startDate.IsSome then
                      RequestPart.query ("start_date", startDate.Value)
                  if endDate.IsSome then
                      RequestPart.query ("end_date", endDate.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/analytics/daywise"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetOrgAnalytics.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetOrgAnalytics" (int status)
        }

    ///<summary>
    ///Returns day-wise livestream analytics for the specified time range.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="startTime">Specify the start time as a Unix timestamp in seconds to access the livestream analytics.</param>
    ///<param name="endTime">Specify the end time as a Unix timestamp in seconds to access the livestream analytics.</param>
    ///<param name="filters">Optional filters for livestream analytics.</param>
    ///<param name="cancellationToken"></param>
    member this.GetLivestreamAnalyticsDaywise
        (
            accountId: string,
            appId: string,
            ?startTime: int64,
            ?endTime: int64,
            ?filters: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  if startTime.IsSome then
                      RequestPart.query ("start_time", startTime.Value)
                  if endTime.IsSome then
                      RequestPart.query ("end_time", endTime.Value)
                  if filters.IsSome then
                      RequestPart.query ("filters", filters.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/analytics/livestreams/daywise"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetLivestreamAnalyticsDaywise.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetLivestreamAnalyticsDaywise" (int status)
        }

    ///<summary>
    ///Returns livestream analytics for the specified time range.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="startTime">Specify the start time as a Unix timestamp in seconds to access the livestream analytics.</param>
    ///<param name="endTime">Specify the end time as a Unix timestamp in seconds to access the livestream analytics.</param>
    ///<param name="filters">Optional filters for livestream analytics.</param>
    ///<param name="cancellationToken"></param>
    member this.GetLivestreamAnalyticsComplete
        (
            accountId: string,
            appId: string,
            ?startTime: int64,
            ?endTime: int64,
            ?filters: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  if startTime.IsSome then
                      RequestPart.query ("start_time", startTime.Value)
                  if endTime.IsSome then
                      RequestPart.query ("end_time", endTime.Value)
                  if filters.IsSome then
                      RequestPart.query ("filters", filters.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/analytics/livestreams/overall"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetLivestreamAnalyticsComplete.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetLivestreamAnalyticsComplete" (int status)
        }

    ///<summary>
    ///Returns details of livestreams associated with the given App ID. It includes livestreams created by your App and RealtimeKit meetings that are livestreamed by your App. If you only want details of livestreams created by your App and not RealtimeKit meetings, you can use the `exclude_meetings` query parameter.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="excludeMeetings">Exclude the RealtimeKit meetings that are livestreamed.</param>
    ///<param name="perPage">Number of results per page.</param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="status">Specifies the status of the operation.</param>
    ///<param name="startTime">Specify the start time range in ISO format to access the live stream.</param>
    ///<param name="endTime">Specify the end time range in ISO format to access the live stream.</param>
    ///<param name="sortOrder">Specifies the sorting order for the results.</param>
    ///<param name="cancellationToken"></param>
    member this.FetchAllLivestreams
        (
            accountId: string,
            appId: string,
            ?excludeMeetings: bool,
            ?perPage: int,
            ?pageNo: int,
            ?status: string,
            ?startTime: System.DateTimeOffset,
            ?endTime: System.DateTimeOffset,
            ?sortOrder: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  if excludeMeetings.IsSome then
                      RequestPart.query ("exclude_meetings", excludeMeetings.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if startTime.IsSome then
                      RequestPart.query ("start_time", startTime.Value)
                  if endTime.IsSome then
                      RequestPart.query ("end_time", endTime.Value)
                  if sortOrder.IsSome then
                      RequestPart.query ("sort_order", sortOrder.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/livestreams"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return FetchAllLivestreams.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for FetchAllLivestreams" (int status)
        }

    ///<summary>
    ///Returns livestream session details for the given livestream session ID. Retrieve the `livestream_session_id`using the `Fetch livestream session details using a session ID` API.
    ///</summary>
    member this.GetV2LivestreamsLivestreamSessionId
        (accountId: string, appId: string, livestreamSessionId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("livestream-session-id", livestreamSessionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/livestreams/sessions/{livestream-session-id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetV2LivestreamsLivestreamSessionId.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetV2LivestreamsLivestreamSessionId" (int status)
        }

    ///<summary>
    ///Returns details of a livestream with sessions for the given livestream ID. Retreive the livestream ID using the `Start livestreaming a meeting` API.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="livestreamId"></param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="perPage">Number of results per page.</param>
    ///<param name="cancellationToken"></param>
    member this.GetV2LivestreamSessionLivestreamId
        (
            accountId: string,
            appId: string,
            livestreamId: string,
            ?pageNo: int,
            ?perPage: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("livestream_id", livestreamId)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/livestreams/{livestream_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetV2LivestreamSessionLivestreamId.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetV2LivestreamSessionLivestreamId" (int status)
        }

    ///<summary>
    ///Returns details of all active livestreams for the given livestream ID. Retreive the livestream ID using the `Start livestreaming a meeting` API.
    ///</summary>
    member this.GetV2ActiveLivestreamSessionDetails
        (accountId: string, appId: string, livestreamId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("livestream_id", livestreamId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/livestreams/{livestream_id}/active-livestream-session"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetV2ActiveLivestreamSessionDetails.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetV2ActiveLivestreamSessionDetails" (int status)
        }

    ///<summary>
    ///Returns all meetings for the given App ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="perPage">Number of results per page</param>
    ///<param name="startTime">The start time range for which you want to retrieve the meetings. The time must be specified in ISO format.</param>
    ///<param name="endTime">The end time range for which you want to retrieve the meetings. The time must be specified in ISO format.</param>
    ///<param name="search">The search query string. You can search using the meeting ID or title.</param>
    ///<param name="status">Filter meetings by status.</param>
    ///<param name="cancellationToken"></param>
    member this.GetAllMeetings
        (
            accountId: string,
            appId: string,
            ?pageNo: float,
            ?perPage: float,
            ?startTime: System.DateTimeOffset,
            ?endTime: System.DateTimeOffset,
            ?search: string,
            ?status: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if startTime.IsSome then
                      RequestPart.query ("start_time", startTime.Value)
                  if endTime.IsSome then
                      RequestPart.query ("end_time", endTime.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetAllMeetings.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetAllMeetings" (int status)
        }

    ///<summary>
    ///Create a meeting for the given App ID.
    ///</summary>
    member this.CreateMeeting
        (accountId: string, appId: string, body: CreateMeetingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return CreateMeeting.Created((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateMeeting" (int status)
        }

    ///<summary>
    ///Returns a meeting details in an App for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting. Fetch the meeting ID using the create a meeting API.</param>
    ///<param name="name"></param>
    ///<param name="cancellationToken"></param>
    member this.GetMeeting
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            ?name: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetMeeting.OK((Serializer.deserialize content))
            | 500 -> return GetMeeting.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetMeeting" (int status)
        }

    ///<summary>
    ///Updates a meeting in an App for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting. Fetch the meeting ID using the create a meeting API.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateMeeting
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            body: UpdateMeetingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateMeeting.OK((Serializer.deserialize content))
            | 500 -> return UpdateMeeting.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateMeeting" (int status)
        }

    ///<summary>
    ///Replaces all the details for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting. Fetch the meeting ID using the create a meeting API.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReplaceMeeting
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            body: ReplaceMeetingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ReplaceMeeting.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ReplaceMeeting" (int status)
        }

    ///<summary>
    ///Returns details of all active livestreams for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting</param>
    ///<param name="cancellationToken"></param>
    member this.GetV2MeetingsMeetingIdActiveLivestream
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/active-livestream"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetV2MeetingsMeetingIdActiveLivestream.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetV2MeetingsMeetingIdActiveLivestream" (int status)
        }

    ///<summary>
    ///Stops the active livestream of a meeting associated with the given meeting ID. Retreive the meeting ID using the `Create a meeting` API.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting</param>
    ///<param name="cancellationToken"></param>
    member this.StopLivestreaming
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/active-livestream/stop"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StopLivestreaming.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StopLivestreaming" (int status)
        }

    ///<summary>
    ///Returns details of an ongoing active session for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting</param>
    ///<param name="cancellationToken"></param>
    member this.GetActiveSession
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/active-session"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetActiveSession.OK((Serializer.deserialize content))
            | 404 -> return GetActiveSession.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetActiveSession" (int status)
        }

    ///<summary>
    ///Kicks one or more participants from an active session using user ID or custom participant ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">ID of the meeting</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.KickPartcipants
        (
            accountId: string,
            appId: string,
            meetingId: string,
            body: InlineUnion_1bc503c8b61073036826d896,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/active-session/kick"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return KickPartcipants.OK((Serializer.deserialize content))
            | 404 -> return KickPartcipants.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for KickPartcipants" (int status)
        }

    ///<summary>
    ///Kicks all participants from an active session for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">ID of the meeting</param>
    ///<param name="cancellationToken"></param>
    member this.KickAllParticipants
        (accountId: string, appId: string, meetingId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/active-session/kick-all"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return KickAllParticipants.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for KickAllParticipants" (int status)
        }

    ///<summary>
    ///Mutes one or more participants from an active session using user ID or custom participant ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">ID of the meeting</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.MuteParticipants
        (
            accountId: string,
            appId: string,
            meetingId: string,
            body: InlineUnion_52d65436919945cab7d1682a,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/active-session/mute"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MuteParticipants.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MuteParticipants" (int status)
        }

    ///<summary>
    ///Mutes all participants of an active session for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">ID of the meeting</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.MuteAllParticipants
        (
            accountId: string,
            appId: string,
            meetingId: string,
            body: MuteAllParticipantsPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/active-session/mute-all"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MuteAllParticipants.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MuteAllParticipants" (int status)
        }

    ///<summary>
    ///Creates a new poll in an active session for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">ID of the meeting</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreatePoll
        (
            accountId: string,
            appId: string,
            meetingId: string,
            body: CreatePollPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/active-session/poll"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return CreatePoll.Created((Serializer.deserialize content))
            | 400 -> return CreatePoll.BadRequest
            | _ -> return failwithf "Unexpected HTTP status %d for CreatePoll" (int status)
        }

    ///<summary>
    ///Returns livestream session details for the given meeting ID. Retreive the meeting ID using the `Create a meeting` API.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting</param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="perPage">Number of results per page.</param>
    ///<param name="cancellationToken"></param>
    member this.LivestreamSessionDetails
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            ?pageNo: int,
            ?perPage: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/livestream"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LivestreamSessionDetails.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LivestreamSessionDetails" (int status)
        }

    ///<summary>
    ///Starts livestream of a meeting associated with the given meeting ID. Retreive the meeting ID using the `Create a meeting` API.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.StartLivestreaming
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            body: StartLivestreamingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/livestreams"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return StartLivestreaming.Created((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StartLivestreaming" (int status)
        }

    ///<summary>
    ///Returns all participants detail for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">ID of the meeting. Fetch the meeting ID using the create a meeting API.</param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="perPage">Number of results per page</param>
    ///<param name="cancellationToken"></param>
    member this.GetMeetingParticipants
        (
            accountId: string,
            appId: string,
            meetingId: System.Guid,
            ?pageNo: float,
            ?perPage: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/participants"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetMeetingParticipants.OK((Serializer.deserialize content))
            | 500 -> return GetMeetingParticipants.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetMeetingParticipants" (int status)
        }

    ///<summary>
    ///Adds a participant to the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">ID of the meeting. Fetch the meeting ID using the create a meeting API.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.AddParticipant
        (
            accountId: string,
            appId: string,
            meetingId: System.Guid,
            body: AddParticipantPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/participants"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return AddParticipant.Created((Serializer.deserialize content))
            | 500 -> return AddParticipant.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AddParticipant" (int status)
        }

    ///<summary>
    ///Deletes a participant for the given meeting and participant ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting. You can fetch the meeting ID using the create a meeting API.</param>
    ///<param name="participantId">ID of the participant. You can fetch the participant ID using the add a participant API.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteMeetingParticipant
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            participantId: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath)
                  RequestPart.path ("participant_id", participantId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/participants/{participant_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteMeetingParticipant.OK((Serializer.deserialize content))
            | 500 -> return DeleteMeetingParticipant.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteMeetingParticipant" (int status)
        }

    ///<summary>
    ///Returns a participant details for the given meeting and participant ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting. You can fetch the meeting ID using the create a meeting API.</param>
    ///<param name="participantId">ID of the participant. You can fetch the participant ID using the add a participant API.</param>
    ///<param name="cancellationToken"></param>
    member this.GetMeetingParticipant
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            participantId: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath)
                  RequestPart.path ("participant_id", participantId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/participants/{participant_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetMeetingParticipant.OK((Serializer.deserialize content))
            | 500 -> return GetMeetingParticipant.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetMeetingParticipant" (int status)
        }

    ///<summary>
    ///Updates a participant's details for the given meeting and participant ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting. You can fetch the meeting ID using the create a meeting API.</param>
    ///<param name="participantId">ID of the participant. You can fetch the participant ID using the add a participant API.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.EditParticipant
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            participantId: string,
            body: EditParticipantPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath)
                  RequestPart.path ("participant_id", participantId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/participants/{participant_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EditParticipant.OK((Serializer.deserialize content))
            | 500 -> return EditParticipant.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EditParticipant" (int status)
        }

    ///<summary>
    ///Replaces a participant's details for the given meeting and participant ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">The unique identifier for the meeting.</param>
    ///<param name="meetingIdInPath">ID of the meeting. You can fetch the meeting ID using the create a meeting API.</param>
    ///<param name="participantId">ID of the participant. You can fetch the participant ID using the add a participant API.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReplaceParticipant
        (
            accountId: string,
            appId: string,
            meetingId: string,
            meetingIdInPath: System.Guid,
            participantId: string,
            body: ReplaceParticipantPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("meeting_id", meetingIdInPath)
                  RequestPart.path ("participant_id", participantId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/participants/{participant_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ReplaceParticipant.OK((Serializer.deserialize content))
            | 500 -> return ReplaceParticipant.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ReplaceParticipant" (int status)
        }

    ///<summary>
    ///Regenerates participant's authentication token for the given meeting and participant ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">ID of the meeting. You can fetch the meeting ID using the create a meeting API.</param>
    ///<param name="participantId">ID of the participant. You can fetch the participant ID using the add a  participant API.</param>
    ///<param name="cancellationToken"></param>
    member this.RegenerateToken
        (
            accountId: string,
            appId: string,
            meetingId: System.Guid,
            participantId: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId)
                  RequestPart.path ("participant_id", participantId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/meetings/{meeting_id}/participants/{participant_id}/token"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RegenerateToken.OK((Serializer.deserialize content))
            | 500 -> return RegenerateToken.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RegenerateToken" (int status)
        }

    ///<summary>
    ///Fetches all the presets belonging to an App.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="perPage">Number of results per page</param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="search">Search presets by name.</param>
    ///<param name="cancellationToken"></param>
    member this.GetPresets
        (
            accountId: string,
            appId: string,
            ?perPage: float,
            ?pageNo: float,
            ?search: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/presets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPresets.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPresets" (int status)
        }

    ///<summary>
    ///Creates a preset belonging to the current App
    ///</summary>
    member this.PostPresets
        (accountId: string, appId: string, body: realtimekit_Preset, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/presets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return PostPresets.Created((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostPresets" (int status)
        }

    ///<summary>
    ///Deletes a preset using the provided preset ID
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="presetId">ID of the preset to fetch</param>
    ///<param name="cancellationToken"></param>
    member this.DeletePresetsPresetId
        (accountId: string, appId: string, presetId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("preset_id", presetId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/presets/{preset_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeletePresetsPresetId.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeletePresetsPresetId" (int status)
        }

    ///<summary>
    ///Fetches details of a preset using the provided preset ID
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="presetId">ID of the preset to fetch</param>
    ///<param name="cancellationToken"></param>
    member this.GetPresetsPresetId
        (accountId: string, appId: string, presetId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("preset_id", presetId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/presets/{preset_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPresetsPresetId.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPresetsPresetId" (int status)
        }

    ///<summary>
    ///Update a preset by the provided preset ID
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="presetId">ID of the preset to fetch</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.PatchPresetsPresetId
        (
            accountId: string,
            appId: string,
            presetId: System.Guid,
            body: realtimekit_UpdatePreset,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("preset_id", presetId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/presets/{preset_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PatchPresetsPresetId.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PatchPresetsPresetId" (int status)
        }

    ///<summary>
    ///Replace all details for the preset using the provided preset ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="presetId">ID of the preset to replace</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.PutPresetsPresetId
        (
            accountId: string,
            appId: string,
            presetId: System.Guid,
            body: realtimekit_Preset,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("preset_id", presetId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/presets/{preset_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PutPresetsPresetId.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PutPresetsPresetId" (int status)
        }

    ///<summary>
    ///Returns all recordings for an App. If the `meeting_id` parameter is passed, returns all recordings for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">ID of a meeting. Optional. Will limit results to only this meeting if passed.</param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="perPage">Number of results per page</param>
    ///<param name="expired">If passed, only shows expired/non-expired recordings on RealtimeKit's bucket</param>
    ///<param name="search">The search query string. You can search using the meeting ID or title.</param>
    ///<param name="sortBy"></param>
    ///<param name="sortOrder"></param>
    ///<param name="startTime">The start time range for which you want to retrieve the meetings. The time must be specified in ISO format.</param>
    ///<param name="endTime">The end time range for which you want to retrieve the meetings. The time must be specified in ISO format.</param>
    ///<param name="status">Filter by one or more recording status</param>
    ///<param name="cancellationToken"></param>
    member this.GetAllRecordings
        (
            accountId: string,
            appId: string,
            ?meetingId: System.Guid,
            ?pageNo: float,
            ?perPage: float,
            ?expired: bool,
            ?search: string,
            ?sortBy: string,
            ?sortOrder: string,
            ?startTime: System.DateTimeOffset,
            ?endTime: System.DateTimeOffset,
            ?status: list<string>,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  if meetingId.IsSome then
                      RequestPart.query ("meeting_id", meetingId.Value)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if expired.IsSome then
                      RequestPart.query ("expired", expired.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if sortBy.IsSome then
                      RequestPart.query ("sort_by", sortBy.Value)
                  if sortOrder.IsSome then
                      RequestPart.query ("sort_order", sortOrder.Value)
                  if startTime.IsSome then
                      RequestPart.query ("start_time", startTime.Value)
                  if endTime.IsSome then
                      RequestPart.query ("end_time", endTime.Value)
                  if status.IsSome then
                      RequestPart.queryComma ("status", status.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/recordings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetAllRecordings.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetAllRecordings" (int status)
        }

    ///<summary>
    ///Starts recording a meeting. The meeting can be started by an App admin directly, or a participant with permissions to start a recording, based on the type of authorization used.
    ///</summary>
    member this.StartRecording
        (accountId: string, appId: string, body: StartRecordingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/recordings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StartRecording.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StartRecording" (int status)
        }

    ///<summary>
    ///Returns the active recording details for the given meeting ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="meetingId">ID of the meeting</param>
    ///<param name="cancellationToken"></param>
    member this.GetActiveRecording
        (accountId: string, appId: string, meetingId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("meeting_id", meetingId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/recordings/active-recording/{meeting_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetActiveRecording.OK((Serializer.deserialize content))
            | 404 -> return GetActiveRecording.NotFound((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetActiveRecording" (int status)
        }

    ///<summary>
    ///Starts track recording for a meeting. Track recording currently records separate participant audio tracks as WebM files in the RealtimeKit bucket. Video track recording is in development. For more information, refer to [Track recording](/realtime/realtimekit/recording-guide/track-recording/).
    ///</summary>
    member this.StartTrackRecordingForAMeeting
        (
            accountId: string,
            appId: string,
            body: StartTrackRecordingForAMeetingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/recordings/track"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StartTrackRecordingForAMeeting.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StartTrackRecordingForAMeeting" (int status)
        }

    ///<summary>
    ///Returns details of a recording for the given recording ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="recordingId">ID of the recording</param>
    ///<param name="cancellationToken"></param>
    member this.GetOneRecording
        (accountId: string, appId: string, recordingId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("recording_id", recordingId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/recordings/{recording_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetOneRecording.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetOneRecording" (int status)
        }

    ///<summary>
    ///Pause/Resume/Stop a given recording ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId">A Cloudflare-generated unique identifier for an item.</param>
    ///<param name="recordingId">ID of the recording</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.PauseResumeStopRecording
        (
            accountId: string,
            appId: string,
            recordingId: System.Guid,
            body: PauseResumeStopRecordingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("recording_id", recordingId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/recordings/{recording_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PauseResumeStopRecording.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PauseResumeStopRecording" (int status)
        }

    ///<summary>
    ///Returns details of all sessions of an App.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="perPage">Number of results per page</param>
    ///<param name="sortBy"></param>
    ///<param name="sortOrder"></param>
    ///<param name="startTime">The start time range for which you want to retrieve the meetings. The time must be specified in ISO format.</param>
    ///<param name="endTime">The end time range for which you want to retrieve the meetings. The time must be specified in ISO format.</param>
    ///<param name="participants"></param>
    ///<param name="status"></param>
    ///<param name="search">Search string that matches sessions based on meeting title, meeting ID, and session ID</param>
    ///<param name="associatedId">ID of the meeting that sessions should be associated with</param>
    ///<param name="cancellationToken"></param>
    member this.GetSessions
        (
            accountId: string,
            appId: string,
            ?pageNo: float,
            ?perPage: float,
            ?sortBy: string,
            ?sortOrder: string,
            ?startTime: System.DateTimeOffset,
            ?endTime: System.DateTimeOffset,
            ?participants: string,
            ?status: string,
            ?search: string,
            ?associatedId: System.Guid,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if sortBy.IsSome then
                      RequestPart.query ("sort_by", sortBy.Value)
                  if sortOrder.IsSome then
                      RequestPart.query ("sort_order", sortOrder.Value)
                  if startTime.IsSome then
                      RequestPart.query ("start_time", startTime.Value)
                  if endTime.IsSome then
                      RequestPart.query ("end_time", endTime.Value)
                  if participants.IsSome then
                      RequestPart.query ("participants", participants.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if associatedId.IsSome then
                      RequestPart.query ("associated_id", associatedId.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/sessions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetSessions.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetSessions" (int status)
        }

    ///<summary>
    ///Returns participant details for the given peer ID along with call statistics.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="peerId">ID of the peer</param>
    ///<param name="filters">Filter to apply to the peer report.</param>
    ///<param name="includePeerEvents">if true, response includes all the peer events of participant.</param>
    ///<param name="cancellationToken"></param>
    member this.GetParticipantDataFromPeerId
        (
            accountId: string,
            appId: string,
            peerId: System.Guid,
            ?filters: string,
            ?includePeerEvents: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("peer_id", peerId)
                  if filters.IsSome then
                      RequestPart.query ("filters", filters.Value)
                  if includePeerEvents.IsSome then
                      RequestPart.query ("include_peer_events", includePeerEvents.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/sessions/peer-report/{peer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetParticipantDataFromPeerId.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetParticipantDataFromPeerId" (int status)
        }

    ///<summary>
    ///Returns data of the given session ID including recording details.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="sessionId">ID of the session</param>
    ///<param name="includeBreakoutRooms">List all breakout rooms</param>
    ///<param name="cancellationToken"></param>
    member this.GetSessionDetails
        (
            accountId: string,
            appId: string,
            sessionId: System.Guid,
            ?includeBreakoutRooms: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("session_id", sessionId)
                  if includeBreakoutRooms.IsSome then
                      RequestPart.query ("include_breakout_rooms", includeBreakoutRooms.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/sessions/{session_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetSessionDetails.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetSessionDetails" (int status)
        }

    ///<summary>
    ///Returns a URL to download all chat messages of the session ID in CSV format.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="sessionId">ID of the session</param>
    ///<param name="cancellationToken"></param>
    member this.GetSessionChat
        (accountId: string, appId: string, sessionId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("session_id", sessionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/sessions/{session_id}/chat"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetSessionChat.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetSessionChat" (int status)
        }

    ///<summary>
    ///Returns livestream session details for the given session ID. Retreive the session ID using the `Fetch all sessions of an App` API.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="sessionId"></param>
    ///<param name="perPage">Number of results per page.</param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="cancellationToken"></param>
    member this.GetV2LivestreamsessionSessionMeetingIdActiveLivestream
        (
            accountId: string,
            appId: string,
            sessionId: System.Guid,
            ?perPage: float,
            ?pageNo: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("session_id", sessionId)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/sessions/{session_id}/livestream-sessions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetV2LivestreamsessionSessionMeetingIdActiveLivestream.OK((Serializer.deserialize content))
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for GetV2LivestreamsessionSessionMeetingIdActiveLivestream"
                        (int status)
        }

    ///<summary>
    ///Returns a list of participants for the given session ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="sessionId">ID of the session</param>
    ///<param name="search">The search query string. You can search using participant ID, custom participant ID, or display name.</param>
    ///<param name="pageNo">The page number from which you want your page search results to be displayed.</param>
    ///<param name="perPage">Number of results per page</param>
    ///<param name="sortOrder"></param>
    ///<param name="sortBy"></param>
    ///<param name="includePeerEvents">if true, response includes all the peer events of participants.</param>
    ///<param name="view">In breakout room sessions, the view parameter can be set to `raw` for session specific duration for participants or `consolidated` to accumulate breakout room durations.</param>
    ///<param name="cancellationToken"></param>
    member this.GetSessionParticipants
        (
            accountId: string,
            appId: string,
            sessionId: System.Guid,
            ?search: string,
            ?pageNo: float,
            ?perPage: float,
            ?sortOrder: string,
            ?sortBy: string,
            ?includePeerEvents: bool,
            ?view: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("session_id", sessionId)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if pageNo.IsSome then
                      RequestPart.query ("page_no", pageNo.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if sortOrder.IsSome then
                      RequestPart.query ("sort_order", sortOrder.Value)
                  if sortBy.IsSome then
                      RequestPart.query ("sort_by", sortBy.Value)
                  if includePeerEvents.IsSome then
                      RequestPart.query ("include_peer_events", includePeerEvents.Value)
                  if view.IsSome then
                      RequestPart.query ("view", view.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/sessions/{session_id}/participants"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetSessionParticipants.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetSessionParticipants" (int status)
        }

    ///<summary>
    ///Returns details of the given participant ID along with call statistics for the given session ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="participantId">ID of the participant</param>
    ///<param name="sessionId">ID of the session</param>
    ///<param name="includePeerEvents">if true, response includes all the peer events of participant.</param>
    ///<param name="cancellationToken"></param>
    member this.GetParticipantDetails
        (
            accountId: string,
            appId: string,
            participantId: System.Guid,
            sessionId: System.Guid,
            ?includePeerEvents: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("participant_id", participantId)
                  RequestPart.path ("session_id", sessionId)
                  if includePeerEvents.IsSome then
                      RequestPart.query ("include_peer_events", includePeerEvents.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/sessions/{session_id}/participants/{participant_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetParticipantDetails.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetParticipantDetails" (int status)
        }

    ///<summary>
    ///Returns a Summary URL to download the Summary of Transcripts for the session ID as plain text.
    ///</summary>
    member this.GetSessionSummary
        (accountId: string, appId: string, sessionId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("session_id", sessionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/sessions/{session_id}/summary"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetSessionSummary.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetSessionSummary" (int status)
        }

    ///<summary>
    ///Trigger Summary generation of Transcripts for the session ID.
    ///</summary>
    member this.PostSessionsSessionIdSummary
        (accountId: string, appId: string, sessionId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("session_id", sessionId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/sessions/{session_id}/summary"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PostSessionsSessionIdSummary.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PostSessionsSessionIdSummary" (int status)
        }

    ///<summary>
    ///Returns a URL to download the transcript for the session ID in CSV format.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="sessionId">ID of the session</param>
    ///<param name="format">Transcript file format to fetch.</param>
    ///<param name="cancellationToken"></param>
    member this.GetSessionTranscript
        (
            accountId: string,
            appId: string,
            sessionId: System.Guid,
            ?format: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("session_id", sessionId)
                  if format.IsSome then
                      RequestPart.query ("format", format.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/sessions/{session_id}/transcript"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetSessionTranscript.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetSessionTranscript" (int status)
        }

    ///<summary>
    ///Returns details of all webhooks for an App.
    ///</summary>
    member this.GetAllWebhooks(accountId: string, appId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/webhooks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetAllWebhooks.OK((Serializer.deserialize content))
            | 401 -> return GetAllWebhooks.Unauthorized
            | _ -> return failwithf "Unexpected HTTP status %d for GetAllWebhooks" (int status)
        }

    ///<summary>
    ///Adds a new webhook to an App.
    ///</summary>
    member this.AddWebhook
        (accountId: string, appId: string, body: realtimekit_WebhookRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/webhooks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return AddWebhook.Created((Serializer.deserialize content))
            | 400 -> return AddWebhook.BadRequest((Serializer.deserialize content))
            | 401 -> return AddWebhook.Unauthorized
            | _ -> return failwithf "Unexpected HTTP status %d for AddWebhook" (int status)
        }

    ///<summary>
    ///Returns the list of webhook event names supported by RealtimeKit.
    ///</summary>
    member this.GetAllWebhookEvents(accountId: string, appId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/webhooks/all"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetAllWebhookEvents.OK((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetAllWebhookEvents" (int status)
        }

    ///<summary>
    ///Removes a webhook for the given webhook ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="webhookId">ID of the webhook</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteWebhook
        (accountId: string, appId: string, webhookId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("webhook_id", webhookId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/webhooks/{webhook_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteDeleteWebhook.OK((Serializer.deserialize content))
            | 400 -> return DeleteDeleteWebhook.BadRequest((Serializer.deserialize content))
            | 401 -> return DeleteDeleteWebhook.Unauthorized
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteWebhook" (int status)
        }

    ///<summary>
    ///Returns webhook details for the given webhook ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="webhookId">ID of the webhook</param>
    ///<param name="cancellationToken"></param>
    member this.GetWebhook
        (accountId: string, appId: string, webhookId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("webhook_id", webhookId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/webhooks/{webhook_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetWebhook.OK((Serializer.deserialize content))
            | 400 -> return GetWebhook.BadRequest((Serializer.deserialize content))
            | 401 -> return GetWebhook.Unauthorized
            | _ -> return failwithf "Unexpected HTTP status %d for GetWebhook" (int status)
        }

    ///<summary>
    ///Edits the webhook details for the given webhook ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="webhookId">ID of the webhook</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.EditWebhook
        (
            accountId: string,
            appId: string,
            webhookId: System.Guid,
            body: realtimekit_PatchWebhookRequest,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("webhook_id", webhookId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/webhooks/{webhook_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EditWebhook.OK((Serializer.deserialize content))
            | 400 -> return EditWebhook.BadRequest((Serializer.deserialize content))
            | 401 -> return EditWebhook.Unauthorized
            | _ -> return failwithf "Unexpected HTTP status %d for EditWebhook" (int status)
        }

    ///<summary>
    ///Replace all details for the given webhook ID.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="appId"></param>
    ///<param name="webhookId">ID of the webhook</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReplaceWebhook
        (
            accountId: string,
            appId: string,
            webhookId: System.Guid,
            body: realtimekit_WebhookRequest,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("app_id", appId)
                  RequestPart.path ("webhook_id", webhookId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/realtime/kit/{app_id}/webhooks/{webhook_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ReplaceWebhook.OK((Serializer.deserialize content))
            | 400 -> return ReplaceWebhook.BadRequest((Serializer.deserialize content))
            | 401 -> return ReplaceWebhook.Unauthorized
            | _ -> return failwithf "Unexpected HTTP status %d for ReplaceWebhook" (int status)
        }

    ///<summary>
    ///Lists up to 1000 videos from a single request. For a specific range, refer to the optional parameters.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="status"></param>
    ///<param name="creator"></param>
    ///<param name="type"></param>
    ///<param name="asc"></param>
    ///<param name="videoName"></param>
    ///<param name="search"></param>
    ///<param name="start"></param>
    ///<param name="end"></param>
    ///<param name="includeCounts"></param>
    ///<param name="id">Filter by video ID(s). Can be a single ID or a comma-separated list of IDs.</param>
    ///<param name="name">Filter by video name/UID(s). Can be a single name or a comma-separated list.</param>
    ///<param name="liveInputId">Filter by live input ID to find videos associated with a specific live stream.</param>
    ///<param name="before">Alias for 'end'. Returns videos created before this date/time (RFC 3339 format).</param>
    ///<param name="after">Alias for 'start'. Returns videos created after this date/time (RFC 3339 format).</param>
    ///<param name="limit">Maximum number of videos to return (default 1000, max 1000).</param>
    ///<param name="cancellationToken"></param>
    member this.StreamVideosListVideos
        (
            accountId: string,
            ?status: string,
            ?creator: string,
            ?``type``: string,
            ?asc: bool,
            ?videoName: string,
            ?search: string,
            ?start: System.DateTimeOffset,
            ?``end``: System.DateTimeOffset,
            ?includeCounts: bool,
            ?id: string,
            ?name: string,
            ?liveInputId: string,
            ?before: System.DateTimeOffset,
            ?after: System.DateTimeOffset,
            ?limit: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if creator.IsSome then
                      RequestPart.query ("creator", creator.Value)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value)
                  if asc.IsSome then
                      RequestPart.query ("asc", asc.Value)
                  if videoName.IsSome then
                      RequestPart.query ("video_name", videoName.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if start.IsSome then
                      RequestPart.query ("start", start.Value)
                  if ``end``.IsSome then
                      RequestPart.query ("end", ``end``.Value)
                  if includeCounts.IsSome then
                      RequestPart.query ("include_counts", includeCounts.Value)
                  if id.IsSome then
                      RequestPart.query ("id", id.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if liveInputId.IsSome then
                      RequestPart.query ("live_input_id", liveInputId.Value)
                  if before.IsSome then
                      RequestPart.query ("before", before.Value)
                  if after.IsSome then
                      RequestPart.query ("after", after.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/stream" requestParts cancellationToken

            match (int status) with
            | 200 -> return StreamVideosListVideos.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamVideosListVideos.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamVideosListVideos" (int status)
        }

    ///<summary>
    ///Initiates a video upload using the TUS protocol. On success, the server responds with a status code 201 (created) and includes a `location` header to indicate where the content should be uploaded. Refer to https://tus.io for protocol details.
    ///</summary>
    member this.StreamVideosInitiateVideoUploadsUsingTus
        (
            tusResumable: string,
            uploadLength: int,
            accountId: string,
            ?uploadCreator: string,
            ?uploadMetadata: string,
            ?directUser: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.header ("Tus-Resumable", tusResumable)
                  RequestPart.header ("Upload-Length", uploadLength)
                  RequestPart.path ("account_id", accountId)
                  if uploadCreator.IsSome then
                      RequestPart.header ("Upload-Creator", uploadCreator.Value)
                  if uploadMetadata.IsSome then
                      RequestPart.header ("Upload-Metadata", uploadMetadata.Value)
                  if directUser.IsSome then
                      RequestPart.query ("direct_user", directUser.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/stream" requestParts cancellationToken

            match (int status) with
            | 201 -> return StreamVideosInitiateVideoUploadsUsingTus.Created
            | _ when (((int status) / 100) = 4) ->
                return StreamVideosInitiateVideoUploadsUsingTus.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for StreamVideosInitiateVideoUploadsUsingTus" (int status)
        }

    ///<summary>
    ///Clips a video based on the specified start and end times provided in seconds.
    ///</summary>
    member this.StreamVideoClippingClipVideosGivenAStartAndEndTime
        (accountId: string, body: stream_videoClipStandard, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/stream/clip" requestParts cancellationToken

            match (int status) with
            | 200 -> return StreamVideoClippingClipVideosGivenAStartAndEndTime.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamVideoClippingClipVideosGivenAStartAndEndTime.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for StreamVideoClippingClipVideosGivenAStartAndEndTime"
                        (int status)
        }

    ///<summary>
    ///Uploads a video to Stream from a provided URL.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body">Copy upload request. Provide `input` (preferred) or `url` (deprecated).</param>
    ///<param name="uploadCreator"></param>
    ///<param name="cancellationToken"></param>
    member this.StreamVideosUploadVideosFromAUrl
        (
            accountId: string,
            body: stream_video_copy_request,
            ?uploadCreator: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if uploadCreator.IsSome then
                      RequestPart.header ("Upload-Creator", uploadCreator.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/stream/copy" requestParts cancellationToken

            match (int status) with
            | 200 -> return StreamVideosUploadVideosFromAUrl.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamVideosUploadVideosFromAUrl.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamVideosUploadVideosFromAUrl" (int status)
        }

    ///<summary>
    ///Creates a direct upload that allows video uploads without an API key.
    ///</summary>
    member this.StreamVideosUploadVideosViaDirectUploadUrLs
        (
            accountId: string,
            body: stream_direct_upload_request,
            ?uploadCreator: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if uploadCreator.IsSome then
                      RequestPart.header ("Upload-Creator", uploadCreator.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/direct_upload"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamVideosUploadVideosViaDirectUploadUrLs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamVideosUploadVideosViaDirectUploadUrLs.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for StreamVideosUploadVideosViaDirectUploadUrLs" (int status)
        }

    ///<summary>
    ///Lists the video ID and creation date and time when a signing key was created.
    ///</summary>
    member this.StreamSigningKeysListSigningKeys(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/stream/keys" requestParts cancellationToken

            match (int status) with
            | 200 -> return StreamSigningKeysListSigningKeys.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamSigningKeysListSigningKeys.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamSigningKeysListSigningKeys" (int status)
        }

    ///<summary>
    ///Creates an RSA private key in PEM and JWK formats. Key files are only displayed once after creation. Keys are created, used, and deleted independently of videos, and every key can sign any video.
    ///</summary>
    member this.StreamSigningKeysCreateSigningKeys(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/stream/keys" requestParts cancellationToken

            match (int status) with
            | 200 -> return StreamSigningKeysCreateSigningKeys.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamSigningKeysCreateSigningKeys.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamSigningKeysCreateSigningKeys" (int status)
        }

    ///<summary>
    ///Deletes signing keys and revokes all signed URLs generated with the key.
    ///</summary>
    member this.StreamSigningKeysDeleteSigningKeys
        (identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/stream/keys/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamSigningKeysDeleteSigningKeys.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamSigningKeysDeleteSigningKeys.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamSigningKeysDeleteSigningKeys" (int status)
        }

    ///<summary>
    ///Lists the live inputs created for an account. To get the credentials needed to stream to a specific live input, request a single live input.
    ///</summary>
    member this.StreamLiveInputsListLiveInputs
        (accountId: string, ?includeCounts: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if includeCounts.IsSome then
                      RequestPart.query ("include_counts", includeCounts.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamLiveInputsListLiveInputs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamLiveInputsListLiveInputs.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamLiveInputsListLiveInputs" (int status)
        }

    ///<summary>
    ///Creates a live input, and returns credentials that you or your users can use to stream live video to Cloudflare Stream.
    ///</summary>
    member this.StreamLiveInputsCreateALiveInput
        (accountId: string, body: stream_create_input_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamLiveInputsCreateALiveInput.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamLiveInputsCreateALiveInput.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamLiveInputsCreateALiveInput" (int status)
        }

    ///<summary>
    ///Prevents a live input from being streamed to and makes the live input inaccessible to any future API calls.
    ///</summary>
    member this.StreamLiveInputsDeleteALiveInput
        (liveInputIdentifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("live_input_identifier", liveInputIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs/{live_input_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamLiveInputsDeleteALiveInput.OK
            | _ when (((int status) / 100) = 4) -> return StreamLiveInputsDeleteALiveInput.Status4XX(int status)
            | _ -> return failwithf "Unexpected HTTP status %d for StreamLiveInputsDeleteALiveInput" (int status)
        }

    ///<summary>
    ///Retrieves details of an existing live input.
    ///</summary>
    member this.StreamLiveInputsRetrieveALiveInput
        (liveInputIdentifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("live_input_identifier", liveInputIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs/{live_input_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamLiveInputsRetrieveALiveInput.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamLiveInputsRetrieveALiveInput.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamLiveInputsRetrieveALiveInput" (int status)
        }

    ///<summary>
    ///Updates a specified live input.
    ///</summary>
    member this.StreamLiveInputsUpdateALiveInput
        (
            liveInputIdentifier: string,
            accountId: string,
            body: stream_update_input_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("live_input_identifier", liveInputIdentifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs/{live_input_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamLiveInputsUpdateALiveInput.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamLiveInputsUpdateALiveInput.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamLiveInputsUpdateALiveInput" (int status)
        }

    ///<summary>
    ///Prevents a live input from being streamed to and makes the live input inaccessible to any future API calls until enabled.
    ///</summary>
    member this.StreamLiveInputsDisableALiveInput
        (liveInputIdentifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("live_input_identifier", liveInputIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs/{live_input_identifier}/disable"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamLiveInputsDisableALiveInput.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamLiveInputsDisableALiveInput.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamLiveInputsDisableALiveInput" (int status)
        }

    ///<summary>
    ///Allows a live input to be streamed to and makes the live input accessible to any future API calls.
    ///</summary>
    member this.StreamLiveInputsEnableALiveInput
        (liveInputIdentifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("live_input_identifier", liveInputIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs/{live_input_identifier}/enable"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamLiveInputsEnableALiveInput.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamLiveInputsEnableALiveInput.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamLiveInputsEnableALiveInput" (int status)
        }

    ///<summary>
    ///Retrieves all outputs associated with a specified live input.
    ///</summary>
    member this.StreamLiveInputsListAllOutputsAssociatedWithASpecifiedLiveInput
        (liveInputIdentifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("live_input_identifier", liveInputIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs/{live_input_identifier}/outputs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    StreamLiveInputsListAllOutputsAssociatedWithASpecifiedLiveInput.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamLiveInputsListAllOutputsAssociatedWithASpecifiedLiveInput.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for StreamLiveInputsListAllOutputsAssociatedWithASpecifiedLiveInput"
                        (int status)
        }

    ///<summary>
    ///Creates a new output that can be used to simulcast or restream live video to other RTMP or SRT destinations. Outputs are always linked to a specific live input — one live input can have many outputs.
    ///</summary>
    member this.``StreamLiveInputsCreateANewOutput,ConnectedToALiveInput``
        (
            liveInputIdentifier: string,
            accountId: string,
            body: stream_create_output_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("live_input_identifier", liveInputIdentifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs/{live_input_identifier}/outputs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return ``StreamLiveInputsCreateANewOutput,ConnectedToALiveInput``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``StreamLiveInputsCreateANewOutput,ConnectedToALiveInput``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for StreamLiveInputsCreateANewOutput,ConnectedToALiveInput"
                        (int status)
        }

    ///<summary>
    ///Deletes an output and removes it from the associated live input.
    ///</summary>
    member this.StreamLiveInputsDeleteAnOutput
        (outputIdentifier: string, liveInputIdentifier: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("output_identifier", outputIdentifier)
                  RequestPart.path ("live_input_identifier", liveInputIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs/{live_input_identifier}/outputs/{output_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamLiveInputsDeleteAnOutput.OK
            | _ when (((int status) / 100) = 4) -> return StreamLiveInputsDeleteAnOutput.Status4XX(int status)
            | _ -> return failwithf "Unexpected HTTP status %d for StreamLiveInputsDeleteAnOutput" (int status)
        }

    ///<summary>
    ///Updates the state of an output.
    ///</summary>
    member this.StreamLiveInputsUpdateAnOutput
        (
            outputIdentifier: string,
            liveInputIdentifier: string,
            accountId: string,
            body: stream_update_output_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("output_identifier", outputIdentifier)
                  RequestPart.path ("live_input_identifier", liveInputIdentifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs/{live_input_identifier}/outputs/{output_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamLiveInputsUpdateAnOutput.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamLiveInputsUpdateAnOutput.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamLiveInputsUpdateAnOutput" (int status)
        }

    ///<summary>
    ///Rotates the credentials for a live input without changing its identifier. Old credentials are revoked, broadcasts using stale credentials are automatically disconnected shortly after rotation, and the response returns refreshed credentials.
    ///</summary>
    member this.StreamLiveInputsRotateKeysForALiveInput
        (liveInputIdentifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("live_input_identifier", liveInputIdentifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/live_inputs/{live_input_identifier}/rotate_keys"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamLiveInputsRotateKeysForALiveInput.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamLiveInputsRotateKeysForALiveInput.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamLiveInputsRotateKeysForALiveInput" (int status)
        }

    ///<summary>
    ///Returns information about an account's storage use.
    ///</summary>
    member this.StreamVideosStorageUsage(accountId: string, ?creator: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if creator.IsSome then
                      RequestPart.query ("creator", creator.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/storage-usage"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamVideosStorageUsage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamVideosStorageUsage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamVideosStorageUsage" (int status)
        }

    ///<summary>
    ///Retrieve Stream usage analytics for an account. Returns time-series data for Stream billable minutes viewed across all zones in the account. The gateway rewrites this path before forwarding to the backend usage handler.
    ///</summary>
    ///<param name="accountId">Standard Cloudflare hex account identifier. The API gateway translates this to an internal numeric ID before forwarding to the backend service.</param>
    ///<param name="metrics">Comma-separated list of metrics to include in the response. Available metrics depend on the endpoint. Billing usage supports: streamMinutesViewed, rateLimitingRequestsAllowed, loadBalancingQueries, argoAcceleratedBytes, workersRequests, workersKVReads, imageResizingRequests, spectrumBytesTransferred, mediaUniqueTransformations. Stream/media usage supports: streamMinutesViewed.</param>
    ///<param name="since">Start of the time range for the query (inclusive). ISO 8601 timestamp. Defaults to 6 hours before the current time.</param>
    ///<param name="until">End of the time range for the query (inclusive). ISO 8601 timestamp. Defaults to the current time.</param>
    ///<param name="timeDelta">Time unit to aggregate usage observations into. Data retention is approximately 18 months. The effective number of data points returned depends on the time range and granularity selected. For example, requesting hourly granularity over 18 months could produce up to ~13,000 data points; use the limit parameter to cap results and be aware that responses may be truncated.</param>
    ///<param name="limit">Maximum number of data points to return. The actual number of results depends on the interaction between the time range (since/until) and time_delta granularity. Results are truncated to this limit without error if the time range produces more data points than the limit allows.</param>
    ///<param name="filters">Filter expressions to apply to the query. Format: field==value. Multiple filters can be combined.</param>
    ///<param name="cancellationToken"></param>
    member this.UsageAnalyticsGetAccountStreamUsage
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
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/stream/usage" requestParts cancellationToken

            match (int status) with
            | 200 -> return UsageAnalyticsGetAccountStreamUsage.OK((Serializer.deserialize content))
            | 400 -> return UsageAnalyticsGetAccountStreamUsage.BadRequest((Serializer.deserialize content))
            | 401 -> return UsageAnalyticsGetAccountStreamUsage.Unauthorized((Serializer.deserialize content))
            | 403 -> return UsageAnalyticsGetAccountStreamUsage.Forbidden((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UsageAnalyticsGetAccountStreamUsage" (int status)
        }

    ///<summary>
    ///Lists all watermark profiles for an account.
    ///</summary>
    member this.StreamWatermarkProfileListWatermarkProfiles(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/watermarks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamWatermarkProfileListWatermarkProfiles.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamWatermarkProfileListWatermarkProfiles.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for StreamWatermarkProfileListWatermarkProfiles" (int status)
        }

    ///<summary>
    ///Creates watermark profiles using a single `HTTP POST multipart/form-data` request.
    ///</summary>
    member this.StreamWatermarkProfileCreateWatermarkProfilesViaBasicUpload
        (
            accountId: string,
            body: StreamWatermarkProfileCreateWatermarkProfilesViaBasicUploadPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/watermarks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return StreamWatermarkProfileCreateWatermarkProfilesViaBasicUpload.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamWatermarkProfileCreateWatermarkProfilesViaBasicUpload.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for StreamWatermarkProfileCreateWatermarkProfilesViaBasicUpload"
                        (int status)
        }

    ///<summary>
    ///Deletes a watermark profile.
    ///</summary>
    member this.StreamWatermarkProfileDeleteWatermarkProfiles
        (identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/stream/watermarks/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamWatermarkProfileDeleteWatermarkProfiles.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamWatermarkProfileDeleteWatermarkProfiles.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for StreamWatermarkProfileDeleteWatermarkProfiles" (int status)
        }

    ///<summary>
    ///Retrieves details for a single watermark profile.
    ///</summary>
    member this.StreamWatermarkProfileWatermarkProfileDetails
        (identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/watermarks/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamWatermarkProfileWatermarkProfileDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamWatermarkProfileWatermarkProfileDetails.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for StreamWatermarkProfileWatermarkProfileDetails" (int status)
        }

    ///<summary>
    ///Deletes a webhook.
    ///</summary>
    member this.StreamWebhookDeleteWebhooks(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/stream/webhook"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamWebhookDeleteWebhooks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamWebhookDeleteWebhooks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamWebhookDeleteWebhooks" (int status)
        }

    ///<summary>
    ///Retrieves a list of webhooks.
    ///</summary>
    member this.StreamWebhookViewWebhooks(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/stream/webhook" requestParts cancellationToken

            match (int status) with
            | 200 -> return StreamWebhookViewWebhooks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamWebhookViewWebhooks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamWebhookViewWebhooks" (int status)
        }

    ///<summary>
    ///Creates a webhook notification.
    ///</summary>
    member this.StreamWebhookCreateWebhooks
        (accountId: string, body: stream_webhook_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/stream/webhook" requestParts cancellationToken

            match (int status) with
            | 200 -> return StreamWebhookCreateWebhooks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamWebhookCreateWebhooks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamWebhookCreateWebhooks" (int status)
        }

    ///<summary>
    ///Deletes a video and its copies from Cloudflare Stream.
    ///</summary>
    member this.StreamVideosDeleteVideo(identifier: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamVideosDeleteVideo.OK
            | _ when (((int status) / 100) = 4) ->
                return StreamVideosDeleteVideo.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamVideosDeleteVideo" (int status)
        }

    ///<summary>
    ///Fetches details for a single video.
    ///</summary>
    member this.StreamVideosRetrieveVideoDetails
        (identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamVideosRetrieveVideoDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamVideosRetrieveVideoDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamVideosRetrieveVideoDetails" (int status)
        }

    ///<summary>
    ///Edit details for a single video.
    ///</summary>
    member this.StreamVideosUpdateVideoDetails
        (identifier: string, accountId: string, body: stream_video_update, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamVideosUpdateVideoDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamVideosUpdateVideoDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamVideosUpdateVideoDetails" (int status)
        }

    ///<summary>
    ///Lists additional audio tracks on a video. Note this API will not return information for audio attached to the video upload.
    ///</summary>
    member this.ListAudioTracks(accountId: string, identifier: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("identifier", identifier) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/audio"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListAudioTracks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListAudioTracks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListAudioTracks" (int status)
        }

    ///<summary>
    ///Adds an additional audio track to a video using the provided audio track URL.
    ///</summary>
    member this.AddAudioTrack
        (accountId: string, identifier: string, body: stream_copyAudioTrack, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/audio/copy"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AddAudioTrack.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AddAudioTrack.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AddAudioTrack" (int status)
        }

    ///<summary>
    ///Deletes additional audio tracks on a video. Deleting a default audio track is not allowed. You must assign another audio track as default prior to deletion.
    ///</summary>
    member this.DeleteAudioTracks
        (accountId: string, identifier: string, audioIdentifier: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.path ("audio_identifier", audioIdentifier) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/audio/{audio_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteAudioTracks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteAudioTracks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteAudioTracks" (int status)
        }

    ///<summary>
    ///Edits additional audio tracks on a video. Editing the default status of an audio track to `true` will mark all other audio tracks on the video default status to `false`.
    ///</summary>
    member this.EditAudioTracks
        (
            accountId: string,
            identifier: string,
            audioIdentifier: string,
            body: stream_editAudioTrack,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.path ("audio_identifier", audioIdentifier)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/audio/{audio_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EditAudioTracks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return EditAudioTracks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EditAudioTracks" (int status)
        }

    ///<summary>
    ///Lists the available captions or subtitles for a specific video.
    ///</summary>
    member this.StreamSubtitlesCaptionsListCaptionsOrSubtitles
        (identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/captions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamSubtitlesCaptionsListCaptionsOrSubtitles.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamSubtitlesCaptionsListCaptionsOrSubtitles.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for StreamSubtitlesCaptionsListCaptionsOrSubtitles"
                        (int status)
        }

    ///<summary>
    ///Removes the captions or subtitles from a video.
    ///</summary>
    member this.StreamSubtitlesCaptionsDeleteCaptionsOrSubtitles
        (language: string, identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("language", language)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/captions/{language}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamSubtitlesCaptionsDeleteCaptionsOrSubtitles.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamSubtitlesCaptionsDeleteCaptionsOrSubtitles.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for StreamSubtitlesCaptionsDeleteCaptionsOrSubtitles"
                        (int status)
        }

    ///<summary>
    ///Lists the captions or subtitles for provided language.
    ///</summary>
    member this.StreamSubtitlesCaptionsGetCaptionOrSubtitleForLanguage
        (language: string, identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("language", language)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/captions/{language}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamSubtitlesCaptionsGetCaptionOrSubtitleForLanguage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamSubtitlesCaptionsGetCaptionOrSubtitleForLanguage.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for StreamSubtitlesCaptionsGetCaptionOrSubtitleForLanguage"
                        (int status)
        }

    ///<summary>
    ///Uploads the caption or subtitle file to the endpoint for a specific BCP47 language. One caption or subtitle file per language is allowed.
    ///</summary>
    ///<param name="language"></param>
    ///<param name="identifier"></param>
    ///<param name="accountId"></param>
    ///<param name="file">The WebVTT file containing the caption or subtitle content.</param>
    ///<param name="cancellationToken"></param>
    member this.StreamSubtitlesCaptionsUploadCaptionsOrSubtitles
        (language: string, identifier: string, accountId: string, file: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "file" ]
                  RequestPart.path ("language", language)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.multipartScalar ("file", "text/plain", file) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/captions/{language}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamSubtitlesCaptionsUploadCaptionsOrSubtitles.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamSubtitlesCaptionsUploadCaptionsOrSubtitles.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for StreamSubtitlesCaptionsUploadCaptionsOrSubtitles"
                        (int status)
        }

    ///<summary>
    ///Generate captions or subtitles for provided language via AI.
    ///</summary>
    member this.StreamSubtitlesCaptionsGenerateCaptionOrSubtitleForLanguage
        (language: string, identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("language", language)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/captions/{language}/generate"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return StreamSubtitlesCaptionsGenerateCaptionOrSubtitleForLanguage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamSubtitlesCaptionsGenerateCaptionOrSubtitleForLanguage.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for StreamSubtitlesCaptionsGenerateCaptionOrSubtitleForLanguage"
                        (int status)
        }

    ///<summary>
    ///Return WebVTT captions for a provided language.
    ///</summary>
    member this.StreamSubtitlesCaptionsGetVttCaptionOrSubtitle
        (language: string, identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("language", language)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/captions/{language}/vtt"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamSubtitlesCaptionsGetVttCaptionOrSubtitle.OK
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamSubtitlesCaptionsGetVttCaptionOrSubtitle.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for StreamSubtitlesCaptionsGetVttCaptionOrSubtitle"
                        (int status)
        }

    ///<summary>
    ///Delete the downloads for a video. Use `/downloads/{download_type}` instead for type-specific downloads. Available types are `default` and `audio`.
    ///</summary>
    member this.StreamMP4DownloadsDeleteDownloads
        (identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/downloads"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamMP4DownloadsDeleteDownloads.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamMP4DownloadsDeleteDownloads.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamMP4DownloadsDeleteDownloads" (int status)
        }

    ///<summary>
    ///Lists the downloads created for a video.
    ///</summary>
    member this.StreamMP4DownloadsListDownloads
        (identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/downloads"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamMP4DownloadsListDownloads.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamMP4DownloadsListDownloads.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamMP4DownloadsListDownloads" (int status)
        }

    ///<summary>
    ///Creates a download for a video when a video is ready to view. Use `/downloads/{download_type}` instead for type-specific downloads. Available types are `default` and `audio`.
    ///</summary>
    member this.StreamMP4DownloadsCreateDownloads
        (identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/downloads"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamMP4DownloadsCreateDownloads.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return StreamMP4DownloadsCreateDownloads.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamMP4DownloadsCreateDownloads" (int status)
        }

    ///<summary>
    ///Delete specific type of download. For backwards-compatibility, DELETE requests to /downloads will delete the default download.
    ///</summary>
    member this.StreamDownloadsDeleteTypeSpecificDownloads
        (identifier: string, accountId: string, downloadType: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("download_type", downloadType) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/downloads/{download_type}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamDownloadsDeleteTypeSpecificDownloads.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamDownloadsDeleteTypeSpecificDownloads.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for StreamDownloadsDeleteTypeSpecificDownloads" (int status)
        }

    ///<summary>
    ///Creates a download for a video of specified type. For backwards-compatibility, POST requests to /downloads will enable the default download.
    ///</summary>
    member this.StreamDownloadsCreateTypeSpecificDownloads
        (identifier: string, accountId: string, downloadType: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("download_type", downloadType) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/downloads/{download_type}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamDownloadsCreateTypeSpecificDownloads.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamDownloadsCreateTypeSpecificDownloads.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for StreamDownloadsCreateTypeSpecificDownloads" (int status)
        }

    ///<summary>
    ///Fetches an HTML code snippet to embed a video in a web page delivered through Cloudflare. On success, returns an HTML fragment for use on web pages to display a video. On failure, returns a JSON response body.
    ///</summary>
    member this.StreamVideosRetreieveEmbedCodeHtml
        (identifier: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/embed"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamVideosRetreieveEmbedCodeHtml.OK
            | _ when (((int status) / 100) = 4) ->
                return StreamVideosRetreieveEmbedCodeHtml.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StreamVideosRetreieveEmbedCodeHtml" (int status)
        }

    ///<summary>
    ///Creates a signed URL token for a video. If a body is not provided in the request, a token is created with default values.
    ///</summary>
    member this.StreamVideosCreateSignedUrlTokensForVideos
        (identifier: string, accountId: string, body: stream_signed_token_request, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/stream/{identifier}/token"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StreamVideosCreateSignedUrlTokensForVideos.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    StreamVideosCreateSignedUrlTokensForVideos.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for StreamVideosCreateSignedUrlTokensForVideos" (int status)
        }

    ///<summary>
    ///Retrieve Media usage analytics for a zone. This endpoint shares the same backend handler as the Stream usage endpoint and returns identical Stream metrics (streamMinutesViewed). The gateway resolves the zone to its owning account and rewrites this path to the shared usage handler.
    ///</summary>
    ///<param name="zoneId">Standard Cloudflare hex zone identifier. The API gateway resolves this to the owning account and translates it to an internal numeric ID before forwarding to the backend service.</param>
    ///<param name="metrics">Comma-separated list of metrics to include in the response. Available metrics depend on the endpoint. Billing usage supports: streamMinutesViewed, rateLimitingRequestsAllowed, loadBalancingQueries, argoAcceleratedBytes, workersRequests, workersKVReads, imageResizingRequests, spectrumBytesTransferred, mediaUniqueTransformations. Stream/media usage supports: streamMinutesViewed.</param>
    ///<param name="since">Start of the time range for the query (inclusive). ISO 8601 timestamp. Defaults to 6 hours before the current time.</param>
    ///<param name="until">End of the time range for the query (inclusive). ISO 8601 timestamp. Defaults to the current time.</param>
    ///<param name="timeDelta">Time unit to aggregate usage observations into. Data retention is approximately 18 months. The effective number of data points returned depends on the time range and granularity selected. For example, requesting hourly granularity over 18 months could produce up to ~13,000 data points; use the limit parameter to cap results and be aware that responses may be truncated.</param>
    ///<param name="limit">Maximum number of data points to return. The actual number of results depends on the interaction between the time range (since/until) and time_delta granularity. Results are truncated to this limit without error if the time range produces more data points than the limit allows.</param>
    ///<param name="filters">Filter expressions to apply to the query. Format: field==value. Multiple filters can be combined.</param>
    ///<param name="cancellationToken"></param>
    member this.UsageAnalyticsGetZoneMediaUsage
        (
            zoneId: string,
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
                [ RequestPart.path ("zone_id", zoneId)
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
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/media/usage" requestParts cancellationToken

            match (int status) with
            | 200 -> return UsageAnalyticsGetZoneMediaUsage.OK((Serializer.deserialize content))
            | 400 -> return UsageAnalyticsGetZoneMediaUsage.BadRequest((Serializer.deserialize content))
            | 401 -> return UsageAnalyticsGetZoneMediaUsage.Unauthorized((Serializer.deserialize content))
            | 403 -> return UsageAnalyticsGetZoneMediaUsage.Forbidden((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UsageAnalyticsGetZoneMediaUsage" (int status)
        }

    ///<summary>
    ///Retrieve Stream usage analytics for a zone. Returns time-series data for Stream billable minutes viewed. The gateway resolves the zone to its owning account and rewrites this path before forwarding to the backend usage handler.
    ///</summary>
    ///<param name="zoneId">Standard Cloudflare hex zone identifier. The API gateway resolves this to the owning account and translates it to an internal numeric ID before forwarding to the backend service.</param>
    ///<param name="metrics">Comma-separated list of metrics to include in the response. Available metrics depend on the endpoint. Billing usage supports: streamMinutesViewed, rateLimitingRequestsAllowed, loadBalancingQueries, argoAcceleratedBytes, workersRequests, workersKVReads, imageResizingRequests, spectrumBytesTransferred, mediaUniqueTransformations. Stream/media usage supports: streamMinutesViewed.</param>
    ///<param name="since">Start of the time range for the query (inclusive). ISO 8601 timestamp. Defaults to 6 hours before the current time.</param>
    ///<param name="until">End of the time range for the query (inclusive). ISO 8601 timestamp. Defaults to the current time.</param>
    ///<param name="timeDelta">Time unit to aggregate usage observations into. Data retention is approximately 18 months. The effective number of data points returned depends on the time range and granularity selected. For example, requesting hourly granularity over 18 months could produce up to ~13,000 data points; use the limit parameter to cap results and be aware that responses may be truncated.</param>
    ///<param name="limit">Maximum number of data points to return. The actual number of results depends on the interaction between the time range (since/until) and time_delta granularity. Results are truncated to this limit without error if the time range produces more data points than the limit allows.</param>
    ///<param name="filters">Filter expressions to apply to the query. Format: field==value. Multiple filters can be combined.</param>
    ///<param name="cancellationToken"></param>
    member this.UsageAnalyticsGetZoneStreamUsage
        (
            zoneId: string,
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
                [ RequestPart.path ("zone_id", zoneId)
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
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/stream/usage" requestParts cancellationToken

            match (int status) with
            | 200 -> return UsageAnalyticsGetZoneStreamUsage.OK((Serializer.deserialize content))
            | 400 -> return UsageAnalyticsGetZoneStreamUsage.BadRequest((Serializer.deserialize content))
            | 401 -> return UsageAnalyticsGetZoneStreamUsage.Unauthorized((Serializer.deserialize content))
            | 403 -> return UsageAnalyticsGetZoneStreamUsage.Forbidden((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UsageAnalyticsGetZoneStreamUsage" (int status)
        }
