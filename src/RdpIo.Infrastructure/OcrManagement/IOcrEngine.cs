using System.Drawing;

namespace RdpIo.Infrastructure.OcrManagement;

/// <summary>
/// Interface for OCR (Optical Character Recognition) engines
/// </summary>
public interface IOcrEngine
{
    /// <summary>
    /// Recognizes text from a bitmap image
    /// </summary>
    /// <param name="image">Image to recognize</param>
    /// <param name="settings">OCR settings</param>
    /// <returns>OCR recognition result</returns>
    /// <exception cref="OcrException">Thrown when OCR operation fails</exception>
    Task<OcrResult> RecognizeTextAsync(Bitmap image, OcrSettings settings);

    /// <summary>
    /// Gets available OCR languages on the system
    /// </summary>
    /// <returns>List of available language codes</returns>
    Task<IReadOnlyList<string>> GetAvailableLanguagesAsync();

    /// <summary>
    /// Checks if a specific language is available
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "ru")</param>
    /// <returns>True if language is available, false otherwise</returns>
    Task<bool> IsLanguageAvailableAsync(string languageCode);

    /// <summary>
    /// Gets the name of the OCR engine
    /// </summary>
    string EngineName { get; }
}
