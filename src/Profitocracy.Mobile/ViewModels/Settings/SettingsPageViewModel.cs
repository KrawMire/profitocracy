using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Profiles;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;
using System.Collections.ObjectModel;

namespace Profitocracy.Mobile.ViewModels.Settings;

public class SettingsPageViewModel : BaseNotifyObject
{
    private readonly ICalculationService _calculationService;
    private readonly IProfileRepository _profileRepository;
    private readonly IProfileService _profileService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISettingsRepository _settingsRepository;

    private ProfileModel? _currentProfile;
    private string _categoriesSummary = string.Empty;
    private string _appearance = string.Empty;

    public SettingsPageViewModel(
        ICalculationService calculationService,
        IProfileRepository profileRepository,
        IProfileService profileService,
        ICategoryRepository categoryRepository,
        ISettingsRepository settingsRepository)
    {
        _calculationService = calculationService;
        _profileRepository = profileRepository;
        _profileService = profileService;
        _categoryRepository = categoryRepository;
        _settingsRepository = settingsRepository;
    }

    public ObservableCollection<ProfileModel> OtherProfiles { get; } = [];

    public ProfileModel? CurrentProfile
    {
        get => _currentProfile;
        private set => SetProperty(ref _currentProfile, value);
    }

    /// <summary>
    /// Number of categories and the sum of their limits, e.g. "6 · €1,975 a month".
    /// </summary>
    public string CategoriesSummary
    {
        get => _categoriesSummary;
        private set => SetProperty(ref _categoriesSummary, value);
    }

    public string Appearance
    {
        get => _appearance;
        private set => SetProperty(ref _appearance, value);
    }

    public bool HasOtherProfiles => OtherProfiles.Count > 0;

    public async Task Initialize()
    {
        var current = await _calculationService.GetCurrentProfile();
        var profiles = await _profileRepository.GetAllProfiles();

        if (current is not null)
        {
            var currentModel = ProfileModel.FromDomain(current);
            currentModel.Detail = string.Format(
                AppResources.Settings_ProfileLeft,
                currentModel.CurrencyName,
                NumberUtils.FormatMoney(current.Balance, currentModel.CurrencySymbol));

            CurrentProfile = currentModel;

            await InitializeCategoriesSummary(current.Id, currentModel.CurrencySymbol);
        }

        OtherProfiles.Clear();

        foreach (var profile in profiles.Where(p => p.Id != current?.Id))
        {
            OtherProfiles.Add(ProfileModel.FromDomain(profile));
        }

        OnPropertyChanged(nameof(HasOtherProfiles));

        var settings = await _settingsRepository.GetCurrentSettings();

        Appearance = settings?.Theme switch
        {
            Theme.Light => AppResources.ThemeSettings_LightTheme,
            Theme.Dark => AppResources.ThemeSettings_Dark,
            _ => AppResources.ThemeSettings_System,
        };
    }

    public async Task SetCurrentProfile(Guid profileId)
    {
        await _profileService.SetCurrentProfile(profileId);
        await Initialize();
    }

    public async Task DeleteProfile(Guid profileId)
    {
        await _profileService.DeleteProfile(profileId);
        await Initialize();
    }

    private async Task InitializeCategoriesSummary(Guid profileId, string symbol)
    {
        var categories = await _categoryRepository.GetAllByProfileId(profileId);
        var limits = categories.Sum(c => c.PlannedAmount ?? 0);

        CategoriesSummary = categories.Count == 0
            ? string.Empty
            : string.Format(
                AppResources.Settings_CategoriesSummary,
                categories.Count,
                NumberUtils.FormatMoneyCompact(limits, symbol));
    }
}
