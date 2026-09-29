namespace rec FSharp.CloudEdge.Management.Networking

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
type NetworkingClient(httpClient: HttpClient) =
    ///<summary>
    ///Fetches all the custom pages at the account level.
    ///</summary>
    member this.CustomPagesForAnAccountListCustomPages
        (accountIdentifier: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_identifier", accountIdentifier) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_identifier}/custom_pages"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomPagesForAnAccountListCustomPages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomPagesForAnAccountListCustomPages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomPagesForAnAccountListCustomPages" (int status)
        }

    ///<summary>
    ///Fetches all the custom assets at the account level.
    ///</summary>
    member this.CustomAssetsForAnAccountListCustomAssets
        (accountIdentifier: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_identifier", accountIdentifier)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_identifier}/custom_pages/assets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomAssetsForAnAccountListCustomAssets.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomAssetsForAnAccountListCustomAssets.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CustomAssetsForAnAccountListCustomAssets" (int status)
        }

    ///<summary>
    ///Creates a new custom asset at the account level.
    ///</summary>
    member this.CustomAssetsForAnAccountCreateACustomAsset
        (
            accountIdentifier: string,
            body: CustomAssetsForAnAccountCreateACustomAssetPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_identifier", accountIdentifier)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_identifier}/custom_pages/assets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomAssetsForAnAccountCreateACustomAsset.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CustomAssetsForAnAccountCreateACustomAsset.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CustomAssetsForAnAccountCreateACustomAsset" (int status)
        }

    ///<summary>
    ///Deletes an existing custom asset.
    ///</summary>
    member this.CustomAssetsForAnAccountDeleteACustomAsset
        (assetName: string, accountIdentifier: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("asset_name", assetName)
                  RequestPart.path ("account_identifier", accountIdentifier) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_identifier}/custom_pages/assets/{asset_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 204 -> return CustomAssetsForAnAccountDeleteACustomAsset.NoContent
            | _ when (((int status) / 100) = 4) ->
                return
                    CustomAssetsForAnAccountDeleteACustomAsset.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CustomAssetsForAnAccountDeleteACustomAsset" (int status)
        }

    ///<summary>
    ///Fetches the details of a custom asset.
    ///</summary>
    member this.CustomAssetsForAnAccountGetACustomAsset
        (assetName: string, accountIdentifier: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("asset_name", assetName)
                  RequestPart.path ("account_identifier", accountIdentifier) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_identifier}/custom_pages/assets/{asset_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomAssetsForAnAccountGetACustomAsset.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomAssetsForAnAccountGetACustomAsset.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomAssetsForAnAccountGetACustomAsset" (int status)
        }

    ///<summary>
    ///Updates the configuration of an existing custom asset.
    ///</summary>
    member this.CustomAssetsForAnAccountUpdateACustomAsset
        (
            assetName: string,
            accountIdentifier: string,
            body: CustomAssetsForAnAccountUpdateACustomAssetPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("asset_name", assetName)
                  RequestPart.path ("account_identifier", accountIdentifier)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_identifier}/custom_pages/assets/{asset_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomAssetsForAnAccountUpdateACustomAsset.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CustomAssetsForAnAccountUpdateACustomAsset.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CustomAssetsForAnAccountUpdateACustomAsset" (int status)
        }

    ///<summary>
    ///Creates a signed JWT token used to preview custom pages before they are published.
    ///</summary>
    member this.CustomPagesForAnAccountCreatePreviewToken
        (accountIdentifier: string, body: custom_u002D_pages_preview_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_identifier", accountIdentifier)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_identifier}/custom_pages/preview_tokens"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomPagesForAnAccountCreatePreviewToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomPagesForAnAccountCreatePreviewToken.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CustomPagesForAnAccountCreatePreviewToken" (int status)
        }

    ///<summary>
    ///Fetches the details of a custom page.
    ///</summary>
    member this.CustomPagesForAnAccountGetACustomPage
        (identifier: string, accountIdentifier: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_identifier", accountIdentifier) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_identifier}/custom_pages/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomPagesForAnAccountGetACustomPage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomPagesForAnAccountGetACustomPage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomPagesForAnAccountGetACustomPage" (int status)
        }

    ///<summary>
    ///Updates the configuration of an existing custom page.
    ///</summary>
    member this.CustomPagesForAnAccountUpdateACustomPage
        (
            identifier: string,
            accountIdentifier: string,
            body: CustomPagesForAnAccountUpdateACustomPagePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("account_identifier", accountIdentifier)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_identifier}/custom_pages/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomPagesForAnAccountUpdateACustomPage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomPagesForAnAccountUpdateACustomPage.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CustomPagesForAnAccountUpdateACustomPage" (int status)
        }

    ///<summary>
    ///List all address maps owned by the account.
    ///</summary>
    member this.IpAddressManagementAddressMapsListAddressMaps
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementAddressMapsListAddressMaps.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsListAddressMaps.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for IpAddressManagementAddressMapsListAddressMaps" (int status)
        }

    ///<summary>
    ///Create a new address map under the account.
    ///</summary>
    member this.IpAddressManagementAddressMapsCreateAddressMap
        (
            accountId: string,
            body: IpAddressManagementAddressMapsCreateAddressMapPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementAddressMapsCreateAddressMap.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsCreateAddressMap.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementAddressMapsCreateAddressMap"
                        (int status)
        }

    ///<summary>
    ///Delete a particular address map owned by the account. An Address Map must be disabled before it can be deleted.
    ///</summary>
    member this.IpAddressManagementAddressMapsDeleteAddressMap
        (
            addressMapId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("address_map_id", addressMapId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps/{address_map_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementAddressMapsDeleteAddressMap.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsDeleteAddressMap.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementAddressMapsDeleteAddressMap"
                        (int status)
        }

    ///<summary>
    ///Show a particular address map owned by the account.
    ///</summary>
    member this.IpAddressManagementAddressMapsAddressMapDetails
        (addressMapId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("address_map_id", addressMapId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps/{address_map_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementAddressMapsAddressMapDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsAddressMapDetails.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementAddressMapsAddressMapDetails"
                        (int status)
        }

    ///<summary>
    ///Modify properties of an address map owned by the account.
    ///</summary>
    member this.IpAddressManagementAddressMapsUpdateAddressMap
        (
            addressMapId: string,
            accountId: string,
            body: IpAddressManagementAddressMapsUpdateAddressMapPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("address_map_id", addressMapId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps/{address_map_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementAddressMapsUpdateAddressMap.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsUpdateAddressMap.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementAddressMapsUpdateAddressMap"
                        (int status)
        }

    ///<summary>
    ///Remove an account as a member of a particular address map.
    ///</summary>
    member this.IpAddressManagementAddressMapsRemoveAnAccountMembershipFromAnAddressMap
        (
            accountId: string,
            addressMapId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("address_map_id", addressMapId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps/{address_map_id}/accounts/{account_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    IpAddressManagementAddressMapsRemoveAnAccountMembershipFromAnAddressMap.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsRemoveAnAccountMembershipFromAnAddressMap.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementAddressMapsRemoveAnAccountMembershipFromAnAddressMap"
                        (int status)
        }

    ///<summary>
    ///Add an account as a member of a particular address map.
    ///</summary>
    member this.IpAddressManagementAddressMapsAddAnAccountMembershipToAnAddressMap
        (
            accountId: string,
            addressMapId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("address_map_id", addressMapId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps/{address_map_id}/accounts/{account_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    IpAddressManagementAddressMapsAddAnAccountMembershipToAnAddressMap.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsAddAnAccountMembershipToAnAddressMap.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementAddressMapsAddAnAccountMembershipToAnAddressMap"
                        (int status)
        }

    ///<summary>
    ///Remove an IP from a particular address map.
    ///</summary>
    member this.IpAddressManagementAddressMapsRemoveAnIpFromAnAddressMap
        (
            ipAddress: string,
            addressMapId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("ip_address", ipAddress)
                  RequestPart.path ("address_map_id", addressMapId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps/{address_map_id}/ips/{ip_address}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return IpAddressManagementAddressMapsRemoveAnIpFromAnAddressMap.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsRemoveAnIpFromAnAddressMap.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementAddressMapsRemoveAnIpFromAnAddressMap"
                        (int status)
        }

    ///<summary>
    ///Add an IP from a prefix owned by the account to a particular address map.
    ///</summary>
    member this.IpAddressManagementAddressMapsAddAnIpToAnAddressMap
        (
            ipAddress: string,
            addressMapId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("ip_address", ipAddress)
                  RequestPart.path ("address_map_id", addressMapId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps/{address_map_id}/ips/{ip_address}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementAddressMapsAddAnIpToAnAddressMap.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsAddAnIpToAnAddressMap.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementAddressMapsAddAnIpToAnAddressMap"
                        (int status)
        }

    ///<summary>
    ///Remove a zone as a member of a particular address map.
    ///</summary>
    member this.IpAddressManagementAddressMapsRemoveAZoneMembershipFromAnAddressMap
        (
            zoneId: string,
            addressMapId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("address_map_id", addressMapId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps/{address_map_id}/zones/{zone_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    IpAddressManagementAddressMapsRemoveAZoneMembershipFromAnAddressMap.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsRemoveAZoneMembershipFromAnAddressMap.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementAddressMapsRemoveAZoneMembershipFromAnAddressMap"
                        (int status)
        }

    ///<summary>
    ///Add a zone as a member of a particular address map.
    ///</summary>
    member this.IpAddressManagementAddressMapsAddAZoneMembershipToAnAddressMap
        (
            zoneId: string,
            addressMapId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("address_map_id", addressMapId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/addressing/address_maps/{address_map_id}/zones/{zone_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    IpAddressManagementAddressMapsAddAZoneMembershipToAnAddressMap.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementAddressMapsAddAZoneMembershipToAnAddressMap.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementAddressMapsAddAZoneMembershipToAnAddressMap"
                        (int status)
        }

    ///<summary>
    ///List all leases owned by the account.
    ///</summary>
    member this.IpAddressManagementListLeases(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/leases"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementListLeases.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAddressManagementListLeases.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for IpAddressManagementListLeases" (int status)
        }

    ///<summary>
    ///Submit LOA document (pdf format) under the account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="loaDocument">LOA document to upload.</param>
    ///<param name="cancellationToken"></param>
    member this.IpAddressManagementPrefixesUploadLoaDocument
        (accountId: string, loaDocument: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "loa_document" ]
                  RequestPart.path ("account_id", accountId)
                  RequestPart.multipartScalar ("loa_document", "text/plain", loaDocument) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/addressing/loa_documents"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return IpAddressManagementPrefixesUploadLoaDocument.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementPrefixesUploadLoaDocument.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesUploadLoaDocument" (int status)
        }

    ///<summary>
    ///Download specified LOA document under the account.
    ///</summary>
    member this.IpAddressManagementPrefixesDownloadLoaDocument
        (loaDocumentId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("loa_document_id", loaDocumentId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, contentBinary) =
                OpenApiHttp.getBinaryAsync
                    httpClient
                    "/accounts/{account_id}/addressing/loa_documents/{loa_document_id}/download"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementPrefixesDownloadLoaDocument.OK(contentBinary)
            | _ when (((int status) / 100) = 4) ->
                let content = Encoding.UTF8.GetString contentBinary

                return
                    IpAddressManagementPrefixesDownloadLoaDocument.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementPrefixesDownloadLoaDocument"
                        (int status)
        }

    ///<summary>
    ///List all prefixes owned by the account.
    ///</summary>
    member this.IpAddressManagementPrefixesListPrefixes(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementPrefixesListPrefixes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAddressManagementPrefixesListPrefixes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesListPrefixes" (int status)
        }

    ///<summary>
    ///Add a new prefix under the account.
    ///</summary>
    member this.IpAddressManagementPrefixesAddPrefix
        (accountId: string, body: IpAddressManagementPrefixesAddPrefixPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return IpAddressManagementPrefixesAddPrefix.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAddressManagementPrefixesAddPrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesAddPrefix" (int status)
        }

    ///<summary>
    ///Delete an unapproved prefix owned by the account.
    ///</summary>
    member this.IpAddressManagementPrefixesDeletePrefix
        (
            prefixId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementPrefixesDeletePrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAddressManagementPrefixesDeletePrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesDeletePrefix" (int status)
        }

    ///<summary>
    ///List a particular prefix owned by the account.
    ///</summary>
    member this.IpAddressManagementPrefixesPrefixDetails
        (prefixId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementPrefixesPrefixDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAddressManagementPrefixesPrefixDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesPrefixDetails" (int status)
        }

    ///<summary>
    ///Modify the description for a prefix owned by the account.
    ///</summary>
    member this.IpAddressManagementPrefixesUpdatePrefixDescription
        (
            prefixId: string,
            accountId: string,
            body: IpAddressManagementPrefixesUpdatePrefixDescriptionPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementPrefixesUpdatePrefixDescription.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementPrefixesUpdatePrefixDescription.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementPrefixesUpdatePrefixDescription"
                        (int status)
        }

    ///<summary>
    ///List all BGP Prefixes within the specified IP Prefix. BGP Prefixes are used to control which specific subnets are advertised to the Internet. It is possible to advertise subnets more specific than an IP Prefix by creating more specific BGP Prefixes.
    ///</summary>
    member this.IpAddressManagementPrefixesListBgpPrefixes
        (accountId: string, prefixId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bgp/prefixes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementPrefixesListBgpPrefixes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementPrefixesListBgpPrefixes.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesListBgpPrefixes" (int status)
        }

    ///<summary>
    ///Create a BGP prefix, controlling the BGP advertisement status of a specific subnet. When created, BGP prefixes are initially withdrawn, and can be advertised with the Update BGP Prefix API.
    ///</summary>
    member this.IpAddressManagementPrefixesCreateBgpPrefix
        (accountId: string, prefixId: string, body: addressing_bgp_prefix_create, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bgp/prefixes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementPrefixesCreateBgpPrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementPrefixesCreateBgpPrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesCreateBgpPrefix" (int status)
        }

    ///<summary>
    ///Delete a BGP Prefix associated with the specified IP Prefix. A BGP Prefix must be withdrawn before it can be deleted.
    ///</summary>
    member this.IpAddressManagementPrefixesDeleteBgpPrefix
        (accountId: string, prefixId: string, bgpPrefixId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("bgp_prefix_id", bgpPrefixId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bgp/prefixes/{bgp_prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementPrefixesDeleteBgpPrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementPrefixesDeleteBgpPrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesDeleteBgpPrefix" (int status)
        }

    ///<summary>
    ///Retrieve a single BGP Prefix according to its identifier
    ///</summary>
    member this.IpAddressManagementPrefixesFetchBgpPrefix
        (accountId: string, prefixId: string, bgpPrefixId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("bgp_prefix_id", bgpPrefixId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bgp/prefixes/{bgp_prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementPrefixesFetchBgpPrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAddressManagementPrefixesFetchBgpPrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesFetchBgpPrefix" (int status)
        }

    ///<summary>
    ///Update the properties of a BGP Prefix, such as the on demand advertisement status (advertised or withdrawn).
    ///</summary>
    member this.IpAddressManagementPrefixesUpdateBgpPrefix
        (
            accountId: string,
            prefixId: string,
            bgpPrefixId: string,
            body: addressing_bgp_prefix_update_advertisement,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("bgp_prefix_id", bgpPrefixId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bgp/prefixes/{bgp_prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementPrefixesUpdateBgpPrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementPrefixesUpdateBgpPrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesUpdateBgpPrefix" (int status)
        }

    ///<summary>
    ///View the current advertisement state for a prefix.
    ///**Deprecated:** Prefer the BGP Prefixes endpoints, which additionally allow for advertising and withdrawing
    ///subnets of an IP prefix.
    ///</summary>
    member this.IpAddressManagementDynamicAdvertisementGetAdvertisementStatus
        (prefixId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bgp/status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    IpAddressManagementDynamicAdvertisementGetAdvertisementStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementDynamicAdvertisementGetAdvertisementStatus.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementDynamicAdvertisementGetAdvertisementStatus"
                        (int status)
        }

    ///<summary>
    ///Advertise or withdraw the BGP route for a prefix.
    ///**Deprecated:** Prefer the BGP Prefixes endpoints, which additionally allow for advertising and withdrawing
    ///subnets of an IP prefix.
    ///</summary>
    member this.IpAddressManagementDynamicAdvertisementUpdatePrefixDynamicAdvertisementStatus
        (
            prefixId: string,
            accountId: string,
            body: IpAddressManagementDynamicAdvertisementUpdatePrefixDynamicAdvertisementStatusPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bgp/status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    IpAddressManagementDynamicAdvertisementUpdatePrefixDynamicAdvertisementStatus.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementDynamicAdvertisementUpdatePrefixDynamicAdvertisementStatus.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementDynamicAdvertisementUpdatePrefixDynamicAdvertisementStatus"
                        (int status)
        }

    ///<summary>
    ///List the Cloudflare services this prefix is currently bound to. Traffic sent to an address within an IP prefix will be routed to the Cloudflare service of the most-specific Service Binding matching the address.
    ///**Example:** binding `192.0.2.0/24` to Cloudflare Magic Transit and `192.0.2.1/32` to the Cloudflare CDN would route traffic for `192.0.2.1` to the CDN, and traffic for all other IPs in the prefix to Cloudflare Magic Transit.
    ///</summary>
    member this.IpAddressManagementServiceBindingsListServiceBindings
        (accountId: string, prefixId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bindings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementServiceBindingsListServiceBindings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementServiceBindingsListServiceBindings.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementServiceBindingsListServiceBindings"
                        (int status)
        }

    ///<summary>
    ///Creates a new Service Binding, routing traffic to IPs within the given CIDR to a service running on Cloudflare's network.
    ///**NOTE:** The first Service Binding created for an IP Prefix must exactly match the IP Prefix's CIDR. Subsequent Service Bindings may be created with a more-specific CIDR. Refer to the  [Service Bindings Documentation](https://developers.cloudflare.com/byoip/service-bindings/) for compatibility details.
    ///</summary>
    member this.IpAddressManagementServiceBindingsCreateServiceBinding
        (
            accountId: string,
            prefixId: string,
            ?cancellationToken: CancellationToken,
            ?body: addressing_create_binding_request
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bindings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 ->
                return IpAddressManagementServiceBindingsCreateServiceBinding.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementServiceBindingsCreateServiceBinding.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementServiceBindingsCreateServiceBinding"
                        (int status)
        }

    ///<summary>
    ///Delete a Service Binding
    ///</summary>
    member this.IpAddressManagementServiceBindingsDeleteServiceBinding
        (accountId: string, prefixId: string, bindingId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("binding_id", bindingId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bindings/{binding_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementServiceBindingsDeleteServiceBinding.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementServiceBindingsDeleteServiceBinding.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementServiceBindingsDeleteServiceBinding"
                        (int status)
        }

    ///<summary>
    ///Fetch a single Service Binding
    ///</summary>
    member this.IpAddressManagementServiceBindingsGetServiceBinding
        (accountId: string, prefixId: string, bindingId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("binding_id", bindingId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/bindings/{binding_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementServiceBindingsGetServiceBinding.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementServiceBindingsGetServiceBinding.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementServiceBindingsGetServiceBinding"
                        (int status)
        }

    ///<summary>
    ///List all delegations for a given account IP prefix.
    ///</summary>
    member this.IpAddressManagementPrefixDelegationListPrefixDelegations
        (prefixId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/delegations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return IpAddressManagementPrefixDelegationListPrefixDelegations.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementPrefixDelegationListPrefixDelegations.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementPrefixDelegationListPrefixDelegations"
                        (int status)
        }

    ///<summary>
    ///Create a new account delegation for a given IP prefix.
    ///</summary>
    member this.IpAddressManagementPrefixDelegationCreatePrefixDelegation
        (
            prefixId: string,
            accountId: string,
            body: IpAddressManagementPrefixDelegationCreatePrefixDelegationPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/delegations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return IpAddressManagementPrefixDelegationCreatePrefixDelegation.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementPrefixDelegationCreatePrefixDelegation.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementPrefixDelegationCreatePrefixDelegation"
                        (int status)
        }

    ///<summary>
    ///Delete an account delegation for a given IP prefix.
    ///</summary>
    member this.IpAddressManagementPrefixDelegationDeletePrefixDelegation
        (
            delegationId: string,
            prefixId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("delegation_id", delegationId)
                  RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/delegations/{delegation_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return IpAddressManagementPrefixDelegationDeletePrefixDelegation.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementPrefixDelegationDeletePrefixDelegation.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementPrefixDelegationDeletePrefixDelegation"
                        (int status)
        }

    ///<summary>
    ///Triggers a new prefix validation. The checks are run asynchronously and include IRR, RPKI, and prefix ownership.
    ///</summary>
    member this.IpAddressManagementPrefixesValidatePrefix
        (prefixId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("prefix_id", prefixId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/addressing/prefixes/{prefix_id}/validate"
                    requestParts
                    cancellationToken

            match (int status) with
            | 202 -> return IpAddressManagementPrefixesValidatePrefix.Accepted((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return IpAddressManagementPrefixesValidatePrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for IpAddressManagementPrefixesValidatePrefix" (int status)
        }

    ///<summary>
    ///List all Regional Services regions available for use by this account.
    ///</summary>
    member this.DlsAccountRegionalHostnamesListRegions(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/regional_hostnames/regions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DlsAccountRegionalHostnamesListRegions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DlsAccountRegionalHostnamesListRegions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DlsAccountRegionalHostnamesListRegions" (int status)
        }

    ///<summary>
    ///Bring-Your-Own IP (BYOIP) prefixes onboarded to Cloudflare must be bound to a service running on the Cloudflare network to enable a Cloudflare product on the IP addresses. This endpoint can be used as a reference of available services on the Cloudflare network, and their service IDs.
    ///</summary>
    member this.IpAddressManagementServiceBindingsListServices
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/addressing/services"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return IpAddressManagementServiceBindingsListServices.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    IpAddressManagementServiceBindingsListServices.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for IpAddressManagementServiceBindingsListServices"
                        (int status)
        }

    ///<summary>
    ///Lists and filters Cloudflare Tunnels in an account.
    ///</summary>
    member this.CloudflareTunnelListCloudflareTunnels
        (
            accountId: string,
            ?name: string,
            ?isDeleted: bool,
            ?existedAt: string,
            ?uuid: System.Guid,
            ?wasActiveAt: System.DateTimeOffset,
            ?wasInactiveAt: System.DateTimeOffset,
            ?includePrefix: string,
            ?excludePrefix: string,
            ?status: string,
            ?perPage: float,
            ?page: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if isDeleted.IsSome then
                      RequestPart.query ("is_deleted", isDeleted.Value)
                  if existedAt.IsSome then
                      RequestPart.query ("existed_at", existedAt.Value)
                  if uuid.IsSome then
                      RequestPart.query ("uuid", uuid.Value)
                  if wasActiveAt.IsSome then
                      RequestPart.query ("was_active_at", wasActiveAt.Value)
                  if wasInactiveAt.IsSome then
                      RequestPart.query ("was_inactive_at", wasInactiveAt.Value)
                  if includePrefix.IsSome then
                      RequestPart.query ("include_prefix", includePrefix.Value)
                  if excludePrefix.IsSome then
                      RequestPart.query ("exclude_prefix", excludePrefix.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/cfd_tunnel" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelListCloudflareTunnels.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareTunnelListCloudflareTunnels.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareTunnelListCloudflareTunnels" (int status)
        }

    ///<summary>
    ///Creates a remotely or locally managed Cloudflare Tunnel in an account. After creation, retrieve its token and run cloudflared to establish the connector connection.
    ///</summary>
    member this.CloudflareTunnelCreateACloudflareTunnel
        (accountId: string, body: CloudflareTunnelCreateACloudflareTunnelPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/cfd_tunnel" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelCreateACloudflareTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareTunnelCreateACloudflareTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareTunnelCreateACloudflareTunnel" (int status)
        }

    ///<summary>
    ///Permanently deletes a Cloudflare Tunnel from an account. The tunnel must have no active connections.
    ///</summary>
    member this.CloudflareTunnelDeleteACloudflareTunnel
        (accountId: string, tunnelId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/cfd_tunnel/{tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelDeleteACloudflareTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareTunnelDeleteACloudflareTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareTunnelDeleteACloudflareTunnel" (int status)
        }

    ///<summary>
    ///Fetches a single Cloudflare Tunnel.
    ///</summary>
    member this.CloudflareTunnelGetACloudflareTunnel
        (accountId: string, tunnelId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/cfd_tunnel/{tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelGetACloudflareTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareTunnelGetACloudflareTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareTunnelGetACloudflareTunnel" (int status)
        }

    ///<summary>
    ///Updates the name or secret of an existing Cloudflare Tunnel.
    ///</summary>
    member this.CloudflareTunnelUpdateACloudflareTunnel
        (
            tunnelId: System.Guid,
            accountId: string,
            body: CloudflareTunnelUpdateACloudflareTunnelPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("tunnel_id", tunnelId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/cfd_tunnel/{tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelUpdateACloudflareTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareTunnelUpdateACloudflareTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareTunnelUpdateACloudflareTunnel" (int status)
        }

    ///<summary>
    ///Retrieves the configuration for a remotely managed Cloudflare Tunnel.
    ///</summary>
    member this.CloudflareTunnelConfigurationGetConfiguration
        (accountId: string, tunnelId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/cfd_tunnel/{tunnel_id}/configurations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelConfigurationGetConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelConfigurationGetConfiguration.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for CloudflareTunnelConfigurationGetConfiguration" (int status)
        }

    ///<summary>
    ///Replaces the configuration for a remotely managed Cloudflare Tunnel, including its ingress rules and origin request settings.
    ///</summary>
    member this.CloudflareTunnelConfigurationPutConfiguration
        (
            accountId: string,
            tunnelId: System.Guid,
            body: CloudflareTunnelConfigurationPutConfigurationPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/cfd_tunnel/{tunnel_id}/configurations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelConfigurationPutConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelConfigurationPutConfiguration.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for CloudflareTunnelConfigurationPutConfiguration" (int status)
        }

    ///<summary>
    ///Removes a connection (aka Cloudflare Tunnel Connector) from a Cloudflare Tunnel independently of its current state. If no connector id (client_id) is provided all connectors will be removed. We recommend running this command after rotating tokens.
    ///</summary>
    member this.CloudflareTunnelCleanUpCloudflareTunnelConnections
        (accountId: string, tunnelId: System.Guid, ?clientId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId)
                  if clientId.IsSome then
                      RequestPart.query ("client_id", clientId.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/cfd_tunnel/{tunnel_id}/connections"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelCleanUpCloudflareTunnelConnections.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelCleanUpCloudflareTunnelConnections.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareTunnelCleanUpCloudflareTunnelConnections"
                        (int status)
        }

    ///<summary>
    ///Lists the connections for a Cloudflare Tunnel, including connector IDs, cloudflared versions, and Cloudflare locations.
    ///</summary>
    member this.CloudflareTunnelListCloudflareTunnelConnections
        (accountId: string, tunnelId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/cfd_tunnel/{tunnel_id}/connections"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelListCloudflareTunnelConnections.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelListCloudflareTunnelConnections.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareTunnelListCloudflareTunnelConnections"
                        (int status)
        }

    ///<summary>
    ///Retrieves a connector and its connection details for a Cloudflare Tunnel, including its cloudflared version, architecture, and connected Cloudflare locations.
    ///</summary>
    member this.CloudflareTunnelGetCloudflareTunnelConnector
        (accountId: string, tunnelId: System.Guid, connectorId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId)
                  RequestPart.path ("connector_id", connectorId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/cfd_tunnel/{tunnel_id}/connectors/{connector_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelGetCloudflareTunnelConnector.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelGetCloudflareTunnelConnector.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for CloudflareTunnelGetCloudflareTunnelConnector" (int status)
        }

    ///<summary>
    ///Creates a short-lived management token for the requested Tunnel management resources, such as streaming logs. Treat the token as a secret.
    ///</summary>
    member this.CloudflareTunnelGetACloudflareTunnelManagementToken
        (
            accountId: string,
            tunnelId: System.Guid,
            body: CloudflareTunnelGetACloudflareTunnelManagementTokenPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/cfd_tunnel/{tunnel_id}/management"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelGetACloudflareTunnelManagementToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelGetACloudflareTunnelManagementToken.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareTunnelGetACloudflareTunnelManagementToken"
                        (int status)
        }

    ///<summary>
    ///Retrieves the token used to run cloudflared and associate it with a specific Cloudflare Tunnel. Treat the token as a secret.
    ///</summary>
    member this.CloudflareTunnelGetACloudflareTunnelToken
        (accountId: string, tunnelId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/cfd_tunnel/{tunnel_id}/token"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelGetACloudflareTunnelToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareTunnelGetACloudflareTunnelToken.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareTunnelGetACloudflareTunnelToken" (int status)
        }

    ///<summary>
    ///Lists all Cloud Network Interconnects (CNIs) configured for the account, showing connection
    ///status and parameters.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="slot">If specified, only show CNIs associated with the specified slot</param>
    ///<param name="tunnelId">If specified, only show cnis associated with the specified tunnel id</param>
    ///<param name="cursor"></param>
    ///<param name="limit"></param>
    ///<param name="cancellationToken"></param>
    member this.ListCnis
        (
            accountId: string,
            ?slot: string,
            ?tunnelId: string,
            ?cursor: int,
            ?limit: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if slot.IsSome then
                      RequestPart.query ("slot", slot.Value)
                  if tunnelId.IsSome then
                      RequestPart.query ("tunnel_id", tunnelId.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/cni/cnis" requestParts cancellationToken

            match (int status) with
            | 200 -> return ListCnis.OK((Serializer.deserialize content))
            | 400 -> return ListCnis.BadRequest
            | 500 -> return ListCnis.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for ListCnis" (int status)
        }

    ///<summary>
    ///Creates a new Cloud Network Interconnect (CNI) for private network connectivity between
    ///Cloudflare and your infrastructure. CNIs enable dedicated, high-performance network links.
    ///</summary>
    member this.CreateCni(accountId: string, body: nsc_CniCreate, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/cni/cnis" requestParts cancellationToken

            match (int status) with
            | 200 -> return CreateCni.OK((Serializer.deserialize content))
            | 400 -> return CreateCni.BadRequest
            | 409 -> return CreateCni.Conflict
            | 500 -> return CreateCni.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for CreateCni" (int status)
        }

    ///<summary>
    ///Permanently removes a Cloud Network Interconnect (CNI) configuration. The private network
    ///connection will be terminated.
    ///</summary>
    ///<param name="cni">CNI ID to retrieve information about</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.DeleteCni(cni: System.Guid, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("cni", cni); RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/cni/cnis/{cni}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteCni.OK
            | 400 -> return DeleteCni.BadRequest
            | 404 -> return DeleteCni.NotFound
            | 500 -> return DeleteCni.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteCni" (int status)
        }

    ///<summary>
    ///Retrieves configuration details for a specific Cloud Network Interconnect (CNI), including
    ///connection status and parameters.
    ///</summary>
    ///<param name="cni">CNI ID to retrieve information about</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.GetCni(cni: System.Guid, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("cni", cni); RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/cni/cnis/{cni}" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetCni.OK((Serializer.deserialize content))
            | 400 -> return GetCni.BadRequest
            | 404 -> return GetCni.NotFound
            | 500 -> return GetCni.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for GetCni" (int status)
        }

    ///<summary>
    ///Updates the configuration of an existing Cloud Network Interconnect (CNI), including
    ///connection parameters and routing settings.
    ///</summary>
    ///<param name="cni">CNI ID to retrieve information about</param>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateCni(cni: System.Guid, accountId: string, body: nsc_Cni, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("cni", cni)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/cni/cnis/{cni}" requestParts cancellationToken

            match (int status) with
            | 200 -> return UpdateCni.OK((Serializer.deserialize content))
            | 400 -> return UpdateCni.BadRequest
            | 404 -> return UpdateCni.NotFound
            | 500 -> return UpdateCni.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateCni" (int status)
        }

    ///<summary>
    ///Lists all network interconnects configured for the account, including physical and virtual
    ///connections.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="site">If specified, only show interconnects located at the given site</param>
    ///<param name="type">If specified, only show interconnects of the given type</param>
    ///<param name="cursor"></param>
    ///<param name="limit"></param>
    ///<param name="cancellationToken"></param>
    member this.ListInterconnects
        (
            accountId: string,
            ?site: string,
            ?``type``: string,
            ?cursor: int,
            ?limit: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if site.IsSome then
                      RequestPart.query ("site", site.Value)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/cni/interconnects"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListInterconnects.OK((Serializer.deserialize content))
            | 400 -> return ListInterconnects.BadRequest
            | 500 -> return ListInterconnects.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for ListInterconnects" (int status)
        }

    ///<summary>
    ///Creates a new network interconnect for connecting Cloudflare's network to external networks.
    ///Interconnects provide dedicated bandwidth and reduced latency for traffic exchange.
    ///</summary>
    member this.CreateInterconnect
        (accountId: string, body: nsc_InterconnectCreate, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/cni/interconnects"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateInterconnect.OK((Serializer.deserialize content))
            | 400 -> return CreateInterconnect.BadRequest
            | 500 -> return CreateInterconnect.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for CreateInterconnect" (int status)
        }

    ///<summary>
    ///Permanently removes a network interconnect configuration. The physical or virtual connection
    ///will be terminated.
    ///</summary>
    ///<param name="icon">Interconnect name to retrieve information about</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.DeleteInterconnect(icon: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("icon", icon); RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/cni/interconnects/{icon}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteInterconnect.OK
            | 400 -> return DeleteInterconnect.BadRequest
            | 404 -> return DeleteInterconnect.NotFound
            | 500 -> return DeleteInterconnect.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteInterconnect" (int status)
        }

    ///<summary>
    ///Retrieves configuration and status details for a specific network interconnect.
    ///</summary>
    ///<param name="icon">Interconnect name to retrieve information about</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.GetInterconnect(icon: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("icon", icon); RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/cni/interconnects/{icon}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetInterconnect.OK((Serializer.deserialize content))
            | 400 -> return GetInterconnect.BadRequest
            | 404 -> return GetInterconnect.NotFound
            | 500 -> return GetInterconnect.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for GetInterconnect" (int status)
        }

    ///<summary>
    ///Downloads the Letter of Authorization (LOA) for a network interconnect, required for
    ///physical cross-connect provisioning.
    ///</summary>
    ///<param name="icon">Interconnect name to retrieve information about</param>
    ///<param name="accountId"></param>
    ///<param name="name">Custom name to use in the LOA instead of the account name (200 Character limit)</param>
    ///<param name="cancellationToken"></param>
    member this.GetInterconnectLoa
        (icon: string, accountId: string, ?name: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("icon", icon)
                  RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/cni/interconnects/{icon}/loa"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetInterconnectLoa.OK
            | 400 -> return GetInterconnectLoa.BadRequest
            | 404 -> return GetInterconnectLoa.NotFound
            | 500 -> return GetInterconnectLoa.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for GetInterconnectLoa" (int status)
        }

    ///<summary>
    ///Retrieves the default customer name that will be used in the LOA if no name is provided to the
    ///`/accounts/{account_id}/cni/interconnects/{icon}/loa` endpoint.
    ///</summary>
    ///<param name="icon">Interconnect name to retrieve information about</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.GetInterconnectLoaDefaultName(icon: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("icon", icon); RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/cni/interconnects/{icon}/loa/default"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetInterconnectLoaDefaultName.OK((Serializer.deserialize content))
            | 400 -> return GetInterconnectLoaDefaultName.BadRequest
            | 404 -> return GetInterconnectLoaDefaultName.NotFound
            | 500 -> return GetInterconnectLoaDefaultName.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for GetInterconnectLoaDefaultName" (int status)
        }

    ///<summary>
    ///Gets the current operational status of a network interconnect, including link state and
    ///traffic metrics.
    ///</summary>
    ///<param name="icon">Interconnect name to retrieve information about</param>
    ///<param name="accountId"></param>
    ///<param name="cancellationToken"></param>
    member this.GetInterconnectStatus(icon: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("icon", icon); RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/cni/interconnects/{icon}/status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetInterconnectStatus.OK((Serializer.deserialize content))
            | 400 -> return GetInterconnectStatus.BadRequest
            | 404 -> return GetInterconnectStatus.NotFound
            | 500 -> return GetInterconnectStatus.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for GetInterconnectStatus" (int status)
        }

    ///<summary>
    ///Retrieves current settings configuration for the specified resource or service.
    ///</summary>
    ///<param name="accountId">Account tag to retrieve settings for</param>
    ///<param name="cancellationToken"></param>
    member this.GetSettings(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/cni/settings" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetSettings.OK((Serializer.deserialize content))
            | 400 -> return GetSettings.BadRequest
            | 404 -> return GetSettings.NotFound
            | 500 -> return GetSettings.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for GetSettings" (int status)
        }

    ///<summary>
    ///Updates configuration settings for the specified resource or service.
    ///</summary>
    ///<param name="accountId">Account tag to update settings for</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateSettings(accountId: string, body: nsc_SettingsRequest, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/cni/settings" requestParts cancellationToken

            match (int status) with
            | 200 -> return UpdateSettings.OK((Serializer.deserialize content))
            | 400 -> return UpdateSettings.BadRequest
            | 404 -> return UpdateSettings.NotFound
            | 500 -> return UpdateSettings.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateSettings" (int status)
        }

    ///<summary>
    ///Lists all available infrastructure slots for the account, showing allocation status and
    ///capacity.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="addressContains">If specified, only show slots with the given text in their address field</param>
    ///<param name="site">If specified, only show slots located at the given site</param>
    ///<param name="speed">If specified, only show slots that support the given speed</param>
    ///<param name="occupied">If specified, only show slots with a specific occupied/unoccupied state</param>
    ///<param name="cursor"></param>
    ///<param name="limit"></param>
    ///<param name="cancellationToken"></param>
    member this.ListSlots
        (
            accountId: string,
            ?addressContains: string,
            ?site: string,
            ?speed: string,
            ?occupied: bool,
            ?cursor: int,
            ?limit: int,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if addressContains.IsSome then
                      RequestPart.query ("address_contains", addressContains.Value)
                  if site.IsSome then
                      RequestPart.query ("site", site.Value)
                  if speed.IsSome then
                      RequestPart.query ("speed", speed.Value)
                  if occupied.IsSome then
                      RequestPart.query ("occupied", occupied.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/cni/slots" requestParts cancellationToken

            match (int status) with
            | 200 -> return ListSlots.OK((Serializer.deserialize content))
            | 400 -> return ListSlots.BadRequest
            | 500 -> return ListSlots.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for ListSlots" (int status)
        }

    ///<summary>
    ///Gets information about a specific infrastructure slot allocation.
    ///</summary>
    member this.GetSlot(slot: System.Guid, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("slot", slot); RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/cni/slots/{slot}" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetSlot.OK((Serializer.deserialize content))
            | 400 -> return GetSlot.BadRequest
            | 404 -> return GetSlot.NotFound
            | 500 -> return GetSlot.InternalServerError
            | _ -> return failwithf "Unexpected HTTP status %d for GetSlot" (int status)
        }

    ///<summary>
    ///Lists the Workers VPC connectivity services in the account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="type"></param>
    ///<param name="page">Current page in the response</param>
    ///<param name="perPage">Max amount of entries returned per page</param>
    ///<param name="cancellationToken"></param>
    member this.ConnectivityServicesList
        (accountId: string, ?``type``: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/connectivity/directory/services"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ConnectivityServicesList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ConnectivityServicesList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ConnectivityServicesList" (int status)
        }

    ///<summary>
    ///Creates a new Workers VPC connectivity service in the account.
    ///</summary>
    member this.ConnectivityServicesPost
        (accountId: string, body: infra_ServiceConfig, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/connectivity/directory/services"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ConnectivityServicesPost.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ConnectivityServicesPost.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ConnectivityServicesPost" (int status)
        }

    ///<summary>
    ///Removes a single Workers VPC connectivity service by its ID.
    ///</summary>
    member this.ConnectivityServicesDelete
        (accountId: string, serviceId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("service_id", serviceId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/connectivity/directory/services/{service_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ConnectivityServicesDelete.OK
            | _ when (((int status) / 100) = 4) ->
                return ConnectivityServicesDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ConnectivityServicesDelete" (int status)
        }

    ///<summary>
    ///Fetches a single Workers VPC connectivity service by its ID.
    ///</summary>
    member this.ConnectivityServicesGet
        (accountId: string, serviceId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("service_id", serviceId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/connectivity/directory/services/{service_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ConnectivityServicesGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ConnectivityServicesGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ConnectivityServicesGet" (int status)
        }

    ///<summary>
    ///Updates an existing Workers VPC connectivity service by its ID.
    ///</summary>
    member this.ConnectivityServicesPut
        (accountId: string, serviceId: System.Guid, body: infra_ServiceConfig, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("service_id", serviceId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/connectivity/directory/services/{service_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ConnectivityServicesPut.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ConnectivityServicesPut.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ConnectivityServicesPut" (int status)
        }

    ///<summary>
    ///List an account's custom nameservers.
    ///</summary>
    member this.AccountLevelCustomNameserversListAccountCustomNameservers
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/custom_ns" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return AccountLevelCustomNameserversListAccountCustomNameservers.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLevelCustomNameserversListAccountCustomNameservers.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLevelCustomNameserversListAccountCustomNameservers"
                        (int status)
        }

    ///<summary>
    ///Adds a custom nameserver to the account for use as a vanity nameserver on zones.
    ///</summary>
    member this.AccountLevelCustomNameserversAddAccountCustomNameserver
        (
            accountId: string,
            body: dns_u002D_custom_u002D_nameservers_CustomNSInput,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/custom_ns" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountLevelCustomNameserversAddAccountCustomNameserver.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLevelCustomNameserversAddAccountCustomNameserver.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLevelCustomNameserversAddAccountCustomNameserver"
                        (int status)
        }

    ///<summary>
    ///Removes a custom nameserver from the account.
    ///</summary>
    member this.AccountLevelCustomNameserversDeleteAccountCustomNameserver
        (
            customNsId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("custom_ns_id", customNsId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/custom_ns/{custom_ns_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return AccountLevelCustomNameserversDeleteAccountCustomNameserver.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLevelCustomNameserversDeleteAccountCustomNameserver.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLevelCustomNameserversDeleteAccountCustomNameserver"
                        (int status)
        }

    ///<summary>
    ///List DLS prefix bindings for an account
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="cursor">Opaque token for cursor-based pagination. Omit for the first page. Pass the value from a previous response to fetch the next page.</param>
    ///<param name="perPage"></param>
    ///<param name="cancellationToken"></param>
    member this.PublicListPrefixBindings
        (accountId: string, ?cursor: string, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/dls/regional_services/prefix_bindings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PublicListPrefixBindings.OK((Serializer.deserialize content))
            | 400 -> return PublicListPrefixBindings.BadRequest((Serializer.deserialize content))
            | 401 -> return PublicListPrefixBindings.Unauthorized((Serializer.deserialize content))
            | 403 -> return PublicListPrefixBindings.Forbidden((Serializer.deserialize content))
            | 500 -> return PublicListPrefixBindings.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PublicListPrefixBindings" (int status)
        }

    ///<summary>
    ///Create a DLS prefix binding
    ///</summary>
    member this.PublicCreatePrefixBinding
        (accountId: string, body: dls_CreatePrefixBindingInput, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/dls/regional_services/prefix_bindings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return PublicCreatePrefixBinding.Created((Serializer.deserialize content))
            | 400 -> return PublicCreatePrefixBinding.BadRequest((Serializer.deserialize content))
            | 401 -> return PublicCreatePrefixBinding.Unauthorized((Serializer.deserialize content))
            | 403 -> return PublicCreatePrefixBinding.Forbidden((Serializer.deserialize content))
            | 409 -> return PublicCreatePrefixBinding.Conflict((Serializer.deserialize content))
            | 500 -> return PublicCreatePrefixBinding.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PublicCreatePrefixBinding" (int status)
        }

    ///<summary>
    ///Delete a DLS prefix binding
    ///</summary>
    member this.PublicDeletePrefixBinding(accountId: string, bindingId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("binding_id", bindingId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/dls/regional_services/prefix_bindings/{binding_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PublicDeletePrefixBinding.OK((Serializer.deserialize content))
            | 401 -> return PublicDeletePrefixBinding.Unauthorized((Serializer.deserialize content))
            | 403 -> return PublicDeletePrefixBinding.Forbidden((Serializer.deserialize content))
            | 404 -> return PublicDeletePrefixBinding.NotFound((Serializer.deserialize content))
            | 500 -> return PublicDeletePrefixBinding.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PublicDeletePrefixBinding" (int status)
        }

    ///<summary>
    ///Get a DLS prefix binding
    ///</summary>
    member this.PublicGetPrefixBinding(accountId: string, bindingId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("binding_id", bindingId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/dls/regional_services/prefix_bindings/{binding_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PublicGetPrefixBinding.OK((Serializer.deserialize content))
            | 401 -> return PublicGetPrefixBinding.Unauthorized((Serializer.deserialize content))
            | 403 -> return PublicGetPrefixBinding.Forbidden((Serializer.deserialize content))
            | 404 -> return PublicGetPrefixBinding.NotFound((Serializer.deserialize content))
            | 500 -> return PublicGetPrefixBinding.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PublicGetPrefixBinding" (int status)
        }

    ///<summary>
    ///Update a DLS prefix binding
    ///</summary>
    member this.PublicPatchPrefixBinding
        (accountId: string, bindingId: string, body: dls_UpdatePrefixBindingInput, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("binding_id", bindingId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/dls/regional_services/prefix_bindings/{binding_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PublicPatchPrefixBinding.OK((Serializer.deserialize content))
            | 400 -> return PublicPatchPrefixBinding.BadRequest((Serializer.deserialize content))
            | 401 -> return PublicPatchPrefixBinding.Unauthorized((Serializer.deserialize content))
            | 403 -> return PublicPatchPrefixBinding.Forbidden((Serializer.deserialize content))
            | 404 -> return PublicPatchPrefixBinding.NotFound((Serializer.deserialize content))
            | 500 -> return PublicPatchPrefixBinding.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PublicPatchPrefixBinding" (int status)
        }

    ///<summary>
    ///List DLS regions for an account
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="cursor">Opaque token for cursor-based pagination. Omit for the first page. Pass the value from a previous response to fetch the next page.</param>
    ///<param name="perPage"></param>
    ///<param name="type">Filter regions by type. Omit to return all regions.</param>
    ///<param name="cancellationToken"></param>
    member this.PublicListRegions
        (accountId: string, ?cursor: string, ?perPage: int, ?``type``: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/dls/regions" requestParts cancellationToken

            match (int status) with
            | 200 -> return PublicListRegions.OK((Serializer.deserialize content))
            | 400 -> return PublicListRegions.BadRequest((Serializer.deserialize content))
            | 401 -> return PublicListRegions.Unauthorized((Serializer.deserialize content))
            | 403 -> return PublicListRegions.Forbidden((Serializer.deserialize content))
            | 500 -> return PublicListRegions.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PublicListRegions" (int status)
        }

    ///<summary>
    ///Get a DLS region
    ///</summary>
    member this.PublicGetRegion(accountId: string, regionId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("region_id", regionId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/dls/regions/{region_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PublicGetRegion.OK((Serializer.deserialize content))
            | 401 -> return PublicGetRegion.Unauthorized((Serializer.deserialize content))
            | 403 -> return PublicGetRegion.Forbidden((Serializer.deserialize content))
            | 404 -> return PublicGetRegion.NotFound((Serializer.deserialize content))
            | 500 -> return PublicGetRegion.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PublicGetRegion" (int status)
        }

    ///<summary>
    ///List DNS Firewall clusters for an account
    ///</summary>
    member this.DnsFirewallListDnsFirewallClusters
        (accountId: string, ?page: float, ?perPage: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/dns_firewall" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsFirewallListDnsFirewallClusters.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsFirewallListDnsFirewallClusters.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsFirewallListDnsFirewallClusters" (int status)
        }

    ///<summary>
    ///Create a DNS Firewall cluster
    ///</summary>
    member this.DnsFirewallCreateDnsFirewallCluster
        (
            accountId: string,
            body: dns_u002D_firewall_dns_u002D_firewall_u002D_cluster_u002D_post,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/dns_firewall" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsFirewallCreateDnsFirewallCluster.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsFirewallCreateDnsFirewallCluster.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsFirewallCreateDnsFirewallCluster" (int status)
        }

    ///<summary>
    ///Delete a DNS Firewall cluster
    ///</summary>
    member this.DnsFirewallDeleteDnsFirewallCluster
        (
            dnsFirewallId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_firewall_id", dnsFirewallId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/dns_firewall/{dns_firewall_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsFirewallDeleteDnsFirewallCluster.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsFirewallDeleteDnsFirewallCluster.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsFirewallDeleteDnsFirewallCluster" (int status)
        }

    ///<summary>
    ///Show a single DNS Firewall cluster for an account
    ///</summary>
    member this.DnsFirewallDnsFirewallClusterDetails
        (dnsFirewallId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_firewall_id", dnsFirewallId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/dns_firewall/{dns_firewall_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsFirewallDnsFirewallClusterDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsFirewallDnsFirewallClusterDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsFirewallDnsFirewallClusterDetails" (int status)
        }

    ///<summary>
    ///Modify the configuration of a DNS Firewall cluster
    ///</summary>
    member this.DnsFirewallUpdateDnsFirewallCluster
        (
            dnsFirewallId: string,
            accountId: string,
            body: dns_u002D_firewall_dns_u002D_firewall_u002D_cluster_u002D_patch,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_firewall_id", dnsFirewallId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/dns_firewall/{dns_firewall_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsFirewallUpdateDnsFirewallCluster.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsFirewallUpdateDnsFirewallCluster.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsFirewallUpdateDnsFirewallCluster" (int status)
        }

    ///<summary>
    ///Retrieves a list of summarised aggregate metrics over a given time period.
    ///See [Analytics API properties](https://developers.cloudflare.com/dns/reference/analytics-api-properties/) for detailed information about the available query parameters.
    ///</summary>
    member this.DnsFirewallAnalyticsTable
        (
            dnsFirewallId: string,
            accountId: string,
            ?metrics: string,
            ?dimensions: string,
            ?since: System.DateTimeOffset,
            ?until: System.DateTimeOffset,
            ?limit: int,
            ?sort: string,
            ?filters: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_firewall_id", dnsFirewallId)
                  RequestPart.path ("account_id", accountId)
                  if metrics.IsSome then
                      RequestPart.query ("metrics", metrics.Value)
                  if dimensions.IsSome then
                      RequestPart.query ("dimensions", dimensions.Value)
                  if since.IsSome then
                      RequestPart.query ("since", since.Value)
                  if until.IsSome then
                      RequestPart.query ("until", until.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if sort.IsSome then
                      RequestPart.query ("sort", sort.Value)
                  if filters.IsSome then
                      RequestPart.query ("filters", filters.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/dns_firewall/{dns_firewall_id}/dns_analytics/report"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsFirewallAnalyticsTable.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsFirewallAnalyticsTable.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsFirewallAnalyticsTable" (int status)
        }

    ///<summary>
    ///Retrieves a list of aggregate metrics grouped by time interval.
    ///See [Analytics API properties](https://developers.cloudflare.com/dns/reference/analytics-api-properties/) for detailed information about the available query parameters.
    ///</summary>
    member this.DnsFirewallAnalyticsByTime
        (
            dnsFirewallId: string,
            accountId: string,
            ?metrics: string,
            ?dimensions: string,
            ?since: System.DateTimeOffset,
            ?until: System.DateTimeOffset,
            ?limit: int,
            ?sort: string,
            ?filters: string,
            ?timeDelta: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_firewall_id", dnsFirewallId)
                  RequestPart.path ("account_id", accountId)
                  if metrics.IsSome then
                      RequestPart.query ("metrics", metrics.Value)
                  if dimensions.IsSome then
                      RequestPart.query ("dimensions", dimensions.Value)
                  if since.IsSome then
                      RequestPart.query ("since", since.Value)
                  if until.IsSome then
                      RequestPart.query ("until", until.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if sort.IsSome then
                      RequestPart.query ("sort", sort.Value)
                  if filters.IsSome then
                      RequestPart.query ("filters", filters.Value)
                  if timeDelta.IsSome then
                      RequestPart.query ("time_delta", timeDelta.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/dns_firewall/{dns_firewall_id}/dns_analytics/report/bytime"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsFirewallAnalyticsByTime.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsFirewallAnalyticsByTime.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsFirewallAnalyticsByTime" (int status)
        }

    ///<summary>
    ///Show reverse DNS configuration (PTR records) for a DNS Firewall cluster
    ///</summary>
    member this.DnsFirewallShowDnsFirewallClusterReverseDns
        (dnsFirewallId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_firewall_id", dnsFirewallId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/dns_firewall/{dns_firewall_id}/reverse_dns"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsFirewallShowDnsFirewallClusterReverseDns.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    DnsFirewallShowDnsFirewallClusterReverseDns.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for DnsFirewallShowDnsFirewallClusterReverseDns" (int status)
        }

    ///<summary>
    ///Update reverse DNS configuration (PTR records) for a DNS Firewall cluster
    ///</summary>
    member this.DnsFirewallUpdateDnsFirewallClusterReverseDns
        (
            dnsFirewallId: string,
            accountId: string,
            body: dns_u002D_firewall_dns_u002D_firewall_u002D_reverse_u002D_dns_u002D_patch,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_firewall_id", dnsFirewallId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/dns_firewall/{dns_firewall_id}/reverse_dns"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsFirewallUpdateDnsFirewallClusterReverseDns.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    DnsFirewallUpdateDnsFirewallClusterReverseDns.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for DnsFirewallUpdateDnsFirewallClusterReverseDns" (int status)
        }

    ///<summary>
    ///Get the current DNS record usage and quota for an account. May include internal DNS usage and quota.
    ///</summary>
    member this.DnsRecordsForAnAccountGetUsage(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/dns_records/usage"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAnAccountGetUsage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAnAccountGetUsage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAnAccountGetUsage" (int status)
        }

    ///<summary>
    ///Show DNS settings for an account
    ///</summary>
    member this.DnsSettingsForAnAccountListDnsSettings(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/dns_settings" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsSettingsForAnAccountListDnsSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsSettingsForAnAccountListDnsSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsSettingsForAnAccountListDnsSettings" (int status)
        }

    ///<summary>
    ///Update DNS settings for an account
    ///</summary>
    member this.DnsSettingsForAnAccountUpdateDnsSettings
        (accountId: string, body: dns_u002D_settings_account_settings_patch, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/accounts/{account_id}/dns_settings" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsSettingsForAnAccountUpdateDnsSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsSettingsForAnAccountUpdateDnsSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for DnsSettingsForAnAccountUpdateDnsSettings" (int status)
        }

    ///<summary>
    ///List DNS Internal Views for an Account
    ///</summary>
    member this.DnsViewsForAnAccountListInternalDnsViews
        (
            accountId: string,
            ?name: string,
            ?nameExact: string,
            ?nameContains: string,
            ?nameStartswith: string,
            ?nameEndswith: string,
            ?zoneId: string,
            ?zoneName: string,
            ?``match``: string,
            ?page: float,
            ?perPage: float,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if nameExact.IsSome then
                      RequestPart.query ("name.exact", nameExact.Value)
                  if nameContains.IsSome then
                      RequestPart.query ("name.contains", nameContains.Value)
                  if nameStartswith.IsSome then
                      RequestPart.query ("name.startswith", nameStartswith.Value)
                  if nameEndswith.IsSome then
                      RequestPart.query ("name.endswith", nameEndswith.Value)
                  if zoneId.IsSome then
                      RequestPart.query ("zone_id", zoneId.Value)
                  if zoneName.IsSome then
                      RequestPart.query ("zone_name", zoneName.Value)
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
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/dns_settings/views"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsViewsForAnAccountListInternalDnsViews.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsViewsForAnAccountListInternalDnsViews.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for DnsViewsForAnAccountListInternalDnsViews" (int status)
        }

    ///<summary>
    ///Create Internal DNS View for an account
    ///</summary>
    member this.DnsViewsForAnAccountCreateInternalDnsViews
        (accountId: string, body: dns_u002D_settings_dns_u002D_view_u002D_post, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/dns_settings/views"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsViewsForAnAccountCreateInternalDnsViews.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    DnsViewsForAnAccountCreateInternalDnsViews.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for DnsViewsForAnAccountCreateInternalDnsViews" (int status)
        }

    ///<summary>
    ///Delete an existing Internal DNS View
    ///</summary>
    member this.DnsViewsForAnAccountDeleteInternalDnsView
        (
            accountId: string,
            viewId: string,
            ?cancellationToken: CancellationToken,
            ?body: System.Text.Json.Nodes.JsonNode
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("view_id", viewId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/dns_settings/views/{view_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsViewsForAnAccountDeleteInternalDnsView.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsViewsForAnAccountDeleteInternalDnsView.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for DnsViewsForAnAccountDeleteInternalDnsView" (int status)
        }

    ///<summary>
    ///Get DNS Internal View
    ///</summary>
    member this.DnsViewsForAnAccountGetInternalDnsView
        (accountId: string, viewId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("view_id", viewId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/dns_settings/views/{view_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsViewsForAnAccountGetInternalDnsView.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsViewsForAnAccountGetInternalDnsView.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsViewsForAnAccountGetInternalDnsView" (int status)
        }

    ///<summary>
    ///Update an existing Internal DNS View
    ///</summary>
    member this.DnsViewsForAnAccountUpdateInternalDnsView
        (
            accountId: string,
            viewId: string,
            body: dns_u002D_settings_dns_u002D_view_u002D_patch,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("view_id", viewId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/dns_settings/views/{view_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsViewsForAnAccountUpdateInternalDnsView.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsViewsForAnAccountUpdateInternalDnsView.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for DnsViewsForAnAccountUpdateInternalDnsView" (int status)
        }

    ///<summary>
    ///List configured account-scoped load balancers.
    ///</summary>
    member this.AccountLoadBalancersListAccountLoadBalancers(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/load_balancers" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancersListAccountLoadBalancers.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancersListAccountLoadBalancers.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for AccountLoadBalancersListAccountLoadBalancers" (int status)
        }

    ///<summary>
    ///Create a new account-scoped load balancer.
    ///</summary>
    member this.AccountLoadBalancersCreateAccountLoadBalancer
        (
            accountId: string,
            body: AccountLoadBalancersCreateAccountLoadBalancerPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/load_balancers" requestParts cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancersCreateAccountLoadBalancer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancersCreateAccountLoadBalancer.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for AccountLoadBalancersCreateAccountLoadBalancer" (int status)
        }

    ///<summary>
    ///List configured monitor groups.
    ///</summary>
    member this.AccountLoadBalancerMonitorGroupsListMonitorGroups
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitor_groups"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorGroupsListMonitorGroups.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancerMonitorGroupsListMonitorGroups.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLoadBalancerMonitorGroupsListMonitorGroups"
                        (int status)
        }

    ///<summary>
    ///Create a new monitor group.
    ///</summary>
    member this.AccountLoadBalancerMonitorGroupsCreateMonitorGroup
        (accountId: string, body: load_u002D_balancing_monitor_u002D_group, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitor_groups"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorGroupsCreateMonitorGroup.OK((Serializer.deserialize content))
            | 412 ->
                return
                    AccountLoadBalancerMonitorGroupsCreateMonitorGroup.PreconditionFailed(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancerMonitorGroupsCreateMonitorGroup.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLoadBalancerMonitorGroupsCreateMonitorGroup"
                        (int status)
        }

    ///<summary>
    ///Delete a configured monitor group.
    ///</summary>
    member this.AccountLoadBalancerMonitorGroupsDeleteMonitorGroup
        (monitorGroupId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_group_id", monitorGroupId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitor_groups/{monitor_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorGroupsDeleteMonitorGroup.OK((Serializer.deserialize content))
            | 412 ->
                return
                    AccountLoadBalancerMonitorGroupsDeleteMonitorGroup.PreconditionFailed(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancerMonitorGroupsDeleteMonitorGroup.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLoadBalancerMonitorGroupsDeleteMonitorGroup"
                        (int status)
        }

    ///<summary>
    ///Fetch a single configured monitor group.
    ///</summary>
    member this.AccountLoadBalancerMonitorGroupsMonitorGroupDetails
        (monitorGroupId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_group_id", monitorGroupId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitor_groups/{monitor_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorGroupsMonitorGroupDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancerMonitorGroupsMonitorGroupDetails.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLoadBalancerMonitorGroupsMonitorGroupDetails"
                        (int status)
        }

    ///<summary>
    ///Apply changes to an existing monitor group, overwriting the supplied properties.
    ///</summary>
    member this.AccountLoadBalancerMonitorGroupsPatchMonitorGroup
        (
            monitorGroupId: string,
            accountId: string,
            body: load_u002D_balancing_monitor_u002D_group,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_group_id", monitorGroupId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitor_groups/{monitor_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorGroupsPatchMonitorGroup.OK((Serializer.deserialize content))
            | 412 ->
                return
                    AccountLoadBalancerMonitorGroupsPatchMonitorGroup.PreconditionFailed(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancerMonitorGroupsPatchMonitorGroup.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLoadBalancerMonitorGroupsPatchMonitorGroup"
                        (int status)
        }

    ///<summary>
    ///Modify a configured monitor group.
    ///</summary>
    member this.AccountLoadBalancerMonitorGroupsUpdateMonitorGroup
        (
            monitorGroupId: string,
            accountId: string,
            body: load_u002D_balancing_monitor_u002D_group,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_group_id", monitorGroupId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitor_groups/{monitor_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorGroupsUpdateMonitorGroup.OK((Serializer.deserialize content))
            | 412 ->
                return
                    AccountLoadBalancerMonitorGroupsUpdateMonitorGroup.PreconditionFailed(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancerMonitorGroupsUpdateMonitorGroup.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLoadBalancerMonitorGroupsUpdateMonitorGroup"
                        (int status)
        }

    ///<summary>
    ///Get the list of resources that reference the provided monitor group.
    ///</summary>
    member this.AccountLoadBalancerMonitorGroupsListMonitorGroupReferences
        (monitorGroupId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_group_id", monitorGroupId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitor_groups/{monitor_group_id}/references"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return AccountLoadBalancerMonitorGroupsListMonitorGroupReferences.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancerMonitorGroupsListMonitorGroupReferences.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLoadBalancerMonitorGroupsListMonitorGroupReferences"
                        (int status)
        }

    ///<summary>
    ///List configured monitors for an account.
    ///</summary>
    member this.AccountLoadBalancerMonitorsListMonitors(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitors"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorsListMonitors.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerMonitorsListMonitors.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountLoadBalancerMonitorsListMonitors" (int status)
        }

    ///<summary>
    ///Create a configured monitor.
    ///</summary>
    member this.AccountLoadBalancerMonitorsCreateMonitor
        (accountId: string, body: load_u002D_balancing_monitor_u002D_editable, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitors"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorsCreateMonitor.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerMonitorsCreateMonitor.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountLoadBalancerMonitorsCreateMonitor" (int status)
        }

    ///<summary>
    ///Delete a configured monitor.
    ///</summary>
    member this.AccountLoadBalancerMonitorsDeleteMonitor
        (
            monitorId: string,
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_id", monitorId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitors/{monitor_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorsDeleteMonitor.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerMonitorsDeleteMonitor.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountLoadBalancerMonitorsDeleteMonitor" (int status)
        }

    ///<summary>
    ///List a single configured monitor for an account.
    ///</summary>
    member this.AccountLoadBalancerMonitorsMonitorDetails
        (monitorId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_id", monitorId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitors/{monitor_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorsMonitorDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerMonitorsMonitorDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountLoadBalancerMonitorsMonitorDetails" (int status)
        }

    ///<summary>
    ///Apply changes to an existing monitor, overwriting the supplied properties.
    ///</summary>
    member this.AccountLoadBalancerMonitorsPatchMonitor
        (
            monitorId: string,
            accountId: string,
            body: load_u002D_balancing_monitor_u002D_editable,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_id", monitorId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitors/{monitor_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorsPatchMonitor.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerMonitorsPatchMonitor.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountLoadBalancerMonitorsPatchMonitor" (int status)
        }

    ///<summary>
    ///Modify a configured monitor.
    ///</summary>
    member this.AccountLoadBalancerMonitorsUpdateMonitor
        (
            monitorId: string,
            accountId: string,
            body: load_u002D_balancing_monitor_u002D_editable,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_id", monitorId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitors/{monitor_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorsUpdateMonitor.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerMonitorsUpdateMonitor.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountLoadBalancerMonitorsUpdateMonitor" (int status)
        }

    ///<summary>
    ///Preview pools using the specified monitor with provided monitor details. The returned preview_id can be used in the preview endpoint to retrieve the results.
    ///</summary>
    member this.AccountLoadBalancerMonitorsPreviewMonitor
        (
            monitorId: string,
            accountId: string,
            body: load_u002D_balancing_monitor_u002D_editable,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_id", monitorId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitors/{monitor_id}/preview"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorsPreviewMonitor.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerMonitorsPreviewMonitor.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountLoadBalancerMonitorsPreviewMonitor" (int status)
        }

    ///<summary>
    ///Get the list of resources that reference the provided monitor.
    ///</summary>
    member this.AccountLoadBalancerMonitorsListMonitorReferences
        (monitorId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("monitor_id", monitorId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/monitors/{monitor_id}/references"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorsListMonitorReferences.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancerMonitorsListMonitorReferences.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLoadBalancerMonitorsListMonitorReferences"
                        (int status)
        }

    ///<summary>
    ///List configured pools.
    ///</summary>
    member this.AccountLoadBalancerPoolsListPools
        (accountId: string, ?monitor: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if monitor.IsSome then
                      RequestPart.query ("monitor", monitor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/pools"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerPoolsListPools.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerPoolsListPools.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountLoadBalancerPoolsListPools" (int status)
        }

    ///<summary>
    ///Apply changes to a number of existing pools, overwriting the supplied properties. Pools are ordered by ascending `name`. Returns the list of affected pools. Supports the standard pagination query parameters, either `limit`/`offset` or `per_page`/`page`.
    ///</summary>
    member this.AccountLoadBalancerPoolsPatchPools
        (accountId: string, body: AccountLoadBalancerPoolsPatchPoolsPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/pools"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerPoolsPatchPools.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerPoolsPatchPools.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountLoadBalancerPoolsPatchPools" (int status)
        }

    ///<summary>
    ///Create a new pool.
    ///</summary>
    member this.AccountLoadBalancerPoolsCreatePool
        (accountId: string, body: AccountLoadBalancerPoolsCreatePoolPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/pools"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerPoolsCreatePool.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerPoolsCreatePool.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountLoadBalancerPoolsCreatePool" (int status)
        }

    ///<summary>
    ///Delete a configured pool.
    ///</summary>
    member this.AccountLoadBalancerPoolsDeletePool
        (poolId: string, accountId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/pools/{pool_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerPoolsDeletePool.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerPoolsDeletePool.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountLoadBalancerPoolsDeletePool" (int status)
        }

    ///<summary>
    ///Fetch a single configured pool.
    ///</summary>
    member this.AccountLoadBalancerPoolsPoolDetails
        (poolId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/pools/{pool_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerPoolsPoolDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerPoolsPoolDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountLoadBalancerPoolsPoolDetails" (int status)
        }

    ///<summary>
    ///Apply changes to an existing pool, overwriting the supplied properties.
    ///</summary>
    member this.AccountLoadBalancerPoolsPatchPool
        (
            poolId: string,
            accountId: string,
            body: AccountLoadBalancerPoolsPatchPoolPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/pools/{pool_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerPoolsPatchPool.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerPoolsPatchPool.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountLoadBalancerPoolsPatchPool" (int status)
        }

    ///<summary>
    ///Modify a configured pool.
    ///</summary>
    member this.AccountLoadBalancerPoolsUpdatePool
        (
            poolId: string,
            accountId: string,
            body: AccountLoadBalancerPoolsUpdatePoolPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/pools/{pool_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerPoolsUpdatePool.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerPoolsUpdatePool.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountLoadBalancerPoolsUpdatePool" (int status)
        }

    ///<summary>
    ///Fetch the latest pool health status for a single pool.
    ///</summary>
    member this.AccountLoadBalancerPoolsPoolHealthDetails
        (poolId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/pools/{pool_id}/health"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerPoolsPoolHealthDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerPoolsPoolHealthDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountLoadBalancerPoolsPoolHealthDetails" (int status)
        }

    ///<summary>
    ///Preview pool health using provided monitor details. The returned preview_id can be used in the preview endpoint to retrieve the results.
    ///</summary>
    member this.AccountLoadBalancerPoolsPreviewPool
        (
            poolId: string,
            accountId: string,
            body: load_u002D_balancing_monitor_u002D_editable,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/pools/{pool_id}/preview"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerPoolsPreviewPool.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerPoolsPreviewPool.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for AccountLoadBalancerPoolsPreviewPool" (int status)
        }

    ///<summary>
    ///Get the list of resources that reference the provided pool.
    ///</summary>
    member this.AccountLoadBalancerPoolsListPoolReferences
        (poolId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pool_id", poolId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/pools/{pool_id}/references"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerPoolsListPoolReferences.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancerPoolsListPoolReferences.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountLoadBalancerPoolsListPoolReferences" (int status)
        }

    ///<summary>
    ///Get the result of a previous preview operation using the provided preview_id.
    ///</summary>
    member this.AccountLoadBalancerMonitorsPreviewResult
        (previewId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("preview_id", previewId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/preview/{preview_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerMonitorsPreviewResult.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerMonitorsPreviewResult.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountLoadBalancerMonitorsPreviewResult" (int status)
        }

    ///<summary>
    ///List all region mappings.
    ///</summary>
    member this.LoadBalancerRegionsListRegions
        (
            accountId: string,
            ?subdivisionCode: string,
            ?subdivisionCodeA2: string,
            ?countryCodeA2: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if subdivisionCode.IsSome then
                      RequestPart.query ("subdivision_code", subdivisionCode.Value)
                  if subdivisionCodeA2.IsSome then
                      RequestPart.query ("subdivision_code_a2", subdivisionCodeA2.Value)
                  if countryCodeA2.IsSome then
                      RequestPart.query ("country_code_a2", countryCodeA2.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/regions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerRegionsListRegions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerRegionsListRegions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerRegionsListRegions" (int status)
        }

    ///<summary>
    ///Get a single region mapping.
    ///</summary>
    member this.LoadBalancerRegionsGetRegion
        (regionId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("region_id", regionId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/regions/{region_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancerRegionsGetRegion.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancerRegionsGetRegion.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancerRegionsGetRegion" (int status)
        }

    ///<summary>
    ///Search for Load Balancing resources.
    ///</summary>
    member this.AccountLoadBalancerSearchSearchResources
        (
            accountId: string,
            ?query: string,
            ?references: string,
            ?page: float,
            ?perPage: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if query.IsSome then
                      RequestPart.query ("query", query.Value)
                  if references.IsSome then
                      RequestPart.query ("references", references.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/search"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancerSearchSearchResources.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancerSearchSearchResources.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountLoadBalancerSearchSearchResources" (int status)
        }

    ///<summary>
    ///Get current load balancer resource usage counts for an account.
    ///</summary>
    member this.AccountLoadBalancersListLoadBalancerUsage(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/usage"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancersListLoadBalancerUsage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return AccountLoadBalancersListLoadBalancerUsage.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for AccountLoadBalancersListLoadBalancerUsage" (int status)
        }

    ///<summary>
    ///Delete a configured account-scoped load balancer.
    ///</summary>
    member this.AccountLoadBalancersDeleteAccountLoadBalancer
        (accountId: string, loadBalancerId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("load_balancer_id", loadBalancerId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/{load_balancer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancersDeleteAccountLoadBalancer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancersDeleteAccountLoadBalancer.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for AccountLoadBalancersDeleteAccountLoadBalancer" (int status)
        }

    ///<summary>
    ///Fetch a single configured account-scoped load balancer.
    ///</summary>
    member this.AccountLoadBalancersAccountLoadBalancerDetails
        (accountId: string, loadBalancerId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("load_balancer_id", loadBalancerId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/{load_balancer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancersAccountLoadBalancerDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancersAccountLoadBalancerDetails.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLoadBalancersAccountLoadBalancerDetails"
                        (int status)
        }

    ///<summary>
    ///Apply changes to an existing account-scoped load balancer, overwriting the supplied properties.
    ///</summary>
    member this.AccountLoadBalancersPatchAccountLoadBalancer
        (
            accountId: string,
            loadBalancerId: string,
            body: load_u002D_balancing_load_u002D_balancer_u002D_editable,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("load_balancer_id", loadBalancerId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/{load_balancer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancersPatchAccountLoadBalancer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancersPatchAccountLoadBalancer.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for AccountLoadBalancersPatchAccountLoadBalancer" (int status)
        }

    ///<summary>
    ///Update a configured account-scoped load balancer.
    ///</summary>
    member this.AccountLoadBalancersUpdateAccountLoadBalancer
        (
            accountId: string,
            loadBalancerId: string,
            body: AccountLoadBalancersUpdateAccountLoadBalancerPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("load_balancer_id", loadBalancerId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/load_balancers/{load_balancer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return AccountLoadBalancersUpdateAccountLoadBalancer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLoadBalancersUpdateAccountLoadBalancer.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for AccountLoadBalancersUpdateAccountLoadBalancer" (int status)
        }

    ///<summary>
    ///Delete all DNS Protection rules for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteDnsProtectionRulesForAccount(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_dns_protection/configs/dns_protection/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteDnsProtectionRulesForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteDnsProtectionRulesForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteDnsProtectionRulesForAccount" (int status)
        }

    ///<summary>
    ///List all DNS Protection rules for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="page">The page number for pagination. Defaults to 1.</param>
    ///<param name="perPage">The number of items per page. Must be between 10 and 1000. Defaults to 25.</param>
    ///<param name="order">The field to order by. Defaults to 'prefix'.</param>
    ///<param name="direction">The direction of ordering (ASC or DESC). Defaults to 'ASC'.</param>
    ///<param name="cancellationToken"></param>
    member this.ListDnsProtectionRulesForAccount
        (
            accountId: string,
            ?page: int64,
            ?perPage: int64,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.queryComma ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.queryComma ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.queryComma ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.queryComma ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_dns_protection/configs/dns_protection/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListDnsProtectionRulesForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListDnsProtectionRulesForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListDnsProtectionRulesForAccount" (int status)
        }

    ///<summary>
    ///Create a DNS Protection rule for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreateDnsProtectionRule
        (accountId: string, body: dos_NewDnsProtectionRule, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_dns_protection/configs/dns_protection/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateDnsProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreateDnsProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateDnsProtectionRule" (int status)
        }

    ///<summary>
    ///Delete a DNS Protection rule specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="ruleId">The UUID of the DNS Protection rule to delete.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteDnsProtectionRule(accountId: string, ruleId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("rule_id", ruleId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_dns_protection/configs/dns_protection/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteDnsProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteDnsProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteDnsProtectionRule" (int status)
        }

    ///<summary>
    ///Get a DNS Protection rule specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="ruleId">The UUID of the DNS Protection rule.</param>
    ///<param name="cancellationToken"></param>
    member this.GetDnsProtectionRule(accountId: string, ruleId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("rule_id", ruleId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_dns_protection/configs/dns_protection/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetDnsProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetDnsProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetDnsProtectionRule" (int status)
        }

    ///<summary>
    ///Update a DNS Protection rule specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="ruleId">The UUID of the DNS Protection rule to update.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateDnsProtectionRule
        (accountId: string, ruleId: string, body: dos_DnsProtectionRuleUpdate, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("rule_id", ruleId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_dns_protection/configs/dns_protection/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateDnsProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateDnsProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateDnsProtectionRule" (int status)
        }

    ///<summary>
    ///Delete all allowlist prefixes for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteAllowlistPrefixesForAccount(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/allowlist"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteAllowlistPrefixesForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteAllowlistPrefixesForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteAllowlistPrefixesForAccount" (int status)
        }

    ///<summary>
    ///List all allowlist prefixes for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="page">The page number for pagination. Defaults to 1.</param>
    ///<param name="perPage">The number of items per page. Must be between 10 and 1000. Defaults to 25.</param>
    ///<param name="order">The field to order by. Defaults to 'prefix'.</param>
    ///<param name="direction">The direction of ordering (ASC or DESC). Defaults to 'ASC'.</param>
    ///<param name="cancellationToken"></param>
    member this.ListAllowlistPrefixesForAccount
        (
            accountId: string,
            ?page: int64,
            ?perPage: int64,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.queryComma ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.queryComma ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.queryComma ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.queryComma ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/allowlist"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListAllowlistPrefixesForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListAllowlistPrefixesForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListAllowlistPrefixesForAccount" (int status)
        }

    ///<summary>
    ///Create an allowlist prefix for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreateAllowlistedPrefix
        (accountId: string, body: dos_NewInfraPrefix, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/allowlist"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateAllowlistedPrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreateAllowlistedPrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateAllowlistedPrefix" (int status)
        }

    ///<summary>
    ///Delete the allowlist prefix for an account given a UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="prefixId">The UUID of the allowlist prefix to delete.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteAllowlistPrefix(accountId: string, prefixId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/allowlist/{prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteAllowlistPrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteAllowlistPrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteAllowlistPrefix" (int status)
        }

    ///<summary>
    ///Get an allowlist prefix specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="prefixId">The UUID of the allowlist prefix.</param>
    ///<param name="cancellationToken"></param>
    member this.GetAllowlistPrefix(accountId: string, prefixId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/allowlist/{prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetAllowlistPrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetAllowlistPrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetAllowlistPrefix" (int status)
        }

    ///<summary>
    ///Update an allowlist prefix specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="prefixId">The UUID of the allowlist prefix to update.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateAllowlistPrefix
        (accountId: string, prefixId: string, body: dos_InfraPrefixUpdate, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/allowlist/{prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateAllowlistPrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateAllowlistPrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateAllowlistPrefix" (int status)
        }

    ///<summary>
    ///Delete all prefixes for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="cancellationToken"></param>
    member this.DeletePrefixesForAccount(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/prefixes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeletePrefixesForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeletePrefixesForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeletePrefixesForAccount" (int status)
        }

    ///<summary>
    ///List all prefixes for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="page">The page number for pagination. Defaults to 1.</param>
    ///<param name="perPage">The number of items per page. Must be between 10 and 1000. Defaults to 25.</param>
    ///<param name="order">The field to order by. Defaults to 'prefix'.</param>
    ///<param name="direction">The direction of ordering (ASC or DESC). Defaults to 'ASC'.</param>
    ///<param name="cancellationToken"></param>
    member this.ListPrefixesForAccount
        (
            accountId: string,
            ?page: int64,
            ?perPage: int64,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.queryComma ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.queryComma ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.queryComma ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.queryComma ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/prefixes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListPrefixesForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListPrefixesForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListPrefixesForAccount" (int status)
        }

    ///<summary>
    ///Create a prefix for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreatePrefix(accountId: string, body: dos_NewPrefix, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/prefixes"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreatePrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreatePrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreatePrefix" (int status)
        }

    ///<summary>
    ///Create multiple prefixes for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BulkCreatePrefixes
        (accountId: string, body: list<dos_NewPrefix>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/prefixes/bulk"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return BulkCreatePrefixes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return BulkCreatePrefixes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for BulkCreatePrefixes" (int status)
        }

    ///<summary>
    ///Delete the prefix for an account given a UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="prefixId">The UUID of the prefix to delete.</param>
    ///<param name="cancellationToken"></param>
    member this.DeletePrefix(accountId: string, prefixId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/prefixes/{prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeletePrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeletePrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeletePrefix" (int status)
        }

    ///<summary>
    ///Get a prefix specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="prefixId">The UUID of the prefix.</param>
    ///<param name="cancellationToken"></param>
    member this.GetPrefix(accountId: string, prefixId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/prefixes/{prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetPrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetPrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetPrefix" (int status)
        }

    ///<summary>
    ///Update a prefix specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="prefixId">The UUID of the prefix to update.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdatePrefix
        (accountId: string, prefixId: string, body: dos_PrefixUpdate, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("prefix_id", prefixId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/prefixes/{prefix_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdatePrefix.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdatePrefix.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdatePrefix" (int status)
        }

    ///<summary>
    ///Delete all SYN Protection filters for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteSynProtectionFiltersForAccount(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/filters"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteSynProtectionFiltersForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteSynProtectionFiltersForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteSynProtectionFiltersForAccount" (int status)
        }

    ///<summary>
    ///List all SYN Protection filters for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="mode">The mode of the filters to get. Optional. Valid values: 'enabled', 'disabled', 'monitoring'.</param>
    ///<param name="page">The page number for pagination. Defaults to 1.</param>
    ///<param name="perPage">The number of items per page. Must be between 10 and 1000. Defaults to 25.</param>
    ///<param name="order">The field to order by. Defaults to 'prefix'.</param>
    ///<param name="direction">The direction of ordering (ASC or DESC). Defaults to 'ASC'.</param>
    ///<param name="cancellationToken"></param>
    member this.ListSynProtectionFiltersForAccount
        (
            accountId: string,
            ?mode: string,
            ?page: int64,
            ?perPage: int64,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if mode.IsSome then
                      RequestPart.queryComma ("mode", mode.Value)
                  if page.IsSome then
                      RequestPart.queryComma ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.queryComma ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.queryComma ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.queryComma ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/filters"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListSynProtectionFiltersForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListSynProtectionFiltersForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListSynProtectionFiltersForAccount" (int status)
        }

    ///<summary>
    ///Create a SYN Protection filter for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreateSynProtectionFilter
        (accountId: string, body: dos_NewExpressionFilter, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/filters"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateSynProtectionFilter.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreateSynProtectionFilter.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateSynProtectionFilter" (int status)
        }

    ///<summary>
    ///Delete a SYN Protection filter specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="filterId">The UUID of the filter to delete.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteSynProtectionFilter(accountId: string, filterId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("filter_id", filterId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/filters/{filter_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteSynProtectionFilter.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteSynProtectionFilter.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteSynProtectionFilter" (int status)
        }

    ///<summary>
    ///Get a SYN Protection filter specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="filterId">The UUID of the filter to retrieve.</param>
    ///<param name="cancellationToken"></param>
    member this.GetSynProtectionFilter(accountId: string, filterId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("filter_id", filterId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/filters/{filter_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetSynProtectionFilter.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetSynProtectionFilter.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetSynProtectionFilter" (int status)
        }

    ///<summary>
    ///Update a SYN Protection filter specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="filterId">The UUID of the filter to update.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateSynProtectionFilter
        (accountId: string, filterId: string, body: dos_ExpressionFilterUpdate, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("filter_id", filterId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/filters/{filter_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateSynProtectionFilter.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateSynProtectionFilter.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateSynProtectionFilter" (int status)
        }

    ///<summary>
    ///Delete all SYN Protection rules for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteSynProtectionRulesForAccount(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteSynProtectionRulesForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteSynProtectionRulesForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteSynProtectionRulesForAccount" (int status)
        }

    ///<summary>
    ///List all SYN Protection rules for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="page">The page number for pagination. Defaults to 1.</param>
    ///<param name="perPage">The number of items per page. Must be between 10 and 1000. Defaults to 25.</param>
    ///<param name="order">The field to order by. Defaults to 'prefix'.</param>
    ///<param name="direction">The direction of ordering (ASC or DESC). Defaults to 'ASC'.</param>
    ///<param name="cancellationToken"></param>
    member this.ListSynProtectionRulesForAccount
        (
            accountId: string,
            ?page: int64,
            ?perPage: int64,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.queryComma ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.queryComma ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.queryComma ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.queryComma ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListSynProtectionRulesForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListSynProtectionRulesForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListSynProtectionRulesForAccount" (int status)
        }

    ///<summary>
    ///Create a SYN Protection rule for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreateSynProtectionRule
        (accountId: string, body: dos_NewSynProtectionRule, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateSynProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreateSynProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateSynProtectionRule" (int status)
        }

    ///<summary>
    ///Delete a SYN Protection rule specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="ruleId">The UUID of the SYN Protection rule to delete.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteSynProtectionRule(accountId: string, ruleId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("rule_id", ruleId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteSynProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteSynProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteSynProtectionRule" (int status)
        }

    ///<summary>
    ///Get a SYN Protection rule specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="ruleId">The UUID of the SYN Protection rule.</param>
    ///<param name="cancellationToken"></param>
    member this.GetSynProtectionRule(accountId: string, ruleId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("rule_id", ruleId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetSynProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetSynProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetSynProtectionRule" (int status)
        }

    ///<summary>
    ///Update a SYN Protection rule specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="ruleId">The UUID of the SYN Protection rule to update.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateSynProtectionRule
        (accountId: string, ruleId: string, body: dos_SynProtectionRuleUpdate, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("rule_id", ruleId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/syn_protection/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateSynProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateSynProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateSynProtectionRule" (int status)
        }

    ///<summary>
    ///Delete all TCP Flow Protection filters for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteTcpFlowProtectionFiltersForAccount(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/filters"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteTcpFlowProtectionFiltersForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteTcpFlowProtectionFiltersForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for DeleteTcpFlowProtectionFiltersForAccount" (int status)
        }

    ///<summary>
    ///List all TCP Flow Protection filters for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="mode">The mode of the filters to get. Optional. Valid values: 'enabled', 'disabled', 'monitoring'.</param>
    ///<param name="page">The page number for pagination. Defaults to 1.</param>
    ///<param name="perPage">The number of items per page. Must be between 10 and 1000. Defaults to 25.</param>
    ///<param name="order">The field to order by. Defaults to 'prefix'.</param>
    ///<param name="direction">The direction of ordering (ASC or DESC). Defaults to 'ASC'.</param>
    ///<param name="cancellationToken"></param>
    member this.ListTcpFlowProtectionFiltersForAccount
        (
            accountId: string,
            ?mode: string,
            ?page: int64,
            ?perPage: int64,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if mode.IsSome then
                      RequestPart.queryComma ("mode", mode.Value)
                  if page.IsSome then
                      RequestPart.queryComma ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.queryComma ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.queryComma ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.queryComma ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/filters"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListTcpFlowProtectionFiltersForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListTcpFlowProtectionFiltersForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListTcpFlowProtectionFiltersForAccount" (int status)
        }

    ///<summary>
    ///Create a TCP Flow Protection filter for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreateTcpFlowProtectionFilter
        (accountId: string, body: dos_NewExpressionFilter, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/filters"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateTcpFlowProtectionFilter.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreateTcpFlowProtectionFilter.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateTcpFlowProtectionFilter" (int status)
        }

    ///<summary>
    ///Delete a TCP Flow Protection filter specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="filterId">The UUID of the filter to delete.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteTcpFlowProtectionFilter
        (accountId: string, filterId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("filter_id", filterId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/filters/{filter_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteTcpFlowProtectionFilter.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteTcpFlowProtectionFilter.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteTcpFlowProtectionFilter" (int status)
        }

    ///<summary>
    ///Get a TCP Flow Protection filter specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="filterId">The UUID of the filter to retrieve.</param>
    ///<param name="cancellationToken"></param>
    member this.GetTcpFlowProtectionFilter(accountId: string, filterId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("filter_id", filterId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/filters/{filter_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetTcpFlowProtectionFilter.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetTcpFlowProtectionFilter.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetTcpFlowProtectionFilter" (int status)
        }

    ///<summary>
    ///Update a TCP Flow Protection filter specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="filterId">The UUID of the filter to update.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateTcpFlowProtectionFilter
        (accountId: string, filterId: string, body: dos_ExpressionFilterUpdate, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("filter_id", filterId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/filters/{filter_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateTcpFlowProtectionFilter.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateTcpFlowProtectionFilter.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateTcpFlowProtectionFilter" (int status)
        }

    ///<summary>
    ///Delete all TCP Flow Protection rules for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteTcpFlowProtectionRulesForAccount(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteTcpFlowProtectionRulesForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteTcpFlowProtectionRulesForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteTcpFlowProtectionRulesForAccount" (int status)
        }

    ///<summary>
    ///List all TCP Flow Protection rules for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="page">The page number for pagination. Defaults to 1.</param>
    ///<param name="perPage">The number of items per page. Must be between 10 and 1000. Defaults to 25.</param>
    ///<param name="order">The field to order by. Defaults to 'prefix'.</param>
    ///<param name="direction">The direction of ordering (ASC or DESC). Defaults to 'ASC'.</param>
    ///<param name="cancellationToken"></param>
    member this.ListTcpFlowProtectionRulesForAccount
        (
            accountId: string,
            ?page: int64,
            ?perPage: int64,
            ?order: string,
            ?direction: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.queryComma ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.queryComma ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.queryComma ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.queryComma ("direction", direction.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ListTcpFlowProtectionRulesForAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ListTcpFlowProtectionRulesForAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ListTcpFlowProtectionRulesForAccount" (int status)
        }

    ///<summary>
    ///Create a TCP Flow Protection rule for an account.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.CreateTcpFlowProtectionRule
        (accountId: string, body: dos_NewTcpFlowProtectionRule, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CreateTcpFlowProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CreateTcpFlowProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CreateTcpFlowProtectionRule" (int status)
        }

    ///<summary>
    ///Delete a TCP Flow Protection rule specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="ruleId">The UUID of the TCP Flow Protection rule to delete.</param>
    ///<param name="cancellationToken"></param>
    member this.DeleteTcpFlowProtectionRule(accountId: string, ruleId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("rule_id", ruleId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DeleteTcpFlowProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DeleteTcpFlowProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteTcpFlowProtectionRule" (int status)
        }

    ///<summary>
    ///Get a TCP Flow Protection rule specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="ruleId">The UUID of the TCP Flow Protection rule.</param>
    ///<param name="cancellationToken"></param>
    member this.GetTcpFlowProtectionRule(accountId: string, ruleId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("rule_id", ruleId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetTcpFlowProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetTcpFlowProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetTcpFlowProtectionRule" (int status)
        }

    ///<summary>
    ///Update a TCP Flow Protection rule specified by the given UUID.
    ///</summary>
    ///<param name="accountId">The ID of the account.</param>
    ///<param name="ruleId">The UUID of the TCP Flow Protection rule to update.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateTcpFlowProtectionRule
        (accountId: string, ruleId: string, body: dos_TcpFlowProtectionRuleUpdate, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("rule_id", ruleId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_flow_protection/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateTcpFlowProtectionRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateTcpFlowProtectionRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateTcpFlowProtectionRule" (int status)
        }

    ///<summary>
    ///Get the protection status of the account.
    ///</summary>
    ///<param name="accountId">The account ID.</param>
    ///<param name="cancellationToken"></param>
    member this.GetProtectionStatus(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_protection_status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return GetProtectionStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetProtectionStatus.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetProtectionStatus" (int status)
        }

    ///<summary>
    ///Update the protection status of the account.
    ///</summary>
    ///<param name="accountId">The account ID.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.UpdateProtectionStatus
        (accountId: string, body: dos_UpdateProtectionStatus, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/advanced_tcp_protection/configs/tcp_protection_status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return UpdateProtectionStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateProtectionStatus.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateProtectionStatus" (int status)
        }

    ///<summary>
    ///Lists Apps associated with an account.
    ///</summary>
    member this.MagicAccountAppsListApps(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/magic/apps" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicAccountAppsListApps.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicAccountAppsListApps.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicAccountAppsListApps" (int status)
        }

    ///<summary>
    ///Creates a new App for an account
    ///</summary>
    member this.MagicAccountAppsAddApp
        (accountId: string, body: magic_app_add_single_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/magic/apps" requestParts cancellationToken

            match (int status) with
            | 201 -> return MagicAccountAppsAddApp.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicAccountAppsAddApp.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicAccountAppsAddApp" (int status)
        }

    ///<summary>
    ///Deletes specific Account App.
    ///</summary>
    member this.MagicAccountAppsDeleteApp
        (accountId: string, accountAppId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("account_app_id", accountAppId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/apps/{account_app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicAccountAppsDeleteApp.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicAccountAppsDeleteApp.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicAccountAppsDeleteApp" (int status)
        }

    ///<summary>
    ///Updates an Account App
    ///</summary>
    member this.MagicAccountAppsPatchApp
        (accountId: string, accountAppId: string, body: magic_app_update_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("account_app_id", accountAppId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/apps/{account_app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicAccountAppsPatchApp.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicAccountAppsPatchApp.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicAccountAppsPatchApp" (int status)
        }

    ///<summary>
    ///Updates an Account App
    ///</summary>
    member this.MagicAccountAppsUpdateApp
        (accountId: string, accountAppId: string, body: magic_app_update_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("account_app_id", accountAppId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/apps/{account_app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicAccountAppsUpdateApp.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicAccountAppsUpdateApp.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicAccountAppsUpdateApp" (int status)
        }

    ///<summary>
    ///Lists all BGP filter profiles for an account.
    ///</summary>
    member this.MagicBgpListFilterProfiles(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/bgp/filter_profiles"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicBgpListFilterProfiles.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicBgpListFilterProfiles.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicBgpListFilterProfiles" (int status)
        }

    ///<summary>
    ///Creates a new BGP filter profile for an account.
    ///</summary>
    member this.MagicBgpCreateFilterProfile
        (accountId: string, body: magic_create_bgp_filter_profile_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/bgp/filter_profiles"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicBgpCreateFilterProfile.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicBgpCreateFilterProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicBgpCreateFilterProfile" (int status)
        }

    ///<summary>
    ///Deletes a BGP filter profile.
    ///</summary>
    member this.MagicBgpDeleteFilterProfile
        (accountId: string, profileId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("profile_id", profileId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/bgp/filter_profiles/{profile_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicBgpDeleteFilterProfile.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicBgpDeleteFilterProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicBgpDeleteFilterProfile" (int status)
        }

    ///<summary>
    ///Gets a specific BGP filter profile for an account.
    ///</summary>
    member this.MagicBgpGetFilterProfile(accountId: string, profileId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("profile_id", profileId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/bgp/filter_profiles/{profile_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicBgpGetFilterProfile.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicBgpGetFilterProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicBgpGetFilterProfile" (int status)
        }

    ///<summary>
    ///Updates a BGP filter profile. Omitted properties are left unchanged. To clear an existing description send `description: ""`.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="profileId"></param>
    ///<param name="body">Partial update for a BGP filter profile. At least one property must be provided; omitted properties are left unchanged. To clear an existing description send `description: ""`.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicBgpUpdateFilterProfile
        (
            accountId: string,
            profileId: string,
            body: magic_update_bgp_filter_profile_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("profile_id", profileId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/bgp/filter_profiles/{profile_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicBgpUpdateFilterProfile.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicBgpUpdateFilterProfile.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicBgpUpdateFilterProfile" (int status)
        }

    ///<summary>
    ///Gets the BGP settings for an account, including the default ASN and redistribution configuration.
    ///</summary>
    member this.MagicBgpGetSettings(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/bgp/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicBgpGetSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicBgpGetSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicBgpGetSettings" (int status)
        }

    ///<summary>
    ///Modifies the BGP settings for an account, including the default ASN and redistribution configuration.
    ///</summary>
    member this.MagicBgpUpdateSettings
        (accountId: string, body: magic_update_bgp_settings_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/bgp/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicBgpUpdateSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicBgpUpdateSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicBgpUpdateSettings" (int status)
        }

    ///<summary>
    ///Lists CF1 Sites associated with an account. A CF1 Site represents a physical customer network location with optional geographic coordinates.
    ///</summary>
    member this.MagicCf1SitesListCf1Sites(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/magic/cf1_sites" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicCf1SitesListCf1Sites.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicCf1SitesListCf1Sites.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicCf1SitesListCf1Sites" (int status)
        }

    ///<summary>
    ///Creates new CF1 Sites for an account. Each site must have a unique name within the account.
    ///</summary>
    member this.MagicCf1SitesCreateCf1Sites
        (accountId: string, body: list<magic_cf1_site>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/magic/cf1_sites" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicCf1SitesCreateCf1Sites.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicCf1SitesCreateCf1Sites.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicCf1SitesCreateCf1Sites" (int status)
        }

    ///<summary>
    ///Deletes a specific CF1 Site for an account.
    ///</summary>
    member this.MagicCf1SitesDeleteCf1Site
        (accountId: string, cf1SiteId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("cf1_site_id", cf1SiteId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf1_sites/{cf1_site_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicCf1SitesDeleteCf1Site.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicCf1SitesDeleteCf1Site.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicCf1SitesDeleteCf1Site" (int status)
        }

    ///<summary>
    ///Gets a specific CF1 Site for an account.
    ///</summary>
    member this.MagicCf1SitesGetCf1Site(accountId: string, cf1SiteId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("cf1_site_id", cf1SiteId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf1_sites/{cf1_site_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicCf1SitesGetCf1Site.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicCf1SitesGetCf1Site.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicCf1SitesGetCf1Site" (int status)
        }

    ///<summary>
    ///Partially updates a specific CF1 Site for an account. Only the fields included in the request body are modified; omitted fields retain their existing values.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="cf1SiteId"></param>
    ///<param name="body">Partial update payload for a CF1 Site. All properties are optional; only fields supplied in the request body are modified.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicCf1SitesUpdateCf1Site
        (accountId: string, cf1SiteId: string, body: magic_cf1_site_update, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("cf1_site_id", cf1SiteId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf1_sites/{cf1_site_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicCf1SitesUpdateCf1Site.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicCf1SitesUpdateCf1Site.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicCf1SitesUpdateCf1Site" (int status)
        }

    ///<summary>
    ///Lists ramps (network connections) associated with a CF1 Site. Ramps represent GRE tunnels, IPsec tunnels, interconnects, or MCONN links.
    ///</summary>
    member this.MagicCf1SitesListCf1SiteRamps
        (accountId: string, cf1SiteId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("cf1_site_id", cf1SiteId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf1_sites/{cf1_site_id}/ramps"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicCf1SitesListCf1SiteRamps.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicCf1SitesListCf1SiteRamps.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicCf1SitesListCf1SiteRamps" (int status)
        }

    ///<summary>
    ///Creates ramps (network connections) for a CF1 Site.
    ///</summary>
    member this.MagicCf1SitesCreateCf1SiteRamps
        (
            accountId: string,
            cf1SiteId: string,
            body: list<magic_cf1_site_ramp_body>,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("cf1_site_id", cf1SiteId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf1_sites/{cf1_site_id}/ramps"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicCf1SitesCreateCf1SiteRamps.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicCf1SitesCreateCf1SiteRamps.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicCf1SitesCreateCf1SiteRamps" (int status)
        }

    ///<summary>
    ///Deletes a specific ramp from a CF1 Site.
    ///</summary>
    member this.MagicCf1SitesDeleteCf1SiteRamp
        (accountId: string, cf1SiteId: string, rampId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("cf1_site_id", cf1SiteId)
                  RequestPart.path ("ramp_id", rampId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf1_sites/{cf1_site_id}/ramps/{ramp_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicCf1SitesDeleteCf1SiteRamp.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicCf1SitesDeleteCf1SiteRamp.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicCf1SitesDeleteCf1SiteRamp" (int status)
        }

    ///<summary>
    ///Gets a specific ramp for a CF1 Site.
    ///</summary>
    member this.MagicCf1SitesGetCf1SiteRamp
        (accountId: string, cf1SiteId: string, rampId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("cf1_site_id", cf1SiteId)
                  RequestPart.path ("ramp_id", rampId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf1_sites/{cf1_site_id}/ramps/{ramp_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicCf1SitesGetCf1SiteRamp.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicCf1SitesGetCf1SiteRamp.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicCf1SitesGetCf1SiteRamp" (int status)
        }

    ///<summary>
    ///Lists interconnects associated with an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the response body will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicInterconnectsListInterconnects
        (accountId: string, ?xMagicNewHcTarget: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf_interconnects"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicInterconnectsListInterconnects.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicInterconnectsListInterconnects.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicInterconnectsListInterconnects" (int status)
        }

    ///<summary>
    ///Updates multiple interconnects associated with an account. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the request and response bodies will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicInterconnectsUpdateMultipleInterconnects
        (
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?xMagicNewHcTarget: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf_interconnects"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicInterconnectsUpdateMultipleInterconnects.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicInterconnectsUpdateMultipleInterconnects.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for MagicInterconnectsUpdateMultipleInterconnects" (int status)
        }

    ///<summary>
    ///Lists details for a specific interconnect.
    ///</summary>
    ///<param name="cfInterconnectId"></param>
    ///<param name="accountId"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the response body will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicInterconnectsListInterconnectDetails
        (cfInterconnectId: string, accountId: string, ?xMagicNewHcTarget: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("cf_interconnect_id", cfInterconnectId)
                  RequestPart.path ("account_id", accountId)
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf_interconnects/{cf_interconnect_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicInterconnectsListInterconnectDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicInterconnectsListInterconnectDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicInterconnectsListInterconnectDetails" (int status)
        }

    ///<summary>
    ///Updates a specific interconnect associated with an account. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes.
    ///</summary>
    ///<param name="cfInterconnectId"></param>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the request and response bodies will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicInterconnectsUpdateInterconnect
        (
            cfInterconnectId: string,
            accountId: string,
            body: magic_interconnect_tunnel_update_request,
            ?xMagicNewHcTarget: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("cf_interconnect_id", cfInterconnectId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/cf_interconnects/{cf_interconnect_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicInterconnectsUpdateInterconnect.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicInterconnectsUpdateInterconnect.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicInterconnectsUpdateInterconnect" (int status)
        }

    ///<summary>
    ///List Catalog Syncs (Closed Beta).
    ///</summary>
    member this.CatalogSyncsList(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/catalog-syncs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CatalogSyncsList.OK((Serializer.deserialize content))
            | 400 -> return CatalogSyncsList.BadRequest((Serializer.deserialize content))
            | 401 -> return CatalogSyncsList.Unauthorized((Serializer.deserialize content))
            | 403 -> return CatalogSyncsList.Forbidden((Serializer.deserialize content))
            | 404 -> return CatalogSyncsList.NotFound((Serializer.deserialize content))
            | 500 -> return CatalogSyncsList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CatalogSyncsList" (int status)
        }

    ///<summary>
    ///Create a new Catalog Sync (Closed Beta).
    ///</summary>
    member this.CatalogSyncsCreate
        (
            accountId: string,
            body: mcn_create_catalog_sync_request,
            ?forwarded: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if forwarded.IsSome then
                      RequestPart.header ("forwarded", forwarded.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/catalog-syncs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return CatalogSyncsCreate.Created((Serializer.deserialize content))
            | 400 -> return CatalogSyncsCreate.BadRequest((Serializer.deserialize content))
            | 401 -> return CatalogSyncsCreate.Unauthorized((Serializer.deserialize content))
            | 403 -> return CatalogSyncsCreate.Forbidden((Serializer.deserialize content))
            | 409 -> return CatalogSyncsCreate.Conflict((Serializer.deserialize content))
            | 422 -> return CatalogSyncsCreate.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return CatalogSyncsCreate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CatalogSyncsCreate" (int status)
        }

    ///<summary>
    ///List prebuilt catalog sync policies (Closed Beta).
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="destinationType">Specify type of destination, omit to return all.</param>
    ///<param name="cancellationToken"></param>
    member this.CatalogSyncsPrebuiltPoliciesList
        (accountId: string, ?destinationType: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if destinationType.IsSome then
                      RequestPart.query ("destination_type", destinationType.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/catalog-syncs/prebuilt-policies"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CatalogSyncsPrebuiltPoliciesList.OK((Serializer.deserialize content))
            | 400 -> return CatalogSyncsPrebuiltPoliciesList.BadRequest((Serializer.deserialize content))
            | 401 -> return CatalogSyncsPrebuiltPoliciesList.Unauthorized((Serializer.deserialize content))
            | 403 -> return CatalogSyncsPrebuiltPoliciesList.Forbidden((Serializer.deserialize content))
            | 500 -> return CatalogSyncsPrebuiltPoliciesList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CatalogSyncsPrebuiltPoliciesList" (int status)
        }

    ///<summary>
    ///Delete a Catalog Sync (Closed Beta).
    ///</summary>
    member this.CatalogSyncsDelete
        (accountId: string, syncId: System.Guid, ?deleteDestination: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("sync_id", syncId)
                  if deleteDestination.IsSome then
                      RequestPart.query ("delete_destination", deleteDestination.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/catalog-syncs/{sync_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CatalogSyncsDelete.OK((Serializer.deserialize content))
            | 400 -> return CatalogSyncsDelete.BadRequest((Serializer.deserialize content))
            | 401 -> return CatalogSyncsDelete.Unauthorized((Serializer.deserialize content))
            | 403 -> return CatalogSyncsDelete.Forbidden((Serializer.deserialize content))
            | 404 -> return CatalogSyncsDelete.NotFound((Serializer.deserialize content))
            | 409 -> return CatalogSyncsDelete.Conflict((Serializer.deserialize content))
            | 500 -> return CatalogSyncsDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CatalogSyncsDelete" (int status)
        }

    ///<summary>
    ///Read a Catalog Sync (Closed Beta).
    ///</summary>
    member this.CatalogSyncsRead(accountId: string, syncId: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("sync_id", syncId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/catalog-syncs/{sync_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CatalogSyncsRead.OK((Serializer.deserialize content))
            | 400 -> return CatalogSyncsRead.BadRequest((Serializer.deserialize content))
            | 401 -> return CatalogSyncsRead.Unauthorized((Serializer.deserialize content))
            | 403 -> return CatalogSyncsRead.Forbidden((Serializer.deserialize content))
            | 404 -> return CatalogSyncsRead.NotFound((Serializer.deserialize content))
            | 500 -> return CatalogSyncsRead.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CatalogSyncsRead" (int status)
        }

    ///<summary>
    ///Update a Catalog Sync (Closed Beta).
    ///</summary>
    member this.CatalogSyncsPatch
        (
            accountId: string,
            syncId: System.Guid,
            body: mcn_update_catalog_sync_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("sync_id", syncId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/catalog-syncs/{sync_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CatalogSyncsPatch.OK((Serializer.deserialize content))
            | 400 -> return CatalogSyncsPatch.BadRequest((Serializer.deserialize content))
            | 401 -> return CatalogSyncsPatch.Unauthorized((Serializer.deserialize content))
            | 403 -> return CatalogSyncsPatch.Forbidden((Serializer.deserialize content))
            | 404 -> return CatalogSyncsPatch.NotFound((Serializer.deserialize content))
            | 409 -> return CatalogSyncsPatch.Conflict((Serializer.deserialize content))
            | 422 -> return CatalogSyncsPatch.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return CatalogSyncsPatch.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CatalogSyncsPatch" (int status)
        }

    ///<summary>
    ///Update a Catalog Sync (Closed Beta).
    ///</summary>
    member this.CatalogSyncsUpdate
        (
            accountId: string,
            syncId: System.Guid,
            body: mcn_update_catalog_sync_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("sync_id", syncId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/catalog-syncs/{sync_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CatalogSyncsUpdate.OK((Serializer.deserialize content))
            | 400 -> return CatalogSyncsUpdate.BadRequest((Serializer.deserialize content))
            | 401 -> return CatalogSyncsUpdate.Unauthorized((Serializer.deserialize content))
            | 403 -> return CatalogSyncsUpdate.Forbidden((Serializer.deserialize content))
            | 404 -> return CatalogSyncsUpdate.NotFound((Serializer.deserialize content))
            | 409 -> return CatalogSyncsUpdate.Conflict((Serializer.deserialize content))
            | 422 -> return CatalogSyncsUpdate.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return CatalogSyncsUpdate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CatalogSyncsUpdate" (int status)
        }

    ///<summary>
    ///Refresh a Catalog Sync's destination by running the sync policy against latest resource catalog (Closed Beta).
    ///</summary>
    member this.CatalogSyncsRefresh(accountId: string, syncId: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("sync_id", syncId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/catalog-syncs/{sync_id}/refresh"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CatalogSyncsRefresh.OK((Serializer.deserialize content))
            | 400 -> return CatalogSyncsRefresh.BadRequest((Serializer.deserialize content))
            | 401 -> return CatalogSyncsRefresh.Unauthorized((Serializer.deserialize content))
            | 403 -> return CatalogSyncsRefresh.Forbidden((Serializer.deserialize content))
            | 404 -> return CatalogSyncsRefresh.NotFound((Serializer.deserialize content))
            | 409 -> return CatalogSyncsRefresh.Conflict((Serializer.deserialize content))
            | 422 -> return CatalogSyncsRefresh.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return CatalogSyncsRefresh.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CatalogSyncsRefresh" (int status)
        }

    ///<summary>
    ///List On-ramps (Closed Beta).
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="orderBy">One of ["updated_at", "id", "cloud_type", "name"].</param>
    ///<param name="desc"></param>
    ///<param name="status"></param>
    ///<param name="vpcs"></param>
    ///<param name="cancellationToken"></param>
    member this.OnrampsList
        (
            accountId: string,
            ?orderBy: string,
            ?desc: bool,
            ?status: bool,
            ?vpcs: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if desc.IsSome then
                      RequestPart.query ("desc", desc.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if vpcs.IsSome then
                      RequestPart.query ("vpcs", vpcs.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OnrampsList.OK((Serializer.deserialize content))
            | 400 -> return OnrampsList.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsList.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsList.Forbidden((Serializer.deserialize content))
            | 500 -> return OnrampsList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsList" (int status)
        }

    ///<summary>
    ///Create a new On-ramp (Closed Beta).
    ///</summary>
    member this.OnrampsCreate
        (accountId: string, body: mcn_create_onramp_request, ?forwarded: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if forwarded.IsSome then
                      RequestPart.header ("forwarded", forwarded.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return OnrampsCreate.Created((Serializer.deserialize content))
            | 400 -> return OnrampsCreate.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsCreate.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsCreate.Forbidden((Serializer.deserialize content))
            | 409 -> return OnrampsCreate.Conflict((Serializer.deserialize content))
            | 422 -> return OnrampsCreate.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return OnrampsCreate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsCreate" (int status)
        }

    ///<summary>
    ///Read the Magic WAN Address Space (Closed Beta).
    ///</summary>
    member this.OnrampsMwanAddrSpaceRead(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps/magic_wan_address_space"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OnrampsMwanAddrSpaceRead.OK((Serializer.deserialize content))
            | 400 -> return OnrampsMwanAddrSpaceRead.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsMwanAddrSpaceRead.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsMwanAddrSpaceRead.Forbidden((Serializer.deserialize content))
            | 404 -> return OnrampsMwanAddrSpaceRead.NotFound((Serializer.deserialize content))
            | 500 -> return OnrampsMwanAddrSpaceRead.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsMwanAddrSpaceRead" (int status)
        }

    ///<summary>
    ///Update the Magic WAN Address Space (Closed Beta).
    ///</summary>
    member this.OnrampsMwanAddrSpacePatch
        (accountId: string, body: mcn_update_magic_wan_address_space_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps/magic_wan_address_space"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OnrampsMwanAddrSpacePatch.OK((Serializer.deserialize content))
            | 400 -> return OnrampsMwanAddrSpacePatch.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsMwanAddrSpacePatch.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsMwanAddrSpacePatch.Forbidden((Serializer.deserialize content))
            | 404 -> return OnrampsMwanAddrSpacePatch.NotFound((Serializer.deserialize content))
            | 422 -> return OnrampsMwanAddrSpacePatch.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return OnrampsMwanAddrSpacePatch.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsMwanAddrSpacePatch" (int status)
        }

    ///<summary>
    ///Update the Magic WAN Address Space (Closed Beta).
    ///</summary>
    member this.OnrampsMwanAddrSpaceUpdate
        (accountId: string, body: mcn_update_magic_wan_address_space_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps/magic_wan_address_space"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OnrampsMwanAddrSpaceUpdate.OK((Serializer.deserialize content))
            | 400 -> return OnrampsMwanAddrSpaceUpdate.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsMwanAddrSpaceUpdate.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsMwanAddrSpaceUpdate.Forbidden((Serializer.deserialize content))
            | 404 -> return OnrampsMwanAddrSpaceUpdate.NotFound((Serializer.deserialize content))
            | 422 -> return OnrampsMwanAddrSpaceUpdate.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return OnrampsMwanAddrSpaceUpdate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsMwanAddrSpaceUpdate" (int status)
        }

    ///<summary>
    ///Delete an On-ramp (Closed Beta).
    ///</summary>
    member this.OnrampsDelete
        (accountId: string, onrampId: System.Guid, ?destroy: bool, ?force: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("onramp_id", onrampId)
                  if destroy.IsSome then
                      RequestPart.query ("destroy", destroy.Value)
                  if force.IsSome then
                      RequestPart.query ("force", force.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps/{onramp_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OnrampsDelete.OK((Serializer.deserialize content))
            | 400 -> return OnrampsDelete.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsDelete.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsDelete.Forbidden((Serializer.deserialize content))
            | 404 -> return OnrampsDelete.NotFound((Serializer.deserialize content))
            | 409 -> return OnrampsDelete.Conflict((Serializer.deserialize content))
            | 500 -> return OnrampsDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsDelete" (int status)
        }

    ///<summary>
    ///Read an On-ramp (Closed Beta).
    ///</summary>
    member this.OnrampsRead
        (
            accountId: string,
            onrampId: System.Guid,
            ?status: bool,
            ?vpcs: bool,
            ?postApplyResources: bool,
            ?plannedResources: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("onramp_id", onrampId)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if vpcs.IsSome then
                      RequestPart.query ("vpcs", vpcs.Value)
                  if postApplyResources.IsSome then
                      RequestPart.query ("post_apply_resources", postApplyResources.Value)
                  if plannedResources.IsSome then
                      RequestPart.query ("planned_resources", plannedResources.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps/{onramp_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OnrampsRead.OK((Serializer.deserialize content))
            | 400 -> return OnrampsRead.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsRead.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsRead.Forbidden((Serializer.deserialize content))
            | 404 -> return OnrampsRead.NotFound((Serializer.deserialize content))
            | 500 -> return OnrampsRead.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsRead" (int status)
        }

    ///<summary>
    ///Update an On-ramp (Closed Beta).
    ///</summary>
    member this.OnrampsPatch
        (
            accountId: string,
            onrampId: System.Guid,
            body: mcn_update_onramp_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("onramp_id", onrampId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps/{onramp_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OnrampsPatch.OK((Serializer.deserialize content))
            | 400 -> return OnrampsPatch.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsPatch.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsPatch.Forbidden((Serializer.deserialize content))
            | 404 -> return OnrampsPatch.NotFound((Serializer.deserialize content))
            | 409 -> return OnrampsPatch.Conflict((Serializer.deserialize content))
            | 422 -> return OnrampsPatch.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return OnrampsPatch.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsPatch" (int status)
        }

    ///<summary>
    ///Update an On-ramp (Closed Beta).
    ///</summary>
    member this.OnrampsUpdate
        (
            accountId: string,
            onrampId: System.Guid,
            body: mcn_update_onramp_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("onramp_id", onrampId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps/{onramp_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OnrampsUpdate.OK((Serializer.deserialize content))
            | 400 -> return OnrampsUpdate.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsUpdate.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsUpdate.Forbidden((Serializer.deserialize content))
            | 404 -> return OnrampsUpdate.NotFound((Serializer.deserialize content))
            | 409 -> return OnrampsUpdate.Conflict((Serializer.deserialize content))
            | 422 -> return OnrampsUpdate.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return OnrampsUpdate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsUpdate" (int status)
        }

    ///<summary>
    ///Apply an On-ramp (Closed Beta).
    ///</summary>
    member this.OnrampsApply(accountId: string, onrampId: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("onramp_id", onrampId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps/{onramp_id}/apply"
                    requestParts
                    cancellationToken

            match (int status) with
            | 202 -> return OnrampsApply.Accepted((Serializer.deserialize content))
            | 400 -> return OnrampsApply.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsApply.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsApply.Forbidden((Serializer.deserialize content))
            | 404 -> return OnrampsApply.NotFound((Serializer.deserialize content))
            | 409 -> return OnrampsApply.Conflict((Serializer.deserialize content))
            | 500 -> return OnrampsApply.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsApply" (int status)
        }

    ///<summary>
    ///Export an On-ramp to terraform ready file(s) (Closed Beta).
    ///</summary>
    member this.OnrampsExport(accountId: string, onrampId: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("onramp_id", onrampId) ]

            let! (status, _, contentBinary) =
                OpenApiHttp.postBinaryAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps/{onramp_id}/export"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return OnrampsExport.Created(contentBinary)
            | 400 ->
                let content = Encoding.UTF8.GetString contentBinary
                return OnrampsExport.BadRequest((Serializer.deserialize content))
            | 401 ->
                let content = Encoding.UTF8.GetString contentBinary
                return OnrampsExport.Unauthorized((Serializer.deserialize content))
            | 403 ->
                let content = Encoding.UTF8.GetString contentBinary
                return OnrampsExport.Forbidden((Serializer.deserialize content))
            | 404 ->
                let content = Encoding.UTF8.GetString contentBinary
                return OnrampsExport.NotFound((Serializer.deserialize content))
            | 409 ->
                let content = Encoding.UTF8.GetString contentBinary
                return OnrampsExport.Conflict((Serializer.deserialize content))
            | 500 ->
                let content = Encoding.UTF8.GetString contentBinary
                return OnrampsExport.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsExport" (int status)
        }

    ///<summary>
    ///Plan an On-ramp (Closed Beta).
    ///</summary>
    member this.OnrampsPlan(accountId: string, onrampId: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("onramp_id", onrampId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/onramps/{onramp_id}/plan"
                    requestParts
                    cancellationToken

            match (int status) with
            | 202 -> return OnrampsPlan.Accepted((Serializer.deserialize content))
            | 400 -> return OnrampsPlan.BadRequest((Serializer.deserialize content))
            | 401 -> return OnrampsPlan.Unauthorized((Serializer.deserialize content))
            | 403 -> return OnrampsPlan.Forbidden((Serializer.deserialize content))
            | 404 -> return OnrampsPlan.NotFound((Serializer.deserialize content))
            | 409 -> return OnrampsPlan.Conflict((Serializer.deserialize content))
            | 500 -> return OnrampsPlan.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OnrampsPlan" (int status)
        }

    ///<summary>
    ///List Cloud Integrations (Closed Beta).
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="status"></param>
    ///<param name="orderBy">One of ["updated_at", "id", "cloud_type", "name"].</param>
    ///<param name="desc"></param>
    ///<param name="cloudflare"></param>
    ///<param name="cancellationToken"></param>
    member this.ProvidersList
        (
            accountId: string,
            ?status: bool,
            ?orderBy: string,
            ?desc: bool,
            ?cloudflare: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if desc.IsSome then
                      RequestPart.query ("desc", desc.Value)
                  if cloudflare.IsSome then
                      RequestPart.query ("cloudflare", cloudflare.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/providers"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ProvidersList.OK((Serializer.deserialize content))
            | 400 -> return ProvidersList.BadRequest((Serializer.deserialize content))
            | 401 -> return ProvidersList.Unauthorized((Serializer.deserialize content))
            | 403 -> return ProvidersList.Forbidden((Serializer.deserialize content))
            | 500 -> return ProvidersList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ProvidersList" (int status)
        }

    ///<summary>
    ///Create a new Cloud Integration (Closed Beta).
    ///</summary>
    member this.ProvidersCreate
        (accountId: string, body: mcn_create_provider_request, ?forwarded: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if forwarded.IsSome then
                      RequestPart.header ("forwarded", forwarded.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/providers"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return ProvidersCreate.Created((Serializer.deserialize content))
            | 400 -> return ProvidersCreate.BadRequest((Serializer.deserialize content))
            | 401 -> return ProvidersCreate.Unauthorized((Serializer.deserialize content))
            | 403 -> return ProvidersCreate.Forbidden((Serializer.deserialize content))
            | 409 -> return ProvidersCreate.Conflict((Serializer.deserialize content))
            | 422 -> return ProvidersCreate.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return ProvidersCreate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ProvidersCreate" (int status)
        }

    ///<summary>
    ///Run discovery for all Cloud Integrations in an account (Closed Beta).
    ///</summary>
    member this.ProvidersDiscoverAll(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/providers/discover"
                    requestParts
                    cancellationToken

            match (int status) with
            | 202 -> return ProvidersDiscoverAll.Accepted((Serializer.deserialize content))
            | 400 -> return ProvidersDiscoverAll.BadRequest((Serializer.deserialize content))
            | 401 -> return ProvidersDiscoverAll.Unauthorized((Serializer.deserialize content))
            | 403 -> return ProvidersDiscoverAll.Forbidden((Serializer.deserialize content))
            | 409 -> return ProvidersDiscoverAll.Conflict((Serializer.deserialize content))
            | 500 -> return ProvidersDiscoverAll.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ProvidersDiscoverAll" (int status)
        }

    ///<summary>
    ///Delete a Cloud Integration (Closed Beta).
    ///</summary>
    member this.ProvidersDelete(accountId: string, providerId: System.Guid, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("provider_id", providerId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/providers/{provider_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ProvidersDelete.OK((Serializer.deserialize content))
            | 400 -> return ProvidersDelete.BadRequest((Serializer.deserialize content))
            | 401 -> return ProvidersDelete.Unauthorized((Serializer.deserialize content))
            | 403 -> return ProvidersDelete.Forbidden((Serializer.deserialize content))
            | 404 -> return ProvidersDelete.NotFound((Serializer.deserialize content))
            | 500 -> return ProvidersDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ProvidersDelete" (int status)
        }

    ///<summary>
    ///Read a Cloud Integration (Closed Beta).
    ///</summary>
    member this.ProvidersRead
        (accountId: string, providerId: System.Guid, ?status: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("provider_id", providerId)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/providers/{provider_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ProvidersRead.OK((Serializer.deserialize content))
            | 400 -> return ProvidersRead.BadRequest((Serializer.deserialize content))
            | 401 -> return ProvidersRead.Unauthorized((Serializer.deserialize content))
            | 403 -> return ProvidersRead.Forbidden((Serializer.deserialize content))
            | 404 -> return ProvidersRead.NotFound((Serializer.deserialize content))
            | 500 -> return ProvidersRead.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ProvidersRead" (int status)
        }

    ///<summary>
    ///Update a Cloud Integration (Closed Beta).
    ///</summary>
    member this.ProvidersPatch
        (
            accountId: string,
            providerId: System.Guid,
            body: mcn_update_provider_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("provider_id", providerId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/providers/{provider_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ProvidersPatch.OK((Serializer.deserialize content))
            | 400 -> return ProvidersPatch.BadRequest((Serializer.deserialize content))
            | 401 -> return ProvidersPatch.Unauthorized((Serializer.deserialize content))
            | 403 -> return ProvidersPatch.Forbidden((Serializer.deserialize content))
            | 404 -> return ProvidersPatch.NotFound((Serializer.deserialize content))
            | 409 -> return ProvidersPatch.Conflict((Serializer.deserialize content))
            | 422 -> return ProvidersPatch.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return ProvidersPatch.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ProvidersPatch" (int status)
        }

    ///<summary>
    ///Update a Cloud Integration (Closed Beta).
    ///</summary>
    member this.ProvidersUpdate
        (
            accountId: string,
            providerId: System.Guid,
            body: mcn_update_provider_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("provider_id", providerId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/providers/{provider_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ProvidersUpdate.OK((Serializer.deserialize content))
            | 400 -> return ProvidersUpdate.BadRequest((Serializer.deserialize content))
            | 401 -> return ProvidersUpdate.Unauthorized((Serializer.deserialize content))
            | 403 -> return ProvidersUpdate.Forbidden((Serializer.deserialize content))
            | 404 -> return ProvidersUpdate.NotFound((Serializer.deserialize content))
            | 409 -> return ProvidersUpdate.Conflict((Serializer.deserialize content))
            | 422 -> return ProvidersUpdate.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return ProvidersUpdate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ProvidersUpdate" (int status)
        }

    ///<summary>
    ///Run discovery for a Cloud Integration (Closed Beta).
    ///</summary>
    member this.ProvidersDiscover
        (accountId: string, providerId: System.Guid, ?v2: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("provider_id", providerId)
                  if v2.IsSome then
                      RequestPart.query ("v2", v2.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/providers/{provider_id}/discover"
                    requestParts
                    cancellationToken

            match (int status) with
            | 202 -> return ProvidersDiscover.Accepted((Serializer.deserialize content))
            | 400 -> return ProvidersDiscover.BadRequest((Serializer.deserialize content))
            | 401 -> return ProvidersDiscover.Unauthorized((Serializer.deserialize content))
            | 403 -> return ProvidersDiscover.Forbidden((Serializer.deserialize content))
            | 404 -> return ProvidersDiscover.NotFound((Serializer.deserialize content))
            | 409 -> return ProvidersDiscover.Conflict((Serializer.deserialize content))
            | 500 -> return ProvidersDiscover.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ProvidersDiscover" (int status)
        }

    ///<summary>
    ///Get initial configuration to complete Cloud Integration setup (Closed Beta).
    ///</summary>
    member this.ProvidersInitialSetup
        (accountId: string, providerId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("provider_id", providerId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/providers/{provider_id}/initial_setup"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ProvidersInitialSetup.OK((Serializer.deserialize content))
            | 400 -> return ProvidersInitialSetup.BadRequest((Serializer.deserialize content))
            | 401 -> return ProvidersInitialSetup.Unauthorized((Serializer.deserialize content))
            | 403 -> return ProvidersInitialSetup.Forbidden((Serializer.deserialize content))
            | 404 -> return ProvidersInitialSetup.NotFound((Serializer.deserialize content))
            | 500 -> return ProvidersInitialSetup.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ProvidersInitialSetup" (int status)
        }

    ///<summary>
    ///List resources in the Resource Catalog (Closed Beta).
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="providerId"></param>
    ///<param name="resourceType"></param>
    ///<param name="resourceId"></param>
    ///<param name="region"></param>
    ///<param name="resourceGroup"></param>
    ///<param name="managed"></param>
    ///<param name="search"></param>
    ///<param name="orderBy">One of ["id", "resource_type", "region"].</param>
    ///<param name="desc"></param>
    ///<param name="perPage"></param>
    ///<param name="page"></param>
    ///<param name="cloudflare"></param>
    ///<param name="v2"></param>
    ///<param name="cancellationToken"></param>
    member this.ResourcesCatalogList
        (
            accountId: string,
            ?providerId: string,
            ?resourceType: list<string>,
            ?resourceId: list<System.Guid>,
            ?region: string,
            ?resourceGroup: string,
            ?managed: bool,
            ?search: list<string>,
            ?orderBy: string,
            ?desc: bool,
            ?perPage: int,
            ?page: int,
            ?cloudflare: bool,
            ?v2: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if providerId.IsSome then
                      RequestPart.query ("provider_id", providerId.Value)
                  if resourceType.IsSome then
                      RequestPart.query ("resource_type", resourceType.Value)
                  if resourceId.IsSome then
                      RequestPart.query ("resource_id", resourceId.Value)
                  if region.IsSome then
                      RequestPart.query ("region", region.Value)
                  if resourceGroup.IsSome then
                      RequestPart.query ("resource_group", resourceGroup.Value)
                  if managed.IsSome then
                      RequestPart.query ("managed", managed.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if desc.IsSome then
                      RequestPart.query ("desc", desc.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if cloudflare.IsSome then
                      RequestPart.query ("cloudflare", cloudflare.Value)
                  if v2.IsSome then
                      RequestPart.query ("v2", v2.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/resources"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ResourcesCatalogList.OK((Serializer.deserialize content))
            | 400 -> return ResourcesCatalogList.BadRequest((Serializer.deserialize content))
            | 401 -> return ResourcesCatalogList.Unauthorized((Serializer.deserialize content))
            | 403 -> return ResourcesCatalogList.Forbidden((Serializer.deserialize content))
            | 404 -> return ResourcesCatalogList.NotFound((Serializer.deserialize content))
            | 500 -> return ResourcesCatalogList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ResourcesCatalogList" (int status)
        }

    ///<summary>
    ///Export resources in the Resource Catalog as a JSON file (Closed Beta).
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="providerId"></param>
    ///<param name="resourceType"></param>
    ///<param name="resourceId"></param>
    ///<param name="region"></param>
    ///<param name="resourceGroup"></param>
    ///<param name="search"></param>
    ///<param name="orderBy">One of ["id", "resource_type", "region"].</param>
    ///<param name="desc"></param>
    ///<param name="v2"></param>
    ///<param name="cancellationToken"></param>
    member this.ResourcesCatalogExport
        (
            accountId: string,
            ?providerId: string,
            ?resourceType: list<string>,
            ?resourceId: list<System.Guid>,
            ?region: string,
            ?resourceGroup: string,
            ?search: list<string>,
            ?orderBy: string,
            ?desc: bool,
            ?v2: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if providerId.IsSome then
                      RequestPart.query ("provider_id", providerId.Value)
                  if resourceType.IsSome then
                      RequestPart.query ("resource_type", resourceType.Value)
                  if resourceId.IsSome then
                      RequestPart.query ("resource_id", resourceId.Value)
                  if region.IsSome then
                      RequestPart.query ("region", region.Value)
                  if resourceGroup.IsSome then
                      RequestPart.query ("resource_group", resourceGroup.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if orderBy.IsSome then
                      RequestPart.query ("order_by", orderBy.Value)
                  if desc.IsSome then
                      RequestPart.query ("desc", desc.Value)
                  if v2.IsSome then
                      RequestPart.query ("v2", v2.Value) ]

            let! (status, _, contentBinary) =
                OpenApiHttp.getBinaryAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/resources/export"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ResourcesCatalogExport.OK(contentBinary)
            | 400 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ResourcesCatalogExport.BadRequest((Serializer.deserialize content))
            | 401 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ResourcesCatalogExport.Unauthorized((Serializer.deserialize content))
            | 403 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ResourcesCatalogExport.Forbidden((Serializer.deserialize content))
            | 404 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ResourcesCatalogExport.NotFound((Serializer.deserialize content))
            | 500 ->
                let content = Encoding.UTF8.GetString contentBinary
                return ResourcesCatalogExport.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ResourcesCatalogExport" (int status)
        }

    ///<summary>
    ///Preview Rego query result against the latest resource catalog (Closed Beta).
    ///</summary>
    member this.ResourcesCatalogPolicyPreview
        (accountId: string, body: mcn_resources_catalog_policy_preview_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/resources/policy-preview"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ResourcesCatalogPolicyPreview.OK((Serializer.deserialize content))
            | 400 -> return ResourcesCatalogPolicyPreview.BadRequest((Serializer.deserialize content))
            | 401 -> return ResourcesCatalogPolicyPreview.Unauthorized((Serializer.deserialize content))
            | 403 -> return ResourcesCatalogPolicyPreview.Forbidden((Serializer.deserialize content))
            | 422 -> return ResourcesCatalogPolicyPreview.UnprocessableEntity((Serializer.deserialize content))
            | 500 -> return ResourcesCatalogPolicyPreview.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ResourcesCatalogPolicyPreview" (int status)
        }

    ///<summary>
    ///Read an resource from the Resource Catalog (Closed Beta).
    ///</summary>
    member this.ResourcesCatalogRead
        (accountId: string, resourceId: System.Guid, ?v2: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("resource_id", resourceId)
                  if v2.IsSome then
                      RequestPart.query ("v2", v2.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/cloud/resources/{resource_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ResourcesCatalogRead.OK((Serializer.deserialize content))
            | 400 -> return ResourcesCatalogRead.BadRequest((Serializer.deserialize content))
            | 401 -> return ResourcesCatalogRead.Unauthorized((Serializer.deserialize content))
            | 403 -> return ResourcesCatalogRead.Forbidden((Serializer.deserialize content))
            | 404 -> return ResourcesCatalogRead.NotFound((Serializer.deserialize content))
            | 500 -> return ResourcesCatalogRead.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ResourcesCatalogRead" (int status)
        }

    ///<summary>
    ///Lists Magic WAN Connectors.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="deviceType">Filter connectors by device type.</param>
    ///<param name="cancellationToken"></param>
    member this.MconnConnectorsList(accountId: string, ?deviceType: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if deviceType.IsSome then
                      RequestPart.query ("device_type", deviceType.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/magic/connectors" requestParts cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorsList.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorsList.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorsList.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorsList.Forbidden((Serializer.deserialize content))
            | 500 -> return MconnConnectorsList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorsList" (int status)
        }

    ///<summary>
    ///Creates a Magic WAN Connector.
    ///</summary>
    member this.MconnConnectorsCreate
        (accountId: string, body: mconn_customer_connectors_create_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorsCreate.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorsCreate.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorsCreate.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorsCreate.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorsCreate.NotFound((Serializer.deserialize content))
            | 409 -> return MconnConnectorsCreate.Conflict((Serializer.deserialize content))
            | 500 -> return MconnConnectorsCreate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorsCreate" (int status)
        }

    ///<summary>
    ///Deletes a Magic WAN Connector.
    ///</summary>
    member this.MconnConnectorsDelete(accountId: string, connectorId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorsDelete.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorsDelete.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorsDelete.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorsDelete.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorsDelete.NotFound((Serializer.deserialize content))
            | 500 -> return MconnConnectorsDelete.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorsDelete" (int status)
        }

    ///<summary>
    ///Gets a Magic WAN Connector.
    ///</summary>
    member this.MconnConnectorsGet(accountId: string, connectorId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorsGet.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorsGet.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorsGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorsGet.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorsGet.NotFound((Serializer.deserialize content))
            | 500 -> return MconnConnectorsGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorsGet" (int status)
        }

    ///<summary>
    ///Edits properties of a Magic WAN Connector. May be used to re-provision a license key.
    ///</summary>
    member this.MconnConnectorsEdit
        (
            accountId: string,
            connectorId: string,
            body: mconn_customer_connectors_edit_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorsEdit.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorsEdit.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorsEdit.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorsEdit.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorsEdit.NotFound((Serializer.deserialize content))
            | 500 -> return MconnConnectorsEdit.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorsEdit" (int status)
        }

    ///<summary>
    ///Updates properties of a Magic WAN Connector. May be used to re-provision a license key.
    ///</summary>
    member this.MconnConnectorsUpdate
        (
            accountId: string,
            connectorId: string,
            body: mconn_customer_connectors_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorsUpdate.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorsUpdate.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorsUpdate.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorsUpdate.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorsUpdate.NotFound((Serializer.deserialize content))
            | 500 -> return MconnConnectorsUpdate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorsUpdate" (int status)
        }

    ///<summary>
    ///Lists interrupts for a Magic WAN Connector.
    ///</summary>
    member this.MconnConnectorInterruptsList
        (accountId: string, connectorId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}/interrupts"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorInterruptsList.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorInterruptsList.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorInterruptsList.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorInterruptsList.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorInterruptsList.NotFound((Serializer.deserialize content))
            | 500 -> return MconnConnectorInterruptsList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorInterruptsList" (int status)
        }

    ///<summary>
    ///Creates an interrupt for a Magic WAN Connector.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="connectorId"></param>
    ///<param name="body">Interrupt action for a connector.</param>
    ///<param name="cancellationToken"></param>
    member this.MconnConnectorInterruptsCreate
        (accountId: string, connectorId: string, body: mconn_interrupt, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}/interrupts"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorInterruptsCreate.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorInterruptsCreate.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorInterruptsCreate.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorInterruptsCreate.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorInterruptsCreate.NotFound((Serializer.deserialize content))
            | 409 -> return MconnConnectorInterruptsCreate.Conflict((Serializer.deserialize content))
            | 500 -> return MconnConnectorInterruptsCreate.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorInterruptsCreate" (int status)
        }

    ///<summary>
    ///Lists Magic WAN Connector Telemetry Events
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="connectorId"></param>
    ///<param name="from"></param>
    ///<param name="to"></param>
    ///<param name="limit"></param>
    ///<param name="cursor"></param>
    ///<param name="k">Filter by event kind</param>
    ///<param name="cancellationToken"></param>
    member this.MconnConnectorTelemetryEventsList
        (
            accountId: string,
            connectorId: string,
            from: float,
            ``to``: float,
            ?limit: float,
            ?cursor: string,
            ?k: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId)
                  RequestPart.query ("from", from)
                  RequestPart.query ("to", ``to``)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value)
                  if k.IsSome then
                      RequestPart.query ("k", k.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}/telemetry/events"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorTelemetryEventsList.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorTelemetryEventsList.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorTelemetryEventsList.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorTelemetryEventsList.Forbidden((Serializer.deserialize content))
            | 429 -> return MconnConnectorTelemetryEventsList.TooManyRequests((Serializer.deserialize content))
            | 500 -> return MconnConnectorTelemetryEventsList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorTelemetryEventsList" (int status)
        }

    ///<summary>
    ///Gets latest Magic WAN Connector Telemetry Events
    ///</summary>
    member this.MconnConnectorTelemetryEventsLatestGet
        (accountId: string, connectorId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}/telemetry/events/latest"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorTelemetryEventsLatestGet.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorTelemetryEventsLatestGet.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorTelemetryEventsLatestGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorTelemetryEventsLatestGet.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorTelemetryEventsLatestGet.NotFound((Serializer.deserialize content))
            | 429 -> return MconnConnectorTelemetryEventsLatestGet.TooManyRequests((Serializer.deserialize content))
            | 500 -> return MconnConnectorTelemetryEventsLatestGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorTelemetryEventsLatestGet" (int status)
        }

    ///<summary>
    ///Gets Magic WAN Connector Telemetry Event
    ///</summary>
    member this.MconnConnectorTelemetryEventsGet
        (accountId: string, connectorId: string, eventT: float, eventN: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId)
                  RequestPart.path ("event_t", eventT)
                  RequestPart.path ("event_n", eventN) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}/telemetry/events/{event_t}.{event_n}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorTelemetryEventsGet.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorTelemetryEventsGet.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorTelemetryEventsGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorTelemetryEventsGet.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorTelemetryEventsGet.NotFound((Serializer.deserialize content))
            | 429 -> return MconnConnectorTelemetryEventsGet.TooManyRequests((Serializer.deserialize content))
            | 500 -> return MconnConnectorTelemetryEventsGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorTelemetryEventsGet" (int status)
        }

    ///<summary>
    ///Lists Magic WAN Connector Telemetry Snapshots
    ///</summary>
    member this.MconnConnectorTelemetrySnapshotsList
        (
            accountId: string,
            connectorId: string,
            from: float,
            ``to``: float,
            ?limit: float,
            ?cursor: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId)
                  RequestPart.query ("from", from)
                  RequestPart.query ("to", ``to``)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if cursor.IsSome then
                      RequestPart.query ("cursor", cursor.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}/telemetry/snapshots"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorTelemetrySnapshotsList.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorTelemetrySnapshotsList.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorTelemetrySnapshotsList.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorTelemetrySnapshotsList.Forbidden((Serializer.deserialize content))
            | 429 -> return MconnConnectorTelemetrySnapshotsList.TooManyRequests((Serializer.deserialize content))
            | 500 -> return MconnConnectorTelemetrySnapshotsList.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorTelemetrySnapshotsList" (int status)
        }

    ///<summary>
    ///Gets latest Magic WAN Connector Telemetry Snapshots
    ///</summary>
    member this.MconnConnectorTelemetrySnapshotsLatestGet
        (accountId: string, connectorId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}/telemetry/snapshots/latest"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorTelemetrySnapshotsLatestGet.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorTelemetrySnapshotsLatestGet.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorTelemetrySnapshotsLatestGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorTelemetrySnapshotsLatestGet.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorTelemetrySnapshotsLatestGet.NotFound((Serializer.deserialize content))
            | 429 -> return MconnConnectorTelemetrySnapshotsLatestGet.TooManyRequests((Serializer.deserialize content))
            | 500 ->
                return MconnConnectorTelemetrySnapshotsLatestGet.InternalServerError((Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MconnConnectorTelemetrySnapshotsLatestGet" (int status)
        }

    ///<summary>
    ///Gets Magic WAN Connector Telemetry Snapshot
    ///</summary>
    member this.MconnConnectorTelemetrySnapshotsGet
        (accountId: string, connectorId: string, snapshotT: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("connector_id", connectorId)
                  RequestPart.path ("snapshot_t", snapshotT) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/connectors/{connector_id}/telemetry/snapshots/{snapshot_t}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MconnConnectorTelemetrySnapshotsGet.OK((Serializer.deserialize content))
            | 400 -> return MconnConnectorTelemetrySnapshotsGet.BadRequest((Serializer.deserialize content))
            | 401 -> return MconnConnectorTelemetrySnapshotsGet.Unauthorized((Serializer.deserialize content))
            | 403 -> return MconnConnectorTelemetrySnapshotsGet.Forbidden((Serializer.deserialize content))
            | 404 -> return MconnConnectorTelemetrySnapshotsGet.NotFound((Serializer.deserialize content))
            | 429 -> return MconnConnectorTelemetrySnapshotsGet.TooManyRequests((Serializer.deserialize content))
            | 500 -> return MconnConnectorTelemetrySnapshotsGet.InternalServerError((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MconnConnectorTelemetrySnapshotsGet" (int status)
        }

    ///<summary>
    ///Lists GRE tunnels associated with an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the response body will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicGreTunnelsListGreTunnels
        (accountId: string, ?xMagicNewHcTarget: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/gre_tunnels"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicGreTunnelsListGreTunnels.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicGreTunnelsListGreTunnels.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicGreTunnelsListGreTunnels" (int status)
        }

    ///<summary>
    ///Creates a new GRE tunnel. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the request and response bodies will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicGreTunnelsCreateGreTunnels
        (
            accountId: string,
            body: magic_create_gre_tunnel_request,
            ?xMagicNewHcTarget: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/gre_tunnels"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicGreTunnelsCreateGreTunnels.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicGreTunnelsCreateGreTunnels.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicGreTunnelsCreateGreTunnels" (int status)
        }

    ///<summary>
    ///Updates multiple GRE tunnels. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the request and response bodies will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicGreTunnelsUpdateMultipleGreTunnels
        (
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?xMagicNewHcTarget: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/gre_tunnels"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicGreTunnelsUpdateMultipleGreTunnels.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicGreTunnelsUpdateMultipleGreTunnels.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicGreTunnelsUpdateMultipleGreTunnels" (int status)
        }

    ///<summary>
    ///Disables and removes a specific static GRE tunnel. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes.
    ///</summary>
    ///<param name="greTunnelId"></param>
    ///<param name="accountId"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the response body will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicGreTunnelsDeleteGreTunnel
        (greTunnelId: string, accountId: string, ?xMagicNewHcTarget: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("gre_tunnel_id", greTunnelId)
                  RequestPart.path ("account_id", accountId)
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/gre_tunnels/{gre_tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicGreTunnelsDeleteGreTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicGreTunnelsDeleteGreTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicGreTunnelsDeleteGreTunnel" (int status)
        }

    ///<summary>
    ///Lists informtion for a specific GRE tunnel.
    ///</summary>
    ///<param name="greTunnelId"></param>
    ///<param name="accountId"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the response body will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicGreTunnelsListGreTunnelDetails
        (greTunnelId: string, accountId: string, ?xMagicNewHcTarget: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("gre_tunnel_id", greTunnelId)
                  RequestPart.path ("account_id", accountId)
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/gre_tunnels/{gre_tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicGreTunnelsListGreTunnelDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicGreTunnelsListGreTunnelDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicGreTunnelsListGreTunnelDetails" (int status)
        }

    ///<summary>
    ///Updates a specific GRE tunnel. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes.
    ///</summary>
    ///<param name="greTunnelId"></param>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the request and response bodies will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicGreTunnelsUpdateGreTunnel
        (
            greTunnelId: string,
            accountId: string,
            body: magic_gre_tunnel_update_request,
            ?xMagicNewHcTarget: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("gre_tunnel_id", greTunnelId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/gre_tunnels/{gre_tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicGreTunnelsUpdateGreTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicGreTunnelsUpdateGreTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicGreTunnelsUpdateGreTunnel" (int status)
        }

    ///<summary>
    ///Lists IPsec tunnels associated with an account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the response body will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicIpsecTunnelsListIpsecTunnels
        (accountId: string, ?xMagicNewHcTarget: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/ipsec_tunnels"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicIpsecTunnelsListIpsecTunnels.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicIpsecTunnelsListIpsecTunnels.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicIpsecTunnelsListIpsecTunnels" (int status)
        }

    ///<summary>
    ///Creates a new IPsec tunnel associated with an account. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the request and response bodies will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicIpsecTunnelsCreateIpsecTunnels
        (
            accountId: string,
            body: magic_ipsec_tunnel_add_request,
            ?xMagicNewHcTarget: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/ipsec_tunnels"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicIpsecTunnelsCreateIpsecTunnels.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicIpsecTunnelsCreateIpsecTunnels.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicIpsecTunnelsCreateIpsecTunnels" (int status)
        }

    ///<summary>
    ///Update multiple IPsec tunnels associated with an account. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the request and response bodies will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicIpsecTunnelsUpdateMultipleIpsecTunnels
        (
            accountId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?xMagicNewHcTarget: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/ipsec_tunnels"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicIpsecTunnelsUpdateMultipleIpsecTunnels.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicIpsecTunnelsUpdateMultipleIpsecTunnels.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for MagicIpsecTunnelsUpdateMultipleIpsecTunnels" (int status)
        }

    ///<summary>
    ///Sets Pre-Shared Keys for multiple IPsec tunnels associated with an account. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes. After PSKs are applied, they are immediately persisted to Cloudflare's edge and cannot be retrieved later. Store the PSKs in a safe place.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="body">Request body for setting PSKs for multiple IPsec tunnels.</param>
    ///<param name="validateOnly">If `true`, only run validation without persisting changes.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicIpsecTunnelsSetPreSharedKeysForIpsecTunnels
        (
            accountId: string,
            body: magic_ipsec_tunnels_psk_request,
            ?validateOnly: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if validateOnly.IsSome then
                      RequestPart.query ("validate_only", validateOnly.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/ipsec_tunnels/psk"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicIpsecTunnelsSetPreSharedKeysForIpsecTunnels.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicIpsecTunnelsSetPreSharedKeysForIpsecTunnels.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for MagicIpsecTunnelsSetPreSharedKeysForIpsecTunnels"
                        (int status)
        }

    ///<summary>
    ///Disables and removes a specific static IPsec Tunnel associated with an account. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes.
    ///</summary>
    ///<param name="ipsecTunnelId"></param>
    ///<param name="accountId"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the response body will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicIpsecTunnelsDeleteIpsecTunnel
        (ipsecTunnelId: string, accountId: string, ?xMagicNewHcTarget: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("ipsec_tunnel_id", ipsecTunnelId)
                  RequestPart.path ("account_id", accountId)
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/ipsec_tunnels/{ipsec_tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicIpsecTunnelsDeleteIpsecTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicIpsecTunnelsDeleteIpsecTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicIpsecTunnelsDeleteIpsecTunnel" (int status)
        }

    ///<summary>
    ///Lists details for a specific IPsec tunnel.
    ///</summary>
    ///<param name="ipsecTunnelId"></param>
    ///<param name="accountId"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the response body will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicIpsecTunnelsListIpsecTunnelDetails
        (ipsecTunnelId: string, accountId: string, ?xMagicNewHcTarget: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("ipsec_tunnel_id", ipsecTunnelId)
                  RequestPart.path ("account_id", accountId)
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/ipsec_tunnels/{ipsec_tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicIpsecTunnelsListIpsecTunnelDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicIpsecTunnelsListIpsecTunnelDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicIpsecTunnelsListIpsecTunnelDetails" (int status)
        }

    ///<summary>
    ///Updates a specific IPsec tunnel associated with an account. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes.
    ///</summary>
    ///<param name="ipsecTunnelId"></param>
    ///<param name="accountId"></param>
    ///<param name="body"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the request and response bodies will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicIpsecTunnelsUpdateIpsecTunnel
        (
            ipsecTunnelId: string,
            accountId: string,
            body: magic_ipsec_tunnel_add_single_request,
            ?xMagicNewHcTarget: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("ipsec_tunnel_id", ipsecTunnelId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/ipsec_tunnels/{ipsec_tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicIpsecTunnelsUpdateIpsecTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicIpsecTunnelsUpdateIpsecTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicIpsecTunnelsUpdateIpsecTunnel" (int status)
        }

    ///<summary>
    ///Generates a Pre-Shared Key for a specific IPsec tunnel used in the IKE session. Use `?validate_only=true` as an optional query parameter to only run validation without persisting changes. After a PSK is generated, the PSK is immediately persisted to Cloudflare's edge and cannot be retrieved later. Store the PSK in a safe place.
    ///</summary>
    member this.``MagicIpsecTunnelsGeneratePreSharedKey(Psk)ForIpsecTunnels``
        (ipsecTunnelId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("ipsec_tunnel_id", ipsecTunnelId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/ipsec_tunnels/{ipsec_tunnel_id}/psk_generate"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    ``MagicIpsecTunnelsGeneratePreSharedKey(Psk)ForIpsecTunnels``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``MagicIpsecTunnelsGeneratePreSharedKey(Psk)ForIpsecTunnels``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for MagicIpsecTunnelsGeneratePreSharedKey(Psk)ForIpsecTunnels"
                        (int status)
        }

    ///<summary>
    ///Lists redundancy groups associated with an account, including full member tunnel data.
    ///</summary>
    member this.MagicRedundancyGroupsListRedundancyGroups(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/redundancy_groups"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicRedundancyGroupsListRedundancyGroups.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicRedundancyGroupsListRedundancyGroups.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicRedundancyGroupsListRedundancyGroups" (int status)
        }

    ///<summary>
    ///Creates a new redundancy group, optionally with tunnel members.
    ///</summary>
    member this.MagicRedundancyGroupsCreateRedundancyGroup
        (accountId: string, body: magic_create_redundancy_group_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/redundancy_groups"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return MagicRedundancyGroupsCreateRedundancyGroup.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicRedundancyGroupsCreateRedundancyGroup.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicRedundancyGroupsCreateRedundancyGroup" (int status)
        }

    ///<summary>
    ///Deletes a redundancy group. Member tunnels are not deleted — their redundancy_group_id is cleared.
    ///</summary>
    member this.MagicRedundancyGroupsDeleteRedundancyGroup
        (accountId: string, redundancyGroupId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("redundancy_group_id", redundancyGroupId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/redundancy_groups/{redundancy_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicRedundancyGroupsDeleteRedundancyGroup.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicRedundancyGroupsDeleteRedundancyGroup.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicRedundancyGroupsDeleteRedundancyGroup" (int status)
        }

    ///<summary>
    ///Gets details for a specific redundancy group, including full member tunnel data.
    ///</summary>
    member this.MagicRedundancyGroupsGetRedundancyGroup
        (accountId: string, redundancyGroupId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("redundancy_group_id", redundancyGroupId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/redundancy_groups/{redundancy_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicRedundancyGroupsGetRedundancyGroup.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicRedundancyGroupsGetRedundancyGroup.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicRedundancyGroupsGetRedundancyGroup" (int status)
        }

    ///<summary>
    ///Replaces the name, description, and full set of members for an existing redundancy group.
    ///</summary>
    member this.MagicRedundancyGroupsUpdateRedundancyGroup
        (
            accountId: string,
            redundancyGroupId: string,
            body: magic_create_redundancy_group_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("redundancy_group_id", redundancyGroupId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/redundancy_groups/{redundancy_group_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicRedundancyGroupsUpdateRedundancyGroup.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicRedundancyGroupsUpdateRedundancyGroup.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicRedundancyGroupsUpdateRedundancyGroup" (int status)
        }

    ///<summary>
    ///Delete multiple Magic static routes.
    ///</summary>
    member this.MagicStaticRoutesDeleteManyRoutes
        (accountId: string, body: magic_route_delete_many_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/accounts/{account_id}/magic/routes" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicStaticRoutesDeleteManyRoutes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicStaticRoutesDeleteManyRoutes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicStaticRoutesDeleteManyRoutes" (int status)
        }

    ///<summary>
    ///List all Magic static routes.
    ///</summary>
    member this.MagicStaticRoutesListRoutes(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/magic/routes" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicStaticRoutesListRoutes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicStaticRoutesListRoutes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicStaticRoutesListRoutes" (int status)
        }

    ///<summary>
    ///Creates a new Magic static route. Use `?validate_only=true` as an optional query parameter to run validation only without persisting changes.
    ///</summary>
    member this.MagicStaticRoutesCreateRoutes
        (accountId: string, body: magic_create_route_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/magic/routes" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicStaticRoutesCreateRoutes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicStaticRoutesCreateRoutes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicStaticRoutesCreateRoutes" (int status)
        }

    ///<summary>
    ///Update multiple Magic static routes. Use `?validate_only=true` as an optional query parameter to run validation only without persisting changes. Only fields for a route that need to be changed need be provided.
    ///</summary>
    member this.MagicStaticRoutesUpdateManyRoutes
        (accountId: string, body: magic_route_update_many_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/magic/routes" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicStaticRoutesUpdateManyRoutes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicStaticRoutesUpdateManyRoutes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicStaticRoutesUpdateManyRoutes" (int status)
        }

    ///<summary>
    ///Disable and remove a specific Magic static route.
    ///</summary>
    member this.MagicStaticRoutesDeleteRoute
        (routeId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("route_id", routeId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/routes/{route_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicStaticRoutesDeleteRoute.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicStaticRoutesDeleteRoute.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicStaticRoutesDeleteRoute" (int status)
        }

    ///<summary>
    ///Get a specific Magic static route.
    ///</summary>
    member this.MagicStaticRoutesRouteDetails
        (routeId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("route_id", routeId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/routes/{route_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicStaticRoutesRouteDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicStaticRoutesRouteDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicStaticRoutesRouteDetails" (int status)
        }

    ///<summary>
    ///Update a specific Magic static route. Use `?validate_only=true` as an optional query parameter to run validation only without persisting changes.
    ///</summary>
    member this.MagicStaticRoutesUpdateRoute
        (routeId: string, accountId: string, body: magic_route_update_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("route_id", routeId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/routes/{route_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicStaticRoutesUpdateRoute.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicStaticRoutesUpdateRoute.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicStaticRoutesUpdateRoute" (int status)
        }

    ///<summary>
    ///Lists Sites associated with an account. Use connectorid query param to return sites where connectorid matches either site.ConnectorID or site.SecondaryConnectorID.
    ///</summary>
    member this.MagicSitesListSites(accountId: string, ?connectorid: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if connectorid.IsSome then
                      RequestPart.query ("connectorid", connectorid.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/magic/sites" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicSitesListSites.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSitesListSites.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSitesListSites" (int status)
        }

    ///<summary>
    ///Creates a new Site
    ///</summary>
    member this.MagicSitesCreateSite
        (accountId: string, body: magic_sites_add_single_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/magic/sites" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicSitesCreateSite.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSitesCreateSite.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSitesCreateSite" (int status)
        }

    ///<summary>
    ///Remove a specific Site.
    ///</summary>
    member this.MagicSitesDeleteSite(siteId: string, accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSitesDeleteSite.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSitesDeleteSite.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSitesDeleteSite" (int status)
        }

    ///<summary>
    ///Get a specific Site.
    ///</summary>
    ///<param name="siteId"></param>
    ///<param name="accountId"></param>
    ///<param name="xMagicNewHcTarget">If true, the health check target in the response body will be presented using the new object format. Defaults to false.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicSitesSiteDetails
        (siteId: string, accountId: string, ?xMagicNewHcTarget: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  if xMagicNewHcTarget.IsSome then
                      RequestPart.header ("x-magic-new-hc-target", xMagicNewHcTarget.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSitesSiteDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSitesSiteDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSitesSiteDetails" (int status)
        }

    ///<summary>
    ///Patch a specific Site.
    ///</summary>
    member this.MagicSitesPatchSite
        (siteId: string, accountId: string, body: magic_site_update_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSitesPatchSite.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSitesPatchSite.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSitesPatchSite" (int status)
        }

    ///<summary>
    ///Update a specific Site.
    ///</summary>
    member this.MagicSitesUpdateSite
        (siteId: string, accountId: string, body: magic_site_update_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSitesUpdateSite.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSitesUpdateSite.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSitesUpdateSite" (int status)
        }

    ///<summary>
    ///Lists Site ACLs associated with an account.
    ///</summary>
    member this.MagicSiteAclsListAcls(accountId: string, siteId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/acls"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteAclsListAcls.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAclsListAcls.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAclsListAcls" (int status)
        }

    ///<summary>
    ///Creates a new Site ACL.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="siteId"></param>
    ///<param name="body">Bidirectional ACL policy for local network traffic within a site.</param>
    ///<param name="cancellationToken"></param>
    member this.MagicSiteAclsCreateAcl
        (accountId: string, siteId: string, body: magic_acls_add_single_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/acls"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteAclsCreateAcl.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAclsCreateAcl.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAclsCreateAcl" (int status)
        }

    ///<summary>
    ///Remove a specific Site ACL.
    ///</summary>
    member this.MagicSiteAclsDeleteAcl
        (siteId: string, accountId: string, aclId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("acl_id", aclId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/acls/{acl_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteAclsDeleteAcl.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAclsDeleteAcl.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAclsDeleteAcl" (int status)
        }

    ///<summary>
    ///Get a specific Site ACL.
    ///</summary>
    member this.MagicSiteAclsAclDetails
        (siteId: string, accountId: string, aclId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("acl_id", aclId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/acls/{acl_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteAclsAclDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAclsAclDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAclsAclDetails" (int status)
        }

    ///<summary>
    ///Patch a specific Site ACL.
    ///</summary>
    member this.MagicSiteAclsPatchAcl
        (
            siteId: string,
            accountId: string,
            aclId: string,
            body: magic_acl_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("acl_id", aclId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/acls/{acl_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteAclsPatchAcl.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAclsPatchAcl.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAclsPatchAcl" (int status)
        }

    ///<summary>
    ///Update a specific Site ACL.
    ///</summary>
    member this.MagicSiteAclsUpdateAcl
        (
            siteId: string,
            accountId: string,
            aclId: string,
            body: magic_acl_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("acl_id", aclId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/acls/{acl_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteAclsUpdateAcl.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAclsUpdateAcl.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAclsUpdateAcl" (int status)
        }

    ///<summary>
    ///Lists App Configs associated with a site.
    ///</summary>
    member this.MagicSiteAppConfigsListAppConfigs
        (accountId: string, siteId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/app_configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteAppConfigsListAppConfigs.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAppConfigsListAppConfigs.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAppConfigsListAppConfigs" (int status)
        }

    ///<summary>
    ///Creates a new App Config for a site
    ///</summary>
    member this.MagicSiteAppConfigsAddAppConfig
        (
            accountId: string,
            siteId: string,
            body: magic_app_config_add_single_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/app_configs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return MagicSiteAppConfigsAddAppConfig.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAppConfigsAddAppConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAppConfigsAddAppConfig" (int status)
        }

    ///<summary>
    ///Deletes specific App Config associated with a site.
    ///</summary>
    member this.MagicSiteAppConfigsDeleteAppConfig
        (accountId: string, siteId: string, appConfigId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId)
                  RequestPart.path ("app_config_id", appConfigId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/app_configs/{app_config_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteAppConfigsDeleteAppConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAppConfigsDeleteAppConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAppConfigsDeleteAppConfig" (int status)
        }

    ///<summary>
    ///Updates an App Config for a site
    ///</summary>
    member this.MagicSiteAppConfigsPatchAppConfig
        (
            accountId: string,
            siteId: string,
            appConfigId: string,
            body: magic_app_config_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId)
                  RequestPart.path ("app_config_id", appConfigId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/app_configs/{app_config_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteAppConfigsPatchAppConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAppConfigsPatchAppConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAppConfigsPatchAppConfig" (int status)
        }

    ///<summary>
    ///Updates an App Config for a site
    ///</summary>
    member this.MagicSiteAppConfigsUpdateAppConfig
        (
            accountId: string,
            siteId: string,
            appConfigId: string,
            body: magic_app_config_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId)
                  RequestPart.path ("app_config_id", appConfigId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/app_configs/{app_config_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteAppConfigsUpdateAppConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteAppConfigsUpdateAppConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteAppConfigsUpdateAppConfig" (int status)
        }

    ///<summary>
    ///Lists Site LANs associated with an account.
    ///</summary>
    member this.MagicSiteLansListLans(accountId: string, siteId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/lans"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteLansListLans.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteLansListLans.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteLansListLans" (int status)
        }

    ///<summary>
    ///Creates a new Site LAN. If the site is in high availability mode, static_addressing is required along with secondary and virtual address.
    ///</summary>
    member this.MagicSiteLansCreateLan
        (accountId: string, siteId: string, body: magic_lans_add_single_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/lans"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteLansCreateLan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteLansCreateLan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteLansCreateLan" (int status)
        }

    ///<summary>
    ///Remove a specific Site LAN.
    ///</summary>
    member this.MagicSiteLansDeleteLan
        (siteId: string, accountId: string, lanId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("lan_id", lanId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/lans/{lan_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteLansDeleteLan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteLansDeleteLan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteLansDeleteLan" (int status)
        }

    ///<summary>
    ///Get a specific Site LAN.
    ///</summary>
    member this.MagicSiteLansLanDetails
        (siteId: string, accountId: string, lanId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("lan_id", lanId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/lans/{lan_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteLansLanDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteLansLanDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteLansLanDetails" (int status)
        }

    ///<summary>
    ///Patch a specific Site LAN.
    ///</summary>
    member this.MagicSiteLansPatchLan
        (
            siteId: string,
            accountId: string,
            lanId: string,
            body: magic_lan_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("lan_id", lanId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/lans/{lan_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteLansPatchLan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteLansPatchLan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteLansPatchLan" (int status)
        }

    ///<summary>
    ///Update a specific Site LAN.
    ///</summary>
    member this.MagicSiteLansUpdateLan
        (
            siteId: string,
            accountId: string,
            lanId: string,
            body: magic_lan_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("lan_id", lanId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/lans/{lan_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteLansUpdateLan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteLansUpdateLan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteLansUpdateLan" (int status)
        }

    ///<summary>
    ///Remove NetFlow configuration for a site.
    ///</summary>
    member this.MagicSiteNetflowConfigDeleteNetflowConfig
        (accountId: string, siteId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/netflow_config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteNetflowConfigDeleteNetflowConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteNetflowConfigDeleteNetflowConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicSiteNetflowConfigDeleteNetflowConfig" (int status)
        }

    ///<summary>
    ///Get NetFlow configuration for a site.
    ///</summary>
    member this.MagicSiteNetflowConfigDetails
        (accountId: string, siteId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/netflow_config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteNetflowConfigDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteNetflowConfigDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteNetflowConfigDetails" (int status)
        }

    ///<summary>
    ///Updates NetFlow configuration for a site.
    ///</summary>
    member this.MagicSiteNetflowConfigPatchNetflowConfig
        (accountId: string, siteId: string, body: magic_netflow_config_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/netflow_config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteNetflowConfigPatchNetflowConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteNetflowConfigPatchNetflowConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicSiteNetflowConfigPatchNetflowConfig" (int status)
        }

    ///<summary>
    ///Creates a NetFlow configuration for a site.
    ///</summary>
    member this.MagicSiteNetflowConfigCreateNetflowConfig
        (accountId: string, siteId: string, body: magic_netflow_config_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/netflow_config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 201 -> return MagicSiteNetflowConfigCreateNetflowConfig.Created((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteNetflowConfigCreateNetflowConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicSiteNetflowConfigCreateNetflowConfig" (int status)
        }

    ///<summary>
    ///Updates NetFlow configuration for a site (partial update).
    ///</summary>
    member this.MagicSiteNetflowConfigUpdateNetflowConfig
        (accountId: string, siteId: string, body: magic_netflow_config_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/netflow_config"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteNetflowConfigUpdateNetflowConfig.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteNetflowConfigUpdateNetflowConfig.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicSiteNetflowConfigUpdateNetflowConfig" (int status)
        }

    ///<summary>
    ///Lists Site WANs associated with an account.
    ///</summary>
    member this.MagicSiteWansListWans(accountId: string, siteId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/wans"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteWansListWans.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteWansListWans.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteWansListWans" (int status)
        }

    ///<summary>
    ///Creates a new Site WAN.
    ///</summary>
    member this.MagicSiteWansCreateWan
        (accountId: string, siteId: string, body: magic_wans_add_single_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("site_id", siteId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/wans"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteWansCreateWan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteWansCreateWan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteWansCreateWan" (int status)
        }

    ///<summary>
    ///Remove a specific Site WAN.
    ///</summary>
    member this.MagicSiteWansDeleteWan
        (siteId: string, accountId: string, wanId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("wan_id", wanId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/wans/{wan_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteWansDeleteWan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteWansDeleteWan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteWansDeleteWan" (int status)
        }

    ///<summary>
    ///Get a specific Site WAN.
    ///</summary>
    member this.MagicSiteWansWanDetails
        (siteId: string, accountId: string, wanId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("wan_id", wanId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/wans/{wan_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteWansWanDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteWansWanDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteWansWanDetails" (int status)
        }

    ///<summary>
    ///Patch a specific Site WAN.
    ///</summary>
    member this.MagicSiteWansPatchWan
        (
            siteId: string,
            accountId: string,
            wanId: string,
            body: magic_wan_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("wan_id", wanId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/wans/{wan_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteWansPatchWan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteWansPatchWan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteWansPatchWan" (int status)
        }

    ///<summary>
    ///Update a specific Site WAN.
    ///</summary>
    member this.MagicSiteWansUpdateWan
        (
            siteId: string,
            accountId: string,
            wanId: string,
            body: magic_wan_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("site_id", siteId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.path ("wan_id", wanId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/magic/sites/{site_id}/wans/{wan_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicSiteWansUpdateWan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicSiteWansUpdateWan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicSiteWansUpdateWan" (int status)
        }

    ///<summary>
    ///Delete an existing network monitoring configuration.
    ///</summary>
    member this.MagicNetworkMonitoringConfigurationDeleteAccountConfiguration
        (accountId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/accounts/{account_id}/mnm/config" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    MagicNetworkMonitoringConfigurationDeleteAccountConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicNetworkMonitoringConfigurationDeleteAccountConfiguration.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for MagicNetworkMonitoringConfigurationDeleteAccountConfiguration"
                        (int status)
        }

    ///<summary>
    ///Lists default sampling, router IPs and warp devices for account.
    ///</summary>
    member this.MagicNetworkMonitoringConfigurationListAccountConfiguration
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/mnm/config" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return MagicNetworkMonitoringConfigurationListAccountConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicNetworkMonitoringConfigurationListAccountConfiguration.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for MagicNetworkMonitoringConfigurationListAccountConfiguration"
                        (int status)
        }

    ///<summary>
    ///Update fields in an existing network monitoring configuration.
    ///</summary>
    member this.MagicNetworkMonitoringConfigurationUpdateAccountConfigurationFields
        (
            accountId: string,
            body: MagicNetworkMonitoringConfigurationUpdateAccountConfigurationFieldsPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/accounts/{account_id}/mnm/config" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    MagicNetworkMonitoringConfigurationUpdateAccountConfigurationFields.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicNetworkMonitoringConfigurationUpdateAccountConfigurationFields.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for MagicNetworkMonitoringConfigurationUpdateAccountConfigurationFields"
                        (int status)
        }

    ///<summary>
    ///Create a new network monitoring configuration.
    ///</summary>
    member this.MagicNetworkMonitoringConfigurationCreateAccountConfiguration
        (
            accountId: string,
            body: MagicNetworkMonitoringConfigurationCreateAccountConfigurationPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/mnm/config" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    MagicNetworkMonitoringConfigurationCreateAccountConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicNetworkMonitoringConfigurationCreateAccountConfiguration.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for MagicNetworkMonitoringConfigurationCreateAccountConfiguration"
                        (int status)
        }

    ///<summary>
    ///Update an existing network monitoring configuration, requires the entire configuration to be updated at once.
    ///</summary>
    member this.MagicNetworkMonitoringConfigurationUpdateAnEntireAccountConfiguration
        (
            accountId: string,
            body: MagicNetworkMonitoringConfigurationUpdateAnEntireAccountConfigurationPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/mnm/config" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    MagicNetworkMonitoringConfigurationUpdateAnEntireAccountConfiguration.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicNetworkMonitoringConfigurationUpdateAnEntireAccountConfiguration.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for MagicNetworkMonitoringConfigurationUpdateAnEntireAccountConfiguration"
                        (int status)
        }

    ///<summary>
    ///Lists default sampling, router IPs, warp devices, and rules for account.
    ///</summary>
    member this.MagicNetworkMonitoringConfigurationListRulesAndAccountConfiguration
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/mnm/config/full" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    MagicNetworkMonitoringConfigurationListRulesAndAccountConfiguration.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicNetworkMonitoringConfigurationListRulesAndAccountConfiguration.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for MagicNetworkMonitoringConfigurationListRulesAndAccountConfiguration"
                        (int status)
        }

    ///<summary>
    ///Lists network monitoring rules for account.
    ///</summary>
    member this.MagicNetworkMonitoringRulesListRules(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/mnm/rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicNetworkMonitoringRulesListRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicNetworkMonitoringRulesListRules.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicNetworkMonitoringRulesListRules" (int status)
        }

    ///<summary>
    ///Create network monitoring rules for account. Currently only supports creating a single rule per API request.
    ///</summary>
    member this.MagicNetworkMonitoringRulesCreateRules
        (
            accountId: string,
            body: magic_u002D_visibility_u002D_mnm_mnm_rule_create,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/mnm/rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicNetworkMonitoringRulesCreateRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicNetworkMonitoringRulesCreateRules.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicNetworkMonitoringRulesCreateRules" (int status)
        }

    ///<summary>
    ///Update network monitoring rules for account.
    ///</summary>
    member this.MagicNetworkMonitoringRulesUpdateRules
        (
            accountId: string,
            body: magic_u002D_visibility_u002D_mnm_mnm_rule_create,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/mnm/rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicNetworkMonitoringRulesUpdateRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicNetworkMonitoringRulesUpdateRules.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicNetworkMonitoringRulesUpdateRules" (int status)
        }

    ///<summary>
    ///Create multiple network monitoring rules for account in a single request. Supports up to 100 rules per request. All rules in a single request must be of the same type.
    ///</summary>
    member this.MagicNetworkMonitoringRulesCreateRulesBulk
        (
            accountId: string,
            body: list<magic_u002D_visibility_u002D_mnm_mnm_rule_create>,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/mnm/rules/bulk" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicNetworkMonitoringRulesCreateRulesBulk.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicNetworkMonitoringRulesCreateRulesBulk.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicNetworkMonitoringRulesCreateRulesBulk" (int status)
        }

    ///<summary>
    ///Update multiple network monitoring rules for account in a single request. Supports up to 100 rules per request. All rules in a single request must be of the same type.
    ///</summary>
    member this.MagicNetworkMonitoringRulesUpdateRulesBulk
        (
            accountId: string,
            body: list<magic_u002D_visibility_u002D_mnm_mnm_rule_create>,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/accounts/{account_id}/mnm/rules/bulk" requestParts cancellationToken

            match (int status) with
            | 200 -> return MagicNetworkMonitoringRulesUpdateRulesBulk.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicNetworkMonitoringRulesUpdateRulesBulk.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for MagicNetworkMonitoringRulesUpdateRulesBulk" (int status)
        }

    ///<summary>
    ///Delete a network monitoring rule for account.
    ///</summary>
    member this.MagicNetworkMonitoringRulesDeleteRule
        (ruleId: string, accountId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("rule_id", ruleId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/mnm/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicNetworkMonitoringRulesDeleteRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicNetworkMonitoringRulesDeleteRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicNetworkMonitoringRulesDeleteRule" (int status)
        }

    ///<summary>
    ///List a single network monitoring rule for account.
    ///</summary>
    member this.MagicNetworkMonitoringRulesGetRule
        (ruleId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("rule_id", ruleId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/mnm/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicNetworkMonitoringRulesGetRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicNetworkMonitoringRulesGetRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicNetworkMonitoringRulesGetRule" (int status)
        }

    ///<summary>
    ///Update a network monitoring rule for account.
    ///</summary>
    member this.MagicNetworkMonitoringRulesUpdateRule
        (
            ruleId: string,
            accountId: string,
            body: magic_u002D_visibility_u002D_mnm_mnm_rule_create,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("rule_id", ruleId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/mnm/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicNetworkMonitoringRulesUpdateRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return MagicNetworkMonitoringRulesUpdateRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for MagicNetworkMonitoringRulesUpdateRule" (int status)
        }

    ///<summary>
    ///Update advertisement for rule.
    ///</summary>
    member this.MagicNetworkMonitoringRulesUpdateAdvertisementForRule
        (ruleId: string, accountId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("rule_id", ruleId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/mnm/rules/{rule_id}/advertisement"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return MagicNetworkMonitoringRulesUpdateAdvertisementForRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicNetworkMonitoringRulesUpdateAdvertisementForRule.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for MagicNetworkMonitoringRulesUpdateAdvertisementForRule"
                        (int status)
        }

    ///<summary>
    ///Generate authentication token for VPC flow logs export.
    ///</summary>
    member this.MagicNetworkMonitoringVpcFlowsGenerateAuthenticationToken
        (accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/mnm/vpc-flows/token"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return MagicNetworkMonitoringVpcFlowsGenerateAuthenticationToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    MagicNetworkMonitoringVpcFlowsGenerateAuthenticationToken.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for MagicNetworkMonitoringVpcFlowsGenerateAuthenticationToken"
                        (int status)
        }

    ///<summary>
    ///List ACLs.
    ///</summary>
    member this.``SecondaryDns(Acl)ListAcLs``(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/acls"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Acl)ListAcLs``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Acl)ListAcLs``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Acl)ListAcLs" (int status)
        }

    ///<summary>
    ///Create ACL.
    ///</summary>
    member this.``SecondaryDns(Acl)CreateAcl``
        (accountId: string, body: ``SecondaryDns(Acl)CreateAclPayload``, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/acls"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Acl)CreateAcl``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Acl)CreateAcl``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Acl)CreateAcl" (int status)
        }

    ///<summary>
    ///Delete ACL.
    ///</summary>
    member this.``SecondaryDns(Acl)DeleteAcl``
        (aclId: string, accountId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("acl_id", aclId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/acls/{acl_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Acl)DeleteAcl``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Acl)DeleteAcl``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Acl)DeleteAcl" (int status)
        }

    ///<summary>
    ///Get ACL.
    ///</summary>
    member this.``SecondaryDns(Acl)AclDetails``
        (aclId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("acl_id", aclId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/acls/{acl_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Acl)AclDetails``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Acl)AclDetails``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Acl)AclDetails" (int status)
        }

    ///<summary>
    ///Modify ACL.
    ///</summary>
    member this.``SecondaryDns(Acl)UpdateAcl``
        (aclId: string, accountId: string, body: secondary_u002D_dns_acl, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("acl_id", aclId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/acls/{acl_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Acl)UpdateAcl``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Acl)UpdateAcl``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Acl)UpdateAcl" (int status)
        }

    ///<summary>
    ///List Peers.
    ///</summary>
    member this.``SecondaryDns(Peer)ListPeers``(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/peers"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Peer)ListPeers``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Peer)ListPeers``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Peer)ListPeers" (int status)
        }

    ///<summary>
    ///Create Peer.
    ///</summary>
    member this.``SecondaryDns(Peer)CreatePeer``
        (accountId: string, body: ``SecondaryDns(Peer)CreatePeerPayload``, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/peers"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Peer)CreatePeer``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Peer)CreatePeer``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Peer)CreatePeer" (int status)
        }

    ///<summary>
    ///Delete Peer.
    ///</summary>
    member this.``SecondaryDns(Peer)DeletePeer``
        (peerId: string, accountId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("peer_id", peerId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/peers/{peer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Peer)DeletePeer``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Peer)DeletePeer``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Peer)DeletePeer" (int status)
        }

    ///<summary>
    ///Get Peer.
    ///</summary>
    member this.``SecondaryDns(Peer)PeerDetails``
        (peerId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("peer_id", peerId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/peers/{peer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Peer)PeerDetails``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Peer)PeerDetails``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Peer)PeerDetails" (int status)
        }

    ///<summary>
    ///Modify Peer.
    ///</summary>
    member this.``SecondaryDns(Peer)UpdatePeer``
        (peerId: string, accountId: string, body: secondary_u002D_dns_peer, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("peer_id", peerId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/peers/{peer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Peer)UpdatePeer``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Peer)UpdatePeer``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Peer)UpdatePeer" (int status)
        }

    ///<summary>
    ///List TSIGs.
    ///</summary>
    member this.``SecondaryDns(Tsig)ListTsiGs``(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/tsigs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Tsig)ListTsiGs``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Tsig)ListTsiGs``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Tsig)ListTsiGs" (int status)
        }

    ///<summary>
    ///Create TSIG.
    ///</summary>
    member this.``SecondaryDns(Tsig)CreateTsig``
        (accountId: string, body: secondary_u002D_dns_tsig, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/tsigs"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Tsig)CreateTsig``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Tsig)CreateTsig``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Tsig)CreateTsig" (int status)
        }

    ///<summary>
    ///Delete TSIG.
    ///</summary>
    member this.``SecondaryDns(Tsig)DeleteTsig``
        (tsigId: string, accountId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("tsig_id", tsigId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/tsigs/{tsig_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Tsig)DeleteTsig``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Tsig)DeleteTsig``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Tsig)DeleteTsig" (int status)
        }

    ///<summary>
    ///Get TSIG.
    ///</summary>
    member this.``SecondaryDns(Tsig)TsigDetails``
        (tsigId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("tsig_id", tsigId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/tsigs/{tsig_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Tsig)TsigDetails``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Tsig)TsigDetails``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Tsig)TsigDetails" (int status)
        }

    ///<summary>
    ///Modify TSIG.
    ///</summary>
    member this.``SecondaryDns(Tsig)UpdateTsig``
        (tsigId: string, accountId: string, body: secondary_u002D_dns_tsig, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("tsig_id", tsigId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/secondary_dns/tsigs/{tsig_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(Tsig)UpdateTsig``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(Tsig)UpdateTsig``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(Tsig)UpdateTsig" (int status)
        }

    ///<summary>
    ///Lists and filters private network routes in an account.
    ///</summary>
    member this.TunnelRouteListTunnelRoutes
        (
            accountId: string,
            ?comment: string,
            ?isDeleted: bool,
            ?networkSubset: string,
            ?networkSuperset: string,
            ?existedAt: string,
            ?tunnelId: System.Guid,
            ?routeId: string,
            ?tunTypes: tunnel_tunnel_types,
            ?virtualNetworkId: System.Guid,
            ?perPage: float,
            ?page: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if comment.IsSome then
                      RequestPart.query ("comment", comment.Value)
                  if isDeleted.IsSome then
                      RequestPart.query ("is_deleted", isDeleted.Value)
                  if networkSubset.IsSome then
                      RequestPart.query ("network_subset", networkSubset.Value)
                  if networkSuperset.IsSome then
                      RequestPart.query ("network_superset", networkSuperset.Value)
                  if existedAt.IsSome then
                      RequestPart.query ("existed_at", existedAt.Value)
                  if tunnelId.IsSome then
                      RequestPart.query ("tunnel_id", tunnelId.Value)
                  if routeId.IsSome then
                      RequestPart.query ("route_id", routeId.Value)
                  if tunTypes.IsSome then
                      RequestPart.query ("tun_types", tunTypes.Value)
                  if virtualNetworkId.IsSome then
                      RequestPart.query ("virtual_network_id", virtualNetworkId.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/teamnet/routes" requestParts cancellationToken

            match (int status) with
            | 200 -> return TunnelRouteListTunnelRoutes.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelRouteListTunnelRoutes.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelRouteListTunnelRoutes" (int status)
        }

    ///<summary>
    ///Routes a private network through a Cloudflare Tunnel.
    ///</summary>
    member this.TunnelRouteCreateATunnelRoute
        (accountId: string, body: TunnelRouteCreateATunnelRoutePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/teamnet/routes" requestParts cancellationToken

            match (int status) with
            | 200 -> return TunnelRouteCreateATunnelRoute.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelRouteCreateATunnelRoute.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelRouteCreateATunnelRoute" (int status)
        }

    ///<summary>
    ///Fetches routes that contain the given IP address.
    ///</summary>
    ///<param name="ip"></param>
    ///<param name="accountId"></param>
    ///<param name="virtualNetworkId"></param>
    ///<param name="defaultVirtualNetworkFallback">When the virtual_network_id parameter is not provided the request filter will default search routes that are in the default virtual network for the account. If this parameter is set to false, the search will include routes that do not have a virtual network.</param>
    ///<param name="cancellationToken"></param>
    member this.TunnelRouteGetTunnelRouteByIp
        (
            ip: string,
            accountId: string,
            ?virtualNetworkId: System.Guid,
            ?defaultVirtualNetworkFallback: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("ip", ip)
                  RequestPart.path ("account_id", accountId)
                  if virtualNetworkId.IsSome then
                      RequestPart.query ("virtual_network_id", virtualNetworkId.Value)
                  if defaultVirtualNetworkFallback.IsSome then
                      RequestPart.query ("default_virtual_network_fallback", defaultVirtualNetworkFallback.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/routes/ip/{ip}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelRouteGetTunnelRouteByIp.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelRouteGetTunnelRouteByIp.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelRouteGetTunnelRouteByIp" (int status)
        }

    ///<summary>
    ///Deletes a private network route from an account. The CIDR in `ip_network_encoded` must be written in URL-encoded format. If no virtual_network_id is provided it will delete the route from the default vnet. If no tun_type is provided it will fetch the type from the tunnel_id or if that is missing it will assume Cloudflare Tunnel as default. If tunnel_id is provided it will delete the route from that tunnel, otherwise it will delete the route based on the vnet and tun_type.
    ///</summary>
    member this.TunnelRouteDeleteATunnelRouteWithCidr
        (
            ipNetworkEncoded: string,
            accountId: string,
            ?virtualNetworkId: System.Guid,
            ?tunType: string,
            ?tunnelId: System.Guid,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("ip_network_encoded", ipNetworkEncoded)
                  RequestPart.path ("account_id", accountId)
                  if virtualNetworkId.IsSome then
                      RequestPart.query ("virtual_network_id", virtualNetworkId.Value)
                  if tunType.IsSome then
                      RequestPart.query ("tun_type", tunType.Value)
                  if tunnelId.IsSome then
                      RequestPart.query ("tunnel_id", tunnelId.Value) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/routes/network/{ip_network_encoded}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelRouteDeleteATunnelRouteWithCidr.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelRouteDeleteATunnelRouteWithCidr.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelRouteDeleteATunnelRouteWithCidr" (int status)
        }

    ///<summary>
    ///Updates an existing private network route in an account. The CIDR in `ip_network_encoded` must be written in URL-encoded format.
    ///</summary>
    member this.TunnelRouteUpdateATunnelRouteWithCidr
        (ipNetworkEncoded: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("ip_network_encoded", ipNetworkEncoded)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/routes/network/{ip_network_encoded}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelRouteUpdateATunnelRouteWithCidr.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelRouteUpdateATunnelRouteWithCidr.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelRouteUpdateATunnelRouteWithCidr" (int status)
        }

    ///<summary>
    ///Routes a private network through a Cloudflare Tunnel. The CIDR in `ip_network_encoded` must be written in URL-encoded format.
    ///</summary>
    member this.TunnelRouteCreateATunnelRouteWithCidr
        (
            ipNetworkEncoded: string,
            accountId: string,
            body: TunnelRouteCreateATunnelRouteWithCidrPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("ip_network_encoded", ipNetworkEncoded)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/routes/network/{ip_network_encoded}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelRouteCreateATunnelRouteWithCidr.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelRouteCreateATunnelRouteWithCidr.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelRouteCreateATunnelRouteWithCidr" (int status)
        }

    ///<summary>
    ///Deletes a private network route from an account.
    ///</summary>
    member this.TunnelRouteDeleteATunnelRoute
        (routeId: string, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("route_id", routeId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/routes/{route_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelRouteDeleteATunnelRoute.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelRouteDeleteATunnelRoute.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelRouteDeleteATunnelRoute" (int status)
        }

    ///<summary>
    ///Get a private network route in an account.
    ///</summary>
    member this.TunnelRouteGetTunnelRoute(accountId: string, routeId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("route_id", routeId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/routes/{route_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelRouteGetTunnelRoute.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelRouteGetTunnelRoute.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelRouteGetTunnelRoute" (int status)
        }

    ///<summary>
    ///Updates an existing private network route in an account. The fields that are meant to be updated should be provided in the body of the request.
    ///</summary>
    member this.TunnelRouteUpdateATunnelRoute
        (
            routeId: string,
            accountId: string,
            body: TunnelRouteUpdateATunnelRoutePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("route_id", routeId)
                  RequestPart.path ("account_id", accountId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/routes/{route_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelRouteUpdateATunnelRoute.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelRouteUpdateATunnelRoute.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelRouteUpdateATunnelRoute" (int status)
        }

    ///<summary>
    ///Lists and filters virtual networks in an account.
    ///</summary>
    member this.TunnelVirtualNetworkListVirtualNetworks
        (
            accountId: string,
            ?id: System.Guid,
            ?name: string,
            ?isDefault: bool,
            ?isDefaultNetwork: bool,
            ?isDeleted: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if id.IsSome then
                      RequestPart.query ("id", id.Value)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if isDefault.IsSome then
                      RequestPart.query ("is_default", isDefault.Value)
                  if isDefaultNetwork.IsSome then
                      RequestPart.query ("is_default_network", isDefaultNetwork.Value)
                  if isDeleted.IsSome then
                      RequestPart.query ("is_deleted", isDeleted.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/virtual_networks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelVirtualNetworkListVirtualNetworks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelVirtualNetworkListVirtualNetworks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelVirtualNetworkListVirtualNetworks" (int status)
        }

    ///<summary>
    ///Adds a new virtual network to an account.
    ///</summary>
    member this.TunnelVirtualNetworkCreateAVirtualNetwork
        (
            accountId: string,
            body: TunnelVirtualNetworkCreateAVirtualNetworkPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/virtual_networks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelVirtualNetworkCreateAVirtualNetwork.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelVirtualNetworkCreateAVirtualNetwork.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for TunnelVirtualNetworkCreateAVirtualNetwork" (int status)
        }

    ///<summary>
    ///Deletes an existing virtual network.
    ///</summary>
    member this.TunnelVirtualNetworkDelete
        (virtualNetworkId: System.Guid, accountId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("virtual_network_id", virtualNetworkId)
                  RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/virtual_networks/{virtual_network_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelVirtualNetworkDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelVirtualNetworkDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelVirtualNetworkDelete" (int status)
        }

    ///<summary>
    ///Get a virtual network.
    ///</summary>
    member this.TunnelVirtualNetworkGet
        (
            accountId: string,
            virtualNetworkId: System.Guid,
            body: TunnelVirtualNetworkGetPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("virtual_network_id", virtualNetworkId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/virtual_networks/{virtual_network_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelVirtualNetworkGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelVirtualNetworkGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelVirtualNetworkGet" (int status)
        }

    ///<summary>
    ///Updates an existing virtual network.
    ///</summary>
    member this.TunnelVirtualNetworkUpdate
        (
            accountId: string,
            virtualNetworkId: System.Guid,
            body: TunnelVirtualNetworkUpdatePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("virtual_network_id", virtualNetworkId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/teamnet/virtual_networks/{virtual_network_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return TunnelVirtualNetworkUpdate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TunnelVirtualNetworkUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TunnelVirtualNetworkUpdate" (int status)
        }

    ///<summary>
    ///Lists and filters all types of Tunnels in an account.
    ///</summary>
    member this.CloudflareTunnelListAllTunnels
        (
            accountId: string,
            ?name: string,
            ?isDeleted: bool,
            ?existedAt: string,
            ?uuid: System.Guid,
            ?wasActiveAt: System.DateTimeOffset,
            ?wasInactiveAt: System.DateTimeOffset,
            ?includePrefix: string,
            ?excludePrefix: string,
            ?tunTypes: tunnel_tunnel_types,
            ?status: string,
            ?perPage: float,
            ?page: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if isDeleted.IsSome then
                      RequestPart.query ("is_deleted", isDeleted.Value)
                  if existedAt.IsSome then
                      RequestPart.query ("existed_at", existedAt.Value)
                  if uuid.IsSome then
                      RequestPart.query ("uuid", uuid.Value)
                  if wasActiveAt.IsSome then
                      RequestPart.query ("was_active_at", wasActiveAt.Value)
                  if wasInactiveAt.IsSome then
                      RequestPart.query ("was_inactive_at", wasInactiveAt.Value)
                  if includePrefix.IsSome then
                      RequestPart.query ("include_prefix", includePrefix.Value)
                  if excludePrefix.IsSome then
                      RequestPart.query ("exclude_prefix", excludePrefix.Value)
                  if tunTypes.IsSome then
                      RequestPart.query ("tun_types", tunTypes.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/tunnels" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelListAllTunnels.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareTunnelListAllTunnels.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareTunnelListAllTunnels" (int status)
        }

    ///<summary>
    ///Lists waiting rooms for account.
    ///</summary>
    ///<param name="accountId"></param>
    ///<param name="page">Page number of paginated results.</param>
    ///<param name="perPage">Maximum number of results per page. Must be a multiple of 5.</param>
    ///<param name="cancellationToken"></param>
    member this.WaitingRoomListWaitingRoomsAccount
        (accountId: string, ?page: float, ?perPage: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/waiting_rooms" requestParts cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomListWaitingRoomsAccount.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomListWaitingRoomsAccount.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomListWaitingRoomsAccount" (int status)
        }

    ///<summary>
    ///Lists and filters Warp Connector Tunnels in an account.
    ///</summary>
    member this.CloudflareTunnelListWarpConnectorTunnels
        (
            accountId: string,
            ?name: string,
            ?isDeleted: bool,
            ?existedAt: string,
            ?uuid: System.Guid,
            ?wasActiveAt: System.DateTimeOffset,
            ?wasInactiveAt: System.DateTimeOffset,
            ?includePrefix: string,
            ?excludePrefix: string,
            ?status: string,
            ?perPage: float,
            ?page: float,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if isDeleted.IsSome then
                      RequestPart.query ("is_deleted", isDeleted.Value)
                  if existedAt.IsSome then
                      RequestPart.query ("existed_at", existedAt.Value)
                  if uuid.IsSome then
                      RequestPart.query ("uuid", uuid.Value)
                  if wasActiveAt.IsSome then
                      RequestPart.query ("was_active_at", wasActiveAt.Value)
                  if wasInactiveAt.IsSome then
                      RequestPart.query ("was_inactive_at", wasInactiveAt.Value)
                  if includePrefix.IsSome then
                      RequestPart.query ("include_prefix", includePrefix.Value)
                  if excludePrefix.IsSome then
                      RequestPart.query ("exclude_prefix", excludePrefix.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/accounts/{account_id}/warp_connector" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelListWarpConnectorTunnels.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareTunnelListWarpConnectorTunnels.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareTunnelListWarpConnectorTunnels" (int status)
        }

    ///<summary>
    ///Creates a new Warp Connector Tunnel in an account.
    ///</summary>
    member this.CloudflareTunnelCreateAWarpConnectorTunnel
        (
            accountId: string,
            body: CloudflareTunnelCreateAWarpConnectorTunnelPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/accounts/{account_id}/warp_connector" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelCreateAWarpConnectorTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelCreateAWarpConnectorTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareTunnelCreateAWarpConnectorTunnel" (int status)
        }

    ///<summary>
    ///Deletes a Warp Connector Tunnel from an account.
    ///</summary>
    member this.CloudflareTunnelDeleteAWarpConnectorTunnel
        (accountId: string, tunnelId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/warp_connector/{tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelDeleteAWarpConnectorTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelDeleteAWarpConnectorTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareTunnelDeleteAWarpConnectorTunnel" (int status)
        }

    ///<summary>
    ///Fetches a single Warp Connector Tunnel.
    ///</summary>
    member this.CloudflareTunnelGetAWarpConnectorTunnel
        (accountId: string, tunnelId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/warp_connector/{tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelGetAWarpConnectorTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareTunnelGetAWarpConnectorTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareTunnelGetAWarpConnectorTunnel" (int status)
        }

    ///<summary>
    ///Updates an existing Warp Connector Tunnel.
    ///</summary>
    member this.CloudflareTunnelUpdateAWarpConnectorTunnel
        (
            accountId: string,
            tunnelId: System.Guid,
            body: CloudflareTunnelUpdateAWarpConnectorTunnelPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/warp_connector/{tunnel_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelUpdateAWarpConnectorTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelUpdateAWarpConnectorTunnel.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for CloudflareTunnelUpdateAWarpConnectorTunnel" (int status)
        }

    ///<summary>
    ///Gets the high-availability configuration for a WARP Connector tunnel.
    ///</summary>
    member this.CloudflareTunnelConfigurationGetWarpConnectorConfiguration
        (accountId: string, tunnelId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/warp_connector/{tunnel_id}/configurations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return CloudflareTunnelConfigurationGetWarpConnectorConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelConfigurationGetWarpConnectorConfiguration.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareTunnelConfigurationGetWarpConnectorConfiguration"
                        (int status)
        }

    ///<summary>
    ///Adds or updates the high-availability configuration for a WARP Connector tunnel.
    ///</summary>
    member this.CloudflareTunnelConfigurationUpdateWarpConnectorConfiguration
        (
            accountId: string,
            tunnelId: System.Guid,
            body: tunnel_mesh_configuration_request_body,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/warp_connector/{tunnel_id}/configurations"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    CloudflareTunnelConfigurationUpdateWarpConnectorConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelConfigurationUpdateWarpConnectorConfiguration.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareTunnelConfigurationUpdateWarpConnectorConfiguration"
                        (int status)
        }

    ///<summary>
    ///Fetches connection details for a WARP Connector Tunnel.
    ///</summary>
    member this.CloudflareTunnelListWarpConnectorTunnelConnections
        (accountId: string, tunnelId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/warp_connector/{tunnel_id}/connections"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelListWarpConnectorTunnelConnections.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelListWarpConnectorTunnelConnections.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareTunnelListWarpConnectorTunnelConnections"
                        (int status)
        }

    ///<summary>
    ///Fetches connector and connection details for a WARP Connector Tunnel.
    ///</summary>
    member this.CloudflareTunnelGetWarpConnectorTunnelConnector
        (accountId: string, tunnelId: System.Guid, connectorId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId)
                  RequestPart.path ("connector_id", connectorId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/warp_connector/{tunnel_id}/connectors/{connector_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelGetWarpConnectorTunnelConnector.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelGetWarpConnectorTunnelConnector.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareTunnelGetWarpConnectorTunnelConnector"
                        (int status)
        }

    ///<summary>
    ///Triggers a manual failover for a specific WARP Connector Tunnel, setting the specified client as the active connector. The tunnel must be configured for high availability (HA) and the client must be linked to the tunnel.
    ///</summary>
    member this.CloudflareTunnelManualFailoverWarpConnectorTunnel
        (
            accountId: string,
            tunnelId: System.Guid,
            body: CloudflareTunnelManualFailoverWarpConnectorTunnelPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/accounts/{account_id}/warp_connector/{tunnel_id}/failover"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelManualFailoverWarpConnectorTunnel.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelManualFailoverWarpConnectorTunnel.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for CloudflareTunnelManualFailoverWarpConnectorTunnel"
                        (int status)
        }

    ///<summary>
    ///Gets the token used to associate warp device with a specific Warp Connector tunnel.
    ///</summary>
    member this.CloudflareTunnelGetAWarpConnectorTunnelToken
        (accountId: string, tunnelId: System.Guid, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("tunnel_id", tunnelId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/warp_connector/{tunnel_id}/token"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CloudflareTunnelGetAWarpConnectorTunnelToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    CloudflareTunnelGetAWarpConnectorTunnelToken.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for CloudflareTunnelGetAWarpConnectorTunnelToken" (int status)
        }

    ///<summary>
    ///Get IPs used on the Cloudflare/JD Cloud network, see https://www.cloudflare.com/ips for Cloudflare IPs or https://developers.cloudflare.com/china-network/reference/infrastructure/ for JD Cloud IPs.
    ///</summary>
    ///<param name="networks">Specified as `jdcloud` to list IPs used by JD Cloud data centers.</param>
    ///<param name="cancellationToken"></param>
    member this.CloudflareIpsCloudflareIpDetails(?networks: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ if networks.IsSome then
                      RequestPart.query ("networks", networks.Value) ]

            let! (status, _, content) = OpenApiHttp.getAsync httpClient "/ips" requestParts cancellationToken

            match (int status) with
            | 200 -> return CloudflareIpsCloudflareIpDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CloudflareIpsCloudflareIpDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CloudflareIpsCloudflareIpDetails" (int status)
        }

    ///<summary>
    ///Fetches all the custom pages at the zone level.
    ///</summary>
    member this.CustomPagesForAZoneListCustomPages(zoneIdentifier: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_identifier", zoneIdentifier) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_identifier}/custom_pages" requestParts cancellationToken

            match (int status) with
            | 200 -> return CustomPagesForAZoneListCustomPages.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomPagesForAZoneListCustomPages.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomPagesForAZoneListCustomPages" (int status)
        }

    ///<summary>
    ///Fetches all the custom assets at the zone level.
    ///</summary>
    member this.CustomAssetsForAZoneListCustomAssets
        (zoneIdentifier: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_identifier", zoneIdentifier)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_identifier}/custom_pages/assets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomAssetsForAZoneListCustomAssets.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomAssetsForAZoneListCustomAssets.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomAssetsForAZoneListCustomAssets" (int status)
        }

    ///<summary>
    ///Creates a new custom asset at the zone level.
    ///</summary>
    member this.CustomAssetsForAZoneCreateACustomAsset
        (
            zoneIdentifier: string,
            body: CustomAssetsForAZoneCreateACustomAssetPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_identifier", zoneIdentifier)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_identifier}/custom_pages/assets"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomAssetsForAZoneCreateACustomAsset.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomAssetsForAZoneCreateACustomAsset.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomAssetsForAZoneCreateACustomAsset" (int status)
        }

    ///<summary>
    ///Deletes an existing custom asset.
    ///</summary>
    member this.CustomAssetsForAZoneDeleteACustomAsset
        (assetName: string, zoneIdentifier: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("asset_name", assetName)
                  RequestPart.path ("zone_identifier", zoneIdentifier) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_identifier}/custom_pages/assets/{asset_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 204 -> return CustomAssetsForAZoneDeleteACustomAsset.NoContent
            | _ when (((int status) / 100) = 4) ->
                return CustomAssetsForAZoneDeleteACustomAsset.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomAssetsForAZoneDeleteACustomAsset" (int status)
        }

    ///<summary>
    ///Fetches the details of a custom asset.
    ///</summary>
    member this.CustomAssetsForAZoneGetACustomAsset
        (assetName: string, zoneIdentifier: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("asset_name", assetName)
                  RequestPart.path ("zone_identifier", zoneIdentifier) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_identifier}/custom_pages/assets/{asset_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomAssetsForAZoneGetACustomAsset.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomAssetsForAZoneGetACustomAsset.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomAssetsForAZoneGetACustomAsset" (int status)
        }

    ///<summary>
    ///Updates the configuration of an existing custom asset.
    ///</summary>
    member this.CustomAssetsForAZoneUpdateACustomAsset
        (
            assetName: string,
            zoneIdentifier: string,
            body: CustomAssetsForAZoneUpdateACustomAssetPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("asset_name", assetName)
                  RequestPart.path ("zone_identifier", zoneIdentifier)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_identifier}/custom_pages/assets/{asset_name}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomAssetsForAZoneUpdateACustomAsset.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomAssetsForAZoneUpdateACustomAsset.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomAssetsForAZoneUpdateACustomAsset" (int status)
        }

    ///<summary>
    ///Creates a signed JWT token used to preview custom pages before they are published. The API gateway rewrites zone-scoped requests to the account-level service endpoint.
    ///</summary>
    member this.CustomPagesForAZoneCreatePreviewToken
        (zoneIdentifier: string, body: custom_u002D_pages_preview_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_identifier", zoneIdentifier)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_identifier}/custom_pages/preview_tokens"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomPagesForAZoneCreatePreviewToken.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomPagesForAZoneCreatePreviewToken.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomPagesForAZoneCreatePreviewToken" (int status)
        }

    ///<summary>
    ///Fetches the details of a custom page.
    ///</summary>
    member this.CustomPagesForAZoneGetACustomPage
        (identifier: string, zoneIdentifier: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_identifier", zoneIdentifier) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_identifier}/custom_pages/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomPagesForAZoneGetACustomPage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomPagesForAZoneGetACustomPage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomPagesForAZoneGetACustomPage" (int status)
        }

    ///<summary>
    ///Updates the configuration of an existing custom page.
    ///</summary>
    member this.CustomPagesForAZoneUpdateACustomPage
        (
            identifier: string,
            zoneIdentifier: string,
            body: CustomPagesForAZoneUpdateACustomPagePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_identifier", zoneIdentifier)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_identifier}/custom_pages/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return CustomPagesForAZoneUpdateACustomPage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return CustomPagesForAZoneUpdateACustomPage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for CustomPagesForAZoneUpdateACustomPage" (int status)
        }

    ///<summary>
    ///List all Regional Hostnames within a zone.
    ///</summary>
    member this.DlsZoneRegionalHostnamesList(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/addressing/regional_hostnames"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DlsZoneRegionalHostnamesList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DlsZoneRegionalHostnamesList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DlsZoneRegionalHostnamesList" (int status)
        }

    ///<summary>
    ///Create a new Regional Hostname entry. Cloudflare will only use data centers that are physically located within the chosen region to decrypt and service HTTPS traffic. Learn more about [Regional Services](https://developers.cloudflare.com/data-localization/regional-services/get-started/).
    ///</summary>
    member this.DlsZoneRegionalHostnamesCreate
        (zoneId: string, ?cancellationToken: CancellationToken, ?body: DlsZoneRegionalHostnamesCreatePayload)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/addressing/regional_hostnames"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DlsZoneRegionalHostnamesCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DlsZoneRegionalHostnamesCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DlsZoneRegionalHostnamesCreate" (int status)
        }

    ///<summary>
    ///Delete the region configuration for a specific Regional Hostname.
    ///</summary>
    member this.DlsZoneRegionalHostnamesDelete
        (zoneId: string, hostname: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("hostname", hostname) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/addressing/regional_hostnames/{hostname}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DlsZoneRegionalHostnamesDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DlsZoneRegionalHostnamesDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DlsZoneRegionalHostnamesDelete" (int status)
        }

    ///<summary>
    ///Fetch the configuration for a specific Regional Hostname, within a zone.
    ///</summary>
    member this.DlsZoneRegionalHostnamesFetch(zoneId: string, hostname: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("hostname", hostname) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/addressing/regional_hostnames/{hostname}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DlsZoneRegionalHostnamesFetch.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DlsZoneRegionalHostnamesFetch.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DlsZoneRegionalHostnamesFetch" (int status)
        }

    ///<summary>
    ///Update the configuration for a specific Regional Hostname. Only the region_key of a hostname is mutable.
    ///</summary>
    member this.DlsZoneRegionalHostnamesPatch
        (
            zoneId: string,
            hostname: string,
            ?cancellationToken: CancellationToken,
            ?body: DlsZoneRegionalHostnamesPatchPayload
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("hostname", hostname)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/addressing/regional_hostnames/{hostname}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DlsZoneRegionalHostnamesPatch.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DlsZoneRegionalHostnamesPatch.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DlsZoneRegionalHostnamesPatch" (int status)
        }

    ///<summary>
    ///Retrieves the value of Argo Smart Routing enablement setting.
    ///</summary>
    member this.ArgoSmartRoutingGetArgoSmartRoutingSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/argo/smart_routing" requestParts cancellationToken

            match (int status) with
            | 200 -> return ArgoSmartRoutingGetArgoSmartRoutingSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ArgoSmartRoutingGetArgoSmartRoutingSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for ArgoSmartRoutingGetArgoSmartRoutingSetting" (int status)
        }

    ///<summary>
    ///Configures the value of the Argo Smart Routing enablement setting.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Configures the enablement of Argo Smart Routing.</param>
    ///<param name="cancellationToken"></param>
    member this.ArgoSmartRoutingPatchArgoSmartRoutingSetting
        (zoneId: string, body: argo_u002D_config_patch, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/argo/smart_routing" requestParts cancellationToken

            match (int status) with
            | 200 -> return ArgoSmartRoutingPatchArgoSmartRoutingSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ArgoSmartRoutingPatchArgoSmartRoutingSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for ArgoSmartRoutingPatchArgoSmartRoutingSetting" (int status)
        }

    ///<summary>
    ///Tiered Cache works by dividing Cloudflare's data centers into a hierarchy of lower-tiers and upper-tiers. If content is not cached in lower-tier data centers (generally the ones closest to a visitor), the lower-tier must ask an upper-tier to see if it has the content. If the upper-tier does not have the content, only the upper-tier can ask the origin for content. This practice improves bandwidth efficiency by limiting the number of data centers that can ask the origin for content, which reduces origin load and makes websites more cost-effective to operate. Additionally, Tiered Cache concentrates connections to origin servers so they come from a small number of data centers rather than the full set of network locations. This results in fewer open connections using server resources.
    ///</summary>
    member this.TieredCachingGetTieredCachingSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/argo/tiered_caching" requestParts cancellationToken

            match (int status) with
            | 200 -> return TieredCachingGetTieredCachingSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TieredCachingGetTieredCachingSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TieredCachingGetTieredCachingSetting" (int status)
        }

    ///<summary>
    ///Tiered Cache works by dividing Cloudflare's data centers into a hierarchy of lower-tiers and upper-tiers. If content is not cached in lower-tier data centers (generally the ones closest to a visitor), the lower-tier must ask an upper-tier to see if it has the content. If the upper-tier does not have the content, only the upper-tier can ask the origin for content. This practice improves bandwidth efficiency by limiting the number of data centers that can ask the origin for content, which reduces origin load and makes websites more cost-effective to operate. Additionally, Tiered Cache concentrates connections to origin servers so they come from a small number of data centers rather than the full set of network locations. This results in fewer open connections using server resources.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Update enablement of Tiered Caching.</param>
    ///<param name="cancellationToken"></param>
    member this.TieredCachingPatchTieredCachingSetting
        (zoneId: string, body: cache_u002D_rules_patch, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/argo/tiered_caching" requestParts cancellationToken

            match (int status) with
            | 200 -> return TieredCachingPatchTieredCachingSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return TieredCachingPatchTieredCachingSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for TieredCachingPatchTieredCachingSetting" (int status)
        }

    ///<summary>
    ///Retrieves the Cloud Connector rules configured for a zone. Rules define how traffic is routed to cloud services.
    ///</summary>
    member this.ZoneCloudConnectorRules(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/cloud_connector/rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneCloudConnectorRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneCloudConnectorRules.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ZoneCloudConnectorRules.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneCloudConnectorRules" (int status)
        }

    ///<summary>
    ///Updates Cloud Connector rules for a zone, replacing the existing rule configuration.
    ///</summary>
    member this.ZoneCloudConenctorRulesPut
        (zoneId: string, ?cancellationToken: CancellationToken, ?body: list<cloud_u002D_connector_rule>)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/cloud_connector/rules" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneCloudConenctorRulesPut.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneCloudConenctorRulesPut.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return ZoneCloudConenctorRulesPut.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneCloudConenctorRulesPut" (int status)
        }

    ///<summary>
    ///Get metadata for account-level custom nameservers on a zone.
    ///Deprecated in favor of [Show DNS Settings](https://developers.cloudflare.com/api/operations/dns-settings-for-a-zone-list-dns-settings).
    ///</summary>
    member this.AccountLevelCustomNameserversUsageForAZoneGetAccountCustomNameserverRelatedZoneMetadata
        (zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/custom_ns" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    AccountLevelCustomNameserversUsageForAZoneGetAccountCustomNameserverRelatedZoneMetadata.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLevelCustomNameserversUsageForAZoneGetAccountCustomNameserverRelatedZoneMetadata.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLevelCustomNameserversUsageForAZoneGetAccountCustomNameserverRelatedZoneMetadata"
                        (int status)
        }

    ///<summary>
    ///Set metadata for account-level custom nameservers on a zone.
    ///If you would like new zones in the account to use account custom nameservers by default, use PUT /accounts/:identifier to set the account setting use_account_custom_ns_by_default to true.
    ///Deprecated in favor of [Update DNS Settings](https://developers.cloudflare.com/api/operations/dns-settings-for-a-zone-update-dns-settings).
    ///</summary>
    member this.AccountLevelCustomNameserversUsageForAZoneSetAccountCustomNameserverRelatedZoneMetadata
        (zoneId: string, body: dns_u002D_custom_u002D_nameservers_zone_metadata, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/custom_ns" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    AccountLevelCustomNameserversUsageForAZoneSetAccountCustomNameserverRelatedZoneMetadata.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    AccountLevelCustomNameserversUsageForAZoneSetAccountCustomNameserverRelatedZoneMetadata.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AccountLevelCustomNameserversUsageForAZoneSetAccountCustomNameserverRelatedZoneMetadata"
                        (int status)
        }

    ///<summary>
    ///Retrieves a list of summarised aggregate metrics over a given time period.
    ///See [Analytics API properties](https://developers.cloudflare.com/dns/reference/analytics-api-properties/) for detailed information about the available query parameters.
    ///</summary>
    member this.DnsAnalyticsTable
        (
            zoneId: string,
            ?metrics: string,
            ?dimensions: string,
            ?since: System.DateTimeOffset,
            ?until: System.DateTimeOffset,
            ?limit: int,
            ?sort: string,
            ?filters: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if metrics.IsSome then
                      RequestPart.query ("metrics", metrics.Value)
                  if dimensions.IsSome then
                      RequestPart.query ("dimensions", dimensions.Value)
                  if since.IsSome then
                      RequestPart.query ("since", since.Value)
                  if until.IsSome then
                      RequestPart.query ("until", until.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if sort.IsSome then
                      RequestPart.query ("sort", sort.Value)
                  if filters.IsSome then
                      RequestPart.query ("filters", filters.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/dns_analytics/report" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsAnalyticsTable.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsAnalyticsTable.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsAnalyticsTable" (int status)
        }

    ///<summary>
    ///Retrieves a list of aggregate metrics grouped by time interval.
    ///See [Analytics API properties](https://developers.cloudflare.com/dns/reference/analytics-api-properties/) for detailed information about the available query parameters.
    ///</summary>
    member this.DnsAnalyticsByTime
        (
            zoneId: string,
            ?metrics: string,
            ?dimensions: string,
            ?since: System.DateTimeOffset,
            ?until: System.DateTimeOffset,
            ?limit: int,
            ?sort: string,
            ?filters: string,
            ?timeDelta: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if metrics.IsSome then
                      RequestPart.query ("metrics", metrics.Value)
                  if dimensions.IsSome then
                      RequestPart.query ("dimensions", dimensions.Value)
                  if since.IsSome then
                      RequestPart.query ("since", since.Value)
                  if until.IsSome then
                      RequestPart.query ("until", until.Value)
                  if limit.IsSome then
                      RequestPart.query ("limit", limit.Value)
                  if sort.IsSome then
                      RequestPart.query ("sort", sort.Value)
                  if filters.IsSome then
                      RequestPart.query ("filters", filters.Value)
                  if timeDelta.IsSome then
                      RequestPart.query ("time_delta", timeDelta.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/dns_analytics/report/bytime"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsAnalyticsByTime.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsAnalyticsByTime.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsAnalyticsByTime" (int status)
        }

    ///<summary>
    ///List, search, sort, and filter a zones' DNS records.
    ///</summary>
    member this.DnsRecordsForAZoneListDnsRecords
        (
            zoneId: string,
            ?name: string,
            ?nameExact: string,
            ?nameContains: string,
            ?nameStartswith: string,
            ?nameEndswith: string,
            ?``type``: string,
            ?content: string,
            ?contentExact: string,
            ?contentContains: string,
            ?contentStartswith: string,
            ?contentEndswith: string,
            ?proxied: bool,
            ?``match``: string,
            ?comment: string,
            ?commentPresent: string,
            ?commentAbsent: string,
            ?commentExact: string,
            ?commentContains: string,
            ?commentStartswith: string,
            ?commentEndswith: string,
            ?tag: string,
            ?tagPresent: string,
            ?tagAbsent: string,
            ?tagExact: string,
            ?tagContains: string,
            ?tagStartswith: string,
            ?tagEndswith: string,
            ?search: string,
            ?tagMatch: string,
            ?page: float,
            ?perPage: float,
            ?order: string,
            ?direction: string,
            ?includeShadowMetadata: bool,
            ?shadowedByName: string,
            ?shadowingName: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if name.IsSome then
                      RequestPart.query ("name", name.Value)
                  if nameExact.IsSome then
                      RequestPart.query ("name.exact", nameExact.Value)
                  if nameContains.IsSome then
                      RequestPart.query ("name.contains", nameContains.Value)
                  if nameStartswith.IsSome then
                      RequestPart.query ("name.startswith", nameStartswith.Value)
                  if nameEndswith.IsSome then
                      RequestPart.query ("name.endswith", nameEndswith.Value)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value)
                  if content.IsSome then
                      RequestPart.query ("content", content.Value)
                  if contentExact.IsSome then
                      RequestPart.query ("content.exact", contentExact.Value)
                  if contentContains.IsSome then
                      RequestPart.query ("content.contains", contentContains.Value)
                  if contentStartswith.IsSome then
                      RequestPart.query ("content.startswith", contentStartswith.Value)
                  if contentEndswith.IsSome then
                      RequestPart.query ("content.endswith", contentEndswith.Value)
                  if proxied.IsSome then
                      RequestPart.query ("proxied", proxied.Value)
                  if ``match``.IsSome then
                      RequestPart.query ("match", ``match``.Value)
                  if comment.IsSome then
                      RequestPart.query ("comment", comment.Value)
                  if commentPresent.IsSome then
                      RequestPart.query ("comment.present", commentPresent.Value)
                  if commentAbsent.IsSome then
                      RequestPart.query ("comment.absent", commentAbsent.Value)
                  if commentExact.IsSome then
                      RequestPart.query ("comment.exact", commentExact.Value)
                  if commentContains.IsSome then
                      RequestPart.query ("comment.contains", commentContains.Value)
                  if commentStartswith.IsSome then
                      RequestPart.query ("comment.startswith", commentStartswith.Value)
                  if commentEndswith.IsSome then
                      RequestPart.query ("comment.endswith", commentEndswith.Value)
                  if tag.IsSome then
                      RequestPart.query ("tag", tag.Value)
                  if tagPresent.IsSome then
                      RequestPart.query ("tag.present", tagPresent.Value)
                  if tagAbsent.IsSome then
                      RequestPart.query ("tag.absent", tagAbsent.Value)
                  if tagExact.IsSome then
                      RequestPart.query ("tag.exact", tagExact.Value)
                  if tagContains.IsSome then
                      RequestPart.query ("tag.contains", tagContains.Value)
                  if tagStartswith.IsSome then
                      RequestPart.query ("tag.startswith", tagStartswith.Value)
                  if tagEndswith.IsSome then
                      RequestPart.query ("tag.endswith", tagEndswith.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if tagMatch.IsSome then
                      RequestPart.query ("tag_match", tagMatch.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if includeShadowMetadata.IsSome then
                      RequestPart.query ("include_shadow_metadata", includeShadowMetadata.Value)
                  if shadowedByName.IsSome then
                      RequestPart.query ("shadowed_by_name", shadowedByName.Value)
                  if shadowingName.IsSome then
                      RequestPart.query ("shadowing_name", shadowingName.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/dns_records" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneListDnsRecords.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneListDnsRecords.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneListDnsRecords" (int status)
        }

    ///<summary>
    ///Create a new DNS record for a zone.
    ///Notes:
    ///- A/AAAA records cannot exist on the same name as CNAME records.
    ///- NS records cannot exist on the same name as any other record type.
    ///- Domain names are always represented in Punycode, even if Unicode
    ///  characters were used when creating the record.
    ///</summary>
    member this.DnsRecordsForAZoneCreateDnsRecord
        (
            zoneId: string,
            body: dns_u002D_records_dns_u002D_record_u002D_post,
            ?includeShadowMetadata: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body
                  if includeShadowMetadata.IsSome then
                      RequestPart.query ("include_shadow_metadata", includeShadowMetadata.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/dns_records" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneCreateDnsRecord.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneCreateDnsRecord.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneCreateDnsRecord" (int status)
        }

    ///<summary>
    ///Send a Batch of DNS Record API calls to be executed together.
    ///Notes:
    ///- Although Cloudflare will execute the batched operations in a single database transaction, Cloudflare's distributed KV store must treat each record change as a single key-value pair. This means that the propagation of changes is not atomic. See [the documentation](https://developers.cloudflare.com/dns/manage-dns-records/how-to/batch-record-changes/ "Batch DNS records") for more information.
    ///- The operations you specify within the /batch request body are always executed in the following order:
    ///    - Deletes
    ///    - Patches
    ///    - Puts
    ///    - Posts
    ///</summary>
    member this.DnsRecordsForAZoneBatchDnsRecords
        (
            zoneId: string,
            body: dns_u002D_records_dns_u002D_request_u002D_batch_u002D_object,
            ?includeShadowMetadata: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body
                  if includeShadowMetadata.IsSome then
                      RequestPart.query ("include_shadow_metadata", includeShadowMetadata.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/dns_records/batch" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneBatchDnsRecords.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneBatchDnsRecords.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneBatchDnsRecords" (int status)
        }

    ///<summary>
    ///You can export your [BIND config](https://en.wikipedia.org/wiki/Zone_file "Zone file") through this endpoint.
    ///See [the documentation](https://developers.cloudflare.com/dns/manage-dns-records/how-to/import-and-export/ "Import and export records") for more information.
    ///</summary>
    member this.DnsRecordsForAZoneExportDnsRecords(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/dns_records/export" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneExportDnsRecords.OK(content)
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneExportDnsRecords.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneExportDnsRecords" (int status)
        }

    ///<summary>
    ///You can upload your [BIND config](https://en.wikipedia.org/wiki/Zone_file "Zone file") through this endpoint. It assumes that cURL is called from a location with bind_config.txt (valid BIND config) present.
    ///See [the documentation](https://developers.cloudflare.com/dns/manage-dns-records/how-to/import-and-export/ "Import and export records") for more information.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="file">
    ///BIND config to import.
    ///**Tip:** When using cURL, a file can be uploaded using `--form 'file=@bind_config.txt'`.
    ///</param>
    ///<param name="cancellationToken"></param>
    ///<param name="proxied">
    ///Whether or not proxiable records should receive the performance and security benefits of Cloudflare.
    ///The value should be either `true` or `false`.
    ///</param>
    member this.DnsRecordsForAZoneImportDnsRecords
        (zoneId: string, file: string, ?cancellationToken: CancellationToken, ?proxied: string)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.MultipartBody
                  RequestPart.MultipartDeclaredFields [ "file"; "proxied" ]
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.multipartScalar ("file", "text/plain", file)
                  if proxied.IsSome then
                      RequestPart.multipartScalar ("proxied", "text/plain", proxied.Value) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/dns_records/import" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneImportDnsRecords.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneImportDnsRecords.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneImportDnsRecords" (int status)
        }

    ///<summary>
    ///Scan for common DNS records on your domain and automatically add them to your zone. Useful if you haven't updated your nameservers yet.
    ///</summary>
    member this.DnsRecordsForAZoneScanDnsRecords
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/dns_records/scan" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneScanDnsRecords.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneScanDnsRecords.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneScanDnsRecords" (int status)
        }

    ///<summary>
    ///Retrieves the list of DNS records discovered up to this point by the asynchronous scan. These records are temporary until explicitly accepted or rejected via `POST /scan/review`. Additional records may be discovered by the scan later.
    ///</summary>
    member this.DnsRecordsForAZoneReviewDnsScan(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/dns_records/scan/review"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneReviewDnsScan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneReviewDnsScan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneReviewDnsScan" (int status)
        }

    ///<summary>
    ///Accept or reject DNS records found by the DNS records scan. Accepted records will be permanently added to the zone, while rejected records will be permanently deleted.
    ///</summary>
    member this.DnsRecordsForAZoneApplyDnsScanResults
        (
            zoneId: string,
            body: dns_u002D_records_dns_u002D_request_u002D_review_u002D_scan_u002D_object,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/dns_records/scan/review"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneApplyDnsScanResults.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneApplyDnsScanResults.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneApplyDnsScanResults" (int status)
        }

    ///<summary>
    ///Initiates an asynchronous scan for common DNS records on your domain. Note that this **does not** automatically add records to your zone. The scan runs in the background, and results can be reviewed later using the `/scan/review` endpoints. Useful if you haven't updated your nameservers yet.
    ///</summary>
    member this.DnsRecordsForAZoneTriggerDnsScan
        (zoneId: string, ?cancellationToken: CancellationToken, ?body: System.Text.Json.Nodes.JsonNode)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if body.IsSome then
                      RequestPart.jsonContent body.Value ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/dns_records/scan/trigger"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneTriggerDnsScan.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneTriggerDnsScan.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneTriggerDnsScan" (int status)
        }

    ///<summary>
    ///Get the current DNS record usage for a zone, including the number of records and the quota limit.
    ///</summary>
    member this.DnsRecordsForAZoneGetUsage(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/dns_records/usage" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneGetUsage.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneGetUsage.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneGetUsage" (int status)
        }

    ///<summary>
    ///Permanently removes a DNS record from the zone.
    ///</summary>
    member this.DnsRecordsForAZoneDeleteDnsRecord
        (
            dnsRecordId: string,
            zoneId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_record_id", dnsRecordId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/dns_records/{dns_record_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneDeleteDnsRecord.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneDeleteDnsRecord.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneDeleteDnsRecord" (int status)
        }

    ///<summary>
    ///Retrieves details for a specific DNS record in the zone.
    ///</summary>
    member this.DnsRecordsForAZoneDnsRecordDetails
        (dnsRecordId: string, zoneId: string, ?includeShadowMetadata: bool, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_record_id", dnsRecordId)
                  RequestPart.path ("zone_id", zoneId)
                  if includeShadowMetadata.IsSome then
                      RequestPart.query ("include_shadow_metadata", includeShadowMetadata.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/dns_records/{dns_record_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneDnsRecordDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneDnsRecordDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneDnsRecordDetails" (int status)
        }

    ///<summary>
    ///Update an existing DNS record.
    ///Notes:
    ///- A/AAAA records cannot exist on the same name as CNAME records.
    ///- NS records cannot exist on the same name as any other record type.
    ///- Domain names are always represented in Punycode, even if Unicode
    ///  characters were used when creating the record.
    ///</summary>
    member this.DnsRecordsForAZonePatchDnsRecord
        (
            dnsRecordId: string,
            zoneId: string,
            body: dns_u002D_records_dns_u002D_record_u002D_patch,
            ?includeShadowMetadata: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_record_id", dnsRecordId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body
                  if includeShadowMetadata.IsSome then
                      RequestPart.query ("include_shadow_metadata", includeShadowMetadata.Value) ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/dns_records/{dns_record_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZonePatchDnsRecord.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZonePatchDnsRecord.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZonePatchDnsRecord" (int status)
        }

    ///<summary>
    ///Overwrite an existing DNS record.
    ///Notes:
    ///- A/AAAA records cannot exist on the same name as CNAME records.
    ///- NS records cannot exist on the same name as any other record type.
    ///- Domain names are always represented in Punycode, even if Unicode
    ///  characters were used when creating the record.
    ///</summary>
    member this.DnsRecordsForAZoneUpdateDnsRecord
        (
            dnsRecordId: string,
            zoneId: string,
            body: dns_u002D_records_dns_u002D_record_u002D_post,
            ?includeShadowMetadata: bool,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("dns_record_id", dnsRecordId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body
                  if includeShadowMetadata.IsSome then
                      RequestPart.query ("include_shadow_metadata", includeShadowMetadata.Value) ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/dns_records/{dns_record_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return DnsRecordsForAZoneUpdateDnsRecord.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsRecordsForAZoneUpdateDnsRecord.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsRecordsForAZoneUpdateDnsRecord" (int status)
        }

    ///<summary>
    ///Show DNS settings for a zone
    ///</summary>
    member this.DnsSettingsForAZoneListDnsSettings(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/dns_settings" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsSettingsForAZoneListDnsSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsSettingsForAZoneListDnsSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsSettingsForAZoneListDnsSettings" (int status)
        }

    ///<summary>
    ///Update DNS settings for a zone
    ///</summary>
    member this.DnsSettingsForAZoneUpdateDnsSettings
        (
            zoneId: string,
            body: dns_u002D_settings_dns_u002D_settings_u002D_zone_u002D_patch,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/dns_settings" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnsSettingsForAZoneUpdateDnsSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnsSettingsForAZoneUpdateDnsSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnsSettingsForAZoneUpdateDnsSettings" (int status)
        }

    ///<summary>
    ///Delete DNSSEC.
    ///</summary>
    member this.DnssecDeleteDnssecRecords
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/zones/{zone_id}/dnssec" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnssecDeleteDnssecRecords.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnssecDeleteDnssecRecords.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnssecDeleteDnssecRecords" (int status)
        }

    ///<summary>
    ///Details about DNSSEC status and configuration.
    ///</summary>
    member this.DnssecDnssecDetails(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/dnssec" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnssecDnssecDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnssecDnssecDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnssecDnssecDetails" (int status)
        }

    ///<summary>
    ///Enable or disable DNSSEC.
    ///</summary>
    member this.DnssecEditDnssecStatus
        (zoneId: string, body: DnssecEditDnssecStatusPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/dnssec" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnssecEditDnssecStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnssecEditDnssecStatus.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnssecEditDnssecStatus" (int status)
        }

    ///<summary>
    ///List the Zone Signing Keys (ZSKs) that DNSSEC uses for the zone.
    ///</summary>
    member this.DnssecListDnssecZsks(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/dnssec/zsk" requestParts cancellationToken

            match (int status) with
            | 200 -> return DnssecListDnssecZsks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return DnssecListDnssecZsks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DnssecListDnssecZsks" (int status)
        }

    ///<summary>
    ///List configured health checks.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="page">Page number of paginated results.</param>
    ///<param name="perPage">Maximum number of results per page. Must be a multiple of 5.</param>
    ///<param name="cancellationToken"></param>
    member this.HealthChecksListHealthChecks
        (zoneId: string, ?page: float, ?perPage: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/healthchecks" requestParts cancellationToken

            match (int status) with
            | 200 -> return HealthChecksListHealthChecks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return HealthChecksListHealthChecks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for HealthChecksListHealthChecks" (int status)
        }

    ///<summary>
    ///Create a new health check.
    ///</summary>
    member this.HealthChecksCreateHealthCheck
        (zoneId: string, body: healthchecks_query_healthcheck, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/healthchecks" requestParts cancellationToken

            match (int status) with
            | 200 -> return HealthChecksCreateHealthCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return HealthChecksCreateHealthCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for HealthChecksCreateHealthCheck" (int status)
        }

    ///<summary>
    ///Create a new preview health check.
    ///</summary>
    member this.HealthChecksCreatePreviewHealthCheck
        (zoneId: string, body: healthchecks_query_healthcheck, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/healthchecks/preview" requestParts cancellationToken

            match (int status) with
            | 200 -> return HealthChecksCreatePreviewHealthCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return HealthChecksCreatePreviewHealthCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for HealthChecksCreatePreviewHealthCheck" (int status)
        }

    ///<summary>
    ///Delete a health check.
    ///</summary>
    member this.HealthChecksDeletePreviewHealthCheck
        (
            healthcheckId: string,
            zoneId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("healthcheck_id", healthcheckId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/healthchecks/preview/{healthcheck_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return HealthChecksDeletePreviewHealthCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return HealthChecksDeletePreviewHealthCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for HealthChecksDeletePreviewHealthCheck" (int status)
        }

    ///<summary>
    ///Fetch a single configured health check preview.
    ///</summary>
    member this.HealthChecksHealthCheckPreviewDetails
        (healthcheckId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("healthcheck_id", healthcheckId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/healthchecks/preview/{healthcheck_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return HealthChecksHealthCheckPreviewDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return HealthChecksHealthCheckPreviewDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for HealthChecksHealthCheckPreviewDetails" (int status)
        }

    ///<summary>
    ///Delete a health check.
    ///</summary>
    member this.HealthChecksDeleteHealthCheck
        (
            healthcheckId: string,
            zoneId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("healthcheck_id", healthcheckId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/healthchecks/{healthcheck_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return HealthChecksDeleteHealthCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return HealthChecksDeleteHealthCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for HealthChecksDeleteHealthCheck" (int status)
        }

    ///<summary>
    ///Fetch a single configured health check.
    ///</summary>
    member this.HealthChecksHealthCheckDetails
        (healthcheckId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("healthcheck_id", healthcheckId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/healthchecks/{healthcheck_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return HealthChecksHealthCheckDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return HealthChecksHealthCheckDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for HealthChecksHealthCheckDetails" (int status)
        }

    ///<summary>
    ///Patch a configured health check.
    ///</summary>
    member this.HealthChecksPatchHealthCheck
        (
            healthcheckId: string,
            zoneId: string,
            body: healthchecks_query_healthcheck,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("healthcheck_id", healthcheckId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/healthchecks/{healthcheck_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return HealthChecksPatchHealthCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return HealthChecksPatchHealthCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for HealthChecksPatchHealthCheck" (int status)
        }

    ///<summary>
    ///Update a configured health check.
    ///</summary>
    member this.HealthChecksUpdateHealthCheck
        (
            healthcheckId: string,
            zoneId: string,
            body: healthchecks_query_healthcheck,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("healthcheck_id", healthcheckId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/healthchecks/{healthcheck_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return HealthChecksUpdateHealthCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return HealthChecksUpdateHealthCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for HealthChecksUpdateHealthCheck" (int status)
        }

    ///<summary>
    ///List the requested TLS setting for the hostnames under this zone.
    ///</summary>
    member this.PerHostnameTlsSettingsList(zoneId: string, settingId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("setting_id", settingId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/hostnames/settings/{setting_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PerHostnameTlsSettingsList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PerHostnameTlsSettingsList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PerHostnameTlsSettingsList" (int status)
        }

    ///<summary>
    ///Delete the tls setting value for the hostname.
    ///</summary>
    member this.PerHostnameTlsSettingsDelete
        (zoneId: string, settingId: string, hostname: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("setting_id", settingId)
                  RequestPart.path ("hostname", hostname) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/hostnames/settings/{setting_id}/{hostname}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PerHostnameTlsSettingsDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PerHostnameTlsSettingsDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PerHostnameTlsSettingsDelete" (int status)
        }

    ///<summary>
    ///Get the requested TLS setting for the hostname.
    ///</summary>
    member this.PerHostnameTlsSettingsGet
        (zoneId: string, settingId: string, hostname: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("setting_id", settingId)
                  RequestPart.path ("hostname", hostname) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/hostnames/settings/{setting_id}/{hostname}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PerHostnameTlsSettingsGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PerHostnameTlsSettingsGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PerHostnameTlsSettingsGet" (int status)
        }

    ///<summary>
    ///Update the tls setting value for the hostname.
    ///</summary>
    member this.PerHostnameTlsSettingsPut
        (
            zoneId: string,
            settingId: string,
            hostname: string,
            body: PerHostnameTlsSettingsPutPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("setting_id", settingId)
                  RequestPart.path ("hostname", hostname)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/hostnames/settings/{setting_id}/{hostname}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PerHostnameTlsSettingsPut.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PerHostnameTlsSettingsPut.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PerHostnameTlsSettingsPut" (int status)
        }

    ///<summary>
    ///List configured load balancers.
    ///</summary>
    member this.LoadBalancersListLoadBalancers(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/load_balancers" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancersListLoadBalancers.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancersListLoadBalancers.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancersListLoadBalancers" (int status)
        }

    ///<summary>
    ///Create a new load balancer.
    ///</summary>
    member this.LoadBalancersCreateLoadBalancer
        (zoneId: string, body: LoadBalancersCreateLoadBalancerPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/load_balancers" requestParts cancellationToken

            match (int status) with
            | 200 -> return LoadBalancersCreateLoadBalancer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancersCreateLoadBalancer.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancersCreateLoadBalancer" (int status)
        }

    ///<summary>
    ///Delete a configured load balancer.
    ///</summary>
    member this.LoadBalancersDeleteLoadBalancer
        (
            zoneId: string,
            loadBalancerId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("load_balancer_id", loadBalancerId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/load_balancers/{load_balancer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancersDeleteLoadBalancer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancersDeleteLoadBalancer.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancersDeleteLoadBalancer" (int status)
        }

    ///<summary>
    ///Fetch a single configured load balancer.
    ///</summary>
    member this.LoadBalancersLoadBalancerDetails
        (zoneId: string, loadBalancerId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("load_balancer_id", loadBalancerId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/load_balancers/{load_balancer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancersLoadBalancerDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancersLoadBalancerDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancersLoadBalancerDetails" (int status)
        }

    ///<summary>
    ///Apply changes to an existing load balancer, overwriting the supplied properties.
    ///</summary>
    member this.LoadBalancersPatchLoadBalancer
        (
            zoneId: string,
            loadBalancerId: string,
            body: LoadBalancersPatchLoadBalancerPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("load_balancer_id", loadBalancerId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/load_balancers/{load_balancer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancersPatchLoadBalancer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancersPatchLoadBalancer.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancersPatchLoadBalancer" (int status)
        }

    ///<summary>
    ///Update a configured load balancer.
    ///</summary>
    member this.LoadBalancersUpdateLoadBalancer
        (
            zoneId: string,
            loadBalancerId: string,
            body: LoadBalancersUpdateLoadBalancerPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("load_balancer_id", loadBalancerId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/load_balancers/{load_balancer_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return LoadBalancersUpdateLoadBalancer.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return LoadBalancersUpdateLoadBalancer.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for LoadBalancersUpdateLoadBalancer" (int status)
        }

    ///<summary>
    ///Returns all IP-to-cloud-region mappings configured for the zone with pagination support. Each mapping tells Cloudflare which cloud vendor and region hosts the origin at that IP, enabling the edge to route via the nearest Tiered Cache upper-tier co-located with that cloud provider. Returns an empty array when no mappings exist.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="page">Page number of paginated results.</param>
    ///<param name="perPage">Number of items per page.</param>
    ///<param name="cancellationToken"></param>
    member this.OriginCloudRegionsV2List
        (zoneId: string, ?page: int, ?perPage: int, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/origin/cloud_regions" requestParts cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsV2List.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsV2List.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsV2List" (int status)
        }

    ///<summary>
    ///Removes up to 100 IP-to-cloud-region mappings in a single request. Each IP is validated independently — successfully deleted items are returned in the `succeeded` array and IPs that could not be found or are invalid are returned in the `failed` array.
    ///</summary>
    member this.OriginCloudRegionsV2BatchDelete
        (zoneId: string, body: list<string>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/origin/cloud_regions/batch"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsV2BatchDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsV2BatchDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsV2BatchDelete" (int status)
        }

    ///<summary>
    ///Upserts up to 100 IP-to-cloud-region mappings in a single request. Items in the request body are created or replaced; mappings not included in the request body are preserved unchanged (this is a merge operation, not a full collection replacement). Each item is validated independently — valid items are applied and invalid items are returned in the `failed` array. The vendor and region for every item are validated against the list from `GET /zones/{zone_id}/origin/cloud_regions/supported_regions`.
    ///</summary>
    member this.OriginCloudRegionsV2BatchUpsert
        (
            zoneId: string,
            body: list<cache_u002D_rules_origin_cloud_region_v2_request>,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/origin/cloud_regions/batch"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsV2BatchUpsert.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsV2BatchUpsert.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsV2BatchUpsert" (int status)
        }

    ///<summary>
    ///Returns the cloud vendors and regions that are valid values for origin cloud region mappings. Each region includes the Tiered Cache upper-tier colocation codes that will be used for cache routing when a mapping targeting that region is active. Requires the zone to have Tiered Cache enabled.
    ///</summary>
    member this.OriginCloudRegionsV2SupportedRegions(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/origin/cloud_regions/supported_regions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsV2SupportedRegions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsV2SupportedRegions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsV2SupportedRegions" (int status)
        }

    ///<summary>
    ///Removes the cloud region mapping for a single origin IP address. The IP path parameter is normalized before lookup. Returns the deleted IP on success. Returns 404 if no mapping exists for the specified IP. When the last mapping for the zone is removed the underlying rule record is also deleted.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="originIp">Origin IP address whose mapping should be deleted.</param>
    ///<param name="cancellationToken"></param>
    member this.OriginCloudRegionsV2Delete(zoneId: string, originIp: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("origin_ip", originIp) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/origin/cloud_regions/{origin_ip}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsV2Delete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsV2Delete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsV2Delete" (int status)
        }

    ///<summary>
    ///Returns the cloud region mapping for a single origin IP address. The IP path parameter is normalized before lookup (RFC 5952 for IPv6). Returns 404 if the zone has no mappings or if the specified IP has no mapping.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="originIp">Origin IP address to look up. IPv4 and IPv6 are supported.</param>
    ///<param name="cancellationToken"></param>
    member this.OriginCloudRegionsV2Get(zoneId: string, originIp: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("origin_ip", originIp) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/origin/cloud_regions/{origin_ip}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsV2Get.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsV2Get.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsV2Get" (int status)
        }

    ///<summary>
    ///Creates a new IP-to-cloud-region mapping or replaces the existing mapping for the specified IP. PUT is idempotent — calling it repeatedly with the same body produces the same result. The IP path parameter is normalized to canonical form (RFC 5952 for IPv6) before storage. The vendor and region are validated against the list from `GET /zones/{zone_id}/origin/cloud_regions/supported_regions`. Returns 400 if the `origin_ip` in the body does not match the URL path parameter. Returns 403 (code 1164) when the zone has reached the limit of 3,500 IP mappings.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="originIp">Origin IP address to create or replace.</param>
    ///<param name="body">Request body for creating or replacing an origin cloud region mapping.</param>
    ///<param name="cancellationToken"></param>
    member this.OriginCloudRegionsV2Upsert
        (
            zoneId: string,
            originIp: string,
            body: cache_u002D_rules_origin_cloud_region_v2_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("origin_ip", originIp)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/origin/cloud_regions/{origin_ip}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsV2Upsert.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsV2Upsert.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsV2Upsert" (int status)
        }

    ///<summary>
    ///### Purge All Cached Content
    ///Removes ALL files from Cloudflare's cache. All tiers can purge everything.
    ///```
    ///{"purge_everything": true}
    ///```
    ///### Purge Cached Content by URL
    ///Granularly removes one or more files from Cloudflare's cache by specifying URLs. All tiers can purge by URL.
    ///To purge files with custom cache keys, include the headers used to compute the cache key as in the example. If you have a device type or geo in your cache key, you will need to include the CF-Device-Type or CF-IPCountry headers. If you have lang in your cache key, you will need to include the Accept-Language header.
    ///**NB:** When including the Origin header, be sure to include the **scheme** and **hostname**. The port number can be omitted if it is the default port (80 for http, 443 for https), but must be included otherwise.
    ///Single file purge example with files:
    ///```
    ///{"files": ["http://www.example.com/css/styles.css", "http://www.example.com/js/index.js"]}
    ///```
    ///Single file purge example with url and header pairs:
    ///```
    ///{"files": [{"url": "http://www.example.com/cat_picture.jpg", "headers": {"CF-IPCountry": "US", "CF-Device-Type": "desktop", "Accept-Language": "zh-CN"}}, {"url": "http://www.example.com/dog_picture.jpg", "headers": {"CF-IPCountry": "EU", "CF-Device-Type": "mobile", "Accept-Language": "en-US"}}]}
    ///```
    ///### Purge Cached Content by Tag, Host or Prefix
    ///Granularly removes one or more files from Cloudflare's cache either by specifying the host, the associated Cache-Tag, or a Prefix.
    ///Flex purge with tags:
    ///```
    ///{"tags": ["a-cache-tag", "another-cache-tag"]}
    ///```
    ///Flex purge with hosts:
    ///```
    ///{"hosts": ["www.example.com", "images.example.com"]}
    ///```
    ///Flex purge with prefixes:
    ///```
    ///{"prefixes": ["www.example.com/foo", "images.example.com/bar/baz"]}
    ///```
    ///### Availability and limits
    ///Please refer to [purge cache availability and limits documentation page](https://developers.cloudflare.com/cache/how-to/purge-cache/#availability-and-limits).
    ///</summary>
    member this.ZonePurge
        (zoneId: string, body: InlineUnion_41b11381e561132f71bc1dd0, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/purge_cache" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZonePurge.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZonePurge.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZonePurge" (int status)
        }

    ///<summary>
    ///Sends AXFR zone transfer request to primary nameserver(s).
    ///</summary>
    member this.``SecondaryDns(SecondaryZone)ForceAxfr``
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/secondary_dns/force_axfr"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(SecondaryZone)ForceAxfr``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ``SecondaryDns(SecondaryZone)ForceAxfr``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(SecondaryZone)ForceAxfr" (int status)
        }

    ///<summary>
    ///Delete secondary zone configuration for incoming zone transfers.
    ///</summary>
    member this.``SecondaryDns(SecondaryZone)DeleteSecondaryZoneConfiguration``
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/secondary_dns/incoming"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    ``SecondaryDns(SecondaryZone)DeleteSecondaryZoneConfiguration``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(SecondaryZone)DeleteSecondaryZoneConfiguration``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(SecondaryZone)DeleteSecondaryZoneConfiguration"
                        (int status)
        }

    ///<summary>
    ///Get secondary zone configuration for incoming zone transfers.
    ///</summary>
    member this.``SecondaryDns(SecondaryZone)SecondaryZoneConfigurationDetails``
        (zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/secondary_dns/incoming" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    ``SecondaryDns(SecondaryZone)SecondaryZoneConfigurationDetails``.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(SecondaryZone)SecondaryZoneConfigurationDetails``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(SecondaryZone)SecondaryZoneConfigurationDetails"
                        (int status)
        }

    ///<summary>
    ///Create secondary zone configuration for incoming zone transfers.
    ///</summary>
    member this.``SecondaryDns(SecondaryZone)CreateSecondaryZoneConfiguration``
        (
            zoneId: string,
            body: secondary_u002D_dns_dns_u002D_secondary_u002D_secondary_u002D_zone,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/secondary_dns/incoming"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return
                    ``SecondaryDns(SecondaryZone)CreateSecondaryZoneConfiguration``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(SecondaryZone)CreateSecondaryZoneConfiguration``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(SecondaryZone)CreateSecondaryZoneConfiguration"
                        (int status)
        }

    ///<summary>
    ///Update secondary zone configuration for incoming zone transfers.
    ///</summary>
    member this.``SecondaryDns(SecondaryZone)UpdateSecondaryZoneConfiguration``
        (
            zoneId: string,
            body: secondary_u002D_dns_dns_u002D_secondary_u002D_secondary_u002D_zone,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/secondary_dns/incoming" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    ``SecondaryDns(SecondaryZone)UpdateSecondaryZoneConfiguration``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(SecondaryZone)UpdateSecondaryZoneConfiguration``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(SecondaryZone)UpdateSecondaryZoneConfiguration"
                        (int status)
        }

    ///<summary>
    ///Delete primary zone configuration for outgoing zone transfers.
    ///</summary>
    member this.``SecondaryDns(PrimaryZone)DeletePrimaryZoneConfiguration``
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/secondary_dns/outgoing"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return ``SecondaryDns(PrimaryZone)DeletePrimaryZoneConfiguration``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(PrimaryZone)DeletePrimaryZoneConfiguration``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(PrimaryZone)DeletePrimaryZoneConfiguration"
                        (int status)
        }

    ///<summary>
    ///Get primary zone configuration for outgoing zone transfers.
    ///</summary>
    member this.``SecondaryDns(PrimaryZone)PrimaryZoneConfigurationDetails``
        (zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/secondary_dns/outgoing" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return ``SecondaryDns(PrimaryZone)PrimaryZoneConfigurationDetails``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(PrimaryZone)PrimaryZoneConfigurationDetails``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(PrimaryZone)PrimaryZoneConfigurationDetails"
                        (int status)
        }

    ///<summary>
    ///Create primary zone configuration for outgoing zone transfers.
    ///</summary>
    member this.``SecondaryDns(PrimaryZone)CreatePrimaryZoneConfiguration``
        (zoneId: string, body: secondary_u002D_dns_single_request_outgoing, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/secondary_dns/outgoing"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return ``SecondaryDns(PrimaryZone)CreatePrimaryZoneConfiguration``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(PrimaryZone)CreatePrimaryZoneConfiguration``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(PrimaryZone)CreatePrimaryZoneConfiguration"
                        (int status)
        }

    ///<summary>
    ///Update primary zone configuration for outgoing zone transfers.
    ///</summary>
    member this.``SecondaryDns(PrimaryZone)UpdatePrimaryZoneConfiguration``
        (zoneId: string, body: secondary_u002D_dns_single_request_outgoing, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/secondary_dns/outgoing" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return ``SecondaryDns(PrimaryZone)UpdatePrimaryZoneConfiguration``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(PrimaryZone)UpdatePrimaryZoneConfiguration``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(PrimaryZone)UpdatePrimaryZoneConfiguration"
                        (int status)
        }

    ///<summary>
    ///Disable outgoing zone transfers for primary zone and clears IXFR backlog of primary zone.
    ///</summary>
    member this.``SecondaryDns(PrimaryZone)DisableOutgoingZoneTransfers``
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/secondary_dns/outgoing/disable"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return ``SecondaryDns(PrimaryZone)DisableOutgoingZoneTransfers``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(PrimaryZone)DisableOutgoingZoneTransfers``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(PrimaryZone)DisableOutgoingZoneTransfers"
                        (int status)
        }

    ///<summary>
    ///Enable outgoing zone transfers for primary zone.
    ///</summary>
    member this.``SecondaryDns(PrimaryZone)EnableOutgoingZoneTransfers``
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/secondary_dns/outgoing/enable"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return ``SecondaryDns(PrimaryZone)EnableOutgoingZoneTransfers``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(PrimaryZone)EnableOutgoingZoneTransfers``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(PrimaryZone)EnableOutgoingZoneTransfers"
                        (int status)
        }

    ///<summary>
    ///Notifies the secondary nameserver(s) and clears IXFR backlog of primary zone.
    ///</summary>
    member this.``SecondaryDns(PrimaryZone)ForceDnsNotify``
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/secondary_dns/outgoing/force_notify"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SecondaryDns(PrimaryZone)ForceDnsNotify``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(PrimaryZone)ForceDnsNotify``.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SecondaryDns(PrimaryZone)ForceDnsNotify" (int status)
        }

    ///<summary>
    ///Get primary zone transfer status.
    ///</summary>
    member this.``SecondaryDns(PrimaryZone)GetOutgoingZoneTransferStatus``
        (zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/secondary_dns/outgoing/status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return ``SecondaryDns(PrimaryZone)GetOutgoingZoneTransferStatus``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SecondaryDns(PrimaryZone)GetOutgoingZoneTransferStatus``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SecondaryDns(PrimaryZone)GetOutgoingZoneTransferStatus"
                        (int status)
        }

    ///<summary>
    ///Retrieves analytics aggregated from the last minute of usage on Spectrum applications underneath a given zone.
    ///</summary>
    member this.SpectrumAggregateAnalyticsGetCurrentAggregatedAnalytics
        (zoneId: string, ?appID: string, ?coloName: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if appID.IsSome then
                      RequestPart.query ("appID", appID.Value)
                  if coloName.IsSome then
                      RequestPart.query ("colo_name", coloName.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/spectrum/analytics/aggregate/current"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SpectrumAggregateAnalyticsGetCurrentAggregatedAnalytics.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SpectrumAggregateAnalyticsGetCurrentAggregatedAnalytics.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SpectrumAggregateAnalyticsGetCurrentAggregatedAnalytics"
                        (int status)
        }

    ///<summary>
    ///Retrieves a list of aggregate metrics grouped by time interval.
    ///</summary>
    member this.``SpectrumAnalytics(ByTime)GetAnalyticsByTime``
        (
            zoneId: string,
            timeDelta: string,
            ?dimensions: spectrum_u002D_analytics_dimensions,
            ?sort: spectrum_u002D_analytics_sort,
            ?until: spectrum_u002D_analytics_until,
            ?metrics: spectrum_u002D_analytics_metrics,
            ?filters: string,
            ?since: spectrum_u002D_analytics_since,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.query ("time_delta", timeDelta)
                  if dimensions.IsSome then
                      RequestPart.query ("dimensions", dimensions.Value)
                  if sort.IsSome then
                      RequestPart.query ("sort", sort.Value)
                  if until.IsSome then
                      RequestPart.query ("until", until.Value)
                  if metrics.IsSome then
                      RequestPart.query ("metrics", metrics.Value)
                  if filters.IsSome then
                      RequestPart.query ("filters", filters.Value)
                  if since.IsSome then
                      RequestPart.query ("since", since.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/spectrum/analytics/events/bytime"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SpectrumAnalytics(ByTime)GetAnalyticsByTime``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SpectrumAnalytics(ByTime)GetAnalyticsByTime``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for SpectrumAnalytics(ByTime)GetAnalyticsByTime" (int status)
        }

    ///<summary>
    ///Retrieves a list of summarised aggregate metrics over a given time period.
    ///</summary>
    member this.``SpectrumAnalytics(Summary)GetAnalyticsSummary``
        (
            zoneId: string,
            ?dimensions: spectrum_u002D_analytics_dimensions,
            ?sort: spectrum_u002D_analytics_sort,
            ?until: spectrum_u002D_analytics_until,
            ?metrics: spectrum_u002D_analytics_metrics,
            ?filters: string,
            ?since: spectrum_u002D_analytics_since,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if dimensions.IsSome then
                      RequestPart.query ("dimensions", dimensions.Value)
                  if sort.IsSome then
                      RequestPart.query ("sort", sort.Value)
                  if until.IsSome then
                      RequestPart.query ("until", until.Value)
                  if metrics.IsSome then
                      RequestPart.query ("metrics", metrics.Value)
                  if filters.IsSome then
                      RequestPart.query ("filters", filters.Value)
                  if since.IsSome then
                      RequestPart.query ("since", since.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/spectrum/analytics/events/summary"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ``SpectrumAnalytics(Summary)GetAnalyticsSummary``.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ``SpectrumAnalytics(Summary)GetAnalyticsSummary``.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for SpectrumAnalytics(Summary)GetAnalyticsSummary" (int status)
        }

    ///<summary>
    ///Retrieves a list of currently existing Spectrum applications inside a zone.
    ///</summary>
    member this.SpectrumApplicationsListSpectrumApplications
        (
            zoneId: spectrum_u002D_config_zone_identifier,
            ?page: float,
            ?perPage: float,
            ?direction: string,
            ?order: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/spectrum/apps" requestParts cancellationToken

            match (int status) with
            | 200 -> return SpectrumApplicationsListSpectrumApplications.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SpectrumApplicationsListSpectrumApplications.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for SpectrumApplicationsListSpectrumApplications" (int status)
        }

    ///<summary>
    ///Creates a new Spectrum application from a configuration using a name for the origin.
    ///</summary>
    member this.SpectrumApplicationsCreateSpectrumApplicationUsingANameForTheOrigin
        (
            zoneId: spectrum_u002D_config_zone_identifier,
            body: spectrum_u002D_config_update_app_config,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/spectrum/apps" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    SpectrumApplicationsCreateSpectrumApplicationUsingANameForTheOrigin.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    SpectrumApplicationsCreateSpectrumApplicationUsingANameForTheOrigin.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SpectrumApplicationsCreateSpectrumApplicationUsingANameForTheOrigin"
                        (int status)
        }

    ///<summary>
    ///Deletes a previously existing application.
    ///</summary>
    member this.SpectrumApplicationsDeleteSpectrumApplication
        (
            appId: spectrum_u002D_config_app_identifier,
            zoneId: spectrum_u002D_config_zone_identifier,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("app_id", appId); RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/spectrum/apps/{app_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SpectrumApplicationsDeleteSpectrumApplication.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SpectrumApplicationsDeleteSpectrumApplication.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for SpectrumApplicationsDeleteSpectrumApplication" (int status)
        }

    ///<summary>
    ///Gets the application configuration of a specific application inside a zone.
    ///</summary>
    member this.SpectrumApplicationsGetSpectrumApplicationConfiguration
        (
            appId: spectrum_u002D_config_app_identifier,
            zoneId: spectrum_u002D_config_zone_identifier,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("app_id", appId); RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/spectrum/apps/{app_id}" requestParts cancellationToken

            match (int status) with
            | 200 -> return SpectrumApplicationsGetSpectrumApplicationConfiguration.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SpectrumApplicationsGetSpectrumApplicationConfiguration.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SpectrumApplicationsGetSpectrumApplicationConfiguration"
                        (int status)
        }

    ///<summary>
    ///Updates a previously existing application's configuration that uses a name for the origin.
    ///</summary>
    member this.SpectrumApplicationsUpdateSpectrumApplicationConfigurationUsingANameForTheOrigin
        (
            appId: spectrum_u002D_config_app_identifier,
            zoneId: spectrum_u002D_config_zone_identifier,
            body: spectrum_u002D_config_update_app_config,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("app_id", appId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/spectrum/apps/{app_id}" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return
                    SpectrumApplicationsUpdateSpectrumApplicationConfigurationUsingANameForTheOrigin.OK(
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 4) ->
                return
                    SpectrumApplicationsUpdateSpectrumApplicationConfigurationUsingANameForTheOrigin.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SpectrumApplicationsUpdateSpectrumApplicationConfigurationUsingANameForTheOrigin"
                        (int status)
        }

    ///<summary>
    ///Retrieves a list of Spectrum application protocols available for a zone.
    ///</summary>
    member this.SpectrumApplicationsListSpectrumApplicationProtocols
        (zoneId: spectrum_u002D_config_zone_identifier, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/spectrum/protocols" requestParts cancellationToken

            match (int status) with
            | 200 -> return SpectrumApplicationsListSpectrumApplicationProtocols.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SpectrumApplicationsListSpectrumApplicationProtocols.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for SpectrumApplicationsListSpectrumApplicationProtocols"
                        (int status)
        }

    ///<summary>
    ///Lists waiting rooms for zone.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="page">Page number of paginated results.</param>
    ///<param name="perPage">Maximum number of results per page. Must be a multiple of 5.</param>
    ///<param name="cancellationToken"></param>
    member this.WaitingRoomListWaitingRooms
        (zoneId: string, ?page: float, ?perPage: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/waiting_rooms" requestParts cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomListWaitingRooms.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomListWaitingRooms.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomListWaitingRooms" (int status)
        }

    ///<summary>
    ///Creates a new waiting room.
    ///</summary>
    member this.WaitingRoomCreateWaitingRoom
        (zoneId: string, body: waitingroom_query_waitingroom, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/waiting_rooms" requestParts cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomCreateWaitingRoom.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomCreateWaitingRoom.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomCreateWaitingRoom" (int status)
        }

    ///<summary>
    ///Creates a waiting room page preview. Upload a custom waiting room page for preview. You will receive a preview URL in the form `http://waitingrooms.dev/preview/&amp;lt;uuid&amp;gt;`. You can use the following query parameters to change the state of the preview:
    ///1. `force_queue`: Boolean indicating if all users will be queued in the waiting room and no one will be let into the origin website (also known as queueAll).
    ///2. `queue_is_full`: Boolean indicating if the waiting room's queue is currently full and not accepting new users at the moment.
    ///3. `queueing_method`: The queueing method currently used by the waiting room.
    ///	- **fifo** indicates a FIFO queue.
    ///	- **random** indicates a Random queue.
    ///	- **passthrough** indicates a Passthrough queue. Keep in mind that the waiting room page will only be displayed if `force_queue=true` or `event=prequeueing` — for other cases the request will pass through to the origin. For our preview, this will be a fake origin website returning \"Welcome\".
    ///	- **reject** indicates a Reject queue.
    ///4. `event`: Used to preview a waiting room event.
    ///	- **none** indicates no event is occurring.
    ///	- **prequeueing** indicates that an event is prequeueing (between `prequeue_start_time` and `event_start_time`).
    ///	- **started** indicates that an event has started (between `event_start_time` and `event_end_time`).
    ///5. `shuffle_at_event_start`: Boolean indicating if the event will shuffle users in the prequeue when it starts. This can only be set to **true** if an event is active (`event` is not **none**).
    ///For example, you can make a request to `http://waitingrooms.dev/preview/&amp;lt;uuid&amp;gt;?force_queue=false&amp;queue_is_full=false&amp;queueing_method=random&amp;event=started&amp;shuffle_at_event_start=true`
    ///6. `waitTime`: Non-zero, positive integer indicating the estimated wait time in minutes. The default value is 10 minutes.
    ///For example, you can make a request to `http://waitingrooms.dev/preview/&amp;lt;uuid&amp;gt;?waitTime=50` to configure the estimated wait time as 50 minutes.
    ///</summary>
    member this.WaitingRoomCreateACustomWaitingRoomPagePreview
        (zoneId: string, body: waitingroom_query_preview, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/waiting_rooms/preview" requestParts cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomCreateACustomWaitingRoomPagePreview.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    WaitingRoomCreateACustomWaitingRoomPagePreview.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for WaitingRoomCreateACustomWaitingRoomPagePreview"
                        (int status)
        }

    ///<summary>
    ///Get zone-level Waiting Room settings.
    ///</summary>
    member this.WaitingRoomGetZoneSettings(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/waiting_rooms/settings" requestParts cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomGetZoneSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomGetZoneSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomGetZoneSettings" (int status)
        }

    ///<summary>
    ///Patch zone-level Waiting Room settings.
    ///</summary>
    member this.WaitingRoomPatchZoneSettings
        (zoneId: string, body: waitingroom_zone_settings, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/settings"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomPatchZoneSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomPatchZoneSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomPatchZoneSettings" (int status)
        }

    ///<summary>
    ///Replace zone-level Waiting Room settings.
    ///</summary>
    member this.WaitingRoomUpdateZoneSettings
        (zoneId: string, body: waitingroom_zone_settings, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/waiting_rooms/settings" requestParts cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomUpdateZoneSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomUpdateZoneSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomUpdateZoneSettings" (int status)
        }

    ///<summary>
    ///Deletes a waiting room.
    ///</summary>
    member this.WaitingRoomDeleteWaitingRoom
        (
            waitingRoomId: string,
            zoneId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomDeleteWaitingRoom.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomDeleteWaitingRoom.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomDeleteWaitingRoom" (int status)
        }

    ///<summary>
    ///Fetches a single configured waiting room.
    ///</summary>
    member this.WaitingRoomWaitingRoomDetails
        (waitingRoomId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomWaitingRoomDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomWaitingRoomDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomWaitingRoomDetails" (int status)
        }

    ///<summary>
    ///Patches a configured waiting room.
    ///</summary>
    member this.WaitingRoomPatchWaitingRoom
        (
            waitingRoomId: string,
            zoneId: string,
            body: waitingroom_query_waitingroom,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomPatchWaitingRoom.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomPatchWaitingRoom.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomPatchWaitingRoom" (int status)
        }

    ///<summary>
    ///Updates a configured waiting room.
    ///</summary>
    member this.WaitingRoomUpdateWaitingRoom
        (
            waitingRoomId: string,
            zoneId: string,
            body: waitingroom_query_waitingroom,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomUpdateWaitingRoom.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomUpdateWaitingRoom.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomUpdateWaitingRoom" (int status)
        }

    ///<summary>
    ///Lists events for a waiting room.
    ///</summary>
    ///<param name="waitingRoomId"></param>
    ///<param name="zoneId"></param>
    ///<param name="page">Page number of paginated results.</param>
    ///<param name="perPage">Maximum number of results per page. Must be a multiple of 5.</param>
    ///<param name="cancellationToken"></param>
    member this.WaitingRoomListEvents
        (waitingRoomId: string, zoneId: string, ?page: float, ?perPage: float, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if perPage.IsSome then
                      RequestPart.query ("per_page", perPage.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/events"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomListEvents.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomListEvents.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomListEvents" (int status)
        }

    ///<summary>
    ///Only available for the Waiting Room Advanced subscription. Creates an event for a waiting room. An event takes place during a specified period of time, temporarily changing the behavior of a waiting room. While the event is active, some of the properties in the event's configuration may either override or inherit from the waiting room's configuration. Note that events cannot overlap with each other, so only one event can be active at a time.
    ///</summary>
    member this.WaitingRoomCreateEvent
        (waitingRoomId: string, zoneId: string, body: waitingroom_query_event, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/events"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomCreateEvent.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomCreateEvent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomCreateEvent" (int status)
        }

    ///<summary>
    ///Deletes an event for a waiting room.
    ///</summary>
    member this.WaitingRoomDeleteEvent
        (
            eventId: string,
            waitingRoomId: string,
            zoneId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("event_id", eventId)
                  RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/events/{event_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomDeleteEvent.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomDeleteEvent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomDeleteEvent" (int status)
        }

    ///<summary>
    ///Fetches a single configured event for a waiting room.
    ///</summary>
    member this.WaitingRoomEventDetails
        (eventId: string, waitingRoomId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("event_id", eventId)
                  RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/events/{event_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomEventDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomEventDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomEventDetails" (int status)
        }

    ///<summary>
    ///Patches a configured event for a waiting room.
    ///</summary>
    member this.WaitingRoomPatchEvent
        (
            eventId: string,
            waitingRoomId: string,
            zoneId: string,
            body: waitingroom_query_event,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("event_id", eventId)
                  RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/events/{event_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomPatchEvent.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomPatchEvent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomPatchEvent" (int status)
        }

    ///<summary>
    ///Updates a configured event for a waiting room.
    ///</summary>
    member this.WaitingRoomUpdateEvent
        (
            eventId: string,
            waitingRoomId: string,
            zoneId: string,
            body: waitingroom_query_event,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("event_id", eventId)
                  RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/events/{event_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomUpdateEvent.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomUpdateEvent.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomUpdateEvent" (int status)
        }

    ///<summary>
    ///Previews an event's configuration as if it was active. Inherited fields from the waiting room will be displayed with their current values.
    ///</summary>
    member this.WaitingRoomPreviewActiveEventDetails
        (eventId: string, waitingRoomId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("event_id", eventId)
                  RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/events/{event_id}/details"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomPreviewActiveEventDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomPreviewActiveEventDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomPreviewActiveEventDetails" (int status)
        }

    ///<summary>
    ///Lists rules for a waiting room.
    ///</summary>
    member this.WaitingRoomListWaitingRoomRules
        (waitingRoomId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomListWaitingRoomRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomListWaitingRoomRules.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomListWaitingRoomRules" (int status)
        }

    ///<summary>
    ///Only available for the Waiting Room Advanced subscription. Creates a rule for a waiting room.
    ///</summary>
    member this.WaitingRoomCreateWaitingRoomRule
        (waitingRoomId: string, zoneId: string, body: waitingroom_create_rule, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomCreateWaitingRoomRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomCreateWaitingRoomRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomCreateWaitingRoomRule" (int status)
        }

    ///<summary>
    ///Only available for the Waiting Room Advanced subscription. Replaces all rules for a waiting room.
    ///</summary>
    member this.WaitingRoomReplaceWaitingRoomRules
        (waitingRoomId: string, zoneId: string, body: waitingroom_update_rules, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/rules"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomReplaceWaitingRoomRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomReplaceWaitingRoomRules.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomReplaceWaitingRoomRules" (int status)
        }

    ///<summary>
    ///Deletes a rule for a waiting room.
    ///</summary>
    member this.WaitingRoomDeleteWaitingRoomRule
        (
            ruleId: string,
            waitingRoomId: string,
            zoneId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("rule_id", ruleId)
                  RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomDeleteWaitingRoomRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomDeleteWaitingRoomRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomDeleteWaitingRoomRule" (int status)
        }

    ///<summary>
    ///Patches a rule for a waiting room.
    ///</summary>
    member this.WaitingRoomPatchWaitingRoomRule
        (
            ruleId: string,
            waitingRoomId: string,
            zoneId: string,
            body: waitingroom_patch_rule,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("rule_id", ruleId)
                  RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/rules/{rule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomPatchWaitingRoomRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomPatchWaitingRoomRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomPatchWaitingRoomRule" (int status)
        }

    ///<summary>
    ///Fetches the status of a configured waiting room. Response fields include:
    ///1. `status`: String indicating the status of the waiting room. The possible status are:
    ///	- **not_queueing** indicates that the configured thresholds have not been met and all users are going through to the origin.
    ///	- **queueing** indicates that the thresholds have been met and some users are held in the waiting room.
    ///	- **event_prequeueing** indicates that an event is active and is currently prequeueing users before it starts.
    ///	- **suspended** indicates that the room is suspended.
    ///2. `event_id`: String of the current event's `id` if an event is active, otherwise an empty string.
    ///3. `estimated_queued_users`: Integer of the estimated number of users currently waiting in the queue.
    ///4. `estimated_total_active_users`: Integer of the estimated number of users currently active on the origin.
    ///5. `max_estimated_time_minutes`: Integer of the maximum estimated time currently presented to the users.
    ///</summary>
    member this.WaitingRoomGetWaitingRoomStatus
        (waitingRoomId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("waiting_room_id", waitingRoomId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/waiting_rooms/{waiting_room_id}/status"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return WaitingRoomGetWaitingRoomStatus.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return WaitingRoomGetWaitingRoomStatus.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for WaitingRoomGetWaitingRoomStatus" (int status)
        }

    ///<summary>
    ///List Web3 Hostnames
    ///</summary>
    member this.Web3HostnameListWeb3Hostnames(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/web3/hostnames" requestParts cancellationToken

            match (int status) with
            | 200 -> return Web3HostnameListWeb3Hostnames.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Web3HostnameListWeb3Hostnames.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return Web3HostnameListWeb3Hostnames.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Web3HostnameListWeb3Hostnames" (int status)
        }

    ///<summary>
    ///Create Web3 Hostname
    ///</summary>
    member this.Web3HostnameCreateWeb3Hostname
        (zoneId: string, body: web3_create_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/web3/hostnames" requestParts cancellationToken

            match (int status) with
            | 200 -> return Web3HostnameCreateWeb3Hostname.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Web3HostnameCreateWeb3Hostname.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return Web3HostnameCreateWeb3Hostname.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Web3HostnameCreateWeb3Hostname" (int status)
        }

    ///<summary>
    ///Delete Web3 Hostname
    ///</summary>
    member this.Web3HostnameDeleteWeb3Hostname
        (
            identifier: string,
            zoneId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/web3/hostnames/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return Web3HostnameDeleteWeb3Hostname.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Web3HostnameDeleteWeb3Hostname.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return Web3HostnameDeleteWeb3Hostname.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Web3HostnameDeleteWeb3Hostname" (int status)
        }

    ///<summary>
    ///Web3 Hostname Details
    ///</summary>
    member this.Web3HostnameWeb3HostnameDetails
        (identifier: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/web3/hostnames/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return Web3HostnameWeb3HostnameDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Web3HostnameWeb3HostnameDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return Web3HostnameWeb3HostnameDetails.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Web3HostnameWeb3HostnameDetails" (int status)
        }

    ///<summary>
    ///Edit Web3 Hostname
    ///</summary>
    member this.Web3HostnameEditWeb3Hostname
        (identifier: string, zoneId: string, body: web3_modify_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/web3/hostnames/{identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return Web3HostnameEditWeb3Hostname.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return Web3HostnameEditWeb3Hostname.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return Web3HostnameEditWeb3Hostname.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for Web3HostnameEditWeb3Hostname" (int status)
        }

    ///<summary>
    ///IPFS Universal Path Gateway Content List Details
    ///</summary>
    member this.Web3HostnameIpfsUniversalPathGatewayContentListDetails
        (identifier: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/web3/hostnames/{identifier}/ipfs_universal_path/content_list"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return Web3HostnameIpfsUniversalPathGatewayContentListDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    Web3HostnameIpfsUniversalPathGatewayContentListDetails.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 5) ->
                return
                    Web3HostnameIpfsUniversalPathGatewayContentListDetails.Status5XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for Web3HostnameIpfsUniversalPathGatewayContentListDetails"
                        (int status)
        }

    ///<summary>
    ///Update IPFS Universal Path Gateway Content List
    ///</summary>
    member this.Web3HostnameUpdateIpfsUniversalPathGatewayContentList
        (
            identifier: string,
            zoneId: string,
            body: web3_content_list_update_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/web3/hostnames/{identifier}/ipfs_universal_path/content_list"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return Web3HostnameUpdateIpfsUniversalPathGatewayContentList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    Web3HostnameUpdateIpfsUniversalPathGatewayContentList.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 5) ->
                return
                    Web3HostnameUpdateIpfsUniversalPathGatewayContentList.Status5XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for Web3HostnameUpdateIpfsUniversalPathGatewayContentList"
                        (int status)
        }

    ///<summary>
    ///List IPFS Universal Path Gateway Content List Entries
    ///</summary>
    member this.Web3HostnameListIpfsUniversalPathGatewayContentListEntries
        (identifier: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/web3/hostnames/{identifier}/ipfs_universal_path/content_list/entries"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return Web3HostnameListIpfsUniversalPathGatewayContentListEntries.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    Web3HostnameListIpfsUniversalPathGatewayContentListEntries.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 5) ->
                return
                    Web3HostnameListIpfsUniversalPathGatewayContentListEntries.Status5XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for Web3HostnameListIpfsUniversalPathGatewayContentListEntries"
                        (int status)
        }

    ///<summary>
    ///Create IPFS Universal Path Gateway Content List Entry
    ///</summary>
    member this.Web3HostnameCreateIpfsUniversalPathGatewayContentListEntry
        (
            identifier: string,
            zoneId: string,
            body: web3_content_list_entry_create_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/web3/hostnames/{identifier}/ipfs_universal_path/content_list/entries"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return Web3HostnameCreateIpfsUniversalPathGatewayContentListEntry.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    Web3HostnameCreateIpfsUniversalPathGatewayContentListEntry.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 5) ->
                return
                    Web3HostnameCreateIpfsUniversalPathGatewayContentListEntry.Status5XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for Web3HostnameCreateIpfsUniversalPathGatewayContentListEntry"
                        (int status)
        }

    ///<summary>
    ///Delete IPFS Universal Path Gateway Content List Entry
    ///</summary>
    member this.Web3HostnameDeleteIpfsUniversalPathGatewayContentListEntry
        (
            contentListEntryIdentifier: string,
            identifier: string,
            zoneId: string,
            body: System.Text.Json.Nodes.JsonNode,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("content_list_entry_identifier", contentListEntryIdentifier)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/web3/hostnames/{identifier}/ipfs_universal_path/content_list/entries/{content_list_entry_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return Web3HostnameDeleteIpfsUniversalPathGatewayContentListEntry.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    Web3HostnameDeleteIpfsUniversalPathGatewayContentListEntry.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 5) ->
                return
                    Web3HostnameDeleteIpfsUniversalPathGatewayContentListEntry.Status5XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for Web3HostnameDeleteIpfsUniversalPathGatewayContentListEntry"
                        (int status)
        }

    ///<summary>
    ///IPFS Universal Path Gateway Content List Entry Details
    ///</summary>
    member this.Web3HostnameIpfsUniversalPathGatewayContentListEntryDetails
        (contentListEntryIdentifier: string, identifier: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("content_list_entry_identifier", contentListEntryIdentifier)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/web3/hostnames/{identifier}/ipfs_universal_path/content_list/entries/{content_list_entry_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return Web3HostnameIpfsUniversalPathGatewayContentListEntryDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    Web3HostnameIpfsUniversalPathGatewayContentListEntryDetails.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 5) ->
                return
                    Web3HostnameIpfsUniversalPathGatewayContentListEntryDetails.Status5XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for Web3HostnameIpfsUniversalPathGatewayContentListEntryDetails"
                        (int status)
        }

    ///<summary>
    ///Edit IPFS Universal Path Gateway Content List Entry
    ///</summary>
    member this.Web3HostnameEditIpfsUniversalPathGatewayContentListEntry
        (
            contentListEntryIdentifier: string,
            identifier: string,
            zoneId: string,
            body: web3_content_list_entry_create_request,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("content_list_entry_identifier", contentListEntryIdentifier)
                  RequestPart.path ("identifier", identifier)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/web3/hostnames/{identifier}/ipfs_universal_path/content_list/entries/{content_list_entry_identifier}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 ->
                return Web3HostnameEditIpfsUniversalPathGatewayContentListEntry.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    Web3HostnameEditIpfsUniversalPathGatewayContentListEntry.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ when (((int status) / 100) = 5) ->
                return
                    Web3HostnameEditIpfsUniversalPathGatewayContentListEntry.Status5XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for Web3HostnameEditIpfsUniversalPathGatewayContentListEntry"
                        (int status)
        }
