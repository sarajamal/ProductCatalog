# Lessons Learned

Short write-ups of things we learned the hard way, so the next developer (or AI agent) does not repeat them.

## Naming

```
L-YYYYMMDD-short-name.md
```

- `YYYYMMDD` — the date the lesson was learned (e.g. `20261117`).
- `short-name` — 2–5 lowercase words with hyphens (e.g. `bat-files-need-crlf`).
- Example: `L-20261117-repo-restructure.md`

## How to add one

1. Copy [`templates/lesson.md`](../templates/lesson.md) into this folder with the new name.
2. Fill in context, what happened, root cause, the lesson, and the action taken.
3. If the lesson creates a rule, update the relevant file in `standards/` in the same PR.
4. Commit with `kb(Scope): add lesson <name>` and add it to the index below.

## Index

| ID | Service | Summary |
|---|---|---|
| [L-20261117-repo-restructure](L-20261117-repo-restructure.md) | Repo | Moving code into `src/` breaks hard-coded paths in scripts; use paths relative to the script (`%~dp0`). |
