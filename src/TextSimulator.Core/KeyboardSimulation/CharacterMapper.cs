using TextSimulator.Infrastructure.Logging;
using TextSimulator.Infrastructure.Win32;

namespace TextSimulator.Core.KeyboardSimulation;

/// <summary>
/// Маппинг символов на виртуальные клавиши
/// TASK-07: Реализация только для ASCII символов
/// </summary>
public class CharacterMapper
{
    private readonly Dictionary<char, KeyMapping> _mappings;
    private readonly ILogger _logger;

    public CharacterMapper(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mappings = InitializeAsciiMappings();
    }

    /// <summary>
    /// Получает маппинг для символа
    /// </summary>
    public KeyMapping GetKeyMapping(char character)
    {
        if (_mappings.TryGetValue(character, out KeyMapping? mapping))
        {
            return mapping;
        }

        throw new UnsupportedCharacterException($"Символ '{character}' (U+{(int)character:X4}) не поддерживается");
    }

    /// <summary>
    /// Проверяет поддержку символа
    /// </summary>
    public bool IsCharacterSupported(char character)
    {
        return _mappings.ContainsKey(character);
    }

    // ===== Initialization =====

    /// <summary>
    /// Инициализирует маппинг для ASCII символов
    /// </summary>
    private Dictionary<char, KeyMapping> InitializeAsciiMappings()
    {
        var mappings = new Dictionary<char, KeyMapping>();

        // Буквы a-z (lowercase)
        for (char c = 'a'; c <= 'z'; c++)
        {
            mappings[c] = new KeyMapping
            {
                Character = c,
                VirtualKeyCode = (VirtualKeyCode)(c - 'a' + (int)VirtualKeyCode.VK_A),
                RequiresShift = false
            };
        }

        // Буквы A-Z (uppercase) - требуют Shift
        for (char c = 'A'; c <= 'Z'; c++)
        {
            mappings[c] = new KeyMapping
            {
                Character = c,
                VirtualKeyCode = (VirtualKeyCode)(c - 'A' + (int)VirtualKeyCode.VK_A),
                RequiresShift = true
            };
        }

        // Цифры 0-9
        for (char c = '0'; c <= '9'; c++)
        {
            mappings[c] = new KeyMapping
            {
                Character = c,
                VirtualKeyCode = (VirtualKeyCode)(c - '0' + (int)VirtualKeyCode.VK_0),
                RequiresShift = false
            };
        }

        // Управляющие символы
        mappings[' '] = new KeyMapping { Character = ' ', VirtualKeyCode = VirtualKeyCode.SPACE, RequiresShift = false };
        mappings['\t'] = new KeyMapping { Character = '\t', VirtualKeyCode = VirtualKeyCode.TAB, RequiresShift = false };
        mappings['\n'] = new KeyMapping { Character = '\n', VirtualKeyCode = VirtualKeyCode.RETURN, RequiresShift = false };
        mappings['\r'] = new KeyMapping { Character = '\r', VirtualKeyCode = VirtualKeyCode.RETURN, RequiresShift = false };

        // Пунктуация (без Shift)
        mappings['.'] = new KeyMapping { Character = '.', VirtualKeyCode = VirtualKeyCode.OEM_PERIOD, RequiresShift = false };
        mappings[','] = new KeyMapping { Character = ',', VirtualKeyCode = VirtualKeyCode.OEM_COMMA, RequiresShift = false };
        mappings[';'] = new KeyMapping { Character = ';', VirtualKeyCode = VirtualKeyCode.OEM_1, RequiresShift = false };
        mappings['-'] = new KeyMapping { Character = '-', VirtualKeyCode = VirtualKeyCode.OEM_MINUS, RequiresShift = false };
        mappings['='] = new KeyMapping { Character = '=', VirtualKeyCode = VirtualKeyCode.OEM_PLUS, RequiresShift = false };
        mappings['['] = new KeyMapping { Character = '[', VirtualKeyCode = VirtualKeyCode.OEM_4, RequiresShift = false };
        mappings[']'] = new KeyMapping { Character = ']', VirtualKeyCode = VirtualKeyCode.OEM_6, RequiresShift = false };
        mappings['\\'] = new KeyMapping { Character = '\\', VirtualKeyCode = VirtualKeyCode.OEM_5, RequiresShift = false };
        mappings['/'] = new KeyMapping { Character = '/', VirtualKeyCode = VirtualKeyCode.OEM_2, RequiresShift = false };
        mappings['\''] = new KeyMapping { Character = '\'', VirtualKeyCode = VirtualKeyCode.OEM_7, RequiresShift = false };
        mappings['`'] = new KeyMapping { Character = '`', VirtualKeyCode = VirtualKeyCode.OEM_3, RequiresShift = false };

        // Пунктуация (с Shift)
        mappings[':'] = new KeyMapping { Character = ':', VirtualKeyCode = VirtualKeyCode.OEM_1, RequiresShift = true };
        mappings['_'] = new KeyMapping { Character = '_', VirtualKeyCode = VirtualKeyCode.OEM_MINUS, RequiresShift = true };
        mappings['+'] = new KeyMapping { Character = '+', VirtualKeyCode = VirtualKeyCode.OEM_PLUS, RequiresShift = true };
        mappings['{'] = new KeyMapping { Character = '{', VirtualKeyCode = VirtualKeyCode.OEM_4, RequiresShift = true };
        mappings['}'] = new KeyMapping { Character = '}', VirtualKeyCode = VirtualKeyCode.OEM_6, RequiresShift = true };
        mappings['|'] = new KeyMapping { Character = '|', VirtualKeyCode = VirtualKeyCode.OEM_5, RequiresShift = true };
        mappings['?'] = new KeyMapping { Character = '?', VirtualKeyCode = VirtualKeyCode.OEM_2, RequiresShift = true };
        mappings['"'] = new KeyMapping { Character = '"', VirtualKeyCode = VirtualKeyCode.OEM_7, RequiresShift = true };
        mappings['~'] = new KeyMapping { Character = '~', VirtualKeyCode = VirtualKeyCode.OEM_3, RequiresShift = true };
        mappings['<'] = new KeyMapping { Character = '<', VirtualKeyCode = VirtualKeyCode.OEM_COMMA, RequiresShift = true };
        mappings['>'] = new KeyMapping { Character = '>', VirtualKeyCode = VirtualKeyCode.OEM_PERIOD, RequiresShift = true };

        // Спецсимволы на цифровых клавишах (с Shift)
        mappings['!'] = new KeyMapping { Character = '!', VirtualKeyCode = VirtualKeyCode.VK_1, RequiresShift = true };
        mappings['@'] = new KeyMapping { Character = '@', VirtualKeyCode = VirtualKeyCode.VK_2, RequiresShift = true };
        mappings['#'] = new KeyMapping { Character = '#', VirtualKeyCode = VirtualKeyCode.VK_3, RequiresShift = true };
        mappings['$'] = new KeyMapping { Character = '$', VirtualKeyCode = VirtualKeyCode.VK_4, RequiresShift = true };
        mappings['%'] = new KeyMapping { Character = '%', VirtualKeyCode = VirtualKeyCode.VK_5, RequiresShift = true };
        mappings['^'] = new KeyMapping { Character = '^', VirtualKeyCode = VirtualKeyCode.VK_6, RequiresShift = true };
        mappings['&'] = new KeyMapping { Character = '&', VirtualKeyCode = VirtualKeyCode.VK_7, RequiresShift = true };
        mappings['*'] = new KeyMapping { Character = '*', VirtualKeyCode = VirtualKeyCode.VK_8, RequiresShift = true };
        mappings['('] = new KeyMapping { Character = '(', VirtualKeyCode = VirtualKeyCode.VK_9, RequiresShift = true };
        mappings[')'] = new KeyMapping { Character = ')', VirtualKeyCode = VirtualKeyCode.VK_0, RequiresShift = true };

        _logger.LogInfo($"ASCII маппинг инициализирован: {mappings.Count} символов");

        return mappings;
    }
}
