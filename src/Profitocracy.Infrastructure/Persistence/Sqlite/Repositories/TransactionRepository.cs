using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Specifications;
using Profitocracy.Infrastructure.Abstractions.Internal;
using Profitocracy.Infrastructure.Persistence.Sqlite.Configuration;
using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Transaction;

namespace Profitocracy.Infrastructure.Persistence.Sqlite.Repositories;

internal class TransactionRepository : ITransactionRepository
{
    private readonly DbConnection _dbConnection;
    private readonly IInfrastructureMapper<Transaction, TransactionModel> _mapper;

    public TransactionRepository(
        DbConnection dbConnection,
        IInfrastructureMapper<Transaction, TransactionModel> mapper)
    {
        _dbConnection = dbConnection;
        _mapper = mapper;
    }

    public async Task<List<Transaction>> GetAllByProfileId(Guid profileId)
    {
        var transactions = await GetAllByProfileIdInternal(profileId);

        var domainTransactions = transactions
            .Select(_mapper.MapToDomain)
            .ToList();

        return domainTransactions;
    }

    public async Task<Transaction?> GetById(Guid transactionId)
    {
        var transaction = await GetByIdInternal(transactionId);

        return transaction is not null
            ? _mapper.MapToDomain(transaction)
            : null;
    }

    public async Task<List<Transaction>> GetForPeriod(Guid profileId, DateTime dateFrom, DateTime dateTo)
    {
        var transactions = await GetForPeriodInternal(profileId, dateFrom, dateTo);

        return transactions
            .Select(_mapper.MapToDomain)
            .ToList();
    }

    public async Task<List<Transaction>> GetFiltered(TransactionsSpecification spec)
    {
        var transactions = await GetFilteredInternal(spec);

        return transactions
            .Select(_mapper.MapToDomain)
            .ToList();
    }

    public async Task<Transaction> Create(Transaction transaction)
    {
        var transactionToCreate = _mapper.MapToModel(transaction);
        var createdTransaction = await CreateInternal(transactionToCreate);

        return _mapper.MapToDomain(createdTransaction);
    }

    internal async Task<TransactionModel> CreateInternal(TransactionModel transaction)
    {
        await _dbConnection.Init();
        await _dbConnection.Database.InsertAsync(transaction);

        return await _dbConnection.Database
            .Table<TransactionModel>()
            .Where(t => t.Id.Equals(transaction.Id))
            .FirstOrDefaultAsync();
    }

    public async Task<Transaction> Update(Transaction transaction)
    {
        await _dbConnection.Init();

        var transactionToUpdate = _mapper.MapToModel(transaction);
        await _dbConnection.Database.UpdateAsync(transactionToUpdate);

        var updatedTransaction = await _dbConnection.Database
            .Table<TransactionModel>()
            .Where(t => t.Id.Equals(transaction.Id))
            .FirstOrDefaultAsync();

        return _mapper.MapToDomain(updatedTransaction);
    }

    public async Task<Guid> ClearWithCategory(Guid categoryId)
    {
        await _dbConnection.Init();

        var transactionsToUpdate = await _dbConnection.Database
            .Table<TransactionModel>()
            .Where(t => t.CategoryId == categoryId)
            .ToListAsync();

        foreach (var transaction in transactionsToUpdate)
        {
            transaction.CategoryId = null;
            transaction.CategoryName = null;
        }

        await _dbConnection.Database.UpdateAllAsync(transactionsToUpdate);

        return categoryId;
    }

    public async Task<Guid> ChangeCategoryName(Guid categoryId, string newName)
    {
        await _dbConnection.Init();

        var transactionsToUpdate = await _dbConnection.Database
            .Table<TransactionModel>()
            .Where(t => t.CategoryId == categoryId)
            .ToListAsync();

        foreach (var transaction in transactionsToUpdate)
        {
            transaction.CategoryName = newName;
        }

        await _dbConnection.Database.UpdateAllAsync(transactionsToUpdate);

        return categoryId;
    }

    public async Task<Guid> Delete(Guid transactionId)
    {
        await _dbConnection.Init();

        await _dbConnection.Database
            .Table<TransactionModel>()
            .DeleteAsync(t => t.Id == transactionId);

        return transactionId;
    }

    public async Task DeleteByProfileId(Guid profileId)
    {
        await _dbConnection.Init();

        await _dbConnection.Database
            .Table<TransactionModel>()
            .DeleteAsync(t => t.ProfileId == profileId);
    }

    internal async Task<List<TransactionModel>> GetAllByProfileIdInternal(Guid profileId)
    {
        await _dbConnection.Init();

        var transactions = await _dbConnection.Database
            .Table<TransactionModel>()
            .Where(t => t.ProfileId.Equals(profileId))
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();

        return transactions ?? [];
    }

    internal async Task<TransactionModel?> GetByIdInternal(Guid transactionId)
    {
        await _dbConnection.Init();

        return await _dbConnection.Database
            .Table<TransactionModel>()
            .Where(t => t.Id.Equals(transactionId))
            .FirstOrDefaultAsync();
    }

    internal async Task<List<TransactionModel>> GetForPeriodInternal(Guid profileId, DateTime dateFrom, DateTime dateTo)
    {
        await _dbConnection.Init();

        var transactions = await _dbConnection.Database
            .Table<TransactionModel>()
            .Where(t => t.ProfileId == profileId)
            .Where(t => t.Timestamp >= dateFrom)
            .Where(t => t.Timestamp <= dateTo)
            .ToListAsync();

        return transactions ?? [];
    }

    internal async Task<List<TransactionModel>> GetFilteredInternal(TransactionsSpecification spec)
    {
        await _dbConnection.Init();

        var query = _dbConnection.Database
            .Table<TransactionModel>()
            .Where(t => t.ProfileId == spec.ProfileId);

        var isMultiCurrency = spec.IsMultiCurrency is not null && spec.IsMultiCurrency.Value;

        if (isMultiCurrency)
        {
            query = spec.CurrencyCode is not null
                ? query.Where(t => t.DestinationCurrencyCode == spec.CurrencyCode)
                : query.Where(t => t.DestinationCurrencyCode != null);

            if (spec.Destination is not null)
            {
                query = query.Where(t => t.Destination == (short)spec.Destination);
            }
        }

        if (spec.SpendingType is not null)
        {
            query = query.Where(t => t.SpendingType.Equals(spec.SpendingType));
        }

        if (spec.CategoryId is not null)
        {
            query = query.Where(t => t.CategoryId == spec.CategoryId);
        }

        if (spec.FromDate is not null)
        {
            query = query.Where(t => t.Timestamp >= spec.FromDate);
        }

        if (spec.ToDate is not null)
        {
            query = query.Where(t => t.Timestamp <= spec.ToDate);
        }

        if (spec.TransactionType is not null)
        {
            query = query.Where(t => t.Type == (short)spec.TransactionType);
        }

        if (!string.IsNullOrWhiteSpace(spec.Description))
        {
            var lowerDesc = spec.Description.ToLower();

            // Not using string.Contains(string, StringComparison)
            // because it causes an error in SQLite for some reason
            query = query.Where(t => t.Description != null
                                     && t.Description
                                         .ToLower()
                                         .Contains(lowerDesc));
        }

        if (spec.Amount is not null)
        {
            query = spec.IsGreaterThanAmount is not null && spec.IsGreaterThanAmount.Value
                ? query.Where(t => t.Amount >= spec.Amount)
                : query.Where(t => t.Amount <= spec.Amount);
        }

        query = query.OrderByDescending(t => t.Timestamp);

        var transactions = await query.ToListAsync();

        return transactions ?? [];
    }
}
