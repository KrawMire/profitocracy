using Foundation;
using Profitocracy.Mobile.Work;
using UIKit;

namespace Profitocracy.Mobile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        RecurringTransactionWorker.RegisterBackgroundTask();
        return base.FinishedLaunching(application, launchOptions);
    }

    public override void OnActivated(UIApplication application)
    {
        RecurringTransactionWorker.ScheduleBackgroundTask();
        base.OnActivated(application);
    }
}


