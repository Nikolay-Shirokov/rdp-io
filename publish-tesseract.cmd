@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
REM ==============================================================================
REM rdp-io - Tesseract OCR build (multi-file, better quality)
REM ==============================================================================
REM Собирает версию с Tesseract OCR для максимального качества распознавания.
REM Результат: папка publish-tesseract с несколькими файлами
REM Требуется .NET SDK на машине сборки.
REM ==============================================================================

echo ========================================
echo rdp-io - Tesseract OCR Build
echo ========================================
echo.

set PROJECT_PATH=src\RdpIo.App\RdpIo.App.csproj
set OUTPUT_DIR=publish-tesseract
set RID=win-x64

if exist "%OUTPUT_DIR%" (
    rmdir /s /q "%OUTPUT_DIR%"
)

echo Публикация с Tesseract OCR (multi-file для нативных библиотек)...

dotnet publish "%PROJECT_PATH%" ^
    --configuration Release ^
    --runtime %RID% ^
    --self-contained true ^
    -p:PublishSingleFile=false ^
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
echo Проверка Tesseract файлов...

set MISSING_FILES=0

if not exist "%OUTPUT_DIR%\tesseract50.dll" (
    echo [ПРЕДУПРЕЖДЕНИЕ] tesseract50.dll не найден
    set MISSING_FILES=1
)

if not exist "%OUTPUT_DIR%\leptonica-1.82.0.dll" (
    echo [ПРЕДУПРЕЖДЕНИЕ] leptonica-1.82.0.dll не найден
    set MISSING_FILES=1
)

if not exist "%OUTPUT_DIR%\tessdata\rus.traineddata" (
    echo [ПРЕДУПРЕЖДЕНИЕ] tessdata\rus.traineddata не найден
    set MISSING_FILES=1
)

if not exist "%OUTPUT_DIR%\tessdata\eng.traineddata" (
    echo [ПРЕДУПРЕЖДЕНИЕ] tessdata\eng.traineddata не найден
    set MISSING_FILES=1
)

if %MISSING_FILES%==1 (
    echo.
    echo Некоторые файлы Tesseract отсутствуют. Проверьте настройки проекта.
)

echo.
echo ========================================
echo Публикация завершена.
echo Выходная папка: %OUTPUT_DIR%\
echo.
echo Структура:
dir /b "%OUTPUT_DIR%" | findstr /v ".pdb"
echo.
echo Запуск: %OUTPUT_DIR%\RdpIo.App.exe

endlocal
