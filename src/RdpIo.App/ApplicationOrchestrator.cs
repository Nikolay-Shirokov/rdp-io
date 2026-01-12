using System.Threading;
using System.Windows;
using System.Windows.Forms;
using RdpIo.Configuration;
using RdpIo.Core.ClipboardManagement;
using RdpIo.Core.KeyboardSimulation;
using RdpIo.Core.StateManagement;
using RdpIo.Infrastructure.Logging;
using RdpIo.UI.SystemTray;
using RdpIo.UI.Windows;

namespace RdpIo.App;

/// <summary>
/// Главный координатор приложения
/// Управляет жизненным циклом, координирует взаимодействие всех компонентов
/// </summary>
public class ApplicationOrchestrator : IDisposable
{
    private readonly SystemTrayManager _systemTrayManager;
    private readonly StateManager _stateManager;
    private readonly IClipboardManager _clipboardManager;
    private readonly IKeyboardSimulator _keyboardSimulator;
    private readonly SettingsManager _settingsManager;
    private readonly ILogger _logger;

    private CountdownWindow? _countdownWindow;
    private ProgressWindow? _progressWindow;
    private CancellationTokenSource? _transmissionCts;

    /// <summary>
    /// Создает новый оркестратор приложения
    /// </summary>
    public ApplicationOrchestrator(
        SystemTrayManager systemTrayManager,
        StateManager stateManager,
        IClipboardManager clipboardManager,
        IKeyboardSimulator keyboardSimulator,
        SettingsManager settingsManager,
        ILogger logger)
    {
        _systemTrayManager = systemTrayManager ?? throw new ArgumentNullException(nameof(systemTrayManager));
        _stateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));
        _clipboardManager = clipboardManager ?? throw new ArgumentNullException(nameof(clipboardManager));
        _keyboardSimulator = keyboardSimulator ?? throw new ArgumentNullException(nameof(keyboardSimulator));
        _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        AttachEventHandlers();

        _logger.LogInfo("ApplicationOrchestrator initialized");
    }

    /// <summary>
    /// Подключает обработчики событий от всех компонентов
    /// </summary>
    private void AttachEventHandlers()
    {
        // События от System Tray
        _systemTrayManager.StartTransmissionRequested += OnStartTransmissionRequested;
        _systemTrayManager.SettingsRequested += OnSettingsRequested;
        _systemTrayManager.ExitRequested += OnExitRequested;

        // События от State Manager
        _stateManager.StateChanged += OnStateChanged;
    }

    /// <summary>
    /// Обработчик запроса запуска передачи из System Tray
    /// </summary>
    private void OnStartTransmissionRequested(object? sender, EventArgs e)
    {
        _logger.LogInfo("Start transmission requested");

        try
        {
            // Переход в состояние валидации буфера обмена
            _stateManager.TransitionTo(ApplicationState.ValidatingClipboard);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to start transmission: {ex.Message}");
            _systemTrayManager.ShowNotification(
                "Ошибка",
                "Не удалось запустить передачу",
                ToolTipIcon.Error);
        }
    }

    /// <summary>
    /// Обработчик запроса открытия окна настроек
    /// </summary>
    private void OnSettingsRequested(object? sender, EventArgs e)
    {
        _logger.LogInfo("Settings window requested");

        try
        {
            var settings = _settingsManager.CurrentSettings;
            var settingsWindow = new SettingsWindow(settings);

            // Показываем окно модально
            var result = settingsWindow.ShowDialog();

            if (result == true)
            {
                // Пользователь нажал "Сохранить"
                _settingsManager.SaveSettings();
                _logger.LogInfo("Settings saved successfully");

                _systemTrayManager.ShowNotification(
                    "Настройки сохранены",
                    "Изменения применены успешно",
                    ToolTipIcon.Info);
            }
            else
            {
                // Пользователь нажал "Отмена"
                _logger.LogInfo("Settings changes cancelled");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to open settings window: {ex.Message}");
            _systemTrayManager.ShowNotification(
                "Ошибка",
                "Не удалось открыть окно настроек",
                ToolTipIcon.Error);
        }
    }

    /// <summary>
    /// Обработчик запроса выхода из приложения
    /// </summary>
    private void OnExitRequested(object? sender, EventArgs e)
    {
        _logger.LogInfo("Exit requested");
        System.Windows.Application.Current.Shutdown();
    }

    /// <summary>
    /// Главный обработчик изменения состояния приложения
    /// Координирует все переходы между состояниями
    /// </summary>
    private void OnStateChanged(object? sender, StateChangedEventArgs e)
    {
        _logger.LogInfo($"State changed: {e.PreviousState} -> {e.CurrentState}");

        switch (e.CurrentState)
        {
            case ApplicationState.ValidatingClipboard:
                HandleValidatingClipboard();
                break;

            case ApplicationState.Countdown:
                HandleCountdown();
                break;

            case ApplicationState.Transmitting:
                HandleTransmitting();
                break;

            case ApplicationState.Completed:
                HandleCompleted();
                break;

            case ApplicationState.Failed:
                HandleFailed();
                break;

            case ApplicationState.Cancelled:
                HandleCancelled();
                break;

            case ApplicationState.Idle:
                HandleIdle();
                break;
        }
    }

    /// <summary>
    /// Обработчик состояния: Валидация буфера обмена
    /// </summary>
    private async void HandleValidatingClipboard()
    {
        _logger.LogInfo("Validating clipboard...");

        try
        {
            // Получаем содержимое буфера обмена
            var clipboardContent = await _clipboardManager.GetTextAsync();

            if (clipboardContent == null || string.IsNullOrWhiteSpace(clipboardContent.Text))
            {
                // Error Flow: пустой буфер
                _logger.LogWarning("Clipboard is empty");
                _systemTrayManager.ShowNotification(
                    "Буфер обмена пуст",
                    "Скопируйте текст и попробуйте снова",
                    ToolTipIcon.Warning);

                _stateManager.TransitionTo(ApplicationState.Idle);
                return;
            }

            // Проверяем неподдерживаемые символы
            var unsupportedChars = _keyboardSimulator.GetUnsupportedCharacters(clipboardContent.Text).ToList();

            if (unsupportedChars.Any())
            {
                _logger.LogWarning($"Found {unsupportedChars.Count} unsupported characters");

                // Можно показать предупреждение, но продолжить
                string charsPreview = string.Join(", ", unsupportedChars.Take(5).Select(c => $"'{c}' (U+{(int)c:X4})"));
                if (unsupportedChars.Count > 5)
                    charsPreview += "...";

                _systemTrayManager.ShowNotification(
                    "Предупреждение",
                    $"Найдены неподдерживаемые символы: {charsPreview}. Они будут пропущены.",
                    ToolTipIcon.Warning);
            }

            _logger.LogInfo($"Clipboard validated: {clipboardContent.Text.Length} characters");

            // Переход к обратному отсчету
            _stateManager.TransitionTo(ApplicationState.Countdown);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Clipboard validation failed: {ex.Message}");
            _systemTrayManager.ShowNotification(
                "Ошибка",
                "Не удалось прочитать буфер обмена",
                ToolTipIcon.Error);

            _stateManager.TransitionTo(ApplicationState.Idle);
        }
    }

    /// <summary>
    /// Обработчик состояния: Обратный отсчет
    /// </summary>
    private void HandleCountdown()
    {
        _logger.LogInfo("Starting countdown...");

        try
        {
            // Создаем окно обратного отсчета
            int countdownSeconds = _settingsManager.CurrentSettings.CountdownSeconds;
            _countdownWindow = new CountdownWindow(countdownSeconds);

            // Подписываемся на события окна
            _countdownWindow.CountdownCompleted += OnCountdownCompleted;
            _countdownWindow.CountdownCancelled += OnCountdownCancelled;

            // Показываем окно
            _countdownWindow.Show();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to show countdown window: {ex.Message}");
            _stateManager.TransitionTo(ApplicationState.Failed);
        }
    }

    /// <summary>
    /// Обработчик завершения обратного отсчета
    /// </summary>
    private void OnCountdownCompleted(object? sender, EventArgs e)
    {
        _logger.LogInfo("Countdown completed");

        // Отписываемся от событий
        if (_countdownWindow != null)
        {
            _countdownWindow.CountdownCompleted -= OnCountdownCompleted;
            _countdownWindow.CountdownCancelled -= OnCountdownCancelled;
            _countdownWindow = null;
        }

        // Переход к передаче
        _stateManager.TransitionTo(ApplicationState.Transmitting);
    }

    /// <summary>
    /// Обработчик отмены обратного отсчета
    /// </summary>
    private void OnCountdownCancelled(object? sender, EventArgs e)
    {
        _logger.LogInfo("Countdown cancelled by user");

        // Отписываемся от событий
        if (_countdownWindow != null)
        {
            _countdownWindow.CountdownCompleted -= OnCountdownCompleted;
            _countdownWindow.CountdownCancelled -= OnCountdownCancelled;
            _countdownWindow = null;
        }

        // Переход к отмене
        _stateManager.TransitionTo(ApplicationState.Cancelled);
    }

    /// <summary>
    /// Обработчик состояния: Передача текста
    /// </summary>
    private async void HandleTransmitting()
    {
        _logger.LogInfo("Starting transmission...");

        try
        {
            // Создаем окно прогресса
            _progressWindow = new ProgressWindow();
            _progressWindow.CancelRequested += OnTransmissionCancelRequested;
            _progressWindow.Show();

            // Получаем текст из буфера обмена
            var clipboardContent = await _clipboardManager.GetTextAsync();

            if (clipboardContent == null || string.IsNullOrWhiteSpace(clipboardContent.Text))
            {
                _logger.LogError("Clipboard is empty during transmission");
                _stateManager.TransitionTo(ApplicationState.Failed);
                return;
            }

            // Устанавливаем стратегию передачи из настроек
            var strategy = _settingsManager.GetTransmissionStrategy();
            _keyboardSimulator.SetTransmissionStrategy(strategy);

            // Создаем CancellationToken для возможности отмены
            _transmissionCts = new CancellationTokenSource();

            // Progress reporter для обновления UI
            var progress = new Progress<TransmissionProgress>(p =>
            {
                _progressWindow?.UpdateProgress(p);
            });

            // Запускаем передачу
            var result = await _keyboardSimulator.TransmitTextAsync(
                clipboardContent.Text,
                progress,
                _transmissionCts.Token);

            // Анализируем результат
            if (result.IsCancelled)
            {
                _logger.LogInfo("Transmission cancelled");
                _stateManager.TransitionTo(ApplicationState.Cancelled);
            }
            else if (result.IsSuccess)
            {
                _logger.LogInfo($"Transmission completed successfully: {result.TransmittedCharacters}/{result.TotalCharacters} characters");
                _stateManager.TransitionTo(ApplicationState.Completed);
            }
            else
            {
                _logger.LogError($"Transmission failed: {result.FailedCharacters} failed characters");
                _stateManager.TransitionTo(ApplicationState.Failed);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInfo("Transmission cancelled by token");
            _stateManager.TransitionTo(ApplicationState.Cancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Transmission error: {ex.Message}");
            _stateManager.TransitionTo(ApplicationState.Failed);
        }
        finally
        {
            _transmissionCts?.Dispose();
            _transmissionCts = null;
        }
    }

    /// <summary>
    /// Обработчик запроса отмены передачи
    /// </summary>
    private void OnTransmissionCancelRequested(object? sender, EventArgs e)
    {
        _logger.LogInfo("Transmission cancellation requested");
        _transmissionCts?.Cancel();
    }

    /// <summary>
    /// Обработчик состояния: Передача завершена успешно
    /// </summary>
    private void HandleCompleted()
    {
        _logger.LogInfo("Handling completion...");

        // Закрываем окно прогресса
        CloseProgressWindow();

        // Показываем уведомление
        _systemTrayManager.ShowNotification(
            "Передача завершена",
            "Текст успешно передан!",
            ToolTipIcon.Info);

        // Переход в Idle
        _stateManager.TransitionTo(ApplicationState.Idle);
    }

    /// <summary>
    /// Обработчик состояния: Передача завершилась с ошибкой
    /// </summary>
    private void HandleFailed()
    {
        _logger.LogInfo("Handling failure...");

        // Закрываем окно прогресса
        CloseProgressWindow();

        // Показываем уведомление об ошибке
        _systemTrayManager.ShowNotification(
            "Ошибка передачи",
            "Не удалось завершить передачу текста",
            ToolTipIcon.Error);

        // Переход в Idle
        _stateManager.TransitionTo(ApplicationState.Idle);
    }

    /// <summary>
    /// Обработчик состояния: Передача отменена
    /// </summary>
    private void HandleCancelled()
    {
        _logger.LogInfo("Handling cancellation...");

        // Закрываем окно прогресса
        CloseProgressWindow();

        // Показываем уведомление
        _systemTrayManager.ShowNotification(
            "Передача отменена",
            "Операция отменена пользователем",
            ToolTipIcon.Info);

        // Переход в Idle
        _stateManager.TransitionTo(ApplicationState.Idle);
    }

    /// <summary>
    /// Обработчик состояния: Idle (готов к работе)
    /// </summary>
    private void HandleIdle()
    {
        _logger.LogInfo("Application is idle and ready");
        // Ничего особенного не делаем, просто готовы к новой передаче
    }

    /// <summary>
    /// Закрывает окно прогресса и отписывается от событий
    /// </summary>
    private void CloseProgressWindow()
    {
        if (_progressWindow != null)
        {
            _progressWindow.CancelRequested -= OnTransmissionCancelRequested;
            _progressWindow.Close();
            _progressWindow = null;
        }
    }

    /// <summary>
    /// Освобождает ресурсы
    /// </summary>
    public void Dispose()
    {
        _transmissionCts?.Dispose();
        _logger.LogInfo("ApplicationOrchestrator disposed");
    }
}

