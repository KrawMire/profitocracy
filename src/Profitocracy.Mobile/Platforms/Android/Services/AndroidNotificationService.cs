using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using Profitocracy.Mobile.Services.PlatformDependent;

namespace Profitocracy.Mobile.Services;

public class NotificationEventArgs : EventArgs
{
    public required string Title { get; set; }
    public required string Message { get; set; }
}

public class AndroidNotificationService : INotificationService
{
    private const string ChannelId = "default";
    private const string ChannelName = "Default";
    private const string ChannelDescription = "The default channel for notifications.";

    public const string TitleKey = "title";
    public const string MessageKey = "message";

    private bool _channelInitialized = false;
    private int _messageId = 0;
    private int _pendingIntentId = 0;

    private NotificationManagerCompat compatManager;

    public static AndroidNotificationService Instance { get; private set; }

    public AndroidNotificationService()
    {
        CreateNotificationChannel();
        compatManager = NotificationManagerCompat.From(Platform.AppContext)
            ?? throw new NullReferenceException("Cannot create a notification manager");
        Instance = this;
    }

    public event EventHandler NotificationReceived;

    public void SendNotification(string title, string message, DateTime? notifyTime = null)
    {
        if (!_channelInitialized)
        {
            CreateNotificationChannel();
        }

        if (notifyTime != null)
        {
            var intent = new Intent(Platform.AppContext, typeof(AlarmHandler));

            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);
            intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

            var pendingIntentFlags = Build.VERSION.SdkInt >= BuildVersionCodes.S
                ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
                : PendingIntentFlags.CancelCurrent;

            var pendingIntent = PendingIntent.GetBroadcast(Platform.AppContext, _pendingIntentId++, intent, pendingIntentFlags);
            long triggerTime = GetNotifyTime(notifyTime.Value);
            var alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }
        else
        {
            Show(title, message);
        }
    }

    public void ReceiveNotification(string title, string message)
    {
        var args = new NotificationEventArgs
        {
            Title = title,
            Message = message,
        };
        NotificationReceived.Invoke(null, args);
    }

    public void Show(string title, string message)
    {
        Intent intent = new Intent(Platform.AppContext, typeof(MainActivity));
        intent.PutExtra(TitleKey, title);
        intent.PutExtra(MessageKey, message);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.UpdateCurrent;

        PendingIntent pendingIntent = PendingIntent.GetActivity(Platform.AppContext, _pendingIntentId++, intent, pendingIntentFlags);
        NotificationCompat.Builder builder = new NotificationCompat.Builder(Platform.AppContext, ChannelId)
            .SetContentIntent(pendingIntent)
            .SetContentTitle(title)
            .SetContentText(message);
        // .SetLargeIcon(BitmapFactory.DecodeResource(Platform.AppContext.Resources, Resource.Drawable.dotnet_logo))
        // .SetSmallIcon(Resource.Drawable.message_small);

        // NetworkReachability.Notification notification = builder.Build();
        // compatManager.Notify(messageId++, notification);
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            return;
        }
#pragma warning disable CA1416 // The call site is only supported on API 26.0+
        var channelNameJava = new Java.Lang.String(ChannelName);
        var channel = new NotificationChannel(ChannelId, channelNameJava, NotificationImportance.Default)
        {
            Description = ChannelDescription,
        };

        if (Platform.AppContext.GetSystemService(Context.NotificationService) is not NotificationManager manager)
        {
            throw new NullReferenceException("Cannot create a notification manager");
        }

        manager.CreateNotificationChannel(channel);
        _channelInitialized = true;
#pragma warning restore CA1416
    }

    private static long GetNotifyTime(DateTime notifyTime)
    {
        var utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
        var epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
        var utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;

        return utcAlarmTime;
    }
}