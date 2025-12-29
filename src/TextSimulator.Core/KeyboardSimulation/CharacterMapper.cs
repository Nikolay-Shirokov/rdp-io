using TextSimulator.Infrastructure.Logging;
using TextSimulator.Infrastructure.Win32;

namespace TextSimulator.Core.KeyboardSimulation;

/// <summary>
/// Маппинг символов на виртуальные клавиши
/// TASK-07: ASCII символы
/// TASK-08: Поддержка русской раскладки (кириллица)
/// </summary>
public class CharacterMapper
{
    private readonly Dictionary<char, KeyMapping> _englishMappings;
    private readonly Dictionary<char, KeyMapping> _russianMappings;
    private readonly ILogger _logger;

    public CharacterMapper(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _englishMappings = InitializeEnglishMappings();
        _russianMappings = InitializeRussianMappings();
    }

    /// <summary>
    /// Получает маппинг для символа с учетом раскладки
    /// </summary>
    public KeyMapping GetKeyMapping(char character, KeyboardLayout layout)
    {
        var mappings = layout == KeyboardLayout.Russian ? _russianMappings : _englishMappings;

        if (mappings.TryGetValue(character, out KeyMapping? mapping))
        {
            return mapping;
        }

        throw new UnsupportedCharacterException($"Символ '{character}' (U+{(int)character:X4}) не поддерживается в раскладке {layout}");
    }

    /// <summary>
    /// Проверяет поддержку символа
    /// </summary>
    public bool IsCharacterSupported(char character)
    {
        return _englishMappings.ContainsKey(character) ||
               _russianMappings.ContainsKey(character);
    }

    // ===== Initialization =====

    /// <summary>
    /// Инициализирует маппинг для английской раскладки (ASCII символы)
    /// </summary>
    private Dictionary<char, KeyMapping> InitializeEnglishMappings()
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

        _logger.LogInfo($"Английский маппинг инициализирован: {mappings.Count} символов");

        return mappings;
    }

    /// <summary>
    /// Инициализирует маппинг для русской раскладки (ЙЦУКЕН)
    /// </summary>
    private Dictionary<char, KeyMapping> InitializeRussianMappings()
    {
        var mappings = new Dictionary<char, KeyMapping>();

        // Русская раскладка ЙЦУКЕН (строчные буквы)
        var lowerRussian = "йцукенгшщзхъфывапролджэячсмитьбю";
        var lowerEnglishKeys = "qwertyuiop[]asdfghjkl;'zxcvbnm,.";

        for (int i = 0; i < lowerRussian.Length; i++)
        {
            char rusChar = lowerRussian[i];
            char engKey = lowerEnglishKeys[i];

            VirtualKeyCode vkCode = GetVirtualKeyForChar(engKey);

            mappings[rusChar] = new KeyMapping
            {
                Character = rusChar,
                VirtualKeyCode = vkCode,
                RequiresShift = false
            };
        }

        // Русская раскладка ЙЦУКЕН (заглавные буквы - требуют Shift)
        var upperRussian = "ЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ";

        for (int i = 0; i < upperRussian.Length; i++)
        {
            char rusChar = upperRussian[i];
            char engKey = lowerEnglishKeys[i];

            VirtualKeyCode vkCode = GetVirtualKeyForChar(engKey);

            mappings[rusChar] = new KeyMapping
            {
                Character = rusChar,
                VirtualKeyCode = vkCode,
                RequiresShift = true
            };
        }

        // Буква Ё/ё (на клавише `)
        mappings['ё'] = new KeyMapping { Character = 'ё', VirtualKeyCode = VirtualKeyCode.OEM_3, RequiresShift = false };
        mappings['Ё'] = new KeyMapping { Character = 'Ё', VirtualKeyCode = VirtualKeyCode.OEM_3, RequiresShift = true };

        _logger.LogInfo($"Русский маппинг инициализирован: {mappings.Count} символов");

        return mappings;
    }

    /// <summary>
    /// Получает VirtualKeyCode для английского символа
    /// </summary>
    private VirtualKeyCode GetVirtualKeyForChar(char c)
    {
        // Буквы a-z
        if (c >= 'a' && c <= 'z')
            return (VirtualKeyCode)(c - 'a' + (int)VirtualKeyCode.VK_A);

        // OEM клавиши
        return c switch
        {
            '[' => VirtualKeyCode.OEM_4,
            ']' => VirtualKeyCode.OEM_6,
            ';' => VirtualKeyCode.OEM_1,
            '\'' => VirtualKeyCode.OEM_7,
            ',' => VirtualKeyCode.OEM_COMMA,
            '.' => VirtualKeyCode.OEM_PERIOD,
            '/' => VirtualKeyCode.OEM_2,
            '`' => VirtualKeyCode.OEM_3,
            _ => VirtualKeyCode.SPACE // Fallback (не должно использоваться)
        };
    }
}
