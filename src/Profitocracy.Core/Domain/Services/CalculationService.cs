using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Domain.Model.Profiles.Entities;
using Profitocracy.Core.Domain.Model.Summaries;
using Profitocracy.Core.Domain.Model.Summaries.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Specifications;

namespace Profitocracy.Core.Domain.Services;

/// <summary>
/// This internal implementation of <see cref="ICalculationService"/> is used
/// because only it should know about <see cref="Profile"/> implementation
/// details and how to deal with them.
/// </summary>
internal class CalculationService : ICalculationService
{
    private readonly IProfileRepository _profileRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CalculationService(
        IProfileRepository profileRepository,
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository)
    {
        _profileRepository = profileRepository;
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    /// <inheritdoc />
    public async Task<Profile?> GetCurrentProfile()
    {
        var profile = await _profileRepository.GetCurrentProfile();

        if (profile is null)
        {
            return null;
        }

        return await PopulateAndProcessProfile(
            profile,
            profile.BillingPeriod.DateFrom,
            profile.BillingPeriod.DateTo);
    }

    /// <inheritdoc />
    public async Task<Profile> PopulateAndProcessProfile(Profile profile, DateTime startDate, DateTime endDate)
    {
        var savingTransactions = await _transactionRepository.GetFiltered(
            new TransactionsSpecification
            {
                ProfileId = profile.Id,
                Destination = TransactionDestination.SavingsBalance,
                ToDate = startDate,
                IsMultiCurrency = true,
            });

        var withdrawTransactions = await _transactionRepository.GetFiltered(
            new TransactionsSpecification
            {
                ProfileId = profile.Id,
                Destination = TransactionDestination.ProfileBalance,
                ToDate = startDate,
                IsMultiCurrency = true,
            });

        var transactions = await _transactionRepository.GetForPeriod(
            profile.Id,
            startDate,
            endDate);

        transactions = transactions
            .Concat(savingTransactions)
            .Concat(withdrawTransactions)
            .ToList();

        var categories = await _categoryRepository.GetAllByProfileId(profile.Id);

        if (categories.Count > 0)
        {
            var profileCategories = categories.Select(c => new ProfileCategory(c.Id)
            {
                Name = c.Name,
                ActualAmount = 0,
                PlannedAmount = c.PlannedAmount
            });

            profile.AddCategories(profileCategories);
        }

        var currentDate = DateTime.Now;

        profile.HandleTransactions(transactions, currentDate);

        if (!profile.IsNewPeriod)
        {
            return profile;
        }

        var updatedProfile = await _profileRepository.Update(profile);

        // Supposed to be executed a maximum of 2 times
        return await PopulateAndProcessProfile(
            updatedProfile,
            updatedProfile.BillingPeriod.DateFrom,
            updatedProfile.BillingPeriod.DateTo);
    }

    /// <inheritdoc />
    public async Task<Summary> GetSummaryForPeriod(DateTime dateFrom, DateTime dateTo)
    {
        var profile = await _profileRepository.GetCurrentProfile();

        if (profile is null)
        {
            throw new InvalidOperationException("Current profile was not found");
        }

        var dateDiff = dateTo - dateFrom;

        if (dateDiff.Days < 0)
        {
            throw new InvalidOperationException("End date must be greater than the start date");
        }

        var monthsDiff = dateDiff.Days / 30;
        var calcType = SummaryCalculationType.None;

        if (monthsDiff <= 3)
        {
            calcType |= SummaryCalculationType.IncludeDaily;
        }

        if (monthsDiff >= 2)
        {
            calcType |= SummaryCalculationType.IncludeWeekly;
        }

        var categories = await _categoryRepository.GetAllByProfileId(profile.Id);
        var transactions = await _transactionRepository.GetForPeriod(profile.Id, dateFrom, dateTo);
        var summary = new Summary(transactions, categories, calcType, dateFrom, dateTo);

        summary.CalculateSummary();

        return summary;
    }
}