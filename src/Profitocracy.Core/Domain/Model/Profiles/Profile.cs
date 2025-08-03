using Profitocracy.Core.Domain.Model.Profiles.Entities;
using Profitocracy.Core.Domain.Model.Profiles.ValueObjects;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Profiles;

/// <summary>
/// Profile aggregate root.
/// Contains data about settings of profile, balances
/// state and methods for calculation of transactions
/// </summary>
public class Profile : AggregateRoot<Guid>
{

    private const decimal MainRatio = 0.5m;
    private const decimal SecondaryRatio = 0.3m;
    private const decimal SavedRatio = 0.2m;

    internal Profile(
        Guid id,
        string name,
        AnchorDate startDate,
        List<ProfileCategory> categoriesExpenses,
        ProfileSettings settings,
        bool isCurrent) : this(
            id,
            name,
            startDate,
            null,
            categoriesExpenses,
            settings,
            isCurrent)
    { }

    internal Profile(
        Guid id,
        string name,
        AnchorDate startDate,
        TimePeriod? billingPeriod,
        List<ProfileCategory> categoriesExpenses,
        ProfileSettings settings,
        bool isCurrent) : base(id)
    {
        Name = name;
        StartDate = startDate;
        Balance = StartDate.InitialBalance;
        CategoriesExpenses = categoriesExpenses;
        Settings = settings;
        IsCurrent = isCurrent;
        Expenses = new ProfileExpenses
        {
            TomorrowBalance = 0,
            TodayBalance = new ProfileExpense
            {
                ActualAmount = 0,
                PlannedAmount = 0
            },
            Main = new ProfileExpense
            {
                ActualAmount = 0,
                PlannedAmount = 0
            },
            Secondary = new ProfileExpense
            {
                ActualAmount = 0,
                PlannedAmount = 0
            },
            Saved = new ProfileExpense
            {
                ActualAmount = 0,
                PlannedAmount = 0
            },
            TotalBalance = new ProfileExpense
            {
                ActualAmount = 0,
                PlannedAmount = 0
            }
        };

        BillingPeriod = billingPeriod ?? new TimePeriod
        {
            DateFrom = new DateTime(
                StartDate.Timestamp.Year,
                StartDate.Timestamp.Month,
                day: 1,
                hour: 0,
                minute: 0,
                second: 0,
                millisecond: 0),
            DateTo = new DateTime(
                StartDate.Timestamp.Year,
                StartDate.Timestamp.Month,
                day: DateTime.DaysInMonth(StartDate.Timestamp.Year, StartDate.Timestamp.Month),
                hour: 23,
                minute: 59,
                second: 59,
                millisecond: 999),
        };

        SavedAmounts = new Dictionary<Currency, decimal>();

        _todayInitialBalance = Balance;
    }

    /// <summary>
    /// Is billing period of profile new
    /// </summary>
    public bool IsNewPeriod { get; private set; }

    /// <summary>
    /// Name of profile
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initial balance from date when profile was created
    /// </summary>
    public AnchorDate StartDate { get; private set; }

    /// <summary>
    /// Start and end dates of billing period
    /// </summary>
    public TimePeriod BillingPeriod { get; private set; }

    /// <summary>
    /// Current balance
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// Calculations of expenses by types: main, secondary and saved
    /// </summary>
    public ProfileExpenses Expenses { get; }

    /// <summary>
    /// Result of expenses calculations for every category
    /// </summary>
    public List<ProfileCategory> CategoriesExpenses { get; }

    public Dictionary<Currency, decimal> SavedAmounts { get; }

    /// <summary>
    /// Profile settings
    /// </summary>
    public ProfileSettings Settings { get; }

    /// <summary>
    /// Is this profile currently using by user
    /// </summary>
    public bool IsCurrent { get; set; }

    /// <summary>
    /// This field is used to correctly calculate money for today.
    /// Without this field we will see decreasing amount of
    /// PlannedAmount of DailyFromActualBalance
    /// </summary>
    private decimal _todayInitialBalance;

