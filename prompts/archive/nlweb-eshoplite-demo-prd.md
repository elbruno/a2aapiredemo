# Product Requirements Document (PRD)

## Project: NLWeb eShopLite Search & Chat Platform (C# + NLWeb)

Version: 2.0  
Solution: `src\eShopLite-NLWeb.slnx`  
Target stack: .NET 9, .NET Aspire 9.4  
Authoring source: `prompts/nlweb-eshoplite-demo.md`

## 1. Overview

Build a comprehensive eShopLite platform in C# that uses NLWeb for both search and conversational chat experiences across the website's rendered content (product pages, categories, FAQs/marketing pages). The platform includes advanced features like real-time chat assistance, automated screenshot documentation, and cloud deployment capabilities. Do not use the existing eShopLite Semantic Search scenario (no embeddings/vector DB from the legacy path). The deliverable is a production-quality demo platform with clear contracts, tests, telemetry, and comprehensive documentation.

References:

- NLWeb blog: <https://news.microsoft.com/source/features/company-news/introducing-nlweb-bringing-conversational-interfaces-directly-to-the-web>
- NLWeb repo: <https://github.com/nlweb-ai/NLWeb>
- eShopLite sample: <https://github.com/Azure-Samples/eShopLite>

## 2. Goals and Non‑Goals

Goals:

- **Search Experience**: Natural-language, site-wide search for eShopLite using NLWeb
- **Chat Interface**: Real-time conversational chat on main page for product assistance and site navigation
- **Clear APIs**: C# Search and Chat APIs with normalized results, used by the Store front-end
- **Azure Deployment**: Production-ready deployment options via BICEP templates
- **Comprehensive Documentation**: Automated screenshot capture and detailed technical documentation
- **Basic Observability**: Reindex capability and minimal observability for both search and chat

Non‑Goals:

- No reuse of the legacy Semantic Search implementation, vector DB, or embeddings pipelines
- No cart/checkout changes beyond navigation from search/chat results
- No complex chat history persistence (session-based only)

## 3. Scope & Constraints

- Language/Framework: C#/.NET 9
- Orchestration: .NET Aspire 9.4; enroll all services
- Working solution: `src\eShopLite-NLWeb.slnx`
- NLWeb is the retrieval engine; use its API/SDK to query and index website content
- Azure deployment via BICEP templates supporting App Services and Container Apps
- Documentation automation via Playwright MCP server for screenshot capture

## 4. Personas & User Stories

### Search Personas
- **Shopper**: "As a shopper, I want to search the site in natural language so I can find products and policies quickly."
  - AC: P50 < 2s, relevant results, clickable links to detail pages.
- **Support agent**: "As support, I need quick answers from policy/FAQ pages to help customers."
  - AC: Authoritative pages with citations/links.

### Chat Personas
- **Customer**: "As a customer, I want to ask questions about products and get instant answers without searching."
  - AC: Real-time responses, product recommendations, contextual help
- **New visitor**: "As a new visitor, I want guidance on navigating the site and understanding product offerings."
  - AC: Conversational onboarding, category explanations, feature discovery

### Operations Personas
- **DevOps Engineer**: "As a DevOps engineer, I want to deploy the platform to Azure with minimal configuration."
  - AC: One-click deployment via BICEP, support for App Services and Container Apps
- **Documentation Maintainer**: "As a maintainer, I want automated screenshots and updated docs for new features."
  - AC: Automated screenshot capture, generated architecture diagrams

### Analytics Personas
- **Merchandiser**: "As merchandiser, I want visibility into zero-result queries and chat interactions to improve content."
  - AC: Zero-results metric, chat interaction analytics available for analysis.

## 5. KPIs & Success Metrics

### Search Metrics
- **Latency**: P50 < 2s, P95 < 4s (local/demo)
- **Error rate**: < 1% 5xx on search API
- **Zero-results rate**: tracked, target reduction
- **CTR on results**: clicks per query

### Chat Metrics
- **Response Time**: P50 < 1s, P95 < 3s for chat responses
- **Session Duration**: Average chat session > 2 minutes
- **Resolution Rate**: >80% of chat sessions end with product navigation
- **User Satisfaction**: Track positive/negative feedback on chat responses

