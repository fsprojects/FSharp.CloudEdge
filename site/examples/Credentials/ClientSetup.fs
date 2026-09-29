module ClientSetup

open System
open System.Net.Http
open System.Net.Http.Headers

let fromEnvironment name =
    match Environment.GetEnvironmentVariable name with
    | null | "" ->
        eprintfn "%s is not set. Load .env into this terminal first." name
        exit 1
    | value -> value

let accountId () = fromEnvironment "CLOUDFLARE_ACCOUNT_ID"

let cloudflareHttp () =
    let token = fromEnvironment "CLOUDFLARE_API_TOKEN"
    let http = new HttpClient()
    http.BaseAddress <- Uri "https://api.cloudflare.com/client/v4"
    let headers = http.DefaultRequestHeaders
    headers.Authorization <- AuthenticationHeaderValue("Bearer", token)
    http
