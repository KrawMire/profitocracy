using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Mobile.Services.Static;
using Profitocracy.Mobile.Utils;

namespace Profitocracy.Mobile.Models.Profiles;

public class SavedAmountModel
{
    public required string CurrencyName { get; set; }
    public required string Amount { get; set; }

    public static SavedAmountModel FromDomain(KeyValuePair<Currency, decimal> savedAmount)
    {
        return new SavedAmountModel
        {
            CurrencyName = CurrencyService.CurrencyNames.GetValueOrDefault(savedAmount.Key.Code, savedAmount.Key.Code),
            Amount = NumberUtils.FormatMoney(savedAmount.Value, savedAmount.Key.Symbol),
        };
    }
}
