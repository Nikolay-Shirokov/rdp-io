namespace RdpIo.Configuration;

/// <summary>
/// Transmission speed modes
/// </summary>
public enum TransmissionMode
{
    Fast,       // 20 ms delay
    Reliable,   // 50 ms delay (default)
    Slow        // 100 ms delay
}