### Platform Metrics
- **Uptime (demo SLO)**: 99% during demo windows
- **Index freshness**: within 24h of content changes (demo SLA)
- **Deployment Success**: 100% successful BICEP deployments
- **Documentation Coverage**: 100% feature coverage with screenshots

## 6. Solution Architecture

Components (under `src/`):

### Core Services
- **Search API** (existing, enhanced) — `Search/`
  - Role: Own the search contract; call NLWeb
  - Endpoints: `GET /api/v1/search`, `POST /api/v1/search/reindex` (protected)
  - Tech: ASP.NET Core Minimal API on .NET 9; typed HttpClient
  - Observability: structured logs, OTEL traces/metrics via Aspire service defaults

- **Chat API** (new) — `Chat/`
  - Role: Handle conversational interactions via NLWeb
  - Endpoints: `POST /api/v1/chat/message`, `GET /api/v1/chat/health`
  - Features: Session management, context retention, typing indicators
  - Tech: ASP.NET Core with SignalR for real-time communication
  - Integration: Calls NLWeb for conversational responses

### Frontend Services
- **Store** (existing, enhanced) — `Store/`
  - Add search box and `/search` page (existing)
  - **NEW**: Add prominent chat widget on main page
  - **NEW**: Chat interface with message history and typing indicators
  - Calls Search API and Chat API; renders results and conversations
  - Real-time updates via SignalR connection to Chat service

### Infrastructure Services
- **App Host** (existing) — `eShopAppHost/`
  - Orchestrate services with Aspire 9.4; use service discovery
  - **NEW**: Add Chat service orchestration and SignalR hub configuration

- **Service Defaults** — `eShopServiceDefaults/`
  - Shared Aspire configuration: service discovery, resiliency, health checks, OTEL
  - **NEW**: SignalR configuration and chat-specific telemetry

### Deployment & Documentation
- **Infrastructure** (new) — `infrastructure/`
  - BICEP templates for Azure App Services deployment
  - BICEP templates for Azure Container Apps deployment (default)
  - ARM parameter files for different environments
  - GitHub Actions workflows for CI/CD

- **Documentation** (new) — `docs/`
  - Solution architecture with Mermaid diagrams
  - User manual with screenshots
  - API documentation and examples
  - Deployment guides and troubleshooting

- **Screenshot Service** (new) — `ScreenshotService/`
  - Playwright MCP server integration for automated screenshot capture
  - Scheduled capture of main site features
  - Integration with documentation generation pipeline

### Aspire Requirements

- Enroll Search, Chat, Store, and AppHost into Aspire 9.4
- Use Aspire service discovery for Store → Search/Chat calls (no hard-coded URLs)
- Centralize logging, tracing, and metrics via Aspire defaults
- Expose readiness/liveness endpoints for diagnostics
- **NEW**: Configure SignalR backplane for Chat service scaling

## 7. Detailed API Contracts

Base path versioning: `/api/v1/`

### 7.1 Search API (Enhanced)

#### Search Endpoint
- **Method/Route**: GET `/api/v1/search`
- **Query params**:
  - `q` (string, required)
  - `top` (int, optional; default 10; 1–50)
  - `skip` (int, optional; default 0; ≥ 0)
- **Response 200**:

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

#### Reindex Endpoint
- **Method/Route**: POST `/api/v1/search/reindex`
- **Auth**: bearer/API key (configurable via environment)
- **Behavior**: trigger NLWeb (re)crawl or sitemap import for `SiteBaseUrl`
- **Response**: 202/200 + operation info; log progress; optional status endpoint (future)

### 7.2 Chat API (New)

#### Chat Message Endpoint
- **Method/Route**: POST `/api/v1/chat/message`
- **Headers**: `Content-Type: application/json`
- **Body**:

```jsonc
{
  "message": "What running shoes do you recommend for beginners?",
  "sessionId": "session-uuid-123", // optional, generated if not provided
  "context": {
    "currentPage": "/products",
    "userPreferences": { "budget": "under-100", "activity": "running" }
  }
}
```

- **Response 200**:

```jsonc
{
  "sessionId": "session-uuid-123",
  "response": "For beginners, I'd recommend starting with our comfort-focused running shoes...",
  "suggestedActions": [
    { "text": "View Beginner Running Shoes", "url": "/products/category/running-beginner" },
    { "text": "Read Running Guide", "url": "/guides/running-basics" }
  ],
  "metadata": {
    "responseTime": 850,
    "confidence": 0.92,
    "sources": ["/products/running", "/guides/running-basics"]
  }
}
```

