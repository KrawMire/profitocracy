using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Specifications;

namespace Profitocracy.Core.Persistence;

/// <summary>
/// Provides operations for working with
/// persistence layer for transactions
/// </summary>
public interface ITransactionRepository
{
	/// <summary>
	/// Retrieves all transactions associated with a specific profile identifier.
	/// </summary>
	/// <param name="profileId">The identifier of the profile for which to retrieve transactions.</param>
	/// <returns>A list of transactions associated with the specified profile identifier.</returns>
	Task<List<Transaction>> GetAllByProfileId(Guid profileId);

	/// <summary>
	/// Retrieves a transaction by its unique identifier.
	/// </summary>
	/// <param name="transactionId">The identifier of the transaction to retrieve.</param>
	/// <returns>The transaction with the specified identifier, or null if not found.</returns>
	Task<Transaction?> GetById(Guid transactionId);

	/// <summary>
	/// Retrieves a list of transactions for a specific profile within a given date range.
	/// </summary>
	/// <param name="profileId">The identifier of the profile for which to retrieve transactions.</param>
	/// <param name="dateFrom">The start date of the period.</param>
	/// <param name="dateTo">The end date of the period.</param>
	/// <returns>A list of transactions for the specified profile within the given date range.</returns>
	Task<List<Transaction>> GetForPeriod(Guid profileId, DateTime dateFrom, DateTime dateTo);

	/// <summary>
	/// Retrieves a filtered list of transactions based on the provided specification.
	/// </summary>
	/// <param name="spec">The specification to filter transactions.</param>
	/// <returns>A list of transactions that match the specification.</returns>
	Task<List<Transaction>> GetFiltered(TransactionsSpecification spec);

	/// <summary>
	/// Creates a new transaction
	/// </summary>
	/// <param name="transaction">The transaction to create</param>
	/// <returns>The created transaction</returns>
	Task<Transaction> Create(Transaction transaction);

	/// <summary>
	/// Updates an existing transaction with new details.
	/// </summary>
	/// <param name="transaction">The transaction object containing updated information.</param>
	/// <returns>The updated transaction instance.</returns>
	Task<Transaction> Update(Transaction transaction);

	/// <summary>
	/// Clears categories references in transactions which reference a category with specified ID.
	/// </summary>
	/// <param name="categoryId">The identifier of the category.</param>
	/// <returns>The identifier of the category for which transactions were cleared.</returns>
	Task<Guid> ClearWithCategory(Guid categoryId);

	/// <summary>
	/// Updates the name of a specified category for all transactions.
	/// </summary>
	/// <param name="categoryId">The unique identifier of the category to be updated.</param>
	/// <param name="newName">The new name of category.</param>
	/// <returns>The unique identifier of the updated category.</returns>
	Task<Guid> ChangeCategoryName(Guid categoryId, string newName);

	/// <summary>
	/// Delete transaction by its ID
	/// </summary>
	/// <param name="transactionId">ID of transaction to delete</param>
	/// <returns>Deleted transaction ID</returns>
	Task<Guid> Delete(Guid transactionId);

	/// <summary>
	/// Deletes all transactions associated with a specific profile identifier.
	/// </summary>
	/// <param name="profileId">The identifier of the profile for which to delete transactions.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	Task DeleteByProfileId(Guid profileId);
}