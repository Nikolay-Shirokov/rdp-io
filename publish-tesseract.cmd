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
echo Организация файлов: перемещение всех файлов в app/...

REM Переименовываем выходную папку во временную
set TEMP_DIR=%OUTPUT_DIR%_temp
if exist "%TEMP_DIR%" rmdir /s /q "%TEMP_DIR%"
ren "%OUTPUT_DIR%" "%OUTPUT_DIR%_temp"

REM Создаём новую структуру
mkdir "%OUTPUT_DIR%"
mkdir "%OUTPUT_DIR%\app"

REM Перемещаем всё из временной папки в app (включая подпапки)
echo Перемещение файлов и папок...
for /d %%D in ("%TEMP_DIR%\*") do move "%%D" "%OUTPUT_DIR%\app\" >nul 2>&1
move "%TEMP_DIR%\*.*" "%OUTPUT_DIR%\app\" >nul 2>&1

REM Удаляем временную папку
rmdir /s /q "%TEMP_DIR%" 2>nul

echo Файлы перемещены в app/

echo.
echo Создание ярлыка в корне...

REM Создаём ярлык с относительным путём через PowerShell
powershell -NoProfile -Command ^
    "$ws = New-Object -ComObject WScript.Shell; " ^
    "$shortcut = $ws.CreateShortcut('%CD%\%OUTPUT_DIR%\rdp-io.lnk'); " ^
    "$shortcut.TargetPath = '%CD%\%OUTPUT_DIR%\app\RdpIo.App.exe'; " ^
    "$shortcut.WorkingDirectory = '%CD%\%OUTPUT_DIR%\app'; " ^
    "$shortcut.IconLocation = '%CD%\%OUTPUT_DIR%\app\RdpIo.App.exe,0'; " ^
    "$shortcut.Description = 'rdp-io'; " ^
    "$shortcut.Save(); " ^
    "$bytes = [System.IO.File]::ReadAllBytes('%CD%\%OUTPUT_DIR%\rdp-io.lnk'); " ^
    "$bytes[0x15] = $bytes[0x15] -bor 0x01; " ^
    "[System.IO.File]::WriteAllBytes('%CD%\%OUTPUT_DIR%\rdp-io.lnk', $bytes)"

if exist "%OUTPUT_DIR%\rdp-io.lnk" (
    echo Ярлык rdp-io.lnk создан
) else (
    echo [ПРЕДУПРЕЖДЕНИЕ] Не удалось создать ярлык
)

echo.
echo Проверка Tesseract файлов...

set MISSING_FILES=0

if not exist "%OUTPUT_DIR%\app\tesseract50.dll" (
    echo [ПРЕДУПРЕЖДЕНИЕ] app\tesseract50.dll не найден
    set MISSING_FILES=1
)

if not exist "%OUTPUT_DIR%\app\leptonica-1.82.0.dll" (
    echo [ПРЕДУПРЕЖДЕНИЕ] app\leptonica-1.82.0.dll не найден
    set MISSING_FILES=1
)

if not exist "%OUTPUT_DIR%\app\tessdata\rus.traineddata" (
    echo [ПРЕДУПРЕЖДЕНИЕ] app\tessdata\rus.traineddata не найден
    set MISSING_FILES=1
)

if not exist "%OUTPUT_DIR%\app\tessdata\eng.traineddata" (
    echo [ПРЕДУПРЕЖДЕНИЕ] app\tessdata\eng.traineddata не найден
    set MISSING_FILES=1
)

if %MISSING_FILES%==1 (
    echo.
    echo Некоторые файлы Tesseract отсутствуют. Проверьте настройки проекта.
)

echo.
echo Создание settings.json с Tesseract OCR...
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
echo   "OcrEngine": "Tesseract",
echo   "OcrLanguage": "ru",
echo   "OcrEnablePreprocessing": false
echo }
) > "%OUTPUT_DIR%\app\settings.json"

echo.
echo Копирование документации...
copy "docs\readme.html" "%OUTPUT_DIR%\readme.html" >nul
if exist "%OUTPUT_DIR%\readme.html" (
    echo readme.html скопирован
) else (
    echo [ПРЕДУПРЕЖДЕНИЕ] Не удалось скопировать readme.html
)

echo.
echo ========================================
echo Публикация завершена.
echo Выходная папка: %OUTPUT_DIR%\
echo.
echo Структура:
echo   rdp-io.lnk       - ярлык (запускайте этот файл)
echo   readme.html      - руководство пользователя
echo   app\             - все файлы приложения
echo.
echo Запуск: %OUTPUT_DIR%\rdp-io.lnk

endlocal
