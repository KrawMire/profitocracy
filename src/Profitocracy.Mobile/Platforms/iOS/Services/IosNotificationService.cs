using Profitocracy.Mobile.Services.PlatformDependent;

namespace Profitocracy.Mobile.Services;

public class IosNotificationService : INotificationService
{
    public event EventHandler? NotificationReceived;
    public void SendNotification(string title, string message, DateTime? notifyTime = null)
    {
        throw new NotImplementedException();
    }

    public void ReceiveNotification(string title, string message)
    {
        throw new NotImplementedException();
    }
}