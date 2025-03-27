using System.Collections.ObjectModel;
using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Profiles;

namespace Profitocracy.Mobile.ViewModels.Profiles;

public class ProfileSettingsPageViewModel : BaseNotifyObject
{
    private readonly IProfileRepository _profileRepository;
    private readonly IProfileService _profileService;
    
    public ProfileSettingsPageViewModel(
        IProfileRepository profileRepository, 
        IProfileService profileService)
    {
        _profileRepository = profileRepository;
        _profileService = profileService;
    }

    public readonly ObservableCollection<ProfileModel> Profiles = [];

    public async Task Initialize()
    {
        var profiles = await _profileRepository.GetAllProfiles();
        
        Profiles.Clear();
        
        foreach (var profile in profiles.Where(profile => profile.IsCurrent))
        {
            Profiles.Add(ProfileModel.FromDomain(profile));
        }

        foreach (var profile in profiles.Where(profile => !profile.IsCurrent))
        {
            Profiles.Add(ProfileModel.FromDomain(profile));
        }
    }

    public async Task DeleteProfile(Guid profileId)
    {
        await _profileService.DeleteProfile(profileId);
        await Initialize();
    }

    public async Task SetCurrentProfile(Guid profileId)
    {
        await _profileService.SetCurrentProfile(profileId);
        await Initialize();
    }
}