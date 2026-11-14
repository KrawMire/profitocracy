using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Specifications;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Constants;
using Profitocracy.Mobile.Models.Transactions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Profitocracy.Mobile.ViewModels.Transactions;

/// <summary>
/// Quick filter of the activity list
/// </summary>
public enum ActivityKind
{
    All,
    Spent,
    Income,
    Saved
}

public class TransactionsPageViewModel : BaseNotifyObject
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly ITransactionService _transactionService;

    private readonly List<TransactionModel> _transactions = [];

    private Guid? _profileId;
    private string _currencySymbol = string.Empty;
    private DateTime _month = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private ActivityKind _kind = ActivityKind.All;
    private bool _isFiltered;
    private bool _isTransactionsListEmpty;
    private string _monthTitle = string.Empty;
    private string _incomeTotal = string.Empty;
    private string _spentTotal = string.Empty;
    private string _savedTotal = string.Empty;
    private string _endText = string.Empty;

    public TransactionsPageViewModel(
        IProfileRepository profileRepository,
        ITransactionRepository transactionRepository,
        ITransactionService transactionService)
    {
        _transactionRepository = transactionRepository;
        _transactionService = transactionService;
        _profileRepository = profileRepository;
    }

    public ObservableCollection<TransactionDayModel> Days { get; } = [];

    /// <summary>
    /// Whether the list is built from the advanced filters instead of the month switcher.
    /// </summary>
    public bool IsFiltered
    {
        get => _isFiltered;
        private set
        {
            SetProperty(ref _isFiltered, value);
            OnPropertyChanged(nameof(IsMonthMode));
        }
    }

    public bool IsMonthMode => !IsFiltered;

    public ActivityKind Kind
    {
        get => _kind;
        set
        {
            if (_kind == value)
            {
                return;
            }

            SetProperty(ref _kind, value);
            OnPropertyChanged(nameof(IsAll));
            OnPropertyChanged(nameof(IsSpent));
            OnPropertyChanged(nameof(IsIncome));
            OnPropertyChanged(nameof(IsSaved));
            BuildDays();
        }
    }

    public bool IsAll => Kind == ActivityKind.All;
    public bool IsSpent => Kind == ActivityKind.Spent;
    public bool IsIncome => Kind == ActivityKind.Income;
    public bool IsSaved => Kind == ActivityKind.Saved;

    public string MonthTitle
    {
        get => _monthTitle;
        private set => SetProperty(ref _monthTitle, value);
    }

    public string IncomeTotal
    {
        get => _incomeTotal;
        private set => SetProperty(ref _incomeTotal, value);
    }

    public string SpentTotal
    {
        get => _spentTotal;
        private set => SetProperty(ref _spentTotal, value);
    }

    public string SavedTotal
    {
        get => _savedTotal;
        private set => SetProperty(ref _savedTotal, value);
    }

    public string EndText
    {
        get => _endText;
        private set => SetProperty(ref _endText, value);
    }

    public bool IsTransactionsListEmpty
    {
        get => _isTransactionsListEmpty;
        private set
        {
            SetProperty(ref _isTransactionsListEmpty, value);
            OnPropertyChanged(nameof(HasTransactions));
        }
    }

    public bool HasTransactions => !IsTransactionsListEmpty;

    public async Task Initialize(TransactionsFiltersPageViewModel filters)
    {
        var profile = await _profileRepository.GetCurrentProfile();

        if (profile is null)
        {
            throw new Exception(AppResources.CommonError_GetCurrentProfile);
        }

        _profileId = profile.Id;
        _currencySymbol = profile.Settings.Currency.Symbol;

        await InitializeTransactions(filters);
    }

    public Task ShowPreviousMonth(TransactionsFiltersPageViewModel filters)
    {
        _month = _month.AddMonths(-1);
        return InitializeTransactions(filters);
    }

    public Task ShowNextMonth(TransactionsFiltersPageViewModel filters)
    {
        _month = _month.AddMonths(1);
        return InitializeTransactions(filters);
    }

    public async Task DeleteTransaction(Guid transactionId)
    {
        var deletedId = await _transactionRepository.Delete(transactionId);

        _transactions.RemoveAll(t => t.Id == deletedId);
        BuildDays();
    }

    public Task<bool> IsTransactionInProfilePeriod(Guid transactionId)
    {
        return _transactionService.CheckTransactionInCurrentPeriod(transactionId);
    }

    private async Task InitializeTransactions(TransactionsFiltersPageViewModel filters)
    {
        if (_profileId is null)
        {
            return;
        }

        IsFiltered = filters.IsApplied;

        var specs = IsFiltered
            ? BuildSpecification(_profileId.Value, filters)
            : BuildMonthSpecification(_profileId.Value);

        MonthTitle = IsFiltered
            ? DateUtils.PeriodTitle(filters.FromDate, filters.ToDate)
            : DateUtils.MonthTitle(_month);

        var transactions = await _transactionRepository.GetFiltered(specs);

        _transactions.Clear();
        _transactions.AddRange(transactions
            .OrderByDescending(t => t.Timestamp)
            .Select(TransactionModel.FromDomain));

        var from = specs.FromDate ?? _month;
        EndText = string.Format(
            AppResources.Activity_End,
            from.ToString("d MMMM", CultureInfo.CurrentCulture));

        BuildSummary();
        BuildDays();
    }

    private void BuildSummary()
    {
        IncomeTotal = NumberUtils.FormatMoney(
            _transactions.Where(t => t.IsIncome).Sum(t => t.Amount),
            _currencySymbol);
        SpentTotal = NumberUtils.FormatMoney(
            _transactions.Where(t => t.IsSpending).Sum(t => t.Amount),
            _currencySymbol);
        SavedTotal = NumberUtils.FormatMoney(
            _transactions.Where(t => t.IsSaved).Sum(t => t.Amount),
            _currencySymbol);
    }

    private void BuildDays()
    {
        var transactions = _transactions.Where(t => Kind switch
        {
            ActivityKind.Spent => t.IsSpending,
            ActivityKind.Income => t.IsIncome,
            ActivityKind.Saved => t.IsSaved,
            _ => true
        });

        Days.Clear();

        foreach (var day in transactions.GroupBy(t => t.Timestamp.Date))
        {
            var total = day.Sum(t => t.IsIncome ? t.Amount : -t.Amount);

            Days.Add(new TransactionDayModel
            {
                Date = day.Key,
                Transactions = day.ToList(),
                Total = NumberUtils.FormatMoney(total, _currencySymbol, showPlus: true),
            });
        }

        IsTransactionsListEmpty = Days.Count == 0;
    }

    private TransactionsSpecification BuildMonthSpecification(Guid profileId)
    {
        var lastDay = DateTime.DaysInMonth(_month.Year, _month.Month);

        return new TransactionsSpecification
        {
            ProfileId = profileId,
            FromDate = _month,
            ToDate = new DateTime(
                _month.Year,
                _month.Month,
                lastDay,
                TimeConstants.MaxHours,
                TimeConstants.MaxMinutes,
                TimeConstants.MaxSeconds,
                TimeConstants.MaxMilliseconds),
        };
    }

    private static TransactionsSpecification BuildSpecification(
        Guid profileId,
        TransactionsFiltersPageViewModel filters)
    {
        TransactionType? transactionType = filters.SelectedTransactionTypeIndex switch
        {
            1 => TransactionType.Income,
            2 => TransactionType.Expense,
            _ => null
        };

        SpendingType? spendingType = filters.SelectedSpendingTypeIndex switch
        {
            1 => SpendingType.Main,
            2 => SpendingType.Secondary,
            3 => SpendingType.Saved,
            _ => null
        };

        return new TransactionsSpecification
        {
            ProfileId = profileId,
            FromDate = filters.FromDate,
            ToDate = filters.ToDate,
            CategoryId = filters.SelectedCategory?.Id,
            TransactionType = transactionType,
            SpendingType = spendingType,
            IsMultiCurrency = filters.IsSearchByCurrency,
            Description = filters.Description,
            CurrencyCode = filters.IsSearchByCurrency
                ? filters.SelectedCurrency.Code
                : null,
            IsGreaterThanAmount = filters.IsSearchByAmount && filters.IsGreaterThan,
            Amount = filters.IsSearchByAmount ? filters.Amount : null,
            Interval = filters.SelectedInterval is null || filters.SelectedInterval.Value is -1 ? null : (RecurringTransactionInterval)filters.SelectedInterval.Value
        };
    }
}
