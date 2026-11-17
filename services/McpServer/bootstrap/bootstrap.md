# Bootstrap — McpServer

> The project is created in phase 2. These are the planned steps.

## Prerequisites
- .NET 8 SDK
- ProductCatalogApi running on http://localhost:5233 (`scripts\start-api.bat`)
- Claude Desktop (phase 3)
- Node.js (only for MCP Inspector)

## Build and run
```bash
dotnet build src/McpServer
dotnet run --project src/McpServer
```

## Test with MCP Inspector
```bash
npx @modelcontextprotocol/inspector dotnet run --project src/McpServer
```

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
