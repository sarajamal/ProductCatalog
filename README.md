# ProductCatalog

Product catalog system: an ASP.NET Core Web API (.NET 8 + EF Core + SQL Server) with an Angular 18 client, plus a planned C# **MCP Server** that wraps the API so Claude can use it as tools.

The repository is organized as a **Knowledge Base**: every service has its own SRS, plan, QA, reviews, gaps and enhancements next to the code.

## Repository layout

```
ProductCatalog/
├── src/
│   ├── ProductCatalogApi/         # ASP.NET Core Web API (.NET 8, EF Core, SQL Server)
│   ├── product-catalog-client/    # Angular 18 standalone client
│   └── McpServer/                 # C# MCP Server: Claude uses the API as tools
├── tests/                         # Unit tests (xUnit) — MCP phase 5
├── services/                      # Knowledge base per service
│   ├── ProductCatalogApi/
│   └── McpServer/
├── standards/                     # Coding, API, commit and branching standards
├── templates/                     # SRS, plan, QA, review, lesson, gap, enhancement
├── lessons/                       # Lessons learned (L-YYYYMMDD-short-name)
├── docs/guides/                   # Guides (PDF)
├── scripts/                       # Windows run scripts (.bat)
├── .github/                       # CODEOWNERS, PR/issue templates, CI
├── kb.json                        # Machine-readable index: projects, phase, owners
├── CLAUDE.md / AGENTS.md          # Instructions for AI assistants
└── ProductCatalog.sln
```

## Prerequisites

- .NET 8 SDK
- Node.js 20+ and npm
- SQL Server (LocalDB / Express / Developer)
- Angular CLI 18 (`npm i -g @angular/cli@18`, optional — `npm start` uses the local CLI)

## First-time setup

```bash
# 1) Database connection string (kept out of Git, in user secrets)
cd src/ProductCatalogApi
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=ProductCatalogDb;Trusted_Connection=True;TrustServerCertificate=True"

# 2) Angular dependencies
cd ../product-catalog-client
npm install
```

See `src/ProductCatalogApi/appsettings.Development.example.json` for the expected settings shape.

## Running

| Script | What it does |
|---|---|
| `scripts\start-dev.bat` | API + Angular in two windows |
| `scripts\start-api.bat` | API only |
| `scripts\open-in-chrome.bat` | Opens http://localhost:4200 |
| `scripts\open-in-vscode.bat` | Opens the repo root in VS Code |
| `scripts\run-tests.bat` | Runs the Angular unit tests; Karma opens Chrome with the results |
| `scripts\build-mcp.bat` | Builds the MCP Server (log in `.logs\mcp-build.log`) |
| `scripts\test-mcp.bat` | Opens the MCP Inspector to try the MCP tools (start the API first) |

- API: http://localhost:5233 — Swagger: http://localhost:5233/swagger
- Client: http://localhost:4200

## Build

```bash
dotnet build ProductCatalog.sln
cd src/product-catalog-client && npm run build
```

## Contributing

Read [CONTRIBUTING.md](CONTRIBUTING.md) and the files in [standards/](standards/). Branches: `feature/`, `fix/`, `kb/`. Commits: `feat(Scope): ...`, `fix(Scope): ...`, `kb(Scope): ...`.

## Roadmap

The current phase and owners are in [kb.json](kb.json). The MCP Server roadmap is in [services/McpServer/plan/plan.md](services/McpServer/plan/plan.md).
