namespace Profitocracy.Core.Domain.Model.Settings.ValueObjects;

/// <summary>
/// Represents the configuration settings for a specific notification event.
/// </summary>
public struct NotificationEventSettings
{
    /// <summary>
    /// Indicates whether the notification event is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Specifies the time at which the notification event is scheduled to occur.
    /// </summary>
    public TimeSpan ScheduledTime { get; set; }
}