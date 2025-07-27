using System.Collections.ObjectModel;
using System.Globalization;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Constants;
using Profitocracy.Mobile.Models.Categories;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;

namespace Profitocracy.Mobile.ViewModels.Transactions;

public class TransactionsFiltersPageViewModel : BaseNotifyObject
{
    private static readonly string AnyType = AppResources.TransactionsFilters_Pickers_Any;
    private static readonly string MainSpendingType = AppResources.Transactions_Main;
    private static readonly string SecondarySpendingType = AppResources.Transactions_Secondary;
    private static readonly string SavedSpendingType = AppResources.Transactions_Saved;
    private static readonly string IncomeSpendingType = AppResources.Transactions_Income;
    private static readonly string ExpenseSpendingType = AppResources.Transactions_Expense;

    private static readonly CategoryModel AnyCategory = new()
    {
        Id = Guid.NewGuid(),
        ProfileId = Guid.Empty,
        Name = AppResources.TransactionsFilters_Pickers_Any
    };

    private Currency _selectedCurrency = AvailableCurrencies[0];

    private bool _isSearchByCurrency;
    private bool _isSearchByAmount;
    private bool _isGreaterThan;
    private bool _isLessThan;
    private bool _isShowSpendingType;

    private DateTime? _profileDateFrom;
    private DateTime? _profileDateTo;
    private DateTime _fromDate;
    private DateTime _toDate;
    private string _description = string.Empty;
    private string _displayAmount = string.Empty;

    private int _selectedTransactionTypeIndex = -1;
    private int _selectedSpendingTypeIndex = -1;
    private CategoryModel? _selectedCategory;

    private readonly IProfileRepository _profileRepository;
    private readonly ICategoryRepository _categoryRepository;

    public TransactionsFiltersPageViewModel(
        ICategoryRepository categoryRepository,
        IProfileRepository profileRepository)
    {
        _categoryRepository = categoryRepository;
        _profileRepository = profileRepository;

        foreach (var currency in Currency.AvailableCurrencies.All.Values)
        {
            AvailableCurrencies.Add(currency);
        }

        Reset();
    }

    public static List<string> TransactionTypes { get; } = [
        AnyType,
        IncomeSpendingType,
        ExpenseSpendingType
    ];

    public static List<string> SpendingTypes { get; } = [
        AnyType,
        MainSpendingType,
        SecondarySpendingType,
        SavedSpendingType
    ];

    public static List<Currency> AvailableCurrencies { get; } = Currency.AvailableCurrencies.All.Values.ToList();
    public ObservableCollection<CategoryModel> AvailableCategories { get; } = [];

    public bool IsApplied { get; private set; }

    public DateTime FromDate
    {
        get => _fromDate;
        set
        {
            var newValue = new DateTime(
                value.Year,
                value.Month,
                value.Day,
                hour: TimeConstants.MinHours,
                minute: TimeConstants.MinMinutes,
                second: TimeConstants.MinSeconds);

            SetProperty(ref _fromDate, newValue);
        }
    }

