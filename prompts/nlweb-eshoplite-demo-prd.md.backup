# Product Requirements Document (PRD)

## Project: NLWeb eShopLite Search (C# + NLWeb)

Version: 1.0  
Solution: `src\eShopLite-NLWeb.slnx`  
Target stack: .NET 9, .NET Aspire 9.4  
Authoring source: `prompts/nlweb-eshoplite-demo.md`

## 1. Overview

Build a new eShopLite search experience in C# that uses NLWeb to search across the website’s rendered content (product pages, categories, FAQs/marketing pages). Do not use the existing eShopLite Semantic Search scenario (no embeddings/vector DB from the legacy path). The deliverable is a production-quality demo slice with clear contracts, tests, and telemetry.

References:

- NLWeb blog: <https://news.microsoft.com/source/features/company-news/introducing-nlweb-bringing-conversational-interfaces-directly-to-the-web>
- NLWeb repo: <https://github.com/nlweb-ai/NLWeb>
- eShopLite sample: <https://github.com/Azure-Samples/eShopLite>

## 2. Goals and Non‑Goals

Goals:

- Natural-language, site-wide search for eShopLite using NLWeb.
- Clear C# Search API with normalized results, used by the Store front-end.
- Basic reindex capability and minimal observability.

Non‑Goals:

- No reuse of the legacy Semantic Search implementation, vector DB, or embeddings pipelines.
- No cart/checkout changes beyond navigation from search results.

## 3. Scope & Constraints

- Language/Framework: C#/.NET 9
- Orchestration: .NET Aspire 9.4; enroll all services
- Working solution: `src\eShopLite-NLWeb.slnx`
- NLWeb is the retrieval engine; use its API/SDK to query and index website content

## 4. Personas & User Stories

- Shopper: “As a shopper, I want to search the site in natural language so I can find products and policies quickly.”
  - AC: P50 < 2s, relevant results, clickable links to detail pages.
- Support agent: “As support, I need quick answers from policy/FAQ pages to help customers.”
  - AC: Authoritative pages with citations/links.
- Merchandiser: “As merchandiser, I want visibility into zero-result queries to improve content.”
  - AC: Zero-results metric available for analysis.

## 5. KPIs & Success Metrics

- Latency: P50 < 2s, P95 < 4s (local/demo)
- Error rate: < 1% 5xx on search API
- Zero-results rate: tracked, target reduction
- CTR on results: clicks per query
- Uptime (demo SLO): 99% during demo windows
- Index freshness: within 24h of content changes (demo SLA)

## 6. Solution Architecture

Components (under `src/`):

- Search API (new) — `Search/`
  - Role: Own the search contract; call NLWeb.
  - Endpoints: `GET /api/v1/search`, `POST /api/v1/search/reindex` (protected)
  - Tech: ASP.NET Core Minimal API (or MVC) on .NET 9; typed HttpClient
  - Observability: structured logs, OTEL traces/metrics via Aspire service defaults
- Store (existing) — `Store/`
  - Add search box and `/search` page
  - Calls Search API; renders titles/snippets/links
- App Host (existing) — `eShopAppHost/`
  - Orchestrate services with Aspire 9.4; use service discovery
- Service Defaults — `eShopServiceDefaults/`
  - Shared Aspire configuration: service discovery, resiliency, health checks, OTEL

Aspire requirements:

- Enroll Search, Store, and AppHost into Aspire 9.4
- Use Aspire service discovery for Store → Search calls (no hard-coded URLs)
- Centralize logging, tracing, and metrics via Aspire defaults
- Expose readiness/liveness endpoints for diagnostics

## 7. Detailed API Contracts

Base path versioning: `/api/v1/`

1. Search

- Method/Route: GET `/api/v1/search`
- Query params:
  - `q` (string, required)
  - `top` (int, optional; default 10; 1–50)
  - `skip` (int, optional; default 0; ≥ 0)
- Response 200:

```jsonc
{
  "query": "running shoes under $60",
  "count": 42,
  "results": [
    {
      "title": "Men's Running Shoes – Budget Picks",
      "url": "/products/123",
      "snippet": "Lightweight trainers ideal for daily runs...",
      "score": 0.87,
      "metadata": { "category": "running", "price": 59.99 }
    }
  ]
}
```

- Errors (JSON): `{ "code": string, "message": string, "correlationId": string }`
  - 400 validation, 429 rate limit, 502 NLWeb failure, 500 server error
- Rate-limit headers: `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`

1. Reindex

- Method/Route: POST `/api/v1/search/reindex`
- Auth: bearer/API key (configurable via environment)
- Behavior: trigger NLWeb (re)crawl or sitemap import for `SiteBaseUrl`
- Response 202/200 + operation info; log progress; optional status endpoint (future)

## 8. Indexing Strategy

