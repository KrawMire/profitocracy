using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Persistence;
using Profitocracy.Infrastructure.Abstractions.Internal;
using Profitocracy.Infrastructure.Persistence.Sqlite.Configuration;
using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Profile;

namespace Profitocracy.Infrastructure.Persistence.Sqlite.Repositories;

internal class ProfileRepository : IProfileRepository
{
    private readonly DbConnection _dbConnection;
    private readonly IInfrastructureMapper<Profile, ProfileModel> _mapper;

    public ProfileRepository(
        DbConnection connection,
        IInfrastructureMapper<Profile, ProfileModel> mapper)
    {
        _dbConnection = connection;
        _mapper = mapper;
    }

    public async Task<Profile> Create(Profile profile)
    {
        await _dbConnection.Init();

        var profileToCreate = _mapper.MapToModel(profile);
        _ = await _dbConnection.Database.InsertAsync(profileToCreate);

        var createdProfile = await _dbConnection.Database
            .Table<ProfileModel>()
            .Where(p => p.Id.Equals(profile.Id))
            .FirstAsync();

        return _mapper.MapToDomain(createdProfile);
    }

    public async Task<Profile> Update(Profile profile)
    {
        await _dbConnection.Init();

        var profileToUpdate = _mapper.MapToModel(profile);
        _ = await _dbConnection.Database.UpdateAsync(profileToUpdate);

        var updatedProfile = await _dbConnection.Database
            .Table<ProfileModel>()
            .Where(p => p.Id.Equals(profile.Id))
            .FirstAsync();

        return _mapper.MapToDomain(updatedProfile);
    }

    public async Task<Guid> Delete(Guid id)
    {
        await _dbConnection.Init();

        await _dbConnection.Database
            .Table<ProfileModel>()
            .DeleteAsync(p => p.Id == id);

        return id;
    }

    public async Task<Guid?> GetCurrentProfileId()
    {
        await _dbConnection.Init();

        var profile = await _dbConnection.Database
            .Table<ProfileModel>()
            .Where(p => p.IsCurrent)
            .FirstOrDefaultAsync();

        return profile?.Id;
    }

    public async Task<List<Profile>> GetAllProfiles()
    {
        await _dbConnection.Init();

        var profiles = await _dbConnection.Database
            .Table<ProfileModel>()
            .ToListAsync();

        return profiles
            .Select(_mapper.MapToDomain)
            .ToList();
    }

    public async Task<Profile?> GetProfileById(Guid id)
    {
        await _dbConnection.Init();

        var profile = await _dbConnection.Database
            .Table<ProfileModel>()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();

        return profile is null
            ? null
            : _mapper.MapToDomain(profile);
    }

    public async Task<Profile?> GetCurrentProfile()
    {
        await _dbConnection.Init();

        var profile = await _dbConnection.Database
            .Table<ProfileModel>()
            .Where(p => p.IsCurrent)
            .FirstOrDefaultAsync();

        return profile is null
            ? null
            : _mapper.MapToDomain(profile);
    }
}