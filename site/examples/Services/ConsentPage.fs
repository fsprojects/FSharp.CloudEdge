module ConsentPage

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module OAuth = FSharp.CloudEdge.Runtime.WorkersOauthProvider

type Env =
    abstract OAUTH_PROVIDER: OAuth.OAuthHelpers

let consent: Workers.ExportedHandler<Env, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request env _ ->
            async {
                let url = Workers.Exports.URL(U2.Case1 request.url)
                if url.pathname <> "/authorize" then
                    return Workers.Exports.Response.Create("Not found", Workers.ResponseInit.Create(status = 404.))
                else
                    let! authRequest = env.OAUTH_PROVIDER.parseAuthRequest request |> Async.AwaitPromise
                    let! client = env.OAUTH_PROVIDER.lookupClient authRequest.clientId |> Async.AwaitPromise
                    let name = client |> Option.bind (fun c -> c.clientName) |> Option.defaultValue authRequest.clientId
                    let scopes = String.concat ", " authRequest.scope
                    return Workers.Exports.Response.Create($"{name} asks for: {scopes}")
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
