# Gaps — McpServer

| ID | Severity | Gap | Plan | Status |
|---|---|---|---|---|
| GAP-001 | Medium | API has no filter/search endpoints; `get_products_by_category`, `search_products`, `low_stock_report` must filter all products in the MCP server. | Acceptable for small data; add API endpoints (ProductCatalogApi ENH-001/002). | Open |
| GAP-002 | Medium | `PUT /api/products/{id}` needs the full product; `update_stock` must read then write (race possible). | Add `PATCH .../stock` (ProductCatalogApi ENH-003). | Open |
| GAP-003 | Low | API depends on ProductCatalogApi GAP-001 (EF Tools version) for a clean CI build. | Fix in API first. | Open |
| GAP-004 | Low | `ModelContextProtocol` C# SDK version to pin. | Pinned to stable 2.4.0 in phase 2. | Closed |
