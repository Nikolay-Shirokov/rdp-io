namespace TextSimulator.Configuration;

/// <summary>
/// Event args for settings changed event
/// </summary>
public class SettingsChangedEventArgs : EventArgs
{
    public AppSettings NewSettings { get; }

    public SettingsChangedEventArgs(AppSettings newSettings)
    {
        NewSettings = newSettings;
    }
}
