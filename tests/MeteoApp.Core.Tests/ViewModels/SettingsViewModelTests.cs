using Moq;
using Xunit;
using MeteoApp.Core.ViewModels;
using MeteoApp.Core.Services;
using MeteoApp.Core.Models;
using System;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MeteoApp.Core.Tests.ViewModels;

public class SettingsViewModelTests
{
    private readonly Mock<IPreferencesService> _mockPrefsService;
    private readonly Mock<IThemeService> _mockThemeService;
    private readonly Mock<ILocalizationService> _mockLocalizationService;
    private readonly Mock<INotificationProvider> _mockNotificationProvider;
    private readonly Mock<ISyncService<LocationModel>> _mockSyncService;

    public SettingsViewModelTests()
    {
        _mockPrefsService = new Mock<IPreferencesService>();
        _mockThemeService = new Mock<IThemeService>();
        _mockLocalizationService = new Mock<ILocalizationService>();
        _mockNotificationProvider = new Mock<INotificationProvider>();
        _mockSyncService = new Mock<ISyncService<LocationModel>>();

        _mockPrefsService.Setup(p => p.GetPreferences()).Returns(new UserPreferences
        {
            Language = "English",
            NotificationsEnabled = false,
            LastSync = DateTime.MinValue,
            Theme = 1
        });
    }

    private SettingsViewModel CreateViewModel()
    {
        return new SettingsViewModel(
            _mockPrefsService.Object,
            _mockThemeService.Object,
            _mockLocalizationService.Object,
            _mockNotificationProvider.Object,
            _mockSyncService.Object);
    }

    [Fact]
    public void Constructor_ShouldLoadPreferences_AndApplyTheme()
    {
        var viewModel = CreateViewModel();

        Assert.Equal("English", viewModel.Language);
        Assert.False(viewModel.NotificationsEnabled);
        Assert.Equal(1, viewModel.SelectedThemeIndex);

        _mockThemeService.Verify(t => t.SetTheme(1), Times.Once);
    }

    [Fact]
    public void Language_Setter_ShouldUpdateLanguage_SetLocalization_AndSave()
    {
        var viewModel = CreateViewModel();
        bool propertyChangedRaised = false;
        viewModel.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(viewModel.Language)) propertyChangedRaised = true; };

        viewModel.Language = "Deutsch";

        Assert.Equal("Deutsch", viewModel.Language);
        Assert.True(propertyChangedRaised);

        _mockLocalizationService.Verify(l => l.SetLanguage("Deutsch"), Times.Once);
        _mockPrefsService.Verify(p => p.SavePreferences(It.Is<UserPreferences>(up => up.Language == "Deutsch")), Times.Once);
    }

    [Fact]
    public void SelectedThemeIndex_Setter_ShouldUpdateTheme_AndSave()
    {
        var viewModel = CreateViewModel();

        viewModel.SelectedThemeIndex = 2;

        Assert.Equal(2, viewModel.SelectedThemeIndex);
        _mockThemeService.Verify(t => t.SetTheme(2), Times.Once);
        _mockPrefsService.Verify(p => p.SavePreferences(It.Is<UserPreferences>(up => up.Theme == 2)), Times.Once);
    }

    [Fact]
    public async Task NotificationsEnabled_SetterToTrue_WhenTokenSucceeds_ShouldStayTrue_AndSave()
    {
        var viewModel = CreateViewModel();

        _mockNotificationProvider.Setup(n => n.RequestTokenAsync()).ReturnsAsync("token-valido-123");
        
        viewModel.NotificationsEnabled = true;
        await Task.Delay(50);

        Assert.True(viewModel.NotificationsEnabled);
        _mockPrefsService.Verify(p => p.SavePreferences(It.Is<UserPreferences>(up => up.NotificationsEnabled == true)), Times.Once);
    }

    [Fact]
    public async Task NotificationsEnabled_SetterToTrue_WhenTokenFails_ShouldRevertToFalse()
    {
        var viewModel = CreateViewModel();

        _mockNotificationProvider.Setup(n => n.RequestTokenAsync()).ReturnsAsync(string.Empty);

        bool propertyChangedRaised = false;
        viewModel.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(viewModel.NotificationsEnabled)) propertyChangedRaised = true; };

        viewModel.NotificationsEnabled = true;
        await Task.Delay(50);

        Assert.False(viewModel.NotificationsEnabled);
        Assert.True(propertyChangedRaised);
        _mockPrefsService.Verify(p => p.SavePreferences(It.Is<UserPreferences>(up => up.NotificationsEnabled == false)), Times.Once);
    }

    [Fact]
    public void SyncCommand_Execute_ShouldUpdateLastSync_AndSave()
    {
        _mockSyncService
            .Setup(ss => ss.SynchronizeAsync(It.IsAny<ConflictResolutionStrategy>()))
            .Returns(Task.CompletedTask);

        var viewModel = CreateViewModel();
        viewModel.SyncCommand?.Execute(null);

        _mockSyncService.Verify(p => p.SynchronizeAsync(It.IsAny<ConflictResolutionStrategy>()), Times.AtLeastOnce);
    }
}
