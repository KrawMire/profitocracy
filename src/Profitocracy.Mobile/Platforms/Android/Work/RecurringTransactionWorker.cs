using Android.Content;
using Android.Util;
using AndroidX.Work;
using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Mobile.Utils;

namespace Profitocracy.Mobile.Platforms.Android.Work;

public class RecurringTransactionWorker(Context context, WorkerParameters workerParams) : Worker(context, workerParams)
{
    public const string WorkerName = "RecurringTransactionWorker";

    public override Result DoWork()
    {
        // This method will be called periodically based on the worker's schedule
        Log.Info(WorkerName, $"Worker running: {DateTime.Now}");
        var transactionService = ServiceHelper.GetService<ITransactionService>();

        try
        {
            Log.Info(WorkerName, "Creating transactions for recurred.");
            var createdTransactionsForRecurred = transactionService.CreateTransactionsForRecurred().Result;

            if (createdTransactionsForRecurred.Count > 0)
            {
                Log.Info(WorkerName, $"Created {createdTransactionsForRecurred.Count} transactions for recurred.");
            }
            else
            {
                Log.Info(WorkerName, "No transactions created for recurred.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(WorkerName, $"Error creating transactions for recurred: {ex.Message}");
            return Result.InvokeRetry();
        }
        finally
        {
            Log.Info(WorkerName, $"Worker finished: {DateTime.Now}");
        }

        return Result.InvokeSuccess();
    }
}
