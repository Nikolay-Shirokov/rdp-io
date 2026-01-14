using System.Windows;
using System.Windows.Input;

namespace RdpIo.App;

/// <summary>
/// Главное окно приложения
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Событие запуска захвата из буфера обмена
    /// </summary>
    public event EventHandler? ClipboardCaptureRequested;

    /// <summary>
    /// Событие запуска OCR захвата
    /// </summary>
    public event EventHandler? OcrCaptureRequested;

    /// <summary>
    /// Событие открытия настроек
    /// </summary>
    public event EventHandler? SettingsRequested;

    /// <summary>
    /// Событие сворачивания в трей
    /// </summary>
    public event EventHandler? MinimizeToTrayRequested;

    /// <summary>
    /// Событие выхода из приложения
    /// </summary>
    public event EventHandler? ExitRequested;

    private readonly MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = new MainWindowViewModel();

        // Подписываемся на события ViewModel
        _viewModel.ClipboardCaptureRequested += (s, e) => ClipboardCaptureRequested?.Invoke(this, EventArgs.Empty);
        _viewModel.OcrCaptureRequested += (s, e) => OcrCaptureRequested?.Invoke(this, EventArgs.Empty);
        _viewModel.SettingsRequested += (s, e) => SettingsRequested?.Invoke(this, EventArgs.Empty);
        _viewModel.MinimizeToTrayRequested += (s, e) => MinimizeToTrayRequested?.Invoke(this, EventArgs.Empty);
        _viewModel.ExitRequested += (s, e) => ExitRequested?.Invoke(this, EventArgs.Empty);

        DataContext = _viewModel;
    }

    /// <summary>
    /// Обновляет статус в главном окне
    /// </summary>
    public void UpdateStatus(string statusText, string statusDetails)
    {
        _viewModel.StatusText = statusText;
        _viewModel.StatusDetails = statusDetails;
    }

    /// <summary>
    /// Обработчик перетаскивания окна за заголовок
    /// </summary>
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}
