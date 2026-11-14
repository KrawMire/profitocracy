using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Overview;

namespace Profitocracy.Mobile.Views.Overview.Pages;

public partial class OverviewPage : BaseContentPage
{
    private readonly OverviewPageViewModel _viewModel;
    private OverviewFiltersPageViewModel _lastAppliedFiltersViewModel;
    private OverviewFiltersPageViewModel _filtersViewModel;

    public OverviewPage(
        OverviewPageViewModel viewModel,
        OverviewFiltersPageViewModel filtersViewModel)
    {
        InitializeComponent();

        _lastAppliedFiltersViewModel = _filtersViewModel = filtersViewModel;
        BindingContext = _viewModel = viewModel;
    }

    private void OverviewPage_OnNavigatedTo(object? sender, NavigatedToEventArgs e)
    {
        ProcessAction(async () =>
        {
            if (_filtersViewModel.IsApplied)
            {
                _lastAppliedFiltersViewModel = _filtersViewModel;
            }

            await _viewModel.Initialize(_lastAppliedFiltersViewModel);
        });
    }

    private void WeekChip_OnTapped(object? sender, EventArgs e) => ChangePeriod(InsightsPeriod.Week);

    private void MonthChip_OnTapped(object? sender, EventArgs e) => ChangePeriod(InsightsPeriod.Month);

    private void YearChip_OnTapped(object? sender, EventArgs e) => ChangePeriod(InsightsPeriod.Year);

    private void CustomChip_OnTapped(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            _filtersViewModel =
                Handler?.MauiContext?.Services.GetService<OverviewFiltersPageViewModel>()
                ?? throw new ArgumentNullException(AppResources.CommonError_OpenOverviewFiltersPage);

            _filtersViewModel.CopyFrom(_lastAppliedFiltersViewModel);
            var filtersPage = new OverviewFiltersPage(_filtersViewModel);
            await Navigation.PushModalAsync(filtersPage);
        });
    }

    private void ChangePeriod(InsightsPeriod period)
    {
        ProcessAction(async () =>
        {
            // A preset period replaces the custom one
            _filtersViewModel.Reset();
            _lastAppliedFiltersViewModel.Reset();

            await _viewModel.ChangePeriod(period);
        });
    }
}
