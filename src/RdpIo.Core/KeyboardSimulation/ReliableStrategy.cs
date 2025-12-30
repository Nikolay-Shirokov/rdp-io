namespace RdpIo.Core.KeyboardSimulation;

/// <summary>
/// Надежная стратегия передачи - 50 мс между символами (по умолчанию)
/// </summary>
public class ReliableStrategy : TransmissionStrategy
{
    /// <summary>
    /// Задержка: 50 мс (надежная передача, рекомендуется)
    /// </summary>
    public override int DelayBetweenCharacters => 50;

    /// <summary>
    /// Название стратегии
    /// </summary>
    public override string Name => "Надежная";

    /// <summary>
    /// Описание стратегии
    /// </summary>
    public override string Description => "Надежная передача текста (50 мс между символами). Рекомендуется для большинства случаев.";
}

