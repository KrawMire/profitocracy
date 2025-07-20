using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions.Entities;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Exceptions;

namespace Profitocracy.Core.Domain.Model.Transactions;

/// <summary>
/// Representation of moving funds from one currency to another.
/// </summary>
public class MultiCurrencyTransaction : Transaction
{
    internal MultiCurrencyTransaction(
        Guid id,
        decimal amount,
        decimal destinationAmount,
        Currency sourceCurrency,
        Currency destinationCurrency,
        Guid profileId,
        TransactionType type,
        SpendingType? spendingType,
        TransactionDestination destination,
        DateTime timestamp,
        string? description,
        TransactionGeoTag? geoTag,
        TransactionCategory? category) :
        base(id, amount, profileId, type, spendingType, timestamp, description, geoTag, category)
    {
        SourceCurrency = sourceCurrency;
        DestinationCurrency = destinationCurrency;
        DestinationAmount = destinationAmount;
        Destination = destination;
    }

    public Currency SourceCurrency { get; }

    public Currency DestinationCurrency { get; }

    /// <summary>
    /// Destination of the transaction.
    /// </summary>
    public TransactionDestination Destination { get; }

    /// <summary>
    /// Amount of the transaction in destination currency.
    /// </summary>
    public decimal DestinationAmount { get; }

    /// <summary>
    /// The ratio between amount of transaction
    /// and destination currency amount. 
    /// </summary>
    public decimal Rate => Amount / DestinationAmount;
}