namespace Profitocracy.Core.Domain.Model.Summaries.ValueObjects;

public record WeeklyExpense
{
    public DateTime DateFrom { get; init; }
    public DateTime DateTo { get; init; }
    public decimal Amount { get; internal set; }
}
