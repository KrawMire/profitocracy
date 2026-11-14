using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Constants;
using Profitocracy.Mobile.Models.Categories;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;
using Profitocracy.Mobile.Views.Overview.Controls;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Profitocracy.Mobile.ViewModels.Overview;

/// <summary>
/// Period the insights are calculated for
/// </summary>
public enum InsightsPeriod
{
    Week,
    Month,
    Year,
    Custom
}

public class OverviewPageViewModel : BaseNotifyObject
{
    private const int MaxDailyBars = 62;
    private const int MaxNoteCategories = 2;

    private readonly ICalculationService _calculationService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    private InsightsPeriod _period = InsightsPeriod.Month;
    private DateTime _dateFrom;
    private DateTime _dateTo;
    private string _dateRange = string.Empty;

    private bool _hasIncome;
    private string _keptPercent = string.Empty;
    private string _keptText = string.Empty;
    private string _income = string.Empty;
    private string _spent = string.Empty;
    private string _putAside = string.Empty;
    private double _incomeRatio;
    private double _spentRatio;
    private double _putAsideRatio;

    private string _usedTotal = string.Empty;
    private double _mainShare;
    private double _secondaryShare;
    private double _savedShare;
    private string _mainAmount = string.Empty;
    private string _secondaryAmount = string.Empty;
    private string _savedAmount = string.Empty;
    private string _mainPercent = string.Empty;
    private string _secondaryPercent = string.Empty;
    private string _savedPercent = string.Empty;

    private string _barsTitle = AppResources.Insights_DayByDay;
    private IReadOnlyList<SpendingBar> _bars = [];
    private decimal _barsAverage;
    private string _barsAverageText = string.Empty;
    private string _barsNote = string.Empty;
    private bool _hasClippedBars;

    private bool _hasLimits;