#### Real-time Chat Hub (SignalR)
- **Hub Route**: `/chat-hub`
- **Methods**:
  - `SendMessage(message, sessionId)` → `ReceiveMessage(response, sessionId)`
  - `JoinSession(sessionId)` → user joins chat session
  - `LeaveSession(sessionId)` → user leaves chat session
  - `TypingIndicator(sessionId, isTyping)` → real-time typing status

### Error Handling (All APIs)
- **Errors (JSON)**: `{ "code": string, "message": string, "correlationId": string }`
  - 400 validation, 429 rate limit, 502 NLWeb failure, 500 server error
- **Rate-limit headers**: `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`

## 8. Indexing Strategy

- **Sources**: `SiteBaseUrl` and sitemap
- **Exclusions**: admin/cart/checkout paths
- **Robots**: honor `robots.txt`
- **Dynamic content**: prefer rendered HTML where feasible
- **Trigger**: manual reindex via POST; scheduled job (future)
- **Retention**: reindex replaces prior state
- **Chat Context**: Include product metadata and FAQ content for conversational responses

## 9. Security & Privacy

### Input Validation & Rate Limiting
- Input validation; enforce `MaxTop` and query length limits
- Strict CORS to Store origin
- Chat message length limits and content filtering
- Session-based rate limiting for chat interactions

### Authentication & Secrets
- Secrets in env/user-secrets; never commit secrets
- Abuse control: basic rate limiting and anomaly logs
- Output encoding; sanitize snippets and chat responses as needed
- **NEW**: Chat session validation and anti-spam measures

### Azure Security
- **Key Vault integration** for sensitive configuration
- **Managed Identity** for service-to-service authentication
- **Network Security Groups** in BICEP templates
- **HTTPS enforcement** across all endpoints

## 10. Accessibility & UX

### Search UX
- WCAG 2.2 AA: keyboard navigation, focus states, screen-reader labels
- States: loading, empty, error; actionable guidance when empty
- Snippet highlighting and truncation (~160 chars)

### Chat UX  
- **Keyboard Navigation**: Full keyboard accessibility for chat interface
- **Screen Reader Support**: Proper ARIA labels and live regions
- **Visual Indicators**: Typing indicators, message status, loading states
- **Mobile Responsive**: Chat widget adapts to different screen sizes
- **Message History**: Scrollable conversation history with timestamps
- **Quick Actions**: Suggested actions and common questions
- **Accessibility Features**: High contrast mode support, font size adjustments

### Screenshot Documentation
- **Automated Capture**: Screenshots of key user journeys
- **Alt Text Generation**: Automated alt text for documentation images
- **Visual Regression**: Detect UI changes through screenshot comparison

## 11. Observability

### Search Metrics (Enhanced)
- Metrics: req/sec, P50/P95 latency, error rate, zero-results, CTR
- Logs: structured with `correlationId`, NLWeb call duration/status, input size (bounded)
- Tracing: spans for API handler, NLWeb client, Store→Search calls (OTEL via Aspire)

### Chat Metrics (New)
- **Real-time Metrics**: Active sessions, message throughput, response times
- **Conversation Analytics**: Message sentiment, session duration, resolution paths
- **NLWeb Integration**: Track chat-specific NLWeb API usage and performance
- **User Experience**: Track user satisfaction scores and feedback

### Platform Observability
- **Dashboard**: basic charts for latency, errors, zero-results, CTR, chat metrics
- **Azure Monitor Integration**: Custom dashboards and alerts in Azure
- **Application Insights**: Deep application performance monitoring
- **Health Checks**: Comprehensive health endpoints for all services

## 12. Testing & Quality Gates

### Existing Tests (Enhanced)
- Unit tests: handlers, validators, mappers (Search)
- Integration test: `/api/v1/search` happy path with fake NLWeb client
- Load test: 10 concurrent users for 5 minutes; meet latency/error thresholds

### New Chat Tests
- **Unit tests**: Chat handlers, session management, SignalR hubs
- **Integration tests**: Chat API endpoints with mock NLWeb responses
- **Real-time tests**: SignalR connection handling and message delivery
- **Load tests**: Multiple concurrent chat sessions with message throughput validation

