using System.Drawing;

namespace RdpIo.Infrastructure.ImageProcessing;

/// <summary>
/// Interface for image preprocessing operations to improve OCR accuracy
/// </summary>
public interface IImageProcessor
{
    /// <summary>
    /// Converts image to grayscale for better OCR recognition
    /// </summary>
    /// <param name="source">Source bitmap</param>
    /// <returns>Grayscale bitmap</returns>
    Bitmap ConvertToGrayscale(Bitmap source);

    /// <summary>
    /// Enhances image contrast to improve text recognition
    /// </summary>
    /// <param name="source">Source bitmap</param>
    /// <param name="contrastLevel">Contrast enhancement level (1.0 = no change, higher = more contrast)</param>
    /// <returns>Contrast-enhanced bitmap</returns>
    Bitmap EnhanceContrast(Bitmap source, double contrastLevel = 1.5);

    /// <summary>
    /// Reduces image noise using simple filtering
    /// </summary>
    /// <param name="source">Source bitmap</param>
    /// <returns>Noise-reduced bitmap</returns>
    Bitmap ReduceNoise(Bitmap source);

    /// <summary>
    /// Performs full preprocessing pipeline for OCR
    /// </summary>
    /// <param name="source">Source bitmap</param>
    /// <param name="enableNoisereduction">Enable noise reduction step</param>
    /// <returns>Preprocessed bitmap ready for OCR</returns>
    Bitmap PreprocessForOcr(Bitmap source, bool enableNoiseReduction = true);

    /// <summary>
    /// Creates a copy of the bitmap to avoid modifying the original
    /// </summary>
    /// <param name="source">Source bitmap</param>
    /// <returns>Copy of the bitmap</returns>
    Bitmap Clone(Bitmap source);
}
