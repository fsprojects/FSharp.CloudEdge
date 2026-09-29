module Pricing

open Fable.Core

module Runtime = FSharp.CloudEdge.Runtime.Workers.Cloudflare.Workers

[<AttachMembers>]
type PricingApi() =
    inherit Runtime.RpcTarget()

    member _.plans() = [| "starter"; "team"; "business" |]

    member _.quote(plan: string, seats: float) =
        let perSeat =
            match plan with
            | "team" -> 8.
            | "business" -> 15.
            | _ -> 0.
        {| plan = plan; seats = seats; monthly = perSeat * seats |}
