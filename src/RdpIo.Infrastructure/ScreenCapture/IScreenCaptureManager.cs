using System.Drawing;

namespace RdpIo.Infrastructure.ScreenCapture;

/// <summary>
/// Interface for screen capture operations
/// </summary>
public interface IScreenCaptureManager
{
    /// <summary>
    /// Captures a region of the screen
    /// </summary>
    /// <param name="region">Region to capture</param>
    /// <returns>Captured bitmap</returns>
    /// <exception cref="ScreenCaptureException">Thrown when capture fails</exception>
    Task<Bitmap> CaptureRegionAsync(ScreenCaptureRegion region);

    /// <summary>
    /// Captures the entire screen
    /// </summary>
    /// <returns>Captured bitmap</returns>
    /// <exception cref="ScreenCaptureException">Thrown when capture fails</exception>
    Task<Bitmap> CaptureFullScreenAsync();

    /// <summary>
    /// Gets the screen dimensions
    /// </summary>
    /// <returns>Screen size</returns>
    Size GetScreenSize();

    /// <summary>
    /// Validates if the region is within screen bounds
    /// </summary>
    /// <param name="region">Region to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsRegionValid(ScreenCaptureRegion region);
}
