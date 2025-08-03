using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Categories;

/// <summary>
/// Represents a category in the domain model
/// that associates with a specific profile.
/// </summary>
public class Category : AggregateRoot<Guid>
{
    private decimal? _plannedAmount;

    internal Category(Guid id) : base(id)
    { }

    /// <summary>
    /// Represents the identifier associated with the profile linked to the category.
    /// </summary>
    public required Guid ProfileId { get; init; }

    /// <summary>
    /// Represents the name of the category.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Represents the planned monetary amount allocated or associated with a category.
    /// This property is optional and can be null, indicating no specific planned amount.
    /// The planned amount cannot be negative; attempts to set a negative value will result in
    /// an <see cref="ArgumentException" />.
    /// </summary>
    public decimal? PlannedAmount
    {
        get => _plannedAmount;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("Planned amount cannot be negative.");
            }

            _plannedAmount = value;
        }
    }
}
