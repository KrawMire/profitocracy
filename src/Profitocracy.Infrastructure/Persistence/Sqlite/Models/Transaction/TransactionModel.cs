using SQLite;

namespace Profitocracy.Infrastructure.Persistence.Sqlite.Models.Transaction;

/// <summary>
/// Persistence representation for
/// <see cref="Profitocracy.Core.Domain.Model.Transactions.Transaction"/> domain model
/// </summary>
public class TransactionModel
{
	[PrimaryKey]
	public Guid Id { get; set; }
	public decimal Amount { get; set; }
	public Guid ProfileId { get; set; }

#pragma warning disable CS8618
    public string SourceCurrencyCode { get; set; }
#pragma warning restore CS8618
	public short Type { get; set; }
	public short? SpendingType { get; set; }
	public DateTime Timestamp { get; set; }
	public string? Description { get; set; }
	public double? GeoTagLongitude { get; set; }
	public double? GeoTagLatitude { get; set; }
	public Guid? CategoryId { get; set; }
	public string? CategoryName { get; set; }
	public short? Destination { get; set; }
	public decimal? DestinationAmount { get; set; }
	public string? DestinationCurrencyCode { get; set; }
	public short? Interval { get; set; }
	public DateTime? LastMaturityDate { get; set; }
}
