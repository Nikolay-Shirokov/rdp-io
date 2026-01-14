using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using RdpIo.Core.ClipboardManagement;
using RdpIo.Core.StateManagement;
using RdpIo.Infrastructure.Logging;

namespace RdpIo.UI.SystemTray;

/// <summary>
/// Управляет System Tray иконкой и контекстным меню
/// Предоставляет пользователю быстрый доступ к функциям приложения
/// </summary>
public class SystemTrayManager : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly IClipboardManager _clipboardManager;
    private readonly StateManager _stateManager;
    private readonly ILogger _logger;
    private ContextMenuStrip? _contextMenu;

    /// <summary>
    /// Событие запроса показа главного окна
    /// </summary>
    public event EventHandler? ShowMainWindowRequested;

    /// <summary>
    /// Событие запроса запуска передачи
    /// </summary>
    public event EventHandler? StartTransmissionRequested;

    /// <summary>
    /// Событие запроса запуска OCR захвата
    /// </summary>
    public event EventHandler? StartOcrCaptureRequested;

    /// <summary>
    /// Событие запроса открытия настроек
    /// </summary>
    public event EventHandler? SettingsRequested;

    /// <summary>
    /// Событие запроса выхода из приложения
    /// </summary>
    public event EventHandler? ExitRequested;

    /// <summary>
    /// Создает новый менеджер System Tray
    /// </summary>
    public SystemTrayManager(
        IClipboardManager clipboardManager,
        StateManager stateManager,
        ILogger logger)
    {
        _clipboardManager = clipboardManager ?? throw new ArgumentNullException(nameof(clipboardManager));
        _stateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _notifyIcon = new NotifyIcon
        {
            Icon = LoadIcon(),
            Visible = true,
            Text = "rdp-io - Готов"
        };

        InitializeContextMenu();
        AttachEventHandlers();

        _logger.LogInfo("SystemTrayManager initialized");
    }

    /// <summary>
    /// Инициализирует контекстное меню
    /// </summary>
    private void InitializeContextMenu()
    {
        _contextMenu = new ContextMenuStrip();

        // Пункты меню
        var showWindowItem = new ToolStripMenuItem("🖥 Показать окно", null, OnShowWindowClick)
        {
            Font = new Font(_contextMenu.Font, FontStyle.Bold)
        };

        var startItem = new ToolStripMenuItem("📋 Напечатать текст", null, OnStartClick);

        var ocrCaptureItem = new ToolStripMenuItem("📷 Захват текста (OCR)", null, OnOcrCaptureClick);

        var settingsItem = new ToolStripMenuItem("⚙ Настройки", null, OnSettingsClick);
        var aboutItem = new ToolStripMenuItem("ℹ О программе", null, OnAboutClick);
        var exitItem = new ToolStripMenuItem("✖ Выход", null, OnExitClick);

        _contextMenu.Items.AddRange(new ToolStripItem[]
        {
            showWindowItem,
            new ToolStripSeparator(),
            startItem,
            ocrCaptureItem,
            new ToolStripSeparator(),
            settingsItem,
            aboutItem,
            new ToolStripSeparator(),
            exitItem
        });

        _notifyIcon.ContextMenuStrip = _contextMenu;
    }

    /// <summary>
    /// Подключает обработчики событий
    /// </summary>
    private void AttachEventHandlers()
    {
        // Двойной клик - показать меню
        _notifyIcon.DoubleClick += (s, e) =>
        {
            ShowContextMenu();
        };

        // Правый клик - показать меню (стандартное поведение через ContextMenuStrip)
        // Левый клик - показать главное окно
        _notifyIcon.Click += (s, e) =>
        {
            if (e is MouseEventArgs me && me.Button == MouseButtons.Left)
            {
                // Левый клик - показать главное окно
                _logger.LogInfo("Main window requested from System Tray (left click)");
                ShowMainWindowRequested?.Invoke(this, EventArgs.Empty);
            }
        };

        // Подписка на изменения состояния для обновления tooltip
        _stateManager.StateChanged += OnStateChanged;
    }

    /// <summary>
    /// Обработчик изменения состояния приложения
    /// </summary>
    private void OnStateChanged(object? sender, StateChangedEventArgs e)
    {
        UpdateTooltip(e.CurrentState);
    }

    /// <summary>
    /// Обновляет tooltip иконки в зависимости от состояния
    /// </summary>
    private void UpdateTooltip(ApplicationState state)
    {
        string tooltip = state switch
        {
            ApplicationState.Idle => GetIdleTooltip(),
            ApplicationState.Countdown => "rdp-io - Обратный отсчет...",
            ApplicationState.Transmitting => "rdp-io - Передача текста...",
            ApplicationState.Paused => "rdp-io - Приостановлено",
            ApplicationState.SelectingRegion => "rdp-io - Выбор области...",
            ApplicationState.CapturingScreen => "rdp-io - Захват экрана...",
            ApplicationState.ProcessingOcr => "rdp-io - Распознавание...",
            ApplicationState.ShowingOcrResult => "rdp-io - Результат OCR",
            _ => "rdp-io"
        };

        // Ограничение длины tooltip (max 63 символа для NotifyIcon)
        if (tooltip.Length > 63)
        {
            tooltip = tooltip.Substring(0, 60) + "...";
        }

        _notifyIcon.Text = tooltip;
    }

    /// <summary>
    /// Получает tooltip для состояния Idle с информацией о буфере обмена
    /// </summary>
    private string GetIdleTooltip()
    {
        try
        {
            string clipboardInfo = _clipboardManager.GetClipboardInfo();
            return $"rdp-io - {clipboardInfo}";
        }
        catch
        {
            return "rdp-io - Готов";
        }
    }

    /// <summary>
    /// Вызывает событие запуска передачи
    /// </summary>
    private void OnStartTransmission()
    {
        _logger.LogInfo("Start transmission requested from System Tray");
        StartTransmissionRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Обработчик клика по пункту "Показать окно"
    /// </summary>
    private void OnShowWindowClick(object? sender, EventArgs e)
    {
        _logger.LogInfo("Show main window requested from System Tray menu");
        ShowMainWindowRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Обработчик клика по пункту "Напечатать текст из буфера обмена"
    /// </summary>
    private void OnStartClick(object? sender, EventArgs e)
    {
        OnStartTransmission();
    }

    /// <summary>
    /// Обработчик клика по пункту "Захват текста (OCR)"
    /// </summary>
    private void OnOcrCaptureClick(object? sender, EventArgs e)
    {
        _logger.LogInfo("OCR capture requested from System Tray");
        StartOcrCaptureRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Обработчик клика по пункту "Настройки"
    /// </summary>
    private void OnSettingsClick(object? sender, EventArgs e)
    {
        _logger.LogInfo("Settings requested from System Tray");
        SettingsRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Обработчик клика по пункту "О программе"
    /// </summary>
    private void OnAboutClick(object? sender, EventArgs e)
    {
        _logger.LogInfo("About dialog opened from System Tray");
        MessageBox.Show(
            "rdp-io v1.0\n\n" +
            "Утилита для передачи текста в изолированную RDP-сессию\n" +
            "через эмуляцию клавиатурного ввода.\n\n" +
            "Поддержка: ASCII + Кириллица (EN/RU раскладки)\n" +
            "Адаптивная скорость передачи: Fast / Reliable / Slow\n\n" +
            "© 2025",
            "О программе rdp-io",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    /// <summary>
    /// Обработчик клика по пункту "Выход"
    /// </summary>
    private void OnExitClick(object? sender, EventArgs e)
    {
        _logger.LogInfo("Exit requested from System Tray");
        ExitRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Показывает контекстное меню около курсора
    /// </summary>
    private void ShowContextMenu()
    {
        // Используем рефлексию для вызова приватного метода ShowContextMenu
        var methodInfo = typeof(NotifyIcon).GetMethod(
            "ShowContextMenu",
            BindingFlags.Instance | BindingFlags.NonPublic);

        methodInfo?.Invoke(_notifyIcon, null);
    }

    /// <summary>
    /// Показывает balloon notification
    /// </summary>
    /// <param name="title">Заголовок уведомления</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="icon">Иконка уведомления</param>
    public void ShowNotification(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
    {
        _notifyIcon.ShowBalloonTip(3000, title, message, icon);
        _logger.LogInfo($"Notification shown: {title} - {message}");
    }

    /// <summary>
    /// Загружает иконку приложения
    /// </summary>
    private Icon LoadIcon()
    {
        try
        {
            // Попытка загрузить иконку из ресурсов или файла
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tray_icon.ico");

            if (File.Exists(iconPath))
            {
                return new Icon(iconPath);
            }

            // Если файл не найден, используем встроенную иконку из ресурсов assembly
            var assembly = Assembly.GetExecutingAssembly();
            var iconStream = assembly.GetManifestResourceStream("RdpIo.UI.Resources.tray_icon.ico");

            if (iconStream != null)
            {
                return new Icon(iconStream);
            }

            // Fallback: системная иконка приложения
            _logger.LogWarning("App icon not found, using system default");
            return SystemIcons.Application;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to load app icon: {ex.Message}");
            return SystemIcons.Application;
        }
    }

    /// <summary>
    /// Освобождает ресурсы
    /// </summary>
    public void Dispose()
    {
        _notifyIcon?.Dispose();
        _contextMenu?.Dispose();
        _logger.LogInfo("SystemTrayManager disposed");
    }
}


