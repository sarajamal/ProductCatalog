# Branching Strategy

## Branches

| Branch | Purpose |
|---|---|
| `main` | Always buildable; protected; only updated through Pull Requests |
| `feature/<scope>-<short-name>` | New functionality — e.g. `feature/mcp-read-tools` |
| `fix/<scope>-<short-name>` | Bug fixes — e.g. `fix/api-update-404` |
| `kb/<scope>-<short-name>` | Docs / knowledge base only — e.g. `kb/mcp-phase2-qa` |

## Flow

1. `git switch main && git pull`
2. `git switch -c feature/mcp-read-tools`
3. Commit with the [commit convention](commit-convention.md).
4. `git push -u origin feature/mcp-read-tools`
5. Open a **Pull Request** to `main` using the PR template.
6. CI must pass and a code owner must approve.
7. **Squash and merge**, then delete the branch.

## Branch protection for `main` (GitHub → Settings → Branches → Add rule)

- Require a pull request before merging
- Require approvals: 1 (code owners)
- Require status checks to pass: `.NET build`, `Angular build`
- Require branches to be up to date before merging
- Do not allow force pushes or deletions

> While the repository has a single developer, the approval requirement can be relaxed; keep the PR + CI requirement.
