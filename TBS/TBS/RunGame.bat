@echo off
if exist "%~dp0TBS.exe" (
    start "" /wait conhost.exe cmd /c "cd /d %~dp0 && TBS.exe"
) else if exist "%~dp0..\bin\Release\net8.0\win-x64\publish\TBS.exe" (
    start "" /wait conhost.exe cmd /c "cd /d %~dp0..\bin\Release\net8.0\win-x64\publish && TBS.exe"
) else (
    start "" /wait conhost.exe cmd /c "cd /d %~dp0 && dotnet run"
)
