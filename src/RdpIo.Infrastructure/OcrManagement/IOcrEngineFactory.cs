namespace RdpIo.Infrastructure.OcrManagement;

/// <summary>
/// Factory for creating OCR engines based on current settings
/// </summary>
public interface IOcrEngineFactory
{
    /// <summary>
    /// Creates an OCR engine based on current settings
    /// </summary>
    IOcrEngine CreateEngine();
}
