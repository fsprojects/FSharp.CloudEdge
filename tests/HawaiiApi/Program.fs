module FSharp.CloudEdge.HawaiiApi.Tests

open System
open System.Net.Http
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Core.Api.Http
open FSharp.CloudEdge.Management.Compute
open FSharp.CloudEdge.Tenancy

let complete (work: System.Threading.Tasks.Task<'a>) = work.GetAwaiter().GetResult()
let check condition message = if not condition then failwith message

[<EntryPoint>]
let main args =
    use http = new HttpClient(BaseAddress = Uri args.[0])
    let tenancy = TenancyClient http
    let compute = ComputeClient http
    check (typeof<AccountsListAccounts>.Assembly = typeof<WorkerAssetsUpload>.Assembly) "Response types lost their canonical shared owner"
    check (typeof<AccountsListAccounts>.Assembly.GetName().Name = "FSharp.CloudEdge.Core.Api") "Unexpected schema owner"
    check (typeof<AccountsListAccounts>.Assembly <> typeof<ComputeClient>.Assembly) "Client assembly duplicated shared schema types"
    let accountId =
        match complete (tenancy.AccountsListAccounts(name = "A&B")) with
        | AccountsListAccounts.OK payload ->
            check payload.success "Account fixture was unsuccessful"
            payload.result.Value.Head.id
        | _ -> failwith "Unexpected account response"
    let fields: MultipartTextField list = [
        { Name = "asset-hash"; Value = Convert.ToBase64String [| 0uy; 255uy; 128uy; 65uy |]; ContentType = "application/wasm" }
    ]
    match complete (compute.WorkerAssetsUpload(accountId, true, fields)) with
    | WorkerAssetsUpload.Created payload -> check (payload.result.Value.jwt = Some "completion-fixture") "Completion payload was lost"
    | _ -> failwith "Expected the exact Created response"
    match complete (compute.WorkerAssetsUpload(accountId, true, fields)) with
    | WorkerAssetsUpload.Status4XX(status, _) -> check (status = 429) "The status class lost its actual HTTP status"
    | _ -> failwith "Expected status-class response"
    let unknownRejected =
        try complete (compute.WorkerAssetsUpload(accountId, true, fields)) |> ignore; false
        with error -> error.Message.Contains("Unexpected HTTP status 599")
    check unknownRejected "An undeclared status was accepted"
    printfn "Grouped Hawaii clients share one Core model and passed four real HTTP exchanges."
    0
