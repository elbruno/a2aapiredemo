# PRD: Add Mock Payment Server / Payment Service to Zava-Aspire

Metadata
- Date: 2025-08-24
- Author: elbruno & Copilot (documentation draft)

Purpose
- Provide a lightweight, reliable mock payment capability to unblock checkout flows and demos without real payment processors.
- Establish a stable payment API contract that the solution can later swap to a real provider with minimal change.

Scope
- In-scope: New Mock Payment Server (service), Store integration, simple persistence, health/telemetry, Aspire registration.
- Out-of-scope: Real PCI card handling, real gateway integrations, invoicing, chargebacks/disputes, subscriptions/recurring billing.

Key success criteria
- Store can authorize, capture, refund, and query payment status via a single service.
- Deterministic, configurable mock behaviors (success, decline, latency) via env/config flags.
- Observability via Aspire (health, logs, traces) and readiness checks.
- Minimal data model sufficient for order reconciliation and refunds.

Quick checklist
- Service runs locally with Aspire; default port 5010.
- REST endpoints: authorize, capture, refund, get status, health/ready.
- Store uses typed HttpClient to call the service; feature toggle to enable.
- Persistence enabled (SQLite default); initial migrations created.
- Health/ready endpoints return 200 in happy path.
- Feature flags/env vars drive mock outcomes (always-approve, decline-rate, latency).

Assumptions
- No real card data is stored or processed; use opaque tokens or test PANs only.
- Order ID and total amount are provided by the Store; taxes/fees are computed upstream.
- Latency and failure simulation are needed for demo scenarios.
- The solution targets .NET 9 and uses Aspire AppHost for service discovery and telemetry.

High-level design overview
- A mock Payment Service (Blazor Server hosting model with minimal APIs/controllers) exposes REST endpoints.
- EF Core + SQLite for local persistence; connection string is configurable and swappable.
- Store integrates via a typed HttpClient (PaymentClient) registered in DI; the base address comes from Aspire service discovery.
- Aspire AppHost registers the Payment Service with health checks, logging, tracing, configuration.
- Feature flags/environment variables control deterministic mock responses and optional artificial latency.

API contract summary (names and intent)
- POST /api/payments/authorize — Reserve funds for an order (no capture yet).
- POST /api/payments/capture — Capture previously authorized funds (full/partial).
- POST /api/payments/refund — Refund a captured payment (full/partial).
- GET /api/payments/{paymentId} — Retrieve payment status/details.
- GET /healthz — Liveness probe.
- GET /readyz — Readiness probe (DB reachable, migrations applied).

Data model summary (high level)
- Payments
  - PaymentId (GUID), OrderId (string), Amount (decimal), Currency (string)
  - Status (Authorized|Captured|Refunded|Declined|Failed|Voided)
  - Method (string), CustomerId (string?), ExternalRef (string?), AuthCode (string?)
  - Metadata (JSON?), CreatedUtc, UpdatedUtc
- PaymentEvents
  - EventId (GUID), PaymentId (GUID), Type (Authorized|Captured|Refunded|Declined|Failed)
  - Amount (decimal), Reason (string?), Data (JSON?), CreatedUtc
- IdempotencyKeys (optional)
  - Key (string), RequestHash (string), PaymentId (GUID), CreatedUtc

Implementation notes
- Service: Blazor Server project hosting minimal APIs/controllers for payment operations.
- Persistence: EF Core with SQLite by default; connection from configuration (e.g., ConnectionStrings__Payments).
- Store integration: Add typed HttpClient and a PaymentClient; map DTOs; surface status to UI.
- Aspire: Register Payment Service in AppHost for service discovery, health, logging/metrics; expose an http binding (default 5010).
- Mock behavior: Env flags for outcomes (always-approve, decline-rate %) and artificial latency (RandomLatencyMs).

Configuration & local defaults
- Suggested local port: 5010 (override via ASPNETCORE_URLS or Aspire service binding).
- Environment variables (double-underscore for .NET options):
  - ConnectionStrings__Payments=Data Source=payments.db
  - Payment__Provider=Mock
  - Payment__DefaultCurrency=USD
  - Payment__AlwaysApprove=true
  - Payment__DeclineRatePercent=0
  - Payment__RandomLatencyMs=0
  - Payment__IdempotencyRequired=true

Security & privacy notes
- Do not store PAN/CVV or PII; only opaque tokens and non-sensitive metadata.
- Redact logs; avoid logging full request bodies or sensitive headers.
- Enforce TLS outside local dev; consider Store-issued JWT for AuthN/Z in future.
- Keep PCI scope out by explicit prohibition of real card data in code and docs.

Testing & validation
- Unit tests for approval/decline logic, latency injection, idempotency handling.
- Integration tests for authorize ? capture ? refund lifecycle.
- Health checks: /healthz and /readyz return 200 and reflect DB state.
- Store E2E test: checkout flow shows appropriate status transitions.

Acceptance criteria
- Endpoints implemented per API contract and documented.
- Deterministic mock behavior controlled by env vars; defaults are sane for demos.
- Data persisted and queryable; status reflects full lifecycle.
- Store integration via typed HttpClient behind feature flag.
- Registered with Aspire AppHost; visible in dashboard with health/telemetry.

Rollout plan
- Phase 1: Implement service, EF model/migrations, health/readiness; run locally with Aspire.
- Phase 2: Wire Store integration behind a feature toggle; add unit/integration tests.
- Phase 3: Add CI build/test and basic smoke tests.
- Phase 4: Demo scenarios with decline/latency simulations; collect feedback and iterate.

Appendix: example request/response JSON

Authorize request
```
{
  "orderId": "ORD-10001",
  "amount": 149.99,
  "currency": "USD",
  "paymentMethod": {
    "type": "token",
    "token": "tok_test_abc123"
  },
  "idempotencyKey": "ORD-10001-auth"
}
```

Authorize response
```
{
  "paymentId": "9d9b2f7e-9a3c-4e62-9a9d-1b4b6c2c1a22",
  "status": "Authorized",
  "authCode": "AUTH-XYZ-123",
  "orderId": "ORD-10001",
  "amount": 149.99,
  "currency": "USD",
  "createdUtc": "2025-08-24T12:00:00Z"
}
```

Capture request
```
{
  "paymentId": "9d9b2f7e-9a3c-4e62-9a9d-1b4b6c2c1a22",
  "amount": 149.99,
  "idempotencyKey": "ORD-10001-cap"
}
```

Capture response
```
{
  "paymentId": "9d9b2f7e-9a3c-4e62-9a9d-1b4b6c2c1a22",
  "status": "Captured",
  "capturedAmount": 149.99,
  "orderId": "ORD-10001",
  "currency": "USD",
  "updatedUtc": "2025-08-24T12:02:00Z"
}
```

Refund request
```
{
  "paymentId": "9d9b2f7e-9a3c-4e62-9a9d-1b4b6c2c1a22",
  "amount": 20.00,
  "reason": "Partial refund for returned item",
  "idempotencyKey": "ORD-10001-ref-1"
}
```

Refund response
```
{
  "paymentId": "9d9b2f7e-9a3c-4e62-9a9d-1b4b6c2c1a22",
  "status": "Refunded",
  "refundedAmount": 20.00,
  "orderId": "ORD-10001",
  "currency": "USD",
  "updatedUtc": "2025-08-24T12:05:00Z"
}
```
