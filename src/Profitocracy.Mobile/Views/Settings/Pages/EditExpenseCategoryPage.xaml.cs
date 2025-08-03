using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.ViewModels.Categories;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class EditExpenseCategoryPage : BaseContentPage
{
    private readonly EditExpenseCategoryPageViewModel _viewModel;

    public EditExpenseCategoryPage(EditExpenseCategoryPageViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public void AddCategoryId(Guid categoryId)
    {
        _viewModel.CategoryId = categoryId;
    }

    private void EditExpenseCategoryPage_OnLoaded(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.Initialize();
        });
    }

    private void CloseButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await Navigation.PopModalAsync();
        });
    }

    private void AddCategoryButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.SaveCategory();
            await Navigation.PopModalAsync();
        });
    }
}
