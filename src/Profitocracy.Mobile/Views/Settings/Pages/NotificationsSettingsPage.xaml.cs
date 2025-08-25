using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Services.Static;
using Profitocracy.Mobile.ViewModels.Settings;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class NotificationsSettingsPage
{
    private readonly NotificationsSettingsPageViewModel _viewModel;

    public NotificationsSettingsPage(NotificationsSettingsPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }

    private void SaveButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            var result = await _viewModel.SaveSettings();

            switch (result)
            {
                case NotificationResult.Success:
                    await DisplayAlert(AppResources.NotificationSettings_SavedAlert_Success_Title,
                        AppResources.NotificationSettings_SavedAlert_Success_Description,
                        AppResources.NotificationSettings_SavedAlert_Success_Ok);
                    await Navigation.PopAsync();
                    return;
                case NotificationResult.NotSupported:
                    await DisplayAlert(AppResources.NotificationSettings_SavedAlert_NotSupported_Title,
                        AppResources.NotificationSettings_SavedAlert_NotSupported_Description,
                        AppResources.NotificationSettings_SavedAlert_NotSupported_Ok);
                    return;
                case NotificationResult.NotPermitted:
                    await DisplayAlert(AppResources.NotificationSettings_SavedAlert_NotPermitted_Title,
                        AppResources.NotificationSettings_SavedAlert_NotPermitted_Description,
                        AppResources.NotificationSettings_SavedAlert_NotPermitted_Ok);
                    return;
                case NotificationResult.Failed:
                    await DisplayAlert(AppResources.NotificationSettings_SavedAlert_Failed_Title,
                        AppResources.NotificationSettings_SavedAlert_Failed_Description,
                        AppResources.NotificationSettings_SavedAlert_Failed_Ok);
                    return;
                default:
                    return;
            }
        });
    }

    private void NotificationsSettingsPage_OnLoaded(object? sender, EventArgs e)
    {
        ProcessAction(_viewModel.Initialize);
    }
}
