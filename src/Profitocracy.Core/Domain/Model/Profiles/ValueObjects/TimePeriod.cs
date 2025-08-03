namespace Profitocracy.Core.Domain.Model.Profiles.ValueObjects;

public class TimePeriod
{
    public required DateTime DateFrom { get; set; }
    public required DateTime DateTo { get; set; }
}
