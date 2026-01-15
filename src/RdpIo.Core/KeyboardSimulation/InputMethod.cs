namespace RdpIo.Core.KeyboardSimulation;

/// <summary>
/// Text input method for transmission
/// </summary>
public enum TextInputMethod
{
    /// <summary>
    /// All characters via Unicode input (KEYEVENTF_UNICODE)
    /// Works in most RDP but may fail in Citrix/some terminals
    /// </summary>
    Unicode = 0,

    /// <summary>
    /// Hybrid mode: Cyrillic via keyboard simulation, rest via Unicode
    /// Requires Russian keyboard layout to be active in target system
    /// Better compatibility with Citrix and some terminals
    /// </summary>
    Hybrid = 1
}
