# Contributing to ProductCatalog

## Workflow

1. Pick or open an issue (bug or feature template).
2. Create a branch from `main`:
   - `feature/<scope>-<short-name>` — new functionality
   - `fix/<scope>-<short-name>` — bug fix
   - `kb/<scope>-<short-name>` — documentation / knowledge base only
3. Commit using the convention in [standards/commit-convention.md](standards/commit-convention.md):
   `feat(Scope): ...`, `fix(Scope): ...`, `kb(Scope): ...`
4. Build and test locally:
   ```bash
   dotnet build ProductCatalog.sln
   cd src/product-catalog-client && npm run build
   ```
5. Open a Pull Request to `main` and fill in the PR template.
6. At least one review from a code owner (see `.github/CODEOWNERS`) and a green CI are required to merge.

## Definition of Done

- Code follows [standards/coding-standards.md](standards/coding-standards.md) and [standards/api-conventions.md](standards/api-conventions.md).
- Builds pass (.NET + Angular); tests added/updated when logic changes.
- The service knowledge base is updated: `plan/`, `qa/`, `evidence/`, and `gaps/` or `enhancements/` when relevant.
- A lesson is added in `lessons/` when something non-obvious was learned.
- No secrets committed (use `dotnet user-secrets`).

## Local setup

See [README.md](README.md#first-time-setup).
