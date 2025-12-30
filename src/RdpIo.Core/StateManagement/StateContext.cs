namespace RdpIo.Core.StateManagement;

/// <summary>
/// Execution context for current state
/// </summary>
public class StateContext
{
    /// <summary>
    /// Text to transmit
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Current position in text (during transmission)
    /// </summary>
    public int CurrentPosition { get; set; }

    /// <summary>
    /// Total characters count
    /// </summary>
    public int TotalCharacters { get; set; }

    /// <summary>
    /// Operation start time
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// CancellationTokenSource for cancellation
    /// </summary>
    public CancellationTokenSource? CancellationTokenSource { get; set; }

    /// <summary>
    /// Additional data
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    public void Reset()
    {
        Text = null;
        CurrentPosition = 0;
        TotalCharacters = 0;
        StartTime = DateTime.MinValue;
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = null;
        AdditionalData.Clear();
    }
}

