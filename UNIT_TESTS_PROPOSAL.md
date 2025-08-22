# Unit Tests Proposal for a2aapiredemo

> Purpose: Identify missing or weak unit tests across the `src/` projects and provide a ready-to-paste GitHub Issue body that describes the work to add those tests.

Last scan: Aug 22, 2025

## Summary / scope

This document enumerates suggested unit tests for the repository root `src/` folder. It is intentionally conservative: no production code changes are proposed here. The goal is to create a prioritized checklist of test work that can be opened as a GitHub Issue and executed later.

Projects analyzed (under `src/`):

- `Products` (API endpoints, AI search integration, data seeding)
- `Store` (Blazor UI app services: `ProductService`, `CartService`, `CheckoutService`)
- `CartEntities`, `DataEntities`, `SearchEntities`, `VectorEntities` (POCOs/value logic)
- `ZavaAppHost` (app host bootstrap / infra)

Existing tests found:

- `src/Products.Tests` contains `ProductApiActionsTests` with good coverage for `ProductApiActions` endpoints (GetAll, GetById, Create, Update, Search).
- `src/Store.Tests` exists but only contains a placeholder `UnitTest1.cs` (no real tests).

Gap summary (high-level):

- `ProductAiActions` (AISearch) and `MemoryContext` are not covered.
- `Store` service classes (`ProductService`, `CartService`, `CheckoutService`) have no unit tests.
- Domain logic for `Cart` (subtotal/tax/total/itemcount) should have focused unit tests.
- `SearchEntities`, `VectorEntities` serialization/formatting and response text logic not covered.
- Integration tests / minimal API tests for endpoint mapping, health checks, and Program bootstrap are missing.

---

## Proposed unit tests (by project)

Each entry shows: target file/class, suggested tests, rationale, difficulty estimate (S/M/L), and notes about mocking.

### 1) Products

- Targets:
  - `Products.Endpoints.ProductAiActions` (method `AISearch`)
  - `Products.Memory.MemoryContext` (initialization and `Search` logic)
  - `Products.Data.DbInitializer` (seed list) — integration-style test

- Suggested tests:
  1. `ProductAiActions.AISearch_ReturnsOk_WithSearchResponse` (M)
     - Mock `MemoryContext` (or provide a test double) to return a `SearchResponse` instance when `Search` is called.
     - Call `AISearch` with a test search term and an in-memory `Context` (or a mocked `Context`).
     - Assert: returned `IResult` is an `Ok` result and contains the expected `SearchResponse` object.
     - Note: `AISearch` is a thin wrapper, so the main focus is ensuring the result wrapping.

  2. `MemoryContext_Search_ReturnsExpectedResults_WhenEmbeddingsAvailable` (L)
     - Unit-test `MemoryContext.Search` method with mocked `IEmbeddingGenerator` and `IChatClient` (both are injected in constructor in `MemoryContext`).
     - Validate the method handles empty DB, or when embeddings return hits.
     - Difficulty: higher due to mocking 3rd-party OpenAI-style interfaces; use small in-memory vectors or wrap dependencies.

  3. `DbInitializer_Initialize_CreatesSeedProducts` (M)
     - Create an in-memory `ProductDataContext` or `Context` (DbContextOptions with InMemoryDatabase).
     - Call `DbInitializer.Initialize(context)` and assert `context.Product` has expected seeded count and contains at least one known seeded product name.

- Mocks & tools:
  - Use `Microsoft.EntityFrameworkCore.InMemory` for DbContext tests.
  - Use a mocking library (MSTest + Moq or NSubstitute) to mock `MemoryContext` dependencies and `IChatClient` / `IEmbeddingGenerator`.

### 2) Store

- Targets:
  - `Store.Services.ProductService` (HTTP client-backed)
  - `Store.Services.CartService` (session-backed cart logic)
  - `Store.Services.CheckoutService` (order processing and session persistence)

