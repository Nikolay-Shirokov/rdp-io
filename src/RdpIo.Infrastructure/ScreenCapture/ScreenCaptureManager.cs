using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using RdpIo.Infrastructure.Win32;

namespace RdpIo.Infrastructure.ScreenCapture;

/// <summary>
/// Screen capture manager using Win32 GDI
/// </summary>
public class ScreenCaptureManager : IScreenCaptureManager
{
    private readonly IWin32ApiWrapper _win32Api;

    public ScreenCaptureManager(IWin32ApiWrapper win32Api)
    {
        _win32Api = win32Api ?? throw new ArgumentNullException(nameof(win32Api));
    }

    /// <summary>
    /// Captures a region of the screen
    /// </summary>
    public async Task<Bitmap> CaptureRegionAsync(ScreenCaptureRegion region)
    {
        if (region == null)
            throw new ArgumentNullException(nameof(region));

        if (!IsRegionValid(region))
            throw new ScreenCaptureException($"Region is out of screen bounds: {region}");

        // Capture is CPU-bound, run on thread pool
        return await Task.Run(() => CaptureRegionInternal(region));
    }

    /// <summary>
    /// Captures the entire screen
    /// </summary>
    public async Task<Bitmap> CaptureFullScreenAsync()
    {
        var screenSize = GetScreenSize();
        var region = new ScreenCaptureRegion(0, 0, screenSize.Width, screenSize.Height);
        return await CaptureRegionAsync(region);
    }

    /// <summary>
    /// Gets the screen dimensions
    /// </summary>
    public Size GetScreenSize()
    {
        var primaryScreen = System.Windows.Forms.Screen.PrimaryScreen
            ?? throw new ScreenCaptureException("Primary screen not found");

        return new Size(
            width: primaryScreen.Bounds.Width,
            height: primaryScreen.Bounds.Height
        );
    }

    /// <summary>
    /// Validates if the region is within screen bounds
    /// </summary>
    public bool IsRegionValid(ScreenCaptureRegion region)
    {
        if (region == null)
            return false;

        var screenSize = GetScreenSize();

        return region.X >= 0 &&
               region.Y >= 0 &&
               region.X + region.Width <= screenSize.Width &&
               region.Y + region.Height <= screenSize.Height;
    }

    /// <summary>
    /// Internal method to capture screen region using Win32 GDI
    /// </summary>
    private Bitmap CaptureRegionInternal(ScreenCaptureRegion region)
    {
        IntPtr hdcScreen = IntPtr.Zero;
        IntPtr hdcMemory = IntPtr.Zero;
        IntPtr hBitmap = IntPtr.Zero;
        IntPtr hOldBitmap = IntPtr.Zero;

        try
        {
            // Get the device context of the entire screen
            hdcScreen = _win32Api.GetDC(IntPtr.Zero);
            if (hdcScreen == IntPtr.Zero)
                throw new ScreenCaptureException("Failed to get screen device context");

            // Create a compatible memory DC
            hdcMemory = _win32Api.CreateCompatibleDC(hdcScreen);
            if (hdcMemory == IntPtr.Zero)
                throw new ScreenCaptureException("Failed to create compatible device context");

            // Create a compatible bitmap
            hBitmap = _win32Api.CreateCompatibleBitmap(hdcScreen, region.Width, region.Height);
            if (hBitmap == IntPtr.Zero)
                throw new ScreenCaptureException("Failed to create compatible bitmap");

            // Select the bitmap into the memory DC
            hOldBitmap = _win32Api.SelectObject(hdcMemory, hBitmap);
            if (hOldBitmap == IntPtr.Zero)
                throw new ScreenCaptureException("Failed to select bitmap into device context");

            // Copy the screen region into the memory DC
            bool success = _win32Api.BitBlt(
                hdcDest: hdcMemory,
                nXDest: 0,
                nYDest: 0,
                nWidth: region.Width,
                nHeight: region.Height,
                hdcSrc: hdcScreen,
                nXSrc: region.X,
                nYSrc: region.Y,
                dwRop: RasterOperationCode.SRCCOPY | RasterOperationCode.CAPTUREBLT
            );

            if (!success)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new ScreenCaptureException($"BitBlt failed with error code: {errorCode}");
            }

            // Create a managed Bitmap from the GDI bitmap
            Bitmap bitmap = Image.FromHbitmap(hBitmap);

            return bitmap;
        }
        catch (Exception ex) when (ex is not ScreenCaptureException)
        {
            throw new ScreenCaptureException($"Screen capture failed: {ex.Message}", ex);
        }
        finally
        {
            // Cleanup GDI resources
            if (hOldBitmap != IntPtr.Zero)
                _win32Api.SelectObject(hdcMemory, hOldBitmap);

            if (hBitmap != IntPtr.Zero)
                _win32Api.DeleteObject(hBitmap);

            if (hdcMemory != IntPtr.Zero)
                _win32Api.DeleteDC(hdcMemory);

            if (hdcScreen != IntPtr.Zero)
                _win32Api.ReleaseDC(IntPtr.Zero, hdcScreen);
        }
    }
}
