namespace RdpIo.Infrastructure.OcrManagement;

/// <summary>
/// OCR configuration settings
/// </summary>
public class OcrSettings
{
    /// <summary>
    /// Language code for OCR (e.g., "en", "ru", "en-US")
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Enable image preprocessing before OCR
    /// </summary>
    public bool EnablePreprocessing { get; set; } = true;

    /// <summary>
    /// OCR engine type to use
    /// </summary>
    public OcrEngineType EngineType { get; set; } = OcrEngineType.Windows;

    /// <summary>
    /// Timeout for OCR operation (seconds)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}

/// <summary>
/// Available OCR engine types
/// </summary>
public enum OcrEngineType
{
    /// <summary>
    /// Windows built-in OCR (Windows.Media.Ocr)
    /// </summary>
    Windows = 0,

    /// <summary>
    /// Tesseract OCR - Better accuracy, especially for colored text and mixed languages
    /// Requires tessdata folder with language files
    /// </summary>
    Tesseract = 1
}