- Suggested tests:
  ProductService:
  1. `GetProducts_ReturnsProducts_WhenHttpOk` (M)
     - Mock `HttpClient` (HttpMessageHandler) to return a 200 with a JSON array of products.
     - Assert `GetProducts` returns the deserialized products list.
  2. `GetProducts_ReturnsEmpty_WhenHttpNotOkOrException` (M)
     - Simulate non-success status code or throw from `SendAsync` and assert an empty list is returned and no exception escapes.
  3. `Search_UsesAiEndpoint_WhenSemanticSearchTrue` (M)
     - Mock `HttpClient` to capture request URI; when `semanticSearch=true`, `GetAsync("/api/aisearch/{term}")` should be called; otherwise `/api/product/search/{term}`.
     - Assert method returns `SearchResponse` or fallback with `Response="No response"` when failure.

  CartService:
  1. `GetCartAsync_ReturnsEmptyCart_WhenNoSession` (S)
     - Mock `ProtectedSessionStorage.GetAsync<string>` to return `.Success = false` or `Value = null`.
     - Assert returns empty `Cart` instance.
  2. `GetCartAsync_ReturnsCart_WhenSessionHasJson` (M)
     - Return an existing cart JSON string and assert parsing.
  3. `GetCartAsync_ReturnsEmptyCart_OnServerSideRendering_InvalidOperationException` (S)
     - Mock `ProtectedSessionStorage` to throw `InvalidOperationException` with message containing "JavaScript interop calls cannot be issued at this time" and assert method returns empty cart.
  4. `AddToCartAsync_AddsNewItem_WhenProductFound` (M)
     - Mock `IProductService.GetProducts` to return a list with a product matching `productId`.
     - Mock `ProtectedSessionStorage.SetAsync` to capture saved cart JSON.
     - Assert cart now contains item with quantity 1 and `SaveCartAsync` was called.
  5. `AddToCartAsync_NoAction_WhenProductNotFound` (S)
     - `IProductService.GetProducts` returns no product; assert `SetAsync` not called / cart unchanged.
  6. `UpdateQuantityAsync_RemovesOrUpdatesItem` (M)
     - Start with a cart with an item; call `UpdateQuantityAsync` with 0 => item removed; call with >0 => item count updated.
  7. `RemoveFromCartAsync_RemovesItem_WhenPresent` (S)
  8. `ClearCartAsync_HandlesSSRException` (S)
  9. `GetCartItemCountAsync_Returns0_OnSSRException` (S)

  CheckoutService:
  1. `ProcessOrderAsync_CreatesConfirmedOrder_AndSavesToSession` (M)
     - Provide a `Cart` and a `Customer`, mock `ProtectedSessionStorage.SetAsync` and `GetAsync` to persist orders list.
     - Assert returned `Order` has `Status == "Confirmed"`, `Subtotal/Tax/Total` match cart, and `OrderNumber` follows expected pattern (currently `ESL-YYYYMMDD-xxxx`).
  2. `GetOrderAsync_ReturnsOrder_WhenExists` (S)
     - Mock session storage GetAsync to return a list with an order; assert retrieval.

- Mocks & tools:
  - Mock `HttpClient` via custom `HttpMessageHandler` in unit tests.
  - `ProtectedSessionStorage` is not interface-based; wrap it behind a small testable interface or use a test double by creating a small shim class in test project (recommend refactor later). For the immediate purpose, tests can use a wrapper interface and a small local implementation in tests that mimics session storage behavior.

### 3) CartEntities / domain

- Targets:
  - `CartEntities.Cart` properties and calculations.

- Suggested tests:
  1. `CartCalculation_SubtotalTaxTotalItemCount_CalculatedCorrectly` (S)
     - Create a `Cart` with several `CartItem` entries and assert `Subtotal`, `Tax` (8%), `Total`, and `ItemCount` match expected values.

### 4) SearchEntities / VectorEntities

- Targets:
  - `SearchEntities.SearchResponse` formatting (the `Response` string creation is in `ProductApiActions.SearchAllProducts` — tests for SearchAllProducts exist but cover response; still add serialization tests if necessary)
  - `VectorEntities.ProductVector` (if it contains logic) — simple unit tests for serialization/constructors

- Suggested tests:
  1. `SearchResponse_Serialization_Roundtrip` (S)
     - Serialize/deserialize `SearchResponse` and ensure object equality for the important fields.

### 5) ZavaAppHost / Program.cs

- Targets:
  - `ZavaAppHost.Program` startup mapping and service registrations.

- Suggested tests:
  1. `Program_ConfiguresExpectedServicesAndEndpoints` (L)
     - Integration-style test using `WebApplicationFactory` or custom host builder to validate services are registered and key endpoints respond (health checks, /swagger if available).
     - This is higher difficulty and optional.

---

## Priorities & estimates (triage)

- High (P0, 1-2 days):
  - ProductService tests (GetProducts success/failure, Search routing)
  - CartService core behaviors (Add, UpdateQuantity, Remove)
  - Cart domain calculations test
- Medium (P1, 2-3 days):
  - CheckoutService tests
  - ProductAiActions (AISearch) basic test
  - DbInitializer seed test
- Low (P2, 3+ days):
  - MemoryContext deeper tests with embeddings/chat client mocks
  - Program/host integration tests
  - SearchEntities/VectorEntities additional serialization tests

Estimates assume one engineer with existing familiarity with MSTest and Moq/NSubstitute.

---

## Suggested test framework & patterns

- Use the repository's existing test style: MSTest is already used in `Products.Tests`.
- Add `Moq` (or `NSubstitute`) as a test-time dependency for mocking complex collaborators.
- Use `Microsoft.EntityFrameworkCore.InMemory` for EF Core DbContext tests.
- For `HttpClient` mocking, use a custom `HttpMessageHandler` that can be injected into `HttpClient`.
- For `ProtectedSessionStorage`, prefer a small wrapper interface (e.g., `ISessionStore`) that can be injected. If you cannot change production code, create a small test-only shim in `Store.Tests` and test behaviors at the service level by constructing `CartService` with the shim.

Example `HttpMessageHandler` test helper (conceptual):

```csharp
// In tests only
public class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;
    public TestHttpMessageHandler(HttpResponseMessage response) => _response = response;
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(_response);
}
```

---

## Acceptance criteria for the Issue

