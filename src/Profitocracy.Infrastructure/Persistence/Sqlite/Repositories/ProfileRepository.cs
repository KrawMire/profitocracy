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
        var profileToCreate = _mapper.MapToModel(profile);
        var createdProfile = await CreateInternal(profileToCreate);

        return _mapper.MapToDomain(createdProfile);
    }

    internal async Task<ProfileModel> CreateInternal(ProfileModel profile)
    {
        await _dbConnection.Init();
        await _dbConnection.Database.InsertAsync(profile);

        return await _dbConnection.Database
            .Table<ProfileModel>()
            .Where(p => p.Id.Equals(profile.Id))
            .FirstAsync();
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
        var profiles = await GetAllProfilesInternal();

        return profiles
            .Select(_mapper.MapToDomain)
            .ToList();
    }

    public async Task<Profile?> GetProfileById(Guid id)
    {
        var profile = await GetProfileByIdInternal(id);

        return profile is null
            ? null
            : _mapper.MapToDomain(profile);
    }

    public async Task<Profile?> GetCurrentProfile()
    {
        var profile = await GetCurrentProfileInternal();

        return profile is null
            ? null
            : _mapper.MapToDomain(profile);
    }

    internal async Task<List<ProfileModel>> GetAllProfilesInternal()
    {
        await _dbConnection.Init();

        var profiles = await _dbConnection.Database
            .Table<ProfileModel>()
            .ToListAsync();

        return profiles ?? [];
    }

    internal async Task<ProfileModel?> GetProfileByIdInternal(Guid id)
    {
        await _dbConnection.Init();

        return await _dbConnection.Database
            .Table<ProfileModel>()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    internal async Task<ProfileModel?> GetCurrentProfileInternal()
    {
        await _dbConnection.Init();

        return await _dbConnection.Database
            .Table<ProfileModel>()
            .Where(p => p.IsCurrent)
            .FirstOrDefaultAsync();
    }
}