    /// <summary>
    /// Starts a new billing period with the specified start and end dates.
    /// </summary>
    /// <param name="startDate">The start date of the new billing period.</param>
    /// <param name="endDate">The end date of the new billing period.</param>
    public void StartNewBillingPeriod(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be greater than the start date.");

        BillingPeriod = new TimePeriod
        {
            DateFrom = startDate,
            DateTo = endDate,
        };

        StartDate = new AnchorDate
        {
            Timestamp = startDate,
            InitialBalance = Balance,
        };
    }

    /// <summary>
    /// Process transaction and
    /// project the results in profile
    /// </summary>
    /// <param name="transactions">Transactions to handle</param>
    /// <param name="currentDate">Current date</param>
    public void HandleTransactions(ICollection<Transaction> transactions, DateTime currentDate)
    {
        foreach (var transaction in transactions)
        {
            if (transaction is MultiCurrencyTransaction multiCurrencyTransaction)
            {
                HandleMultiCurrencyTransaction(multiCurrencyTransaction, currentDate);
            }
            else
            {
                HandleTransaction(transaction, currentDate);
            }
        }

        if (BillingPeriod.DateTo < currentDate || currentDate < BillingPeriod.DateFrom)
        {
            IsNewPeriod = true;

            StartDate = new AnchorDate
            {
                InitialBalance = Balance,
                Timestamp = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day),
            };

            var endDate = new DateTime(
                currentDate.Year,
                currentDate.Month,
                day: DateTime.DaysInMonth(currentDate.Year, currentDate.Month));

            endDate = endDate.Add(DateTime.MaxValue.TimeOfDay);

            BillingPeriod = new TimePeriod
            {
                DateFrom = StartDate.Timestamp.Date,
                DateTo = endDate,
            };
        }

