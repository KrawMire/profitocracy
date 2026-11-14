namespace Profitocracy.Mobile.Models.Categories;

/// <summary>
/// Spending of a category compared to its limit on a shared scale.
/// </summary>
public class CategoryLimitModel
{
    public required string Name { get; init; }
    public required string Actual { get; init; }

    /// <summary>
    /// Limit next to the spent amount, e.g. "of €300" or "no limit".
    /// </summary>
    public required string Limit { get; init; }

    /// <summary>
    /// Spent amount on the shared scale of all the categories, from 0 to 1.
    /// </summary>
    public required double Progress { get; init; }

    /// <summary>
    /// Position of the limit on the shared scale, negative when there is no limit.
    /// </summary>
    public required double Mark { get; init; }

    public required bool IsOver { get; init; }
}
