namespace Profitocracy.Core.Domain.Model.Summaries.ValueObjects;

public class CategoryExpense
{
    public CategoryExpense(string categoryName)
    {
        CategoryName = categoryName;
        Amount = 0;
    }

    public string CategoryName { get; init; }
    public decimal Amount { get; internal set; }
}
