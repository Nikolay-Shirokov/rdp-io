using System.Runtime.InteropServices;

namespace TextSimulator.Infrastructure.Win32;

/// <summary>
/// Win32 API wrapper implementation with P/Invoke
/// </summary>
public class Win32ApiWrapper : IWin32ApiWrapper
{
    // ===== Keyboard Input P/Invoke =====

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    /// <summary>
    /// Synthesizes keystrokes, mouse motions, and button clicks
    /// </summary>
    public uint SendInput(uint nInputs, INPUT[] pInputs)
    {
        int size = Marshal.SizeOf(typeof(INPUT));
        return SendInput(nInputs, pInputs, size);
    }

    [DllImport("user32.dll", EntryPoint = "MapVirtualKey", SetLastError = true)]
    private static extern uint NativeMapVirtualKey(uint uCode, uint uMapType);

    /// <summary>
    /// Translates (maps) a virtual-key code into a scan code or character value
    /// </summary>
    public uint MapVirtualKey(uint uCode, uint uMapType)
    {
        return NativeMapVirtualKey(uCode, uMapType);
    }

    // ===== Clipboard P/Invoke =====

    [DllImport("user32.dll", EntryPoint = "OpenClipboard", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeOpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll", EntryPoint = "CloseClipboard", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeCloseClipboard();

    [DllImport("user32.dll", EntryPoint = "GetClipboardData", SetLastError = true)]
    private static extern IntPtr NativeGetClipboardData(uint uFormat);

    [DllImport("kernel32.dll", EntryPoint = "GlobalLock", SetLastError = true)]
    private static extern IntPtr NativeGlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll", EntryPoint = "GlobalUnlock", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeGlobalUnlock(IntPtr hMem);

    [DllImport("kernel32.dll", EntryPoint = "GlobalSize", SetLastError = true)]
    private static extern UIntPtr NativeGlobalSize(IntPtr hMem);

    [DllImport("user32.dll", EntryPoint = "IsClipboardFormatAvailable", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeIsClipboardFormatAvailable(uint format);

    /// <summary>
    /// Opens the clipboard for examination
    /// </summary>
    public bool OpenClipboard(IntPtr hWndNewOwner)
    {
        return NativeOpenClipboard(hWndNewOwner);
    }

    /// <summary>
    /// Closes the clipboard
    /// </summary>
    public bool CloseClipboard()
    {
        return NativeCloseClipboard();
    }

    /// <summary>
    /// Retrieves data from the clipboard in a specified format
    /// </summary>
    public IntPtr GetClipboardData(ClipboardFormat uFormat)
    {
        return NativeGetClipboardData((uint)uFormat);
    }

    /// <summary>
    /// Locks a global memory object and returns a pointer to the first byte
    /// </summary>
    public IntPtr GlobalLock(IntPtr hMem)
    {
        return NativeGlobalLock(hMem);
    }

    /// <summary>
    /// Unlocks a global memory object
    /// </summary>
    public bool GlobalUnlock(IntPtr hMem)
    {
        return NativeGlobalUnlock(hMem);
    }

    /// <summary>
    /// Returns the size of the global memory object
    /// </summary>
    public UIntPtr GlobalSize(IntPtr hMem)
    {
        return NativeGlobalSize(hMem);
    }

    /// <summary>
    /// Determines whether the clipboard contains data in the specified format
    /// </summary>
    public bool IsClipboardFormatAvailable(ClipboardFormat format)
    {
        return NativeIsClipboardFormatAvailable((uint)format);
    }
}
