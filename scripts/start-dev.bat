@echo off
REM ============================================================
REM  start-dev.bat
REM  Starts the API (http://localhost:5233) and the Angular
REM  client (http://localhost:4200) in two separate windows.
REM ============================================================
setlocal
set "ROOT=%~dp0.."

echo Starting ProductCatalogApi (http://localhost:5233) ...
start "ProductCatalogApi" cmd /k "cd /d "%ROOT%\src\ProductCatalogApi" && dotnet run --launch-profile http"

echo Starting product-catalog-client (http://localhost:4200) ...
start "product-catalog-client" cmd /k "cd /d "%ROOT%\src\product-catalog-client" && npm start"

echo.
echo Both windows are starting. Swagger: http://localhost:5233/swagger
endlocal
