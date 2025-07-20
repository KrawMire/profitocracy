namespace Profitocracy.Core.Domain.Model.Profiles.ValueObjects;

public struct AnchorDate
{
    public required DateTime Timestamp { get; set; }
    public required decimal InitialBalance { get; set; }
}