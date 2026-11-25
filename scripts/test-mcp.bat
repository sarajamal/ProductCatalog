@echo off
REM ============================================================
REM  test-mcp.bat
REM  Opens the MCP Inspector (a test page in the browser) connected
REM  to the MCP Server. Start the API first (start-api.bat).
REM  The Inspector address (with its access token) is written to
REM  .logs\mcp-inspector.log. Close the window to stop it.
REM ============================================================
setlocal
set "ROOT=%~dp0.."
if not exist "%ROOT%\.logs" mkdir "%ROOT%\.logs"
start "mcp-inspector" cmd /k "cd /d "%ROOT%" && npx -y @modelcontextprotocol/inspector dotnet run --project src\McpServer\McpServer.csproj > .logs\mcp-inspector.log 2>&1"
endlocal
