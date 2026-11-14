using Foundation;
using Profitocracy.Mobile.Work;
using UIKit;

namespace Profitocracy.Mobile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private NSObject? _didBecomeActiveObserver;

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        RecurringTransactionWorker.RegisterBackgroundTask();

        // With the UIScene life cycle (required since the iOS 27 SDK) applicationDidBecomeActive
        // is no longer called, so the background task is rescheduled on the notification instead.
        _didBecomeActiveObserver = UIApplication.Notifications.ObserveDidBecomeActive((_, _) =>
        {
            RecurringTransactionWorker.ScheduleBackgroundTask();
        });

        return base.FinishedLaunching(application, launchOptions);
    }
}
