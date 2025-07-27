using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Auth;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class AuthSettingsPage
{
    private readonly AuthSettingsPageViewModel _viewModel;

    public AuthSettingsPage(AuthSettingsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private void AuthSettingsPage_OnLoaded(object? sender, EventArgs e)
    {
        ProcessAction(_viewModel.Initialize);
    }

    private void SaveButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.Save();
            await DisplayAlert(
                AppResources.AuthSettings_SavedAlert_Title,
                AppResources.AuthSettings_SavedAlert_Description,
                AppResources.AuthSettings_SavedAlert_Ok);
            await Navigation.PopAsync();
        });
    }
}

