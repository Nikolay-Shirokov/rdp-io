namespace RdpIo.Core.StateManagement;

/// <summary>
/// Application states
/// </summary>
public enum ApplicationState
{
    /// <summary>
    /// Application in idle mode
    /// </summary>
    Idle,

    /// <summary>
    /// Validating clipboard content
    /// </summary>
    ValidatingClipboard,

    /// <summary>
    /// Countdown before transmission starts
    /// </summary>
    Countdown,

    /// <summary>
    /// Active text transmission
    /// </summary>
    Transmitting,

    /// <summary>
    /// Transmission paused (focus lost)
    /// </summary>
    Paused,

    /// <summary>
    /// Transmission completed successfully
    /// </summary>
    Completed,

    /// <summary>
    /// Transmission failed with error
    /// </summary>
    Failed,

    /// <summary>
    /// Transmission cancelled by user
    /// </summary>
    Cancelled,

    /// <summary>
    /// Settings window is open
    /// </summary>
    Settings,

    /// <summary>
    /// User selecting screen region for OCR capture
    /// </summary>
    SelectingRegion,

    /// <summary>
    /// Capturing screenshot of selected region
    /// </summary>
    CapturingScreen,

    /// <summary>
    /// OCR recognition in progress
    /// </summary>
    ProcessingOcr,

    /// <summary>
    /// Displaying OCR results to user
    /// </summary>
    ShowingOcrResult
}

