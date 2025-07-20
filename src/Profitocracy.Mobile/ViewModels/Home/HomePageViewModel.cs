using System.Collections.ObjectModel;
using System.Windows.Input;
using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Profiles.Entities;
using Profitocracy.Core.Domain.Model.Profiles.ValueObjects;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Categories;
using Profitocracy.Mobile.Models.Profiles;
using Profitocracy.Mobile.Utils;

namespace Profitocracy.Mobile.ViewModels.Home;

public class HomePageViewModel : BaseNotifyObject
{
    private string _profileName = string.Empty;
    private string _balance = string.Empty;
    private string _totalActualAmount = string.Empty;
    private string _totalPlannedAmount = string.Empty;
    private string _todayActualAmount = string.Empty;
    private string _todayPlannedAmount = string.Empty;
    private string _balanceForTomorrow = string.Empty;
    private string _mainActualAmount = string.Empty;
    private string _mainPlannedAmount = string.Empty;
    private string _secondaryActualAmount = string.Empty;
    private string _secondaryPlannedAmount = string.Empty;
    private string _savedActualAmount = string.Empty;
    private string _savedPlannedAmount = string.Empty;

    private float _totalBalanceRatio;
    private float _todayBalanceRatio;
    private float _mainExpensesRatio;
    private float _secondaryExpensesRatio;
    private float _savedRatio;

    private string _profileCurrency = string.Empty;
    private string _dateFrom = string.Empty;
    private string _dateTo = string.Empty;

    private bool _isDisplayNoCategories;
    private bool _isRefreshing = true;
    private bool _isShowSavedAmounts;

    private readonly ICalculationService _calculationService;

    public Guid ProfileId { get; private set; }

    public string ProfileName
    {
        get => _profileName;
        set => SetProperty(ref _profileName, value);
    }

    public string Balance
    {
        get => _balance;
        set => SetProperty(ref _balance, value);
    }

    public string TotalActualAmount
    {
        get => _totalActualAmount;
        set => SetProperty(ref _totalActualAmount, value);
    }

    public string TotalPlannedAmount
    {
        get => _totalPlannedAmount;
        set => SetProperty(ref _totalPlannedAmount, value);
    }

    public string TodayActualAmount
    {
        get => _todayActualAmount;
        set => SetProperty(ref _todayActualAmount, value);
    }

    public string TodayPlannedAmount
    {
        get => _todayPlannedAmount;
        set => SetProperty(ref _todayPlannedAmount, value);
    }

    public string BalanceForTomorrow
    {
        get => _balanceForTomorrow;
        set => SetProperty(ref _balanceForTomorrow, value);
    }

    public string MainActualAmount
    {
        get => _mainActualAmount;
        set => SetProperty(ref _mainActualAmount, value);
    }

    public string MainPlannedAmount
    {
        get => _mainPlannedAmount;
        set => SetProperty(ref _mainPlannedAmount, value);
    }

    public string SecondaryActualAmount
    {
        get => _secondaryActualAmount;
        set => SetProperty(ref _secondaryActualAmount, value);
    }

    public string SecondaryPlannedAmount
    {
        get => _secondaryPlannedAmount;
        set => SetProperty(ref _secondaryPlannedAmount, value);
    }

    public string SavedActualAmount
    {
        get => _savedActualAmount;
        set => SetProperty(ref _savedActualAmount, value);
    }

    public string SavedPlannedAmount
    {
        get => _savedPlannedAmount;
        set => SetProperty(ref _savedPlannedAmount, value);
    }

    public float TotalBalanceRatio
    {
        get => _totalBalanceRatio;
        private set => SetProperty(ref _totalBalanceRatio, value);
    }

    public float TodayBalanceRatio
    {
        get => _todayBalanceRatio;
        private set => SetProperty(ref _todayBalanceRatio, value);
    }

    public float MainExpensesRatio
    {
        get => _mainExpensesRatio;
        private set => SetProperty(ref _mainExpensesRatio, value);
    }

    public float SecondaryExpensesRatio
    {
        get => _secondaryExpensesRatio;
        private set => SetProperty(ref _secondaryExpensesRatio, value);
    }

    public float SavedRatio
    {
        get => _savedRatio;
        private set => SetProperty(ref _savedRatio, value);
    }

    public string DateFrom
    {
        get => _dateFrom;
        private set => SetProperty(ref _dateFrom, value);
    }

    public string DateTo
    {
        get => _dateTo;
        private set => SetProperty(ref _dateTo, value);
    }

