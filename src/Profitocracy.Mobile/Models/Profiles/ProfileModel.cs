using Profitocracy.Core.Domain.Model.Profiles;

namespace Profitocracy.Mobile.Models.Profiles;

public class ProfileModel
{
    public Guid? Id { get; set; }
    public required string Name { get; init; }
    public required decimal Balance { get; init; }
    public required string CurrencySymbol { get; init; }
    public bool IsCurrent { get; set; }

    public bool IsNotCurrent => !IsCurrent;

    public static ProfileModel FromDomain(Profile profile)
    {
        return new ProfileModel
        {
            Id = profile.Id,
            Name = profile.Name,
            Balance = profile.Balance,
            CurrencySymbol = profile.Settings.Currency.Symbol,
            IsCurrent = profile.IsCurrent
        };
    }
}