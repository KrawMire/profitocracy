using Profitocracy.Core.Domain.Model.Profiles;

namespace Profitocracy.Core.Persistence;

/// <summary>
/// Provides operations for working with
/// persistence layer for profiles
/// </summary>
public interface IProfileRepository
{
	/// <summary>
	/// Get current active profile ID
	/// </summary>
	/// <returns>ID of the current profile if exists, otherwise, null</returns>
	Task<Guid?> GetCurrentProfileId();

	/// <summary>
	/// Retrieve a list of all profiles.
	/// </summary>
	/// <returns>A list containing all profiles.</returns>
	Task<List<Profile>> GetAllProfiles();

	/// <summary>
	/// Retrieves a profile by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the profile to retrieve.</param>
	/// <returns>The profile with the specified identifier if found; otherwise, null.</returns>
	Task<Profile?> GetProfileById(Guid id);

	/// <summary>
	/// Get current active profile
	/// </summary>
	/// <returns>Current profile if exists, otherwise, null</returns>
	Task<Profile?> GetCurrentProfile();
	
	/// <summary>
	/// Create new profile
	/// </summary>
	/// <param name="profile">New profile to create</param>
	/// <returns>Created profile</returns>
	Task<Profile> Create(Profile profile);
	
	/// <summary>
	/// Update profile
	/// </summary>
	/// <param name="profile">Profile to update</param>
	/// <returns>Updated profile</returns>
	Task<Profile> Update(Profile profile);

	/// <summary>
	/// Deletes a profile by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the profile to delete.</param>
	/// <returns>The identifier of the deleted profile.</returns>
	Task<Guid> Delete(Guid id);
}