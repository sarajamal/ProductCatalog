# SRS — ProductCatalogApi

| Field | Value |
|---|---|
| Service | ProductCatalogApi + product-catalog-client |
| Owner | @sarajamal |
| Version / date | 1.0 — 2026-11-17 (documents the existing system) |
| Status | Approved (as-built) |

## 1. Purpose
Manage a catalog of products grouped by categories: list, view, add, edit and delete products through a REST API and an Angular UI. It is also the backend that the MCP Server will wrap so Claude can query and manage the catalog.

## 2. Scope
In scope: products CRUD, read-only categories, Angular UI for products.
Out of scope (today): authentication, categories CRUD, paging, search/filter endpoints.

## 3. Actors
- Catalog user (Angular UI)
- Developer (Swagger)
- MCP Server (planned client of the API)

## 4. Functional requirements
| ID | Requirement | Endpoint | Status |
|---|---|---|---|
| FR-01 | List all products with their category | `GET /api/products` | Done |
| FR-02 | Get one product by id (404 if missing) | `GET /api/products/{id}` | Done |
| FR-03 | Create a product; price > 0, stock > 0, category required | `POST /api/products` → 201 | Done |
| FR-04 | Update a product (400 if id mismatch) | `PUT /api/products/{id}` → 204 | Done |
| FR-05 | Delete a product (404 if missing) | `DELETE /api/products/{id}` → 204 | Done |
| FR-06 | List categories | `GET /api/categories` | Done |
| FR-07 | Get one category by id | `GET /api/categories/{id}` | Done |
| FR-08 | UI: product list with refresh, edit, delete | Angular `product-list` | Done |
| FR-09 | UI: add / edit product form with category dropdown | Angular `product-form` | Done |

## 5. Non-functional requirements
| ID | Requirement |
|---|---|
| NFR-01 | CORS allows only `http://localhost:4200` (policy `AngularClient`) |
| NFR-02 | Connection string is not stored in source; it comes from user secrets |
| NFR-03 | JSON serialization ignores reference cycles (Product ↔ Category) |
| NFR-04 | Swagger enabled in Development |

## 6. Data model
| Entity | Fields |
|---|---|
| Category | `CategoryId` (int, PK), `CategoryName` (string), `Products` (nav) |
| Product | `ProductId` (int, PK), `ProductName` (string), `Price` (decimal), `Stock` (int), `CategoryId` (FK), `Category` (nav) |

DbContext: `AppDbContext` with `Products` and `Categories` (SQL Server).

## 7. Acceptance criteria
- All endpoints visible and callable in Swagger.
- Angular client lists, adds, edits and deletes products against the running API.
- Invalid create requests return `400` with a problem-details body listing each invalid field.
