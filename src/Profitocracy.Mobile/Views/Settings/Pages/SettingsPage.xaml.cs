using Plugin.Maui.AppRating;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Constants;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class SettingsPage : BaseContentPage
{
    private readonly IAppRating _appRating;

    public SettingsPage(IAppRating appRating)
    {
        InitializeComponent();
        VersionLabel.Text = AppInfo.VersionString;
        _appRating = appRating;
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

    private void RateAppButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(RateAppAsync);
    }

    private async Task NavigateToPage<T>() where T : Page
    {
        var page = Handler?.MauiContext?.Services.GetService<T>();

        if (page is not null)
        {
            await Navigation.PushAsync(page);
        }
    }

#if ANDROID
    private async Task RateAppAsync()
    {
        try
        {
            await _appRating.PerformRatingOnStoreAsync(UrlConstants.ApplicationId);
            return;
        }
        catch (Exception)
        {
            // Ignore
        }

        try
        {
            await _appRating.PerformInAppRateAsync();
            return;
        }
        catch (Exception)
        {
            // Ignore
        }

        // At least open the app page in a browser
        await Browser.Default.OpenAsync(UrlConstants.StoreUrl, BrowserLaunchMode.SystemPreferred);
    }
#elif IOS
    private async Task RateAppAsync()
    {
        try
        {
            await _appRating.PerformInAppRateAsync();
            return;
        }
        catch (Exception)
        {
            // Ignore
        }

        try
        {
            await _appRating.PerformRatingOnStoreAsync(UrlConstants.ApplicationId);
            return;
        }
        catch (Exception)
        {
            // Ignore
        }

        // At least open the app page in a browser
        await Browser.Default.OpenAsync(UrlConstants.StoreUrl, BrowserLaunchMode.SystemPreferred);
    }
#else
    private async Task RateAppAsync()
    {
        await Browser.Default.OpenAsync(UrlConstants.StoreUrl, BrowserLaunchMode.SystemPreferred);
    }
#endif
}