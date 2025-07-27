using Plugin.LocalNotification;
using Profitocracy.Mobile.Resources.Strings;

namespace Profitocracy.Mobile.Services.Static;

public static class NotificationService
{
    private const int AddTransactionNotificationId = 100;

    public static async Task<ScheduleNotificationResult> ScheduleAddTransactionReminderNotification(TimeSpan scheduleTime)
    {
        var notificationService = LocalNotificationCenter.Current;

        if (!notificationService.IsSupported)
        {
            return ScheduleNotificationResult.NotSupported;
        }

        var enabled = await notificationService.AreNotificationsEnabled();

        if (!enabled)
        {
            var permitted = await notificationService.RequestNotificationPermission();

            if (!permitted)
            {
                return ScheduleNotificationResult.NotPermitted;
            }
        }

        var currentDate = DateTime.Now.Date;
        var scheduledTime = currentDate
            .AddDays(1)
            .Add(scheduleTime);

        var notification = new NotificationRequest
        {
            NotificationId = AddTransactionNotificationId,
            Title = AppResources.Notifications_AddTransactionReminder_Title,
            Description = AppResources.Notifications_AddTransactionReminder_Description,
            ReturningData = string.Empty,
            Schedule =
            {
                NotifyTime = scheduledTime,
            },
        };

        var success = await notificationService.Show(notification);

        return success ?
            ScheduleNotificationResult.Success :
            ScheduleNotificationResult.Failed;
    }
}

public enum ScheduleNotificationResult
{
    Success,
    NotSupported,
    NotPermitted,
    Failed,
}