@echo off
REM Opens the Angular client in Chrome.
start "" chrome "http://localhost:4200"
if errorlevel 1 start "" "http://localhost:4200"
