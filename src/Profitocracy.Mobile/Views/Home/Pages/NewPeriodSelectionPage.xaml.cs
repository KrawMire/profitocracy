using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.ViewModels.Home;

namespace Profitocracy.Mobile.Views.Home.Pages;

public partial class NewPeriodSelectionPage : BaseContentPage
{
    private readonly NewPeriodSelectionPageViewModel _viewModel;

    public NewPeriodSelectionPage(NewPeriodSelectionPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }

    private void StartNewPeriodButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.StartNewPeriodAsync();
            await Navigation.PopModalAsync();
        });
    }

    private void ModalHeaderView_OnCloseClicked(object? sender, EventArgs e)
    {
        ProcessAction(Navigation.PopModalAsync);
    }
}
