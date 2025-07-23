using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;

namespace Profitocracy.Mobile.ViewModels.Home;

public class NewPeriodSelectionPageViewModel : BaseNotifyObject
{
    private DateTime _dateTo;

    private readonly IProfileService _profileService;
    private readonly IProfileRepository _profileRepository;

    public NewPeriodSelectionPageViewModel(
        IProfileService profileService,
        IProfileRepository profileRepository)
    {
        _profileService = profileService;
        _profileRepository = profileRepository;

        _dateTo = DateTime.Now;
    }

    public DateTime DateTo
    {
        get => _dateTo;
        set => SetProperty(ref _dateTo, value);
    }

    public async Task StartNewPeriodAsync()
    {
        var profileId = await _profileRepository.GetCurrentProfileId();

        if (profileId is null)
        {
            throw new Exception(AppResources.CommonError_GetCurrentProfile);
        }

        await _profileService.StartNewProfilePeriod((Guid)profileId, DateTime.Now, DateTo);
    }
}
