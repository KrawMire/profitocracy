using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.ViewModels.Transactions;

namespace Profitocracy.Mobile.Views.Transactions.Pages;

public partial class TransactionsFiltersPage : BaseContentPage
{
    private readonly TransactionsFiltersPageViewModel _viewModel;

    public TransactionsFiltersPage(TransactionsFiltersPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;

        CategoryPicker.ItemsSource = _viewModel.AvailableCategories;
        CurrencyPicker.ItemsSource = TransactionsFiltersPageViewModel.AvailableCurrencies;
        TransactionTypePicker.ItemsSource = TransactionsFiltersPageViewModel.TransactionTypes;
        SpendingTypePicker.ItemsSource = TransactionsFiltersPageViewModel.SpendingTypes;
    }

    private void CloseButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await Navigation.PopModalAsync();
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

    private void TransactionsFiltersPage_OnLoaded(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.Initialize();
        });
    }

    private void ResetFiltersButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(() =>
        {
            _viewModel.Reset();
            return Task.CompletedTask;
        });
    }
}