    public DateTime ToDate
    {
        get => _toDate;
        set
        {
            var newValue = new DateTime(
                value.Year,
                value.Month,
                value.Day,
                hour: TimeConstants.MaxHours,
                minute: TimeConstants.MaxMinutes,
                second: TimeConstants.MaxSeconds,
                millisecond: TimeConstants.MaxMilliseconds);

            SetProperty(ref _toDate, newValue);
        }
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public bool IsSearchByCurrency
    {
        get => _isSearchByCurrency;
        set => SetProperty(ref _isSearchByCurrency, value);
    }

    public Currency SelectedCurrency
    {
        get => _selectedCurrency;
        set => SetProperty(ref _selectedCurrency, value);
    }

    public bool IsSearchByAmount
    {
        get => _isSearchByAmount;
        set => SetProperty(ref _isSearchByAmount, value);
    }

    public bool IsLessThan
    {
        get => _isLessThan;
        set
        {
            if (value == _isLessThan)
            {
                return;
            }

            _isGreaterThan = !value;
            SetProperty(ref _isLessThan, value);
        }
    }

    public bool IsGreaterThan
    {
        get => _isGreaterThan;
        set
        {
            if (value == _isGreaterThan)
            {
                return;
            }

            _isLessThan = !value;
            SetProperty(ref _isGreaterThan, value);
        }
    }

    public CategoryModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public int SelectedTransactionTypeIndex
    {
        get => _selectedTransactionTypeIndex;
        set
        {
            IsShowSpendingType = value == 2;
            SetProperty(ref _selectedTransactionTypeIndex, value);
        }
    }

    public bool IsShowSpendingType
    {
        get => _isShowSpendingType;
        set => SetProperty(ref _isShowSpendingType, value);
    }

    public int SelectedSpendingTypeIndex
    {
        get => _selectedSpendingTypeIndex;
        set => SetProperty(ref _selectedSpendingTypeIndex, value);
    }

    public decimal Amount { get; private set; }

    public string DisplayAmount
    {
        get => _displayAmount;
        set => SetProperty(ref _displayAmount, value);
    }

    public void CopyFrom(TransactionsFiltersPageViewModel other)
    {
        FromDate = other.FromDate;
        ToDate = other.ToDate;
        IsGreaterThan = other.IsGreaterThan;
        IsLessThan = other.IsLessThan;
        IsSearchByAmount = other.IsSearchByAmount;
        IsSearchByCurrency = other.IsSearchByCurrency;
        SelectedCurrency = other.SelectedCurrency;
        SelectedTransactionTypeIndex = other.SelectedTransactionTypeIndex;
        SelectedSpendingTypeIndex = other.SelectedSpendingTypeIndex;
        SelectedCategory = other.SelectedCategory;
        Description = other.Description;
        DisplayAmount = other.Amount.ToString(CultureInfo.InvariantCulture);
    }

    public void Reset()
    {
        IsLessThan = true;

        IsApplied = false;
        IsGreaterThan = false;
        IsSearchByAmount = false;
        IsSearchByCurrency = false;

        SelectedCurrency = AvailableCurrencies[0];
        SelectedTransactionTypeIndex = 0;
        SelectedSpendingTypeIndex = 0;
        SelectedCategory = AvailableCategories.Count > 0 ? AvailableCategories[0] : null;

        DisplayAmount = string.Empty;
        Description = string.Empty;

        var currentDate = DateTime.Now;

        FromDate = _profileDateFrom ?? new DateTime(currentDate.Year, currentDate.Month, 1);
        ToDate = _profileDateTo ?? new DateTime(
            currentDate.Year,
            currentDate.Month,
            day: DateTime.DaysInMonth(currentDate.Year, currentDate.Month),
            hour: TimeConstants.MaxHours,
            minute: TimeConstants.MaxMinutes,
            second: TimeConstants.MaxSeconds,
            millisecond: TimeConstants.MaxMilliseconds);
    }

    public async Task Initialize()
    {
        var profile = await _profileRepository.GetCurrentProfile();

        if (profile is null)
        {
            throw new Exception(AppResources.CommonError_GetCurrentProfile);
        }

        var categories = await _categoryRepository.GetAllByProfileId(profile.Id);

        AvailableCategories.Add(AnyCategory);

        foreach (var category in categories)
        {
            AvailableCategories.Add(CategoryModel.FromDomain(category));
        }

        if (SelectedCategory is not null && SelectedCategory.Id != AnyCategory.Id)
        {
            SelectedCategory = AvailableCategories
                .FirstOrDefault(
                    c => c.Id == SelectedCategory.Id,
                    AvailableCategories[0]);
        }
        else
        {
            SelectedCategory = AvailableCategories[0];
        }

        if (SelectedTransactionTypeIndex == -1)
        {
            SelectedTransactionTypeIndex = 0;
        }

        if (SelectedSpendingTypeIndex == -1)
        {
            SelectedSpendingTypeIndex = 0;
        }

        _profileDateFrom = profile.BillingPeriod.DateFrom;
        _profileDateTo = profile.BillingPeriod.DateTo;

        FromDate = (DateTime)_profileDateFrom;
        ToDate = (DateTime)_profileDateTo;

        OnPropertyChanged(nameof(SelectedCategory));
        OnPropertyChanged(nameof(SelectedTransactionTypeIndex));
        OnPropertyChanged(nameof(SelectedSpendingTypeIndex));
        OnPropertyChanged(nameof(SelectedCurrency));
    }

    public void Apply()
    {
        var amount = 0m;

        if (IsSearchByAmount && !NumberUtils.TryParseDecimal(DisplayAmount, out amount))
        {
            throw new InvalidCastException(AppResources.CommonError_AmountNumber);
        }

        Amount = amount;

        if (SelectedCategory is not null && SelectedCategory.Id == AnyCategory.Id)
        {
            SelectedCategory = null;
        }

        if (SelectedTransactionTypeIndex != 2)
        {
            SelectedSpendingTypeIndex = 0;
        }

        FromDate = new DateTime(
            FromDate.Year,
            FromDate.Month,
            FromDate.Day,
            TimeConstants.MinHours,
            TimeConstants.MinMinutes,
            TimeConstants.MinSeconds,
            TimeConstants.MinMilliseconds);

        ToDate = new DateTime(
            ToDate.Year,
            ToDate.Month,
            ToDate.Day,
            TimeConstants.MaxHours,
            TimeConstants.MaxMinutes,
            TimeConstants.MaxSeconds,
            TimeConstants.MaxMilliseconds);

        IsApplied = true;
    }
}
