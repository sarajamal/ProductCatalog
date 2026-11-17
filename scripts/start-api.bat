@echo off
REM ============================================================
REM  start-api.bat
REM  Starts the API only (http://localhost:5233/swagger).
REM ============================================================
setlocal
set "ROOT=%~dp0.."
cd /d "%ROOT%\src\ProductCatalogApi"
dotnet run --launch-profile http
endlocal
