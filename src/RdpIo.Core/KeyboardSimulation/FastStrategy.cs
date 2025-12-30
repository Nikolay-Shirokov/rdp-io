namespace RdpIo.Core.KeyboardSimulation;

/// <summary>
/// Быстрая стратегия передачи - 20 мс между символами
/// </summary>
public class FastStrategy : TransmissionStrategy
{
    /// <summary>
    /// Задержка: 20 мс (быстрая передача)
    /// </summary>
    public override int DelayBetweenCharacters => 20;

    /// <summary>
    /// Название стратегии
    /// </summary>
    public override string Name => "Быстрая";

    /// <summary>
    /// Описание стратегии
    /// </summary>
    public override string Description => "Быстрая передача текста (20 мс между символами). Может вызывать пропуски в некоторых приложениях.";
}

