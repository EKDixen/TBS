@echo off
echo Building TBS Launcher...
echo.

dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true

echo.
echo ========================================
echo Build Complete!
echo ========================================
echo.
echo Your launcher is ready at:
echo TBSLauncher\bin\Release\net9.0-windows\win-x64\publish\TBSLauncher.exe
echo.
echo Copy this file to your game folder and distribute it to users!
echo.
pause
