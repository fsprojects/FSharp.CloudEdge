module RequestMetrics

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

let recordRequest (dataset: Workers.AnalyticsEngineDataset) (tenant: string) (route: string) (elapsedMs: float) =
    dataset.writeDataPoint(
        Workers.AnalyticsEngineDataPoint.Create(
            indexes = [| Some (U2.Case1 tenant) |],
            blobs = [| Some (U2.Case1 route) |],
            doubles = [| elapsedMs |]))
