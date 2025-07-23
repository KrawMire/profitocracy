using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Exceptions;
using Profitocracy.Core.Persistence;

namespace Profitocracy.Core.Domain.Services;

internal class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProfileService(
        IProfileRepository profileRepository,
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository)
    {
        _profileRepository = profileRepository;
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    /// <inheritdoc />
    public async Task<Profile> SetCurrentProfile(Guid profileId)
    {
        var profiles = await _profileRepository.GetAllProfiles();

        foreach (var profile in profiles)
        {
            var isCurrent = profile.Id == profileId;

            if (isCurrent == profile.IsCurrent)
            {
                continue;
            }

            profile.IsCurrent = isCurrent;
            await _profileRepository.Update(profile);
        }

        var currentProfile = await _profileRepository.GetCurrentProfile();

        if (currentProfile is null)
        {
            throw new NullReferenceException("An error occurred while setting current profile.");
        }

        return currentProfile;
    }

    /// <inheritdoc />
    public async Task<Profile> StartNewProfilePeriod(Guid profileId, DateTime currentDate, DateTime endDate)
    {
        var profile = await _profileRepository.GetProfileById(profileId);

        if (profile is null)
        {
            throw new InvalidOperationException("Current profile was not found");
        }

        var startPeriodDate = new DateTime(currentDate.Year, currentDate.Month, 1);
        var endPeriodDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

        endPeriodDate = endPeriodDate.Add(DateTime.MaxValue.TimeOfDay);

        profile.StartNewBillingPeriod(startPeriodDate, endPeriodDate);

        return await _profileRepository.Update(profile);
    }

    /// <inheritdoc />
    public async Task<Guid> DeleteProfile(Guid profileId)
    {
        var profiles = await _profileRepository.GetAllProfiles();

        if (profiles.Count == 1)
        {
            throw new LastProfileDeletionException(profiles[0].Name);
        }

        var profileToDelete = await _profileRepository.GetProfileById(profileId);

        if (profileToDelete is null)
        {
            return profileId;
        }

        var updateCurrent = profileToDelete.IsCurrent;

        var tasks = new[]
        {
            _transactionRepository.DeleteByProfileId(profileId),
            _categoryRepository.DeleteByProfileId(profileId),
            _profileRepository.Delete(profileId)
        };

        await Task.WhenAll(tasks);

        if (!updateCurrent)
        {
            return profileId;
        }

        var nextCurrent = profiles.First(p => p.Id != profileId && !p.IsCurrent);
        await SetCurrentProfile(nextCurrent.Id);

        return profileId;
    }
}