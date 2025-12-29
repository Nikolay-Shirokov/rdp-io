namespace TextSimulator.Infrastructure.Logging;

/// <summary>
/// Logger interface
/// </summary>
public interface ILogger
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogDebug(string message);
}
