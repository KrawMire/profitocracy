using Profitocracy.Core.Domain.Model.Categories;
using Profitocracy.Core.Persistence;

namespace Profitocracy.Core.Tests.Mocks.Persistence;

public class MockCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categories = [];

    public Task<List<Category>> GetAllByProfileId(Guid profileId)
    {
        return Task.FromResult(_categories.Where(c => c.ProfileId == profileId).ToList());
    }

    public Task<Category?> GetById(Guid categoryId)
    {
        return Task.FromResult(_categories.FirstOrDefault(c => c.Id == categoryId));
    }

    public Task<Category> Create(Category category)
    {
        _categories.Add(category);
        return Task.FromResult(category);
    }

    public Task<Category> Update(Category category)
    {
        var existing = _categories.FirstOrDefault(c => c.Id == category.Id);
        if (existing != null)
        {
            _categories.Remove(existing);
            _categories.Add(category);
        }
        return Task.FromResult(category);
    }

    public Task<Guid> Delete(Guid categoryId)
    {
        var category = _categories.FirstOrDefault(c => c.Id == categoryId);
        if (category != null)
        {
            _categories.Remove(category);
        }
        return Task.FromResult(categoryId);
    }

    public Task DeleteByProfileId(Guid profileId)
    {
        var category = _categories.FirstOrDefault(c => c.ProfileId == profileId);
        if (category != null)
        {
            _categories.Remove(category);
        }
        return Task.CompletedTask;
    }
}
