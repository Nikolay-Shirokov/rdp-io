using RdpIo.Infrastructure.Logging;

namespace RdpIo.Configuration;

/// <summary>
/// Application settings model
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Transmission speed mode
    /// </summary>
    public TransmissionMode TransmissionMode { get; set; } = TransmissionMode.Reliable;

    /// <summary>
    /// Countdown delay in seconds
    /// </summary>
    public int CountdownSeconds { get; set; } = 5;

    /// <summary>
    /// Enable sound notifications
    /// </summary>
    public bool EnableSounds { get; set; } = false;

    /// <summary>
    /// Sound on transmission start
    /// </summary>
    public bool SoundOnStart { get; set; } = true;

    /// <summary>
    /// Sound on completion
    /// </summary>
    public bool SoundOnComplete { get; set; } = true;

    /// <summary>
    /// Sound on error
    /// </summary>
    public bool SoundOnError { get; set; } = true;

    /// <summary>
    /// Clipboard cache lifetime in seconds
    /// </summary>
    public int ClipboardCacheLifetimeSeconds { get; set; } = 5;

    /// <summary>
    /// Minimum log level
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.None;

    /// <summary>
    /// Maximum log file size in MB
    /// </summary>
    public int MaxLogFileSizeMB { get; set; } = 10;

    // ===== OCR Settings =====

    /// <summary>
    /// OCR engine to use: "Windows" (built-in) or "Tesseract" (better quality, requires tessdata)
    /// </summary>
    public string OcrEngine { get; set; } = "Tesseract";

    /// <summary>
    /// OCR language code (e.g., "en", "ru", "en-US") or "auto" for automatic detection
    /// </summary>
    public string OcrLanguage { get; set; } = "ru";

    /// <summary>
    /// Enable image preprocessing before OCR
    /// </summary>
    public bool OcrEnablePreprocessing { get; set; } = false;
}

