using System.Windows;
using RdpIo.Infrastructure.OcrManagement;

namespace RdpIo.UI.Windows;

/// <summary>
/// Окно отображения результатов OCR
/// </summary>
public partial class OcrResultWindow : Window
{
    private readonly OcrResultViewModel _viewModel;

    /// <summary>
    /// Событие запроса отправки текста в RDP
    /// </summary>
    public event EventHandler? SendToRdpRequested;

    /// <summary>
    /// Создает новое окно результатов OCR
    /// </summary>
    public OcrResultWindow()
    {
        InitializeComponent();

        _viewModel = new OcrResultViewModel();
        _viewModel.SendToRdpRequested += (s, e) =>
        {
            SendToRdpRequested?.Invoke(this, EventArgs.Empty);
            Close();
        };
        _viewModel.CloseRequested += (s, e) => Close();
        DataContext = _viewModel;
    }

    /// <summary>
    /// Устанавливает результат OCR для отображения
    /// </summary>
    /// <param name="result">Результат OCR распознавания</param>
    public void SetResult(OcrResult result)
    {
        _viewModel.SetResult(result);
    }

    /// <summary>
    /// Получает текст из окна (может быть отредактирован пользователем)
    /// </summary>
    public string GetText()
    {
        return _viewModel.RecognizedText;
    }

    /// <summary>
    /// Обработчик перетаскивания окна за заголовок
    /// </summary>
    private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}
