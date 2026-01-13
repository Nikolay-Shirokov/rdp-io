using RdpIo.Configuration;
using RdpIo.Core.ClipboardManagement;
using RdpIo.Core.KeyboardSimulation;
using RdpIo.Core.StateManagement;
using RdpIo.Infrastructure.ImageProcessing;
using RdpIo.Infrastructure.Logging;
using RdpIo.Infrastructure.OcrManagement;
using RdpIo.Infrastructure.ScreenCapture;
using RdpIo.Infrastructure.Win32;
using RdpIo.UI.SystemTray;

namespace RdpIo.App;

/// <summary>
/// Настройка Dependency Injection контейнера
/// Регистрирует все зависимости приложения
/// </summary>
public static class AppBootstrapper
{
    /// <summary>
    /// Конфигурирует и создает ServiceProvider со всеми зависимостями
    /// </summary>
    public static IServiceProvider ConfigureServices()
    {
        var services = new SimpleServiceProvider();

        // ===== INFRASTRUCTURE =====
        // Logger - Singleton (один экземпляр на всё приложение)
        services.RegisterSingleton<ILogger>(() =>
            new FileLogger(
                logDirectory: null, // Рядом с .exe
                minLogLevel: LogLevel.Info,
                maxFileSizeMB: 10));

        // Win32 API Wrapper - Singleton
        services.RegisterSingleton<IWin32ApiWrapper>(() => new Win32ApiWrapper());

        // ===== CONFIGURATION =====
        // Settings Manager - Singleton
        services.RegisterSingleton<SettingsManager>(() =>
        {
            var logger = services.GetRequiredService<ILogger>();
            return new SettingsManager(logger, settingsDirectory: null);
        });

        // ===== CORE: STATE MANAGEMENT =====
        // State Manager - Singleton (единый источник состояния)
        services.RegisterSingleton<StateManager>(() =>
        {
            var logger = services.GetRequiredService<ILogger>();
            return new StateManager(logger);
        });

        // ===== CORE: CLIPBOARD MANAGEMENT =====
        // Clipboard Validator - Singleton
        services.RegisterSingleton<ClipboardValidator>(() =>
        {
            var logger = services.GetRequiredService<ILogger>();
            return new ClipboardValidator(logger);
        });

        // Clipboard Cache - Singleton
        services.RegisterSingleton<ClipboardCache>(() => new ClipboardCache());

        // Clipboard Manager - Singleton
        services.RegisterSingleton<IClipboardManager>(() =>
        {
            var win32Api = services.GetRequiredService<IWin32ApiWrapper>();
            var logger = services.GetRequiredService<ILogger>();
            var validator = services.GetRequiredService<ClipboardValidator>();
            var cache = services.GetRequiredService<ClipboardCache>();
            return new WindowsClipboardManager(win32Api, logger, validator, cache);
        });

        // ===== CORE: KEYBOARD SIMULATION =====
        // Layout Manager - Singleton
        services.RegisterSingleton<LayoutManager>(() =>
        {
            var logger = services.GetRequiredService<ILogger>();
            return new LayoutManager(logger);
        });

        // Character Mapper - Singleton
        services.RegisterSingleton<CharacterMapper>(() =>
        {
            var logger = services.GetRequiredService<ILogger>();
            return new CharacterMapper(logger);
        });

        // Keyboard Simulator - создается при каждом запросе
        services.RegisterSingleton<IKeyboardSimulator>(() =>
        {
            var layoutManager = services.GetRequiredService<LayoutManager>();
            var characterMapper = services.GetRequiredService<CharacterMapper>();
            var win32Api = services.GetRequiredService<IWin32ApiWrapper>();
            var logger = services.GetRequiredService<ILogger>();
            var settingsManager = services.GetRequiredService<SettingsManager>();

            // Получаем стратегию из настроек
            var defaultStrategy = settingsManager.GetTransmissionStrategy();

            return new KeyboardSimulatorEngine(
                layoutManager,
                characterMapper,
                win32Api,
                logger,
                defaultStrategy);
        });

        // ===== OCR: SCREEN CAPTURE =====
        // Screen Capture Manager - Singleton
        services.RegisterSingleton<IScreenCaptureManager>(() =>
        {
            var win32Api = services.GetRequiredService<IWin32ApiWrapper>();
            return new ScreenCaptureManager(win32Api);
        });

        // ===== OCR: IMAGE PROCESSING =====
        // Image Processor - Singleton
        services.RegisterSingleton<IImageProcessor>(() => new ImageProcessor());

        // ===== OCR: TEXT RECOGNITION =====
        // OCR Engine - Singleton
        services.RegisterSingleton<IOcrEngine>(() => new WindowsOcrEngine());

        // ===== UI =====
        // System Tray Manager - Singleton
        services.RegisterSingleton<SystemTrayManager>(() =>
        {
            var clipboardManager = services.GetRequiredService<IClipboardManager>();
            var stateManager = services.GetRequiredService<StateManager>();
            var logger = services.GetRequiredService<ILogger>();
            return new SystemTrayManager(clipboardManager, stateManager, logger);
        });

        // ===== APPLICATION =====
        // Application Orchestrator - Singleton (главный координатор)
        services.RegisterSingleton<ApplicationOrchestrator>(() =>
        {
            var systemTrayManager = services.GetRequiredService<SystemTrayManager>();
            var stateManager = services.GetRequiredService<StateManager>();
            var clipboardManager = services.GetRequiredService<IClipboardManager>();
            var keyboardSimulator = services.GetRequiredService<IKeyboardSimulator>();
            var settingsManager = services.GetRequiredService<SettingsManager>();
            var logger = services.GetRequiredService<ILogger>();
            var screenCaptureManager = services.GetRequiredService<IScreenCaptureManager>();
            var imageProcessor = services.GetRequiredService<IImageProcessor>();
            var ocrEngine = services.GetRequiredService<IOcrEngine>();

            return new ApplicationOrchestrator(
                systemTrayManager,
                stateManager,
                clipboardManager,
                keyboardSimulator,
                settingsManager,
                logger,
                screenCaptureManager,
                imageProcessor,
                ocrEngine);
        });

        // Возвращаем ServiceProvider
        return services;
    }
}

