using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Services.Static;

namespace Profitocracy.Mobile.ViewModels.Settings;

public class NotificationsSettingsPageViewModel : BaseNotifyObject
{
    private readonly ISettingsRepository _settingsRepository;

    private bool _isEnabled;
    private bool _isAddTransactionReminderEnabled;
    private TimeSpan _notifyTime = DateTime.Now.TimeOfDay;

    public NotificationsSettingsPageViewModel(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public bool IsAddTransactionReminderEnabled
    {
        get => _isAddTransactionReminderEnabled;
        set => SetProperty(ref _isAddTransactionReminderEnabled, value);
    }

    public TimeSpan AddTransactionReminderTime
    {
        get => _notifyTime;
        set => SetProperty(ref _notifyTime, value);
    }

    public async Task Initialize()
    {
        var settings = await _settingsRepository.GetCurrentSettings();

        if (settings is null)
        {
            throw new Exception(AppResources.CommonError_GetSettings);
        }

        IsEnabled = settings.Notifications.IsEnabled;

        if (IsEnabled)
        {
            IsAddTransactionReminderEnabled = settings.Notifications.AddTransactionReminder.IsEnabled;
            AddTransactionReminderTime = settings.Notifications.AddTransactionReminder.ScheduledTime;
        }
    }

    public async Task<ScheduleNotificationResult> SaveSettings()
    {
        var settings = await _settingsRepository.GetCurrentSettings();

        if (settings is null)
        {
            throw new Exception(AppResources.CommonError_GetSettings);
        }

        if (IsEnabled)
        {
            var addTransactionReminderSettings = new NotificationEventSettings
            {
                IsEnabled = IsAddTransactionReminderEnabled,
                ScheduledTime = AddTransactionReminderTime,
            };

            settings.EnableNotifications(addTransactionReminderSettings);
        }
        else
        {
            settings.DisableNotifications();
        }

        var result = ScheduleNotificationResult.Success;

        if (IsEnabled && IsAddTransactionReminderEnabled)
        {
            result = await NotificationService.ScheduleAddTransactionReminderNotification(AddTransactionReminderTime);
        }

        await _settingsRepository.CreateOrUpdate(settings);

        return result;
    }
}