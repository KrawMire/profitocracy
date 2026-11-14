using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Transactions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Transactions;

namespace Profitocracy.Mobile.Views.Transactions.Pages;

public partial class TransactionsPage : BaseContentPage
{
    private readonly TransactionsPageViewModel _viewModel;
    private TransactionsFiltersPageViewModel _lastAppliedFiltersViewModel;
    private TransactionsFiltersPageViewModel _filtersViewModel;

    public TransactionsPage(
        TransactionsPageViewModel viewModel,
        TransactionsFiltersPageViewModel filtersViewModel)
    {
        InitializeComponent();

        _lastAppliedFiltersViewModel = _filtersViewModel = filtersViewModel;
        BindingContext = _viewModel = viewModel;
    }

    private void TransactionsPage_NavigatedTo(object? sender, EventArgs e)
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

    private void PreviousMonthButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(() => _viewModel.ShowPreviousMonth(_lastAppliedFiltersViewModel));
    }

    private void NextMonthButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(() => _viewModel.ShowNextMonth(_lastAppliedFiltersViewModel));
    }

    private void ResetFiltersButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            _filtersViewModel.Reset();
            _lastAppliedFiltersViewModel.Reset();

            await _viewModel.Initialize(_lastAppliedFiltersViewModel);
        });
    }

    private void AllChip_OnTapped(object? sender, EventArgs e) => _viewModel.Kind = ActivityKind.All;

    private void SpentChip_OnTapped(object? sender, EventArgs e) => _viewModel.Kind = ActivityKind.Spent;

    private void IncomeChip_OnTapped(object? sender, EventArgs e) => _viewModel.Kind = ActivityKind.Income;

    private void SavedChip_OnTapped(object? sender, EventArgs e) => _viewModel.Kind = ActivityKind.Saved;

    private void RecurringButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            var recurringPage = Handler?.MauiContext?.Services.GetService<RecurringTransactionsPage>();

            if (recurringPage is not null)
            {
                await Navigation.PushAsync(recurringPage);
            }
        });
    }

    private void TransactionRow_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            if ((sender as BindableObject)?.BindingContext is not TransactionModel { Id: not null } transaction)
            {
                throw new ArgumentNullException(AppResources.CommonError_FindTransactionToEdit);
            }

            await EditTransaction(transaction);
        });
    }

    private void DeleteTransactionSwipeItem_OnInvoked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            if (sender is not SwipeItemView swipeItem)
            {
                throw new InvalidCastException(AppResources.CommonError_InternalErrorTryAgain);
            }

            var transaction = swipeItem.BindingContext as TransactionModel;

            if (transaction?.Id is null)
            {
                throw new ArgumentNullException(AppResources.CommonError_FindTransactionToDelete);
            }

            var isDelete = await DisplayAlert(
                AppResources.Transactions_DeleteAlert_Title,
                string.Format(AppResources.Transactions_DeleteAlert_Description, transaction.Title),
                AppResources.Transactions_DeleteAlert_Ok,
                AppResources.Transactions_DeleteAlert_Cancel);

            if (isDelete)
            {
                await _viewModel.DeleteTransaction((Guid)transaction.Id);
            }
        });
    }

    private async Task EditTransaction(TransactionModel transaction)
    {
        var isTransactionInPeriod = await _viewModel.IsTransactionInProfilePeriod((Guid)transaction.Id!);

        if (!isTransactionInPeriod)
        {
            var isEdit = await DisplayAlert(
                AppResources.Transactions_EditNotInPeriodAlert_Title,
                string.Format(AppResources.Transactions_EditNotInPeriodAlert_Description, transaction.Title),
                AppResources.Transactions_EditNotInPeriodAlert_Ok,
                AppResources.Transactions_EditNotInPeriodAlert_Cancel);

            if (!isEdit)
            {
                return;
            }
        }

        var editPage = Handler?.MauiContext?.Services.GetService<EditTransactionPage>();

        if (editPage is null)
        {
            throw new ArgumentNullException(AppResources.CommonError_OpenEditTransactionPage);
        }

        editPage.AddTransactionId((Guid)transaction.Id!);
        await Navigation.PushModalAsync(editPage);
    }

    private void TransactionFilters_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            _filtersViewModel =
                Handler?.MauiContext?.Services.GetService<TransactionsFiltersPageViewModel>()
                ?? throw new ArgumentNullException(AppResources.CommonError_OpenTransactionsFiltersPage);

            _filtersViewModel.CopyFrom(_lastAppliedFiltersViewModel);
            var filtersPage = new TransactionsFiltersPage(_filtersViewModel);
            await Navigation.PushModalAsync(filtersPage);
        });
    }
}
