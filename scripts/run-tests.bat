@echo off
REM ============================================================
REM  run-tests.bat
REM  Runs the Angular unit tests (npm test) in a new window.
REM  Karma opens Chrome with the results.
REM  The same results are available at http://localhost:9876/debug.html
REM  Close the window (or Ctrl+C) to stop.
REM ============================================================
setlocal
set "ROOT=%~dp0.."
echo Running Angular unit tests ...
start "product-catalog-tests" cmd /k "cd /d "%ROOT%\src\product-catalog-client" && npm test"
endlocal
