using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
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
            var currentTheme = Application.Current?.RequestedTheme;
            if (currentTheme is not null)
            {
                LiveCharts.Configure(settings =>
                {
                    if (currentTheme == AppTheme.Dark)
                    {
                        settings.AddDarkTheme();
                    }
                    else
                    {
                        settings.AddLightTheme();
                    }
                });
            }

            if (_filtersViewModel.IsApplied)
            {
                _lastAppliedFiltersViewModel = _filtersViewModel;
            }

            await _viewModel.Initialize(_lastAppliedFiltersViewModel);
        });
    }

    private void PeriodButton_Clicked(object? sender, EventArgs e)
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
}
