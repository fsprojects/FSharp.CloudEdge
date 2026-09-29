module PageViewReport

open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Management.Observability

let topPages (observability: ObservabilityClient) accountId =
    task {
        let sql =
            "SELECT blob1 AS path, SUM(_sample_interval) AS views FROM page_views "
            + "WHERE timestamp > NOW() - INTERVAL '7' DAY "
            + "GROUP BY path ORDER BY views DESC LIMIT 10 FORMAT JSON"
        match! observability.AnalyticsEngineSqlQueryPost(accountId, sql) with
        | AnalyticsEngineSqlQueryPost.OK report ->
            for row in report.data do
                printfn "%O  %O" row["path"] row["views"]
        | AnalyticsEngineSqlQueryPost.BadRequest message ->
            printfn "Query rejected: %s" message
        | other ->
            printfn "%A" other
    }
