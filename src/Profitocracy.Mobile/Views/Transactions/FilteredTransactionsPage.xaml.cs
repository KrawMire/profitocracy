using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.ViewModels.Transactions;

namespace Profitocracy.Mobile.Views.Transactions;

public partial class FilteredTransactionsPage : BaseContentPage
{
    private readonly FilteredTransactionsPageViewModel _viewModel;
    
    public FilteredTransactionsPage(FilteredTransactionsPageViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        InitializeComponent();

        TransactionsCollectionView.ItemsSource = _viewModel.Transactions;
    }

    public async Task Initialize(
        Guid profileId, 
        Guid? categoryId, 
        SpendingType? spendingType, 
        DateTime dateFrom, 
        DateTime dateTo)
    {
        await _viewModel.Initialize(profileId, categoryId, spendingType, dateFrom, dateTo);
    }
    
    private void CloseButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await Navigation.PopModalAsync(); 
        });
    }
}