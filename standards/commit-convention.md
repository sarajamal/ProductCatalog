# Commit Convention

Format:

```
<type>(<Scope>): <short summary in imperative mood>

[optional body: what and why]

[optional footer: Closes #12]
```

## Types

| Type | Use for |
|---|---|
| `feat` | New functionality |
| `fix` | Bug fix |
| `kb` | Knowledge base / documentation only (srs, plan, qa, lessons, standards, README) |
| `test` | Adding or fixing tests only |
| `refactor` | Code change with no behaviour change |
| `chore` | Build, CI, tooling, dependencies |

## Scopes

`ProductCatalogApi`, `Client`, `McpServer`, `KB`, `Repo`, `CI`

## Examples

```
feat(McpServer): add list_products tool
fix(ProductCatalogApi): return 404 when updating a missing product
kb(McpServer): add phase 2 QA evidence
chore(Repo): restructure repository as knowledge base
```

## Rules

- Summary ≤ 72 characters, no trailing period.
- One logical change per commit.
- Reference the issue in the footer when there is one.
