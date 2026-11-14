namespace Profitocracy.Mobile.Models.Categories;

/// <summary>
/// Category with its monthly limit and spending of the current period.
/// </summary>
public class CategorySettingsModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }

    /// <summary>
    /// Monthly limit, e.g. "€1,000", or "No limit".
    /// </summary>
    public required string Limit { get; init; }

    /// <summary>
    /// Caption under the limit: "a month" or a hint to set one.
    /// </summary>
    public required string LimitCaption { get; init; }

    /// <summary>
    /// Spending of the current period, e.g. "€63.70 spent this month".
    /// </summary>
    public required string Spent { get; init; }

    public required bool HasLimit { get; init; }
    public required bool IsOver { get; init; }

    public bool HasNoLimit => !HasLimit;
}
