using TextSimulator.Infrastructure.Win32;

namespace TextSimulator.Core.KeyboardSimulation;

/// <summary>
/// Описывает маппинг символа на виртуальную клавишу
/// </summary>
public class KeyMapping
{
    /// <summary>
    /// Исходный символ
    /// </summary>
    public char Character { get; set; }

    /// <summary>
    /// Виртуальный код клавиши
    /// </summary>
    public VirtualKeyCode VirtualKeyCode { get; set; }

    /// <summary>
    /// Требуется ли нажатие Shift
    /// </summary>
    public bool RequiresShift { get; set; }

    /// <summary>
    /// Требуется ли нажатие Ctrl
    /// </summary>
    public bool RequiresCtrl { get; set; }

    /// <summary>
    /// Требуется ли нажатие Alt
    /// </summary>
    public bool RequiresAlt { get; set; }

    /// <summary>
    /// Scan код (опционально, для специфичных случаев)
    /// </summary>
    public ushort? ScanCode { get; set; }

    public override string ToString()
    {
        return $"'{Character}' -> {VirtualKeyCode}" +
               (RequiresShift ? " + Shift" : "") +
               (RequiresCtrl ? " + Ctrl" : "") +
               (RequiresAlt ? " + Alt" : "");
    }
}
