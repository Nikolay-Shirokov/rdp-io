namespace RdpIo.Core.KeyboardSimulation;

/// <summary>
/// Исключение при ошибке симуляции клавиатурного ввода
/// </summary>
public class KeyboardSimulationException : Exception
{
    /// <summary>
    /// Создает новое исключение с указанным сообщением
    /// </summary>
    public KeyboardSimulationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Создает новое исключение с указанным сообщением и внутренним исключением
    /// </summary>
    public KeyboardSimulationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

