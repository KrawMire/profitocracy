using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Mobile.Resources.Strings;
using System.Globalization;

namespace Profitocracy.Mobile.Models.Transactions;

public class TransactionModel
{
    private static readonly string[] SpendingTypes =
    [
        AppResources.Transactions_Main,
        AppResources.Transactions_Secondary,
        AppResources.Transactions_Saved,
        AppResources.Transactions_Income
    ];

    public Guid? Id { get; set; }
    public bool IsMultiCurrency { get; set; }
    public required decimal Amount { get; set; }
    public decimal? DestinationAmount { get; set; }
    public string? DestinationCurrency { get; set; }
    public required Guid ProfileId { get; set; }
    public required int Type { get; set; }
    public required int? SpendingType { get; set; }
    public required DateTime Timestamp { get; set; }
    public string? TimestampDisplay => Timestamp.ToString("g", CultureInfo.CurrentCulture);
    public TransactionCategoryModel? Category { get; set; }
    public string? Description { get; set; }

    public bool IsIncome => SpendingType is null or -1;

    public string DisplaySpendingType => IsIncome ? SpendingTypes[3] : SpendingTypes[(int)SpendingType!];

    public string DisplayAmount
    {
        get
        {
            var amount = Amount.ToString(CultureInfo.CurrentCulture);

            return Type == 0
                ? $"+{amount.ToString(CultureInfo.CurrentCulture)}"
                : $"-{amount.ToString(CultureInfo.CurrentCulture)}";
        }
    }

    public string AdditionalDisplayAmount
    {
        get
        {
            if (!IsMultiCurrency)
            {
                return string.Empty;
            }

            var amount = IsMultiCurrency
                ? DestinationCurrency + ((decimal)DestinationAmount!).ToString(CultureInfo.CurrentCulture)
                : Amount.ToString(CultureInfo.CurrentCulture);

            if (Type == 0)
            {
                // In case of multi currency transaction we take from saved, so "-" is used
                return IsMultiCurrency ? $"-{amount}" : $"+{amount}";
            }

            // In case of expense, if it's saving transaction,
            // we take from profile balance and put it to saved amount
            return SpendingType == 2
                ? $"+{amount}"
                : $"-{amount}";
        }
    }

    public RecurringTransactionIntervalModel? Interval { get; set; }

    public bool IsRecurring => Interval is not null && Interval.Value > 0;

    public static TransactionModel FromDomain(Transaction transaction)
    {
        TransactionCategoryModel? category = null;

        if (transaction.Category is not null)
        {
            category = new TransactionCategoryModel
            {
                CategoryId = transaction.Category.Id,
                Name = transaction.Category.Name
            };
        }

        RecurringTransactionIntervalModel? interval = null;

        if (transaction.RecurringTransactionInfo is not null)
        {
            interval = RecurringTransactionIntervalModel.FromDomain(transaction.RecurringTransactionInfo.Interval);
        }

        var multiTransaction = transaction as MultiCurrencyTransaction;
        var isMultiCurrency = multiTransaction is not null;

        return new TransactionModel
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            IsMultiCurrency = isMultiCurrency,
            DestinationAmount = isMultiCurrency ? multiTransaction!.DestinationAmount : null,
            DestinationCurrency = isMultiCurrency ? multiTransaction!.DestinationCurrency.Symbol : null,
            ProfileId = transaction.ProfileId,
            Type = (int)transaction.Type,
            SpendingType = transaction.SpendingType is null ? null : (int)transaction.SpendingType,
            Description = transaction.Description,
            Timestamp = transaction.Timestamp,
            Category = category,
            Interval = interval
        };
    }
}
