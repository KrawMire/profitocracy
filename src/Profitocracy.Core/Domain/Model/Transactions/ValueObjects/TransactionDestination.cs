namespace Profitocracy.Core.Domain.Model.Transactions.ValueObjects;

public enum TransactionDestination
{
    /// <summary>
    /// A multi currency transaction will be treated as moving funds
    /// from savings balance to a profile balance.
    /// </summary>
    ProfileBalance,

    /// <summary>
    /// A multi currency transaction will be treated as expense.
    /// </summary>
    Expense,

    /// <summary>
    /// A multi currency transaction will be treated as moving funds
    /// from profile balance to a savings balance.
    /// </summary>
    SavingsBalance
}