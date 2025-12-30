using System.Windows.Input;

namespace RdpIo.UI.Commands;

/// <summary>
/// Реализация ICommand для использования в MVVM
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>
    /// Создает новую команду
    /// </summary>
    /// <param name="execute">Действие для выполнения</param>
    /// <param name="canExecute">Условие, когда команда доступна (опционально)</param>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Определяет, может ли команда быть выполнена
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    /// <summary>
    /// Выполняет команду
    /// </summary>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    /// <summary>
    /// Происходит при изменении условий выполнения команды
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Уведомляет об изменении состояния команды
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}

