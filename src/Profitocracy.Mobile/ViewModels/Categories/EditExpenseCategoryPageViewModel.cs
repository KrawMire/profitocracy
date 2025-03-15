using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Categories;
using Profitocracy.Core.Domain.Model.Categories.Factories;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;

namespace Profitocracy.Mobile.ViewModels.Categories;

public class EditExpenseCategoryPageViewModel : BaseNotifyObject
{
    private Guid? _categoryId;
    private string _categoryName = string.Empty;
    private bool _isPlannedAmountPresent;
    private string? _plannedAmountStr;

    private readonly ICategoryRepository _categoryRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly ICategoryService _categoryService;

    public EditExpenseCategoryPageViewModel(
        ICategoryRepository categoryRepository,
        IProfileRepository profileRepository, 
        ICategoryService categoryService)
    {
        _categoryRepository = categoryRepository;
        _profileRepository = profileRepository;
        _categoryService = categoryService;

        _categoryId = null;
        _isPlannedAmountPresent = true;
    }

    public Guid CategoryId
    {
        set => _categoryId = value;
    }

    public string Name
    {
        get => _categoryName;
        set
        {
            _categoryName = value;
            OnPropertyChanged();
        }
    }

    public bool IsPlannedAmountPresent
    {
        get => _isPlannedAmountPresent;
        set
        {
            if (!value)
            {
                _plannedAmountStr = null;
            }

            _isPlannedAmountPresent = value;
            OnPropertyChanged();
        }
    }

    public string PlannedAmount
    {
        get => _plannedAmountStr ?? string.Empty;
        set => SetProperty(ref _plannedAmountStr, value);
    }

    public async Task Initialize()
    {
        if (_categoryId is null)
        {
            return;
        }
        
        var category = await _categoryRepository.GetById((Guid)_categoryId);

        if (category is null)
        {
            throw new ArgumentNullException(AppResources.CommonError_FindCategoryToEdit);
        }
        
        Name = category.Name;
        IsPlannedAmountPresent = category.PlannedAmount is not null;
        PlannedAmount = category.PlannedAmount?.ToString() ?? string.Empty;
    }

    public Task SaveCategory()
    {
        return _categoryId is null
            ? CreateCategory()
            : UpdateCategory();
    }

    private async Task UpdateCategory()
    {
        var category = await BuildCategory(_categoryId);
        await _categoryService.UpdateCategory(category);
    }
    
    private async Task CreateCategory()
    {
        var category = await BuildCategory(null);
        await _categoryRepository.Create(category);
    }

    private async Task<Category> BuildCategory(Guid? categoryId)
    {
        decimal? plannedAmount = null;
        
        if (_isPlannedAmountPresent)
        {
            if (!NumberUtils.TryParseDecimal(_plannedAmountStr, out var decPlannedAmount))
            {
                throw new InvalidCastException(AppResources.CommonError_PlannedAmountNumber);   
            }
            
            plannedAmount = decPlannedAmount;
        }

        var profileId = await _profileRepository.GetCurrentProfileId();

        if (profileId is null)
        {
            throw new ArgumentNullException(AppResources.CommonError_GetCurrentProfile);
        }

        return CategoryFactory.CreateCategory(
            categoryId,
            (Guid)profileId,
            _categoryName,
            plannedAmount);
    }
}