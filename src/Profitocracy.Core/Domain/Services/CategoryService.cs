using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Categories;
using Profitocracy.Core.Persistence;

namespace Profitocracy.Core.Domain.Services;

internal class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository)
    {
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Guid> DeleteCategory(Guid id)
    {
        await _transactionRepository.ClearWithCategory(id);
        return await _categoryRepository.Delete(id);
    }

    public async Task<Guid> UpdateCategory(Category category)
    {
        await _transactionRepository.ChangeCategoryName(category.Id, category.Name);
        var updatedCategody = await _categoryRepository.Update(category);

        return updatedCategody.Id;
    }
}
