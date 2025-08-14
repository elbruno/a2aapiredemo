# Add Semantic Search Azure Function, wire to SQL2005 scenario, enroll in .NET Aspire orchestration, and update README

You are an expert .NET developer and automated coding agent. Make all changes directly in this repository to implement a new Azure Function project that performs Semantic Search over the SQL2005 scenario database, enroll it into the .NET Aspire orchestration so the Store front-end can call it, and update the root README with implementation notes and references.

## Important repo facts

- Work with the repository root that contains `src/`.
- The orchestration entrypoint is `src/eShopAppHost/Program.cs` (open and read this file; it declares the SQL container and `productsDb`).
- Solution file to edit/add to is `src/eShopLite-AzFnc.slnx`.
- Use scenario reference: [SQL2005 scenario](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/08-Sql2025) — follow its recommended SQL schema, indexing/semantic search patterns, and sample queries where applicable.
- New Function project path: `src/SemanticSearchFunction/` (create this folder and project).

## High-level goals (must be achieved)

1. Add a new Azure Functions .NET project `SemanticSearchFunction` to the `src` folder and to the solution.
2. Implement an HTTP-triggered Azure Function endpoint that accepts a search query and returns semantically ranked results from the `productsDb` SQL database.
3. Ensure the Function reads its DB connection string/config from configuration/environment (use `ConnectionStrings:productsDb` or a clearly documented env var).
4. Wire the Function into the .NET Aspire orchestration in `src/eShopAppHost/Program.cs` so:
   - The Semantic Search Function has a persistent lifetime where required.
   - It has access to `productsDb` (use `.WithReference(productsDb)` or equivalent).
   - The `store` project can discover/call the semantic search endpoint (e.g., `store.WithReference(semanticsearch)` and mark function as external HTTP endpoint).
5. Update the root `README.md` with a new section describing the scenario, how to run locally, env vars, and the SQL2005 reference link.
6. Create at least one unit/integration test for the Function (happy path + DB connection smoke test or clear instructions to run an integration test against a test DB).

---

## Additional clarifications & constraints (paste these into the prompt to reduce ambiguity)

- Platform versions: target `.NET 9 (net9.0)` and `.NET Aspire 9.4` packages.
- Do not modify unrelated projects except to add references. Keep existing product behavior unchanged unless strictly required.
- Exact file/project names (enforce these to avoid confusion):
  - Project: `src/SemanticSearchFunction/SemanticSearchFunction.csproj`
  - Main function: `src/SemanticSearchFunction/Functions/SearchFunction.cs`
  - Repository: `src/SemanticSearchFunction/Repositories/SqlSemanticSearchRepository.cs`
  - Tests: `src/SemanticSearch.Tests/`

- Configuration keys and precedence (explicit):
  1. Environment variable `ConnectionStrings__productsDb`
  2. `appsettings.json` / `local.settings.json` `ConnectionStrings:productsDb`

- Recommended env vars to declare in README and local settings:
  - `ConnectionStrings__productsDb` (required)
  - `SEMANTIC_SEARCH_TOP_DEFAULT` (default 10)
  - `SEMANTIC_SEARCH_USE_EMBEDDINGS` (true/false)
  - `AI_embeddingsDeploymentName` (optional, for OpenAI embeddings)

- Security & operational rules:
  - Use parameterized SQL commands or stored procedures; no string concatenation.
  - Use CancellationToken and a default command timeout (e.g., 15s).
  - Do not log raw connection strings or sensitive data.
  - Return a `traceId` on 500/503 errors and log it for troubleshooting.
- Tests requirement:
  - Update or add unit tests if needed. If existing unit tests in the repo are failing, fix them as part of this work so the test suite passes. If you make behavior changes that affect tests, update the tests accordingly.

---

## Detailed implementation steps (do these in order)

### A. Create the Function project

- Create new folder `src/SemanticSearchFunction/`.
- Create an Azure Functions project targeting the solution's .NET version (follow repo conventions). Use HTTP-trigger single function template.
- Add required NuGet packages (match repo conventions):
  - `Microsoft.Azure.Functions.Worker` or `Microsoft.Azure.WebJobs` depending on repo style.
  - `Microsoft.Data.SqlClient` for SQL access.
  - `System.Text.Json` (or match repo serializer).
- Implement `SearchFunction.cs` with a single HTTP POST endpoint `/api/semanticsearch`. Input: JSON `{ "query": string, "top": int? }`. Output: JSON `{ "results": [ { "id","title","score","snippet","metadata": {} } ] }`.

### B. Implement SQL semantic search logic

- Read the SQL connection string from configuration in order of precedence given above.
- Use parameterized queries and/or the sample patterns from the SQL2005 reference. Follow the SQL scenario example for semantic ranking or vector search features. Use stored procs if recommended.
- Add `ISemanticSearchRepository` and `SqlSemanticSearchRepository` implementations. Use `Microsoft.Data.SqlClient` and ensure parameterization and cancellation support.

### C. Orchestration registration

- Update `src/eShopAppHost/Program.cs` to register the project. Example (adapt to the builder API in repo):

```csharp
var semantic = builder.AddProject<Projects.SemanticSearch>("semanticsearch")
    .WithReference(productsDb)
    .WaitFor(productsDb)
    .WithExternalHttpEndpoints();

store.WithReference(semantic);
```

### D. Solution & project wiring

- Add the new project to `src/eShopLite-AzFnc.slnx`.
- Ensure project reference and `Projects.SemanticSearch` mapping (if the repo uses that pattern) are added.

