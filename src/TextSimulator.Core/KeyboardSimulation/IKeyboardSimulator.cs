namespace TextSimulator.Core.KeyboardSimulation;

/// <summary>
/// Интерфейс для симуляции клавиатурного ввода
/// </summary>
public interface IKeyboardSimulator
{
    /// <summary>
    /// Передает текст посимвольно через эмуляцию клавиатурного ввода
    /// </summary>
    /// <param name="text">Текст для передачи</param>
    /// <param name="progress">Callback для отслеживания прогресса передачи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат передачи с детальной статистикой</returns>
    Task<TransmissionResult> TransmitTextAsync(
        string text,
        IProgress<TransmissionProgress>? progress,
        CancellationToken cancellationToken);

    /// <summary>
    /// Устанавливает стратегию скорости передачи
    /// </summary>
    /// <param name="strategy">Стратегия передачи (Fast/Reliable/Slow)</param>
    void SetTransmissionStrategy(TransmissionStrategy strategy);

    /// <summary>
    /// Проверяет, поддерживается ли указанный символ
    /// </summary>
    /// <param name="character">Символ для проверки</param>
    /// <returns>true, если символ поддерживается</returns>
    bool IsCharacterSupported(char character);

    /// <summary>
    /// Получает список неподдерживаемых символов в тексте
    /// </summary>
    /// <param name="text">Текст для анализа</param>
    /// <returns>Коллекция уникальных неподдерживаемых символов</returns>
    IEnumerable<char> GetUnsupportedCharacters(string text);
}
