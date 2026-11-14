using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Mobile.Services.Static;

namespace Profitocracy.Mobile.Models.Profiles;

public class ProfileModel
{
    public Guid? Id { get; set; }
    public required string Name { get; init; }
    public required decimal Balance { get; init; }
    public required string CurrencySymbol { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public string CurrencyName { get; init; } = string.Empty;
    public bool IsCurrent { get; set; }

    public bool IsNotCurrent => !IsCurrent;

    /// <summary>
    /// Text of the currency tile: the symbol, or the code when the symbol is not distinctive.
    /// </summary>
    public string CurrencyBadge => CurrencySymbol.Length <= 2 ? CurrencySymbol : CurrencyCode;

    public bool IsCodeBadge => CurrencySymbol.Length > 2;

    /// <summary>
    /// Line under the name of the profile, can be overridden with more details.
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    public static ProfileModel FromDomain(Profile profile)
    {
        var currency = profile.Settings.Currency;

        return new ProfileModel
        {
            Id = profile.Id,
            Name = profile.Name,
            Balance = profile.Balance,
            CurrencySymbol = currency.Symbol,
            CurrencyCode = currency.Code,
            CurrencyName = CurrencyService.CurrencyNames.GetValueOrDefault(currency.Code, currency.Code),
            IsCurrent = profile.IsCurrent,
            Detail = CurrencyService.CurrencyNames.GetValueOrDefault(currency.Code, currency.Code),
        };
    }
}
