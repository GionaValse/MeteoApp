using MeteoApp.Core.Helpers;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace MeteoApp.Core.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly IPreferencesService _preferencesService;
    private readonly IThemeService _themeService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationProvider _notificationProvider;

    private string _language;
    private bool _notificationsEnabled;
    private DateTime _lastSync;
    private int _selectedThemeIndex;

    public SettingsViewModel(
        IPreferencesService preferencesService, 
        IThemeService themeService,
        ILocalizationService localizationService,
        INotificationProvider notificationProvider
        )
    {
        _preferencesService = preferencesService;
        _themeService = themeService;
        _localizationService = localizationService;
        _notificationProvider = notificationProvider;

        var prefs = _preferencesService.GetPreferences();

        _language = prefs.Language ?? "Italiano";
        _notificationsEnabled = prefs.NotificationsEnabled;
        _lastSync = prefs.LastSync;
        _selectedThemeIndex = prefs.Theme;

        _themeService.SetTheme(_selectedThemeIndex);
        SyncCommand = new RelayCommand(ExecuteSync);
    }

    public string Language
    {
        get => _language;
        set
        {
            if (_language != value)
            {
                _language = value;
                OnPropertyChanged();

                SaveSettings();
                _localizationService.SetLanguage(value);
            }
        }
    }

    public bool NotificationsEnabled
    {
        get => _notificationsEnabled;
        set
        {
            if (_notificationsEnabled != value)
            {
                _notificationsEnabled = value;
                OnPropertyChanged();

                _ = HandleNotificationsChangedAsync(value);
            }
        }
    }

    private async Task HandleNotificationsChangedAsync(bool enabled)
    {
        if (enabled)
        {
            string token = await _notificationProvider.RequestTokenAsync();
            
            if (string.IsNullOrEmpty(token))
            {
                _notificationsEnabled = false;
                OnPropertyChanged(nameof(NotificationsEnabled));
            }
        }
        else
        {
            // _notificationProvider.DisableNotifications();
        }

        SaveSettings();
    }

    public DateTime LastSync
    {
        get => _lastSync;
        set
        {
            if (_lastSync != value)
            {
                _lastSync = value;
                OnPropertyChanged();
            }
        }
    }

    public int SelectedThemeIndex
    {
        get => _selectedThemeIndex;
        set
        {
            if (_selectedThemeIndex != value)
            {
                _selectedThemeIndex = value;
                OnPropertyChanged();

                _themeService.SetTheme(value);
                SaveSettings();
            }
        }
    }

    public ICommand SyncCommand { get; protected set; }

    private void ExecuteSync()
    {
        LastSync = DateTime.Now;
        SaveSettings();
    }

    private void SaveSettings()
    {
        var prefs = new UserPreferences
        {
            Language = this.Language,
            NotificationsEnabled = this.NotificationsEnabled,
            LastSync = this.LastSync,
            Theme = this.SelectedThemeIndex
        };

        _preferencesService.SavePreferences(prefs);
    }
}