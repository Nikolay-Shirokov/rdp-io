namespace RdpIo.Core.OcrManagement;

/// <summary>
/// Exception thrown when OCR operation fails
/// </summary>
public class OcrException : Exception
{
    /// <summary>
    /// Creates a new OcrException
    /// </summary>
    public OcrException()
    {
    }

    /// <summary>
    /// Creates a new OcrException with a message
    /// </summary>
    /// <param name="message">Error message</param>
    public OcrException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new OcrException with a message and inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public OcrException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
