# rdp-io - Инструкция по сборке и публикации

## 📋 Требования

- **.NET 10 SDK** (или выше)
- **Windows 7/10/11** (x64)
- **Visual Studio 2022** (опционально, для разработки)

## 🔨 Сборка проекта

### Debug сборка (разработка)

```bash
dotnet build
```

Результат: `src/RdpIo.App/bin/Debug/net10.0-windows/`

### Release сборка

```bash
dotnet build --configuration Release
```

Результат: `src/RdpIo.App/bin/Release/net10.0-windows/`

### Сборка в отдельную директорию

Используйте скрипт `publish.cmd`:

```cmd
publish.cmd
```

Результат: `publish/`

Или вручную:

```bash
dotnet build src/RdpIo.App/RdpIo.App.csproj --configuration Release --output publish
```

### Self-contained single-file publish (без .NET на целевой машине)

```cmd
publish-selfcontained.cmd
```

Вывод: `publish-selfcontained/` (EXE включает .NET runtime для win-x64). Требуется .NET SDK на машине сборки.

## 📦 Portable Deployment

### Текущая конфигурация (.NET 10 Preview)

Из-за preview-статуса .NET 10, single-file publish пока не поддерживается.

Текущая сборка создает:
- `RdpIo.App.exe` (главный исполняемый файл)
- Библиотеки: `RdpIo.*.dll`
- Зависимости .NET Framework

**Требования на целевой машине:**
- .NET 10 Runtime установлен

### Будущая конфигурация (после релиза .NET 10)

После выхода стабильной версии .NET 10 можно будет включить в `RdpIo.App.csproj`:

```xml
<PropertyGroup>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  <PublishTrimmed>true</PublishTrimmed>
  <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
  <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
</PropertyGroup>
```

Команда для создания single-file executable:

```bash
dotnet publish src/RdpIo.App/RdpIo.App.csproj ^
    --configuration Release ^
    --runtime win-x64 ^
    --self-contained true ^
    /p:PublishSingleFile=true ^
    /p:PublishTrimmed=true ^
    --output publish
```

Результат: один файл `RdpIo.App.exe` (~10-20 MB)

## 🚀 Запуск приложения

### Из сборки

```cmd
cd publish
RdpIo.App.exe
```

### Portable deployment

1. Скопируйте всю директорию `publish/` на целевую машину
2. Убедитесь, что установлен .NET 10 Runtime
3. Запустите `RdpIo.App.exe`

Приложение создаст рядом с .exe:
- `settings.json` - файл настроек
- `logs/app.log` - логи приложения

## 📁 Структура сборки

```
publish/
├── RdpIo.App.exe           # Главный исполняемый файл (159 KB)
├── RdpIo.App.dll           # Основная библиотека (25 KB)
├── RdpIo.Core.dll          # Ядро логики (37 KB)
├── RdpIo.UI.dll            # UI компоненты (35 KB)
├── RdpIo.Configuration.dll # Настройки (9.5 KB)
├── RdpIo.Infrastructure.dll# Инфраструктура (12 KB)
├── RdpIo.Shared.dll        # Общие типы (4 KB)
└── [прочие DLL зависимости]
```

**Общий размер:** ~2-5 MB (с зависимостями .NET Framework)

## 🔧 Оптимизация размера

После релиза .NET 10 для уменьшения размера можно использовать:

1. **Trimming** - удаление неиспользуемого кода
   ```xml
   <PublishTrimmed>true</PublishTrimmed>
   ```

2. **ReadyToRun** - AOT компиляция
   ```xml
   <PublishReadyToRun>true</PublishReadyToRun>
   ```

3. **Compression** - сжатие в single-file
   ```xml
   <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
   ```

## 📝 Примечания

- Текущая сборка оптимизирована для разработки и тестирования
- Для production deployment рекомендуется дождаться релиза .NET 10
- Все настройки публикации закомментированы в `RdpIo.App.csproj`
- Скрипт `publish.cmd` автоматизирует процесс сборки

## 🐛 Troubleshooting

### Ошибка: "не удалось найти .NET Runtime"

Установите .NET 10 Runtime: https://dotnet.microsoft.com/download/dotnet/10.0

### Ошибка: "NU1100: Не удалось разрешить пакеты"

Это нормально для .NET 10 Preview. Используйте обычный `dotnet build` вместо `dotnet publish`.

### Приложение не запускается

Проверьте:
1. Установлен ли .NET 10 Runtime
2. Все ли DLL файлы находятся в одной директории с .exe
3. Логи в `logs/app.log`



