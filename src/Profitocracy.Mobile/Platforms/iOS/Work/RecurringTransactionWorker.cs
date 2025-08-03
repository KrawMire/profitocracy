using BackgroundTasks;
using Foundation;
using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Mobile.Utils;

namespace Profitocracy.Mobile.Work;

#pragma warning disable CA1416 // This call site is reachable on ....
public static class RecurringTransactionWorker
{
    public const string BackgroundTaskIdentifier = "com.krawmire.profitocracy.recurring-transactions-worker";

    public static void RegisterBackgroundTask()
    {
        var success = BGTaskScheduler.Shared.Register(BackgroundTaskIdentifier, null, task =>
        {
            ExecuteBackgroundTask(task);
        });

        Console.WriteLine(success ?
            "Background task has been registered successfully."
            : "Failed to register background task.");
    }

    public static void ScheduleBackgroundTask()
    {
        try
        {
            var request = new BGAppRefreshTaskRequest(BackgroundTaskIdentifier)
            {
                EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(15 * 60),
            };

            BGTaskScheduler.Shared.Submit(request, out var error);

            if (error is not null)
            {
                throw new Exception(error.LocalizedDescription);
            }

            Console.WriteLine("Background task scheduled successfully. Waiting for execution...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to schedule background task: {ex.Message}");
        }
    }

    private static void ExecuteBackgroundTask(BGTask task)
    {
        ScheduleBackgroundTask();

        Console.WriteLine("Background task execution started.");

        var transactionService = ServiceHelper.GetService<ITransactionService>();

        try
        {
            Console.WriteLine("Creating transactions for recurred.");
            var createdTransactionsForRecurred = transactionService.CreateTransactionsForRecurred().Result;

            if (createdTransactionsForRecurred.Count > 0)
            {
                Console.WriteLine($"Created {createdTransactionsForRecurred.Count} transactions for recurred.");
            }
            else
            {
                Console.WriteLine("No transactions created for recurred.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating transactions for recurred: {ex.Message}");
            task.SetTaskCompleted(false);
            return;
        }

        Console.WriteLine("Background task execution finished.");
        task.SetTaskCompleted(true);
    }
}
#pragma warning restore CA1416
