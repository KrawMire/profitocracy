using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Domain.Model.Profiles.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Categories;
using Profitocracy.Mobile.Models.Profiles;
using Profitocracy.Mobile.Models.Transactions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Profitocracy.Mobile.ViewModels.Home;

public class HomePageViewModel : BaseNotifyObject
{
    private const int RecentTransactionsCount = 3;

    private string _profileName = string.Empty;
    private string _periodTitle = string.Empty;
    private string _balanceCaption = AppResources.Home_LeftThisMonth;
    private string _daysToGo = string.Empty;
    private string _balance = string.Empty;
    private string _usedOfIncome = string.Empty;
    private double _usedRatio;
    private string _todayLeft = string.Empty;
    private string _todayLeftOf = string.Empty;
    private string _tomorrow = string.Empty;
    private string _spreadHint = string.Empty;
    private string _emptyPeriodText = string.Empty;
    private string _recalculatingText = string.Empty;

    private bool _hasOverLimit;
    private string _overLimitTitle = string.Empty;
    private string _overLimitDetail = string.Empty;

    private string _mainSpent = string.Empty;
    private string _mainDetail = string.Empty;
    private string _mainRemaining = string.Empty;
    private double _mainRatio;
    private string _secondarySpent = string.Empty;
    private string _secondaryDetail = string.Empty;
    private string _secondaryRemaining = string.Empty;
    private double _secondaryRatio;
    private string _savedSpent = string.Empty;
    private string _savedDetail = string.Empty;
    private string _savedRemaining = string.Empty;
    private double _savedRatio;

    private bool _isLoaded;
    private bool _hasIncome = true;
    private bool _isDisplayNoCategories;
    private bool _hasRecent;
    private bool _hasSavedAmounts;
    private bool _isRefreshing = true;

    private readonly ICalculationService _calculationService;
    private readonly ITransactionRepository _transactionRepository;

    public HomePageViewModel(
        ICalculationService calculationService,
        ITransactionRepository transactionRepository)
    {
        _calculationService = calculationService;
        _transactionRepository = transactionRepository;

        RefreshCommand = new Command(
            execute: Refresh,
            canExecute: () => !IsRefreshing);
    }

    public Guid ProfileId { get; private set; }
    public DateTime DateFrom { get; private set; }
    public DateTime DateTo { get; private set; }

    public ObservableCollection<CategoryExpenseModel> CategoriesExpenses { get; } = [];
    public ObservableCollection<TransactionModel> RecentTransactions { get; } = [];
    public ObservableCollection<SavedAmountModel> SavedAmounts { get; } = [];

    /// <summary>
    /// Category with the biggest overspend, the banner leads to it.
    /// </summary>
    public CategoryExpenseModel? OverLimitCategory { get; private set; }

    public ICommand RefreshCommand { get; }

    public string ProfileName
    {
        get => _profileName;
        private set => SetProperty(ref _profileName, value);
    }

    public string PeriodTitle
    {
        get => _periodTitle;
        private set => SetProperty(ref _periodTitle, value);
    }

    public string BalanceCaption
    {
        get => _balanceCaption;
        private set => SetProperty(ref _balanceCaption, value);
    }

    public string DaysToGo
    {
        get => _daysToGo;
        private set => SetProperty(ref _daysToGo, value);
    }

    public string Balance
    {
        get => _balance;
        private set => SetProperty(ref _balance, value);
    }

    public string UsedOfIncome
    {
        get => _usedOfIncome;
        private set => SetProperty(ref _usedOfIncome, value);
    }

    public double UsedRatio
    {
        get => _usedRatio;
        private set => SetProperty(ref _usedRatio, value);
    }

    public string TodayLeft
    {
        get => _todayLeft;
        private set => SetProperty(ref _todayLeft, value);
    }

    public string TodayLeftOf
    {
        get => _todayLeftOf;
        private set => SetProperty(ref _todayLeftOf, value);
    }

