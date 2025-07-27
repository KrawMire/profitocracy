using Profitocracy.Core.Domain.Model.Settings;
using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Infrastructure.Abstractions.Internal;
using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Settings;

namespace Profitocracy.Infrastructure.Persistence.Sqlite.Mappers;

internal class SettingsMapper : IInfrastructureMapper<Settings, SettingsModel>
{
    public Settings MapToDomain(SettingsModel model)
    {
        var authSettings = new AuthenticationSettings
        {
            IsAuthenticationEnabled = model.IsAuthenticationEnabled,
            IsBiometricAuthEnabled = model.IsBiometricAuthEnabled,
            PasswordHash = model.Password,
        };

        var notifySettings = new NotificationsSettings
        {
            IsEnabled = model.IsNotificationsEnabled,
            AddTransactionReminder = new NotificationEventSettings
            {
                IsEnabled = model.IsAddTransactionReminderEnabled,
                ScheduledTime = model.AddTransactionReminderTime,
            },
        };

        return new Settings(
            model.Id,
            (Theme)model.Theme,
            model.Language,
            authSettings,
            notifySettings);
    }

    public SettingsModel MapToModel(Settings entity)
    {
        return new SettingsModel
        {
            Id = entity.Id,
            Theme = (short)entity.Theme,
            Language = entity.Language,
            IsAuthenticationEnabled = entity.Authentication.IsAuthenticationEnabled,
            IsBiometricAuthEnabled = entity.Authentication.IsBiometricAuthEnabled,
            Password = entity.Authentication.PasswordHash,
            IsNotificationsEnabled = entity.Notifications.IsEnabled,
            IsAddTransactionReminderEnabled = entity.Notifications.AddTransactionReminder.IsEnabled,
            AddTransactionReminderTime = entity.Notifications.AddTransactionReminder.ScheduledTime,
        };
    }
}