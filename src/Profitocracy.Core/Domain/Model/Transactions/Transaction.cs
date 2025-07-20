using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions.Entities;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Domain.SharedKernel;
using Profitocracy.Core.Exceptions;

namespace Profitocracy.Core.Domain.Model.Transactions;

/// <summary>
/// Basic representation of moving funds.
/// </summary>
public class Transaction : AggregateRoot<Guid>
{
    internal Transaction(
        Guid id,
        decimal amount,
        Guid profileId,
        TransactionType type,
        SpendingType? spendingType,
        DateTime timestamp,
        string? description,
        TransactionGeoTag? geoTag,
        TransactionCategory? category) : base(id)
    {
        if (type == TransactionType.Expense && spendingType is null)
        {
            throw new InvalidTransactionSpendingType("If transaction is expense then spendingType should be specified");
        }

        Amount = amount;
        ProfileId = profileId;
        Timestamp = timestamp;
        Description = description;
        GeoTag = geoTag;
        Category = category;
        Type = type;
        SpendingType = spendingType;
    }

    public decimal Amount { get; set; }
    public Guid ProfileId { get; set; }
    public TransactionType Type { get; set; }
    public SpendingType? SpendingType { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }
    public TransactionGeoTag? GeoTag { get; set; }
    public TransactionCategory? Category { get; set; }
}