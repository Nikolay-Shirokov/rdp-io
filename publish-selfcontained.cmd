@echo off
setlocal enabledelayedexpansion
REM ==============================================================================
REM TextSimulator - Self-contained single-file publish (без .NET на целевой машине)
REM ==============================================================================
REM Собирает переносимый EXE для машин без установленного .NET.
REM Результат: publish-selfcontained\TextSimulator.App.exe
REM Требуется .NET SDK на машине сборки.
REM ==============================================================================

echo ========================================
echo TextSimulator - Self-contained Publish
echo ========================================
echo.

set PROJECT_PATH=src\TextSimulator.App\TextSimulator.App.csproj
set OUTPUT_DIR=publish-selfcontained
set RID=win-x64

if exist "%OUTPUT_DIR%" (
    rmdir /s /q "%OUTPUT_DIR%"
)

echo Публикация self-contained single-file...

dotnet publish "%PROJECT_PATH%" ^
    --configuration Release ^
    --runtime %RID% ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -p:EnableCompressionInSingleFile=true ^
    -p:PublishTrimmed=false ^
    -p:DebugType=None ^
    -p:DebugSymbols=false ^
    --output "%OUTPUT_DIR%"

if errorlevel 1 (
    echo.
    echo [ОШИБКА] Публикация не удалась.
    exit /b 1
)

if not exist "%OUTPUT_DIR%\TextSimulator.App.exe" (
    echo.
    echo [ОШИБКА] Итоговый EXE не найден в %OUTPUT_DIR%.
    exit /b 1
)

for %%A in ("%OUTPUT_DIR%\TextSimulator.App.exe") do (
    set SIZE=%%~zA
)
set /a SIZE_MB=!SIZE! / 1048576

echo.
echo ========================================
echo Публикация завершена.
echo Выходной файл: %OUTPUT_DIR%\TextSimulator.App.exe
if defined SIZE_MB echo Размер: !SIZE_MB! MB

endlocal
