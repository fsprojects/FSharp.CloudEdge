module TokenCheck

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Tenancy

let checkToken (tenancy: TenancyClient) =
    task {
        match! tenancy.UserApiTokensVerifyToken() with
        | UserApiTokensVerifyToken.OK payload ->
            printfn "Token accepted: %b" payload.success
            return true
        | UserApiTokensVerifyToken.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d, code %d: %s" status error.code error.message
            return false
    }
