using MeteoApp.Core.ViewModels;
using MeteoApp.Resources.Strings;

namespace MeteoApp.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();

        ThemePicker.ItemsSource = new List<string>
        {
            AppResources.theme_system, 
            AppResources.theme_light,  
            AppResources.theme_dark   
        };

        StrategyPicker.ItemsSource = new List<string>
        {
            AppResources.sync_strategy_latest, 
            AppResources.sync_strategy_local,  
            AppResources.sync_strategy_remote  
        };

        BindingContext = viewModel;
    }
}