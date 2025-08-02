using Profitocracy.Core.Domain.Model.Categories;
using Profitocracy.Core.Domain.Model.Categories.Factories;
using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Domain.Model.Profiles.Factories;
using Profitocracy.Core.Domain.Model.Settings;
using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Domain.Model.Transactions.Factories;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;

namespace Profitocracy.Core.Tests.Mocks.Factories;

public class MockEntityFactory
{
    /// <summary>
    /// Creates a mock profile using the ProfileBuilder.
    /// </summary>
    /// <returns>A newly created Profile instance.</returns>
    public static Profile CreateMockProfile(bool isCurrent = true)
    {
        return new ProfileBuilder()
            .AddBalance(1000)
            .AddBillingPeriod(DateTime.Now.Date.AddDays(-30), DateTime.Now.Date.AddDays(10))
            .AddCurrency(Currency.AvailableCurrencies.Usd)
            .AddIsCurrent(isCurrent)
            .AddName("Test profile")
            .AddStartDate(DateTime.Now.Date.AddDays(-10), 1000)
            .Build();
    }

    /// <summary>
    /// Creates a mock category using the CategoryFactory.
    /// </summary>
    /// <param name="profileId">The ID of the profile associated with the category.</param>
    /// <returns>A newly created Category instance.</returns>
    public static Category CreateMockCategory(Guid profileId)
    {
        return CategoryFactory.CreateCategory(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test category",
            null);
    }

    public static Category CreateMockCategory(Guid profileId, string name, decimal plannedAmount)
    {
        return CategoryFactory.CreateCategory(
            Guid.NewGuid(),
            Guid.NewGuid(),
            name,
            plannedAmount);
    }

    /// <summary>
    /// Creates a mock transaction using the TransactionFactory.
    /// </summary>
    /// <param name="profileId">The ID of the profile the transaction is associated with.</param>
    /// <param name="amount">The amount for the transaction.</param>
    /// <returns>A newly created Transaction instance.</returns>
    public static Transaction CreateMockTransaction(Guid profileId, decimal amount)
    {
        return TransactionFactory.CreateTransaction(
            Guid.NewGuid(),
            amount,
            profileId,
            TransactionType.Expense,
            SpendingType.Secondary,
            DateTime.Now.AddDays(-5),
            "Test transaction",
            null,
            null,
            null);
    }
    
    /// <summary>
    /// Creates a mock recurring transaction using the TransactionFactory.
    /// </summary>
    /// <param name="profileId">The ID of the profile the recurring transaction is associated with.</param>
    /// <param name="amount">The amount for the recurring transaction.</param>
    /// <param name="startDate">The start date for the recurring transaction.</param>
    /// <param name="interval">The interval for the recurring transaction.</param>
    /// <returns>A newly created recurring Transaction instance.</returns>
    public static Transaction CreateMockRecurringTransaction(Guid profileId, decimal amount, DateTime startDate,
        RecurringTransactionInterval? interval)
    {
        return TransactionFactory.CreateTransaction(
            Guid.NewGuid(),
            amount,
            profileId,
            TransactionType.Expense,
            SpendingType.Secondary,
            startDate,
            "Test recurring transaction",
            null,
            null,
            interval is null ? null : new RecurringTransactionInfo {Interval = (RecurringTransactionInterval)interval});
    }

    /// <summary>
    /// Creates a mock settings object.
    /// </summary>
    /// <returns>A newly created Settings instance.</returns>
    public static Settings CreateMockSettings()
    {
        return new Settings(
            Guid.NewGuid(),
            Theme.Dark,
            "en",
            new AuthenticationSettings
            {
                IsAuthenticationEnabled = false,
                IsBiometricAuthEnabled = false,
                PasswordHash = null,
            });
    }

}
