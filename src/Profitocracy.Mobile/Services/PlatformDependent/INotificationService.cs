namespace Profitocracy.Mobile.Services.PlatformDependent;

/// <summary>
/// Defines methods and events for sending and receiving notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// An event that is triggered when a notification is received.
    /// </summary>
    event EventHandler NotificationReceived;

    /// <summary>
    /// Sends a notification with the specified title and message, optionally at a specific time.
    /// </summary>
    /// <param name="title">The title of the notification.</param>
    /// <param name="message">The message to include in the notification.</param>
    /// <param name="notifyTime">The optional time at which to send the notification. If null, the notification is sent immediately.</param>
    void SendNotification(string title, string message, DateTime? notifyTime = null);

    /// <summary>
    /// Handles the reception of a notification with the specified title and message.
    /// </summary>
    /// <param name="title">The title of the received notification.</param>
    /// <param name="message">The message content of the received notification.</param>
    void ReceiveNotification(string title, string message);
}