# Scenario 01 Guidance & Costs

This document centralizes guidance and rough cost considerations for the baseline scenario (Store + Products + Semantic Search via direct API or Azure Function) using SQL Server 2025 vector search.

> IMPORTANT: This is not a pricing quote. Always validate with the official Azure/AWS/on‑prem calculators and your licensing program.

## Components

| Component | Notes | Cost Drivers |
| --------- | ----- | ------------ |
| .NET Aspire AppHost | Orchestration / dev-time only or lightweight host | Minimal (dev) |
| Store (Blazor) | Front-end service | App Service / Container App / compute SKU |
| Products API | Keyword + direct semantic search | Same as Store; CPU for embeddings generation |
| SemanticSearchFunction | Azure Function semantic endpoint | Consumption (GB-s), executions, cold starts |
| SQL Server 2025 | Vector index + data storage | vCore / DTU or license-per-core; storage; high availability |
| Embeddings Model | Depending on provider (Azure OpenAI etc.) | Tokens generated for each semantic query & initialization |

## Two semantic paths – trade-offs

| Path | Pros | Cons |
| ---- | ---- | ---- |
| Direct (Products API) | Lower latency, fewer hops, simpler deploy | Couples semantic logic to Products release cycle |
| Azure Function | Independent scaling, can be secured/exposed separately, polyglot flexibility | Extra network hop, potential cold start latency |

## Cost Awareness Tips

1. Batch embedding generation when initializing product catalog to reduce per-item overhead.
2. Cache frequently used embedding vectors (e.g., for popular queries) if model/provider billing justifies it.
3. Monitor SQL vector index size and adjust dimension or precision if storage grows unexpectedly.
4. If Function cold starts impact UX, consider premium plan or pre-warming strategies.
5. Use logging/metrics (OpenTelemetry) to profile latency distribution among: embedding generation, SQL vector distance evaluation, network hops.

## Operational Guidance

- Observability: integrate distributed tracing across Store -> Products -> Function (if used) -> SQL to isolate bottlenecks.
- Resilience: use retry policies (Aspire HttpClient defaults) for transient failures on Function / Products.
- Security: prefer managed identity / secure secrets store for DB connection strings; restrict Function exposure (auth keys / APIM) if required.
- Scaling: monitor query rate and memory usage of SQL vector index; allocate cores accordingly.

## When to Pick Each Semantic Mode

| Situation | Recommended Mode |
| --------- | ---------------- |
| Lowest latency required | Direct Products Semantic |
| Need independent release cadence | Azure Function |
| Expect spiky, unpredictable demand | Azure Function (consumption can auto-scale) |
| Want to consolidate logic | Direct Products Semantic |

## Next Steps / Enhancements

- Add a background job to update embeddings when product descriptions change.
- Introduce hybrid search: combine keyword + vector scoring.
- Implement result scoring explainability (show vector distance).
- Add automated load test to compare cost per 1K queries for both paths.

---

Return to [README](../README.md).
