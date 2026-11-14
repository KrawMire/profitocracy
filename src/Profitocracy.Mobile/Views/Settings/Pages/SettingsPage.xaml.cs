using Plugin.Maui.AppRating;
using Profitocracy.Core.Exceptions;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Constants;
using Profitocracy.Mobile.Models.Profiles;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Settings;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class SettingsPage : BaseContentPage
{
    private readonly IAppRating _appRating;
    private readonly SettingsPageViewModel _viewModel;

    public SettingsPage(IAppRating appRating, SettingsPageViewModel viewModel)
    {
        InitializeComponent();
        VersionLabel.Text = AppInfo.VersionString;
        _appRating = appRating;
        BindingContext = _viewModel = viewModel;
    }

    private void SettingsPage_OnNavigatedTo(object? sender, NavigatedToEventArgs e)
    {
        ProcessAction(_viewModel.Initialize);
    }

    private void AddProfileButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(() => OpenEditProfilePage(null));
    }

    private void CurrentProfile_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(() => OpenEditProfilePage(_viewModel.CurrentProfile?.Id));
    }

    private void SwitchProfile_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            if ((sender as BindableObject)?.BindingContext is not ProfileModel { Id: not null } profile)
            {
                throw new ArgumentNullException(AppResources.CommonError_GetProfileById);
            }

            var changeCurrentProfile = await DisplayAlert(
                AppResources.ProfileSettings_ChangeAlert_Title,
                string.Format(AppResources.ProfileSettings_ChangeAlert_Desription, profile.Name),
                AppResources.ProfileSettings_ChangeAlert_Ok,
                AppResources.ProfileSettings_ChangeAlert_Cancel);

            if (changeCurrentProfile)
            {
                await _viewModel.SetCurrentProfile(profile.Id.Value);
            }
        });
    }

    private void EditProfile_OnInvoked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            if ((sender as BindableObject)?.BindingContext is not ProfileModel { Id: not null } profile)
            {
                throw new ArgumentNullException(AppResources.CommonError_GetProfileById);
            }

            await OpenEditProfilePage(profile.Id);
        });
    }

    private void DeleteProfile_OnInvoked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            if ((sender as BindableObject)?.BindingContext is not ProfileModel { Id: not null } profile)
            {
                throw new ArgumentNullException(AppResources.CommonError_GetProfileById);
            }

            var isDelete = await DisplayAlert(
                AppResources.ProfileSettings_DeleteAlert_Title,
                string.Format(AppResources.ProfileSettings_DeleteAlert_Description, profile.Name),
                AppResources.ProfileSettings_DeleteAlert_Ok,
                AppResources.ProfileSettings_DeleteAlert_Cancel);

            if (!isDelete)
            {
                return;
            }

            try
            {
                await _viewModel.DeleteProfile(profile.Id.Value);
            }
            catch (LastProfileDeletionException ex)
            {
                await DisplayAlert(
                    AppResources.ProfileSettings_LastProfileErrorAlert_Title,
                    string.Format(AppResources.ProfileSettings_LastProfileErrorAlert_Description, ex.ProfileName),
                    AppResources.ProfileSettings_LastProfileErrorAlert_Ok);
            }
        });
    }

    private void BudgetSplitButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await DisplayAlert(
                AppResources.Settings_BudgetSplit,
                AppResources.Settings_BudgetSplitInfo,
                AppResources.ErrorAlert_Ok);
        });
    }

    private async Task OpenEditProfilePage(Guid? profileId)
    {
        var editPage = Handler?.MauiContext?.Services.GetService<EditProfilePage>();

        if (editPage is null)
        {
            throw new ArgumentNullException(AppResources.CommonError_GetProfileById);
        }

        if (profileId is not null)
        {
            editPage.AddProfileId((Guid)profileId);
        }

        await Navigation.PushModalAsync(editPage);
    }

    private void CategoriesButton_OnClicked(object? sender, EventArgs e)
        => NavigateToPage<ExpenseCategoriesSettingsPage>();

    private void NotificationsButton_OnClicked(object? sender, EventArgs e)
        => NavigateToPage<NotificationsSettingsPage>();

    private void ImportExportButton_OnClicked(object? sender, EventArgs e)
        => NavigateToPage<ImportExportSettingsPage>();

    private void AuthenticationButton_OnClicked(object? sender, EventArgs e)
        => NavigateToPage<AuthSettingsPage>();

    private void ThemeButton_OnClicked(object? sender, EventArgs e)
        => NavigateToPage<ThemeSettingsPage>();

    private void LanguageButton_OnClicked(object? sender, EventArgs e)
        => NavigateToPage<LanguageSettingsPage>();

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

    private void NavigateToPage<T>() where T : Page
    {
        ProcessAction(async () =>
        {
            var page = Handler?.MauiContext?.Services.GetService<T>();

            if (page is not null)
            {
                await Navigation.PushAsync(page);
            }
        });
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