    public string Tomorrow
    {
        get => _tomorrow;
        private set => SetProperty(ref _tomorrow, value);
    }

    public string SpreadHint
    {
        get => _spreadHint;
        private set => SetProperty(ref _spreadHint, value);
    }

    public string EmptyPeriodText
    {
        get => _emptyPeriodText;
        private set => SetProperty(ref _emptyPeriodText, value);
    }

    public string RecalculatingText
    {
        get => _recalculatingText;
        private set => SetProperty(ref _recalculatingText, value);
    }

    public bool HasOverLimit
    {
        get => _hasOverLimit;
        private set => SetProperty(ref _hasOverLimit, value);
    }

    public string OverLimitTitle
    {
        get => _overLimitTitle;
        private set => SetProperty(ref _overLimitTitle, value);
    }

    public string OverLimitDetail
    {
        get => _overLimitDetail;
        private set => SetProperty(ref _overLimitDetail, value);
    }

    public string MainSpent
    {
        get => _mainSpent;
        private set => SetProperty(ref _mainSpent, value);
    }

    public string MainDetail
    {
        get => _mainDetail;
        private set => SetProperty(ref _mainDetail, value);
    }

    public string MainRemaining
    {
        get => _mainRemaining;
        private set => SetProperty(ref _mainRemaining, value);
    }

    public double MainRatio
    {
        get => _mainRatio;
        private set => SetProperty(ref _mainRatio, value);
    }

    public string SecondarySpent
    {
        get => _secondarySpent;
        private set => SetProperty(ref _secondarySpent, value);
    }

    public string SecondaryDetail
    {
        get => _secondaryDetail;
        private set => SetProperty(ref _secondaryDetail, value);
    }

    public string SecondaryRemaining
    {
        get => _secondaryRemaining;
        private set => SetProperty(ref _secondaryRemaining, value);
    }

    public double SecondaryRatio
    {
        get => _secondaryRatio;
        private set => SetProperty(ref _secondaryRatio, value);
    }

    public string SavedSpent
    {
        get => _savedSpent;
        private set => SetProperty(ref _savedSpent, value);
    }

    public string SavedDetail
    {
        get => _savedDetail;
        private set => SetProperty(ref _savedDetail, value);
    }

    public string SavedRemaining
    {
        get => _savedRemaining;
        private set => SetProperty(ref _savedRemaining, value);
    }

    public double SavedRatio
    {
        get => _savedRatio;
        private set => SetProperty(ref _savedRatio, value);
    }

    /// <summary>
    /// False until the first calculation finishes, the page shows placeholders meanwhile.
    /// </summary>
    public bool IsLoaded
    {
        get => _isLoaded;
        private set
        {
            SetProperty(ref _isLoaded, value);
            OnPropertyChanged(nameof(IsLoading));
            OnPropertyChanged(nameof(IsShowFilledPeriod));
            OnPropertyChanged(nameof(IsShowEmptyPeriod));
        }
    }

    public bool IsLoading => !IsLoaded;

    /// <summary>
    /// Whether there is any money in the period. Without it there is nothing to split.
    /// </summary>
    public bool HasIncome
    {
        get => _hasIncome;
        private set
        {
            SetProperty(ref _hasIncome, value);
            OnPropertyChanged(nameof(IsShowFilledPeriod));
            OnPropertyChanged(nameof(IsShowEmptyPeriod));
        }
    }

    public bool IsShowFilledPeriod => IsLoaded && HasIncome;
    public bool IsShowEmptyPeriod => IsLoaded && !HasIncome;

    public bool IsDisplayNoCategories
    {
        get => _isDisplayNoCategories;
        private set
        {
            SetProperty(ref _isDisplayNoCategories, value);
            OnPropertyChanged(nameof(HasCategories));
        }
    }

    public bool HasCategories => !IsDisplayNoCategories;

