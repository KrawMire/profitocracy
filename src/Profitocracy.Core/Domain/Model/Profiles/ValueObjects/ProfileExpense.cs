namespace Profitocracy.Core.Domain.Model.Profiles.ValueObjects;

public class ProfileExpense
{
    public required decimal ActualAmount { get; set; }
    public required decimal PlannedAmount { get; set; }
}
