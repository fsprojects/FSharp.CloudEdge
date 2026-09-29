module TokenCheck

open System.Net.Http
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Tenancy

let checkToken (http: HttpClient) =
    task {
        let tenancy = TenancyClient http
        match! tenancy.UserApiTokensVerifyToken() with
        | UserApiTokensVerifyToken.OK payload ->
            let tokenStatus =
                match payload.result with
                | Some result -> string result["status"]
                | None -> "unknown"
            printfn "Token status: %s" tokenStatus
            return tokenStatus = "active"
        | UserApiTokensVerifyToken.Status4XX(httpStatus, failure) ->
            for error in failure.errors do
                eprintfn "HTTP %d: %s" httpStatus error.message
            return false
    }
