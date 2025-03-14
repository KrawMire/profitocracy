using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Mobile.Resources.Strings;

namespace Profitocracy.Mobile.Services;

public class CurrencyService
{
    public static readonly Dictionary<string, string> CurrencyNames = new();

    static CurrencyService()
    {
        foreach (var currency in Currency.AvailableCurrencies.All.Values)
        {
            var resourceName = $"Currencies_{currency.Code}";
            var currencyName = AppResources.ResourceManager.GetString(resourceName, AppResources.Culture);

            if (string.IsNullOrWhiteSpace(currencyName))
            {
                continue;   
            }
            
            CurrencyNames.Add(currency.Code, currencyName);
        }
    }
}