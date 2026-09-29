module CloudEdgeDurableObjectComposition

open System
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers
module Transport = FSharp.CloudEdge.Support.Workers.DurableObjects

type NativeRequest = Transport.NativeRequest
type Frame = {| messageId: string; delta: float |}
type Receipt = {| count: float; duplicate: bool |}

type FetchTransport = Transport.FetchTransport

let requireFetchTransport (stub: obj) : FetchTransport = Transport.requireFetchTransport stub

type Environment =
    abstract Probe: Workers.DurableObjectNamespace<obj>

let namedTransport (namespaceBinding: Workers.DurableObjectNamespace<'T>) name =
    let id = namespaceBinding.idFromName name
    let recoveredId = namespaceBinding.idFromString (id.toString())
    if not (id.equals recoveredId) then invalidOp "Durable Object identity round trip failed"
    Transport.getFetchById namespaceBinding recoveredId

let forward (env: Environment) name (request: NativeRequest) =
    (namedTransport env.Probe name).fetch request

let forwardByName (env: Environment) name (request: NativeRequest) =
    (Transport.getFetchByName env.Probe name).fetch request

let applyFrame (storage: Workers.DurableObjectStorage) (frame: Frame) : JS.Promise<Receipt> =
    storage.transaction(fun transaction ->
        async {
            let key = "message:" + frame.messageId
            let! prior = transaction.get<float>(key) |> Async.AwaitPromise
            match prior with
            | Some count -> return {| count = count; duplicate = true |}
            | None ->
                let! current = transaction.get<float>("count") |> Async.AwaitPromise
                let count = Option.defaultValue 0. current + frame.delta
                do! transaction.put("count", count) |> Async.AwaitPromise
                do! transaction.put(key, count) |> Async.AwaitPromise
                return {| count = count; duplicate = false |}
        }
        |> Async.StartAsPromise)

type ProbeDurableObject(ctx: Workers.DurableObjectState<obj>, env: Environment) =
    inherit Runtime.DurableObject<Environment, obj>(ctx, env)

    let mutable initialized = false

    do
        ctx.blockConcurrencyWhile(fun () ->
            async {
                let! _ = ctx.storage.get<float>("count") |> Async.AwaitPromise
                initialized <- true
            }
            |> Async.StartAsPromise)
        |> ignore

    interface Runtime.DurableObject.IFetchHandler with
        member _.fetch request =
            async {
                if not initialized then invalidOp "Fetch preceded initialization"
                if request.``method`` = "POST" && request.url.EndsWith("/frame", StringComparison.Ordinal) then
                    let! frame = request.json<Frame>() |> Async.AwaitPromise
                    if String.IsNullOrWhiteSpace frame.messageId || Double.IsNaN frame.delta || Double.IsInfinity frame.delta then
                        return Workers.Exports.Response.json({| error = "Invalid frame" |}, U2.Case2(Workers.ResponseInit.Create(status = 400.)))
                    else
                        let! receipt = applyFrame ctx.storage frame |> Async.AwaitPromise
                        return Workers.Exports.Response.json receipt
                elif request.``method`` = "POST" && request.url.EndsWith("/alarm", StringComparison.Ordinal) then
                    do! ctx.storage.put("alarm-fired", false) |> Async.AwaitPromise
                    let due = float (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) + 150.
                    do! ctx.storage.setAlarm(U2.Case1 due) |> Async.AwaitPromise
                    return Workers.Exports.Response.json {| scheduled = true |}
                elif request.``method`` = "DELETE" then
                    do! ctx.storage.deleteAlarm() |> Async.AwaitPromise
                    do! ctx.storage.deleteAll() |> Async.AwaitPromise
                    return Workers.Exports.Response.json {| deleted = true |}
                else
                    let! count = ctx.storage.get<float>("count") |> Async.AwaitPromise
                    let! alarms = ctx.storage.get<float>("alarms") |> Async.AwaitPromise
                    let! attempts = ctx.storage.get<float>("alarm-attempts") |> Async.AwaitPromise
                    return Workers.Exports.Response.json {| count = Option.defaultValue 0. count; alarms = Option.defaultValue 0. alarms; alarmAttempts = Option.defaultValue 0. attempts; initialized = initialized |}
            }
            |> Async.StartAsPromise
            |> U2.Case1

    interface Runtime.DurableObject.IAlarmHandler with
        member _.alarm _ =
            async {
                let! failAfterCommit =
                    ctx.storage.transaction(fun transaction ->
                        async {
                            let! attempts = transaction.get<float>("alarm-attempts") |> Async.AwaitPromise
                            do! transaction.put("alarm-attempts", Option.defaultValue 0. attempts + 1.) |> Async.AwaitPromise
                            let! fired = transaction.get<bool>("alarm-fired") |> Async.AwaitPromise
                            if fired <> Some true then
                                let! alarms = transaction.get<float>("alarms") |> Async.AwaitPromise
                                do! transaction.put("alarms", Option.defaultValue 0. alarms + 1.) |> Async.AwaitPromise
                                do! transaction.put("alarm-fired", true) |> Async.AwaitPromise
                            return fired <> Some true
                        }
                        |> Async.StartAsPromise)
                    |> Async.AwaitPromise
                if failAfterCommit then failwith "Intentional probe failure after durable alarm progress"
            }
            |> Async.StartAsPromise
            |> Some

/// Typed facet startup preserves the marker's instance parameter across the generated API.
let facetStartup (classHandle: Workers.DurableObjectClass<'T>) (id: Workers.DurableObjectId) : Workers.FacetStartupOptions<'T> =
    Workers.FacetStartupOptions<'T>.Create(classHandle, id = U2.Case2 id)

let getFacet (facets: Workers.DurableObjectFacets) name (startup: Workers.FacetStartupOptions<'T>) : FetchTransport =
    facets.get<'T>(name, fun () -> U2.Case1 startup) |> requireFetchTransport

let stopFacet (facets: Workers.DurableObjectFacets) name = facets.delete name
