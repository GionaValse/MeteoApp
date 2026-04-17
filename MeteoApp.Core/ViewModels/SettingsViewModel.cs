using System;
using System.Windows.Input;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp.Core.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly IPreferencesService _preferencesService;
    private string _language;
    private bool _notificationsEnabled;
    private DateTime _lastSync;

    public SettingsViewModel(IPreferencesService preferencesService)
    {
        _preferencesService = preferencesService;

        var prefs = _preferencesService.GetPreferences();

        _language = prefs.Language ?? "Italiano";
        _notificationsEnabled = prefs.NotificationsEnabled;
        _lastSync = prefs.LastSync;
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
                SaveSettings();
            }
        }
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
            LastSync = this.LastSync
        };

        _preferencesService.SavePreferences(prefs);
    }
}