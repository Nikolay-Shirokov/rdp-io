using System.Drawing;
using System.Drawing.Imaging;
using Tesseract;

namespace RdpIo.Infrastructure.OcrManagement;

/// <summary>
/// OCR engine implementation using Tesseract
/// Provides better accuracy than Windows OCR, especially for colored text and mixed languages
/// </summary>
public class TesseractOcrEngine : IOcrEngine
{
    private readonly string _tessDataPath;
    private TesseractEngine? _engine;
    private string? _currentLanguage;

    /// <summary>
    /// Gets the name of the OCR engine
    /// </summary>
    public string EngineName => "Tesseract OCR";

    public TesseractOcrEngine()
    {
        // tessdata должна быть в папке рядом с исполняемым файлом
        _tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

        if (!Directory.Exists(_tessDataPath))
        {
            throw new OcrException($"Tesseract data directory not found: {_tessDataPath}. " +
                "Please ensure tessdata folder with language files is present.");
        }
    }

    /// <summary>
    /// Recognizes text from bitmap image using Tesseract OCR
    /// </summary>
    public async Task<OcrResult> RecognizeTextAsync(Bitmap image, OcrSettings settings)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        return await Task.Run(() =>
        {
            try
            {
                // Initialize or reinitialize engine if language changed
                InitializeEngine(settings.Language);

                // Convert Bitmap to Pix (Tesseract's image format)
                using var pix = ConvertBitmapToPix(image);

                // Perform OCR
                using var page = _engine!.Process(pix);

                var text = page.GetText();
                var confidence = page.GetMeanConfidence();

                // Create OCR result (LineCount and CharacterCount are computed from Text and Lines)
                return new OcrResult
                {
                    Text = text.Trim(),
                    Confidence = confidence,
                    Lines = new List<OcrLine>()  // Tesseract doesn't provide detailed line info in simple mode
                };
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException?.Message ?? "No inner exception";
                var stackTrace = ex.StackTrace ?? "No stack trace";
                throw new OcrException($"Tesseract OCR failed: {ex.Message}. Inner: {innerMsg}. Stack: {stackTrace}", ex);
            }
        });
    }

    /// <summary>
    /// Checks if a language is available for OCR
    /// </summary>
    public Task<bool> IsLanguageAvailableAsync(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return Task.FromResult(false);

        // Normalize language code (rus, ru, ru-RU -> rus for Tesseract)
        var tessLang = NormalizeTesseractLanguage(languageCode);
        var langFile = Path.Combine(_tessDataPath, $"{tessLang}.traineddata");

        return Task.FromResult(File.Exists(langFile));
    }

    /// <summary>
    /// Gets list of available OCR languages
    /// </summary>
    public Task<IReadOnlyList<string>> GetAvailableLanguagesAsync()
    {
        try
        {
            if (!Directory.Exists(_tessDataPath))
                return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

            var languages = Directory.GetFiles(_tessDataPath, "*.traineddata")
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .Where(lang => !string.IsNullOrEmpty(lang))
                .ToList();

            return Task.FromResult<IReadOnlyList<string>>(languages);
        }
        catch
        {
            return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        }
    }

    /// <summary>
    /// Initializes or reinitializes Tesseract engine with specified language
    /// </summary>
    private void InitializeEngine(string languageCode)
    {
        var tessLang = NormalizeTesseractLanguage(languageCode);

        // Check if we need to reinitialize
        if (_engine != null && _currentLanguage == tessLang)
            return;

        // Dispose old engine
        _engine?.Dispose();

        // Create new engine
        _engine = new TesseractEngine(_tessDataPath, tessLang, EngineMode.Default);

        // Configure for better accuracy
        _engine.SetVariable("tessedit_char_whitelist", "");  // Allow all characters
        _engine.SetVariable("load_system_dawg", "1");
        _engine.SetVariable("load_freq_dawg", "1");

        _currentLanguage = tessLang;
    }

    /// <summary>
    /// Converts System.Drawing.Bitmap to Tesseract.Pix
    /// </summary>
    private Pix ConvertBitmapToPix(Bitmap bitmap)
    {
        // Convert to 24bpp RGB format for Tesseract
        using var ms = new MemoryStream();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Tiff);
        ms.Position = 0;

        return Pix.LoadFromMemory(ms.ToArray());
    }

    /// <summary>
    /// Normalizes language code for Tesseract
    /// ru, ru-RU -> rus
    /// en, en-US -> eng
    /// </summary>
    private string NormalizeTesseractLanguage(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return "eng";

        var lower = languageCode.ToLowerInvariant();

        // Map common language codes to Tesseract's 3-letter codes
        if (lower.StartsWith("ru")) return "rus";
        if (lower.StartsWith("en")) return "eng";
        if (lower.StartsWith("de")) return "deu";
        if (lower.StartsWith("fr")) return "fra";
        if (lower.StartsWith("es")) return "spa";
        if (lower.StartsWith("zh")) return "chi_sim";
        if (lower.StartsWith("ja")) return "jpn";
        if (lower.StartsWith("ko")) return "kor";

        // If already 3-letter code, use as-is
        if (lower.Length == 3)
            return lower;

        // Default to English
        return "eng";
    }

    public void Dispose()
    {
        _engine?.Dispose();
        _engine = null;
    }
}
