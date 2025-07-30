using Microsoft.Extensions.DependencyInjection;
using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Tests.Mocks.Factories;
using Profitocracy.Core.Tests.Mocks.Persistence;

namespace Profitocracy.Core.Tests.Domain.Services;

public class CalculationServiceTests
{
    private readonly MockProfileRepository _profileRepository;
    private readonly MockTransactionRepository _transactionRepository;
    private readonly ICalculationService _calculationService;

    public CalculationServiceTests()
    {
        _profileRepository = new MockProfileRepository();
        var categoryRepository = new MockCategoryRepository();
        var settingsRepository = new MockSettingsRepository();
        _transactionRepository = new MockTransactionRepository();

        var provider = new ServiceCollection()
            .RegisterCoreServices()
            .AddSingleton<IProfileRepository>(_profileRepository)
            .AddSingleton<ICategoryRepository>(categoryRepository)
            .AddSingleton<ISettingsRepository>(settingsRepository)
            .AddSingleton<ITransactionRepository>(_transactionRepository)
            .BuildServiceProvider();

        _calculationService = provider.GetRequiredService<ICalculationService>();
    }

    [Fact]
    public async Task GetCurrentProfile_ShouldCorrectCalculateProfile()
    {
        var profile = MockEntityFactory.CreateMockProfile();
        var transaction1 = MockEntityFactory.CreateMockTransaction(profile.Id, 100);
        var transaction2 = MockEntityFactory.CreateMockTransaction(profile.Id, 100);

        await _profileRepository.Create(profile);
        await _transactionRepository.Create(transaction1);
        await _transactionRepository.Create(transaction2);

        var result = await _calculationService.GetCurrentProfile();

        Assert.NotNull(result);
        Assert.Equal(800, result.Balance);
    }

    [Fact]
    public async Task GetSummaryForPeriod_ShouldCorrectCalculateSummary()
    {
        var profile = MockEntityFactory.CreateMockProfile();
        var transaction1 = MockEntityFactory.CreateMockTransaction(profile.Id, 100);
        var transaction2 = MockEntityFactory.CreateMockTransaction(profile.Id, 300);
        await _profileRepository.Create(profile);
        await _transactionRepository.Create(transaction1);
        await _transactionRepository.Create(transaction2);

        var result = await _calculationService.GetSummaryForPeriod(
            profile.BillingPeriod.DateFrom,
            profile.BillingPeriod.DateTo);

        Assert.NotNull(result);
        Assert.Equal(400, result.TotalExpenses);
    }
}