    public bool HasRecent
    {
        get => _hasRecent;
        private set
        {
            SetProperty(ref _hasRecent, value);
            OnPropertyChanged(nameof(HasNoRecent));
        }
    }

    public bool HasNoRecent => !HasRecent;

    public bool HasSavedAmounts
    {
        get => _hasSavedAmounts;
        private set => SetProperty(ref _hasSavedAmounts, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    private async void Refresh()
    {
        try
        {
            await Initialize();
        }
        catch (Exception)
        {
            // Ignored
        }
    }

    public async Task Initialize()
    {
        var profile = await _calculationService.GetCurrentProfile();

        if (profile is null)
        {
            IsRefreshing = false;
            return;
        }

        var currency = profile.Settings.Currency;
        var period = profile.BillingPeriod;
        var isMonth = DateUtils.IsCalendarMonth(period.DateFrom, period.DateTo);

        ProfileId = profile.Id;
        DateFrom = period.DateFrom;
        DateTo = period.DateTo;

        ProfileName = profile.Name;
        PeriodTitle = DateUtils.PeriodTitle(period.DateFrom, period.DateTo);
        BalanceCaption = isMonth ? AppResources.Home_LeftThisMonth : AppResources.Home_LeftThisPeriod;
        SpreadHint = string.Format(AppResources.Home_SpreadHint, DateUtils.MonthName(period.DateFrom));
        EmptyPeriodText = string.Format(AppResources.Home_EmptyMonthText, DateUtils.MonthName(period.DateFrom));
        RecalculatingText = string.Format(AppResources.Home_Recalculating, DateUtils.MonthName(period.DateFrom));

        var daysLeft = (period.DateTo.Date - DateTime.Today).Days;
        DaysToGo = daysLeft > 0
            ? string.Format(AppResources.Home_DaysToGo, daysLeft)
            : AppResources.Home_LastDay;

        Balance = NumberUtils.FormatMoney(profile.Balance, currency.Symbol);
        HasIncome = profile.Expenses.TotalBalance.PlannedAmount > 0;

        InitializeExpenses(profile, currency.Symbol);
        InitializeCategoriesExpenses(profile, currency.Symbol);
        InitializeSavedAmounts(profile);
        await InitializeRecentTransactions(profile);

        IsLoaded = true;
        IsRefreshing = false;
    }

    private void InitializeExpenses(Profile profile, string symbol)
    {
        var expenses = profile.Expenses;

        UsedOfIncome = string.Format(
            AppResources.Home_UsedOfIncome,
            NumberUtils.FormatMoney(expenses.TotalBalance.ActualAmount, symbol),
            NumberUtils.FormatMoney(expenses.TotalBalance.PlannedAmount, symbol));
        UsedRatio = GetRatio(expenses.TotalBalance);

        TodayLeft = NumberUtils.FormatMoney(
            expenses.TodayBalance.PlannedAmount - expenses.TodayBalance.ActualAmount,
            symbol);
        TodayLeftOf = string.Format(
            AppResources.Home_LeftOf,
            NumberUtils.FormatMoney(expenses.TodayBalance.PlannedAmount, symbol));
        Tomorrow = NumberUtils.FormatMoney(expenses.TomorrowBalance, symbol);

        MainSpent = NumberUtils.FormatMoney(expenses.Main.ActualAmount, symbol);
        MainDetail = string.Format(
            AppResources.Home_TypeBudget,
            AppResources.Common_Needs,
            NumberUtils.FormatMoney(expenses.Main.PlannedAmount, symbol, decimals: 0));
        MainRemaining = FormatRemaining(expenses.Main, symbol);
        MainRatio = GetRatio(expenses.Main);

        SecondarySpent = NumberUtils.FormatMoney(expenses.Secondary.ActualAmount, symbol);
        SecondaryDetail = string.Format(
            AppResources.Home_TypeBudget,
            AppResources.Common_Wants,
            NumberUtils.FormatMoney(expenses.Secondary.PlannedAmount, symbol, decimals: 0));
        SecondaryRemaining = FormatRemaining(expenses.Secondary, symbol);
        SecondaryRatio = GetRatio(expenses.Secondary);

        SavedSpent = NumberUtils.FormatMoney(expenses.Saved.ActualAmount, symbol);
        SavedDetail = string.Format(
            AppResources.Home_TypeBudget,
            AppResources.Common_Savings,
            NumberUtils.FormatMoney(expenses.Saved.PlannedAmount, symbol, decimals: 0));

        var toGoal = expenses.Saved.PlannedAmount - expenses.Saved.ActualAmount;
        SavedRemaining = toGoal > 0
            ? string.Format(AppResources.Home_ToGoal, NumberUtils.FormatMoney(toGoal, symbol))
            : AppResources.Home_GoalReached;
        SavedRatio = GetRatio(expenses.Saved);
    }

    private void InitializeCategoriesExpenses(Profile profile, string symbol)
    {
        var categories = profile.CategoriesExpenses
            .Select(c => CategoryExpenseModel.FromDomain(c, symbol))
            .OrderByDescending(c => c.HasLimit)
            .ThenByDescending(c => c.Ratio)
            .ToList();

        CategoriesExpenses.Clear();

        foreach (var category in categories)
        {
            CategoriesExpenses.Add(category);
        }

        IsDisplayNoCategories = categories.Count == 0;

        OverLimitCategory = categories
            .Where(c => c.IsOver)
            .MaxBy(c => c.ActualAmount - c.PlannedAmount!.Value);

        HasOverLimit = OverLimitCategory is not null;

        if (OverLimitCategory is null)
        {
            return;
        }

        OverLimitTitle = string.Format(
            AppResources.Home_OverLimitTitle,
            OverLimitCategory.Name,
            NumberUtils.FormatMoney(OverLimitCategory.ActualAmount - OverLimitCategory.PlannedAmount!.Value, symbol));
        OverLimitDetail = string.Format(
            AppResources.Home_OverLimitDetail,
            NumberUtils.FormatMoney(OverLimitCategory.ActualAmount, symbol),
            NumberUtils.FormatMoney(OverLimitCategory.PlannedAmount.Value, symbol));
    }

    private void InitializeSavedAmounts(Profile profile)
    {
        SavedAmounts.Clear();

        foreach (var savedAmount in profile.SavedAmounts)
        {
            SavedAmounts.Add(SavedAmountModel.FromDomain(savedAmount));
        }

        HasSavedAmounts = SavedAmounts.Count > 0;
    }

    private async Task InitializeRecentTransactions(Profile profile)
    {
        var transactions = await _transactionRepository.GetForPeriod(
            profile.Id,
            profile.BillingPeriod.DateFrom,
            profile.BillingPeriod.DateTo);

        RecentTransactions.Clear();

        foreach (var transaction in transactions
                     .OrderByDescending(t => t.Timestamp)
                     .Take(RecentTransactionsCount))
        {
            RecentTransactions.Add(TransactionModel.FromDomain(transaction));
        }

        HasRecent = RecentTransactions.Count > 0;
    }

    private static string FormatRemaining(ProfileExpense expense, string symbol)
    {
        var remaining = expense.PlannedAmount - expense.ActualAmount;

        return remaining >= 0
            ? string.Format(AppResources.Home_AmountLeft, NumberUtils.FormatMoney(remaining, symbol))
            : string.Format(AppResources.Home_AmountOver, NumberUtils.FormatMoney(-remaining, symbol));
    }

    private static double GetRatio(ProfileExpense expense)
    {
        if (expense.PlannedAmount <= 0)
        {
            return expense.ActualAmount > 0 ? 1 : 0;
        }

        return (double)(expense.ActualAmount / expense.PlannedAmount);
    }
}
