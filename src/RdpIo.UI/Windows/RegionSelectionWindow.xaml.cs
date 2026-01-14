using System.Windows;
using System.Windows.Input;
using RdpIo.Infrastructure.ScreenCapture;

namespace RdpIo.UI.Windows;

/// <summary>
/// Полноэкранное окно для выбора области экрана для OCR
/// </summary>
public partial class RegionSelectionWindow : Window
{
    private readonly RegionSelectionViewModel _viewModel;

    /// <summary>
    /// Событие завершения выбора области
    /// </summary>
    public event EventHandler<ScreenCaptureRegion>? RegionSelected;

    /// <summary>
    /// Событие отмены выбора области
    /// </summary>
    public event EventHandler? SelectionCancelled;

    /// <summary>
    /// Создает новое окно выбора области экрана
    /// </summary>
    public RegionSelectionWindow()
    {
        InitializeComponent();

        // Устанавливаем окно на весь виртуальный экран (все мониторы)
        Left = SystemParameters.VirtualScreenLeft;
        Top = SystemParameters.VirtualScreenTop;
        Width = SystemParameters.VirtualScreenWidth;
        Height = SystemParameters.VirtualScreenHeight;

        _viewModel = new RegionSelectionViewModel();
        _viewModel.CancelRequested += (s, e) =>
        {
            SelectionCancelled?.Invoke(this, EventArgs.Empty);
            Close();
        };
        DataContext = _viewModel;

        // Обработка клавиши Esc для отмены
        KeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                _viewModel.Cancel();
            }
        };

        // Автофокус для обработки клавиатуры
        Loaded += (s, e) => Focus();
    }

    /// <summary>
    /// Начало выделения области (MouseLeftButtonDown)
    /// </summary>
    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var position = e.GetPosition(SelectionCanvas);
        _viewModel.StartSelection(position);

        // Захватываем мышь для получения событий вне окна
        SelectionCanvas.CaptureMouse();
    }

    /// <summary>
    /// Движение мыши при выделении области
    /// </summary>
    private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        // Обновляем позицию кастомного курсора
        var position = e.GetPosition(SelectionCanvas);
        System.Windows.Controls.Canvas.SetLeft(CustomCursor, position.X);
        System.Windows.Controls.Canvas.SetTop(CustomCursor, position.Y);

        // Обновляем выделение если идёт процесс
        if (!_viewModel.IsSelecting)
            return;

        _viewModel.UpdateSelection(position);
    }

    /// <summary>
    /// Завершение выделения области (MouseLeftButtonUp)
    /// </summary>
    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_viewModel.IsSelecting)
            return;

        // Освобождаем захват мыши
        SelectionCanvas.ReleaseMouseCapture();

        _viewModel.EndSelection();

        // Проверяем валидность выбранной области (минимум 10x10 пикселей)
        if (!_viewModel.IsSelectionValid(minSize: 10))
        {
            System.Windows.MessageBox.Show(
                "Выбранная область слишком мала. Минимальный размер: 10×10 пикселей.",
                "rdp-io - Ошибка выбора области",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning
            );
            return;
        }

        // Получаем DPI масштаб для конвертации WPF координат в физические пиксели
        var source = PresentationSource.FromVisual(this);
        double dpiScaleX = 1.0;
        double dpiScaleY = 1.0;

        if (source != null)
        {
            dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
            dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
        }

        // ДИАГНОСТИКА
        System.Diagnostics.Debug.WriteLine($"=== REGION SELECTION DEBUG ===");
        System.Diagnostics.Debug.WriteLine($"Window.Left: {Left}, Window.Top: {Top}");
        System.Diagnostics.Debug.WriteLine($"Selection (WPF): X={_viewModel.SelectionX}, Y={_viewModel.SelectionY}, W={_viewModel.SelectionWidth}, H={_viewModel.SelectionHeight}");
        System.Diagnostics.Debug.WriteLine($"DPI Scale: X={dpiScaleX}, Y={dpiScaleY}");
        System.Diagnostics.Debug.WriteLine($"VirtualScreen: Left={SystemParameters.VirtualScreenLeft}, Top={SystemParameters.VirtualScreenTop}");

        // Преобразуем WPF координаты в физические пиксели для screen capture
        var region = new ScreenCaptureRegion(
            x: (int)((_viewModel.SelectionX + Left) * dpiScaleX),
            y: (int)((_viewModel.SelectionY + Top) * dpiScaleY),
            width: (int)(_viewModel.SelectionWidth * dpiScaleX),
            height: (int)(_viewModel.SelectionHeight * dpiScaleY)
        );

        System.Diagnostics.Debug.WriteLine($"Final region (physical pixels): {region}");
        System.Diagnostics.Debug.WriteLine($"==============================");

        // Вызываем событие с выбранной областью
        RegionSelected?.Invoke(this, region);

        // Закрываем окно
        Close();
    }
}
