# McpServer — Knowledge Base

| Field | Value |
|---|---|
| Source | `src/McpServer` (created in phase 2) |
| Stack | .NET 8, C#, Model Context Protocol C# SDK (`ModelContextProtocol` NuGet), stdio transport |
| Talks to | ProductCatalogApi over HTTP (http://localhost:5233) |
| Consumer | Claude Desktop (and any MCP client) |
| Owner | @sarajamal |
| Status | Planned — phase 1 in progress |

## What it is
A small C# console app that exposes the ProductCatalog API as **MCP tools**, so Claude can answer questions like "which products are low on stock?" or "add a product" by calling the real API.

```
Claude Desktop ──(MCP / stdio)──> McpServer ──(HTTP JSON)──> ProductCatalogApi ──> SQL Server
```

## Contents
| Folder | What's inside |
|---|---|
| [srs/](srs/srs.md) | Tools and requirements |
| [plan/](plan/plan.md) | The six phases |
| [bootstrap/](bootstrap/bootstrap.md) | Setup and Claude Desktop config |
| [gaps/](gaps/gaps.md) | Open questions / gaps |
| [enhancements/](enhancements/enhancements.md) | Later ideas |
| [qa/](qa/qa.md) | Test cases per phase |
| [evidence/](evidence/README.md) | Proof of QA |
| [reviews/](reviews/README.md) | Review records |
