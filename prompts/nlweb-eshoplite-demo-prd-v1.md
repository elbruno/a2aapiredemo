# Product Requirements Document (PRD)

## Project: NLWeb eShopLite Search (C# + NLWeb)

Version: 1.0  
Solution: `src\eShopLite-NLWeb.slnx`  
Target stack: .NET 9, .NET Aspire 9.4  
Authoring source: `prompts/nlweb-eshoplite-demo.md`

## 1. Overview

Build a new eShopLite search experience in C# that uses NLWeb to search across the website’s rendered content (product pages, categories, FAQs/marketing pages). The Search API **must be implemented using the official NLWebNet library** (<https://github.com/nlweb-ai/nlweb-net>), the .NET 9 implementation of the NLWeb protocol. Do not use the existing eShopLite Semantic Search scenario (no embeddings/vector DB from the legacy path). The deliverable is a production-quality demo slice with clear contracts, tests, and telemetry.

**Chat Implementation Note:**
The chat experience is now fully HTTP-only. All SignalR dependencies, code, and documentation have been removed. Chat and search requests are handled via RESTful HTTP endpoints. NLWebNet is integrated for conversational and search capabilities, exposing `/ask` and `/mcp` endpoints in addition to the custom `/api/v1/chat/message` endpoint for Store compatibility.

References:

- NLWebNet (.NET 9 implementation): <https://github.com/nlweb-ai/nlweb-net>
- NLWebNet NuGet: <https://www.nuget.org/packages/NLWebNet/>
- NLWebNet demo-setup-guide: <https://github.com/nlweb-ai/nlweb-net/blob/main/doc/demo-setup-guide.md>
- NLWebNet manual-testing-guide: <https://github.com/nlweb-ai/nlweb-net/blob/main/doc/manual-testing-guide.md>
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
  - Role: Own the search contract; call NLWeb using the NLWebNet library.
  - Endpoints: `GET /api/v1/search`, `POST /api/v1/search/reindex` (protected)
  - Tech: ASP.NET Core Minimal API (or MVC) on .NET 9; integrate NLWebNet endpoints and services
  - Implementation: Use NLWebNet NuGet package for protocol compliance and MCP integration
  - Observability: structured logs, OTEL traces/metrics via Aspire service defaults
  - Reference: See NLWebNet documentation and sample applications for integration patterns
  - **Data Loading:** The Search service must load and query real website data using NLWeb's ingestion and indexing capabilities. Mock clients are not permitted; all queries must use the actual NLWebNet integration and real indexed data.

- Store (existing) — `Store/`
  - Add search box and `/search` page
  - Calls Search API; renders titles/snippets/links
  - Add static "About Us" page with sample information about Contoso (see below)
  - Add static "Careers" page with sample job offerings for Contoso (see below)

- App Host (existing) — `eShopAppHost/`
  - Orchestrate services with Aspire 9.4; use service discovery
  - **NLWeb Docker Container Resource:** Add a new resource to Aspire orchestration representing the NLWeb Docker container. This resource must:
    - Be orchestrated and discoverable via Aspire service discovery
    - Expose health/readiness endpoints
    - Support configuration via environment variables for AI backends (Ollama, Azure OpenAI)
    - Mount host directories for `/data` (persisted knowledge base) and `/config` (read-only config)
    - Use official NLWeb Docker image and documented startup commands
    - Support in-memory vector DB analysis for rapid prototyping and evaluation
    - Document how to load real website data into NLWeb using the documented data loading command (e.g., `docker exec -it <container_id> python -m data_loading.db_load <url> <name>`)

- Service Defaults — `eShopServiceDefaults/`
  - Shared Aspire configuration: service discovery, resiliency, health checks, OTEL

**Service Defaults:**
All HTTP calls, timeouts, service discovery, and resiliency options must use .NET Aspire service defaults. Do not hard-code values; always use Aspire configuration and extension methods for best practices.

Aspire requirements:

- Enroll Search, Store, AppHost, and NLWeb Docker resource into Aspire 9.4
- Use Aspire service discovery for Store → Search and Search → NLWeb calls (no hard-coded URLs)
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

## 8. Indexing & Data Loading Strategy

- Sources: `SiteBaseUrl` and sitemap
- Exclusions: admin/cart/checkout paths
- Robots: honor `robots.txt`
- Dynamic content: prefer rendered HTML where feasible
- Trigger: manual reindex via POST; scheduled job (future)
- Retention: reindex replaces prior state
- **Real Data Loading:** All data indexed by NLWeb must be loaded from the live website or static content pages (e.g., About Us, Careers) using NLWeb's documented ingestion commands. Mock data or hard-coded results are not permitted in production or demo scenarios.
- **Data Loading Command:** Use the documented NLWeb Docker command to ingest site data:
  - `docker exec -it <container_id> python -m data_loading.db_load <url> <name>`
  - For Docker Compose: `docker-compose exec nlweb python -m data_loading.db_load <url> <name>`
- **Volume Mounts:** Persist data using `/data` volume; provide config via `/config` (read-only).
- **Vector DB Support:** NLWeb supports in-memory vector DB for rapid prototyping, as well as external vector DBs (Qdrant, Milvus, Azure AI Search, etc.). For demo scenarios, in-memory vector DB may be used, but production should use a persistent backend.

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
- Store UI: static `/about-us` page with sample Contoso company information
- Store UI: static `/careers` page with sample job offerings for Contoso
- Aspire AppHost updates for orchestration and service discovery
- Documentation: run/config steps (README), metrics/logging notes

## 19. Implementation Guide for Copilot Coding Agent

