# Repository Guidelines

## Язык общения и документации
- Всю переписку ведем на русском.
- Комментарии в коде, сообщения в документации и пояснения в PR также пишем на русском.

## Project Structure & Module Organization
Репозиторий — WPF приложение на .NET 10, разделенное на проекты по ответственности:
- `src/RdpIo.App/` — точка входа и композиция приложения.
- `src/RdpIo.Core/` — основная логика (клавиатура/буфер/состояние).
- `src/RdpIo.UI/` — WPF UI компоненты и окна.
- `src/RdpIo.Infrastructure/` — Win32, файловая система, логирование.
- `src/RdpIo.Configuration/` — модели настроек и значения по умолчанию.
- `src/RdpIo.Shared/` — общие типы и утилиты.
Сопутствующие материалы: `tests/`, `task/`, `BUILD.md`, `USER_MANUAL.md`, `publish.cmd`. Решение — `RdpIo.slnx`.

## Build, Test, and Development Commands
- `dotnet build` — Debug сборка (`src/RdpIo.App/bin/Debug/net10.0-windows/`).
- `dotnet build --configuration Release` — Release сборка.
- `dotnet test` — запуск xUnit тестов из `tests/RdpIo.Core.Tests` (с `coverlet.collector`).
- `publish.cmd` — публикация в `publish/`.
- `publish-selfcontained.cmd` — self-contained single-file EXE для целевой машины без .NET (вывод в `publish-selfcontained/`).
- `dotnet publish src/RdpIo.App/RdpIo.App.csproj --configuration Release --output publish` — ручной вариант публикации.

## Coding Style & Naming Conventions
- C# использует file-scoped namespaces, nullable reference types и XML-док комментарии для публичных API.
- Отступы — 4 пробела; фигурные скобки на отдельных строках, как в текущих файлах.
- Имена по .NET стандартам: PascalCase для публичных типов/членов, camelCase для локальных, `_camelCase` для приватных полей.
- Имена XAML ресурсов и UI классов согласовывать между собой.

## Testing Guidelines
- Фреймворк: xUnit (`tests/RdpIo.Core.Tests`).
- Имена тестов должны описывать поведение и результат; новые классы — с суффиксом `*Tests`.
- Перед PR запускать `dotnet test`; для изменений Core/Infrastructure добавлять тесты.

## Commit & Pull Request Guidelines
- История использует Conventional Commits: `feat: ...`, `fix(scope): ...`, `docs: ...`; при наличии указывать тег задачи (например, `(TASK-17)`).
- PR должен описывать изменения, шаги проверки, ссылки на задачи/issue и скриншоты для UI правок.

## Configuration & Runtime Artifacts
- Настройки — `settings.json` рядом с EXE; логи — `logs/app.log`.
- Не коммитить локальные настройки и генерируемые логи.