- Each proposed test has at least one unit test implemented and green in CI.
- Tests avoid hitting external services; all network/AI calls are mocked.
- Tests exercise edge cases: empty results, exceptions, and normal success paths.

---

## How to run tests locally (once tests are added)

Open PowerShell (repository root) and run:

```powershell
# run all tests
dotnet test src/Products.Tests -c Debug
dotnet test src/Store.Tests -c Debug
# or run all tests in the solution
dotnet test src/Zava-Aspire.slnx -c Debug
```

---

## Copy-paste GitHub Issue content

Use the following as the Issue body. The text below is already formatted; copy the entire block into a new GitHub Issue.

Title:

```
Add missing unit tests for Products, Store, and domain entities
```

Body (copy-paste):

```
### Summary
This issue tracks adding a set of unit tests across the codebase to improve coverage and confidence before making further changes.

### Scope
- Tests for `Products` (AI-search wrapper and seed initializer)
- Tests for `Store` services: `ProductService`, `CartService`, `CheckoutService`
- Unit tests for `CartEntities` domain logic
- Serialization/format tests for `SearchEntities` / `VectorEntities`
- (Optional) integration tests for `ZavaAppHost` program/endpoint mappings

### Acceptance Criteria
- Unit tests added and passing for the items listed below
- No external network calls in unit tests (mock AI/HTTP clients)
- Tests validate success paths and important edge cases (empty results, exceptions)

### Tasks (checklist)
- [ ] Products: `ProductApiActions` / `ProductAiActions` / `MemoryContext` tests
  - [ ] `AISearch_ReturnsOk_WithSearchResponse`
  - [ ] `DbInitializer_Initialize_CreatesSeedProducts`
  - [ ] (optional) `MemoryContext` embedding-based search tests (mocked)

- [ ] Store: `ProductService` tests
  - [ ] `GetProducts_ReturnsProducts_WhenHttpOk`
  - [ ] `GetProducts_ReturnsEmpty_WhenHttpNotOkOrException`
  - [ ] `Search_UsesAiEndpoint_WhenSemanticSearchTrue`

- [ ] Store: `CartService` tests
  - [ ] `GetCartAsync_ReturnsEmptyCart_WhenNoSession`
  - [ ] `GetCartAsync_ReturnsCart_WhenSessionHasJson`
  - [ ] `GetCartAsync_ReturnsEmptyCart_OnSSRException`
  - [ ] `AddToCartAsync_AddsNewItem_WhenProductFound`
  - [ ] `AddToCartAsync_NoAction_WhenProductNotFound`
  - [ ] `UpdateQuantityAsync_RemovesOrUpdatesItem`
  - [ ] `RemoveFromCartAsync_RemovesItem_WhenPresent`
  - [ ] `ClearCartAsync_HandlesSSRException`
  - [ ] `GetCartItemCountAsync_Returns0_OnSSRException`

- [ ] Store: `CheckoutService` tests
  - [ ] `ProcessOrderAsync_CreatesConfirmedOrder_AndSavesToSession`
  - [ ] `GetOrderAsync_ReturnsOrder_WhenExists`

- [ ] Domain: `CartEntities.Cart` calculation tests
  - [ ] `CartCalculation_SubtotalTaxTotalItemCount_CalculatedCorrectly`

- [ ] (Optional) Integration: `ZavaAppHost` program/endpoint registration tests

### Implementation notes
- Use MSTest to match existing tests, and add `Moq` or `NSubstitute` for mocking.
- For EF Core tests, use `Microsoft.EntityFrameworkCore.InMemory` provider.
- For `HttpClient`-backed `ProductService`, use a mock `HttpMessageHandler` injected into `HttpClient`.
- For `ProtectedSessionStorage`, either wrap the dependency behind an interface or create a small test shim in `Store.Tests` to simulate session storage.

### Labels (suggested)
`area:tests`, `type:chore`, `priority:medium`

### Estimate
8-16 story points (broken down across multiple PRs). Prioritize Store service tests and Cart domain tests first.

```

---

## Next steps (recommended implementation order)

1. Add unit tests for `Cart` domain computations (small, quick win).
2. Add `ProductService` tests (HttpClient mocking — medium effort).
3. Add `CartService` tests (session shim or wrapper needed; medium effort).
4. Add `CheckoutService` tests.
5. Add `ProductAiActions` / `MemoryContext` tests.
6. Optional: Program/host integration tests.

---

## Companion notes / suggestions

- The code currently generates order numbers with the `ESL-` prefix (`ESL-YYYYMMDD-xxxx`) in `CheckoutService.GenerateOrderNumber`. If the project is being rebranded to `Zava`, consider updating this to `ZV-` or `ZAVA-` and add a test that checks the new format.
- There are several `.vs` and `.slnx` artifacts referencing `eShopLite`; these are cosmetic in the workspace and not test-related, but worth cleaning in a follow-up.

---

If you'd like, I can also open a draft PR adding the easiest tests first (Cart domain and ProductService GetProducts) and include example test files showing mocking patterns (MSTest + Moq + InMemory EF). Let me know which test(s) you want implemented first and I'll create them.
