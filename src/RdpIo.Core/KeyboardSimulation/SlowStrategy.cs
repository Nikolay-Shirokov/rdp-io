namespace RdpIo.Core.KeyboardSimulation;

/// <summary>
/// Медленная стратегия передачи - 100 мс между символами
/// </summary>
public class SlowStrategy : TransmissionStrategy
{
    /// <summary>
    /// Задержка: 100 мс (медленная передача)
    /// </summary>
    public override int DelayBetweenCharacters => 100;

    /// <summary>
    /// Название стратегии
    /// </summary>
    public override string Name => "Медленная";

    /// <summary>
    /// Описание стратегии
    /// </summary>
    public override string Description => "Медленная передача текста (100 мс между символами). Максимальная надежность для медленных приложений.";
}

