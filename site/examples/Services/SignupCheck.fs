module SignupCheck

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module Cabidela = FSharp.CloudEdge.Runtime.Cabidela

let schema =
    {| ``type`` = "object"
       required = [| "email" |]
       properties =
        {| email = {| ``type`` = "string"; minLength = 3 |}
           plan = {| ``type`` = "string"; enum = [| "free"; "team" |]; ``default`` = "free" |} |} |}

let signup = Cabidela.Exports.Cabidela(schema, Cabidela.CabidelaOptions.Create(applyDefaults = true))

[<ExportDefault>]
let worker: Workers.ExportedHandler<obj, obj, obj, obj> =
    Workers.ExportedHandler.Create(
        fetch = fun request _ _ ->
            async {
                try
                    let! form = request.json<obj>() |> Async.AwaitPromise
                    signup.validate form |> ignore
                    return Workers.Exports.Response.json form
                with error ->
                    let status = Workers.ResponseInit.Create(status = 400.)
                    return Workers.Exports.Response.json({| error = error.Message |}, U2.Case2 status)
            }
            |> Async.StartAsPromise
            |> U2.Case1
    )