### Infrastructure Tests
- **BICEP Validation**: Template syntax and deployment validation
- **Infrastructure as Code**: Automated testing of Azure resource provisioning
- **Deployment Tests**: End-to-end deployment validation in test environments

### Documentation Tests
- **Screenshot Validation**: Automated verification of captured screenshots
- **Link Checking**: Validate all documentation links and references
- **Content Freshness**: Ensure documentation stays current with code changes

### Quality Gates
- **Chaos Testing**: NLWeb timeouts/5xx simulated; graceful fallback verified
- **Security Tests**: OWASP ZAP scanning, dependency vulnerability checks
- **Performance Tests**: Chat response times under concurrent load
- **Exit criteria**: all tests green; KPIs within thresholds; successful Azure deployment

## 13. Environments & Rollout

### Local Development
- Aspire 9.4 orchestrates services; service discovery resolves endpoints; single-command startup
- **NEW**: Chat service and SignalR hub running locally
- **NEW**: Playwright screenshot service for documentation generation

### Cloud Environments
- **Azure App Services**: Traditional PaaS deployment option
- **Azure Container Apps**: Default deployment option with better scaling
- **CI/CD Pipeline**: GitHub Actions with BICEP template deployment
- **Environment Promotion**: Dev → Staging → Production with approval gates

### Feature Deployment
- Feature flag for new chat UI; rollback by toggling flag
- **NEW**: Gradual rollout of chat features with A/B testing capability
- **NEW**: Blue-green deployment support via Azure Container Apps

## 14. Risks, Assumptions, Open Questions

### Risks
- **NLWeb Dependencies**: Rate limits/quotas → client rate limiting and optional caching
- **Real-time Performance**: SignalR scaling under high concurrent chat load
- **Azure Costs**: Container Apps scaling costs under high usage
- **Screenshot Automation**: Playwright stability and screenshot consistency

### Assumptions
- Stable product URLs and a complete sitemap
- NLWeb API/SDK provides site-scoped query and conversational capabilities
- **NEW**: NLWeb supports conversational context and follow-up questions
- **NEW**: Azure subscription has sufficient quotas for Container Apps deployment

### Open Questions
- Anchor-level deep links available from NLWeb results?
- **NEW**: NLWeb chat session state management capabilities?
- **NEW**: Integration patterns for Playwright MCP server in production?
- **NEW**: Preferred Azure regions for optimal NLWeb API performance?

## 15. Timeline & Ownership

**Roles**: Engineering (Search/Chat/Infrastructure), PM (requirements/KPIs), QA (tests), Ops (Aspire orchestration), DevOps (Azure deployment).

### Implementation Phases

#### Phase 1: Enhanced Search (Complete)
- Document hardening — finalize prompt/PRD ✅
- Search API skeleton — project scaffolding, config, health, empty endpoint ✅
- NLWeb integration — `INlWebClient`, search happy path (fake then real) ✅
- Store UI wiring — header search box, `/search` page, render results ✅
- Reindex + hardening — `POST /api/v1/search/reindex`, CORS/rate limit/validation, logs/metrics ✅
- Tests & docs — unit/integration tests, README updates ✅

#### Phase 2: Chat Integration (New)
- Chat API skeleton — project scaffolding, SignalR configuration
- NLWeb chat integration — conversational API implementation
- Store UI enhancement — prominent chat widget on main page
- Real-time communication — SignalR hub and client implementation
- Session management — chat context and conversation history
- Chat testing — unit/integration tests for chat functionality

#### Phase 3: Azure Deployment (New)
- BICEP template creation — App Services and Container Apps options
- GitHub Actions workflows — CI/CD pipeline setup
- Environment configuration — dev/staging/production parameter files
- Deployment validation — automated testing of Azure resources
- Documentation — deployment guides and troubleshooting

#### Phase 4: Documentation & Screenshots (New)
- Playwright integration — screenshot service implementation
- Documentation structure — docs folder with architecture and user guides
- Mermaid diagrams — solution architecture and user flow diagrams
- Automated documentation — screenshot capture and content generation
- Documentation testing — link validation and content freshness

#### Phase 5: Integration & Hardening
- End-to-end testing — full platform validation
- Performance optimization — chat and search performance tuning
- Security hardening — Azure security best practices implementation
- Production readiness — monitoring, alerting, and operational procedures

