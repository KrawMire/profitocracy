using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Settings.ValueObjects;

/// <summary>
/// Represents settings for notifications within the system.
/// </summary>
public class NotificationsSettings : ValueObject
{
    /// <summary>
    /// Indicates whether the notifications feature is enabled or disabled in the system.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Defines the settings of reminders related to transaction notifications.
    /// </summary>
    public NotificationEventSettings AddTransactionReminder { get; set; }
}