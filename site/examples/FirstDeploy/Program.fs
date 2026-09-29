module Program

open System.IO
open FSharp.CloudEdge.Management.Compute

let deploy () =
    task {
        use http = ClientSetup.cloudflareHttp ()
        let compute = ComputeClient http
        let accountId = ClientSetup.accountId ()
        let bundle = File.ReadAllBytes "dist/worker.js"
        let fingerprint = ChangeCheck.fingerprint (WorkerUpload.metadata.ToJsonString()) bundle
        if ChangeCheck.unchanged fingerprint then
            printfn "No change since the last upload"
        else
            let! uploaded = WorkerUpload.upload compute accountId bundle
            if not uploaded then exit 1
            ChangeCheck.remember fingerprint
        do! WorkerAddress.show compute accountId
    }

deploy().GetAwaiter().GetResult()
