# Bootstrap — McpServer

> Created in phase 2 (2026-11-25). Source: `src/McpServer`.

## Prerequisites
- .NET 8 SDK
- ProductCatalogApi running on http://localhost:5233 (`scripts\start-api.bat`)
- Claude Desktop (phase 3)
- Node.js (only for MCP Inspector)

## Build and run
- Double-click `scripts\build-mcp.bat` (output also saved to `.logs\mcp-build.log`), or:
```bash
dotnet build src/McpServer
dotnet run --project src/McpServer
```

## Test with MCP Inspector
1. Start the API: `scripts\start-api.bat`
2. Double-click `scripts\test-mcp.bat`
3. Open the address written in `.logs\mcp-inspector.log` (it contains the access token)
4. Switch the server on (Connected) → Tools tab → pick a tool → Execute Tool

## Claude Desktop config (phase 3)
File: `%APPDATA%\Claude\claude_desktop_config.json`
```json
{
  "mcpServers": {
    "product-catalog": {
      "command": "dotnet",
      "args": ["run", "--project", "C:\\Users\\Lenovo\\ProductCatalogApi\\src\\McpServer", "--no-build"],
      "env": { "ProductCatalogApi__BaseUrl": "http://localhost:5233" }
    }
  }
}
```
Build once before starting Claude Desktop (`--no-build` keeps startup fast and stdout clean).
