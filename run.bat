@echo off
chcp 65001 >nul
echo MP3 Cut Merge Tool
echo ==================

echo Checking for .NET SDK...
dotnet --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo .NET SDK not found!
    echo Please install .NET 6.0 SDK from: https://dotnet.microsoft.com/download/dotnet/6.0
    echo.
    echo Alternative: Open Mp3CutMerge_Framework.csproj with Visual Studio
    pause
    exit /b 1
)

echo Restoring packages...
dotnet restore

echo Building project...
dotnet build --configuration Release

echo Running application...
dotnet run --configuration Release

pause