## 16. Dependencies

### Technical Dependencies
- .NET 9 SDK; .NET Aspire 9.4 workloads
- NLWeb endpoint/API key with conversational capabilities
- **NEW**: Azure subscription with Container Apps and App Services quotas
- **NEW**: GitHub Actions runners for CI/CD pipeline
- **NEW**: Playwright browsers for screenshot automation

### Content Dependencies
- Existing eShopLite Store site content
- **NEW**: Product catalog data for chat context
- **NEW**: FAQ and support content for conversational responses

### Service Dependencies
- **NEW**: SignalR backplane for multi-instance chat scaling
- **NEW**: Azure Key Vault for secrets management
- **NEW**: Azure Application Insights for monitoring

## 17. Acceptance Criteria (DoD)

### Search Criteria (Enhanced)
- Search queries return titles/snippets/links; navigation works ✅
- P50 < 2s, P95 < 4s locally for 10 concurrent users ✅
- Resilience: NLWeb failures show helpful fallback; no unhandled exceptions ✅
- Security: CORS restricted; input validation; rate limiting ✅
- Quality: unit tests present; at least one integration test for search endpoint ✅

### Chat Criteria (New)
- **Chat Interface**: Prominent chat widget on main page with real-time messaging
- **Conversational Flow**: Multi-turn conversations with context retention
- **Performance**: P50 < 1s, P95 < 3s for chat responses under normal load
- **Real-time**: SignalR connection maintains stable communication for 10+ minutes
- **Integration**: Chat responses include relevant product links and suggestions
- **Accessibility**: Full keyboard navigation and screen reader support

### Deployment Criteria (New)
- **Azure Deployment**: Successful deployment via BICEP templates to both App Services and Container Apps
- **CI/CD Pipeline**: Automated deployment from GitHub with proper validation gates
- **Environment Parity**: Identical functionality across local/dev/staging/production environments
- **Rollback Capability**: Quick rollback mechanism for failed deployments

### Documentation Criteria (New)
- **Architecture Documentation**: Complete solution architecture with Mermaid diagrams
- **User Manual**: Comprehensive user guide with screenshots of all major features
- **API Documentation**: Complete API reference with examples and error codes
- **Screenshot Automation**: Playwright service captures and updates screenshots automatically
- **Deployment Guide**: Step-by-step instructions for Azure deployment and troubleshooting

## 18. Deliverables

