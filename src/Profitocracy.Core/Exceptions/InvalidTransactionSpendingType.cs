namespace Profitocracy.Core.Exceptions;

public class InvalidTransactionSpendingType : Exception
{
    public InvalidTransactionSpendingType(string message) : base(message) { }
}
