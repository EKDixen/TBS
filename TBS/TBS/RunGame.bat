@echo off
start "" /wait conhost.exe cmd /c "cd /d %~dp0 && dotnet run"
