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

        BindingContext = viewModel;
    }
}