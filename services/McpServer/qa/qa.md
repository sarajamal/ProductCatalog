# QA — McpServer

## Phase 2 — read tools (planned)
| ID | Case | Expected | Result | Evidence |
|---|---|---|---|---|
| QA-201 | `list_categories` | all categories | | |
| QA-202 | `list_products` | all products with category | | |
| QA-203 | `get_product` existing id | product details | | |
| QA-204 | `get_product` missing id | "not found" text, no crash | | |
| QA-205 | `get_products_by_category` | only that category | | |
| QA-206 | `search_products` "lap" | matches "Laptop" (case-insensitive) | | |
| QA-207 | API stopped | readable error message | | |

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
