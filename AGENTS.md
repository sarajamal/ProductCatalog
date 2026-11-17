# AGENTS.md

Instructions for any AI coding agent (Claude, Copilot, Codex, ...) working in this repository.
The full working agreement (Arabic) is in [CLAUDE.md](CLAUDE.md) — it applies to all agents.

## Orientation

1. Read `kb.json` — current phase, services, owners.
2. Read `services/<Service>/README.md` and its `plan/plan.md` for the service you touch.
3. Follow `standards/` (coding, API, commits, branching).

## Commands

| Task | Command |
|---|---|
| Build everything (.NET) | `dotnet build ProductCatalog.sln` |
| Run API | `cd src/ProductCatalogApi && dotnet run --launch-profile http` |
| Run client | `cd src/product-catalog-client && npm start` |
| Build client | `cd src/product-catalog-client && npm run build` |
| Tests (.NET, from phase 5) | `dotnet test ProductCatalog.sln` |

## Rules

- Work on a branch (`feature/`, `fix/`, `kb/`), never directly on `main`.
- Commit format: `feat(Scope): ...`, `fix(Scope): ...`, `kb(Scope): ...`.
- Never commit secrets. The DB connection string lives in user secrets.
- Do not change existing behaviour, the database schema or data without explaining and getting approval.
- Keep `.bat` files with CRLF line endings.
- After a change: build, run, verify, then update the service KB (`qa/`, `evidence/`, `gaps/`) and report which files changed, what changed, and the verification result.
