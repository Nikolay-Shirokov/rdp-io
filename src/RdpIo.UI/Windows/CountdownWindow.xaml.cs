using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RdpIo.UI.Windows;

/// <summary>
/// Окно обратного отсчета перед началом передачи текста
/// Always-on-top, с возможностью отмены через Esc
/// </summary>
public partial class CountdownWindow : Window
{
    private readonly CountdownViewModel _viewModel;
    private readonly DispatcherTimer _timer;

    /// <summary>
    /// Событие завершения обратного отсчета
    /// </summary>
    public event EventHandler? CountdownCompleted;

    /// <summary>
    /// Событие отмены обратного отсчета
    /// </summary>
    public event EventHandler? CountdownCancelled;

    /// <summary>
    /// Создает новое окно обратного отсчета
    /// </summary>
    /// <param name="countdownSeconds">Количество секунд для отсчета (по умолчанию 5)</param>
    public CountdownWindow(int countdownSeconds = 5)
    {
        InitializeComponent();

        _viewModel = new CountdownViewModel(countdownSeconds);
        _viewModel.CancelRequested += (s, e) => Cancel();
        DataContext = _viewModel;

        // Таймер тикает каждую секунду
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += OnTimerTick;

        // Обработка клавиши Esc для отмены
        KeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                Cancel();
            }
        };

        // Запускаем таймер после загрузки окна
        Loaded += (s, e) => _timer.Start();
    }

    /// <summary>
    /// Обработчик тика таймера
    /// </summary>
    private void OnTimerTick(object? sender, EventArgs e)
    {
        _viewModel.RemainingSeconds--;

        if (_viewModel.RemainingSeconds == 0)
        {
            // Отсчет завершен
            _timer.Stop();
            CountdownCompleted?.Invoke(this, EventArgs.Empty);
            Close();
        }
        else if (_viewModel.RemainingSeconds == 1)
        {
            // На последней секунде меняем текст на "START!" с зеленым цветом
            CountdownText.Text = "START!";
            CountdownText.Foreground = new SolidColorBrush(Colors.LimeGreen);
        }
    }

    /// <summary>
    /// Отменяет обратный отсчет и закрывает окно
    /// </summary>
    private void Cancel()
    {
        _timer.Stop();
        CountdownCancelled?.Invoke(this, EventArgs.Empty);
        Close();
    }
}

