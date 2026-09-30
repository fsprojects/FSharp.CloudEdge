---
title: Analytics Engine
description: Write event measurements and query aggregate results with SQL.
---

<div class="ce-block-head">
<p class="ce-block-lead">Write event measurements and query aggregate results with SQL.</p>
<ul class="ce-facts">
<li><span>Library</span> <code>Runtime.Workers</code></li>
<li><span>npm</span> <code>@cloudflare/workers-types</code> 5.20260906.1</li>
</ul>
</div>

## Events and queries

Workers Analytics Engine collects event data such as route latency, usage, or job outcomes. A Worker writes through an `AnalyticsEngineDataset` binding; readers query the dataset through a separate [SQL HTTP API](https://developers.cloudflare.com/analytics/analytics-engine/sql-api/). It is an analytics service with its own event model and sampling behavior.

Follow the [dataset setup guide](https://developers.cloudflare.com/analytics/analytics-engine/get-started/) to configure the binding and query access. Define what each positional field means before several producers start writing to the dataset.

## Record a request

The caller supplies its dataset binding, tenant identity, route, and measured duration. This convention places the tenant in the sampling index, the route in `blob1`, and elapsed milliseconds in `double1`.

```fsharp
open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers

let recordRequest (dataset: Workers.AnalyticsEngineDataset) (tenant: string) (route: string) (elapsedMs: float) =
    dataset.writeDataPoint(
        Workers.AnalyticsEngineDataPoint.Create(
            indexes = [| Some (U2.Case1 tenant) |],
            blobs = [| Some (U2.Case1 route) |],
            doubles = [| elapsedMs |]))
```

`writeDataPoint` returns immediately; the runtime handles the write in the background. The return value is not an ingestion receipt. Read access uses the SQL API and its credentials separately from the Worker binding.

## Query with sampling in mind

Analytics Engine can sample data on ingestion and querying. Its [`_sample_interval` field](https://developers.cloudflare.com/analytics/analytics-engine/sampling/) supplies the weight for aggregates. For a dataset named `REQUESTS`, this query estimates request count and mean duration by route:

```text
SELECT blob1 AS route,
       SUM(_sample_interval) AS requests,
       SUM(double1 * _sample_interval) / SUM(_sample_interval) AS mean_ms
FROM REQUESTS
WHERE timestamp > NOW() - INTERVAL '1' HOUR
GROUP BY route
```

Choose the sampling index deliberately; using the tenant here allows sampling to reflect that tenant's event volume. These aggregates describe telemetry. Keep application records that require exact transactional updates in a store such as [D1](d1.md).


## Help verify the binding

Useful checks include positional field encoding, dataset configuration, ingestion visibility, and weighted query results. A compiling consumer does not establish hosted behavior. Record the package version, configuration, and observed result using the [verification guide](../../guide/verify-bindings.md).

## Related Pages

- [Data & Analytics](index.md)
- [Account setup](../control-plane/account-setup.md)
- [Worker bindings](../control-plane/worker-upload.md)

## NuGet packages

[Runtime.Workers 0.1.0](https://www.nuget.org/packages/FSharp.CloudEdge.Runtime.Workers/0.1.0).

See [installation and release availability](../../guide/packages.md).
