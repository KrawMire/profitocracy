namespace Profitocracy.Core.Domain.Model.Categories.Factories;

public class CategoryFactory
{
    public static Category CreateCategory(
        Guid? id,
        Guid profileId,
        string name,
        decimal? plannedAmount)
    {
        id ??= Guid.NewGuid();

        return new Category((Guid)id)
        {
            ProfileId = profileId,
            Name = name,
            PlannedAmount = plannedAmount
        };
    }
}
