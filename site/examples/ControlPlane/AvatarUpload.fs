module AvatarUpload

open System
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Media

let uploadUrl (media: MediaClient) accountId (userId: string) =
    task {
        let expiry = DateTimeOffset.UtcNow.AddMinutes 30.
        match! media.CloudflareImagesCreateAuthenticatedDirectUploadUrlV2(accountId, creator = userId, expiry = expiry) with
        | CloudflareImagesCreateAuthenticatedDirectUploadUrlV2.OK payload ->
            return Some(string payload.result["uploadURL"])
        | CloudflareImagesCreateAuthenticatedDirectUploadUrlV2.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
            return None
    }
