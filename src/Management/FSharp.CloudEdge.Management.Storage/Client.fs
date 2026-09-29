namespace rec FSharp.CloudEdge.Management.Storage

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
type StorageClient(httpClient: HttpClient) =
    ///<summary>
    ///Lists Artifacts namespaces for an account.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="limit"></param>
    ///<param name="cursor"></param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsNamespacesList
        (accountId: string, ?limit: int, ?cursor: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsNamespacesList.OK((Serializer.deserialize content))
            | 400 -> return ArtifactsNamespacesList.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsNamespacesList.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsNamespacesList.Forbidden((Serializer.deserialize content))
            | 500 -> return ArtifactsNamespacesList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsNamespacesList" (int status)
        }

    ///<summary>
    ///Returns an Artifacts namespace summary.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsNamespacesGet
        (accountId: string, ``namespace``: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsNamespacesGet.OK((Serializer.deserialize content))
            | 400 -> return ArtifactsNamespacesGet.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsNamespacesGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsNamespacesGet.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsNamespacesGet.NotFound((Serializer.deserialize content))
            | 500 -> return ArtifactsNamespacesGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsNamespacesGet" (int status)
        }

    ///<summary>
    ///Lists repositories in a namespace.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="limit"></param>
    ///<param name="cursor"></param>
    ///<param name="search"></param>
    ///<param name="sort"></param>
    ///<param name="direction"></param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposList
        (
            accountId: string,
            ``namespace``: string,
            ?limit: int,
            ?cursor: string,
            ?search: string,
            ?sort: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if sort.IsSome then
                      RequestPart.query ("sort", sort.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsReposList.OK((Serializer.deserialize content))
            | 400 -> return ArtifactsReposList.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsReposList.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsReposList.Forbidden((Serializer.deserialize content))
            | 500 -> return ArtifactsReposList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposList" (int status)
        }

    ///<summary>
    ///Creates a Git-compatible Artifacts repository in a namespace.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposCreate
        (
            accountId: string,
            ``namespace``: string,
            body: ArtifactsReposCreatePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return ArtifactsReposCreate.Created((Serializer.deserialize content))
            | 400 -> return ArtifactsReposCreate.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsReposCreate.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsReposCreate.Forbidden((Serializer.deserialize content))
            | 409 -> return ArtifactsReposCreate.Conflict((Serializer.deserialize content))
            | 500 -> return ArtifactsReposCreate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposCreate" (int status)
        }

    ///<summary>
    ///Deletes a repository and schedules cleanup of its backing data.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposDelete
        (accountId: string, ``namespace``: string, name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 202 -> return ArtifactsReposDelete.Accepted((Serializer.deserialize content))
            | 400 -> return ArtifactsReposDelete.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsReposDelete.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsReposDelete.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsReposDelete.NotFound((Serializer.deserialize content))
            | 500 -> return ArtifactsReposDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposDelete" (int status)
        }

    ///<summary>
    ///Returns repository metadata.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposGet
        (accountId: string, ``namespace``: string, name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsReposGet.OK((Serializer.deserialize content))
            | 400 -> return ArtifactsReposGet.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsReposGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsReposGet.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsReposGet.NotFound((Serializer.deserialize content))
            | 409 -> return ArtifactsReposGet.Conflict((Serializer.deserialize content))
            | 500 -> return ArtifactsReposGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposGet" (int status)
        }

    ///<summary>
    ///Returns raw bytes for an immutable Git blob object. Blob responses are cacheable forever by hash.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="hash">40-character lowercase hexadecimal Git object hash.</param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposBlobGet
        (accountId: string, ``namespace``: string, name: string, hash: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name)
                  RequestPart.path ("hash", hash) ]

            let! (status, _, contentBinary) =
                OpenApiHttp.getBinaryAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}/blob/{hash}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsReposBlobGet.OK(contentBinary)
            | 400 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposBlobGet.BadRequest((Serializer.deserialize content))
            | 401 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposBlobGet.Unauthorized((Serializer.deserialize content))
            | 403 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposBlobGet.Forbidden((Serializer.deserialize content))
            | 404 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposBlobGet.NotFound((Serializer.deserialize content))
            | 413 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposBlobGet.RequestEntityTooLarge((Serializer.deserialize content))
            | 500 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposBlobGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposBlobGet" (int status)
        }

    ///<summary>
    ///Returns decoded metadata for an immutable Git commit object. Commit responses are cacheable forever by hash.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="hash">40-character lowercase hexadecimal Git object hash.</param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposCommitGet
        (accountId: string, ``namespace``: string, name: string, hash: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name)
                  RequestPart.path ("hash", hash) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}/commit/{hash}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsReposCommitGet.OK((Serializer.deserialize content))
            | 400 -> return ArtifactsReposCommitGet.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsReposCommitGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsReposCommitGet.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsReposCommitGet.NotFound((Serializer.deserialize content))
            | 500 -> return ArtifactsReposCommitGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposCommitGet" (int status)
        }

    ///<summary>
    ///Returns raw bytes for a file resolved by ref and path.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="ref">Git ref, branch, tag, or commit hash.</param>
    ///<param name="path">File path.</param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposFileGet
        (
            accountId: string,
            ``namespace``: string,
            name: string,
            ref: string,
            path: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name)
                  RequestPart.query ("ref", ref)
                  RequestPart.query ("path", path) ]

            let! (status, _, contentBinary) =
                OpenApiHttp.getBinaryAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}/file"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsReposFileGet.OK(contentBinary)
            | 400 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposFileGet.BadRequest((Serializer.deserialize content))
            | 401 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposFileGet.Unauthorized((Serializer.deserialize content))
            | 403 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposFileGet.Forbidden((Serializer.deserialize content))
            | 404 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposFileGet.NotFound((Serializer.deserialize content))
            | 413 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposFileGet.RequestEntityTooLarge((Serializer.deserialize content))
            | 500 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ArtifactsReposFileGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposFileGet" (int status)
        }

    ///<summary>
    ///Forks a source repository into a new repository.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposFork
        (
            accountId: string,
            ``namespace``: string,
            name: string,
            body: ArtifactsReposForkPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}/fork"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return ArtifactsReposFork.Created((Serializer.deserialize content))
            | 400 -> return ArtifactsReposFork.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsReposFork.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsReposFork.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsReposFork.NotFound((Serializer.deserialize content))
            | 409 -> return ArtifactsReposFork.Conflict((Serializer.deserialize content))
            | 500 -> return ArtifactsReposFork.InternalServerError((Serializer.deserialize content))
            | 503 -> return ArtifactsReposFork.ServiceUnavailable((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposFork" (int status)
        }

    ///<summary>
    ///Imports an HTTPS Git repository into an Artifacts repository.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposImport
        (
            accountId: string,
            ``namespace``: string,
            name: string,
            body: ArtifactsReposImportPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}/import"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return ArtifactsReposImport.Created((Serializer.deserialize content))
            | 400 -> return ArtifactsReposImport.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsReposImport.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsReposImport.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsReposImport.NotFound((Serializer.deserialize content))
            | 409 -> return ArtifactsReposImport.Conflict((Serializer.deserialize content))
            | 413 -> return ArtifactsReposImport.RequestEntityTooLarge((Serializer.deserialize content))
            | 422 -> return ArtifactsReposImport.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return ArtifactsReposImport.InternalServerError((Serializer.deserialize content))
            | 502 -> return ArtifactsReposImport.BadGateway((Serializer.deserialize content))
            | 503 -> return ArtifactsReposImport.ServiceUnavailable((Serializer.deserialize content))
            | 504 -> return ArtifactsReposImport.GatewayTimeout((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposImport" (int status)
        }

    ///<summary>
    ///Returns commit metadata walking backwards from a ref, branch, tag, or HEAD.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="ref">Git ref, branch, tag, or commit hash. Defaults to HEAD.</param>
    ///<param name="limit"></param>
    ///<param name="offset"></param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposLogGet
        (
            accountId: string,
            ``namespace``: string,
            name: string,
            ?ref: string,
            ?limit: int,
            ?offset: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name)
                  if ref.IsSome then
                      RequestPart.query ("ref", ref.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}/log"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsReposLogGet.OK((Serializer.deserialize content))
            | 400 -> return ArtifactsReposLogGet.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsReposLogGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsReposLogGet.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsReposLogGet.NotFound((Serializer.deserialize content))
            | 500 -> return ArtifactsReposLogGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposLogGet" (int status)
        }

    ///<summary>
    ///Returns file bytes resolved by ref and path, with a sniffed content type and browser-safe response headers.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="ref">Git ref, branch, tag, or 40-character commit hash.</param>
    ///<param name="path">File path. May contain slashes.</param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposRawGet
        (
            accountId: string,
            ``namespace``: string,
            name: string,
            ref: string,
            path: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name)
                  RequestPart.path ("ref", ref)
                  RequestPart.path ("path", path) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}/raw/{ref}/{path}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsReposRawGet.OK((Serializer.deserialize content))
            | 400 -> return ArtifactsReposRawGet.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsReposRawGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsReposRawGet.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsReposRawGet.NotFound((Serializer.deserialize content))
            | 413 -> return ArtifactsReposRawGet.RequestEntityTooLarge((Serializer.deserialize content))
            | 500 -> return ArtifactsReposRawGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposRawGet" (int status)
        }

    ///<summary>
    ///Lists tokens for a repository.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="state"></param>
    ///<param name="page"></param>
    ///<param name="perPage"></param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsRepoTokensList
        (
            accountId: string,
            ``namespace``: string,
            name: string,
            ?state: string,
            ?page: int,
            ?perPage: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name)
                  if state.IsSome then
                      RequestPart.query ("state", state.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}/tokens"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsRepoTokensList.OK((Serializer.deserialize content))
            | 400 -> return ArtifactsRepoTokensList.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsRepoTokensList.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsRepoTokensList.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsRepoTokensList.NotFound((Serializer.deserialize content))
            | 409 -> return ArtifactsRepoTokensList.Conflict((Serializer.deserialize content))
            | 500 -> return ArtifactsRepoTokensList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsRepoTokensList" (int status)
        }

    ///<summary>
    ///Returns decoded entries for an immutable Git tree object. Tree responses are cacheable forever by hash.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="name">Repository name.</param>
    ///<param name="hash">40-character lowercase hexadecimal Git object hash.</param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsReposTreeGet
        (accountId: string, ``namespace``: string, name: string, hash: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("name", name)
                  RequestPart.path ("hash", hash) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/repos/{name}/tree/{hash}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsReposTreeGet.OK((Serializer.deserialize content))
            | 400 -> return ArtifactsReposTreeGet.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsReposTreeGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsReposTreeGet.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsReposTreeGet.NotFound((Serializer.deserialize content))
            | 500 -> return ArtifactsReposTreeGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsReposTreeGet" (int status)
        }

    ///<summary>
    ///Creates a scoped Git token for a repository.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsTokensCreate
        (
            accountId: string,
            ``namespace``: string,
            body: ArtifactsTokensCreatePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/tokens"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return ArtifactsTokensCreate.Created((Serializer.deserialize content))
            | 400 -> return ArtifactsTokensCreate.BadRequest((Serializer.deserialize content))
            | 401 -> return ArtifactsTokensCreate.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsTokensCreate.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsTokensCreate.NotFound((Serializer.deserialize content))
            | 409 -> return ArtifactsTokensCreate.Conflict((Serializer.deserialize content))
            | 500 -> return ArtifactsTokensCreate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsTokensCreate" (int status)
        }

    ///<summary>
    ///Revokes an Artifacts repository token.
    ///</summary>
    ///<param name="accountId">Cloudflare account ID.</param>
    ///<param name="namespace">Artifacts namespace name.</param>
    ///<param name="id">Token ID. Must match /^[0-9a-z]{16}$/.</param>
    ///<param name="cancellationToken"></param>
    member this.ArtifactsTokensRevoke
        (accountId: string, ``namespace``: string, id: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/artifacts/namespaces/{namespace}/tokens/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ArtifactsTokensRevoke.OK((Serializer.deserialize content))
            | 401 -> return ArtifactsTokensRevoke.Unauthorized((Serializer.deserialize content))
            | 403 -> return ArtifactsTokensRevoke.Forbidden((Serializer.deserialize content))
            | 404 -> return ArtifactsTokensRevoke.NotFound((Serializer.deserialize content))
            | 500 -> return ArtifactsTokensRevoke.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ArtifactsTokensRevoke" (int status)
        }

    ///<summary>
    ///Returns a list of D1 databases.
    ///</summary>
    member this.D1ListDatabases
        (accountId: string, ?name: string, ?page: float, ?perPage: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/d1/database" requestParts cancellationToken

            match (int status) with
            | 200 -> return D1ListDatabases.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1ListDatabases.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1ListDatabases" (int status)
        }

    ///<summary>
    ///Returns the created D1 database.
    ///</summary>
    member this.D1CreateDatabase
        (accountId: string, body: D1CreateDatabasePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/d1/database" requestParts cancellationToken

            match (int status) with
            | 200 -> return D1CreateDatabase.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1CreateDatabase.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1CreateDatabase" (int status)
        }

    ///<summary>
    ///Deletes the specified D1 database.
    ///</summary>
    member this.D1DeleteDatabase(accountId: string, databaseId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("database_id", databaseId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/d1/database/{database_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return D1DeleteDatabase.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1DeleteDatabase.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1DeleteDatabase" (int status)
        }

    ///<summary>
    ///Returns the specified D1 database.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="databaseId"></param>
    ///<param name="fields">
    ///Comma-separated list of fields to include in the response. When omitted,
    ///all fields are returned.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.D1GetDatabase
        (
            accountId: string,
            databaseId: InlineUnion_81aef760b0dc53840fcd7a49,
            ?fields: list<string>,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("database_id", databaseId)
                  if fields.IsSome then
                      RequestPart.queryComma ("fields", fields.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/d1/database/{database_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return D1GetDatabase.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1GetDatabase.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1GetDatabase" (int status)
        }

    ///<summary>
    ///Updates partially the specified D1 database.
    ///</summary>
    member this.D1UpdatePartialDatabase
        (
            accountId: string,
            databaseId: string,
            body: d1_database_u002D_update_u002D_partial_u002D_request_u002D_body,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("database_id", databaseId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/d1/database/{database_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return D1UpdatePartialDatabase.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1UpdatePartialDatabase.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1UpdatePartialDatabase" (int status)
        }

    ///<summary>
    ///Updates the specified D1 database.
    ///</summary>
    member this.D1UpdateDatabase
        (
            accountId: string,
            databaseId: string,
            body: d1_database_u002D_update_u002D_request_u002D_body,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("database_id", databaseId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/d1/database/{database_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return D1UpdateDatabase.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1UpdateDatabase.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1UpdateDatabase" (int status)
        }

    ///<summary>
    ///Returns a URL where the SQL contents of your D1 can be downloaded. Note: this process may take
    ///some time for larger DBs, during which your D1 will be unavailable to serve queries. To avoid
    ///blocking your DB unnecessarily, an in-progress export must be continually polled or will automatically cancel.
    ///</summary>
    member this.D1ExportDatabase
        (accountId: string, databaseId: string, body: D1ExportDatabasePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("database_id", databaseId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/d1/database/{database_id}/export"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return D1ExportDatabase.OK((Serializer.deserialize content))
            | 202 -> return D1ExportDatabase.Accepted((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1ExportDatabase.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1ExportDatabase" (int status)
        }

    ///<summary>
    ///Generates a temporary URL for uploading an SQL file to, then instructing the D1 to import it
    ///and polling it for status updates. Imports block the D1 for their duration.
    ///</summary>
    member this.D1ImportDatabase
        (
            accountId: string,
            databaseId: string,
            body: InlineUnion_93fa66fbd5e826af1d1a6dd3,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("database_id", databaseId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/d1/database/{database_id}/import"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return D1ImportDatabase.OK((Serializer.deserialize content))
            | 202 -> return D1ImportDatabase.Accepted((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1ImportDatabase.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1ImportDatabase" (int status)
        }

    ///<summary>
    ///Returns the query result as an object.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="databaseId"></param>
    ///<param name="body">A single query object or a batch query object</param>
    ///<param name="cancellationToken"></param>
    member this.D1QueryDatabase
        (accountId: string, databaseId: string, body: d1_batch_u002D_query, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("database_id", databaseId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/d1/database/{database_id}/query"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return D1QueryDatabase.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1QueryDatabase.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1QueryDatabase" (int status)
        }

    ///<summary>
    ///Returns the query result rows as arrays rather than objects. This is a performance-optimized version of the /query endpoint.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="databaseId"></param>
    ///<param name="body">A single query object or a batch query object</param>
    ///<param name="cancellationToken"></param>
    member this.D1RawDatabaseQuery
        (accountId: string, databaseId: string, body: d1_batch_u002D_query, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("database_id", databaseId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/d1/database/{database_id}/raw"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return D1RawDatabaseQuery.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1RawDatabaseQuery.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1RawDatabaseQuery" (int status)
        }

    ///<summary>
    ///Retrieves the current bookmark, or the nearest bookmark at or before a provided timestamp.
    ///Bookmarks can be used with the restore endpoint to revert the database to a previous point in time.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="databaseId"></param>
    ///<param name="timestamp">An optional ISO 8601 timestamp. If provided, returns the nearest available bookmark at or before this timestamp. If omitted, returns the current bookmark.</param>
    ///<param name="cancellationToken"></param>
    member this.D1TimeTravelGetBookmark
        (accountId: string, databaseId: string, ?timestamp: System.DateTimeOffset, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("database_id", databaseId)
                  if timestamp.IsSome then
                      RequestPart.query ("timestamp", timestamp.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/d1/database/{database_id}/time_travel/bookmark"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return D1TimeTravelGetBookmark.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1TimeTravelGetBookmark.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1TimeTravelGetBookmark" (int status)
        }

    ///<summary>
    ///Restores a D1 database to a previous point in time either via a bookmark or a timestamp.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="databaseId"></param>
    ///<param name="bookmark">A bookmark to restore the database to. Required if `timestamp` is not provided.</param>
    ///<param name="timestamp">An ISO 8601 timestamp to restore the database to. Required if `bookmark` is not provided.</param>
    ///<param name="cancellationToken"></param>
    member this.D1TimeTravelRestore
        (
            accountId: string,
            databaseId: string,
            ?bookmark: string,
            ?timestamp: System.DateTimeOffset,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("database_id", databaseId)
                  if bookmark.IsSome then
                      RequestPart.query ("bookmark", bookmark.Value)
                  if timestamp.IsSome then
                      RequestPart.query ("timestamp", timestamp.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/d1/database/{database_id}/time_travel/restore"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return D1TimeTravelRestore.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return D1TimeTravelRestore.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for D1TimeTravelRestore" (int status)
        }

    ///<summary>
    ///Returns a list of Hyperdrives.
    ///</summary>
    ///<param name="accountId">The Cloudflare account ID.</param>
    ///<param name="page">Page number of paginated results.</param>
    ///<param name="perPage">Maximum number of results per page.</param>
    ///<param name="cancellationToken"></param>
    member this.ListHyperdrive(accountId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken) =
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
                    "/accounts/{account_id}/hyperdrive/configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListHyperdrive.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListHyperdrive.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListHyperdrive" (int status)
        }

    ///<summary>
    ///Creates and returns a new Hyperdrive configuration.
    ///</summary>
    ///<param name="accountId">The Cloudflare account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreateHyperdrive
        (accountId: string, body: hyperdrive_hyperdrive_u002D_config, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/hyperdrive/configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateHyperdrive.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreateHyperdrive.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateHyperdrive" (int status)
        }

    ///<summary>
    ///Deletes the specified Hyperdrive.
    ///</summary>
    ///<param name="accountId">The Cloudflare account ID.</param>
    ///<param name="hyperdriveId">The unique identifier of the Hyperdrive configuration.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteHyperdrive(accountId: string, hyperdriveId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("hyperdrive_id", hyperdriveId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/hyperdrive/configs/{hyperdrive_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteHyperdrive.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteHyperdrive.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteHyperdrive" (int status)
        }

    ///<summary>
    ///Returns the specified Hyperdrive configuration.
    ///</summary>
    ///<param name="accountId">The Cloudflare account ID.</param>
    ///<param name="hyperdriveId">The unique identifier of the Hyperdrive configuration.</param>
    ///<param name="cancellationToken"></param>
    member this.GetHyperdrive(accountId: string, hyperdriveId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("hyperdrive_id", hyperdriveId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/hyperdrive/configs/{hyperdrive_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetHyperdrive.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetHyperdrive.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetHyperdrive" (int status)
        }

    ///<summary>
    ///Updates and returns the specified fields of the Hyperdrive configuration. Custom caching settings are not kept if caching is disabled.
    ///</summary>
    ///<param name="accountId">The Cloudflare account ID.</param>
    ///<param name="hyperdriveId">The unique identifier of the Hyperdrive configuration.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.PatchHyperdrive
        (
            accountId: string,
            hyperdriveId: string,
            body: hyperdrive_hyperdrive_u002D_config_u002D_patch,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("hyperdrive_id", hyperdriveId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/hyperdrive/configs/{hyperdrive_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PatchHyperdrive.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PatchHyperdrive.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PatchHyperdrive" (int status)
        }

    ///<summary>
    ///Replaces and returns the specified Hyperdrive configuration. The request must include the name and complete origin connection details. Omitted caching settings are reset to their defaults, while omitted mTLS settings and origin connection limits are preserved. Use the update operation to modify only selected fields.
    ///</summary>
    ///<param name="accountId">The Cloudflare account ID.</param>
    ///<param name="hyperdriveId">The unique identifier of the Hyperdrive configuration.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateHyperdrive
        (
            accountId: string,
            hyperdriveId: string,
            body: hyperdrive_hyperdrive_u002D_config,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("hyperdrive_id", hyperdriveId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/hyperdrive/configs/{hyperdrive_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateHyperdrive.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateHyperdrive.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateHyperdrive" (int status)
        }

    ///<summary>
    ///Restarts the connection pool for the specified Hyperdrive configuration without changing its configuration. Existing connections are drained and a new pool is established at the edge.
    ///</summary>
    ///<param name="accountId">The Cloudflare account ID.</param>
    ///<param name="hyperdriveId">The unique identifier of the Hyperdrive configuration.</param>
    ///<param name="cancellationToken"></param>
    member this.RestartHyperdrive(accountId: string, hyperdriveId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("hyperdrive_id", hyperdriveId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/hyperdrive/configs/{hyperdrive_id}/restart"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return RestartHyperdrive.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return RestartHyperdrive.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for RestartHyperdrive" (int status)
        }

    ///<summary>
    ///Returns a short-lived signed authorization for creating a database that is billed through Cloudflare. The caller passes these values to the integration partner's own CLI, which verifies the signature before creating the database. Requires the account to be entitled to Cloudflare-billed databases for the integration.
    ///</summary>
    ///<param name="accountId">The Cloudflare account ID.</param>
    ///<param name="integration">The database integration to authorize against.</param>
    ///<param name="cancellationToken"></param>
    member this.CreateHyperdriveDatabaseSignature
        (accountId: string, integration: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("integration", integration) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/hyperdrive/integrationsOperations/{integration}/createDatabaseSignature"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateHyperdriveDatabaseSignature.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreateHyperdriveDatabaseSignature.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateHyperdriveDatabaseSignature" (int status)
        }

    ///<summary>
    ///Returns a list of R2 buckets that have been enabled as Apache Iceberg catalogs
    ///for the specified account. Each catalog represents an R2 bucket configured
    ///to store Iceberg metadata and data files.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="cancellationToken"></param>
    member this.ListCatalogs(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/r2-catalog" requestParts cancellationToken

            match (int status) with
            | 200 -> return ListCatalogs.OK((Serializer.deserialize content))
            | 400 -> return ListCatalogs.BadRequest((Serializer.deserialize content))
            | 401 -> return ListCatalogs.Unauthorized((Serializer.deserialize content))
            | 403 -> return ListCatalogs.Forbidden((Serializer.deserialize content))
            | 500 -> return ListCatalogs.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListCatalogs" (int status)
        }

    ///<summary>
    ///Retrieve detailed information about a specific R2 catalog by bucket name.
    ///Returns catalog status, maintenance configuration, and credential status.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name.</param>
    ///<param name="cancellationToken"></param>
    member this.GetCatalogDetails(accountId: string, bucketName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetCatalogDetails.OK((Serializer.deserialize content))
            | 400 -> return GetCatalogDetails.BadRequest((Serializer.deserialize content))
            | 401 -> return GetCatalogDetails.Unauthorized((Serializer.deserialize content))
            | 403 -> return GetCatalogDetails.Forbidden((Serializer.deserialize content))
            | 404 -> return GetCatalogDetails.NotFound((Serializer.deserialize content))
            | 500 -> return GetCatalogDetails.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetCatalogDetails" (int status)
        }

    ///<summary>
    ///Store authentication credentials for a catalog. These credentials are used
    ///to authenticate with R2 storage when performing catalog operations.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name.</param>
    ///<param name="body">Contains request to store catalog credentials.</param>
    ///<param name="cancellationToken"></param>
    member this.StoreCredentials
        (
            accountId: string,
            bucketName: string,
            body: r2_u002D_data_u002D_catalog_catalog_u002D_credential_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/credential"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return StoreCredentials.OK((Serializer.deserialize content))
            | 400 -> return StoreCredentials.BadRequest((Serializer.deserialize content))
            | 401 -> return StoreCredentials.Unauthorized((Serializer.deserialize content))
            | 403 -> return StoreCredentials.Forbidden((Serializer.deserialize content))
            | 404 -> return StoreCredentials.NotFound((Serializer.deserialize content))
            | 500 -> return StoreCredentials.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for StoreCredentials" (int status)
        }

    ///<summary>
    ///Removes the catalog from the control plane without deleting R2 bucket objects.
    ///Set force=true to remove catalog namespaces, tables, views, and maintenance
    ///metadata. Force deletion is limited to a configured catalog object count.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="bucketName"></param>
    ///<param name="force">Remove child metadata before deleting the catalog.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteCatalog
        (accountId: string, bucketName: string, ?force: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  if force.IsSome then
                      RequestPart.query ("force", force.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/delete"
                    requestParts
                    cancellationToken

            match (int status) with
            | 204 -> return DeleteCatalog.NoContent
            | 400 -> return DeleteCatalog.BadRequest((Serializer.deserialize content))
            | 401 -> return DeleteCatalog.Unauthorized
            | 404 -> return DeleteCatalog.NotFound
            | 409 -> return DeleteCatalog.Conflict((Serializer.deserialize content))
            | 500 -> return DeleteCatalog.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteCatalog" (int status)
        }

    ///<summary>
    ///Disable an R2 bucket as a catalog. This operation deactivates the catalog
    ///but preserves existing metadata and data files. The catalog can be
    ///re-enabled later.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name to disable as catalog.</param>
    ///<param name="cancellationToken"></param>
    member this.DisableCatalog(accountId: string, bucketName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/disable"
                    requestParts
                    cancellationToken

            match (int status) with
            | 204 -> return DisableCatalog.NoContent
            | 400 -> return DisableCatalog.BadRequest((Serializer.deserialize content))
            | 401 -> return DisableCatalog.Unauthorized((Serializer.deserialize content))
            | 403 -> return DisableCatalog.Forbidden((Serializer.deserialize content))
            | 404 -> return DisableCatalog.NotFound((Serializer.deserialize content))
            | 500 -> return DisableCatalog.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DisableCatalog" (int status)
        }

    ///<summary>
    ///Enable an R2 bucket as an Apache Iceberg catalog. This operation creates
    ///the necessary catalog infrastructure and activates the bucket for storing
    ///Iceberg metadata and data files.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name to enable as catalog.</param>
    ///<param name="cancellationToken"></param>
    member this.EnableCatalog(accountId: string, bucketName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/enable"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return EnableCatalog.OK((Serializer.deserialize content))
            | 400 -> return EnableCatalog.BadRequest((Serializer.deserialize content))
            | 401 -> return EnableCatalog.Unauthorized((Serializer.deserialize content))
            | 403 -> return EnableCatalog.Forbidden((Serializer.deserialize content))
            | 404 -> return EnableCatalog.NotFound((Serializer.deserialize content))
            | 409 -> return EnableCatalog.Conflict((Serializer.deserialize content))
            | 500 -> return EnableCatalog.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for EnableCatalog" (int status)
        }

    ///<summary>
    ///Retrieve the maintenance configuration for a specific catalog,
    ///including compaction settings and credential status.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name.</param>
    ///<param name="cancellationToken"></param>
    member this.GetMaintenanceConfig(accountId: string, bucketName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/maintenance-configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetMaintenanceConfig.OK((Serializer.deserialize content))
            | 400 -> return GetMaintenanceConfig.BadRequest((Serializer.deserialize content))
            | 401 -> return GetMaintenanceConfig.Unauthorized((Serializer.deserialize content))
            | 403 -> return GetMaintenanceConfig.Forbidden((Serializer.deserialize content))
            | 404 -> return GetMaintenanceConfig.NotFound((Serializer.deserialize content))
            | 500 -> return GetMaintenanceConfig.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetMaintenanceConfig" (int status)
        }

    ///<summary>
    ///Update the maintenance configuration for a catalog. This allows you to
    ///enable or disable compaction and adjust target file sizes for optimization.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name.</param>
    ///<param name="body">Contains request to update catalog maintenance configuration.</param>
    ///<param name="cancellationToken"></param>
    member this.UpdateMaintenanceConfig
        (
            accountId: string,
            bucketName: string,
            body: r2_u002D_data_u002D_catalog_catalog_u002D_maintenance_u002D_update_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/maintenance-configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateMaintenanceConfig.OK((Serializer.deserialize content))
            | 400 -> return UpdateMaintenanceConfig.BadRequest((Serializer.deserialize content))
            | 401 -> return UpdateMaintenanceConfig.Unauthorized((Serializer.deserialize content))
            | 403 -> return UpdateMaintenanceConfig.Forbidden((Serializer.deserialize content))
            | 404 -> return UpdateMaintenanceConfig.NotFound((Serializer.deserialize content))
            | 500 -> return UpdateMaintenanceConfig.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateMaintenanceConfig" (int status)
        }

    ///<summary>
    ///Returns a list of namespaces in the specified R2 catalog.
    ///Supports hierarchical filtering and pagination for efficient traversal
    ///of large namespace hierarchies.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name.</param>
    ///<param name="pageToken">
    ///Opaque pagination token from a previous response.
    ///Use this to fetch the next page of results.
    ///</param>
    ///<param name="pageSize">
    ///Maximum number of namespaces to return per page.
    ///Defaults to 100, maximum 1000.
    ///</param>
    ///<param name="parent">
    ///Parent namespace to filter by. Only returns direct children of this namespace.
    ///For nested namespaces, use %1F as separator (e.g., "bronze%1Fanalytics").
    ///Omit this parameter to list top-level namespaces.
    ///</param>
    ///<param name="returnUuids">
    ///Whether to include namespace UUIDs in the response.
    ///Set to true to receive the namespace_uuids array.
    ///</param>
    ///<param name="returnDetails">
    ///Whether to include additional metadata (timestamps).
    ///When true, response includes created_at and updated_at arrays.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.ListNamespaces
        (
            accountId: string,
            bucketName: string,
            ?pageToken: string,
            ?pageSize: int,
            ?parent: string,
            ?returnUuids: bool,
            ?returnDetails: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  if pageToken.IsSome then
                      RequestPart.query ("page_token", pageToken.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if parent.IsSome then
                      RequestPart.query ("parent", parent.Value)
                  if returnUuids.IsSome then
                      RequestPart.query ("return_uuids", returnUuids.Value)
                  if returnDetails.IsSome then
                      RequestPart.query ("return_details", returnDetails.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListNamespaces.OK((Serializer.deserialize content))
            | 400 -> return ListNamespaces.BadRequest((Serializer.deserialize content))
            | 401 -> return ListNamespaces.Unauthorized((Serializer.deserialize content))
            | 403 -> return ListNamespaces.Forbidden((Serializer.deserialize content))
            | 404 -> return ListNamespaces.NotFound((Serializer.deserialize content))
            | 500 -> return ListNamespaces.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListNamespaces" (int status)
        }

    ///<summary>
    ///Returns a list of tables in the specified namespace within an R2 catalog.
    ///Supports pagination for efficient traversal of large table collections.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name.</param>
    ///<param name="namespace">
    ///The namespace identifier.
    ///For nested namespaces, use %1F as separator (e.g., "bronze%1Fanalytics").
    ///</param>
    ///<param name="pageToken">
    ///Opaque pagination token from a previous response.
    ///Use this to fetch the next page of results.
    ///</param>
    ///<param name="pageSize">
    ///Maximum number of tables to return per page.
    ///Defaults to 100, maximum 1000.
    ///</param>
    ///<param name="returnUuids">
    ///Whether to include table UUIDs in the response.
    ///Set to true to receive the table_uuids array.
    ///</param>
    ///<param name="returnDetails">
    ///Whether to include additional metadata (timestamps, locations).
    ///When true, response includes created_at, updated_at, metadata_locations, and locations arrays.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.ListTables
        (
            accountId: string,
            bucketName: string,
            ``namespace``: string,
            ?pageToken: string,
            ?pageSize: int,
            ?returnUuids: bool,
            ?returnDetails: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("namespace", ``namespace``)
                  if pageToken.IsSome then
                      RequestPart.query ("page_token", pageToken.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if returnUuids.IsSome then
                      RequestPart.query ("return_uuids", returnUuids.Value)
                  if returnDetails.IsSome then
                      RequestPart.query ("return_details", returnDetails.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/namespaces/{namespace}/tables"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListTables.OK((Serializer.deserialize content))
            | 400 -> return ListTables.BadRequest((Serializer.deserialize content))
            | 401 -> return ListTables.Unauthorized((Serializer.deserialize content))
            | 403 -> return ListTables.Forbidden((Serializer.deserialize content))
            | 404 -> return ListTables.NotFound((Serializer.deserialize content))
            | 500 -> return ListTables.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListTables" (int status)
        }

    ///<summary>
    ///Returns full Apache Iceberg metadata for a single table: schema,
    ///partition specs, sort orders, properties, and recent snapshot history.
    ///Designed for catalog introspection UIs that need per-table details
    ///without holding R2 credentials.
    ///The `metadata.snapshots`, `metadata.snapshot-log`, and
    ///`metadata.metadata-log` arrays are pruned to the most recent 10
    ///entries by `timestamp-ms`. Use `total_snapshots` and
    ///`returned_snapshots` to surface the truncation to end users.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name.</param>
    ///<param name="namespace">
    ///The namespace identifier.
    ///For nested namespaces, use %1F as separator (e.g., "bronze%1Fanalytics").
    ///</param>
    ///<param name="tableName">The table name within the given namespace.</param>
    ///<param name="cancellationToken"></param>
    member this.GetTable
        (
            accountId: string,
            bucketName: string,
            ``namespace``: string,
            tableName: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("table_name", tableName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/namespaces/{namespace}/tables/{table_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetTable.OK((Serializer.deserialize content))
            | 400 -> return GetTable.BadRequest((Serializer.deserialize content))
            | 401 -> return GetTable.Unauthorized((Serializer.deserialize content))
            | 403 -> return GetTable.Forbidden((Serializer.deserialize content))
            | 404 -> return GetTable.NotFound((Serializer.deserialize content))
            | 500 -> return GetTable.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetTable" (int status)
        }

    ///<summary>
    ///Retrieve the maintenance configuration for a specific table,
    ///including compaction settings.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name.</param>
    ///<param name="namespace">The namespace identifier (use %1F as separator for nested namespaces).</param>
    ///<param name="tableName">The table name.</param>
    ///<param name="cancellationToken"></param>
    member this.GetTableMaintenanceConfig
        (
            accountId: string,
            bucketName: string,
            ``namespace``: string,
            tableName: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("table_name", tableName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/namespaces/{namespace}/tables/{table_name}/maintenance-configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetTableMaintenanceConfig.OK((Serializer.deserialize content))
            | 400 -> return GetTableMaintenanceConfig.BadRequest((Serializer.deserialize content))
            | 401 -> return GetTableMaintenanceConfig.Unauthorized((Serializer.deserialize content))
            | 403 -> return GetTableMaintenanceConfig.Forbidden((Serializer.deserialize content))
            | 404 -> return GetTableMaintenanceConfig.NotFound((Serializer.deserialize content))
            | 500 -> return GetTableMaintenanceConfig.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetTableMaintenanceConfig" (int status)
        }

    ///<summary>
    ///Update the maintenance configuration for a specific table. This allows you to
    ///enable or disable compaction and adjust target file sizes for optimization.
    ///</summary>
    ///<param name="accountId">Identifies the account.</param>
    ///<param name="bucketName">Specifies the R2 bucket name.</param>
    ///<param name="namespace">The namespace identifier (use %1F as separator for nested namespaces).</param>
    ///<param name="tableName">The table name.</param>
    ///<param name="body">Contains request to update table maintenance configuration.</param>
    ///<param name="cancellationToken"></param>
    member this.UpdateTableMaintenanceConfig
        (
            accountId: string,
            bucketName: string,
            ``namespace``: string,
            tableName: string,
            body: r2_u002D_data_u002D_catalog_table_u002D_maintenance_u002D_update_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("namespace", ``namespace``)
                  RequestPart.path ("table_name", tableName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/r2-catalog/{bucket_name}/namespaces/{namespace}/tables/{table_name}/maintenance-configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateTableMaintenanceConfig.OK((Serializer.deserialize content))
            | 400 -> return UpdateTableMaintenanceConfig.BadRequest((Serializer.deserialize content))
            | 401 -> return UpdateTableMaintenanceConfig.Unauthorized((Serializer.deserialize content))
            | 403 -> return UpdateTableMaintenanceConfig.Forbidden((Serializer.deserialize content))
            | 404 -> return UpdateTableMaintenanceConfig.NotFound((Serializer.deserialize content))
            | 500 -> return UpdateTableMaintenanceConfig.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateTableMaintenanceConfig" (int status)
        }

    ///<summary>
    ///Lists all R2 buckets on your account.
    ///</summary>
    member this.R2ListBuckets
        (
            accountId: string,
            ?nameContains: string,
            ?startAfter: string,
            ?perPage: float,
            ?order: string,
            ?direction: string,
            ?cursor: string,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if nameContains.IsSome then
                      RequestPart.query ("name_contains", nameContains.Value)
                  if startAfter.IsSome then
                      RequestPart.query ("start_after", startAfter.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/r2/buckets" requestParts cancellationToken

            match (int status) with
            | 200 -> return R2ListBuckets.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2ListBuckets.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2ListBuckets" (int status)
        }

    ///<summary>
    ///Creates a new R2 bucket.
    ///</summary>
    member this.R2CreateBucket
        (
            accountId: string,
            body: R2CreateBucketPayload,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/r2/buckets" requestParts cancellationToken

            match (int status) with
            | 200 -> return R2CreateBucket.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2CreateBucket.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2CreateBucket" (int status)
        }

    ///<summary>
    ///Deletes an existing R2 bucket.
    ///</summary>
    member this.R2DeleteBucket
        (bucketName: string, accountId: string, ?cfR2Jurisdiction: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2DeleteBucket.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2DeleteBucket.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2DeleteBucket" (int status)
        }

    ///<summary>
    ///Gets properties of an existing R2 bucket.
    ///</summary>
    member this.R2GetBucket
        (accountId: string, bucketName: string, ?cfR2Jurisdiction: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetBucket.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetBucket.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetBucket" (int status)
        }

    ///<summary>
    ///Updates properties of an existing R2 bucket.
    ///</summary>
    member this.R2PatchBucket
        (
            accountId: string,
            bucketName: string,
            cfR2StorageClass: string,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.header ("cf-r2-storage-class", cfR2StorageClass)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2PatchBucket.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2PatchBucket.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2PatchBucket" (int status)
        }

    ///<summary>
    ///Creates a new R2 bucket using the name from the URL path. Similar to `r2-create-bucket` (POST), but the bucket name comes from the path and the optional storage class is supplied via the `cf-r2-storage-class` header. There is no request body. Unlike the POST variant, this endpoint does not accept a location hint — the bucket is placed in the R2 region for the caller's edge colo. Use the POST variant if you need to set a `locationHint`.
    ///</summary>
    member this.R2CreateBucketByName
        (
            accountId: string,
            bucketName: string,
            ?cfR2Jurisdiction: string,
            ?cfR2StorageClass: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value)
                  if cfR2StorageClass.IsSome then
                      RequestPart.header ("cf-r2-storage-class", cfR2StorageClass.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2CreateBucketByName.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2CreateBucketByName.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2CreateBucketByName" (int status)
        }

    ///<summary>
    ///Delete the CORS policy for a bucket.
    ///</summary>
    member this.R2DeleteBucketCorsPolicy
        (bucketName: string, accountId: string, ?cfR2Jurisdiction: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/cors"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2DeleteBucketCorsPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2DeleteBucketCorsPolicy.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2DeleteBucketCorsPolicy" (int status)
        }

    ///<summary>
    ///Get the CORS policy for a bucket.
    ///</summary>
    member this.R2GetBucketCorsPolicy
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
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/cors"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetBucketCorsPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetBucketCorsPolicy.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetBucketCorsPolicy" (int status)
        }

    ///<summary>
    ///Set the CORS policy for a bucket.
    ///</summary>
    member this.R2PutBucketCorsPolicy
        (
            bucketName: string,
            accountId: string,
            body: R2PutBucketCorsPolicyPayload,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/cors"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2PutBucketCorsPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2PutBucketCorsPolicy.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2PutBucketCorsPolicy" (int status)
        }

    ///<summary>
    ///Gets a list of all custom domains registered with an existing R2 bucket.
    ///</summary>
    member this.R2ListCustomDomains
        (accountId: string, bucketName: string, ?cfR2Jurisdiction: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/domains/custom"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2ListCustomDomains.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2ListCustomDomains.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2ListCustomDomains" (int status)
        }

    ///<summary>
    ///Register a new custom domain for an existing R2 bucket.
    ///</summary>
    member this.R2AddCustomDomain
        (
            accountId: string,
            bucketName: string,
            body: r2_add_custom_domain_request,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.jsonContent body
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/domains/custom"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2AddCustomDomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2AddCustomDomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2AddCustomDomain" (int status)
        }

    ///<summary>
    ///Remove custom domain registration from an existing R2 bucket.
    ///</summary>
    member this.R2DeleteCustomDomain
        (
            bucketName: string,
            accountId: string,
            domain: string,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("domain", domain)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/domains/custom/{domain}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2DeleteCustomDomain.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2DeleteCustomDomain.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2DeleteCustomDomain" (int status)
        }

    ///<summary>
    ///Get the configuration for a custom domain on an existing R2 bucket.
    ///</summary>
    member this.R2GetCustomDomainSettings
        (
            accountId: string,
            bucketName: string,
            domain: string,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("domain", domain)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/domains/custom/{domain}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetCustomDomainSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetCustomDomainSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetCustomDomainSettings" (int status)
        }

    ///<summary>
    ///Edit the configuration for a custom domain on an existing R2 bucket.
    ///</summary>
    member this.R2EditCustomDomainSettings
        (
            accountId: string,
            bucketName: string,
            domain: string,
            body: r2_edit_custom_domain_request,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("domain", domain)
                  RequestPart.jsonContent body
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/domains/custom/{domain}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2EditCustomDomainSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2EditCustomDomainSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2EditCustomDomainSettings" (int status)
        }

    ///<summary>
    ///Gets state of public access over the bucket's R2-managed (r2.dev) domain.
    ///</summary>
    member this.R2GetBucketPublicPolicy
        (accountId: string, bucketName: string, ?cfR2Jurisdiction: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/domains/managed"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetBucketPublicPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetBucketPublicPolicy.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetBucketPublicPolicy" (int status)
        }

    ///<summary>
    ///Updates state of public access over the bucket's R2-managed (r2.dev) domain.
    ///</summary>
    member this.R2PutBucketPublicPolicy
        (
            accountId: string,
            bucketName: string,
            body: r2_edit_managed_domain_request,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.jsonContent body
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/domains/managed"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2PutBucketPublicPolicy.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2PutBucketPublicPolicy.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2PutBucketPublicPolicy" (int status)
        }

    ///<summary>
    ///Lists background jobs for an R2 bucket. Use this endpoint to poll jobs returned by
    ///asynchronous operations such as deleting objects by prefix or emptying a bucket.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="bucketName"></param>
    ///<param name="cfR2Jurisdiction"></param>
    ///<param name="jobType">Restricts results to jobs of the specified type.</param>
    ///<param name="status">Restricts results to jobs with the specified status. `jobType` is required when this parameter is provided.</param>
    ///<param name="maxKeys">Maximum number of jobs to return.</param>
    ///<param name="continuationToken">Pagination token received as `nextContinuationToken` in the previous response.</param>
    ///<param name="cancellationToken"></param>
    member this.R2ListBucketJobs
        (
            accountId: string,
            bucketName: string,
            ?cfR2Jurisdiction: string,
            ?jobType: string,
            ?status: string,
            ?maxKeys: int,
            ?continuationToken: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value)
                  if jobType.IsSome then
                      RequestPart.query ("jobType", jobType.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if maxKeys.IsSome then
                      RequestPart.query ("maxKeys", maxKeys.Value)
                  if continuationToken.IsSome then
                      RequestPart.query ("continuationToken", continuationToken.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/jobs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2ListBucketJobs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2ListBucketJobs.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2ListBucketJobs" (int status)
        }

    ///<summary>
    ///Gets the current status of a background job for an R2 bucket. Poll this endpoint with
    ///the job identifier returned when the operation was submitted until the status is
    ///`COMPLETED`, `FAILED`, or `CANCELLED`.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="bucketName"></param>
    ///<param name="jobId">Identifier returned when the background job was submitted.</param>
    ///<param name="cfR2Jurisdiction"></param>
    ///<param name="cancellationToken"></param>
    member this.R2GetBucketJob
        (
            accountId: string,
            bucketName: string,
            jobId: string,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("job_id", jobId)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/jobs/{job_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetBucketJob.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetBucketJob.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetBucketJob" (int status)
        }

    ///<summary>
    ///Get object lifecycle rules for a bucket.
    ///</summary>
    member this.R2GetBucketLifecycleConfiguration
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
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/lifecycle"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetBucketLifecycleConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetBucketLifecycleConfiguration.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetBucketLifecycleConfiguration" (int status)
        }

    ///<summary>
    ///Set the object lifecycle rules for a bucket.
    ///</summary>
    member this.R2PutBucketLifecycleConfiguration
        (
            bucketName: string,
            accountId: string,
            body: R2PutBucketLifecycleConfigurationPayload,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/lifecycle"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2PutBucketLifecycleConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2PutBucketLifecycleConfiguration.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2PutBucketLifecycleConfiguration" (int status)
        }

    ///<summary>
    ///Get the local uploads configuration for a bucket. When enabled, object's data is written to the nearest region first, then asynchronously replicated to the bucket's primary region.
    ///</summary>
    member this.R2GetBucketLocalUploadsConfiguration
        (bucketName: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/local-uploads"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetBucketLocalUploadsConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetBucketLocalUploadsConfiguration.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetBucketLocalUploadsConfiguration" (int status)
        }

    ///<summary>
    ///Set the local uploads configuration for a bucket. When enabled, object's data is written to the nearest region first, then asynchronously replicated to the bucket's primary region.
    ///</summary>
    member this.R2PutBucketLocalUploadsConfiguration
        (
            bucketName: string,
            accountId: string,
            body: R2PutBucketLocalUploadsConfigurationPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/local-uploads"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2PutBucketLocalUploadsConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2PutBucketLocalUploadsConfiguration.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2PutBucketLocalUploadsConfiguration" (int status)
        }

    ///<summary>
    ///Get lock rules for a bucket.
    ///</summary>
    member this.R2GetBucketLockConfiguration
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
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/lock"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetBucketLockConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetBucketLockConfiguration.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetBucketLockConfiguration" (int status)
        }

    ///<summary>
    ///Set lock rules for a bucket.
    ///</summary>
    member this.R2PutBucketLockConfiguration
        (
            bucketName: string,
            accountId: string,
            body: R2PutBucketLockConfigurationPayload,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/lock"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2PutBucketLockConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2PutBucketLockConfiguration.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2PutBucketLockConfiguration" (int status)
        }

    ///<summary>
    ///Deletes objects from an R2 bucket. Three modes are supported:
    ///1. **Delete by list** (default): Provide a JSON array of object keys in the request body.
    ///   All listed objects are deleted; per-key errors are reported in the response.
    ///2. **Delete by prefix**: Provide a non-empty `prefix` query parameter and no request body
    ///   to delete every object whose key begins with that prefix.
    ///3. **Empty bucket**: Provide the `prefix` query parameter with an empty value (`?prefix=`)
    ///   and no request body to delete all objects in the bucket.
    ///Prefix and empty-bucket requests return a job descriptor. Small jobs can finish
    ///synchronously and return `COMPLETED`; larger jobs continue in the background. Poll the
    ///returned `id` with the Get Bucket Job endpoint. Objects uploaded after a background job
    ///starts are not deleted by that job. Abort active multipart uploads before submitting the
    ///request; a synchronously completed job does not abort them. Avoid writing objects or
    ///starting multipart uploads while a bucket-emptying job is in progress.
    ///Each repeated or concurrent request creates a distinct job. The number of active jobs is
    ///limited per bucket; wait for an existing job to finish before retrying a request rejected
    ///with HTTP 429.
    ///A bucket cannot be emptied while event notifications are configured. Remove the event
    ///notification rules and retry requests rejected with HTTP 409 / error code 10034. To
    ///protect a bucket with R2 Data Catalog enabled, send the `cf-r2-data-catalog-check` header;
    ///a conflict is returned with HTTP 409 / error code 10081.
    ///For most workloads, we recommend using R2's [S3-compatible API](https://developers.cloudflare.com/r2/api/s3/api/) or a [Worker with an R2 binding](https://developers.cloudflare.com/r2/api/workers/workers-api-reference/) instead.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="bucketName"></param>
    ///<param name="cfR2Jurisdiction"></param>
    ///<param name="prefix"></param>
    ///<param name="cfR2DataCatalogCheck">Set this header to reject the operation when R2 Data Catalog is enabled for the bucket.</param>
    ///<param name="cancellationToken"></param>
    ///<param name="body"></param>
    member this.R2DeleteObjects
        (
            accountId: string,
            bucketName: string,
            ?cfR2Jurisdiction: string,
            ?prefix: string,
            ?cfR2DataCatalogCheck: string,
            ?cancellationToken: CancellationToken,
            ?body: list<string>
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value)
                  if prefix.IsSome then
                      RequestPart.query ("prefix", prefix.Value)
                  if cfR2DataCatalogCheck.IsSome then
                      RequestPart.header ("cf-r2-data-catalog-check", cfR2DataCatalogCheck.Value)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/objects"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2DeleteObjects.OK((Serializer.deserialize content))
            | 409 -> return R2DeleteObjects.Conflict((Serializer.deserialize content))
            | 429 -> return R2DeleteObjects.TooManyRequests((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2DeleteObjects.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2DeleteObjects" (int status)
        }

    ///<summary>
    ///Lists objects in an R2 bucket. Returns object metadata including key, size, etag, last modified date, HTTP metadata, and custom metadata.
    ///For most workloads, we recommend using R2's [S3-compatible API](https://developers.cloudflare.com/r2/api/s3/api/) or a [Worker with an R2 binding](https://developers.cloudflare.com/r2/api/workers/workers-api-reference/) instead.
    ///</summary>
    member this.R2ListObjects
        (
            accountId: string,
            bucketName: string,
            ?cfR2Jurisdiction: string,
            ?perPage: int,
            ?prefix: string,
            ?delimiter: string,
            ?cursor: string,
            ?startAfter: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if prefix.IsSome then
                      RequestPart.query ("prefix", prefix.Value)
                  if delimiter.IsSome then
                      RequestPart.query ("delimiter", delimiter.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if startAfter.IsSome then
                      RequestPart.query ("start_after", startAfter.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/objects"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2ListObjects.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2ListObjects.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2ListObjects" (int status)
        }

    ///<summary>
    ///Deletes an object from an R2 bucket.
    ///For most workloads, we recommend using R2's [S3-compatible API](https://developers.cloudflare.com/r2/api/s3/api/) or a [Worker with an R2 binding](https://developers.cloudflare.com/r2/api/workers/workers-api-reference/) instead.
    ///</summary>
    member this.R2DeleteObject
        (
            accountId: string,
            bucketName: string,
            objectKey: string,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("object_key", objectKey)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/objects/{object_key}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2DeleteObject.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2DeleteObject.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2DeleteObject" (int status)
        }

    ///<summary>
    ///Retrieves an object from an R2 bucket. Returns the object body along with metadata headers.
    ///For most workloads, we recommend using R2's [S3-compatible API](https://developers.cloudflare.com/r2/api/s3/api/) or a [Worker with an R2 binding](https://developers.cloudflare.com/r2/api/workers/workers-api-reference/) instead.
    ///</summary>
    member this.R2GetObject
        (
            accountId: string,
            bucketName: string,
            objectKey: string,
            ?cfR2Jurisdiction: string,
            ?ifNoneMatch: string,
            ?ifModifiedSince: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("object_key", objectKey)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value)
                  if ifNoneMatch.IsSome then
                      RequestPart.header ("If-None-Match", ifNoneMatch.Value)
                  if ifModifiedSince.IsSome then
                      RequestPart.header ("If-Modified-Since", ifModifiedSince.Value) ]

            let! (status, _, contentBinary) =
                OpenApiHttp.getBinaryAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/objects/{object_key}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetObject.OK(contentBinary)
            | 304 -> return R2GetObject.NotModified
            | _ when (((int status) / 100) = 4) ->
                let content = Encoding.UTF8.GetString contentBinary
                return R2GetObject.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetObject" (int status)
        }

    ///<summary>
    ///Uploads an object to an R2 bucket. The object body is provided as the request body. Returns metadata about the uploaded object.
    ///The maximum upload size for this endpoint is 300 MB. For most workloads, we recommend using R2's [S3-compatible API](https://developers.cloudflare.com/r2/api/s3/api/) or a [Worker with an R2 binding](https://developers.cloudflare.com/r2/api/workers/workers-api-reference/) instead.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="bucketName"></param>
    ///<param name="objectKey"></param>
    ///<param name="body">The object body to upload.</param>
    ///<param name="cfR2Jurisdiction"></param>
    ///<param name="contentType"></param>
    ///<param name="contentLength"></param>
    ///<param name="cfR2StorageClass">Storage class for this object. Overrides the bucket default.</param>
    ///<param name="cancellationToken"></param>
    member this.R2PutObject
        (
            accountId: string,
            bucketName: string,
            objectKey: string,
            body: byte[],
            ?cfR2Jurisdiction: string,
            ?contentType: string,
            ?contentLength: int,
            ?cfR2StorageClass: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("object_key", objectKey)
                  RequestPart.rawContent ("application/octet-stream", body)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value)
                  if contentType.IsSome then
                      RequestPart.header ("Content-Type", contentType.Value)
                  if contentLength.IsSome then
                      RequestPart.header ("Content-Length", contentLength.Value)
                  if cfR2StorageClass.IsSome then
                      RequestPart.header ("cf-r2-storage-class", cfR2StorageClass.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/objects/{object_key}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2PutObject.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2PutObject.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2PutObject" (int status)
        }

    ///<summary>
    ///Disables Sippy on this bucket.
    ///</summary>
    member this.R2DeleteBucketSippyConfig
        (bucketName: string, accountId: string, ?cfR2Jurisdiction: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("bucket_name", bucketName)
                  RequestPart.path ("account_id", accountId)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/sippy"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2DeleteBucketSippyConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2DeleteBucketSippyConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2DeleteBucketSippyConfig" (int status)
        }

    ///<summary>
    ///Gets configuration for Sippy for an existing R2 bucket.
    ///</summary>
    member this.R2GetBucketSippyConfig
        (accountId: string, bucketName: string, ?cfR2Jurisdiction: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/sippy"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2GetBucketSippyConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetBucketSippyConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetBucketSippyConfig" (int status)
        }

    ///<summary>
    ///Sets configuration for Sippy for an existing R2 bucket.
    ///</summary>
    member this.R2PutBucketSippyConfig
        (
            accountId: string,
            bucketName: string,
            body: InlineUnion_9efd8424a0e61e695ceec0c8,
            ?cfR2Jurisdiction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("bucket_name", bucketName)
                  RequestPart.jsonContent body
                  if cfR2Jurisdiction.IsSome then
                      RequestPart.header ("cf-r2-jurisdiction", cfR2Jurisdiction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/r2/buckets/{bucket_name}/sippy"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2PutBucketSippyConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2PutBucketSippyConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2PutBucketSippyConfig" (int status)
        }

    ///<summary>
    ///Get Storage/Object Count Metrics across all buckets in your account. Note that Account-Level Metrics may not immediately reflect the latest data.
    ///</summary>
    member this.R2GetAccountLevelMetrics(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/r2/metrics" requestParts cancellationToken

            match (int status) with
            | 200 -> return R2GetAccountLevelMetrics.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2GetAccountLevelMetrics.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2GetAccountLevelMetrics" (int status)
        }

    ///<summary>
    ///Creates temporary access credentials on a bucket that can be optionally scoped to prefixes or objects.
    ///</summary>
    member this.R2CreateTempAccessCredentials
        (accountId: string, body: r2_temp_access_creds_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/r2/temp-access-credentials"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return R2CreateTempAccessCredentials.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return R2CreateTempAccessCredentials.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for R2CreateTempAccessCredentials" (int status)
        }

    ///<summary>
    ///List the applications available to an account, both the applications Cloudflare
    ///curates and the custom applications the account has defined.
    ///Results are paginated. Use `filter` and `search` to narrow the list, `order_by` to
    ///sort it, and `fields` to reduce each result to only the properties you need.
    ///The authenticated principal must have access to the account identified by
    ///`account_id`.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="filter">
    ///Filter applications using key:value format. Supported filter keys:
    ///- name: Filter by application name (e.g., name:HR)
    ///- id: Filter by application ID (e.g., id:498)
    ///- human_id: Filter by human-readable ID (e.g., human_id:HR)
    ///- hostname: Filter by hostname or support domain (e.g., hostname:portal.example.com)
    ///- source: Filter by application source name (e.g., source:cloudflare)
    ///- ip_subnet: Filter by IP subnet using CIDR containment — returns applications where any stored subnet contains the search value (e.g., ip_subnet:10.0.1.5/32 matches apps with 10.0.0.0/16)
    ///- category_id: Filter by category ID (e.g., category_id:12).
    ///- category_name: Filter by category name (e.g., category_name:HR).
    ///- supported: Filter by supported Cloudflare product (e.g., supported:ACCESS). Values: GATEWAY, ACCESS, CASB.
    ///- review_status: Filter by the account's Gateway review status. Values: approved, unapproved, in_review, unreviewed.
    ///.
    ///</param>
    ///<param name="limit">Limit of number of results to return (max 250).</param>
    ///<param name="offset">Offset of results to return.</param>
    ///<param name="orderBy">
    ///Order results using field:direction format. Supported fields are name, id, human_id,
    ///category_id, application_type, application_confidence_score, and gen_ai_score.
    ///Supported directions are asc and desc. Ignored when search is provided; results are
    ///ranked by relevance instead.
    ///</param>
    ///<param name="search">Fuzzy search across application name and hostnames. Results are ranked by relevance. Must be between 2 and 200 characters. Can be combined with filter parameters.</param>
    ///<param name="fields">
    ///Return only the listed properties on each application, as a comma-separated list.
    ///Use this to keep responses small when you only need part of each application — for
    ///example populating a picker with `fields=id,name` instead of downloading every
    ///hostname and IP subnet.
    ///Omit this parameter to receive the full application object.
    ///`id` is always returned.
    ///Selectable properties: `id`, `name`, `human_id`, `version`, `hostnames`,
    ///`support_domains`, `ip_subnets`, `port_protocols`, `supported`, `gen_ai_score`,
    ///`application_confidence_score`, `created_at`, `updated_at`, `review_status`.
    ///Unknown or empty property names return `400`.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.GetResourceLibraryApplications
        (
            accountId: string,
            ?filter: string,
            ?limit: int,
            ?offset: int,
            ?orderBy: string,
            ?search: string,
            ?fields: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if filter.IsSome then
                      RequestPart.query ("filter", filter.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if fields.IsSome then
                      RequestPart.query ("fields", fields.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/resource-library/applications"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetResourceLibraryApplications.OK((Serializer.deserialize content))
            | 403 -> return GetResourceLibraryApplications.Forbidden((Serializer.deserialize content))
            | 502 -> return GetResourceLibraryApplications.BadGateway((Serializer.deserialize content))
            | 503 -> return GetResourceLibraryApplications.ServiceUnavailable((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetResourceLibraryApplications.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetResourceLibraryApplications" (int status)
        }

    ///<summary>
    ///Create a custom application for an account.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreateResourceLibraryApplication
        (accountId: string, body: alexandria_create_application_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/resource-library/applications"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return CreateResourceLibraryApplication.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreateResourceLibraryApplication.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateResourceLibraryApplication" (int status)
        }

    ///<summary>
    ///Delete a custom application and all of its versions. Deletion is rejected when other resources reference the application.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="id">Application ID.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteResourceLibraryApplication(accountId: string, id: int64, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/resource-library/applications/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteResourceLibraryApplication.OK((Serializer.deserialize content))
            | 409 -> return DeleteResourceLibraryApplication.Conflict((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteResourceLibraryApplication.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteResourceLibraryApplication" (int status)
        }

    ///<summary>
    ///Get application by ID.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="id">Application ID.</param>
    ///<param name="cancellationToken"></param>
    member this.GetResourceLibraryApplicationById(accountId: string, id: int64, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/resource-library/applications/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetResourceLibraryApplicationById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetResourceLibraryApplicationById.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetResourceLibraryApplicationById" (int status)
        }

    ///<summary>
    ///Replace the network matchers for a custom application and create a new version.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="id">Application ID.</param>
    ///<param name="body">Updates the network matchers for the application. Omitted matcher lists are left unchanged; send an empty array to clear a list.</param>
    ///<param name="cancellationToken"></param>
    member this.UpdateResourceLibraryApplication
        (
            accountId: string,
            id: int64,
            body: alexandria_update_application_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("id", id)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/resource-library/applications/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateResourceLibraryApplication.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateResourceLibraryApplication.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateResourceLibraryApplication" (int status)
        }

    ///<summary>
    ///List application categories.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="limit">Limit of number of results to return.</param>
    ///<param name="offset">Offset of results to return.</param>
    ///<param name="cancellationToken"></param>
    member this.GetResourceLibraryCategories
        (accountId: string, ?limit: int, ?offset: int, ?cancellationToken: CancellationToken)
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
                    "/accounts/{account_id}/resource-library/categories"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetResourceLibraryCategories.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetResourceLibraryCategories.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetResourceLibraryCategories" (int status)
        }

    ///<summary>
    ///Get application category by ID.
    ///</summary>
    ///<param name="accountId">Account ID.</param>
    ///<param name="id">Application category ID.</param>
    ///<param name="cancellationToken"></param>
    member this.GetResourceLibraryCategoryById(accountId: string, id: int64, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.path ("id", id) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/resource-library/categories/{id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetResourceLibraryCategoryById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetResourceLibraryCategoryById.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetResourceLibraryCategoryById" (int status)
        }

    ///<summary>
    ///Lists the number of secrets used in the account.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreQuota(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/quota"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SecretsStoreQuota.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreQuota.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreQuota" (int status)
        }

    ///<summary>
    ///Lists all the stores in an account.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="direction">Direction to sort objects.</param>
    ///<param name="page">Page number.</param>
    ///<param name="perPage">Number of objects to return per page.</param>
    ///<param name="order">Order stores by values in the given field.</param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreList
        (
            accountId: string,
            ?direction: string,
            ?page: int,
            ?perPage: int,
            ?order: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SecretsStoreList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreList" (int status)
        }

    ///<summary>
    ///Creates a store in the account.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreCreate
        (accountId: string, body: secrets_u002D_store_createStoreObject, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SecretsStoreCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreCreate" (int status)
        }

    ///<summary>
    ///Deletes a single store. By default, a store that still contains secrets
    ///cannot be deleted and returns HTTP 409 (Conflict) with the "store_not_empty"
    ///error. Pass `force=true` to cascade-delete all secrets in the store.
    ///Empty stores are always deleted regardless of the force parameter.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="storeId">Store identifier.</param>
    ///<param name="force">
    ///When true, cascade-deletes all secrets in the store before deleting the store itself.
    ///Required when deleting a non-empty store. Without this parameter, attempting to
    ///delete a non-empty store returns 409.
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreDeleteById
        (accountId: string, storeId: string, ?force: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("store_id", storeId)
                  if force.IsSome then
                      RequestPart.query ("force", force.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores/{store_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SecretsStoreDeleteById.OK((Serializer.deserialize content))
            | 409 -> return SecretsStoreDeleteById.Conflict((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreDeleteById.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreDeleteById" (int status)
        }

    ///<summary>
    ///Returns details of a single store.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="storeId">Store identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreGetStoreById(accountId: string, storeId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("store_id", storeId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores/{store_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SecretsStoreGetStoreById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreGetStoreById.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreGetStoreById" (int status)
        }

    ///<summary>
    ///Deletes one or more secrets.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="storeId">Store identifier.</param>
    ///<param name="body">Request body for bulk deleting secrets.</param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreDeleteBulk
        (
            accountId: string,
            storeId: string,
            body: secrets_u002D_store_deleteSecretsRequest,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("store_id", storeId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores/{store_id}/secrets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 202 -> return SecretsStoreDeleteBulk.Accepted((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreDeleteBulk.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreDeleteBulk" (int status)
        }

    ///<summary>
    ///Lists all store secrets.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="storeId">Store identifier.</param>
    ///<param name="direction">Direction to sort objects.</param>
    ///<param name="page">Page number.</param>
    ///<param name="perPage">Number of objects to return per page.</param>
    ///<param name="search">Search secrets using a filter string, filtering across name and comment.</param>
    ///<param name="order">Order secrets by values in the given field.</param>
    ///<param name="scopes">Only secrets with the given scopes will be returned.</param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreSecretsList
        (
            accountId: string,
            storeId: string,
            ?direction: string,
            ?page: int,
            ?perPage: int,
            ?search: string,
            ?order: string,
            ?scopes: list<string>,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("store_id", storeId)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if scopes.IsSome then
                      RequestPart.queryComma ("scopes", scopes.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores/{store_id}/secrets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SecretsStoreSecretsList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreSecretsList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreSecretsList" (int status)
        }

    ///<summary>
    ///Creates a secret in the account.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="storeId">Store identifier.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreSecretCreate
        (
            accountId: string,
            storeId: string,
            body: list<secrets_u002D_store_createSecretObject>,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("store_id", storeId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores/{store_id}/secrets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SecretsStoreSecretCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreSecretCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreSecretCreate" (int status)
        }

    ///<summary>
    ///Deletes a single secret.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="storeId">Store identifier.</param>
    ///<param name="secretId">Secret identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreSecretDeleteById
        (accountId: string, storeId: string, secretId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("store_id", storeId)
                  RequestPart.path ("secret_id", secretId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores/{store_id}/secrets/{secret_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 202 -> return SecretsStoreSecretDeleteById.Accepted((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreSecretDeleteById.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreSecretDeleteById" (int status)
        }

    ///<summary>
    ///Returns details of a single secret.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="storeId">Store identifier.</param>
    ///<param name="secretId">Secret identifier.</param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreGetById
        (accountId: string, storeId: string, secretId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("store_id", storeId)
                  RequestPart.path ("secret_id", secretId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores/{store_id}/secrets/{secret_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SecretsStoreGetById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreGetById.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreGetById" (int status)
        }

    ///<summary>
    ///Updates a single secret.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="storeId">Store identifier.</param>
    ///<param name="secretId">Secret identifier.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStorePatchById
        (
            accountId: string,
            storeId: string,
            secretId: string,
            body: secrets_u002D_store_patchSecretObject,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("store_id", storeId)
                  RequestPart.path ("secret_id", secretId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores/{store_id}/secrets/{secret_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SecretsStorePatchById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStorePatchById.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStorePatchById" (int status)
        }

    ///<summary>
    ///Creates a duplicate of the secret, keeping the value.
    ///</summary>
    ///<param name="accountId">Account identifier.</param>
    ///<param name="storeId">Store identifier.</param>
    ///<param name="secretId">Secret identifier.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.SecretsStoreDuplicateById
        (
            accountId: string,
            storeId: string,
            secretId: string,
            body: secrets_u002D_store_duplicateSecretObject,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("store_id", storeId)
                  RequestPart.path ("secret_id", secretId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/secrets_store/stores/{store_id}/secrets/{secret_id}/duplicate"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SecretsStoreDuplicateById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SecretsStoreDuplicateById.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecretsStoreDuplicateById" (int status)
        }

    ///<summary>
    ///Lists all R2 Super Slurper migration jobs for the account with their status.
    ///</summary>
    member this.SlurperListJobs(accountId: string, ?limit: int, ?offset: int, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/slurper/jobs" requestParts cancellationToken

            match (int status) with
            | 200 -> return SlurperListJobs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperListJobs.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperListJobs" (int status)
        }

    ///<summary>
    ///Creates a new R2 Super Slurper migration job to transfer objects from a source bucket (e.g. S3, GCS, R2) to R2.
    ///</summary>
    member this.SlurperCreateJob
        (accountId: string, body: r2_u002D_slurper_CreateJobRequest, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/slurper/jobs" requestParts cancellationToken

            match (int status) with
            | 201 -> return SlurperCreateJob.Created((Serializer.deserialize content))
            | 409 -> return SlurperCreateJob.Conflict((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperCreateJob.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperCreateJob" (int status)
        }

    ///<summary>
    ///Cancels all running R2 Super Slurper migration jobs for the account. Any objects in the middle of a transfer will finish, but no new objects will start transferring.
    ///</summary>
    member this.SlurperAbortAllJobs(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/slurper/jobs/abortAll"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SlurperAbortAllJobs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperAbortAllJobs.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperAbortAllJobs" (int status)
        }

    ///<summary>
    ///Deletes a completed, aborted, or errored R2 Super Slurper migration job. Active jobs cannot be deleted.
    ///</summary>
    member this.SlurperDeleteJob(accountId: string, jobId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("job_id", jobId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/slurper/jobs/{job_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SlurperDeleteJob.OK((Serializer.deserialize content))
            | 409 -> return SlurperDeleteJob.Conflict((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperDeleteJob.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperDeleteJob" (int status)
        }

    ///<summary>
    ///Retrieves detailed status and configuration for a specific R2 Super Slurper migration job.
    ///</summary>
    member this.SlurperGetJob(accountId: string, jobId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("job_id", jobId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/slurper/jobs/{job_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SlurperGetJob.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperGetJob.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperGetJob" (int status)
        }

    ///<summary>
    ///Cancels a specific R2 Super Slurper migration job. Any objects in the middle of a transfer will finish, but no new objects will start transferring.
    ///</summary>
    member this.SlurperAbortJob(accountId: string, jobId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("job_id", jobId) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/slurper/jobs/{job_id}/abort"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SlurperAbortJob.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperAbortJob.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperAbortJob" (int status)
        }

    ///<summary>
    ///Gets log entries for an R2 Super Slurper migration job, showing migration status changes, errors, etc.
    ///</summary>
    member this.SlurperGetJobLogs
        (accountId: string, jobId: string, ?limit: int, ?offset: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("job_id", jobId)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if offset.IsSome then
                      RequestPart.query ("offset", offset.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/slurper/jobs/{job_id}/logs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SlurperGetJobLogs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperGetJobLogs.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperGetJobLogs" (int status)
        }

    ///<summary>
    ///Pauses a running R2 Super Slurper migration job. The job can be resumed later to continue transferring.
    ///</summary>
    member this.SlurperPauseJob(accountId: string, jobId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("job_id", jobId) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/slurper/jobs/{job_id}/pause"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SlurperPauseJob.OK((Serializer.deserialize content))
            | 409 -> return SlurperPauseJob.Conflict((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperPauseJob.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperPauseJob" (int status)
        }

    ///<summary>
    ///Retrieves current progress metrics for an R2 Super Slurper migration job
    ///</summary>
    member this.SlurperGetJobProgress(accountId: string, jobId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("job_id", jobId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/slurper/jobs/{job_id}/progress"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SlurperGetJobProgress.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperGetJobProgress.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperGetJobProgress" (int status)
        }

    ///<summary>
    ///Resumes a paused R2 Super Slurper migration job, continuing the transfer from where it stopped.
    ///</summary>
    member this.SlurperResumeJob(accountId: string, jobId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("job_id", jobId) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/slurper/jobs/{job_id}/resume"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SlurperResumeJob.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperResumeJob.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperResumeJob" (int status)
        }

    ///<summary>
    ///Check whether tokens are valid against the source bucket
    ///</summary>
    member this.SlurperCheckSourceConnectivity
        (accountId: string, body: r2_u002D_slurper_SourceJobSchema, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/slurper/source/connectivity-precheck"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SlurperCheckSourceConnectivity.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperCheckSourceConnectivity.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperCheckSourceConnectivity" (int status)
        }

    ///<summary>
    ///Check whether tokens are valid against the target bucket
    ///</summary>
    member this.SlurperCheckTargetConnectivity
        (accountId: string, body: r2_u002D_slurper_R2TargetSchema, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/slurper/target/connectivity-precheck"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SlurperCheckTargetConnectivity.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SlurperCheckTargetConnectivity.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SlurperCheckTargetConnectivity" (int status)
        }

    ///<summary>
    ///Returns the namespaces owned by an account.
    ///</summary>
    member this.WorkersKvNamespaceListNamespaces
        (
            accountId: string,
            ?page: float,
            ?perPage: float,
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
                    "/accounts/{account_id}/storage/kv/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceListNamespaces.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersKvNamespaceListNamespaces.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersKvNamespaceListNamespaces" (int status)
        }

    ///<summary>
    ///Creates a namespace under the given title. A `400` is returned if the account already owns a namespace with this title. A namespace must be explicitly deleted to be replaced.
    ///</summary>
    member this.WorkersKvNamespaceCreateANamespace
        (accountId: string, body: workers_u002D_kv_create_namespace_body, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceCreateANamespace.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersKvNamespaceCreateANamespace.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersKvNamespaceCreateANamespace" (int status)
        }

    ///<summary>
    ///Deletes the namespace corresponding to the given ID.
    ///</summary>
    member this.WorkersKvNamespaceRemoveANamespace
        (
            namespaceId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceRemoveANamespace.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersKvNamespaceRemoveANamespace.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersKvNamespaceRemoveANamespace" (int status)
        }

    ///<summary>
    ///Get the namespace corresponding to the given ID.
    ///</summary>
    member this.WorkersKvNamespaceGetANamespace
        (namespaceId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceGetANamespace.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersKvNamespaceGetANamespace.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersKvNamespaceGetANamespace" (int status)
        }

    ///<summary>
    ///Modifies a namespace's title.
    ///</summary>
    member this.WorkersKvNamespaceRenameANamespace
        (
            namespaceId: string,
            accountId: string,
            body: workers_u002D_kv_create_rename_namespace_body,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceRenameANamespace.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersKvNamespaceRenameANamespace.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersKvNamespaceRenameANamespace" (int status)
        }

    ///<summary>
    ///Remove multiple KV pairs from the namespace. Body should be an array of up to 10,000 keys to be removed.
    ///</summary>
    member this.WorkersKvNamespaceDeleteMultipleKeyValuePairsDeprecated
        (
            namespaceId: string,
            accountId: string,
            body: workers_u002D_kv_bulk_delete,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}/bulk"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceDeleteMultipleKeyValuePairsDeprecated.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    WorkersKvNamespaceDeleteMultipleKeyValuePairsDeprecated.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for WorkersKvNamespaceDeleteMultipleKeyValuePairsDeprecated"
                        (int status)
        }

    ///<summary>
    ///Write multiple keys and values at once. Body should be an array of up to 10,000 key-value pairs to be stored, along with optional expiration information. Existing values and expirations will be overwritten. If neither `expiration` nor `expiration_ttl` is specified, the key-value pair will never expire. If both are set, `expiration_ttl` is used and `expiration` is ignored. The entire request size must be 100 megabytes or less.
    ///</summary>
    member this.WorkersKvNamespaceWriteMultipleKeyValuePairs
        (
            namespaceId: string,
            accountId: string,
            body: workers_u002D_kv_bulk_write,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}/bulk"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceWriteMultipleKeyValuePairs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    WorkersKvNamespaceWriteMultipleKeyValuePairs.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for WorkersKvNamespaceWriteMultipleKeyValuePairs" (int status)
        }

    ///<summary>
    ///Remove multiple KV pairs from the namespace. Body should be an array of up to 10,000 keys to be removed.
    ///</summary>
    member this.WorkersKvNamespaceDeleteMultipleKeyValuePairs
        (
            namespaceId: string,
            accountId: string,
            body: workers_u002D_kv_bulk_delete,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}/bulk/delete"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceDeleteMultipleKeyValuePairs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    WorkersKvNamespaceDeleteMultipleKeyValuePairs.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for WorkersKvNamespaceDeleteMultipleKeyValuePairs" (int status)
        }

    ///<summary>
    ///Retrieve up to 100 KV pairs from the namespace. Keys must contain text-based values. JSON values can optionally be parsed instead of being returned as a string value. Metadata can be included if `withMetadata` is true.
    ///</summary>
    member this.WorkersKvNamespaceGetMultipleKeyValuePairs
        (
            namespaceId: string,
            accountId: string,
            body: WorkersKvNamespaceGetMultipleKeyValuePairsPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}/bulk/get"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceGetMultipleKeyValuePairs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    WorkersKvNamespaceGetMultipleKeyValuePairs.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for WorkersKvNamespaceGetMultipleKeyValuePairs" (int status)
        }

    ///<summary>
    ///Lists a namespace's keys.
    ///</summary>
    member this.WorkersKvNamespaceListANamespace'SKeys
        (
            namespaceId: string,
            accountId: string,
            ?limit: float,
            ?prefix: string,
            ?cursor: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if prefix.IsSome then
                      RequestPart.query ("prefix", prefix.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}/keys"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceListANamespace'SKeys.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersKvNamespaceListANamespace'SKeys.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersKvNamespaceListANamespace'SKeys" (int status)
        }

    ///<summary>
    ///Returns the metadata associated with the given key in the given namespace. Use URL-encoding to use special characters (for example, `:`, `!`, `%`) in the key name.
    ///</summary>
    member this.WorkersKvNamespaceReadTheMetadataForAKey
        (keyName: string, namespaceId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("key_name", keyName)
                  RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}/metadata/{key_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceReadTheMetadataForAKey.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersKvNamespaceReadTheMetadataForAKey.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for WorkersKvNamespaceReadTheMetadataForAKey" (int status)
        }

    ///<summary>
    ///Remove a KV pair from the namespace. Use URL-encoding to use special characters (for example, `:`, `!`, `%`) in the key name.
    ///</summary>
    member this.WorkersKvNamespaceDeleteKeyValuePair
        (
            keyName: string,
            namespaceId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("key_name", keyName)
                  RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}/values/{key_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceDeleteKeyValuePair.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WorkersKvNamespaceDeleteKeyValuePair.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersKvNamespaceDeleteKeyValuePair" (int status)
        }

    ///<summary>
    ///Returns the value associated with the given key in the given namespace. Use URL-encoding to use special characters (for example, `:`, `!`, `%`) in the key name. If the KV-pair is set to expire at some point, the expiration time as measured in seconds since the UNIX epoch will be returned in the `expiration` response header.
    ///</summary>
    member this.WorkersKvNamespaceReadKeyValuePair
        (keyName: string, namespaceId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("key_name", keyName)
                  RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, contentBinary) =
                OpenApiHttp.getBinaryAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}/values/{key_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceReadKeyValuePair.OK(contentBinary)
            | _ when (((int status) / 100) = 4) ->
                let content = Encoding.UTF8.GetString contentBinary
                return WorkersKvNamespaceReadKeyValuePair.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WorkersKvNamespaceReadKeyValuePair" (int status)
        }

    ///<summary>
    ///Write a value identified by a key. Use URL-encoding to use special characters (for example, `:`, `!`, `%`) in the key name. Body should be the value to be stored. If JSON metadata to be associated with the key/value pair is needed, use `multipart/form-data` content type for your PUT request (see dropdown below in `REQUEST BODY SCHEMA`). Existing values, expirations, and metadata will be overwritten. If neither `expiration` nor `expiration_ttl` is specified, the key-value pair will never expire. If both are set, `expiration_ttl` is used and `expiration` is ignored.
    ///</summary>
    ///<param name="keyName"></param>
    ///<param name="namespaceId"></param>
    ///<param name="accountId"></param>
    ///<param name="value">A byte sequence to be stored, up to 25 MiB in length.</param>
    ///<param name="expiration"></param>
    ///<param name="expirationTtl"></param>
    ///<param name="cancellationToken"></param>
    ///<param name="metadata"></param>
    member this.WorkersKvNamespaceWriteKeyValuePairWithMetadata
        (
            keyName: string,
            namespaceId: string,
            accountId: string,
            value: MultipartTextOrBinary,
            ?expiration: float,
            ?expirationTtl: float,
            ?cancellationToken: CancellationToken,
            ?metadata: workers_u002D_kv_metadata
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "value"; "metadata" ]
                  RequestPart.path ("key_name", keyName)
                  RequestPart.path ("namespace_id", namespaceId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.multipartTextOrBinary ("value", value)
                  if expiration.IsSome then
                      RequestPart.query ("expiration", expiration.Value)
                  if expirationTtl.IsSome then
                      RequestPart.query ("expiration_ttl", expirationTtl.Value)
                  if metadata.IsSome then
                      RequestPart.multipartJson ("metadata", "application/json", metadata.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/storage/kv/namespaces/{namespace_id}/values/{key_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WorkersKvNamespaceWriteKeyValuePairWithMetadata.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    WorkersKvNamespaceWriteKeyValuePairWithMetadata.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for WorkersKvNamespaceWriteKeyValuePairWithMetadata"
                        (int status)
        }

    ///<summary>
    ///Returns a list of Vectorize Indexes
    ///</summary>
    member this.``Vectorize(Deprecated)ListVectorizeIndexes``
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/indexes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``Vectorize(Deprecated)ListVectorizeIndexes``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``Vectorize(Deprecated)ListVectorizeIndexes``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return failwithf "Unexpected HTTP status %d for Vectorize(Deprecated)ListVectorizeIndexes" (int status)
        }

    ///<summary>
    ///Creates and returns a new Vectorize Index.
    ///</summary>
    member this.``Vectorize(Deprecated)CreateVectorizeIndex``
        (accountId: string, body: vectorize_create_u002D_index_u002D_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/indexes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``Vectorize(Deprecated)CreateVectorizeIndex``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``Vectorize(Deprecated)CreateVectorizeIndex``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return failwithf "Unexpected HTTP status %d for Vectorize(Deprecated)CreateVectorizeIndex" (int status)
        }

    ///<summary>
    ///Deletes the specified Vectorize Index.
    ///</summary>
    member this.``Vectorize(Deprecated)DeleteVectorizeIndex``
        (accountId: string, indexName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/indexes/{index_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``Vectorize(Deprecated)DeleteVectorizeIndex``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``Vectorize(Deprecated)DeleteVectorizeIndex``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return failwithf "Unexpected HTTP status %d for Vectorize(Deprecated)DeleteVectorizeIndex" (int status)
        }

    ///<summary>
    ///Returns the specified Vectorize Index.
    ///</summary>
    member this.``Vectorize(Deprecated)GetVectorizeIndex``
        (accountId: string, indexName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/indexes/{index_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``Vectorize(Deprecated)GetVectorizeIndex``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``Vectorize(Deprecated)GetVectorizeIndex``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Vectorize(Deprecated)GetVectorizeIndex" (int status)
        }

    ///<summary>
    ///Updates and returns the specified Vectorize Index.
    ///</summary>
    member this.``Vectorize(Deprecated)UpdateVectorizeIndex``
        (
            accountId: string,
            indexName: string,
            body: vectorize_update_u002D_index_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/indexes/{index_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``Vectorize(Deprecated)UpdateVectorizeIndex``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``Vectorize(Deprecated)UpdateVectorizeIndex``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return failwithf "Unexpected HTTP status %d for Vectorize(Deprecated)UpdateVectorizeIndex" (int status)
        }

    ///<summary>
    ///Delete a set of vectors from an index by their vector identifiers.
    ///</summary>
    member this.``Vectorize(Deprecated)DeleteVectorsById``
        (
            accountId: string,
            indexName: string,
            body: vectorize_index_u002D_delete_u002D_vectors_u002D_by_u002D_id_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/indexes/{index_name}/delete-by-ids"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``Vectorize(Deprecated)DeleteVectorsById``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``Vectorize(Deprecated)DeleteVectorsById``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Vectorize(Deprecated)DeleteVectorsById" (int status)
        }

    ///<summary>
    ///Get a set of vectors from an index by their vector identifiers.
    ///</summary>
    member this.``Vectorize(Deprecated)GetVectorsById``
        (
            accountId: string,
            indexName: string,
            body: vectorize_index_u002D_get_u002D_vectors_u002D_by_u002D_id_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/indexes/{index_name}/get-by-ids"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``Vectorize(Deprecated)GetVectorsById``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``Vectorize(Deprecated)GetVectorsById``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Vectorize(Deprecated)GetVectorsById" (int status)
        }

    ///<summary>
    ///Inserts vectors into the specified index and returns the count of the vectors successfully inserted.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="indexName"></param>
    ///<param name="body">ndjson file containing vectors to insert.</param>
    ///<param name="cancellationToken"></param>
    member this.``Vectorize(Deprecated)InsertVector``
        (accountId: string, indexName: string, body: byte[], ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.rawContent ("application/x-ndjson", body) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/indexes/{index_name}/insert"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``Vectorize(Deprecated)InsertVector``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``Vectorize(Deprecated)InsertVector``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Vectorize(Deprecated)InsertVector" (int status)
        }

    ///<summary>
    ///Finds vectors closest to a given vector in an index.
    ///</summary>
    member this.``Vectorize(Deprecated)QueryVector``
        (
            accountId: string,
            indexName: string,
            body: vectorize_index_u002D_query_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/indexes/{index_name}/query"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``Vectorize(Deprecated)QueryVector``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``Vectorize(Deprecated)QueryVector``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Vectorize(Deprecated)QueryVector" (int status)
        }

    ///<summary>
    ///Upserts vectors into the specified index, creating them if they do not exist and returns the count of values and ids successfully inserted.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="indexName"></param>
    ///<param name="body">ndjson file containing vectors to upsert.</param>
    ///<param name="cancellationToken"></param>
    member this.``Vectorize(Deprecated)UpsertVector``
        (accountId: string, indexName: string, body: byte[], ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.rawContent ("application/x-ndjson", body) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/indexes/{index_name}/upsert"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``Vectorize(Deprecated)UpsertVector``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``Vectorize(Deprecated)UpsertVector``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Vectorize(Deprecated)UpsertVector" (int status)
        }

    ///<summary>
    ///Returns a list of Vectorize Indexes
    ///</summary>
    member this.VectorizeListVectorizeIndexes(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeListVectorizeIndexes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeListVectorizeIndexes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeListVectorizeIndexes" (int status)
        }

    ///<summary>
    ///Creates and returns a new Vectorize Index.
    ///</summary>
    member this.VectorizeCreateVectorizeIndex
        (accountId: string, body: vectorize_create_u002D_index_u002D_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeCreateVectorizeIndex.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeCreateVectorizeIndex.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeCreateVectorizeIndex" (int status)
        }

    ///<summary>
    ///Deletes the specified Vectorize Index.
    ///</summary>
    member this.VectorizeDeleteVectorizeIndex
        (accountId: string, indexName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeDeleteVectorizeIndex.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeDeleteVectorizeIndex.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeDeleteVectorizeIndex" (int status)
        }

    ///<summary>
    ///Returns the specified Vectorize Index.
    ///</summary>
    member this.VectorizeGetVectorizeIndex
        (accountId: string, indexName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeGetVectorizeIndex.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeGetVectorizeIndex.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeGetVectorizeIndex" (int status)
        }

    ///<summary>
    ///Delete a set of vectors from an index by their vector identifiers.
    ///</summary>
    member this.VectorizeDeleteVectorsById
        (
            accountId: string,
            indexName: string,
            body: vectorize_index_u002D_delete_u002D_vectors_u002D_by_u002D_id_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}/delete_by_ids"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeDeleteVectorsById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeDeleteVectorsById.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeDeleteVectorsById" (int status)
        }

    ///<summary>
    ///Get a set of vectors from an index by their vector identifiers.
    ///</summary>
    member this.VectorizeGetVectorsById
        (
            accountId: string,
            indexName: string,
            body: vectorize_index_u002D_get_u002D_vectors_u002D_by_u002D_id_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}/get_by_ids"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeGetVectorsById.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeGetVectorsById.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeGetVectorsById" (int status)
        }

    ///<summary>
    ///Get information about a vectorize index.
    ///</summary>
    member this.VectorizeIndexInfo(accountId: string, indexName: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}/info"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeIndexInfo.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeIndexInfo.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeIndexInfo" (int status)
        }

    ///<summary>
    ///Inserts vectors into the specified index and returns a mutation id corresponding to the vectors enqueued for insertion.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="indexName"></param>
    ///<param name="body">ndjson file containing vectors to insert.</param>
    ///<param name="unparsableBehavior"></param>
    ///<param name="cancellationToken"></param>
    member this.VectorizeInsertVector
        (
            accountId: string,
            indexName: string,
            body: byte[],
            ?unparsableBehavior: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.rawContent ("application/x-ndjson", body)
                  if unparsableBehavior.IsSome then
                      RequestPart.query ("unparsable-behavior", unparsableBehavior.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}/insert"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeInsertVector.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeInsertVector.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeInsertVector" (int status)
        }

    ///<summary>
    ///Returns a paginated list of vector identifiers from the specified index.
    ///</summary>
    member this.VectorizeListVectors
        (accountId: string, indexName: string, ?count: int, ?cursor: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  if count.IsSome then
                      RequestPart.query ("count", count.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}/list"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeListVectors.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeListVectors.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeListVectors" (int status)
        }

    ///<summary>
    ///Enable metadata filtering based on metadata property. Limited to 10 properties.
    ///</summary>
    member this.VectorizeCreateMetadataIndex
        (
            accountId: string,
            indexName: string,
            body: vectorize_create_u002D_metadata_u002D_index_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}/metadata_index/create"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeCreateMetadataIndex.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeCreateMetadataIndex.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeCreateMetadataIndex" (int status)
        }

    ///<summary>
    ///Allow Vectorize to delete the specified metadata index.
    ///</summary>
    member this.VectorizeDeleteMetadataIndex
        (
            accountId: string,
            indexName: string,
            body: vectorize_delete_u002D_metadata_u002D_index_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}/metadata_index/delete"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeDeleteMetadataIndex.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeDeleteMetadataIndex.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeDeleteMetadataIndex" (int status)
        }

    ///<summary>
    ///List Metadata Indexes for the specified Vectorize Index.
    ///</summary>
    member this.VectorizeListMetadataIndexes
        (accountId: string, indexName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}/metadata_index/list"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeListMetadataIndexes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeListMetadataIndexes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeListMetadataIndexes" (int status)
        }

    ///<summary>
    ///Finds vectors closest to a given vector in an index.
    ///</summary>
    member this.VectorizeQueryVector
        (
            accountId: string,
            indexName: string,
            body: vectorize_index_u002D_query_u002D_v2_u002D_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}/query"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeQueryVector.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeQueryVector.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeQueryVector" (int status)
        }

    ///<summary>
    ///Upserts vectors into the specified index, creating them if they do not exist and returns a mutation id corresponding to the vectors enqueued for upsertion.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="indexName"></param>
    ///<param name="body">ndjson file containing vectors to upsert.</param>
    ///<param name="unparsableBehavior"></param>
    ///<param name="cancellationToken"></param>
    member this.VectorizeUpsertVector
        (
            accountId: string,
            indexName: string,
            body: byte[],
            ?unparsableBehavior: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("index_name", indexName)
                  RequestPart.rawContent ("application/x-ndjson", body)
                  if unparsableBehavior.IsSome then
                      RequestPart.query ("unparsable-behavior", unparsableBehavior.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/vectorize/v2/indexes/{index_name}/upsert"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return VectorizeUpsertVector.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return VectorizeUpsertVector.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for VectorizeUpsertVector" (int status)
        }
