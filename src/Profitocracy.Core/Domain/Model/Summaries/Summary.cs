using Profitocracy.Core.Domain.Model.Categories;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Summaries.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Summaries;

public class Summary : AggregateRoot<Guid>
{
    private const int DaysInWeek = 7;
    
    private readonly ICollection<Transaction> _transactions;
    private readonly Dictionary<Guid, Category> _categories;
    private readonly int _daysInPeriod;
    private readonly SummaryCalculationType _calcType;
    private readonly Dictionary<DateTime, decimal> _expensesByDay;
    private readonly Dictionary<DateTime, decimal> _expensesByWeek;
    
    internal Summary(
        ICollection<Transaction> transactions,
        ICollection<Category> categories,
        SummaryCalculationType calcType,
        DateTime dateFrom,
        DateTime dateTo) : base(Guid.NewGuid())
    {
        if (dateFrom >= dateTo)
        {
            throw new ArgumentException("Date from must be less than date to.");
        }
        
        DateFrom = dateFrom;
        DateTo = dateTo;
        
        _transactions = transactions;
        
        var period = DateTo - DateFrom;

        _daysInPeriod = period.Days;
        _calcType = calcType;
        
        DailyAverage = 0;
        TotalIncome = 0;
        TotalExpenses = 0;
        
        _expensesByDay = new Dictionary<DateTime, decimal>();
        _expensesByWeek = new Dictionary<DateTime, decimal>();
        
        CategoryExpenseExpectations = new Dictionary<Guid, CategoryExpenseExpectation>();
        CategoryExpenses = new Dictionary<Guid, CategoryExpense>();
        SpendingTypesExpenses = new Dictionary<SpendingType, decimal>
        {
            { SpendingType.Main, 0 },
            { SpendingType.Secondary, 0 },
            { SpendingType.Saved, 0 }
        };
        
        _categories = new Dictionary<Guid, Category>();

        foreach (var category in categories)
        {
            _categories.Add(category.Id, category);
            CategoryExpenses.Add(category.Id, new CategoryExpense(category.Name));
            
            if (category.PlannedAmount is not null)
            {
                CategoryExpenseExpectations.Add(category.Id, new CategoryExpenseExpectation(category.Name, category.PlannedAmount.Value));   
            }
        }
    }

    public DateTime DateFrom { get; }
    public DateTime DateTo { get; }
    
    public decimal TotalIncome { get; private set; }
    public decimal TotalExpenses { get; private set; }
    public decimal DailyAverage { get; private set; }
    public Dictionary<Guid, CategoryExpenseExpectation> CategoryExpenseExpectations { get; }
    public Dictionary<Guid, CategoryExpense> CategoryExpenses { get; }
    public Dictionary<SpendingType, decimal> SpendingTypesExpenses { get; }
    
    public ICollection<WeeklyExpense>? WeeklyExpenses { get; set; }
    public ICollection<DailyExpense>? DailyExpenses { get; set; }
    
    public void CalculateSummary()
    {
        foreach (var transaction in _transactions)
        {
            HandleTransaction(transaction);
        }
        
        if (_calcType is SummaryCalculationType.ForMonth or SummaryCalculationType.ForThreeMonths)
        {
            CalculateDailyExpenses();
        }

        if (_calcType is SummaryCalculationType.ForThreeMonths or SummaryCalculationType.ForSixMonths)
        {
            CalculateWeeklyExpenses();
        }
    }

    private void HandleTransaction(Transaction transaction)
    {
        if (transaction.Type == TransactionType.Income)
        {
            TotalIncome += transaction.Amount;
            return;
        }
        
        if (transaction.Category is not null)
        {
            HandleCategoryTransaction(transaction);
        }
        
        HandleExpenseTransaction(transaction);
        DailyAverage = TotalExpenses / _daysInPeriod;
    }
    
    private void HandleCategoryTransaction(Transaction transaction)
    {
        CategoryExpenses[transaction.Category!.Id].Amount += transaction.Amount;

        if (_categories[transaction.Category.Id].PlannedAmount is not null)
        {
            CategoryExpenseExpectations[transaction.Category!.Id].ActualAmount += transaction.Amount;
        }
    }

    private void HandleExpenseTransaction(Transaction transaction)
    {
        TotalExpenses += transaction.Amount;

        var calcDailyExpenses = _calcType
            is SummaryCalculationType.ForMonth
            or SummaryCalculationType.ForThreeMonths;

        var calcWeeklyExpenses = _calcType
            is SummaryCalculationType.ForThreeMonths
            or SummaryCalculationType.ForSixMonths;
        
        if (calcDailyExpenses)
        {
            _expensesByDay.TryAdd(transaction.Timestamp.Date, 0);
            _expensesByDay[transaction.Timestamp.Date] += transaction.Amount;
        }
        
        if (calcWeeklyExpenses)
        {
            var transactionDayOfWeek = (int)transaction.Timestamp.DayOfWeek;
            var weekStart = transaction.Timestamp.AddDays(-transactionDayOfWeek);
            
            _expensesByWeek.TryAdd(weekStart.Date, 0);
            _expensesByWeek[weekStart.Date] += transaction.Amount;
        }
        
        if (transaction.SpendingType is null)
        {
            throw new ArgumentNullException(nameof(transaction), "Spending type of expense transaction is null");
        }

        SpendingTypesExpenses[transaction.SpendingType.Value] += transaction.Amount;
    }

    private void CalculateDailyExpenses()
    {
        DailyExpenses = new List<DailyExpense>();

        for (var i = 0; i < _daysInPeriod; i++)
        {
            var currentDay = DateFrom.AddDays(i).Date;
            DailyExpenses.Add(new DailyExpense
            {
                Date = currentDay,
                Amount = _expensesByDay.GetValueOrDefault(currentDay, 0)
            });
        }
    }

    private void CalculateWeeklyExpenses()
    {
        WeeklyExpenses = new List<WeeklyExpense>();

        for (var i = 0; i < _daysInPeriod; i++)
        {
            var currentDay = DateFrom.AddDays(i).Date;

            if (!_expensesByWeek.TryGetValue(currentDay, out var weekExpense))
            {
                continue;
            }
                
            var dayOfWeek = (int)currentDay.DayOfWeek;
            var weekStart = currentDay.AddDays(-dayOfWeek);
            var weekEnd = currentDay.AddDays(DaysInWeek - dayOfWeek);
                
            WeeklyExpenses.Add(new WeeklyExpense
            {
                DateFrom = weekStart,
                DateTo = weekEnd,
                Amount = weekExpense
            });
        }
    }
}