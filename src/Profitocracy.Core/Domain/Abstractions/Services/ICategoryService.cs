using Profitocracy.Core.Domain.Model.Categories;

namespace Profitocracy.Core.Domain.Abstractions.Services;

/// <summary>
/// Core business logic related
/// to complex categories management
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Deletes the category with the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the category to be deleted.</param>
    /// <returns>Returns the unique identifier of the deleted category.</returns>
    Task<Guid> DeleteCategory(Guid id);

    /// <summary>
    /// Updates the details of an existing category.
    /// </summary>
    /// <param name="category">The category object containing updated information.</param>
    /// <returns>Returns the unique identifier of the updated category.</returns>
    Task<Guid> UpdateCategory(Category category);
}
