using Microsoft.Extensions.DependencyInjection;
using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Tests.Mocks.Factories;
using Profitocracy.Core.Tests.Mocks.Persistence;

namespace Profitocracy.Core.Tests.Domain.Services;

public class TransactionServiceTests
{
    private readonly ITransactionService _transactionService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProfileRepository _profileRepository;

    public TransactionServiceTests()
    {
        _profileRepository = new MockProfileRepository();
        var categoryRepository = new MockCategoryRepository();
        var settingsRepository = new MockSettingsRepository();
        _transactionRepository = new MockTransactionRepository();

        var provider = new ServiceCollection()
            .RegisterCoreServices()
            .AddSingleton(_profileRepository)
            .AddSingleton<ICategoryRepository>(categoryRepository)
            .AddSingleton<ISettingsRepository>(settingsRepository)
            .AddSingleton(_transactionRepository)
            .BuildServiceProvider();

        _transactionService = provider.GetRequiredService<ITransactionService>();
    }

    [Fact]
    public async Task CheckTransactionInCurrentPeriod_ShouldReturnTrue()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        var transaction = MockEntityFactory.CreateMockTransaction(profile.Id, 100);
        await _transactionRepository.Create(transaction);
        var result = await _transactionService.CheckTransactionInCurrentPeriod(transaction.Id);

        Assert.True(result);
    }
}
