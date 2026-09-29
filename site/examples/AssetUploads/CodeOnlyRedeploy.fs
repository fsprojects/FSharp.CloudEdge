module CodeOnlyRedeploy

open System.Text.Json

let redeploy compute accountId =
    let settings =
        {| main_module = "worker.js"
           compatibility_date = "2026-09-06"
           keep_assets = true |}
    Deployment.uploadWorker compute accountId (JsonSerializer.SerializeToNode(settings).AsObject())
