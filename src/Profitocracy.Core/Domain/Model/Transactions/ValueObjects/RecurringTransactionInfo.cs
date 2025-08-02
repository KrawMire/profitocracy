namespace Profitocracy.Core.Domain.Model.Transactions.ValueObjects;

public class RecurringTransactionInfo
{
    public required RecurringTransactionInterval Interval { get; set; }
    public DateTime? LastMaturityDate { get; set; }
}
