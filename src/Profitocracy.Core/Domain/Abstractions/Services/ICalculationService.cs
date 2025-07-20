using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Domain.Model.Summaries;

namespace Profitocracy.Core.Domain.Abstractions.Services;

/// <summary>
/// Defines operations related to core calculations
/// </summary>
public interface ICalculationService
{
    /// <summary>
    /// Get current profile with calculated
    /// transactions for current period
    /// </summary>
    /// <returns>Current profile</returns>
    Task<Profile?> GetCurrentProfile();

    /// <summary>
    /// Calculates a summary for the specified date range.
    /// </summary>
    /// <param name="dateFrom">Start date of the period.</param>
    /// <param name="dateTo">End date of the period.</param>
    /// <returns>Summary containing transaction and category data for the period.</returns>
    Task<Summary> GetSummaryForPeriod(DateTime dateFrom, DateTime dateTo);
}