### E. Tests

- Add unit tests and an integration smoke test:
  - Unit tests for repository logic (mock ADO or use in-memory where appropriate).
  - Integration smoke test that runs the orchestration (containerized SQL) and calls `/api/semanticsearch`.

- Fix existing failing tests:
  - If the repo contains existing unit tests that fail, diagnose and fix them as part of this task. Do not leave the test suite broken. Update tests if intended behavior changes.
- Add or update tests as necessary to cover new behavior introduced by the Semantic Search function.

### F. README update

- Update `README.md` with:
  - Env var names and example `local.settings.json` snippet.
  - Sample curl/PowerShell requests and sample responses.
  - How to run tests locally and in CI.

### G. Code quality & docs

- Add inline docs and structured logging. Keep changes style-consistent with the repo.

---

## Contracts / Data shapes

- Request:
  - POST /api/semanticsearch
  - JSON: `{ "query": "string", "top": 10 }`
- Response:
  - 200 OK with JSON:

```json
{
  "results": [
    { "id": "<string|int>", "title": "<string>", "score": 0.0, "snippet": "<text>", "metadata": {} }
  ],
  "traceId": "<trace id>"
}
```

- Errors:
  - 400 Bad Request for invalid input
  - 503 Service Unavailable for DB connectivity
  - 500 Internal Server Error (with `traceId`)

---

## Acceptance criteria (must pass)

1. The new project `src/SemanticSearchFunction/` exists and is added to `src/eShopLite-AzFnc.slnx`.
2. `src/eShopAppHost/Program.cs` registers the new semantic function project and links it to `productsDb` via `.WithReference(productsDb)` (or equivalent).
3. The `store` project registration references the new semantic search project so the orchestration knows the dependency.
4. The function endpoint `/api/semanticsearch` returns semantic results from the `productsDb` (verify by calling endpoint after running the orchestration locally).
5. The root `README.md` has a new "Semantic Search" section that includes usage, env vars, SQL reference link, and example requests.
6. Unit/integration tests exist and pass locally and in CI. Any existing failing unit tests in the repository must be fixed as part of this work.

---

## Verification steps (local)

- Build the solution:

```pwsh
dotnet build src/eShopLite-AzFnc.slnx
```

- Run orchestration host:

```pwsh
dotnet run --project src/eShopAppHost
```

- Call the Function endpoint (PowerShell):

```pwsh
$body = @{ query = "wireless headphones"; top = 5 } | ConvertTo-Json
Invoke-RestMethod -Method POST -Uri "http://localhost:<port>/api/semanticsearch" -Body $body -ContentType "application/json"
```

- Run tests:

```pwsh
dotnet test src\SemanticSearch.Tests
```

---

## Examples & snippets (paste these into the prompt to guide implementation)

### A) Example local.settings.json

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ConnectionStrings__productsDb": "Server=localhost,1433;Database=productsDb;User Id=sa;Password=Your_password123;",
    "SEMANTIC_SEARCH_USE_EMBEDDINGS": "false"
  }
}
```

### B) HTTP contract (explicit)

POST /api/semanticsearch

Request JSON:

```json
{ "query": "string", "top": 10 }
```

Successful response (200):

```json
{
  "results": [
    {
      "id": 123,
      "title": "string",
      "score": 0.85,
      "snippet": "short text snippet",
      "metadata": { "category": "audio" }
    }
  ],
  "traceId": "<trace id>"
}
```

### C) SQL query pattern (parameterized template)

```sql
-- Pseudo SQL - adapt to SQL2025 scenario functions/stored procs
DECLARE @query NVARCHAR(MAX) = @p0;
DECLARE @top INT = @p1;

SELECT TOP(@top) p.Id, p.Title, p.Description,
  dbo.SemanticScoreFunction(p.Title, p.Description, @query) AS Score
FROM dbo.Products p
WHERE dbo.SemanticScoreFunction(p.Title, p.Description, @query) > 0
ORDER BY Score DESC;
```

### D) Repository DTO & interface

```csharp
public record SearchResult(int Id, string Title, double Score, string Snippet, Dictionary<string,string> Metadata);

public interface ISemanticSearchRepository
{
    Task<IEnumerable<SearchResult>> SearchAsync(string query, int top, CancellationToken ct);
}
```

### E) Unit test skeleton (MSTest)

```csharp
[TestMethod]
public async Task SearchRepository_ReturnsResults_WhenQueryMatches()
{
    // Arrange
    var repo = new SqlSemanticSearchRepository(connectionString);
    // Seed test data or mock ADO
    // Act
    var results = await repo.SearchAsync("wireless", 5, CancellationToken.None);
    // Assert
    Assert.IsTrue(results.Any());
    Assert.IsTrue(results.First().Score > 0);
}
```

### F) Sample GitHub Actions job (CI)

```yaml
name: CI
on: [push, pull_request]
jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 9.0.x
      - name: Restore
        run: dotnet restore src/eShopLite-AzFnc.slnx
      - name: Build
        run: dotnet build --configuration Release src/eShopLite-AzFnc.slnx --no-restore
      - name: Test
        run: dotnet test src --no-build --verbosity normal
```

## PR and delivery checklist

- One commit adds the new project and code.
- One commit adds tests.
- One commit updates docs (`docs/copilot-semanticsearch-prompt.md` and root `README.md`).
- CI passes (build + tests).
- PR description includes "How to test locally" and sample request/response.

If you want, I can also insert the exact SQL seed scripts or an integration test runner into the prompt; tell me and I will append them.
