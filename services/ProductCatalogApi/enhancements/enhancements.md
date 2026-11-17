# Enhancements — ProductCatalogApi

| ID | Priority | Proposal | Benefit | Needed by | Status |
|---|---|---|---|---|---|
| ENH-001 | High | `GET /api/products?categoryId=&search=` query parameters | Lets MCP tools `get_products_by_category` and `search_products` filter on the server instead of loading everything | McpServer phase 2 | Idea |
| ENH-002 | Medium | `GET /api/products/low-stock?threshold=5` | Backs MCP `low_stock_report` | McpServer phase 4 | Idea |
| ENH-003 | Medium | `PATCH /api/products/{id}/stock` | Backs MCP `update_stock` without sending the whole product | McpServer phase 4 | Idea |
| ENH-004 | Low | Paging (`page`, `pageSize`) on list endpoints | Scales with data | — | Idea |
| ENH-005 | Low | Move API base URL in Angular to `environment.ts` | One place to change URLs | — | Idea |
| ENH-006 | Low | Categories CRUD | Manage categories from UI | — | Idea |
