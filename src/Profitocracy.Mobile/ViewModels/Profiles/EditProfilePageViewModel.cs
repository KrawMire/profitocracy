using System.Collections.ObjectModel;
using System.Globalization;
using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Domain.Model.Profiles.Factories;
using Profitocracy.Core.Domain.Model.Profiles.ValueObjects;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;

namespace Profitocracy.Mobile.ViewModels.Profiles;

public class EditProfilePageViewModel : BaseNotifyObject
{
    private Guid? _profileId;
    private string _name = "";
    private string _initialBalance = "0";
    private Currency _currency;
    private bool _isCurrent;
    private bool _isNotFirstProfile = true;
    private TimePeriod? _billingPeriod;

    private readonly IProfileRepository _profileRepository;

    public EditProfilePageViewModel(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
        _isCurrent = false;

        foreach (var currency in Currency.AvailableCurrencies.All.Values)
        {
            AvailableCurrencies.Add(currency);
        }

        _currency = AvailableCurrencies[0];
    }

    public Guid? ProfileId
    {
        set => _profileId = value;
    }

    public bool IsNotFirstProfile
    {
        get => _isNotFirstProfile;
        private set => SetProperty(ref _isNotFirstProfile, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string InitialBalance
    {
        get => _initialBalance;
        set => SetProperty(ref _initialBalance, value);
    }

    public Currency SelectedCurrency
    {
        get => _currency;
        set => SetProperty(ref _currency, value);
    }

    public ObservableCollection<Currency> AvailableCurrencies { get; } = [];

    public async Task Initialize()
    {
        var existingProfiles = await _profileRepository.GetAllProfiles();
        _isCurrent = !existingProfiles.Any(p => p.IsCurrent && p.Id != _profileId);

        IsNotFirstProfile = existingProfiles.Count != 0;
        SelectedCurrency = AvailableCurrencies[0];

        if (_profileId is not null)
        {
            var profile = await _profileRepository.GetProfileById((Guid)_profileId);

            if (profile is null)
            {
                throw new ArgumentNullException(nameof(_profileId), AppResources.CommonError_GetProfileById);
            }

            InitializeEditableProfile(profile);
        }
    }

    private void InitializeEditableProfile(Profile profile)
    {
        Name = profile.Name;
        InitialBalance = profile.Balance.ToString(CultureInfo.CurrentCulture);
        SelectedCurrency = profile.Settings.Currency;
        _billingPeriod = profile.BillingPeriod;
    }

    public async Task CreateProfile()
    {
        if (!NumberUtils.TryParseDecimal(_initialBalance, out var numValue))
        {
            throw new InvalidCastException(AppResources.CommonError_BalanceNumber);
        }

        var profileBuilder = new ProfileBuilder(_profileId)
            .AddStartDate(DateTime.Now, numValue)
            .AddName(_name)
            .AddCurrency(_currency)
            .AddIsCurrent(_isCurrent);

        if (_billingPeriod is not null)
        {
            profileBuilder.AddBillingPeriod(_billingPeriod.DateFrom, _billingPeriod.DateTo);
        }

        if (_profileId is not null)
        {
            await _profileRepository.Update(profileBuilder.Build());
        }
        else
        {
            await _profileRepository.Create(profileBuilder.Build());
        }
    }
}