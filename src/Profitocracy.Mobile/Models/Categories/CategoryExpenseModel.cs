using Profitocracy.Core.Domain.Model.Profiles.Entities;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;

namespace Profitocracy.Mobile.Models.Categories;

/// <summary>
/// Spending of a category in the current period, prepared for display.
/// </summary>
public class CategoryExpenseModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required decimal ActualAmount { get; init; }
    public decimal? PlannedAmount { get; init; }

    /// <summary>
    /// Amount on the right: what is left of the limit, the overspend or "No limit".
    /// </summary>
    public required string Value { get; init; }

    /// <summary>
    /// Spent amount compared to the limit, e.g. "€25.98 of €100.00".
    /// </summary>
    public required string Detail { get; init; }

    /// <summary>
    /// Word under the value: "left", "over" or nothing for categories without limit.
    /// </summary>
    public required string Caption { get; init; }

    public bool HasLimit => PlannedAmount is > 0;
    public bool HasNoLimit => !HasLimit;
    public bool IsOver => HasLimit && ActualAmount > PlannedAmount;
    public double Ratio => HasLimit ? (double)(ActualAmount / PlannedAmount!.Value) : 0;

    public static CategoryExpenseModel FromDomain(ProfileCategory category, string currencySymbol)
    {
        var actual = category.ActualAmount;
        var planned = category.PlannedAmount;

        if (planned is null or 0)
        {
            return new CategoryExpenseModel
            {
                Id = category.Id,
                Name = category.Name,
                ActualAmount = actual,
                Value = AppResources.Common_NoLimit,
                Detail = string.Format(AppResources.Home_CategorySpent, NumberUtils.FormatMoney(actual, currencySymbol)),
                Caption = string.Empty,
            };
        }

        var isOver = actual > planned;

        return new CategoryExpenseModel
        {
            Id = category.Id,
            Name = category.Name,
            ActualAmount = actual,
            PlannedAmount = planned,
            Value = NumberUtils.FormatMoney(Math.Abs(planned.Value - actual), currencySymbol),
            Detail = string.Format(
                AppResources.Home_CategoryOf,
                NumberUtils.FormatMoney(actual, currencySymbol),
                NumberUtils.FormatMoney(planned.Value, currencySymbol)),
            Caption = isOver ? AppResources.Common_Over : AppResources.Common_Left,
        };
    }
}
