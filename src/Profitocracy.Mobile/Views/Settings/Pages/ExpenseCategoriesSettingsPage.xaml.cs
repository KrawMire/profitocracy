using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Categories;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Categories;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class ExpenseCategoriesSettingsPage : BaseContentPage
{
    private readonly ExpenseCategoriesSettingsPageViewModel _viewModel;

    public ExpenseCategoriesSettingsPage(ExpenseCategoriesSettingsPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }

    private void UpdateCategoriesList(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.Initialize();
        });
    }

    private void BackButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            await Navigation.PopAsync();
        });
    }

    private void AddCategoryButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await OpenEditCategoryPage(null);
        });
    }

    private void Category_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            if ((sender as BindableObject)?.BindingContext is not CategorySettingsModel category)
            {
                throw new ArgumentNullException(AppResources.CommonError_FindCategoryToEdit);
            }

            await OpenEditCategoryPage(category.Id);
        });
    }

    private void DeleteCategory_OnInvoked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            if ((sender as BindableObject)?.BindingContext is not CategorySettingsModel category)
            {
                throw new ArgumentNullException(AppResources.CommonError_FindCategoryToDelete);
            }

            var isDelete = await DisplayAlert(
                AppResources.CategoriesSettings_DeleteAlert_Title,
                string.Format(AppResources.CategoriesSettings_DeleteAlert_Description, category.Name),
                AppResources.CategoriesSettings_DeleteAlert_Ok,
                AppResources.CategoriesSettings_DeleteAlert_Cancel);

            if (isDelete)
            {
                await _viewModel.DeleteCategory(category.Id);
            }
        });
    }

    private async Task OpenEditCategoryPage(Guid? categoryId)
    {
        var addPage = Handler?.MauiContext?.Services.GetService<EditExpenseCategoryPage>();

        if (addPage is null)
        {
            throw new ArgumentNullException(AppResources.CommonError_OpenAddCategoryPage);
        }

        if (categoryId is not null)
        {
            addPage.AddCategoryId((Guid)categoryId);
        }

        await Navigation.PushModalAsync(addPage);
    }
}
