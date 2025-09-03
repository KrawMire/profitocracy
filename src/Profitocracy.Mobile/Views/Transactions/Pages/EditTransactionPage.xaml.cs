using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.ViewModels.Transactions;

namespace Profitocracy.Mobile.Views.Transactions.Pages;

public partial class EditTransactionPage : BaseContentPage
{
	private readonly EditTransactionPageViewModel _viewModel;
	
	public EditTransactionPage(EditTransactionPageViewModel viewModel)
	{
		InitializeComponent();
		
		BindingContext = _viewModel = viewModel;
		CategoryPicker.ItemsSource = _viewModel.AvailableCategories;
		IncomeCurrencyPicker.ItemsSource = _viewModel.AvailableCurrencies;
		ExpenseCurrencyPicker.ItemsSource = _viewModel.AvailableCurrencies;
		IntervalPicker.ItemsSource = _viewModel.AvailableIntervals;
    }

    public void AddTransactionId(Guid transactionId)
    {
        _viewModel.TransactionId = transactionId;
    }

	private void EditTransactionPage_OnLoaded(object? sender, EventArgs e)
	{
		ProcessAction(async () =>
		{
			await _viewModel.Initialize();
			if (_viewModel.Category is not null)
			{
				CategoryPicker.SelectedItem = _viewModel.Category;	
			}

			if (_viewModel.SelectedInterval is not null)
			{
				IntervalPicker.SelectedItem = _viewModel.SelectedInterval;
			}
		});
	}
	
	private void CloseButton_OnClicked(object? sender, EventArgs e)
	{
		ProcessAction(async () =>
		{
			await Navigation.PopModalAsync();
		});
	}

    private void EditTransactionButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.SaveTransaction();
            await Navigation.PopModalAsync();
        });
    }

    private void IncomeButton_OnClicked(object? sender, EventArgs e)
    {
        _viewModel.TransactionType = 0;
    }

    private void ExpenseButton_OnClicked(object? sender, EventArgs e)
    {
        _viewModel.TransactionType = 1;
    }

    private void MainTypeButton_OnClicked(object? sender, EventArgs e)
    {
        _viewModel.SpendingType = 0;
    }

    private void SecondaryTypeButton_OnClicked(object? sender, EventArgs e)
    {
        _viewModel.SpendingType = 1;
    }

    private void SavedTypeButton_OnClicked(object? sender, EventArgs e)
    {
        _viewModel.SpendingType = 2;
    }
}