        RecalculateExpenses(currentDate);
    }

    /// <summary>
    /// Add profile categories to transaction processing
    /// </summary>
    /// <param name="categories">List of categories to add to profile</param>
    public void AddCategories(IEnumerable<ProfileCategory> categories)
    {
        CategoriesExpenses.AddRange(categories);
    }

    private void HandleMultiCurrencyTransaction(MultiCurrencyTransaction transaction, DateTime currentDate)
    {
        switch (transaction.Destination)
        {
            case TransactionDestination.ProfileBalance:
                HandleMultiCurrencyWithdrawalTransaction(transaction);
                break;
            case TransactionDestination.Expense:
                HandleExpenseTransaction(transaction, currentDate);
                break;
            case TransactionDestination.SavingsBalance:
                HandleMultiCurrencySavingTransaction(transaction, currentDate);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(transaction), "Transaction destination is out of range");
        }
    }

    private void HandleMultiCurrencyWithdrawalTransaction(MultiCurrencyTransaction transaction)
    {
        if (SavedAmounts.ContainsKey(transaction.DestinationCurrency))
        {
            SavedAmounts[transaction.DestinationCurrency] -= transaction.DestinationAmount;
        }
        else
        {
            SavedAmounts.Add(transaction.DestinationCurrency, -transaction.DestinationAmount);
        }

        if (BillingPeriod.DateFrom <= transaction.Timestamp && transaction.Timestamp <= BillingPeriod.DateTo)
        {
            HandleIncomeTransaction(transaction);
        }
    }

    private void HandleMultiCurrencySavingTransaction(MultiCurrencyTransaction transaction, DateTime currentDate)
    {
        if (SavedAmounts.ContainsKey(transaction.DestinationCurrency))
        {
            SavedAmounts[transaction.DestinationCurrency] += transaction.DestinationAmount;
        }
        else
        {
            SavedAmounts.Add(transaction.DestinationCurrency, transaction.DestinationAmount);
        }

        if (BillingPeriod.DateFrom <= transaction.Timestamp && transaction.Timestamp <= BillingPeriod.DateTo)
        {
            HandleExpenseTransaction(transaction, currentDate);
        }
    }

    private void HandleTransaction(Transaction transaction, DateTime currentDate)
    {
        if (transaction.Type == TransactionType.Income)
        {
            HandleIncomeTransaction(transaction);
        }
        else
        {
            if (transaction.SpendingType == SpendingType.Saved && transaction.Timestamp < BillingPeriod.DateFrom)
            {
                HandleSavingSpendingTransaction(transaction, currentDate);
                return;
            }

            HandleExpenseTransaction(transaction, currentDate);
        }
    }

    private void RecalculateExpenses(DateTime currentDate)
    {
        var currentPeriodDiff = BillingPeriod.DateTo - currentDate;

        var daysInActualPeriod = currentPeriodDiff.Days + 1;
        var daysInPeriodFromTomorrow = daysInActualPeriod - 1;

        // Used to prevent division by zero
        daysInActualPeriod = daysInActualPeriod == 0 ? 1 : daysInActualPeriod;
        daysInPeriodFromTomorrow = daysInPeriodFromTomorrow == 0 ? 1 : daysInPeriodFromTomorrow;

        Expenses.TotalBalance.PlannedAmount += StartDate.InitialBalance;

        Expenses.TodayBalance.PlannedAmount = _todayInitialBalance < 0 ? 0 : _todayInitialBalance / daysInActualPeriod;
        Expenses.TomorrowBalance = Balance < 0 ? 0 : Balance / daysInPeriodFromTomorrow;

        Expenses.Main.PlannedAmount = Expenses.TotalBalance.PlannedAmount * MainRatio;
        Expenses.Secondary.PlannedAmount = Expenses.TotalBalance.PlannedAmount * SecondaryRatio;
        Expenses.Saved.PlannedAmount = Expenses.TotalBalance.PlannedAmount * SavedRatio;
    }

    private void HandleIncomeTransaction(Transaction transaction)
    {
        _todayInitialBalance += transaction.Amount;
        Balance += transaction.Amount;
        Expenses.TotalBalance.PlannedAmount += transaction.Amount;
    }

    private void HandleExpenseTransaction(Transaction transaction, DateTime currentDate)
    {
        Balance -= transaction.Amount;

        if (transaction.Timestamp.Date == currentDate.Date)
        {
            Expenses.TodayBalance.ActualAmount += transaction.Amount;
        }
        else
        {
            _todayInitialBalance -= transaction.Amount;
        }

        Expenses.TotalBalance.ActualAmount += transaction.Amount;

        switch (transaction.SpendingType)
        {
            case SpendingType.Main:
                HandleMainSpendingTransaction(transaction);
                break;
            case SpendingType.Secondary:
                HandleSecondarySpendingTransaction(transaction);
                break;
            case SpendingType.Saved:
                HandleSavingSpendingTransaction(transaction, currentDate);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(transaction), "Spending type is out of range");
        }

        if (transaction.Category is not null)
        {
            HandleCategoryTransaction(transaction);
        }
    }

    private void HandleMainSpendingTransaction(Transaction transaction)
    {
        Expenses.Main.ActualAmount += transaction.Amount;
    }

    private void HandleSecondarySpendingTransaction(Transaction transaction)
    {
        Expenses.Secondary.ActualAmount += transaction.Amount;
    }

    private void HandleSavingSpendingTransaction(Transaction transaction, DateTime currentDate)
    {
        if (transaction.Timestamp.Month == currentDate.Month)
        {
            Expenses.Saved.ActualAmount += transaction.Amount;
        }
    }

    private void HandleCategoryTransaction(Transaction transaction)
    {
        var category = CategoriesExpenses.Find(c => c.Id.Equals(transaction.Category!.Id));

        if (category is null)
        {
            return;
        }

        category.ActualAmount += transaction.Amount;
    }
}
