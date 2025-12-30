namespace RdpIo.Core.ClipboardManagement;

/// <summary>
/// Result of clipboard content validation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Whether the content is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Error message if validation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Warning messages (non-critical issues)
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}

