# tests

Unit test projects (xUnit), one per service:

| Project | Covers | Status |
|---|---|---|
| `tests/McpServer.Tests` | All 8 MCP tools and `ProductCatalogClient`, against a fake API (`FakeApiHandler`) — no network, no database | 48 tests (phase 5) |
| `tests/ProductCatalogApi.Tests` | API controllers with EF Core InMemory | Optional, not started |

## Run

```
dotnet test ProductCatalog.sln
```

CI runs the same command on every Pull Request (`.github/workflows/ci.yml`, step "Test").

> On the developer PC, Claude Desktop keeps `ProductCatalog.McpServer.exe` running, which locks the normal build output (MSB3027).
> Either stop it first (`scripts\build-mcp.bat` does this) or build to another folder:
> `dotnet test tests\McpServer.Tests --artifacts-path %TEMP%\pc-test`

## How the fake API works

```csharp
var handler = TestApi.WithCatalog()                       // GET api/categories + GET api/products with sample data
    .Returns("PUT", "api/products/9", HttpStatusCode.NoContent);
var result = await CatalogWriteTools.UpdateStock(TestApi.Client(handler), 9, 30, CancellationToken.None);
Assert.Single(handler.Sent("PUT", "api/products/9"));     // every request is recorded
```
