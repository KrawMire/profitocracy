using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Transactions.Entities;

public class TransactionCategory(Guid categoryId) : Entity<Guid>(categoryId)
{
    public required string Name { get; set; }
}