using System.Runtime.InteropServices;

namespace TextSimulator.Infrastructure.Win32;

/// <summary>
/// INPUT structure for SendInput
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct INPUT
{
    public InputType Type;
    public InputUnion Data;
}

/// <summary>
/// Union of input types
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct InputUnion
{
    [FieldOffset(0)]
    public MOUSEINPUT Mouse;

    [FieldOffset(0)]
    public KEYBDINPUT Keyboard;

    [FieldOffset(0)]
    public HARDWAREINPUT Hardware;
}

/// <summary>
/// Keyboard input structure
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct KEYBDINPUT
{
    public ushort Vk;           // Virtual Key code
    public ushort Scan;         // Scan code
    public KeyEventFlags Flags; // Flags (KEYUP, etc.)
    public uint Time;           // Timestamp (0 = system will provide)
    public IntPtr ExtraInfo;    // Extra info
}

/// <summary>
/// Mouse input structure
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MOUSEINPUT
{
    public int X;
    public int Y;
    public uint MouseData;
    public uint Flags;
    public uint Time;
    public IntPtr ExtraInfo;
}

/// <summary>
/// Hardware input structure
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct HARDWAREINPUT
{
    public uint Msg;
    public ushort ParamL;
    public ushort ParamH;
}

/// <summary>
/// Input type enumeration
/// </summary>
public enum InputType : uint
{
    MOUSE = 0,
    KEYBOARD = 1,
    HARDWARE = 2
}

/// <summary>
/// Key event flags
/// </summary>
[Flags]
public enum KeyEventFlags : uint
{
    EXTENDEDKEY = 0x0001,
    KEYUP = 0x0002,
    UNICODE = 0x0004,
    SCANCODE = 0x0008
}
