namespace TextSimulator.Core.KeyboardSimulation;

/// <summary>
/// Результат передачи текста
/// </summary>
public class TransmissionResult
{
    /// <summary>
    /// Общее количество символов в тексте
    /// </summary>
    public int TotalCharacters { get; set; }

    /// <summary>
    /// Количество успешно переданных символов
    /// </summary>
    public int TransmittedCharacters { get; set; }

    /// <summary>
    /// Количество символов, которые не удалось передать
    /// </summary>
    public int FailedCharacters { get; set; }

    /// <summary>
    /// Список неподдерживаемых символов, которые были пропущены
    /// </summary>
    public List<char> UnsupportedCharacters { get; set; } = new();

    /// <summary>
    /// Успешно ли завершена передача (без ошибок)
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Была ли отменена передача пользователем
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Сообщение об ошибке (если есть)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Время начала передачи
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Время завершения передачи
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Длительность передачи
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;
}
