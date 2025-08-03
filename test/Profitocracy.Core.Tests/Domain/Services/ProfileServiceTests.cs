using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Tests.Mocks.Factories;
using Profitocracy.Core.Tests.Mocks.Persistence;

namespace Profitocracy.Core.Tests.Domain.Services;

public class ProfileServiceTests
{
    private readonly IProfileService _profileService;
    private readonly MockProfileRepository _profileRepository;

    public ProfileServiceTests()
    {
        _profileRepository = new MockProfileRepository();
        var categoryRepository = new MockCategoryRepository();
        var settingsRepository = new MockSettingsRepository();
        var transactionRepository = new MockTransactionRepository();

        var provider = new ServiceCollection()
            .RegisterCoreServices()
            .AddSingleton<IProfileRepository>(_profileRepository)
            .AddSingleton<ICategoryRepository>(categoryRepository)
            .AddSingleton<ISettingsRepository>(settingsRepository)
            .AddSingleton<ITransactionRepository>(transactionRepository)
            .BuildServiceProvider();

        _profileService = provider.GetRequiredService<IProfileService>();
    }

    [Fact]
    public async Task SetCurrentProfile_ShouldReturnProfile()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        await _profileRepository.Create(profile);
        var result = await _profileService.SetCurrentProfile(profile.Id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(profile);
    }

    [Fact]
    public async Task StartNewProfilePeriod_ShouldReturnProfile()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());

        var newStart = DateTime.Now.AddMonths(-1).Date;
        var newEnd = DateTime.Now.Date;
        var result = await _profileService.StartNewProfilePeriod(profile.Id, newStart, newEnd);

        result.Should().NotBeNull();
        result.BillingPeriod.DateFrom.Date.Should().Be(newStart);
        result.BillingPeriod.DateTo.Date.Should().Be(newEnd);
    }

    [Fact]
    public async Task DeleteProfile_ShouldReturnProfileId()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        await _profileRepository.Create(MockEntityFactory.CreateMockProfile(false));

        var result = await _profileService.DeleteProfile(profile.Id);

        Assert.Equal(result, profile.Id);
    }
}
