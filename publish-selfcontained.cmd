@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
REM ==============================================================================
REM rdp-io - Self-contained publish (без .NET на целевой машине)
REM ==============================================================================
REM Собирает переносимый EXE для машин без установленного .NET.
REM .NET runtime упакован в exe, но Tesseract OCR требует дополнительные файлы.
REM Результат: publish-selfcontained\RdpIo.App.exe + DLL + tessdata\
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
echo Копирование Tesseract нативных библиотек и tessdata...

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

REM Копируем папку tessdata с языковыми файлами
set TESSDATA_SOURCE=src\RdpIo.App\tessdata

if exist "%TESSDATA_SOURCE%" (
    xcopy /E /I /Y "%TESSDATA_SOURCE%" "%OUTPUT_DIR%\tessdata" >nul
    if errorlevel 1 (
        echo [ПРЕДУПРЕЖДЕНИЕ] Не удалось скопировать tessdata. OCR может не работать.
    ) else (
        echo tessdata скопирована успешно.
    )
) else (
    echo [ПРЕДУПРЕЖДЕНИЕ] tessdata не найдена в %TESSDATA_SOURCE%. OCR может не работать.
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
    echo Некоторые файлы Tesseract отсутствуют. Tesseract OCR может не работать.
    echo Приложение будет использовать Windows OCR как fallback.
)

REM Создаём settings.json с Tesseract по умолчанию
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
echo   "LogLevel": 1,
echo   "MaxLogFileSizeMB": 10,
echo   "OcrEngine": "Tesseract",
echo   "OcrLanguage": "ru",
echo   "OcrEnablePreprocessing": false
echo }
) > "%OUTPUT_DIR%\settings.json"

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
echo ВАЖНО: Хотя .NET runtime упакован в exe, Tesseract OCR требует
echo        дополнительные файлы рядом с исполняемым файлом:
echo.
echo Структура для переноса:
echo   RdpIo.App.exe          (основное приложение с .NET runtime)
echo   tesseract50.dll        (Tesseract движок)
echo   leptonica-1.82.0.dll   (библиотека обработки изображений)
echo   tessdata\              (языковые файлы OCR)
echo     rus.traineddata      (русский язык)
echo     eng.traineddata      (английский язык)
echo.
echo Скопируйте всю папку %OUTPUT_DIR% на целевую машину.

endlocal


