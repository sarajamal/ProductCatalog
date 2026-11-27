# Gaps — McpServer

| ID | Severity | Gap | Plan | Status |
|---|---|---|---|---|
| GAP-001 | Medium | API has no filter/search endpoints; `get_products_by_category`, `search_products`, `low_stock_report` must filter all products in the MCP server. | Acceptable for small data; add API endpoints (ProductCatalogApi ENH-001/002). | Open |
| GAP-002 | Medium | `PUT /api/products/{id}` needs the full product; `update_stock` must read then write (race possible). | Add `PATCH .../stock` (ProductCatalogApi ENH-003). | Open |
| GAP-003 | Low | API depends on ProductCatalogApi GAP-001 (EF Tools version) for a clean CI build. | Fix in API first. | Open |
| GAP-004 | Low | `ModelContextProtocol` C# SDK version to pin. | Pinned to stable 2.4.0 in phase 2. | Closed |
| GAP-005 | Low | `get_products_by_category.category` is `string?`, so its JSON schema type is `["string","null"]`. MCP Inspector warns this is less portable (some clients accept only one type). | Change to `string category = ""` (plain optional string) and re-check the Inspector warning. | Open |
| GAP-006 | Low | When the API is running but answers 500, `EnsureSuccessStatusCode` throws `HttpRequestException` and `Run` reports "Could not reach ProductCatalogApi ... start-api.bat", which is misleading. Found in phase 5; pinned by test `ApiServerError500_IsReportedAsUnreachable_KnownGap006`. | In `Run`, check `HttpRequestException.StatusCode`: when it has a value, return "The API answered {code}" instead of the "could not reach" message; then update the test. | Open |
