using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Domain.Model.Transactions.Factories;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Persistence;

namespace Profitocracy.Core.Domain.Services;

internal class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProfileRepository _profileRepository;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IProfileRepository profileRepository)
    {
        _transactionRepository = transactionRepository;
        _profileRepository = profileRepository;
    }

    public async Task<bool> CheckTransactionInCurrentPeriod(Guid transactionId)
    {
        var transaction = await _transactionRepository.GetById(transactionId);

        if (transaction is null)
        {
            return false;
        }

        var profile = await _profileRepository.GetCurrentProfile();

        if (profile is null)
        {
            return false;
        }

        return profile.BillingPeriod.DateFrom <= transaction.Timestamp &&
               transaction.Timestamp <= profile.BillingPeriod.DateTo;
    }

    public async Task<List<Transaction>> CreateTransactionsForRecurred()
    {
        var profile = await _profileRepository.GetCurrentProfile();

        if (profile is null)
        {
            return [];
        }

        var createdTransactionsForRecurred = new List<Transaction>();
        var recurringTransactions = await _transactionRepository.GetRecurringTransactions(profile.Id);
        var today = DateTime.Now;

        foreach (var recurringTransaction in recurringTransactions)
        {
            var recurringTransactionInfo = recurringTransaction.RecurringTransactionInfo;
            var lastMaturityDate = recurringTransactionInfo!.LastMaturityDate ?? recurringTransaction.Timestamp;
            var hasMoreRecurringTransactions = lastMaturityDate <= today;

            while (hasMoreRecurringTransactions)
            {
                var nextMaturityDate = recurringTransactionInfo.Interval switch
                {
                    RecurringTransactionInterval.Daily => lastMaturityDate.AddDays(1),
                    RecurringTransactionInterval.Weekly => lastMaturityDate.AddDays(7),
                    RecurringTransactionInterval.Monthly => lastMaturityDate.AddMonths(1),
                    RecurringTransactionInterval.Quarterly => lastMaturityDate.AddMonths(3),
                    RecurringTransactionInterval.EverySixMonths => lastMaturityDate.AddMonths(6),
                    RecurringTransactionInterval.Annually => lastMaturityDate.AddYears(1),
                    _ => throw new ArgumentOutOfRangeException(nameof(recurringTransactionInfo.Interval),
                        "Invalid recurring transaction interval")
                };
                lastMaturityDate = nextMaturityDate;
                hasMoreRecurringTransactions = nextMaturityDate <= today;

                Transaction newTransaction;
                if (recurringTransaction is MultiCurrencyTransaction recurringMultiCurrencyTransaction)
                {
                    newTransaction = TransactionFactory.CreateMultiCurrencyTransaction(
                        null,
                        recurringMultiCurrencyTransaction.Amount,
                        recurringMultiCurrencyTransaction.DestinationAmount,
                        recurringMultiCurrencyTransaction.SourceCurrency,
                        recurringMultiCurrencyTransaction.DestinationCurrency,
                        recurringMultiCurrencyTransaction.ProfileId,
                        recurringMultiCurrencyTransaction.Type,
                        recurringMultiCurrencyTransaction.SpendingType,
                        recurringMultiCurrencyTransaction.Destination,
                        nextMaturityDate,
                        recurringMultiCurrencyTransaction.Description,
                        recurringMultiCurrencyTransaction.GeoTag,
                        recurringMultiCurrencyTransaction.Category,
                        null);
                }
                else
                {
                    newTransaction = TransactionFactory.CreateTransaction(
                        null,
                        recurringTransaction.Amount,
                        recurringTransaction.ProfileId,
                        recurringTransaction.Type,
                        recurringTransaction.SpendingType,
                        nextMaturityDate,
                        recurringTransaction.Description,
                        recurringTransaction.GeoTag,
                        recurringTransaction.Category,
                        null);
                }

                // Create a new transaction for recurred.
                await _transactionRepository.Create(newTransaction);
                createdTransactionsForRecurred.Add(newTransaction);
                // Update the recurring transaction's last maturity date.
                recurringTransactionInfo.LastMaturityDate = nextMaturityDate;
                recurringTransaction.RecurringTransactionInfo = recurringTransactionInfo;
                await _transactionRepository.Update(recurringTransaction);
            }
        }

        return createdTransactionsForRecurred;
    }
}
