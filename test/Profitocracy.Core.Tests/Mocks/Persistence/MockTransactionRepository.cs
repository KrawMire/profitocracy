using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Specifications;

namespace Profitocracy.Core.Tests.Mocks.Persistence;

public class MockTransactionRepository : ITransactionRepository
{
    private readonly Dictionary<Guid, Transaction> _transactions = new();

    public Task<List<Transaction>> GetAllByProfileId(Guid profileId)
    {
        return Task.FromResult(_transactions.Values.Where(t => t.ProfileId == profileId).ToList());
    }

    public Task<Transaction?> GetById(Guid transactionId)
    {
        return Task.FromResult(_transactions.GetValueOrDefault(transactionId));
    }

    public Task<List<Transaction>> GetForPeriod(Guid profileId, DateTime dateFrom, DateTime dateTo)
    {
        return Task.FromResult(_transactions.Values
            .Where(t => t.ProfileId == profileId && t.Timestamp >= dateFrom && t.Timestamp <= dateTo)
            .ToList());
    }

    public Task<List<Transaction>> GetFiltered(TransactionsSpecification spec)
    {
        var result = _transactions.Values
            .Where(t => IsTransactionSatisfied(t, ref spec))
            .ToList();
        return Task.FromResult(result);
    }

    private static bool IsTransactionSatisfied(Transaction transaction, ref TransactionsSpecification spec)
    {
        var result = true;

        if (spec.ProfileId.HasValue)
        {
            result &= transaction.ProfileId == spec.ProfileId.Value;
        }

        if (spec.FromDate.HasValue)
        {
            result &= transaction.Timestamp >= spec.FromDate.Value;
        }

        if (spec.ToDate.HasValue)
        {
            result &= transaction.Timestamp <= spec.ToDate.Value;
        }

        if (spec.TransactionType.HasValue)
        {
            result &= transaction.Type == spec.TransactionType.Value;
        }

        if (spec.SpendingType.HasValue)
        {
            result &= transaction.SpendingType == spec.SpendingType.Value;
        }

        if (spec.CategoryId is not null)
        {
            result &= transaction.Category?.Id == spec.CategoryId;
        }

        if (!string.IsNullOrEmpty(spec.Description))
        {
            if (transaction.Description is null)
            {
                result = false;
            }
            else
            {
                result &= transaction.Description.Contains(spec.Description, StringComparison.OrdinalIgnoreCase);
            }
        }

        return result;
    }

    public Task<Transaction> Create(Transaction transaction)
    {
        _transactions.Add(transaction.Id, transaction);
        return Task.FromResult(transaction);
    }

    public Task<Transaction> Update(Transaction transaction)
    {
        _transactions[transaction.Id] = transaction;
        return Task.FromResult(transaction);
    }

    public Task<Guid> ClearWithCategory(Guid categoryId)
    {
        foreach (var transaction in _transactions.Values.Where(t => t.Category?.Id == categoryId))
        {
            transaction.Category = null;
        }
        return Task.FromResult(categoryId);
    }

    public Task<Guid> ChangeCategoryName(Guid categoryId, string newName)
    {
        foreach (var transaction in _transactions.Values.Where(t => t.Category?.Id == categoryId))
        {
            if (transaction.Category is null)
            {
                continue;
            }

            transaction.Category.Name = newName;
        }
        return Task.FromResult(categoryId);
    }

    public Task<Guid> Delete(Guid transactionId)
    {
        _transactions.Remove(transactionId);
        return Task.FromResult(transactionId);
    }

    public Task DeleteByProfileId(Guid profileId)
    {
        var toRemove = _transactions.Values.Where(t => t.ProfileId == profileId).Select(t => t.Id).ToList();
        foreach (var id in toRemove)
        {
            _transactions.Remove(id);
        }
        return Task.CompletedTask;
    }

    public Task<List<Transaction>> GetRecurringTransactions(Guid profileId)
    {
        return Task.FromResult(_transactions.Values
            .Where(t => t.ProfileId == profileId && t.RecurringTransactionInfo is not null &&
                        t.RecurringTransactionInfo.Interval > 0)
            .ToList());
    }
}
