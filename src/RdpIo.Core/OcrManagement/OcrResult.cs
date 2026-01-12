using System.Drawing;

namespace RdpIo.Core.OcrManagement;

/// <summary>
/// Result of OCR recognition
/// </summary>
public class OcrResult
{
    /// <summary>
    /// Full recognized text
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Lines of text with positions
    /// </summary>
    public List<OcrLine> Lines { get; init; } = new();

    /// <summary>
    /// Confidence score (0.0 - 1.0)
    /// </summary>
    public double Confidence { get; init; }

    /// <summary>
    /// Time taken to process OCR
    /// </summary>
    public TimeSpan ProcessingTime { get; init; }

    /// <summary>
    /// Number of recognized characters
    /// </summary>
    public int CharacterCount => Text.Length;

    /// <summary>
    /// Number of recognized lines
    /// </summary>
    public int LineCount => Lines.Count;

    /// <summary>
    /// Number of recognized words
    /// </summary>
    public int WordCount => Lines.Sum(l => l.Words.Count);
}

/// <summary>
/// Single line of OCR text
/// </summary>
public class OcrLine
{
    /// <summary>
    /// Text content of the line
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Bounding rectangle of the line
    /// </summary>
    public Rectangle BoundingRect { get; init; }

    /// <summary>
    /// Words in the line
    /// </summary>
    public List<OcrWord> Words { get; init; } = new();
}

/// <summary>
/// Single word of OCR text
/// </summary>
public class OcrWord
{
    /// <summary>
    /// Text content of the word
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Bounding rectangle of the word
    /// </summary>
    public Rectangle BoundingRect { get; init; }
}
