using System.Windows;
using System.Windows.Input;
using RdpIo.Core.KeyboardSimulation;

namespace RdpIo.UI.Windows;

/// <summary>
/// Окно отображения прогресса передачи текста
/// Always-on-top, с отображением статистики и возможностью отмены
/// </summary>
public partial class ProgressWindow : Window
{
    private readonly ProgressViewModel _viewModel;

    /// <summary>
    /// Событие запроса отмены передачи
    /// </summary>
    public event EventHandler? CancelRequested;

    /// <summary>
    /// Создает новое окно прогресса передачи
    /// </summary>
    public ProgressWindow()
    {
        InitializeComponent();

        _viewModel = new ProgressViewModel();
        _viewModel.CancelRequested += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
        DataContext = _viewModel;

        // Обработка клавиши Esc для отмены
        KeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                _viewModel.Cancel();
            }
        };
    }

    /// <summary>
    /// Обновляет отображаемый прогресс передачи
    /// Безопасно вызывается из любого потока
    /// </summary>
    /// <param name="progress">Данные о прогрессе передачи</param>
    public void UpdateProgress(TransmissionProgress progress)
    {
        // Используем Dispatcher для безопасного обновления UI из другого потока
        Dispatcher.Invoke(() =>
        {
            _viewModel.CurrentPosition = progress.CurrentPosition;
            _viewModel.TotalCharacters = progress.TotalCharacters;
            _viewModel.PercentageComplete = progress.PercentageComplete;
            _viewModel.EstimatedTimeRemaining = FormatTimeSpan(progress.EstimatedTimeRemaining);
        });
    }

    /// <summary>
    /// Форматирует TimeSpan в читаемую строку
    /// </summary>
    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 1)
            return "< 1 сек";

        if (timeSpan.TotalSeconds < 60)
            return $"{timeSpan.TotalSeconds:F0} сек";

        if (timeSpan.TotalMinutes < 60)
            return $"{timeSpan.Minutes} мин {timeSpan.Seconds} сек";

        return $"{timeSpan.Hours} ч {timeSpan.Minutes} мин";
    }
}

