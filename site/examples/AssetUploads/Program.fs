module Program

open FSharp.CloudEdge.Management.Compute

let publish (args: string[]) =
    task {
        use http = ClientSetup.cloudflareHttp ()
        let compute = ComputeClient http
        let accountId = ClientSetup.accountId ()
        if args = [| "--code-only" |] then
            do! CodeOnlyRedeploy.redeploy compute accountId
        else
            let assets = Manifest.scan "public"
            match! MissingFiles.check compute accountId assets with
            | Some(completionToken, []) -> do! Deployment.deploy compute accountId completionToken
            | Some(uploadToken, buckets) ->
                match! BatchedUpload.upload accountId uploadToken assets buckets with
                | Some completionToken -> do! Deployment.deploy compute accountId completionToken
                | None -> printfn "The upload returned no completion token"
            | None -> ()
    }

[<EntryPoint>]
let main args =
    (publish args).GetAwaiter().GetResult()
    0
