using SQLite;

namespace Profitocracy.Infrastructure.Persistence.Sqlite.Models.Category;

public class CategoryModel
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }

#pragma warning disable CS8618
    [NotNull]
    public string Name { get; set; }
#pragma warning restore CS8618

    public decimal? PlannedAmount { get; set; }
}