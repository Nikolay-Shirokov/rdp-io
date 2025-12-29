namespace TextSimulator.Infrastructure.Logging;

/// <summary>
/// Null implementation of ILogger for testing
/// </summary>
public class NullLogger : ILogger
{
    public void LogInfo(string message) { }
    public void LogWarning(string message) { }
    public void LogError(string message) { }
    public void LogDebug(string message) { }
}
