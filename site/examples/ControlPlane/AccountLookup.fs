module AccountLookup

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Tenancy

let findAccount (tenancy: TenancyClient) (name: string) =
    task {
        try
            match! tenancy.AccountsListAccounts(name = name) with
            | AccountsListAccounts.OK payload ->
                match Option.defaultValue [] payload.result with
                | account :: _ -> return Ok account.id
                | [] -> return Error $"No account named {name}"
            | AccountsListAccounts.Status4XX(status, failure) ->
                let details = failure.errors |> List.map (fun error -> error.message) |> String.concat "; "
                return Error $"HTTP {status}: {details}"
        with error ->
            return Error error.Message
    }
