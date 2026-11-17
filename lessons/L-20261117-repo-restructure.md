# L-20261117-repo-restructure

| Field | Value |
|---|---|
| Date | 2026-11-17 |
| Service | Repo |
| Author | Sarah Alkhuraiji |
| Tags | git, scripts, structure |

## Context
The project was restructured into a knowledge-base repository (`src/`, `services/`, `standards/`, ...).

## What happened
The old `.bat` files in the root used paths like `cd product-catalog-client`. After moving the code into `src/`, those paths no longer existed. A first restructuring attempt was also interrupted midway.

## Root cause
Paths were relative to the current working directory instead of to the script location; and the change was not done in small, recoverable steps.

## Lesson
- In `.bat` scripts, build paths from the script's own folder: `set "ROOT=%~dp0.."`.
- Before any structural change, take a backup (`.backup/...tar.gz`) and work in small steps that can be resumed.
- `.bat` files must stay CRLF — enforced with `.gitattributes` (`*.bat text eol=crlf`).

## Action taken / rule added
- Scripts rewritten in `scripts/` using `%~dp0`.
- `.gitattributes` and `.editorconfig` added.
