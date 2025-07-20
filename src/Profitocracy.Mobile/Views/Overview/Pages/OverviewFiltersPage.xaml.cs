using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.ViewModels.Overview;

namespace Profitocracy.Mobile.Views.Overview.Pages;

public partial class OverviewFiltersPage : BaseContentPage
{
    private readonly OverviewFiltersPageViewModel _viewModel;

    public OverviewFiltersPage(OverviewFiltersPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }

    private void ModalHeaderView_OnCloseClicked(object? sender, EventArgs e)
    {
        ProcessAction(Navigation.PopModalAsync);
    }

    private void ResetFiltersButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(() =>
        {
            _viewModel.Reset();
            return Task.CompletedTask;
        });
    }

    private void ApplyFiltersButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            _viewModel.Apply();
            await Navigation.PopModalAsync();
        });
    }
}

