using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Categories;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Home;
using Profitocracy.Mobile.Views.Settings.Pages;
using Profitocracy.Mobile.Views.Transactions.Pages;

namespace Profitocracy.Mobile.Views.Home.Pages;

public partial class HomePage : BaseContentPage
{
    private readonly HomePageViewModel _viewModel;

    public HomePage(HomePageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }

    private void HomePage_OnNavigated(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.Initialize();
        });
    }

    private void ProfileButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            await Shell.Current.GoToAsync("//" + nameof(SettingsPage));
        });
    }

    private void PeriodButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            var newPeriodPage = Handler?.MauiContext?.Services.GetService<NewPeriodSelectionPage>();

            if (newPeriodPage is null)
            {
                throw new Exception(AppResources.CommonError_OpenNewPeriodPage);
            }

            await Navigation.PushModalAsync(newPeriodPage);
        });
    }

    private void AddIncomeButton_OnTapped(object? sender, TappedEventArgs e)
    {
        (Shell.Current as AppShell)?.OpenNewTransactionPage(isIncome: true);
    }

    private void OverLimitBanner_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            if (_viewModel.OverLimitCategory is not null)
            {
                await OpenFilteredTransactionsByCategoryPage(_viewModel.OverLimitCategory);
            }
        });
    }

    private void EditLimitsButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            var categoriesPage = Handler?.MauiContext?.Services.GetService<ExpenseCategoriesSettingsPage>();

            if (categoriesPage is not null)
            {
                await Navigation.PushAsync(categoriesPage);
            }
        });
    }

    private void SeeAllButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            await Shell.Current.GoToAsync("//" + nameof(TransactionsPage));
        });
    }

    private void CategoryLayout_OnTapped(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            if ((sender as BindableObject)?.BindingContext is not CategoryExpenseModel category)
            {
                throw new Exception(AppResources.CommonError_GetCategoryInfo);
            }

            await OpenFilteredTransactionsByCategoryPage(category);
        });
    }

    private void MainSpendingTypeLayout_OnTapped(object? sender, EventArgs e)
    {
        SpendingTypeLayout_OnTapped(SpendingType.Main);
    }

    private void SecondarySpendingTypeLayout_OnTapped(object? sender, EventArgs e)
    {
        SpendingTypeLayout_OnTapped(SpendingType.Secondary);
    }

    private void SavedSpendingTypeLayout_OnTapped(object? sender, EventArgs e)
    {
        SpendingTypeLayout_OnTapped(SpendingType.Saved);
    }

    private void SpendingTypeLayout_OnTapped(SpendingType type)
    {
        ProcessAction(async () =>
        {
            await OpenFilteredTransactionsBySpendingTypePage(type);
        });
    }

    private async Task OpenFilteredTransactionsByCategoryPage(CategoryExpenseModel category)
    {
        var filteredPage = Handler?.MauiContext?.Services.GetService<FilteredTransactionsPage>();

        if (filteredPage is null)
        {
            throw new Exception(AppResources.CommonError_ShowFilteredTransactions);
        }

        await filteredPage.Initialize(
            _viewModel.ProfileId,
            category.Id,
            spendingType: null,
            dateFrom: _viewModel.DateFrom,
            dateTo: _viewModel.DateTo);

        await Navigation.PushModalAsync(filteredPage);
    }

    private async Task OpenFilteredTransactionsBySpendingTypePage(SpendingType type)
    {
        var filteredPage = Handler?.MauiContext?.Services.GetService<FilteredTransactionsPage>();

        if (filteredPage is null)
        {
            throw new Exception(AppResources.CommonError_ShowFilteredTransactions);
        }

        await filteredPage.Initialize(
            _viewModel.ProfileId,
            categoryId: null,
            type,
            dateFrom: _viewModel.DateFrom,
            dateTo: _viewModel.DateTo);

        await Navigation.PushModalAsync(filteredPage);
    }
}
