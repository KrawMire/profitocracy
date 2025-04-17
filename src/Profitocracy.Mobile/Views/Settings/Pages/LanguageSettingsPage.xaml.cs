using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Services;
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
    }

    private void LanguageSettingsPage_OnLoaded(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.Initialize();
        });
    }

    private void EnglishLanguage_OnSelected(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            await ChangeLanguage(LocalizationService.English);
        });
    }

    private void RussianLanguage_OnSelected(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            await ChangeLanguage(LocalizationService.Russian);
        });
    }

    private void FrenchLanguage_OnSelected(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            await ChangeLanguage(LocalizationService.French);
        });
    }

    private async Task ChangeLanguage(string language)
    {
        await _viewModel.ChangeLanguage(language);

        await DisplayAlert(
            AppResources.InfoAlert_ChangeLanguage_Title,
            AppResources.InfoAlert_ChangeLanguage_Message,
            AppResources.InfoAlert_ChangeLanguage_Ok);
    }
}