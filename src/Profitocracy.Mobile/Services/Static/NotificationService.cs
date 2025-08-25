using Plugin.LocalNotification;
using Profitocracy.Mobile.Resources.Strings;

namespace Profitocracy.Mobile.Services.Static;

public static class NotificationService
{
    private const int AddTransactionNotificationId = 100;
    private const int CreateTransactionsForRecurredId = 200;

    public static async Task<bool> AreNotificationsEnabled()
    {
        return await LocalNotificationCenter.Current.AreNotificationsEnabled();
    }
    
    public static async Task<NotificationResult> ScheduleAddTransactionReminderNotification(TimeSpan scheduleTime)
    {
        var notificationService = LocalNotificationCenter.Current;

        if (!notificationService.IsSupported)
        {
            return NotificationResult.NotSupported;
        }

        var enabled = await notificationService.AreNotificationsEnabled();

        if (!enabled)
        {
            var permitted = await notificationService.RequestNotificationPermission();

            if (!permitted)
            {
                return NotificationResult.NotPermitted;
            }
        }

        var currentDate = DateTime.Now.Date;
        var scheduledTime =
# if DEBUG
            currentDate.Add(scheduleTime);
# else
            currentDate.AddDays(1).Add(scheduleTime);
# endif

        var notification = new NotificationRequest
        {
            NotificationId = AddTransactionNotificationId,
            Title = AppResources.Notifications_AddTransactionReminder_Title,
            Description = AppResources.Notifications_AddTransactionReminder_Description,
            ReturningData = string.Empty,
            Schedule =
            {
                NotifyTime = scheduledTime,
                RepeatType = NotificationRepeat.Daily,
            },
        };

        var success = await notificationService.Show(notification);

        return success ?
            NotificationResult.Success :
            NotificationResult.Failed;
    }
    
    public static void CancelScheduledAddTransactionReminderNotification()
    {
        var notificationService = LocalNotificationCenter.Current;

        if (notificationService.IsSupported)
        {
            notificationService.Cancel(AddTransactionNotificationId);
        }
    }

    public static async Task<NotificationResult> SendCreateTransactionsForRecurredNotification(
        int createdTransactionsForRecurredCount)
    {
        if (createdTransactionsForRecurredCount <= 0)
        {
            return NotificationResult.Success;
        }
        
        var notificationService = LocalNotificationCenter.Current;

        if (!notificationService.IsSupported)
        {
            return NotificationResult.NotSupported;
        }

        var enabled = await notificationService.AreNotificationsEnabled();

        if (!enabled)
        {
            var permitted = await notificationService.RequestNotificationPermission();

            if (!permitted)
            {
                return NotificationResult.NotPermitted;
            }
        }

        var notification = new NotificationRequest
        {
            NotificationId = CreateTransactionsForRecurredId,
            Title = AppResources.RecurringTransactionWorker_CreateTransactionsForRecurred_Title,
            Description =
                string.Format(AppResources.RecurringTransactionWorker_CreateTransactionsForRecurred_Description,
                    createdTransactionsForRecurredCount),
            ReturningData = string.Empty
        };

        var success = await notificationService.Show(notification);

        return success ?
            NotificationResult.Success :
            NotificationResult.Failed;
    }
}

public enum NotificationResult
{
    Success,
    NotSupported,
    NotPermitted,
    Failed,
}
