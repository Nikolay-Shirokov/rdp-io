namespace TextSimulator.Infrastructure.Win32;

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
}
