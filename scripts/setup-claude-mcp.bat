@echo off
REM ============================================================
REM  setup-claude-mcp.bat
REM  One click: adds the ProductCatalog MCP Server to Claude Desktop.
REM  Result is also saved to .logs\setup-claude-mcp.log
REM ============================================================
setlocal
set "ROOT=%~dp0.."
powershell -NoProfile -ExecutionPolicy Bypass -File "%ROOT%\scripts\setup-claude-mcp.ps1"
echo.
pause
endlocal
