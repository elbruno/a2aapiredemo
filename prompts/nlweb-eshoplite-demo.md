# NLWeb eShopLite Demo Scenario

## Overview

This document is a single "big prompt" that instructs an implementation agent to build a new eShopLite search experience in C# that uses NLWeb to search across the website’s rendered content (product pages, category listings, FAQs/marketing pages). It intentionally avoids eShopLite’s existing Semantic Search scenario—no embeddings/vector stores or the prior semantic-search code path. The result should be a production-ready demo slice with clear contracts, tests, and telemetry that can later be formalized into a Product Requirements Document (PRD).

Key references:

- NLWeb blog: <https://news.microsoft.com/source/features/company-news/introducing-nlweb-bringing-conversational-interfaces-directly-to-the-web>
- NLWeb repo: <https://github.com/nlweb-ai/NLWeb>
- eShopLite (base sample): <https://github.com/Azure-Samples/eShopLite> (do not reuse the Semantic Search scenario implementation)

Scope constraints:

- Use C#/.NET 9 for the search service and UI wiring in eShopLite.
- Use NLWeb to perform site-content search; do not rely on the existing eShopLite Semantic Search.
- Keep infra minimal for local/demo runs; later cloud deploy can be planned separately.
- Working solution: `src\eShopLite-NLWeb.slnx`.
- Use Aspire 9.4 for local app hosting/integration via AppHost where applicable.

Non-goals (for this demo):

- No vector DB, embeddings pipelines, or OpenAI-based semantic search from the existing scenario.
- No cart/checkout changes beyond linking search results to existing product pages.

Success criteria:

- A user can type a natural-language query in the Store UI and receive ranked results from site content via NLWeb in under ~2 seconds P50 locally, with clear snippets and deep links.
- Clear separation of concerns: a dedicated C# Search API integrates with NLWeb; the Store calls that API.
- Basic automated tests and diagnostics exist to ensure reliability and troubleshootability.

## Phased plan (small tasks)

1. Document hardening — Normalize Markdown formatting and finalize this prompt (current step).
1. Search API skeleton — Create `Search/` .NET project, config binding, health endpoint, and empty `/api/search`.
1. NLWeb client integration — Implement `INlWebClient` and wire `GET /api/search` happy path with mock/fake first, then real.
1. Store UI wiring — Add header search box and `/search` page in `Store/`, call API, render results + empty/error states.
1. Reindex + hardening — Implement `POST /api/search/reindex`, add rate limit/CORS/validation, structured logs and minimal metrics.
1. Tests and docs — Unit tests (NLWeb client fakes) + 1 integration test; update README with run instructions.

## Objective

Deliver a natural-language, site-wide search for eShopLite powered by NLWeb:

- Users ask questions like "show budget running shoes under $60" or "what is the return policy?"
- The system queries NLWeb against the live website (or a pre-indexed mirror) and returns relevant page sections with titles, snippets, and URLs.
- Results render in the Store front-end with filters and quick navigation to product detail pages.

Functional requirements:

- Search endpoint in C# that proxies/coordinates NLWeb queries and normalizes results.
- Website content coverage: product catalog pages, category pages, and general content pages included in the Store.
- Query features: support free-form natural language and keyword queries; optional top-K, offset/pagination.
- Result shape: title, url, snippet/excerpt, score, and optional facets (category, price range if available).
- Freshness: provide an endpoint to trigger (re)index of site content via NLWeb or to load from sitemap.

Non-functional requirements:

- Performance: P50 < 2s, P95 < 4s (local/dev), for 10 concurrent users.
- Reliability: graceful degradation when NLWeb is unreachable; return a helpful message.
- Security: input validation, rate limiting, and CORS set to Store origin.
- Observability: structured logs, minimal trace spans for NLWeb calls, and basic metrics (requests, latency, errors).

Assumptions:

- NLWeb provides a callable API or SDK to submit a query against a configured site scope and to manage indexing; placeholders are provided here—replace with the actual NLWeb SDK/HTTP contracts once confirmed.
- The Store project already exposes public product pages with stable URLs.

## Solution Structure

Projects/components (suggested within `src/`):

- Search API (new): `Search/`
  - Purpose: own the search contract and call NLWeb.
  - Exposes: `GET /api/search?q=...&top=...&skip=...`, `POST /api/search/reindex`.
  - Configuration: `NLWEB_ENDPOINT`, `NLWEB_API_KEY` (or equivalent), `SEARCH_ALLOWED_HOSTS`.
  - Implementation: .NET 9 Minimal API or ASP.NET Core with typed HTTP client.
  - Tests: `Search.Tests/` with unit/integration tests using NLWeb client fakes.
- Store (existing): `Store/`
  - Add a search box + results page (`/search`).
  - Calls the Search API, displays ranked results with snippets and links.
  - Optional facets (if derivable from URLs or metadata).
- eShopAppHost (existing): `eShopAppHost/`
  - Wire the new Search service into the app host profile (Aspire 9.4) if AppHost is used locally.

Key contracts:

- Request: `GET /api/search`
  - Query params: `q` (required), `top` (default 10), `skip` (default 0)
- Response:

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

Configuration model (appsettings):