Use `src\eShopLite-NLWeb.slnx` and enroll all services into Aspire 9.4.

Step-by-step tasks:

1. Create `Search/` project (.NET 9). Reference `eShopServiceDefaults`. Add `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`.
2. Implement `GET /api/v1/search`: validate inputs, call NLWebNet endpoint (`/ask`) using NLWebNet library, map results, add telemetry. **All queries must use real indexed data from NLWeb; mock clients are not permitted.**
3. Implement `POST /api/v1/search/reindex`: protected route; call NLWebNet reindex logic as per protocol.
4. Integrate NLWebNet NuGet package and configure endpoints/services according to documentation and sample code.
5. Store: add search box and `/search` page; call `/api/v1/search` using service discovery (no hard-coded URL).
6. Tests: unit tests for validators/mappers/handlers; integration test with NLWebNet and real data.
7. Aspire: update `eShopAppHost` to add the Search project, NLWeb Docker resource, and service wiring; enable service discovery and telemetry.
8. Orchestrate NLWeb Docker container in Aspire:
   - Use official NLWeb Docker image
   - Pass environment variables for AI backend selection (Ollama, Azure OpenAI)
   - Mount `/data` and `/config` volumes
   - Expose health/readiness endpoints
   - Document and automate data loading using NLWeb's ingestion command
   - Support in-memory vector DB for rapid prototyping; document how to switch to persistent DBs for production

**Reference Implementation Details:**

- NLWebNet provides minimal API endpoints (`/ask`, `/mcp`) for natural language queries and MCP integration.
- Add NLWebNet to your ASP.NET Core project:

  ```csharp
  using NLWebNet;
  builder.Services.AddNLWebNet(options =>
  {
      options.DefaultMode = NLWebNet.Models.QueryMode.List;
      options.EnableStreaming = true;
  });
  app.MapNLWebNet();
  ```

- Configure AI/data backends and secrets as per NLWebNet and NLWeb Docker documentation. For Docker, use environment variables such as:
  - `OPENAI_API_KEY`, `AZURE_OPENAI_API_KEY`, `AZURE_VECTOR_SEARCH_ENDPOINT`, etc.
  - See `.env.template` in NLWeb repo for all options.
- Load real website data into NLWeb using documented commands (see above).
- See <https://github.com/nlweb-ai/nlweb-net/blob/main/doc/demo-setup-guide.md> and <https://github.com/nlweb-ai/NLWeb/blob/main/docs/setup-docker.md> for step-by-step setup.

**Note:** NLWebNet is alpha-quality and intended for prototyping and evaluation. Production use is not recommended at this time.
Edge cases to cover:

- Empty/overly long `q`; `top` out of range; large `skip`
- NLWeb timeout/5xx; return 502 with error body; preserve correlationId
- Zero results; return `count: 0` and empty array with helpful message

## 20. Glossary & References

- NLWeb: natural language web interaction/search framework
- Aspire: .NET application composition and orchestration with service discovery and OTEL
- PRD: Product Requirements Document

## 21. NLWeb Implementation Document Requirement

A separate document must be created to describe the specific implementation of NLWeb in this sample scenario. This document should include:

- Integration steps for NLWebNet in the Search and Chat APIs
- Configuration details (appsettings, secrets, environment variables)
- Endpoint mapping and usage patterns (including `/ask`, `/mcp`, and `/api/v1/chat/message`)
- Data backend and AI service setup (Azure OpenAI, Azure Search, etc.)
- Security, observability, and deployment notes
- Documentation updates reflecting HTTP-only and NLWebNet usage
- References to sample code and documentation

## 22. Static Content Requirements

The following static pages must be added to the Store front-end. Their content will be indexed by NLWeb and used to answer user questions in chat and search:

### About Us Page (`/about-us`)

Create a static page with sample information about the Contoso company. Example content:

> **About Contoso**
> Contoso is a leading retailer of electronics, apparel, and home goods. Founded in 1985, Contoso is committed to providing quality products and exceptional customer service. Our mission is to make shopping easy and enjoyable for everyone. With over 200 stores worldwide and a robust online presence, Contoso continues to innovate in the retail space.

### Careers Page (`/careers`)

Create a static page with sample job offerings at Contoso. Example content:

> **Careers at Contoso**
> Join our team and help shape the future of retail! Current openings:
>
> - Software Engineer (Remote)
> - Store Manager (New York)
> - Marketing Specialist (London)
> - Customer Support Representative (Remote)
> - Warehouse Associate (Berlin)
> We offer competitive salaries, flexible work arrangements, and opportunities for growth. Apply today to become part of the Contoso family.

See NLWebNet [demo-setup-guide](https://github.com/nlweb-ai/nlweb-net/blob/main/doc/demo-setup-guide.md) and [manual-testing-guide](https://github.com/nlweb-ai/nlweb-net/blob/main/doc/manual-testing-guide.md) for examples.

---

**Latest Implementation Summary (August 2025):**

- SignalR is fully removed from all code, configuration, and documentation.
- Chat and search are HTTP-only, using RESTful endpoints.
- NLWebNet is integrated in the Chat service, exposing `/ask` and `/mcp` endpoints, with configuration bound from `appsettings.json`.
- The custom `/api/v1/chat/message` endpoint is preserved for Store compatibility.
- Documentation (`docs/api/chat-api.md`) is updated to reflect HTTP-only and NLWebNet usage, with example requests and configuration notes.
- Repository hygiene: deprecated hub files are deleted, and all SignalR references are purged.
- All tests and builds pass; KPIs and requirements are met.
