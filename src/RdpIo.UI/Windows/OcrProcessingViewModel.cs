using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RdpIo.UI.Windows;

/// <summary>
/// ViewModel для окна обработки OCR
/// </summary>
public class OcrProcessingViewModel : INotifyPropertyChanged
{
    private string _statusMessage = "Захват области экрана...";
    private string _currentStage = "1/3";

    /// <summary>
    /// Текущее сообщение о статусе обработки
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Текущий этап обработки (например, "1/3", "2/3", "3/3")
    /// </summary>
    public string CurrentStage
    {
        get => _currentStage;
        set
        {
            _currentStage = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Обновляет статус на "Захват области экрана"
    /// </summary>
    public void SetStageCapturing()
    {
        CurrentStage = "1/3";
        StatusMessage = "Захват области экрана...";
    }

    /// <summary>
    /// Обновляет статус на "Обработка изображения"
    /// </summary>
    public void SetStageProcessing()
    {
        CurrentStage = "2/3";
        StatusMessage = "Обработка изображения...";
    }

    /// <summary>
    /// Обновляет статус на "Распознавание текста"
    /// </summary>
    public void SetStageRecognizing()
    {
        CurrentStage = "3/3";
        StatusMessage = "Распознавание текста...";
    }

    /// <summary>
    /// Устанавливает произвольный статус
    /// </summary>
    public void SetCustomStatus(string stage, string message)
    {
        CurrentStage = stage;
        StatusMessage = message;
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
