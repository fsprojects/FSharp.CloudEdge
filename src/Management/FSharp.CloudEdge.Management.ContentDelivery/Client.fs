namespace rec FSharp.CloudEdge.Management.ContentDelivery

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
type ContentDeliveryClient(httpClient: HttpClient) =
    ///<summary>
    ///Deletes the stripe config for a crawler.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlCrawlerDeleteStripeConfig(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/crawler/stripe"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlCrawlerDeleteStripeConfig.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlCrawlerDeleteStripeConfig.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlCrawlerDeleteStripeConfig" (int status)
        }

    ///<summary>
    ///Gets the stripe config for a crawler.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlCrawlerGetStripeConfig(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/crawler/stripe"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlCrawlerGetStripeConfig.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlCrawlerGetStripeConfig.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlCrawlerGetStripeConfig" (int status)
        }

    ///<summary>
    ///Creates the stripe config for a crawler.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlCrawlerCreateStripeConfig(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/crawler/stripe"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlCrawlerCreateStripeConfig.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlCrawlerCreateStripeConfig.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlCrawlerCreateStripeConfig" (int status)
        }

    ///<summary>
    ///Lists the crawlers known to pay-per-crawl.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlListCrawlers(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/crawlers"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlListCrawlers.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlListCrawlers.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlListCrawlers" (int status)
        }

    ///<summary>
    ///Deletes the stripe config for a publisher.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlPublisherDeleteStripeConfig(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/publisher/stripe"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlPublisherDeleteStripeConfig.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlPublisherDeleteStripeConfig.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlPublisherDeleteStripeConfig" (int status)
        }

    ///<summary>
    ///Gets the stripe config for a publisher.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlPublisherGetStripeConfig(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/publisher/stripe"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlPublisherGetStripeConfig.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlPublisherGetStripeConfig.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlPublisherGetStripeConfig" (int status)
        }

    ///<summary>
    ///Creates the stripe config for a publisher.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlPublisherCreateStripeConfig(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/publisher/stripe"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlPublisherCreateStripeConfig.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlPublisherCreateStripeConfig.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlPublisherCreateStripeConfig" (int status)
        }

    ///<summary>
    ///Gets a download link for the account's signed pay-per-crawl terms.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlGetTermsSignatureLink(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/signature_link"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlGetTermsSignatureLink.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlGetTermsSignatureLink.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlGetTermsSignatureLink" (int status)
        }

    ///<summary>
    ///Gets the pay-per-crawl terms and conditions contract.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlGetTerms(accountId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("account_id", accountId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/terms"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlGetTerms.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlGetTerms.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlGetTerms" (int status)
        }

    ///<summary>
    ///Gets the account's pay-per-crawl terms signature status.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="remote">Refresh the signature status from Ironclad.</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlGetTermsSignature(accountId: string, ?remote: bool, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  if remote.IsSome then
                      RequestPart.query ("remote", remote.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/terms/signature"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlGetTermsSignature.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlGetTermsSignature.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlGetTermsSignature" (int status)
        }

    ///<summary>
    ///Records that an account displayed or agreed to the pay-per-crawl terms.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="eventType">The terms activity to record.</param>
    ///<param name="vid">Ironclad contract version ID.</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlSendTermsSignatureEvent
        (accountId: string, eventType: string, vid: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.query ("event_type", eventType)
                  RequestPart.query ("vid", vid) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/terms/signature"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlSendTermsSignatureEvent.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlSendTermsSignatureEvent.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlSendTermsSignatureEvent" (int status)
        }

    ///<summary>
    ///Allows an account admin to set the can_be_enabled setting on a list of zones.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlSetZonesCanBeEnabled
        (
            accountId: string,
            body: pay_u002D_per_u002D_crawl_ZonesCanBeEnabledPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/zones_can_be_enabled"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlSetZonesCanBeEnabled.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlSetZonesCanBeEnabled.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlSetZonesCanBeEnabled" (int status)
        }

    ///<summary>
    ///Provided a list of pay-per-crawl configured zones this method will return whether they can enable PPC or not.
    ///</summary>
    ///<param name="accountId">account id</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlQueryZonesCanBeEnabled
        (
            accountId: string,
            body: pay_u002D_per_u002D_crawl_ZonesCanBeEnabledQueryPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("account_id", accountId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/accounts/{account_id}/pay-per-crawl/zones_can_be_enabled/query"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlQueryZonesCanBeEnabled.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlQueryZonesCanBeEnabled.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlQueryZonesCanBeEnabled" (int status)
        }

    ///<summary>
    ///Increase cache lifetimes by automatically storing all cacheable files into Cloudflare's persistent object storage buckets. Requires Cache Reserve subscription. Note: using Tiered Cache with Cache Reserve is highly recommended to reduce Reserve operations costs. See the [developer docs](https://developers.cloudflare.com/cache/about/cache-reserve) for more information.
    ///</summary>
    member this.ZoneCacheSettingsGetCacheReserveSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/cache/cache_reserve" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsGetCacheReserveSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneCacheSettingsGetCacheReserveSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneCacheSettingsGetCacheReserveSetting" (int status)
        }

    ///<summary>
    ///Increase cache lifetimes by automatically storing all cacheable files into Cloudflare's persistent object storage buckets. Requires Cache Reserve subscription. Note: using Tiered Cache with Cache Reserve is highly recommended to reduce Reserve operations costs. See the [developer docs](https://developers.cloudflare.com/cache/about/cache-reserve) for more information.
    ///</summary>
    member this.ZoneCacheSettingsChangeCacheReserveSetting
        (zoneId: string, body: ZoneCacheSettingsChangeCacheReserveSettingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/cache/cache_reserve" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsChangeCacheReserveSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsChangeCacheReserveSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for ZoneCacheSettingsChangeCacheReserveSetting" (int status)
        }

    ///<summary>
    ///You can use Cache Reserve Clear to clear your Cache Reserve, but you must first disable Cache Reserve. In most cases, this will be accomplished within 24 hours. You cannot re-enable Cache Reserve while this process is ongoing. Keep in mind that you cannot undo or cancel this operation.
    ///</summary>
    member this.ZoneCacheSettingsGetCacheReserveClear(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/cache/cache_reserve_clear"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsGetCacheReserveClear.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneCacheSettingsGetCacheReserveClear.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneCacheSettingsGetCacheReserveClear" (int status)
        }

    ///<summary>
    ///You can use Cache Reserve Clear to clear your Cache Reserve, but you must first disable Cache Reserve. In most cases, this will be accomplished within 24 hours. You cannot re-enable Cache Reserve while this process is ongoing. Keep in mind that you cannot undo or cancel this operation.
    ///</summary>
    member this.ZoneCacheSettingsStartCacheReserveClear
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/cache/cache_reserve_clear"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsStartCacheReserveClear.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneCacheSettingsStartCacheReserveClear.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneCacheSettingsStartCacheReserveClear" (int status)
        }

    ///<summary>
    ///Returns all IP-to-cloud-region mappings configured for the zone. Each mapping tells Cloudflare which cloud vendor and region hosts the origin at that IP, enabling the edge to route via the nearest Tiered Cache upper-tier co-located with that cloud provider. Returns an empty array when no mappings exist.
    ///</summary>
    member this.OriginCloudRegionsList(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/cache/origin_cloud_regions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsList" (int status)
        }

    ///<summary>
    ///Adds or updates a single IP-to-cloud-region mapping for the zone. Unlike POST, this operation is idempotent — if a mapping for the IP already exists it is overwritten. Returns the complete updated list of all mappings for the zone. Returns 403 (code 1164) when the zone has reached the limit of 3,500 IP mappings.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Request body for creating or updating an origin cloud region mapping.</param>
    ///<param name="cancellationToken"></param>
    member this.OriginCloudRegionsUpsert
        (zoneId: string, body: cache_u002D_rules_origin_cloud_region_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/cache/origin_cloud_regions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsUpsert.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsUpsert.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsUpsert" (int status)
        }

    ///<summary>
    ///Adds a single IP-to-cloud-region mapping for the zone. The IP must be a valid IPv4 or IPv6 address and is normalized to canonical form before storage (RFC 5952 for IPv6). Returns 400 (code 1145) if a mapping for that IP already exists — use PATCH to update an existing entry. The vendor and region are validated against the list from `GET /zones/{zone_id}/cache/origin_cloud_regions/supported_regions`.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Request body for creating or updating an origin cloud region mapping.</param>
    ///<param name="cancellationToken"></param>
    member this.OriginCloudRegionsCreate
        (zoneId: string, body: cache_u002D_rules_origin_cloud_region_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/cache/origin_cloud_regions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsCreate" (int status)
        }

    ///<summary>
    ///Removes up to 100 IP-to-cloud-region mappings in a single request. Each IP is validated independently — successfully deleted items are returned in the `succeeded` array and IPs that could not be found or are invalid are returned in the `failed` array.
    ///</summary>
    member this.OriginCloudRegionsBatchDelete
        (zoneId: string, body: list<string>, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/cache/origin_cloud_regions/batch"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsBatchDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsBatchDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return OriginCloudRegionsBatchDelete.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsBatchDelete" (int status)
        }

    ///<summary>
    ///Adds or updates up to 100 IP-to-cloud-region mappings in a single request. Each item is validated independently — valid items are applied and invalid items are returned in the `failed` array. The vendor and region for every item are validated against the list from `GET /zones/{zone_id}/cache/origin_cloud_regions/supported_regions`.
    ///</summary>
    member this.OriginCloudRegionsBatchUpsert
        (
            zoneId: string,
            body: list<cache_u002D_rules_origin_cloud_region_request>,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/cache/origin_cloud_regions/batch"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsBatchUpsert.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsBatchUpsert.Status4XX(int status, (Serializer.deserialize content))
            | _ when (((int status) / 100) = 5) ->
                return OriginCloudRegionsBatchUpsert.Status5XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsBatchUpsert" (int status)
        }

    ///<summary>
    ///Returns the cloud vendors and regions that are valid values for origin cloud region mappings. Each region includes the Tiered Cache upper-tier colocation codes that will be used for cache routing when a mapping targeting that region is active. Requires the zone to have Tiered Cache enabled.
    ///</summary>
    member this.OriginCloudRegionsSupportedRegions(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/cache/origin_cloud_regions/supported_regions"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsSupportedRegions.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsSupportedRegions.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsSupportedRegions" (int status)
        }

    ///<summary>
    ///Removes the cloud region mapping for a single origin IP address. The IP path parameter is normalized before lookup. Returns the deleted entry on success. Returns 404 (code 1163) if no mapping exists for the specified IP. When the last mapping for the zone is removed the underlying rule record is also deleted.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="originIp">Origin IP address whose mapping should be deleted.</param>
    ///<param name="cancellationToken"></param>
    member this.OriginCloudRegionsDelete(zoneId: string, originIp: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("origin_ip", originIp) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/cache/origin_cloud_regions/{origin_ip}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsDelete" (int status)
        }

    ///<summary>
    ///Returns the cloud region mapping for a single origin IP address. The IP path parameter is normalized before lookup (RFC 5952 for IPv6). Returns 404 (code 1142) if the zone has no mappings or if the specified IP has no mapping.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="originIp">Origin IP address to look up. IPv4 and IPv6 are supported.</param>
    ///<param name="cancellationToken"></param>
    member this.OriginCloudRegionsGet(zoneId: string, originIp: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("origin_ip", originIp) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/cache/origin_cloud_regions/{origin_ip}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return OriginCloudRegionsGet.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return OriginCloudRegionsGet.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for OriginCloudRegionsGet" (int status)
        }

    ///<summary>
    ///Instructs Cloudflare to check a regional hub data center on the way to your upper tier. This can help improve performance for smart and custom tiered cache topologies.
    ///</summary>
    member this.ZoneCacheSettingsGetRegionalTieredCacheSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/cache/regional_tiered_cache"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsGetRegionalTieredCacheSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsGetRegionalTieredCacheSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneCacheSettingsGetRegionalTieredCacheSetting"
                        (int status)
        }

    ///<summary>
    ///Instructs Cloudflare to check a regional hub data center on the way to your upper tier. This can help improve performance for smart and custom tiered cache topologies.
    ///</summary>
    member this.ZoneCacheSettingsChangeRegionalTieredCacheSetting
        (
            zoneId: string,
            body: ZoneCacheSettingsChangeRegionalTieredCacheSettingPayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/cache/regional_tiered_cache"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsChangeRegionalTieredCacheSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    ZoneCacheSettingsChangeRegionalTieredCacheSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for ZoneCacheSettingsChangeRegionalTieredCacheSetting"
                        (int status)
        }

    ///<summary>
    ///Smart Tiered Cache dynamically selects the single closest upper tier for each of your website’s origins with no configuration required, using our in-house performance and routing data. Cloudflare collects latency data for each request to an origin, and uses the latency data to determine how well any upper-tier data center is connected with an origin. As a result, Cloudflare can select the data center with the lowest latency to be the upper-tier for an origin.
    ///</summary>
    member this.SmartTieredCacheDeleteSmartTieredCacheSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/cache/tiered_cache_smart_topology_enable"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartTieredCacheDeleteSmartTieredCacheSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SmartTieredCacheDeleteSmartTieredCacheSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for SmartTieredCacheDeleteSmartTieredCacheSetting" (int status)
        }

    ///<summary>
    ///Smart Tiered Cache dynamically selects the single closest upper tier for each of your website’s origins with no configuration required, using our in-house performance and routing data. Cloudflare collects latency data for each request to an origin, and uses the latency data to determine how well any upper-tier data center is connected with an origin. As a result, Cloudflare can select the data center with the lowest latency to be the upper-tier for an origin.
    ///</summary>
    member this.SmartTieredCacheGetSmartTieredCacheSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/cache/tiered_cache_smart_topology_enable"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartTieredCacheGetSmartTieredCacheSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SmartTieredCacheGetSmartTieredCacheSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for SmartTieredCacheGetSmartTieredCacheSetting" (int status)
        }

    ///<summary>
    ///Smart Tiered Cache dynamically selects the single closest upper tier for each of your website’s origins with no configuration required, using our in-house performance and routing data. Cloudflare collects latency data for each request to an origin, and uses the latency data to determine how well any upper-tier data center is connected with an origin. As a result, Cloudflare can select the data center with the lowest latency to be the upper-tier for an origin.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Update enablement of Smart Tiered Cache.</param>
    ///<param name="cancellationToken"></param>
    member this.SmartTieredCachePatchSmartTieredCacheSetting
        (zoneId: string, body: cache_u002D_rules_smart_tiered_cache_patch, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/cache/tiered_cache_smart_topology_enable"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartTieredCachePatchSmartTieredCacheSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SmartTieredCachePatchSmartTieredCacheSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for SmartTieredCachePatchSmartTieredCacheSetting" (int status)
        }

    ///<summary>
    ///Smart Tiered Cache dynamically selects the single closest upper tier for each of your website's origins with no configuration required, using our in-house performance and routing data. Cloudflare collects latency data for each request to an origin, and uses the latency data to determine how well any upper-tier data center is connected with an origin. As a result, Cloudflare can select the data center with the lowest latency to be the upper-tier for an origin.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">Update enablement of Smart Tiered Cache.</param>
    ///<param name="cancellationToken"></param>
    member this.SmartTieredCacheCreateSmartTieredCacheSetting
        (zoneId: string, body: cache_u002D_rules_smart_tiered_cache_patch, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/cache/tiered_cache_smart_topology_enable"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartTieredCacheCreateSmartTieredCacheSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    SmartTieredCacheCreateSmartTieredCacheSetting.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf "Unexpected HTTP status %d for SmartTieredCacheCreateSmartTieredCacheSetting" (int status)
        }

    ///<summary>
    ///Variant support enables caching variants of images with certain file extensions in addition to the original. This only applies when the origin server sends the 'Vary: Accept' response header. If the origin server sends 'Vary: Accept' but does not serve the variant requested, the response will not be cached. This will be indicated with BYPASS cache status in the response headers.
    ///</summary>
    member this.ZoneCacheSettingsDeleteVariantsSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/zones/{zone_id}/cache/variants" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsDeleteVariantsSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneCacheSettingsDeleteVariantsSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneCacheSettingsDeleteVariantsSetting" (int status)
        }

    ///<summary>
    ///Variant support enables caching variants of images with certain file extensions in addition to the original. This only applies when the origin server sends the 'Vary: Accept' response header. If the origin server sends 'Vary: Accept' but does not serve the variant requested, the response will not be cached. This will be indicated with BYPASS cache status in the response headers.
    ///</summary>
    member this.ZoneCacheSettingsGetVariantsSetting(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/cache/variants" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsGetVariantsSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneCacheSettingsGetVariantsSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneCacheSettingsGetVariantsSetting" (int status)
        }

    ///<summary>
    ///Variant support enables caching variants of images with certain file extensions in addition to the original. This only applies when the origin server sends the 'Vary: Accept' response header. If the origin server sends 'Vary: Accept' but does not serve the variant requested, the response will not be cached. This will be indicated with BYPASS cache status in the response headers.
    ///</summary>
    member this.ZoneCacheSettingsChangeVariantsSetting
        (zoneId: string, body: ZoneCacheSettingsChangeVariantsSettingPayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/cache/variants" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZoneCacheSettingsChangeVariantsSetting.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneCacheSettingsChangeVariantsSetting.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneCacheSettingsChangeVariantsSetting" (int status)
        }

    ///<summary>
    ///Lists configured environments for a zone.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="cancellationToken"></param>
    member this.ZonesEnvironmentsList(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/environments" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZonesEnvironmentsList.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZonesEnvironmentsList.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZonesEnvironmentsList" (int status)
        }

    ///<summary>
    ///Applies partial updates to zone environments.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ZonesEnvironmentsEdit
        (zoneId: string, body: kamino_environments_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/environments" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZonesEnvironmentsEdit.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZonesEnvironmentsEdit.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZonesEnvironmentsEdit" (int status)
        }

    ///<summary>
    ///Creates environments for a zone.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ZonesEnvironmentsCreate
        (zoneId: string, body: kamino_environments_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/environments" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZonesEnvironmentsCreate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZonesEnvironmentsCreate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZonesEnvironmentsCreate" (int status)
        }

    ///<summary>
    ///Replaces the full environment configuration for a zone.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ZonesEnvironmentsUpdate
        (zoneId: string, body: kamino_environments_request, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/environments" requestParts cancellationToken

            match (int status) with
            | 200 -> return ZonesEnvironmentsUpdate.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZonesEnvironmentsUpdate.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZonesEnvironmentsUpdate" (int status)
        }

    ///<summary>
    ///Deletes a zone environment by reference identifier.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="environmentId">Reference identifier for the environment.</param>
    ///<param name="cancellationToken"></param>
    member this.ZonesEnvironmentsDelete(zoneId: string, environmentId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("environment_id", environmentId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/environments/{environment_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZonesEnvironmentsDelete.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZonesEnvironmentsDelete.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZonesEnvironmentsDelete" (int status)
        }

    ///<summary>
    ///Purge cached content scoped to a specific environment. Supports the same purge types as the zone-level endpoint (purge everything, by URL, by tag, host, or prefix).
    ///### Availability and limits
    ///Please refer to [purge cache availability and limits documentation page](https://developers.cloudflare.com/cache/how-to/purge-cache/#availability-and-limits).
    ///</summary>
    member this.ZoneEnvironmentPurge
        (
            zoneId: string,
            environmentId: string,
            body: InlineUnion_0e1573bd5fabb12c47e4dddb,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("environment_id", environmentId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/environments/{environment_id}/purge_cache"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZoneEnvironmentPurge.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZoneEnvironmentPurge.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZoneEnvironmentPurge" (int status)
        }

    ///<summary>
    ///Rolls a zone environment back to its previous version.
    ///</summary>
    ///<param name="zoneId">Identifier of the zone.</param>
    ///<param name="environmentId">Reference identifier for the environment.</param>
    ///<param name="cancellationToken"></param>
    member this.ZonesEnvironmentsRollback
        (zoneId: string, environmentId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  RequestPart.path ("environment_id", environmentId) ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/environments/{environment_id}/rollback"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return ZonesEnvironmentsRollback.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return ZonesEnvironmentsRollback.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for ZonesEnvironmentsRollback" (int status)
        }

    ///<summary>
    ///Fetches Page Rules in a zone.
    ///</summary>
    member this.PageRulesListPageRules
        (
            zoneId: string,
            ?order: string,
            ?direction: string,
            ?``match``: string,
            ?status: string,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId)
                  if order.IsSome then
                      RequestPart.query ("order", order.Value)
                  if direction.IsSome then
                      RequestPart.query ("direction", direction.Value)
                  if ``match``.IsSome then
                      RequestPart.query ("match", ``match``.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/pagerules" requestParts cancellationToken

            match (int status) with
            | 200 -> return PageRulesListPageRules.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PageRulesListPageRules.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PageRulesListPageRules" (int status)
        }

    ///<summary>
    ///Creates a new Page Rule.
    ///</summary>
    member this.PageRulesCreateAPageRule
        (zoneId: string, body: PageRulesCreateAPageRulePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync httpClient "/zones/{zone_id}/pagerules" requestParts cancellationToken

            match (int status) with
            | 200 -> return PageRulesCreateAPageRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PageRulesCreateAPageRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PageRulesCreateAPageRule" (int status)
        }

    ///<summary>
    ///Returns a list of settings (and their details) that Page Rules can apply to matching requests.
    ///</summary>
    member this.AvailablePageRulesSettingsListAvailablePageRulesSettings
        (zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/pagerules/settings" requestParts cancellationToken

            match (int status) with
            | 200 ->
                return AvailablePageRulesSettingsListAvailablePageRulesSettings.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return
                    AvailablePageRulesSettingsListAvailablePageRulesSettings.Status4XX(
                        int status,
                        (Serializer.deserialize content)
                    )
            | _ ->
                return
                    failwithf
                        "Unexpected HTTP status %d for AvailablePageRulesSettingsListAvailablePageRulesSettings"
                        (int status)
        }

    ///<summary>
    ///Deletes an existing Page Rule.
    ///</summary>
    member this.PageRulesDeleteAPageRule(pageruleId: string, zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pagerule_id", pageruleId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/zones/{zone_id}/pagerules/{pagerule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PageRulesDeleteAPageRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PageRulesDeleteAPageRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PageRulesDeleteAPageRule" (int status)
        }

    ///<summary>
    ///Fetches the details of a Page Rule.
    ///</summary>
    member this.PageRulesGetAPageRule(pageruleId: string, zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pagerule_id", pageruleId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/pagerules/{pagerule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PageRulesGetAPageRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PageRulesGetAPageRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PageRulesGetAPageRule" (int status)
        }

    ///<summary>
    ///Updates one or more fields of an existing Page Rule.
    ///</summary>
    member this.PageRulesEditAPageRule
        (pageruleId: string, zoneId: string, body: PageRulesEditAPageRulePayload, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pagerule_id", pageruleId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/pagerules/{pagerule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PageRulesEditAPageRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PageRulesEditAPageRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PageRulesEditAPageRule" (int status)
        }

    ///<summary>
    ///Replaces the configuration of an existing Page Rule. The configuration of the updated Page Rule will exactly match the data passed in the API request.
    ///</summary>
    member this.PageRulesUpdateAPageRule
        (
            pageruleId: string,
            zoneId: string,
            body: PageRulesUpdateAPageRulePayload,
            ?cancellationToken: CancellationToken
        ) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("pagerule_id", pageruleId)
                  RequestPart.path ("zone_id", zoneId)
                  RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/zones/{zone_id}/pagerules/{pagerule_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PageRulesUpdateAPageRule.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return PageRulesUpdateAPageRule.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PageRulesUpdateAPageRule" (int status)
        }

    ///<summary>
    ///Gets whether pay-per-crawl can be enabled for a zone.
    ///</summary>
    ///<param name="zoneId">zone id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlGetZoneCanBeEnabled(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/pay-per-crawl/can_be_enabled"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlGetZoneCanBeEnabled.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlGetZoneCanBeEnabled.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlGetZoneCanBeEnabled" (int status)
        }

    ///<summary>
    ///Gets the pay-per-crawl config for a zone including the bot configuration.
    ///</summary>
    ///<param name="zoneId">zone id</param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlGetConfig(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/pay-per-crawl/configuration"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlGetConfig.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlGetConfig.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlGetConfig" (int status)
        }

    ///<summary>
    ///Changes the pay-per-crawl config for a zone.
    ///</summary>
    ///<param name="zoneId">zone id</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlPatchConfig
        (zoneId: string, body: pay_u002D_per_u002D_crawl_DaricConfig, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/zones/{zone_id}/pay-per-crawl/configuration"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlPatchConfig.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlPatchConfig.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlPatchConfig" (int status)
        }

    ///<summary>
    ///Creates the pay-per-crawl config for a zone.
    ///</summary>
    ///<param name="zoneId">zone id</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.PayPerCrawlCreateConfig
        (zoneId: string, body: pay_u002D_per_u002D_crawl_DaricConfig, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/pay-per-crawl/configuration"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return PayPerCrawlCreateConfig.OK((Serializer.deserialize content))
            | 400 -> return PayPerCrawlCreateConfig.BadRequest((Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for PayPerCrawlCreateConfig" (int status)
        }

    ///<summary>
    ///Retrieve Smart Shield Settings.
    ///</summary>
    member this.SmartShieldGetSettings(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/smart_shield" requestParts cancellationToken

            match (int status) with
            | 200 -> return SmartShieldGetSettings.OK((Serializer.deserialize content))
            | 500 -> return SmartShieldGetSettings.InternalServerError((Serializer.deserialize content))
            | 502 -> return SmartShieldGetSettings.BadGateway((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SmartShieldGetSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SmartShieldGetSettings" (int status)
        }

    ///<summary>
    ///Set Smart Shield Settings.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">The patch body for Smart Shield.</param>
    ///<param name="cancellationToken"></param>
    member this.SmartShieldPatchSettings
        (zoneId: string, body: smartshield_smart_shield_settings_patch_body, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.patchAsync httpClient "/zones/{zone_id}/smart_shield" requestParts cancellationToken

            match (int status) with
            | 200 -> return SmartShieldPatchSettings.OK((Serializer.deserialize content))
            | 500 -> return SmartShieldPatchSettings.InternalServerError((Serializer.deserialize content))
            | 502 -> return SmartShieldPatchSettings.BadGateway((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SmartShieldPatchSettings.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SmartShieldPatchSettings" (int status)
        }

    ///<summary>
    ///You can use Cache Reserve Clear to clear your Cache Reserve, but you must first disable Cache Reserve. In most cases, this will be accomplished within 24 hours. You cannot re-enable Cache Reserve while this process is ongoing. Keep in mind that you cannot undo or cancel this operation.
    ///</summary>
    member this.SmartShieldSettingsGetCacheReserveClear(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/smart_shield/cache_reserve_clear"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartShieldSettingsGetCacheReserveClear.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SmartShieldSettingsGetCacheReserveClear.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SmartShieldSettingsGetCacheReserveClear" (int status)
        }

    ///<summary>
    ///You can use Cache Reserve Clear to clear your Cache Reserve, but you must first disable Cache Reserve. In most cases, this will be accomplished within 24 hours. You cannot re-enable Cache Reserve while this process is ongoing. Keep in mind that you cannot undo or cancel this operation.
    ///</summary>
    member this.SmartShieldSettingsStartCacheReserveClear
        (zoneId: string, body: System.Text.Json.Nodes.JsonNode, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/smart_shield/cache_reserve_clear"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartShieldSettingsStartCacheReserveClear.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SmartShieldSettingsStartCacheReserveClear.Status4XX(int status, (Serializer.deserialize content))
            | _ ->
                return failwithf "Unexpected HTTP status %d for SmartShieldSettingsStartCacheReserveClear" (int status)
        }

    ///<summary>
    ///List configured health checks.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="page">Page number of paginated results.</param>
    ///<param name="perPage">Maximum number of results per page. Must be a multiple of 5.</param>
    ///<param name="cancellationToken"></param>
    member this.SmartShieldListHealthChecks
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
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/smart_shield/healthchecks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartShieldListHealthChecks.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SmartShieldListHealthChecks.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SmartShieldListHealthChecks" (int status)
        }

    ///<summary>
    ///Create a new health check.
    ///</summary>
    member this.SmartShieldCreateHealthCheck
        (zoneId: string, body: smartshield_query_healthcheck, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/zones/{zone_id}/smart_shield/healthchecks"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartShieldCreateHealthCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SmartShieldCreateHealthCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SmartShieldCreateHealthCheck" (int status)
        }

    ///<summary>
    ///Delete a health check.
    ///</summary>
    member this.SmartShieldDeleteHealthCheck
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
                    "/zones/{zone_id}/smart_shield/healthchecks/{healthcheck_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartShieldDeleteHealthCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SmartShieldDeleteHealthCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SmartShieldDeleteHealthCheck" (int status)
        }

    ///<summary>
    ///Fetch a single configured health check.
    ///</summary>
    member this.SmartShieldHealthCheckDetails
        (healthcheckId: string, zoneId: string, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("healthcheck_id", healthcheckId)
                  RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/zones/{zone_id}/smart_shield/healthchecks/{healthcheck_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartShieldHealthCheckDetails.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SmartShieldHealthCheckDetails.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SmartShieldHealthCheckDetails" (int status)
        }

    ///<summary>
    ///Patch a configured health check.
    ///</summary>
    member this.SmartShieldPatchHealthCheck
        (
            healthcheckId: string,
            zoneId: string,
            body: smartshield_query_healthcheck,
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
                    "/zones/{zone_id}/smart_shield/healthchecks/{healthcheck_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartShieldPatchHealthCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SmartShieldPatchHealthCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SmartShieldPatchHealthCheck" (int status)
        }

    ///<summary>
    ///Update a configured health check.
    ///</summary>
    member this.SmartShieldUpdateHealthCheck
        (
            healthcheckId: string,
            zoneId: string,
            body: smartshield_single_hc_response,
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
                    "/zones/{zone_id}/smart_shield/healthchecks/{healthcheck_id}"
                    requestParts
                    cancellationToken

            match (int status) with
            | 200 -> return SmartShieldUpdateHealthCheck.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return SmartShieldUpdateHealthCheck.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for SmartShieldUpdateHealthCheck" (int status)
        }

    ///<summary>
    ///Deletes the URL Normalization settings.
    ///</summary>
    member this.DeleteUrlNormalization(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.deleteAsync httpClient "/zones/{zone_id}/url_normalization" requestParts cancellationToken

            match (int status) with
            | 204 -> return DeleteUrlNormalization.NoContent
            | _ when (((int status) / 100) = 4) ->
                return DeleteUrlNormalization.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for DeleteUrlNormalization" (int status)
        }

    ///<summary>
    ///Fetches the current URL Normalization settings.
    ///</summary>
    member this.GetUrlNormalization(zoneId: string, ?cancellationToken: CancellationToken) =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts = [ RequestPart.path ("zone_id", zoneId) ]

            let! (status, _, content) =
                OpenApiHttp.getAsync httpClient "/zones/{zone_id}/url_normalization" requestParts cancellationToken

            match (int status) with
            | 200 -> return GetUrlNormalization.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return GetUrlNormalization.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for GetUrlNormalization" (int status)
        }

    ///<summary>
    ///Updates the URL Normalization settings.
    ///</summary>
    ///<param name="zoneId"></param>
    ///<param name="body">A URL Normalization object.</param>
    ///<param name="cancellationToken"></param>
    member this.UpdateUrlNormalization
        (zoneId: string, body: rulesets_UrlNormalization, ?cancellationToken: CancellationToken)
        =
        Microsoft.FSharp.Control.TaskBuilder.task {
            let requestParts =
                [ RequestPart.path ("zone_id", zoneId); RequestPart.jsonContent body ]

            let! (status, _, content) =
                OpenApiHttp.putAsync httpClient "/zones/{zone_id}/url_normalization" requestParts cancellationToken

            match (int status) with
            | 200 -> return UpdateUrlNormalization.OK((Serializer.deserialize content))
            | _ when (((int status) / 100) = 4) ->
                return UpdateUrlNormalization.Status4XX(int status, (Serializer.deserialize content))
            | _ -> return failwithf "Unexpected HTTP status %d for UpdateUrlNormalization" (int status)
        }
