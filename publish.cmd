@echo off
REM ================================================================================
REM TextSimulator - Build Script
REM ================================================================================
REM Создает Release сборку приложения для Windows
REM
REM Выходной директория: publish\
REM Главный файл: publish\TextSimulator.App.exe
REM
REM ПРИМЕЧАНИЕ: .NET 10 Preview - single-file publish пока не поддерживается
REM После релиза .NET 10 можно будет создавать portable single-file executable
REM
REM Требования: .NET 10 Runtime на целевой системе
REM ================================================================================

echo ========================================
echo TextSimulator - Build Script
echo ========================================
echo.

REM Определяем путь к проекту
set PROJECT_PATH=src\TextSimulator.App\TextSimulator.App.csproj
set OUTPUT_DIR=publish

echo Очистка предыдущей сборки...
if exist "%OUTPUT_DIR%" (
    rmdir /s /q "%OUTPUT_DIR%"
)

echo.
echo Сборка приложения...
echo Конфигурация: Release
echo Платформа: .NET 10.0 Windows
echo.

dotnet build "%PROJECT_PATH%" ^
    --configuration Release ^
    --output "%OUTPUT_DIR%"

if errorlevel 1 (
    echo.
    echo [ОШИБКА] Публикация завершилась с ошибкой!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Публикация завершена успешно!
echo ========================================
echo.

REM Показываем информацию о файле
if exist "%OUTPUT_DIR%\TextSimulator.App.exe" (
    echo Исполняемый файл: %OUTPUT_DIR%\TextSimulator.App.exe
    echo.

    REM Получаем размер файла в KB
    for %%A in ("%OUTPUT_DIR%\TextSimulator.App.exe") do (
        set SIZE=%%~zA
    )

    REM Конвертируем байты в мегабайты
    set /a SIZE_MB=%SIZE% / 1048576

    echo Размер файла: %SIZE_MB% MB
    echo.

    REM Проверяем размер (цель: меньше 50 MB для single-file WPF приложения)
    if %SIZE_MB% GTR 50 (
        echo [ПРЕДУПРЕЖДЕНИЕ] Размер файла превышает 50 MB
    ) else (
        echo [OK] Размер файла в пределах нормы
    )

    echo.
    echo Содержимое директории publish:
    dir /b "%OUTPUT_DIR%"

    echo.
    echo ========================================
    echo Приложение готово к использованию!
    echo ========================================
    echo.
    echo Для запуска: %OUTPUT_DIR%\TextSimulator.App.exe
    echo.
    echo Настройки сохраняются в: settings.json (в директории с .exe)
    echo Логи сохраняются в: logs\app.log (в директории с .exe)
    echo.
) else (
    echo [ОШИБКА] Исполняемый файл не найден!
    pause
    exit /b 1
)

pause
