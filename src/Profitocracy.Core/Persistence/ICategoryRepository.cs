using Profitocracy.Core.Domain.Model.Categories;

namespace Profitocracy.Core.Persistence;

/// <summary>
/// Provides operations for working with
/// persistence layer for categories
/// </summary>
public interface ICategoryRepository
{
	/// <summary>
	/// Get all categories for profile with specified ID
	/// </summary>
	/// <param name="profileId">ID of profile which categories are related to</param>
	/// <returns>List of categories of profile</returns>
	Task<List<Category>> GetAllByProfileId(Guid profileId);

	/// <summary>
	/// Gets a category by its ID.
	/// </summary>
	/// <param name="categoryId">The ID of the category to retrieve.</param>
	/// <returns>The category with the specified ID, or null if not found.</returns>
	Task<Category?> GetById(Guid categoryId);

	/// <summary>
	/// Create new category
	/// </summary>
	/// <param name="category">Category to create</param>
	/// <returns>Created category</returns>
	Task<Category> Create(Category category);

	/// <summary>
	/// Update an existing category.
	/// </summary>
	/// <param name="category">The category to update.</param>
	/// <returns>The updated category.</returns>
	Task<Category> Update(Category category);

	/// <summary>
	/// Delete a category by its ID
	/// </summary>
	/// <param name="categoryId">Category ID to delete</param>
	/// <returns>ID of deleted category</returns>
	Task<Guid> Delete(Guid categoryId);

	/// <summary>
	/// Deletes all categories associated with the specified profile ID.
	/// </summary>
	/// <param name="profileId">The ID of the profile whose categories should be deleted.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task DeleteByProfileId(Guid profileId);
}