    public OverviewPageViewModel(
        ICalculationService calculationService,
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository)
    {
        _calculationService = calculationService;
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    public ObservableCollection<CategoryLimitModel> Limits { get; } = [];

    public InsightsPeriod Period
    {
        get => _period;
        private set
        {
            SetProperty(ref _period, value);
            OnPropertyChanged(nameof(IsWeek));
            OnPropertyChanged(nameof(IsMonth));
            OnPropertyChanged(nameof(IsYear));
            OnPropertyChanged(nameof(IsCustom));
        }
    }

    public bool IsWeek => Period == InsightsPeriod.Week;
    public bool IsMonth => Period == InsightsPeriod.Month;
    public bool IsYear => Period == InsightsPeriod.Year;
    public bool IsCustom => Period == InsightsPeriod.Custom;

    public DateTime DateFrom
    {
        get => _dateFrom;
        private set => SetProperty(ref _dateFrom, value);
    }

    public DateTime DateTo
    {
        get => _dateTo;
        private set => SetProperty(ref _dateTo, value);
    }

    /// <summary>
    /// Covered days of the period, e.g. "01–11.03.2025".
    /// </summary>
    public string DateRange
    {
        get => _dateRange;
        private set => SetProperty(ref _dateRange, value);
    }

    public bool HasIncome
    {
        get => _hasIncome;
        private set
        {
            SetProperty(ref _hasIncome, value);
            OnPropertyChanged(nameof(HasNoIncome));
        }
    }

    public bool HasNoIncome => !HasIncome;

    public string KeptPercent
    {
        get => _keptPercent;
        private set => SetProperty(ref _keptPercent, value);
    }

    public string KeptText
    {
        get => _keptText;
        private set => SetProperty(ref _keptText, value);
    }

    public string Income
    {
        get => _income;
        private set => SetProperty(ref _income, value);
    }

    public string Spent
    {
        get => _spent;
        private set => SetProperty(ref _spent, value);
    }

    public string PutAside
    {
        get => _putAside;
        private set => SetProperty(ref _putAside, value);
    }

    public double IncomeRatio
    {
        get => _incomeRatio;
        private set => SetProperty(ref _incomeRatio, value);
    }

    public double SpentRatio
    {
        get => _spentRatio;
        private set => SetProperty(ref _spentRatio, value);
    }

    public double PutAsideRatio
    {
        get => _putAsideRatio;
        private set => SetProperty(ref _putAsideRatio, value);
    }

    /// <summary>
    /// Spent and saved in the period, in the middle of the gauge.
    /// </summary>
    public string UsedTotal
    {
        get => _usedTotal;
        private set => SetProperty(ref _usedTotal, value);
    }

    public double MainShare
    {
        get => _mainShare;
        private set => SetProperty(ref _mainShare, value);
    }

    public double SecondaryShare
    {
        get => _secondaryShare;
        private set => SetProperty(ref _secondaryShare, value);
    }

    public double SavedShare
    {
        get => _savedShare;
        private set => SetProperty(ref _savedShare, value);
    }

    public string MainAmount
    {
        get => _mainAmount;
        private set => SetProperty(ref _mainAmount, value);
    }

    public string SecondaryAmount
    {
        get => _secondaryAmount;
        private set => SetProperty(ref _secondaryAmount, value);
    }

    public string SavedAmount
    {
        get => _savedAmount;
        private set => SetProperty(ref _savedAmount, value);
    }

    public string MainPercent
    {
        get => _mainPercent;
        private set => SetProperty(ref _mainPercent, value);
    }

    public string SecondaryPercent
    {
        get => _secondaryPercent;
        private set => SetProperty(ref _secondaryPercent, value);
    }

    public string SavedPercent
    {
        get => _savedPercent;
        private set => SetProperty(ref _savedPercent, value);
    }

    public string BarsTitle
    {
        get => _barsTitle;
        private set => SetProperty(ref _barsTitle, value);
    }

    public IReadOnlyList<SpendingBar> Bars
    {
        get => _bars;
        private set => SetProperty(ref _bars, value);
    }

    public decimal BarsAverage
    {
        get => _barsAverage;
        private set => SetProperty(ref _barsAverage, value);
    }

    public string BarsAverageText
    {
        get => _barsAverageText;
        private set => SetProperty(ref _barsAverageText, value);
    }

    public string BarsNote
    {
        get => _barsNote;
        private set => SetProperty(ref _barsNote, value);
    }

    /// <summary>
    /// Some bars are taller than the scale and are cut short.
    /// </summary>
    public bool HasClippedBars
    {
        get => _hasClippedBars;
        private set => SetProperty(ref _hasClippedBars, value);
    }

    public bool HasLimits
    {
        get => _hasLimits;
        private set
        {
            SetProperty(ref _hasLimits, value);
            OnPropertyChanged(nameof(HasNoLimits));
        }
    }

    public bool HasNoLimits => !HasLimits;

    public async Task Initialize(OverviewFiltersPageViewModel filters)
    {
        if (filters.IsApplied)
        {
            Period = InsightsPeriod.Custom;
            DateFrom = filters.DateFrom;
            DateTo = filters.DateTo;
        }
        else if (Period == InsightsPeriod.Custom)
        {
            Period = InsightsPeriod.Month;
        }

        await Update();
    }

    public Task ChangePeriod(InsightsPeriod period)
    {
        Period = period;
        return Update();
    }

    private async Task Update()
    {
        var profile = await _calculationService.GetCurrentProfile();

        if (profile is null)
        {
            return;
        }

        var today = DateTime.Today;

        switch (Period)
        {
            case InsightsPeriod.Week:
                var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
                DateFrom = weekStart;
                DateTo = EndOfDay(weekStart.AddDays(6));
                break;
            case InsightsPeriod.Month:
                DateFrom = profile.BillingPeriod.DateFrom;
                DateTo = profile.BillingPeriod.DateTo;
                break;
            case InsightsPeriod.Year:
                DateFrom = new DateTime(today.Year, 1, 1);
                DateTo = EndOfDay(new DateTime(today.Year, 12, 31));
                break;
            case InsightsPeriod.Custom:
            default:
                break;
        }

        var lastShownDay = DateTo.Date > today && DateFrom.Date <= today ? today : DateTo.Date;
        DateRange = FormatDateRange(DateFrom.Date, lastShownDay);

        var symbol = profile.Settings.Currency.Symbol;
        var transactions = await _transactionRepository.GetForPeriod(profile.Id, DateFrom, DateTo);
        var categories = await _categoryRepository.GetAllByProfileId(profile.Id);

        var income = Sum(transactions, t => t.Type == TransactionType.Income);
        var spent = Sum(transactions, t => t.Type == TransactionType.Expense && t.SpendingType != SpendingType.Saved);
        var main = Sum(transactions, t => t.Type == TransactionType.Expense && t.SpendingType == SpendingType.Main);
        var secondary = Sum(transactions, t => t.Type == TransactionType.Expense && t.SpendingType == SpendingType.Secondary);
        var saved = Sum(transactions, t => t.Type == TransactionType.Expense && t.SpendingType == SpendingType.Saved);

        // The month of the profile also carries the balance of the previous one,
        // so its budget matches the one on the home page
        var budget = Period == InsightsPeriod.Month
            ? profile.Expenses.TotalBalance.PlannedAmount
            : income;

        InitializeKept(budget, income, spent, saved, symbol);
        InitializeSplit(main, secondary, saved, symbol);
        InitializeBars(transactions, budget, symbol);
        InitializeLimits(transactions, categories, symbol);
    }

    private void InitializeKept(decimal budget, decimal income, decimal spent, decimal saved, string symbol)
    {
        HasIncome = budget > 0;

        var kept = HasIncome ? (budget - spent - saved) / budget : 0;
        var keptPercent = FormatPercent(kept);

        KeptPercent = HasIncome ? keptPercent : "—";
        KeptText = HasIncome
            ? string.Format(
                AppResources.Insights_StillYours,
                NumberUtils.FormatMoney(budget - spent - saved, symbol),
                NumberUtils.FormatMoney(budget, symbol))
            : AppResources.Insights_NoIncome;

        Income = NumberUtils.FormatMoney(income, symbol);
        Spent = NumberUtils.FormatMoney(spent, symbol);
        PutAside = NumberUtils.FormatMoney(saved, symbol);

        IncomeRatio = HasIncome ? (double)(income / budget) : 0;
        SpentRatio = HasIncome ? (double)(spent / budget) : 0;
        PutAsideRatio = HasIncome ? (double)(saved / budget) : 0;
    }

    private void InitializeSplit(decimal main, decimal secondary, decimal saved, string symbol)
    {
        var used = main + secondary + saved;

        UsedTotal = NumberUtils.FormatMoney(used, symbol);

        var mainShare = used > 0 ? main / used : 0;
        var secondaryShare = used > 0 ? secondary / used : 0;
        var savedShare = used > 0 ? saved / used : 0;

        MainShare = (double)mainShare;
        SecondaryShare = (double)secondaryShare;
        SavedShare = (double)savedShare;

        MainAmount = NumberUtils.FormatMoney(main, symbol);
        SecondaryAmount = NumberUtils.FormatMoney(secondary, symbol);
        SavedAmount = NumberUtils.FormatMoney(saved, symbol);

        MainPercent = FormatPercent(mainShare);
        SecondaryPercent = FormatPercent(secondaryShare);
        SavedPercent = FormatPercent(savedShare);
    }

    private void InitializeBars(List<Transaction> transactions, decimal budget, string symbol)
    {
        var today = DateTime.Today;
        var days = (DateTo.Date - DateFrom.Date).Days + 1;
        var isDaily = days <= MaxDailyBars;
        var step = isDaily ? 1 : 7;
        var count = (days + step - 1) / step;

        var spendings = transactions
            .Where(t => t.Type == TransactionType.Expense && t.SpendingType != SpendingType.Saved)
            .ToList();

        var bars = new List<SpendingBar>(count);
        var barTransactions = new List<List<Transaction>>(count);

        for (var i = 0; i < count; i++)
        {
            var start = DateFrom.Date.AddDays(i * step);
            var end = start.AddDays(step);
            var inBar = spendings.Where(t => t.Timestamp >= start && t.Timestamp < end).ToList();
            var isToday = start <= today && today < end;

            var label = i == 0
                ? start.ToString("d MMM", CultureInfo.CurrentCulture)
                : isToday
                    ? $"{start.Day} · {AppResources.Insights_TodayMark}"
                    : i == count - 1
                        ? start.Day.ToString(CultureInfo.CurrentCulture)
                        : string.Empty;

            bars.Add(new SpendingBar(inBar.Sum(t => t.Amount), start > today, label));
            barTransactions.Add(inBar);
        }

        BarsTitle = isDaily ? AppResources.Insights_DayByDay : AppResources.Insights_WeekByWeek;
        BarsAverage = count > 0 ? budget / count : 0;
        BarsAverageText = BarsAverage > 0
            ? string.Format(
                isDaily ? AppResources.Insights_PerDay : AppResources.Insights_PerWeek,
                NumberUtils.FormatMoney(BarsAverage, symbol, decimals: 0))
            : string.Empty;

        HasClippedBars = BarsAverage > 0 && bars.Any(b => b.Amount > BarsAverage * SpendingBarsView.CapAverages);

        var biggest = bars.Select((b, i) => (Bar: b, Index: i)).MaxBy(b => b.Bar.Amount);
        BarsNote = biggest.Bar is not null && biggest.Bar.Amount > BarsAverage && BarsAverage > 0
            ? $"{NumberUtils.FormatMoneyCompact(biggest.Bar.Amount, symbol)} · {DescribeSpending(barTransactions[biggest.Index])}"
            : string.Empty;

        Bars = bars;
    }

    private void InitializeLimits(List<Transaction> transactions, List<Core.Domain.Model.Categories.Category> categories, string symbol)
    {
        var periodDays = (DateTo.Date - DateFrom.Date).Days + 1;
        var monthDays = DateTime.DaysInMonth(DateFrom.Year, DateFrom.Month);

        // Limits are monthly, other periods get a proportional limit
        var limitScale = Period == InsightsPeriod.Month ? 1m : (decimal)periodDays / monthDays;

        var rows = categories
            .Select(c => (
                c.Name,
                Actual: transactions
                    .Where(t => t.Type == TransactionType.Expense && t.Category?.Id == c.Id)
                    .Sum(t => t.Amount),
                Limit: c.PlannedAmount is > 0 ? Math.Round(c.PlannedAmount.Value * limitScale, 2) : (decimal?)null))
            .Where(r => r.Actual > 0 || r.Limit is not null)
            .OrderByDescending(r => r.Actual)
            .ToList();

        var scale = rows.Count > 0
            ? rows.Max(r => Math.Max(r.Actual, r.Limit ?? 0))
            : 0;

        Limits.Clear();

        foreach (var row in rows)
        {
            Limits.Add(new CategoryLimitModel
            {
                Name = row.Name,
                Actual = NumberUtils.FormatMoney(row.Actual, symbol),
                Limit = row.Limit is null
                    ? AppResources.Insights_NoLimitLower
                    : string.Format(AppResources.Insights_OfLimit, NumberUtils.FormatMoneyCompact(row.Limit.Value, symbol)),
                Progress = scale > 0 ? (double)(row.Actual / scale) : 0,
                Mark = row.Limit is null || scale <= 0 ? -1 : (double)(row.Limit.Value / scale),
                IsOver = row.Limit is not null && row.Actual > row.Limit,
            });
        }

        HasLimits = Limits.Count > 0;
    }

    /// <summary>
    /// What the money of a bar was spent on: its categories, or descriptions without them.
    /// </summary>
    private static string DescribeSpending(List<Transaction> transactions)
    {
        var names = transactions
            .OrderByDescending(t => t.Amount)
            .Select(t => t.Category?.Name ?? t.Description)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct()
            .Take(MaxNoteCategories);

        return string.Join(" + ", names);
    }

    private static decimal Sum(List<Transaction> transactions, Func<Transaction, bool> predicate)
        => transactions.Where(predicate).Sum(t => t.Amount);

    /// <summary>
    /// Covered days, e.g. "1–11 March", "24 February – 2 March" or "1 January 2025 – 11 March 2026".
    /// </summary>
    private static string FormatDateRange(DateTime from, DateTime to)
    {
        var culture = CultureInfo.CurrentCulture;

        if (from.Year != to.Year)
        {
            return $"{from.ToString("d MMM yyyy", culture)} – {to.ToString("d MMM yyyy", culture)}";
        }

        if (from.Month != to.Month)
        {
            return $"{from.ToString("d MMM", culture)} – {to.ToString("d MMM", culture)}";
        }

        return from.Day == to.Day
            ? to.ToString("d MMMM", culture)
            : $"{from.Day}–{to.ToString("d MMMM", culture)}";
    }

    private static string FormatPercent(decimal ratio)
        => ratio.ToString("P0", CultureInfo.CurrentCulture);

    private static DateTime EndOfDay(DateTime date)
        => new(
            date.Year,
            date.Month,
            date.Day,
            TimeConstants.MaxHours,
            TimeConstants.MaxMinutes,
            TimeConstants.MaxSeconds,
            TimeConstants.MaxMilliseconds);
}
