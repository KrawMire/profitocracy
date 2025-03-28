using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.ViewModels.Overview;

namespace Profitocracy.Mobile.Views.Overview.Pages;

public partial class OverviewPage : BaseContentPage
{
    private readonly OverviewPageViewModel _viewModel;
    
    public OverviewPage(OverviewPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        CalculationTypePicker.ItemsSource = OverviewPageViewModel.DisplayCalculationTypes;
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
            
            await _viewModel.Initialize();
            CalculationTypePicker.SelectedItem = _viewModel.SelectedDisplayCalculationType;
        });
    }

    private void CalculationTypePicker_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.Initialize(true);
        });
    }
}