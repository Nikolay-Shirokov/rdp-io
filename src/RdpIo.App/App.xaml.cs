using System.Windows;
using RdpIo.Infrastructure.Logging;
using RdpIo.UI.SystemTray;

namespace RdpIo.App;

/// <summary>
/// Главный класс приложения
/// Инициализирует Dependency Injection и запускает приложение
/// </summary>
public partial class App : Application
{
    private IServiceProvider? _serviceProvider;
    private SystemTrayManager? _systemTrayManager;
    private ApplicationOrchestrator? _orchestrator;
    private ILogger? _logger;

    /// <summary>
    /// Обработчик запуска приложения
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        try
        {
            // Приложение работает через System Tray без главного окна
            // Не завершать при закрытии последнего окна
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Конфигурируем Dependency Injection
            _serviceProvider = AppBootstrapper.ConfigureServices();

            // Получаем логгер
            var serviceProvider = (SimpleServiceProvider)_serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _logger.LogInfo("=== rdp-io Application Starting ===");

            // Получаем System Tray Manager (это запускает иконку в трее)
            _systemTrayManager = serviceProvider.GetRequiredService<SystemTrayManager>();
            _logger.LogInfo("System Tray Manager initialized");

            // Получаем Application Orchestrator (это инициализирует все обработчики)
            _orchestrator = serviceProvider.GetRequiredService<ApplicationOrchestrator>();
            _logger.LogInfo("Application Orchestrator initialized");

            _logger.LogInfo("=== rdp-io Application Started Successfully ===");

            // Показываем главное окно при запуске
            _orchestrator.ShowMainWindow();
            _logger.LogInfo("Main window displayed on startup");

            // Показываем приветственное уведомление
            _systemTrayManager.ShowNotification(
                "rdp-io запущен",
                "Приложение готово к работе.",
                System.Windows.Forms.ToolTipIcon.Info);
        }
        catch (Exception ex)
        {
            // Если что-то пошло не так, показываем MessageBox
            MessageBox.Show(
                $"Ошибка при запуске приложения:\n\n{ex.Message}\n\n{ex.StackTrace}",
                "Ошибка запуска rdp-io",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Логируем если возможно
            _logger?.LogError($"Fatal error during startup: {ex.Message}");

            // Завершаем приложение
            Shutdown(1);
        }
    }

    /// <summary>
    /// Обработчик завершения приложения
    /// </summary>
    private void OnExit(object sender, ExitEventArgs e)
    {
        try
        {
            _logger?.LogInfo("=== rdp-io Application Shutting Down ===");

            // Освобождаем ресурсы
            _orchestrator?.Dispose();
            _systemTrayManager?.Dispose();

            // Dispose ServiceProvider если он IDisposable
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _logger?.LogInfo("=== rdp-io Application Stopped ===");
        }
        catch (Exception ex)
        {
            // Логируем ошибку при завершении, но не показываем UI
            _logger?.LogError($"Error during shutdown: {ex.Message}");
        }
    }
}


