using Profitocracy.Core.Domain.Model.Profiles.Entities;
using Profitocracy.Core.Domain.Model.Profiles.ValueObjects;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;

namespace Profitocracy.Core.Domain.Model.Profiles.Factories;

public class ProfileBuilder(Guid? profileId = null)
{
	private Guid _id = profileId ?? Guid.NewGuid();
	private decimal _balance;
	private string _name = "Default";
	private bool _isCurrent = true;
		
	private readonly List<ProfileCategory> _categories = [];
	
	private AnchorDate? _startDate;
	private ProfileSettings? _settings;

	public ProfileBuilder AddBalance(decimal balance)
	{
		_balance = balance;
		return this;
	}
	
	public ProfileBuilder AddName(string name)
	{
		_name = name;
		return this;
	}

	public ProfileBuilder AddStartDate(DateTime date, decimal balance)
	{
		_startDate = new AnchorDate
		{
			Timestamp = date,
			InitialBalance = balance
		};
		return this;
	}

	public ProfileBuilder AddCategoryExpense(Guid id, string name, decimal? plannedAmount = null)
	{
		_categories.Add(new ProfileCategory(id)
		{
			Name = name,
			ActualAmount = 0,
			PlannedAmount = plannedAmount
		});
		return this;
	}

	public ProfileBuilder AddCurrency(Currency currency)
	{
		_settings = new ProfileSettings
		{
			Currency = currency
		};
		
		return this;
	}

	public ProfileBuilder AddIsCurrent(bool isCurrent)
	{
		_isCurrent = isCurrent;
		return this;
	}

	public Profile Build()
	{
		if (_startDate is null)
		{
			AddStartDate(DateTime.Now, _balance);	
		}

		if (_settings is null)
		{
			AddCurrency(Currency.AvailableCurrencies.DefaultCurrency);
		}

		return new Profile(
			_id,
			_name,
			(AnchorDate)_startDate!,
			_categories,
			(ProfileSettings)_settings!,
			_isCurrent);
	}
}