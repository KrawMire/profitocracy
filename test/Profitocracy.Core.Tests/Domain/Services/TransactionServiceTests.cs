using Microsoft.Extensions.DependencyInjection;
using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
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
    
    [Fact]
    public async Task CheckRecurringTransactionIsNotExecutedWithNull_ShouldNotCreateTransactions()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        // "null" is the same as the "None" interval.
        var recurringTransaction = MockEntityFactory.CreateMockRecurringTransaction(profile.Id, 100,
            DateTime.Now.AddDays(-4), null);
        await _transactionRepository.Create(recurringTransaction);
        var createdTransactionsForRecurred = await _transactionService.CreateTransactionsForRecurred();

        Assert.Empty(createdTransactionsForRecurred);
    }
    
    [Fact]
    public async Task CheckRecurringTransactionIsNotExecutedWithNone_ShouldNotCreateTransactions()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        // By default, "None" is returned if a user has not selected an interval or has selected an interval, e.g.,
        // "Daily", and then changed the selection to "None".
        var recurringTransaction = MockEntityFactory.CreateMockRecurringTransaction(profile.Id, 100,
            DateTime.Now.AddDays(-4), RecurringTransactionInterval.None);
        await _transactionRepository.Create(recurringTransaction);
        var createdTransactionsForRecurred = await _transactionService.CreateTransactionsForRecurred();

        Assert.Empty(createdTransactionsForRecurred);
    }
    
    [Fact]
    public async Task CheckRecurringTransactionIsExecutedDaily_ShouldCreateTransactionsForLastFourDaysAndToday()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        var recurringTransaction = MockEntityFactory.CreateMockRecurringTransaction(profile.Id, 100,
            DateTime.Now.AddDays(-4), RecurringTransactionInterval.Daily);
        await _transactionRepository.Create(recurringTransaction);
        var createdTransactionsForRecurred = await _transactionService.CreateTransactionsForRecurred();

        Assert.Equal(5, createdTransactionsForRecurred.Count);
    }
    
    [Fact]
    public async Task CheckRecurringTransactionIsExecutedWeekly_ShouldCreateTransactionsForLastThreeWeeksAndToday()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        var recurringTransaction = MockEntityFactory.CreateMockRecurringTransaction(profile.Id, 100,
            DateTime.Now.AddDays(-21), RecurringTransactionInterval.Weekly);
        await _transactionRepository.Create(recurringTransaction);
        var createdTransactionsForRecurred = await _transactionService.CreateTransactionsForRecurred();

        Assert.Equal(4, createdTransactionsForRecurred.Count);
    }
    
    [Fact]
    public async Task CheckRecurringTransactionIsExecutedMonthly_ShouldCreateTransactionsForLastTwoMonthsAndToday()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        var recurringTransaction = MockEntityFactory.CreateMockRecurringTransaction(profile.Id, 100,
            DateTime.Now.AddMonths(-2), RecurringTransactionInterval.Monthly);
        await _transactionRepository.Create(recurringTransaction);
        var createdTransactionsForRecurred = await _transactionService.CreateTransactionsForRecurred();

        Assert.Equal(3, createdTransactionsForRecurred.Count);
    }
    
    [Fact]
    public async Task CheckRecurringTransactionIsExecutedQuarterly_ShouldCreateTransactionsForLastQuarterYearAndToday()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        var recurringTransaction = MockEntityFactory.CreateMockRecurringTransaction(profile.Id, 100,
            DateTime.Now.AddMonths(-3), RecurringTransactionInterval.Quarterly);
        await _transactionRepository.Create(recurringTransaction);
        var createdTransactionsForRecurred = await _transactionService.CreateTransactionsForRecurred();

        Assert.Equal(2, createdTransactionsForRecurred.Count);
    }
    
    [Fact]
    public async Task CheckRecurringTransactionIsExecutedEverySixMonths_ShouldCreateTransactionsForLastHalfYearAndToday()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        var recurringTransaction = MockEntityFactory.CreateMockRecurringTransaction(profile.Id, 100,
            DateTime.Now.AddMonths(-6), RecurringTransactionInterval.EverySixMonths);
        await _transactionRepository.Create(recurringTransaction);
        var createdTransactionsForRecurred = await _transactionService.CreateTransactionsForRecurred();

        Assert.Equal(2, createdTransactionsForRecurred.Count);
    }
    
    [Fact]
    public async Task CheckRecurringTransactionIsExecutedAnnually_ShouldCreateTransactionsForLastTwoYearsAndToday()
    {
        var profile = await _profileRepository.Create(MockEntityFactory.CreateMockProfile());
        var recurringTransaction = MockEntityFactory.CreateMockRecurringTransaction(profile.Id, 100,
            DateTime.Now.AddYears(-2), RecurringTransactionInterval.Annually);
        await _transactionRepository.Create(recurringTransaction);
        var createdTransactionsForRecurred = await _transactionService.CreateTransactionsForRecurred();

        Assert.Equal(3, createdTransactionsForRecurred.Count);
    }
}
