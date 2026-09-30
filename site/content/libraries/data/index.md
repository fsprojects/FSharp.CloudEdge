---
title: Data & Analytics
description: Choose a data service by its records, access patterns, and lifecycle.
---

Data services deserve their own design decisions: schema, keys, consistency, retention, and access. FSharp.CloudEdge exposes their Worker-side APIs through `Runtime.Workers`, while management clients configure the underlying resources. Sharing an assembly does not make their lifecycles interchangeable.

| Service | Data and access pattern | Dedicated page |
| --- | --- | --- |
| D1 | SQL records, prepared queries, transactional batches | [D1](d1.md) |
| R2 | Object bodies, streams, and metadata | [R2](r2.md) |
| Workers KV | Distributed key-value reads | [KV](kv.md) |
| Vectorize | Vectors, metadata filters, similarity retrieval | [Vectorize](vectorize.md) |
| Analytics Engine | Event measurements and sampled SQL aggregates | [Analytics Engine](analytics-engine.md) |

The [Hybrid Search recipe](../hybrid-search.md) combines D1 and Vectorize with an embedding model. [Artifacts](../agents/artifacts.md) has its own treatment for Git-compatible repositories and versioned file trees.

Start with [Account Setup](../control-plane/account-setup.md) to see management clients create resources, then [Worker Upload](../control-plane/worker-upload.md) for environment bindings. Each service page calls out behaviors that need runtime verification; [community reports](../../guide/verify-bindings.md) help mature those bindings.
