using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Settings;

public class Settings : AggregateRoot<Guid>
{
    public Settings(
        Guid id,
        Theme theme,
        string language,
        AuthenticationSettings authSettings,
        NotificationsSettings notifications) : base(id)
    {
        Theme = theme;
        Language = language;

        if (!authSettings.IsAuthenticationEnabled)
        {
            authSettings.IsBiometricAuthEnabled = false;
            authSettings.PasswordHash = null;
        }

        if (!notifications.IsEnabled)
        {
            notifications.AddTransactionReminder = new NotificationEventSettings
            {
                IsEnabled = false,
                ScheduledTime = TimeSpan.Zero,
            };
        }

        Authentication = authSettings;
        Notifications = notifications;
    }

    /// <summary>
    /// Current used theme.
    /// </summary>
    public Theme Theme { get; set; }

    /// <summary>
    /// Language is represented by a lang code.
    /// (Example: English - en, Russian - ru)
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Authentication-related settings.
    /// </summary>
    public AuthenticationSettings Authentication { get; private set; }

    /// <summary>
    /// Settings of notifications.
    /// </summary>
    public NotificationsSettings Notifications { get; set; }

    /// <summary>
    /// Enables authentication by configuring authentication settings.
    /// </summary>
    /// <param name="useBiometricAuth">Specifies whether biometric authentication should be enabled.</param>
    /// <param name="passwordHash">The hashed password to be used for authentication.</param>
    public void EnableAuthentication(bool useBiometricAuth, string passwordHash)
    {
        Authentication = new AuthenticationSettings
        {
            IsAuthenticationEnabled = true,
            IsBiometricAuthEnabled = useBiometricAuth,
            PasswordHash = passwordHash,
        };
    }

    /// <summary>
    /// Disables authentication by resetting authentication settings to default values.
    /// </summary>
    public void DisableAuthentication()
    {
        Authentication = new AuthenticationSettings
        {
            IsAuthenticationEnabled = false,
            IsBiometricAuthEnabled = false,
            PasswordHash = null,
        };
    }

    public void EnableNotifications(NotificationEventSettings addTransactionReminder)
    {
        Notifications = new NotificationsSettings
        {
            IsEnabled = true,
            AddTransactionReminder = addTransactionReminder,
        };
    }

    public void DisableNotifications()
    {
        Notifications = new NotificationsSettings
        {
            IsEnabled = false,
            AddTransactionReminder = new NotificationEventSettings
            {
                IsEnabled = false,
                ScheduledTime = TimeSpan.Zero,
            },
        };
    }
}