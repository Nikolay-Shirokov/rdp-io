using RdpIo.Configuration;
using RdpIo.Infrastructure.Logging;
using RdpIo.Infrastructure.OcrManagement;

namespace RdpIo.App;

/// <summary>
/// Factory for creating OCR engines based on current settings
/// </summary>
public class OcrEngineFactory : IOcrEngineFactory
{
    private readonly SettingsManager _settingsManager;
    private readonly ILogger _logger;

    public OcrEngineFactory(SettingsManager settingsManager, ILogger logger)
    {
        _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates an OCR engine based on current settings
    /// </summary>
    public IOcrEngine CreateEngine()
    {
        var settings = _settingsManager.CurrentSettings;

        if (settings.OcrEngine.Equals("Tesseract", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                _logger.LogDebug("Creating Tesseract OCR engine");
                return new TesseractOcrEngine();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to create Tesseract OCR: {ex.Message}. Using Windows OCR.");
                return new WindowsOcrEngine();
            }
        }

        _logger.LogDebug("Creating Windows OCR engine");
        return new WindowsOcrEngine();
    }
}
