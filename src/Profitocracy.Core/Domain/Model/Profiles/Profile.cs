using Profitocracy.Core.Domain.Model.Profiles.Entities;
using Profitocracy.Core.Domain.Model.Profiles.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Profiles;

/// <summary>
/// Profile aggregate root.
/// Contains data about settings of profile, balances
/// state and methods for calculation of transactions
/// </summary>
public class Profile : AggregateRoot<Guid>
{
	public Profile(
		Guid id,
		string name,
		AnchorDate startDate,
		decimal savedBalance,
		List<ProfileCategory> categoriesBalances,
		ProfileSettings settings,
		bool isCurrent) : base(id)
	{
		Name = name;
		StartDate = startDate;
		Balance = StartDate.InitialBalance;
		SavedBalance = savedBalance;
		CategoriesBalances = categoriesBalances;
		Settings = settings;
		IsCurrent = isCurrent;
		Expenses = new ProfileExpenses
		{
			DailyFromActualBalance = new ProfileExpense
			{
				ActualAmount = 0,
				PlannedAmount = 0
			},
			DailyFromInitialBalance = new ProfileExpense
			{
				ActualAmount = 0,
				PlannedAmount = 0
			},
			Main = new ProfileExpense
			{
				ActualAmount = 0,
				PlannedAmount = 0
			},
			Secondary = new ProfileExpense
			{
				ActualAmount = 0,
				PlannedAmount = 0
			},
			Saved = new ProfileExpense
			{
				ActualAmount = 0,
				PlannedAmount = 0
			},
			TotalBalance = new ProfileExpense
			{
				ActualAmount = 0,
				PlannedAmount = 0
			}
		};
		
		var currentDate = DateTime.Now;

		BillingPeriod = new TimePeriod
		{
			DateFrom = new DateTime(currentDate.Year, currentDate.Month, 1),
			DateTo = new DateTime(
				currentDate.Year,
				currentDate.Month,
				day: DateTime.DaysInMonth(currentDate.Year, currentDate.Month))
		};
	}

	/// <summary>
	/// Does profile need to be updated and saved
	/// </summary>
	public bool NeedUpdate { get; private set; }
	
	/// <summary>
	/// Name of profile
	/// </summary>
	public string Name { get; }
	
	/// <summary>
	/// Initial balance from date when profile was created
	/// </summary>
	public AnchorDate StartDate { get; private set; }
	
	/// <summary>
	/// Start and end dates of billing period
	/// </summary>
	public TimePeriod BillingPeriod { get; }
	
	/// <summary>
	/// Current balance
	/// </summary>
	public decimal Balance { get; private set; }
	
	/// <summary>
	/// Total saved amount
	/// </summary>
	public decimal SavedBalance { get; private set; }
	
	/// <summary>
	/// Calculations of expenses by types: main, secondary and saved
	/// </summary>
	public ProfileExpenses Expenses { get; }
	
	/// <summary>
	/// Result of expenses calculations for every category
	/// </summary>
	public List<ProfileCategory> CategoriesBalances { get; }
	
	/// <summary>
	/// Profile settings
	/// </summary>
	public ProfileSettings Settings { get; }

	/// <summary>
	/// Is this profile currently using by user
	/// </summary>
	public bool IsCurrent { get; }

	/// <summary>
	/// Process transaction and
	/// project results in profile
	/// </summary>
	/// <param name="transactions">Transactions to handle</param>
	public void HandleTransactions(List<Transaction> transactions)
	{
		foreach (var transaction in transactions)
		{
			HandleTransaction(transaction);
		}

		var currentDate = DateTime.Now;
		
		if (StartDate.Timestamp.Month != currentDate.Month)
		{
			StartDate = new AnchorDate
			{
				InitialBalance = Balance, 
				Timestamp = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day)
			};
			
			NeedUpdate = true;
		}
		
		RecalculateExpenses();
	}
	
	/// <summary>
	/// Add profile categories to transaction processing
	/// </summary>
	/// <param name="categories">List of categories to add to profile</param>
	public void AddCategories(IEnumerable<ProfileCategory> categories)
	{
		CategoriesBalances.AddRange(categories);
	}
	
	/// <summary>
	/// Process transaction and
	/// project results in profile
	/// </summary>
	/// <param name="transaction">Transaction to handle</param>
	private void HandleTransaction(Transaction transaction)
	{
		if (transaction.Type == TransactionType.Income)
		{
			HandleIncomeTransaction(transaction);
		}
		else
		{
			HandleExpenseTransaction(transaction);
		}
	}
	
	/// <summary>
	/// Recalculate expenses
	/// </summary>
	private void RecalculateExpenses()
	{
		var currentDay = DateTime.Now;
		var daysInInitialPeriod = BillingPeriod.DateTo.Day - BillingPeriod.DateFrom.Day;
		var daysInActualPeriod = BillingPeriod.DateTo.Day - currentDay.Day;
		
		daysInInitialPeriod = daysInInitialPeriod == 0 ? 1 : daysInInitialPeriod;
		daysInActualPeriod = daysInActualPeriod == 0 ? 1 : daysInActualPeriod;

		Expenses.TotalBalance.PlannedAmount += StartDate.InitialBalance;
		Expenses.DailyFromActualBalance.PlannedAmount = Balance / daysInActualPeriod;
		Expenses.DailyFromInitialBalance.PlannedAmount = Expenses.TotalBalance.PlannedAmount / daysInInitialPeriod;
		Expenses.Main.PlannedAmount = Expenses.TotalBalance.PlannedAmount * 0.5m;
		Expenses.Secondary.PlannedAmount = Expenses.TotalBalance.PlannedAmount * 0.3m;
		Expenses.Saved.PlannedAmount = Expenses.TotalBalance.PlannedAmount * 0.2m;
	}
	
	private void HandleIncomeTransaction(Transaction transaction)
	{
		Balance += transaction.Amount;
		Expenses.TotalBalance.PlannedAmount += transaction.Amount;
	}

	private void HandleExpenseTransaction(Transaction transaction)
	{
		Balance -= transaction.Amount;

		if (transaction.Timestamp.Day == DateTime.Now.Day)
		{
			Expenses.DailyFromInitialBalance.ActualAmount += transaction.Amount;
			Expenses.DailyFromActualBalance.ActualAmount += transaction.Amount;
		}

		Expenses.TotalBalance.ActualAmount += transaction.Amount;
		
		switch (transaction.SpendingType)
		{
			case SpendingType.Main:
				HandleMainSpendingTransaction(transaction);
				break;
			case SpendingType.Secondary:
				HandleSecondarySpendingTransaction(transaction);
				break;
			case SpendingType.Saved:
				HandleSavingSpendingTransaction(transaction);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(transaction.SpendingType));
		}

		if (transaction.Category is not null)
		{
			HandleCategoryTransaction(transaction);
		}
	}

	private void HandleMainSpendingTransaction(Transaction transaction)
	{
		Expenses.Main.ActualAmount += transaction.Amount;
	}
	
	private void HandleSecondarySpendingTransaction(Transaction transaction)
	{
		Expenses.Secondary.ActualAmount += transaction.Amount;
	}
	
	private void HandleSavingSpendingTransaction(Transaction transaction)
	{
		SavedBalance += transaction.Amount;
		Expenses.Saved.ActualAmount = transaction.Amount;
	}

	private void HandleCategoryTransaction(Transaction transaction)
	{
		var category = CategoriesBalances.Find(c => c.Id.Equals(transaction.Category!.Id));

		if (category is null)
		{
			return;
		}

		category.ActualAmount += transaction.Amount;
	}
}