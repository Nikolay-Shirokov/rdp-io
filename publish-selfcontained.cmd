@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
REM ==============================================================================
REM rdp-io - Self-contained single-file publish (без .NET на целевой машине)
REM ==============================================================================
REM Собирает переносимый EXE для машин без установленного .NET.
REM .NET runtime упакован в exe. Использует Windows OCR (встроенный).
REM Результат: publish-selfcontained\RdpIo.App.exe (~87 MB)
REM
REM ВАЖНО: Single-file несовместим с Tesseract OCR.
REM        Для Tesseract используйте publish-tesseract.cmd
REM
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
echo Single-file exe собран с Windows OCR (встроенный в Windows 10/11).

REM Создаём settings.json с Windows OCR (Tesseract не работает в single-file)
echo.
echo Создание settings.json...
(
echo {
echo   "TransmissionMode": 1,
echo   "CountdownSeconds": 5,
echo   "EnableSounds": false,
echo   "SoundOnStart": true,
echo   "SoundOnComplete": true,
echo   "SoundOnError": true,
echo   "ClipboardCacheLifetimeSeconds": 5,
echo   "LogLevel": 4,
echo   "MaxLogFileSizeMB": 10,
echo   "OcrEngine": "Windows",
echo   "OcrLanguage": "ru",
echo   "OcrEnablePreprocessing": false
echo }
) > "%OUTPUT_DIR%\settings.json"

echo.
echo Копирование документации...
copy "docs\readme.html" "%OUTPUT_DIR%\readme.html" >nul
if exist "%OUTPUT_DIR%\readme.html" (
    echo readme.html скопирован
) else (
    echo [ПРЕДУПРЕЖДЕНИЕ] Не удалось скопировать readme.html
)

echo.
echo ПРИМЕЧАНИЕ: Single-file exe использует Windows OCR (встроенный).
echo             Tesseract OCR несовместим с single-file deployment.
echo             Для Tesseract используйте publish-tesseract.cmd

for %%A in ("%OUTPUT_DIR%\RdpIo.App.exe") do (
    set SIZE=%%~zA
)
set /a SIZE_MB=!SIZE! / 1048576

echo.
echo ========================================
echo Публикация завершена.
echo Выходной файл: %OUTPUT_DIR%\RdpIo.App.exe
if defined SIZE_MB echo Размер EXE: !SIZE_MB! MB
echo.
echo Особенности single-file сборки:
echo   - Один исполняемый файл (удобно переносить)
echo   - .NET runtime включен (не требует установки .NET)
echo   - Windows OCR (встроенный в Windows 10/11)
echo.
echo ПРИМЕЧАНИЕ: Для лучшего качества OCR используйте publish-tesseract.cmd
echo             (Tesseract OCR, но требует ~300+ файлов)

endlocal