```jsonc
{
 "Search": {
  "NlWeb": {
   "Endpoint": "https://<nlweb-host>/api",
   "ApiKey": "<secret>",
   "SiteBaseUrl": "https://localhost:*****" // the Store base URL to search
  },
  "AllowedHosts": ["https://localhost:*****"],
  "DefaultTop": 10,
  "MaxTop": 50,
  "TimeoutSeconds": 8
 }
}
```

## Orchestration Flow

High-level sequence:

1. User types a query in Store’s search box and submits.
2. Store calls `GET /api/search?q=...` on the Search API.
3. Search API validates input, applies defaults, and calls NLWeb. If needed, it constrains scope to `SiteBaseUrl` and/or sitemap and passes user query with retrieval options (top/skip, language, etc.).
4. NLWeb returns ranked results from the website content.
5. Search API normalizes the payload, computes facets (optional), and returns JSON.
6. Store renders results with title/snippet/link; clicking opens the target page or scrolls to anchor if provided.

Reindex flow:

1. Operator calls `POST /api/search/reindex` (authorized) or runs a scheduled job.
2. Search API triggers NLWeb to (re)crawl or re-import the sitemap.
3. Status is logged; optional progress endpoint can be added if NLWeb supports it.

## Implementation Tasks

Back-end (Search API):

1. Create `Search` .NET 9 project (Minimal API or ASP.NET Core).
2. Add configuration binding for `Search:NlWeb` and HTTP client factory with resilient policies (timeouts/retries).
3. Implement `GET /api/search` with input validation, rate limiting, and CORS.
4. Implement `POST /api/search/reindex` (authorized) to trigger NLWeb indexing (sitemap or site-crawl).
5. Create `INlWebClient` abstraction with methods like `QueryAsync(query, top, skip)` and `ReindexAsync()`.
6. Map NLWeb response to the standardized `SearchResult` DTOs (title, url, snippet, score, metadata).
7. Add logging, tracing spans around NLWeb calls, and minimal metrics (requests, latency, failures).
8. Add unit tests with NLWeb client fakes and an integration test for the search endpoint.

Front-end (Store):

1. Add a search box in the header and a `/search` page/view to display results.
2. Call the Search API with `q`, `top`, `skip`; handle loading, empty states, and errors.
3. Render result title, snippet, URL; ensure links preserve navigation to products.
4. Optional: add simple filters (e.g., category) if metadata is available.

Indexing/content:

1. Provide a configuration for `SiteBaseUrl` and optionally a sitemap URL.
2. Implement the reindex endpoint to call NLWeb’s indexing capability.
3. Document a local-dev path (e.g., run Store, then call reindex to capture content).

Security and governance:

1. Validate query inputs (length, allowed chars), enforce `MaxTop`, and set timeouts.
2. Enable CORS for the Store origin only. Add basic rate limiting.
3. Store NLWeb secrets outside source control (user-secrets or env vars).

Observability and quality:

1. Structured logs for request start/end and NLWeb responses (without sensitive data).
2. Add a simple health endpoint and a dependency check against NLWeb.
3. Tests: happy path, NLWeb error/timeout, empty results, large results (pagination).

Dev experience:

1. Provide `README` updates with run instructions and configuration samples.
2. Optionally wire `Search` into `eShopAppHost` for one-command startup (Aspire 9.4 AppHost).
3. Ensure .NET 9 SDK and Aspire 9.4 workloads are installed in the dev environment.

Out of scope for this iteration:

- Advanced ranking/faceting beyond what NLWeb returns or is trivially derivable from URLs/metadata.
- Chat-style multi-turn reasoning or agentic browsing (see Future Scenarios).

## Expected Outcome

Demo capabilities:

- A new search box and results page in Store.
- A C# Search API that queries NLWeb and returns normalized results.
- Simple reindex capability to keep content fresh.
- Tests, logs, and minimal metrics for confidence.

Acceptance criteria:

- Functional: Queries return relevant titles/snippets/links; clicking navigates correctly.
- Performance: P50 < 2s, P95 < 4s for local/demo.
- Resilience: NLWeb failures return a fallback message; no unhandled exceptions.
- Security: CORS restricted, basic rate limiting, input validation enforced.
- Quality: Unit tests for core logic; one integration test for the endpoint.

Deliverables:

- `Search/` service with endpoints, config, and tests.
- Store UI changes with `/search` page.
- Documentation: updated root `README` or service-specific `README` with local run steps.

## Additional NLWeb Scenario Ideas (future)

These are not part of the current demo but are suggested evolutions that leverage NLWeb:

1. Conversational shopping assistant: NLWeb-powered assistant embedded in the Store to refine intent (budget, size, style) and deep-link to products.
2. Policy and helpdesk Q&A: instant answers from policy/FAQ pages, with citations and page anchors.
3. Guided onboarding tours: NLWeb drives in-page highlights and navigation for new users.
4. Accessibility voice mode: voice queries to navigate and search the site hands-free.
5. Content gap analysis: periodically query NLWeb for common unanswered questions and feed a content backlog.
6. Multi-lingual search: detect locale and prompt NLWeb for language-aware retrieval.
7. A/B testing of prompts: experiment with different NLWeb query templates and measure CTR/engagement.

Notes:

- Ensure all future features avoid the legacy Semantic Search path unless the PRD explicitly calls for a comparison study.
- Treat NLWeb as the primary search/retrieval engine for site content in this track.
