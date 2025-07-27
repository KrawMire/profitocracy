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
                case ScheduleNotificationResult.Success:
                    await DisplayAlert("Success", "Settings has been successfully saved", "OK");
                    await Navigation.PopAsync();
                    return;
                case ScheduleNotificationResult.NotSupported:
                    await DisplayAlert("Not supported", "This device does not support notifications", "OK");
                    return;
                case ScheduleNotificationResult.NotPermitted:
                    await DisplayAlert("Not permitted", "Notifications are not permitted", "OK");
                    return;
                case ScheduleNotificationResult.Failed:
                    await DisplayAlert("Failed", "An attempt to register a notification failed", "OK");
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