module SetupProgram

open System.IO
open FSharp.CloudEdge.Core.Api.Http
open FSharp.CloudEdge.Core.Api.Types
open ApiClient

let uploadGuestbook databaseId namespaceId =
    let worker: MultipartFile =
        { Bytes = File.ReadAllBytes "dist/worker.js"
          FileName = "worker.js"
          ContentType = Some "application/javascript+module"
          PartName = Some "worker.js" }
    let metadata = GuestbookWorker.metadata databaseId namespaceId
    compute.WorkerScriptUploadWorkerModule(accountId, "guestbook", metadata, files = [ worker ])

let setup () =
    task {
        let! tokenAccepted = TokenCheck.checkToken tenancy
        let! databaseId = GuestbookDatabase.createDatabase storage accountId
        let! namespaceId = SessionStore.createNamespace storage accountId
        let! bucketCreated = PhotoBucket.createBucket storage accountId
        let! queueId = OrderQueue.createQueue compute accountId
        match databaseId, namespaceId, queueId with
        | Some databaseId, Some namespaceId, Some queueId when tokenAccepted && bucketCreated ->
            match! uploadGuestbook databaseId namespaceId with
            | WorkerScriptUploadWorkerModule.OK _ ->
                do! OrderConsumer.attach compute accountId queueId
                do! NightlyCleanup.schedule compute accountId
                do! PublicUrl.publish compute accountId "guestbook"
                return 0
            | WorkerScriptUploadWorkerModule.Status4XX(status, _) ->
                printfn "Upload rejected with HTTP %d" status
                return 1
        | _ -> return 1
    }

[<EntryPoint>]
let main _ = setup().GetAwaiter().GetResult()
