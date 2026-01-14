using System.Windows;

namespace RdpIo.UI.Windows;

/// <summary>
/// Окно индикатора обработки OCR
/// </summary>
public partial class OcrProcessingWindow : Window
{
    private readonly OcrProcessingViewModel _viewModel;

    /// <summary>
    /// Создает новое окно обработки OCR
    /// </summary>
    public OcrProcessingWindow()
    {
        InitializeComponent();

        _viewModel = new OcrProcessingViewModel();
        DataContext = _viewModel;
    }

    /// <summary>
    /// Обновляет статус на "Захват области экрана"
    /// Безопасно вызывается из любого потока
    /// </summary>
    public void SetStageCapturing()
    {
        Dispatcher.Invoke(() => _viewModel.SetStageCapturing());
    }

    /// <summary>
    /// Обновляет статус на "Обработка изображения"
    /// Безопасно вызывается из любого потока
    /// </summary>
    public void SetStageProcessing()
    {
        Dispatcher.Invoke(() => _viewModel.SetStageProcessing());
    }

    /// <summary>
    /// Обновляет статус на "Распознавание текста"
    /// Безопасно вызывается из любого потока
    /// </summary>
    public void SetStageRecognizing()
    {
        Dispatcher.Invoke(() => _viewModel.SetStageRecognizing());
    }

    /// <summary>
    /// Устанавливает произвольный статус
    /// Безопасно вызывается из любого потока
    /// </summary>
    public void SetCustomStatus(string stage, string message)
    {
        Dispatcher.Invoke(() => _viewModel.SetCustomStatus(stage, message));
    }
}
