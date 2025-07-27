using Profitocracy.Core.Domain.Model.Profiles;

namespace Profitocracy.Core.Domain.Abstractions.Services;

public interface IProfileService
{
    /// <summary>
    /// Sets the provided profile as the
    /// current profile in the application.
    /// </summary>
    /// <param name="profileId">The ID of the profile to be set as the current profile.</param>
    /// <returns>Returns the updated profile that is now marked as the current profile.</returns>
    Task<Profile> SetCurrentProfile(Guid profileId);

    /// <summary>
    /// Starts a new period of a profile.
    /// Start date is taken from the current date,
    /// end date is specified via an argument.
    /// </summary>
    /// <param name="profileId">ID of the profile to start new period for.</param>
    /// <param name="currentDate">Current date.</param>
    /// <param name="endDate">Period end date.</param>
    /// <returns>Profile with updated period.</returns>
    Task<Profile> StartNewProfilePeriod(Guid profileId, DateTime currentDate, DateTime endDate);

    /// <summary>
    /// Deletes the profile with the specified profile ID.
    /// </summary>
    /// <param name="profileId">The ID of the profile to be deleted.</param>
    /// <returns>Returns the GUID of the deleted profile.</returns>
    Task<Guid> DeleteProfile(Guid profileId);
}