using System.Runtime.InteropServices;

namespace RdpIo.Infrastructure.Win32;

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

    // ===== GDI Screen Capture P/Invoke =====

    [DllImport("user32.dll", EntryPoint = "GetDC", SetLastError = true)]
    private static extern IntPtr NativeGetDC(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "ReleaseDC", SetLastError = true)]
    private static extern int NativeReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
    private static extern IntPtr NativeCreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap", SetLastError = true)]
    private static extern IntPtr NativeCreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

    [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeBitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
        IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

    [DllImport("gdi32.dll", EntryPoint = "SelectObject", SetLastError = true)]
    private static extern IntPtr NativeSelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll", EntryPoint = "DeleteDC", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeDeleteDC(IntPtr hDC);

    [DllImport("gdi32.dll", EntryPoint = "DeleteObject", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeDeleteObject(IntPtr hObject);

    /// <summary>
    /// Retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen
    /// </summary>
    public IntPtr GetDC(IntPtr hWnd)
    {
        return NativeGetDC(hWnd);
    }

    /// <summary>
    /// Releases a device context (DC), freeing it for use by other applications
    /// </summary>
    public int ReleaseDC(IntPtr hWnd, IntPtr hDC)
    {
        return NativeReleaseDC(hWnd, hDC);
    }

    /// <summary>
    /// Creates a memory device context (DC) compatible with the specified device
    /// </summary>
    public IntPtr CreateCompatibleDC(IntPtr hDC)
    {
        return NativeCreateCompatibleDC(hDC);
    }

    /// <summary>
    /// Creates a bitmap compatible with the device that is associated with the specified device context
    /// </summary>
    public IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight)
    {
        return NativeCreateCompatibleBitmap(hDC, nWidth, nHeight);
    }

    /// <summary>
    /// Performs a bit-block transfer of color data from source to destination DC
    /// </summary>
    public bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop)
    {
        return NativeBitBlt(hdcDest, nXDest, nYDest, nWidth, nHeight, hdcSrc, nXSrc, nYSrc, dwRop);
    }

    /// <summary>
    /// Selects an object into the specified device context (DC)
    /// </summary>
    public IntPtr SelectObject(IntPtr hDC, IntPtr hObject)
    {
        return NativeSelectObject(hDC, hObject);
    }

    /// <summary>
    /// Deletes the specified device context (DC)
    /// </summary>
    public bool DeleteDC(IntPtr hDC)
    {
        return NativeDeleteDC(hDC);
    }

    /// <summary>
    /// Deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources
    /// </summary>
    public bool DeleteObject(IntPtr hObject)
    {
        return NativeDeleteObject(hObject);
    }
}

