using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TextSimulator.UI.Commands;

namespace TextSimulator.UI.Windows;

/// <summary>
/// ViewModel для окна прогресса передачи текста
/// </summary>
public class ProgressViewModel : INotifyPropertyChanged
{
    private int _currentPosition;
    private int _totalCharacters;
    private double _percentageComplete;
    private string _estimatedTimeRemaining = "расчет...";

    /// <summary>
    /// Текущая позиция в тексте
    /// </summary>
    public int CurrentPosition
    {
        get => _currentPosition;
        set
        {
            _currentPosition = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Общее количество символов для передачи
    /// </summary>
    public int TotalCharacters
    {
        get => _totalCharacters;
        set
        {
            _totalCharacters = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Процент завершения (0-100)
    /// </summary>
    public double PercentageComplete
    {
        get => _percentageComplete;
        set
        {
            _percentageComplete = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Оценочное оставшееся время в текстовом формате
    /// </summary>
    public string EstimatedTimeRemaining
    {
        get => _estimatedTimeRemaining;
        set
        {
            _estimatedTimeRemaining = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Команда отмены передачи
    /// </summary>
    public ICommand CancelCommand { get; }

    /// <summary>
    /// Событие запроса отмены (обрабатывается в Window)
    /// </summary>
    public event EventHandler? CancelRequested;

    /// <summary>
    /// Создает новый ViewModel для окна прогресса
    /// </summary>
    public ProgressViewModel()
    {
        CancelCommand = new RelayCommand(_ => Cancel());
    }

    /// <summary>
    /// Вызывает событие отмены передачи
    /// </summary>
    public void Cancel()
    {
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
