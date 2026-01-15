namespace RdpIo.Infrastructure.Win32;

/// <summary>
/// Interface for Win32 API wrapper
/// </summary>
public interface IWin32ApiWrapper
{
    // ===== Keyboard Input =====

    /// <summary>
    /// Synthesizes keystrokes, mouse motions, and button clicks
    /// </summary>
    uint SendInput(uint nInputs, INPUT[] pInputs);

    /// <summary>
    /// Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code.
    /// </summary>
    uint MapVirtualKey(uint uCode, uint uMapType);

    /// <summary>
    /// Retrieves the status of the specified virtual key (pressed state and toggle state)
    /// </summary>
    short GetKeyState(int nVirtKey);

    // ===== Clipboard =====

    /// <summary>
    /// Opens the clipboard for examination
    /// </summary>
    bool OpenClipboard(IntPtr hWndNewOwner);

    /// <summary>
    /// Closes the clipboard
    /// </summary>
    bool CloseClipboard();

    /// <summary>
    /// Retrieves data from the clipboard in a specified format
    /// </summary>
    IntPtr GetClipboardData(ClipboardFormat uFormat);

    /// <summary>
    /// Locks a global memory object and returns a pointer to the first byte
    /// </summary>
    IntPtr GlobalLock(IntPtr hMem);

    /// <summary>
    /// Unlocks a global memory object
    /// </summary>
    bool GlobalUnlock(IntPtr hMem);

    /// <summary>
    /// Returns the size of the global memory object
    /// </summary>
    UIntPtr GlobalSize(IntPtr hMem);

    /// <summary>
    /// Determines whether the clipboard contains data in the specified format
    /// </summary>
    bool IsClipboardFormatAvailable(ClipboardFormat format);

    // ===== GDI Screen Capture =====

    /// <summary>
    /// Retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen
    /// </summary>
    IntPtr GetDC(IntPtr hWnd);

    /// <summary>
    /// Releases a device context (DC), freeing it for use by other applications
    /// </summary>
    int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    /// <summary>
    /// Creates a memory device context (DC) compatible with the specified device
    /// </summary>
    IntPtr CreateCompatibleDC(IntPtr hDC);

    /// <summary>
    /// Creates a bitmap compatible with the device that is associated with the specified device context
    /// </summary>
    IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

    /// <summary>
    /// Performs a bit-block transfer of color data from source to destination DC
    /// </summary>
    bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

    /// <summary>
    /// Selects an object into the specified device context (DC)
    /// </summary>
    IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    /// <summary>
    /// Deletes the specified device context (DC)
    /// </summary>
    bool DeleteDC(IntPtr hDC);

    /// <summary>
    /// Deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources
    /// </summary>
    bool DeleteObject(IntPtr hObject);
}