### Core Services
- `Search/` service with endpoints/config/tests (C#/.NET 9) ✅
- **NEW**: `Chat/` service with real-time messaging and NLWeb integration
- Store UI: search box and `/search` page ✅
- **NEW**: Store UI: prominent chat widget and conversation interface
- Aspire AppHost updates for orchestration and service discovery ✅
- **NEW**: SignalR hub configuration and chat service orchestration

### Infrastructure & Deployment
- **NEW**: `infrastructure/` folder with BICEP templates for Azure deployment
- **NEW**: GitHub Actions workflows for CI/CD pipeline
- **NEW**: ARM parameter files for different environments
- **NEW**: Deployment validation and testing scripts

### Documentation & Automation
- **NEW**: `docs/` folder with comprehensive documentation
- **NEW**: Solution architecture diagrams using Mermaid
- **NEW**: User manual with automated screenshots
- **NEW**: `ScreenshotService/` with Playwright MCP server integration
- Documentation: run/config steps (README), metrics/logging notes ✅

### Testing & Quality
- **NEW**: Comprehensive test suites for chat functionality
- **NEW**: Infrastructure as Code testing
- **NEW**: End-to-end platform validation tests
- **NEW**: Performance tests for chat and search under load

## 19. Implementation Guide for Copilot Coding Agent

Use `src\eShopLite-NLWeb.slnx` and enroll all services into Aspire 9.4.

### Phase 2: Chat Integration Tasks

1. **Create Chat Service**:
   - Create `Chat/` project (.NET 9)
   - Reference `eShopServiceDefaults`
   - Add `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`
   - Configure SignalR with proper CORS and connection settings

2. **Implement Chat API**:
   - Implement `POST /api/v1/chat/message`: validate inputs, call `INlWebClient.ChatAsync`, manage sessions
   - Add SignalR hub for real-time messaging: `ChatHub` with proper connection handling
   - Session management: track conversation context and user preferences
   - Add typed `HttpClient` with resilience/timeouts for NLWeb chat API

3. **Enhance Store UI**:
   - Add prominent chat widget to main page (homepage/landing)
   - Implement chat interface with message history and typing indicators
   - Configure SignalR client connection with proper error handling
   - Style chat interface to match existing Store design patterns

4. **NLWeb Chat Integration**:
   - Extend `INlWebClient` interface with chat methods: `ChatAsync`, `GetChatContextAsync`
   - Implement `MockNlWebClient` chat functionality with realistic conversational responses
   - Add conversation context management and multi-turn dialog support

### Phase 3: Azure Deployment Tasks

5. **Create BICEP Templates**:
   - Create `infrastructure/` folder structure
   - Implement `main.bicep` with Container Apps deployment (default)
   - Implement `app-services.bicep` with App Services deployment option
   - Create parameter files for dev/staging/production environments

6. **Setup CI/CD Pipeline**:
   - Create GitHub Actions workflow for automated deployment
   - Add BICEP validation and deployment steps
   - Configure secrets management with Azure Key Vault
   - Add deployment validation and rollback procedures

### Phase 4: Documentation & Screenshots Tasks

7. **Create Documentation Structure**:
   - Create `docs/` folder with proper structure
   - Implement solution architecture documentation with Mermaid diagrams
   - Create comprehensive user manual with feature descriptions
   - Add API documentation with examples and troubleshooting guides

8. **Implement Screenshot Service**:
   - Create `ScreenshotService/` project with Playwright MCP server integration
   - Implement automated screenshot capture of main site features
   - Add screenshot comparison and validation capabilities
   - Integrate with documentation generation pipeline

9. **Documentation Automation**:
   - Create scripts for automated documentation generation
   - Implement Mermaid diagram generation for architecture views
   - Add screenshot automation for user manual and feature documentation
   - Setup documentation validation and freshness checking

### Phase 5: Integration & Testing Tasks

10. **Aspire Integration**:
    - Update `eShopAppHost` to include Chat service and SignalR configuration
    - Configure service discovery for Store → Chat communication
    - Add health checks and telemetry for all new services
    - Enable distributed tracing across search and chat interactions

11. **Comprehensive Testing**:
    - Unit tests: Chat handlers, session management, SignalR hubs
    - Integration tests: Chat API endpoints and real-time messaging
    - Infrastructure tests: BICEP template validation and deployment testing
    - Performance tests: Chat response times and concurrent session handling

12. **Security & Hardening**:
    - Implement proper authentication and authorization for chat
    - Add input validation and content filtering for chat messages
    - Configure CORS policies for SignalR connections
    - Add rate limiting and anti-spam measures for chat interactions

### Edge Cases to Cover

#### Chat-Specific Edge Cases
- Empty/overly long chat messages; session timeout handling
- SignalR connection drops; automatic reconnection with message recovery
- NLWeb chat API timeout/5xx; return helpful fallback responses with correlationId
- Multiple browser tabs; proper session handling across connections
- Mobile browser chat; responsive design and touch-friendly interface

#### Deployment Edge Cases
- Azure resource quota limits; graceful failure with clear error messages
- BICEP template validation failures; detailed error reporting and resolution steps
- Network connectivity issues during deployment; retry mechanisms and rollback procedures
- Environment variable configuration; validation and error handling for missing/invalid values

#### Documentation Edge Cases
- Screenshot capture failures; fallback mechanisms and manual override options
- Mermaid diagram rendering issues; validation and error handling
- Documentation link validation; automated checking and broken link reporting
- Content synchronization; ensure documentation stays current with code changes

## 20. Glossary & References

- **NLWeb**: Natural language web interaction/search framework with conversational capabilities
- **Aspire**: .NET application composition and orchestration with service discovery and OTEL
- **SignalR**: Real-time web communication library for .NET
- **BICEP**: Domain-specific language for deploying Azure resources
- **Container Apps**: Azure's serverless container platform with automatic scaling
- **Playwright MCP**: Microsoft Cloud Platform integration for automated browser testing and screenshots
- **Mermaid**: Markdown-based diagramming and charting tool
- **PRD**: Product Requirements Document