using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RdpIo.Configuration;
using RdpIo.Infrastructure.Logging;
using RdpIo.UI.Commands;

namespace RdpIo.UI.Windows;

/// <summary>
/// ViewModel для окна настроек
/// </summary>
public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly AppSettings _settings;
    private TransmissionMode _selectedTransmissionMode;
    private int _countdownSeconds;
    private bool _enableSounds;
    private bool _soundOnStart;
    private bool _soundOnComplete;
    private bool _soundOnError;
    private int _clipboardCacheLifetime;
    private LogLevel _selectedLogLevel;
    private int _maxLogFileSizeMB;
    private string _selectedOcrLanguage;
    private bool _ocrEnablePreprocessing;

    public event PropertyChangedEventHandler? PropertyChanged;

    public SettingsViewModel(AppSettings settings)
    {
        _settings = settings;

        // Инициализация из текущих настроек
        _selectedTransmissionMode = settings.TransmissionMode;
        _countdownSeconds = settings.CountdownSeconds;
        _enableSounds = settings.EnableSounds;
        _soundOnStart = settings.SoundOnStart;
        _soundOnComplete = settings.SoundOnComplete;
        _soundOnError = settings.SoundOnError;
        _clipboardCacheLifetime = settings.ClipboardCacheLifetimeSeconds;
        _selectedLogLevel = settings.LogLevel;
        _maxLogFileSizeMB = settings.MaxLogFileSizeMB;
        _selectedOcrLanguage = settings.OcrLanguage;
        _ocrEnablePreprocessing = settings.OcrEnablePreprocessing;

        // Команды
        SaveCommand = new RelayCommand(_ => Save(), _ => CanSave());
        CancelCommand = new RelayCommand(_ => Cancel());
    }

    #region Properties

    /// <summary>
    /// Режим передачи
    /// </summary>
    public TransmissionMode SelectedTransmissionMode
    {
        get => _selectedTransmissionMode;
        set
        {
            if (_selectedTransmissionMode != value)
            {
                _selectedTransmissionMode = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Время обратного отсчета (секунды)
    /// </summary>
    public int CountdownSeconds
    {
        get => _countdownSeconds;
        set
        {
            if (_countdownSeconds != value)
            {
                _countdownSeconds = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Включить звуковые уведомления
    /// </summary>
    public bool EnableSounds
    {
        get => _enableSounds;
        set
        {
            if (_enableSounds != value)
            {
                _enableSounds = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Звук при старте
    /// </summary>
    public bool SoundOnStart
    {
        get => _soundOnStart;
        set
        {
            if (_soundOnStart != value)
            {
                _soundOnStart = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Звук при завершении
    /// </summary>
    public bool SoundOnComplete
    {
        get => _soundOnComplete;
        set
        {
            if (_soundOnComplete != value)
            {
                _soundOnComplete = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Звук при ошибке
    /// </summary>
    public bool SoundOnError
    {
        get => _soundOnError;
        set
        {
            if (_soundOnError != value)
            {
                _soundOnError = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Время жизни кэша буфера обмена (секунды)
    /// </summary>
    public int ClipboardCacheLifetime
    {
        get => _clipboardCacheLifetime;
        set
        {
            if (_clipboardCacheLifetime != value)
            {
                _clipboardCacheLifetime = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Уровень логирования
    /// </summary>
    public LogLevel SelectedLogLevel
    {
        get => _selectedLogLevel;
        set
        {
            if (_selectedLogLevel != value)
            {
                _selectedLogLevel = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Максимальный размер файла лога (MB)
    /// </summary>
    public int MaxLogFileSizeMB
    {
        get => _maxLogFileSizeMB;
        set
        {
            if (_maxLogFileSizeMB != value)
            {
                _maxLogFileSizeMB = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Язык OCR распознавания
    /// </summary>
    public string SelectedOcrLanguage
    {
        get => _selectedOcrLanguage;
        set
        {
            if (_selectedOcrLanguage != value)
            {
                _selectedOcrLanguage = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Включить предобработку изображения для OCR
    /// </summary>
    public bool OcrEnablePreprocessing
    {
        get => _ocrEnablePreprocessing;
        set
        {
            if (_ocrEnablePreprocessing != value)
            {
                _ocrEnablePreprocessing = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Список доступных режимов передачи
    /// </summary>
    public TransmissionMode[] TransmissionModes { get; } =
        (TransmissionMode[])Enum.GetValues(typeof(TransmissionMode));

    /// <summary>
    /// Список доступных уровней логирования
    /// </summary>
    public LogLevel[] LogLevels { get; } =
        (LogLevel[])Enum.GetValues(typeof(LogLevel));

    /// <summary>
    /// Список доступных языков OCR
    /// </summary>
    public string[] OcrLanguages { get; } = new[]
    {
        "auto",    // Automatic language detection
        "en",      // English
        "ru",      // Russian
        "en-US",   // English (United States)
        "en-GB",   // English (United Kingdom)
        "ru-RU",   // Russian (Russia)
        "de",      // German
        "fr",      // French
        "es",      // Spanish
        "zh-CN",   // Chinese (Simplified)
        "ja",      // Japanese
        "ko"       // Korean
    };

    #endregion

    #region Commands

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event EventHandler? Saved;
    public event EventHandler? Cancelled;

    #endregion

    #region Private Methods

    private bool CanSave()
    {
        // Валидация: время отсчета должно быть от 1 до 60 секунд
        if (_countdownSeconds < 1 || _countdownSeconds > 60)
            return false;

        // Кэш буфера: от 1 до 300 секунд (5 минут)
        if (_clipboardCacheLifetime < 1 || _clipboardCacheLifetime > 300)
            return false;

        // Размер лога: от 1 до 100 MB
        if (_maxLogFileSizeMB < 1 || _maxLogFileSizeMB > 100)
            return false;

        // Язык OCR не должен быть пустым
        if (string.IsNullOrWhiteSpace(_selectedOcrLanguage))
            return false;

        return true;
    }

    private void Save()
    {
        // Применяем изменения к настройкам
        _settings.TransmissionMode = _selectedTransmissionMode;
        _settings.CountdownSeconds = _countdownSeconds;
        _settings.EnableSounds = _enableSounds;
        _settings.SoundOnStart = _soundOnStart;
        _settings.SoundOnComplete = _soundOnComplete;
        _settings.SoundOnError = _soundOnError;
        _settings.ClipboardCacheLifetimeSeconds = _clipboardCacheLifetime;
        _settings.LogLevel = _selectedLogLevel;
        _settings.MaxLogFileSizeMB = _maxLogFileSizeMB;
        _settings.OcrLanguage = _selectedOcrLanguage;
        _settings.OcrEnablePreprocessing = _ocrEnablePreprocessing;

        Saved?.Invoke(this, EventArgs.Empty);
    }

    private void Cancel()
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}

