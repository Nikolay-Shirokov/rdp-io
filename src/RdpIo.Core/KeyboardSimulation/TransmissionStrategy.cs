namespace RdpIo.Core.KeyboardSimulation;

/// <summary>
/// Базовый абстрактный класс для стратегий скорости передачи
/// </summary>
public abstract class TransmissionStrategy
{
    /// <summary>
    /// Задержка между символами в миллисекундах
    /// </summary>
    public abstract int DelayBetweenCharacters { get; }

    /// <summary>
    /// Название стратегии
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Описание стратегии
    /// </summary>
    public abstract string Description { get; }
}

