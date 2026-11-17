# API Conventions

## Routes

- Base: `api/[controller]` → plural, lowercase in URLs: `/api/products`, `/api/categories`.
- Resource by id: `/api/products/{id}`.
- Filters as query parameters: `/api/products?categoryId=2&search=phone`.

## HTTP verbs and status codes

| Action | Verb | Success | Common errors |
|---|---|---|---|
| List | GET | 200 | — |
| Get by id | GET | 200 | 404 |
| Create | POST | 201 + `Location` header (`CreatedAtAction`) | 400 (validation) |
| Update | PUT | 204 | 400 (id mismatch / validation), 404 |
| Delete | DELETE | 204 | 404 |

## Errors

- Validation errors use `ValidationProblem(ModelState)` → RFC 7807 `application/problem+json`.
- Do not return exception messages or stack traces to clients.

## Payloads

- JSON, camelCase property names (System.Text.Json default).
- Navigation cycles are ignored (`ReferenceHandler.IgnoreCycles`) — prefer DTOs for new endpoints.
- Money: `decimal`. Ids: `int` named `<Entity>Id` (`ProductId`, `CategoryId`).

## Documentation

- Every endpoint appears in Swagger (`/swagger` in Development).
- Keep `src/ProductCatalogApi/ProductCatalogApi.http` up to date with example requests.

## MCP tools mapping

MCP tools wrap these endpoints 1:1 where possible (see `services/McpServer/srs/srs.md`). A new endpoint needed by an MCP tool is added to the API first, with its own PR.
