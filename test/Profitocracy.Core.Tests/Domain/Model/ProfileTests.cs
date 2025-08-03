using FluentAssertions;
using Profitocracy.Core.Domain.Model.Profiles.Factories;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Domain.Model.Transactions.Entities;
using Profitocracy.Core.Domain.Model.Transactions.Factories;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;

namespace Profitocracy.Core.Tests.Domain.Model;

public class ProfileTests
{
    [Fact]
    public void ProfileBuilder_CreateProfile_ShouldSetCorrectValues()
    {
        var id = Guid.NewGuid();
        var expectedName = "Personal Budget";
        var startDate = DateTime.Now.AddMonths(-1);
        var endDate = DateTime.Now;

        var profile = new ProfileBuilder(id)
            .AddName(expectedName)
            .AddBalance(500)
            .AddBillingPeriod(startDate, endDate)
            .AddCurrency(Currency.AvailableCurrencies.Usd)
            .Build();

        profile.Id.Should().Be(id);
        profile.Name.Should().Be(expectedName);
        profile.BillingPeriod.DateFrom.Should().Be(startDate);
        profile.BillingPeriod.DateTo.Should().Be(endDate);
        profile.Balance.Should().Be(500);
        profile.Settings.Currency.Should().Be(Currency.AvailableCurrencies.Usd);
    }

    [Fact]
    public void HandleTransaction_ValidTransaction_ShouldUpdateBalances()
    {
        var profile = new ProfileBuilder()
            .AddName("Test Profile")
            .AddBillingPeriod(DateTime.Now.AddMonths(-1), DateTime.Now.AddDays(1))
            .Build();

        var transaction = TransactionFactory.CreateTransaction(
            id: Guid.NewGuid(),
            amount: 150,
            profileId: profile.Id,
            type: TransactionType.Income,
            spendingType: null,
            timestamp: DateTime.Now,
            description: "Test transaction",
            geoTag: null,
            category: null,
            recurringTransactionInfo: null
        );

        profile.HandleTransactions([transaction], DateTime.Now);
        profile.Balance.Should().Be(150);
    }

    [Fact]
    public void HandleTransactions_MultipleValidTransactions_ShouldAggregateBalance()
    {
        var profile = new ProfileBuilder()
            .AddName("Test Profile")
            .AddBillingPeriod(DateTime.Now.AddMonths(-1), DateTime.Now.AddDays(1))
            .Build();

        var transactions = new List<Transaction>
        {
            TransactionFactory.CreateTransaction(
                id: Guid.NewGuid(),
                amount: 100,
                profileId: profile.Id,
                type: TransactionType.Income,
                spendingType: null,
                timestamp: DateTime.Now.AddDays(-10),
                description: "Transaction 1",
                geoTag: null,
                category: null,
                recurringTransactionInfo: null
            ),
            TransactionFactory.CreateTransaction(
                id: Guid.NewGuid(),
                amount: 200,
                profileId: profile.Id,
                type: TransactionType.Income,
                spendingType: null,
                timestamp: DateTime.Now.AddDays(-5),
                description: "Transaction 2",
                geoTag: null,
                category: null,
                recurringTransactionInfo: null
            )
        };

        foreach (var transaction in transactions)
        {
            profile.HandleTransactions([transaction], DateTime.Now);
        }

        profile.Balance.Should().Be(300);
    }

    [Fact]
    public void HandleTransaction_ValidWithCategory_ShouldUpdateCategoryActualAmount()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var profile = new ProfileBuilder()
            .AddName("Test Profile")
            .AddBillingPeriod(DateTime.Now.AddMonths(-1), DateTime.Now.AddDays(1))
            .AddCategoryExpense(categoryId, "Groceries", 1000)
            .Build();

        var transaction = TransactionFactory.CreateTransaction(
            id: Guid.NewGuid(),
            amount: 500,
            profileId: profile.Id,
            type: TransactionType.Expense,
            spendingType: SpendingType.Main,
            timestamp: DateTime.Now,
            description: "Groceries transaction",
            geoTag: null,
            category: new TransactionCategory(categoryId)
            {
                Name = "Groceries",
            },
            recurringTransactionInfo: null
        );

        // Act
        profile.HandleTransactions([transaction], DateTime.Now);

        // Assert
        profile.CategoriesExpenses
            .Should()
            .ContainSingle(c => c.Id == categoryId && c.ActualAmount == 500);
    }
}
