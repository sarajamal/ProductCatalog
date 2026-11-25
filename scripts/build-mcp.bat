@echo off
REM ============================================================
REM  build-mcp.bat
REM  Restores and builds the MCP Server (src\McpServer).
REM  Output is shown in the window and saved to .logs\mcp-build.log
REM ============================================================
setlocal
set "ROOT=%~dp0.."
if not exist "%ROOT%\.logs" mkdir "%ROOT%\.logs"
start "mcp-build" cmd /k "cd /d "%ROOT%" && dotnet build src\McpServer\McpServer.csproj > .logs\mcp-build.log 2>&1 & type .logs\mcp-build.log"
endlocal
