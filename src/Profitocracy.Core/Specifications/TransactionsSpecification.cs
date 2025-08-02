using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;

namespace Profitocracy.Core.Specifications;

/// <summary>
/// Represents the specification for filtering
/// transactions based on various criteria
/// </summary>
public record struct TransactionsSpecification
{
    public Guid? ProfileId { get; init; }
    public Guid? CategoryId { get; init; }
    public TransactionType? TransactionType { get; init; }
    public SpendingType? SpendingType { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public TransactionDestination? Destination { get; init; }
    public bool? IsMultiCurrency { get; init; }
    public string? Description { get; init; }
    public string? CurrencyCode { get; init; }
    public bool? IsGreaterThanAmount { get; init; }
    public decimal? Amount { get; init; }
    public RecurringTransactionInterval? Interval { get; init; }
}
