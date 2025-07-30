using FluentAssertions;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions.Factories;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;

namespace Profitocracy.Core.Tests.Domain.Model;

public class TransactionTests
{
    [Fact]
    public void CreateTransaction_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        const decimal amount = 100m;
        const string description = "Test Transaction";
        var date = DateTime.Now;
        const TransactionType type = TransactionType.Income;
        var spendingType = SpendingType.Main;
        var profileId = Guid.NewGuid();

        var transaction = TransactionFactory.CreateTransaction(
            id,
            amount,
            profileId,
            type,
            spendingType,
            date,
            description,
            null,
            null);

        transaction.Id.Should().Be(id);
        transaction.Amount.Should().Be(amount);
        transaction.Description.Should().Be(description);
        transaction.Type.Should().Be(type);
        transaction.SpendingType.Should().Be(spendingType);
        transaction.ProfileId.Should().Be(profileId);
    }

    [Fact]
    public void CreateMultiCurrencyTransaction_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        const decimal amount = 200m;
        const decimal destinationAmount = 180m;
        var sourceCurrency = Currency.AvailableCurrencies.Usd;
        var destinationCurrency = Currency.AvailableCurrencies.Eur;
        var date = DateTime.Now;
        const TransactionType type = TransactionType.Expense;
        var profileId = Guid.NewGuid();

        var multiCurrencyTransaction = TransactionFactory.CreateMultiCurrencyTransaction(
            id,
            amount,
            destinationAmount,
            sourceCurrency,
            destinationCurrency,
            profileId,
            type,
            SpendingType.Main,
            TransactionDestination.Expense,
            date,
            "MultiCurrency Transaction",
            null,
            null);

        // Assert
        multiCurrencyTransaction.Id.Should().Be(id);
        multiCurrencyTransaction.Amount.Should().Be(amount);
        multiCurrencyTransaction.DestinationAmount.Should().Be(destinationAmount);
        multiCurrencyTransaction.SourceCurrency.Should().Be(sourceCurrency);
        multiCurrencyTransaction.DestinationCurrency.Should().Be(destinationCurrency);
        multiCurrencyTransaction.Type.Should().Be(type);
        multiCurrencyTransaction.ProfileId.Should().Be(profileId);
    }
}
