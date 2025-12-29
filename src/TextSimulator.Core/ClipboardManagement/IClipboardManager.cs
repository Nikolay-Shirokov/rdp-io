namespace TextSimulator.Core.ClipboardManagement;

/// <summary>
/// Interface for clipboard management
/// </summary>
public interface IClipboardManager
{
    /// <summary>
    /// Gets text content from clipboard
    /// </summary>
    /// <returns>Clipboard content or null if empty</returns>
    Task<ClipboardContent?> GetTextAsync();

    /// <summary>
    /// Checks if clipboard contains text
    /// </summary>
    bool HasText();

    /// <summary>
    /// Gets brief clipboard info (for tooltip)
    /// </summary>
    string GetClipboardInfo();

    /// <summary>
    /// Clears cache
    /// </summary>
    void ClearCache();
}
