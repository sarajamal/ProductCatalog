# Gaps — ProductCatalogApi

Recorded while documenting the existing code on 2026-11-17. Nothing here was changed in phase 1 (behaviour preserved).

| ID | Severity | Area | Gap | Proposed fix | Status |
|---|---|---|---|---|---|
| GAP-001 | High | csproj | `Microsoft.EntityFrameworkCore.Tools` is **10.0.11** while EF Core / Design are **8.0.26**. EF Tools 10 depends on Design 10, which targets .NET 10 — restore on `net8.0` may fail (NU1202), notably in CI. | Align Tools to `8.0.26`. Verify with `dotnet restore`. | Open |
| GAP-002 | Medium | Data | No EF Core migrations in the repo — a new developer cannot create the database. | Add `InitialCreate` migration (or a SQL script under `docs/`). | Open |
| GAP-003 | Medium | ProductsController | `PUT` does not validate price/stock/category and does not return 404 for a missing product (throws `DbUpdateConcurrencyException`). | Reuse create validation; check existence first. | Open |
| GAP-004 | Low | Program.cs | `AddControllers()` is called twice (second call adds JSON options). | Single `AddControllers().AddJsonOptions(...)`. | Open |
| GAP-005 | Low | Program.cs | `UseHttpsRedirection()` with the `http` profile only — harmless locally but misleading. | Keep for https profile or make conditional. | Open |
| GAP-006 | Low | ProductCatalogApi.http | Still contains the template `GET /weatherforecast` request. | Replace with products/categories examples. | Open |
| GAP-007 | Low | launchSettings | `http` profile binds `0.0.0.0:5233` (all interfaces). | Use `localhost` unless LAN access is needed. | Open |
| GAP-008 | Low | API design | Entities are returned directly (no DTOs). | Introduce DTOs before exposing more endpoints to MCP. | Open |
| GAP-009 | Low | swagger.json | A static `swagger.json` in the project folder lists only Products endpoints (outdated). | Delete or regenerate; Swagger UI is generated at runtime. | Open |
| GAP-010 | Medium | Tests | No unit tests. | Phase 5 (xUnit). | Planned |
