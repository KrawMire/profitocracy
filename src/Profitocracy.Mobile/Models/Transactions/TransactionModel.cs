using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;
using Profitocracy.Mobile.Views.Shared.Controls;
using System.Globalization;

namespace Profitocracy.Mobile.Models.Transactions;

public class TransactionModel
{
    private static readonly string[] SpendingTypes =
    [
        AppResources.Transactions_Main,
        AppResources.Transactions_Secondary,
        AppResources.Transactions_Saved,
        AppResources.Transactions_Income,
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
    public string TimestampDisplay => Timestamp.ToString("g", CultureInfo.CurrentCulture);
    public TransactionCategoryModel? Category { get; set; }
    public string? Description { get; set; }

    public string? CurrencySymbol { get; set; }

    public bool IsIncome => SpendingType is null or -1;
    public bool IsSaved => !IsIncome && SpendingType == 2;
    public bool IsSpending => !IsIncome && !IsSaved;

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

    /// <summary>
    /// Word of the 50/30/20 split the transaction belongs to: "needs", "wants" or "savings".
    /// </summary>
    public string SpendingWord => SpendingType switch
    {
        0 => AppResources.Common_NeedsWord,
        1 => AppResources.Common_WantsWord,
        2 => AppResources.Common_SavingsWord,
        _ => AppResources.Transactions_Income
    };

    /// <summary>
    /// Main line of the transaction row.
    /// </summary>
    public string Title => !string.IsNullOrWhiteSpace(Description)
        ? Description
        : Category?.Name ?? DisplaySpendingType;

    /// <summary>
    /// Category (or spending type) and the 50/30/20 word, e.g. "Food · needs".
    /// </summary>
    public string Meta => IsIncome
        ? AppResources.Transactions_Income
        : $"{Category?.Name ?? DisplaySpendingType} · {SpendingWord}";

    /// <summary>
    /// Category (or spending type) and the day, e.g. "Food · Today".
    /// </summary>
    public string MetaWithDay => IsIncome
        ? $"{AppResources.Transactions_Income} · {DateUtils.ShortDay(Timestamp)}"
        : $"{Category?.Name ?? DisplaySpendingType} · {DateUtils.ShortDay(Timestamp)}";

    /// <summary>
    /// Signed amount in the currency the money was actually paid or saved in.
    /// </summary>
    public string PrimaryAmount
    {
        get
        {
            var isForeign = IsMultiCurrency && !IsIncome && DestinationAmount is not null;
            var amount = isForeign ? DestinationAmount!.Value : Amount;
            var symbol = isForeign ? DestinationCurrency : CurrencySymbol;
            var signed = IsSpending ? -amount : amount;

            return NumberUtils.FormatMoney(signed, symbol, showPlus: true);
        }
    }

    /// <summary>
    /// The other side of a multi-currency transaction, empty otherwise.
    /// </summary>
    public string SecondaryAmount
    {
        get
        {
            if (!IsMultiCurrency || DestinationAmount is null)
            {
                return string.Empty;
            }

            // Income in another currency is taken from savings
            return IsIncome
                ? NumberUtils.FormatMoney(-DestinationAmount.Value, DestinationCurrency)
                : NumberUtils.FormatMoney(Amount, CurrencySymbol);
        }
    }

    public bool HasSecondaryAmount => !string.IsNullOrEmpty(SecondaryAmount);

    public string IconData => IsIncome
        ? Icons.Wallet
        : IsSaved
            ? Icons.Coins
            : Category is null
                ? Icons.Receipt
                : Icons.Tag;

    public RecurringTransactionIntervalModel? Interval { get; set; }

    public bool IsRecurring => Interval is not null && Interval.Value > 0;
    public string? DisplayInterval => Interval?.Name;

    public static TransactionModel FromDomain(Transaction transaction)
    {
        TransactionCategoryModel? category = null;

        if (transaction.Category is not null)
        {
            category = new TransactionCategoryModel
            {
                CategoryId = transaction.Category.Id,
                Name = transaction.Category.Name,
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
            CurrencySymbol = transaction.SourceCurrency.Symbol,
            ProfileId = transaction.ProfileId,
            Type = (int)transaction.Type,
            SpendingType = transaction.SpendingType is null ? null : (int)transaction.SpendingType,
            Description = transaction.Description,
            Timestamp = transaction.Timestamp,
            Category = category,
            Interval = interval,
        };
    }
}
