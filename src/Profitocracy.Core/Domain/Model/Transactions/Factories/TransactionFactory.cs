using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions.Entities;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;

namespace Profitocracy.Core.Domain.Model.Transactions.Factories;

public class TransactionFactory
{
	public static Transaction CreateTransaction(
		Guid? id,
		decimal amount,
		Guid profileId,
		TransactionType type,
		SpendingType? spendingType,
		DateTime timestamp,
		string? description,
		TransactionGeoTag? geoTag,
		TransactionCategory? category,
		RecurringTransactionInfo? recurringTransactionInfo)
	{
		id ??= Guid.NewGuid();

		return new Transaction(
			(Guid)id,
			amount,
			profileId,
			type,
			spendingType,
			timestamp,
			description,
			geoTag,
			category,
			recurringTransactionInfo);
	}

	public static MultiCurrencyTransaction CreateMultiCurrencyTransaction(
		Guid? id,
		decimal amount,
		decimal destinationAmount,
		Currency sourceCurrency,
		Currency destinationCurrency,
		Guid profileId,
		TransactionType type,
		SpendingType? spendingType,
		TransactionDestination destination,
		DateTime timestamp,
		string? description,
		TransactionGeoTag? geoTag,
		TransactionCategory? category,
		RecurringTransactionInfo? recurringTransactionInfo)
	{
		id ??= Guid.NewGuid();

		return new MultiCurrencyTransaction(
			(Guid)id,
			amount,
			destinationAmount,
			sourceCurrency,
			destinationCurrency,
			profileId,
			type,
			spendingType,
			destination,
			timestamp,
			description,
			geoTag,
			category,
			recurringTransactionInfo);
	}
}
