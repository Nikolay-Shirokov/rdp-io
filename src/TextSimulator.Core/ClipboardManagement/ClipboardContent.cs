using TextSimulator.Infrastructure.Win32;

namespace TextSimulator.Core.ClipboardManagement;

/// <summary>
/// Model representing clipboard content
/// </summary>
public class ClipboardContent
{
    /// <summary>
    /// Text content from clipboard
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Length of text (for quick access)
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Time when content was read from clipboard
    /// </summary>
    public DateTime ReadTime { get; set; }

    /// <summary>
    /// Clipboard format
    /// </summary>
    public ClipboardFormat Format { get; set; }

    /// <summary>
    /// Validation warnings (if any)
    /// </summary>
    public List<string> ValidationWarnings { get; set; } = new();

    /// <summary>
    /// Whether there are validation warnings
    /// </summary>
    public bool HasWarnings => ValidationWarnings.Any();

    public override string ToString()
    {
        return $"ClipboardContent: {Length} characters, read at {ReadTime:HH:mm:ss}";
    }
}
