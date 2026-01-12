namespace RdpIo.Core.ScreenCapture;

/// <summary>
/// Exception thrown when screen capture operation fails
/// </summary>
public class ScreenCaptureException : Exception
{
    /// <summary>
    /// Creates a new ScreenCaptureException
    /// </summary>
    public ScreenCaptureException()
    {
    }

    /// <summary>
    /// Creates a new ScreenCaptureException with a message
    /// </summary>
    /// <param name="message">Error message</param>
    public ScreenCaptureException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new ScreenCaptureException with a message and inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public ScreenCaptureException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
