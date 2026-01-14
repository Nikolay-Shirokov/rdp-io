namespace RdpIo.Infrastructure.Win32;

/// <summary>
/// Raster operation codes for BitBlt function
/// </summary>
public static class RasterOperationCode
{
    /// <summary>
    /// Copies the source rectangle directly to the destination rectangle (most common)
    /// </summary>
    public const uint SRCCOPY = 0x00CC0020;

    /// <summary>
    /// Inverts the source rectangle
    /// </summary>
    public const uint NOTSRCCOPY = 0x00330008;

    /// <summary>
    /// Combines the colors of the source and destination rectangles by using the Boolean OR operator
    /// </summary>
    public const uint SRCPAINT = 0x00EE0086;

    /// <summary>
    /// Combines the colors of the source and destination rectangles by using the Boolean AND operator
    /// </summary>
    public const uint SRCAND = 0x008800C6;

    /// <summary>
    /// Combines the inverted colors of the destination rectangle with the colors of the source rectangle by using the Boolean AND operator
    /// </summary>
    public const uint SRCINVERT = 0x00660046;

    /// <summary>
    /// Combines the colors of the source and destination rectangles by using the Boolean XOR operator
    /// </summary>
    public const uint SRCERASE = 0x00440328;

    /// <summary>
    /// Inverts the destination rectangle
    /// </summary>
    public const uint DSTINVERT = 0x00550009;

    /// <summary>
    /// Fills the destination rectangle using the color associated with index 0 in the physical palette (black for default physical palette)
    /// </summary>
    public const uint BLACKNESS = 0x00000042;

    /// <summary>
    /// Fills the destination rectangle using the color associated with index 1 in the physical palette (white for default physical palette)
    /// </summary>
    public const uint WHITENESS = 0x00FF0062;

    /// <summary>
    /// Captures the window (not just the client area) by using the PrintWindow API
    /// </summary>
    public const uint CAPTUREBLT = 0x40000000;
}
