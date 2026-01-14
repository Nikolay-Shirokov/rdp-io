using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RdpIo.Infrastructure.OcrManagement;
using RdpIo.UI.Commands;

namespace RdpIo.UI.Windows;

/// <summary>
/// ViewModel для окна результатов OCR
/// </summary>
public class OcrResultViewModel : INotifyPropertyChanged
{
    private OcrResult? _result;
    private string _recognizedText = string.Empty;
    private string _statistics = string.Empty;

    /// <summary>
    /// Распознанный текст
    /// </summary>
    public string RecognizedText
    {
        get => _recognizedText;
        set
        {
            _recognizedText = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Статистика распознавания (строки, слова, символы, время)
    /// </summary>
    public string Statistics
    {
        get => _statistics;
        set
        {
            _statistics = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Команда копирования текста в буфер обмена
    /// </summary>
    public ICommand CopyToClipboardCommand { get; }

    /// <summary>
    /// Команда отправки текста в RDP
    /// </summary>
    public ICommand SendToRdpCommand { get; }

    /// <summary>
    /// Команда закрытия окна
    /// </summary>
    public ICommand CloseCommand { get; }

    /// <summary>
    /// Событие запроса отправки текста в RDP
    /// </summary>
    public event EventHandler? SendToRdpRequested;

    /// <summary>
    /// Событие запроса закрытия окна
    /// </summary>
    public event EventHandler? CloseRequested;

    /// <summary>
    /// Создает новый ViewModel для окна результатов OCR
    /// </summary>
    public OcrResultViewModel()
    {
        CopyToClipboardCommand = new RelayCommand(_ => CopyToClipboard());
        SendToRdpCommand = new RelayCommand(_ => SendToRdp());
        CloseCommand = new RelayCommand(_ => Close());
    }

    /// <summary>
    /// Устанавливает результат OCR для отображения
    /// </summary>
    public void SetResult(OcrResult result)
    {
        _result = result ?? throw new ArgumentNullException(nameof(result));

        RecognizedText = result.Text;
        Statistics = FormatStatistics(result);
    }

    /// <summary>
    /// Копирует распознанный текст в буфер обмена
    /// </summary>
    private void CopyToClipboard()
    {
        if (string.IsNullOrWhiteSpace(RecognizedText))
        {
            System.Windows.MessageBox.Show(
                "Нет текста для копирования.",
                "rdp-io - Ошибка",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning
            );
            return;
        }

        try
        {
            System.Windows.Clipboard.SetText(RecognizedText);
            System.Windows.MessageBox.Show(
                "Текст скопирован в буфер обмена!",
                "rdp-io - Успех",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information
            );
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Ошибка копирования в буфер обмена: {ex.Message}",
                "rdp-io - Ошибка",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error
            );
        }
    }

    /// <summary>
    /// Отправляет текст в RDP (через ApplicationOrchestrator)
    /// </summary>
    private void SendToRdp()
    {
        if (string.IsNullOrWhiteSpace(RecognizedText))
        {
            System.Windows.MessageBox.Show(
                "Нет текста для отправки.",
                "rdp-io - Ошибка",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning
            );
            return;
        }

        SendToRdpRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Закрывает окно
    /// </summary>
    private void Close()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Форматирует статистику OCR в читаемую строку
    /// </summary>
    private string FormatStatistics(OcrResult result)
    {
        return $"Строк: {result.LineCount} | " +
               $"Слов: {result.WordCount} | " +
               $"Символов: {result.CharacterCount} | " +
               $"Confidence: {result.Confidence:P0} | " +
               $"Время: {result.ProcessingTime.TotalMilliseconds:F0} мс";
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
