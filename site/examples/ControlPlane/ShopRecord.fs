module ShopRecord

open System.Text.Json
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Networking

let addRecord (networking: NetworkingClient) zoneId =
    task {
        let record =
            JsonSerializer.SerializeToElement
                {| ``type`` = "CNAME"; name = "shop.example.com"; content = "shop.example.net"; proxied = true; ttl = 1 |}
            |> dns_u002D_records_dns_u002D_record_u002D_post.FromJson
        match! networking.DnsRecordsForAZoneCreateDnsRecord(zoneId, record) with
        | DnsRecordsForAZoneCreateDnsRecord.OK payload ->
            printfn "DNS record created: %b" payload.success
        | DnsRecordsForAZoneCreateDnsRecord.Status4XX(status, failure) ->
            for error in failure.errors do
                printfn "HTTP %d: %s" status error.message
    }
