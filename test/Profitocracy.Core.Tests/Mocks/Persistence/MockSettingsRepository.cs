using Profitocracy.Core.Domain.Model.Settings;
using Profitocracy.Core.Persistence;

namespace Profitocracy.Core.Tests.Mocks.Persistence;

public class MockSettingsRepository : ISettingsRepository
{
    private Settings? _settings;

    public Task<Settings?> GetCurrentSettings()
    {
        return Task.FromResult(_settings);
    }

    public Task<Settings> CreateOrUpdate(Settings settings)
    {
        _settings = settings;
        return Task.FromResult(settings);
    }
}
