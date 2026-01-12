using System.Text.Json;
using RdpIo.Core.KeyboardSimulation;
using RdpIo.Infrastructure.Logging;

namespace RdpIo.Configuration;

/// <summary>
/// Manages application settings with JSON persistence
/// </summary>
public class SettingsManager
{
    private readonly string _settingsFilePath;
    private readonly ILogger _logger;
    private AppSettings _currentSettings;

    private static readonly string DefaultSettingsFileName = "settings.json";

    public AppSettings CurrentSettings => _currentSettings;

    public event EventHandler<SettingsChangedEventArgs>? SettingsChanged;

    public SettingsManager(ILogger logger, string? settingsDirectory = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Путь к файлу настроек (рядом с .exe)
        string directory = settingsDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
        _settingsFilePath = Path.Combine(directory, DefaultSettingsFileName);

        _currentSettings = new AppSettings();
        LoadSettings();
    }

    /// <summary>
    /// Loads settings from JSON file
    /// </summary>
    public void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                string json = File.ReadAllText(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);

                if (settings != null)
                {
                    _currentSettings = settings;
                    _logger.LogInfo($"Settings loaded from: {_settingsFilePath}");
                }
            }
            else
            {
                _logger.LogInfo("Settings file not found, using defaults");
                _currentSettings = new AppSettings();
                SaveSettings(); // Создаем файл с дефолтными настройками
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to load settings: {ex.Message}");
            _currentSettings = new AppSettings();
        }
    }

    /// <summary>
    /// Saves settings to JSON file
    /// </summary>
    public void SaveSettings()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(_currentSettings, options);
            File.WriteAllText(_settingsFilePath, json);

            _logger.LogInfo($"Settings saved to: {_settingsFilePath}");

            // Notify subscribers
            SettingsChanged?.Invoke(this, new SettingsChangedEventArgs(_currentSettings));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save settings: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates settings
    /// </summary>
    public void UpdateSettings(AppSettings newSettings)
    {
        _currentSettings = newSettings ?? throw new ArgumentNullException(nameof(newSettings));
        SaveSettings();
    }

    /// <summary>
    /// Resets settings to defaults
    /// </summary>
    public void ResetToDefaults()
    {
        _logger.LogInfo("Resetting settings to defaults");
        _currentSettings = new AppSettings();
        SaveSettings();
    }

    /// <summary>
    /// Получает стратегию передачи на основе текущих настроек
    /// </summary>
    public TransmissionStrategy GetTransmissionStrategy()
    {
        return _currentSettings.TransmissionMode switch
        {
            TransmissionMode.Fast => new FastStrategy(),
            TransmissionMode.Reliable => new ReliableStrategy(),
            TransmissionMode.Slow => new SlowStrategy(),
            _ => new ReliableStrategy() // Default fallback
        };
    }
}

