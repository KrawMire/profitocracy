using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Constants;

namespace Profitocracy.Mobile.ViewModels.Overview;

public class OverviewFiltersPageViewModel : BaseNotifyObject
{
    private DateTime _dateFrom;
    private DateTime _dateTo;

    public OverviewFiltersPageViewModel()
    {
        Reset();
    }

    public bool IsApplied { get; private set; }

    public DateTime DateFrom
    {
        get => _dateFrom;
        set => SetProperty(ref _dateFrom, value);
    }

    public DateTime DateTo
    {
        get => _dateTo;
        set => SetProperty(ref _dateTo, value);
    }

    public void CopyFrom(OverviewFiltersPageViewModel viewModel)
    {
        DateFrom = viewModel.DateFrom;
        DateTo = viewModel.DateTo;
    }

    public void Reset()
    {
        var currentDate = DateTime.Now;

        IsApplied = false;

        DateFrom = new DateTime(currentDate.Year, currentDate.Month, 1);
        DateTo = new DateTime(
            currentDate.Year,
            currentDate.Month,
            day: DateTime.DaysInMonth(currentDate.Year, currentDate.Month),
            hour: TimeConstants.MaxHours,
            minute: TimeConstants.MaxMinutes,
            second: TimeConstants.MaxSeconds,
            millisecond: TimeConstants.MaxMilliseconds);
    }

    public void Apply()
    {
        DateFrom = new DateTime(
            DateFrom.Year,
            DateFrom.Month,
            DateFrom.Day,
            TimeConstants.MinHours,
            TimeConstants.MinMinutes,
            TimeConstants.MinSeconds,
            TimeConstants.MinMilliseconds);

        DateTo = new DateTime(
            DateTo.Year,
            DateTo.Month,
            DateTo.Day,
            TimeConstants.MaxHours,
            TimeConstants.MaxMinutes,
            TimeConstants.MaxSeconds,
            TimeConstants.MaxMilliseconds);

        IsApplied = true;
    }
}