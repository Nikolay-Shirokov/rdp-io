using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RdpIo.UI.Commands;

namespace RdpIo.App;

/// <summary>
/// ViewModel для главного окна приложения
/// </summary>
public class MainWindowViewModel : INotifyPropertyChanged
{
    private string _statusText = "✓ Готов к работе";
    private string _statusDetails = "Приложение готово к передаче данных";

    /// <summary>
    /// Текст статуса
    /// </summary>
    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Детали статуса
    /// </summary>
    public string StatusDetails
    {
        get => _statusDetails;
        set
        {
            _statusDetails = value;
            OnPropertyChanged();
        }
    }

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

    /// <summary>
    /// Команда запуска захвата из буфера обмена
    /// </summary>
    public ICommand StartClipboardCaptureCommand { get; }

    /// <summary>
    /// Команда запуска OCR захвата
    /// </summary>
    public ICommand StartOcrCaptureCommand { get; }

    /// <summary>
    /// Команда открытия настроек
    /// </summary>
    public ICommand OpenSettingsCommand { get; }

    /// <summary>
    /// Команда сворачивания в трей
    /// </summary>
    public ICommand MinimizeToTrayCommand { get; }

    /// <summary>
    /// Команда выхода
    /// </summary>
    public ICommand ExitCommand { get; }

    public MainWindowViewModel()
    {
        StartClipboardCaptureCommand = new RelayCommand(OnStartClipboardCapture);
        StartOcrCaptureCommand = new RelayCommand(OnStartOcrCapture);
        OpenSettingsCommand = new RelayCommand(OnOpenSettings);
        MinimizeToTrayCommand = new RelayCommand(OnMinimizeToTray);
        ExitCommand = new RelayCommand(OnExit);
    }

    private void OnStartClipboardCapture(object? parameter)
    {
        ClipboardCaptureRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnStartOcrCapture(object? parameter)
    {
        OcrCaptureRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnOpenSettings(object? parameter)
    {
        SettingsRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnMinimizeToTray(object? parameter)
    {
        MinimizeToTrayRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnExit(object? parameter)
    {
        ExitRequested?.Invoke(this, EventArgs.Empty);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
