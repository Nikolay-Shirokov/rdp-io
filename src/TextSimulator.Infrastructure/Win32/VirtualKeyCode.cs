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

    // Дополнительные клавиши (Win, Apps, и др)
    LWIN = 0x5B,
    RWIN = 0x5C,
    APPS = 0x5D,
    SLEEP = 0x5F,
    
    // NumPud
    NUMPAD0 = 0x60,
    NUMPAD1 = 0x61,
    NUMPAD2 = 0x62,
    NUMPAD3 = 0x63,
    NUMPAD4 = 0x64,
    NUMPAD5 = 0x65,
    NUMPAD6 = 0x66,
    NUMPAD7 = 0x67,
    NUMPAD8 = 0x68,
    NUMPAD9 = 0x69,
    MULTIPLY = 0x6A,
    ADD = 0x6B,
    SEPARATOR = 0x6C,
    SUBTRACT = 0x6D,
    DECIMAL = 0x6E,
    DIVIDE = 0x6F,

    NUMLOCK = 0x90,
    SCROLL = 0x91,
    
    LSHIFT = 0xA0,
    RSHIFT = 0xA1,
    LCONTROL = 0xA2,
    RCONTROL = 0xA3,
    LMENU = 0xA4,
    RMENU = 0xA5,

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
