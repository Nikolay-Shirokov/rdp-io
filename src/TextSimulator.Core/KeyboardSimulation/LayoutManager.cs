using TextSimulator.Infrastructure.Logging;

namespace TextSimulator.Core.KeyboardSimulation;

/// <summary>
/// Управляет определением и переключением раскладок клавиатуры
/// </summary>
public class LayoutManager
{
    private readonly ILogger _logger;

    /// <summary>
    /// Текущая активная раскладка
    /// </summary>
    public KeyboardLayout CurrentLayout { get; set; } = KeyboardLayout.English;

    public LayoutManager(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Определяет необходимую раскладку для символа
    /// </summary>
    public KeyboardLayout GetLayoutForCharacter(char character)
    {
        // Кириллица: U+0400 - U+04FF
        if (character >= '\u0400' && character <= '\u04FF')
        {
            return KeyboardLayout.Russian;
        }

        // ASCII и спецсимволы: U+0020 - U+007E
        if (character >= '\u0020' && character <= '\u007E')
        {
            return KeyboardLayout.English;
        }

        // Управляющие символы (Tab, Enter, и т.д.)
        if (char.IsControl(character))
        {
            return KeyboardLayout.Neutral; // Не зависит от раскладки
        }

        // По умолчанию - английская раскладка
        _logger.LogWarning($"Неизвестный диапазон символа '{character}' (U+{(int)character:X4}), используется английская раскладка");
        return KeyboardLayout.English;
    }

    /// <summary>
    /// Проверяет, нужно ли переключать раскладку для данного символа
    /// </summary>
    public bool RequiresLayoutSwitch(char character)
    {
        KeyboardLayout required = GetLayoutForCharacter(character);
        return required != KeyboardLayout.Neutral && required != CurrentLayout;
    }
}
