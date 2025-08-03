using Microsoft.Extensions.DependencyInjection;
using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Tests.Mocks.Factories;
using Profitocracy.Core.Tests.Mocks.Persistence;

namespace Profitocracy.Core.Tests.Domain.Services;

public class CategoryServiceTests
{
    private readonly ICategoryService _categoryService;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryServiceTests()
    {
        var profileRepository = new MockProfileRepository();
        _categoryRepository = new MockCategoryRepository();
        var settingsRepository = new MockSettingsRepository();
        var transactionRepository = new MockTransactionRepository();

        var provider = new ServiceCollection()
            .RegisterCoreServices()
            .AddSingleton<IProfileRepository>(profileRepository)
            .AddSingleton(_categoryRepository)
            .AddSingleton<ISettingsRepository>(settingsRepository)
            .AddSingleton<ITransactionRepository>(transactionRepository)
            .BuildServiceProvider();

        _categoryService = provider.GetRequiredService<ICategoryService>();
    }


    [Fact]
    public async Task DeleteCategory_ShouldDeleteCategory()
    {
        var category = MockEntityFactory.CreateMockCategory(Guid.NewGuid());

        await _categoryRepository.Create(category);
        var result = await _categoryService.DeleteCategory(category.Id);

        Assert.Equal(result, category.Id);
    }

    [Fact]
    public async Task UpdateCategory_ShouldUpdateCategory()
    {
        var category = MockEntityFactory.CreateMockCategory(Guid.NewGuid());

        await _categoryRepository.Create(category);

        var newCategory = MockEntityFactory.CreateMockCategory(category.Id, "NewCategory", 1000);
        var result = await _categoryService.UpdateCategory(newCategory);

        Assert.Equal(result, newCategory.Id);
    }
}
