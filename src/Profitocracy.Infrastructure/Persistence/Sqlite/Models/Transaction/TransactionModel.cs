using SQLite;

namespace Profitocracy.Infrastructure.Persistence.Sqlite.Models.Transaction;

/// <summary>
/// Persistence representation for
/// <see cref="Profitocracy.Core.Domain.Model.Transactions.Transaction"/> domain model 
/// </summary>
internal class TransactionModel
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public Guid ProfileId { get; set; }
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
    public string? SourceCurrencyCode { get; set; }
    public string? DestinationCurrencyCode { get; set; }
}