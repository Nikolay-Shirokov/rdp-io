@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
REM ==============================================================================
REM rdp-io - Self-contained single-file publish (без .NET на целевой машине)
REM ==============================================================================
REM Собирает переносимый EXE для машин без установленного .NET.
REM Результат: publish-selfcontained\RdpIo.App.exe
REM Требуется .NET SDK на машине сборки.
REM ==============================================================================

echo ========================================
echo rdp-io - Self-contained Publish
echo ========================================
echo.

set PROJECT_PATH=src\RdpIo.App\RdpIo.App.csproj
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

if not exist "%OUTPUT_DIR%\RdpIo.App.exe" (
    echo.
    echo [ОШИБКА] Итоговый EXE не найден в %OUTPUT_DIR%.
    exit /b 1
)

echo.
echo Копирование Tesseract нативных библиотек...

REM Определяем путь к NuGet пакетам
set NUGET_PACKAGES=%USERPROFILE%\.nuget\packages\tesseract\5.2.0\x64

if exist "%NUGET_PACKAGES%\*.dll" (
    copy /Y "%NUGET_PACKAGES%\*.dll" "%OUTPUT_DIR%\" >nul
    if errorlevel 1 (
        echo [ПРЕДУПРЕЖДЕНИЕ] Не удалось скопировать Tesseract DLL. OCR может не работать.
    ) else (
        echo Tesseract DLL скопированы успешно.
    )
) else (
    echo [ПРЕДУПРЕЖДЕНИЕ] Tesseract DLL не найдены в NuGet кэше. OCR может не работать.
)

for %%A in ("%OUTPUT_DIR%\RdpIo.App.exe") do (
    set SIZE=%%~zA
)
set /a SIZE_MB=!SIZE! / 1048576

echo.
echo ========================================
echo Публикация завершена.
echo Выходной файл: %OUTPUT_DIR%\RdpIo.App.exe
if defined SIZE_MB echo Размер: !SIZE_MB! MB
echo.
echo Структура:
echo   RdpIo.App.exe          (основное приложение)
echo   tessdata\              (языковые файлы OCR)
echo   tesseract50.dll        (Tesseract движок)
echo   leptonica-1.82.0.dll   (библиотека обработки изображений)

endlocal


