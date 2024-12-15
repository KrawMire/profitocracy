using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Domain.Model.Profiles.Entities;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Specifications;

namespace Profitocracy.Core.Domain.Services;

internal class ProfileService : IProfileService
{
	private readonly IProfileRepository _profileRepository;
	private readonly ITransactionRepository _transactionRepository;
	private readonly ICategoryRepository _categoryRepository;
	
	public ProfileService(
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
		
		return await PopulateAndProcessProfile(profile);
	}

	private async Task<Profile> PopulateAndProcessProfile(Profile profile)
	{
		var savingTransactions = await _transactionRepository.GetFiltered(
			new TransactionsSpecification
			{
				ProfileId = profile.Id,
				SpendingType = SpendingType.Saved,
				ToDate = profile.BillingPeriod.DateFrom
			});
		
		var transactions = await _transactionRepository.GetForPeriod(
			profile.Id, 
			profile.BillingPeriod.DateFrom,
			profile.BillingPeriod.DateTo);
		
		transactions = transactions
			.Concat(savingTransactions)
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
		return await PopulateAndProcessProfile(updatedProfile);

	} 
	
	/// <inheritdoc />
	public async Task<Profile?> GetCurrentProfileForPeriod(DateTime dateFrom, DateTime dateTo)
	{
		var profile = await _profileRepository.GetCurrentProfile();

		if (profile is null)
		{
			return null;
		}

		var transactions = await _transactionRepository.GetForPeriod(
			profile.Id,
			dateFrom,
			dateTo);

		throw new NotImplementedException();
	}
}