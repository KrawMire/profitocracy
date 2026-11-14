using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Categories;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Transactions;

namespace Profitocracy.Mobile.Views.Transactions.Pages;

public partial class EditTransactionPage : BaseContentPage
{
	private readonly EditTransactionPageViewModel _viewModel;

	public EditTransactionPage(EditTransactionPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = _viewModel = viewModel;
		IntervalPicker.ItemsSource = _viewModel.AvailableIntervals;
    }

    /// <summary>
    /// Opens the page with the income type selected.
    /// </summary>
    public void StartWithIncome()
    {
        _viewModel.TransactionType = 0;
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

			if (_viewModel.SelectedInterval is not null)
			{
				IntervalPicker.SelectedItem = _viewModel.SelectedInterval;
			}

			// A new transaction starts with the amount, so the number pad is opened right away
			if (!_viewModel.IsEditing)
			{
				AmountEntry.Focus();
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

    private void CurrencyButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            var profileCurrency = _viewModel.ProfileCurrency;
            var currencies = _viewModel.AvailableCurrencies
                .Where(c => c != profileCurrency)
                .Prepend(profileCurrency)
                .ToList();

            var choice = await DisplayActionSheet(
                AppResources.NewTransaction_Currency,
                AppResources.Common_Cancel,
                destruction: null,
                currencies.Select(c => $"{c.Code} · {c.Symbol}").ToArray());

            var currency = currencies.FirstOrDefault(c => $"{c.Code} · {c.Symbol}" == choice);

            if (currency is not null)
            {
                _viewModel.SelectCurrency(currency);
            }
        });
    }

    private void ConvertedAmount_OnTapped(object? sender, TappedEventArgs e)
    {
        _viewModel.SwitchEditedAmount();
        AmountEntry.Focus();
    }

    private void AmountCard_OnTapped(object? sender, TappedEventArgs e)
    {
        AmountEntry.Focus();
    }

    private void CategoryChip_OnTapped(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is CategoryChipModel chip)
        {
            _viewModel.SelectCategory(chip);
        }
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
