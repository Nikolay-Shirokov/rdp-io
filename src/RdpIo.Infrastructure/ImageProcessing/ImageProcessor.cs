using System.Drawing;
using System.Drawing.Imaging;

namespace RdpIo.Infrastructure.ImageProcessing;

/// <summary>
/// Image processor for preparing images for OCR
/// </summary>
public class ImageProcessor : IImageProcessor
{
    /// <summary>
    /// Converts image to grayscale for better OCR recognition
    /// </summary>
    public Bitmap ConvertToGrayscale(Bitmap source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var grayscale = new Bitmap(source.Width, source.Height);

        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                var pixel = source.GetPixel(x, y);

                // Standard luminance formula: 0.299*R + 0.587*G + 0.114*B
                int gray = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);

                var grayColor = Color.FromArgb(gray, gray, gray);
                grayscale.SetPixel(x, y, grayColor);
            }
        }

        return grayscale;
    }

    /// <summary>
    /// Enhances image contrast to improve text recognition
    /// </summary>
    public Bitmap EnhanceContrast(Bitmap source, double contrastLevel = 1.5)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (contrastLevel < 0)
            throw new ArgumentException("Contrast level must be non-negative", nameof(contrastLevel));

        var enhanced = new Bitmap(source.Width, source.Height);
        double factor = (259.0 * (contrastLevel * 100 + 255)) / (255.0 * (259 - contrastLevel * 100));

        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                var pixel = source.GetPixel(x, y);

                int r = Clamp((int)(factor * (pixel.R - 128) + 128), 0, 255);
                int g = Clamp((int)(factor * (pixel.G - 128) + 128), 0, 255);
                int b = Clamp((int)(factor * (pixel.B - 128) + 128), 0, 255);

                enhanced.SetPixel(x, y, Color.FromArgb(r, g, b));
            }
        }

        return enhanced;
    }

    /// <summary>
    /// Reduces image noise using simple 3x3 median filter
    /// </summary>
    public Bitmap ReduceNoise(Bitmap source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var denoised = new Bitmap(source.Width, source.Height);

        for (int y = 1; y < source.Height - 1; y++)
        {
            for (int x = 1; x < source.Width - 1; x++)
            {
                // Collect 3x3 neighborhood
                var neighbors = new List<int>();
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        var pixel = source.GetPixel(x + dx, y + dy);
                        neighbors.Add((pixel.R + pixel.G + pixel.B) / 3);
                    }
                }

                // Get median value
                neighbors.Sort();
                int median = neighbors[neighbors.Count / 2];

                denoised.SetPixel(x, y, Color.FromArgb(median, median, median));
            }
        }

        // Copy borders without filtering
        for (int x = 0; x < source.Width; x++)
        {
            denoised.SetPixel(x, 0, source.GetPixel(x, 0));
            denoised.SetPixel(x, source.Height - 1, source.GetPixel(x, source.Height - 1));
        }
        for (int y = 0; y < source.Height; y++)
        {
            denoised.SetPixel(0, y, source.GetPixel(0, y));
            denoised.SetPixel(source.Width - 1, y, source.GetPixel(source.Width - 1, y));
        }

        return denoised;
    }

    /// <summary>
    /// Performs full preprocessing pipeline for OCR
    /// </summary>
    public Bitmap PreprocessForOcr(Bitmap source, bool enableNoiseReduction = true)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        // Step 1: Convert to grayscale
        var grayscale = ConvertToGrayscale(source);

        // Step 2: Enhance contrast
        var contrasted = EnhanceContrast(grayscale, 1.3);
        grayscale.Dispose();

        // Step 3: Reduce noise (optional)
        if (enableNoiseReduction)
        {
            var denoised = ReduceNoise(contrasted);
            contrasted.Dispose();
            return denoised;
        }

        return contrasted;
    }

    /// <summary>
    /// Light preprocessing optimized for upscaled images
    /// Only grayscale conversion and subtle contrast boost - no noise reduction
    /// </summary>
    public Bitmap LightPreprocessForOcr(Bitmap source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        // Use faster Graphics-based grayscale conversion
        var grayscale = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        using (var graphics = System.Drawing.Graphics.FromImage(grayscale))
        {
            // Grayscale color matrix
            var colorMatrix = new System.Drawing.Imaging.ColorMatrix(new float[][]
            {
                new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

            using (var attributes = new System.Drawing.Imaging.ImageAttributes())
            {
                attributes.SetColorMatrix(colorMatrix);
                graphics.DrawImage(source,
                    new Rectangle(0, 0, source.Width, source.Height),
                    0, 0, source.Width, source.Height,
                    System.Drawing.GraphicsUnit.Pixel,
                    attributes);
            }
        }

        return grayscale;
    }

    /// <summary>
    /// Creates a copy of the bitmap to avoid modifying the original
    /// </summary>
    public Bitmap Clone(Bitmap source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new Bitmap(source);
    }

    /// <summary>
    /// Upscales image to improve OCR accuracy for small text
    /// Uses high-quality bicubic interpolation
    /// </summary>
    public Bitmap Upscale(Bitmap source, double scaleFactor = 2.0)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (scaleFactor <= 0 || scaleFactor > 10)
            throw new ArgumentException("Scale factor must be between 0 and 10", nameof(scaleFactor));

        int newWidth = (int)(source.Width * scaleFactor);
        int newHeight = (int)(source.Height * scaleFactor);

        var upscaled = new Bitmap(newWidth, newHeight);

        using (var graphics = System.Drawing.Graphics.FromImage(upscaled))
        {
            // High quality settings for upscaling
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            graphics.DrawImage(source, 0, 0, newWidth, newHeight);
        }

        return upscaled;
    }

    /// <summary>
    /// Clamps a value between min and max
    /// </summary>
    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