- Sources: `SiteBaseUrl` and sitemap
- Exclusions: admin/cart/checkout paths
- Robots: honor `robots.txt`
- Dynamic content: prefer rendered HTML where feasible
- Trigger: manual reindex via POST; scheduled job (future)
- Retention: reindex replaces prior state

## 9. Security & Privacy

- Input validation; enforce `MaxTop` and query length limits
- Strict CORS to Store origin
- Secrets in env/user-secrets; never commit secrets
- Abuse control: basic rate limiting and anomaly logs
- Output encoding; sanitize snippets as needed

## 10. Accessibility & UX

- WCAG 2.2 AA: keyboard navigation, focus states, screen-reader labels
- States: loading, empty, error; actionable guidance when empty
- Snippet highlighting and truncation (~160 chars)

## 11. Observability

- Metrics: req/sec, P50/P95 latency, error rate, zero-results, CTR
- Logs: structured with `correlationId`, NLWeb call duration/status, input size (bounded)
- Tracing: spans for API handler, NLWeb client, Store→Search calls (OTEL via Aspire)
- Dashboard: basic charts for latency, errors, zero-results, CTR

## 12. Testing & Quality Gates

- Unit tests: handlers, validators, mappers (Search)
- Integration test: `/api/v1/search` happy path with fake NLWeb client
- Load test: 10 concurrent users for 5 minutes; meet latency/error thresholds
- Chaos: NLWeb timeouts/5xx simulated; graceful fallback verified
- Exit criteria: all tests green; KPIs within thresholds

## 13. Environments & Rollout

- Local: Aspire 9.4 orchestrates services; service discovery resolves endpoints; single-command startup
- CI: build/test gates; artifacts packaged as needed
- Feature flag for new search UI; rollback by toggling flag

## 14. Risks, Assumptions, Open Questions

Risks:

- NLWeb rate limits/quotas → client rate limiting and optional caching

Assumptions:

- Stable product URLs and a complete sitemap
- NLWeb API/SDK provides site-scoped query and indexing

Open Questions:

- Anchor-level deep links available from NLWeb results?

## 15. Timeline & Ownership

Roles: Engineering (Search/Store), PM (requirements/KPIs), QA (tests), Ops (Aspire orchestration).

Phases (from prompt):

1. Document hardening — finalize prompt/PRD
2. Search API skeleton — project scaffolding, config, health, empty endpoint
3. NLWeb integration — `INlWebClient`, search happy path (fake then real)
4. Store UI wiring — header search box, `/search` page, render results
5. Reindex + hardening — `POST /api/v1/search/reindex`, CORS/rate limit/validation, logs/metrics
6. Tests & docs — unit/integration tests, README updates

## 16. Dependencies

- .NET 9 SDK; .NET Aspire 9.4 workloads
- NLWeb endpoint/API key
- Existing eShopLite Store site content

## 17. Acceptance Criteria (DoD)

- Search queries return titles/snippets/links; navigation works
- P50 < 2s, P95 < 4s locally for 10 concurrent users
- Resilience: NLWeb failures show helpful fallback; no unhandled exceptions
- Security: CORS restricted; input validation; rate limiting
- Quality: unit tests present; at least one integration test for search endpoint

## 18. Deliverables

- `Search/` service with endpoints/config/tests (C#/.NET 9)
- Store UI: search box and `/search` page
- Aspire AppHost updates for orchestration and service discovery
- Documentation: run/config steps (README), metrics/logging notes

## 19. Implementation Guide for Copilot Coding Agent

Use `src\eShopLite-NLWeb.slnx` and enroll all services into Aspire 9.4.

Step-by-step tasks:

1. Create `Search/` project (.NET 9). Reference `eShopServiceDefaults`. Add `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`.
2. Implement `GET /api/v1/search`: validate inputs, call `INlWebClient.QueryAsync`, map results, add telemetry.
3. Implement `POST /api/v1/search/reindex`: protected route; call `INlWebClient.ReindexAsync`.
4. Add typed `HttpClient` with resilience/timeouts; wire OpenTelemetry via Aspire defaults.
5. Store: add search box and `/search` page; call `/api/v1/search` using service discovery (no hard-coded URL).
6. Tests: unit tests for validators/mappers/handlers; integration test with fake NLWeb client.
7. Aspire: update `eShopAppHost` to add the Search project and service wiring; enable service discovery and telemetry.

Edge cases to cover:

- Empty/overly long `q`; `top` out of range; large `skip`
- NLWeb timeout/5xx; return 502 with error body; preserve correlationId
- Zero results; return `count: 0` and empty array with helpful message

## 20. Glossary & References

- NLWeb: natural language web interaction/search framework
- Aspire: .NET application composition and orchestration with service discovery and OTEL
- PRD: Product Requirements Document
