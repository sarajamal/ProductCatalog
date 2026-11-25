# Plan — McpServer

| # | Phase | Status | Branch |
|---|---|---|---|
| 1 | Repository structure and Git | Done (branch protection still to enable) | `main` (initial commit) |
| 2 | MCP Server — read tools | Done (2026-11-25) | `feature/mcp-read-tools` |
| 3 | Connect to Claude Desktop | Planned | `feature/mcp-claude-desktop` |
| 4 | MCP Server — write tools | Planned | `feature/mcp-write-tools` |
| 5 | Unit tests (xUnit) | Planned | `feature/mcp-unit-tests` |
| 6 | Documentation | Planned | `kb/mcp-docs` |

---

## Phase 1 — Repository structure and Git
**Goal:** a clean, multi-developer repository organized as a knowledge base.
- [x] Backup (`.backup/pre-restructure-2026-11-17.tar.gz`)
- [x] Code moved to `src/`, guide PDF to `docs/guides/`
- [x] Scripts in `scripts/` (CRLF, paths relative to script)
- [x] README, CLAUDE.md, AGENTS.md, kb.json, .gitignore, .gitattributes, .editorconfig, CONTRIBUTING.md, ProductCatalog.sln
- [x] `.github/` (CODEOWNERS, PR template, issue templates, CI)
- [x] `standards/`, `templates/`, `lessons/`, `services/`
- [x] `git init` on `main`, first commit, remote `origin`
- [ ] Push and enable branch protection (Sarah)

**Done when:** pushed to GitHub and CI runs.

## Phase 2 — MCP Server (read tools)
**Goal:** a C# MCP server exposing read-only tools.
- [x] `src/McpServer` console project (net8.0) added to `ProductCatalog.sln`
- [x] Packages: `ModelContextProtocol` 2.4.0 (stable), `Microsoft.Extensions.Hosting`, `Microsoft.Extensions.Http`
- [x] `Program.cs`: Host + `AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly()`; logging to stderr
- [x] `ProductCatalogClient` (typed HttpClient, base URL from config)
- [x] Tools: `list_categories`, `list_products`, `get_product`, `get_products_by_category`, `search_products`
- [x] Test with MCP Inspector (`scripts\test-mcp.bat`)

**Done when:** all five tools return real data from the running API in MCP Inspector.

## Phase 3 — Connect to Claude Desktop
- [ ] Add server to `%APPDATA%\Claude\claude_desktop_config.json` (see bootstrap)
- [ ] Restart Claude Desktop, confirm tools appear
- [ ] Ask 5 real questions; save screenshots in `evidence/`

**Done when:** Claude answers catalog questions using the tools.

## Phase 4 — Write tools
- [ ] `create_product` (validation: name, price > 0, stock > 0, category exists)
- [ ] `update_stock` (GET product → set stock → PUT)
- [ ] `low_stock_report` (threshold, default 5)
- [ ] Optional API support endpoints (ENH-001..003 in ProductCatalogApi)

**Done when:** Claude can add a product and update stock, visible in the Angular UI.

## Phase 5 — Unit tests (xUnit)
- [ ] `tests/McpServer.Tests` (xUnit) added to the solution
- [ ] Fake `HttpMessageHandler` to simulate API responses (200 / 404 / 400)
- [ ] Tests for every tool + validation paths
- [ ] Optional: `tests/ProductCatalogApi.Tests` with EF Core InMemory for controllers
- [ ] `dotnet test` green in CI

**Done when:** CI runs tests on every PR.

## Phase 6 — Documentation
- [ ] Final SRS and README for McpServer
- [ ] QA results + evidence for all phases
- [ ] Lessons learned
- [ ] Short demo / summary for the annual review goal ("Developing Web Application Through Vibe Coding")
