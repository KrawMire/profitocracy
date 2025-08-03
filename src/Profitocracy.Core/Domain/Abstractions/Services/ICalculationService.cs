using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Domain.Model.Summaries;

namespace Profitocracy.Core.Domain.Abstractions.Services;

/// <summary>
/// Defines operations related to core calculations
/// </summary>
public interface ICalculationService
{
    /// <summary>
    /// Get the current profile with calculated
    /// transactions for the current period.
    /// </summary>
    /// <returns>Current profile</returns>
    Task<Profile?> GetCurrentProfile();

    /// <summary>
    /// Populates the profile with transactions' data and processes calculations.
    /// </summary>
    /// <param name="profile">The profile to be populated and processed.</param>
    /// <param name="startDate">Start date of the profile calculations.</param>
    /// <param name="endDate">End date of the profile calculations.</param>
    /// <returns>The updated and processed profile.</returns>
    Task<Profile> PopulateAndProcessProfile(Profile profile, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Calculates a summary for the specified date range.
    /// </summary>
    /// <param name="dateFrom">Start date of the period.</param>
    /// <param name="dateTo">End date of the period.</param>
    /// <returns>Summary containing transaction and category data for the period.</returns>
    Task<Summary> GetSummaryForPeriod(DateTime dateFrom, DateTime dateTo);
}