    public bool IsDisplayNoCategories
    {
        get => _isDisplayNoCategories;
        private set => SetProperty(ref _isDisplayNoCategories, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public bool IsShowSavedAmounts
    {
        get => _isShowSavedAmounts;
        set => SetProperty(ref _isShowSavedAmounts, value);
    }

    public ICommand RefreshCommand { get; private set; }

    public readonly ObservableCollection<SavedAmountModel> SavedAmounts = [];
    public readonly ObservableCollection<CategoryExpenseModel> CategoriesExpenses = [];

    public HomePageViewModel(ICalculationService calculationService)
    {
        _calculationService = calculationService;
        RefreshCommand = new Command(
            execute: Refresh,
            canExecute: () => !IsRefreshing);
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
            return;
        }

        _profileCurrency = profile.Settings.Currency.Symbol;

        ProfileId = profile.Id;
        ProfileName = profile.Name;
        Balance = NumberUtils.RoundDecimalMoney(profile.Balance, _profileCurrency);
        DateFrom = profile.BillingPeriod.DateFrom.ToShortDateString();
        DateTo = profile.BillingPeriod.DateTo.ToShortDateString();

        InitializeExpenses(profile.Expenses);
        CategoriesExpenses.Clear();

        if (profile.CategoriesExpenses.Count > 0)
        {
            IsDisplayNoCategories = false;
            InitializeCategoriesExpenses(profile.CategoriesExpenses);
        }
        else
        {
            IsDisplayNoCategories = true;
        }

        SavedAmounts.Clear();
        IsShowSavedAmounts = profile.SavedAmounts.Count > 0;

        foreach (var savedAmount in profile.SavedAmounts)
        {
            var savedAmountModel = SavedAmountModel.FromDomain(savedAmount);
            SavedAmounts.Add(savedAmountModel);
        }

        IsRefreshing = false;
    }

    private void InitializeExpenses(ProfileExpenses expenses)
    {
        TotalActualAmount = NumberUtils.RoundDecimalMoney(expenses.TotalBalance.ActualAmount, _profileCurrency);
        TotalPlannedAmount = NumberUtils.RoundDecimalMoney(expenses.TotalBalance.PlannedAmount, _profileCurrency);
        TodayActualAmount = NumberUtils.RoundDecimalMoney(expenses.TodayBalance.ActualAmount, _profileCurrency);
        TodayPlannedAmount = NumberUtils.RoundDecimalMoney(expenses.TodayBalance.PlannedAmount, _profileCurrency);
        BalanceForTomorrow = NumberUtils.RoundDecimalMoney(expenses.TomorrowBalance, _profileCurrency);
        MainActualAmount = NumberUtils.RoundDecimalMoney(expenses.Main.ActualAmount, _profileCurrency);
        MainPlannedAmount = NumberUtils.RoundDecimalMoney(expenses.Main.PlannedAmount, _profileCurrency);
        SecondaryActualAmount = NumberUtils.RoundDecimalMoney(expenses.Secondary.ActualAmount, _profileCurrency);
        SecondaryPlannedAmount = NumberUtils.RoundDecimalMoney(expenses.Secondary.PlannedAmount, _profileCurrency);
        SavedActualAmount = NumberUtils.RoundDecimalMoney(expenses.Saved.ActualAmount, _profileCurrency);
        SavedPlannedAmount = NumberUtils.RoundDecimalMoney(expenses.Saved.PlannedAmount, _profileCurrency);

        InitializeExpenseRatios(expenses);
    }

    private void InitializeExpenseRatios(ProfileExpenses expenses)
    {
        TotalBalanceRatio = GetExpenseRatio(expenses.TotalBalance);
        TodayBalanceRatio = GetExpenseRatio(expenses.TodayBalance);
        MainExpensesRatio = GetExpenseRatio(expenses.Main);
        SecondaryExpensesRatio = GetExpenseRatio(expenses.Secondary);
        SavedRatio = GetExpenseRatio(expenses.Saved);
    }

    private void InitializeCategoriesExpenses(List<ProfileCategory> categories)
    {
        foreach (var category in categories)
        {
            decimal? plannedAmount = null;
            float? ratio = null;

            var showRatio = category.PlannedAmount is not (null or 0);

            if (showRatio)
            {
                plannedAmount = category.PlannedAmount;
                ratio = GetCategoryRatio(category);
            }

            var categoryExpense = new CategoryExpenseModel(
                Id: category.Id,
                Name: category.Name,
                ActualAmount: NumberUtils.RoundDecimal(category.ActualAmount),
                IsShowRatio: showRatio,
                PlannedAmount: plannedAmount,
                Ratio: ratio,
                _profileCurrency);

            CategoriesExpenses.Add(categoryExpense);
        }
    }

    private static float GetExpenseRatio(ProfileExpense expense)
    {
        if (expense.PlannedAmount == 0)
        {
            return 1;
        }

        return NumberUtils.GetFloatRatio(expense.ActualAmount, expense.PlannedAmount);
    }

    private static float GetCategoryRatio(ProfileCategory category)
    {
        if (category.PlannedAmount is null or 0)
        {
            return 1;
        }

        return NumberUtils.GetFloatRatio(category.ActualAmount, (decimal)category.PlannedAmount);
    }
}