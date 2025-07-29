namespace Profitocracy.Core.Domain.Abstractions.Services;

/// <summary>
/// Represents a service to manage and
/// validate transactions within specific contexts
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Checks if the specified transaction is within the profile's current period.
    /// </summary>
    /// <param name="transactionId">The unique identifier of the transaction to check.</param>
    /// <returns>True if the transaction in the profile's current period, otherwise, false</returns>
    Task<bool> CheckTransactionInCurrentPeriod(Guid transactionId);
}
