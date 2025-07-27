using Android.Content;

namespace Profitocracy.Mobile.Services;

[BroadcastReceiver(Enabled = true, Label = "Local Notifications Broadcast Receiver")]
public class AlarmHandler : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent?.Extras is null)
        {
            return;
        }

        var title = intent.GetStringExtra(AndroidNotificationService.TitleKey);
        var message = intent.GetStringExtra(AndroidNotificationService.MessageKey);

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var manager = AndroidNotificationService.Instance;
        manager.Show(title, message);
    }
}