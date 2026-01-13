using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using RdpIo.UI.Commands;

namespace RdpIo.UI.Windows;

/// <summary>
/// ViewModel для окна выбора области экрана для OCR
/// </summary>
public class RegionSelectionViewModel : INotifyPropertyChanged
{
    private System.Windows.Point _selectionStart;
    private System.Windows.Point _selectionCurrent;
    private bool _isSelecting;
    private Visibility _selectionRectangleVisibility = Visibility.Collapsed;

    /// <summary>
    /// Начальная точка выделения
    /// </summary>
    public System.Windows.Point SelectionStart
    {
        get => _selectionStart;
        set
        {
            _selectionStart = value;
            OnPropertyChanged();
            UpdateSelectionRectangle();
        }
    }

    /// <summary>
    /// Текущая точка выделения (движение мыши)
    /// </summary>
    public System.Windows.Point SelectionCurrent
    {
        get => _selectionCurrent;
        set
        {
            _selectionCurrent = value;
            OnPropertyChanged();
            UpdateSelectionRectangle();
        }
    }

    /// <summary>
    /// Флаг активного выделения области
    /// </summary>
    public bool IsSelecting
    {
        get => _isSelecting;
        set
        {
            _isSelecting = value;
            OnPropertyChanged();
            SelectionRectangleVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Видимость прямоугольника выделения
    /// </summary>
    public Visibility SelectionRectangleVisibility
    {
        get => _selectionRectangleVisibility;
        set
        {
            _selectionRectangleVisibility = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// X координата прямоугольника выделения
    /// </summary>
    public double SelectionX { get; private set; }

    /// <summary>
    /// Y координата прямоугольника выделения
    /// </summary>
    public double SelectionY { get; private set; }

    /// <summary>
    /// Ширина прямоугольника выделения
    /// </summary>
    public double SelectionWidth { get; private set; }

    /// <summary>
    /// Высота прямоугольника выделения
    /// </summary>
    public double SelectionHeight { get; private set; }

    /// <summary>
    /// Команда отмены выбора области (Esc)
    /// </summary>
    public ICommand CancelCommand { get; }

    /// <summary>
    /// Событие запроса отмены
    /// </summary>
    public event EventHandler? CancelRequested;

    /// <summary>
    /// Создает новый ViewModel для окна выбора области
    /// </summary>
    public RegionSelectionViewModel()
    {
        CancelCommand = new RelayCommand(_ => Cancel());
    }

    /// <summary>
    /// Начинает выделение области с указанной точки
    /// </summary>
    public void StartSelection(System.Windows.Point point)
    {
        SelectionStart = point;
        SelectionCurrent = point;
        IsSelecting = true;
    }

    /// <summary>
    /// Обновляет текущую точку выделения (движение мыши)
    /// </summary>
    public void UpdateSelection(System.Windows.Point point)
    {
        if (!IsSelecting)
            return;

        SelectionCurrent = point;
    }

    /// <summary>
    /// Завершает выделение области
    /// </summary>
    public void EndSelection()
    {
        IsSelecting = false;
    }

    /// <summary>
    /// Отменяет выбор области
    /// </summary>
    public void Cancel()
    {
        IsSelecting = false;
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Обновляет параметры прямоугольника выделения
    /// </summary>
    private void UpdateSelectionRectangle()
    {
        // Вычисляем X, Y, Width, Height из двух точек
        SelectionX = Math.Min(SelectionStart.X, SelectionCurrent.X);
        SelectionY = Math.Min(SelectionStart.Y, SelectionCurrent.Y);
        SelectionWidth = Math.Abs(SelectionCurrent.X - SelectionStart.X);
        SelectionHeight = Math.Abs(SelectionCurrent.Y - SelectionStart.Y);

        OnPropertyChanged(nameof(SelectionX));
        OnPropertyChanged(nameof(SelectionY));
        OnPropertyChanged(nameof(SelectionWidth));
        OnPropertyChanged(nameof(SelectionHeight));
    }

    /// <summary>
    /// Проверяет валидность выбранной области (минимальный размер)
    /// </summary>
    public bool IsSelectionValid(int minSize = 10)
    {
        return SelectionWidth >= minSize && SelectionHeight >= minSize;
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
