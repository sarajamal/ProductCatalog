# SRS — McpServer

| Field | Value |
|---|---|
| Service | McpServer |
| Owner | @sarajamal |
| Version / date | 0.1 — 2026-11-17 |
| Status | Draft |

## 1. Purpose
Let Claude read and manage the product catalog through MCP tools that call ProductCatalogApi, instead of copy-pasting data into the chat.

## 2. Scope
In scope: read tools (phase 2), Claude Desktop integration (phase 3), write tools (phase 4), unit tests (phase 5).
Out of scope: direct database access (the server only talks to the API), authentication (local use).

## 3. Tools
| Tool | Phase | Parameters | Calls | Returns |
|---|---|---|---|---|
| `list_categories` | 2 | — | `GET /api/categories` | id + name list |
| `list_products` | 2 | — | `GET /api/products` | products with category, price, stock |
| `get_product` | 2 | `productId:int` | `GET /api/products/{id}` | product or "not found" |
| `get_products_by_category` | 2 | `categoryId:int` | `GET /api/products` (+ filter; ENH-001 in API later) | products in category |
| `search_products` | 2 | `query:string` | `GET /api/products` (+ name contains, case-insensitive) | matching products |
| `create_product` | 4 | `name, price, stock, categoryId` | `POST /api/products` | created product or validation errors |
| `update_stock` | 4 | `productId:int, stock:int` | `GET` then `PUT /api/products/{id}` | updated product |
| `low_stock_report` | 4 | `threshold:int = 5` | `GET /api/products` (+ filter) | products with stock ≤ threshold |

## 4. Functional requirements
| ID | Requirement |
|---|---|
| FR-01 | Each tool has a clear `[Description]` so Claude knows when to use it. |
| FR-02 | API errors (404, 400) are returned as readable text, not exceptions. |
| FR-03 | API base URL is configurable (`ProductCatalogApi:BaseUrl`, default `http://localhost:5233`). |
| FR-04 | Write tools validate input before calling the API (price > 0, stock ≥ 0). |

## 5. Non-functional requirements
| ID | Requirement |
|---|---|
| NFR-01 | stdio transport; logs go to **stderr** only (stdout is reserved for MCP messages). |
| NFR-02 | Uses `IHttpClientFactory` / typed `HttpClient`. |
| NFR-03 | Business logic in a service class (`ProductCatalogClient`) so it is unit-testable with a fake `HttpMessageHandler`. |

## 6. Acceptance criteria
- Claude Desktop lists the tools and answers catalog questions using real API data.
- All tools covered by xUnit tests (phase 5).
