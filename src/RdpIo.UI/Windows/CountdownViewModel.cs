using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RdpIo.UI.Commands;

namespace RdpIo.UI.Windows;

/// <summary>
/// ViewModel для окна обратного отсчета
/// </summary>
public class CountdownViewModel : INotifyPropertyChanged
{
    private int _remainingSeconds;

    /// <summary>
    /// Количество оставшихся секунд
    /// </summary>
    public int RemainingSeconds
    {
        get => _remainingSeconds;
        set
        {
            _remainingSeconds = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Команда отмены обратного отсчета
    /// </summary>
    public ICommand CancelCommand { get; }

    /// <summary>
    /// Событие запроса отмены (обрабатывается в Window)
    /// </summary>
    public event EventHandler? CancelRequested;

    /// <summary>
    /// Создает новый ViewModel с указанным количеством секунд
    /// </summary>
    public CountdownViewModel(int initialSeconds)
    {
        RemainingSeconds = initialSeconds;
        CancelCommand = new RelayCommand(_ => OnCancel());
    }

    /// <summary>
    /// Вызывает событие отмены
    /// </summary>
    private void OnCancel()
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

