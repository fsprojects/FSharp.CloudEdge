module ApiClient

open System
open System.Net.Http
open System.Net.Http.Headers
open FSharp.CloudEdge.Management.Compute
open FSharp.CloudEdge.Management.Storage
open FSharp.CloudEdge.Tenancy

let connect () =
    let token = Environment.GetEnvironmentVariable "CLOUDFLARE_API_TOKEN"
    let http = new HttpClient(BaseAddress = Uri "https://api.cloudflare.com/client/v4")
    http.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", token)
    http

let accountId = Environment.GetEnvironmentVariable "CLOUDFLARE_ACCOUNT_ID"
let http = connect ()
let tenancy = TenancyClient http
let storage = StorageClient http
let compute = ComputeClient http
