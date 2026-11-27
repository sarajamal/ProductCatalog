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
| QA-207 | API stopped | readable error message | Pass — see QA-405 | — |
| QA-208 | Project builds | `dotnet build src\McpServer` succeeds | Pass — 0 warnings, 0 errors (scripts\build-mcp.bat) | — |

## Phase 3 — Claude Desktop
| ID | Case | Expected | Result | Evidence |
|---|---|---|---|---|
| QA-301 | Server loads in Claude Desktop | `product-catalog` connected, tools listed | Pass — state "announced" after setup-claude-mcp.bat writes `%LOCALAPPDATA%\Claude-Data\<package>\claude_desktop_config.json` and Claude is fully restarted | — |
| QA-302 | "Which products have less than 20 in stock?" | uses `list_products`, live data | Pass — Mechanical Keyboard (16), Product3 (19) | — |

## Phase 4 — write tools
| ID | Case | Expected | Result | Evidence |
|---|---|---|---|---|
| QA-401 | `create_product` valid (MCP Test Cable, 10, 5, Electronics) | product created | Pass — id 9 | — |
| QA-401b | `create_product` same name, other case | refused, no duplicate | Pass — "'MCP Test Cable' already exists (id 9...)" | — |
| QA-402 | `create_product` price 0, category "Toys" | validation message, nothing created | Pass — "Price must be greater than 0." | — |
| QA-403 | `update_stock` id 9 → 25 | stock changed | Pass — "5 -> 25" | — |
| QA-403b | `update_stock` id 999 | "not found", nothing changed | Pass | — |
| QA-404 | `low_stock_report` default (20) | stock < 20, lowest first, count per category | Pass — 3 products, Electronics 3 | — |
| QA-405 | Tool call with API stopped | readable message | Pass — "Could not reach ProductCatalogApi ... scripts\start-api.bat" (also closes QA-207) | — |
| QA-406 | New product visible in Angular UI | listed on products page | Pass (2026-11-27) — "MCP Test Cable, Electronics, 10.00, 25" on http://localhost:4200 in Chrome, sorted by stock, no low-stock badge (25 ≥ 20) | `evidence/20261127-QA-406-angular-mcp-test-cable.jpg` |
| QA-407 | Build while Claude Desktop runs the server | build succeeds | Pass — build-mcp.bat now stops the running ProductCatalog.McpServer.exe first | — |

## Phase 5 — unit tests (`tests/McpServer.Tests`, xUnit, fake API)
| ID | Case | Expected | Result | Evidence |
|---|---|---|---|---|
| QA-501 | `ProductCatalogClient` (6 tests) | 404 → null; 400/404 bodies → one readable line; PUT sends product without its Category object | Pass | `evidence/20261127-phase5-dotnet-test.txt` |
| QA-502 | `list_categories`, `list_products` | all categories; products lowest stock first | Pass | same |
| QA-503 | `get_product`, `get_products_by_category` | found / not found; category by id or name in any case, empty = all, unknown lists available, empty category says so | Pass | same |
| QA-504 | `search_products` | blank asks for text, case-insensitive, no match says so | Pass | same |
| QA-505 | `create_product` | every validation message at once with no API call; unknown category and duplicate name refused with no POST; valid POST sends trimmed name + category id; API refusal returned | Pass | same |
| QA-506 | `update_stock` | negative refused with no call; unknown id; same stock sends no PUT; old -> new; low-stock note; API refusal returned | Pass | same |
| QA-507 | `low_stock_report` | threshold ≤ 0 refused; default 20 lowest first with count per category; custom threshold; none below | Pass | same |
| QA-508 | API down / slow / 500, all 8 tools | readable message with `scripts\start-api.bat`; timeout message; 500 pinned as GAP-006 | Pass | same |
| QA-509 | Whole solution, Release (same as CI) | `dotnet test ProductCatalog.sln -c Release` green, 0 warnings | Pass — 48/48 | same |
