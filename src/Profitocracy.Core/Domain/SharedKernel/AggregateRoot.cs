namespace Profitocracy.Core.Domain.SharedKernel;

public abstract class AggregateRoot<T> : Entity<T>
{
    protected AggregateRoot(T id) : base(id) { }
}
