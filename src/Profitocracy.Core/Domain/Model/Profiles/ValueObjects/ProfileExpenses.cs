using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Profiles.ValueObjects;

/// <summary>
/// Set of profile expenses.
/// Contains types (main, secondary, saved) and daily amounts
/// </summary>
public class ProfileExpenses : ValueObject
{
    /// <summary>
    /// Daily amount for tomorrow
    /// </summary>
    public required decimal TomorrowBalance { get; set; }

    /// <summary>
    /// Total amount of expenses
    /// </summary>
    public required ProfileExpense TotalBalance { get; init; }

    /// <summary>
    /// Daily amount for today
    /// </summary>
    public required ProfileExpense TodayBalance { get; init; }

    /// <summary>
    /// Main expenses amount
    /// </summary>
    public required ProfileExpense Main { get; init; }

    /// <summary>
    /// Secondary expenses amount
    /// </summary>
    public required ProfileExpense Secondary { get; init; }

    /// <summary>
    /// Saved amount
    /// </summary>
    public required ProfileExpense Saved { get; init; }
}