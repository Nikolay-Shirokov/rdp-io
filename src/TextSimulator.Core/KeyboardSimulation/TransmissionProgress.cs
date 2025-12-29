namespace TextSimulator.Core.KeyboardSimulation;

/// <summary>
/// Информация о прогрессе передачи текста
/// </summary>
public class TransmissionProgress
{
    /// <summary>
    /// Текущая позиция в тексте (количество обработанных символов)
    /// </summary>
    public int CurrentPosition { get; set; }

    /// <summary>
    /// Общее количество символов для передачи
    /// </summary>
    public int TotalCharacters { get; set; }

    /// <summary>
    /// Текущий обрабатываемый символ
    /// </summary>
    public char CurrentCharacter { get; set; }

    /// <summary>
    /// Оценочное время до завершения передачи
    /// </summary>
    public TimeSpan EstimatedTimeRemaining { get; set; }

    /// <summary>
    /// Процент завершения (0-100)
    /// </summary>
    public double PercentageComplete =>
        TotalCharacters > 0 ? (double)CurrentPosition / TotalCharacters * 100 : 0;

    /// <summary>
    /// Количество символов в секунду
    /// </summary>
    public int CharactersPerSecond { get; set; }
}
