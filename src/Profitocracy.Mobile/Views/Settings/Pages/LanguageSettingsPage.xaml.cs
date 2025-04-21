using CommunityToolkit.Mvvm.Messaging;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Settings;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class LanguageSettingsPage : BaseContentPage
{
    private readonly LanguageSettingsViewModel _viewModel;

    public LanguageSettingsPage(LanguageSettingsViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();

        BindingContext = _viewModel;

        // Subscribe to language changes to show the alert
        WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, async (recipient, message) =>
        {
            await DisplayAlert(
                AppResources.InfoAlert_ChangeLanguage_Title,
                AppResources.InfoAlert_ChangeLanguage_Message,
                AppResources.InfoAlert_ChangeLanguage_Ok);
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.Unregister<LanguageChangedMessage>(this);
    }

    private void LanguageSettingsPage_OnLoaded(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.Initialize();
        });
    }
}

// Define the message class for language changes
public class LanguageChangedMessage
{
}
