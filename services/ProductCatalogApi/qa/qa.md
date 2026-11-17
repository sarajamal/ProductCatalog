# QA — ProductCatalogApi

## Phase 1 — Repository restructure (2026-11-17)

| ID | Case | Expected | Result | Evidence |
|---|---|---|---|---|
| QA-01 | Angular client builds from new path `src/product-catalog-client` | `npm run build` succeeds | See phase 1 verification in [evidence](../evidence/README.md) | evidence/README.md |
| QA-02 | API builds from new path | `dotnet run` from `src/ProductCatalogApi` succeeds | Pass (2026-11-17, started by start-dev.bat, data loaded in client) | — |
| QA-03 | `scripts\start-dev.bat` opens two windows and both apps start | API on 5233, client on 4200 | Pass (2026-11-17, 6 products + 3 categories shown, no console errors) | — |
| QA-04 | `.bat` files have CRLF endings | `file *.bat` shows CRLF | Pass | evidence/README.md |
| QA-05 | No secrets in tracked files | connection string empty in `appsettings.json` | Pass | — |

## Client — lighter page background (2026-11-17, branch `feature/client-lighter-background`)

| ID | Case | Expected | Result | Evidence |
|---|---|---|---|---|
| QA-06 | Page background gradient is lighter; title and cards still readable | Lighter purple/blue gradient, white title readable | Pass — verified in Chrome at http://localhost:4200 | evidence/20261117-QA-06-lighter-background.jpg |

## Regression checklist (run after any API change)
- [ ] `GET /api/products` returns list with categories
- [ ] `POST /api/products` with price 0 → 400 with `Price` error
- [ ] `PUT /api/products/{id}` → 204
- [ ] `DELETE /api/products/{id}` → 204, then `GET` → 404
- [ ] `GET /api/categories` returns list
- [ ] Angular list / add / edit / delete work at http://localhost:4200
