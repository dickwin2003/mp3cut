@echo off
echo MP3剪切合并工具 - 构建脚本
echo ================================

echo 检查.NET SDK...
dotnet --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo 错误：未找到.NET SDK
    echo 请先安装.NET 6.0 SDK：https://dotnet.microsoft.com/download/dotnet/6.0
    pause
    exit /b 1
)

echo 还原NuGet包...
dotnet restore
if %ERRORLEVEL% neq 0 (
    echo 错误：包还原失败
    pause
    exit /b 1
)

echo 编译项目...
dotnet build --configuration Release
if %ERRORLEVEL% neq 0 (
    echo 错误：编译失败
    pause
    exit /b 1
)

echo 编译成功！
echo 可执行文件位置：bin\Release\net6.0-windows\Mp3CutMerge.exe
echo.
echo 按任意键运行程序...
pause >nul

echo 启动程序...
dotnet run --configuration Release

pause
