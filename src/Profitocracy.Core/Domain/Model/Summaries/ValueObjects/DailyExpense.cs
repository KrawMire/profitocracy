namespace Profitocracy.Core.Domain.Model.Summaries.ValueObjects;

public class DailyExpense
{
    public DateTime Date { get; init; }
    public decimal Amount { get; internal set; }
}
