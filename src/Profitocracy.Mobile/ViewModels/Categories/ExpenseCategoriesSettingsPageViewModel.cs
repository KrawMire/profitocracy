using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Categories;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Profitocracy.Mobile.ViewModels.Categories;

public class ExpenseCategoriesSettingsPageViewModel : BaseNotifyObject
{
    private readonly ICalculationService _calculationService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryService _categoryService;

    private bool _isCategoriesListEmpty;
    private string _subtitle = string.Empty;
    private string _limitsTotal = string.Empty;

    public ExpenseCategoriesSettingsPageViewModel(
        ICalculationService calculationService,
        ICategoryRepository categoryRepository,
        ICategoryService categoryService)
    {
        _calculationService = calculationService;
        _categoryRepository = categoryRepository;
        _categoryService = categoryService;
    }

    public ObservableCollection<CategorySettingsModel> Categories { get; } = [];

    public bool IsCategoriesListEmpty
    {
        get => _isCategoriesListEmpty;
        set
        {
            SetProperty(ref _isCategoriesListEmpty, value);
            OnPropertyChanged(nameof(HasCategories));
        }
    }

    public bool HasCategories => !IsCategoriesListEmpty;

    public string Subtitle
    {
        get => _subtitle;
        private set => SetProperty(ref _subtitle, value);
    }

    /// <summary>
    /// Sum of the limits and its share of the income, e.g. "€1,975 a month · 37% of income".
    /// </summary>
    public string LimitsTotal
    {
        get => _limitsTotal;
        private set => SetProperty(ref _limitsTotal, value);
    }

    public async Task Initialize()
    {
        var profile = await _calculationService.GetCurrentProfile();

        if (profile is null)
        {
            throw new Exception(AppResources.CommonError_GetCurrentProfile);
        }

        var symbol = profile.Settings.Currency.Symbol;
        var categories = await _categoryRepository.GetAllByProfileId(profile.Id);
        var spending = profile.CategoriesExpenses.ToDictionary(c => c.Id, c => c.ActualAmount);

        Subtitle = string.Format(AppResources.Categories_Subtitle, profile.Name);

        Categories.Clear();

        foreach (var category in categories)
        {
            var spent = spending.GetValueOrDefault(category.Id);
            var limit = category.PlannedAmount is > 0 ? category.PlannedAmount : null;
            var isOver = limit is not null && spent > limit;

            Categories.Add(new CategorySettingsModel
            {
                Id = category.Id,
                Name = category.Name,
                HasLimit = limit is not null,
                IsOver = isOver,
                Limit = limit is null
                    ? AppResources.Common_NoLimit
                    : NumberUtils.FormatMoneyCompact(limit.Value, symbol),
                LimitCaption = limit is null
                    ? AppResources.Categories_SetOne
                    : AppResources.Categories_AMonth,
                Spent = isOver
                    ? string.Format(
                        AppResources.Categories_OverSpent,
                        NumberUtils.FormatMoney(spent - limit!.Value, symbol),
                        NumberUtils.FormatMoney(spent, symbol))
                    : string.Format(AppResources.Categories_SpentThisMonth, NumberUtils.FormatMoney(spent, symbol)),
            });
        }

        IsCategoriesListEmpty = Categories.Count == 0;

        var limits = categories.Sum(c => c.PlannedAmount ?? 0);
        var income = profile.Expenses.TotalBalance.PlannedAmount;

        LimitsTotal = income > 0
            ? string.Format(
                AppResources.Categories_LimitsTotal,
                NumberUtils.FormatMoneyCompact(limits, symbol),
                (limits / income).ToString("P0", CultureInfo.CurrentCulture))
            : string.Format(
                AppResources.Categories_LimitsTotalNoIncome,
                NumberUtils.FormatMoneyCompact(limits, symbol));
    }

    public async Task DeleteCategory(Guid categoryId)
    {
        var deletedId = await _categoryService.DeleteCategory(categoryId);

        var category = Categories.SingleOrDefault(c => c.Id == deletedId);

        if (category is not null)
        {
            Categories.Remove(category);
        }

        IsCategoriesListEmpty = Categories.Count == 0;
    }
}
