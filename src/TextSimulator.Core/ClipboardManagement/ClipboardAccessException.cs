namespace TextSimulator.Core.ClipboardManagement;

/// <summary>
/// Exception thrown when clipboard access fails
/// </summary>
public class ClipboardAccessException : Exception
{
    public ClipboardAccessException(string message)
        : base(message)
    {
    }

    public ClipboardAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
