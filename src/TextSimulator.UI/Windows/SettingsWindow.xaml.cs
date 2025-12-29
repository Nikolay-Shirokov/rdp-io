using System.Windows;
using System.Windows.Input;
using TextSimulator.Configuration;

namespace TextSimulator.UI.Windows;

/// <summary>
/// Окно настроек приложения
/// </summary>
public partial class SettingsWindow : Window
{
    private readonly SettingsViewModel _viewModel;

    public SettingsWindow(AppSettings settings)
    {
        InitializeComponent();

        _viewModel = new SettingsViewModel(settings);
        DataContext = _viewModel;

        // Обработка событий ViewModel
        _viewModel.Saved += OnSaved;
        _viewModel.Cancelled += OnCancelled;

        // Обработка Esc для отмены
        KeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        };
    }

    private void OnSaved(object? sender, EventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnCancelled(object? sender, EventArgs e)
    {
        DialogResult = false;
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Отписываемся от событий
        _viewModel.Saved -= OnSaved;
        _viewModel.Cancelled -= OnCancelled;
    }
}
