namespace TextSimulator.Infrastructure.Logging;

/// <summary>
/// File-based logger with rotation
/// </summary>
public class FileLogger : ILogger, IDisposable
{
    private readonly string _logFilePath;
    private readonly LogLevel _minLogLevel;
    private readonly long _maxFileSizeBytes;
    private readonly object _lock = new object();
    private StreamWriter? _writer;

    public FileLogger(string? logDirectory = null, LogLevel minLogLevel = LogLevel.Info, int maxFileSizeMB = 10)
    {
        _minLogLevel = minLogLevel;
        _maxFileSizeBytes = maxFileSizeMB * 1024 * 1024;

        string directory = logDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
        _logFilePath = Path.Combine(directory, "app.log");

        InitializeLogFile();
    }

    private void InitializeLogFile()
    {
        try
        {
            // Проверяем ротацию
            CheckRotation();

            _writer = new StreamWriter(_logFilePath, append: true)
            {
                AutoFlush = true
            };

            LogInfo("Logger initialized");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize logger: {ex.Message}");
        }
    }

    private void CheckRotation()
    {
        if (File.Exists(_logFilePath))
        {
            var fileInfo = new FileInfo(_logFilePath);

            if (fileInfo.Length > _maxFileSizeBytes)
            {
                // Ротация: переименовываем старый файл
                string backupPath = $"{_logFilePath}.{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                File.Move(_logFilePath, backupPath);

                // Удаляем старые бэкапы (храним только 3 последних)
                CleanupOldBackups();
            }
        }
    }

    private void CleanupOldBackups()
    {
        try
        {
            var directory = Path.GetDirectoryName(_logFilePath);
            if (directory == null) return;

            var backupFiles = Directory.GetFiles(directory, "app.log.*.bak")
                .OrderByDescending(f => f)
                .Skip(3);

            foreach (var file in backupFiles)
            {
                File.Delete(file);
            }
        }
        catch
        {
            // Игнорируем ошибки при очистке
        }
    }

    public void LogInfo(string message)
    {
        Log(LogLevel.Info, message);
    }

    public void LogWarning(string message)
    {
        Log(LogLevel.Warning, message);
    }

    public void LogError(string message)
    {
        Log(LogLevel.Error, message);
    }

    public void LogDebug(string message)
    {
        Log(LogLevel.Debug, message);
    }

    private void Log(LogLevel level, string message)
    {
        if (level < _minLogLevel)
            return;

        lock (_lock)
        {
            try
            {
                string logEntry = FormatLogEntry(level, message);
                _writer?.WriteLine(logEntry);

                // Также выводим в Console для отладки
#if DEBUG
                Console.WriteLine(logEntry);
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }
    }

    private string FormatLogEntry(LogLevel level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string levelStr = level.ToString().ToUpper().PadRight(7);
        return $"[{timestamp}] [{levelStr}] {message}";
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _writer?.Flush();
            _writer?.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
