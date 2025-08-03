namespace Profitocracy.Infrastructure.Persistence.Sqlite.Models.Profile;

public class ProfileCategoryModel
{
    public Guid CategoryId { get; set; }

#pragma warning disable CS8618
    public string Name { get; set; }
#pragma warning restore CS8618

    public decimal ActualAmount { get; set; }
    public decimal? PlannedAmount { get; set; }
}
