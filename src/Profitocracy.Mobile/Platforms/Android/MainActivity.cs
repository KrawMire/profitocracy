using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Profitocracy.Mobile.Services;
using Profitocracy.Mobile.Services.PlatformDependent;

namespace Profitocracy.Mobile;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Intent is not null)
        {
            CreateNotificationFromIntent(Intent);
        }
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);

        if (intent is not null)
        {
            CreateNotificationFromIntent(intent);
        }
    }

    private static void CreateNotificationFromIntent(Intent intent)
    {
        if (intent.Extras is null)
        {
            return;
        }

        var title = intent.GetStringExtra(AndroidNotificationService.TitleKey);
        var message = intent.GetStringExtra(AndroidNotificationService.MessageKey);

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var service = IPlatformApplication.Current?.Services.GetService<INotificationService>();

        service?.ReceiveNotification(title, message);
    }
}