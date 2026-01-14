using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace RdpIo.Infrastructure.OcrManagement;

/// <summary>
/// OCR engine using Windows.Media.Ocr (WinRT)
/// </summary>
public class WindowsOcrEngine : IOcrEngine
{
    /// <summary>
    /// Gets the name of the OCR engine
    /// </summary>
    public string EngineName => "Windows OCR (Windows.Media.Ocr)";

    /// <summary>
    /// Recognizes text from a bitmap image
    /// </summary>
    public async Task<OcrResult> RecognizeTextAsync(Bitmap image, OcrSettings settings)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Create OCR engine for the specified language
            var language = new Language(settings.Language);
            var ocrEngine = OcrEngine.TryCreateFromLanguage(language);

            if (ocrEngine == null)
                throw new OcrException($"Failed to create OCR engine for language '{settings.Language}'. Language may not be installed.");

            // Convert System.Drawing.Bitmap to Windows.Graphics.Imaging.SoftwareBitmap
            using var softwareBitmap = await ConvertToSoftwareBitmapAsync(image);

            // Perform OCR recognition
            var ocrResult = await ocrEngine.RecognizeAsync(softwareBitmap);

            // Convert Windows OCR result to our OcrResult model
            var result = ConvertOcrResult(ocrResult, stopwatch.Elapsed);

            return result;
        }
        catch (Exception ex) when (ex is not OcrException)
        {
            throw new OcrException($"OCR recognition failed: {ex.Message}", ex);
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    /// <summary>
    /// Gets available OCR languages on the system
    /// </summary>
    public Task<IReadOnlyList<string>> GetAvailableLanguagesAsync()
    {
        var availableLanguages = OcrEngine.AvailableRecognizerLanguages
            .Select(lang => lang.LanguageTag)
            .ToList();

        return Task.FromResult<IReadOnlyList<string>>(availableLanguages);
    }

    /// <summary>
    /// Checks if a specific language is available
    /// </summary>
    public Task<bool> IsLanguageAvailableAsync(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return Task.FromResult(false);

        var isAvailable = OcrEngine.AvailableRecognizerLanguages
            .Any(lang => lang.LanguageTag.Equals(languageCode, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(isAvailable);
    }

    /// <summary>
    /// Converts System.Drawing.Bitmap to Windows.Graphics.Imaging.SoftwareBitmap
    /// </summary>
    private async Task<SoftwareBitmap> ConvertToSoftwareBitmapAsync(Bitmap bitmap)
    {
        // Save bitmap to memory stream as PNG
        using var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Png);
        memoryStream.Position = 0;

        // Create BitmapDecoder from stream
        var randomAccessStream = memoryStream.AsRandomAccessStream();
        var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);

        // Get SoftwareBitmap
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        // Convert to BGRA8 format if needed (required for OCR)
        if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
            softwareBitmap.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
        {
            softwareBitmap = SoftwareBitmap.Convert(
                softwareBitmap,
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied
            );
        }

        return softwareBitmap;
    }

    /// <summary>
    /// Converts Windows.Media.Ocr.OcrResult to our OcrResult model
    /// </summary>
    private Infrastructure.OcrManagement.OcrResult ConvertOcrResult(
        Windows.Media.Ocr.OcrResult windowsOcrResult,
        TimeSpan processingTime)
    {
        var lines = new List<OcrLine>();
        var allText = windowsOcrResult.Text;

        foreach (var windowsLine in windowsOcrResult.Lines)
        {
            var words = new List<OcrWord>();

            foreach (var windowsWord in windowsLine.Words)
            {
                var wordRect = new Rectangle(
                    (int)windowsWord.BoundingRect.X,
                    (int)windowsWord.BoundingRect.Y,
                    (int)windowsWord.BoundingRect.Width,
                    (int)windowsWord.BoundingRect.Height
                );

                words.Add(new OcrWord
                {
                    Text = windowsWord.Text,
                    BoundingRect = wordRect
                });
            }

            var lineRect = new Rectangle(
                words.Any() ? words.Min(w => w.BoundingRect.X) : 0,
                words.Any() ? words.Min(w => w.BoundingRect.Y) : 0,
                words.Any() ? words.Max(w => w.BoundingRect.Right) - words.Min(w => w.BoundingRect.X) : 0,
                words.Any() ? words.Max(w => w.BoundingRect.Bottom) - words.Min(w => w.BoundingRect.Y) : 0
            );

            lines.Add(new OcrLine
            {
                Text = windowsLine.Text,
                BoundingRect = lineRect,
                Words = words
            });
        }

        // Calculate average confidence (Windows OCR doesn't provide this, so we estimate)
        // If text was recognized, assume reasonable confidence
        double confidence = string.IsNullOrWhiteSpace(allText) ? 0.0 : 0.85;

        return new Infrastructure.OcrManagement.OcrResult
        {
            Text = allText ?? string.Empty,
            Lines = lines,
            Confidence = confidence,
            ProcessingTime = processingTime
        };
    }
}
