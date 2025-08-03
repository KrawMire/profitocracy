using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Persistence;

namespace Profitocracy.Core.Tests.Mocks.Persistence;

public class MockProfileRepository : IProfileRepository
{
    private readonly List<Profile> _profiles = [];

    public Task<Guid?> GetCurrentProfileId()
    {
        return Task.FromResult(_profiles.FirstOrDefault()?.Id);
    }

    public Task<List<Profile>> GetAllProfiles()
    {
        return Task.FromResult(_profiles.ToList());
    }

    public Task<Profile?> GetProfileById(Guid id)
    {
        return Task.FromResult(_profiles.FirstOrDefault(p => p.Id == id));
    }

    public Task<Profile?> GetCurrentProfile()
    {
        return Task.FromResult(_profiles.FirstOrDefault());
    }

    public Task<Profile> Create(Profile profile)
    {
        _profiles.Add(profile);
        return Task.FromResult(profile);
    }

    public Task<Profile> Update(Profile profile)
    {
        var existing = _profiles.FirstOrDefault(p => p.Id == profile.Id);
        if (existing != null)
        {
            _profiles.Remove(existing);
            _profiles.Add(profile);
        }
        return Task.FromResult(profile);
    }

    public Task<Guid> Delete(Guid id)
    {
        var profile = _profiles.FirstOrDefault(p => p.Id == id);
        if (profile != null)
        {
            _profiles.Remove(profile);
        }
        return Task.FromResult(id);
    }
}
