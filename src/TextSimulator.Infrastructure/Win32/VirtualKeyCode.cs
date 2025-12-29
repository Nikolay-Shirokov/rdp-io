namespace TextSimulator.Infrastructure.Win32;

/// <summary>
/// Virtual Key Codes for Windows keyboard input
/// </summary>
public enum VirtualKeyCode : ushort
{
    // Буквы A-Z
    VK_A = 0x41,
    VK_B = 0x42,
    VK_C = 0x43,
    VK_D = 0x44,
    VK_E = 0x45,
    VK_F = 0x46,
    VK_G = 0x47,
    VK_H = 0x48,
    VK_I = 0x49,
    VK_J = 0x4A,
    VK_K = 0x4B,
    VK_L = 0x4C,
    VK_M = 0x4D,
    VK_N = 0x4E,
    VK_O = 0x4F,
    VK_P = 0x50,
    VK_Q = 0x51,
    VK_R = 0x52,
    VK_S = 0x53,
    VK_T = 0x54,
    VK_U = 0x55,
    VK_V = 0x56,
    VK_W = 0x57,
    VK_X = 0x58,
    VK_Y = 0x59,
    VK_Z = 0x5A,

    // Цифры 0-9
    VK_0 = 0x30,
    VK_1 = 0x31,
    VK_2 = 0x32,
    VK_3 = 0x33,
    VK_4 = 0x34,
    VK_5 = 0x35,
    VK_6 = 0x36,
    VK_7 = 0x37,
    VK_8 = 0x38,
    VK_9 = 0x39,

    // Модификаторы
    SHIFT = 0x10,
    CONTROL = 0x11,
    MENU = 0x12,  // Alt key

    // Специальные клавиши
    SPACE = 0x20,
    RETURN = 0x0D,
    TAB = 0x09,
    ESCAPE = 0x1B,
    BACK = 0x08,
    DELETE = 0x2E,
    INSERT = 0x2D,
    HOME = 0x24,
    END = 0x23,
    PRIOR = 0x21,  // Page Up
    NEXT = 0x22,   // Page Down

    // Стрелки
    LEFT = 0x25,
    UP = 0x26,
    RIGHT = 0x27,
    DOWN = 0x28,

    // OEM keys (зависят от раскладки клавиатуры)
    OEM_1 = 0xBA,      // ';:' на US клавиатуре
    OEM_2 = 0xBF,      // '/?' на US клавиатуре
    OEM_3 = 0xC0,      // '`~' на US клавиатуре
    OEM_4 = 0xDB,      // '[{' на US клавиатуре
    OEM_5 = 0xDC,      // '\|' на US клавиатуре
    OEM_6 = 0xDD,      // ']}' на US клавиатуре
    OEM_7 = 0xDE,      // ''"' на US клавиатуре
    OEM_PLUS = 0xBB,   // '=+' на US клавиатуре
    OEM_COMMA = 0xBC,  // ',<' на US клавиатуре
    OEM_MINUS = 0xBD,  // '-_' на US клавиатуре
    OEM_PERIOD = 0xBE  // '.>' на US клавиатуре
}
