using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Transactions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Transactions;

namespace Profitocracy.Mobile.Views.Transactions;

public partial class TransactionsPage : BaseContentPage
{
	private readonly TransactionsPageViewModel _viewModel;
	
	public TransactionsPage(TransactionsPageViewModel viewModel)
	{
		InitializeComponent();
		
		BindingContext = _viewModel = viewModel;
		TransactionsCollectionView.ItemsSource = _viewModel.Transactions;
	}

	private void TransactionsPage_NavigatedTo(object? sender, EventArgs e)
	{
		ProcessAction(async () =>
		{
			await _viewModel.Initialize();
		});
	}

	private void AddTransactionButton_OnClicked(object? sender, EventArgs e)
	{
		ProcessAction(async () =>
		{
			var editPage = Handler?.MauiContext?.Services.GetService<EditTransactionPage>();

			if (editPage is null)
			{
				throw new ArgumentNullException(AppResources.CommonError_OpenEditTransactionPage);
			}
		
			await Navigation.PushModalAsync(editPage);
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
				string.Format(AppResources.Transactions_DeleteAlert_Description, transaction.Description),
				AppResources.Transactions_DeleteAlert_Ok,
				AppResources.Transactions_DeleteAlert_Cancel);

			if (isDelete)
			{
				await _viewModel.DeleteTransaction((Guid)transaction.Id);	
			}
		});
	}

	private void EditTransactionSwipeItem_OnInvoked(object? sender, EventArgs e)
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
				throw new ArgumentNullException(AppResources.CommonError_FindTransactionToEdit);
			}

			var isTransactionInPeriod = await _viewModel.IsTransactionInProfilePeriod((Guid)transaction.Id);
			var isEdit = true;
			
			if (!isTransactionInPeriod)
			{
				isEdit = await DisplayAlert(
					AppResources.Transactions_EditNotInPeriodAlert_Title,
					string.Format(AppResources.Transactions_EditNotInPeriodAlert_Description, transaction.Description),
					AppResources.Transactions_EditNotInPeriodAlert_Ok,
					AppResources.Transactions_EditNotInPeriodAlert_Cancel);
			}

			if (!isEdit)
			{
				return;
			}
			
			var editPage = Handler?.MauiContext?.Services.GetService<EditTransactionPage>();

			if (editPage is null)
			{
				throw new ArgumentNullException(AppResources.CommonError_OpenEditTransactionPage);
			}
		
			editPage.AddTransactionId((Guid)transaction.Id);
			await Navigation.PushModalAsync(editPage);
		});
	}
}