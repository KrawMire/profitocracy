using SQLite;

namespace Profitocracy.Infrastructure.Persistence.Sqlite.Models.Profile;

internal class ProfileModel
{
    [PrimaryKey]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public string Name { get; set; }

    public string CurrencyCode { get; set; }
#pragma warning restore CS8618

    public DateTime StartTimestamp { get; set; }
    public decimal InitialBalance { get; set; }
    public DateTime? BillingStartDate { get; set; }
    public DateTime? BillingEndDate { get; set; }
    public decimal Balance { get; set; }

    public bool IsCurrent { get; set; }

    [Ignore]
    public List<ProfileCategoryModel>? Categories { get; set; }
}