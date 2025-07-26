using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Constants;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class SettingsPage : BaseContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        VersionLabel.Text = AppInfo.VersionString;
    }

    private void ProfilesButton_OnClicked(object? sender, EventArgs e)
        => ProcessAction(NavigateToPage<ProfilesSettingsPage>);

    private void CategoriesButton_OnClicked(object? sender, EventArgs e)
        => ProcessAction(NavigateToPage<ExpenseCategoriesSettingsPage>);

    private void AuthenticationButton_OnClicked(object? sender, EventArgs e)
        => ProcessAction(NavigateToPage<AuthSettingsPage>);

    private void ThemeButton_OnClicked(object? sender, EventArgs e)
        => ProcessAction(NavigateToPage<ThemeSettingsPage>);

    private void LanguageButton_OnClicked(object? sender, EventArgs e)
        => ProcessAction(NavigateToPage<LanguageSettingsPage>);

    private void GitHubButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await Browser.Default.OpenAsync(UrlConstants.ProjectGitHubUrl, BrowserLaunchMode.SystemPreferred);
        });
    }

    private async Task NavigateToPage<T>() where T : Page
    {
        var page = Handler?.MauiContext?.Services.GetService<T>();

        if (page is not null)
        {
            await Navigation.PushAsync(page);
        }
    }
}