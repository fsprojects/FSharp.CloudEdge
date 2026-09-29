module AccountApi

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module OAuth = FSharp.CloudEdge.Runtime.WorkersOauthProvider

type User = {| userId: string |}

let account: OAuth.OAuthProviderOptions.ApiHandler<obj> =
    OAuth.OAuthProviderOptions.ApiHandler.Create(
        fetch = fun _ _ (ctx: Workers.ExecutionContext<User>) ->
            Workers.Exports.Response.json {| userId = ctx.props.userId |})

let home: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun _ _ _ -> U2.Case2(Workers.Exports.Response.Create("Sign in to continue")))

[<ExportDefault>]
let provider =
    OAuth.Exports.OAuthProvider(
        OAuth.OAuthProviderOptions.Create(
            defaultHandler = home,
            authorizeEndpoint = "/authorize",
            tokenEndpoint = "/oauth/token",
            clientRegistrationEndpoint = "/oauth/register",
            apiRoute = U2.Case1 "/api/",
            apiHandler = U2.Case2 account))
