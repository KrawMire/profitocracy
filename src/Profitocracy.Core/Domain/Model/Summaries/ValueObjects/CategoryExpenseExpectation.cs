namespace Profitocracy.Core.Domain.Model.Summaries.ValueObjects;

public class CategoryExpenseExpectation
{
    public CategoryExpenseExpectation(string categoryName, decimal plannedAmount)
    {
        CategoryName = categoryName;
        PlannedAmount = plannedAmount;
        ActualAmount = 0;
    }

    public string CategoryName { get; }
    public decimal PlannedAmount { get; }
    public decimal ActualAmount { get; internal set; }
}