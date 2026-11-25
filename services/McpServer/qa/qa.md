# QA — McpServer

## Phase 2 — read tools (2026-11-25, branch `feature/mcp-read-tools`, tested with MCP Inspector v2.13.0)
| ID | Case | Expected | Result | Evidence |
|---|---|---|---|---|
| QA-201 | `list_categories` | all categories | Pass — Electronics, Books, Clothing | — |
| QA-202 | `list_products` | all products with category | Pass — sorted by stock, category name included | evidence/20261125-QA-202-mcp-inspector-list-products.jpg |
| QA-203 | `get_product` existing id | product details | Not run yet | — |
| QA-204 | `get_product` missing id | "not found" text, no crash | Pass — "No product found with id 999." | — |
| QA-205 | `get_products_by_category` empty | every category with its products | Pass — 3 categories, products sorted by stock | — |
| QA-205b | `get_products_by_category` "books" (name, any case) | only Books | Pass — Product1, Clean Code Book | — |
| QA-205c | `get_products_by_category` "4" (missing id) | message with the available categories | Pass — "Category '4' does not exist. Available categories: 1 Electronics, 2 Books, 3 Clothing." | — |
| QA-206 | `search_products` "key" | matches "Mechanical Keyboard" (case-insensitive) | Pass | — |
| QA-207 | API stopped | readable error message | Not run yet | — |
| QA-208 | Project builds | `dotnet build src\McpServer` succeeds | Pass — 0 warnings, 0 errors (scripts\build-mcp.bat) | — |

## Phase 3 — Claude Desktop
| ID | Case | Expected | Result | Evidence |
|---|---|---|---|---|
| QA-301 | Tools appear in Claude Desktop | 5 tools listed | | |
| QA-302 | "What categories do we have?" | uses `list_categories` | | |

## Phase 4 — write tools
| ID | Case | Expected | Result | Evidence |
|---|---|---|---|---|
| QA-401 | `create_product` valid | 201, visible in Angular | | |
| QA-402 | `create_product` price 0 | validation message | | |
| QA-403 | `update_stock` | stock changed | | |
| QA-404 | `low_stock_report` threshold 5 | only stock ≤ 5 | | |
