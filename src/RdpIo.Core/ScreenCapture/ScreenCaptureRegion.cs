namespace RdpIo.Core.ScreenCapture;

/// <summary>
/// Represents a rectangular region of the screen to capture
/// </summary>
public record ScreenCaptureRegion
{
    /// <summary>
    /// X coordinate of the top-left corner
    /// </summary>
    public int X { get; init; }

    /// <summary>
    /// Y coordinate of the top-left corner
    /// </summary>
    public int Y { get; init; }

    /// <summary>
    /// Width of the region in pixels
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// Height of the region in pixels
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// Creates a new screen capture region
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    /// <exception cref="ArgumentException">If width or height is not positive</exception>
    public ScreenCaptureRegion(int x, int y, int width, int height)
    {
        if (width <= 0)
            throw new ArgumentException("Width must be positive", nameof(width));
        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));

        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Returns a string representation of the region
    /// </summary>
    public override string ToString() => $"({X}, {Y}) {Width}×{Height}";
}
